using System.Windows;
using System;
using System.Windows.Controls;
using NCRVisual.web.DataModel;
using WorldbankDataGraphs.Entities;
using System.Collections.Generic;
using System.ServiceModel.DomainServices.Client;
using NCRVisual.web.Services;
using System.Linq;

namespace WorldMap
{
    /// <summary>
    /// Child window show data about a country
    /// </summary>
    public partial class CustomChildWindow
    {
        Controller _worldMapController;
        private WorldbankDataGraphs.WorldbankColumnChartControl columnChartControl = null;
        private tbl_countries _selectedCountry;
        private LoadOperation<tbl_indicators> tblIndLoadOp = null;
        private LoadOperation<ref_country_indicator> loadOp = null;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="worldMapController"></param>
        /// <param name="country"></param>
        public CustomChildWindow(Controller worldMapController, tbl_countries selectedCountry, List<int> checkedIndicatorPKs)
        {
            InitializeComponent();
            this._worldMapController = worldMapController;
            this._selectedCountry = selectedCountry;

            this.CountryNameTextBlock.Text = _selectedCountry.country_name;

            WorldbankDataGraphs.WorldbankColumnChartControl control = new WorldbankDataGraphs.WorldbankColumnChartControl();
            getIndicatorFromPK(checkedIndicatorPKs[0]);
            this.columnChartControl = control;
            GetDataForGraph(_selectedCountry, checkedIndicatorPKs[0]);
            this.columnChartTab.Children.Add(control);            
        }

        private void getIndicatorFromPK(int indPK)
        {
            // get the indicator
            EntityQuery<tbl_indicators> tblIndQuery =
                from ind in _worldMapController.Context.GetTbl_indicatorsQuery()
                where (ind.indicator_id_pk == indPK)
                select ind;
            tblIndLoadOp = _worldMapController.Context.Load(tblIndQuery);
            tblIndLoadOp.Completed += new EventHandler(tblIndLoadOp_Completed);
        }

        private void tblIndLoadOp_Completed(object sender, EventArgs e)
        {
            List<tbl_indicators> selectedInd = new List<tbl_indicators>(tblIndLoadOp.Entities);
            if (selectedInd.Count > 0)
            {
                // get the first one, of course it's the only result because we query based on pk
                this.columnChartControl.ChartTitle = selectedInd[0].indicator_unit;
                this.tabItem2.Header = selectedInd[0].indicator_name;
            }
        }

        private void GetDataForGraph(tbl_countries selectedCountries, int checkedIndicator)
        {
            EntityQuery<ref_country_indicator> query =
                from c in _worldMapController.Context.GetRef_country_indicatorQuery()
                // == tmpTC.country_id_pk
                where (c.country_id == selectedCountries.country_id_pk && c.indicator_id == checkedIndicator)
                select c;
            loadOp = _worldMapController.Context.Load(query);
            loadOp.Completed += new EventHandler(loadOp_Completed);
        }

        private void loadOp_Completed(object sender, EventArgs e)
        {
            List<ref_country_indicator> refCountryIndic = new List<ref_country_indicator>(loadOp.Entities);
            List<Country> finalResult = new List<Country>();
            string key = refCountryIndic[0].indicator_id.ToString();

            // create a new country to send as param to the ColumnChartControl
            Country tmpC = new Country();
            tmpC.Name = _selectedCountry.country_name;
            foreach (ref_country_indicator tmpRCI in refCountryIndic)
            {
                YearData tmpYD = new YearData();
                tmpYD.Year = (int)tmpRCI.country_indicator_year;
                tmpYD.Attributes.Add(key, tmpRCI.country_indicator_value.ToString());
                tmpC.Years.Add(tmpYD);
            }
            finalResult.Add(tmpC);


            this.columnChartControl.AttributeShownName = key;
            this.columnChartControl.CountriesShown = finalResult;
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

        #region for pie chart tab
        #endregion

    }
}

