using Newtonsoft.Json;
using RollUpApi.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using VstsConnector;
using System.Net.Mail;


namespace RollUpApi.Controllers
{
    public class RollUpController : ApiController
    {
        private string logPath;
        private string logFileName;

        [HttpPost]
        [Route("api/Rollup")]
        public void Rollupworkitem(object requestJson)
        {
            var re = Request;
            var headers = re.Headers;

            int[] parentWorkItems = new int[4] { 0, 0, 0, 0 };

            Stopwatch watch = new Stopwatch();
            watch.Start();
            string eventOccured = string.Empty;
            string workItemType = string.Empty;

            WorkItem objWi = new WorkItem();
            RollUpMethods objMethods = new RollUpMethods();
            string URL = string.Empty;
            double[] OldValue = new double[3] { 0, 0, 0 };
            double[] NewValue = new double[3] { 0, 0, 0 };
            string credentials = headers.Authorization.Parameter;

            try
            {
                //logPath = System.Web.HttpContext.Current.Server.MapPath("~/ApiLog");
                //logFileName = logPath + "\\RollUp_TestData" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
                //LogData("Vsts call has been made at:" + DateTime.Now + Environment.NewLine + Environment.NewLine + headers + Environment.NewLine + requestJson.ToString());

                Event.WorkItem objEvent = JsonConvert.DeserializeObject<Event.WorkItem>(requestJson.ToString());

                RollUpWorkItem.WorkItem workItemChanged = new RollUpWorkItem.WorkItem();
                RollUpWiCreate.CreatedWI workItemCreated = new RollUpWiCreate.CreatedWI();
                eventOccured = objEvent.eventType;

                if (eventOccured == "workitem.created" || eventOccured == "workitem.restored")
                {
                    workItemCreated = JsonConvert.DeserializeObject<RollUpWiCreate.CreatedWI>(requestJson.ToString());
                    workItemType = workItemCreated.resource.fields.SystemWorkItemType;
                }
                else if (eventOccured == "workitem.updated")
                {
                    workItemChanged = JsonConvert.DeserializeObject<RollUpWorkItem.WorkItem>(requestJson.ToString());
                    workItemType = workItemChanged.resource.revision.fields.WorkItemType;
                }

                //Delete event for Task, useCase, Requirement[Update event for UseCase,Requirement,Feature]
                if (workItemType == "Epic" || workItemType == "Feature" || workItemType == "Requirement" || workItemType == "User Story" || workItemType == "Task" || workItemType == "Product Backlog Item" || workItemType == "Bug" && (eventOccured == "workitem.updated"))
                {
                    if (workItemChanged.resource != null)
                    {
                        if (workItemChanged.resource.relations != null)
                        {
                            if (workItemChanged.resource.relations.removed != null && workItemChanged.resource.relations.removed.Count > 0)
                            {
                                if (workItemChanged.resource.relations.removed.FirstOrDefault().rel == "System.LinkTypes.Hierarchy-Forward")
                                {
                                    URL = workItemChanged.resource.relations.removed.FirstOrDefault().url;
                                    int _apisIndex = URL.IndexOf("_apis");
                                    URL = URL.Substring(0, _apisIndex);

                                    int deletedWiId = Convert.ToInt32(workItemChanged.resource.relations.removed.FirstOrDefault().url.Split('/').Last());
                                    bool wIisDeleted = objWi.isWorkItemDeleted(deletedWiId, URL, "3.0", credentials);
                                    if (wIisDeleted)
                                    {
                                        int firstParent = workItemChanged.resource.workItemId;
                                        parentWorkItems = objMethods.GetParentIds(firstParent, credentials, URL);
                                        objMethods.updateFields(parentWorkItems, URL, credentials);

                                        //logFileName = logPath + "\\RollUp_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
                                        //LogData("Vsts call has been made at:" + DateTime.Now + Environment.NewLine + Environment.NewLine + headers + Environment.NewLine + "Time Taken: " + watch.Elapsed.TotalSeconds.ToString() + Environment.NewLine + requestJson.ToString());
                                        watch.Stop();
                                        return;
                                    }
                                }
                            }
                        }
                    }
                }
                //Restore event for UseCase and Requirement
                if ((workItemType == "Epic" || workItemType == "Feature" || workItemType == "Requirement" || workItemType == "User Story" || workItemType == "Product Backlog Item") && eventOccured == "workitem.restored")
                {

                    URL = workItemCreated.resource.relations.FirstOrDefault().url;
                    int _apisIndex = URL.IndexOf("_apis");
                    URL = URL.Substring(0, _apisIndex);

                    parentWorkItems[0] = workItemCreated.resource.id;

                    if (parentWorkItems[0] > 0)
                    {
                        parentWorkItems = objMethods.GetParentIds(parentWorkItems[0], credentials, URL);
                        objMethods.updateFields(parentWorkItems, URL, credentials);
                        //logFileName = logPath + "\\RollUp_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
                        //LogData("Vsts call has been made at:" + DateTime.Now + Environment.NewLine + Environment.NewLine + headers + Environment.NewLine + "Time Taken: " + watch.Elapsed.TotalSeconds.ToString() + Environment.NewLine + requestJson.ToString());
                        watch.Stop();
                        return;
                    }

                }

                //All events for Task and VTO   
                if (workItemType == "Epic" || workItemType == "Feature" || workItemType == "Requirement" || workItemType == "User Story" || workItemType == "Product Backlog Item" || workItemType == "Task" || workItemType == "Bug")
                {
                    //When Task link change(Re-parenting)
                    if (eventOccured == "workitem.updated" && workItemChanged.resource.fields == null)
                    {
                        if (workItemChanged.resource.relations.removed != null && workItemChanged.resource.relations.removed.Count > 0)
                        {
                            URL = workItemChanged.resource.relations.removed.FirstOrDefault().url;
                            int _apisIndex = URL.IndexOf("_apis");
                            URL = URL.Substring(0, _apisIndex);

                            foreach (var removed in workItemChanged.resource.relations.removed)
                            {
                                if (removed.rel == "System.LinkTypes.Hierarchy-Reverse")
                                {
                                    int firstParent = Convert.ToInt32(removed.url.Split('/').Last());
                                    parentWorkItems = objMethods.GetParentIds(firstParent, credentials, URL);
                                    objMethods.updateFields(parentWorkItems, URL, credentials);
                                }
                                if (removed.rel == "System.LinkTypes.Hierarchy-Forward")
                                {
                                    int firstParent = workItemChanged.resource.workItemId;
                                    parentWorkItems = objMethods.GetParentIds(firstParent, credentials, URL);
                                    objMethods.updateFields(parentWorkItems, URL, credentials);
                                }
                            }
                        }
                        if (workItemChanged.resource.relations.added != null && workItemChanged.resource.relations.added.Count > 0)
                        {
                            parentWorkItems = new int[4] { 0, 0, 0, 0 };

                            URL = workItemChanged.resource.relations.added.FirstOrDefault().url;
                            int _apisIndex = URL.IndexOf("_apis");
                            URL = URL.Substring(0, _apisIndex);

                            foreach (var added in workItemChanged.resource.relations.added)
                            {
                                if (added.rel == "System.LinkTypes.Hierarchy-Reverse")
                                {
                                    int firstParent = Convert.ToInt32(added.url.Split('/').Last());
                                    parentWorkItems = objMethods.GetParentIds(firstParent, credentials, URL);
                                    objMethods.updateFields(parentWorkItems, URL, credentials);
                                }
                                if (added.rel == "System.LinkTypes.Hierarchy-Forward")
                                {
                                    int firstParent = workItemChanged.resource.workItemId;
                                    parentWorkItems = objMethods.GetParentIds(firstParent, credentials, URL);
                                    objMethods.updateFields(parentWorkItems, URL, credentials);
                                }
                            }
                        }
                        if (!Directory.Exists(logPath)) //Create the log folder if it does not exist
                        {
                            Directory.CreateDirectory(logPath);
                        }
                        //logFileName = logPath + "\\RollUp_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
                        //LogData("Vsts call has been made at:" + DateTime.Now + Environment.NewLine + Environment.NewLine + headers + Environment.NewLine + "Time Taken: " + watch.Elapsed.TotalSeconds.ToString() + Environment.NewLine + requestJson.ToString());
                        //watch.Stop();
                        return;
                    }
                    if (eventOccured == "workitem.updated")
                    {
                        if (workItemChanged.resource.fields.OriginalEstimate == null && workItemChanged.resource.fields.RemainingWork == null && workItemChanged.resource.fields.CompletedWork == null)
                        {
                            return;
                        }

                        URL = workItemChanged.resource.revision.relations.FirstOrDefault().url;
                        int _apisIndex = URL.IndexOf("_apis");
                        URL = URL.Substring(0, _apisIndex);

                        if (workItemChanged.resource.revision.relations != null)
                        {
                            foreach (var rel in workItemChanged.resource.revision.relations)
                            {
                                if (rel.rel == "System.LinkTypes.Hierarchy-Reverse")
                                {
                                    parentWorkItems[0] = Convert.ToInt32(rel.url.Split('/').Last());
                                }
                            }
                        }
                        int parentId = parentWorkItems[0];
                        workItemChanged = objMethods.GetoldNewvalue(requestJson.ToString());
                        if (workItemChanged.resource.fields.OriginalEstimate != null)
                        {
                            OldValue[0] = workItemChanged.resource.fields.OriginalEstimate.oldValue;
                            NewValue[0] = workItemChanged.resource.fields.OriginalEstimate.newValue;
                        }
                        if (workItemChanged.resource.fields.RemainingWork != null)
                        {
                            OldValue[1] = workItemChanged.resource.fields.RemainingWork.oldValue;
                            NewValue[1] = workItemChanged.resource.fields.RemainingWork.newValue;
                        }

                        if (workItemChanged.resource.fields.CompletedWork != null)
                        {
                            OldValue[2] = workItemChanged.resource.fields.CompletedWork.oldValue;
                            NewValue[2] = workItemChanged.resource.fields.CompletedWork.newValue;
                        }
                        double[] ComparedVal = objMethods.CompareValue(OldValue, NewValue, parentId, credentials, URL);
                        bool isUpdated = objMethods.UpdateNewval(parentId, ComparedVal, credentials, URL);

                        //logFileName = logPath + "\\RollUp_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
                        //LogData("Vsts call has been made at:" + DateTime.Now + Environment.NewLine + Environment.NewLine + headers + Environment.NewLine + "Time Taken: " + watch.Elapsed.TotalSeconds.ToString() + Environment.NewLine + requestJson.ToString());
                        watch.Stop();
                        return;

                    }

                    else if (eventOccured == "workitem.created" || eventOccured == "workitem.restored")
                    {
                        URL = workItemCreated.resource.relations.FirstOrDefault().url;
                        int _apisIndex = URL.IndexOf("_apis");
                        URL = URL.Substring(0, _apisIndex);
                        if (workItemCreated.resource.relations != null)
                        {
                            foreach (var rel in workItemCreated.resource.relations)
                            {
                                if (rel.rel == "System.LinkTypes.Hierarchy-Reverse")
                                {
                                    parentWorkItems[0] = Convert.ToInt32(rel.url.Split('/').Last());
                                }
                            }
                        }
                        if (parentWorkItems[0] > 0)
                        {
                            parentWorkItems = objMethods.GetParentIds(parentWorkItems[0], credentials, URL);
                            bool isUpdated = objMethods.updateFields(parentWorkItems, URL, credentials);
                        }

                    }
                    //Log message
                    if (!Directory.Exists(logPath)) //Create the log folder if it does not exist
                    {
                        Directory.CreateDirectory(logPath);
                    }
                    //logFileName = logPath + "\\RollUp_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
                  //  LogData("Vsts call has been made at:" + DateTime.Now + Environment.NewLine + Environment.NewLine + headers + Environment.NewLine + "Time Taken: " + watch.Elapsed.TotalSeconds.ToString() + Environment.NewLine + requestJson.ToString());
                    watch.Stop();
                    return;
                }
            }
            catch (Exception ex)
            {
                logPath = System.Web.HttpContext.Current.Server.MapPath("~/ApiLog/RollUpErrors");
                logFileName = logPath + "\\ERROR_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
                LogData("Vsts call has been made at:" + DateTime.Now + Environment.NewLine + Environment.NewLine + headers + Environment.NewLine + "Time Taken: " + watch.Elapsed.TotalSeconds.ToString() + Environment.NewLine + "Error has been occured :" + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace);
                watch.Stop();
            }
        }
        private void LogData(string message)
        {
            //File.Create(logFileName);
            System.IO.File.AppendAllText(logFileName, message);
        }
    }
}