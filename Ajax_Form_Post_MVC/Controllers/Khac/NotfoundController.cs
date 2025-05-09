using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Form_Post_MVC.Controllers
{
    public class NotfoundController : Controller
    {
        // GET: Notfound
        public ActionResult Index()
        {
            ViewBag.TitleError = TempData["TitleError"] ?? "";
            ViewBag.DetailError = TempData["DetailError"] ?? "";
            return PartialView();
        }
    }
}