using System.Windows.Controls;
using System;

namespace NCRVisual.API
{
    public partial class BaseDataAnalysisControl : UserControl
    {
        public event EventHandler UploadComplete;

        public string OutputFileName { get; set; }


        protected void OnComplete(EventArgs e)
        {
            EventHandler handler = UploadComplete;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        
        public BaseDataAnalysisControl()
        {
            InitializeComponent();
        }
    }
}
