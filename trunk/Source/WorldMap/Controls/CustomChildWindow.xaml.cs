using System.Windows;
using System;
using System.Windows.Controls;
using NCRVisual.web.DataModel;

namespace WorldMap
{
    /// <summary>
    /// Child window show data about a country
    /// </summary>
    public partial class CustomChildWindow
    {
        Controller _worldMapController;
        tbl_countries _country;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="worldMapController"></param>
        /// <param name="country"></param>
        public CustomChildWindow(Controller worldMapController, tbl_countries country)
        {
            InitializeComponent();
            this._worldMapController = worldMapController;
            this._country = country;

            this._worldMapController.GetTabCountryData(_country.country_id_pk);
            this._worldMapController.GetTabCountryDataCompleted += new EventHandler(_worldMapController_GetTabCountryDataCompleted);

            this.CountryNameTextBlock.Text = _country.country_name;
            
            WorldbankDataGraphs.WorldbankColumnChartControl control = new WorldbankDataGraphs.WorldbankColumnChartControl();
            this.PeopleTab.Children.Add(control);            
        }

        void _worldMapController_GetTabCountryDataCompleted(object sender, EventArgs e)
        {            
            foreach (View_GeneralCountry data in this._worldMapController.Context.View_GeneralCountries)
            {
                this.RegionNameTextBlock.Text = data.region_name;
                this.LendingTypeTextBlock.Text = data.lending_type_name;
                this.IncomeLevelTextBLock.Text = data.income_level_name;
            }
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

