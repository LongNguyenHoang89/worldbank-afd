using System.Windows;
using System;
using System.Windows.Controls;
using NCRVisual.web.DataModel;
using WorldbankDataGraphs.Entities;
using System.Collections.Generic;
using System.ServiceModel.DomainServices.Client;
using NCRVisual.web.Services;
using System.Linq;
using WorldbankDataGraphs;
using WorldMap.Helper;
using ImageTools.Helpers;
using ImageTools;
using ImageTools.IO;
using ImageTools.IO.Png;
using ImageTools.IO.Jpeg;
using ImageTools.IO.Bmp;
using System.IO;
using System.Windows.Media.Imaging;
using WorldMap.PredictionService;
using System.Collections.ObjectModel;

namespace WorldMap
{
    /// <summary>
    /// Child window show data about a country
    /// </summary>
    public partial class CompareCountriesChildWindow
    {
        Controller _worldMapController;
        private WorldbankDataGraphs.WorldbankGeneralChartControl columnChartControl = null;
        private List<tbl_countries> _selectedCountries;
        private List<int> _checkedIndicatorPKs;
        private LoadOperation<tbl_indicators> tblIndLoadOp = null;
        private LoadOperation<ref_country_indicator> loadOp = null;

        public event EventHandler RefreshControl;

        /// <summary>
        /// Default constructor
        /// </summary>
        public CompareCountriesChildWindow()
        {
            InitializeComponent();
            this.RefeshButton.Click += new RoutedEventHandler(RefeshButton_Click);
        }

        /// <summary>
        /// populate All data
        /// </summary>
        /// <param name="worldMapController"></param>
        /// <param name="country"></param>
        public void PopulateData(Controller worldMapController, List<tbl_countries> selectedCountries, List<int> checkedIndicatorPKs)
        {            
            this._worldMapController = worldMapController;
            this._selectedCountries = selectedCountries;
            this._checkedIndicatorPKs = checkedIndicatorPKs;
            // pupulate the combobox
            comboBoxRenderStyle.Items.Clear();
            comboBoxRenderStyle.Items.Add(WorldbankGeneralChartControl.RA_LINE_DESC);
            comboBoxRenderStyle.Items.Add(WorldbankGeneralChartControl.RA_COLUMN_DESC);
            comboBoxRenderStyle.Items.Add("3D " + WorldbankGeneralChartControl.RA_COLUMN_DESC);
            comboBoxRenderStyle.Items.Add(WorldbankGeneralChartControl.RA_BAR_DESC);
            comboBoxRenderStyle.Items.Add("3D " + WorldbankGeneralChartControl.RA_BAR_DESC);
            comboBoxRenderStyle.Items.Add(WorldbankGeneralChartControl.RA_AREA_DESC);
            // select the first choice of the combobox
            comboBoxRenderStyle.SelectedIndex = 0;
            // query the indicators from DB
            getIndicatorFromPK(_checkedIndicatorPKs);
        }

        private void getIndicatorFromPK(List<int> indPKs)
        {
            // get the indicator
            EntityQuery<tbl_indicators> tblIndQuery =
                from ind in _worldMapController.Context.GetTbl_indicatorsInPKListQuery(indPKs)
                select ind;
            tblIndLoadOp = _worldMapController.Context.Load(tblIndQuery);
            tblIndLoadOp.Completed += new EventHandler(tblIndLoadOp_Completed);
        }

        private void tblIndLoadOp_Completed(object sender, EventArgs e)
        {            
            List<tbl_indicators> selectedInd = new List<tbl_indicators>(tblIndLoadOp.Entities);
            comboBoxIndicatorSelector.DisplayMemberPath = "indicator_name";
            comboBoxIndicatorSelector.ItemsSource = selectedInd;
            
            // select the first indicator
            if (selectedInd.Count > 0)
            {
                comboBoxIndicatorSelector.SelectedIndex = 0;
            }
            // load the graph
            button1_Click(this, new RoutedEventArgs());
        }

