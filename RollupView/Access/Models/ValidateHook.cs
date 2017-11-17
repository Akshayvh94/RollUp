using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Access.Models
{
    public class ValidateHook
    {
        public class CreatedBy
        {
            public string id { get; set; }
        }

        public class ModifiedBy
        {
            public string id { get; set; }
        }

        public class PublisherInputs
        {
            public string buildStatus { get; set; }
            public string definitionName { get; set; }
            public string hostId { get; set; }
            public string projectId { get; set; }
            public string tfsSubscriptionId { get; set; }
            public string branch { get; set; }
            public string repository { get; set; }
            public string path { get; set; }
            public string areaPath { get; set; }
            public string commentPattern { get; set; }
            public string workItemType { get; set; }
            public string changedFields { get; set; }
        }

        public class ConsumerInputs
        {
            public string feedId { get; set; }
            public string packageSourceId { get; set; }
            public string buildName { get; set; }
            public string serverBaseUrl { get; set; }
            public string username { get; set; }
            public string password { get; set; }
            public string url { get; set; }
            public string buildParameterized { get; set; }
            public string addToTop { get; set; }
            public string boardId { get; set; }
            public string listId { get; set; }
            public string userToken { get; set; }
            public string accountName { get; set; }
            public string apiToken { get; set; }
        }

        public class Value
        {
            public string id { get; set; }
            public string url { get; set; }
            public string publisherId { get; set; }
            public string eventType { get; set; }
            public string resourceVersion { get; set; }
            public string eventDescription { get; set; }
            public string consumerId { get; set; }
            public string consumerActionId { get; set; }
            public string actionDescription { get; set; }
            public CreatedBy createdBy { get; set; }
            public DateTime createdDate { get; set; }
            public ModifiedBy modifiedBy { get; set; }
            public DateTime modifiedDate { get; set; }
            public PublisherInputs publisherInputs { get; set; }
            public ConsumerInputs consumerInputs { get; set; }
            public string status { get; set; }
            public int? probationRetries { get; set; }
        }

        public class Hooks
        {
            public int count { get; set; }
            public string existmsg { get; set; }

            public IList<Value> value { get; set; }
        }


    }
}