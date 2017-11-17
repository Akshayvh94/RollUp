using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VstsConnector.Models;

namespace RollUpApi.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index(ProjectCount pro)
        {
            ViewBag.Title = "Home Page";

            return View(pro);
        }
    }
}
