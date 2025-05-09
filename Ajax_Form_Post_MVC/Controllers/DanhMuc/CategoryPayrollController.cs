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
    public class CategoryPayrollController : Controller
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
                if (!Utility.KiemTraQuyen(API.dm_BangLuong, API.Xem))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                ShowSearchValue = Utility.GetShowSearchValue<dm_BangLuong>(ShowSearchValue);
                var apiResponse = Utility.GetListData<v_dm_BangLuong>(API.dm_BangLuong, ShowSearchValue, SearchString, "");
                if (!apiResponse.Success)
                {
                    TempData["TitleError"] = apiResponse.Message;
                    return RedirectToAction("Index", "Notfound");
                }
                IPagedList<v_dm_BangLuong> lstpage = (apiResponse.Data as List<v_dm_BangLuong>).ToPagedList(Page, Utility.GetPageSize());

                v_v_dm_BangLuong dm_BangLuong = new v_v_dm_BangLuong();
                dm_BangLuong.IPagedList = lstpage;
                ViewBag.searchValue = SearchString;
                ViewBag.showsearchValue = ShowSearchValue;

                ViewBag.PermissionEdit = Utility.KiemTraQuyen(API.dm_BangLuong, API.Edit);
                ViewBag.PermissionDelete = Utility.KiemTraQuyen(API.dm_BangLuong, API.Delete);
                ViewBag.PermissionCreate = Utility.KiemTraQuyen(API.dm_BangLuong, API.Create);
                return View(dm_BangLuong);
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
                if (!Utility.KiemTraQuyen(API.dm_BangLuong, API.Create))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                v_v_dm_BangLuong dm_BangLuong = new v_v_dm_BangLuong();
                //dm_BangLuong.lstweb_Menu = Utility.GetListData<v_web_Menu>(API.web_Menu).Data as List<v_web_Menu>;
                dm_BangLuong.lstdm_BangLuong_ChiTiet = new List<v_dm_BangLuong_ChiTiet>();
                dm_BangLuong.lstdm_PhongBan = new List<v_dm_PhongBan>();
                //dm_BangLuong.lstdm_BangLuong_ChiTiet = Utility.GetListData<view_dm_BangLuong_Parameter>(API.web_Parameter).Data as List<view_dm_BangLuong_Parameter>;
                dm_BangLuong.ID = Guid.NewGuid().ToString();
                //@LSTKHOAINGOAI
                return View(dm_BangLuong);
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
        public ActionResult Create([Bind(Include = "LOC_ID,ID,MA,NAME,ISACTIVE,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO,ID_PHONGBAN")] v_v_dm_BangLuong dm_BangLuong)
        {
            try
            {
                if (Utility.KiemTra())
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (!Utility.KiemTraQuyen(API.dm_BangLuong, API.Create))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                if (ModelState.IsValid)
                {

                    //@ConvertObjectUnicodeToTCVN3
                    var apiResponse = Utility.Create<dm_BangLuong>(dm_BangLuong, API.dm_BangLuong);
                    if (apiResponse.Success)
                        return RedirectToAction("Index");
                    else
                    {
                        ModelState.AddModelError(string.Empty, apiResponse.Message);
                        if (apiResponse.CheckValue)
                            ViewBag.ID = Guid.NewGuid().ToString();
                    }
                }
                return View(dm_BangLuong);
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
                if (!Utility.KiemTraQuyen(API.dm_BangLuong, API.Edit))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                v_v_dm_BangLuong dm_BangLuong = new v_v_dm_BangLuong();
                if (!string.IsNullOrEmpty(id))
                {
                    apiResponse = Utility.GetDetail<v_v_dm_BangLuong>(id, API.dm_BangLuong);
                    if (!apiResponse.Success)
                    {
                        TempData["TitleError"] = apiResponse.Message;
                        return RedirectToAction("Index", "Notfound");
                    }
                    if (apiResponse.Data != null)
                        dm_BangLuong = apiResponse.Data as v_v_dm_BangLuong;
                }
                dm_BangLuong.lstdm_PhongBan = new List<v_dm_PhongBan>();
                
                return View(dm_BangLuong);
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
        public ActionResult Edit([Bind(Include = "LOC_ID,ID,MA,NAME,ISACTIVE,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO,ID_PHONGBAN")] v_v_dm_BangLuong dm_BangLuong)
        {
            try
            {
                if (Utility.KiemTra())
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (!Utility.KiemTraQuyen(API.dm_BangLuong, API.Edit))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                if (ModelState.IsValid)
                {
                    //@ConvertObjectUnicodeToTCVN3
                    var apiResponse = Utility.Edit<v_dm_BangLuong>( dm_BangLuong.MA, dm_BangLuong, API.dm_BangLuong);
                    if (apiResponse.Success)
                        return RedirectToAction("Index");
                    else
                        ModelState.AddModelError(string.Empty, apiResponse.Message);
                }
              
                return View(dm_BangLuong);
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
                if (!Utility.KiemTraQuyen(API.dm_BangLuong, API.Delete))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                var apiResponse = Utility.Delete<v_dm_BangLuong>(id, API.dm_BangLuong);
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
                if (!Utility.KiemTraQuyen(API.dm_BangLuong, API.Create))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                v_v_dm_BangLuong dm_BangLuong = new v_v_dm_BangLuong();
                apiResponse.Success = true;
                dm_BangLuong.ID = Guid.NewGuid().ToString();
                dm_BangLuong.lstdm_BangLuong_ChiTiet = new List<v_dm_BangLuong_ChiTiet>();
                Session[Sessions.lstdm_LuongThang_ChiTiet] = dm_BangLuong.lstdm_BangLuong_ChiTiet;
                dm_BangLuong.lstdm_PhongBan = new List<v_dm_PhongBan>();
                dm_BangLuong.lstdm_PhongBan = Utility.GetListData<v_dm_PhongBan>(API.dm_PhongBan, "", "", Utility.LOC_ID).Data as List<v_dm_PhongBan>;
                var lstdm_LoaiLuong = Utility.GetListData<v_dm_LoaiLuong>(API.dm_LoaiLuong, "", "", Utility.LOC_ID).Data as List<v_dm_LoaiLuong>;
                var lst = Utility.ConvertobjectTo<v_v_dm_BangLuong>(dm_BangLuong);
                v_dm_BangLuong_ChiTiet newv_dm_BangLuong_ChiTiet = new v_dm_BangLuong_ChiTiet();
                newv_dm_BangLuong_ChiTiet.ID = Guid.NewGuid().ToString();
                dm_BangLuong.lstdm_BangLuong_ChiTiet.Add(newv_dm_BangLuong_ChiTiet);
                apiResponse.ProductCombo = Utility.GetCategoryPayroll(dm_BangLuong.lstdm_BangLuong_ChiTiet, lstdm_LoaiLuong);
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
        public ActionResult CreatePopup([Bind(Include = "LOC_ID,ID,MA,NAME,ISACTIVE,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO,ID_PHONGBAN")] v_v_dm_BangLuong dm_BangLuong)
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                if (Utility.KiemTra())
                {
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Admin");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                if (!Utility.KiemTraQuyen(API.dm_BangLuong, API.Create))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                var lstKey_ID_LOAILUONG = Request.Form.AllKeys.Where(e => e.StartsWith("ID_LOAILUONG|"));
                var lstKey_TYPE_LUONG = Request.Form.AllKeys.Where(e => e.StartsWith("TYPE_LUONG|"));
                var lstKey_TYPE_QUYTACTINHLUONG = Request.Form.AllKeys.Where(e => e.StartsWith("TYPE_QUYTACTINHLUONG|"));
                var lstKey = Request.Form.AllKeys.Where(e => e.StartsWith("txtSOTIEN|"));
                if (lstKey == null || lstKey.Count() == 0)
                {
                    ModelState.AddModelError("lstdm_BangLuong_ChiTiet", "Thêm danh sách parameter.");
                }
                if (ModelState.IsValid)
                {
                    dm_BangLuong.lstdm_BangLuong_ChiTiet = new List<v_dm_BangLuong_ChiTiet>();
                    int i = 0;
                    foreach (string Key in lstKey)
                    {
                        var lstString = Key.ToString().Split('|');
                        var value_st = HttpContext.Request.Params.GetValues(Key.ToString());
                        var value_ID_LOAILUONG = HttpContext.Request.Params.GetValues(lstKey_ID_LOAILUONG.ToList()[i].ToString());
                        var value_TYPE_LUONG  = HttpContext.Request.Params.GetValues(lstKey_TYPE_LUONG.ToList()[i].ToString());
                        var value_TYPE_QUYTACTINHLUONG = HttpContext.Request.Params.GetValues(lstKey_TYPE_QUYTACTINHLUONG.ToList()[i].ToString());
                        string ShowSearchValue = clsMaHoa.Decrypt(lstString[1].ToString(), clsMaHoa.PassMaHoa);
                        var Checkct_PhieuNhap_ChiTiet = JsonConvert.DeserializeObject<v_dm_BangLuong_ChiTiet>(ShowSearchValue);
                        if (lstString != null)
                        {
                            if (string.IsNullOrEmpty(Checkct_PhieuNhap_ChiTiet.ID))
                                Checkct_PhieuNhap_ChiTiet.ID = Guid.NewGuid().ToString();

                            Checkct_PhieuNhap_ChiTiet.LOC_ID = Utility.LOC_ID;
                            Checkct_PhieuNhap_ChiTiet.ID_BANGLUONG = dm_BangLuong.ID;
                            Checkct_PhieuNhap_ChiTiet.SOTIEN = Utility.ConvertStringToDouble(value_st[0]);
                            Checkct_PhieuNhap_ChiTiet.ID_LOAILUONG = value_ID_LOAILUONG[0];
                            Checkct_PhieuNhap_ChiTiet.TYPE_LUONG = Convert.ToInt32(Utility.ConvertStringToDouble(value_TYPE_LUONG[0]));
                            Checkct_PhieuNhap_ChiTiet.TYPE_QUYTACTINHLUONG = Convert.ToInt32(Utility.ConvertStringToDouble(value_TYPE_QUYTACTINHLUONG[0]));
                            dm_BangLuong.lstdm_BangLuong_ChiTiet.Add(Checkct_PhieuNhap_ChiTiet);
                        }
                      
                        i += 1;
                    }
                    dm_BangLuong.LOC_ID = Utility.LOC_ID;
                    dm_BangLuong.ID_NGUOITAO = Session[Sessions.idUser].ToString();
                    dm_BangLuong.THOIGIANTHEM = Utility.CurrentTime;
                    apiResponse = Utility.Create<v_dm_BangLuong>(dm_BangLuong, API.dm_BangLuong);
                    if (apiResponse.Success)
                    {
                        apiResponse.NewID = Guid.NewGuid().ToString();
                        if (apiResponse.Data != null && !string.IsNullOrEmpty(apiResponse.Data.ToString()))
                            dm_BangLuong = JsonConvert.DeserializeObject<v_v_dm_BangLuong>(apiResponse.Data.ToString());

                        dm_BangLuong.lstdm_BangLuong_ChiTiet = new List<v_dm_BangLuong_ChiTiet>();
                        dm_BangLuong.lstdm_BangLuong_ChiTiet = dm_BangLuong.lstdm_BangLuong_ChiTiet;
                        Session[Sessions.lstdm_LuongThang_ChiTiet] = dm_BangLuong.lstdm_BangLuong_ChiTiet;
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
                    apiResponse.Data = Utility.GetModelState(ModelState, API.dm_BangLuong);
                }
                dm_BangLuong.lstdm_PhongBan = new List<v_dm_PhongBan>();
                dm_BangLuong.lstdm_PhongBan = Utility.GetListData<v_dm_PhongBan>(API.dm_PhongBan, "", "", Utility.LOC_ID).Data as List<v_dm_PhongBan>;
                apiResponse.ID = dm_BangLuong.ID;
                apiResponse.Detail = Utility.ConvertobjectToView<v_dm_BangLuong>(dm_BangLuong);
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
                if (!Utility.KiemTraQuyen(API.dm_BangLuong, API.Edit))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                v_v_dm_BangLuong dm_BangLuong = new v_v_dm_BangLuong();
                if (!string.IsNullOrEmpty(id))
                {
                    apiResponse = Utility.GetDetail<v_v_dm_BangLuong>(id, API.dm_BangLuong);
                    if (!apiResponse.Success)
                    {
                        TempData["TitleError"] = apiResponse.Message;
                        apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                        return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                    }
                    if (apiResponse.Data != null)
                        dm_BangLuong = apiResponse.Data as v_v_dm_BangLuong;
                }
                apiResponse.Success = true;
                dm_BangLuong.lstdm_PhongBan = new List<v_dm_PhongBan>();
                dm_BangLuong.lstdm_PhongBan = Utility.GetListData<v_dm_PhongBan>(API.dm_PhongBan, "", "", Utility.LOC_ID).Data as List<v_dm_PhongBan>;
                var lstdm_LoaiLuong = Utility.GetListData<v_dm_LoaiLuong>(API.dm_LoaiLuong, "", "", Utility.LOC_ID).Data as List<v_dm_LoaiLuong>;
                var lst = Utility.ConvertobjectTo<v_v_dm_BangLuong>(dm_BangLuong);
                Session[Sessions.lstdm_LuongThang_ChiTiet] = dm_BangLuong.lstdm_BangLuong_ChiTiet;
                apiResponse.ProductCombo = Utility.GetCategoryPayroll(dm_BangLuong.lstdm_BangLuong_ChiTiet, lstdm_LoaiLuong);
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
        public ActionResult EditPopup([Bind(Include = "LOC_ID,ID,MA,NAME,ISACTIVE,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO,ID_PHONGBAN")] v_v_dm_BangLuong dm_BangLuong)
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                if (Utility.KiemTra())
                {
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Admin");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                if (!Utility.KiemTraQuyen(API.dm_BangLuong, API.Edit))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                var lstKey_ID_LOAILUONG = Request.Form.AllKeys.Where(e => e.StartsWith("ID_LOAILUONG|"));
                var lstKey_TYPE_LUONG = Request.Form.AllKeys.Where(e => e.StartsWith("TYPE_LUONG|"));
                var lstKey_TYPE_QUYTACTINHLUONG = Request.Form.AllKeys.Where(e => e.StartsWith("TYPE_QUYTACTINHLUONG|"));
                var lstKey = Request.Form.AllKeys.Where(e => e.StartsWith("txtSOTIEN|"));
                if (lstKey == null || lstKey.Count() == 0)
                {
                    ModelState.AddModelError("lstdm_BangLuong_ChiTiet", "Thêm danh sách.");
                }
                if (ModelState.IsValid)
                {
                    dm_BangLuong.lstdm_BangLuong_ChiTiet = new List<v_dm_BangLuong_ChiTiet>();
                    int i = 0;
                    foreach (string Key in lstKey)
                    {
                        var lstString = Key.ToString().Split('|');
                        var value_st = HttpContext.Request.Params.GetValues(Key.ToString());
                        var value_ID_LOAILUONG = HttpContext.Request.Params.GetValues(lstKey_ID_LOAILUONG.ToList()[i].ToString());
                        var value_TYPE_LUONG = HttpContext.Request.Params.GetValues(lstKey_TYPE_LUONG.ToList()[i].ToString());
                        var value_TYPE_QUYTACTINHLUONG = HttpContext.Request.Params.GetValues(lstKey_TYPE_QUYTACTINHLUONG.ToList()[i].ToString());
                        string ShowSearchValue = clsMaHoa.Decrypt(lstString[1].ToString(), clsMaHoa.PassMaHoa);
                        var Checkct_PhieuNhap_ChiTiet = JsonConvert.DeserializeObject<v_dm_BangLuong_ChiTiet>(ShowSearchValue);
                        if (lstString != null)
                        {
                            if (string.IsNullOrEmpty(Checkct_PhieuNhap_ChiTiet.ID))
                                Checkct_PhieuNhap_ChiTiet.ID = Guid.NewGuid().ToString();

                            Checkct_PhieuNhap_ChiTiet.LOC_ID = Utility.LOC_ID;
                            Checkct_PhieuNhap_ChiTiet.ID_BANGLUONG = dm_BangLuong.ID;
                            Checkct_PhieuNhap_ChiTiet.SOTIEN = Utility.ConvertStringToDouble(value_st[0]);
                            Checkct_PhieuNhap_ChiTiet.ID_LOAILUONG = value_ID_LOAILUONG[0];
                            Checkct_PhieuNhap_ChiTiet.TYPE_LUONG = Convert.ToInt32(Utility.ConvertStringToDouble(value_TYPE_LUONG[0]));
                            Checkct_PhieuNhap_ChiTiet.TYPE_QUYTACTINHLUONG = Convert.ToInt32(Utility.ConvertStringToDouble(value_TYPE_QUYTACTINHLUONG[0]));
                            dm_BangLuong.lstdm_BangLuong_ChiTiet.Add(Checkct_PhieuNhap_ChiTiet);
                        }

                        i += 1;
                    }
                    dm_BangLuong.ID_NGUOISUA = Session[Sessions.idUser].ToString();
                    dm_BangLuong.THOIGIANSUA = Utility.CurrentTime;
                    apiResponse = Utility.Edit<v_dm_BangLuong>(dm_BangLuong.MA, dm_BangLuong, API.dm_BangLuong);
                    if (apiResponse.Success)
                    {
                        apiResponse.ID = dm_BangLuong.ID;
                        if (apiResponse.Data != null && !string.IsNullOrEmpty(apiResponse.Data.ToString()))
                            dm_BangLuong = JsonConvert.DeserializeObject<v_v_dm_BangLuong>(apiResponse.Data.ToString());

                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, apiResponse.Message);
                    }
                }
                else
                {
                    apiResponse.Success = false;
                    apiResponse.Data = Utility.GetModelState(ModelState, API.dm_BangLuong);
                }
                dm_BangLuong.lstdm_PhongBan = new List<v_dm_PhongBan>();
                dm_BangLuong.lstdm_PhongBan = Utility.GetListData<v_dm_PhongBan>(API.dm_PhongBan, "", "", Utility.LOC_ID).Data as List<v_dm_PhongBan>;
                var lst = Utility.ConvertobjectToView<v_v_dm_BangLuong>(dm_BangLuong);
                var lstdm_LoaiLuong = Utility.GetListData<v_dm_LoaiLuong>(API.dm_LoaiLuong, "", "", Utility.LOC_ID).Data as List<v_dm_LoaiLuong>;
                apiResponse.ProductCombo = Utility.GetCategoryPayroll(dm_BangLuong.lstdm_BangLuong_ChiTiet, lstdm_LoaiLuong);
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
                if (!Utility.KiemTraQuyen(API.dm_BangLuong, API.Delete))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                apiResponse = Utility.Delete<v_dm_BangLuong>(id, API.dm_BangLuong);
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
        [HttpPost]
        public ActionResult AddPayroll()
        {
            ApiResponse apiResponse = new ApiResponse();
            v_dm_BangLuong_ChiTiet newv_dm_BangLuong_ChiTiet = new v_dm_BangLuong_ChiTiet();
            newv_dm_BangLuong_ChiTiet.ID = Guid.NewGuid().ToString();
            Utility.Lstdm_BangLuong_ChiTiet.Add(newv_dm_BangLuong_ChiTiet);
            var lstdm_LoaiLuong = Utility.GetListData<v_dm_LoaiLuong>(API.dm_LoaiLuong, "", "", Utility.LOC_ID).Data as List<v_dm_LoaiLuong>;
            apiResponse.ProductCombo = Utility.GetCategoryPayroll(Utility.Lstdm_BangLuong_ChiTiet, lstdm_LoaiLuong);
            apiResponse.Success = true;
            return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
        }

        [HttpPost]
        public ActionResult RemovePayroll(string ID)
        {
            ApiResponse apiResponse = new ApiResponse();
            v_v_dm_HangHoa dm_HangHoa = new v_v_dm_HangHoa();
            var LstKPISale_YeuCau = Utility.Lstdm_BangLuong_ChiTiet;
            var check = Utility.Lstdm_BangLuong_ChiTiet.Where(e => e.ID == ID).FirstOrDefault();
            if (check != null)
                LstKPISale_YeuCau.Remove(check);

            Session[Sessions.lstdm_LuongThang_ChiTiet] = LstKPISale_YeuCau;
            var lstdm_LoaiLuong = Utility.GetListData<v_dm_LoaiLuong>(API.dm_LoaiLuong, "", "", Utility.LOC_ID).Data as List<v_dm_LoaiLuong>;
            apiResponse.ProductCombo = Utility.GetCategoryPayroll(Utility.Lstdm_BangLuong_ChiTiet, lstdm_LoaiLuong);
            apiResponse.Success = true;
            return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
        }
    }
}