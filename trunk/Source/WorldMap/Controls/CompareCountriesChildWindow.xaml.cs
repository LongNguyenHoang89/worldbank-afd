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
    public partial class CompareCountriesChildWindow
    {
        Controller _worldMapController;
        private WorldbankDataGraphs.WorldbankGeneralChartControl columnChartControl = null;
        private List<tbl_countries> _selectedCountries;
        private List<int> _checkedIndicatorPKs;
        private LoadOperation<tbl_indicators> tblIndLoadOp = null;
        private LoadOperation<ref_country_indicator> loadOp = null;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="worldMapController"></param>
        /// <param name="country"></param>
        public CompareCountriesChildWindow(Controller worldMapController, List<tbl_countries> selectedCountries, List<int> checkedIndicatorPKs)
        {
            InitializeComponent();
            this._worldMapController = worldMapController;
            this._selectedCountries = selectedCountries;
            this._checkedIndicatorPKs = checkedIndicatorPKs;
            // pupulate the combobox
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
            foreach (tbl_indicators tmpI in selectedInd)
            {
                comboBoxIndicatorSelector.Items.Add(tmpI);
            }
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
            List<ref_country_indicator> refCountryIndic = new List<ref_country_indicator>(loadOp.Entities);
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
            // re-enable the render button
            button1.IsEnabled = true;
        }
        #endregion

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            WorldbankGeneralChartControl control = new WorldbankDataGraphs.WorldbankGeneralChartControl();
            this.columnChartControl = control;
            // select render style for the graph
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
            // disable the combobox
            this.button1.IsEnabled = false;
            // get the data needed to show the graph
            GetDataForGraph(_selectedCountries, ((tbl_indicators)comboBoxIndicatorSelector.SelectedItem).indicator_id_pk);
            // remove all other graphs from gridChart
            int childCount = gridChart.Children.Count;
            for (int i = 0; i < childCount; i++)
            {
                gridChart.Children.RemoveAt(0);
            }
            // add the chart to the gridChart
            this.gridChart.Children.Add(control);
        }
    }
}

