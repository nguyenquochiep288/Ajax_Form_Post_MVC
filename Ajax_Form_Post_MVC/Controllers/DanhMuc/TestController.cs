using DatabaseTHP;
using MVC_QuanLyTHP.Models;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Configuration;
using MVC_QuanLyTHP.Class;
using System.Web.UI;
using System.Collections.Generic;
using System;
using System.Web.DynamicData;
using PagedList;
using Syncfusion.EJ2.Linq;
using System.Reflection;
using System.Web.Routing;
using DatabaseTHP.Class;
using Newtonsoft.Json;
using SixLabors.ImageSharp.PixelFormats;

namespace MVC_QuanLyTHP.Controllers
{
    public class TestController : Controller
    {

        // GET: Area
        public ActionResult Index()
        {
            try
            {
                double lat1 = 10.7045182;
                double lon1 = 106.7130296;
                double lat2 = 10.8314986;
                double lon2 = 106.6111826;
                var khoangcach = API.CalculateDistance(lat1,lon1, lat2, lon2);
                return View();
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
                TempData["TitleError"] = API.TitleTryCatch;
                TempData["DetailError"] = ex.Message;
                return RedirectToAction("Index", "Notfound");
            }
        }
    }
}