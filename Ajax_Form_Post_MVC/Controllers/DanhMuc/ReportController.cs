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
    public class ReportController : Controller
    {

        // GET: Report
        public ActionResult Index(int Page = 1, string SearchString = "", string ShowSearchValue = "")
        {
            try
            {
                if (Utility.KiemTra())
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (!Utility.KiemTraQuyen(API.web_Report, API.Xem))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                ShowSearchValue = Utility.GetShowSearchValue<web_Report>(ShowSearchValue);
                var apiResponse = Utility.GetListData<v_web_Report>(API.web_Report, ShowSearchValue, SearchString, "");
                if (!apiResponse.Success)
                {
                    TempData["TitleError"] = apiResponse.Message;
                    return RedirectToAction("Index", "Notfound");
                }
                IPagedList<v_web_Report> lstpage = (apiResponse.Data as List<v_web_Report>).ToPagedList(Page, Utility.GetPageSize());

                v_v_web_Report web_Report = new v_v_web_Report();
                web_Report.IPagedList = lstpage;
                //@LSTKHOAINGOAI

                ViewBag.searchValue = SearchString;
                ViewBag.showsearchValue = ShowSearchValue;

                ViewBag.PermissionEdit = Utility.KiemTraQuyen(API.web_Report, API.Edit);
                ViewBag.PermissionDelete = Utility.KiemTraQuyen(API.web_Report, API.Delete);
                ViewBag.PermissionCreate = Utility.KiemTraQuyen(API.web_Report, API.Create);
                return View(web_Report);
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
                TempData["TitleError"] = API.TitleTryCatch;
                TempData["DetailError"] = ex.Message;
                return RedirectToAction("Index", "Notfound");
            }
        }

        // GET: Report/Create
        public ActionResult Create(int type = 2)
        {
            try
            {
                Session[Sessions.IntWidth] = type;
                if (Utility.KiemTra())
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (!Utility.KiemTraQuyen(API.web_Report, API.Create))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                v_v_web_Report web_Report = new v_v_web_Report();
                web_Report.lstweb_Menu = new List<v_web_Menu>();
                //web_Report.lstweb_Menu = Utility.GetListData<v_web_Menu>(API.web_Menu).Data as List<v_web_Menu>;
                web_Report.lstweb_Report_Parameter = new List<v_web_Report_Parameter>();
                //web_Report.lstweb_Report_Parameter = Utility.GetListData<view_web_Report_Parameter>(API.web_Parameter).Data as List<view_web_Report_Parameter>;
                web_Report.ID = Guid.NewGuid().ToString();
                //@LSTKHOAINGOAI
                return View(web_Report);
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
                TempData["TitleError"] = API.TitleTryCatch;
                TempData["DetailError"] = ex.Message;
                return RedirectToAction("Index", "Notfound");
            }
        }

        // POST: Report/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID_MENU,ID,MA,NAME,NAME_SP,NOTE,REPORT")] v_v_web_Report web_Report)
        {
            try
            {
                if (Utility.KiemTra())
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (!Utility.KiemTraQuyen(API.web_Report, API.Create))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                if (ModelState.IsValid)
                {

                    //@ConvertObjectUnicodeToTCVN3
                    var apiResponse = Utility.Create<web_Report>(web_Report, API.web_Report);
                    if (apiResponse.Success)
                        return RedirectToAction("Index");
                    else
                    {
                        ModelState.AddModelError(string.Empty, apiResponse.Message);
                        if (apiResponse.CheckValue)
                            ViewBag.ID = Guid.NewGuid().ToString();
                    }
                }
                web_Report.lstweb_Menu = new List<v_web_Menu>();
                web_Report.lstweb_Menu = Utility.GetListData<v_web_Menu>(API.web_Menu).Data as List<v_web_Menu>;
                web_Report.lstweb_Report_Parameter = new List<v_web_Report_Parameter>();
                web_Report.lstweb_Report_Parameter = Utility.GetListData<v_web_Report_Parameter>(API.web_Parameter).Data as List<v_web_Report_Parameter>;
                return View(web_Report);
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
                TempData["TitleError"] = API.TitleTryCatch;
                TempData["DetailError"] = ex.Message;
                return RedirectToAction("Index", "Notfound");
            }
        }

        // GET: Report/Edit/5
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
                if (!Utility.KiemTraQuyen(API.web_Report, API.Edit))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                v_v_web_Report web_Report = new v_v_web_Report();
                if (!string.IsNullOrEmpty(id))
                {
                    apiResponse = Utility.GetDetail<v_v_web_Report>(id, API.web_Report);
                    if (!apiResponse.Success)
                    {
                        TempData["TitleError"] = apiResponse.Message;
                        return RedirectToAction("Index", "Notfound");
                    }
                    if (apiResponse.Data != null)
                        web_Report = apiResponse.Data as v_v_web_Report;
                }
                web_Report.lstweb_Menu = new List<v_web_Menu>();
                //web_Report.lstweb_Menu = Utility.GetListData<v_web_Menu>(API.web_Menu).Data as List<v_web_Menu>;
                web_Report.lstweb_Report_Parameter = new List<v_web_Report_Parameter>();
                //web_Report.lstweb_Report_Parameter = Utility.GetListData<view_web_Report_Parameter>(API.web_Parameter).Data as List<view_web_Report_Parameter>;
                //@ConvertObjectTCVN3ToUnicode
                //@LSTKHOAINGOAI
                return View(web_Report);
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
                TempData["TitleError"] = API.TitleTryCatch;
                TempData["DetailError"] = ex.Message;
                return RedirectToAction("Index", "Notfound");
            }
        }

        // POST: Report/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID_MENU,ID,MA,NAME,NAME_SP,NOTE,REPORT")] v_v_web_Report web_Report)
        {
            try
            {
                if (Utility.KiemTra())
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (!Utility.KiemTraQuyen(API.web_Report, API.Edit))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                if (ModelState.IsValid)
                {
                    //@ConvertObjectUnicodeToTCVN3
                    var apiResponse = Utility.Edit<v_web_Report>( web_Report.MA, web_Report, API.web_Report);
                    if (apiResponse.Success)
                        return RedirectToAction("Index");
                    else
                        ModelState.AddModelError(string.Empty, apiResponse.Message);
                }
                web_Report.lstweb_Menu = new List<v_web_Menu>();
                web_Report.lstweb_Menu = Utility.GetListData<v_web_Menu>(API.web_Menu).Data as List<v_web_Menu>;
                web_Report.lstweb_Report_Parameter = new List<v_web_Report_Parameter>();
                web_Report.lstweb_Report_Parameter = Utility.GetListData<v_web_Report_Parameter>(API.web_Parameter).Data as List<v_web_Report_Parameter>;
                return View(web_Report);
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
                TempData["TitleError"] = API.TitleTryCatch;
                TempData["DetailError"] = ex.Message;
                return RedirectToAction("Index", "Notfound");
            }
        }

        // POST: Report/Delete/5
        [HttpPost]
        public ActionResult Delete(string id)
        {
            try
            {
                if (Utility.KiemTra())
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (!Utility.KiemTraQuyen(API.web_Report, API.Delete))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                var apiResponse = Utility.Delete<v_web_Report>(id, API.web_Report);
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
                if (!Utility.KiemTraQuyen(API.web_Report, API.Create))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                v_v_web_Report web_Report = new v_v_web_Report();
                apiResponse.Success = true;
                web_Report.ID = Guid.NewGuid().ToString();
                web_Report.lstweb_Menu = new List<v_web_Menu>();
                web_Report.lstweb_Menu = Utility.GetListData<v_web_Menu>(API.web_Menu).Data as List<v_web_Menu>;
                web_Report.lstweb_Report_Parameter = new List<v_web_Report_Parameter>();
                web_Report.lstweb_Report_Parameter = Utility.GetListData<v_web_Report_Parameter>(API.web_Parameter).Data as List<v_web_Report_Parameter>;
                var lst = Utility.ConvertobjectTo<v_v_web_Report>(web_Report);
                apiResponse.ProductCombo = Utility.GetParameter(web_Report.lstweb_Report_Parameter);
                lst.Add(new ValueEdit { Key = "tbodyReport_Add", Value = apiResponse.ProductCombo });
                apiResponse.Detail = lst;
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
        public ActionResult CreatePopup([Bind(Include = "ID_MENU,ID,MA,NAME,NAME_SP,NOTE,REPORT")] v_v_web_Report web_Report)
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                if (Utility.KiemTra())
                {
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Admin");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                if (!Utility.KiemTraQuyen(API.web_Report, API.Create))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                var lstKey = Request.Form.AllKeys.Where(e => e.StartsWith("txt"));
                if (lstKey == null || lstKey.Count() == 0)
                {
                    ModelState.AddModelError("lstweb_Report_Parameter", "Thêm danh sách parameter.");
                }
                if (ModelState.IsValid)
                {
                    web_Report.lstweb_Report_Parameter = new List<v_web_Report_Parameter>();
                    v_web_Report_Parameter web_Report_Parameter = new v_web_Report_Parameter();
                    foreach (string Key in lstKey)
                    {
                        var lstString = Key.ToString().Split('|');
                        var value = HttpContext.Request.Params.GetValues(Key.ToString());
                        string ShowSearchValue = clsMaHoa.Decrypt(lstString[1].ToString(), clsMaHoa.PassMaHoa);
                        var Checkct_PhieuNhap_ChiTiet = JsonConvert.DeserializeObject<v_web_Report_Parameter>(ShowSearchValue);
                        if (web_Report_Parameter.ID != Checkct_PhieuNhap_ChiTiet.ID)
                        {
                            web_Report_Parameter = new v_web_Report_Parameter();
                            web_Report_Parameter = JsonConvert.DeserializeObject<v_web_Report_Parameter>(ShowSearchValue);
                            web_Report.lstweb_Report_Parameter.Add(web_Report_Parameter);
                        }
                        Utility.EditObject(web_Report_Parameter, lstString[0].ToString().Substring(3, lstString[0].ToString().Length - 3), value[0]);
                    }

                    apiResponse = Utility.Create<v_web_Report>(web_Report, API.web_Report);
                    if (apiResponse.Success)
                    {
                        apiResponse.NewID = Guid.NewGuid().ToString();
                        if (apiResponse.Data != null && !string.IsNullOrEmpty(apiResponse.Data.ToString()))
                            web_Report = JsonConvert.DeserializeObject<v_v_web_Report>(apiResponse.Data.ToString());

                        web_Report.lstweb_Report_Parameter = new List<v_web_Report_Parameter>();
                        web_Report.lstweb_Report_Parameter = Utility.GetListData<v_web_Report_Parameter>(API.web_Parameter).Data as List<v_web_Report_Parameter>;
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
                    apiResponse.Data = Utility.GetModelState(ModelState, API.web_Report);
                }
                apiResponse.ID = web_Report.ID;
                web_Report.lstweb_Menu = new List<v_web_Menu>();
                web_Report.lstweb_Menu = Utility.GetListData<v_web_Menu>(API.web_Menu).Data as List<v_web_Menu>;
              
                apiResponse.Detail = Utility.ConvertobjectToView<v_web_Report>(web_Report);
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
                if (!Utility.KiemTraQuyen(API.web_Report, API.Edit))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                v_v_web_Report web_Report = new v_v_web_Report();
                if (!string.IsNullOrEmpty(id))
                {
                    apiResponse = Utility.GetDetail<v_v_web_Report>(id, API.web_Report);
                    if (!apiResponse.Success)
                    {
                        TempData["TitleError"] = apiResponse.Message;
                        apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                        return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                    }
                    if (apiResponse.Data != null)
                        web_Report = apiResponse.Data as v_v_web_Report;
                }
                apiResponse.Success = true;
                web_Report.lstweb_Menu = new List<v_web_Menu>();
                web_Report.lstweb_Menu = Utility.GetListData<v_web_Menu>(API.web_Menu).Data as List<v_web_Menu>;
                var lstweb_Report_Parameter = new List<v_web_Report_Parameter>();
                lstweb_Report_Parameter = Utility.GetListData<v_web_Report_Parameter>(API.web_Parameter).Data as List<v_web_Report_Parameter>;
                foreach(var itm in lstweb_Report_Parameter)
                {
                    if (web_Report.lstweb_Report_Parameter.Where(e => e.ID_PARAMETER == itm.ID).Count() == 0)
                        web_Report.lstweb_Report_Parameter.Add(itm);
                }
                var lst = Utility.ConvertobjectTo<v_v_web_Report>(web_Report);
                apiResponse.ProductCombo = Utility.GetParameter(web_Report.lstweb_Report_Parameter);
                lst.Add(new ValueEdit { Key = "tbodyReport_Edit", Value = apiResponse.ProductCombo });
                apiResponse.Detail = lst;
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
        public ActionResult EditPopup([Bind(Include = "ID_MENU,ID,MA,NAME,NAME_SP,NOTE,REPORT")] v_v_web_Report web_Report)
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                if (Utility.KiemTra())
                {
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Admin");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                if (!Utility.KiemTraQuyen(API.web_Report, API.Edit))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                var lstKey = Request.Form.AllKeys.Where(e => e.StartsWith("txt"));
                if (lstKey == null || lstKey.Count() == 0)
                {
                    ModelState.AddModelError("lstweb_Report_Parameter", "Thêm danh sách parameter.");
                }
                if (ModelState.IsValid)
                {
                    web_Report.lstweb_Report_Parameter = new List<v_web_Report_Parameter>();
                    v_web_Report_Parameter web_Report_Parameter = new v_web_Report_Parameter();
                    foreach (string Key in lstKey)
                    {
                        var lstString = Key.ToString().Split('|');
                        var value = HttpContext.Request.Params.GetValues(Key.ToString());
                        string ShowSearchValue = clsMaHoa.Decrypt(lstString[1].ToString(), clsMaHoa.PassMaHoa);
                        var Checkct_PhieuNhap_ChiTiet = JsonConvert.DeserializeObject<v_web_Report_Parameter>(ShowSearchValue);
                        if (web_Report_Parameter.ID != Checkct_PhieuNhap_ChiTiet.ID)
                        {
                            web_Report_Parameter = new v_web_Report_Parameter();
                            web_Report_Parameter = JsonConvert.DeserializeObject<v_web_Report_Parameter>(ShowSearchValue);
                            web_Report_Parameter.ISACTIVE = false;
                            web_Report.lstweb_Report_Parameter.Add(web_Report_Parameter);
                        }
                        Utility.EditObject(web_Report_Parameter, lstString[0].ToString().Substring(3, lstString[0].ToString().Length - 3), value[0]);
                    }
                    apiResponse = Utility.Edit<v_web_Report>(web_Report.MA, web_Report, API.web_Report);
                    if (apiResponse.Success)
                    {
                        apiResponse.ID = web_Report.ID;
                        if (apiResponse.Data != null && !string.IsNullOrEmpty(apiResponse.Data.ToString()))
                            web_Report = JsonConvert.DeserializeObject<v_v_web_Report>(apiResponse.Data.ToString());

                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, apiResponse.Message);
                    }
                }
                else
                {
                    apiResponse.Success = false;
                    apiResponse.Data = Utility.GetModelState(ModelState, API.web_Report);
                }
                web_Report.lstweb_Menu = new List<v_web_Menu>();
                web_Report.lstweb_Menu = Utility.GetListData<v_web_Menu>(API.web_Menu).Data as List<v_web_Menu>;
                var lst = Utility.ConvertobjectToView<v_v_web_Report>(web_Report);
                apiResponse.ProductCombo = Utility.GetParameter(web_Report.lstweb_Report_Parameter);
                lst.Add(new ValueEdit { Key = "tbodyReport_Edit", Value = apiResponse.ProductCombo });
                apiResponse.Detail = lst;
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
                if (!Utility.KiemTraQuyen(API.web_Report, API.Delete))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                apiResponse = Utility.Delete<v_web_Report>(id, API.web_Report);
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