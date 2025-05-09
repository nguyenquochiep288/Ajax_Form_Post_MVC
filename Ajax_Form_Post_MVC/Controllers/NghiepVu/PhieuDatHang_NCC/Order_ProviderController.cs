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
using System.Drawing;

namespace MVC_QuanLyTHP.Controllers
{
    public class Order_ProviderController : Controller
    {

        // GET: Input
        public ActionResult Index(int Page = 1, DateTime? FromDate = null, DateTime? ToDate = null, string SearchString = "", string MAPHIEU = "", string IDCODE = "")
        {
            try
            {
                if (Utility.KiemTra())
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (!Utility.KiemTraQuyen(API.ct_PhieuDatHangNCC, API.Xem))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
               // ShowSearchValue = Utility.GetShowSearchValue<ct_PhieuDatHangNCC>(ShowSearchValue);
                ApiResponse apiResponse = new ApiResponse();
                IPagedList<v_ct_PhieuDatHangNCC> lstpage = (new List<v_ct_PhieuDatHangNCC>()).ToList().ToPagedList(Page, Utility.GetPageSize());
                if (FromDate != null || !string.IsNullOrEmpty(IDCODE))
                {
                    if (!string.IsNullOrEmpty(IDCODE))
                    {
                        apiResponse = Utility.Get_DanhSachPhieuDatHangNCC<v_ct_PhieuDatHangNCC>("", null, null, MAPHIEU, IDCODE);
                    }
                    if (FromDate != null)
                    {
                        apiResponse = Utility.Get_DanhSachPhieuDatHangNCC<v_ct_PhieuDatHangNCC>("", FromDate, ToDate, SearchString);
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
                    lstpage = (apiResponse.Data as List<v_ct_PhieuDatHangNCC>).ToPagedList(Page, Utility.GetPageSize());
                }
                v_v_ct_PhieuDatHangNCC ct_PhieuDatHangNCC = new v_v_ct_PhieuDatHangNCC();
                ct_PhieuDatHangNCC.IPagedList = lstpage;
                ct_PhieuDatHangNCC.lstdm_NhaCungCap = new List<ComboboxFrom>();
                //ct_PhieuDatHangNCC.lstdm_NhaCungCap = Utility.GetListData<v_dm_NhaCungCap>(API.dm_NhaCungCap, "", "", Utility.LOC_ID).Data as List<v_dm_NhaCungCap>;
                
                ct_PhieuDatHangNCC.lstdm_Kho = new List<v_dm_Kho>();
                //ct_PhieuDatHangNCC.lstdm_Kho = Utility.GetListData<v_dm_Kho>(API.dm_Kho, "", "", Utility.LOC_ID).Data as List<v_dm_Kho>;
                ct_PhieuDatHangNCC.lstdm_LoaiPhieuNhap = new List<v_dm_LoaiPhieuNhap>();
                var lstdm_LoaiPhieuNhap = Utility.GetListData<v_dm_LoaiPhieuNhap>(API.dm_LoaiPhieuNhap, "", "", Utility.LOC_ID).Data as List<v_dm_LoaiPhieuNhap>;
                if(lstdm_LoaiPhieuNhap != null)
                {
                    ct_PhieuDatHangNCC.lstdm_LoaiPhieuNhap = lstdm_LoaiPhieuNhap.Where(e => e.ISACTIVE == true && e.TYPE == 1).OrderBy(e => e.TYPE).ToList();
                }
                else
                {
                    ct_PhieuDatHangNCC.lstdm_LoaiPhieuNhap = new List<v_dm_LoaiPhieuNhap>();
                }
               
                ct_PhieuDatHangNCC.lstdm_NhanVien = new List<ComboboxFrom>();
                ct_PhieuDatHangNCC.lstdm_NhanVien = Utility.GetListData<ComboboxFrom>(API.dm_NhanVien, "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;


                ViewBag.searchValue = SearchString;

                ViewBag.PermissionEdit = Utility.KiemTraQuyen(API.ct_PhieuDatHangNCC, API.Edit);
                ViewBag.PermissionDelete = Utility.KiemTraQuyen(API.ct_PhieuDatHangNCC, API.Delete);
                ViewBag.PermissionCreate = Utility.KiemTraQuyen(API.ct_PhieuDatHangNCC, API.Create);
                ViewBag.PermissionCreateInput = Utility.KiemTraQuyen(API.ct_PhieuDatHangNCC, API.CreateInput);
                ViewBag.fromdate = FromDate != null ? FromDate.Value.ToString("yyyy-MM-dd") : Utility.CurrentTime.AddMonths(-1).ToString("yyyy-MM-dd");
                ViewBag.todate = ToDate != null ? ToDate.Value.ToString("yyyy-MM-dd") : Utility.CurrentTime.ToString("yyyy-MM-dd");
                return View(ct_PhieuDatHangNCC);
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
                if (!Utility.KiemTraQuyen(API.ct_PhieuDatHangNCC, API.Create))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                v_v_ct_PhieuDatHangNCC ct_PhieuDatHangNCC = new v_v_ct_PhieuDatHangNCC();
                ct_PhieuDatHangNCC.LOC_ID = Utility.LOC_ID;
                ct_PhieuDatHangNCC.ID_NGUOITAO = Session[Sessions.idUser].ToString();
                ct_PhieuDatHangNCC.THOIGIANTHEM = Utility.CurrentTime;
                ct_PhieuDatHangNCC.NGAYLAP = Utility.CurrentTime;
                ct_PhieuDatHangNCC.SOPHIEU = Utility.GetMaxID<ct_PhieuDatHangNCC>(ct_PhieuDatHangNCC, Utility.LOC_ID, ct_PhieuDatHangNCC.NGAYLAP.ToString("yyyy-MM-dd"));
                ct_PhieuDatHangNCC.MAPHIEU = API.GetMaPhieu(API.ct_PhieuDatHangNCC, ct_PhieuDatHangNCC.NGAYLAP, ct_PhieuDatHangNCC.SOPHIEU);
                

                ct_PhieuDatHangNCC.ID = Guid.NewGuid().ToString();
                ct_PhieuDatHangNCC.lstdm_NhaCungCap = new List<ComboboxFrom>();
                //ct_PhieuDatHangNCC.lstdm_NhaCungCap = Utility.GetListData<v_dm_NhaCungCap>(API.dm_NhaCungCap, "", "", Utility.LOC_ID).Data as List<v_dm_NhaCungCap>;
                ct_PhieuDatHangNCC.lstdm_Kho = new List<v_dm_Kho>();
                //ct_PhieuDatHangNCC.lstdm_Kho = Utility.GetListData<v_dm_Kho>(API.dm_Kho, "", "", Utility.LOC_ID).Data as List<v_dm_Kho>;
                ct_PhieuDatHangNCC.lstdm_LoaiPhieuNhap = new List<v_dm_LoaiPhieuNhap>();
                //ct_PhieuDatHangNCC.lstdm_LoaiPhieuNhap = Utility.GetListData<v_dm_LoaiPhieuNhap>(API.dm_LoaiPhieuNhap, "", "", Utility.LOC_ID).Data as List<v_dm_LoaiPhieuNhap>;
                ct_PhieuDatHangNCC.lstdm_NhanVien = new List<ComboboxFrom>();
                //ct_PhieuDatHangNCC.lstdm_NhanVien = Utility.GetListData<v_dm_NhanVien>(API.dm_NhanVien, "", "", Utility.LOC_ID).Data as List<v_dm_NhanVien>;
                ViewBag.myModalAdd = myModalAdd;
                ct_PhieuDatHangNCC.myModalAdd = myModalAdd;
                return View(ct_PhieuDatHangNCC);
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
        public ActionResult Create([Bind(Include = "LOC_ID,ID,ID_LOAIPHIEUNHAP,ID_KHO,MAPHIEU,SOPHIEU,NGAYLAP,ID_NHACUNGCAP,ID_KHACHHANG,ID_NHANVIEN,GHICHU,TONGTHANHTIEN,CHIETKHAU,TONGTIENGIAMGIA,TONGTIENVAT,TONGTIEN,CHUNGTUKEMTHEO")] v_ct_PhieuDatHangNCC ct_PhieuDatHangNCC)
        {
            try
            {
                if (Utility.KiemTra())
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (!Utility.KiemTraQuyen(API.ct_PhieuDatHangNCC, API.Create))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                if (ModelState.IsValid)
                {
                    ct_PhieuDatHangNCC.LOC_ID = Utility.LOC_ID;
                    ct_PhieuDatHangNCC.ID_NGUOITAO = Session[Sessions.idUser].ToString();
                    ct_PhieuDatHangNCC.THOIGIANTHEM = Utility.CurrentTime;

                    //@ConvertObjectUnicodeToTCVN3
                    var apiResponse = Utility.Create<ct_PhieuDatHangNCC>(ct_PhieuDatHangNCC, API.ct_PhieuDatHangNCC);
                    if (apiResponse.Success)
                        return RedirectToAction("Index");
                    else
                    {
                        ModelState.AddModelError(string.Empty, apiResponse.Message);
                        if (apiResponse.CheckValue)
                            ViewBag.ID = Guid.NewGuid().ToString();
                    }
                }
                return View(ct_PhieuDatHangNCC);
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
                if (!Utility.KiemTraQuyen(API.ct_PhieuDatHangNCC, API.Edit))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                v_v_ct_PhieuDatHangNCC ct_PhieuDatHangNCC = new v_v_ct_PhieuDatHangNCC();
                if (!string.IsNullOrEmpty(id))
                {
                    apiResponse = Utility.GetDetail<v_ct_PhieuDatHangNCC>(Utility.LOC_ID + "/" + id, API.ct_PhieuDatHangNCC);
                    if (!apiResponse.Success)
                    {
                        TempData["TitleError"] = apiResponse.Message;
                        return RedirectToAction("Index", "Notfound");
                    }
                    if (apiResponse.Data != null)
                        ct_PhieuDatHangNCC = apiResponse.Data as v_v_ct_PhieuDatHangNCC;
                }
                //@ConvertObjectTCVN3ToUnicode
                
                //ct_PhieuDatHangNCC.lstdm_KhachHang = Utility.GetListData<v_dm_KhachHang>(API.dm_KhachHang, "", "", Utility.LOC_ID).Data as List<v_dm_KhachHang>;
                ct_PhieuDatHangNCC.lstdm_NhaCungCap = new List<ComboboxFrom>();
                //ct_PhieuDatHangNCC.lstdm_NhaCungCap = Utility.GetListData<v_dm_NhaCungCap>(API.dm_NhaCungCap, "", "", Utility.LOC_ID).Data as List<v_dm_NhaCungCap>;
                ct_PhieuDatHangNCC.lstdm_Kho = new List<v_dm_Kho>();
                //ct_PhieuDatHangNCC.lstdm_Kho = Utility.GetListData<v_dm_Kho>(API.dm_Kho, "", "", Utility.LOC_ID).Data as List<v_dm_Kho>;
                ct_PhieuDatHangNCC.lstdm_LoaiPhieuNhap = new List<v_dm_LoaiPhieuNhap>();
                //ct_PhieuDatHangNCC.lstdm_LoaiPhieuNhap = Utility.GetListData<v_dm_LoaiPhieuNhap>(API.dm_LoaiPhieuNhap, "", "", Utility.LOC_ID).Data as List<v_dm_LoaiPhieuNhap>;
                ct_PhieuDatHangNCC.lstdm_NhanVien = new List<ComboboxFrom>();
                //ct_PhieuDatHangNCC.lstdm_NhanVien = Utility.GetListData<v_dm_NhanVien>(API.dm_NhanVien, "", "", Utility.LOC_ID).Data as List<v_dm_NhanVien>;

                return View(ct_PhieuDatHangNCC);
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
        public ActionResult Edit([Bind(Include = "LOC_ID,ID,ID_LOAIPHIEUNHAP,ID_KHO,MAPHIEU,SOPHIEU,NGAYLAP,ID_NHACUNGCAP,ID_KHACHHANG,ID_NHANVIEN,GHICHU,TONGTHANHTIEN,CHIETKHAU,TONGTIENGIAMGIA,TONGTIENVAT,TONGTIEN,CHUNGTUKEMTHEO")] v_ct_PhieuDatHangNCC ct_PhieuDatHangNCC)
        {
            try
            {
                if (Utility.KiemTra())
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (!Utility.KiemTraQuyen(API.ct_PhieuDatHangNCC, API.Edit))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                if (ModelState.IsValid)
                {
                    ct_PhieuDatHangNCC.LOC_ID = Utility.LOC_ID;
                    ct_PhieuDatHangNCC.ID_NGUOISUA = Session[Sessions.idUser].ToString();
                    ct_PhieuDatHangNCC.THOIGIANSUA = Utility.CurrentTime;
                    //@ConvertObjectUnicodeToTCVN3
                    var apiResponse = Utility.Edit<v_ct_PhieuDatHangNCC>(Utility.LOC_ID + "/" + ct_PhieuDatHangNCC.ID, ct_PhieuDatHangNCC, API.ct_PhieuDatHangNCC);
                    if (apiResponse.Success)
                        return RedirectToAction("Index");
                    else
                        ModelState.AddModelError(string.Empty, apiResponse.Message);
                }
                return View(ct_PhieuDatHangNCC);
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
                if (!Utility.KiemTraQuyen(API.ct_PhieuDatHangNCC, API.Delete))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                var apiResponse = Utility.Delete<v_ct_PhieuDatHangNCC>(Utility.LOC_ID + "/" + id, API.ct_PhieuDatHangNCC);
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
        public ActionResult CreatePopup(string ID_LOAIPHIEU, string ID_KHACHAHANG = "", string CHUNGTUKEMTHEO = "")
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                if (Utility.KiemTra())
                {
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Admin");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                if (!Utility.KiemTraQuyen(API.ct_PhieuDatHangNCC, API.Create))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }

                var lstdm_LoaiPhieuNhap = Utility.GetListData<v_dm_LoaiPhieuNhap>(API.dm_LoaiPhieuNhap, "", "", Utility.LOC_ID).Data as List<v_dm_LoaiPhieuNhap>;
                var dm_LoaiPhieuNhap = lstdm_LoaiPhieuNhap.Where(e => e.ID == ID_LOAIPHIEU).FirstOrDefault();
                if(dm_LoaiPhieuNhap == null || string.IsNullOrEmpty(dm_LoaiPhieuNhap.ID))
                {
                    TempData["TitleError"] = "Không tìm thấy loại phiếu nhập";
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }    
                 v_v_ct_PhieuDatHangNCC ct_PhieuDatHangNCC = new v_v_ct_PhieuDatHangNCC();
                apiResponse.Success = true;
                ct_PhieuDatHangNCC.ID_LOAIPHIEUNHAP = ID_LOAIPHIEU;
                ct_PhieuDatHangNCC.LOC_ID = Utility.LOC_ID;
                ct_PhieuDatHangNCC.ID = Guid.NewGuid().ToString();
                ct_PhieuDatHangNCC.NGAYLAP = Utility.CurrentTime;
                ct_PhieuDatHangNCC.SOPHIEU = Utility.GetMaxID<ct_PhieuDatHangNCC>(ct_PhieuDatHangNCC, Utility.LOC_ID, ct_PhieuDatHangNCC.NGAYLAP.ToString("yyyy-MM-dd"));
                ct_PhieuDatHangNCC.MAPHIEU = API.GetMaPhieu(API.ct_PhieuDatHangNCC, ct_PhieuDatHangNCC.NGAYLAP, ct_PhieuDatHangNCC.SOPHIEU);
                ct_PhieuDatHangNCC.CHUNGTUKEMTHEO = CHUNGTUKEMTHEO;
                ct_PhieuDatHangNCC.lstdm_NhaCungCap = new List<ComboboxFrom>();
                if(dm_LoaiPhieuNhap.TYPE == 1)
                {
                    ct_PhieuDatHangNCC.lstdm_NhaCungCap = Utility.GetListData<ComboboxFrom>(API.dm_NhaCungCap, "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
                    ct_PhieuDatHangNCC.ID_NHACUNGCAP = ID_KHACHAHANG;
                    apiResponse.TYPE = "divNCCAdd";
                }    
                 
                  
                ct_PhieuDatHangNCC.lstdm_Kho = new List<v_dm_Kho>();
                ct_PhieuDatHangNCC.lstdm_Kho = Utility.GetListData<v_dm_Kho>(API.dm_Kho, "", "", Utility.LOC_ID).Data as List<v_dm_Kho>;
                ct_PhieuDatHangNCC.lstdm_LoaiPhieuNhap = new List<v_dm_LoaiPhieuNhap>();
                ct_PhieuDatHangNCC.lstdm_LoaiPhieuNhap = Utility.GetListData<v_dm_LoaiPhieuNhap>(API.dm_LoaiPhieuNhap, "", "", Utility.LOC_ID).Data as List<v_dm_LoaiPhieuNhap>;
                ct_PhieuDatHangNCC.lstdm_NhanVien = new List<ComboboxFrom>();
                ct_PhieuDatHangNCC.lstdm_NhanVien = Utility.GetListData<ComboboxFrom>(API.dm_NhanVien, "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
                Session[Sessions.lstProductInput] = new List<Product_Detail>();
               
                var lst = Utility.ConvertobjectTo<v_v_ct_PhieuDatHangNCC>(ct_PhieuDatHangNCC);
                apiResponse.ProductCombo = Utility.GetProductInputOutput(new List<Product_Detail>(), "InputOutput");
                lst.Add(new ValueEdit { Key = "tbodyTempItemInput", Value = apiResponse.ProductCombo });
                lst.Add(new ValueEdit { Key = "lblName", Value = dm_LoaiPhieuNhap.NAME.ToUpper() });
                apiResponse.Detail = lst;
                if (!string.IsNullOrEmpty(ID_KHACHAHANG))
                    apiResponse.NAME = "myModalAddInput";
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

        // POST: Menu/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult CreatePopup([Bind(Include = "LOC_ID,ID,ID_LOAIPHIEUNHAP,ID_KHO,MAPHIEU,SOPHIEU,NGAYLAP,ID_NHACUNGCAP,ID_KHACHHANG,ID_NHANVIEN,GHICHU,TONGTHANHTIEN,CHIETKHAU,TONGTIENGIAMGIA,TONGTIENVAT,TONGTIEN,CHUNGTUKEMTHEO,myModalAdd")] v_v_ct_PhieuDatHangNCC ct_PhieuDatHangNCC)
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                if (Utility.KiemTra())
                {
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Admin");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                if (!Utility.KiemTraQuyen(API.ct_PhieuDatHangNCC, API.Create))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                var lstKey = Request.Form.AllKeys.Where(e => e.StartsWith("txt"));
                if (lstKey == null || lstKey.Count() == 0)
                {
                    ModelState.AddModelError("lstct_PhieuDatHangNCC_ChiTiet", "Thêm danh sách hàng hóa.");
                }

                if (ModelState.IsValid)
                {
                    if(ct_PhieuDatHangNCC.MAPHIEU.Contains("PN-"))
                    {
                        v_v_ct_PhieuNhap ct_PhieuNhap = new v_v_ct_PhieuNhap();
                        string strInput = JsonConvert.SerializeObject(ct_PhieuDatHangNCC);
                        ct_PhieuNhap = JsonConvert.DeserializeObject<v_v_ct_PhieuNhap>(strInput) ?? new v_v_ct_PhieuNhap();
                        ct_PhieuNhap.NGAYLAP = ct_PhieuNhap.NGAYLAP.AddHours(Utility.CurrentTime.Hour).AddMinutes(Utility.CurrentTime.Minute);
                        ct_PhieuNhap.LOC_ID = Utility.LOC_ID;
                        ct_PhieuNhap.ID_NGUOITAO = Session[Sessions.idUser].ToString();
                        ct_PhieuNhap.THOIGIANTHEM = Utility.CurrentTime;
                        ct_PhieuNhap.lstct_PhieuNhap_ChiTiet = new List<v_ct_PhieuNhap_ChiTiet>();
                        v_ct_PhieuNhap_ChiTiet ct_PhieuNhap_ChiTiet = new v_ct_PhieuNhap_ChiTiet();

                        foreach (string Key in lstKey)
                        {
                            var lstString = Key.ToString().Split('|');
                            var value = HttpContext.Request.Params.GetValues(Key.ToString());
                            string ShowSearchValue = clsMaHoa.Decrypt(lstString[1].ToString(), clsMaHoa.PassMaHoa);
                            var Checkct_PhieuNhap_ChiTiet = JsonConvert.DeserializeObject<v_ct_PhieuNhap_ChiTiet>(ShowSearchValue);
                            if (ct_PhieuNhap_ChiTiet.ID != Checkct_PhieuNhap_ChiTiet.ID)
                            {
                                ct_PhieuNhap_ChiTiet = new v_ct_PhieuNhap_ChiTiet();
                                ct_PhieuNhap_ChiTiet = JsonConvert.DeserializeObject<v_ct_PhieuNhap_ChiTiet>(ShowSearchValue);
                                ct_PhieuNhap_ChiTiet.LOC_ID = ct_PhieuNhap.LOC_ID;
                                ct_PhieuNhap.lstct_PhieuNhap_ChiTiet.Add(ct_PhieuNhap_ChiTiet);
                            }
                            Utility.EditObject(ct_PhieuNhap_ChiTiet, lstString[0].ToString().Substring(3, lstString[0].ToString().Length - 3), value[0]);
                        }

                        apiResponse = Utility.Create<v_ct_PhieuNhap>(ct_PhieuNhap, API.ct_PhieuNhap);
                        if (apiResponse.Success)
                        {
                            //ct_PhieuDatHangNCC.NGAYLAP = Utility.CurrentTime;
                            //apiResponse.SOPHIEU = ct_PhieuNhap.SOPHIEU = Utility.GetMaxID<ct_PhieuNhap>(ct_PhieuNhap, Utility.LOC_ID, ct_PhieuNhap.NGAYLAP.ToString("yyyy-MM-dd"));
                            //ct_PhieuDatHangNCC.MAPHIEU = API.GetMaPhieu(API.ct_PhieuNhap, ct_PhieuNhap.NGAYLAP, ct_PhieuNhap.SOPHIEU);
                            //apiResponse.NewID = Guid.NewGuid().ToString();
                            //apiResponse.MAPHIEU = ct_PhieuNhap.MAPHIEU;

                            if (apiResponse.Data != null)
                                ct_PhieuDatHangNCC = JsonConvert.DeserializeObject<v_v_ct_PhieuDatHangNCC>(apiResponse.Data.ToString());
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, apiResponse.Message);
                            if (apiResponse.CheckValue)
                            {
                                ct_PhieuNhap.NGAYLAP = ct_PhieuDatHangNCC.NGAYLAP = Utility.CurrentTime;
                                apiResponse.SOPHIEU = ct_PhieuNhap.SOPHIEU = Utility.GetMaxID<ct_PhieuNhap>(ct_PhieuNhap, Utility.LOC_ID, ct_PhieuNhap.NGAYLAP.ToString("yyyy-MM-dd"));
                                ct_PhieuNhap.MAPHIEU = ct_PhieuDatHangNCC.MAPHIEU = API.GetMaPhieu(API.ct_PhieuNhap, ct_PhieuNhap.NGAYLAP, ct_PhieuNhap.SOPHIEU);
                                apiResponse.NewID = Guid.NewGuid().ToString();
                                apiResponse.MAPHIEU = ct_PhieuNhap.MAPHIEU;
                            }
                        }
                    }
                    else
                    {
                        ct_PhieuDatHangNCC.NGAYLAP = ct_PhieuDatHangNCC.NGAYLAP.AddHours(Utility.CurrentTime.Hour).AddMinutes(Utility.CurrentTime.Minute);
                        ct_PhieuDatHangNCC.LOC_ID = Utility.LOC_ID;
                        ct_PhieuDatHangNCC.ID_NGUOITAO = Session[Sessions.idUser].ToString();
                        ct_PhieuDatHangNCC.THOIGIANTHEM = Utility.CurrentTime;
                        ct_PhieuDatHangNCC.lstct_PhieuNhap_ChiTiet = new List<v_ct_PhieuDatHangNCC_ChiTiet>();
                        v_ct_PhieuDatHangNCC_ChiTiet ct_PhieuDatHangNCC_ChiTiet = new v_ct_PhieuDatHangNCC_ChiTiet();

                        foreach (string Key in lstKey)
                        {
                            var lstString = Key.ToString().Split('|');
                            var value = HttpContext.Request.Params.GetValues(Key.ToString());
                            string ShowSearchValue = clsMaHoa.Decrypt(lstString[1].ToString(), clsMaHoa.PassMaHoa);
                            var Checkct_PhieuDatHangNCC_ChiTiet = JsonConvert.DeserializeObject<v_ct_PhieuDatHangNCC_ChiTiet>(ShowSearchValue);
                            if (ct_PhieuDatHangNCC_ChiTiet.ID != Checkct_PhieuDatHangNCC_ChiTiet.ID)
                            {
                                ct_PhieuDatHangNCC_ChiTiet = new v_ct_PhieuDatHangNCC_ChiTiet();
                                ct_PhieuDatHangNCC_ChiTiet = JsonConvert.DeserializeObject<v_ct_PhieuDatHangNCC_ChiTiet>(ShowSearchValue);
                                ct_PhieuDatHangNCC_ChiTiet.LOC_ID = ct_PhieuDatHangNCC.LOC_ID;
                                ct_PhieuDatHangNCC.lstct_PhieuNhap_ChiTiet.Add(ct_PhieuDatHangNCC_ChiTiet);
                            }
                            Utility.EditObject(ct_PhieuDatHangNCC_ChiTiet, lstString[0].ToString().Substring(3, lstString[0].ToString().Length - 3), value[0]);
                        }

                        apiResponse = Utility.Create<v_ct_PhieuDatHangNCC>(ct_PhieuDatHangNCC, API.ct_PhieuDatHangNCC);
                        if (apiResponse.Success)
                        {
                            ct_PhieuDatHangNCC.NGAYLAP = Utility.CurrentTime;
                            apiResponse.SOPHIEU = ct_PhieuDatHangNCC.SOPHIEU = Utility.GetMaxID<ct_PhieuDatHangNCC>(ct_PhieuDatHangNCC, Utility.LOC_ID, ct_PhieuDatHangNCC.NGAYLAP.ToString("yyyy-MM-dd"));
                            ct_PhieuDatHangNCC.MAPHIEU = API.GetMaPhieu(API.ct_PhieuDatHangNCC, ct_PhieuDatHangNCC.NGAYLAP, ct_PhieuDatHangNCC.SOPHIEU);
                            apiResponse.NewID = Guid.NewGuid().ToString();
                            apiResponse.MAPHIEU = ct_PhieuDatHangNCC.MAPHIEU;

                            if (apiResponse.Data != null)
                                ct_PhieuDatHangNCC = JsonConvert.DeserializeObject<v_v_ct_PhieuDatHangNCC>(apiResponse.Data.ToString());
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, apiResponse.Message);
                            if (apiResponse.CheckValue)
                            {
                                ct_PhieuDatHangNCC.NGAYLAP = Utility.CurrentTime;
                                apiResponse.SOPHIEU = ct_PhieuDatHangNCC.SOPHIEU = Utility.GetMaxID<ct_PhieuDatHangNCC>(ct_PhieuDatHangNCC, Utility.LOC_ID, ct_PhieuDatHangNCC.NGAYLAP.ToString("yyyy-MM-dd"));
                                ct_PhieuDatHangNCC.MAPHIEU = API.GetMaPhieu(API.ct_PhieuDatHangNCC, ct_PhieuDatHangNCC.NGAYLAP, ct_PhieuDatHangNCC.SOPHIEU);
                                apiResponse.NewID = Guid.NewGuid().ToString();
                                apiResponse.MAPHIEU = ct_PhieuDatHangNCC.MAPHIEU;
                            }
                        }
                    }
                    
                }
                else
                {
                    apiResponse.Success = false;
                    apiResponse.Data = Utility.GetModelState(ModelState, API.ct_PhieuDatHangNCC);
                }
                apiResponse.ID = ct_PhieuDatHangNCC.ID;
                var lstdm_LoaiPhieuNhap = Utility.GetListData<v_dm_LoaiPhieuNhap>(API.dm_LoaiPhieuNhap, "", "", Utility.LOC_ID).Data as List<v_dm_LoaiPhieuNhap>;
                var dm_LoaiPhieuNhap = lstdm_LoaiPhieuNhap.Where(e => e.ID == ct_PhieuDatHangNCC.ID_LOAIPHIEUNHAP).FirstOrDefault();
                if (dm_LoaiPhieuNhap == null || string.IsNullOrEmpty(dm_LoaiPhieuNhap.ID))
                {
                    TempData["TitleError"] = "Không tìm thấy loại phiếu nhập";
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                ct_PhieuDatHangNCC.lstdm_NhaCungCap = new List<ComboboxFrom>();
                if (dm_LoaiPhieuNhap.TYPE == 1)
                {
                    ct_PhieuDatHangNCC.lstdm_NhaCungCap = Utility.GetListData<ComboboxFrom>(API.dm_NhaCungCap, "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
                    apiResponse.TYPE = "divNCCAdd";
                }
               
                //ct_PhieuDatHangNCC.lstdm_NhaCungCap = new List<v_dm_NhaCungCap>();
                //ct_PhieuDatHangNCC.lstdm_NhaCungCap = Utility.GetListData<v_dm_NhaCungCap>(API.dm_NhaCungCap, "", "", Utility.LOC_ID).Data as List<v_dm_NhaCungCap>;
                ct_PhieuDatHangNCC.lstdm_Kho = new List<v_dm_Kho>();
                ct_PhieuDatHangNCC.lstdm_Kho = Utility.GetListData<v_dm_Kho>(API.dm_Kho, "", "", Utility.LOC_ID).Data as List<v_dm_Kho>;
                ct_PhieuDatHangNCC.lstdm_LoaiPhieuNhap = new List<v_dm_LoaiPhieuNhap>();
                //ct_PhieuDatHangNCC.lstdm_LoaiPhieuNhap = lstdm_LoaiPhieuNhap;
                ct_PhieuDatHangNCC.lstdm_NhanVien = new List<ComboboxFrom>();
                ct_PhieuDatHangNCC.lstdm_NhanVien = Utility.GetListData<ComboboxFrom>(API.dm_NhanVien, "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
                var lst = Utility.ConvertobjectToView<v_v_ct_PhieuDatHangNCC>(ct_PhieuDatHangNCC);
                apiResponse.ProductCombo = Utility.GetProductInputOutput(new List<Product_Detail>(), "InputOutput");
                lst.Add(new ValueEdit { Key = "tbodyTempItemInput", Value = apiResponse.ProductCombo });
                apiResponse.Detail = lst;
                apiResponse.NAME = ct_PhieuDatHangNCC.myModalAdd;
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
                if (!Utility.KiemTraQuyen(API.ct_PhieuDatHangNCC, API.Edit))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
               
                v_v_ct_PhieuDatHangNCC ct_PhieuDatHangNCC = new v_v_ct_PhieuDatHangNCC();
                if (!string.IsNullOrEmpty(id))
                {
                    apiResponse = Utility.GetDetail<v_v_ct_PhieuDatHangNCC>(Utility.LOC_ID + "/" + id, API.ct_PhieuDatHangNCC);
                    if (!apiResponse.Success)
                    {
                        TempData["TitleError"] = apiResponse.Message;
                        apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                        return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                    }
                    if (apiResponse.Data != null)
                        ct_PhieuDatHangNCC = apiResponse.Data as v_v_ct_PhieuDatHangNCC;
                }
                if(ct_PhieuDatHangNCC.ISHOANTAT)
                {
                    TempData["TitleError"] = "Phiếu " + ct_PhieuDatHangNCC.MAPHIEU + " đã hoàn thành! Vui lòng kiểm tra lại!";
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                ct_PhieuDatHangNCC.lstdm_NhaCungCap = new List<ComboboxFrom>();
                apiResponse.Success = true;
                var lstdm_LoaiPhieuNhap = Utility.GetListData<v_dm_LoaiPhieuNhap>(API.dm_LoaiPhieuNhap, "", "", Utility.LOC_ID).Data as List<v_dm_LoaiPhieuNhap>;
                var dm_LoaiPhieuNhap = lstdm_LoaiPhieuNhap.Where(e => e.ID == ct_PhieuDatHangNCC.ID_LOAIPHIEUNHAP).FirstOrDefault();
                if (dm_LoaiPhieuNhap == null || string.IsNullOrEmpty(dm_LoaiPhieuNhap.ID))
                {
                    TempData["TitleError"] = "Không tìm thấy loại phiếu nhập";
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                if (dm_LoaiPhieuNhap.TYPE == 1)
                {
                    ct_PhieuDatHangNCC.lstdm_NhaCungCap = Utility.GetListData<ComboboxFrom>(API.dm_NhaCungCap, "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
                    apiResponse.TYPE = "divNCCEdit";
                }
              
                ct_PhieuDatHangNCC.lstdm_Kho = new List<v_dm_Kho>();
                ct_PhieuDatHangNCC.lstdm_Kho = Utility.GetListData<v_dm_Kho>(API.dm_Kho, "", "", Utility.LOC_ID).Data as List<v_dm_Kho>;
                ct_PhieuDatHangNCC.lstdm_LoaiPhieuNhap = new List<v_dm_LoaiPhieuNhap>();
                ct_PhieuDatHangNCC.lstdm_LoaiPhieuNhap = lstdm_LoaiPhieuNhap;
                ct_PhieuDatHangNCC.lstdm_NhanVien = new List<ComboboxFrom>();
                ct_PhieuDatHangNCC.lstdm_NhanVien = Utility.GetListData<ComboboxFrom>(API.dm_NhanVien, "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
                List<Product_Detail> lstProduct_Detail = new List<Product_Detail>();
                foreach (var itm in ct_PhieuDatHangNCC.lstct_PhieuNhap_ChiTiet)
                {
                    lstProduct_Detail.Add(Utility.ConvertobjectToProduct_Detail<v_ct_PhieuDatHangNCC_ChiTiet>(itm, new Product_Detail()));
                }
                Session[Sessions.lstProductInput] = lstProduct_Detail;
                var lst = Utility.ConvertobjectTo<v_v_ct_PhieuDatHangNCC>(ct_PhieuDatHangNCC);
                apiResponse.ProductCombo = Utility.GetProductInputOutput(lstProduct_Detail, "InputOutput", false , ct_PhieuDatHangNCC.TONGTIENGIAMGIA, ct_PhieuDatHangNCC.TONGTHANHTIEN, ct_PhieuDatHangNCC.TONGTIENVAT, ct_PhieuDatHangNCC.TONGTIEN);
                lst.Add(new ValueEdit { Key = "tbodyTempItemInputEdit", Value = apiResponse.ProductCombo });
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
        public ActionResult EditPopup([Bind(Include = "LOC_ID,ID,ID_LOAIPHIEUNHAP,ID_KHO,MAPHIEU,SOPHIEU,NGAYLAP,ID_NHACUNGCAP,ID_KHACHHANG,ID_NHANVIEN,GHICHU,TONGTHANHTIEN,CHIETKHAU,TONGTIENGIAMGIA,TONGTIENVAT,TONGTIEN,CHUNGTUKEMTHEO")] v_v_ct_PhieuDatHangNCC ct_PhieuDatHangNCC)
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                if (Utility.KiemTra())
                {
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Admin");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                if (!Utility.KiemTraQuyen(API.ct_PhieuDatHangNCC, API.Edit))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }

                var lstKey = Request.Form.AllKeys.Where(e => e.StartsWith("txt"));
                if (lstKey == null || lstKey.Count() == 0)
                {
                    ModelState.AddModelError("lstct_PhieuDatHangNCC_ChiTiet", "Thêm danh sách hàng hóa.");
                }
                if (ModelState.IsValid)
                {
                    ct_PhieuDatHangNCC.LOC_ID = Utility.LOC_ID;
                    ct_PhieuDatHangNCC.ID_NGUOISUA = Session[Sessions.idUser].ToString();
                    ct_PhieuDatHangNCC.THOIGIANSUA = Utility.CurrentTime;
                    ct_PhieuDatHangNCC.lstct_PhieuNhap_ChiTiet = new List<v_ct_PhieuDatHangNCC_ChiTiet>();
                    v_ct_PhieuDatHangNCC_ChiTiet ct_PhieuDatHangNCC_ChiTiet = new v_ct_PhieuDatHangNCC_ChiTiet();

                    foreach (string Key in lstKey)
                    {
                        var lstString = Key.ToString().Split('|');
                        var value = HttpContext.Request.Params.GetValues(Key.ToString());
                        string ShowSearchValue = clsMaHoa.Decrypt(lstString[1].ToString(), clsMaHoa.PassMaHoa);
                        var Checkct_PhieuDatHangNCC_ChiTiet = JsonConvert.DeserializeObject<v_ct_PhieuDatHangNCC_ChiTiet>(ShowSearchValue);
                        if (ct_PhieuDatHangNCC_ChiTiet.ID != Checkct_PhieuDatHangNCC_ChiTiet.ID)
                        {
                            ct_PhieuDatHangNCC_ChiTiet = new v_ct_PhieuDatHangNCC_ChiTiet();
                            ct_PhieuDatHangNCC_ChiTiet = JsonConvert.DeserializeObject<v_ct_PhieuDatHangNCC_ChiTiet>(ShowSearchValue);
                            ct_PhieuDatHangNCC_ChiTiet.LOC_ID = ct_PhieuDatHangNCC.LOC_ID;
                            ct_PhieuDatHangNCC.lstct_PhieuNhap_ChiTiet.Add(ct_PhieuDatHangNCC_ChiTiet);
                        }
                        Utility.EditObject(ct_PhieuDatHangNCC_ChiTiet, lstString[0].ToString().Substring(3, lstString[0].ToString().Length - 3), value[0]);
                    }

                    apiResponse = Utility.Edit<v_ct_PhieuDatHangNCC>(Utility.LOC_ID + "/" + ct_PhieuDatHangNCC.ID, ct_PhieuDatHangNCC, API.ct_PhieuDatHangNCC);
                    if (apiResponse.Success)
                    {
                        //return RedirectToAction("Index");
                        apiResponse.ID = ct_PhieuDatHangNCC.ID;
                        if (apiResponse.Data != null)
                            ct_PhieuDatHangNCC = JsonConvert.DeserializeObject<v_v_ct_PhieuDatHangNCC>(apiResponse.Data.ToString());
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, apiResponse.Message);
                    }
                }
                else
                {
                    apiResponse.Success = false;
                    apiResponse.Data = Utility.GetModelState(ModelState,API.ct_PhieuDatHangNCC);
                }
                ct_PhieuDatHangNCC.lstdm_NhaCungCap = new List<ComboboxFrom>();
                var lstdm_LoaiPhieuNhap = Utility.GetListData<v_dm_LoaiPhieuNhap>(API.dm_LoaiPhieuNhap, "", "", Utility.LOC_ID).Data as List<v_dm_LoaiPhieuNhap>;
                var dm_LoaiPhieuNhap = lstdm_LoaiPhieuNhap.Where(e => e.ID == ct_PhieuDatHangNCC.ID_LOAIPHIEUNHAP).FirstOrDefault();
                if (dm_LoaiPhieuNhap == null || string.IsNullOrEmpty(dm_LoaiPhieuNhap.ID))
                {
                    TempData["TitleError"] = "Không tìm thấy loại phiếu nhập";
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                if (dm_LoaiPhieuNhap.TYPE == 1)
                {
                    ct_PhieuDatHangNCC.lstdm_NhaCungCap = Utility.GetListData<ComboboxFrom>(API.dm_NhaCungCap, "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
                    apiResponse.TYPE = "divNCCEdit";
                }
               
                ct_PhieuDatHangNCC.lstdm_Kho = new List<v_dm_Kho>();
                ct_PhieuDatHangNCC.lstdm_Kho = Utility.GetListData<v_dm_Kho>(API.dm_Kho, "", "", Utility.LOC_ID).Data as List<v_dm_Kho>;
                ct_PhieuDatHangNCC.lstdm_LoaiPhieuNhap = new List<v_dm_LoaiPhieuNhap>();
                ct_PhieuDatHangNCC.lstdm_LoaiPhieuNhap = lstdm_LoaiPhieuNhap;
                ct_PhieuDatHangNCC.lstdm_NhanVien = new List<ComboboxFrom>();
                ct_PhieuDatHangNCC.lstdm_NhanVien = Utility.GetListData<ComboboxFrom>(API.dm_NhanVien, "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
                apiResponse.Detail = Utility.ConvertobjectToView<v_v_ct_PhieuDatHangNCC>(ct_PhieuDatHangNCC);
                List<Product_Detail> lstProduct_Detail = new List<Product_Detail>();
                lstProduct_Detail = Utility.GetlstProductInput();
                var lst = Utility.ConvertobjectToView<v_v_ct_PhieuDatHangNCC>(ct_PhieuDatHangNCC);
                apiResponse.ProductCombo = Utility.GetProductInputOutput(lstProduct_Detail, "InputOutput");
                lst.Add(new ValueEdit { Key = "tbodyTempItemInputEdit", Value = apiResponse.ProductCombo });
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
                if (!Utility.KiemTraQuyen(API.ct_PhieuDatHangNCC, API.Delete))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                apiResponse = Utility.Delete<v_ct_PhieuDatHangNCC>(Utility.LOC_ID + "/" + id, API.ct_PhieuDatHangNCC);
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
                v_ct_PhieuDatHangNCC PhieuNhap = new v_ct_PhieuDatHangNCC();
                SP_Parameter objParameter = new SP_Parameter();
                objParameter.ID_PHIEUNHAP = ID;
                apiResponse = Utility.ExecuteStoredProc<v_ct_PhieuDatHangNCC>(objParameter, API.Sp_Get_DanhSachPhieuDatHangNCC);
                if (!apiResponse.Success)
                {
                    apiResponse.Success = false;
                    apiResponse.Message = apiResponse.Message;
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                if (apiResponse.Data != null)
                    PhieuNhap = (apiResponse.Data as List<v_ct_PhieuDatHangNCC>).FirstOrDefault();

                SP_Parameter_Report objParameter_Report = new SP_Parameter_Report();
                objParameter_Report.LOC_ID = Utility.LOC_ID;
                objParameter_Report.ID_PHIEUNHAP = ID;
                var report = new ReportClass();
                
                apiResponse = Utility.ExecuteStoredProc<DataTable>(objParameter_Report, API.Sp_Get_DanhSachPhieuDatHangNCC_Chitiet);
                if (!apiResponse.Success)
                {
                    apiResponse.Success = false;
                    apiResponse.Message = apiResponse.Message;
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }

                //byte[] BinaryData = System.Text.Encoding.UTF8.GetBytes("https://ironsoftware.com/csharp/barcode/");
                // WRITE QR with Binary Content
                //String fullpath = Path.Combine(Server.MapPath("~" + API.PathProduct), "MyBinaryQR.png");
                //String fullpathLogo = Path.Combine(Server.MapPath("~" + API.PathLogo), "logoTrangHiepPhat.jpg");

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
                apiResponse.NAME = Utility.GetTitleFrom(API.ct_PhieuDatHangNCC) + " - " + PhieuNhap.MAPHIEU;
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

        public ActionResult Completed(string ID = "", string TRANGTHAI = "")
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                if (Utility.KiemTra())
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (!Utility.KiemTraQuyen(API.ct_PhieuDatHangNCC, API.Xem))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }

                v_ct_PhieuDatHangNCC ct_PhieuGiaoHang = new v_ct_PhieuDatHangNCC();
                if (!string.IsNullOrEmpty(ID))
                {
                    apiResponse = Utility.GetDetail<v_ct_PhieuDatHangNCC>(Utility.LOC_ID + "/" + ID, API.ct_PhieuDatHangNCC);
                    if (!apiResponse.Success)
                    {
                        TempData["TitleError"] = apiResponse.Message;
                        apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                        return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                    }
                    if (apiResponse.Data != null)
                        ct_PhieuGiaoHang = apiResponse.Data as v_ct_PhieuDatHangNCC;


                    ct_PhieuGiaoHang.ISHOANTAT = TRANGTHAI == "1";
                    apiResponse = Utility.Edit<v_ct_PhieuDatHangNCC>(Utility.LOC_ID + "/" + ID + "/" + TRANGTHAI, null, API.ct_PhieuDatHangNCC);
                }
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