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
        LoadSilverlightModuleHelper loadMapModuleHelper;
        
        string mapModuleToLoad = "WorldMap";

        ChildWindow ChildWindow;

        string InputFileName = string.Empty;

        public MainPage(string announce, string xmlFile)
        {
            InitializeComponent();
            ChildWindow = new ChildWindow();

            // Init the load module helper and assign a listener for it        
            loadMapModuleHelper = new LoadSilverlightModuleHelper();
            
            loadMapModuleHelper.SilverlightModuleLoaded += new EventHandler(loadMapModuleHelper_SilverlightModuleLoaded);
            loadMapModuleHelper.loadXapFile(mapModuleToLoad);               
        }
                
        void loadMapModuleHelper_SilverlightModuleLoaded(object sender, EventArgs e)
        {
            Assembly asm = loadMapModuleHelper.LoadedModules[mapModuleToLoad];
            UserControl uc = Activator.CreateInstance(asm.GetType("WorldMap.MainPage")) as UserControl;
            this.Content = uc;
        }        
    }
}
