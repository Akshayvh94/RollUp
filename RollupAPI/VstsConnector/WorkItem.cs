using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using VstsConnector.Models;

namespace VstsConnector
{
    public class WorkItem
    {
        public double[] GetTotalRollUpValues(string workItems, string credentials, string url, string version)
        {
            double[] result = new double[3];
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);
                HttpResponseMessage finalResponse = client.GetAsync(url + "DefaultCollection/_apis/wit/WorkItems?ids=" + workItems + "&fields=System.Id,System.WorkItemType,Microsoft.VSTS.Scheduling.RemainingWork,Microsoft.VSTS.Scheduling.OriginalEstimate,Microsoft.VSTS.Scheduling.CompletedWork&api-version=" + version + "").Result;
                if (finalResponse.IsSuccessStatusCode)
                {
                    string resFinal = finalResponse.Content.ReadAsStringAsync().Result;
                    WorkItemResponse.WorkItems finalWIs = JsonConvert.DeserializeObject<WorkItemResponse.WorkItems>(resFinal);
                    if (finalWIs.count > 0)
                    {
                        foreach (var WIs in finalWIs.value)
                        {
                            result[0] = result[0] + WIs.fields.MicrosoftVSTSSchedulingOriginalEstimate;
                            result[1] = result[1] + WIs.fields.MicrosoftVSTSSchedulingCompletedWork;
                            result[2] = result[2] + WIs.fields.MicrosoftVSTSSchedulingRemainingWork;
                        }
                    }
                }
            }
            return result;
        }
        public WorkItemResponse.WorkItems GetWorkItemsDetailinBatch(int id, string credentials, string url, string version)
        {

            WorkItemResponse.WorkItems viewModel = new WorkItemResponse.WorkItems();

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);

                HttpResponseMessage response = client.GetAsync(url + "DefaultCollection/_apis/wit/workitems?api-version=" + version + "&ids=" + id + "&$expand=relations").Result;
                if (response.IsSuccessStatusCode)
                {
                    string res = response.Content.ReadAsStringAsync().Result;
                    viewModel = Newtonsoft.Json.JsonConvert.DeserializeObject<WorkItemResponse.WorkItems>(res);
                }

                return viewModel;
            }
        }
        public bool UpdateWorkItem(int WItoUpdate, object[] patchWorkItem, string credentials, string URl, string version)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);

                // serialize the fields array into a json string          
                var patchValue = new StringContent(JsonConvert.SerializeObject(patchWorkItem), Encoding.UTF8, "application/json-patch+json"); // mediaType needs to be application/json-patch+json for a patch call

                var method = new HttpMethod("PATCH");
                var request = new HttpRequestMessage(method, URl + "DefaultCollection/_apis/wit/workitems/" + WItoUpdate + "?bypassRules=true&api-version=" + version + "") { Content = patchValue };
                var response = client.SendAsync(request).Result;

                return response.IsSuccessStatusCode;
            }
        }
        public bool isWorkItemDeleted(int workItemId, string URL, string version, string credentials)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);

                HttpResponseMessage response = client.GetAsync(URL + "DefaultCollection/_apis/wit/recyclebin/" + workItemId + "?api-version=" + version + "-preview").Result;
                if (response.IsSuccessStatusCode)
                {
                    return true;

                }

                return false;
            }
        }

        
    }
}
