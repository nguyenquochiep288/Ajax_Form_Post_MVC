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
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using DatabaseTHP.StoredProcedure.Parameter;
using System.Data;
using System.IO;

namespace MVC_QuanLyTHP.Controllers
{
    public class Payroll_KPI_Sale_TempController : Controller
    {

        // GET: Payment
        public ActionResult Index()
        {
            try
            {
                if (Utility.KiemTra())
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (!Utility.KiemTraQuyen(API.ct_PhieuChi, API.Xem))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                ApiResponse apiResponse = new ApiResponse();
                IPagedList<v_Tinh_KPI_KinhDoanh> lstpage = (new List<v_Tinh_KPI_KinhDoanh>()).ToList().ToPagedList(1, Utility.GetPageSize());
                v_v_Tinh_KPI_KinhDoanh ct_PhieuChi = new v_v_Tinh_KPI_KinhDoanh();
                ct_PhieuChi.IPagedList = lstpage;
                ct_PhieuChi.lstdm_NhanVien = new List<v_dm_NhanVien>();
                ct_PhieuChi.lstdm_NhanVien = Utility.GetListData<v_dm_NhanVien>(API.dm_NhanVien, "", "", Utility.LOC_ID).Data as List<v_dm_NhanVien>;
                ct_PhieuChi.lstweb_NhomQuyen = new List<v_web_NhomQuyen>();
                ct_PhieuChi.lstweb_NhomQuyen = Utility.GetListData<v_web_NhomQuyen>(API.web_NhomQuyen, "", "", Utility.LOC_ID).Data as List<v_web_NhomQuyen>;

                ct_PhieuChi.TUNGAY = DateTime.Now;
                ct_PhieuChi.DENNGAY = DateTime.Now;
                ViewBag.PermissionEdit = Utility.KiemTraQuyen(API.ct_PhieuChi, API.Edit);
                ViewBag.PermissionDelete = Utility.KiemTraQuyen(API.ct_PhieuChi, API.Delete);
                ViewBag.PermissionCreate = Utility.KiemTraQuyen(API.ct_PhieuChi, API.Create);
                return View(ct_PhieuChi);
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
                TempData["TitleError"] = API.TitleTryCatch;
                TempData["DetailError"] = ex.Message;
                return RedirectToAction("Index", "Notfound");
            }
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Index(SP_Parameter objParameter)
        {
            try
            {
                if (Utility.KiemTra())
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (!Utility.KiemTraQuyen(API.ct_PhieuChi, API.Xem))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                ApiResponse apiResponse = new ApiResponse();
                IPagedList<v_Tinh_KPI_KinhDoanh> lstpage = (new List<v_Tinh_KPI_KinhDoanh>()).ToList().ToPagedList(1, Utility.GetPageSize());
                objParameter.LOC_ID = Utility.LOC_ID;
                apiResponse = Utility.Edit<SP_Parameter>("PutKPI_Tam", objParameter, API.dm_KPI_KinhDoanh);
                if (!apiResponse.Success)
                {
                    TempData["TitleError"] = apiResponse.Message;
                    return RedirectToAction("Index", "Notfound");
                }

                var lst = JsonConvert.DeserializeObject<List<v_Tinh_KPI_KinhDoanh>>(apiResponse.Data.ToString());
                lstpage = lst.ToPagedList(1, lst.Count() > 0 ? lst.Count() : 50);
                v_v_Tinh_KPI_KinhDoanh ct_PhieuChi = new v_v_Tinh_KPI_KinhDoanh();
                ct_PhieuChi.IPagedList = lstpage;
                ct_PhieuChi.lstdm_NhanVien = new List<v_dm_NhanVien>();
                ct_PhieuChi.lstdm_NhanVien = Utility.GetListData<v_dm_NhanVien>(API.dm_NhanVien, "", "", Utility.LOC_ID).Data as List<v_dm_NhanVien>;
                ct_PhieuChi.lstweb_NhomQuyen = new List<v_web_NhomQuyen>();
                ct_PhieuChi.lstweb_NhomQuyen = Utility.GetListData<v_web_NhomQuyen>(API.web_NhomQuyen, "", "", Utility.LOC_ID).Data as List<v_web_NhomQuyen>;

                ct_PhieuChi.TUNGAY = DateTime.Now;
                ct_PhieuChi.DENNGAY = DateTime.Now;
                ViewBag.PermissionEdit = Utility.KiemTraQuyen(API.ct_PhieuChi, API.Edit);
                ViewBag.PermissionDelete = Utility.KiemTraQuyen(API.ct_PhieuChi, API.Delete);
                ViewBag.PermissionCreate = Utility.KiemTraQuyen(API.ct_PhieuChi, API.Create);
                return View(ct_PhieuChi);
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