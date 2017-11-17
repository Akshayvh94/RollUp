using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Access.Models
{
    public class ProjectProp
    {
        public class ProcessTemplate
        {
            public string templateName { get; set; }
            public string templateTypeId { get; set; }
        }

        public class Versioncontrol
        {
            public string sourceControlType { get; set; }
            public string gitEnabled { get; set; }
            public string tfvcEnabled { get; set; }
        }

        public class Capabilities
        {
            public ProcessTemplate processTemplate { get; set; }
            public Versioncontrol versioncontrol { get; set; }
        }

        public class Self
        {
            public string href { get; set; }
        }

        public class Collection
        {
            public string href { get; set; }
        }

        public class Web
        {
            public string href { get; set; }
        }

        public class Links
        {
            public Self self { get; set; }
            public Collection collection { get; set; }
            public Web web { get; set; }
        }

        public class DefaultTeam
        {
            public string id { get; set; }
            public string name { get; set; }
            public string url { get; set; }
        }

        public class Prop
        {
            public string id { get; set; }
            public string name { get; set; }
            public string url { get; set; }
            public string state { get; set; }
            public Capabilities capabilities { get; set; }
            public int revision { get; set; }
            public Links _links { get; set; }
            public string visibility { get; set; }
            public DefaultTeam defaultTeam { get; set; }
        }


    }
}