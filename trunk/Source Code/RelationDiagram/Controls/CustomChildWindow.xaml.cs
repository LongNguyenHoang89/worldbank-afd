using System.Windows;
using System;
using System.Windows.Controls;

namespace NCRVisual.RelationDiagram
{
    public partial class CustomChildWindow
    {
        public event EventHandler SelectAlgorithmCompleted;
        private string Algorithm = "Kamada - Kawai";

        public CustomChildWindow()
        {
            InitializeComponent();
            KK.IsChecked = true;                      
        }

        /// <summary>
        /// Set algorithm from the beginning
        /// </summary>
        public void InitAlgo()
        {
            SelectAlgorithmCompleted(Algorithm, new RoutedEventArgs());
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            SelectAlgorithmCompleted(Algorithm, e);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton rad = (RadioButton)sender;
            this.Algorithm = rad.Content.ToString();            
        }
    }
}

