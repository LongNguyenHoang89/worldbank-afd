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
        tbl_countries _country;
        private WorldbankDataGraphs.WorldbankColumnChartControl columnChartControl = null;
        private List<tbl_countries> selectedCountries;
        private LoadOperation<tbl_indicators> tblIndLoadOp = null;
        private LoadOperation<ref_country_indicator> loadOp = null;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="worldMapController"></param>
        /// <param name="country"></param>
        public CustomChildWindow(Controller worldMapController, List<tbl_countries> selectedCountries, List<int> checkedIndicatorPKs)
        {
            InitializeComponent();
            this._worldMapController = worldMapController;
            this._country = selectedCountries[0];
            this.selectedCountries = selectedCountries;

            this._worldMapController.GetTabCountryData(_country.country_id_pk);
            this._worldMapController.GetTabCountryDataCompleted += new EventHandler(_worldMapController_GetTabCountryDataCompleted);

            this.CountryNameTextBlock.Text = _country.country_name;
            
            WorldbankDataGraphs.WorldbankColumnChartControl control = new WorldbankDataGraphs.WorldbankColumnChartControl();
            this.columnChartControl = control;
            GetDataForGraph(selectedCountries, checkedIndicatorPKs[0]);
            getIndicatorFromPK(checkedIndicatorPKs[0]);
            this.PeopleTab.Children.Add(control);            
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
            }
        }

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
            foreach (tbl_countries tmpTCountry in selectedCountries)
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
        }

        void _worldMapController_GetTabCountryDataCompleted(object sender, EventArgs e)
        {            
            //foreach (View_GeneralCountry data in this._worldMapController.Context.View_GeneralCountries)
            //{
            //    this.RegionNameTextBlock.Text = data.region_name;
            //    this.LendingTypeTextBlock.Text = data.lending_type_name;
            //    this.IncomeLevelTextBLock.Text = data.income_level_name;
            //}
        }
    
        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

    }
}

