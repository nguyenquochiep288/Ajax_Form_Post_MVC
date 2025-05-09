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
using DatabaseTHP.StoredProcedure;

namespace MVC_QuanLyTHP.Controllers
{
    public class PayrollController : Controller
    {

        // GET: Payment
        public ActionResult Index(int Page = 1, DateTime? FromDate = null, DateTime? ToDate = null, string SearchString = "", string MAPHIEU = "", string IDCODE = "")
        {
            try
            {
                if (Utility.KiemTra())
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (!Utility.KiemTraQuyen(API.nv_BangLuong, API.Xem))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                ViewBag.PermissionCreate = Utility.KiemTraQuyen(API.nv_BangLuong, API.Create);
                ApiResponse apiResponse = new ApiResponse();
                IPagedList<v_nv_BangLuong> lstpage = (new List<v_nv_BangLuong>()).ToList().ToPagedList(Page, Utility.GetPageSize());
                if (FromDate != null || !string.IsNullOrEmpty(IDCODE) || !string.IsNullOrEmpty(MAPHIEU))
                {
                    if (FromDate != null || !string.IsNullOrEmpty(IDCODE) || !string.IsNullOrEmpty(MAPHIEU))
                    {
                        if (!string.IsNullOrEmpty(IDCODE) || !string.IsNullOrEmpty(MAPHIEU))
                        {
                            apiResponse = Utility.Get_DanhSachPhieuLuong<v_nv_BangLuong>(null, null, MAPHIEU, IDCODE);
                        }
                        if (FromDate != null)
                        {
                            apiResponse = Utility.Get_DanhSachPhieuLuong<v_nv_BangLuong>(FromDate, ToDate, SearchString);
                        }
                        if (!apiResponse.Success)
                        {
                            apiResponse.Data = new List<v_dm_HangHoa>();
                            TempData["TitleError"] = apiResponse.Message;
                            apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                            return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                        }

                        if (!apiResponse.Success)
                        {
                            TempData["TitleError"] = apiResponse.Message;
                            return RedirectToAction("Index", "Notfound");
                        }
                        List<v_nv_BangLuong> lst = new List<v_nv_BangLuong>();
                        if (!ViewBag.PermissionCreate)
                            lst = (apiResponse.Data as List<v_nv_BangLuong>).Where(s => s.ID_NHANVIEN == Session[Sessions.idUser].ToString()).ToList();
                        else
                            lst = (apiResponse.Data as List<v_nv_BangLuong>).ToList();

                        lstpage = lst.ToPagedList(Page, Utility.GetPageSize());
                    }
                }
                v_v_nv_BangLuong nv_BangLuong = new v_v_nv_BangLuong();
                nv_BangLuong.IPagedList = lstpage;
                nv_BangLuong.lstdm_NhanVien = new List<ComboboxFrom>();
                ViewBag.searchValue = SearchString;

                ViewBag.PermissionEdit = Utility.KiemTraQuyen(API.nv_BangLuong, API.Edit);
                ViewBag.PermissionDelete = Utility.KiemTraQuyen(API.nv_BangLuong, API.Delete);
                

                ViewBag.fromdate = FromDate != null ? FromDate.Value.ToString("yyyy-MM-dd") : Utility.CurrentTime.AddMonths(-1).ToString("yyyy-MM-dd");
                ViewBag.todate = ToDate != null ? ToDate.Value.ToString("yyyy-MM-dd") : Utility.CurrentTime.ToString("yyyy-MM-dd");
                return View(nv_BangLuong);

            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
                TempData["TitleError"] = API.TitleTryCatch;
                TempData["DetailError"] = ex.Message;
                return RedirectToAction("Index", "Notfound");
            }
        }

        // GET: Input/Create
        public ActionResult Create(int type = 2, string myModalAdd = "myModalAdd")
        {
            try
            {
                Session[Sessions.IntWidth] = type;
                if (Utility.KiemTra())
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (!Utility.KiemTraQuyen(API.nv_BangLuong, API.Create))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                v_v_nv_BangLuong nv_BangLuong = new v_v_nv_BangLuong();
                nv_BangLuong.LOC_ID = Utility.LOC_ID;
                nv_BangLuong.ID_NGUOITAO = Session[Sessions.idUser].ToString();
                nv_BangLuong.THOIGIANTHEM = Utility.CurrentTime;
                nv_BangLuong.NGAYLAP = Utility.CurrentTime;
                nv_BangLuong.SOPHIEU = Utility.GetMaxID<nv_BangLuong>(nv_BangLuong, Utility.LOC_ID, nv_BangLuong.NGAYLAP.ToString("yyyy-MM-dd"));
                nv_BangLuong.MAPHIEU = API.GetMaPhieu(API.nv_BangLuong, nv_BangLuong.NGAYLAP, nv_BangLuong.SOPHIEU);

                nv_BangLuong.ID = Guid.NewGuid().ToString();
                nv_BangLuong.lstdm_ThangLuong = new List<ComboboxFrom>();
                nv_BangLuong.lstdm_NhanVien = new List<ComboboxFrom>();
                ViewBag.myModalAdd = myModalAdd;
                return View(nv_BangLuong);
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
                TempData["TitleError"] = API.TitleTryCatch;
                TempData["DetailError"] = ex.Message;
                return RedirectToAction("Index", "Notfound");
            }
        }

