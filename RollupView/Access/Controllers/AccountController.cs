using Access.Models;
using Newtonsoft.Json;
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
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Test()
        {

            return View();
        }
       
        public ActionResult Verify()
        {
            //vso.identity vso.profile_write vso.work_write
            string url = "https://app.vssps.visualstudio.com/oauth2/authorize?client_id={0}&response_type=Assertion&state=User1&scope=vso.identity%20vso.profile_write%20vso.work_write&redirect_uri={1}";

            string redirectUrl = System.Configuration.ConfigurationManager.AppSettings["RedirectUri"];
            string clientId = System.Configuration.ConfigurationManager.AppSettings["ClientId"];
            url = string.Format(url, clientId, redirectUrl);

            return Redirect(url);
        }
        public ActionResult Callback(Dashboard model)
        {
            try
            {
                string code = Request.QueryString["code"];

                string redirectUrl = System.Configuration.ConfigurationManager.AppSettings["RedirectUri"];
                string clientId = System.Configuration.ConfigurationManager.AppSettings["ClientSecret"];

                string accessRequestBody = GenerateRequestPostData(clientId, code, redirectUrl);

                //AccessDetails AccessDetails = GetAccessToken(accessRequestBody);

                //return RedirectToAction("Test", "Account", AccessDetails);
                //AccessDetails AccessDetails = new AccessDetails();
                //AccessDetails.access_token = "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6Im9PdmN6NU1fN3AtSGpJS2xGWHo5M3VfVjBabyJ9.eyJuYW1laWQiOiI5ZjNlMTMyOS0yNzE3LTYxZWMtOTE1Yy04ODdlZDRjY2YxZjEiLCJzY3AiOiJ2c28uYWdlbnRwb29sc19tYW5hZ2UgdnNvLmJ1aWxkX2V4ZWN1dGUgdnNvLmNoYXRfbWFuYWdlIHZzby5jb2RlX2Z1bGwgdnNvLmNvZGVfc3RhdHVzIHZzby5jb2Rlc2VhcmNoIHZzby5jb25uZWN0ZWRfc2VydmVyIHZzby5kYXNoYm9hcmRzIHZzby5kYXNoYm9hcmRzX21hbmFnZSB2c28uZW50aXRsZW1lbnRzIHZzby5leHRlbnNpb24uZGF0YV93cml0ZSB2c28uZXh0ZW5zaW9uX21hbmFnZSB2c28uZ2FsbGVyeV9hY3F1aXJlIHZzby5nYWxsZXJ5X21hbmFnZSB2c28uaWRlbnRpdHlfbWFuYWdlIHZzby5sb2FkdGVzdF93cml0ZSB2c28ubm90aWZpY2F0aW9uX21hbmFnZSB2c28ucGFja2FnaW5nX21hbmFnZSB2c28ucHJvZmlsZV93cml0ZSB2c28ucHJvamVjdF9tYW5hZ2UgdnNvLnJlbGVhc2VfbWFuYWdlIHZzby5zZWN1cml0eV9tYW5hZ2UgdnNvLnNlcnZpY2VlbmRwb2ludF9tYW5hZ2UgdnNvLnRhc2tncm91cHNfbWFuYWdlIHZzby50ZXN0X3dyaXRlIHZzby53aWtpX3dyaXRlIHZzby53b3JrX2Z1bGwgdnNvLndvcmtpdGVtc2VhcmNoIiwiaXNzIjoiYXBwLnZzc3BzLnZpc3VhbHN0dWRpby5jb20iLCJhdWQiOiJhcHAudnNzcHMudmlzdWFsc3R1ZGlvLmNvbSIsIm5iZiI6MTUwNzU0Mjg2MywiZXhwIjoxNTA3NTQ2NDYzfQ.1hT-Vw-2a83gAqlETS_vI6lTCJ46oSPIYMrfFt5oZCcOmworZDj-aN8SNyYv7jnXeoTHHOEFwXKj3EzpBKxKnaabfJig7f2oeI8p3IEGoSp51eHRBO0esGE8XUso5FRoN0Gaj8guSSWyyhmPg6Ofn22ES64PiFRBf5PZMotYaxa5cqvX02B_JEujPwMKK29l_fTDIIDrtlkEToF1NXpTEYf1ZCe-s1QQdf4J5T-QTmNXC-6csC5HgcrWgXAL4n4Yhkj63WueAK5020sjxbi_le0DOIml4b-7h_ksyyIQ6RO1mClzA7XUdCMEBj9fX7t-6CrmmEg3epdgI5m1x1aKnA";
                //AccessDetails.refresh_token = "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6Im9PdmN6NU1fN3AtSGpJS2xGWHo5M3VfVjBabyJ9.eyJuYW1laWQiOiI5ZjNlMTMyOS0yNzE3LTYxZWMtOTE1Yy04ODdlZDRjY2YxZjEiLCJzY3AiOiJ2c28uaWRlbnRpdHkgdnNvLnByb2ZpbGVfd3JpdGUgdnNvLndvcmtfd3JpdGUiLCJpc3MiOiJhcHAudnNzcHMudmlzdWFsc3R1ZGlvLmNvbSIsImF1ZCI6ImFwcC52c3Nwcy52aXN1YWxzdHVkaW8uY29tIiwibmJmIjoxNTAyNzA0MjMxLCJleHAiOjE1MDI3MDc4MzF9.cnd_ObvdmVFN2RyC9YKM8B4CJYoSfY5R6UHHZMR9nP8yyDZcLhq8wscX_qAEgmXEJpj7bfnl3n8yJMHGZ6lqYIiUqhURS-9rp2VnxgoEhwnDKjqSEe1jRhv_fqpj2EE8LyOmmLG__7FGuIFl3UOj5HXavtMuSFWvtmPBMCtKhYhmUm7bWPZY1dVSe8LEoEobz9bPsZVJxAz4yjK0wb5p6mTeX2AsR87_TzE0pPFhwW6jcuUUYZ7iJb7LWsgS7W0Dy0ZcRY0i-B3Cw3mNJ5jqgBaDxId7irtRLpTGZyHSqLFilAomTAw82yB3xWYuYjLBLPnJkgGrXFMbrvwbFVcKoQ";

                ProfileDetails Profile = GetProfile(AccessDetails);
                Accounts.AccountList accountList = GetAccounts(Profile.id, AccessDetails);

                model.accessToken = AccessDetails.access_token;
                model.refreshToken = AccessDetails.refresh_token;
                model.accountsForDdl = new List<string>();
                model.Email = Profile.emailAddress;
                model.Name = Profile.displayName;
                if (accountList.count > 0)
                {
                    foreach (var account in accountList.value)
                    {
                        model.accountsForDdl.Add(account.accountName);
                    }
                    model.hasAccount = true;
                }
                return View(model);
            }
            catch (Exception ex)
            {
                return View();
            }

        }
        public string GenerateRequestPostData(string appSecret, string authCode, string callbackUrl)
        {
            return String.Format("client_assertion_type=urn:ietf:params:oauth:client-assertion-type:jwt-bearer&client_assertion={0}&grant_type=urn:ietf:params:oauth:grant-type:jwt-bearer&assertion={1}&redirect_uri={2}",
                        HttpUtility.UrlEncode(appSecret),
                        HttpUtility.UrlEncode(authCode),
                        callbackUrl
                 );
        }
        public AccessDetails GetAccessToken(string body)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri("https://app.vssps.visualstudio.com");

            var request = new HttpRequestMessage(HttpMethod.Post, "/oauth2/token");

            var requestContent = body;
            request.Content = new StringContent(requestContent, Encoding.UTF8, "application/x-www-form-urlencoded");

            var response = client.SendAsync(request).Result;
            if (response.IsSuccessStatusCode)
            {
                string result = response.Content.ReadAsStringAsync().Result;
                AccessDetails details = Newtonsoft.Json.JsonConvert.DeserializeObject<AccessDetails>(result);
                return details;
            }
            return new AccessDetails();
        }

        public static ProfileDetails GetProfile(AccessDetails accessDetails)
        {
            ProfileDetails Profile = new ProfileDetails();

            var client = new HttpClient();
            client.BaseAddress = new Uri("https://app.vssps.visualstudio.com");
            var request = new HttpRequestMessage(HttpMethod.Get, "/_apis/profile/profiles/me");

            var requestContent = string.Format(
                "site={0}&api-version={1}", Uri.EscapeDataString("https://app.vssps.visualstudio.com"), Uri.EscapeDataString("1.0"));

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.Add("Authorization", string.Format("Bearer {0}", accessDetails.access_token));
            try
            {
                var response = client.SendAsync(request).Result;
                if (response.StatusCode == HttpStatusCode.NonAuthoritativeInformation)
                {
                    string accessToken = Refresh_AccessToken(accessDetails.refresh_token);
                    GetProfile(new AccessDetails() { access_token = accessToken });
                }
                else if (response.IsSuccessStatusCode)
                {
                    string result = response.Content.ReadAsStringAsync().Result;
                    Profile = JsonConvert.DeserializeObject<ProfileDetails>(result);
                }
                else
                {
                    var errorMessage = response.Content.ReadAsStringAsync();
                    Profile = null;
                }
            }
            catch (Exception ex)
            {
                Profile.ErrorMessage = ex.Message;
            }
            return Profile;
        }

        public static string Refresh_AccessToken(string refreshToken)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri("https://app.vssps.visualstudio.com");
            //client.BaseAddress = new Uri(token_Uri);
            var request = new HttpRequestMessage(HttpMethod.Post, "/oauth2/token");
            //string RefreshToken = AuthParams.Refresh_Token;

            string redirectUri = System.Configuration.ConfigurationManager.AppSettings["RedirectUri"];
            string ClientSecret = System.Configuration.ConfigurationManager.AppSettings["ClientSecret"];

            var requestContent = string.Format(
                "site={0}&client_assertion_type={1}&client_assertion={2}&grant_type={3}&assertion={4}&redirect_uri={5}",
                Uri.EscapeDataString("https://app.vssps.visualstudio.com"), Uri.EscapeDataString("urn:ietf:params:oauth:client-assertion-type:jwt-bearer"),
                Uri.EscapeDataString(ClientSecret), Uri.EscapeDataString("refresh_token"),
                Uri.EscapeDataString(refreshToken), Uri.EscapeDataString(redirectUri)
                );

            request.Content = new StringContent(requestContent, Encoding.UTF8, "application/x-www-form-urlencoded");
            try
            {
                var response = client.SendAsync(request).Result;
                if (response.IsSuccessStatusCode)
                {
                    string result = response.Content.ReadAsStringAsync().Result;
                    AccessDetails accesDetails = JsonConvert.DeserializeObject<AccessDetails>(result);
                    return accesDetails.access_token;
                }
                else
                {
                    return string.Empty;
                }
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public static Accounts.AccountList GetAccounts(string MemberID, AccessDetails Details)
        {
            Accounts.AccountList Accounts = new Accounts.AccountList();
            var client = new HttpClient();
            string requestContent = "https://app.vssps.visualstudio.com/_apis/Accounts?memberId=" + MemberID + "&api-version=3.2-preview";
            var request = new HttpRequestMessage(HttpMethod.Get, requestContent);
            request.Headers.Add("Authorization", "Bearer " + Details.access_token);
            try
            {
                var response = client.SendAsync(request).Result;
                if (response.StatusCode == HttpStatusCode.NonAuthoritativeInformation)
                {
                    string accessToken = Refresh_AccessToken(Details.refresh_token);
                    GetAccounts(MemberID, new AccessDetails() { access_token = accessToken });
                }
                else if (response.IsSuccessStatusCode)
                {
                    string result = response.Content.ReadAsStringAsync().Result;
                    Accounts = JsonConvert.DeserializeObject<Accounts.AccountList>(result);
                }
                else
                {
                    var errorMessage = response.Content.ReadAsStringAsync();
                    Accounts = null;
                }
            }
            catch (Exception ex)
            {
                return Accounts;
            }
            return Accounts;
        }

        public ActionResult Logout()
        {
            string clientId = System.Configuration.ConfigurationManager.AppSettings["ClientId"];

            string logout = "https://login.live.com/oauth20_logout.srf?client_id={0}&redirect_uri=https://vstsrollupservice.ecanarys.com/";
            logout = string.Format(logout, clientId);
            //return Redirect("https://createwebhooks.azurewebsites.net");
            return Redirect("https://vstsrollupservice.ecanarys.com");

            

        }
    }
}