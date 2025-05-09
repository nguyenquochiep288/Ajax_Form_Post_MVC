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
using static System.Data.Entity.Infrastructure.Design.Executor;
using System.Net.NetworkInformation;
using System.Net.Http;
using System.Threading.Tasks;
using System.Diagnostics;

namespace MVC_QuanLyTHP.Controllers
{
    public class AnnualLeaveController : Controller
    {

        // GET: Area
        public ActionResult Index(int Page = 1, string SearchString = "", string ShowSearchValue = "")
        {
            try
            {
                if (Utility.KiemTra())
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (!Utility.KiemTraQuyen(API.nv_PhepNam, API.Xem))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                ShowSearchValue = Utility.GetShowSearchValue<nv_PhepNam>(ShowSearchValue);
                var apiResponse = Utility.GetListData<v_nv_PhepNam>(API.nv_PhepNam, ShowSearchValue, SearchString, Utility.LOC_ID);
                if (!apiResponse.Success)
                {
                    TempData["TitleError"] = apiResponse.Message;
                    return RedirectToAction("Index", "Notfound");
                }
                IPagedList<v_nv_PhepNam> lstpage = (apiResponse.Data as List<v_nv_PhepNam>).ToPagedList(Page, Utility.GetPageSize());

                v_v_nv_PhepNam nv_PhepNam = new v_v_nv_PhepNam();
                nv_PhepNam.IPagedList = lstpage;
                //@LSTKHOAINGOAI

                ViewBag.searchValue = SearchString;
                ViewBag.showsearchValue = ShowSearchValue;

                ViewBag.PermissionEdit = Utility.KiemTraQuyen(API.nv_PhepNam, API.Edit);
                ViewBag.PermissionDelete = Utility.KiemTraQuyen(API.nv_PhepNam, API.Delete);
                ViewBag.PermissionCreate = Utility.KiemTraQuyen(API.nv_PhepNam, API.Create);
                return View(nv_PhepNam);
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
                TempData["TitleError"] = API.TitleTryCatch;
                TempData["DetailError"] = ex.Message;
                return RedirectToAction("Index", "Notfound");
            }
        }

        // GET: AnnualLeave/Create
        public ActionResult Create(int type = 2)
        {
            try
            {
                Session[Sessions.IntWidth] = type;
                if (Utility.KiemTra())
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (!Utility.KiemTraQuyen(API.nv_PhepNam, API.Create))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                v_v_nv_PhepNam nv_PhepNam = new v_v_nv_PhepNam();
                nv_PhepNam.LOC_ID = Utility.LOC_ID;
                nv_PhepNam.ID_NGUOITAO = Session[Sessions.idUser].ToString();
                nv_PhepNam.THOIGIANTHEM = Utility.CurrentTime;

                nv_PhepNam.ID = Guid.NewGuid().ToString();
                nv_PhepNam.lstdm_NhanVien = new List<ComboboxFrom>();
                nv_PhepNam.lstdm_NhanVien = Utility.GetListData<ComboboxFrom>(API.AspNetUser, "", "", "").Data as List<ComboboxFrom>;

                return View(nv_PhepNam);
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
                TempData["TitleError"] = API.TitleTryCatch;
                TempData["DetailError"] = ex.Message;
                return RedirectToAction("Index", "Notfound");
            }
        }

        // POST: AnnualLeave/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "LOC_ID,ID,ID_NHANVIEN,ID_THANGLUONG,NAM,NGAYBATDAU,NGAYKETTHUC,SONGAYPHEP,GHICHU,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO,SONGAYPHEPDADUNG")] v_nv_PhepNam nv_PhepNam)
        {
            try
            {
                if (Utility.KiemTra())
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (!Utility.KiemTraQuyen(API.nv_PhepNam, API.Create))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                if (ModelState.IsValid)
                {
                    nv_PhepNam.LOC_ID = Utility.LOC_ID;
                    nv_PhepNam.ID_NGUOITAO = Session[Sessions.idUser].ToString();
                    nv_PhepNam.THOIGIANTHEM = Utility.CurrentTime;

                    //@ConvertObjectUnicodeToTCVN3
                    var apiResponse = Utility.Create<nv_PhepNam>(nv_PhepNam, API.nv_PhepNam);
                    if (apiResponse.Success)
                        return RedirectToAction("Index");
                    else
                    {
                        ModelState.AddModelError(string.Empty, apiResponse.Message);
                        if (apiResponse.CheckValue)
                            ViewBag.ID = Guid.NewGuid().ToString();
                    }
                }
                return View(nv_PhepNam);
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
                TempData["TitleError"] = API.TitleTryCatch;
                TempData["DetailError"] = ex.Message;
                return RedirectToAction("Index", "Notfound");
            }
        }