        #region funcs to load data needed to show the graph
        private void GetDataForGraph(List<tbl_countries> selectedCountries, int checkedIndicator)
        {
            List<int> selectedCountryIds = new List<int>();
            foreach (tbl_countries tmpTC in selectedCountries)
            {
                selectedCountryIds.Add(tmpTC.country_id_pk);
            }
            EntityQuery<ref_country_indicator> query =
                from c in _worldMapController.Context.GetRef_country_indicatorInCountryIdListQuery(selectedCountryIds)
                // == tmpTC.country_id_pk
                where (/*selectedCountryIds.Contains((int)c.country_id) && */ c.indicator_id == checkedIndicator)
                select c;
            loadOp = _worldMapController.Context.Load(query);
            loadOp.Completed += new EventHandler(loadOp_Completed);
        }

        private void loadOp_Completed(object sender, EventArgs e)
        {
            // convert the entities to a list
            List<ref_country_indicator> refCountryIndic = new List<ref_country_indicator>(loadOp.Entities);;

            // draw the graph
            drawGraph(refCountryIndic);

            // re-enable the render button
            button1.IsEnabled = true;
            buttonShowPrediction.IsEnabled = true;
        }
        #endregion

        // this function draw the graph
        private void drawGraph(List<ref_country_indicator> refCountryIndic)
        {
            List<Country> finalResult = new List<Country>();
            string key = refCountryIndic[0].indicator_id.ToString();
            foreach (tbl_countries tmpTCountry in _selectedCountries)
            {
                Country tmpC = new Country();
                tmpC.Name = tmpTCountry.country_name;
                // get the indicators related to this country
                List<ref_country_indicator> tmpThisCountryIndicators = new List<ref_country_indicator>(
                    from n in refCountryIndic
                    where n.country_id == tmpTCountry.country_id_pk
                    select n);
                // then generate the data needed for the graph control
                foreach (ref_country_indicator tmpRCI in tmpThisCountryIndicators)
                {
                    YearData tmpYD = new YearData();
                    tmpYD.Year = (int)tmpRCI.country_indicator_year;
                    tmpYD.Attributes.Add(key, tmpRCI.country_indicator_value.ToString());
                    tmpC.Years.Add(tmpYD);
                }
                finalResult.Add(tmpC);
            }
            this.columnChartControl.AttributeShownName = key;
            this.columnChartControl.CountriesShown = finalResult;
        }

        private void drawGraph(List<refCountryIndicator> refCountryIndicator)
        {
            List<Country> finalResult = new List<Country>();
            string key = refCountryIndicator[0].indicator_id.ToString();
            foreach (tbl_countries tmpTCountry in _selectedCountries)
            {
                Country tmpC = new Country();
                tmpC.Name = tmpTCountry.country_name;
                // get the indicators related to this country
                List<refCountryIndicator> tmpThisCountryIndicators = new List<refCountryIndicator>(
                    from n in refCountryIndicator
                    where n.country_id == tmpTCountry.country_id_pk
                    select n);
                // then generate the data needed for the graph control
                foreach (refCountryIndicator tmpRCI in tmpThisCountryIndicators)
                {
                    YearData tmpYD = new YearData();
                    tmpYD.Year = (int)tmpRCI.country_indicator_year;
                    tmpYD.Attributes.Add(key, tmpRCI.country_indicator_value.ToString());
                    tmpC.Years.Add(tmpYD);
                }
                finalResult.Add(tmpC);
            }
            this.columnChartControl.AttributeShownName = key;
            this.columnChartControl.CountriesShown = finalResult;
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            WorldbankGeneralChartControl control = new WorldbankDataGraphs.WorldbankGeneralChartControl();
            this.columnChartControl = control;
            //RefeshButton_Click(sender, e);
            if (comboBoxRenderStyle.SelectedItem != null && comboBoxIndicatorSelector.SelectedItem != null)
            {
                refreshControlRenderStyle();
                // disable the combobox
                this.button1.IsEnabled = false;
                this.buttonShowPrediction.IsEnabled = false;
                // get the data needed to show the graph
                GetDataForGraph(_selectedCountries, ((tbl_indicators)comboBoxIndicatorSelector.SelectedItem).indicator_id_pk);
                // remove all other graphs from gridChart
                gridChart.Children.Clear();
                // add the chart to the gridChart
                this.gridChart.Children.Add(this.columnChartControl);
            }
            else
            {
                ErrorNotification errorNoti = new ErrorNotification("You must select a graph style and an indicator");
            }
        }

