using System.Windows;
using System.Windows.Controls;

namespace WorldMap
{
    public partial class ErrorNotification : ChildWindow
    {
        /// <summary>
        /// Create a error notification
        /// </summary>
        /// <param name="content">the error messages</param>
        public ErrorNotification(string content)
        {
            InitializeComponent();
            this.DisplayContent.Text = content;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }       
    }
}

