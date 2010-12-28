using System.Windows;
using System;
using System.Windows.Controls;

namespace WorldMap
{
    public partial class CustomChildWindow
    {
        public CustomChildWindow()
        {
            InitializeComponent();
            WorldbankDataGraphs.WorldbankColumnChartControl control = new WorldbankDataGraphs.WorldbankColumnChartControl();
            this.LayoutRoot.Children.Add(control);
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

