using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Access.Models
{
    public class GetDashboardByID
    {

        public class Widget
        {
            public string id { get; set; }
            public string eTag { get; set; }
            public string name { get; set; }
            public string settings { get; set; }
            public string artifactId { get; set; }
            public string url { get; set; }
            public bool isEnabled { get; set; }
            public object contentUri { get; set; }
            public string contributionId { get; set; }
            public string typeId { get; set; }
            public string configurationContributionId { get; set; }
            public string configurationContributionRelativeId { get; set; }
            public bool isNameConfigurable { get; set; }
            public string loadingImageUrl { get; set; }
        }

        public class Dashboard
        {
            public string id { get; set; }
            public string name { get; set; }
            public string eTag { get; set; }
            public IList<Widget> widgets { get; set; }
        }
    }
}