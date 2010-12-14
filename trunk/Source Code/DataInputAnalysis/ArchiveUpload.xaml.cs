using System.Windows.Controls;
using System;
using NCRVisual.API;
using System.Windows.Browser;

namespace DataInputAnalysis
{
    public partial class ArchiveUpload : BaseDataAnalysisControl
    {
        //public event EventHandler UploadComplete;

        public ArchiveUpload()
        {
            InitializeComponent();
            //string outputFilename = Guid.NewGuid().ToString();
            //this.OutputFileName = outputFilename;
            string path = App.Current.Host.Source.AbsoluteUri;
            path = path.Remove(path.IndexOf("ClientBin"));
            Control.FileUploader Uploader = new Control.FileUploader(path + "Upload.ashx", OutputFileName);
            //Control.FileUploader Uploader = new Control.FileUploader(App.Current.Host.Source.AbsoluteUri + "../../Upload.ashx", OutputFileName);
            this.rootPanel.Children.Add(Uploader);
            Uploader.UploadCompleted += new System.EventHandler(Uploader_UploadCompleted);
        }

        void Uploader_UploadCompleted(object sender, System.EventArgs e)
        {
            this.OutputFileName = sender.ToString();
            this.OnComplete(e);
        }
    }
}
