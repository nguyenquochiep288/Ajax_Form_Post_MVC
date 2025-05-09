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
    public class MapController : Controller
    {
        // GET: Area
        public ActionResult Index(string ID, string LATITUDE, string LONGITUDE)
        {
            try
            {
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

        [HttpPost]
        public ActionResult GetListMap(string ID, string LATITUDE, string LONGITUDE)
        {
            List<Coordinates> lst = new List<Coordinates>();
            if (Utility.KiemTra())
            {
                return RedirectToAction("Index", "Admin");
            }

            Double lat1 = 0;
            Double lon1 = 0;
            if (!string.IsNullOrEmpty(ID))
            {
                v_dm_KhachHang dm_KhachHang = new v_dm_KhachHang();
                ApiResponse apiResponse = Utility.GetDetail<v_v_dm_KhachHang>(Utility.LOC_ID + "/" + ID, API.dm_KhachHang);
                if (!apiResponse.Success)
                {
                    TempData["TitleError"] = apiResponse.Message;
                    return RedirectToAction("Index", "Notfound");
                }
                if (apiResponse.Data != null)
                    dm_KhachHang = apiResponse.Data as v_v_dm_KhachHang;
                if (dm_KhachHang != null && dm_KhachHang.LATITUDE != null && dm_KhachHang.LATITUDE != 0)
                {
                    Coordinates newCoordinates = new Coordinates();
                    newCoordinates.Text = "<b>" + dm_KhachHang.NAME + "</b><br><b>Vĩ độ:</b> " + dm_KhachHang.LATITUDE + "<br><b>Kinh độ:</b> " + dm_KhachHang.LONGITUDE;
                    newCoordinates.Latitude = dm_KhachHang.LATITUDE;
                    newCoordinates.Longitude = dm_KhachHang.LONGITUDE;
                    lst.Add(newCoordinates);
                }
            }

            if (!string.IsNullOrEmpty(LATITUDE) && !string.IsNullOrEmpty(LONGITUDE))
            {
                try
                {
                    lat1 = Convert.ToDouble(LATITUDE.Replace(".", ","));
                    lon1 = Convert.ToDouble(LONGITUDE.Replace(".", ","));
                }
                catch { }
                Coordinates newCoordinates = new Coordinates();
                if ((lst.Count() == 0 && lat1 != 0 && lon1 != 0) || (lst.Count() > 0 && lst[0].Latitude != lat1 && lst[0].Longitude != lon1))
                {
                    newCoordinates.Text = "Vị trí hiện tại";
                    newCoordinates.Latitude = lat1;
                    newCoordinates.Longitude = lon1;
                    lst.Add(newCoordinates);
                }
            }
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SetMap(Double LATITUDE, Double LONGITUDE)
        {
            Utility.Latitude = LATITUDE;
            Utility.Longitude = LONGITUDE;
            return Json("", JsonRequestBehavior.AllowGet);
        }
    }
    public class Coordinates
    {
        public string Text { get; set; }
        public Double? Latitude { get; set; }
        public Double? Longitude { get; set; }
        
    }
}