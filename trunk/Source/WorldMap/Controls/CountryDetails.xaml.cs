using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.DomainServices.Client;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using NCRVisual.web.DataModel;
using WorldbankDataGraphs;
using WorldbankDataGraphs.Entities;
using WorldMap.Helper;
using NCRVisual.Web.Helper;
using NCRVisual.Web.Items;
using System.Windows.Browser;
using ImageTools.Helpers;
using ImageTools;
using ImageTools.IO;
using ImageTools.IO.Png;
using ImageTools.IO.Jpeg;
using ImageTools.IO.Bmp;
using System.IO;

namespace WorldMap
{
    /// <summary>
    /// Child window show data about a country
    /// </summary>
    public partial class CountryDetails
    {
        #region constants
        const int FEED_LIMIT = 3;
        #endregion

        public Controller _worldMapController { get; set; }
        private WorldbankDataGraphs.WorldbankGeneralChartControl columnChartControl = null;
        public tbl_countries _selectedCountry;
        private LoadOperation<tbl_indicators> tblIndsLoadOp = null;
        private LoadOperation<ref_country_indicator> loadOp = null;
        private List<int> listboxIndicatorPKSelected = new List<int>();
        private List<tbl_indicators> listIndicatorSelectedFromWM;
        public List<tbl_indicators> shortListIndicatorsSelected;
        public MainPage mainPage { get; set; }

        public event EventHandler SaveGraphButton_Completed;

        public event EventHandler ProjectSelectionChanged;
        /// <summary>
        /// Default constructor
        /// </summary>
        public CountryDetails()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Start populate everything
        /// </summary>
        /// <param name="worldMapController"></param>
        /// <param name="selectedCountry"></param>
        /// <param name="checkedIndicatorPKs"></param>
        public void PopulateData(Controller worldMapController, tbl_countries selectedCountry, List<int> checkedIndicatorPKs)
        {
            // set some var with input params
            this._worldMapController = worldMapController;
            this._selectedCountry = selectedCountry;
            // generate the list of shortlist indicators
            getIndicatorsFromPKs(checkedIndicatorPKs);
            // user related processing
            this.mainPage = mainPage;
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

            //Get country overview data
            worldMapController.GetTabCountryData(selectedCountry.country_id_pk);
            worldMapController.GetTabCountryDataCompleted += new EventHandler(worldMapController_GetTabCountryDataCompleted);

            Uri uriSource = new Uri(Application.Current.Host.Source + "../../../flags/" + selectedCountry.country_iso_code + ".png", UriKind.Absolute);
            Flag.Source = new BitmapImage(uriSource);

            //Get project data
            worldMapController.GetCountryWBProject(selectedCountry.country_id_pk);
            worldMapController.GetCountryWBProject_completed += new EventHandler(worldMapController_GetCountryWBProject_completed);
        }

        void worldMapController_GetCountryWBProject_completed(object sender, EventArgs e)
        {
            List<tbl_projects> returnList = new List<tbl_projects>(
                from c in _worldMapController.Context.tbl_projects
                where (c.country_id == _selectedCountry.country_id_pk)
                select c
                );
            this.ProjectListDataGrid.ItemsSource = returnList;
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

            ButtonSaveShortCut.IsEnabled = true;

            button1.IsEnabled = true;
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

        private void ButtonSaveShortCut_Click(object sender, RoutedEventArgs e)
        {
            SaveGraphButton_Completed(sender, e);
        }

        #region rss feed
        RSSReader rssReader = new RSSReader();
        List<tbl_tabs> userFavTabList;

        private void tabControl1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0) // this makes sure we won't get an "Out of bound" error
            {
                TabItem tmpTI = (TabItem)e.AddedItems[0];
                if (tmpTI.Header.ToString().ToUpper().Equals("News".ToUpper())) // make sure the header is "News"
                {
                    this.loadingIndicator.IsBusy = true;
                    if (this.mainPage.user != null)
                    {
                        foreach (object o in listBoxFeedList.Items)
                        {
                            listBoxFeedList.Items.RemoveAt(0);
                        }
                        // get user's favourite tabs
                        _worldMapController.GetUserFavTab(mainPage.user);
                        _worldMapController.LoadUserFavTabCompleted += new Controller.LoadUserFavTabCompletedDelegate(_worldMapController_LoadUserFavTabCompleted);
                    }
                    else
                    {
                        // show error on page
                    }
                }
            }
        }

