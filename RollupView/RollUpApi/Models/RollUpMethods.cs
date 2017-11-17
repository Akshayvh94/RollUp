using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VstsConnector.Models;
using VstsConnector;
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

        public RollUpWorkItem.WorkItem GetoldNewvalue(string requestJson)
        {
            RollUpWorkItem.WorkItem fields = new RollUpWorkItem.WorkItem();
            fields = JsonConvert.DeserializeObject<RollUpWorkItem.WorkItem>(requestJson.ToString());
            return fields;
        }

        public double[] CompareValue(double[] OldVal, double[] NewVal, int PID, string credentials, string URL)
        {
            WorkItem data = new WorkItem();
            string parentid = Convert.ToString(PID);
            double[] UpdatedVal = new double[3] { 0, 0, 0 };
            double originalEstimate;
            double RemainingWork;
            double completedWork;

            double[] ParentVals = data.GetNewRollUpValues(parentid, credentials, URL, "2.2");
            if (ParentVals[0] != 0)
            {
                originalEstimate = ParentVals[0] + ((-1) * (OldVal[0] - NewVal[0]));
            }
            else
            {
                originalEstimate = NewVal[0];
            }
            if (ParentVals[1] != 0)
            {
                RemainingWork = ParentVals[1] + ((-1) * (OldVal[1] - NewVal[1]));
            }
            else
            {
                RemainingWork = NewVal[1];
            }
            if (ParentVals[2] != 0)
            {
                completedWork = ParentVals[2] + ((-1) * (OldVal[2] - NewVal[2]));
            }
            else
            {
                completedWork = NewVal[2];
            }

            UpdatedVal[0] = originalEstimate;
            UpdatedVal[1] = RemainingWork;
            UpdatedVal[2] = completedWork;

            return UpdatedVal;
        }
        public bool UpdateNewval(int PID, double[] UpdatedVals, string credentials, string URL)
        {
            WorkItem data = new WorkItem();
            double OriginalEstimate = Math.Round(UpdatedVals[0], 2);
            double RemainingWork = Math.Round(UpdatedVals[1], 2);
            double CompletedWork = Math.Round(UpdatedVals[2], 2);

            Object[] patchDocument = new Object[3];

            patchDocument[0] = new { op = "add", path = "/fields/Microsoft.VSTS.Scheduling.OriginalEstimate", value = OriginalEstimate };
            patchDocument[1] = new { op = "add", path = "/fields/Microsoft.VSTS.Scheduling.CompletedWork", value = CompletedWork };
            patchDocument[2] = new { op = "add", path = "/fields/Microsoft.VSTS.Scheduling.RemainingWork", value = RemainingWork };

            bool isUpdated = data.UpdateWorkItem(PID, patchDocument, credentials, URL, "2.2");
            UpdatedVals = new double[3];
            return isUpdated;
        }

        
    }
}