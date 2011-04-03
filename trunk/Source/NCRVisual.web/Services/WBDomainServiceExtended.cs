using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel.DomainServices.EntityFramework;
using NCRVisual.web.DataModel;
using System.Data;
using System.ServiceModel.DomainServices.Hosting;

namespace NCRVisual.web.Services
{    
    public partial class WBDomainService : LinqToEntitiesDomainService<WBEntities>
    {

        #region for rss reader
        /// <summary>
        /// This is a custom function to return indicator query that only select a few indicator in a list of specified indicator_id_pk
        /// </summary>
        /// <returns></returns>
        public IQueryable<tbl_tabs> GetTbl_tabsInPKList(List<int> pks)
        {
            return this.ObjectContext.tbl_tabs.Where(i => pks.Contains((int)i.tab_id_pk));
        }
        #endregion

        public IQueryable<ref_country_indicator> GetRef_country_indicatorInCountryIdList(List<int> countryIdList)
        {
            return this.ObjectContext.ref_country_indicator.Where(c => countryIdList.Contains((int)c.country_id));
        }

        /// <summary>
        /// This is a custom function to return indicator query that only select a few indicator in a list of specified indicator_id_pk
        /// </summary>
        /// <returns></returns>
        public IQueryable<tbl_indicators> GetTbl_indicatorsInPKList(List<int> pks)
        {
            return this.ObjectContext.tbl_indicators.Where(i => pks.Contains((int)i.indicator_id_pk));
        }

        public IQueryable<ref_country_indicator> GetRef_country_indicatorInIndicatorIDList(List<int> indicatorIdList)
        {
            return this.ObjectContext.ref_country_indicator.Where(i => indicatorIdList.Contains((int)i.indicator_id));
        }
                


        /// <summary>
        /// Get general information about a country
        /// </summary>
        /// <param name="country_pk"></param>
        /// <returns></returns>
        ///        
        public IQueryable<View_GeneralCountry> GetCountryGeneralInfo(int country_pk)
        {
            return this.ObjectContext.View_GeneralCountry.Where(c => c.country_id_pk == country_pk);
        }


        /// <summary>
        /// Get all import data of a country and a list of export countries
        /// </summary>
        /// <param name="importCountry"></param>
        /// <param name="exportCountryIdList"></param>
        /// <returns></returns>
        public IQueryable<tbl_trades> GetImportData(int importCountry, List<int> exportCountryIdList, int year)
        {
            return this.ObjectContext.tbl_trades.Where(c => exportCountryIdList.Contains((int)c.country_from_id) && importCountry == c.country_to_id && c.trade_year == year);
        }

        /// <summary>
        /// Get all export data of a country and a list of import countries
        /// </summary>
        /// <param name="exportCountry"></param>
        /// <param name="importCountryIdList"></param>
        /// <returns></returns>
        public IQueryable<tbl_trades> GetExportData(int exportCountry, List<int> importCountryIdList, int year)
        {
            return this.ObjectContext.tbl_trades.Where(c => importCountryIdList.Contains((int)c.country_to_id) && exportCountry == c.country_from_id && c.trade_year == year);
        }                

        /// <summary>
        /// Get the border of a selected country
        /// </summary>
        /// <param name="ISO">Iso code for country</param>
        /// <returns></returns>
        public IQueryable<View_CountryBorder> GetCountryBorder(string ISO)
        {
            return this.ObjectContext.View_CountryBorder.Where(c => c.ISO2 == ISO);
        }

        /// <summary>
        /// Get the worl bank project list of a specific country
        /// </summary>
        /// <param name="countryId">Country Id</param>
        /// <returns></returns>
        public IQueryable<tbl_projects> GetCountryProjects(int countryId)        
        {
            return this.ObjectContext.tbl_projects.Where(c => c.country_id == countryId);
        }

        #region save n load data
        public IQueryable<tbl_users> GetUser(string cid)
        {
            return this.ObjectContext.tbl_users.Where(c => c.msn_id == cid);
        }
        public IQueryable<ref_user_country> GetUserCountry(int user_id)
        {
            return this.ObjectContext.ref_user_country.Where(c => c.user_id == user_id);
        }

