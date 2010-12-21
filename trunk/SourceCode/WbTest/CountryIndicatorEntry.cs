using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WbTest
{
    public class CountryIndicatorEntry
    {
        public int country_indicator_id_pk { get; set; }

        public int country_indicator_year { get; set; }

        public int country_id { get; set; }

        public int indicator_id { get; set; }

        public double country_indicator_value { get; set; }
    }
}
