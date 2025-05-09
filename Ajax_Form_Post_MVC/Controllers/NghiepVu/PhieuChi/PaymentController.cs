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

namespace MVC_QuanLyTHP.Controllers
{
    public class PaymentController : Controller
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
                if (!Utility.KiemTraQuyen(API.ct_PhieuChi, API.Xem))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                ApiResponse apiResponse = new ApiResponse();
                string TotalSum = "";
                IPagedList<v_ct_PhieuChi> lstpage = (new List<v_ct_PhieuChi>()).ToList().ToPagedList(Page, Utility.GetPageSize());
                if (FromDate != null || !string.IsNullOrEmpty(IDCODE) || !string.IsNullOrEmpty(MAPHIEU))
                {
                    if (FromDate != null || !string.IsNullOrEmpty(IDCODE) || !string.IsNullOrEmpty(MAPHIEU))
                    {
                        if (!string.IsNullOrEmpty(IDCODE) || !string.IsNullOrEmpty(MAPHIEU))
                        {
                            apiResponse = Utility.Get_DanhSachPhieuChi<v_ct_PhieuChi>("", null, null, MAPHIEU, IDCODE);
                        }
                        if (FromDate != null)
                        {
                            apiResponse = Utility.Get_DanhSachPhieuChi<v_ct_PhieuChi>("", FromDate, ToDate, SearchString);
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
                        TotalSum = (apiResponse.Data as List<v_ct_PhieuChi>).Sum(s => s.SOTIEN).ToString("N0");
                        lstpage = (apiResponse.Data as List<v_ct_PhieuChi>).ToPagedList(Page, Utility.GetPageSize());
                    }
                }
                v_v_ct_PhieuChi ct_PhieuChi = new v_v_ct_PhieuChi();
                ct_PhieuChi.IPagedList = lstpage;
                ct_PhieuChi.lstdm_NhaCungCap = new List<ComboboxFrom>();
                //ct_PhieuChi.lstdm_NhaCungCap = Utility.GetListData<v_dm_NhaCungCap>(API.dm_NhaCungCap, "", "", Utility.LOC_ID).Data as List<v_dm_NhaCungCap>;
                ct_PhieuChi.lstdm_KhachHang = new List<ComboboxFrom>();
                ct_PhieuChi.lstdm_LoaiPhieuChi = new List<v_dm_LoaiPhieuChi>();
                ct_PhieuChi.lstdm_TaiKhoanNganHang = new List<v_dm_TaiKhoanNganHang>();
                var lstdm_LoaiPhieuChi = Utility.GetListData<v_dm_LoaiPhieuChi>(API.dm_LoaiPhieuChi, "", "", Utility.LOC_ID).Data as List<v_dm_LoaiPhieuChi>;
                if (lstdm_LoaiPhieuChi != null)
                {
                    ct_PhieuChi.lstdm_LoaiPhieuChi = lstdm_LoaiPhieuChi.Where(e => e.ISACTIVE == true).OrderBy(e => e.TYPE).ToList();
                }
                else
                {
                    ct_PhieuChi.lstdm_LoaiPhieuChi = new List<v_dm_LoaiPhieuChi>();
                }

                ct_PhieuChi.lstdm_NhanVien = new List<ComboboxFrom>();
                ct_PhieuChi.lstdm_Xe = new List<ComboboxFrom>();
                //ct_PhieuChi.lstdm_NhanVien = Utility.GetListData<v_dm_NhanVien>(API.dm_NhanVien, "", "", Utility.LOC_ID).Data as List<v_dm_NhanVien>;


                ViewBag.searchValue = SearchString;
                ViewBag.TotalSum = TotalSum;
                ViewBag.PermissionEdit = Utility.KiemTraQuyen(API.ct_PhieuChi, API.Edit);
                ViewBag.PermissionDelete = Utility.KiemTraQuyen(API.ct_PhieuChi, API.Delete);
                ViewBag.PermissionCreate = Utility.KiemTraQuyen(API.ct_PhieuChi, API.Create);

                ViewBag.fromdate = FromDate != null ? FromDate.Value.ToString("yyyy-MM-dd") : Utility.CurrentTime.AddMonths(-1).ToString("yyyy-MM-dd");
                ViewBag.todate = ToDate != null ? ToDate.Value.ToString("yyyy-MM-dd") : Utility.CurrentTime.ToString("yyyy-MM-dd");
                return View(ct_PhieuChi);

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
                if (!Utility.KiemTraQuyen(API.ct_PhieuChi, API.Create))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                v_v_ct_PhieuChi ct_PhieuChi = new v_v_ct_PhieuChi();
                ct_PhieuChi.LOC_ID = Utility.LOC_ID;
                ct_PhieuChi.ID_NGUOITAO = Session[Sessions.idUser].ToString();
                ct_PhieuChi.THOIGIANTHEM = Utility.CurrentTime;
                ct_PhieuChi.NGAYLAP = Utility.CurrentTime;
                ct_PhieuChi.SOPHIEU = Utility.GetMaxID<ct_PhieuChi>(ct_PhieuChi, Utility.LOC_ID, ct_PhieuChi.NGAYLAP.ToString("yyyy-MM-dd"));
                ct_PhieuChi.MAPHIEU = API.GetMaPhieu(API.ct_PhieuChi, ct_PhieuChi.NGAYLAP, ct_PhieuChi.SOPHIEU);

                ct_PhieuChi.ID = Guid.NewGuid().ToString();
                ct_PhieuChi.lstdm_NhaCungCap = new List<ComboboxFrom>();
                //ct_PhieuChi.lstdm_NhaCungCap = Utility.GetListData<v_dm_NhaCungCap>(API.dm_NhaCungCap, "", "", Utility.LOC_ID).Data as List<v_dm_NhaCungCap>;
                ct_PhieuChi.lstdm_KhachHang = new List<ComboboxFrom>();
                //ct_PhieuChi.lstdm_Kho = Utility.GetListData<v_dm_Kho>(API.dm_Kho, "", "", Utility.LOC_ID).Data as List<v_dm_Kho>;
                ct_PhieuChi.lstdm_LoaiPhieuChi = new List<v_dm_LoaiPhieuChi>();
                //ct_PhieuChi.lstdm_LoaiPhieuChi = Utility.GetListData<v_dm_LoaiPhieuChi>(API.dm_LoaiPhieuChi, "", "", Utility.LOC_ID).Data as List<v_dm_LoaiPhieuChi>;
                ct_PhieuChi.lstdm_NhanVien = new List<ComboboxFrom>();
                ct_PhieuChi.lstdm_Xe = new List<ComboboxFrom>();
                //ct_PhieuChi.lstdm_NhanVien = Utility.GetListData<v_dm_NhanVien>(API.dm_NhanVien, "", "", Utility.LOC_ID).Data as List<v_dm_NhanVien>;
                ct_PhieuChi.lstdm_TaiKhoanNganHang = new List<v_dm_TaiKhoanNganHang>();
                ViewBag.myModalAdd = myModalAdd;
                ct_PhieuChi.myModalAdd = myModalAdd;
                return View(ct_PhieuChi);
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
        public ActionResult Create([Bind(Include = "LOC_ID,ID,NAME_LOAIPHIEUCHI,ID_LOAIPHIEUCHI,NGAYLAP,MAPHIEU,SOPHIEU,NAME_KHACHHANG_NCC_NHANVIEN,ID_KHACHHANG,ID_NHACUNGCAP,ID_NHANVIEN,TENNGUOINHAN,DIACHI,NGUOICHITIEN,SOTIEN,LYDO,CHUNGTUKEMTHEO,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO,ID_TAIKHOANNGANHANG")] v_ct_PhieuChi ct_PhieuChi)
        {
            try
            {
                if (Utility.KiemTra())
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (!Utility.KiemTraQuyen(API.ct_PhieuChi, API.Create))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                if (ModelState.IsValid)
                {
                    ct_PhieuChi.LOC_ID = Utility.LOC_ID;
                    ct_PhieuChi.ID_NGUOITAO = Session[Sessions.idUser].ToString();
                    ct_PhieuChi.THOIGIANTHEM = Utility.CurrentTime;

                    //@ConvertObjectUnicodeToTCVN3
                    var apiResponse = Utility.Create<ct_PhieuChi>(ct_PhieuChi, API.ct_PhieuChi);
                    if (apiResponse.Success)
                        return RedirectToAction("Index");
                    else
                    {
                        ModelState.AddModelError(string.Empty, apiResponse.Message);
                        if (apiResponse.CheckValue)
                            ViewBag.ID = Guid.NewGuid().ToString();
                    }
                }
                return View(ct_PhieuChi);
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
                if (!Utility.KiemTraQuyen(API.ct_PhieuChi, API.Edit))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                v_v_ct_PhieuChi ct_PhieuChi = new v_v_ct_PhieuChi();
                if (!string.IsNullOrEmpty(id))
                {
                    apiResponse = Utility.GetDetail<v_ct_PhieuChi>(Utility.LOC_ID + "/" + id, API.ct_PhieuChi);
                    if (!apiResponse.Success)
                    {
                        TempData["TitleError"] = apiResponse.Message;
                        return RedirectToAction("Index", "Notfound");
                    }
                    if (apiResponse.Data != null)
                        ct_PhieuChi = apiResponse.Data as v_v_ct_PhieuChi;
                }
                //@ConvertObjectTCVN3ToUnicode
                ct_PhieuChi.lstdm_KhachHang = new List<ComboboxFrom>();
                //ct_PhieuChi.lstdm_KhachHang = Utility.GetListData<v_dm_KhachHang>(API.dm_KhachHang, "", "", Utility.LOC_ID).Data as List<v_dm_KhachHang>;
                ct_PhieuChi.lstdm_NhaCungCap = new List<ComboboxFrom>();
                //ct_PhieuChi.lstdm_NhaCungCap = Utility.GetListData<v_dm_NhaCungCap>(API.dm_NhaCungCap, "", "", Utility.LOC_ID).Data as List<v_dm_NhaCungCap>;
                ct_PhieuChi.lstdm_LoaiPhieuChi = new List<v_dm_LoaiPhieuChi>();
                //ct_PhieuChi.lstdm_LoaiPhieuChi = Utility.GetListData<v_dm_LoaiPhieuChi>(API.dm_LoaiPhieuChi, "", "", Utility.LOC_ID).Data as List<v_dm_LoaiPhieuChi>;
                ct_PhieuChi.lstdm_NhanVien = new List<ComboboxFrom>();
                ct_PhieuChi.lstdm_Xe = new List<ComboboxFrom>();
                //ct_PhieuChi.lstdm_NhanVien = Utility.GetListData<v_dm_NhanVien>(API.dm_NhanVien, "", "", Utility.LOC_ID).Data as List<v_dm_NhanVien>;
                ct_PhieuChi.lstdm_TaiKhoanNganHang = new List<v_dm_TaiKhoanNganHang>();
                return View(ct_PhieuChi);
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
        public ActionResult Edit([Bind(Include = "LOC_ID,ID,NAME_LOAIPHIEUCHI,ID_LOAIPHIEUCHI,NGAYLAP,MAPHIEU,SOPHIEU,NAME_KHACHHANG_NCC_NHANVIEN,ID_KHACHHANG,ID_NHACUNGCAP,ID_NHANVIEN,TENNGUOINHAN,DIACHI,NGUOICHITIEN,SOTIEN,LYDO,CHUNGTUKEMTHEO,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO,ID_TAIKHOANNGANHANG")] v_ct_PhieuChi ct_PhieuChi)
        {
            try
            {
                if (Utility.KiemTra())
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (!Utility.KiemTraQuyen(API.ct_PhieuChi, API.Edit))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                if (ModelState.IsValid)
                {
                    ct_PhieuChi.LOC_ID = Utility.LOC_ID;
                    ct_PhieuChi.ID_NGUOISUA = Session[Sessions.idUser].ToString();
                    ct_PhieuChi.THOIGIANSUA = Utility.CurrentTime;
                    //@ConvertObjectUnicodeToTCVN3
                    var apiResponse = Utility.Edit<v_ct_PhieuChi>(Utility.LOC_ID + "/" + ct_PhieuChi.ID, ct_PhieuChi, API.ct_PhieuChi);
                    if (apiResponse.Success)
                        return RedirectToAction("Index");
                    else
                        ModelState.AddModelError(string.Empty, apiResponse.Message);
                }
                return View(ct_PhieuChi);
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
                if (!Utility.KiemTraQuyen(API.ct_PhieuChi, API.Delete))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                var apiResponse = Utility.Delete<v_ct_PhieuChi>(Utility.LOC_ID + "/" + id, API.ct_PhieuChi);
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

        public ActionResult CreatePopup(string ID, string ID_LOAIPHIEU, string ID_KHACHAHANG = "", string CHUNGTUKEMTHEO = "")
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                if (Utility.KiemTra())
                {
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Admin");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                if (!Utility.KiemTraQuyen(API.ct_PhieuChi, API.Create))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }

                var lstdm_LoaiPhieuChi = Utility.GetListData<v_dm_LoaiPhieuChi>(API.dm_LoaiPhieuChi, "", "", Utility.LOC_ID).Data as List<v_dm_LoaiPhieuChi>;
                var dm_LoaiPhieuChi = lstdm_LoaiPhieuChi.Where(e => e.ID == ID_LOAIPHIEU).FirstOrDefault();
                if (dm_LoaiPhieuChi == null || string.IsNullOrEmpty(dm_LoaiPhieuChi.ID))
                {
                    TempData["TitleError"] = "Không tìm thấy loại phiếu chi";
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                v_v_ct_PhieuChi ct_PhieuChi = new v_v_ct_PhieuChi();
                apiResponse.Success = true;
                ct_PhieuChi.ID_LOAIPHIEUCHI = ID_LOAIPHIEU;
                ct_PhieuChi.LOC_ID = Utility.LOC_ID;
                ct_PhieuChi.ID = Guid.NewGuid().ToString();
                ct_PhieuChi.NGAYLAP = Utility.CurrentTime;
                
                //string NameController = API.ct_PhieuXuat;
                //var apiResponse_PX = GetValue<v_v_ct_PhieuGiaoHang>(apiResponse, NameController, CHUNGTUKEMTHEO);

                //if (apiResponse_PX.Detail != null)
                //ct_PhieuChi.SOTIEN = (apiResponse_PX.Detail as v_v_ct_PhieuGiaoHang).TONGTIEN;

                ct_PhieuChi.CHUNGTUKEMTHEO = CHUNGTUKEMTHEO;
                ct_PhieuChi.lstdm_NhaCungCap = new List<ComboboxFrom>();
                if (dm_LoaiPhieuChi.TYPE == 1)
                {
                    ct_PhieuChi.lstdm_NhaCungCap = Utility.GetListData<ComboboxFrom>(API.dm_NhaCungCap, "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
                    ct_PhieuChi.ID_NHACUNGCAP = ID_KHACHAHANG;
                    apiResponse.TYPE = "divNCCAdd";
                    foreach (var itm in ct_PhieuChi.lstdm_NhaCungCap.Where(s => s.ISDEFAULT))
                    {
                        itm.ISDEFAULT = false;
                    }
                    var ISDEFAULT = ct_PhieuChi.lstdm_NhaCungCap.Where(s => s.ID == ct_PhieuChi.ID_NHANVIEN).FirstOrDefault();
                    if (ISDEFAULT != null)
                        ISDEFAULT.ISDEFAULT = true;
                }

                ct_PhieuChi.lstdm_KhachHang = new List<ComboboxFrom>();
                if (dm_LoaiPhieuChi.TYPE == 2)
                {
                    apiResponse.TYPE = "divKHACHHANGAdd";
                    ct_PhieuChi.lstdm_KhachHang = Utility.GetListData<ComboboxFrom>(API.dm_KhachHang, "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
                    ct_PhieuChi.ID_KHACHHANG = ID_KHACHAHANG;
                    foreach (var itm in ct_PhieuChi.lstdm_KhachHang.Where(s => s.ISDEFAULT))
                    {
                        itm.ISDEFAULT = false;
                    }
                    var ISDEFAULT = ct_PhieuChi.lstdm_KhachHang.Where(s => s.ID == ct_PhieuChi.ID_KHACHHANG).FirstOrDefault();
                    if (ISDEFAULT != null)
                        ISDEFAULT.ISDEFAULT = true;
                }

                ct_PhieuChi.lstdm_NhanVien = new List<ComboboxFrom>();
                if (dm_LoaiPhieuChi.TYPE == 3)
                {
                    apiResponse.TYPE = "divNHANVIENAdd";
                    ct_PhieuChi.lstdm_NhanVien = Utility.GetListData<ComboboxFrom>(API.dm_NhanVien, "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
                    ct_PhieuChi.ID_NHANVIEN = ID_KHACHAHANG;
                    foreach (var itm in ct_PhieuChi.lstdm_NhanVien.Where(s => s.ISDEFAULT))
                    {
                        itm.ISDEFAULT = false;
                    }
                    var ISDEFAULT = ct_PhieuChi.lstdm_NhanVien.Where(s => s.ID == ct_PhieuChi.ID_NHANVIEN).FirstOrDefault();
                    if (ISDEFAULT != null)
                        ISDEFAULT.ISDEFAULT = true;
                }

                ct_PhieuChi.lstdm_Xe = new List<ComboboxFrom>();
                if (dm_LoaiPhieuChi.TYPE == 4)
                {
                    apiResponse.TYPE = "divXEAdd";
                    ct_PhieuChi.lstdm_Xe = Utility.GetListData<ComboboxFrom>(API.dm_Xe, "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
                    ct_PhieuChi.ID_XE = ID_KHACHAHANG;
                    foreach (var itm in ct_PhieuChi.lstdm_Xe.Where(s => s.ISDEFAULT))
                    {
                        itm.ISDEFAULT = false;
                    }
                    var ISDEFAULT = ct_PhieuChi.lstdm_Xe.Where(s => s.ID == ct_PhieuChi.ID_XE).FirstOrDefault();
                    if (ISDEFAULT != null)
                        ISDEFAULT.ISDEFAULT = true;
                }

                if (CHUNGTUKEMTHEO.StartsWith("PGH-"))
                {
                    string NameController = API.ct_PhieuGiaoHang;
                    var apiResponse_PX = GetValue<v_v_ct_PhieuGiaoHang>(apiResponse, NameController, CHUNGTUKEMTHEO);

                    if (apiResponse_PX.Detail != null)
                    {
                        ct_PhieuChi.NGAYLAP = (apiResponse_PX.Detail as v_v_ct_PhieuGiaoHang).NGAYLAP;
                    }
                }
                if (CHUNGTUKEMTHEO.StartsWith("PX-"))
                {
                    string NameController = API.ct_PhieuXuat;
                    var apiResponse_PX = GetValue<v_v_ct_PhieuXuat>(apiResponse, NameController, CHUNGTUKEMTHEO);

                    if (apiResponse_PX.Detail != null)
                    {
                        ct_PhieuChi.SOTIEN = (apiResponse_PX.Detail as v_v_ct_PhieuXuat).TONGTIEN;
                        ct_PhieuChi.NGAYLAP = (apiResponse_PX.Detail as v_v_ct_PhieuXuat).NGAYLAP;
                    }
                }
                ct_PhieuChi.SOPHIEU = Utility.GetMaxID<ct_PhieuChi>(ct_PhieuChi, Utility.LOC_ID, ct_PhieuChi.NGAYLAP.ToString("yyyy-MM-dd"));
                ct_PhieuChi.MAPHIEU = API.GetMaPhieu(API.ct_PhieuChi, ct_PhieuChi.NGAYLAP, ct_PhieuChi.SOPHIEU);
                ct_PhieuChi.lstdm_LoaiPhieuChi = new List<v_dm_LoaiPhieuChi>();
                ct_PhieuChi.lstdm_LoaiPhieuChi = Utility.GetListData<v_dm_LoaiPhieuChi>(API.dm_LoaiPhieuChi, "", "", Utility.LOC_ID).Data as List<v_dm_LoaiPhieuChi>;
                ct_PhieuChi.lstdm_TaiKhoanNganHang = new List<v_dm_TaiKhoanNganHang>();
                ct_PhieuChi.lstdm_TaiKhoanNganHang = Utility.GetListData<v_dm_TaiKhoanNganHang>(API.dm_TaiKhoanNganHang, "", "", Utility.LOC_ID).Data as List<v_dm_TaiKhoanNganHang>;
                var lst = Utility.ConvertobjectTo<v_v_ct_PhieuChi>(ct_PhieuChi);
                ValueEdit objValueEdit = new ValueEdit();
                objValueEdit.Key = "lblName";
                objValueEdit.Value = dm_LoaiPhieuChi.NAME.ToUpper();
                lst.Add(objValueEdit);
                apiResponse.Detail = lst;
                if (!string.IsNullOrEmpty(ID_KHACHAHANG) || !string.IsNullOrEmpty(CHUNGTUKEMTHEO))
                    apiResponse.NAME = "myModalAddPayment";
                else
                    apiResponse.NAME = "myModalAdd";
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
        // GET: Menu/Create
        //public ActionResult CreatePopup(string ID_LOAIPHIEU)
        //{
        //    ApiResponse apiResponse = new ApiResponse();
        //    try
        //    {
        //        if (Utility.KiemTra())
        //        {
        //            apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Admin");
        //            return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
        //        }
        //        if (!Utility.KiemTraQuyen(API.ct_PhieuChi, API.Create))
        //        {
        //            TempData["TitleError"] = API.TitlePermission;
        //            apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
        //            return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
        //        }

        //        var lstdm_LoaiPhieuChi = Utility.GetListData<v_dm_LoaiPhieuChi>(API.dm_LoaiPhieuChi, "", "", Utility.LOC_ID).Data as List<v_dm_LoaiPhieuChi>;
        //        var dm_LoaiPhieuChi = lstdm_LoaiPhieuChi.Where(e => e.ID == ID_LOAIPHIEU).FirstOrDefault();
        //        if (dm_LoaiPhieuChi == null || string.IsNullOrEmpty(dm_LoaiPhieuChi.ID))
        //        {
        //            TempData["TitleError"] = "Không tìm thấy loại phiếu chi";
        //            apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
        //            return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
        //        }
        //        v_v_ct_PhieuChi ct_PhieuChi = new v_v_ct_PhieuChi();
        //        apiResponse.Success = true;
        //        ct_PhieuChi.ID_LOAIPHIEUCHI = ID_LOAIPHIEU;
        //        ct_PhieuChi.LOC_ID = Utility.LOC_ID;
        //        ct_PhieuChi.ID = Guid.NewGuid().ToString();
        //        ct_PhieuChi.NGAYLAP = Utility.CurrentTime;
        //        ct_PhieuChi.SOPHIEU = Utility.GetMaxID<ct_PhieuChi>(ct_PhieuChi, Utility.LOC_ID, ct_PhieuChi.NGAYLAP.ToString("yyyy-MM-dd"));
        //        ct_PhieuChi.MAPHIEU = API.GetMaPhieu(API.ct_PhieuChi, ct_PhieuChi.NGAYLAP, ct_PhieuChi.SOPHIEU);
        //        ct_PhieuChi.lstdm_NhaCungCap = new List<ComboboxFrom>();
        //        if (dm_LoaiPhieuChi.TYPE == 1)
        //        {
        //            ct_PhieuChi.lstdm_NhaCungCap = Utility.GetListData<ComboboxFrom>(API.dm_NhaCungCap, "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
        //            apiResponse.TYPE = "divNCCAdd";
        //        }

        //        ct_PhieuChi.lstdm_KhachHang = new List<ComboboxFrom>();
        //        if (dm_LoaiPhieuChi.TYPE == 2)
        //        {
        //            apiResponse.TYPE = "divKHACHHANGAdd";
        //            ct_PhieuChi.lstdm_KhachHang = Utility.GetListData<ComboboxFrom>(API.dm_KhachHang, "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
        //        }

        //        ct_PhieuChi.lstdm_NhanVien = new List<ComboboxFrom>();
        //        if (dm_LoaiPhieuChi.TYPE == 3)
        //        {
        //            apiResponse.TYPE = "divNHANVIENAdd";
        //            ct_PhieuChi.lstdm_NhanVien = Utility.GetListData<ComboboxFrom>(API.dm_NhanVien, "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
        //        }
        //        ct_PhieuChi.lstdm_Xe = new List<ComboboxFrom>();
        //        if (dm_LoaiPhieuChi.TYPE == 4)
        //        {
        //            apiResponse.TYPE = "divXeAdd";
        //            ct_PhieuChi.lstdm_Xe = Utility.GetListData<ComboboxFrom>(API.dm_Xe, "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
        //        }
                
        //        ct_PhieuChi.lstdm_LoaiPhieuChi = new List<v_dm_LoaiPhieuChi>();
        //        ct_PhieuChi.lstdm_LoaiPhieuChi = Utility.GetListData<v_dm_LoaiPhieuChi>(API.dm_LoaiPhieuChi, "", "", Utility.LOC_ID).Data as List<v_dm_LoaiPhieuChi>;
        //        ct_PhieuChi.lstdm_TaiKhoanNganHang = new List<v_dm_TaiKhoanNganHang>();
        //        ct_PhieuChi.lstdm_TaiKhoanNganHang = Utility.GetListData<v_dm_TaiKhoanNganHang>(API.dm_TaiKhoanNganHang, "", "", Utility.LOC_ID).Data as List<v_dm_TaiKhoanNganHang>;
        //        var lst = Utility.ConvertobjectTo<v_v_ct_PhieuChi>(ct_PhieuChi);
        //        apiResponse.Detail = lst;
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

        // POST: Menu/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult CreatePopup([Bind(Include = "LOC_ID,ID,NAME_LOAIPHIEUCHI,ID_LOAIPHIEUCHI,NGAYLAP,MAPHIEU,SOPHIEU,NAME_KHACHHANG_NCC_NHANVIEN,ID_KHACHHANG,ID_NHACUNGCAP,ID_NHANVIEN,TENNGUOINHAN,DIACHI,NGUOICHITIEN,SOTIEN,LYDO,CHUNGTUKEMTHEO,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO,ID_TAIKHOANNGANHANG")] v_v_ct_PhieuChi ct_PhieuChi)
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                if (Utility.KiemTra())
                {
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Admin");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                if (!Utility.KiemTraQuyen(API.ct_PhieuChi, API.Create))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                if (ModelState.IsValid)
                {
                    ct_PhieuChi.NGAYLAP = ct_PhieuChi.NGAYLAP.AddHours(Utility.CurrentTime.Hour).AddMinutes(Utility.CurrentTime.Minute);
                    ct_PhieuChi.LOC_ID = Utility.LOC_ID;
                    ct_PhieuChi.ID_NGUOITAO = Session[Sessions.idUser].ToString();
                    ct_PhieuChi.THOIGIANTHEM = Utility.CurrentTime;

                    apiResponse = Utility.Create<v_ct_PhieuChi>(ct_PhieuChi, API.ct_PhieuChi);
                    if (apiResponse.Success)
                    {
                        ct_PhieuChi.NGAYLAP = Utility.CurrentTime;
                        apiResponse.SOPHIEU = ct_PhieuChi.SOPHIEU = Utility.GetMaxID<ct_PhieuChi>(ct_PhieuChi, Utility.LOC_ID, ct_PhieuChi.NGAYLAP.ToString("yyyy-MM-dd"));
                        ct_PhieuChi.MAPHIEU = API.GetMaPhieu(API.ct_PhieuChi, ct_PhieuChi.NGAYLAP, ct_PhieuChi.SOPHIEU);
                        apiResponse.NewID = Guid.NewGuid().ToString();
                        apiResponse.MAPHIEU = ct_PhieuChi.MAPHIEU;

                        if (apiResponse.Data != null)
                            ct_PhieuChi = JsonConvert.DeserializeObject<v_v_ct_PhieuChi>(apiResponse.Data.ToString());
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, apiResponse.Message);
                        if (apiResponse.CheckValue)
                        {
                            ct_PhieuChi.NGAYLAP = Utility.CurrentTime;
                            apiResponse.SOPHIEU = ct_PhieuChi.SOPHIEU = Utility.GetMaxID<ct_PhieuChi>(ct_PhieuChi, Utility.LOC_ID, ct_PhieuChi.NGAYLAP.ToString("yyyy-MM-dd"));
                            ct_PhieuChi.MAPHIEU = API.GetMaPhieu(API.ct_PhieuChi, ct_PhieuChi.NGAYLAP, ct_PhieuChi.SOPHIEU);
                            apiResponse.NewID = Guid.NewGuid().ToString();
                            apiResponse.MAPHIEU = ct_PhieuChi.MAPHIEU;
                        }
                    }
                }
                else
                {
                    apiResponse.Success = false;
                    apiResponse.Data = Utility.GetModelState(ModelState, API.ct_PhieuChi);
                }
                apiResponse.ID = ct_PhieuChi.ID;
                var lstdm_LoaiPhieuChi = Utility.GetListData<v_dm_LoaiPhieuChi>(API.dm_LoaiPhieuChi, "", "", Utility.LOC_ID).Data as List<v_dm_LoaiPhieuChi>;
                var dm_LoaiPhieuChi = lstdm_LoaiPhieuChi.Where(e => e.ID == ct_PhieuChi.ID_LOAIPHIEUCHI).FirstOrDefault();
                if (dm_LoaiPhieuChi == null || string.IsNullOrEmpty(dm_LoaiPhieuChi.ID))
                {
                    TempData["TitleError"] = "Không tìm thấy loại phiếu chi";
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                ct_PhieuChi.lstdm_NhaCungCap = new List<ComboboxFrom>();
                if (dm_LoaiPhieuChi.TYPE == 1)
                {
                    ct_PhieuChi.lstdm_NhaCungCap = Utility.GetListData<ComboboxFrom>(API.dm_NhaCungCap, "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
                    apiResponse.TYPE = "divNCCAdd";
                }

                ct_PhieuChi.lstdm_KhachHang = new List<ComboboxFrom>();
                if (dm_LoaiPhieuChi.TYPE == 2)
                {
                    apiResponse.TYPE = "divKHACHHANGAdd";
                    ct_PhieuChi.lstdm_KhachHang = Utility.GetListData<ComboboxFrom>(API.dm_KhachHang, "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
                }
                ct_PhieuChi.lstdm_NhanVien = new List<ComboboxFrom>();
                if (dm_LoaiPhieuChi.TYPE == 3)
                {
                    apiResponse.TYPE = "divNHANVIENAdd";
                    ct_PhieuChi.lstdm_NhanVien = Utility.GetListData<ComboboxFrom>(API.dm_NhanVien, "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
                }
                ct_PhieuChi.lstdm_Xe = new List<ComboboxFrom>();
                if (dm_LoaiPhieuChi.TYPE == 4)
                {
                    apiResponse.TYPE = "divXeAdd";
                    ct_PhieuChi.lstdm_Xe = Utility.GetListData<ComboboxFrom>(API.dm_Xe, "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
                }
                ct_PhieuChi.lstdm_LoaiPhieuChi = new List<v_dm_LoaiPhieuChi>();
                ct_PhieuChi.lstdm_TaiKhoanNganHang = new List<v_dm_TaiKhoanNganHang>();
                ct_PhieuChi.lstdm_TaiKhoanNganHang = Utility.GetListData<v_dm_TaiKhoanNganHang>(API.dm_TaiKhoanNganHang, "", "", Utility.LOC_ID).Data as List<v_dm_TaiKhoanNganHang>;
                var lst = Utility.ConvertobjectToView<v_v_ct_PhieuChi>(ct_PhieuChi);
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
                if (!Utility.KiemTraQuyen(API.ct_PhieuChi, API.Edit))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }

                v_v_ct_PhieuChi ct_PhieuChi = new v_v_ct_PhieuChi();
                if (!string.IsNullOrEmpty(id))
                {
                    apiResponse = Utility.GetDetail<v_v_ct_PhieuChi>(Utility.LOC_ID + "/" + id, API.ct_PhieuChi);
                    if (!apiResponse.Success)
                    {
                        TempData["TitleError"] = apiResponse.Message;
                        apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                        return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                    }
                    if (apiResponse.Data != null)
                        ct_PhieuChi = apiResponse.Data as v_v_ct_PhieuChi;
                }
                ct_PhieuChi.lstdm_NhaCungCap = new List<ComboboxFrom>();
                apiResponse.Success = true;
                var lstdm_LoaiPhieuChi = Utility.GetListData<v_dm_LoaiPhieuChi>(API.dm_LoaiPhieuChi, "", "", Utility.LOC_ID).Data as List<v_dm_LoaiPhieuChi>;
                var dm_LoaiPhieuChi = lstdm_LoaiPhieuChi.Where(e => e.ID == ct_PhieuChi.ID_LOAIPHIEUCHI).FirstOrDefault();
                if (dm_LoaiPhieuChi == null || string.IsNullOrEmpty(dm_LoaiPhieuChi.ID))
                {
                    TempData["TitleError"] = "Không tìm thấy loại phiếu chi";
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                if (dm_LoaiPhieuChi.TYPE == 1)
                {
                    ct_PhieuChi.lstdm_NhaCungCap = Utility.GetListData<ComboboxFrom>(API.dm_NhaCungCap, "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
                    apiResponse.TYPE = "divNCCEdit";
                }

                ct_PhieuChi.lstdm_KhachHang = new List<ComboboxFrom>();
                if (dm_LoaiPhieuChi.TYPE == 2)
                {
                    apiResponse.TYPE = "divKHACHHANGEdit";
                    ct_PhieuChi.lstdm_KhachHang = Utility.GetListData<ComboboxFrom>(API.dm_KhachHang, "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
                }
                ct_PhieuChi.lstdm_NhanVien = new List<ComboboxFrom>();
                if (dm_LoaiPhieuChi.TYPE == 3)
                {
                    apiResponse.TYPE = "divNHANVIENEdit";
                    ct_PhieuChi.lstdm_NhanVien = Utility.GetListData<ComboboxFrom>(API.dm_NhanVien, "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
                }
                ct_PhieuChi.lstdm_Xe = new List<ComboboxFrom>();
                if (dm_LoaiPhieuChi.TYPE == 4)
                {
                    apiResponse.TYPE = "divXeEdit";
                    ct_PhieuChi.lstdm_Xe = Utility.GetListData<ComboboxFrom>(API.dm_Xe, "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
                }
                ct_PhieuChi.lstdm_LoaiPhieuChi = new List<v_dm_LoaiPhieuChi>();
                ct_PhieuChi.lstdm_LoaiPhieuChi = lstdm_LoaiPhieuChi;
                ct_PhieuChi.lstdm_TaiKhoanNganHang = new List<v_dm_TaiKhoanNganHang>();
                ct_PhieuChi.lstdm_TaiKhoanNganHang = Utility.GetListData<v_dm_TaiKhoanNganHang>(API.dm_TaiKhoanNganHang, "", "", Utility.LOC_ID).Data as List<v_dm_TaiKhoanNganHang>;
                var lst = Utility.ConvertobjectTo<v_v_ct_PhieuChi>(ct_PhieuChi);
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
        public ActionResult EditPopup([Bind(Include = "LOC_ID,ID,NAME_LOAIPHIEUCHI,ID_LOAIPHIEUCHI,NGAYLAP,MAPHIEU,SOPHIEU,NAME_KHACHHANG_NCC_NHANVIEN,ID_KHACHHANG,ID_NHACUNGCAP,ID_NHANVIEN,TENNGUOINHAN,DIACHI,NGUOICHITIEN,SOTIEN,LYDO,CHUNGTUKEMTHEO,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO,ID_TAIKHOANNGANHANG")] v_v_ct_PhieuChi ct_PhieuChi)
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                if (Utility.KiemTra())
                {
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Admin");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                if (!Utility.KiemTraQuyen(API.ct_PhieuChi, API.Edit))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }

                if (ModelState.IsValid)
                {
                    ct_PhieuChi.LOC_ID = Utility.LOC_ID;
                    ct_PhieuChi.ID_NGUOISUA = Session[Sessions.idUser].ToString();
                    ct_PhieuChi.THOIGIANSUA = Utility.CurrentTime;

                    apiResponse = Utility.Edit<v_ct_PhieuChi>(Utility.LOC_ID + "/" + ct_PhieuChi.ID, ct_PhieuChi, API.ct_PhieuChi);
                    if (apiResponse.Success)
                    {
                        //return RedirectToAction("Index");
                        apiResponse.ID = ct_PhieuChi.ID;
                        if (apiResponse.Data != null)
                            ct_PhieuChi = JsonConvert.DeserializeObject<v_v_ct_PhieuChi>(apiResponse.Data.ToString());
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, apiResponse.Message);
                    }
                }
                else
                {
                    apiResponse.Success = false;
                    apiResponse.Data = Utility.GetModelState(ModelState, API.ct_PhieuChi);
                }
                ct_PhieuChi.lstdm_NhaCungCap = new List<ComboboxFrom>();
                var lstdm_LoaiPhieuChi = Utility.GetListData<v_dm_LoaiPhieuChi>(API.dm_LoaiPhieuChi, "", "", Utility.LOC_ID).Data as List<v_dm_LoaiPhieuChi>;
                var dm_LoaiPhieuChi = lstdm_LoaiPhieuChi.Where(e => e.ID == ct_PhieuChi.ID_LOAIPHIEUCHI).FirstOrDefault();
                if (dm_LoaiPhieuChi == null || string.IsNullOrEmpty(dm_LoaiPhieuChi.ID))
                {
                    TempData["TitleError"] = "Không tìm thấy loại phiếu chi";
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                if (dm_LoaiPhieuChi.TYPE == 1)
                {
                    ct_PhieuChi.lstdm_NhaCungCap = Utility.GetListData<ComboboxFrom>(API.dm_NhaCungCap, "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
                    apiResponse.TYPE = "divNCCEdit";
                }

                ct_PhieuChi.lstdm_KhachHang = new List<ComboboxFrom>();
                if (dm_LoaiPhieuChi.TYPE == 2)
                {
                    apiResponse.TYPE = "divKHACHHANGEdit";
                    ct_PhieuChi.lstdm_KhachHang = Utility.GetListData<ComboboxFrom>(API.dm_KhachHang, "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
                }

                ct_PhieuChi.lstdm_NhanVien = new List<ComboboxFrom>();
                if (dm_LoaiPhieuChi.TYPE == 3)
                {
                    apiResponse.TYPE = "divNHANVIENEdit";
                    ct_PhieuChi.lstdm_NhanVien = Utility.GetListData<ComboboxFrom>(API.dm_NhanVien, "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
                }
                ct_PhieuChi.lstdm_Xe = new List<ComboboxFrom>();
                if (dm_LoaiPhieuChi.TYPE == 4)
                {
                    apiResponse.TYPE = "divXeEdit";
                    ct_PhieuChi.lstdm_Xe = Utility.GetListData<ComboboxFrom>(API.dm_Xe, "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
                }
                ct_PhieuChi.lstdm_LoaiPhieuChi = new List<v_dm_LoaiPhieuChi>();
                ct_PhieuChi.lstdm_LoaiPhieuChi = lstdm_LoaiPhieuChi;
                ct_PhieuChi.lstdm_TaiKhoanNganHang = new List<v_dm_TaiKhoanNganHang>();
                ct_PhieuChi.lstdm_TaiKhoanNganHang = Utility.GetListData<v_dm_TaiKhoanNganHang>(API.dm_TaiKhoanNganHang, "", "", Utility.LOC_ID).Data as List<v_dm_TaiKhoanNganHang>;
                apiResponse.Detail = Utility.ConvertobjectToView<v_v_ct_PhieuChi>(ct_PhieuChi);
                var lst = Utility.ConvertobjectToView<v_v_ct_PhieuChi>(ct_PhieuChi);
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
                if (!Utility.KiemTraQuyen(API.ct_PhieuChi, API.Delete))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                apiResponse = Utility.Delete<v_ct_PhieuChi>(Utility.LOC_ID + "/" + id, API.ct_PhieuChi);
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

                v_ct_PhieuChi PhieuNhap = new v_ct_PhieuChi();
                SP_Parameter objParameter = new SP_Parameter();
                objParameter.ID_PHIEUCHI = ID;
                apiResponse = Utility.ExecuteStoredProc<v_ct_PhieuChi>(objParameter, API.Sp_Get_DanhSachPhieuChi);
                if (!apiResponse.Success)
                {
                    apiResponse.Success = false;
                    apiResponse.Message = apiResponse.Message;
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                if (apiResponse.Data != null)
                    PhieuNhap = (apiResponse.Data as List<v_ct_PhieuChi>).FirstOrDefault();

                var report = new ReportClass();
                report.FileName = Server.MapPath("~/Report/rptPhieuChi.rpt");

                SP_Parameter_Report objParameter_Report = new SP_Parameter_Report();
                objParameter_Report.LOC_ID = Utility.LOC_ID;
                objParameter_Report.ID_PHIEUCHI = ID;
                apiResponse = Utility.ExecuteStoredProc<DataTable>(objParameter_Report, API.Sp_Get_DanhSachPhieuChi);
                if (!apiResponse.Success)
                {
                    apiResponse.Success = false;
                    apiResponse.Message = apiResponse.Message;
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }

                //QRCodeLogo qrCodeLogo = new QRCodeLogo(fullpathLogo);
                //GeneratedBarcode MyVerifiedQR = QRCodeWriter.CreateQrCodeWithLogo(BinaryData, qrCodeLogo, 500);
                //MyVerifiedQR.ResizeTo(500, 500).SetMargins(10).ChangeBarCodeColor(Color.DarkGreen);
                //MyVerifiedQR.SaveAsImage(fullpath);

                DataTable data = (apiResponse.Data as DataTable);
                if (apiResponse.CheckValue)
                    data.Rows.Clear();

                report = Utility.GetFormulaFields(report, PhieuNhap);
                report.SetDataSource(data);
                Response.Buffer = false;
                Response.ClearContent();
                Response.ClearHeaders();
                Stream stream = report.ExportToStream(ExportFormatType.PortableDocFormat);
                Utility.Report = report;
                apiResponse = new ApiResponse();
                apiResponse.Success = true;
                apiResponse.NAME = Utility.GetTitleFrom(API.ct_PhieuChi) + " - " + PhieuNhap.MAPHIEU;
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