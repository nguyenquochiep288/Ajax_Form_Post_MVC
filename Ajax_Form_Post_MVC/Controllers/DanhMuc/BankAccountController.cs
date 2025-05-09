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

namespace MVC_QuanLyTHP.Controllers
{
    public class BankAccountController : Controller
    {

        // GET: BankAccount
        public ActionResult Index(int Page = 1, string SearchString = "", string ShowSearchValue = "")
        {
            try
            {
                if (Utility.KiemTra())
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (!Utility.KiemTraQuyen(API.dm_TaiKhoanNganHang, API.Xem))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                ShowSearchValue = Utility.GetShowSearchValue<dm_TaiKhoanNganHang>(ShowSearchValue);
                var apiResponse = Utility.GetListData<v_dm_TaiKhoanNganHang>(API.dm_TaiKhoanNganHang, ShowSearchValue, SearchString, Utility.LOC_ID);
                if (!apiResponse.Success)
                {
                    TempData["TitleError"] = apiResponse.Message;
                    return RedirectToAction("Index", "Notfound");
                }
                IPagedList<v_dm_TaiKhoanNganHang> lstpage = (apiResponse.Data as List<v_dm_TaiKhoanNganHang>).ToPagedList(Page, Utility.GetPageSize());

                v_v_dm_TaiKhoanNganHang dm_TaiKhoanNganHang = new v_v_dm_TaiKhoanNganHang();
                dm_TaiKhoanNganHang.IPagedList = lstpage;
                //@LSTKHOAINGOAI

                ViewBag.searchValue = SearchString;
                ViewBag.showsearchValue = ShowSearchValue;

                ViewBag.PermissionEdit = Utility.KiemTraQuyen(API.dm_TaiKhoanNganHang, API.Edit);
                ViewBag.PermissionDelete = Utility.KiemTraQuyen(API.dm_TaiKhoanNganHang, API.Delete);
                ViewBag.PermissionCreate = Utility.KiemTraQuyen(API.dm_TaiKhoanNganHang, API.Create);
                return View(dm_TaiKhoanNganHang);
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
                TempData["TitleError"] = API.TitleTryCatch;
                TempData["DetailError"] = ex.Message;
                return RedirectToAction("Index", "Notfound");
            }
        }

        // GET: BankAccount/Create
        public ActionResult Create(int type = 2)
        {
            try
            {
                Session[Sessions.IntWidth] = type;
                if (Utility.KiemTra())
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (!Utility.KiemTraQuyen(API.dm_TaiKhoanNganHang, API.Create))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                v_v_dm_TaiKhoanNganHang dm_TaiKhoanNganHang = new v_v_dm_TaiKhoanNganHang();
                dm_TaiKhoanNganHang.LOC_ID = Utility.LOC_ID;
                dm_TaiKhoanNganHang.ID_NGUOITAO = Session[Sessions.idUser].ToString();
                dm_TaiKhoanNganHang.THOIGIANTHEM = Utility.CurrentTime;

                dm_TaiKhoanNganHang.ID = Guid.NewGuid().ToString();
                //@LSTKHOAINGOAI
                return View(dm_TaiKhoanNganHang);
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
                TempData["TitleError"] = API.TitleTryCatch;
                TempData["DetailError"] = ex.Message;
                return RedirectToAction("Index", "Notfound");
            }
        }

        // POST: BankAccount/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,LOC_ID,MA,NAME,SOTAIKHOAN,MANGANHANG,TENNGANHANG,CHINHANH,TINHTP,CHUTAIKHOAN,ISDEFAULT,ISACTIVE,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO,CONGNODAUKY")] v_v_dm_TaiKhoanNganHang dm_TaiKhoanNganHang)
        {
            try
            {
                if (Utility.KiemTra())
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (!Utility.KiemTraQuyen(API.dm_TaiKhoanNganHang, API.Create))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                if (ModelState.IsValid)
                {
                    dm_TaiKhoanNganHang.LOC_ID = Utility.LOC_ID;
                    dm_TaiKhoanNganHang.ID_NGUOITAO = Session[Sessions.idUser].ToString();
                    dm_TaiKhoanNganHang.THOIGIANTHEM = Utility.CurrentTime;

                    //@ConvertObjectUnicodeToTCVN3
                    var apiResponse = Utility.Create<dm_TaiKhoanNganHang>(dm_TaiKhoanNganHang, API.dm_TaiKhoanNganHang);
                    if (apiResponse.Success)
                        return RedirectToAction("Index");
                    else
                    {
                        ModelState.AddModelError(string.Empty, apiResponse.Message);
                        if (apiResponse.CheckValue)
                            ViewBag.ID = Guid.NewGuid().ToString();
                    }
                }
                return View(dm_TaiKhoanNganHang);
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
                TempData["TitleError"] = API.TitleTryCatch;
                TempData["DetailError"] = ex.Message;
                return RedirectToAction("Index", "Notfound");
            }
        }

