using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RollUpApi.Models
{
    public class ProjectCapabilities
    {
        public class ProcessTemplate
        {
            public string templateName { get; set; }
            public string templateTypeId { get; set; }
        }



        public class Capabilities
        {
            public ProcessTemplate processTemplate { get; set; }
        }


        public class DefaultTeam
        {
            public string id { get; set; }
            public string name { get; set; }
            public string url { get; set; }
        }

        public class Capability
        {
            public string id { get; set; }
            public string name { get; set; }
            public string url { get; set; }
            public string state { get; set; }
            public Capabilities capabilities { get; set; }
            public int revision { get; set; }
            public string visibility { get; set; }
            public DefaultTeam defaultTeam { get; set; }
        }


    }
}