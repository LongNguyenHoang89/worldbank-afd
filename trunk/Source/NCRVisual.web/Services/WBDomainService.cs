
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
        [Query(IsDefault = true)]
        public IQueryable<ref_country_indicator> GetRef_country_indicator()
        {
            return this.ObjectContext.ref_country_indicator;
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
        // To support paging you will need to add ordering to the 'tbl_countries' query.
        [Query(IsDefault = true)]
        public IQueryable<tbl_countries> GetTbl_countries()
        {
            return this.ObjectContext.tbl_countries;
        }

        public void InsertTbl_countries(tbl_countries tbl_countries)
        {
            if ((tbl_countries.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(tbl_countries, EntityState.Added);
            }
            else
            {
                this.ObjectContext.tbl_countries.AddObject(tbl_countries);
            }
        }

        public void UpdateTbl_countries(tbl_countries currenttbl_countries)
        {
            this.ObjectContext.tbl_countries.AttachAsModified(currenttbl_countries, this.ChangeSet.GetOriginal(currenttbl_countries));
        }

        public void DeleteTbl_countries(tbl_countries tbl_countries)
        {
            if ((tbl_countries.EntityState == EntityState.Detached))
            {
                this.ObjectContext.tbl_countries.Attach(tbl_countries);
            }
            this.ObjectContext.tbl_countries.DeleteObject(tbl_countries);
        }

        // TODO:
        // Consider constraining the results of your query method.  If you need additional input you can
        // add parameters to this method or create additional query methods with different names.
        // To support paging you will need to add ordering to the 'tbl_income_levels' query.
        [Query(IsDefault = true)]
        public IQueryable<tbl_income_levels> GetTbl_income_levels()
        {
            return this.ObjectContext.tbl_income_levels;
        }

        public void InsertTbl_income_levels(tbl_income_levels tbl_income_levels)
        {
            if ((tbl_income_levels.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(tbl_income_levels, EntityState.Added);
            }
            else
            {
                this.ObjectContext.tbl_income_levels.AddObject(tbl_income_levels);
            }
        }

        public void UpdateTbl_income_levels(tbl_income_levels currenttbl_income_levels)
        {
            this.ObjectContext.tbl_income_levels.AttachAsModified(currenttbl_income_levels, this.ChangeSet.GetOriginal(currenttbl_income_levels));
        }

        public void DeleteTbl_income_levels(tbl_income_levels tbl_income_levels)
        {
            if ((tbl_income_levels.EntityState == EntityState.Detached))
            {
                this.ObjectContext.tbl_income_levels.Attach(tbl_income_levels);
            }
            this.ObjectContext.tbl_income_levels.DeleteObject(tbl_income_levels);
        }

        // TODO:
        // Consider constraining the results of your query method.  If you need additional input you can
        // add parameters to this method or create additional query methods with different names.
        // To support paging you will need to add ordering to the 'tbl_indicators' query.
        [Query(IsDefault = true)]
        public IQueryable<tbl_indicators> GetTbl_indicators()
        {
            //Note: get data with LINQ
            return this.ObjectContext.tbl_indicators.Where(e => e.is_gotten == true);
        }

        public void InsertTbl_indicators(tbl_indicators tbl_indicators)
        {
            if ((tbl_indicators.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(tbl_indicators, EntityState.Added);
            }
            else
            {
                this.ObjectContext.tbl_indicators.AddObject(tbl_indicators);
            }
        }

        public void UpdateTbl_indicators(tbl_indicators currenttbl_indicators)
        {
            this.ObjectContext.tbl_indicators.AttachAsModified(currenttbl_indicators, this.ChangeSet.GetOriginal(currenttbl_indicators));
        }

        public void DeleteTbl_indicators(tbl_indicators tbl_indicators)
        {
            if ((tbl_indicators.EntityState == EntityState.Detached))
            {
                this.ObjectContext.tbl_indicators.Attach(tbl_indicators);
            }
            this.ObjectContext.tbl_indicators.DeleteObject(tbl_indicators);
        }

        // TODO:
        // Consider constraining the results of your query method.  If you need additional input you can
        // add parameters to this method or create additional query methods with different names.
        // To support paging you will need to add ordering to the 'tbl_lending_types' query.
        [Query(IsDefault = true)]
        public IQueryable<tbl_lending_types> GetTbl_lending_types()
        {
            return this.ObjectContext.tbl_lending_types;
        }

        public void InsertTbl_lending_types(tbl_lending_types tbl_lending_types)
        {
            if ((tbl_lending_types.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(tbl_lending_types, EntityState.Added);
            }
            else
            {
                this.ObjectContext.tbl_lending_types.AddObject(tbl_lending_types);
            }
        }

        public void UpdateTbl_lending_types(tbl_lending_types currenttbl_lending_types)
        {
            this.ObjectContext.tbl_lending_types.AttachAsModified(currenttbl_lending_types, this.ChangeSet.GetOriginal(currenttbl_lending_types));
        }

        public void DeleteTbl_lending_types(tbl_lending_types tbl_lending_types)
        {
            if ((tbl_lending_types.EntityState == EntityState.Detached))
            {
                this.ObjectContext.tbl_lending_types.Attach(tbl_lending_types);
            }
            this.ObjectContext.tbl_lending_types.DeleteObject(tbl_lending_types);
        }

        // TODO:
        // Consider constraining the results of your query method.  If you need additional input you can
        // add parameters to this method or create additional query methods with different names.
        // To support paging you will need to add ordering to the 'tbl_regions' query.
        [Query(IsDefault = true)]
        public IQueryable<tbl_regions> GetTbl_regions()
        {
            return this.ObjectContext.tbl_regions;
        }

        public void InsertTbl_regions(tbl_regions tbl_regions)
        {
            if ((tbl_regions.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(tbl_regions, EntityState.Added);
            }
            else
            {
                this.ObjectContext.tbl_regions.AddObject(tbl_regions);
            }
        }

        public void UpdateTbl_regions(tbl_regions currenttbl_regions)
        {
            this.ObjectContext.tbl_regions.AttachAsModified(currenttbl_regions, this.ChangeSet.GetOriginal(currenttbl_regions));
        }

        public void DeleteTbl_regions(tbl_regions tbl_regions)
        {
            if ((tbl_regions.EntityState == EntityState.Detached))
            {
                this.ObjectContext.tbl_regions.Attach(tbl_regions);
            }
            this.ObjectContext.tbl_regions.DeleteObject(tbl_regions);
        }

        // TODO:
        // Consider constraining the results of your query method.  If you need additional input you can
        // add parameters to this method or create additional query methods with different names.
        // To support paging you will need to add ordering to the 'tbl_trades' query.
        [Query(IsDefault = true)]
        public IQueryable<tbl_trades> GetTbl_trades()
        {
            return this.ObjectContext.tbl_trades;
        }

        public void InsertTbl_trades(tbl_trades tbl_trades)
        {
            if ((tbl_trades.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(tbl_trades, EntityState.Added);
            }
            else
            {
                this.ObjectContext.tbl_trades.AddObject(tbl_trades);
            }
        }

        public void UpdateTbl_trades(tbl_trades currenttbl_trades)
        {
            this.ObjectContext.tbl_trades.AttachAsModified(currenttbl_trades, this.ChangeSet.GetOriginal(currenttbl_trades));
        }

        public void DeleteTbl_trades(tbl_trades tbl_trades)
        {
            if ((tbl_trades.EntityState == EntityState.Detached))
            {
                this.ObjectContext.tbl_trades.Attach(tbl_trades);
            }
            this.ObjectContext.tbl_trades.DeleteObject(tbl_trades);
        }
    }
}