        public void InsertTbl_users(tbl_users tbl_users)
        {
            if ((tbl_users.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(tbl_users, EntityState.Added);
            }
            else
            {
                this.ObjectContext.tbl_users.AddObject(tbl_users);
            }
        }

        public void UpdateTbl_users(tbl_users currenttbl_users)
        {
            this.ObjectContext.tbl_users.AttachAsModified(currenttbl_users, this.ChangeSet.GetOriginal(currenttbl_users));
        }

        public void DeleteTbl_users(tbl_users tbl_users)
        {
            if ((tbl_users.EntityState == EntityState.Detached))
            {
                this.ObjectContext.tbl_users.Attach(tbl_users);
            }
            this.ObjectContext.tbl_users.DeleteObject(tbl_users);
        }

        public void InsertRef_user_country(ref_user_country ref_user_country)
        {
            if ((ref_user_country.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(ref_user_country, EntityState.Added);
            }
            else
            {
                this.ObjectContext.ref_user_country.AddObject(ref_user_country);
            }
        }

        public void UpdateRef_user_country(ref_user_country currentref_user_country)
        {
            this.ObjectContext.ref_user_country.AttachAsModified(currentref_user_country, this.ChangeSet.GetOriginal(currentref_user_country));
        }

        public void DeleteRef_user_country(ref_user_country ref_user_country)
        {
            if ((ref_user_country.EntityState == EntityState.Detached))
            {
                this.ObjectContext.ref_user_country.Attach(ref_user_country);
            }
            this.ObjectContext.ref_user_country.DeleteObject(ref_user_country);
        }

        public void InsertRef_user_indicator(ref_user_indicator ref_user_indicator)
        {
            if ((ref_user_indicator.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(ref_user_indicator, EntityState.Added);
            }
            else
            {
                this.ObjectContext.ref_user_indicator.AddObject(ref_user_indicator);
            }
        }

        public void UpdateRef_user_indicator(ref_user_indicator currentref_user_indicator)
        {
            this.ObjectContext.ref_user_indicator.AttachAsModified(currentref_user_indicator, this.ChangeSet.GetOriginal(currentref_user_indicator));
        }

        public void DeleteRef_user_indicator(ref_user_indicator ref_user_indicator)
        {
            if ((ref_user_indicator.EntityState == EntityState.Detached))
            {
                this.ObjectContext.ref_user_indicator.Attach(ref_user_indicator);
            }
            this.ObjectContext.ref_user_indicator.DeleteObject(ref_user_indicator);
        }

        public void InsertTbl_graphs(tbl_graphs currenttbl_graphs)
        {
            if ((currenttbl_graphs.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(currenttbl_graphs, EntityState.Added);
            }
            else
            {
                this.ObjectContext.tbl_graphs.AddObject(currenttbl_graphs);
            }
        }

        public void UpdateTbl_graphs(tbl_graphs currenttbl_graphs)
        {
            this.ObjectContext.tbl_graphs.AttachAsModified(currenttbl_graphs, this.ChangeSet.GetOriginal(currenttbl_graphs));
        }

        public void DeleteTbl_graphs(tbl_graphs tbl_graphs)
        {
            if ((tbl_graphs.EntityState == EntityState.Detached))
            {
                this.ObjectContext.tbl_graphs.Attach(tbl_graphs);
            }
            this.ObjectContext.tbl_graphs.DeleteObject(tbl_graphs);
        }


        #endregion

        #region Search Country

        /// <summary>
        /// Get the country 
        /// </summary>
        /// <param name="indicatorIdPK"></param>
        /// <param name="year"></param>
        /// <param name="fromValue"></param>
        /// <param name="toValue"></param>
        /// <returns></returns>
        public IQueryable<ref_country_indicator> GetCountryByIndicators(int indicatorIdPK, int year, int? fromValue, int? toValue)
        {
            if (fromValue == null && toValue != null)
            {
                return this.ObjectContext.ref_country_indicator.Where(c => c.indicator_id == indicatorIdPK
                    && c.country_indicator_year == year
                    && c.country_indicator_value <= toValue);
            }
            else if (fromValue != null && toValue == null)
            {
                return this.ObjectContext.ref_country_indicator.Where(c => c.indicator_id == indicatorIdPK
                    && c.country_indicator_year == year                    
                    && c.country_indicator_value >= fromValue);
            }
            else if (fromValue != null && toValue != null)
            {
                return this.ObjectContext.ref_country_indicator.Where(c => c.indicator_id == indicatorIdPK
                    && c.country_indicator_year == year
                    && c.country_indicator_value <= toValue
                    && c.country_indicator_value >= fromValue);
            }
            else
            {
                return this.ObjectContext.ref_country_indicator.Where(c => c.indicator_id == indicatorIdPK
                    && c.country_indicator_year == year);
            }
        }

        #endregion
    }
}