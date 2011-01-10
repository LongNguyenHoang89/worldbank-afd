using NCRVisual.web.Services;
using System.Linq;
using NCRVisual.web.DataModel;
using System;
using System.Collections.Generic;

namespace WorldMap
{
    /// <summary>
    /// Controller class for worldmap, get data from database
    /// </summary>
    public class Controller
    {
        /// <summary>
        /// The context of services working with WB database
        /// </summary>
        public WBDomainContext Context { get; set; }

        #region EventHandler
        /// <summary>
        /// Event after load base data completed
        /// </summary>
        public event EventHandler LoadInitDataCompleted;

        /// <summary>
        /// Event after load tab data completed
        /// </summary>
        public event EventHandler GetTabCountryDataCompleted;

        /// <summary>
        /// Event after tab indicator completed
        /// </summary>
        public event EventHandler GetView_TabIndicatorQueryCompleted;

        /// <summary>
        /// Event after get import data completed
        /// </summary>
        public event EventHandler GetImportData_Completed;

        /// <summary>
        /// Event after get export data completed
        /// </summary>
        public event EventHandler GetExportData_Completed;

        #endregion

        /// <summary>
        /// Default constructor
        /// </summary>
        public Controller()
        {
            //Default indicators
            Context = new WBDomainContext();

            //Note: Get indicator data
            var loadTabIndicator = Context.Load(Context.GetView_TabIndicatorQuery());
            loadTabIndicator.Completed += new EventHandler(loadTabIndicator_Completed);                  
            
            //Note: get country data
            var loadCountry = Context.Load(Context.GetTbl_countriesQuery());
            loadCountry.Completed += new System.EventHandler(load_Completed);


        }

        void loadTabIndicator_Completed(object sender, EventArgs e)
        {
            if (this.GetView_TabIndicatorQueryCompleted != null)
            {
                GetView_TabIndicatorQueryCompleted(sender, e);
            }
        }

        void load_Completed(object sender, System.EventArgs e)
        {
            if (this.LoadInitDataCompleted != null)
            {
                LoadInitDataCompleted(null, null);
            }
        }

        /// <summary>
        /// Get a country record base on its name
        /// </summary>
        /// <param name="countryName"></param>
        /// <returns></returns>
        public tbl_countries GetCountry(string countryName)
        {
            var country = from n in Context.tbl_countries 
                          where n.country_name == countryName
                          select n;

            foreach(var a in country)
            {
                return a as tbl_countries;
            }

            return null;
        }

        public void GetTabCountryData(int country_pk)
        {
            var loadTabCountryData = Context.Load(Context.GetCountryGeneralInfoQuery(country_pk));
            loadTabCountryData.Completed += new EventHandler(loadTabCountryData_Completed);

            //var loadCountryOverviewData = Context.Load(Context.
        }

        void loadTabCountryData_Completed(object sender, EventArgs e)
        {
            if (GetTabCountryDataCompleted != null)
            {
                GetTabCountryDataCompleted(sender, e);
            }
        }

        public void GetImportData(int importCountryId, List<int> exportCountryLists, int year)
        {
            var getImportData = Context.Load(Context.GetImportDataQuery(importCountryId, exportCountryLists,year));
            getImportData.Completed += new EventHandler(getImportData_Completed);
        }

        void getImportData_Completed(object sender, EventArgs e)
        {
            if (GetImportData_Completed != null)
            {
                GetImportData_Completed(sender, e);
            }
        }

        public void GetExportData(int exportCountryId, List<int> importCountryLists, int year)
        {
            var getExportData = Context.Load(Context.GetImportDataQuery(exportCountryId, importCountryLists, year));
            getExportData.Completed += new EventHandler(getExportData_Completed);            
        }

        void getExportData_Completed(object sender, EventArgs e)
        {
            if (GetExportData_Completed != null)
            {
                GetExportData_Completed(sender, e);
            }
        }        
    }
}