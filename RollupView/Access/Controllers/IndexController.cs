using Access.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Access.Controllers
{
    public class IndexController : Controller
    {
        // GET: Index
        //public ActionResult Index(Dashboard mod)
        //{
        //    AreaUpload.Authentication model = new AreaUpload.Authentication();
        //    model.accname = mod.accountName;
        //    model.pat = mod.accessToken;

        //    //if (Session["Username"] != null && Session["Password"] != null)
        //    //{
        //    return View(model);
        //    //}
        //    //return RedirectToAction("Login", "Access");
        //}
        //public ActionResult SelectProject(Dashboard mod)
        //{
        //    return View();
        //}
        //[HttpPost]

        private string logSucPath;
        private string Logerrpath;
        private string logFileName;


        public ActionResult Callback()
        {
            return View();
        }
        public ActionResult Index(Dashboard mod)
        {
            AreaUpload.Authentication model = new AreaUpload.Authentication();
            model.accname = mod.accountName;
            model.pat = mod.NewPat;
            mod.Message = string.Empty;
            bool isvalid = ValidateDetails(model);
            if (isvalid)
            {

                return RedirectToAction("CreateHooks", mod);

            }
            else
            {
                mod.Message = "Invalid Account Name or PAT";
                return View("Callback", mod);
            }
        }



        public ActionResult CreateHooks(Dashboard dash)
        {
            try
            {

                //logSucPath = Server.MapPath("~") + @"ViewLog\ViewSuccess";
                //logFileName = logSucPath + "//Test_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
                //LogData("Vsts call has been made at:" + DateTime.Now + Environment.NewLine + Environment.NewLine + "Acccount Name : " + dash.accountName + Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine + "Request has been made");


                AreaUpload.Authentication mod = new AreaUpload.Authentication();
                mod.accname = dash.accountName;
                mod.pat = dash.NewPat;

                AreaUpload.ProjectCount modal = new AreaUpload.ProjectCount();
                if (mod.accname != null && mod.pat != null)
                {

                    modal = GetprojectList(mod);
                    List<SelectListItem> projects = new List<SelectListItem>();
                    foreach (var project in modal.value)
                    {
                        projects.Add(new SelectListItem { Text = project.name, Value = project.id });
                    }
                    modal.ProjectList = new SelectList(projects.OrderBy(x => x.Text));
                    modal.AccName = dash.accountName;
                    modal.PAT = dash.NewPat;
                    modal.ErrList = dash.ErrList;
                    modal.SuccessMsg = dash.SuccessMsg;
                    modal.SelectedID = dash.SelectedID;
                    modal.accURL = " https://" + modal.AccName + ".visualstudio.com/" + modal.SelectedID + "/_apps/hub/ms.vss-servicehooks-web.manageServiceHooks-project";
                    return View(modal);
                }
                return RedirectToAction("Index", "Account");

            }
            catch (Exception ex)
            {
                string message = ex.Message.ToString();
                errors = errors + message + '*';
                Logerrpath = Server.MapPath("~") + @"ViewLog\ViewErrors";
                logFileName = Logerrpath + "\\Error_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
                LogData("Vsts call has been made at:" + DateTime.Now + Environment.NewLine + Environment.NewLine + "Acccount Name : " + dash.accountName + Environment.NewLine + Environment.NewLine + " Error has been occured :" + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace);

            }
            return RedirectToAction("Index", "Account");
        }
        string accname = string.Empty;
        string PAT = string.Empty;
        string URL = string.Empty;
        string projectName = "";
        string errors = string.Empty;

        public bool ValidateDetails(AreaUpload.Authentication auth)
        {
            AreaUpload.ProjectCount load = new AreaUpload.ProjectCount();
            accname = auth.accname;
            PAT = auth.pat;
            string URL = "https://" + accname + ".visualstudio.com/DefaultCollection/";
            string _credentials = Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", "", auth.pat)));
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(URL);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("appication/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _credentials);
                    HttpResponseMessage response = client.GetAsync("/_apis/projects?stateFilter=All&api-version=1.0").Result;
                    if (response.IsSuccessStatusCode)
                    {
                        string res = response.Content.ReadAsStringAsync().Result;
                        load = JsonConvert.DeserializeObject<AreaUpload.ProjectCount>(res);
                        return true;
                    }
                    else
                    {
                        var errMessage = response.Content.ReadAsStringAsync();
                        string error = GeterroMessage(errMessage.Result.ToString());
                        Logerrpath = Server.MapPath("~") + @"ViewLog\ViewErrors";
                        logFileName = Logerrpath + "\\Error_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
                        LogData("Vsts call has been made at:" + DateTime.Now + Environment.NewLine + Environment.NewLine + "Acccount Name : " + auth.accname + Environment.NewLine + Environment.NewLine + " Error has been occured :" + Environment.NewLine + Environment.NewLine + error);
                    }


                }
            }
            catch (Exception ex)
            {
                string message = ex.Message.ToString();
                errors = errors + message + '*';
                Logerrpath = Server.MapPath("~") + @"ViewLog\ViewErrors";
                logFileName = Logerrpath + "\\Error_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
                LogData("Vsts call has been made at:" + DateTime.Now + Environment.NewLine + Environment.NewLine + "Acccount Name : " + auth.accname + Environment.NewLine + Environment.NewLine + " Error has been occured :" + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace);

            }
            return false;

        }

        public AreaUpload.ProjectCount GetprojectList(AreaUpload.Authentication auth)
        {
            AreaUpload.ProjectCount load = new AreaUpload.ProjectCount();
            accname = auth.accname;
            PAT = auth.pat;
            URL = "https://" + accname + ".visualstudio.com/DefaultCollection/";
            //string _credentials = auth.pat;
            string _credentials = Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", "", auth.pat)));
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(URL);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("appication/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _credentials);
                    HttpResponseMessage response = client.GetAsync("/_apis/projects?stateFilter=WellFormed&api-version=1.0").Result;
                    if (response.IsSuccessStatusCode)
                    {
                        string res = response.Content.ReadAsStringAsync().Result;
                        load = JsonConvert.DeserializeObject<AreaUpload.ProjectCount>(res);
                    }
                    else
                    {
                        var errMessage = response.Content.ReadAsStringAsync();
                        string error = GeterroMessage(errMessage.Result.ToString());
                        Logerrpath = Server.MapPath("~") + @"ViewLog\ViewErrors";
                        logFileName = Logerrpath + "\\Error_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
                        LogData("Vsts call has been made at:" + DateTime.Now + Environment.NewLine + Environment.NewLine + "Acccount Name : " + auth.accname + Environment.NewLine + Environment.NewLine + " Error has been occured :" + Environment.NewLine + Environment.NewLine + error);
                    }

                }
            }
            catch (Exception ex)
            {
                string message = ex.Message.ToString();
                errors = errors + message + '*';
                Logerrpath = Server.MapPath("~") + @"ViewLog\ViewErrors";
                logFileName = Logerrpath + "\\Error_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
                LogData("Vsts call has been made at:" + DateTime.Now + Environment.NewLine + Environment.NewLine + "Acccount Name : " + auth.accname + Environment.NewLine + Environment.NewLine + " Error has been occured :" + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace);

            }
            return load;

        }
        /// <summary>
        /// Get All the web hooks
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ValidateHook.Hooks IsHook(AreaUpload.ProjectCount model)
        {
            projectName = model.SelectedID;
            ValidateHook.Hooks val = new ValidateHook.Hooks();
            string URL = "https://" + model.AccName + ".visualstudio.com/DefaultCollection/";

            string _credentials = Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", "", model.NewPat)));
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(URL);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("appication/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _credentials);
                    HttpResponseMessage response = client.GetAsync("/_apis/hooks/subscriptions?api-version=1.0").Result;

                    //POST https://fabrikam-fiber-inc.visualstudio.com/DefaultCollection/_apis/hooks/subscriptions?api-version=1.0
                    if (response.IsSuccessStatusCode)
                    {
                        string res = response.Content.ReadAsStringAsync().Result;
                        val = JsonConvert.DeserializeObject<ValidateHook.Hooks>(res);
                        return val;
                    }
                    else
                    {
                        var errMessage = response.Content.ReadAsStringAsync();
                        string error = GeterroMessage(errMessage.Result.ToString());
                        Logerrpath = Server.MapPath("~") + @"ViewLog\ViewErrors";
                        logFileName = Logerrpath + "\\Error_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
                        LogData("Vsts call has been made at:" + DateTime.Now + Environment.NewLine + Environment.NewLine + "Acccount Name : " + model.AccName + Environment.NewLine + Environment.NewLine + " Error has been occured :" + Environment.NewLine + Environment.NewLine + error);
                    }

                }

            }
            catch (Exception ex)
            {
                string message = ex.Message.ToString();
                errors = errors + message + '*';
                Logerrpath = Server.MapPath("~") + @"ViewLog\ViewErrors";
                logFileName = Logerrpath + "\\Error_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
                LogData("Vsts call has been made at:" + DateTime.Now + Environment.NewLine + Environment.NewLine + "Acccount Name : " + model.AccName + Environment.NewLine + Environment.NewLine + " Error has been occured :" + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace);

            }
            return val;
        }
        /// <summary>
        /// check , if is there any hooks created with our URL, return True or false
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool ValidateHooks(AreaUpload.ProjectCount model)
        {
            ValidateHook.Hooks val = new ValidateHook.Hooks();
            val = IsHook(model);
            bool exist = false;
            if (val.count > 0)
            {
                foreach (var data in val.value)
                {
                    if (data.eventType == "workitem.created" && data.consumerInputs.url == "https://vstsrollupapi.azurewebsites.net/api/Rollup" && data.publisherInputs.projectId == model.SelectedID)
                    {
                        model.createmsg = "1";//"Service Hook for event type workitem.created is already exis";
                        exist = true;
                    }
                    if (data.eventType == "workitem.updated" && data.consumerInputs.url == "https://vstsrollupapi.azurewebsites.net/api/Rollup" && data.publisherInputs.projectId == model.SelectedID)
                    {
                        model.updatemsg = "2";//"Service Hook for event type workitem.updated is already exist";
                        exist = true;

                    }
                    if (data.eventType == "workitem.restored" && data.consumerInputs.url == "https://vstsrollupapi.azurewebsites.net/api/Rollup" && data.publisherInputs.projectId == model.SelectedID)
                    {
                        model.restoremsg = "3";//"Service Hook for event type workitem.restored is already exist";
                        exist = true;
                    }
                }
            }
            return exist;
        }

        /// <summary>
        /// creating service hooks, Query and Dashboard
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>

        /// <summary>
        /// Changed Logic
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult SomeDefferentLogic(AreaUpload.ProjectCount model)
        {
            bool hookExist = ValidateHooks(model);
            Dashboard mod = new Dashboard();
            mod.ErrList = string.Empty;
            mod.SuccessMsg = string.Empty;

            CheckQuery.Query isQuery = new CheckQuery.Query();
            isQuery = IsQueryExist(model);

            //if Hook exist, checking for which hook and creating not existing hook;
            if (hookExist)
            {
                if (model.createmsg == "1") { mod.ErrList = mod.ErrList + "Service Hook for event type workitem.created is already exist,"; }
                else { bool create = CreatServiceHook(model); mod.SuccessMsg = mod.SuccessMsg + "Service Hook for event type workitem.created has been created,"; }
                if (model.updatemsg == "2") { mod.ErrList = mod.ErrList + "Service Hook for event type workitem.updated is already exist,"; }
                else { bool update = UpdateServiceHook(model); mod.SuccessMsg = mod.SuccessMsg + "Service Hook for event type workitem.updated has been created,"; }
                if (model.restoremsg == "3") { mod.ErrList = mod.ErrList + "Service Hook for event type workitem.restored is already exist,"; }
                else { bool restore = RestoreServiceHook(model); mod.SuccessMsg = mod.SuccessMsg + "Service Hook for event type workitem.restored has been created,"; }
            }
            //if hookExist returns false, creating All hooks
            else
            {
                bool create = CreatServiceHook(model);
                if (create) { mod.SuccessMsg = mod.SuccessMsg + "Service Hook for event type workitem.created has been created,"; }
                else { mod.ErrList = mod.ErrList + "Can not create Service Hook for event type Workitem.created,"; }

                bool update = UpdateServiceHook(model);
                if (update) { mod.SuccessMsg = mod.SuccessMsg + "Service Hook for event type workitem.updated has been created,"; }
                else { mod.ErrList = mod.ErrList + "Can not create Service Hook for event type Workitem.updated,"; }

                bool restore = RestoreServiceHook(model);
                if (restore) { mod.SuccessMsg = mod.SuccessMsg + "Service Hook for event type workitem.restored has been created,"; }
                else { mod.ErrList = mod.ErrList + "Can not create Service Hook for event type Workitem.restored,"; }
            }
            //Cheking for Query "RollUpView". if exist, checking for dashboard "RollUp", if Dashboard is not exist, creating dashboard with existing Query name.
            if (isQuery != null)
            {
                if (isQuery.name == "RollUpView")
                {
                    mod.ErrList = mod.ErrList + "RollUpView Query Already exist,";

                    DashboardList.List Dashboardlist = new DashboardList.List();
                    Dashboardlist = GetListofDashboard(model);
                    if (Dashboardlist != null)
                    {
                        bool RollUpDashExist = false;
                        foreach (var dash in Dashboardlist.dashboardEntries)
                        {
                            if (dash.name == "RollUp")
                            {
                                RollUpDashExist = true;
                                mod.ErrList = mod.ErrList + "RollUp Dashboard Already exist,";
                            }
                        }
                        if (RollUpDashExist == false)
                        {
                            bool isBashBoardCreated = CreateDashboard(model, isQuery.id);
                            if (isBashBoardCreated == true)
                            {
                                mod.SuccessMsg = mod.SuccessMsg + "Created RollUp Dashboard,";
                            }
                        }
                    }
                    else
                    {
                        bool isDBoardCreated = CreateDashboard(model, isQuery.id);
                        if (isDBoardCreated == true)
                        {
                            mod.SuccessMsg = mod.SuccessMsg + "Created RollUp Dashboard,";
                        }
                    }
                }
            }
            // if "RollUpView" query doesn't exist, checking for Work item name and creating Query based on WIT name. After creating Query, checking for Dashboard "Rollup", if its not there, creating Dashboard.
            else
            {
                WorkItemNameResponse.WorkItem ItemName = new WorkItemNameResponse.WorkItem();
                ItemName = GetWorkItemNames(model);
                if (ItemName.count > 0)
                {
                    string QryCreatedID = null;

                    foreach (var names in ItemName.value)
                    {
                        if (names.name == "User Story")
                        {
                            QryCreatedID = CreateAgileQuery(model);

                            if (QryCreatedID != null)
                            {
                                mod.SuccessMsg = mod.SuccessMsg + "Created RollUpView Query,";
                            }
                        }
                        else if (names.name == "Requirement")
                        {
                            QryCreatedID = CreateCMMIQuery(model);
                            if (QryCreatedID != null)
                            {
                                mod.SuccessMsg = mod.SuccessMsg + "Created RollUpView Query,";
                            }
                        }
                        else if (names.name == "Product Backlog Item")
                        {
                            QryCreatedID = CreateScrumQuery(model);
                            if (QryCreatedID != null)
                            {
                                mod.SuccessMsg = mod.SuccessMsg + "Created RollUpView Query,";
                            }
                        }
                    }

                    if (QryCreatedID != null)
                    {
                        DashboardList.List Dashboardlist = new DashboardList.List();
                        Dashboardlist = GetListofDashboard(model);
                        if (Dashboardlist != null)
                        {
                            bool RollUpDashExist = false;
                            foreach (var dash in Dashboardlist.dashboardEntries)
                            {
                                if (dash.name == "RollUp")
                                {
                                    RollUpDashExist = true;
                                    mod.ErrList = mod.ErrList + "RollUp Dashboard Already exist,";
                                }
                            }
                            if (RollUpDashExist == false)
                            {
                                bool isDBoardCreated = CreateDashboard(model, QryCreatedID);
                                if (isDBoardCreated == true)
                                {
                                    mod.SuccessMsg = mod.SuccessMsg + "Created RollUp Dashboard,";
                                }
                            }
                        }
                        else
                        {
                            bool isDBoardCreated = CreateDashboard(model, QryCreatedID);
                            if (isDBoardCreated == true)
                            {
                                mod.SuccessMsg = mod.SuccessMsg + "Created RollUp Dashboard,";
                            }
                        }

                    }
                }
            }


            mod.accountName = model.AccName;
            mod.NewPat = model.PAT;
            mod.SelectedID = model.SelectedID;
            mod.ErrList = mod.ErrList.TrimEnd(',');
            mod.SuccessMsg = mod.SuccessMsg.TrimEnd(',');
            return RedirectToAction("CreateHooks", mod);
        }
        /// <summary>
        /// creating service hook for WorkItem.Created
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool CreatServiceHook(AreaUpload.ProjectCount model)
        {
            projectName = model.SelectedID;
            string URL = "https://" + model.AccName + ".visualstudio.com/DefaultCollection/";
            string _credentials = Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", "", model.NewPat)));
            string json = System.IO.File.ReadAllText(System.Web.Hosting.HostingEnvironment.MapPath("~/Files/HookCreate.json"));
            json = json.Replace("$PAT$", model.NewPat);
            json = json.Replace("$PRO$", model.SelectedID);

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(URL);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("appication/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _credentials);

                    var jsonContent = new StringContent(json, Encoding.UTF8, "application/json");
                    var method = new HttpMethod("POST");

                    var request = new HttpRequestMessage(method, "_apis/hooks/subscriptions?api-version=1.0") { Content = jsonContent };
                    var response = client.SendAsync(request).Result;

                    // HttpResponseMessage response = client.GetAsync("/_apis/hooks/subscriptions?api-version=1.0").Result;

                    //POST https://fabrikam-fiber-inc.visualstudio.com/DefaultCollection/_apis/hooks/subscriptions?api-version=1.0
                    if (response.IsSuccessStatusCode)
                    {
                        string res = response.Content.ReadAsStringAsync().Result;
                        return true;
                    }
                    else
                    {
                        var errMessage = response.Content.ReadAsStringAsync();
                        string error = GeterroMessage(errMessage.Result.ToString());
                        Logerrpath = Server.MapPath("~") + @"ViewLog\ViewErrors";
                        logFileName = Logerrpath + "\\Error_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
                        LogData("Vsts call has been made at:" + DateTime.Now + Environment.NewLine + Environment.NewLine + "Acccount Name : " + model.AccName + Environment.NewLine + Environment.NewLine + " Error has been occured :" + Environment.NewLine + Environment.NewLine + error);
                    }

                }
            }
            catch (Exception ex)
            {
                string message = ex.Message.ToString();
                errors = errors + message + '*';
                Logerrpath = Server.MapPath("~") + @"ViewLog\ViewErrors";
                logFileName = Logerrpath + "\\Error_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
                LogData("Vsts call has been made at:" + DateTime.Now + Environment.NewLine + Environment.NewLine + "Acccount Name : " + model.AccName + Environment.NewLine + Environment.NewLine + " Error has been occured :" + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace);

            }
            return false;
        }
        /// <summary>
        /// creating service hook for WorkItem.Updated
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool UpdateServiceHook(AreaUpload.ProjectCount model)
        {
            projectName = model.SelectedID;
            string URL = "https://" + model.AccName + ".visualstudio.com/DefaultCollection/";
            string _credentials = Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", "", model.NewPat)));
            string json = System.IO.File.ReadAllText(System.Web.Hosting.HostingEnvironment.MapPath("~/Files/HookUpdated.json"));
            json = json.Replace("$PAT$", model.NewPat);
            json = json.Replace("$PRO$", model.SelectedID);

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(URL);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("appication/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _credentials);

                    var jsonContent = new StringContent(json, Encoding.UTF8, "application/json");
                    var method = new HttpMethod("POST");

                    var request = new HttpRequestMessage(method, "_apis/hooks/subscriptions?api-version=1.0") { Content = jsonContent };
                    var response = client.SendAsync(request).Result;

                    // HttpResponseMessage response = client.GetAsync("/_apis/hooks/subscriptions?api-version=1.0").Result;

                    //POST https://fabrikam-fiber-inc.visualstudio.com/DefaultCollection/_apis/hooks/subscriptions?api-version=1.0
                    if (response.IsSuccessStatusCode)
                    {
                        string res = response.Content.ReadAsStringAsync().Result;
                        return true;
                    }
                    else
                    {
                        var errMessage = response.Content.ReadAsStringAsync();
                        string error = GeterroMessage(errMessage.Result.ToString());
                        Logerrpath = Server.MapPath("~") + @"ViewLog\ViewErrors";
                        logFileName = Logerrpath + "\\Error_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
                        LogData("Vsts call has been made at:" + DateTime.Now + Environment.NewLine + Environment.NewLine + "Acccount Name : " + model.AccName + Environment.NewLine + Environment.NewLine + " Error has been occured :" + Environment.NewLine + Environment.NewLine + error);
                    }
                }
            }
            catch (Exception ex)
            {
                string message = ex.Message.ToString();
                errors = errors + message + '*';
                Logerrpath = Server.MapPath("~") + @"ViewLog\ViewErrors";
                logFileName = Logerrpath + "\\Error_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
                LogData("Vsts call has been made at:" + DateTime.Now + Environment.NewLine + Environment.NewLine + "Acccount Name : " + model.AccName + Environment.NewLine + Environment.NewLine + " Error has been occured :" + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace);

            }
            return false;
        }
        /// <summary>
        /// creating service hook for WorkItem.Restored
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool RestoreServiceHook(AreaUpload.ProjectCount model)
        {
            projectName = model.SelectedID;
            string URL = "https://" + model.AccName + ".visualstudio.com/DefaultCollection/";
            string _credentials = Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", "", model.NewPat)));
            string json = System.IO.File.ReadAllText(System.Web.Hosting.HostingEnvironment.MapPath("~/Files/HookRestore.json"));
            json = json.Replace("$PAT$", model.NewPat);
            json = json.Replace("$PRO$", model.SelectedID);

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(URL);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("appication/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _credentials);

                    var jsonContent = new StringContent(json, Encoding.UTF8, "application/json");
                    var method = new HttpMethod("POST");

                    var request = new HttpRequestMessage(method, "_apis/hooks/subscriptions?api-version=1.0") { Content = jsonContent };
                    var response = client.SendAsync(request).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        string res = response.Content.ReadAsStringAsync().Result;
                        return true;
                    }
                    else
                    {
                        var errMessage = response.Content.ReadAsStringAsync();
                        string error = GeterroMessage(errMessage.Result.ToString());
                        Logerrpath = Server.MapPath("~") + @"ViewLog\ViewErrors";
                        logFileName = Logerrpath + "\\Error_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
                        LogData("Vsts call has been made at:" + DateTime.Now + Environment.NewLine + Environment.NewLine + "Acccount Name : " + model.AccName + Environment.NewLine + Environment.NewLine + " Error has been occured :" + Environment.NewLine + Environment.NewLine + error);
                    }
                }
            }
            catch (Exception ex)
            {
                string message = ex.Message.ToString();
                errors = errors + message + '*';
                Logerrpath = Server.MapPath("~") + @"ViewLog\ViewErrors";
                logFileName = Logerrpath + "\\Error_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
                LogData("Vsts call has been made at:" + DateTime.Now + Environment.NewLine + Environment.NewLine + "Acccount Name : " + model.AccName + Environment.NewLine + Environment.NewLine + " Error has been occured :" + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace);

            }
            return false;
        }

        /// <summary>
        /// Creating Query for Agile Template
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public string CreateAgileQuery(AreaUpload.ProjectCount model)
        {
            projectName = model.ProjectName;
            string URL = "https://" + model.AccName + ".visualstudio.com/DefaultCollection/";
            string _credentials = Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", "", model.NewPat)));
            string json = System.IO.File.ReadAllText(System.Web.Hosting.HostingEnvironment.MapPath("~/Files/RollUAgileQuery.json"));
            json = json.Replace("$ProjectID1$", projectName).Replace("$ProjectID2$", projectName).Replace("$ProjectID3$", projectName);
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(URL);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("appication/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _credentials);

                    var jsonContent = new StringContent(json, Encoding.UTF8, "application/json");
                    var method = new HttpMethod("POST");

                    var request = new HttpRequestMessage(method, projectName + "/_apis/wit/queries/Shared%20Queries?api-version=2.2") { Content = jsonContent };
                    var response = client.SendAsync(request).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        QueryResponse.Response responseID = new QueryResponse.Response();
                        string res = response.Content.ReadAsStringAsync().Result;
                        responseID = JsonConvert.DeserializeObject<QueryResponse.Response>(res);
                        return responseID.id;
                    }
                    else
                    {
                        var errMessage = response.Content.ReadAsStringAsync();
                        string error = GeterroMessage(errMessage.Result.ToString());


                        Logerrpath = Server.MapPath("~") + @"ViewLog\ViewErrors";
                        logFileName = Logerrpath + "\\Error_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
                        LogData("Vsts call has been made at:" + DateTime.Now + Environment.NewLine + Environment.NewLine + "Acccount Name : " + model.AccName + Environment.NewLine + Environment.NewLine + " Error has been occured :" + Environment.NewLine + error);

                    }
                }
            }
            catch (Exception ex)
            {
                string message = ex.Message.ToString();
                errors = errors + message + '*';
                Logerrpath = Server.MapPath("~") + @"ViewLog\ViewErrors";
                logFileName = Logerrpath + "\\Error_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
                LogData("Vsts call has been made at:" + DateTime.Now + Environment.NewLine + Environment.NewLine + "Acccount Name : " + model.AccName + Environment.NewLine + Environment.NewLine + " Error has been occured :" + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace);

            }
            return null;
        }
        /// <summary>
        /// Creating Query for  Scrum Template
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public string CreateScrumQuery(AreaUpload.ProjectCount model)
        {
            projectName = model.ProjectName;
            string URL = "https://" + model.AccName + ".visualstudio.com/DefaultCollection/";
            string _credentials = Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", "", model.NewPat)));
            string json = System.IO.File.ReadAllText(System.Web.Hosting.HostingEnvironment.MapPath("~/Files/RollUpScrumQuery.json"));
            json = json.Replace("$Projectname1$", projectName).Replace("$Projectname2$", projectName).Replace("$Projectname3$", projectName);
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(URL);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("appication/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _credentials);

                    var jsonContent = new StringContent(json, Encoding.UTF8, "application/json");
                    var method = new HttpMethod("POST");

                    var request = new HttpRequestMessage(method, projectName + "/_apis/wit/queries/Shared%20Queries?api-version=2.2") { Content = jsonContent };
                    var response = client.SendAsync(request).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        QueryResponse.Response responseID = new QueryResponse.Response();
                        string res = response.Content.ReadAsStringAsync().Result;
                        responseID = JsonConvert.DeserializeObject<QueryResponse.Response>(res);
                        return responseID.id;
                    }
                    else
                    {
                        var errMessage = response.Content.ReadAsStringAsync();
                        string error = GeterroMessage(errMessage.Result.ToString());


                        Logerrpath = Server.MapPath("~") + @"ViewLog\ViewErrors";
                        logFileName = Logerrpath + "\\Error_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
                        LogData("Vsts call has been made at:" + DateTime.Now + Environment.NewLine + Environment.NewLine + "Acccount Name : " + model.AccName + Environment.NewLine + Environment.NewLine + " Error has been occured :" + Environment.NewLine + error);

                    }
                }
            }
            catch (Exception ex)
            {
                string message = ex.Message.ToString();
                errors = errors + message + '*';
                Logerrpath = Server.MapPath("~") + @"ViewLog\ViewErrors";
                logFileName = Logerrpath + "\\Error_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
                LogData("Vsts call has been made at:" + DateTime.Now + Environment.NewLine + Environment.NewLine + "Acccount Name : " + model.AccName + Environment.NewLine + Environment.NewLine + " Error has been occured :" + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace);

            }
            return null;
        }
        /// <summary>
        /// Creating Query for CMMI Template
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public string CreateCMMIQuery(AreaUpload.ProjectCount model)
        {
            projectName = model.ProjectName;
            string URL = "https://" + model.AccName + ".visualstudio.com/DefaultCollection/";
            string _credentials = Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", "", model.NewPat)));
            string json = System.IO.File.ReadAllText(System.Web.Hosting.HostingEnvironment.MapPath("~/Files/RollUpCMMIQuery.json"));
            json = json.Replace("$Projectname1$", projectName).Replace("$Projectname2$", projectName).Replace("$Projectname3$", projectName);
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(URL);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("appication/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _credentials);

                    var jsonContent = new StringContent(json, Encoding.UTF8, "application/json");
                    var method = new HttpMethod("POST");

                    var request = new HttpRequestMessage(method, projectName + "/_apis/wit/queries/Shared%20Queries?api-version=2.2") { Content = jsonContent };
                    var response = client.SendAsync(request).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        QueryResponse.Response responseID = new QueryResponse.Response();
                        string res = response.Content.ReadAsStringAsync().Result;
                        responseID = JsonConvert.DeserializeObject<QueryResponse.Response>(res);
                        return responseID.id;
                    }
                    else
                    {
                        var errMessage = response.Content.ReadAsStringAsync();
                        string error = GeterroMessage(errMessage.Result.ToString());


                        Logerrpath = Server.MapPath("~") + @"ViewLog\ViewErrors";
                        logFileName = Logerrpath + "\\Error_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
                        LogData("Vsts call has been made at:" + DateTime.Now + Environment.NewLine + Environment.NewLine + "Acccount Name : " + model.AccName + Environment.NewLine + Environment.NewLine + " Error has been occured :" + Environment.NewLine + error);

                    }
                }
            }
            catch (Exception ex)
            {
                string message = ex.Message.ToString();
                errors = errors + message + '*';

                Logerrpath = Server.MapPath("~") + @"ViewLog\ViewErrors";
                logFileName = Logerrpath + "\\Error_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
                LogData("Vsts call has been made at:" + DateTime.Now + Environment.NewLine + Environment.NewLine + "Acccount Name : " + model.AccName + Environment.NewLine + Environment.NewLine + " Error has been occured :" + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace);

            }
            return null;
        }

        /// <summary>
        /// Creating Query for Dashboard
        /// </summary>
        /// <param name="model, QueryID"></param>
        /// <returns></returns>
        public bool CreateDashboard(AreaUpload.ProjectCount model, string QID)
        {
            projectName = model.SelectedID;
            string URL = "https://" + model.AccName + ".visualstudio.com/DefaultCollection/";
            string _credentials = Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", "", model.NewPat)));
            string json = System.IO.File.ReadAllText(System.Web.Hosting.HostingEnvironment.MapPath("~/Files/DashboardRollUp.json"));
            json = json.Replace("$QID$", QID);

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(URL);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("appication/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _credentials);

                    var jsonContent = new StringContent(json, Encoding.UTF8, "application/json");
                    var method = new HttpMethod("POST");

                    var request = new HttpRequestMessage(method, projectName + "/_apis/Dashboard/Dashboards/?api-version=3.0-preview.2") { Content = jsonContent };
                    var response = client.SendAsync(request).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        string res = response.Content.ReadAsStringAsync().Result;
                        return true;
                    }
                    else
                    {
                        var errMessage = response.Content.ReadAsStringAsync();
                        string error = GeterroMessage(errMessage.Result.ToString());


                        Logerrpath = Server.MapPath("~") + @"ViewLog\ViewErrors";
                        logFileName = Logerrpath + "\\Error_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
                        LogData("Vsts call has been made at:" + DateTime.Now + Environment.NewLine + Environment.NewLine + "Acccount Name : " + model.AccName + Environment.NewLine + Environment.NewLine + " Error has been occured :" + Environment.NewLine + error);

                    }
                }
            }
            catch (Exception ex)
            {
                string message = ex.Message.ToString();
                errors = errors + message + '*';

                Logerrpath = Server.MapPath("~") + @"ViewLog\ViewErrors";
                logFileName = Logerrpath + "\\Error_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
                LogData("Vsts call has been made at:" + DateTime.Now + Environment.NewLine + Environment.NewLine + "Acccount Name : " + model.AccName + Environment.NewLine + Environment.NewLine + " Error has been occured :" + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace);

            }
            return false;
        }


        /// <summary>
        /// Getting work item names to get to know the template based on User story, PBi or Requirement
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public WorkItemNameResponse.WorkItem GetWorkItemNames(AreaUpload.ProjectCount model)
        {
            projectName = model.SelectedID;
            string URL = "https://" + model.AccName + ".visualstudio.com/DefaultCollection/";
            string _credentials = Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", "", model.NewPat)));
            WorkItemNameResponse.WorkItem work = new WorkItemNameResponse.WorkItem();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(URL);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("appication/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _credentials);
                    //https://rolluppro.visualstudio.com/DefaultCollection/DemoGen/_apis/wit/workItemTypes?api-version=2.0
                    HttpResponseMessage response = client.GetAsync(projectName + "/_apis/wit/workItemTypes?api-version=2.0").Result;

                    if (response.IsSuccessStatusCode)
                    {
                        string res = response.Content.ReadAsStringAsync().Result;
                        work = JsonConvert.DeserializeObject<WorkItemNameResponse.WorkItem>(res);
                        return work;
                    }
                    else
                    {
                        var errMessage = response.Content.ReadAsStringAsync();
                        string error = GeterroMessage(errMessage.Result.ToString());
                        Logerrpath = Server.MapPath("~") + @"ViewLog\ViewErrors";
                        logFileName = Logerrpath + "\\Error_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
                        LogData("Vsts call has been made at:" + DateTime.Now + Environment.NewLine + Environment.NewLine + "Acccount Name : " + model.AccName + Environment.NewLine + Environment.NewLine + " Error has been occured :" + Environment.NewLine + Environment.NewLine + error);
                    }
                }
            }
            catch (Exception ex)
            {
                string message = ex.Message.ToString();
                errors = errors + message + '*';
                Logerrpath = Server.MapPath("~") + @"ViewLog\ViewErrors";
                logFileName = Logerrpath + "\\Error_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
                LogData("Vsts call has been made at:" + DateTime.Now + Environment.NewLine + Environment.NewLine + "Acccount Name : " + model.AccName + Environment.NewLine + Environment.NewLine + " Error has been occured :" + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace);

            }
            return null;
        }

        /// <summary>
        /// Checking for RollUpView Query, if exist returning model, else returning Null
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public CheckQuery.Query IsQueryExist(AreaUpload.ProjectCount model)
        {
            projectName = model.SelectedID;
            string URL = "https://" + model.AccName + ".visualstudio.com/DefaultCollection/";
            string _credentials = Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", "", model.NewPat)));
            CheckQuery.Query work = new CheckQuery.Query();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(URL);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("appication/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _credentials);
                    HttpResponseMessage response = client.GetAsync(projectName + "/_apis/wit/queries/Shared%20Queries/RollUpView?api-version=2.2").Result;


                    if (response.IsSuccessStatusCode)
                    {
                        string res = response.Content.ReadAsStringAsync().Result;
                        work = JsonConvert.DeserializeObject<CheckQuery.Query>(res);

                        return work;
                    }
                    else
                    {
                        var errMessage = response.Content.ReadAsStringAsync();
                        string error = GeterroMessage(errMessage.Result.ToString());
                        Logerrpath = Server.MapPath("~") + @"ViewLog\ViewErrors";
                        logFileName = Logerrpath + "\\Error_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
                        LogData("Vsts call has been made at:" + DateTime.Now + Environment.NewLine + Environment.NewLine + "Acccount Name : " + model.AccName + Environment.NewLine + Environment.NewLine + " Error has been occured :" + Environment.NewLine + Environment.NewLine + error);
                    }
                }
            }
            catch (Exception ex)
            {
                string message = ex.Message.ToString();
                errors = errors + message + '*';
                Logerrpath = Server.MapPath("~") + @"ViewLog\ViewErrors";
                logFileName = Logerrpath + "\\Error_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
                LogData("Vsts call has been made at:" + DateTime.Now + Environment.NewLine + Environment.NewLine + "Acccount Name : " + model.AccName + Environment.NewLine + Environment.NewLine + " Error has been occured :" + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace);

            }
            return null;
        }

        /// <summary>
        /// Getting list of dashboard details. if dashboard is not exist returning Null
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public DashboardList.List GetListofDashboard(AreaUpload.ProjectCount model)
        {

            projectName = model.SelectedID;
            string URL = "https://" + model.AccName + ".visualstudio.com/DefaultCollection/";
            string _credentials = Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", "", model.NewPat)));
            DashboardList.List dash = new DashboardList.List();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(URL);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("appication/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _credentials);
                    HttpResponseMessage response = client.GetAsync(projectName + "/_apis/dashboard/dashboards/?api-version=3.0-preview.2").Result;

                    if (response.IsSuccessStatusCode)
                    {
                        string res = response.Content.ReadAsStringAsync().Result;
                        dash = JsonConvert.DeserializeObject<DashboardList.List>(res);
                        return dash;
                    }
                    else
                    {
                        var errMessage = response.Content.ReadAsStringAsync();
                        string error = GeterroMessage(errMessage.Result.ToString());
                        Logerrpath = Server.MapPath("~") + @"ViewLog\ViewErrors";
                        logFileName = Logerrpath + "\\Error_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
                        LogData("Vsts call has been made at:" + DateTime.Now + Environment.NewLine + Environment.NewLine + "Acccount Name : " + model.AccName + Environment.NewLine + Environment.NewLine + " Error has been occured :" + Environment.NewLine + Environment.NewLine + error);
                    }
                }
            }
            catch (Exception ex)
            {
                string message = ex.Message.ToString();
                errors = errors + message + '*';
                Logerrpath = Server.MapPath("~") + @"ViewLog\ViewErrors";
                logFileName = Logerrpath + "\\Error_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
                LogData("Vsts call has been made at:" + DateTime.Now + Environment.NewLine + Environment.NewLine + "Acccount Name : " + model.AccName + Environment.NewLine + Environment.NewLine + " Error has been occured :" + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace);

            }
            return null;
        }

        /// <summary>
        /// Getting a dashboard details. if dashboard is not exist returning Null
        /// </summary>
        /// <param name="model, DashboardID"></param>
        /// <returns></returns>
        public GetDashboardByID.Dashboard GetAdashboardbyID(AreaUpload.ProjectCount model, string DashboardID)
        {
            projectName = model.SelectedID;
            string URL = "https://" + model.AccName + ".visualstudio.com/DefaultCollection/";
            string _credentials = Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", "", model.NewPat)));
            GetDashboardByID.Dashboard dash = new GetDashboardByID.Dashboard();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(URL);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("appication/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _credentials);
                    HttpResponseMessage response = client.GetAsync(projectName + "/_apis/dashboard/dashboards/" + DashboardID + "?api-version=3.0-preview.2").Result;
                    //https://rolluppro.VisualStudio.com/DefaultCollection/TFVC/_apis/dashboard/dashboards/2af36498-632a-4597-a430-f6c09ebb3e17?api-version=3.0-preview.2
                    if (response.IsSuccessStatusCode)
                    {
                        string res = response.Content.ReadAsStringAsync().Result;
                        dash = JsonConvert.DeserializeObject<GetDashboardByID.Dashboard>(res);
                        return dash;
                    }
                    else
                    {
                        var errMessage = response.Content.ReadAsStringAsync();
                        string error = GeterroMessage(errMessage.Result.ToString());
                        Logerrpath = Server.MapPath("~") + @"ViewLog\ViewErrors";
                        logFileName = Logerrpath + "\\Error_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
                        LogData("Vsts call has been made at:" + DateTime.Now + Environment.NewLine + Environment.NewLine + "Acccount Name : " + model.AccName + Environment.NewLine + Environment.NewLine + " Error has been occured :" + Environment.NewLine + Environment.NewLine + error);
                    }
                }
            }
            catch (Exception ex)
            {
                string message = ex.Message.ToString();
                errors = errors + message + '*';
                Logerrpath = Server.MapPath("~") + @"ViewLog\ViewErrors";
                logFileName = Logerrpath + "\\Error_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
                LogData("Vsts call has been made at:" + DateTime.Now + Environment.NewLine + Environment.NewLine + "Acccount Name : " + model.AccName + Environment.NewLine + Environment.NewLine + " Error has been occured :" + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace);

            }
            return null;
        }

        public static string GeterroMessage(string Exception)
        {
            string message = string.Empty;
            try
            {
                JObject jItems = JObject.Parse(Exception);
                message = jItems["message"].ToString();

                return message;
            }
            catch (Exception ex)
            {
                return message;
            }
        }
        private void LogData(string message)
        {
            System.IO.File.AppendAllText(logFileName, message);
        }

        /// <summary>
        /// Get The template name, Not using below methods
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// 
        public string GetTemplateName(AreaUpload.ProjectCount model)
        {
            ProjectProp.Prop qury = new ProjectProp.Prop();
            projectName = model.SelectedID;
            string URL = "https://" + model.AccName + ".visualstudio.com/DefaultCollection/";
            string _credentials = Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", "", model.NewPat)));
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(URL);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("appication/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _credentials);
                    HttpResponseMessage response = client.GetAsync("/_apis/projects/" + projectName + "?includeCapabilities=true&api-version=1.0").Result;

                    if (response.IsSuccessStatusCode)
                    {
                        string res = response.Content.ReadAsStringAsync().Result;
                        qury = JsonConvert.DeserializeObject<ProjectProp.Prop>(res);
                        return qury.capabilities.processTemplate.templateName;
                    }
                }
            }
            catch (Exception ex)
            {
                string message = ex.Message.ToString();
                errors = errors + message + '*';

                Logerrpath = Server.MapPath("~") + @"ViewLog\ViewErrors";
                logFileName = Logerrpath + "\\Error_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
                LogData("Vsts call has been made at:" + DateTime.Now + Environment.NewLine + Environment.NewLine + "Acccount Name : " + model.AccName + Environment.NewLine + Environment.NewLine + " Error has been occured :" + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace);

            }
            return null;
        }

        public JsonResult GetTeamList(AreaUpload.ProjectCount model)
        {
            try
            {
                Teams.TeamList team = new Teams.TeamList();

                projectName = model.SelectedID;
                string URL = "https://" + model.AccName + ".visualstudio.com/DefaultCollection/";
                string _credentials = Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", "", model.NewPat)));
                /// string json = System.IO.File.ReadAllText(System.Web.Hosting.HostingEnvironment.MapPath("~/Files/HookRestore.json"));

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(URL);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("appication/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _credentials);
                    HttpResponseMessage response = client.GetAsync("/_apis/projects/" + projectName + "/teams?api-version=2.2").Result;
                    if (response.IsSuccessStatusCode)
                    {
                        string res = response.Content.ReadAsStringAsync().Result;
                        team = JsonConvert.DeserializeObject<Teams.TeamList>(res);
                        var reslt = Json(team, JsonRequestBehavior.AllowGet);
                        return reslt;
                    }
                }
            }
            catch (Exception ex)
            {
                Logerrpath = Server.MapPath("~") + @"ViewLog\ViewErrors";
                logFileName = Logerrpath + "\\Error_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
                LogData("Vsts call has been made at:" + DateTime.Now + Environment.NewLine + Environment.NewLine + "Acccount Name : " + model.AccName + Environment.NewLine + Environment.NewLine + " Error has been occured :" + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace);

            }
            return null;

        }

        public QueryList.Query GetListofQueries(AreaUpload.ProjectCount model)
        {
            QueryList.Query qury = new QueryList.Query();
            projectName = model.SelectedID;
            string URL = "https://" + model.AccName + ".visualstudio.com/DefaultCollection/";
            string _credentials = Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", "", model.NewPat)));
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(URL);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("appication/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _credentials);
                    HttpResponseMessage response = client.GetAsync(projectName + "/_apis/wit/queries?$depth=1&api-version=2.2").Result;

                    if (response.IsSuccessStatusCode)
                    {
                        string res = response.Content.ReadAsStringAsync().Result;
                        qury = JsonConvert.DeserializeObject<QueryList.Query>(res);
                    }
                }
            }
            catch (Exception ex)
            {
                string message = ex.Message.ToString();
                errors = errors + message + '*';
                Logerrpath = Server.MapPath("~") + @"ViewLog\ViewErrors";
                logFileName = Logerrpath + "\\Error_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
                LogData("Vsts call has been made at:" + DateTime.Now + Environment.NewLine + Environment.NewLine + "Acccount Name : " + model.AccName + Environment.NewLine + Environment.NewLine + " Error has been occured :" + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace);

            }
            return qury;
        }

        public ActionResult ServiceHook(AreaUpload.ProjectCount model)
        {
            bool exist = ValidateHooks(model);
            Dashboard mod = new Dashboard();
            mod.ErrList = string.Empty;
            mod.SuccessMsg = string.Empty;

            if (exist)
            {
                if (model.createmsg == "1")
                {
                    mod.ErrList = mod.ErrList + "Service Hook for event type workitem.created is already exist,";
                }
                else
                {
                    bool create = CreatServiceHook(model);
                    mod.SuccessMsg = mod.SuccessMsg + "Service Hook for event type workitem.created has been created,";
                }
                if (model.updatemsg == "2")
                {
                    mod.ErrList = mod.ErrList + "Service Hook for event type workitem.updated is already exist,";
                }
                else
                {
                    bool update = UpdateServiceHook(model);
                    mod.SuccessMsg = mod.SuccessMsg + "Service Hook for event type workitem.updated has been created,";
                }
                if (model.restoremsg == "3")
                {
                    mod.ErrList = mod.ErrList + "Service Hook for event type workitem.restored is already exist,";
                }
                else
                {
                    bool restore = RestoreServiceHook(model);
                    mod.SuccessMsg = mod.SuccessMsg + "Service Hook for event type workitem.restored has been created,";

                }
                /// for time being i'm doing this. should change.

                WorkItemNameResponse.WorkItem ItemName = new WorkItemNameResponse.WorkItem();
                ItemName = GetWorkItemNames(model);
                if (ItemName.count > 0)
                {
                    string QryCreatedID = null;

                    foreach (var names in ItemName.value)
                    {
                        if (names.name == "User Story")
                        {
                            QryCreatedID = CreateAgileQuery(model);

                            if (QryCreatedID != null)
                            {
                                mod.SuccessMsg = mod.SuccessMsg + "Created RollUpView Query,";
                            }
                        }
                        else if (names.name == "Requirement")
                        {
                            QryCreatedID = CreateCMMIQuery(model);
                            if (QryCreatedID != null)
                            {
                                mod.SuccessMsg = mod.SuccessMsg + "Created RollUpView Query,";
                            }
                        }
                        else if (names.name == "Product Backlog Item")
                        {
                            QryCreatedID = CreateScrumQuery(model);
                            if (QryCreatedID != null)
                            {
                                mod.SuccessMsg = mod.SuccessMsg + "Created RollUpView Query,";
                            }
                        }
                    }

                    if (QryCreatedID != null)
                    {

                        bool isBashBoardCreated = CreateDashboard(model, QryCreatedID);
                        if (isBashBoardCreated == true)
                        {
                            mod.SuccessMsg = mod.SuccessMsg + "Created RollUp Dashboard,";
                        }
                    }
                }
                //
                mod.accountName = model.AccName;
                mod.NewPat = model.PAT;
                mod.SelectedID = model.SelectedID;
                mod.ErrList = mod.ErrList.TrimEnd(',');
                mod.SuccessMsg = mod.SuccessMsg.TrimEnd(',');

                logSucPath = Server.MapPath("~") + @"ViewLog\ViewSuccess";
                logFileName = logSucPath + "\\Suc_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
                LogData("Vsts call has been made at:" + DateTime.Now + Environment.NewLine + Environment.NewLine + "Acccount Name : " + model.AccName + Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine + mod.ErrList);


                return RedirectToAction("CreateHooks", mod);
            }
            else
            {
                WorkItemNameResponse.WorkItem ItemName = new WorkItemNameResponse.WorkItem();
                ItemName = GetWorkItemNames(model);
                if (ItemName.count > 0)
                {
                    string QryCreatedID = null;

                    foreach (var names in ItemName.value)
                    {
                        if (names.name == "User Story")
                        {
                            QryCreatedID = CreateAgileQuery(model);

                            if (QryCreatedID != null)
                            {
                                mod.SuccessMsg = mod.SuccessMsg + "Created RollUpView Query,";
                            }
                        }
                        else if (names.name == "Requirement")
                        {
                            QryCreatedID = CreateCMMIQuery(model);
                            if (QryCreatedID != null)
                            {
                                mod.SuccessMsg = mod.SuccessMsg + "Created RollUpView Query,";
                            }
                        }
                        else if (names.name == "Product Backlog Item")
                        {
                            QryCreatedID = CreateScrumQuery(model);
                            if (QryCreatedID != null)
                            {
                                mod.SuccessMsg = mod.SuccessMsg + "Created RollUpView Query,";
                            }
                        }
                    }

                    if (QryCreatedID != null)
                    {

                        bool isBashBoardCreated = CreateDashboard(model, QryCreatedID);
                        if (isBashBoardCreated == true)
                        {
                            mod.SuccessMsg = mod.SuccessMsg + "Created RollUp Dashboard,";
                        }
                    }
                    bool create = CreatServiceHook(model);
                    mod.SuccessMsg = mod.SuccessMsg + "Service Hook for event type workitem.created has been created,";

                    bool update = UpdateServiceHook(model);
                    mod.SuccessMsg = mod.SuccessMsg + "Service Hook for event type workitem.updated has been created,";


                    bool restore = RestoreServiceHook(model);
                    mod.SuccessMsg = mod.SuccessMsg + "Service Hook for event type workitem.restored has been created,";
                }
                else
                {
                    mod.ErrList = mod.ErrList + "Can not create RollUp Dashboard, Query and Service Hooks,";
                    Logerrpath = Server.MapPath("~") + @"ViewLog\ViewErrors";
                    logFileName = Logerrpath + "\\Error_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
                    LogData("Vsts call has been made at:" + DateTime.Now + Environment.NewLine + Environment.NewLine + "Acccount Name : " + model.AccName + Environment.NewLine + Environment.NewLine + " Error has been occured :" + Environment.NewLine + Environment.NewLine + "Can not create RollUp dashboard, Query and Service Hooks");

                }
            }
            mod.accountName = model.AccName;
            mod.NewPat = model.PAT;
            mod.SelectedID = model.SelectedID;
            mod.ErrList = mod.ErrList.TrimEnd(',');
            mod.SuccessMsg = mod.SuccessMsg.TrimEnd(',');
            return RedirectToAction("CreateHooks", mod);
        }


    }
}