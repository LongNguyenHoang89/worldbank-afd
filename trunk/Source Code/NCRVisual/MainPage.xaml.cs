using System.Windows.Controls;
using NCRVisual.Helper;
using System;
using System.Reflection;
using NCRVisual.API;
using System.Windows.Browser;
using System.Threading;

namespace NCRVisual
{
    public partial class MainPage : UserControl
    {
        LoadSilverlightModuleHelper loadDataModuleHelper;
        LoadSilverlightModuleHelper loadGraphModuleHelper;
        LoadSilverlightModuleHelper loadMapModuleHelper;

        string dataModuleToLoad = "DataInputAnalysis";
        string graphModuleToLoad = "RelationDiagram";
        string mapModuleToLoad = "WorldMap";

        ChildWindow ChildWindow;

        string InputFileName = string.Empty;

        public MainPage(string announce, string xmlFile)
        {
            InitializeComponent();
            ChildWindow = new ChildWindow();

            // Init the load module helper and assign a listener for it
            loadDataModuleHelper = new LoadSilverlightModuleHelper();
            loadGraphModuleHelper = new LoadSilverlightModuleHelper();
            loadMapModuleHelper = new LoadSilverlightModuleHelper();

            loadDataModuleHelper.SilverlightModuleLoaded += new System.EventHandler(loadModuleHelper_SilverlightModuleLoaded);
            loadDataModuleHelper.loadXapFile(dataModuleToLoad);

            loadGraphModuleHelper.SilverlightModuleLoaded += new EventHandler(loadGraphModuleHelper_SilverlightModuleLoaded);
            loadGraphModuleHelper.loadXapFile(graphModuleToLoad);

            loadMapModuleHelper.SilverlightModuleLoaded += new EventHandler(loadMapModuleHelper_SilverlightModuleLoaded);
            loadMapModuleHelper.loadXapFile(mapModuleToLoad);    

            this.Result_TextBox.Text = announce;
            InputFileName = xmlFile;

            if (!string.IsNullOrEmpty(xmlFile))
            {
                this.VisualizeButton.Visibility = System.Windows.Visibility.Visible;
                this.VisualizeLabel.Visibility = System.Windows.Visibility.Visible;
            }
        }
        
        void loadGraphModuleHelper_SilverlightModuleLoaded(object sender, EventArgs e)
        {
            VisualizeButton.MouseClick += new EventHandler(VisualizeButton_MouseClick);
        }

        void loadMapModuleHelper_SilverlightModuleLoaded(object sender, EventArgs e)
        {
            WordMapButton.MouseClick += new EventHandler(WordMapButton_MouseClick);
        }

        void WordMapButton_MouseClick(object sender, EventArgs e)
        {
            Assembly asm = loadMapModuleHelper.LoadedModules[mapModuleToLoad];
            UserControl uc = Activator.CreateInstance(asm.GetType("WorldMap.MainPage")) as UserControl;
            this.Content = uc;
        }

        void VisualizeButton_MouseClick(object sender, EventArgs e)
        {
            Assembly asm = loadGraphModuleHelper.LoadedModules[graphModuleToLoad];
            UserControl uc = Activator.CreateInstance(asm.GetType(LoadSilverlightModuleHelper.MAIN_NAMESPACE + graphModuleToLoad + LoadSilverlightModuleHelper.MODULE_EXT), InputFileName + ".xml") as UserControl;
            this.Content = uc;
        }

        void UploadButton_MouseClick(object sender, EventArgs e)
        {

            // This function run when a module we requested to load is downloaded to client and was initialized
            Assembly asm = loadDataModuleHelper.LoadedModules[dataModuleToLoad];

            // Init the user control

            UserControl uc = asm.CreateInstance("DataInputAnalysis.ArchiveUpload") as UserControl;
            BaseDataAnalysisControl ctrl = (BaseDataAnalysisControl)uc;
            ctrl.UploadComplete += new EventHandler(ctrl_UploadComplete);            

            // add it to the main page
            ChildWindow.Content = ctrl;
            ChildWindow.Show();
        }

        void ctrl_UploadComplete(object sender, EventArgs e)
        {
            System.Windows.Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                InputFileName = (sender as BaseDataAnalysisControl).OutputFileName;
                ChildWindow.Close();
                this.Result_TextBox.Text = "You have uploaded an Archive file, press Visualize button for View it";
                this.VisualizeButton.Visibility = System.Windows.Visibility.Visible;
                this.VisualizeLabel.Visibility = System.Windows.Visibility.Visible; 
            });                                 
            //HtmlPage.Window.Navigate(new Uri("\\NcrVisual.aspx?Param=arc&FileName="+this.InputFileName, UriKind.Relative));
        }

        void PhpBBButton_MouseClick(object sender, EventArgs e)
        {
            HtmlPage.Window.Navigate(new Uri("\\PHPBB_DB_Info.aspx", UriKind.Relative));
        }

        #region listeners
        public void loadModuleHelper_SilverlightModuleLoaded(object source, EventArgs e)
        {
            UploadButton.MouseClick += new EventHandler(UploadButton_MouseClick);
            PhpBBButton.MouseClick += new EventHandler(PhpBBButton_MouseClick);
        }
        #endregion
    }
}
