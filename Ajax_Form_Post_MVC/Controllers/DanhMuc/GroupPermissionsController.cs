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

namespace MVC_QuanLyTHP.Controllers
{
    public class GroupPermissionsController : Controller
    {
       
        // GET: GroupPermissions
        public ActionResult Index(int Page = 1, string SearchString = "", string ShowSearchValue = "")
        {
            try
            {
                if (Utility.KiemTra())
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (!Utility.KiemTraQuyen(API.web_NhomQuyen, API.Xem))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                ShowSearchValue = Utility.GetShowSearchValue<web_NhomQuyen>(ShowSearchValue);
                var apiResponse = Utility.GetListData<v_web_NhomQuyen>(API.web_NhomQuyen, ShowSearchValue, SearchString, Utility.LOC_ID);
                if(!apiResponse.Success)
                {
                    TempData["TitleError"] = apiResponse.Message;
                    return RedirectToAction("Index", "Notfound");
                }
                IPagedList<v_web_NhomQuyen> lstpage = (apiResponse.Data as List<v_web_NhomQuyen>).ToPagedList(Page, Utility.GetPageSize());
				
	    v_v_web_NhomQuyen web_NhomQuyen = new v_v_web_NhomQuyen();
	    web_NhomQuyen.IPagedList= lstpage;
                //@LSTKHOAINGOAI
                
                ViewBag.searchValue = SearchString;
                ViewBag.showsearchValue = ShowSearchValue;

                ViewBag.PermissionEdit = Utility.KiemTraQuyen(API.web_NhomQuyen, API.Edit);
                ViewBag.PermissionDelete = Utility.KiemTraQuyen(API.web_NhomQuyen, API.Delete);
                ViewBag.PermissionCreate = Utility.KiemTraQuyen(API.web_NhomQuyen, API.Create);
                return View(web_NhomQuyen);
            }
            catch(Exception ex) 
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex.Message);
                TempData["TitleError"] = API.TitleTryCatch;
                TempData["DetailError"] = ex.Message;
                return RedirectToAction("Index", "Notfound");
            }
        }

        // GET: GroupPermissions/Create
        public ActionResult Create(int type = 2)
        {
            try
            {
                Session[Sessions.IntWidth] = type;
                if (Utility.KiemTra())
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (!Utility.KiemTraQuyen(API.web_NhomQuyen, API.Create))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                v_web_NhomQuyen web_NhomQuyen = new v_web_NhomQuyen();
                 web_NhomQuyen.LOC_ID = Utility.LOC_ID;
web_NhomQuyen.ID_NGUOITAO = Session[Sessions.idUser].ToString();
web_NhomQuyen.THOIGIANTHEM = Utility.CurrentTime;

                 web_NhomQuyen.ID = Guid.NewGuid().ToString();
                //@LSTKHOAINGOAI
                return View(web_NhomQuyen);
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex.Message);
                TempData["TitleError"] = API.TitleTryCatch;
                TempData["DetailError"] = ex.Message;
                return RedirectToAction("Index", "Notfound");
            }
        }

        // POST: GroupPermissions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,LOC_ID,MA,NAME,NOTE,ISPHANQUYEN,ISACTIVE,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO")] v_web_NhomQuyen web_NhomQuyen)
        {
            try
            {
                if (Utility.KiemTra())
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (!Utility.KiemTraQuyen(API.web_NhomQuyen, API.Create))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                 if (ModelState.IsValid)
                 {
                   web_NhomQuyen.LOC_ID = Utility.LOC_ID;
web_NhomQuyen.ID_NGUOITAO = Session[Sessions.idUser].ToString();
web_NhomQuyen.THOIGIANTHEM = Utility.CurrentTime;

                   //@ConvertObjectUnicodeToTCVN3
                    var apiResponse = Utility.Create<web_NhomQuyen>(web_NhomQuyen, API.web_NhomQuyen);
                    if (apiResponse.Success)
                        return RedirectToAction("Index");
                     else
                 {
                    ModelState.AddModelError(string.Empty, apiResponse.Message);
                    if (apiResponse.CheckValue)
                    ViewBag.ID = Guid.NewGuid().ToString();
                 }    
                }
                return View(web_NhomQuyen);
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex.Message);
                TempData["TitleError"] = API.TitleTryCatch;
                TempData["DetailError"] = ex.Message;
                return RedirectToAction("Index", "Notfound");
            }
        }

        // GET: GroupPermissions/Edit/5
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
                if (!Utility.KiemTraQuyen(API.web_NhomQuyen, API.Edit))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
				 v_web_NhomQuyen web_NhomQuyen = new v_web_NhomQuyen();
				 if (!string.IsNullOrEmpty(id))
				 {
					 apiResponse = Utility.GetDetail<v_web_NhomQuyen>(Utility.LOC_ID + "/" + id, API.web_NhomQuyen);
					 if (!apiResponse.Success)
					 {
						 TempData["TitleError"] = apiResponse.Message;
						 return RedirectToAction("Index", "Notfound");
					 }
					 if (apiResponse.Data != null)
						 web_NhomQuyen = apiResponse.Data as v_web_NhomQuyen;
				 }
                //@ConvertObjectTCVN3ToUnicode
                //@LSTKHOAINGOAI
                return View(web_NhomQuyen);
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex.Message);
                TempData["TitleError"] = API.TitleTryCatch;
                TempData["DetailError"] = ex.Message;
                return RedirectToAction("Index", "Notfound");
            }
        }

        // POST: GroupPermissions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,LOC_ID,MA,NAME,NOTE,ISPHANQUYEN,ISACTIVE,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO")] v_web_NhomQuyen web_NhomQuyen)
        {
            try
            {
                if (Utility.KiemTra())
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (!Utility.KiemTraQuyen(API.web_NhomQuyen, API.Edit))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                if (ModelState.IsValid)
                {
                      web_NhomQuyen.LOC_ID = Utility.LOC_ID;
           web_NhomQuyen.ID_NGUOISUA = Session[Sessions.idUser].ToString();
           web_NhomQuyen.THOIGIANSUA = Utility.CurrentTime;
                     //@ConvertObjectUnicodeToTCVN3
                    var apiResponse = Utility.Edit<v_web_NhomQuyen>(Utility.LOC_ID + "/" + web_NhomQuyen.MA, web_NhomQuyen, API.web_NhomQuyen);
                    if (apiResponse.Success)
                        return RedirectToAction("Index");
                    else
                        ModelState.AddModelError(string.Empty, apiResponse.Message);
                }
                return View(web_NhomQuyen);
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex.Message);
                TempData["TitleError"] = API.TitleTryCatch;
                TempData["DetailError"] = ex.Message;
                return RedirectToAction("Index", "Notfound");
            }
        }

        // POST: GroupPermissions/Delete/5
        [HttpPost]
        public ActionResult Delete(string id)
        {
            try
            {
                if (Utility.KiemTra())
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (!Utility.KiemTraQuyen(API.web_NhomQuyen, API.Delete))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                var apiResponse = Utility.Delete<v_web_NhomQuyen>(Utility.LOC_ID + "/" + id, API.web_NhomQuyen);
                if (apiResponse.Success)
                    return RedirectToAction("Index");
                else
                    ModelState.AddModelError(string.Empty, apiResponse.Message);
                return View();
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex.Message);
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
             return Json(apiResponse, JsonRequestBehavior.AllowGet);
         }
         if (!Utility.KiemTraQuyen(API.web_NhomQuyen, API.Create))
         {
             TempData["TitleError"] = API.TitlePermission;
             apiResponse.URL = Url.Action("Index", "Notfound");
             return Json(apiResponse, JsonRequestBehavior.AllowGet);
         }
         v_web_NhomQuyen web_NhomQuyen = new v_web_NhomQuyen();
         apiResponse.Success = true;
     web_NhomQuyen.LOC_ID = Utility.LOC_ID;
     web_NhomQuyen.ID = Guid.NewGuid().ToString();
         apiResponse.Detail = Utility.ConvertobjectTo<web_NhomQuyen>(web_NhomQuyen);
         return Json(apiResponse, JsonRequestBehavior.AllowGet);
     }
     catch (Exception ex)
     {
         Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex.Message);
         TempData["TitleError"] = API.TitleTryCatch;
         TempData["DetailError"] = ex.Message;
         apiResponse.URL = Url.Action("Index", "Notfound");
         return Json(apiResponse, JsonRequestBehavior.AllowGet);
     }
 }

 // POST: Menu/Create
 // To protect from overposting attacks, enable the specific properties you want to bind to, for 
 // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
 [HttpPost, ValidateInput(false)]
 [ValidateAntiForgeryToken]
 public ActionResult CreatePopup([Bind(Include = "ID,LOC_ID,MA,NAME,NOTE,ISPHANQUYEN,ISACTIVE,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO")] v_web_NhomQuyen web_NhomQuyen)
 {
     ApiResponse apiResponse = new ApiResponse();
     try
     {
         if (Utility.KiemTra())
         {
             apiResponse.URL = Url.Action("Index", "Admin");
             return Json(apiResponse, JsonRequestBehavior.AllowGet);
         }
         if (!Utility.KiemTraQuyen(API.web_NhomQuyen, API.Create))
         {
             TempData["TitleError"] = API.TitlePermission;
             apiResponse.URL = Url.Action("Index", "Notfound");
             return Json(apiResponse, JsonRequestBehavior.AllowGet);
         }
         if (ModelState.IsValid)
         {
             web_NhomQuyen.LOC_ID = Utility.LOC_ID;
web_NhomQuyen.ID_NGUOITAO = Session[Sessions.idUser].ToString();
web_NhomQuyen.THOIGIANTHEM = Utility.CurrentTime;

             apiResponse = Utility.Create<web_NhomQuyen>(web_NhomQuyen, API.web_NhomQuyen);
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
             apiResponse.Data = Utility.GetModelState(ModelState);
         }
         apiResponse.ID = web_NhomQuyen.ID;
         apiResponse.Detail = Utility.ConvertobjectTo<web_NhomQuyen>(web_NhomQuyen);
         return Json(apiResponse, JsonRequestBehavior.AllowGet);
     }
     catch (Exception ex)
     {
         Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex.Message);
         TempData["TitleError"] = API.TitleTryCatch;
         TempData["DetailError"] = ex.Message;
         apiResponse.URL = Url.Action("Index", "Notfound");
         return Json(apiResponse, JsonRequestBehavior.AllowGet);
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
             return Json(apiResponse, JsonRequestBehavior.AllowGet);
         }
         if (!Utility.KiemTraQuyen(API.web_NhomQuyen, API.Edit))
         {
             TempData["TitleError"] = API.TitlePermission;
             apiResponse.URL = Url.Action("Index", "Notfound");
             return Json(apiResponse, JsonRequestBehavior.AllowGet);
         }
         v_web_NhomQuyen web_NhomQuyen = new v_web_NhomQuyen();
         if (!string.IsNullOrEmpty(id))
         {
             apiResponse = Utility.GetDetail<v_web_NhomQuyen>(Utility.LOC_ID + "/" + id, API.web_NhomQuyen);
             if (!apiResponse.Success)
             {
                 TempData["TitleError"] = apiResponse.Message;
                 apiResponse.URL = Url.Action("Index", "Notfound");
                 return Json(apiResponse, JsonRequestBehavior.AllowGet);
             }
             if (apiResponse.Data != null)
                 web_NhomQuyen = apiResponse.Data as v_web_NhomQuyen;
         }
         apiResponse.Success = true;
         apiResponse.Detail = Utility.ConvertobjectTo<web_NhomQuyen>(web_NhomQuyen);
         return Json(apiResponse, JsonRequestBehavior.AllowGet);
     }
     catch (Exception ex)
     {
         Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex.Message);
         TempData["TitleError"] = API.TitleTryCatch;
         TempData["DetailError"] = ex.Message;
         apiResponse.URL = Url.Action("Index", "Notfound");
         return Json(apiResponse, JsonRequestBehavior.AllowGet);
     }
 }

 // POST: Menu/Edit/5
 // To protect from overposting attacks, enable the specific properties you want to bind to, for 
 // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
 [HttpPost, ValidateInput(false)]
 [ValidateAntiForgeryToken]
 public ActionResult EditPopup([Bind(Include = "ID,LOC_ID,MA,NAME,NOTE,ISPHANQUYEN,ISACTIVE,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO")] v_web_NhomQuyen web_NhomQuyen)
 {
     ApiResponse apiResponse = new ApiResponse();
     try
     {
         if (Utility.KiemTra())
         {
             apiResponse.URL = Url.Action("Index", "Admin");
             return Json(apiResponse, JsonRequestBehavior.AllowGet);
         }
         if (!Utility.KiemTraQuyen(API.web_NhomQuyen, API.Edit))
         {
             TempData["TitleError"] = API.TitlePermission;
             apiResponse.URL = Url.Action("Index", "Notfound");
             return Json(apiResponse, JsonRequestBehavior.AllowGet);
         }
         if (ModelState.IsValid)
         {
             web_NhomQuyen.LOC_ID = Utility.LOC_ID;
      web_NhomQuyen.ID_NGUOISUA = Session[Sessions.idUser].ToString();
      web_NhomQuyen.THOIGIANSUA = Utility.CurrentTime;

             apiResponse = Utility.Edit<v_web_NhomQuyen>(Utility.LOC_ID + "/" + web_NhomQuyen.MA, web_NhomQuyen, API.web_NhomQuyen);
             if (apiResponse.Success)
             {
                 //return RedirectToAction("Index");
                 apiResponse.ID = web_NhomQuyen.ID;
             }
             else
             {
                 ModelState.AddModelError(string.Empty, apiResponse.Message);
             }
         }
         else
         {
             apiResponse.Success = false;
             apiResponse.Data = Utility.GetModelState(ModelState);
         }
         apiResponse.Detail = Utility.ConvertobjectTo<web_NhomQuyen>(web_NhomQuyen);
         return Json(apiResponse, JsonRequestBehavior.AllowGet);
     }
     catch (Exception ex)
     {
         Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex.Message);
         TempData["TitleError"] = API.TitleTryCatch;
         TempData["DetailError"] = ex.Message;
         apiResponse.URL = Url.Action("Index", "Notfound");
         return Json(apiResponse, JsonRequestBehavior.AllowGet);
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
             return Json(apiResponse, JsonRequestBehavior.AllowGet);
         }
         if (!Utility.KiemTraQuyen(API.web_NhomQuyen, API.Delete))
         {
             TempData["TitleError"] = API.TitlePermission;
             apiResponse.URL = Url.Action("Index", "Notfound");
             return Json(apiResponse, JsonRequestBehavior.AllowGet);
         }
         apiResponse = Utility.Delete<v_web_NhomQuyen>(Utility.LOC_ID + "/" + id, API.web_NhomQuyen);
         apiResponse.ID = id;
         return Json(apiResponse, JsonRequestBehavior.AllowGet);
     }
     catch (Exception ex)
     {
         Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex.Message);
         TempData["TitleError"] = API.TitleTryCatch;
         TempData["DetailError"] = ex.Message;
         apiResponse.URL = Url.Action("Index", "Notfound");
         return Json(apiResponse, JsonRequestBehavior.AllowGet);
     }
 }
 #endregion
    }
}