        // GET: BankAccount/Edit/5
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
                if (!Utility.KiemTraQuyen(API.dm_TaiKhoanNganHang, API.Edit))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                v_v_dm_TaiKhoanNganHang dm_TaiKhoanNganHang = new v_v_dm_TaiKhoanNganHang();
                if (!string.IsNullOrEmpty(id))
                {
                    apiResponse = Utility.GetDetail<v_v_dm_TaiKhoanNganHang>(Utility.LOC_ID + "/" + id, API.dm_TaiKhoanNganHang);
                    if (!apiResponse.Success)
                    {
                        TempData["TitleError"] = apiResponse.Message;
                        return RedirectToAction("Index", "Notfound");
                    }
                    if (apiResponse.Data != null)
                        dm_TaiKhoanNganHang = apiResponse.Data as v_v_dm_TaiKhoanNganHang;
                }
                //@ConvertObjectTCVN3ToUnicode
                //@LSTKHOAINGOAI
                return View(dm_TaiKhoanNganHang);
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
                TempData["TitleError"] = API.TitleTryCatch;
                TempData["DetailError"] = ex.Message;
                return RedirectToAction("Index", "Notfound");
            }
        }

        // POST: BankAccount/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,LOC_ID,MA,NAME,SOTAIKHOAN,MANGANHANG,TENNGANHANG,CHINHANH,TINHTP,CHUTAIKHOAN,ISDEFAULT,ISACTIVE,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO,CONGNODAUKY")] v_v_dm_TaiKhoanNganHang dm_TaiKhoanNganHang)
        {
            try
            {
                if (Utility.KiemTra())
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (!Utility.KiemTraQuyen(API.dm_TaiKhoanNganHang, API.Edit))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                if (ModelState.IsValid)
                {
                    dm_TaiKhoanNganHang.LOC_ID = Utility.LOC_ID;
                    dm_TaiKhoanNganHang.ID_NGUOISUA = Session[Sessions.idUser].ToString();
                    dm_TaiKhoanNganHang.THOIGIANSUA = Utility.CurrentTime;
                    //@ConvertObjectUnicodeToTCVN3
                    var apiResponse = Utility.Edit<v_dm_TaiKhoanNganHang>(Utility.LOC_ID + "/" + dm_TaiKhoanNganHang.MA, dm_TaiKhoanNganHang, API.dm_TaiKhoanNganHang);
                    if (apiResponse.Success)
                        return RedirectToAction("Index");
                    else
                        ModelState.AddModelError(string.Empty, apiResponse.Message);
                }
                return View(dm_TaiKhoanNganHang);
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
                TempData["TitleError"] = API.TitleTryCatch;
                TempData["DetailError"] = ex.Message;
                return RedirectToAction("Index", "Notfound");
            }
        }

        // POST: BankAccount/Delete/5
        [HttpPost]
        public ActionResult Delete(string id)
        {
            try
            {
                if (Utility.KiemTra())
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (!Utility.KiemTraQuyen(API.dm_TaiKhoanNganHang, API.Delete))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                var apiResponse = Utility.Delete<v_dm_TaiKhoanNganHang>(Utility.LOC_ID + "/" + id, API.dm_TaiKhoanNganHang);
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
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Admin");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                if (!Utility.KiemTraQuyen(API.dm_TaiKhoanNganHang, API.Create))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                v_v_dm_TaiKhoanNganHang dm_TaiKhoanNganHang = new v_v_dm_TaiKhoanNganHang();
                apiResponse.Success = true;
                dm_TaiKhoanNganHang.LOC_ID = Utility.LOC_ID;
                dm_TaiKhoanNganHang.ID = Guid.NewGuid().ToString();
                apiResponse.Detail = Utility.ConvertobjectTo<v_v_dm_TaiKhoanNganHang>(dm_TaiKhoanNganHang);
                return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
                TempData["TitleError"] = API.TitleTryCatch;
                TempData["DetailError"] = ex.Message;
                apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
            }
        }

        // POST: Menu/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult CreatePopup([Bind(Include = "ID,LOC_ID,MA,NAME,SOTAIKHOAN,MANGANHANG,TENNGANHANG,CHINHANH,TINHTP,CHUTAIKHOAN,ISDEFAULT,ISACTIVE,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO,CONGNODAUKY")] v_v_dm_TaiKhoanNganHang dm_TaiKhoanNganHang)
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                if (Utility.KiemTra())
                {
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Admin");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                if (!Utility.KiemTraQuyen(API.dm_TaiKhoanNganHang, API.Create))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                if (ModelState.IsValid)
                {
                    dm_TaiKhoanNganHang.LOC_ID = Utility.LOC_ID;
                    dm_TaiKhoanNganHang.ID_NGUOITAO = Session[Sessions.idUser].ToString();
                    dm_TaiKhoanNganHang.THOIGIANTHEM = Utility.CurrentTime;

                    apiResponse = Utility.Create<dm_TaiKhoanNganHang>(dm_TaiKhoanNganHang, API.dm_TaiKhoanNganHang);
                    if (apiResponse.Success)
                    {
                        apiResponse.NewID = Guid.NewGuid().ToString();
                        if (apiResponse.Data != null)
                            dm_TaiKhoanNganHang = JsonConvert.DeserializeObject<v_v_dm_TaiKhoanNganHang>(apiResponse.Data.ToString());
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
                    apiResponse.Data = Utility.GetModelState(ModelState, API.dm_TaiKhoanNganHang);
                }
                apiResponse.ID = dm_TaiKhoanNganHang.ID;
                apiResponse.Detail = Utility.ConvertobjectToView<v_dm_TaiKhoanNganHang>(dm_TaiKhoanNganHang);
                return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
                TempData["TitleError"] = API.TitleTryCatch;
                TempData["DetailError"] = ex.Message;
                apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
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
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Admin");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                if (!Utility.KiemTraQuyen(API.dm_TaiKhoanNganHang, API.Edit))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                v_v_dm_TaiKhoanNganHang dm_TaiKhoanNganHang = new v_v_dm_TaiKhoanNganHang();
                if (!string.IsNullOrEmpty(id))
                {
                    apiResponse = Utility.GetDetail<v_v_dm_TaiKhoanNganHang>(Utility.LOC_ID + "/" + id, API.dm_TaiKhoanNganHang);
                    if (!apiResponse.Success)
                    {
                        TempData["TitleError"] = apiResponse.Message;
                        apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                        return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                    }
                    if (apiResponse.Data != null)
                        dm_TaiKhoanNganHang = apiResponse.Data as v_v_dm_TaiKhoanNganHang;
                }
                apiResponse.Success = true;
                apiResponse.Detail = Utility.ConvertobjectTo<v_v_dm_TaiKhoanNganHang>(dm_TaiKhoanNganHang);
                return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
                TempData["TitleError"] = API.TitleTryCatch;
                TempData["DetailError"] = ex.Message;
                apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
            }
        }

        // POST: Menu/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult EditPopup([Bind(Include = "ID,LOC_ID,MA,NAME,SOTAIKHOAN,MANGANHANG,TENNGANHANG,CHINHANH,TINHTP,CHUTAIKHOAN,ISDEFAULT,ISACTIVE,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO,CONGNODAUKY")] v_v_dm_TaiKhoanNganHang dm_TaiKhoanNganHang)
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                if (Utility.KiemTra())
                {
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Admin");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                if (!Utility.KiemTraQuyen(API.dm_TaiKhoanNganHang, API.Edit))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                if (ModelState.IsValid)
                {
                    dm_TaiKhoanNganHang.LOC_ID = Utility.LOC_ID;
                    dm_TaiKhoanNganHang.ID_NGUOISUA = Session[Sessions.idUser].ToString();
                    dm_TaiKhoanNganHang.THOIGIANSUA = Utility.CurrentTime;

                    apiResponse = Utility.Edit<v_dm_TaiKhoanNganHang>(Utility.LOC_ID + "/" + dm_TaiKhoanNganHang.MA, dm_TaiKhoanNganHang, API.dm_TaiKhoanNganHang);
                    if (apiResponse.Success)
                    {
                        //return RedirectToAction("Index");
                        apiResponse.ID = dm_TaiKhoanNganHang.ID;
                        if (apiResponse.Data != null)
                            dm_TaiKhoanNganHang = JsonConvert.DeserializeObject<v_v_dm_TaiKhoanNganHang>(apiResponse.Data.ToString());
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, apiResponse.Message);
                    }
                }
                else
                {
                    apiResponse.Success = false;
                    apiResponse.Data = Utility.GetModelState(ModelState, API.dm_TaiKhoanNganHang);
                }
                apiResponse.Detail = Utility.ConvertobjectToView<v_dm_TaiKhoanNganHang>(dm_TaiKhoanNganHang);
                return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
                TempData["TitleError"] = API.TitleTryCatch;
                TempData["DetailError"] = ex.Message;
                apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
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
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Admin");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                if (!Utility.KiemTraQuyen(API.dm_TaiKhoanNganHang, API.Delete))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                apiResponse = Utility.Delete<v_dm_TaiKhoanNganHang>(Utility.LOC_ID + "/" + id, API.dm_TaiKhoanNganHang);
                apiResponse.ID = id;
                return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
                TempData["TitleError"] = API.TitleTryCatch;
                TempData["DetailError"] = ex.Message;
                apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
            }
        }
        #endregion
    }
}