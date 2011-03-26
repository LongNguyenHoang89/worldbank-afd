using NCRVisual.web.Services;
using System.Linq;
using NCRVisual.web.DataModel;
using System;
using System.Collections.Generic;
using System.ServiceModel.DomainServices.Client;
using System.Windows.Media;

namespace WorldMap.Helper
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

        /// <summary>
        /// Converter to turn sql geometry type to Silverlight collection Point
        /// </summary>
        public WKTToGeomatryPoint GeomatryConverter { get; set; }

        #region EventHandler
        /// <summary>
        /// Event after loading base data completed
        /// </summary>
        public event EventHandler LoadInitDataCompleted;

        /// <summary>
        /// Event after loading tab data completed
        /// </summary>
        public event EventHandler GetTabCountryDataCompleted;

        /// <summary>
        /// Event after loading tab indicator completed
        /// </summary>
        public event EventHandler GetView_TabIndicatorQueryCompleted;

        /// <summary>
        /// Event after getting import data completed
        /// </summary>
        public event EventHandler GetImportData_Completed;

        /// <summary>
        /// Event after gettin export data completed
        /// </summary>
        public event EventHandler GetExportData_Completed;

        /// <summary>
        /// Event after getting Border of a country completed;
        /// </summary>
        public event EventHandler GetBorder_completed;

        /// <summary>
        ///  Event after getting WB Project list of a country completed;
        /// </summary>
        public event EventHandler GetCountryWBProject_completed;

        public event EventHandler LoadUserData_Completed;
        public event EventHandler CheckUserCompleted;

        public event EventHandler DeleteUserCountry_Completed;
        public event EventHandler InsertUserCountry_Completed;
        public event EventHandler LoadUserCountry_Completed;
        #endregion

        /// <summary>
        /// Default constructor
        /// </summary>
        public Controller()
        {
            //Default indicators
            Context = new WBDomainContext();
            GeomatryConverter = new WKTToGeomatryPoint();

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

            foreach (var a in country)
            {
                return a as tbl_countries;
            }

            return null;
        }

        /// <summary>
        /// Get the overview country data
        /// </summary>
        /// <param name="country_pk"></param>
        public void GetTabCountryData(int country_pk)
        {
            var loadTabCountryData = Context.Load(Context.GetCountryGeneralInfoQuery(country_pk));
            loadTabCountryData.Completed += new EventHandler(loadTabCountryData_Completed);
        }

        /// <summary>
        /// Get the WKT for country border
        /// </summary>
        /// <param name="countryCode"></param>
        public void GetCountryBorder(string countryCode, SolidColorBrush color)
        {
            EntityQuery<View_CountryBorder> query = Context.GetCountryBorderQuery(countryCode);
            Action<LoadOperation<View_CountryBorder>> completeProcessing = delegate(LoadOperation<View_CountryBorder> loadOp)
            {
                if (!loadOp.HasError)
                {
                    loadBorder_Completed(loadOp.Entities, color, countryCode);
                }
                else
                {
                    //LogAndNotify(loadOp.Error);
                }

            };
            LoadOperation<View_CountryBorder> loadOperation = Context.Load(query, completeProcessing, true);
        }

        private void loadBorder_Completed(IEnumerable<View_CountryBorder> results, SolidColorBrush countryColor, string countryCode)
        {
            foreach (View_CountryBorder result in results)
            {
                if (GetBorder_completed != null)
                {
                    GetBorder_completed(new object[3] { GeomatryConverter.GetObject(result.geom), countryColor, countryCode }, null);
                }
            }
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
            var getImportData = Context.Load(Context.GetImportDataQuery(importCountryId, exportCountryLists, year));
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

        /// <summary>
        /// Get country project data
        /// </summary>
        /// <param name="countryId"></param>
        public void GetCountryWBProject(int countryId)
        {
            var getCountryProjectData = Context.Load(Context.GetCountryProjectsQuery(countryId));
            getCountryProjectData.Completed += new EventHandler(getCountryProjectData_Completed);
        }

        void getCountryProjectData_Completed(object sender, EventArgs e)
        {
            if (GetCountryWBProject_completed != null)
            {
                GetCountryWBProject_completed(sender, e);
            }
        }

        #region save n load data

        public void InsertUser(tbl_users tbl_user)
        {
            Context.tbl_users.Add(tbl_user);
            Context.SubmitChanges();

        }

        void loadUserData_Completed(IEnumerable<tbl_users> r, string cid)
        {
            var a = from n in r
                    where n.msn_id == cid
                    select n;
            tbl_users user = new tbl_users();
            foreach (tbl_users x in a)
            {
                user = x;
            }
            if (this.LoadUserData_Completed != null)
            {
                LoadUserData_Completed(user, null);
            }
        }
        

        public void CheckExist(string cid)
        {            

            //EntityQuery<View_CountryBorder> query = Context.GetCountryBorderQuery(countryCode);
            Action<LoadOperation<tbl_users>> completeProcessing = delegate(LoadOperation<tbl_users> loadOp)
            {
                if (!loadOp.HasError)
                {
                    loadUserData_Completed(loadOp.Entities, cid);
                }
                else
                {
                    //LogAndNotify(loadOp.Error);
                }

            };
            //LoadOperation<View_CountryBorder> loadOperation = Context.Load(query, completeProcessing, true);
            var loadUser_Data = Context.Load(Context.GetUserQuery(cid),completeProcessing,true);
            
        }

        public tbl_users GetUser(string cid)
        {
            Context.Load(Context.GetUserQuery(cid));
            var user = from n in Context.tbl_users
                       where n.msn_id == cid
                       select n;

            foreach (var a in user)
            {
                return a as tbl_users;
            }

            return null;
        }

        void loadUserCountry_Completed(IEnumerable<ref_user_country> r, tbl_users tbl_users, List<ref_user_country> countries)
        {
            
            var a = from n in r
                    where tbl_users.user_id_pk == n.user_id
                    select n;
            foreach (ref_user_country x in a)
            {
                Context.ref_user_countries.Remove(x);
            }
            foreach (ref_user_country x in countries)
            {
                x.user_id = tbl_users.user_id_pk;
                Context.ref_user_countries.Add(x);
            }

            Context.SubmitChanges();            
                        
        }

        public void InsertUserCountry(tbl_users tbl_user, List<ref_user_country> countries)
        {
            //EntityQuery<View_CountryBorder> query = Context.GetCountryBorderQuery(countryCode);
            Action<LoadOperation<ref_user_country>> completeProcessing = delegate(LoadOperation<ref_user_country> loadOp)
            {
                if (!loadOp.HasError)
                {
                    loadUserCountry_Completed(loadOp.Entities, tbl_user, countries);
                }
                else
                {
                    //LogAndNotify(loadOp.Error);
                }

            };
            //LoadOperation<View_CountryBorder> loadOperation = Context.Load(query, completeProcessing, true);
            var loadUser_Data = Context.Load(Context.GetRef_user_countryQuery(), completeProcessing, true);
        }

        void loadRefUserCountry_Completed(object sender, EventArgs e)
        {
            if (this.LoadUserCountry_Completed != null)
            {
                LoadUserCountry_Completed(sender, e);
            }

        }

        public void LoadUserCountry()
        {
            var loadRefUserCountry = Context.Load(Context.GetRef_user_countryQuery());
            loadRefUserCountry.Completed += new EventHandler(loadRefUserCountry_Completed);            
        }

        public List<ref_user_country> LoadRefUserCountry(tbl_users tbl_user)
        {
            var ref_user_country = from n in Context.ref_user_countries
                                   where n.user_id == tbl_user.user_id_pk
                                   select n;
            List<ref_user_country> listCountry = new List<ref_user_country>();
            foreach (var a in ref_user_country)
            {
                listCountry.Add(a);
            }
            return listCountry;
        }

        #endregion
    }
}