
namespace NCRVisual.web.Services
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Data;
    using System.Linq;
    using System.ServiceModel.DomainServices.EntityFramework;
    using System.ServiceModel.DomainServices.Hosting;
    using System.ServiceModel.DomainServices.Server;
    using NCRVisual.web.DataModel;


    // Implements application logic using the WBEntities context.
    // TODO: Add your application logic to these methods or in additional methods.
    // TODO: Wire up authentication (Windows/ASP.NET Forms) and uncomment the following to disable anonymous access
    // Also consider adding roles to restrict access as appropriate.
    // [RequiresAuthentication]
    [EnableClientAccess()]
    public class WBDomainService : LinqToEntitiesDomainService<WBEntities>
    {

        // TODO:
        // Consider constraining the results of your query method.  If you need additional input you can
        // add parameters to this method or create additional query methods with different names.
        // To support paging you will need to add ordering to the 'ref_country_indicator' query.
        public IQueryable<ref_country_indicator> GetRef_country_indicator()
        {
            return this.ObjectContext.ref_country_indicator;
        }

        public IQueryable<ref_country_indicator> GetRef_country_indicatorInCountryIdList(List<int> countryIdList)
        {
            return this.ObjectContext.ref_country_indicator.Where(c => countryIdList.Contains((int)c.country_id));
        }

        public void InsertRef_country_indicator(ref_country_indicator ref_country_indicator)
        {
            if ((ref_country_indicator.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(ref_country_indicator, EntityState.Added);
            }
            else
            {
                this.ObjectContext.ref_country_indicator.AddObject(ref_country_indicator);
            }
        }

        public void UpdateRef_country_indicator(ref_country_indicator currentref_country_indicator)
        {
            this.ObjectContext.ref_country_indicator.AttachAsModified(currentref_country_indicator, this.ChangeSet.GetOriginal(currentref_country_indicator));
        }

        public void DeleteRef_country_indicator(ref_country_indicator ref_country_indicator)
        {
            if ((ref_country_indicator.EntityState == EntityState.Detached))
            {
                this.ObjectContext.ref_country_indicator.Attach(ref_country_indicator);
            }
            this.ObjectContext.ref_country_indicator.DeleteObject(ref_country_indicator);
        }

        // TODO:
        // Consider constraining the results of your query method.  If you need additional input you can
        // add parameters to this method or create additional query methods with different names.
        // To support paging you will need to add ordering to the 'ref_tab_indicator' query.
        public IQueryable<ref_tab_indicator> GetRef_tab_indicator()
        {
            return this.ObjectContext.ref_tab_indicator;
        }

        // TODO:
        // Consider constraining the results of your query method.  If you need additional input you can
        // add parameters to this method or create additional query methods with different names.
        // To support paging you will need to add ordering to the 'tbl_countries' query.
        public IQueryable<tbl_countries> GetTbl_countries()
        {
            return this.ObjectContext.tbl_countries;
        }

        // TODO:
        // Consider constraining the results of your query method.  If you need additional input you can
        // add parameters to this method or create additional query methods with different names.
        // To support paging you will need to add ordering to the 'tbl_income_levels' query.
        public IQueryable<tbl_income_levels> GetTbl_income_levels()
        {
            return this.ObjectContext.tbl_income_levels;
        }

        // TODO:
        // Consider constraining the results of your query method.  If you need additional input you can
        // add parameters to this method or create additional query methods with different names.
        // To support paging you will need to add ordering to the 'tbl_indicators' query.
        public IQueryable<tbl_indicators> GetTbl_indicators()
        {
            return this.ObjectContext.tbl_indicators;
        }

        // TODO:
        // Consider constraining the results of your query method.  If you need additional input you can
        // add parameters to this method or create additional query methods with different names.
        // To support paging you will need to add ordering to the 'tbl_lending_types' query.
        public IQueryable<tbl_lending_types> GetTbl_lending_types()
        {
            return this.ObjectContext.tbl_lending_types;
        }

        // TODO:
        // Consider constraining the results of your query method.  If you need additional input you can
        // add parameters to this method or create additional query methods with different names.
        // To support paging you will need to add ordering to the 'tbl_regions' query.
        public IQueryable<tbl_regions> GetTbl_regions()
        {
            return this.ObjectContext.tbl_regions;
        }

        // TODO:
        // Consider constraining the results of your query method.  If you need additional input you can
        // add parameters to this method or create additional query methods with different names.
        // To support paging you will need to add ordering to the 'tbl_tabs' query.
        public IQueryable<tbl_tabs> GetTbl_tabs()
        {
            return this.ObjectContext.tbl_tabs;
        }

        // TODO:
        // Consider constraining the results of your query method.  If you need additional input you can
        // add parameters to this method or create additional query methods with different names.
        // To support paging you will need to add ordering to the 'tbl_trades' query.
        public IQueryable<tbl_trades> GetTbl_trades()
        {
            return this.ObjectContext.tbl_trades;
        }
    }
}


