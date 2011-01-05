using NCRVisual.web.Services;
using System.Linq;
using NCRVisual.web.DataModel;
using System;

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

        #endregion

        /// <summary>
        /// Default constructor
        /// </summary>
        public Controller()
        {
            //Default indicators
            Context = new WBDomainContext();

            //Note: Get indicator data
            Context.Load(Context.GetTbl_indicatorsQuery());    
       
            //Note: Get tabs data
            Context.Load(Context.GetRef_tab_indicatorQuery());
            
            //Note: get country data
            var load = Context.Load(Context.GetTbl_countriesQuery());
            load.Completed += new System.EventHandler(load_Completed);            
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
            var loadTabCountryData = Context.Load(Context.GetView_GeneralCountryQuery(country_pk));
            loadTabCountryData.Completed += new EventHandler(loadTabCountryData_Completed);

            var loadCountryOverviewData = Context.Load(Context.getview
        }

        void loadTabCountryData_Completed(object sender, EventArgs e)
        {
            if (GetTabCountryDataCompleted != null)
            {
                GetTabCountryDataCompleted(sender, e);
            }
        }
    }
}