        // POST: Input/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "LOC_ID,ID,ID_THANGLUONG,ID_NHANVIEN,SONGAYCONG,MUCLUONG,SONGAYLAMVIEC,SONGAYNGHIPHEP,TIENLUONG,TIENLUONGKHAC,TIENGIAM,TIENTHUCNHAN,GHICHU,NGAYLAP,ISTINHLUONG,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO,MAPHIEU,SOPHIEU,SONGAYNGHIKHONGPHEP")] v_nv_BangLuong nv_BangLuong)
        {
            try
            {
                if (Utility.KiemTra())
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (!Utility.KiemTraQuyen(API.nv_BangLuong, API.Create))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                if (ModelState.IsValid)
                {
                    nv_BangLuong.LOC_ID = Utility.LOC_ID;
                    nv_BangLuong.ID_NGUOITAO = Session[Sessions.idUser].ToString();
                    nv_BangLuong.THOIGIANTHEM = Utility.CurrentTime;

                    //@ConvertObjectUnicodeToTCVN3
                    var apiResponse = Utility.Create<nv_BangLuong>(nv_BangLuong, API.nv_BangLuong);
                    if (apiResponse.Success)
                        return RedirectToAction("Index");
                    else
                    {
                        ModelState.AddModelError(string.Empty, apiResponse.Message);
                        if (apiResponse.CheckValue)
                            ViewBag.ID = Guid.NewGuid().ToString();
                    }
                }
                return View(nv_BangLuong);
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
                TempData["TitleError"] = API.TitleTryCatch;
                TempData["DetailError"] = ex.Message;
                return RedirectToAction("Index", "Notfound");
            }
        }

