using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VstsConnector.Models;
using VstsConnector;
using System.Net.Http;
using Newtonsoft.Json;

namespace RollUpApi.Models
{
    public class RollUpMethods
    {
        public bool updateFields(int[] parentWorkItems, string URL, string credentials)
        {
            double[] result = new double[4];
            WorkItem objWi = new WorkItem();

            foreach (int id in parentWorkItems)
            {
                if (id > 0)
                {
                    string childWorkItems = string.Empty;
                    WorkItemResponse.WorkItems parentResponse = objWi.GetWorkItemsDetailinBatch(id, credentials, URL, "2.2");
                    if (parentResponse.count > 0)
                    {
                        if (parentResponse.value.FirstOrDefault().relations != null)
                        {
                            foreach (var relation in parentResponse.value.FirstOrDefault().relations)
                            {
                                if (relation.rel == "System.LinkTypes.Hierarchy-Forward")
                                {
                                    childWorkItems = childWorkItems + relation.url.Split('/').Last() + ",";
                                }
                            }
                            childWorkItems = childWorkItems.TrimEnd(',');

                            result = objWi.GetTotalRollUpValues(childWorkItems, credentials, URL, "1.0");
                        }
                    }
                }
                if (result != null)
                {
                    double OriginalEstimate = Math.Round(result[0], 2);
                    double CompletedWork = Math.Round(result[1], 2);
                    double RemainingWork = Math.Round(result[2], 2);

                    Object[] patchDocument = new Object[3];

                    patchDocument[0] = new { op = "add", path = "/fields/Microsoft.VSTS.Scheduling.OriginalEstimate", value = OriginalEstimate };
                    patchDocument[1] = new { op = "add", path = "/fields/Microsoft.VSTS.Scheduling.CompletedWork", value = CompletedWork };
                    patchDocument[2] = new { op = "add", path = "/fields/Microsoft.VSTS.Scheduling.RemainingWork", value = RemainingWork };

                    bool isUpdated = objWi.UpdateWorkItem(id, patchDocument, credentials, URL, "2.2");
                    result = new double[3];
                }
            }
            return true;
        }

        public int[] GetParentIds(int firstParentId, string credentials, string URL)
        {
            WorkItem objWi = new WorkItem();

            int[] parentWorkItems = new int[4] { 0, 0, 0, 0 };
            parentWorkItems[0] = firstParentId;

            if (parentWorkItems[0] > 0)
            {
                WorkItemResponse.WorkItems parent1Response = objWi.GetWorkItemsDetailinBatch(parentWorkItems[0], credentials, URL, "2.2");
                if (parent1Response.value.FirstOrDefault().relations != null)
                {
                    foreach (var relation1 in parent1Response.value.FirstOrDefault().relations)
                    {
                        if (relation1.rel == "System.LinkTypes.Hierarchy-Reverse")
                        {
                            parentWorkItems[1] = Convert.ToInt32(relation1.url.Split('/').Last());
                        }
                    }
                }
                if (parentWorkItems[1] > 0)
                {
                    WorkItemResponse.WorkItems parent2Response = objWi.GetWorkItemsDetailinBatch(parentWorkItems[1], credentials, URL, "2.2");
                    if (parent2Response.value.FirstOrDefault().relations != null)
                    {
                        foreach (var relation2 in parent2Response.value.FirstOrDefault().relations)
                        {
                            if (relation2.rel == "System.LinkTypes.Hierarchy-Reverse")
                            {
                                parentWorkItems[2] = Convert.ToInt32(relation2.url.Split('/').Last());
                            }
                        }
                    }
                }
                if (parentWorkItems[2] > 0)
                {
                    WorkItemResponse.WorkItems parent3Response = objWi.GetWorkItemsDetailinBatch(parentWorkItems[2], credentials, URL, "2.2");
                    if (parent3Response.value.FirstOrDefault().relations != null)
                    {
                        foreach (var relation3 in parent3Response.value.FirstOrDefault().relations)
                        {
                            if (relation3.rel == "System.LinkTypes.Hierarchy-Reverse")
                            {
                                parentWorkItems[3] = Convert.ToInt32(relation3.url.Split('/').Last());
                            }
                        }
                    }
                }
            }
            return parentWorkItems;
        }

