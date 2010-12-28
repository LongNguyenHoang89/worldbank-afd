using System.Windows.Controls;
using System;

namespace NCRVisual
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
            if (MouseClick != null)
            {
                MouseClick(sender, e);
            }
        }
    }
}
