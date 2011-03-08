using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WbTest
{
    public class ProjectEntry
    {
        public int project_id_pk { get; set; }

        public int country_id { get; set; }
        
        public string project_link {get;set;}

        public string project_approval_date{get;set;}

        public string project_closing_date{get;set;}

        public string project_status{get;set;}

        public string project_cost{get;set;}

        public string project_region{get;set;}

        public string project_major_sector{get;set;}

        public string project_themes{get;set;}

        public string project_borrower{get;set;}

        public string project_implement_agency{get;set;}

        public string project_outcome{get;set;}        

        public string project_wb_id { get; set; }

        public string project_name { get; set; }
    }
}
