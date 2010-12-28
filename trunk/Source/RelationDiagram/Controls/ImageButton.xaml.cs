using System.Windows.Controls;
using System;

namespace NCRVisual.RelationDiagram
{
    public partial class ImageButton : UserControl
    {
        public event EventHandler MouseClick;

        public ImageButton()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            MouseClick(sender, e);
        }
    }
}