        //Fields Calculation

        public FieldValues.FieldList GetThreeFields(int WitID, string credentials, string URL)
        {
            FieldValues.FieldList vals = new FieldValues.FieldList();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(URL);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", credentials);
                HttpResponseMessage response = client.GetAsync("DefaultCollection/_apis/wit/workitems?ids=" + WitID + "&fields=Microsoft.VSTS.Scheduling.RemainingWork,Microsoft.VSTS.Scheduling.OriginalEstimate,Microsoft.VSTS.Scheduling.CompletedWork&api-version=1.0").Result;
                if (response.IsSuccessStatusCode)
                {
                    string res = response.Content.ReadAsStringAsync().Result;
                    vals = JsonConvert.DeserializeObject<FieldValues.FieldList>(res);
                    return vals;
                }
                else
                {
                    return new FieldValues.FieldList();
                }
            }
        }

        public ProjectCapabilities.Capability GetProjectCapability(string projectname, string credentials, string URL)
        {
            ProjectCapabilities.Capability capobj = new ProjectCapabilities.Capability();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(URL);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", credentials);
                HttpResponseMessage response = client.GetAsync("DefaultCollection/_apis/projects/" + projectname + "?includeCapabilities=true&api-version=1.0").Result;
                if (response.IsSuccessStatusCode)
                {
                    string res = response.Content.ReadAsStringAsync().Result;
                    capobj = JsonConvert.DeserializeObject<ProjectCapabilities.Capability>(res);
                    return capobj;
                }
                else
                {
                    return new ProjectCapabilities.Capability();
                }
            }
        }

        public double[] CalculateFields(FieldValues.FieldList vals, string processtemplate)
        {
            double[] values = new double[3];
            values[0] = vals.value.FirstOrDefault().fields.OriginalEstimate;
            values[1] = vals.value.FirstOrDefault().fields.CompletedWork;
            values[2] = vals.value.FirstOrDefault().fields.RemainingWork;

            if ((values[2] + values[1]) == values[0])
            {
                return values;
            }
            else if (values[2] == 0 && values[1] == 0 && values[0] == 0)
            {
                return values;
            }
            //else if (values[0] == 0 && values[1] == 0 && processtemplate == "Scrum")
            //{
            //    return values;
            //}
            else if (values[0] == 0 && values[1] == 0)
            {
                return values;
            }
            else if (values[2] == 0 && values[1] == 0)
            {
                values[2] = values[0];
                values[1] = values[0] - values[2];
                return values;
            }
            else
            {
                values[2] = values[0] - values[1];
                return values;
            }
        }

        public bool UpdateAWitFields(double[] vals, string credentials, string URL, int id)
        {
            double[] result = new double[4];
            WorkItem objWi = new WorkItem();

            double OriginalEstimate = Math.Round(vals[0], 2);
            double CompletedWork = Math.Round(vals[1], 2);
            double RemainingWork = Math.Round(vals[2], 2);

            Object[] patchDocument = new Object[3];

            patchDocument[0] = new { op = "add", path = "/fields/Microsoft.VSTS.Scheduling.OriginalEstimate", value = OriginalEstimate };
            patchDocument[1] = new { op = "add", path = "/fields/Microsoft.VSTS.Scheduling.CompletedWork", value = CompletedWork };
            patchDocument[2] = new { op = "add", path = "/fields/Microsoft.VSTS.Scheduling.RemainingWork", value = RemainingWork };

            bool isUpdated = objWi.UpdateWorkItem(id, patchDocument, credentials, URL, "2.2");
            result = new double[3];
            return true;
        }
    }
}