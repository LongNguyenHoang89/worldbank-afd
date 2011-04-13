using System.Windows;
using System.Windows.Controls;

namespace WorldMap
{
    public partial class CommentForm : ChildWindow
    {
        public CommentForm()
        {
            InitializeComponent();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}

