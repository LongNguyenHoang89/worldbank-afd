using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WbTest
{
    public class IndicatorEntry
    {
        public int indicator_id_pk { get; set; }

        public string indicator_code { get; set; }

        public string indicator_name { get; set; }

        public string indicator_description { get; set; }

        public string indicator_unit { get; set; }

        public bool is_yearly { get; set; }

        public int last_update { get; set; }

        public bool is_gotten { get; set; }
    }
}