        // GET: Input/Edit/5
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
                if (!Utility.KiemTraQuyen(API.nv_BangLuong, API.Edit))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                v_v_nv_BangLuong nv_BangLuong = new v_v_nv_BangLuong();
                if (!string.IsNullOrEmpty(id))
                {
                    apiResponse = Utility.GetDetail<v_nv_BangLuong>(Utility.LOC_ID + "/" + id, API.nv_BangLuong);
                    if (!apiResponse.Success)
                    {
                        TempData["TitleError"] = apiResponse.Message;
                        return RedirectToAction("Index", "Notfound");
                    }
                    if (apiResponse.Data != null)
                        nv_BangLuong = apiResponse.Data as v_v_nv_BangLuong;
                }
                //@ConvertObjectTCVN3ToUnicode
                nv_BangLuong.lstdm_NhanVien = new List<ComboboxFrom>();
                //nv_BangLuong.lstdm_KhachHang = Utility.GetListData<v_dm_KhachHang>(API.dm_KhachHang, "", "", Utility.LOC_ID).Data as List<v_dm_KhachHang>;
                nv_BangLuong.lstdm_ThangLuong = new List<ComboboxFrom>();
                return View(nv_BangLuong);
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
                TempData["TitleError"] = API.TitleTryCatch;
                TempData["DetailError"] = ex.Message;
                return RedirectToAction("Index", "Notfound");
            }
        }

        // POST: Input/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "LOC_ID,ID,ID_THANGLUONG,ID_NHANVIEN,SONGAYCONG,MUCLUONG,SONGAYLAMVIEC,SONGAYNGHIPHEP,TIENLUONG,TIENLUONGKHAC,TIENGIAM,TIENTHUCNHAN,GHICHU,NGAYLAP,ISTINHLUONG,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO,MAPHIEU,SOPHIEU,SONGAYNGHIKHONGPHEP")] v_nv_BangLuong nv_BangLuong)
        {
            try
            {
                if (Utility.KiemTra())
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (!Utility.KiemTraQuyen(API.nv_BangLuong, API.Edit))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                if (ModelState.IsValid)
                {
                    nv_BangLuong.LOC_ID = Utility.LOC_ID;
                    nv_BangLuong.ID_NGUOISUA = Session[Sessions.idUser].ToString();
                    nv_BangLuong.THOIGIANSUA = Utility.CurrentTime;
                    //@ConvertObjectUnicodeToTCVN3
                    var apiResponse = Utility.Edit<v_nv_BangLuong>(Utility.LOC_ID + "/" + nv_BangLuong.ID, nv_BangLuong, API.nv_BangLuong);
                    if (apiResponse.Success)
                        return RedirectToAction("Index");
                    else
                        ModelState.AddModelError(string.Empty, apiResponse.Message);
                }
                return View(nv_BangLuong);
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
                TempData["TitleError"] = API.TitleTryCatch;
                TempData["DetailError"] = ex.Message;
                return RedirectToAction("Index", "Notfound");
            }
        }

        // POST: Input/Delete/5
        [HttpPost]
        public ActionResult Delete(string id)
        {
            try
            {
                if (Utility.KiemTra())
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (!Utility.KiemTraQuyen(API.nv_BangLuong, API.Delete))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                var apiResponse = Utility.Delete<v_nv_BangLuong>(Utility.LOC_ID + "/" + id, API.nv_BangLuong);
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
                if (!Utility.KiemTraQuyen(API.nv_BangLuong, API.Create))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                v_v_nv_BangLuong nv_BangLuong = new v_v_nv_BangLuong();
                apiResponse.Success = true;
                nv_BangLuong.LOC_ID = Utility.LOC_ID;
                nv_BangLuong.ID = Guid.NewGuid().ToString();
                nv_BangLuong.NGAYLAP = Utility.CurrentTime;
                nv_BangLuong.SOPHIEU = Utility.GetMaxID<nv_BangLuong>(nv_BangLuong, Utility.LOC_ID, nv_BangLuong.NGAYLAP.ToString("yyyy-MM-dd"));
                nv_BangLuong.MAPHIEU = API.GetMaPhieu(API.nv_BangLuong, nv_BangLuong.NGAYLAP, nv_BangLuong.SOPHIEU);
                
                nv_BangLuong.lstdm_NhanVien = new List<ComboboxFrom>();
                nv_BangLuong.lstdm_NhanVien = Utility.GetListData<ComboboxFrom>(API.dm_NhanVien, "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
                nv_BangLuong.lstdm_ThangLuong = new List<ComboboxFrom>();
                nv_BangLuong.lstdm_ThangLuong = Utility.GetListData<ComboboxFrom>(API.dm_ThangLuong, "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
                var lst = Utility.ConvertobjectTo<v_v_nv_BangLuong>(nv_BangLuong);
                Session[Sessions.lstnv_BangLuong_ChiTiet] = new List<nv_BangLuong_ChiTiet>();
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
        private ApiResponse GetValue<T>(ApiResponse apiResponse, string NameController, string KeyCode)
        {
            apiResponse = Utility.GetListDataCode<T>(NameController, "MAPHIEU.ToUpper() == @0", KeyCode.ToUpper(), Utility.LOC_ID);
            if (!apiResponse.Success)
            {
                TempData["TitleError"] = apiResponse.Message;
                apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                return apiResponse;
            }
            if (apiResponse.Data != null)
            {
                List<T> lst = apiResponse.Data as List<T>;
                if (lst != null && lst.Count > 0)
                    apiResponse.Detail = lst.FirstOrDefault();
            }

            return apiResponse;
        }
        
        // POST: Menu/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult CreatePopup([Bind(Include = "LOC_ID,ID,ID_THANGLUONG,ID_NHANVIEN,SONGAYCONG,MUCLUONG,SONGAYLAMVIEC,SONGAYNGHIPHEP,TIENLUONG,TIENLUONGKHAC,TIENGIAM,TIENTHUCNHAN,GHICHU,NGAYLAP,ISTINHLUONG,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO,MAPHIEU,SOPHIEU,SONGAYNGHIKHONGPHEP,BUTTONTYPE,TIENDAUKY")] v_v_nv_BangLuong nv_BangLuong)
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                if (Utility.KiemTra())
                {
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Admin");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                if (!Utility.KiemTraQuyen(API.nv_BangLuong, API.Create))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                nv_BangLuong.lstnv_BangLuong_ChiTiet = new List<nv_BangLuong_ChiTiet>();
                List<nv_BangLuong_ChiTiet> lstOrderProduct = new List<nv_BangLuong_ChiTiet>();
                var lstKey = Request.Form.AllKeys.Where(e => e.StartsWith("txt"));
                if (lstKey == null || lstKey.Count() == 0)
                {
                    ModelState.AddModelError("lstnv_BangLuong_ChiTiet", "Thêm danh sách hàng hóa.");
                }
                else
                {
                    v_nv_BangLuong_ChiTiet nv_BangLuong_ChiTiet = new v_nv_BangLuong_ChiTiet();
                    foreach (string Key in lstKey)
                    {
                        var lstString = Key.ToString().Split('|');
                        var value = HttpContext.Request.Params.GetValues(Key.ToString());
                        string ShowSearchValue = clsMaHoa.Decrypt(lstString[1].ToString(), clsMaHoa.PassMaHoa);
                        var Checkct_PhieuDatHang_ChiTiet = JsonConvert.DeserializeObject<nv_BangLuong_ChiTiet>(ShowSearchValue);

                        if (nv_BangLuong_ChiTiet.ID != Checkct_PhieuDatHang_ChiTiet.ID)
                        {
                            nv_BangLuong_ChiTiet = new v_nv_BangLuong_ChiTiet();
                            nv_BangLuong_ChiTiet = JsonConvert.DeserializeObject<v_nv_BangLuong_ChiTiet>(ShowSearchValue);
                            nv_BangLuong_ChiTiet.LOC_ID = Utility.LOC_ID;
                            nv_BangLuong.lstnv_BangLuong_ChiTiet.Add(nv_BangLuong_ChiTiet);
                            lstOrderProduct.Add(Checkct_PhieuDatHang_ChiTiet);
                        }
                        Utility.EditObject(nv_BangLuong_ChiTiet, lstString[0].ToString().Substring(3, lstString[0].ToString().Length - 3), value[0]);
                    }
                }
                if (ModelState.IsValid)
                {
                    nv_BangLuong.NGAYLAP = nv_BangLuong.NGAYLAP.AddHours(Utility.CurrentTime.Hour).AddMinutes(Utility.CurrentTime.Minute);
                    nv_BangLuong.LOC_ID = Utility.LOC_ID;
                    nv_BangLuong.ID_NGUOITAO = Session[Sessions.idUser].ToString();
                    nv_BangLuong.THOIGIANTHEM = Utility.CurrentTime;
                    

                    apiResponse = Utility.Create<v_nv_BangLuong>(nv_BangLuong, API.nv_BangLuong);
                    if (apiResponse.Success)
                    {
                        nv_BangLuong.NGAYLAP = Utility.CurrentTime;
                        apiResponse.SOPHIEU = nv_BangLuong.SOPHIEU = Utility.GetMaxID<nv_BangLuong>(nv_BangLuong, Utility.LOC_ID, nv_BangLuong.NGAYLAP.ToString("yyyy-MM-dd"));
                        nv_BangLuong.MAPHIEU = API.GetMaPhieu(API.nv_BangLuong, nv_BangLuong.NGAYLAP, nv_BangLuong.SOPHIEU);
                        apiResponse.NewID = Guid.NewGuid().ToString();
                        apiResponse.MAPHIEU = nv_BangLuong.MAPHIEU;

                        if (apiResponse.Data != null)
                            nv_BangLuong = JsonConvert.DeserializeObject<v_v_nv_BangLuong>(apiResponse.Data.ToString());
                        lstOrderProduct = new List<nv_BangLuong_ChiTiet>();
                        
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, apiResponse.Message);
                        if (apiResponse.CheckValue)
                        {
                            nv_BangLuong.NGAYLAP = Utility.CurrentTime;
                            apiResponse.SOPHIEU = nv_BangLuong.SOPHIEU = Utility.GetMaxID<nv_BangLuong>(nv_BangLuong, Utility.LOC_ID, nv_BangLuong.NGAYLAP.ToString("yyyy-MM-dd"));
                            nv_BangLuong.MAPHIEU = API.GetMaPhieu(API.nv_BangLuong, nv_BangLuong.NGAYLAP, nv_BangLuong.SOPHIEU);
                            apiResponse.NewID = Guid.NewGuid().ToString();
                            apiResponse.MAPHIEU = nv_BangLuong.MAPHIEU;
                        }
                    }
                }
                else
                {
                    apiResponse.Success = false;
                    apiResponse.Data = Utility.GetModelState(ModelState, API.nv_BangLuong);
                }
                Session[Sessions.lstnv_BangLuong_ChiTiet] = lstOrderProduct;
                apiResponse.ID = nv_BangLuong.ID;
                nv_BangLuong.lstdm_NhanVien = new List<ComboboxFrom>();
                nv_BangLuong.lstdm_NhanVien = Utility.GetListData<ComboboxFrom>(API.dm_NhanVien, "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
                nv_BangLuong.lstdm_ThangLuong = new List<ComboboxFrom>();
                nv_BangLuong.lstdm_ThangLuong = Utility.GetListData<ComboboxFrom>(API.dm_ThangLuong, "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
                var lst = Utility.ConvertobjectToView<v_v_nv_BangLuong>(nv_BangLuong);
                var lstdm_LoaiLuong = Utility.GetListData<v_dm_LoaiLuong>(API.dm_LoaiLuong, "", "", Utility.LOC_ID).Data as List<v_dm_LoaiLuong>;
                apiResponse.ProductCombo = Utility.GetPayrollDetail(lstOrderProduct, lstdm_LoaiLuong);
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
                if (!Utility.KiemTraQuyen(API.nv_BangLuong, API.Edit))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }

                v_v_nv_BangLuong nv_BangLuong = new v_v_nv_BangLuong();
                if (!string.IsNullOrEmpty(id))
                {
                    apiResponse = Utility.GetDetail<v_v_nv_BangLuong>(Utility.LOC_ID + "/" + id, API.nv_BangLuong);
                    if (!apiResponse.Success)
                    {
                        TempData["TitleError"] = apiResponse.Message;
                        apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                        return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                    }
                    if (apiResponse.Data != null)
                        nv_BangLuong = apiResponse.Data as v_v_nv_BangLuong;
                }
                nv_BangLuong.lstdm_NhanVien = new List<ComboboxFrom>();
                nv_BangLuong.lstdm_NhanVien = Utility.GetListData<ComboboxFrom>(API.dm_NhanVien, "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
                nv_BangLuong.lstdm_ThangLuong = new List<ComboboxFrom>();
                nv_BangLuong.lstdm_ThangLuong = Utility.GetListData<ComboboxFrom>(API.dm_ThangLuong, "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
                var lst = Utility.ConvertobjectTo<v_v_nv_BangLuong>(nv_BangLuong);
                var lstdm_LoaiLuong = Utility.GetListData<v_dm_LoaiLuong>(API.dm_LoaiLuong, "", "", Utility.LOC_ID).Data as List<v_dm_LoaiLuong>;
                apiResponse.ProductCombo = Utility.GetPayrollDetail(nv_BangLuong.lstnv_BangLuong_ChiTiet, lstdm_LoaiLuong);
                lst.Add(new ValueEdit { Key = "tbodyReport_Edit", Value = apiResponse.ProductCombo });
                Session[Sessions.lstnv_BangLuong_ChiTiet] = nv_BangLuong.lstnv_BangLuong_ChiTiet;
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
        public ActionResult EditPopup([Bind(Include = "LOC_ID,ID,ID_THANGLUONG,ID_NHANVIEN,SONGAYCONG,MUCLUONG,SONGAYLAMVIEC,SONGAYNGHIPHEP,TIENLUONG,TIENLUONGKHAC,TIENGIAM,TIENTHUCNHAN,GHICHU,NGAYLAP,ISTINHLUONG,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO,MAPHIEU,SOPHIEU,SONGAYNGHIKHONGPHEP,BUTTONTYPE,TIENDAUKY")] v_v_nv_BangLuong nv_BangLuong)
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                if (Utility.KiemTra())
                {
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Admin");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                if (!Utility.KiemTraQuyen(API.nv_BangLuong, API.Edit))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                nv_BangLuong.lstnv_BangLuong_ChiTiet = new List<nv_BangLuong_ChiTiet>();
                List<nv_BangLuong_ChiTiet> lstOrderProduct = new List<nv_BangLuong_ChiTiet>();
                var lstKey = Request.Form.AllKeys.Where(e => e.StartsWith("txt"));
                if (lstKey == null || lstKey.Count() == 0)
                {
                    ModelState.AddModelError("lstnv_BangLuong_ChiTiet", "Thêm danh sách hàng hóa.");
                }
                else
                {
                    v_nv_BangLuong_ChiTiet nv_BangLuong_ChiTiet = new v_nv_BangLuong_ChiTiet();
                    foreach (string Key in lstKey)
                    {
                        var lstString = Key.ToString().Split('|');
                        var value = HttpContext.Request.Params.GetValues(Key.ToString());
                        string ShowSearchValue = clsMaHoa.Decrypt(lstString[1].ToString(), clsMaHoa.PassMaHoa);
                        var Checkct_PhieuDatHang_ChiTiet = JsonConvert.DeserializeObject<nv_BangLuong_ChiTiet>(ShowSearchValue);

                        if (nv_BangLuong_ChiTiet.ID != Checkct_PhieuDatHang_ChiTiet.ID)
                        {
                            nv_BangLuong_ChiTiet = new v_nv_BangLuong_ChiTiet();
                            nv_BangLuong_ChiTiet = JsonConvert.DeserializeObject<v_nv_BangLuong_ChiTiet>(ShowSearchValue);
                            nv_BangLuong_ChiTiet.LOC_ID = Utility.LOC_ID;
                            nv_BangLuong.lstnv_BangLuong_ChiTiet.Add(nv_BangLuong_ChiTiet);
                            lstOrderProduct.Add(Checkct_PhieuDatHang_ChiTiet);
                        }
                        Utility.EditObject(nv_BangLuong_ChiTiet, lstString[0].ToString().Substring(3, lstString[0].ToString().Length - 3), value[0]);
                    }
                }
                if (ModelState.IsValid)
                {
                    nv_BangLuong.LOC_ID = Utility.LOC_ID;
                    nv_BangLuong.ID_NGUOISUA = Session[Sessions.idUser].ToString();
                    nv_BangLuong.THOIGIANSUA = Utility.CurrentTime;
                    apiResponse = Utility.Edit<v_nv_BangLuong>(Utility.LOC_ID + "/" + nv_BangLuong.ID, nv_BangLuong, API.nv_BangLuong);
                    if (apiResponse.Success)
                    {
                        //return RedirectToAction("Index");
                        apiResponse.ID = nv_BangLuong.ID;
                        if (apiResponse.Data != null)
                            nv_BangLuong = JsonConvert.DeserializeObject<v_v_nv_BangLuong>(apiResponse.Data.ToString());

                        lstOrderProduct = new List<nv_BangLuong_ChiTiet>();
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, apiResponse.Message);
                    }
                }
                else
                {
                    apiResponse.Success = false;
                    apiResponse.Data = Utility.GetModelState(ModelState, API.nv_BangLuong);
                }
                Session[Sessions.lstnv_BangLuong_ChiTiet] = lstOrderProduct;
                nv_BangLuong.lstdm_NhanVien = new List<ComboboxFrom>();
                nv_BangLuong.lstdm_NhanVien = Utility.GetListData<ComboboxFrom>(API.dm_NhanVien, "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
                nv_BangLuong.lstdm_ThangLuong = new List<ComboboxFrom>();
                nv_BangLuong.lstdm_ThangLuong = Utility.GetListData<ComboboxFrom>(API.dm_ThangLuong, "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
                //apiResponse.Detail = Utility.ConvertobjectToView<v_v_nv_BangLuong>(nv_BangLuong);
                var lst = Utility.ConvertobjectToView<v_v_nv_BangLuong>(nv_BangLuong);
                
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
                if (!Utility.KiemTraQuyen(API.nv_BangLuong, API.Delete))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                apiResponse = Utility.Delete<v_nv_BangLuong>(Utility.LOC_ID + "/" + id, API.nv_BangLuong);
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

        //public ActionResult ViewReport(string ID)
        //{
        //    ApiResponse apiResponse = new ApiResponse();
        //    try
        //    {

        //        if (Utility.KiemTra())
        //        {
        //            apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Admin");
        //            return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
        //        }

        //        v_nv_BangLuong PhieuNhap = new v_nv_BangLuong();
        //        SP_Parameter objParameter = new SP_Parameter();
        //        objParameter.ID_PHIEUCHI = ID;
        //        apiResponse = Utility.ExecuteStoredProc<v_nv_BangLuong>(objParameter, API.Sp_Get_DanhSachPhieuChi);
        //        if (!apiResponse.Success)
        //        {
        //            apiResponse.Success = false;
        //            apiResponse.Message = apiResponse.Message;
        //            return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
        //        }
        //        if (apiResponse.Data != null)
        //            PhieuNhap = (apiResponse.Data as List<v_nv_BangLuong>).FirstOrDefault();

        //        var report = new ReportClass();
        //        report.FileName = Server.MapPath("~/Report/rptPhieuChi.rpt");

        //        SP_Parameter_Report objParameter_Report = new SP_Parameter_Report();
        //        objParameter_Report.LOC_ID = Utility.LOC_ID;
        //        objParameter_Report.ID_PHIEUCHI = ID;
        //        apiResponse = Utility.ExecuteStoredProc<DataTable>(objParameter_Report, API.Sp_Get_DanhSachPhieuChi);
        //        if (!apiResponse.Success)
        //        {
        //            apiResponse.Success = false;
        //            apiResponse.Message = apiResponse.Message;
        //            return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
        //        }

        //        //QRCodeLogo qrCodeLogo = new QRCodeLogo(fullpathLogo);
        //        //GeneratedBarcode MyVerifiedQR = QRCodeWriter.CreateQrCodeWithLogo(BinaryData, qrCodeLogo, 500);
        //        //MyVerifiedQR.ResizeTo(500, 500).SetMargins(10).ChangeBarCodeColor(Color.DarkGreen);
        //        //MyVerifiedQR.SaveAsImage(fullpath);

        //        DataTable data = (apiResponse.Data as DataTable);
        //        if (apiResponse.CheckValue)
        //            data.Rows.Clear();

        //        report = Utility.GetFormulaFields(report, PhieuNhap);
        //        report.SetDataSource(data);
        //        Response.Buffer = false;
        //        Response.ClearContent();
        //        Response.ClearHeaders();
        //        Stream stream = report.ExportToStream(ExportFormatType.PortableDocFormat);
        //        Utility.Report = report;
        //        apiResponse = new ApiResponse();
        //        apiResponse.Success = true;
        //        apiResponse.NAME = Utility.GetTitleFrom(API.nv_BangLuong) + " - " + PhieuNhap.MAPHIEU;
        //        return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
        //    }
        //    catch (Exception ex)
        //    {
        //        Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
        //        TempData["TitleError"] = API.TitleTryCatch;
        //        TempData["DetailError"] = ex.Message;
        //        apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
        //        return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
        //    }

        //}

        #region Lấy địa chỉ khách hàng
        [HttpPost]
        public ActionResult CallChangePayroll(string id,string type)
        {
            if (Utility.KiemTra())
            {
                return RedirectToAction("Index", "Admin");
            }

            if (ModelState.IsValid)
            {
                Return newReturn = new Return();
                ApiResponse apiResponse = new ApiResponse();
                if(type == "dm_ThangLuong")
                {
                    apiResponse = Utility.GetDetail<v_dm_ThangLuong>(Utility.LOC_ID + "/" + id, API.dm_ThangLuong);
                }
                else if(type == "dm_NhanVien")
                {
                    apiResponse = Utility.GetDetail<v_dm_NhanVien>(Utility.LOC_ID + "/" + id, API.dm_NhanVien);
                }    
                else if (type == "NGAYLAP")
                {
                    apiResponse.Success = true;
                }
                
                if (!apiResponse.Success)
                {
                    TempData["TitleError"] = apiResponse.Message;
                    newReturn.URL = Url.Action("Index", "Notfound");
                }
                else
                {
                    if (type == "dm_ThangLuong")
                    {
                        var KhachHang = (apiResponse.Data as v_dm_ThangLuong);
                        newReturn.DataObject = KhachHang;
                    }
                    else if (type == "dm_NhanVien")
                    {
                        var KhachHang = (apiResponse.Data as v_dm_NhanVien);
                        newReturn.DataObject = KhachHang;
                    }
                    else if (type == "NGAYLAP")
                    {
                        nv_BangLuong nv_BangLuong = new nv_BangLuong();
                        nv_BangLuong.NGAYLAP = Convert.ToDateTime(id);
                        nv_BangLuong.SOPHIEU = Utility.GetMaxID<nv_BangLuong>(nv_BangLuong, Utility.LOC_ID, nv_BangLuong.NGAYLAP.ToString("yyyy-MM-dd"));
                        nv_BangLuong.MAPHIEU = API.GetMaPhieu(API.nv_BangLuong, nv_BangLuong.NGAYLAP, nv_BangLuong.SOPHIEU);
                        newReturn.DataObject = nv_BangLuong;
                    }
                }
                newReturn.DATA = type;
                return Json(newReturn, JsonRequestBehavior.AllowGet);
            }
            else
            {
                Return newReturn = new Return();
                newReturn.DATA = "";

                return Json(newReturn, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult GetPayrollDetail(string ID_THANGLUONG, string ID_NHANVIEN, string ID)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (Utility.KiemTra())
            {
                return RedirectToAction("Index", "Admin");
            }

            if (ModelState.IsValid)
            {
                
                v_nv_BangLuong nv_BangLuong = new v_nv_BangLuong();
                nv_BangLuong.LOC_ID = Utility.LOC_ID;
                nv_BangLuong.ID_NHANVIEN = ID_NHANVIEN;
                nv_BangLuong.ID_THANGLUONG = ID_THANGLUONG;
                nv_BangLuong.ID = ID;
                apiResponse = Utility.Create<v_nv_BangLuong>(nv_BangLuong, API.nv_BangLuong + "/" + Utility.LOC_ID);
                nv_BangLuong = JsonConvert.DeserializeObject<v_v_nv_BangLuong>(apiResponse.Data.ToString());
                if (!apiResponse.Success)
                {
                    
                }
                else
                {
                    v_v_dm_ThangLuong dm_ThangLuong = new v_v_dm_ThangLuong();
                    double SoTienCongNo = 0;
                    apiResponse = Utility.GetDetail<v_v_dm_ThangLuong>(Utility.LOC_ID + "/" + ID_THANGLUONG, API.dm_ThangLuong);
                    if (!apiResponse.Success)
                    {
                        TempData["TitleError"] = apiResponse.Message;
                        return RedirectToAction("Index", "Notfound");
                    }
                    if (apiResponse.Data != null)
                        dm_ThangLuong = apiResponse.Data as v_v_dm_ThangLuong;
                    SP_Parameter sp_Parameter = new SP_Parameter();
                    //ApiResponse apiResponse = new ApiResponse();
                    sp_Parameter.LOC_ID = Utility.LOC_ID;
                    sp_Parameter.ID_NHANVIEN = ID_NHANVIEN;
                    sp_Parameter.ISTHEOTHOIGIAN = true;
                    sp_Parameter.TUNGAY = dm_ThangLuong.NGAYBATDAU;
                    sp_Parameter.DENNGAY = dm_ThangLuong.NGAYKETTHUC;
                    sp_Parameter.ISPHATSINHCONGNO = false;
                    sp_Parameter.ISPHATSINHCONGNOTRONGKY = false;
                    sp_Parameter.ISCONCONGNO = false;
                    apiResponse = Utility.Get_ThongKeCongNoNhanVien<v_ThongKeCongNoNhanVien>(sp_Parameter);
                    if (!apiResponse.Success)
                    {
                        TempData["TitleError"] = apiResponse.Message;
                        return RedirectToAction("Index", "Notfound");
                    }
                    if (apiResponse.Data != null)
                    {
                        var CongNo = (apiResponse.Data as List<v_ThongKeCongNoNhanVien>).FirstOrDefault();
                        if (CongNo != null)
                        {
                            SoTienCongNo = CongNo.TONGTIENCONGNOCUOIKY;
                        }
                    }
                    else
                    {
                    }

                    if (nv_BangLuong.lstnv_BangLuong_ChiTiet != null && nv_BangLuong.lstnv_BangLuong_ChiTiet.Count > 0)
                    {
                        var lstdm_LoaiLuong = Utility.GetListData<v_dm_LoaiLuong>(API.dm_LoaiLuong, "", "", Utility.LOC_ID).Data as List<v_dm_LoaiLuong>;
                        apiResponse.ProductCombo = Utility.GetPayrollDetail(nv_BangLuong.lstnv_BangLuong_ChiTiet, lstdm_LoaiLuong);
                        var lst =new List<ValueEdit>();
                        lst.Add(new ValueEdit { Key = "SONGAYCONG", Value = nv_BangLuong.SONGAYCONG });
                        lst.Add(new ValueEdit { Key = "SONGAYLAMVIEC", Value = nv_BangLuong.SONGAYLAMVIEC });
                        lst.Add(new ValueEdit { Key = "SONGAYNGHIPHEP", Value = nv_BangLuong.SONGAYNGHIPHEP });
                        lst.Add(new ValueEdit { Key = "SONGAYNGHIKHONGPHEP", Value = nv_BangLuong.SONGAYNGHIKHONGPHEP });
                        lst.Add(new ValueEdit { Key = "TIENLUONG", Value = nv_BangLuong.TIENLUONG });
                        lst.Add(new ValueEdit { Key = "TIENGIAM", Value = nv_BangLuong.TIENGIAM });
                        lst.Add(new ValueEdit { Key = "TIENTHUCNHAN", Value = nv_BangLuong.TIENTHUCNHAN });
                        lst.Add(new ValueEdit { Key = "GHICHU", Value = nv_BangLuong.GHICHU });
                        lst.Add(new ValueEdit { Key = "TIENDAUKY", Value = SoTienCongNo });
                        Session[Sessions.lstnv_BangLuong_ChiTiet] = nv_BangLuong.lstnv_BangLuong_ChiTiet;
                        apiResponse.Detail = lst;
                    }
                }
                return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
            }
            else
            {
                return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
            }
        }
        #endregion
      
        [HttpPost]
        public ActionResult AddPayroll()
        {
            ApiResponse apiResponse = new ApiResponse();
            v_nv_BangLuong_ChiTiet newv_dm_BangLuong_ChiTiet = new v_nv_BangLuong_ChiTiet();
            newv_dm_BangLuong_ChiTiet.ID = Guid.NewGuid().ToString();
            Utility.Lstnv_BangLuong_ChiTiet.Add(newv_dm_BangLuong_ChiTiet);
            var lstdm_LoaiLuong = Utility.GetListData<v_dm_LoaiLuong>(API.dm_LoaiLuong, "", "", Utility.LOC_ID).Data as List<v_dm_LoaiLuong>;
            apiResponse.ProductCombo = Utility.GetPayrollDetail(Utility.Lstnv_BangLuong_ChiTiet, lstdm_LoaiLuong);
            apiResponse.Success = true;
            return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
        }

        [HttpPost]
        public ActionResult RemovePayroll(string ID)
        {
            ApiResponse apiResponse = new ApiResponse();
            v_v_dm_HangHoa dm_HangHoa = new v_v_dm_HangHoa();
            var LstKPISale_YeuCau = Utility.Lstnv_BangLuong_ChiTiet;
            var check = Utility.Lstnv_BangLuong_ChiTiet.Where(e => e.ID == ID).FirstOrDefault();
            if (check != null)
                LstKPISale_YeuCau.Remove(check);

            Session[Sessions.lstnv_BangLuong_ChiTiet] = LstKPISale_YeuCau;
            var lstdm_LoaiLuong = Utility.GetListData<v_dm_LoaiLuong>(API.dm_LoaiLuong, "", "", Utility.LOC_ID).Data as List<v_dm_LoaiLuong>;
            apiResponse.ProductCombo = Utility.GetPayrollDetail(Utility.Lstnv_BangLuong_ChiTiet, lstdm_LoaiLuong);
            apiResponse.Success = true;
            return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
        }

        public ActionResult ViewReport(string ID)
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {

                if (Utility.KiemTra())
                {
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Admin");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                view_nv_BangLuong_ChiTiet view_nv_BangLuong_ChiTiet = new view_nv_BangLuong_ChiTiet();
                apiResponse = Utility.Create<view_nv_BangLuong>(null, API.nv_BangLuong + "/" + Utility.LOC_ID + "/" + ID);
                if (!apiResponse.Success)
                {
                    apiResponse.Success = false;
                    apiResponse.Message = apiResponse.Message;
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }

                if (apiResponse.Data != null)
                    view_nv_BangLuong_ChiTiet = JsonConvert.DeserializeObject<List<view_nv_BangLuong_ChiTiet>>(apiResponse.Data.ToString()).FirstOrDefault(); 

                var report = new ReportClass();
                report.FileName = Server.MapPath("~/Report/rptPhieuLuong.rpt");

                //QRCodeLogo qrCodeLogo = new QRCodeLogo(fullpathLogo);
                //GeneratedBarcode MyVerifiedQR = QRCodeWriter.CreateQrCodeWithLogo(BinaryData, qrCodeLogo, 500);
                //MyVerifiedQR.ResizeTo(500, 500).SetMargins(10).ChangeBarCodeColor(Color.DarkGreen);
                //MyVerifiedQR.SaveAsImage(fullpath);
                List<view_nv_BangLuong_ChiTiet> lstTam = new List<view_nv_BangLuong_ChiTiet>();
                lstTam = JsonConvert.DeserializeObject<List<view_nv_BangLuong_ChiTiet>>(apiResponse.Data.ToString()).OrderBy(s => s.TYPE).ToList();
                if(lstTam == null) lstTam = new List<view_nv_BangLuong_ChiTiet>();
                List<view_nv_BangLuong_ChiTiet> lstview_nv_BangLuong_ChiTiet = (from itm in lstTam
                                                   orderby itm.TYPE, itm.SOTIEN descending
                                                   select itm).ToList();
                DataTable data = Utility.ToDataTable<view_nv_BangLuong_ChiTiet>(lstview_nv_BangLuong_ChiTiet);
                if (apiResponse.CheckValue)
                    data.Rows.Clear();

                report = Utility.GetFormulaFields(report, view_nv_BangLuong_ChiTiet);
                report.SetDataSource(data);
                Response.Buffer = false;
                Response.ClearContent();
                Response.ClearHeaders();
                Stream stream = report.ExportToStream(ExportFormatType.PortableDocFormat);
                Utility.Report = report;
                apiResponse = new ApiResponse();
                apiResponse.Success = true;
                apiResponse.NAME = Utility.GetTitleFrom(API.nv_BangLuong) + " - " + view_nv_BangLuong_ChiTiet.MAPHIEU;
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
    }
}