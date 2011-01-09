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

namespace WorldMap
{
    /// <summary>
    /// Child window show data about a country
    /// </summary>
    public partial class CustomChildWindow
    {
        Controller _worldMapController;
        private WorldbankDataGraphs.WorldbankGeneralChartControl columnChartControl = null;
        private tbl_countries _selectedCountry;
        private LoadOperation<tbl_indicators> tblIndLoadOp = null;
        private LoadOperation<tbl_indicators> tblIndsLoadOp = null;
        private LoadOperation<ref_country_indicator> loadOp = null;
        private List<int> listboxIndicatorPKSelected = new List<int>();
        private List<tbl_indicators> listIndicatorSelectedFromWM;
        private List<tbl_indicators> shortListIndicatorsSelected;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="worldMapController"></param>
        /// <param name="country"></param>
        public CustomChildWindow(Controller worldMapController, tbl_countries selectedCountry, List<int> checkedIndicatorPKs)
        {
            InitializeComponent();
            // set some var with input params
            this._worldMapController = worldMapController;
            this._selectedCountry = selectedCountry;
            // generate the list of shortlist indicators
            getIndicatorsFromPKs(checkedIndicatorPKs);
            // default is all selected
            listboxIndicatorPKSelected = checkedIndicatorPKs;
            this.CountryNameTextBlock.Text = _selectedCountry.country_name;
            // gen combobox's items
            comboBoxRenderStyle.Items.Add(WorldbankGeneralChartControl.RA_LINE_DESC);
            comboBoxRenderStyle.Items.Add(WorldbankGeneralChartControl.RA_COLUMN_DESC);
            comboBoxRenderStyle.Items.Add("3D " + WorldbankGeneralChartControl.RA_COLUMN_DESC);
            comboBoxRenderStyle.Items.Add(WorldbankGeneralChartControl.RA_BAR_DESC);
            comboBoxRenderStyle.Items.Add("3D " + WorldbankGeneralChartControl.RA_BAR_DESC);
            comboBoxRenderStyle.Items.Add(WorldbankGeneralChartControl.RA_AREA_DESC);
            // select the first choice of the combobox
            comboBoxRenderStyle.SelectedIndex = 0;

            worldMapController.GetTabCountryData(selectedCountry.country_id_pk);
            worldMapController.GetTabCountryDataCompleted += new EventHandler(worldMapController_GetTabCountryDataCompleted);
        }

        void worldMapController_GetTabCountryDataCompleted(object sender, EventArgs e)
        {
            foreach (View_GeneralCountry v in _worldMapController.Context.View_GeneralCountries)
            {
                RegionNameTextBlock.Text = v.region_name;
                IncomeLevelTextBLock.Text = v.income_level_name;
                LendingTypeTextBlock.Text = v.lending_type_name;               
            }
        }

        private void getIndicatorsFromPKs(List<int> indPKs)
        {
            // get the indicator
            EntityQuery<tbl_indicators> tblIndQuery =
                from ind in _worldMapController.Context.GetTbl_indicatorsInPKListQuery(indPKs)
                select ind;
            tblIndsLoadOp = _worldMapController.Context.Load(tblIndQuery);
            tblIndsLoadOp.Completed += new EventHandler(tblIndsLoadOp_Completed);
        }

        private void tblIndsLoadOp_Completed(object sender, EventArgs e)
        {
            listIndicatorSelectedFromWM = new List<tbl_indicators>(tblIndsLoadOp.Entities);
            IndicatorListBox.ItemsSource = listIndicatorSelectedFromWM;
            // enable the render button
            buttonRenderChart.IsEnabled = true;
            buttonRenderChart_Click(this, new RoutedEventArgs());
        }

        private List<tbl_indicators> getIndicatorFromPKForGraph(List<int> indPKs)
        {
            List<tbl_indicators> returnList = new List<tbl_indicators>(
                from tmp in listIndicatorSelectedFromWM
                where indPKs.Contains(tmp.indicator_id_pk)
                select tmp
                );
            return returnList;
            /* Obsolete code
            // get the indicator
            EntityQuery<tbl_indicators> tblIndQuery =
                from ind in _worldMapController.Context.GetTbl_indicatorsInPKListQuery(indPKs)
                select ind;
            tblIndLoadOp = _worldMapController.Context.Load(tblIndQuery);
            tblIndLoadOp.Completed += new EventHandler(tblIndLoadOp_Completed);
             */
        }

        //private void tblIndLoadOp_Completed(object sender, EventArgs e)
        //{
        //    List<tbl_indicators> selectedInd = new List<tbl_indicators>(tblIndLoadOp.Entities);
        //    //nqk
        //    if (selectedInd.Count > 0)
        //    {
        //        this.columnChartControl.ChartTitle = selectedInd[0].indicator_unit;
        //        this.tabItem2.Header = selectedInd[0].indicator_name;
        //    }
        //}

