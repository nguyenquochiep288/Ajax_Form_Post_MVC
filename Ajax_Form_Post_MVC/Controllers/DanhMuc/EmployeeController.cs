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
    public class EmployeeController : Controller
    {

        // GET: Employee
        public ActionResult Index(int Page = 1, string SearchString = "", string ShowSearchValue = "")
        {
            try
            {
                if (Utility.KiemTra())
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (!Utility.KiemTraQuyen(API.dm_NhanVien, API.Xem))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                ShowSearchValue = Utility.GetShowSearchValue<dm_NhanVien>(ShowSearchValue);
                var apiResponse = Utility.GetListData<v_dm_NhanVien>(API.dm_NhanVien, ShowSearchValue, SearchString, Utility.LOC_ID);
                if (!apiResponse.Success)
                {
                    TempData["TitleError"] = apiResponse.Message;
                    return RedirectToAction("Index", "Notfound");
                }
                IPagedList<v_dm_NhanVien> lstpage = (apiResponse.Data as List<v_dm_NhanVien>).ToPagedList(Page, Utility.GetPageSize());

                v_v_dm_NhanVien dm_NhanVien = new v_v_dm_NhanVien();
                dm_NhanVien.IPagedList = lstpage;
                dm_NhanVien.lstdm_ChucVu = new List<v_dm_ChucVu>();
                dm_NhanVien.lstdm_ChucVu = Utility.GetListData<v_dm_ChucVu>(API.dm_ChucVu, "", "", Utility.LOC_ID).Data as List<v_dm_ChucVu>;
                dm_NhanVien.lstdm_PhongBan = new List<v_dm_PhongBan>();
                dm_NhanVien.lstdm_PhongBan = Utility.GetListData<v_dm_PhongBan>(API.dm_PhongBan, "", "", Utility.LOC_ID).Data as List<v_dm_PhongBan>;


                ViewBag.searchValue = SearchString;
                ViewBag.showsearchValue = ShowSearchValue;

                ViewBag.PermissionEdit = Utility.KiemTraQuyen(API.dm_NhanVien, API.Edit);
                ViewBag.PermissionDelete = Utility.KiemTraQuyen(API.dm_NhanVien, API.Delete);
                ViewBag.PermissionCreate = Utility.KiemTraQuyen(API.dm_NhanVien, API.Create);
                return View(dm_NhanVien);
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
                TempData["TitleError"] = API.TitleTryCatch;
                TempData["DetailError"] = ex.Message;
                return RedirectToAction("Index", "Notfound");
            }
        }

        // GET: Employee/Create
        public ActionResult Create(int type = 2)
        {
            try
            {
                Session[Sessions.IntWidth] = type;
                if (Utility.KiemTra())
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (!Utility.KiemTraQuyen(API.dm_NhanVien, API.Create))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                v_v_dm_NhanVien dm_NhanVien = new v_v_dm_NhanVien();
                dm_NhanVien.LOC_ID = Utility.LOC_ID;
                dm_NhanVien.ID_NGUOITAO = Session[Sessions.idUser].ToString();
                dm_NhanVien.THOIGIANTHEM = Utility.CurrentTime;

                dm_NhanVien.ID = Guid.NewGuid().ToString();
                dm_NhanVien.lstdm_ChucVu = new List<v_dm_ChucVu>();
                dm_NhanVien.lstdm_ChucVu = Utility.GetListData<v_dm_ChucVu>(API.dm_ChucVu, "", "", Utility.LOC_ID).Data as List<v_dm_ChucVu>;
                dm_NhanVien.lstdm_PhongBan = new List<v_dm_PhongBan>();
                dm_NhanVien.lstdm_PhongBan = Utility.GetListData<v_dm_PhongBan>(API.dm_PhongBan, "", "", Utility.LOC_ID).Data as List<v_dm_PhongBan>;
                dm_NhanVien.lstAspNetUsers = new List<v_AspNetUsers>();
                dm_NhanVien.lstAspNetUsers = Utility.GetListData<v_AspNetUsers>(API.AspNetUser, "", "", "").Data as List<v_AspNetUsers>;
                foreach (var itm in dm_NhanVien.lstAspNetUsers)
                    itm.NAME = itm.UserName;

                return View(dm_NhanVien);
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
                TempData["TitleError"] = API.TitleTryCatch;
                TempData["DetailError"] = ex.Message;
                return RedirectToAction("Index", "Notfound");
            }
        }

        // POST: Employee/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "LOC_ID,ID,MA,NAME,ID_CHUCVU,GIOITINH,ADDRESS,TEL,ID_NUMBER,DATEOFBIRTH,DATEJOIN,LUONGCB,QUYCD,BHXH_ND,BHXH_NLD,DATCOC,ID_PHONGBAN,LOAINHANVIEN,EMAIL,GHICHU,LUONG_BH,TIENAN,TIENSOANHANG,TIENGIAYIN,STT_MAYCHAMCONG,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO,ISACTIVE,ID_TAIKHOAN,CONGNODAUKY,LUONGCOBAN,SONGAYPHEP")] v_v_dm_NhanVien dm_NhanVien)
        {
            try
            {
                if (Utility.KiemTra())
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (!Utility.KiemTraQuyen(API.dm_NhanVien, API.Create))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                if (ModelState.IsValid)
                {
                    dm_NhanVien.LOC_ID = Utility.LOC_ID;
                    dm_NhanVien.ID_NGUOITAO = Session[Sessions.idUser].ToString();
                    dm_NhanVien.THOIGIANTHEM = Utility.CurrentTime;

                    //@ConvertObjectUnicodeToTCVN3
                    var apiResponse = Utility.Create<dm_NhanVien>(dm_NhanVien, API.dm_NhanVien);
                    if (apiResponse.Success)
                        return RedirectToAction("Index");
                    else
                    {
                        ModelState.AddModelError(string.Empty, apiResponse.Message);
                        if (apiResponse.CheckValue)
                            ViewBag.ID = Guid.NewGuid().ToString();
                    }
                }
                return View(dm_NhanVien);
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
                TempData["TitleError"] = API.TitleTryCatch;
                TempData["DetailError"] = ex.Message;
                return RedirectToAction("Index", "Notfound");
            }
        }

        // GET: Employee/Edit/5
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
                if (!Utility.KiemTraQuyen(API.dm_NhanVien, API.Edit))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                v_v_dm_NhanVien dm_NhanVien = new v_v_dm_NhanVien();
                if (!string.IsNullOrEmpty(id))
                {
                    apiResponse = Utility.GetDetail<v_v_dm_NhanVien>(Utility.LOC_ID + "/" + id, API.dm_NhanVien);
                    if (!apiResponse.Success)
                    {
                        TempData["TitleError"] = apiResponse.Message;
                        return RedirectToAction("Index", "Notfound");
                    }
                    if (apiResponse.Data != null)
                        dm_NhanVien = apiResponse.Data as v_v_dm_NhanVien;
                }
                //@ConvertObjectTCVN3ToUnicode
                dm_NhanVien.lstdm_ChucVu = new List<v_dm_ChucVu>();
                dm_NhanVien.lstdm_ChucVu = Utility.GetListData<v_dm_ChucVu>(API.dm_ChucVu, "", "", Utility.LOC_ID).Data as List<v_dm_ChucVu>;
                dm_NhanVien.lstdm_PhongBan = new List<v_dm_PhongBan>();
                dm_NhanVien.lstdm_PhongBan = Utility.GetListData<v_dm_PhongBan>(API.dm_PhongBan, "", "", Utility.LOC_ID).Data as List<v_dm_PhongBan>;
                dm_NhanVien.lstAspNetUsers = new List<v_AspNetUsers>();
                dm_NhanVien.lstAspNetUsers = Utility.GetListData<v_AspNetUsers>(API.AspNetUser, "", "", "").Data as List<v_AspNetUsers>;
                foreach (var itm in dm_NhanVien.lstAspNetUsers)
                    itm.NAME = itm.UserName;
                return View(dm_NhanVien);
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
                TempData["TitleError"] = API.TitleTryCatch;
                TempData["DetailError"] = ex.Message;
                return RedirectToAction("Index", "Notfound");
            }
        }

        // POST: Employee/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "LOC_ID,ID,MA,NAME,ID_CHUCVU,GIOITINH,ADDRESS,TEL,ID_NUMBER,DATEOFBIRTH,DATEJOIN,LUONGCB,QUYCD,BHXH_ND,BHXH_NLD,DATCOC,ID_PHONGBAN,LOAINHANVIEN,EMAIL,GHICHU,LUONG_BH,TIENAN,TIENSOANHANG,TIENGIAYIN,STT_MAYCHAMCONG,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO,ISACTIVE,ID_TAIKHOAN,CONGNODAUKY,LUONGCOBAN,SONGAYPHEP")] v_v_dm_NhanVien dm_NhanVien)
        {
            try
            {
                if (Utility.KiemTra())
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (!Utility.KiemTraQuyen(API.dm_NhanVien, API.Edit))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                if (ModelState.IsValid)
                {
                    dm_NhanVien.LOC_ID = Utility.LOC_ID;
                    dm_NhanVien.ID_NGUOISUA = Session[Sessions.idUser].ToString();
                    dm_NhanVien.THOIGIANSUA = Utility.CurrentTime;
                    //@ConvertObjectUnicodeToTCVN3
                    var apiResponse = Utility.Edit<v_dm_NhanVien>(Utility.LOC_ID + "/" + dm_NhanVien.MA, dm_NhanVien, API.dm_NhanVien);
                    if (apiResponse.Success)
                        return RedirectToAction("Index");
                    else
                        ModelState.AddModelError(string.Empty, apiResponse.Message);

                }
                return View(dm_NhanVien);
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
                TempData["TitleError"] = API.TitleTryCatch;
                TempData["DetailError"] = ex.Message;
                return RedirectToAction("Index", "Notfound");
            }
        }

        // POST: Employee/Delete/5
        [HttpPost]
        public ActionResult Delete(string id)
        {
            try
            {
                if (Utility.KiemTra())
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (!Utility.KiemTraQuyen(API.dm_NhanVien, API.Delete))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                var apiResponse = Utility.Delete<v_dm_NhanVien>(Utility.LOC_ID + "/" + id, API.dm_NhanVien);
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
                if (!Utility.KiemTraQuyen(API.dm_NhanVien, API.Create))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                v_v_dm_NhanVien dm_NhanVien = new v_v_dm_NhanVien();
                dm_NhanVien.lstdm_ChucVu = new List<v_dm_ChucVu>();
                dm_NhanVien.lstdm_ChucVu = Utility.GetListData<v_dm_ChucVu>(API.dm_ChucVu, "", "", Utility.LOC_ID).Data as List<v_dm_ChucVu>;
                dm_NhanVien.lstdm_PhongBan = new List<v_dm_PhongBan>();
                dm_NhanVien.lstdm_PhongBan = Utility.GetListData<v_dm_PhongBan>(API.dm_PhongBan, "", "", Utility.LOC_ID).Data as List<v_dm_PhongBan>;
                dm_NhanVien.lstAspNetUsers = new List<v_AspNetUsers>();
                dm_NhanVien.lstAspNetUsers = Utility.GetListData<v_AspNetUsers>(API.AspNetUser, "", "", "").Data as List<v_AspNetUsers>;
                foreach (var itm in dm_NhanVien.lstAspNetUsers)
                    itm.NAME = itm.UserName;
                apiResponse.Success = true;
                dm_NhanVien.LOC_ID = Utility.LOC_ID;
                dm_NhanVien.ID = Guid.NewGuid().ToString();
                apiResponse.Detail = Utility.ConvertobjectTo<dm_NhanVien>(dm_NhanVien);
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
        public ActionResult CreatePopup([Bind(Include = "LOC_ID,ID,MA,NAME,ID_CHUCVU,GIOITINH,ADDRESS,TEL,ID_NUMBER,DATEOFBIRTH,DATEJOIN,LUONGCB,QUYCD,BHXH_ND,BHXH_NLD,DATCOC,ID_PHONGBAN,LOAINHANVIEN,EMAIL,GHICHU,LUONG_BH,TIENAN,TIENSOANHANG,TIENGIAYIN,STT_MAYCHAMCONG,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO,ISACTIVE,ID_TAIKHOAN,CONGNODAUKY,LUONGCOBAN,SONGAYPHEP")] v_v_dm_NhanVien dm_NhanVien)
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                if (Utility.KiemTra())
                {
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Admin");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                if (!Utility.KiemTraQuyen(API.dm_NhanVien, API.Create))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                if (ModelState.IsValid)
                {
                    dm_NhanVien.LOC_ID = Utility.LOC_ID;
                    dm_NhanVien.ID_NGUOITAO = Session[Sessions.idUser].ToString();
                    dm_NhanVien.THOIGIANTHEM = Utility.CurrentTime;

                    apiResponse = Utility.Create<dm_NhanVien>(dm_NhanVien, API.dm_NhanVien);
                    if (apiResponse.Success)
                    {
                        apiResponse.NewID = Guid.NewGuid().ToString();
                        if (apiResponse.Data != null)
                            dm_NhanVien = JsonConvert.DeserializeObject<v_v_dm_NhanVien>(apiResponse.Data.ToString());
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
                    apiResponse.Data = Utility.GetModelState(ModelState, API.dm_NhanVien);
                }
                apiResponse.ID = dm_NhanVien.ID;
                dm_NhanVien.lstdm_ChucVu = new List<v_dm_ChucVu>();
                dm_NhanVien.lstdm_ChucVu = Utility.GetListData<v_dm_ChucVu>(API.dm_ChucVu, "", "", Utility.LOC_ID).Data as List<v_dm_ChucVu>;
                dm_NhanVien.lstdm_PhongBan = new List<v_dm_PhongBan>();
                dm_NhanVien.lstdm_PhongBan = Utility.GetListData<v_dm_PhongBan>(API.dm_PhongBan, "", "", Utility.LOC_ID).Data as List<v_dm_PhongBan>;
                dm_NhanVien.lstAspNetUsers = new List<v_AspNetUsers>();
                dm_NhanVien.lstAspNetUsers = Utility.GetListData<v_AspNetUsers>(API.AspNetUser, "", "", "").Data as List<v_AspNetUsers>;
                foreach (var itm in dm_NhanVien.lstAspNetUsers)
                    itm.NAME = itm.UserName;
                apiResponse.Detail = Utility.ConvertobjectToView<v_dm_NhanVien>(dm_NhanVien);
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
                if (!Utility.KiemTraQuyen(API.dm_NhanVien, API.Edit))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                v_v_dm_NhanVien dm_NhanVien = new v_v_dm_NhanVien();
                if (!string.IsNullOrEmpty(id))
                {
                    apiResponse = Utility.GetDetail<v_v_dm_NhanVien>(Utility.LOC_ID + "/" + id, API.dm_NhanVien);
                    if (!apiResponse.Success)
                    {
                        TempData["TitleError"] = apiResponse.Message;
                        apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                        return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                    }
                    if (apiResponse.Data != null)
                        dm_NhanVien = apiResponse.Data as v_v_dm_NhanVien;
                }
                dm_NhanVien.lstdm_ChucVu = new List<v_dm_ChucVu>();
                dm_NhanVien.lstdm_ChucVu = Utility.GetListData<v_dm_ChucVu>(API.dm_ChucVu, "", "", Utility.LOC_ID).Data as List<v_dm_ChucVu>;
                dm_NhanVien.lstdm_PhongBan = new List<v_dm_PhongBan>();
                dm_NhanVien.lstdm_PhongBan = Utility.GetListData<v_dm_PhongBan>(API.dm_PhongBan, "", "", Utility.LOC_ID).Data as List<v_dm_PhongBan>;
                dm_NhanVien.lstAspNetUsers = new List<v_AspNetUsers>();
                dm_NhanVien.lstAspNetUsers = Utility.GetListData<v_AspNetUsers>(API.AspNetUser, "", "", "").Data as List<v_AspNetUsers>;
                foreach (var itm in dm_NhanVien.lstAspNetUsers)
                    itm.NAME = itm.UserName;
                apiResponse.Success = true;
                apiResponse.Detail = Utility.ConvertobjectTo<v_v_dm_NhanVien>(dm_NhanVien);
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
        public ActionResult EditPopup([Bind(Include = "LOC_ID,ID,MA,NAME,ID_CHUCVU,GIOITINH,ADDRESS,TEL,ID_NUMBER,DATEOFBIRTH,DATEJOIN,LUONGCB,QUYCD,BHXH_ND,BHXH_NLD,DATCOC,ID_PHONGBAN,LOAINHANVIEN,EMAIL,GHICHU,LUONG_BH,TIENAN,TIENSOANHANG,TIENGIAYIN,STT_MAYCHAMCONG,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO,ISACTIVE,ID_TAIKHOAN,CONGNODAUKY,LUONGCOBAN,SONGAYPHEP")] v_v_dm_NhanVien dm_NhanVien)
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                if (Utility.KiemTra())
                {
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Admin");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                if (!Utility.KiemTraQuyen(API.dm_NhanVien, API.Edit))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                if (ModelState.IsValid)
                {
                    dm_NhanVien.LOC_ID = Utility.LOC_ID;
                    dm_NhanVien.ID_NGUOISUA = Session[Sessions.idUser].ToString();
                    dm_NhanVien.THOIGIANSUA = Utility.CurrentTime;

                    apiResponse = Utility.Edit<v_dm_NhanVien>(Utility.LOC_ID + "/" + dm_NhanVien.MA, dm_NhanVien, API.dm_NhanVien);
                    if (apiResponse.Success)
                    {
                        apiResponse.ID = dm_NhanVien.ID;
                        if (apiResponse.Data != null)
                            dm_NhanVien = JsonConvert.DeserializeObject<v_v_dm_NhanVien>(apiResponse.Data.ToString());
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, apiResponse.Message);
                    }
                }
                else
                {
                    apiResponse.Success = false;
                    apiResponse.Data = Utility.GetModelState(ModelState, API.dm_NhanVien);
                }
                dm_NhanVien.lstdm_ChucVu = new List<v_dm_ChucVu>();
                dm_NhanVien.lstdm_ChucVu = Utility.GetListData<v_dm_ChucVu>(API.dm_ChucVu, "", "", Utility.LOC_ID).Data as List<v_dm_ChucVu>;
                dm_NhanVien.lstdm_PhongBan = new List<v_dm_PhongBan>();
                dm_NhanVien.lstdm_PhongBan = Utility.GetListData<v_dm_PhongBan>(API.dm_PhongBan, "", "", Utility.LOC_ID).Data as List<v_dm_PhongBan>;
                dm_NhanVien.lstAspNetUsers = new List<v_AspNetUsers>();
                dm_NhanVien.lstAspNetUsers = Utility.GetListData<v_AspNetUsers>(API.AspNetUser, "", "", "").Data as List<v_AspNetUsers>;
                foreach (var itm in dm_NhanVien.lstAspNetUsers)
                    itm.NAME = itm.UserName;
                apiResponse.Detail = Utility.ConvertobjectToView<v_dm_NhanVien>(dm_NhanVien);
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
                if (!Utility.KiemTraQuyen(API.dm_NhanVien, API.Delete))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                apiResponse = Utility.Delete<v_dm_NhanVien>(Utility.LOC_ID + "/" + id, API.dm_NhanVien);
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