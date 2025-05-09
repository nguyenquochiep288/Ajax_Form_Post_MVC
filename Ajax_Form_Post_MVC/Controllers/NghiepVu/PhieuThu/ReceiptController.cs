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
using static System.Runtime.CompilerServices.RuntimeHelpers;

namespace MVC_QuanLyTHP.Controllers
{
    public class ReceiptController : Controller
    {

        // GET: Receipt
        public ActionResult Index(int Page = 1, DateTime? FromDate = null, DateTime? ToDate = null, string SearchString = "", string MAPHIEU = "", string IDCODE = "")
        {
            try
            {
                if (Utility.KiemTra())
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (!Utility.KiemTraQuyen(API.ct_PhieuThu, API.Xem))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                string TotalSum = "";
                ApiResponse apiResponse = new ApiResponse();
                IPagedList<v_ct_PhieuThu> lstpage = (new List<v_ct_PhieuThu>()).ToList().ToPagedList(Page, Utility.GetPageSize());
                if (FromDate != null || !string.IsNullOrEmpty(IDCODE) || !string.IsNullOrEmpty(MAPHIEU))
                {
                    if (!string.IsNullOrEmpty(IDCODE) || !string.IsNullOrEmpty(MAPHIEU))
                    {
                        apiResponse = Utility.Get_DanhSachPhieuThu<v_ct_PhieuThu>("", null, null, MAPHIEU, IDCODE);
                    }
                    if (FromDate != null)
                    {
                        apiResponse = Utility.Get_DanhSachPhieuThu<v_ct_PhieuThu>("", FromDate, ToDate, SearchString);
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
                    TotalSum = (apiResponse.Data as List<v_ct_PhieuThu>).Sum(s => s.SOTIEN).ToString("N0");
                    lstpage = (apiResponse.Data as List<v_ct_PhieuThu>).ToPagedList(Page, Utility.GetPageSize());
                }
                v_v_ct_PhieuThu ct_PhieuThu = new v_v_ct_PhieuThu();
                ct_PhieuThu.IPagedList = lstpage;
                ct_PhieuThu.lstdm_NhaCungCap = new List<ComboboxFrom>();
                //ct_PhieuThu.lstdm_NhaCungCap = Utility.GetListData<v_dm_NhaCungCap>(API.dm_NhaCungCap, "", "", Utility.LOC_ID).Data as List<v_dm_NhaCungCap>;
                ct_PhieuThu.lstdm_KhachHang = new List<ComboboxFrom>();
                ct_PhieuThu.lstdm_LoaiPhieuThu = new List<v_dm_LoaiPhieuThu>();
                var lstdm_LoaiPhieuThu = Utility.GetListData<v_dm_LoaiPhieuThu>(API.dm_LoaiPhieuThu, "", "", Utility.LOC_ID).Data as List<v_dm_LoaiPhieuThu>;
                if (lstdm_LoaiPhieuThu != null)
                {
                    ct_PhieuThu.lstdm_LoaiPhieuThu = lstdm_LoaiPhieuThu.Where(e => e.ISACTIVE == true).OrderBy(e => e.TYPE).ToList();
                }
                else
                {
                    ct_PhieuThu.lstdm_LoaiPhieuThu = new List<v_dm_LoaiPhieuThu>();
                }

                ct_PhieuThu.lstdm_NhanVien = new List<ComboboxFrom>();
                //ct_PhieuThu.lstdm_NhanVien = Utility.GetListData<v_dm_NhanVien>(API.dm_NhanVien, "", "", Utility.LOC_ID).Data as List<v_dm_NhanVien>;
                ct_PhieuThu.lstdm_TaiKhoanNganHang = new List<v_dm_TaiKhoanNganHang>();
                ViewBag.TotalSum = TotalSum;
                ViewBag.searchValue = SearchString;

                ViewBag.PermissionEdit = Utility.KiemTraQuyen(API.ct_PhieuThu, API.Edit);
                ViewBag.PermissionDelete = Utility.KiemTraQuyen(API.ct_PhieuThu, API.Delete);
                ViewBag.PermissionCreate = Utility.KiemTraQuyen(API.ct_PhieuThu, API.Create);

                ViewBag.fromdate = FromDate != null ? FromDate.Value.ToString("yyyy-MM-dd") : Utility.CurrentTime.AddMonths(-1).ToString("yyyy-MM-dd");
                ViewBag.todate = ToDate != null ? ToDate.Value.ToString("yyyy-MM-dd") : Utility.CurrentTime.ToString("yyyy-MM-dd");
                return View(ct_PhieuThu);
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
        public ActionResult Create(int type = 2, string myModalAdd = "myModalAdd", string hienthichuyencongno = "0")
        {
            try
            {
                Session[Sessions.IntWidth] = type;
                if (Utility.KiemTra())
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (!Utility.KiemTraQuyen(API.ct_PhieuGiaoHang, API.Delivery_CreateReceipt))
                {
                    if (!Utility.KiemTraQuyen(API.ct_PhieuThu, API.Create))
                    {
                        TempData["TitleError"] = API.TitlePermission;
                        return RedirectToAction("Index", "Notfound");
                    }
                }    
                    
                v_v_ct_PhieuThu ct_PhieuThu = new v_v_ct_PhieuThu();
                ct_PhieuThu.LOC_ID = Utility.LOC_ID;
                ct_PhieuThu.ID_NGUOITAO = Session[Sessions.idUser].ToString();
                ct_PhieuThu.THOIGIANTHEM = Utility.CurrentTime;
                ct_PhieuThu.NGAYLAP = Utility.CurrentTime;
                ct_PhieuThu.SOPHIEU = Utility.GetMaxID<ct_PhieuThu>(ct_PhieuThu, Utility.LOC_ID, ct_PhieuThu.NGAYLAP.ToString("yyyy-MM-dd"));
                ct_PhieuThu.MAPHIEU = API.GetMaPhieu(API.ct_PhieuThu, ct_PhieuThu.NGAYLAP, ct_PhieuThu.SOPHIEU);
                ct_PhieuThu.lstdm_KhachHang = new List<ComboboxFrom>();

                ct_PhieuThu.ID = Guid.NewGuid().ToString();
                ct_PhieuThu.lstdm_NhaCungCap = new List<ComboboxFrom>();
                //ct_PhieuThu.lstdm_NhaCungCap = Utility.GetListData<v_dm_NhaCungCap>(API.dm_NhaCungCap, "", "", Utility.LOC_ID).Data as List<v_dm_NhaCungCap>;
                ct_PhieuThu.lstdm_KhachHang = new List<ComboboxFrom>();
                //ct_PhieuThu.lstdm_Kho = Utility.GetListData<v_dm_Kho>(API.dm_Kho, "", "", Utility.LOC_ID).Data as List<v_dm_Kho>;
                ct_PhieuThu.lstdm_LoaiPhieuThu = new List<v_dm_LoaiPhieuThu>();
                //ct_PhieuThu.lstdm_LoaiPhieuThu = Utility.GetListData<v_dm_LoaiPhieuThu>(API.dm_LoaiPhieuThu, "", "", Utility.LOC_ID).Data as List<v_dm_LoaiPhieuThu>;
                ct_PhieuThu.lstdm_NhanVien = new List<ComboboxFrom>();
                //ct_PhieuThu.lstdm_NhanVien = Utility.GetListData<v_dm_NhanVien>(API.dm_NhanVien, "", "", Utility.LOC_ID).Data as List<v_dm_NhanVien>;
                ct_PhieuThu.lstdm_TaiKhoanNganHang = new List<v_dm_TaiKhoanNganHang>();
                ViewBag.myModalAdd = myModalAdd;
                ct_PhieuThu.myModalAdd = myModalAdd;

                ViewBag.HienThiChuyenCongNo = (hienthichuyencongno == "1");
                return View(ct_PhieuThu);
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
        public ActionResult Create([Bind(Include = "ISCHUYENCONGNOCHONHANVIEN,LOC_ID,ID,ID_LOAIPHIEUTHU,NAME_LOAIPHIEUTHU,NGAYLAP,MAPHIEU,SOPHIEU,NAME_KHACHHANG_NCC_NHANVIEN,ID_KHACHHANG,ID_NHACUNGCAP,ID_NHANVIEN,NGUOINHANTIEN,TENNGUOINOPTIEN,DIACHI,SOTIEN,LYDO,CHUNGTUKEMTHEO,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO,ID_TAIKHOANNGANHANG")] v_ct_PhieuThu ct_PhieuThu)
        {
            try
            {
                if (Utility.KiemTra())
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (!Utility.KiemTraQuyen(API.ct_PhieuThu, API.Create))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                if (ModelState.IsValid)
                {
                    ct_PhieuThu.LOC_ID = Utility.LOC_ID;
                    ct_PhieuThu.ID_NGUOITAO = Session[Sessions.idUser].ToString();
                    ct_PhieuThu.THOIGIANTHEM = Utility.CurrentTime;

                    //@ConvertObjectUnicodeToTCVN3
                    var apiResponse = Utility.Create<ct_PhieuThu>(ct_PhieuThu, API.ct_PhieuThu);
                    if (apiResponse.Success)
                        return RedirectToAction("Index");
                    else
                    {
                        ModelState.AddModelError(string.Empty, apiResponse.Message);
                        if (apiResponse.CheckValue)
                            ViewBag.ID = Guid.NewGuid().ToString();
                    }
                }
                return View(ct_PhieuThu);
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
                if (!Utility.KiemTraQuyen(API.ct_PhieuThu, API.Edit))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                v_v_ct_PhieuThu ct_PhieuThu = new v_v_ct_PhieuThu();
                if (!string.IsNullOrEmpty(id))
                {
                    apiResponse = Utility.GetDetail<v_ct_PhieuThu>(Utility.LOC_ID + "/" + id, API.ct_PhieuThu);
                    if (!apiResponse.Success)
                    {
                        TempData["TitleError"] = apiResponse.Message;
                        return RedirectToAction("Index", "Notfound");
                    }
                    if (apiResponse.Data != null)
                        ct_PhieuThu = apiResponse.Data as v_v_ct_PhieuThu;
                }
                //@ConvertObjectTCVN3ToUnicode
                ct_PhieuThu.lstdm_KhachHang = new List<ComboboxFrom>();
                //ct_PhieuThu.lstdm_KhachHang = Utility.GetListData<v_dm_KhachHang>(API.dm_KhachHang, "", "", Utility.LOC_ID).Data as List<v_dm_KhachHang>;
                ct_PhieuThu.lstdm_NhaCungCap = new List<ComboboxFrom>();
                //ct_PhieuThu.lstdm_NhaCungCap = Utility.GetListData<v_dm_NhaCungCap>(API.dm_NhaCungCap, "", "", Utility.LOC_ID).Data as List<v_dm_NhaCungCap>;
                ct_PhieuThu.lstdm_LoaiPhieuThu = new List<v_dm_LoaiPhieuThu>();
                //ct_PhieuThu.lstdm_LoaiPhieuThu = Utility.GetListData<v_dm_LoaiPhieuThu>(API.dm_LoaiPhieuThu, "", "", Utility.LOC_ID).Data as List<v_dm_LoaiPhieuThu>;
                ct_PhieuThu.lstdm_NhanVien = new List<ComboboxFrom>();
                //ct_PhieuThu.lstdm_NhanVien = Utility.GetListData<v_dm_NhanVien>(API.dm_NhanVien, "", "", Utility.LOC_ID).Data as List<v_dm_NhanVien>;
                ct_PhieuThu.lstdm_TaiKhoanNganHang = new List<v_dm_TaiKhoanNganHang>();
                
                return View(ct_PhieuThu);
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
        public ActionResult Edit([Bind(Include = "ISCHUYENCONGNOCHONHANVIEN,LOC_ID,ID,ID_LOAIPHIEUTHU,NAME_LOAIPHIEUTHU,NGAYLAP,MAPHIEU,SOPHIEU,NAME_KHACHHANG_NCC_NHANVIEN,ID_KHACHHANG,ID_NHACUNGCAP,ID_NHANVIEN,NGUOINHANTIEN,TENNGUOINOPTIEN,DIACHI,SOTIEN,LYDO,CHUNGTUKEMTHEO,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO,ID_TAIKHOANNGANHANG")] v_ct_PhieuThu ct_PhieuThu)
        {
            try
            {
                if (Utility.KiemTra())
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (!Utility.KiemTraQuyen(API.ct_PhieuThu, API.Edit))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                if (ModelState.IsValid)
                {
                    ct_PhieuThu.LOC_ID = Utility.LOC_ID;
                    ct_PhieuThu.ID_NGUOISUA = Session[Sessions.idUser].ToString();
                    ct_PhieuThu.THOIGIANSUA = Utility.CurrentTime;
                    //@ConvertObjectUnicodeToTCVN3
                    var apiResponse = Utility.Edit<v_ct_PhieuThu>(Utility.LOC_ID + "/" + ct_PhieuThu.ID, ct_PhieuThu, API.ct_PhieuThu);
                    if (apiResponse.Success)
                        return RedirectToAction("Index");
                    else
                        ModelState.AddModelError(string.Empty, apiResponse.Message);
                }
                return View(ct_PhieuThu);
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
                if (!Utility.KiemTraQuyen(API.ct_PhieuThu, API.Delete))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                var apiResponse = Utility.Delete<v_ct_PhieuThu>(Utility.LOC_ID + "/" + id, API.ct_PhieuThu);
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
                if (!Utility.KiemTraQuyen(API.ct_PhieuGiaoHang, API.Delivery_CreateReceipt))
                {
                    if (!Utility.KiemTraQuyen(API.ct_PhieuThu, API.Create))
                    {
                        TempData["TitleError"] = API.TitlePermission;
                        apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                        return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                    }
                }

                var lstdm_LoaiPhieuThu = Utility.GetListData<v_dm_LoaiPhieuThu>(API.dm_LoaiPhieuThu, "", "", Utility.LOC_ID).Data as List<v_dm_LoaiPhieuThu>;
                var dm_LoaiPhieuThu = lstdm_LoaiPhieuThu.Where(e => e.ID == ID_LOAIPHIEU).FirstOrDefault();
                if (dm_LoaiPhieuThu == null || string.IsNullOrEmpty(dm_LoaiPhieuThu.ID))
                {
                    TempData["TitleError"] = "Không tìm thấy loại phiếu thu";
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                v_v_ct_PhieuThu ct_PhieuThu = new v_v_ct_PhieuThu();
                apiResponse.Success = true;
                ct_PhieuThu.ID_LOAIPHIEUTHU = ID_LOAIPHIEU;
                ct_PhieuThu.LOC_ID = Utility.LOC_ID;
                ct_PhieuThu.ID = Guid.NewGuid().ToString();
                ct_PhieuThu.NGAYLAP = Utility.CurrentTime;
                if (CHUNGTUKEMTHEO.StartsWith("PX-"))
                {
                    string NameController = API.ct_PhieuXuat;
                    var apiResponse_PX = GetValue<v_v_ct_PhieuXuat>(apiResponse, NameController, CHUNGTUKEMTHEO);

                    if (apiResponse_PX.Detail != null)
                    {
                        ct_PhieuThu.SOTIEN = (apiResponse_PX.Detail as v_v_ct_PhieuXuat).TONGTIEN;
                        ct_PhieuThu.NGAYLAP = (apiResponse_PX.Detail as v_v_ct_PhieuXuat).NGAYLAP;
                    }
                }
                if (CHUNGTUKEMTHEO.StartsWith("PGH-"))
                {
                    string NameController = API.ct_PhieuGiaoHang;
                    var apiResponse_PX = GetValue<v_v_ct_PhieuGiaoHang>(apiResponse, NameController, CHUNGTUKEMTHEO);

                    if (apiResponse_PX.Detail != null)
                    {
                        ct_PhieuThu.NGAYLAP = (apiResponse_PX.Detail as v_v_ct_PhieuGiaoHang).NGAYLAP;
                    }
                }


                ct_PhieuThu.SOPHIEU = Utility.GetMaxID<ct_PhieuThu>(ct_PhieuThu, Utility.LOC_ID, ct_PhieuThu.NGAYLAP.ToString("yyyy-MM-dd"));
                ct_PhieuThu.MAPHIEU = API.GetMaPhieu(API.ct_PhieuThu, ct_PhieuThu.NGAYLAP, ct_PhieuThu.SOPHIEU);

                ct_PhieuThu.CHUNGTUKEMTHEO = CHUNGTUKEMTHEO;
                ct_PhieuThu.lstdm_NhaCungCap = new List<ComboboxFrom>();
                if (dm_LoaiPhieuThu.TYPE == 1)
                {
                    ct_PhieuThu.lstdm_NhaCungCap = Utility.GetListData<ComboboxFrom>(API.dm_NhaCungCap, "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
                    ct_PhieuThu.ID_NHACUNGCAP = ID_KHACHAHANG;
                    apiResponse.TYPE = "divNCCAdd";
                    foreach (var itm in ct_PhieuThu.lstdm_NhaCungCap.Where(s => s.ISDEFAULT))
                    {
                        itm.ISDEFAULT = false;
                    }
                    var ISDEFAULT = ct_PhieuThu.lstdm_NhaCungCap.Where(s => s.ID == ct_PhieuThu.ID_NHANVIEN).FirstOrDefault();
                    if(ISDEFAULT != null)
                        ISDEFAULT.ISDEFAULT = true;
                }

                ct_PhieuThu.lstdm_KhachHang = new List<ComboboxFrom>();
                if (dm_LoaiPhieuThu.TYPE == 2)
                {
                    apiResponse.TYPE = "divKHACHHANGAdd";
                    ct_PhieuThu.lstdm_KhachHang = Utility.GetListData<ComboboxFrom>(API.dm_KhachHang, "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
                    ct_PhieuThu.ID_KHACHHANG = ID_KHACHAHANG;
                    foreach(var itm in ct_PhieuThu.lstdm_KhachHang.Where(s => s.ISDEFAULT))
                    {
                        itm.ISDEFAULT = false;
                    }
                    var ISDEFAULT = ct_PhieuThu.lstdm_KhachHang.Where(s => s.ID == ct_PhieuThu.ID_KHACHHANG).FirstOrDefault();
                    if (ISDEFAULT != null)
                        ISDEFAULT.ISDEFAULT = true;
                }

                ct_PhieuThu.lstdm_NhanVien = new List<ComboboxFrom>();
                if (dm_LoaiPhieuThu.TYPE == 3)
                {
                    apiResponse.TYPE = "divNHANVIENAdd";
                    ct_PhieuThu.lstdm_NhanVien = Utility.GetListData<ComboboxFrom>(API.dm_NhanVien, "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
                    ct_PhieuThu.ID_NHANVIEN = ID_KHACHAHANG;
                    foreach (var itm in ct_PhieuThu.lstdm_NhanVien.Where(s => s.ISDEFAULT))
                    {
                        itm.ISDEFAULT = false;
                    }
                    var ISDEFAULT = ct_PhieuThu.lstdm_NhanVien.Where(s => s.ID == ct_PhieuThu.ID_NHANVIEN).FirstOrDefault();
                    if (ISDEFAULT != null)
                        ISDEFAULT.ISDEFAULT = true;
                }
                ct_PhieuThu.lstdm_LoaiPhieuThu = new List<v_dm_LoaiPhieuThu>();
                ct_PhieuThu.lstdm_LoaiPhieuThu = Utility.GetListData<v_dm_LoaiPhieuThu>(API.dm_LoaiPhieuThu, "", "", Utility.LOC_ID).Data as List<v_dm_LoaiPhieuThu>;
                ct_PhieuThu.lstdm_TaiKhoanNganHang = new List<v_dm_TaiKhoanNganHang>();
                ct_PhieuThu.lstdm_TaiKhoanNganHang = Utility.GetListData<v_dm_TaiKhoanNganHang>(API.dm_TaiKhoanNganHang, "", "", Utility.LOC_ID).Data as List<v_dm_TaiKhoanNganHang>;
                var lst = Utility.ConvertobjectTo<v_v_ct_PhieuThu>(ct_PhieuThu);
                ValueEdit objValueEdit = new ValueEdit();
                objValueEdit.Key = "lblName";
                objValueEdit.Value = dm_LoaiPhieuThu.NAME.ToUpper();
                lst.Add(objValueEdit);
                apiResponse.Detail = lst;
                if(!string.IsNullOrEmpty(ID_KHACHAHANG) || !string.IsNullOrEmpty(CHUNGTUKEMTHEO))
                    apiResponse.NAME = "myModalAddReceipt";
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
        // POST: Menu/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult CreatePopup([Bind(Include = "ISCHUYENCONGNOCHONHANVIEN,LOC_ID,ID,ID_LOAIPHIEUTHU,NAME_LOAIPHIEUTHU,NGAYLAP,MAPHIEU,SOPHIEU,NAME_KHACHHANG_NCC_NHANVIEN,ID_KHACHHANG,ID_NHACUNGCAP,ID_NHANVIEN,NGUOINHANTIEN,TENNGUOINOPTIEN,DIACHI,SOTIEN,LYDO,CHUNGTUKEMTHEO,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO,ID_TAIKHOANNGANHANG,myModalAdd")] v_v_ct_PhieuThu ct_PhieuThu)
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                if (Utility.KiemTra())
                {
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Admin");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                if (!Utility.KiemTraQuyen(API.ct_PhieuGiaoHang, API.Delivery_CreateReceipt))
                {
                    if (!Utility.KiemTraQuyen(API.ct_PhieuThu, API.Create))
                    {
                        TempData["TitleError"] = API.TitlePermission;
                        apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                        return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                    }
                }
                if (ModelState.IsValid)
                {
                    ct_PhieuThu.NGAYLAP = ct_PhieuThu.NGAYLAP.AddHours(Utility.CurrentTime.Hour).AddMinutes(Utility.CurrentTime.Minute);
                    ct_PhieuThu.LOC_ID = Utility.LOC_ID;
                    ct_PhieuThu.ID_NGUOITAO = Session[Sessions.idUser].ToString();
                    ct_PhieuThu.THOIGIANTHEM = Utility.CurrentTime;
                   
                    apiResponse = Utility.Create<v_ct_PhieuThu>(ct_PhieuThu, API.ct_PhieuThu);
                    if (apiResponse.Success)
                    {
                        ct_PhieuThu.NGAYLAP = Utility.CurrentTime;
                        apiResponse.SOPHIEU = ct_PhieuThu.SOPHIEU = Utility.GetMaxID<ct_PhieuThu>(ct_PhieuThu, Utility.LOC_ID, ct_PhieuThu.NGAYLAP.ToString("yyyy-MM-dd"));
                        ct_PhieuThu.MAPHIEU = API.GetMaPhieu(API.ct_PhieuThu, ct_PhieuThu.NGAYLAP, ct_PhieuThu.SOPHIEU);
                        apiResponse.NewID = Guid.NewGuid().ToString();
                        apiResponse.MAPHIEU = ct_PhieuThu.MAPHIEU;

                        if (apiResponse.Data != null)
                            ct_PhieuThu = JsonConvert.DeserializeObject<v_v_ct_PhieuThu>(apiResponse.Data.ToString());
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, apiResponse.Message);
                        if (apiResponse.CheckValue)
                        {
                            ct_PhieuThu.NGAYLAP = Utility.CurrentTime;
                            apiResponse.SOPHIEU = ct_PhieuThu.SOPHIEU = Utility.GetMaxID<ct_PhieuThu>(ct_PhieuThu, Utility.LOC_ID, ct_PhieuThu.NGAYLAP.ToString("yyyy-MM-dd"));
                            ct_PhieuThu.MAPHIEU = API.GetMaPhieu(API.ct_PhieuThu, ct_PhieuThu.NGAYLAP, ct_PhieuThu.SOPHIEU);
                            apiResponse.NewID = Guid.NewGuid().ToString();
                            apiResponse.MAPHIEU = ct_PhieuThu.MAPHIEU;
                        }
                    }
                }
                else
                {
                    apiResponse.Success = false;
                    apiResponse.Data = Utility.GetModelState(ModelState, API.ct_PhieuThu);
                }
                apiResponse.ID = ct_PhieuThu.ID;
                var lstdm_LoaiPhieuThu = Utility.GetListData<v_dm_LoaiPhieuThu>(API.dm_LoaiPhieuThu, "", "", Utility.LOC_ID).Data as List<v_dm_LoaiPhieuThu>;
                var dm_LoaiPhieuThu = lstdm_LoaiPhieuThu.Where(e => e.ID == ct_PhieuThu.ID_LOAIPHIEUTHU).FirstOrDefault();
                if (dm_LoaiPhieuThu == null || string.IsNullOrEmpty(dm_LoaiPhieuThu.ID))
                {
                    TempData["TitleError"] = "Không tìm thấy loại phiếu thu";
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                ct_PhieuThu.lstdm_NhaCungCap = new List<ComboboxFrom>();
                if (dm_LoaiPhieuThu.TYPE == 1)
                {
                    ct_PhieuThu.lstdm_NhaCungCap = Utility.GetListData<ComboboxFrom>(API.dm_NhaCungCap, "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
                    apiResponse.TYPE = "divNCCAdd";
                }

                ct_PhieuThu.lstdm_KhachHang = new List<ComboboxFrom>();
                if (dm_LoaiPhieuThu.TYPE == 2)
                {
                    apiResponse.TYPE = "divKHACHHANGAdd";
                    ct_PhieuThu.lstdm_KhachHang = Utility.GetListData<ComboboxFrom>(API.dm_KhachHang, "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
                }
                ct_PhieuThu.lstdm_NhanVien = new List<ComboboxFrom>();
                if (dm_LoaiPhieuThu.TYPE == 3)
                {
                    apiResponse.TYPE = "divNHANVIENAdd";
                    ct_PhieuThu.lstdm_NhanVien = Utility.GetListData<ComboboxFrom>(API.dm_NhanVien, "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
                }
                
                ct_PhieuThu.lstdm_LoaiPhieuThu = new List<v_dm_LoaiPhieuThu>();
                ct_PhieuThu.lstdm_TaiKhoanNganHang = new List<v_dm_TaiKhoanNganHang>();
                ct_PhieuThu.lstdm_TaiKhoanNganHang = Utility.GetListData<v_dm_TaiKhoanNganHang>(API.dm_TaiKhoanNganHang, "", "", Utility.LOC_ID).Data as List<v_dm_TaiKhoanNganHang>;
                var lst = Utility.ConvertobjectToView<v_v_ct_PhieuThu>(ct_PhieuThu);
                apiResponse.Detail = lst;
                apiResponse.NAME = ct_PhieuThu.myModalAdd;
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
                if (!Utility.KiemTraQuyen(API.ct_PhieuThu, API.Edit))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }

                v_v_ct_PhieuThu ct_PhieuThu = new v_v_ct_PhieuThu();
                if (!string.IsNullOrEmpty(id))
                {
                    apiResponse = Utility.GetDetail<v_v_ct_PhieuThu>(Utility.LOC_ID + "/" + id, API.ct_PhieuThu);
                    if (!apiResponse.Success)
                    {
                        TempData["TitleError"] = apiResponse.Message;
                        apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                        return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                    }
                    if (apiResponse.Data != null)
                        ct_PhieuThu = apiResponse.Data as v_v_ct_PhieuThu;
                }
                ct_PhieuThu.lstdm_NhaCungCap = new List<ComboboxFrom>();
                apiResponse.Success = true;
                var lstdm_LoaiPhieuThu = Utility.GetListData<v_dm_LoaiPhieuThu>(API.dm_LoaiPhieuThu, "", "", Utility.LOC_ID).Data as List<v_dm_LoaiPhieuThu>;
                var dm_LoaiPhieuThu = lstdm_LoaiPhieuThu.Where(e => e.ID == ct_PhieuThu.ID_LOAIPHIEUTHU).FirstOrDefault();
                if (dm_LoaiPhieuThu == null || string.IsNullOrEmpty(dm_LoaiPhieuThu.ID))
                {
                    TempData["TitleError"] = "Không tìm thấy loại phiếu thu";
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                if (dm_LoaiPhieuThu.TYPE == 1)
                {
                    ct_PhieuThu.lstdm_NhaCungCap = Utility.GetListData<ComboboxFrom>(API.dm_NhaCungCap, "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
                    apiResponse.TYPE = "divNCCEdit";
                }

                ct_PhieuThu.lstdm_KhachHang = new List<ComboboxFrom>();
                if (dm_LoaiPhieuThu.TYPE == 2)
                {
                    apiResponse.TYPE = "divKHACHHANGEdit";
                    ct_PhieuThu.lstdm_KhachHang = Utility.GetListData<ComboboxFrom>(API.dm_KhachHang, "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
                }
                ct_PhieuThu.lstdm_NhanVien = new List<ComboboxFrom>();
                if (dm_LoaiPhieuThu.TYPE == 3)
                {
                    apiResponse.TYPE = "divNHANVIENEdit";
                    ct_PhieuThu.lstdm_NhanVien = Utility.GetListData<ComboboxFrom>(API.dm_NhanVien, "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
                }
                ct_PhieuThu.lstdm_LoaiPhieuThu = new List<v_dm_LoaiPhieuThu>();
                ct_PhieuThu.lstdm_LoaiPhieuThu = lstdm_LoaiPhieuThu;
                ct_PhieuThu.lstdm_TaiKhoanNganHang = new List<v_dm_TaiKhoanNganHang>();
                ct_PhieuThu.lstdm_TaiKhoanNganHang = Utility.GetListData<v_dm_TaiKhoanNganHang>(API.dm_TaiKhoanNganHang, "", "", Utility.LOC_ID).Data as List<v_dm_TaiKhoanNganHang>;
                var lst = Utility.ConvertobjectTo<v_v_ct_PhieuThu>(ct_PhieuThu);
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
        public ActionResult EditPopup([Bind(Include = "ISCHUYENCONGNOCHONHANVIEN,LOC_ID,ID,ID_LOAIPHIEUTHU,NAME_LOAIPHIEUTHU,NGAYLAP,MAPHIEU,SOPHIEU,NAME_KHACHHANG_NCC_NHANVIEN,ID_KHACHHANG,ID_NHACUNGCAP,ID_NHANVIEN,NGUOINHANTIEN,TENNGUOINOPTIEN,DIACHI,SOTIEN,LYDO,CHUNGTUKEMTHEO,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO,ID_TAIKHOANNGANHANG")] v_v_ct_PhieuThu ct_PhieuThu)
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                if (Utility.KiemTra())
                {
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Admin");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                if (!Utility.KiemTraQuyen(API.ct_PhieuThu, API.Edit))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }

                if (ModelState.IsValid)
                {
                    ct_PhieuThu.LOC_ID = Utility.LOC_ID;
                    ct_PhieuThu.ID_NGUOISUA = Session[Sessions.idUser].ToString();
                    ct_PhieuThu.THOIGIANSUA = Utility.CurrentTime;
                   
                    apiResponse = Utility.Edit<v_ct_PhieuThu>(Utility.LOC_ID + "/" + ct_PhieuThu.ID, ct_PhieuThu, API.ct_PhieuThu);
                    if (apiResponse.Success)
                    {
                        //return RedirectToAction("Index");
                        apiResponse.ID = ct_PhieuThu.ID;
                        if (apiResponse.Data != null)
                            ct_PhieuThu = JsonConvert.DeserializeObject<v_v_ct_PhieuThu>(apiResponse.Data.ToString());
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, apiResponse.Message);
                    }
                }
                else
                {
                    apiResponse.Success = false;
                    apiResponse.Data = Utility.GetModelState(ModelState, API.ct_PhieuThu);
                }
                ct_PhieuThu.lstdm_NhaCungCap = new List<ComboboxFrom>();
                var lstdm_LoaiPhieuThu = Utility.GetListData<v_dm_LoaiPhieuThu>(API.dm_LoaiPhieuThu, "", "", Utility.LOC_ID).Data as List<v_dm_LoaiPhieuThu>;
                var dm_LoaiPhieuThu = lstdm_LoaiPhieuThu.Where(e => e.ID == ct_PhieuThu.ID_LOAIPHIEUTHU).FirstOrDefault();
                if (dm_LoaiPhieuThu == null || string.IsNullOrEmpty(dm_LoaiPhieuThu.ID))
                {
                    TempData["TitleError"] = "Không tìm thấy loại phiếu thu";
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                if (dm_LoaiPhieuThu.TYPE == 1)
                {
                    ct_PhieuThu.lstdm_NhaCungCap = Utility.GetListData<ComboboxFrom>(API.dm_NhaCungCap, "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
                    apiResponse.TYPE = "divNCCEdit";
                }

                ct_PhieuThu.lstdm_KhachHang = new List<ComboboxFrom>();
                if (dm_LoaiPhieuThu.TYPE == 2)
                {
                    apiResponse.TYPE = "divKHACHHANGEdit";
                    ct_PhieuThu.lstdm_KhachHang = Utility.GetListData<ComboboxFrom>(API.dm_KhachHang, "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
                }

                ct_PhieuThu.lstdm_NhanVien = new List<ComboboxFrom>();
                if (dm_LoaiPhieuThu.TYPE == 3)
                {
                    apiResponse.TYPE = "divNHANVIENEdit";
                    ct_PhieuThu.lstdm_NhanVien = Utility.GetListData<ComboboxFrom>(API.dm_NhanVien, "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
                }

                ct_PhieuThu.lstdm_LoaiPhieuThu = new List<v_dm_LoaiPhieuThu>();
                ct_PhieuThu.lstdm_LoaiPhieuThu = lstdm_LoaiPhieuThu;
                ct_PhieuThu.lstdm_TaiKhoanNganHang = new List<v_dm_TaiKhoanNganHang>();
                ct_PhieuThu.lstdm_TaiKhoanNganHang = Utility.GetListData<v_dm_TaiKhoanNganHang>(API.dm_TaiKhoanNganHang, "", "", Utility.LOC_ID).Data as List<v_dm_TaiKhoanNganHang>;
                apiResponse.Detail = Utility.ConvertobjectToView<v_v_ct_PhieuThu>(ct_PhieuThu);
                var lst = Utility.ConvertobjectToView<v_v_ct_PhieuThu>(ct_PhieuThu);
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
                if (!Utility.KiemTraQuyen(API.ct_PhieuThu, API.Delete))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                apiResponse = Utility.Delete<v_ct_PhieuThu>(Utility.LOC_ID + "/" + id, API.ct_PhieuThu);
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

                byte[] BinaryData = System.Text.Encoding.UTF8.GetBytes("https://ironsoftware.com/csharp/barcode/");
                // WRITE QR with Binary Content
                String fullpath = Path.Combine(Server.MapPath("~" + API.PathProduct), "MyBinaryQR.png");
                String fullpathLogo = Path.Combine(Server.MapPath("~" + API.PathLogo), "logoTrangHiepPhat.jpg");


                v_ct_PhieuThu PhieuNhap = new v_ct_PhieuThu();
                SP_Parameter objParameter = new SP_Parameter();
                objParameter.ID_PHIEUTHU = ID;
                apiResponse = Utility.ExecuteStoredProc<v_ct_PhieuThu>(objParameter, API.Sp_Get_DanhSachPhieuThu);
                if (!apiResponse.Success)
                {
                    apiResponse.Success = false;
                    apiResponse.Message = apiResponse.Message;
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                if (apiResponse.Data != null)
                    PhieuNhap = (apiResponse.Data as List<v_ct_PhieuThu>).FirstOrDefault();

                var report = new ReportClass();
                SP_Parameter_Report objParameter_Report = new SP_Parameter_Report();
                objParameter_Report.LOC_ID = Utility.LOC_ID;
                objParameter_Report.ID_PHIEUTHU = ID;
                apiResponse = Utility.ExecuteStoredProc<DataTable>(objParameter_Report, API.Sp_Get_DanhSachPhieuThu);
                if (!apiResponse.Success)
                {
                    apiResponse.Success = false;
                    apiResponse.Message = apiResponse.Message;
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
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
                apiResponse.NAME = Utility.GetTitleFrom(API.ct_PhieuThu) + " - " + PhieuNhap.MAPHIEU;
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