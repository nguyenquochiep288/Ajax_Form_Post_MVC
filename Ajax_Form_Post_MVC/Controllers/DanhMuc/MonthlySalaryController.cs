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
    public class MonthlySalaryController : Controller
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
                if (!Utility.KiemTraQuyen(API.dm_ThangLuong, API.Xem))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                ShowSearchValue = Utility.GetShowSearchValue<dm_ThangLuong>(ShowSearchValue);
                var apiResponse = Utility.GetListData<v_dm_ThangLuong>(API.dm_ThangLuong, ShowSearchValue, SearchString, Utility.LOC_ID);
                if (!apiResponse.Success)
                {
                    TempData["TitleError"] = apiResponse.Message;
                    return RedirectToAction("Index", "Notfound");
                }
                IPagedList<v_dm_ThangLuong> lstpage = (apiResponse.Data as List<v_dm_ThangLuong>).ToPagedList(Page, Utility.GetPageSize());

                v_v_dm_ThangLuong dm_ThangLuong = new v_v_dm_ThangLuong();
                dm_ThangLuong.IPagedList = lstpage;
                //@LSTKHOAINGOAI

                ViewBag.searchValue = SearchString;
                ViewBag.showsearchValue = ShowSearchValue;

                ViewBag.PermissionEdit = Utility.KiemTraQuyen(API.dm_ThangLuong, API.Edit);
                ViewBag.PermissionDelete = Utility.KiemTraQuyen(API.dm_ThangLuong, API.Delete);
                ViewBag.PermissionCreate = Utility.KiemTraQuyen(API.dm_ThangLuong, API.Create);
                return View(dm_ThangLuong);
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
                TempData["TitleError"] = API.TitleTryCatch;
                TempData["DetailError"] = ex.Message;
                return RedirectToAction("Index", "Notfound");
            }
        }

        // GET: Area/Create
        public ActionResult Create(int type = 2)
        {
            try
            {
                Session[Sessions.IntWidth] = type;
                if (Utility.KiemTra())
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (!Utility.KiemTraQuyen(API.dm_ThangLuong, API.Create))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                v_v_dm_ThangLuong dm_ThangLuong = new v_v_dm_ThangLuong();
                dm_ThangLuong.LOC_ID = Utility.LOC_ID;
                //dm_ThangLuong.ID_NGUOITAO = Session[Sessions.idUser].ToString();
                //dm_ThangLuong.THOIGIANTHEM = Utility.CurrentTime;

                dm_ThangLuong.ID = Guid.NewGuid().ToString();
                //@LSTKHOAINGOAI
                return View(dm_ThangLuong);
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
                TempData["TitleError"] = API.TitleTryCatch;
                TempData["DetailError"] = ex.Message;
                return RedirectToAction("Index", "Notfound");
            }
        }

        // POST: Area/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "LOC_ID,ID,MA,NAME,THANG,NAM,SONGAY,SONGAYCONG,GHICHU,NGAYBATDAU,NGAYKETTHUC,ID_PHONGBAN,ISCHAMCONG,ISACTIVE,GIOBATDAU,GIOKETTHUC,SOGIONGHITRUA,DANHSACHNGAYNGHI")] v_v_dm_ThangLuong dm_ThangLuong)
        {
            try
            {
                if (Utility.KiemTra())
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (!Utility.KiemTraQuyen(API.dm_ThangLuong, API.Create))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                if (ModelState.IsValid)
                {
                    dm_ThangLuong.LOC_ID = Utility.LOC_ID;
                    //dm_ThangLuong.ID_NGUOITAO = Session[Sessions.idUser].ToString();
                    //dm_ThangLuong.THOIGIANTHEM = Utility.CurrentTime;

                    //@ConvertObjectUnicodeToTCVN3
                    var apiResponse = Utility.Create<dm_ThangLuong>(dm_ThangLuong, API.dm_ThangLuong);
                    if (apiResponse.Success)
                        return RedirectToAction("Index");
                    else
                    {
                        ModelState.AddModelError(string.Empty, apiResponse.Message);
                        if (apiResponse.CheckValue)
                            ViewBag.ID = Guid.NewGuid().ToString();
                    }
                }
                return View(dm_ThangLuong);
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
                TempData["TitleError"] = API.TitleTryCatch;
                TempData["DetailError"] = ex.Message;
                return RedirectToAction("Index", "Notfound");
            }
        }

        // GET: Area/Edit/5
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
                if (!Utility.KiemTraQuyen(API.dm_ThangLuong, API.Edit))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                v_v_dm_ThangLuong dm_ThangLuong = new v_v_dm_ThangLuong();
                if (!string.IsNullOrEmpty(id))
                {
                    apiResponse = Utility.GetDetail<v_v_dm_ThangLuong>(Utility.LOC_ID + "/" + id, API.dm_ThangLuong);
                    if (!apiResponse.Success)
                    {
                        TempData["TitleError"] = apiResponse.Message;
                        return RedirectToAction("Index", "Notfound");
                    }
                    if (apiResponse.Data != null)
                        dm_ThangLuong = apiResponse.Data as v_v_dm_ThangLuong;
                }
                //@ConvertObjectTCVN3ToUnicode
                //@LSTKHOAINGOAI
                return View(dm_ThangLuong);
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
                TempData["TitleError"] = API.TitleTryCatch;
                TempData["DetailError"] = ex.Message;
                return RedirectToAction("Index", "Notfound");
            }
        }

        // POST: Area/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "LOC_ID,ID,MA,NAME,THANG,NAM,SONGAY,SONGAYCONG,GHICHU,NGAYBATDAU,NGAYKETTHUC,ID_PHONGBAN,ISCHAMCONG,ISACTIVE,GIOBATDAU,GIOKETTHUC,SOGIONGHITRUA,DANHSACHNGAYNGHI")] v_v_dm_ThangLuong dm_ThangLuong)
        {
            try
            {
                if (Utility.KiemTra())
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (!Utility.KiemTraQuyen(API.dm_ThangLuong, API.Edit))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                if (ModelState.IsValid)
                {
                    dm_ThangLuong.LOC_ID = Utility.LOC_ID;
                    //dm_ThangLuong.ID_NGUOISUA = Session[Sessions.idUser].ToString();
                    //dm_ThangLuong.THOIGIANSUA = Utility.CurrentTime;
                    //@ConvertObjectUnicodeToTCVN3
                    var apiResponse = Utility.Edit<v_dm_ThangLuong>(Utility.LOC_ID + "/" + dm_ThangLuong.MA, dm_ThangLuong, API.dm_ThangLuong);
                    if (apiResponse.Success)
                        return RedirectToAction("Index");
                    else
                        ModelState.AddModelError(string.Empty, apiResponse.Message);
                }
                return View(dm_ThangLuong);
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
                TempData["TitleError"] = API.TitleTryCatch;
                TempData["DetailError"] = ex.Message;
                return RedirectToAction("Index", "Notfound");
            }
        }

        // POST: Area/Delete/5
        [HttpPost]
        public ActionResult Delete(string id)
        {
            try
            {
                if (Utility.KiemTra())
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (!Utility.KiemTraQuyen(API.dm_ThangLuong, API.Delete))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                var apiResponse = Utility.Delete<v_dm_ThangLuong>(Utility.LOC_ID + "/" + id, API.dm_ThangLuong);
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
                if (!Utility.KiemTraQuyen(API.dm_ThangLuong, API.Create))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                v_v_dm_ThangLuong dm_ThangLuong = new v_v_dm_ThangLuong();
                apiResponse.Success = true;
                dm_ThangLuong.LOC_ID = Utility.LOC_ID;
                dm_ThangLuong.ID = Guid.NewGuid().ToString();
                dm_ThangLuong.NAM = Utility.CurrentTime.Year;
                dm_ThangLuong.THANG = Utility.CurrentTime.Month;
                dm_ThangLuong.NAME = dm_ThangLuong.MA = Utility.CurrentTime.Month.ToString("00") + Utility.CurrentTime.Year.ToString();
                DateTime firstDayOfMonth = new DateTime(Utility.CurrentTime.Year, Utility.CurrentTime.Month, 1);
                dm_ThangLuong.NGAYBATDAU = firstDayOfMonth;
                DateTime lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
                dm_ThangLuong.NGAYKETTHUC = lastDayOfMonth;
                dm_ThangLuong.SONGAY = DateTime.DaysInMonth(Utility.CurrentTime.Year, Utility.CurrentTime.Month);
                dm_ThangLuong.SONGAYCONG = dm_ThangLuong.SONGAY - CountSundaysInMonth(Utility.CurrentTime.Year, Utility.CurrentTime.Month);
                dm_ThangLuong.ISCHAMCONG = true;
                dm_ThangLuong.ISACTIVE = true;
                apiResponse.Detail = Utility.ConvertobjectTo<v_v_dm_ThangLuong>(dm_ThangLuong);
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
        static int CountSundaysInMonth(int year, int month)
        {
            int count = 0;
            DateTime firstDayOfMonth = new DateTime(year, month, 1);
            int daysInMonth = DateTime.DaysInMonth(year, month);

            for (int day = 1; day <= daysInMonth; day++)
            {
                DateTime currentDay = new DateTime(year, month, day);
                if (currentDay.DayOfWeek == DayOfWeek.Sunday)
                {
                    count++;
                }
            }

            return count;
        }
        // POST: Menu/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult CreatePopup([Bind(Include = "LOC_ID,ID,MA,NAME,THANG,NAM,SONGAY,SONGAYCONG,GHICHU,NGAYBATDAU,NGAYKETTHUC,ID_PHONGBAN,ISCHAMCONG,ISACTIVE,GIOBATDAU,GIOKETTHUC,SOGIONGHITRUA,DANHSACHNGAYNGHI")] v_v_dm_ThangLuong dm_ThangLuong)
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                if (Utility.KiemTra())
                {
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Admin");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                if (!Utility.KiemTraQuyen(API.dm_ThangLuong, API.Create))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                if (ModelState.IsValid)
                {
                    dm_ThangLuong.LOC_ID = Utility.LOC_ID;
                    //dm_ThangLuong.ID_NGUOITAO = Session[Sessions.idUser].ToString();
                    //dm_ThangLuong.THOIGIANTHEM = Utility.CurrentTime;

                    apiResponse = Utility.Create<dm_ThangLuong>(dm_ThangLuong, API.dm_ThangLuong);
                    if (apiResponse.Success)
                    {
                        apiResponse.NewID = Guid.NewGuid().ToString();
                        if (apiResponse.Data != null && !string.IsNullOrEmpty(apiResponse.Data.ToString()))
                            dm_ThangLuong = JsonConvert.DeserializeObject<v_v_dm_ThangLuong>(apiResponse.Data.ToString());
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
                    apiResponse.Data = Utility.GetModelState(ModelState, API.dm_ThangLuong);
                }
                apiResponse.ID = dm_ThangLuong.ID;
                apiResponse.Detail = Utility.ConvertobjectToView<v_dm_ThangLuong>(dm_ThangLuong);
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
                if (!Utility.KiemTraQuyen(API.dm_ThangLuong, API.Edit))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                v_v_dm_ThangLuong dm_ThangLuong = new v_v_dm_ThangLuong();
                if (!string.IsNullOrEmpty(id))
                {
                    apiResponse = Utility.GetDetail<v_v_dm_ThangLuong>(Utility.LOC_ID + "/" + id, API.dm_ThangLuong);
                    if (!apiResponse.Success)
                    {
                        TempData["TitleError"] = apiResponse.Message;
                        apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                        return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                    }
                    if (apiResponse.Data != null)
                        dm_ThangLuong = apiResponse.Data as v_v_dm_ThangLuong;
                }
                apiResponse.Success = true;
                apiResponse.Detail = Utility.ConvertobjectTo<v_v_dm_ThangLuong>(dm_ThangLuong);
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
        public ActionResult EditPopup([Bind(Include = "LOC_ID,ID,MA,NAME,THANG,NAM,SONGAY,SONGAYCONG,GHICHU,NGAYBATDAU,NGAYKETTHUC,ID_PHONGBAN,ISCHAMCONG,ISACTIVE,GIOBATDAU,GIOKETTHUC,SOGIONGHITRUA,DANHSACHNGAYNGHI")] v_v_dm_ThangLuong dm_ThangLuong)
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                if (Utility.KiemTra())
                {
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Admin");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                if (!Utility.KiemTraQuyen(API.dm_ThangLuong, API.Edit))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                if (ModelState.IsValid)
                {
                    dm_ThangLuong.LOC_ID = Utility.LOC_ID;
                    //dm_ThangLuong.ID_NGUOISUA = Session[Sessions.idUser].ToString();
                    //dm_ThangLuong.THOIGIANSUA = Utility.CurrentTime;

                    apiResponse = Utility.Edit<v_dm_ThangLuong>(Utility.LOC_ID + "/" + dm_ThangLuong.MA, dm_ThangLuong, API.dm_ThangLuong);
                    if (apiResponse.Success)
                    {
                        apiResponse.ID = dm_ThangLuong.ID;
                        if (apiResponse.Data != null && !string.IsNullOrEmpty(apiResponse.Data.ToString()))
                            dm_ThangLuong = JsonConvert.DeserializeObject<v_v_dm_ThangLuong>(apiResponse.Data.ToString());
                        
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, apiResponse.Message);
                    }
                }
                else
                {
                    apiResponse.Success = false;
                    apiResponse.Data = Utility.GetModelState(ModelState, API.dm_ThangLuong);
                }
                apiResponse.Detail = Utility.ConvertobjectToView<v_dm_ThangLuong>(dm_ThangLuong);
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
                if (!Utility.KiemTraQuyen(API.dm_ThangLuong, API.Delete))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                apiResponse = Utility.Delete<v_dm_ThangLuong>(Utility.LOC_ID + "/" + id, API.dm_ThangLuong);
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