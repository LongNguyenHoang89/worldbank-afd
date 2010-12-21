using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WbTest
{
    public class TradeEntry
    {
        public int country_from_id { get; set; }

        public int country_to_id { get; set; }

        public double import_value { get; set; }

        public double export_value { get; set; }

        public int trade_year { get; set; }
    }
}
