using System.Windows;
using System.Windows.Controls;
using NCRVisual.web.DataModel;
using System.Collections.Generic;
using System;

namespace WorldMap
{
    public partial class TradeMode
    {
        public event EventHandler Refresh;
        public event EventHandler Complete;

        /// <summary>
        /// Default constructor
        /// </summary>
        public TradeMode()
        {
            InitializeComponent();
        }

        public void PopulateData(IList<tbl_countries> country)
        {            
            this.TypeComboBox.SelectionChanged += new SelectionChangedEventHandler(ComboBox_SelectionChanged);
            this.YearComboBox.SelectionChanged += new SelectionChangedEventHandler(ComboBox_SelectionChanged);
            this.CountryComboBox.SelectionChanged += new SelectionChangedEventHandler(ComboBox_SelectionChanged);

            string[] typeList = { "Import", "Export" };
            this.TypeComboBox.ItemsSource = typeList;

            for (int i = 1996; i <= 2009; i++)
            {
                this.YearComboBox.Items.Add(i);
            }

            this.CountryComboBox.ItemsSource = country;
        }

        void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TypeComboBox.SelectedItem != null && YearComboBox.SelectedItem != null && CountryComboBox.SelectedItem != null)
            {
                this.OKButton.IsEnabled = true;
            }
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if (Complete != null)
            {
                Complete(sender, null);
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            if (Refresh != null)
            {
                Refresh(sender, null);
            }
        }        
    }
}

