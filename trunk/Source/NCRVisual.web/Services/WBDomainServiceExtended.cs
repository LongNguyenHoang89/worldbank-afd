using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel.DomainServices.EntityFramework;
using NCRVisual.web.DataModel;
using System.Data;

namespace NCRVisual.web.Services
{
    public partial class WBDomainService : LinqToEntitiesDomainService<WBEntities>
    {
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

    }
}