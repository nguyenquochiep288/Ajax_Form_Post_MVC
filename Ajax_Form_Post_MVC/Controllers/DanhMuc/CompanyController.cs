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
    public class CompanyController : Controller
    {

        // GET: Company
        public ActionResult Index(int Page = 1, string SearchString = "", string ShowSearchValue = "")
        {
            try
            {
                if (Utility.KiemTra())
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (!Utility.KiemTraQuyen(API.dm_CongTy, API.Xem))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                ShowSearchValue = Utility.GetShowSearchValue<dm_CongTy>(ShowSearchValue);
                var apiResponse = Utility.GetListData<v_dm_CongTy>(API.dm_CongTy, ShowSearchValue, SearchString);
                if (!apiResponse.Success)
                {
                    TempData["TitleError"] = apiResponse.Message;
                    return RedirectToAction("Index", "Notfound");
                }
                IPagedList<v_dm_CongTy> lstpage = (apiResponse.Data as List<v_dm_CongTy>).ToPagedList(Page, Utility.GetPageSize());

                v_v_dm_CongTy dm_CongTy = new v_v_dm_CongTy();
                dm_CongTy.IPagedList = lstpage;
                //@LSTKHOAINGOAI

                ViewBag.searchValue = SearchString;
                ViewBag.showsearchValue = ShowSearchValue;

                ViewBag.PermissionEdit = Utility.KiemTraQuyen(API.dm_CongTy, API.Edit);
                ViewBag.PermissionDelete = Utility.KiemTraQuyen(API.dm_CongTy, API.Delete);
                ViewBag.PermissionCreate = Utility.KiemTraQuyen(API.dm_CongTy, API.Create);
                return View(dm_CongTy);
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
                TempData["TitleError"] = API.TitleTryCatch;
                TempData["DetailError"] = ex.Message;
                return RedirectToAction("Index", "Notfound");
            }
        }

        // GET: Company/Create
        public ActionResult Create(int type = 2)
        {
            try
            {
                Session[Sessions.IntWidth] = type;
                if (Utility.KiemTra())
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (!Utility.KiemTraQuyen(API.dm_CongTy, API.Create))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                v_v_dm_CongTy dm_CongTy = new v_v_dm_CongTy();

                dm_CongTy.ID = Guid.NewGuid().ToString();
                //@LSTKHOAINGOAI
                return View(dm_CongTy);
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
                TempData["TitleError"] = API.TitleTryCatch;
                TempData["DetailError"] = ex.Message;
                return RedirectToAction("Index", "Notfound");
            }
        }

        // POST: Company/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,MA,NAME,ADDRESS,TEL,FAX,CONTACT,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO,ISACTIVE")] v_v_dm_CongTy dm_CongTy)
        {
            try
            {
                if (Utility.KiemTra())
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (!Utility.KiemTraQuyen(API.dm_CongTy, API.Create))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                if (ModelState.IsValid)
                {

                    //@ConvertObjectUnicodeToTCVN3
                    var apiResponse = Utility.Create<dm_CongTy>(dm_CongTy, API.dm_CongTy);
                    if (apiResponse.Success)
                        return RedirectToAction("Index");
                    else
                    {
                        ModelState.AddModelError(string.Empty, apiResponse.Message);
                        if (apiResponse.CheckValue)
                            ViewBag.ID = Guid.NewGuid().ToString();
                    }
                }
                return View(dm_CongTy);
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
                TempData["TitleError"] = API.TitleTryCatch;
                TempData["DetailError"] = ex.Message;
                return RedirectToAction("Index", "Notfound");
            }
        }

        // GET: Company/Edit/5
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
                if (!Utility.KiemTraQuyen(API.dm_CongTy, API.Edit))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                if (!string.IsNullOrEmpty(id))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                v_v_dm_CongTy dm_CongTy = new v_v_dm_CongTy();
                if (!string.IsNullOrEmpty(id))
                {
                    apiResponse = Utility.GetDetail<v_v_dm_CongTy>(id, API.dm_CongTy);
                    if (!apiResponse.Success)
                    {
                        TempData["TitleError"] = apiResponse.Message;
                        return RedirectToAction("Index", "Notfound");
                    }
                    if (apiResponse.Data != null)
                        dm_CongTy = apiResponse.Data as v_v_dm_CongTy;
                }
                //@ConvertObjectTCVN3ToUnicode
                //@LSTKHOAINGOAI
                return View(dm_CongTy);
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
                TempData["TitleError"] = API.TitleTryCatch;
                TempData["DetailError"] = ex.Message;
                return RedirectToAction("Index", "Notfound");
            }
        }

        // POST: Company/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,MA,NAME,ADDRESS,TEL,FAX,CONTACT,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO,ISACTIVE")] v_v_dm_CongTy dm_CongTy)
        {
            try
            {
                if (Utility.KiemTra())
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (!Utility.KiemTraQuyen(API.dm_CongTy, API.Edit))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                if (ModelState.IsValid)
                {
                    //@ConvertObjectUnicodeToTCVN3
                    var apiResponse = Utility.Edit<v_dm_CongTy>(dm_CongTy.MA, dm_CongTy, API.dm_CongTy);
                    if (apiResponse.Success)
                        return RedirectToAction("Index");
                    else
                        ModelState.AddModelError(string.Empty, apiResponse.Message);
                }
                return View(dm_CongTy);
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
                TempData["TitleError"] = API.TitleTryCatch;
                TempData["DetailError"] = ex.Message;
                return RedirectToAction("Index", "Notfound");
            }
        }

        // POST: Company/Delete/5
        [HttpPost]
        public ActionResult Delete(string id)
        {
            try
            {
                if (Utility.KiemTra())
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (!Utility.KiemTraQuyen(API.dm_CongTy, API.Delete))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                if (!string.IsNullOrEmpty(id))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                var apiResponse = Utility.Delete<v_dm_CongTy>(id, API.dm_CongTy);
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
                if (!Utility.KiemTraQuyen(API.dm_CongTy, API.Create))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                v_v_dm_CongTy dm_CongTy = new v_v_dm_CongTy();
                apiResponse.Success = true;
                dm_CongTy.ID = Guid.NewGuid().ToString();
                apiResponse.Detail = Utility.ConvertobjectTo<v_v_dm_CongTy>(dm_CongTy);
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
        public ActionResult CreatePopup([Bind(Include = "ID,MA,NAME,ADDRESS,TEL,FAX,CONTACT,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO,ISACTIVE")] v_v_dm_CongTy dm_CongTy)
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                if (Utility.KiemTra())
                {
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Admin");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                if (!Utility.KiemTraQuyen(API.dm_CongTy, API.Create))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                if (ModelState.IsValid)
                {

                    apiResponse = Utility.Create<dm_CongTy>(dm_CongTy, API.dm_CongTy);
                    if (apiResponse.Success)
                    {
                        apiResponse.NewID = Guid.NewGuid().ToString();
                        if (apiResponse.Data != null)
                            dm_CongTy = JsonConvert.DeserializeObject<v_v_dm_CongTy>(apiResponse.Data.ToString());
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
                    apiResponse.Data = Utility.GetModelState(ModelState, API.dm_CongTy);
                }
                apiResponse.ID = dm_CongTy.ID;
                apiResponse.Detail = Utility.ConvertobjectToView<v_dm_CongTy>(dm_CongTy);
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
                if (!Utility.KiemTraQuyen(API.dm_CongTy, API.Edit))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                v_v_dm_CongTy dm_CongTy = new v_v_dm_CongTy();
                if (!string.IsNullOrEmpty(id))
                {
                    apiResponse = Utility.GetDetail<v_v_dm_CongTy>(id, API.dm_CongTy);
                    if (!apiResponse.Success)
                    {
                        TempData["TitleError"] = apiResponse.Message;
                        apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                        return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                    }
                    if (apiResponse.Data != null)
                        dm_CongTy = apiResponse.Data as v_v_dm_CongTy;
                }
                apiResponse.Success = true;
                apiResponse.Detail = Utility.ConvertobjectTo<v_v_dm_CongTy>(dm_CongTy);
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
        public ActionResult EditPopup([Bind(Include = "ID,MA,NAME,ADDRESS,TEL,FAX,CONTACT,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO,ISACTIVE")] v_v_dm_CongTy dm_CongTy)
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                if (Utility.KiemTra())
                {
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Admin");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                if (!Utility.KiemTraQuyen(API.dm_CongTy, API.Edit))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                if (ModelState.IsValid)
                {
                    apiResponse = Utility.Edit<v_v_dm_CongTy>(dm_CongTy.MA, dm_CongTy, API.dm_CongTy);
                    if (apiResponse.Success)
                    {
                        //return RedirectToAction("Index");
                        apiResponse.ID = dm_CongTy.ID;
                        if (apiResponse.Data != null)
                            dm_CongTy = JsonConvert.DeserializeObject<v_v_dm_CongTy>(apiResponse.Data.ToString());
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, apiResponse.Message);
                    }
                }
                else
                {
                    apiResponse.Success = false;
                    apiResponse.Data = Utility.GetModelState(ModelState, API.dm_CongTy);
                }
                apiResponse.Detail = Utility.ConvertobjectToView<v_dm_CongTy>(dm_CongTy);
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
                if (!Utility.KiemTraQuyen(API.dm_CongTy, API.Delete))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                apiResponse = Utility.Delete<v_dm_CongTy>(id, API.dm_CongTy);
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