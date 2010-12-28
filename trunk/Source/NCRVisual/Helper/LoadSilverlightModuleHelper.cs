using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Windows;
using System.Windows.Resources;
using System.Xml.Linq;

namespace NCRVisual.Helper
{
    public class LoadSilverlightModuleHelper
    {
        #region Constants
        private const string XAP_EXT = ".xap";
        private const string DLL_EXT = ".dll";        
        private const string MANIFEST_FILENAME = "AppManifest.xaml";
        private const string FILE_NAME_ATTR = "Source";

        public const string MODULE_EXT = ".RelationDiagramControl";
        public const string MAIN_NAMESPACE = "NCRVisual.";
        
        #endregion

        #region Variables
        private string loadingModuleName = "";

        private Dictionary<string, Assembly> loadedModules = new Dictionary<string, Assembly>();
        public Dictionary<string, Assembly> LoadedModules
        {
            get { return loadedModules; }
            set { loadedModules = value; }
        }
        #endregion

        public event EventHandler SilverlightModuleLoaded;

        public void loadXapFile(string xapFileName)
        {
            loadingModuleName = xapFileName.Replace(XAP_EXT, "");

            if (loadedModules.ContainsKey(loadingModuleName))
            {
                // That mean we already loaded this module, so we don't need to load it anymore
                // That also mean we can fire a loaded event here
                this.SilverlightModuleLoaded(this, new EventArgs());
            }
            else
            {
                WebClient wc = new WebClient();
                wc.OpenReadCompleted += new OpenReadCompletedEventHandler(wc_OpenReadCompleted);
                wc.OpenReadAsync(new Uri(loadingModuleName + XAP_EXT, UriKind.Relative));
                //wc.OpenReadAsync(new Uri("D:/Working/MHST2010/Source/NCRVisual.Web/ClientBin/RelationDiagram.xap"));
            }
        }

        private void wc_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            if (null != e.Error)
            {
                //report the download error
                // TODO: show error to user
                return;
            }

            // Read the AppManifest.xaml to know which files need to be loaded
            StreamResourceInfo manifestStream = Application.GetResourceStream(new StreamResourceInfo(e.Result, null), new Uri(MANIFEST_FILENAME, UriKind.Relative));
            StreamReader streamReader = new StreamReader(manifestStream.Stream);
            XElement appManifestContent = XDocument.Parse(streamReader.ReadToEnd()).Root;
            // Get the file list from the content of AppManifest.xaml
            List<XElement> parts = (from assemblyParts in appManifestContent.Elements().Elements()
                                    select assemblyParts).ToList();

            Assembly aDLL = null;
            AssemblyPart assemblyPart = new AssemblyPart();

            // Iterate through each dll file listed in the AppManifest.xaml and load the assembly parts
            foreach (XElement tmpXElement in parts)
            {
                String tmpFileName = tmpXElement.Attribute(FILE_NAME_ATTR).Value;
                StreamResourceInfo streamInfo = Application.GetResourceStream(new StreamResourceInfo(e.Result, "application/binary"), new Uri(tmpFileName, UriKind.Relative));

                if (tmpFileName.Equals(loadingModuleName + DLL_EXT))
                {
                    // It's the application's dll
                    aDLL = assemblyPart.Load(streamInfo.Stream);
                }
                else
                {
                    // It's the references dlls
                    assemblyPart.Load(streamInfo.Stream);
                }
            }


            if (null == aDLL)
            {
                //report the assembly extracting error
                // TODO: Show error
                return;
            }

            //// Get an instance of the XAML object
            //UserControl content = aDLL.CreateInstance(loadingModuleName + MODULE_EXT) as UserControl;

            //if (null == content)
            //{
            //    //report the type instnatiating error
            //    // TODO: Report error
            //    return;
            //}

            // add it to the dictionary
            this.loadedModules.Add(loadingModuleName, aDLL);
            // fire the loaded event to let everyone know that loading of a module is completed
            this.SilverlightModuleLoaded(this, new EventArgs());
        }
    }
}