        private void refreshControlRenderStyle()
        {
            // select render style for the graph
            if (comboBoxRenderStyle.SelectedItem != null)
            {
                if (((string)comboBoxRenderStyle.SelectedItem).Equals(WorldbankGeneralChartControl.RA_AREA_DESC))
                {
                    this.columnChartControl.ThisChartRenderAs = WorldbankGeneralChartControl.RA_AREA;
                }
                else if (((string)comboBoxRenderStyle.SelectedItem).Equals(WorldbankGeneralChartControl.RA_BAR_DESC))
                {
                    this.columnChartControl.ThisChartRenderAs = WorldbankGeneralChartControl.RA_BAR;
                }
                else if (((string)comboBoxRenderStyle.SelectedItem).Equals(WorldbankGeneralChartControl.RA_COLUMN_DESC))
                {
                    this.columnChartControl.ThisChartRenderAs = WorldbankGeneralChartControl.RA_COLUMN;
                }
                else if (((string)comboBoxRenderStyle.SelectedItem).Equals(WorldbankGeneralChartControl.RA_LINE_DESC))
                {
                    this.columnChartControl.ThisChartRenderAs = WorldbankGeneralChartControl.RA_LINE;
                }
                else if (((string)comboBoxRenderStyle.SelectedItem).Equals("3D " + WorldbankGeneralChartControl.RA_COLUMN_DESC))
                {
                    this.columnChartControl.RenderAs3D = true;
                    this.columnChartControl.ThisChartRenderAs = WorldbankGeneralChartControl.RA_COLUMN;
                }
                else if (((string)comboBoxRenderStyle.SelectedItem).Equals("3D " + WorldbankGeneralChartControl.RA_BAR_DESC))
                {
                    this.columnChartControl.RenderAs3D = true;
                    this.columnChartControl.ThisChartRenderAs = WorldbankGeneralChartControl.RA_BAR;
                }
            }
        }