        private void GetDataForGraph(tbl_countries selectedCountry, List<tbl_indicators> checkedIndicator)
        {
            // create a list of indicator pks
            List<int> tmpIndPKs = new List<int>();
            foreach (tbl_indicators tmpTI in checkedIndicator)
            {
                tmpIndPKs.Add(tmpTI.indicator_id_pk);
            }

            EntityQuery<ref_country_indicator> query =
                from c in _worldMapController.Context.GetRef_country_indicatorInIndicatorIDListQuery(tmpIndPKs)
                // == tmpTC.country_id_pk
                where (c.country_id == selectedCountry.country_id_pk)
                select c;


            loadOp = _worldMapController.Context.Load(query);
            //loadOp = _worldMapController.Context.Load(_worldMapController.Context.GetRef_country_indicatorInCountryIdListQuery(tmpIndPKs));
            //loadOp = _worldMapController.Context.Load(_worldMapController.Context.GetRef_country_indicatorQuery(tmpIndPKs));
            
            loadOp.Completed += new EventHandler(loadOp_Completed);
        }

        private void loadOp_Completed(object sender, EventArgs e)
        {
            List<ref_country_indicator> refCountryIndic = new List<ref_country_indicator>(loadOp.Entities);
            List<Country> finalResult = new List<Country>();
            string key = "key"; // it could be any random string

            foreach (tbl_indicators tmpTI in shortListIndicatorsSelected)
            {
                // create a new country to send as param to the ColumnChartControl
                Country tmpC = new Country();
                tmpC.Name = tmpTI.indicator_name;

                List<ref_country_indicator> tmprefCountryIndic = new List<ref_country_indicator>(
                    from r in refCountryIndic
                    where r.indicator_id == tmpTI.indicator_id_pk
                    select r
                    );

                foreach (ref_country_indicator tmpRCI in tmprefCountryIndic)
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

            // re-enable the render button
            buttonRenderChart.IsEnabled = true;
        }

        #region obsolete funcs, might need to be removed
        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
        #endregion

        private void buttonRenderChart_Click(object sender, RoutedEventArgs e)
        {
            // disable the button
            buttonRenderChart.IsEnabled = false;

            WorldbankGeneralChartControl control = new WorldbankGeneralChartControl();
            this.columnChartControl = control;
            // remove all controls from columnChartTab
            int countChild = this.columnChartTab.Children.Count;
            for (int i = 0; i < countChild; i++)
            {
                this.columnChartTab.Children.RemoveAt(0);
            }
            // add the chart to columnChartTab
            this.columnChartTab.Children.Add(control);

            shortListIndicatorsSelected = getIndicatorFromPKForGraph(listboxIndicatorPKSelected);

            // change render style if needed
            //if (shortListIndicatorsSelected.Count > 1)
            //{
            //    control.ThisChartRenderAs = WorldbankGeneralChartControl.RA_LINE;
            //}
            if (((string)comboBoxRenderStyle.SelectedItem).Equals(WorldbankGeneralChartControl.RA_AREA_DESC))
            {
                control.ThisChartRenderAs = WorldbankGeneralChartControl.RA_AREA;
            }
            else if (((string)comboBoxRenderStyle.SelectedItem).Equals(WorldbankGeneralChartControl.RA_BAR_DESC))
            {
                control.ThisChartRenderAs = WorldbankGeneralChartControl.RA_BAR;
            }
            else if (((string)comboBoxRenderStyle.SelectedItem).Equals(WorldbankGeneralChartControl.RA_COLUMN_DESC))
            {
                control.ThisChartRenderAs = WorldbankGeneralChartControl.RA_COLUMN;
            }
            else if (((string)comboBoxRenderStyle.SelectedItem).Equals(WorldbankGeneralChartControl.RA_LINE_DESC))
            {
                control.ThisChartRenderAs = WorldbankGeneralChartControl.RA_LINE;
            }
            else if (((string)comboBoxRenderStyle.SelectedItem).Equals("3D " + WorldbankGeneralChartControl.RA_COLUMN_DESC))
            {
                control.RenderAs3D = true;
                control.ThisChartRenderAs = WorldbankGeneralChartControl.RA_COLUMN;
            }
            else if (((string)comboBoxRenderStyle.SelectedItem).Equals("3D " + WorldbankGeneralChartControl.RA_BAR_DESC))
            {
                control.RenderAs3D = true;
                control.ThisChartRenderAs = WorldbankGeneralChartControl.RA_BAR;
            }

            GetDataForGraph(_selectedCountry, shortListIndicatorsSelected);
        }

        private void IndicatorCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            listboxIndicatorPKSelected.Add(Convert.ToInt32(((CheckBox)sender).Tag));
        }

        private void IndicatorCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            listboxIndicatorPKSelected.Remove(Convert.ToInt32(((CheckBox)sender).Tag));
        }
    }
}

