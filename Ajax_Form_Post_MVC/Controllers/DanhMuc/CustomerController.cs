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
    public class CustomerController : Controller
    {

        // GET: Customer
        public ActionResult Index(int Page = 1, string SearchString = "", string ShowSearchValue = "")
        {
            try
            {
                if (Utility.KiemTra())
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (!Utility.KiemTraQuyen(API.dm_KhachHang, API.Xem))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                ShowSearchValue = Utility.GetShowSearchValue<dm_KhachHang>(ShowSearchValue);
                var apiResponse = Utility.GetListData<v_dm_KhachHang>(API.dm_KhachHang, ShowSearchValue, SearchString, Utility.LOC_ID);
                if (!apiResponse.Success)
                {
                    TempData["TitleError"] = apiResponse.Message;
                    return RedirectToAction("Index", "Notfound");
                }
                IPagedList<v_dm_KhachHang> lstpage = (apiResponse.Data as List<v_dm_KhachHang>).ToPagedList(Page, Utility.GetPageSize());

                v_v_dm_KhachHang dm_KhachHang = new v_v_dm_KhachHang();
                dm_KhachHang.IPagedList = lstpage;
                dm_KhachHang.lstdm_KhuVuc = new List<v_dm_KhuVuc>();
                dm_KhachHang.lstdm_KhuVuc = Utility.GetListData<v_dm_KhuVuc>(API.dm_KhuVuc, "", "", Utility.LOC_ID).Data as List<v_dm_KhuVuc>;
                dm_KhachHang.lstdm_NhomKhachHang = new List<v_dm_NhomKhachHang>();
                dm_KhachHang.lstdm_NhomKhachHang = Utility.GetListData<v_dm_NhomKhachHang>(API.dm_NhomKhachHang, "", "", Utility.LOC_ID).Data as List<v_dm_NhomKhachHang>;


                ViewBag.searchValue = SearchString;
                ViewBag.showsearchValue = ShowSearchValue;

                ViewBag.PermissionEdit = Utility.KiemTraQuyen(API.dm_KhachHang, API.Edit);
                ViewBag.PermissionDelete = Utility.KiemTraQuyen(API.dm_KhachHang, API.Delete);
                ViewBag.PermissionCreate = Utility.KiemTraQuyen(API.dm_KhachHang, API.Create);
                return View(dm_KhachHang);
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
                TempData["TitleError"] = API.TitleTryCatch;
                TempData["DetailError"] = ex.Message;
                return RedirectToAction("Index", "Notfound");
            }
        }

        // GET: Customer/Create
        public ActionResult Create(int type = 2)
        {
            try
            {
                Session[Sessions.IntWidth] = type;
                if (Utility.KiemTra())
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (!Utility.KiemTraQuyen(API.dm_KhachHang, API.Create))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                v_v_dm_KhachHang dm_KhachHang = new v_v_dm_KhachHang();
                dm_KhachHang.LOC_ID = Utility.LOC_ID;
                dm_KhachHang.ID_NGUOITAO = Session[Sessions.idUser].ToString();
                dm_KhachHang.THOIGIANTHEM = Utility.CurrentTime;

                dm_KhachHang.ID = Guid.NewGuid().ToString();
                dm_KhachHang.lstdm_KhuVuc = new List<v_dm_KhuVuc>();
                dm_KhachHang.lstdm_KhuVuc = Utility.GetListData<v_dm_KhuVuc>(API.dm_KhuVuc, "", "", Utility.LOC_ID).Data as List<v_dm_KhuVuc>;
                dm_KhachHang.lstdm_NhomKhachHang = new List<v_dm_NhomKhachHang>();
                dm_KhachHang.lstdm_NhomKhachHang = Utility.GetListData<v_dm_NhomKhachHang>(API.dm_NhomKhachHang, "", "", Utility.LOC_ID).Data as List<v_dm_NhomKhachHang>;

                return View(dm_KhachHang);
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
                TempData["TitleError"] = API.TitleTryCatch;
                TempData["DetailError"] = ex.Message;
                return RedirectToAction("Index", "Notfound");
            }
        }

        // POST: Customer/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "LOC_ID,ID,MA,NAME,ADDRESS,TEL,FAX,EMAIL,NGAYSINH,DIS,RATE,ID_NHOMKHACHHANG,MAX_CONGNO,SONGAY,MAHANG_KH_LK,LEVEL_PRICE,ID_KHUVUC,ISACTIVE,ISDEFAULT,CONGNODAUKY,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO")] v_v_dm_KhachHang dm_KhachHang)
        {
            try
            {
                if (Utility.KiemTra())
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (!Utility.KiemTraQuyen(API.dm_KhachHang, API.Create))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                if (ModelState.IsValid)
                {
                    dm_KhachHang.LOC_ID = Utility.LOC_ID;
                    dm_KhachHang.ID_NGUOITAO = Session[Sessions.idUser].ToString();
                    dm_KhachHang.THOIGIANTHEM = Utility.CurrentTime;

                    //@ConvertObjectUnicodeToTCVN3
                    var apiResponse = Utility.Create<dm_KhachHang>(dm_KhachHang, API.dm_KhachHang);
                    if (apiResponse.Success)
                        return RedirectToAction("Index");
                    else
                    {
                        ModelState.AddModelError(string.Empty, apiResponse.Message);
                        if (apiResponse.CheckValue)
                            ViewBag.ID = Guid.NewGuid().ToString();
                    }
                }
                return View(dm_KhachHang);
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
                TempData["TitleError"] = API.TitleTryCatch;
                TempData["DetailError"] = ex.Message;
                return RedirectToAction("Index", "Notfound");
            }
        }

        // GET: Customer/Edit/5
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
                if (!Utility.KiemTraQuyen(API.dm_KhachHang, API.Edit))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                v_v_dm_KhachHang dm_KhachHang = new v_v_dm_KhachHang();
                if (!string.IsNullOrEmpty(id))
                {
                    apiResponse = Utility.GetDetail<v_v_dm_KhachHang>(Utility.LOC_ID + "/" + id, API.dm_KhachHang);
                    if (!apiResponse.Success)
                    {
                        TempData["TitleError"] = apiResponse.Message;
                        return RedirectToAction("Index", "Notfound");
                    }
                    if (apiResponse.Data != null)
                        dm_KhachHang = apiResponse.Data as v_v_dm_KhachHang;
                }
                //@ConvertObjectTCVN3ToUnicode
                dm_KhachHang.lstdm_KhuVuc = new List<v_dm_KhuVuc>();
                dm_KhachHang.lstdm_KhuVuc = Utility.GetListData<v_dm_KhuVuc>(API.dm_KhuVuc, "", "", Utility.LOC_ID).Data as List<v_dm_KhuVuc>;
                dm_KhachHang.lstdm_NhomKhachHang = new List<v_dm_NhomKhachHang>();
                dm_KhachHang.lstdm_NhomKhachHang = Utility.GetListData<v_dm_NhomKhachHang>(API.dm_NhomKhachHang, "", "", Utility.LOC_ID).Data as List<v_dm_NhomKhachHang>;

                return View(dm_KhachHang);
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
                TempData["TitleError"] = API.TitleTryCatch;
                TempData["DetailError"] = ex.Message;
                return RedirectToAction("Index", "Notfound");
            }
        }

        // POST: Customer/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "LOC_ID,ID,MA,NAME,ADDRESS,TEL,FAX,EMAIL,NGAYSINH,DIS,RATE,ID_NHOMKHACHHANG,MAX_CONGNO,SONGAY,MAHANG_KH_LK,LEVEL_PRICE,ID_KHUVUC,ISACTIVE,ISDEFAULT,CONGNODAUKY,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO")] v_v_dm_KhachHang dm_KhachHang)
        {
            try
            {
                if (Utility.KiemTra())
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (!Utility.KiemTraQuyen(API.dm_KhachHang, API.Edit))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                if (ModelState.IsValid)
                {
                    dm_KhachHang.LOC_ID = Utility.LOC_ID;
                    dm_KhachHang.ID_NGUOISUA = Session[Sessions.idUser].ToString();
                    dm_KhachHang.THOIGIANSUA = Utility.CurrentTime;
                    //@ConvertObjectUnicodeToTCVN3
                    var apiResponse = Utility.Edit<v_dm_KhachHang>(Utility.LOC_ID + "/" + dm_KhachHang.MA, dm_KhachHang, API.dm_KhachHang);
                    if (apiResponse.Success)
                        return RedirectToAction("Index");
                    else
                        ModelState.AddModelError(string.Empty, apiResponse.Message);
                }
                return View(dm_KhachHang);
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
                TempData["TitleError"] = API.TitleTryCatch;
                TempData["DetailError"] = ex.Message;
                return RedirectToAction("Index", "Notfound");
            }
        }

        // POST: Customer/Delete/5
        [HttpPost]
        public ActionResult Delete(string id)
        {
            try
            {
                if (Utility.KiemTra())
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (!Utility.KiemTraQuyen(API.dm_KhachHang, API.Delete))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                var apiResponse = Utility.Delete<v_dm_KhachHang>(Utility.LOC_ID + "/" + id, API.dm_KhachHang);
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
                if (!Utility.KiemTraQuyen(API.dm_KhachHang, API.Create))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                v_v_dm_KhachHang dm_KhachHang = new v_v_dm_KhachHang();
                dm_KhachHang.lstdm_KhuVuc = new List<v_dm_KhuVuc>();
                dm_KhachHang.lstdm_KhuVuc = Utility.GetListData<v_dm_KhuVuc>(API.dm_KhuVuc, "", "", Utility.LOC_ID).Data as List<v_dm_KhuVuc>;
                dm_KhachHang.lstdm_NhomKhachHang = new List<v_dm_NhomKhachHang>();
                dm_KhachHang.lstdm_NhomKhachHang = Utility.GetListData<v_dm_NhomKhachHang>(API.dm_NhomKhachHang, "", "", Utility.LOC_ID).Data as List<v_dm_NhomKhachHang>;
                apiResponse.Success = true;
                dm_KhachHang.LOC_ID = Utility.LOC_ID;
                dm_KhachHang.ID = Guid.NewGuid().ToString();
                apiResponse.Detail = Utility.ConvertobjectTo<dm_KhachHang>(dm_KhachHang);
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
        public ActionResult CreatePopup([Bind(Include = "LOC_ID,ID,MA,NAME,ADDRESS,TEL,FAX,EMAIL,NGAYSINH,DIS,RATE,ID_NHOMKHACHHANG,MAX_CONGNO,SONGAY,MAHANG_KH_LK,LEVEL_PRICE,ID_KHUVUC,ISACTIVE,ISDEFAULT,CONGNODAUKY,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO")] v_v_dm_KhachHang dm_KhachHang)
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                if (Utility.KiemTra())
                {
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Admin");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                if (!Utility.KiemTraQuyen(API.dm_KhachHang, API.Create))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                if (ModelState.IsValid)
                {
                    dm_KhachHang.LOC_ID = Utility.LOC_ID;
                    dm_KhachHang.ID_NGUOITAO = Session[Sessions.idUser].ToString();
                    dm_KhachHang.THOIGIANTHEM = Utility.CurrentTime;

                    apiResponse = Utility.Create<dm_KhachHang>(dm_KhachHang, API.dm_KhachHang);
                    if (apiResponse.Success)
                    {
                        apiResponse.NewID = Guid.NewGuid().ToString();
                        if (apiResponse.Data != null)
                            dm_KhachHang = JsonConvert.DeserializeObject<v_v_dm_KhachHang>(apiResponse.Data.ToString());
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
                    apiResponse.Data = Utility.GetModelState(ModelState, API.dm_KhachHang);
                }
                apiResponse.ID = dm_KhachHang.ID;
                apiResponse.Detail = Utility.ConvertobjectToView<dm_KhachHang>(dm_KhachHang);
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
                if (!Utility.KiemTraQuyen(API.dm_KhachHang, API.Edit))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                v_v_dm_KhachHang dm_KhachHang = new v_v_dm_KhachHang();
                if (!string.IsNullOrEmpty(id))
                {
                    apiResponse = Utility.GetDetail<v_v_dm_KhachHang>(Utility.LOC_ID + "/" + id, API.dm_KhachHang);
                    if (!apiResponse.Success)
                    {
                        TempData["TitleError"] = apiResponse.Message;
                        apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                        return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                    }
                    if (apiResponse.Data != null)
                        dm_KhachHang = apiResponse.Data as v_v_dm_KhachHang;
                }
                dm_KhachHang.lstdm_KhuVuc = new List<v_dm_KhuVuc>();
                dm_KhachHang.lstdm_KhuVuc = Utility.GetListData<v_dm_KhuVuc>(API.dm_KhuVuc, "", "", Utility.LOC_ID).Data as List<v_dm_KhuVuc>;
                dm_KhachHang.lstdm_NhomKhachHang = new List<v_dm_NhomKhachHang>();
                dm_KhachHang.lstdm_NhomKhachHang = Utility.GetListData<v_dm_NhomKhachHang>(API.dm_NhomKhachHang, "", "", Utility.LOC_ID).Data as List<v_dm_NhomKhachHang>;
                apiResponse.Success = true;
                apiResponse.Detail = Utility.ConvertobjectTo<dm_KhachHang>(dm_KhachHang);
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
        public ActionResult EditPopup([Bind(Include = "LOC_ID,ID,MA,NAME,ADDRESS,TEL,FAX,EMAIL,NGAYSINH,DIS,RATE,ID_NHOMKHACHHANG,MAX_CONGNO,SONGAY,MAHANG_KH_LK,LEVEL_PRICE,ID_KHUVUC,ISACTIVE,ISDEFAULT,CONGNODAUKY,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO")] v_v_dm_KhachHang dm_KhachHang)
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                if (Utility.KiemTra())
                {
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Admin");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                if (!Utility.KiemTraQuyen(API.dm_KhachHang, API.Edit))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                if (ModelState.IsValid)
                {
                    dm_KhachHang.LOC_ID = Utility.LOC_ID;
                    dm_KhachHang.ID_NGUOISUA = Session[Sessions.idUser].ToString();
                    dm_KhachHang.THOIGIANSUA = Utility.CurrentTime;

                    apiResponse = Utility.Edit<v_dm_KhachHang>(Utility.LOC_ID + "/" + dm_KhachHang.MA, dm_KhachHang, API.dm_KhachHang);
                    if (apiResponse.Success)
                    {
                        //return RedirectToAction("Index");
                        apiResponse.ID = dm_KhachHang.ID;
                        if (apiResponse.Data != null)
                            dm_KhachHang = JsonConvert.DeserializeObject<v_v_dm_KhachHang>(apiResponse.Data.ToString());
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, apiResponse.Message);
                    }
                }
                else
                {
                    apiResponse.Success = false;
                    apiResponse.Data = Utility.GetModelState(ModelState, API.dm_KhachHang);
                }
                apiResponse.Detail = Utility.ConvertobjectToView<v_dm_KhachHang>(dm_KhachHang);
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
                if (!Utility.KiemTraQuyen(API.dm_KhachHang, API.Delete))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                apiResponse = Utility.Delete<v_dm_KhachHang>(Utility.LOC_ID + "/" + id, API.dm_KhachHang);
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