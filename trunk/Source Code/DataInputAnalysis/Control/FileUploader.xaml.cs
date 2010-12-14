using System.Windows.Controls;
using System;
using System.Net;
using System.IO;
using System.Windows.Browser;

namespace DataInputAnalysis.Control
{
    public partial class FileUploader : UserControl
    {
        /// <summary>
        /// URL of the uploaded file
        /// </summary>
        public string URL { get; set; }

        /// <summary>
        /// URI to the handler file
        /// </summary>
        public string URI { get; set; }

        /// <summary>
        /// The current File Info
        /// </summary>
        public FileInfo CurrentFileInfo { get; set; }

        /// <summary>
        /// check if user selected a file to upload or not
        /// </summary>
        public bool HasSelectedAFile { get; set; }

        public event EventHandler UploadCompleted;

        public string OutputFileName { get; set; }
        
        public FileUploader(string uri, string outputFileName)
        {
            InitializeComponent();
            this.URI = uri;
            HasSelectedAFile = false;
            BrowseButton.Click += new System.Windows.RoutedEventHandler(BrowseButton_Click);
            //UploadButton.Click += new System.Windows.RoutedEventHandler(UploadButton_Click);
            OutputFileName = outputFileName;
            this.uploadTextBlock.Visibility = System.Windows.Visibility.Collapsed;
            this.BrowseButton.IsEnabled = true;
        }
      
        void BrowseButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Text files(*.txt)|*.txt";
            dlg.Multiselect = false;            

            bool? retval = dlg.ShowDialog();

            if (retval != null && retval == true)
            {
                System.IO.Stream fileStream = dlg.File.OpenRead();
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri(URI));
                request.Method = "POST";                
                buffer = new byte[(int)fileStream.Length];
                fileStream.Read(buffer, 0, (int)fileStream.Length);                
                fileStream.Close();
                this.uploadTextBlock.Visibility = System.Windows.Visibility.Visible;
                this.BrowseButton.IsEnabled = false;
                request.BeginGetRequestStream(new AsyncCallback(ReadCallback), request);
            }            
        }
        
        private  void ResponseCallback(IAsyncResult asynchronousResult)
        {
            HttpWebRequest request = (HttpWebRequest)asynchronousResult.AsyncState;
            HttpWebResponse resp = (HttpWebResponse)request.EndGetResponse(asynchronousResult);
            Stream streamResponse = resp.GetResponseStream();
            StreamReader streamRead = new StreamReader(streamResponse);
            string responseString = streamRead.ReadToEnd();
                   
             // Close the stream object.
            streamResponse.Close();
            streamRead.Close();

            // Release the HttpWebResponse.
            resp.Close();

            if (this.UploadCompleted != null)
            {
                this.UploadCompleted(responseString, null);
            }
        }

        byte[] buffer;

        private  void ReadCallback(IAsyncResult asynchronousResult)
        {
            HttpWebRequest request = (HttpWebRequest)asynchronousResult.AsyncState;
            // End the operation.
            Stream postStream = request.EndGetRequestStream(asynchronousResult);
            postStream.Write(buffer, 0, buffer.Length);
            postStream.Close();

            request.BeginGetResponse(new AsyncCallback(ResponseCallback), request);
        }
        
    }
}
