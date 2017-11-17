using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Access.Models
{
    public class DashboardList
    {
        public class DashboardEntry
        {
            public string id { get; set; }
            public string name { get; set; }
            public int refreshInterval { get; set; }
            public int position { get; set; }
            public string url { get; set; }
        }

        public class Self
        {
            public string href { get; set; }
        }

        public class Dashboard
        {
            public string href { get; set; }
        }

        public class Links
        {
            public Self self { get; set; }
            public IList<Dashboard> dashboard { get; set; }
        }

        public class List
        {
            public IList<DashboardEntry> dashboardEntries { get; set; }
            public string permission { get; set; }
            public string url { get; set; }
            public Links _links { get; set; }
        }


    }
}