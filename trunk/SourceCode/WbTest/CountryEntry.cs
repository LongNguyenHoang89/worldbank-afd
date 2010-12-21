using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace WbWCF.Contract.Data
{    
    public class CountryEntry
    {
        
        public int country_id_pk { get; set; }

        
        public string country_iso_code { get; set; }

        
        public string country_name { get; set; }

        
        public string region_id { get; set; }

        
        public string income_level_id { get; set; }

        
        public string lending_type_id { get; set; }

        
        public double country_longitude { get; set; }

        
        public double country_latitude { get; set; }

        
        public bool is_region { get; set; }


        public int country_code { get; set; }

    }
}