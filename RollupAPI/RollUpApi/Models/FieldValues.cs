using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RollUpApi.Models
{
    public class FieldValues
    {
        public class Fields
        {
            [JsonProperty(PropertyName = "Microsoft.VSTS.Scheduling.RemainingWork")]
            public double RemainingWork { get; set; }
            [JsonProperty(PropertyName = "Microsoft.VSTS.Scheduling.OriginalEstimate")]
            public double OriginalEstimate { get; set; }
            [JsonProperty(PropertyName = "Microsoft.VSTS.Scheduling.CompletedWork")]
            public double CompletedWork { get; set; }
        }

        public class Value
        {
            public int id { get; set; }
            public int rev { get; set; }
            public Fields fields { get; set; }
            public string url { get; set; }
        }

        public class FieldList
        {
            public int count { get; set; }
            public IList<Value> value { get; set; }
        }


    }
}