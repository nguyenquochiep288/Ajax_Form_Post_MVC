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
    public class ProviderController : Controller
    {

        // GET: Provider
        public ActionResult Index(int Page = 1, string SearchString = "", string ShowSearchValue = "")
        {
            try
            {
                if (Utility.KiemTra())
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (!Utility.KiemTraQuyen(API.dm_NhaCungCap, API.Xem))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                ShowSearchValue = Utility.GetShowSearchValue<dm_NhaCungCap>(ShowSearchValue);
                var apiResponse = Utility.GetListData<v_dm_NhaCungCap>(API.dm_NhaCungCap, ShowSearchValue, SearchString, Utility.LOC_ID);
                if (!apiResponse.Success)
                {
                    TempData["TitleError"] = apiResponse.Message;
                    return RedirectToAction("Index", "Notfound");
                }
                IPagedList<v_dm_NhaCungCap> lstpage = (apiResponse.Data as List<v_dm_NhaCungCap>).ToPagedList(Page, Utility.GetPageSize());

                v_v_dm_NhaCungCap dm_NhaCungCap = new v_v_dm_NhaCungCap();
                dm_NhaCungCap.IPagedList = lstpage;
                dm_NhaCungCap.lstdm_NhomNhaCungCap = new List<v_dm_NhomNhaCungCap>();
                dm_NhaCungCap.lstdm_NhomNhaCungCap = Utility.GetListData<v_dm_NhomNhaCungCap>(API.dm_NhomNhaCungCap, "", "", Utility.LOC_ID).Data as List<v_dm_NhomNhaCungCap>;


                ViewBag.searchValue = SearchString;
                ViewBag.showsearchValue = ShowSearchValue;

                ViewBag.PermissionEdit = Utility.KiemTraQuyen(API.dm_NhaCungCap, API.Edit);
                ViewBag.PermissionDelete = Utility.KiemTraQuyen(API.dm_NhaCungCap, API.Delete);
                ViewBag.PermissionCreate = Utility.KiemTraQuyen(API.dm_NhaCungCap, API.Create);
                return View(dm_NhaCungCap);
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
                TempData["TitleError"] = API.TitleTryCatch;
                TempData["DetailError"] = ex.Message;
                return RedirectToAction("Index", "Notfound");
            }
        }

        // GET: Provider/Create
        public ActionResult Create(int type = 2)
        {
            try
            {
                Session[Sessions.IntWidth] = type;
                if (Utility.KiemTra())
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (!Utility.KiemTraQuyen(API.dm_NhaCungCap, API.Create))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                v_v_dm_NhaCungCap dm_NhaCungCap = new v_v_dm_NhaCungCap();
                dm_NhaCungCap.LOC_ID = Utility.LOC_ID;
                dm_NhaCungCap.ID_NGUOITAO = Session[Sessions.idUser].ToString();
                dm_NhaCungCap.THOIGIANTHEM = Utility.CurrentTime;

                dm_NhaCungCap.ID = Guid.NewGuid().ToString();
                dm_NhaCungCap.lstdm_NhomNhaCungCap = new List<v_dm_NhomNhaCungCap>();
                dm_NhaCungCap.lstdm_NhomNhaCungCap = Utility.GetListData<v_dm_NhomNhaCungCap>(API.dm_NhomNhaCungCap, "", "", Utility.LOC_ID).Data as List<v_dm_NhomNhaCungCap>;

                return View(dm_NhaCungCap);
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
                TempData["TitleError"] = API.TitleTryCatch;
                TempData["DetailError"] = ex.Message;
                return RedirectToAction("Index", "Notfound");
            }
        }

        // POST: Provider/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,LOC_ID,MA,NAME,ADDRESS,TEL,FAX,EMAIL,ID_NHOMNCC,ISACTIVE,ISDEFAULT,CONGNODAUKY,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO,MASOTHUE,TENNGANHANG,CHUTAIKHOAN,SOTAIKHOAN")] v_v_dm_NhaCungCap dm_NhaCungCap)
        {
            try
            {
                if (Utility.KiemTra())
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (!Utility.KiemTraQuyen(API.dm_NhaCungCap, API.Create))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                if (ModelState.IsValid)
                {
                    dm_NhaCungCap.LOC_ID = Utility.LOC_ID;
                    dm_NhaCungCap.ID_NGUOITAO = Session[Sessions.idUser].ToString();
                    dm_NhaCungCap.THOIGIANTHEM = Utility.CurrentTime;

                    //@ConvertObjectUnicodeToTCVN3
                    var apiResponse = Utility.Create<dm_NhaCungCap>(dm_NhaCungCap, API.dm_NhaCungCap);
                    if (apiResponse.Success)
                        return RedirectToAction("Index");
                    else
                    {
                        ModelState.AddModelError(string.Empty, apiResponse.Message);
                        if (apiResponse.CheckValue)
                            ViewBag.ID = Guid.NewGuid().ToString();
                    }
                }
                return View(dm_NhaCungCap);
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
                TempData["TitleError"] = API.TitleTryCatch;
                TempData["DetailError"] = ex.Message;
                return RedirectToAction("Index", "Notfound");
            }
        }

        // GET: Provider/Edit/5
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
                if (!Utility.KiemTraQuyen(API.dm_NhaCungCap, API.Edit))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                v_v_dm_NhaCungCap dm_NhaCungCap = new v_v_dm_NhaCungCap();
                if (!string.IsNullOrEmpty(id))
                {
                    apiResponse = Utility.GetDetail<v_v_dm_NhaCungCap>(Utility.LOC_ID + "/" + id, API.dm_NhaCungCap);
                    if (!apiResponse.Success)
                    {
                        TempData["TitleError"] = apiResponse.Message;
                        return RedirectToAction("Index", "Notfound");
                    }
                    if (apiResponse.Data != null)
                        dm_NhaCungCap = apiResponse.Data as v_v_dm_NhaCungCap;
                }
                //@ConvertObjectTCVN3ToUnicode
                dm_NhaCungCap.lstdm_NhomNhaCungCap = new List<v_dm_NhomNhaCungCap>();
                dm_NhaCungCap.lstdm_NhomNhaCungCap = Utility.GetListData<v_dm_NhomNhaCungCap>(API.dm_NhomNhaCungCap, "", "", Utility.LOC_ID).Data as List<v_dm_NhomNhaCungCap>;

                return View(dm_NhaCungCap);
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
                TempData["TitleError"] = API.TitleTryCatch;
                TempData["DetailError"] = ex.Message;
                return RedirectToAction("Index", "Notfound");
            }
        }

        // POST: Provider/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,LOC_ID,MA,NAME,ADDRESS,TEL,FAX,EMAIL,ID_NHOMNCC,ISACTIVE,ISDEFAULT,CONGNODAUKY,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO,MASOTHUE,TENNGANHANG,CHUTAIKHOAN,SOTAIKHOAN")] v_v_dm_NhaCungCap dm_NhaCungCap)
        {
            try
            {
                if (Utility.KiemTra())
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (!Utility.KiemTraQuyen(API.dm_NhaCungCap, API.Edit))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                if (ModelState.IsValid)
                {
                    dm_NhaCungCap.LOC_ID = Utility.LOC_ID;
                    dm_NhaCungCap.ID_NGUOISUA = Session[Sessions.idUser].ToString();
                    dm_NhaCungCap.THOIGIANSUA = Utility.CurrentTime;
                    //@ConvertObjectUnicodeToTCVN3
                    var apiResponse = Utility.Edit<v_dm_NhaCungCap>(Utility.LOC_ID + "/" + dm_NhaCungCap.MA, dm_NhaCungCap, API.dm_NhaCungCap);
                    if (apiResponse.Success)
                        return RedirectToAction("Index");
                    else
                        ModelState.AddModelError(string.Empty, apiResponse.Message);
                }
                return View(dm_NhaCungCap);
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
                TempData["TitleError"] = API.TitleTryCatch;
                TempData["DetailError"] = ex.Message;
                return RedirectToAction("Index", "Notfound");
            }
        }

        // POST: Provider/Delete/5
        [HttpPost]
        public ActionResult Delete(string id)
        {
            try
            {
                if (Utility.KiemTra())
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (!Utility.KiemTraQuyen(API.dm_NhaCungCap, API.Delete))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                var apiResponse = Utility.Delete<v_dm_NhaCungCap>(Utility.LOC_ID + "/" + id, API.dm_NhaCungCap);
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
                if (!Utility.KiemTraQuyen(API.dm_NhaCungCap, API.Create))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                v_v_dm_NhaCungCap dm_NhaCungCap = new v_v_dm_NhaCungCap();
                dm_NhaCungCap.lstdm_NhomNhaCungCap = new List<v_dm_NhomNhaCungCap>();
                dm_NhaCungCap.lstdm_NhomNhaCungCap = Utility.GetListData<v_dm_NhomNhaCungCap>(API.dm_NhomNhaCungCap, "", "", Utility.LOC_ID).Data as List<v_dm_NhomNhaCungCap>;
                apiResponse.Success = true;
                dm_NhaCungCap.LOC_ID = Utility.LOC_ID;
                dm_NhaCungCap.ID = Guid.NewGuid().ToString();
                apiResponse.Detail = Utility.ConvertobjectTo<dm_NhaCungCap>(dm_NhaCungCap);
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
        public ActionResult CreatePopup([Bind(Include = "ID,LOC_ID,MA,NAME,ADDRESS,TEL,FAX,EMAIL,ID_NHOMNCC,ISACTIVE,ISDEFAULT,CONGNODAUKY,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO,MASOTHUE,TENNGANHANG,CHUTAIKHOAN,SOTAIKHOAN")] v_v_dm_NhaCungCap dm_NhaCungCap)
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                if (Utility.KiemTra())
                {
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Admin");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                if (!Utility.KiemTraQuyen(API.dm_NhaCungCap, API.Create))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                if (ModelState.IsValid)
                {
                    dm_NhaCungCap.LOC_ID = Utility.LOC_ID;
                    dm_NhaCungCap.ID_NGUOITAO = Session[Sessions.idUser].ToString();
                    dm_NhaCungCap.THOIGIANTHEM = Utility.CurrentTime;

                    apiResponse = Utility.Create<dm_NhaCungCap>(dm_NhaCungCap, API.dm_NhaCungCap);
                    if (apiResponse.Success)
                    {
                        apiResponse.NewID = Guid.NewGuid().ToString();
                        if (apiResponse.Data != null && !string.IsNullOrEmpty(apiResponse.Data.ToString()))
                            dm_NhaCungCap = JsonConvert.DeserializeObject<v_v_dm_NhaCungCap>(apiResponse.Data.ToString());
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
                    apiResponse.Data = Utility.GetModelState(ModelState, API.dm_NhaCungCap);
                }
                apiResponse.ID = dm_NhaCungCap.ID;
                apiResponse.Detail = Utility.ConvertobjectToView<dm_NhaCungCap>(dm_NhaCungCap);
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
                if (!Utility.KiemTraQuyen(API.dm_NhaCungCap, API.Edit))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                v_v_dm_NhaCungCap dm_NhaCungCap = new v_v_dm_NhaCungCap();
                if (!string.IsNullOrEmpty(id))
                {
                    apiResponse = Utility.GetDetail<v_v_dm_NhaCungCap>(Utility.LOC_ID + "/" + id, API.dm_NhaCungCap);
                    if (!apiResponse.Success)
                    {
                        TempData["TitleError"] = apiResponse.Message;
                        apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                        return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                    }
                    if (apiResponse.Data != null)
                        dm_NhaCungCap = apiResponse.Data as v_v_dm_NhaCungCap;
                }
                dm_NhaCungCap.lstdm_NhomNhaCungCap = new List<v_dm_NhomNhaCungCap>();
                dm_NhaCungCap.lstdm_NhomNhaCungCap = Utility.GetListData<v_dm_NhomNhaCungCap>(API.dm_NhomNhaCungCap, "", "", Utility.LOC_ID).Data as List<v_dm_NhomNhaCungCap>;

                apiResponse.Success = true;
                apiResponse.Detail = Utility.ConvertobjectTo<v_v_dm_NhaCungCap>(dm_NhaCungCap);
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
        public ActionResult EditPopup([Bind(Include = "ID,LOC_ID,MA,NAME,ADDRESS,TEL,FAX,EMAIL,ID_NHOMNCC,ISACTIVE,ISDEFAULT,CONGNODAUKY,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO,MASOTHUE,TENNGANHANG,CHUTAIKHOAN,SOTAIKHOAN")] v_v_dm_NhaCungCap dm_NhaCungCap)
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                if (Utility.KiemTra())
                {
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Admin");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                if (!Utility.KiemTraQuyen(API.dm_NhaCungCap, API.Edit))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                if (ModelState.IsValid)
                {
                    dm_NhaCungCap.LOC_ID = Utility.LOC_ID;
                    dm_NhaCungCap.ID_NGUOISUA = Session[Sessions.idUser].ToString();
                    dm_NhaCungCap.THOIGIANSUA = Utility.CurrentTime;

                    apiResponse = Utility.Edit<v_dm_NhaCungCap>(Utility.LOC_ID + "/" + dm_NhaCungCap.MA, dm_NhaCungCap, API.dm_NhaCungCap);
                    if (apiResponse.Success)
                    {
                        //return RedirectToAction("Index");
                        apiResponse.ID = dm_NhaCungCap.ID;
                        if (apiResponse.Data != null && !string.IsNullOrEmpty(apiResponse.Data.ToString()))
                            dm_NhaCungCap = JsonConvert.DeserializeObject<v_v_dm_NhaCungCap>(apiResponse.Data.ToString());
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, apiResponse.Message);
                    }
                }
                else
                {
                    apiResponse.Success = false;
                    apiResponse.Data = Utility.GetModelState(ModelState, API.dm_NhaCungCap);
                }
                apiResponse.Detail = Utility.ConvertobjectToView<v_dm_NhaCungCap>(dm_NhaCungCap);
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
                if (!Utility.KiemTraQuyen(API.dm_NhaCungCap, API.Delete))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                apiResponse = Utility.Delete<v_dm_NhaCungCap>(Utility.LOC_ID + "/" + id, API.dm_NhaCungCap);
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