        // GET: AnnualLeave/Edit/5
        public ActionResult Edit(string id, int type = 2)
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                Session[Sessions.IntWidth] = type;
                if (Utility.KiemTra())
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (!Utility.KiemTraQuyen(API.nv_PhepNam, API.Edit))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                v_v_nv_PhepNam nv_PhepNam = new v_v_nv_PhepNam();
                if (!string.IsNullOrEmpty(id))
                {
                    apiResponse = Utility.GetDetail<v_nv_PhepNam>(Utility.LOC_ID + "/" + id, API.nv_PhepNam);
                    if (!apiResponse.Success)
                    {
                        TempData["TitleError"] = apiResponse.Message;
                        return RedirectToAction("Index", "Notfound");
                    }
                    if (apiResponse.Data != null)
                        nv_PhepNam = apiResponse.Data as v_v_nv_PhepNam;
                }
                //@ConvertObjectTCVN3ToUnicode
                nv_PhepNam.lstdm_NhanVien = new List<ComboboxFrom>();
                nv_PhepNam.lstdm_NhanVien = Utility.GetListData<ComboboxFrom>(API.AspNetUser, "", "", "").Data as List<ComboboxFrom>;

                return View(nv_PhepNam);
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
                TempData["TitleError"] = API.TitleTryCatch;
                TempData["DetailError"] = ex.Message;
                return RedirectToAction("Index", "Notfound");
            }
        }

        // POST: AnnualLeave/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "LOC_ID,ID,ID_NHANVIEN,ID_THANGLUONG,NAM,NGAYBATDAU,NGAYKETTHUC,SONGAYPHEP,GHICHU,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO,SONGAYPHEPDADUNG")] v_nv_PhepNam nv_PhepNam)
        {
            try
            {
                if (Utility.KiemTra())
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (!Utility.KiemTraQuyen(API.nv_PhepNam, API.Edit))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                if (ModelState.IsValid)
                {
                    nv_PhepNam.LOC_ID = Utility.LOC_ID;
                    nv_PhepNam.ID_NGUOISUA = Session[Sessions.idUser].ToString();
                    nv_PhepNam.THOIGIANSUA = Utility.CurrentTime;
                    //@ConvertObjectUnicodeToTCVN3
                    var apiResponse = Utility.Edit<v_nv_PhepNam>(Utility.LOC_ID + "/" + nv_PhepNam.ID, nv_PhepNam, API.nv_PhepNam);
                    if (apiResponse.Success)
                        return RedirectToAction("Index");
                    else
                        ModelState.AddModelError(string.Empty, apiResponse.Message);
                }
                return View(nv_PhepNam);
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
                TempData["TitleError"] = API.TitleTryCatch;
                TempData["DetailError"] = ex.Message;
                return RedirectToAction("Index", "Notfound");
            }
        }

        // POST: AnnualLeave/Delete/5
        [HttpPost]
        public ActionResult Delete(string id)
        {
            try
            {
                if (Utility.KiemTra())
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (!Utility.KiemTraQuyen(API.nv_PhepNam, API.Delete))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                var apiResponse = Utility.Delete<v_nv_PhepNam>(Utility.LOC_ID + "/" + id, API.nv_PhepNam);
                if (apiResponse.Success)
                    return RedirectToAction("Index");
                else
                    ModelState.AddModelError(string.Empty, apiResponse.Message);
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
        #region Popup
        // GET: Menu/Create
        public ActionResult CreatePopup()
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                if (Utility.KiemTra())
                {
                    apiResponse.URL = Url.Action("Index", "Admin");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                if (!Utility.KiemTraQuyen(API.nv_PhepNam, API.Create))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                v_v_nv_PhepNam nv_PhepNam = new v_v_nv_PhepNam();
                apiResponse.Success = true;
                nv_PhepNam.LOC_ID = Utility.LOC_ID;
                nv_PhepNam.ID = Guid.NewGuid().ToString();
                nv_PhepNam.NAM = Utility.CurrentTime.Year;
                nv_PhepNam.NGAYBATDAU = new DateTime(Utility.CurrentTime.Year, 1, 1);
                nv_PhepNam.NGAYKETTHUC = new DateTime(Utility.CurrentTime.Year, 12, 31);
                nv_PhepNam.lstdm_NhanVien = new List<ComboboxFrom>();
                var lstdm_NhanVien = Utility.GetListData<ComboboxFrom>(API.AspNetUser, "", "", "").Data as List<ComboboxFrom>;
                foreach(var itm in lstdm_NhanVien)
                {
                    itm.ISACTIVE = true;
                }
                nv_PhepNam.lstdm_NhanVien = lstdm_NhanVien;
                apiResponse.Detail = Utility.ConvertobjectTo<v_v_nv_PhepNam>(nv_PhepNam);
                return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
                TempData["TitleError"] = API.TitleTryCatch;
                TempData["DetailError"] = ex.Message;
                apiResponse.URL = Url.Action("Index", "Notfound");
                return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
            }
        }

        // POST: Menu/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ValidateInput(false)]
        //[ValidateAntiForgeryToken]
        public ActionResult CreatePopup([Bind(Include = "ISALL,LOC_ID,ID,ID_NHANVIEN,ID_THANGLUONG,NAM,NGAYBATDAU,NGAYKETTHUC,SONGAYPHEP,GHICHU,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO,SONGAYPHEPDADUNG")] v_nv_PhepNam nv_PhepNam)
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                if (Utility.KiemTra())
                {
                    apiResponse.URL = Url.Action("Index", "Admin");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                if (!Utility.KiemTraQuyen(API.nv_PhepNam, API.Create))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                if(!nv_PhepNam.ISALL && string.IsNullOrEmpty( nv_PhepNam.ID_NHANVIEN))
                {
                    ModelState.AddModelError("ID_NHANVIEN", "Vui lòng chọn nhân viên!");
                }
                if (ModelState.IsValid)
                {
                    nv_PhepNam.LOC_ID = Utility.LOC_ID;
                    nv_PhepNam.ID_NGUOITAO = Session[Sessions.idUser].ToString();
                    nv_PhepNam.THOIGIANTHEM = Utility.CurrentTime;

                    apiResponse = Utility.Create<v_nv_PhepNam>(nv_PhepNam, API.nv_PhepNam);
                    if (apiResponse.Success)
                    {
                        apiResponse.NewID = Guid.NewGuid().ToString();
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, apiResponse.Message);
                        if (apiResponse.CheckValue)
                            apiResponse.NewID = Guid.NewGuid().ToString();
                    }
                }
                else
                {
                    apiResponse.Success = false;
                    apiResponse.Data = Utility.GetModelState(ModelState, API.nv_PhepNam);
                }
                apiResponse.ID = nv_PhepNam.ID;
                apiResponse.Detail = Utility.ConvertobjectTo<nv_PhepNam>(nv_PhepNam);
                return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
                TempData["TitleError"] = API.TitleTryCatch;
                TempData["DetailError"] = ex.Message;
                apiResponse.URL = Url.Action("Index", "Notfound");
                return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
            }
        }

        // GET: Menu/Edit/5
        public ActionResult EditPopup(string id)
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                if (Utility.KiemTra())
                {
                    apiResponse.URL = Url.Action("Index", "Admin");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                if (!Utility.KiemTraQuyen(API.nv_PhepNam, API.Edit))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                v_v_nv_PhepNam nv_PhepNam = new v_v_nv_PhepNam();
                if (!string.IsNullOrEmpty(id))
                {
                    apiResponse = Utility.GetDetail<v_v_nv_PhepNam>(Utility.LOC_ID + "/" + id, API.nv_PhepNam);
                    if (!apiResponse.Success)
                    {
                        TempData["TitleError"] = apiResponse.Message;
                        apiResponse.URL = Url.Action("Index", "Notfound");
                        return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                    }
                    if (apiResponse.Data != null)
                        nv_PhepNam = apiResponse.Data as v_v_nv_PhepNam;
                }
                apiResponse.Success = true;
                nv_PhepNam.lstdm_NhanVien = new List<ComboboxFrom>();
                var lstdm_NhanVien = Utility.GetListData<ComboboxFrom>(API.AspNetUser, "", "", "").Data as List<ComboboxFrom>;
                foreach (var itm in lstdm_NhanVien)
                {
                    itm.ISACTIVE = true;
                }  
                nv_PhepNam.lstdm_NhanVien = lstdm_NhanVien;
                apiResponse.Detail = Utility.ConvertobjectTo<v_v_nv_PhepNam>(nv_PhepNam);
                return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
                TempData["TitleError"] = API.TitleTryCatch;
                TempData["DetailError"] = ex.Message;
                apiResponse.URL = Url.Action("Index", "Notfound");
                return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
            }
        }

        // POST: Menu/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ValidateInput(false)]
        //[ValidateAntiForgeryToken]
        public ActionResult EditPopup([Bind(Include = "LOC_ID,ID,ID_NHANVIEN,ID_THANGLUONG,NAM,NGAYBATDAU,NGAYKETTHUC,SONGAYPHEP,GHICHU,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO,SONGAYPHEPDADUNG")] v_nv_PhepNam nv_PhepNam)
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                if (Utility.KiemTra())
                {
                    apiResponse.URL = Url.Action("Index", "Admin");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                if (!Utility.KiemTraQuyen(API.nv_PhepNam, API.Edit))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                if (ModelState.IsValid)
                {
                    nv_PhepNam.LOC_ID = Utility.LOC_ID;
                    nv_PhepNam.ID_NGUOISUA = Session[Sessions.idUser].ToString();
                    nv_PhepNam.THOIGIANSUA = Utility.CurrentTime;

                    apiResponse = Utility.Edit<v_nv_PhepNam>(Utility.LOC_ID + "/" + nv_PhepNam.ID_NHANVIEN + "/" + nv_PhepNam.NAM, nv_PhepNam, API.nv_PhepNam);
                    if (apiResponse.Success)
                    {
                        //return RedirectToAction("Index");
                        apiResponse.ID = nv_PhepNam.ID;
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, apiResponse.Message);
                    }
                }
                else
                {
                    apiResponse.Success = false;
                    apiResponse.Data = Utility.GetModelState(ModelState, API.nv_PhepNam);
                }
                apiResponse.Detail = Utility.ConvertobjectTo<v_nv_PhepNam>(nv_PhepNam, "dd/MM/yyyy");
                return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
                TempData["TitleError"] = API.TitleTryCatch;
                TempData["DetailError"] = ex.Message;
                apiResponse.URL = Url.Action("Index", "Notfound");
                return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
            }
        }

        // POST: Menu/Delete/5
        [HttpPost]
        public ActionResult DeletePopup(string id)
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                if (Utility.KiemTra())
                {
                    apiResponse.URL = Url.Action("Index", "Admin");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                if (!Utility.KiemTraQuyen(API.nv_PhepNam, API.Delete))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                apiResponse = Utility.Delete<v_nv_PhepNam>(Utility.LOC_ID + "/" + id, API.nv_PhepNam);
                apiResponse.ID = id;
                return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
                TempData["TitleError"] = API.TitleTryCatch;
                TempData["DetailError"] = ex.Message;
                apiResponse.URL = Url.Action("Index", "Notfound");
                return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
            }
        }
        #endregion
    }
}