        private void RefeshButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.RefreshControl != null)
            {
                RefreshControl(sender, null);
            }
        }

        private void buttonExportGraph_Click(object sender, RoutedEventArgs e)
        {
            // prompt for a location to save the image
            try
            {
                SaveFileDialog dialog = new SaveFileDialog();
                dialog.DefaultExt = ".jpg";
                dialog.Filter = "JPEG image|*.jpg|PNG image|*.png|BMP image|*.bmp";
                if (dialog.ShowDialog() == true)
                {
                    WriteableBitmap bitmap = new WriteableBitmap(columnChartControl, null);
                    // the "using" block ensures the stream is cleared upon completed
                    using (Stream stream = dialog.OpenFile())
                    {
                        // encode the stream
                        //JPGUtil.EncodeJpg(bitmap, stream);
                        ImageTools.IO.Encoders.AddEncoder<BmpEncoder>();
                        ImageTools.IO.Encoders.AddEncoder<PngEncoder>();
                        ImageTools.IO.Encoders.AddEncoder<JpegEncoder>();
                        ExtendedImage image = bitmap.ToImage();
                        image.WriteToStream(stream, dialog.SafeFileName);
                    }
                }
            }
            catch (Exception ex)
            {
                //this.tblError.Text = "Error configuring SaveFileDialog: " + ex.Message;
            }
        }

        #region prediction functions

        private List<int> countriesToPredict = null;
        private int indicatorToPredictID = -1;
        private LoadOperation predictionLO;
        private List<refCountryIndicator> listPredictedResult;

        private void buttonShowPrediction_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedCountries != null &&
                comboBoxIndicatorSelector.SelectedItem != null &&
                _selectedCountries.Count > 1)
            {
                // get countries to predict
                countriesToPredict = new List<int>(3); // 3 is the common countries to compare, maximize utilization of memory
                foreach (tbl_countries c in _selectedCountries)
                {
                    countriesToPredict.Add(c.country_id_pk);
                }

                // get selected indicator to predict
                this.indicatorToPredictID = ((tbl_indicators)comboBoxIndicatorSelector.SelectedItem).indicator_id_pk;

                // init the graph if not already initiated
                WorldbankGeneralChartControl control = new WorldbankDataGraphs.WorldbankGeneralChartControl();
                this.columnChartControl = control;

                // remove all other graphs from gridChart
                gridChart.Children.Clear();

                // add the chart to the gridChart
                this.gridChart.Children.Add(this.columnChartControl);

                if (comboBoxRenderStyle.SelectedItem != null)
                {
                    refreshControlRenderStyle();
                    // disable the render button and the prediction button
                    this.button1.IsEnabled = false;
                    this.buttonShowPrediction.IsEnabled = false;

                    int nextCountryID = countriesToPredict[0];
                    countriesToPredict.RemoveAt(0);

                    predictionLO = this._worldMapController.Context.Load(this._worldMapController.Context.GetRef_country_indicatorListFromCountryIDAndIndicatorIDQuery(nextCountryID, indicatorToPredictID));
                    listPredictedResult = new List<refCountryIndicator>();
                    predictionLO.Completed -= new EventHandler(predictionLO_Completed);
                    predictionLO.Completed += new EventHandler(predictionLO_Completed);
                }
                else
                {
                    ErrorNotification errorNoti = new ErrorNotification("You must select a graph style");
                }
            }
        }

        private void predictionLO_Completed(object sender, EventArgs e)
        {
            List<ref_country_indicator> list = new List<ref_country_indicator>();

            foreach (ref_country_indicator rci in predictionLO.Entities)
            {
                list.Add(rci);
            }

            ObservableCollection<refCountryIndicator> tmpCol = convertRCI(list);

            PredictionServiceClient predictionServiceClient = new PredictionServiceClient();
            predictionServiceClient.PredictDataNextYearsCompleted -= new EventHandler<PredictDataNextYearsCompletedEventArgs>(predictionServiceClient_PredictDataNextYearsCompleted);
            predictionServiceClient.PredictDataNextYearsCompleted += new EventHandler<PredictDataNextYearsCompletedEventArgs>(predictionServiceClient_PredictDataNextYearsCompleted);
            predictionServiceClient.PredictDataNextYearsAsync(tmpCol, 3);

        }

        private void predictionServiceClient_PredictDataNextYearsCompleted(object sender, PredictDataNextYearsCompletedEventArgs e)
        {
            foreach (refCountryIndicator refCI in e.Result)
            {
                listPredictedResult.Add(refCI);
            }

            if (countriesToPredict.Count > 0)
            {
                int nextCountryID = countriesToPredict[0];
                countriesToPredict.RemoveAt(0);
                predictionLO = this._worldMapController.Context.Load(this._worldMapController.Context.GetRef_country_indicatorListFromCountryIDAndIndicatorIDQuery(nextCountryID, indicatorToPredictID));
                predictionLO.Completed -= new EventHandler(predictionLO_Completed);
                predictionLO.Completed += new EventHandler(predictionLO_Completed);
            }
            else
            {
                drawGraph(listPredictedResult);
                // enable buttons
                this.button1.IsEnabled = true;
                this.buttonShowPrediction.IsEnabled = true;
            }
        }

        private ObservableCollection<refCountryIndicator> convertRCI(List<ref_country_indicator> inputList)
        {
            ObservableCollection<refCountryIndicator> resultList = new ObservableCollection<refCountryIndicator>();
            foreach (ref_country_indicator i in inputList)
            {
                refCountryIndicator tmpI = new refCountryIndicator();
                tmpI.country_id = (int)i.country_id;
                tmpI.country_indicator_value = (float)i.country_indicator_value;
                tmpI.country_indicator_year = (int)i.country_indicator_year;
                tmpI.indicator_id = (int)i.indicator_id;
                resultList.Add(tmpI);
            }
            return resultList;
        }

        #endregion
    }
}