        private void _worldMapController_LoadUserFavTabCompleted(object source, WorldMap.Helper.Controller.LoadUserFavTabEventArgs e)
        {
            // load the rss feed
            userFavTabList = e.UserFavTabs;
            rssReader.RSSReadCompleted += new RSSReader.RSSReadCompletedDelegate(rssReader_RSSReadCompleted);
            rssReader.FeedLimit = FEED_LIMIT;
            if (userFavTabList.Count > 0)
            {
                string feedLink = userFavTabList[0].tab_feed_link;
                userFavTabList.RemoveAt(0);
                rssReader.ReadRSS(feedLink);
            }
            //rssReader.ReadRSS("http://wbws.worldbank.org/feeds/xml/Social_Development.xml");
        }

        private void rssReader_RSSReadCompleted(object source, EventArgs e)
        {
            foreach (RSSFeed tmpFeed in rssReader.RSSFeedList)
            {
                listBoxFeedList.Items.Add(tmpFeed);
            }
            this.loadingIndicator.IsBusy = false;
            // continues to load other feed if all fav tab not load
            if (userFavTabList != null && userFavTabList.Count > 0)
            {
                string feedLink = userFavTabList[0].tab_feed_link;
                userFavTabList.RemoveAt(0);
                rssReader.ReadRSS(feedLink);
            }
        }

        private void listBoxFeedList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                string rawSummary = ((RSSFeed)e.AddedItems[0]).Description;
                int firstDiv = rawSummary.IndexOf("</div>");
                string summary = "";
                if (firstDiv != -1)
                {
                    summary = rawSummary.Substring(0, firstDiv);
                }
                else
                {
                    summary = rawSummary;
                }
                textBoxFeedContent.Text = ((RSSFeed)e.AddedItems[0]).Title + "\n\n" + summary;
            }
        }

        private void buttonGetNews_Click(object sender, RoutedEventArgs e)
        {
            this.loadingIndicator.IsBusy = true;
            listBoxFeedList.Items.Clear();
            try
            {
                rssReader.ReadRSS(textBoxCustomRSSLink.Text);
            }
            catch (Exception)
            {
                ErrorNotification errorNoti = new ErrorNotification("You must enter a valid RSS link");
            }

        }

        private void buttonViewFullRSS_Click(object sender, RoutedEventArgs e)
        {
            if (listBoxFeedList.SelectedItem != null)
            {
                RSSFeed rssFeed = (RSSFeed)listBoxFeedList.SelectedItem;
                if (rssFeed.ArticleLink != null)
                {
                    HtmlPage.Window.Navigate(rssFeed.ArticleLink, "_blank");
                }
                else
                {
                    ErrorNotification errorNoti = new ErrorNotification("The RSS doesn't have a full content page");
                }
            }
            else
            {
                ErrorNotification errorNoti = new ErrorNotification("No feed selected, please select a feed");
            }
        }

        private void buttonOptions_Click(object sender, RoutedEventArgs e)
        {
            /*RSSFeedOptions feedOptions = new RSSFeedOptions();
            feedOptions.Show();*/
        }
        #endregion

        // this is the button to export graph image
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SaveFileDialog dialog = new SaveFileDialog();
                dialog.DefaultExt = ".jpg";
                dialog.Filter = "JPEG image|*.jpg|PNG image|*.png|BMP image|*.bmp";
                // prompt for a location to save the image
                if (dialog.ShowDialog() == true)
                {
                    // the "using" block ensures the stream is cleared upon completed
                    using (Stream stream = dialog.OpenFile())
                    {
                        WriteableBitmap bitmap = new WriteableBitmap(columnChartTab, null);
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
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Display project data when change selection of Project List
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProjectListDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {                        
            if (ProjectSelectionChanged != null)
            {
                this.ProjectSelectionChanged(ProjectListDataGrid.SelectedItem as tbl_projects, null);
            }
            
        }
    }
}

