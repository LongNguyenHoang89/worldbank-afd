using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Data;

namespace WbTest.DataAccess
{
    public static class Conversion
    {
        public static Collection<IndicatorEntry> ToIndicatorEntries(this DataRowCollection drc)
        {
            Collection<IndicatorEntry> indicators = new Collection<IndicatorEntry>();
            for (int i = 0; i < drc.Count; i++)
            {
                IndicatorEntry indicator = new IndicatorEntry
                {
                    indicator_id_pk=Int32.Parse(drc[i]["indicator_id_pk"].ToString()),
                    indicator_code = drc[i]["indicator_code"].ToString(),                    
                    indicator_name = drc[i]["indicator_name"].ToString()
                };
                indicators.Add(indicator);
            }
            return indicators;
        }
    }
}
