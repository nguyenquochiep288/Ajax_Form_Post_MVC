using DatabaseTHP.Class;
using DatabaseTHP.StoredProcedure.Parameter;
using DatabaseTHP.StoredProcedure;
using MVC_QuanLyTHP.Class;
using MVC_QuanLyTHP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using DatabaseTHP;
using System.Web.DynamicData;
using System.Threading;
using System.Reflection;
using System.Diagnostics;
using Syncfusion.EJ2.Popups;
using PagedList;
using Newtonsoft.Json;
using System.Web.UI;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System.Data;
using System.IO;
using System.Diagnostics.Eventing.Reader;

namespace MVC_QuanLyTHP.Controllers
{
    public class OutputController : Controller
    {
        // GET: Output
        #region Output
        public ActionResult Index(int Page = 1, DateTime? FromDate = null, DateTime? ToDate = null, string SearchString = "", string ShowSearchValue = "", string MAPHIEU = "", string IDCODE = "")
        {
            try
            {
                if (Utility.KiemTra())
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (!Utility.KiemTraQuyen(API.ct_PhieuXuat, API.Xem))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                string TotalSum = "";
                ShowSearchValue = Utility.GetShowSearchValue<ct_PhieuXuat>(ShowSearchValue);
                ApiResponse apiResponse = new ApiResponse();
                IPagedList<v_ct_PhieuXuat> lstpage = (new List<v_ct_PhieuXuat>()).OrderByDescending(s => s.NGAYLAP).ToList().ToPagedList(Page, Utility.GetPageSize()); ;
                if (FromDate != null || !string.IsNullOrEmpty(IDCODE) || !string.IsNullOrEmpty(MAPHIEU))
                {
                    if (!string.IsNullOrEmpty(IDCODE) || !string.IsNullOrEmpty(MAPHIEU))
                    {
                        apiResponse = Utility.Get_DanhSachPhieuXuat<v_ct_PhieuXuat>("", null, null, MAPHIEU, IDCODE);
                    }
                    if (FromDate != null)
                    {
                        apiResponse = Utility.Get_DanhSachPhieuXuat<v_ct_PhieuXuat>("", FromDate, ToDate, SearchString);
                    }
                    //apiResponse = Utility.GetListDataOrder<v_ct_PhieuXuat>(API.ct_PhieuXuat, FromDate, ToDate, ShowSearchValue, SearchString, Utility.LOC_ID);
                    if (!apiResponse.Success)
                    {
                        TempData["TitleError"] = apiResponse.Message;
                        return RedirectToAction("Index", "Notfound");
                    }
                    if (apiResponse.Data != null)
                    {
                        var Login_Model = (Login_Model)Session[Sessions.Login_Model];
                        if (Utility.KiemTraQuyen(API.ct_PhieuXuat, API.AllData))
                        {
                            lstpage = (apiResponse.Data as List<v_ct_PhieuXuat>).OrderByDescending(s => s.NGAYLAP).ToList().ToPagedList(Page, Utility.GetPageSize());
                            TotalSum = (apiResponse.Data as List<v_ct_PhieuXuat>).Sum(s => s.TONGTIEN).ToString("N0");
                        }
                        else
                        {
                            if (Utility.KiemTraQuyen(API.ct_PhieuXuat, API.UserData))
                            {
                                lstpage = (apiResponse.Data as List<v_ct_PhieuXuat>).Where(s => s.ID_NHANVIEN == Login_Model.iduser).OrderByDescending(s => s.NGAYLAP).ToList().ToPagedList(Page, Utility.GetPageSize());
                                TotalSum = (apiResponse.Data as List<v_ct_PhieuXuat>).Sum(s => s.TONGTIEN).ToString("N0");
                            }    
                        }
                    }
                }

                v_v_ct_PhieuXuat ct_PhieuXuat = new v_v_ct_PhieuXuat();
                ct_PhieuXuat.IPagedList = lstpage;
                ct_PhieuXuat.lstdm_ThueSuat = new List<v_dm_ThueSuat>();
                ct_PhieuXuat.lstdm_ThueSuat = Utility.GetListData<v_dm_ThueSuat>(API.dm_ThueSuat, "", "", Utility.LOC_ID).Data as List<v_dm_ThueSuat>;
                //ct_PhieuXuat.lstdm_Kho = new List<v_dm_Kho>();
                //ct_PhieuXuat.lstdm_Kho = Utility.GetListData<v_dm_Kho>(API.dm_Kho, "", "", Utility.LOC_ID).Data as List<v_dm_Kho>;
                //ct_PhieuXuat.lstdm_KhachHang = new List<v_dm_KhachHang>();
                //ct_PhieuXuat.lstdm_KhachHang = Utility.GetListData<v_dm_KhachHang>(API.dm_KhachHang, "", "", Utility.LOC_ID).Data as List<v_dm_KhachHang>;
                //ct_PhieuXuat.lstdm_NhanVien = new List<v_dm_NhanVien>();
                //ct_PhieuXuat.lstdm_NhanVien = Utility.GetListData<v_dm_NhanVien>(API.dm_NhanVien, "", "", Utility.LOC_ID).Data as List<v_dm_NhanVien>;
                ct_PhieuXuat.lstdm_LoaiPhieuXuat = new List<v_dm_LoaiPhieuXuat>();
                var lstdm_LoaiPhieuXuat = Utility.GetListData<v_dm_LoaiPhieuXuat>(API.dm_LoaiPhieuXuat, "", "", Utility.LOC_ID).Data as List<v_dm_LoaiPhieuXuat>;
                if (lstdm_LoaiPhieuXuat != null)
                {
                    ct_PhieuXuat.lstdm_LoaiPhieuXuat = lstdm_LoaiPhieuXuat.Where(e => e.ISACTIVE == true).OrderBy(e => e.TYPE).ToList();
                }
                else
                {
                    ct_PhieuXuat.lstdm_LoaiPhieuXuat = new List<v_dm_LoaiPhieuXuat>();
                }
                
                ViewBag.TotalSum = TotalSum;
                ViewBag.searchValue = SearchString;
                ViewBag.showsearchValue = ShowSearchValue;
                ViewBag.fromdate = FromDate != null ? FromDate.Value.ToString("yyyy-MM-dd") : Utility.CurrentTime.ToString("yyyy-MM-dd");
                ViewBag.todate = ToDate != null ? ToDate.Value.ToString("yyyy-MM-dd") : Utility.CurrentTime.ToString("yyyy-MM-dd");
                ViewBag.PermissionEdit = Utility.KiemTraQuyen(API.ct_PhieuXuat, API.Edit);
                ViewBag.PermissionDelete = Utility.KiemTraQuyen(API.ct_PhieuXuat, API.Delete);
                ViewBag.PermissionCreate = Utility.KiemTraQuyen(API.ct_PhieuXuat, API.Create);
                ViewBag.PermissionCreateInput = Utility.KiemTraQuyen(API.ct_PhieuXuat, API.CreateInput);
                return View(ct_PhieuXuat);
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
                TempData["TitleError"] = API.TitleTryCatch;
                TempData["DetailError"] = ex.Message;
                return RedirectToAction("Index", "Notfound");
            }
        }


        public ActionResult Create(int type = 2)
        {
            Session[Sessions.IntWidth] = type;
            try
            {
                if (Utility.KiemTra())
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (!Utility.KiemTraQuyen(API.ct_PhieuXuat, API.Create))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                v_v_ct_PhieuXuat ct_PhieuXuat = new v_v_ct_PhieuXuat();
                //ct_PhieuXuat.NGAYLAP = Utility.CurrentTime;
                //ct_PhieuXuat.LOC_ID = Utility.LOC_ID;
                //ct_PhieuXuat.SOPHIEU = Utility.GetMaxID<ct_PhieuXuat>(ct_PhieuXuat, Utility.LOC_ID, ct_PhieuXuat.NGAYLAP.ToString("yyyy-MM-dd"));
                //ct_PhieuXuat.MAPHIEU = API.GetMaPhieu(API.ct_PhieuXuat, ct_PhieuXuat.NGAYLAP, ct_PhieuXuat.SOPHIEU);
                ct_PhieuXuat.lstdm_ThueSuat = new List<v_dm_ThueSuat>();
                //ct_PhieuXuat.lstdm_ThueSuat = Utility.GetListData<v_dm_ThueSuat>(API.dm_ThueSuat, "", "", Utility.LOC_ID).Data as List<v_dm_ThueSuat>;
                ct_PhieuXuat.lstdm_Kho = new List<v_dm_Kho>();
                //ct_PhieuXuat.lstdm_Kho = Utility.GetListData<v_dm_Kho>(API.dm_Kho, "", "", Utility.LOC_ID).Data as List<v_dm_Kho>;
                ct_PhieuXuat.lstdm_KhachHang = new List<ComboboxFrom>();
                //ct_PhieuXuat.lstdm_KhachHang = GetDanhSachKhachHang().Data as List<v_dm_KhachHang>;
                ct_PhieuXuat.lstdm_NhaCungCap = new List<ComboboxFrom>();
                ct_PhieuXuat.lstdm_NhanVien = new List<ComboboxFrom>();
                List<Product_Detail> lstOrderProduct = new List<Product_Detail>();
                Session[Sessions.lstProductInput] = lstOrderProduct;
                return View(ct_PhieuXuat);
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
                TempData["TitleError"] = API.TitleTryCatch;
                TempData["DetailError"] = ex.Message;
                return RedirectToAction("Index", "Notfound");
            }
        }


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
                if (!Utility.KiemTraQuyen(API.ct_PhieuXuat, API.Create))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }

                var lstdm_LoaiPhieuXuat = Utility.GetListData<v_dm_LoaiPhieuXuat>(API.dm_LoaiPhieuXuat, "", "", Utility.LOC_ID).Data as List<v_dm_LoaiPhieuXuat>;
                var dm_LoaiPhieuXuat = lstdm_LoaiPhieuXuat.Where(e => e.ID == ID_LOAIPHIEU).FirstOrDefault();
                if (dm_LoaiPhieuXuat == null || string.IsNullOrEmpty(dm_LoaiPhieuXuat.ID))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                v_v_ct_PhieuXuat ct_PhieuXuat = new v_v_ct_PhieuXuat();
                apiResponse.Success = true;
                ct_PhieuXuat.ID_LOAIPHIEUXUAT = ID_LOAIPHIEU;
                ct_PhieuXuat.LOC_ID = Utility.LOC_ID;
                ct_PhieuXuat.ID = Guid.NewGuid().ToString();
                ct_PhieuXuat.NGAYLAP = Utility.CurrentTime;
                ct_PhieuXuat.SOPHIEU = Utility.GetMaxID<ct_PhieuXuat>(ct_PhieuXuat, Utility.LOC_ID, ct_PhieuXuat.NGAYLAP.ToString("yyyy-MM-dd"));
                ct_PhieuXuat.MAPHIEU = API.GetMaPhieu(API.ct_PhieuXuat, ct_PhieuXuat.NGAYLAP, ct_PhieuXuat.SOPHIEU);
                ct_PhieuXuat.CHUNGTUKEMTHEO = CHUNGTUKEMTHEO;
                ct_PhieuXuat.lstdm_NhaCungCap = new List<ComboboxFrom>();
                if (dm_LoaiPhieuXuat.TYPE == 1)
                {
                    ct_PhieuXuat.lstdm_NhaCungCap = Utility.GetListData<ComboboxFrom>(API.dm_NhaCungCap, "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
                    ct_PhieuXuat.ID_NHACUNGCAP = ID_KHACHAHANG;
                    apiResponse.TYPE = "divNCCAdd";
                }

                ct_PhieuXuat.lstdm_KhachHang = new List<ComboboxFrom>();
                if (dm_LoaiPhieuXuat.TYPE == 2)
                {
                    apiResponse.TYPE = "divKHACHHANGAdd";
                    ct_PhieuXuat.ID_KHACHHANG = ID_KHACHAHANG;
                    ct_PhieuXuat.lstdm_KhachHang = Utility.GetListData<ComboboxFrom>(API.dm_KhachHang, "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
                }

                ct_PhieuXuat.lstdm_NhanVien = new List<ComboboxFrom>();
                if (dm_LoaiPhieuXuat.TYPE == 3)
                {
                    apiResponse.TYPE = "divNHANVIENAdd";
                    ct_PhieuXuat.lstdm_NhanVien = Utility.GetListData<ComboboxFrom>(API.dm_NhanVien, "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
                    ct_PhieuXuat.ID_NHANVIEN = ID_KHACHAHANG;
                }
                ct_PhieuXuat.lstdm_Kho = new List<v_dm_Kho>();
                ct_PhieuXuat.lstdm_Kho = Utility.GetListData<v_dm_Kho>(API.dm_Kho, "", "", Utility.LOC_ID).Data as List<v_dm_Kho>;
                ct_PhieuXuat.lstdm_LoaiPhieuXuat = new List<v_dm_LoaiPhieuXuat>();
                ct_PhieuXuat.lstdm_LoaiPhieuXuat = Utility.GetListData<v_dm_LoaiPhieuXuat>(API.dm_LoaiPhieuXuat, "", "", Utility.LOC_ID).Data as List<v_dm_LoaiPhieuXuat>;
                //ct_PhieuXuat.lstdm_NhanVien = new List<ComboboxFrom>();
                //ct_PhieuXuat.lstdm_NhanVien = Utility.GetListData<ComboboxFrom>(API.dm_NhanVien, "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
                Session[Sessions.lstProductInput] = new List<Product_Detail>();

                var lst = Utility.ConvertobjectTo<v_v_ct_PhieuXuat>(ct_PhieuXuat);
                apiResponse.ProductCombo = Utility.GetProductInputOutput(new List<Product_Detail>(), "Deposit_Temp");
                lst.Add(new ValueEdit { Key = "tbodyTempItemInput", Value = apiResponse.ProductCombo });
                lst.Add(new ValueEdit { Key = "lblName", Value = dm_LoaiPhieuXuat.NAME.ToUpper() });
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

        // POST: Deposit_TEMP/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult CreatePopup([Bind(Include = "LOC_ID,ID,ID_LOAIPHIEUXUAT,ID_KHO,MAPHIEU,SOPHIEU,NGAYLAP,ID_KHACHHANG,ID_NHACUNGCAP,ID_NHANVIEN,GHICHU,TONGTHANHTIEN,CHIETKHAU,TONGTIENGIAMGIA,TONGTIENVAT,TONGTIEN,BUTTONTYPE,ADDRESS,TEL,CHUNGTUKEMTHEO")] v_v_ct_PhieuXuat ct_PhieuXuat)
        {
            try
            {
                if (Utility.KiemTra())
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (!Utility.KiemTraQuyen(API.ct_PhieuXuat, API.Create))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }

                ct_PhieuXuat.lstct_PhieuXuat_ChiTiet = new List<v_ct_PhieuXuat_ChiTiet>();
                List<Product_Detail> lstOrderProduct = new List<Product_Detail>();
                var lstKey = Request.Form.AllKeys.Where(e => e.StartsWith("txt"));
                if (lstKey == null || lstKey.Count() == 0)
                {
                    ModelState.AddModelError("lstct_PhieuXuat_ChiTiet", "Thêm danh sách hàng hóa.");
                }
                else
                {
                    v_ct_PhieuXuat_ChiTiet ct_PhieuXuat_ChiTiet = new v_ct_PhieuXuat_ChiTiet();
                    foreach (string Key in lstKey)
                    {
                        var lstString = Key.ToString().Split('|');
                        var value = HttpContext.Request.Params.GetValues(Key.ToString());
                        string ShowSearchValue = clsMaHoa.Decrypt(lstString[1].ToString(), clsMaHoa.PassMaHoa);
                        var Checkct_PhieuXuat_ChiTiet = JsonConvert.DeserializeObject<Product_Detail>(ShowSearchValue);
                        if (ct_PhieuXuat_ChiTiet.ID != Checkct_PhieuXuat_ChiTiet.ID)
                        {
                            ct_PhieuXuat_ChiTiet = new v_ct_PhieuXuat_ChiTiet();
                            ct_PhieuXuat_ChiTiet = JsonConvert.DeserializeObject<v_ct_PhieuXuat_ChiTiet>(ShowSearchValue);
                            ct_PhieuXuat_ChiTiet.LOC_ID = ct_PhieuXuat.LOC_ID;
                            ct_PhieuXuat.lstct_PhieuXuat_ChiTiet.Add(ct_PhieuXuat_ChiTiet);
                            lstOrderProduct.Add(Checkct_PhieuXuat_ChiTiet);
                        }
                        Utility.EditObject(ct_PhieuXuat_ChiTiet, lstString[0].ToString().Substring(3, lstString[0].ToString().Length - 3), value[0]);
                    }
                }
                ApiResponse apiResponse = new ApiResponse();
                if (ct_PhieuXuat.BUTTONTYPE == "GetPromotion")
                {
                    // Do Next Here
                    apiResponse = Utility.Create<List<v_ct_PhieuXuat_ChiTiet>>(ct_PhieuXuat.lstct_PhieuXuat_ChiTiet, API.ct_PhieuXuat + "/" + Utility.LOC_ID);
                    lstOrderProduct = JsonConvert.DeserializeObject<List<Product_Detail>>(apiResponse.Data.ToString());

                    apiResponse.GETPROMOTION = Utility.GetProductInputOutput(lstOrderProduct, "Deposit_Temp");

                    apiResponse.SOPHIEU = ct_PhieuXuat.SOPHIEU = Utility.GetMaxID<ct_PhieuXuat>(ct_PhieuXuat, Utility.LOC_ID, ct_PhieuXuat.NGAYLAP.ToString("yyyy-MM-dd"));
                    ct_PhieuXuat.MAPHIEU = API.GetMaPhieu(API.ct_PhieuXuat, ct_PhieuXuat.NGAYLAP, ct_PhieuXuat.SOPHIEU);
                    apiResponse.NewID = ct_PhieuXuat.ID;
                    apiResponse.MAPHIEU = ct_PhieuXuat.MAPHIEU;
                }
                if (ct_PhieuXuat.BUTTONTYPE == "Save")
                {
                    if (ModelState.IsValid)
                    {
                        ct_PhieuXuat.NGAYLAP = ct_PhieuXuat.NGAYLAP.AddHours(Utility.CurrentTime.Hour).AddMinutes(Utility.CurrentTime.Minute);
                        ct_PhieuXuat.ID = Guid.NewGuid().ToString();
                        ct_PhieuXuat.LOC_ID = Utility.LOC_ID;
                        ct_PhieuXuat.ID_NGUOITAO = Session[Sessions.idUser].ToString();
                        ct_PhieuXuat.THOIGIANTHEM = Utility.CurrentTime;
                        ct_PhieuXuat.ID_NHANVIEN = Session[Sessions.idUser].ToString();
                        apiResponse = Utility.Create<v_ct_PhieuXuat>(ct_PhieuXuat, API.ct_PhieuXuat);
                        if (apiResponse.Success)
                        {
                            ct_PhieuXuat.NGAYLAP = Utility.CurrentTime;
                            apiResponse.SOPHIEU = ct_PhieuXuat.SOPHIEU = Utility.GetMaxID<ct_PhieuXuat>(ct_PhieuXuat, Utility.LOC_ID, ct_PhieuXuat.NGAYLAP.ToString("yyyy-MM-dd"));
                            ct_PhieuXuat.MAPHIEU = API.GetMaPhieu(API.ct_PhieuXuat, ct_PhieuXuat.NGAYLAP, ct_PhieuXuat.SOPHIEU);
                            apiResponse.NewID = Guid.NewGuid().ToString();
                            apiResponse.MAPHIEU = ct_PhieuXuat.MAPHIEU;

                            if (apiResponse.Data != null)
                                ct_PhieuXuat = JsonConvert.DeserializeObject<v_v_ct_PhieuXuat>(apiResponse.Data.ToString());

                            lstOrderProduct = new List<Product_Detail>();
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, apiResponse.Message);
                            if (apiResponse.CheckValue)
                            {
                                ct_PhieuXuat.NGAYLAP = Utility.CurrentTime;
                                apiResponse.SOPHIEU = ct_PhieuXuat.SOPHIEU = Utility.GetMaxID<ct_PhieuXuat>(ct_PhieuXuat, Utility.LOC_ID, ct_PhieuXuat.NGAYLAP.ToString("yyyy-MM-dd"));
                                ct_PhieuXuat.MAPHIEU = API.GetMaPhieu(API.ct_PhieuXuat, ct_PhieuXuat.NGAYLAP, ct_PhieuXuat.SOPHIEU);
                                apiResponse.NewID = Guid.NewGuid().ToString();
                                apiResponse.MAPHIEU = ct_PhieuXuat.MAPHIEU;
                            }
                        }
                    }
                    else
                    {
                        apiResponse.Success = false;
                        apiResponse.Data = Utility.GetModelState(ModelState, API.ct_PhieuXuat);
                    }
                }

                Session[Sessions.lstProductInput] = lstOrderProduct;
                apiResponse.ID = ct_PhieuXuat.ID;
                var lstdm_LoaiPhieuXuat = Utility.GetListData<v_dm_LoaiPhieuXuat>(API.dm_LoaiPhieuXuat, "", "", Utility.LOC_ID).Data as List<v_dm_LoaiPhieuXuat>;
                var dm_LoaiPhieuXuat = lstdm_LoaiPhieuXuat.Where(e => e.ID == ct_PhieuXuat.ID_LOAIPHIEUXUAT).FirstOrDefault();
                if (dm_LoaiPhieuXuat == null || string.IsNullOrEmpty(dm_LoaiPhieuXuat.ID))
                {
                    TempData["TitleError"] = "Không tìm thấy loại phiếu xuất";
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                ct_PhieuXuat.lstdm_NhaCungCap = new List<ComboboxFrom>();
                if (dm_LoaiPhieuXuat.TYPE == 1)
                {
                    ct_PhieuXuat.lstdm_NhaCungCap = Utility.GetListData<ComboboxFrom>(API.dm_NhaCungCap, "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
                    apiResponse.TYPE = "divNCCAdd";
                }

                ct_PhieuXuat.lstdm_KhachHang = new List<ComboboxFrom>();
                if (dm_LoaiPhieuXuat.TYPE == 2)
                {
                    apiResponse.TYPE = "divKHACHHANGAdd";
                    ct_PhieuXuat.lstdm_KhachHang = Utility.GetListData<ComboboxFrom>(API.dm_KhachHang, "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
                }
                ct_PhieuXuat.lstdm_NhanVien = new List<ComboboxFrom>();
                if (dm_LoaiPhieuXuat.TYPE == 3)
                {
                    apiResponse.TYPE = "divNHANVIENAdd";
                    ct_PhieuXuat.lstdm_NhanVien = Utility.GetListData<ComboboxFrom>(API.dm_NhanVien, "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
                }
                ct_PhieuXuat.lstdm_Kho = new List<v_dm_Kho>();
                ct_PhieuXuat.lstdm_Kho = Utility.GetListData<v_dm_Kho>(API.dm_Kho, "", "", Utility.LOC_ID).Data as List<v_dm_Kho>;
                ct_PhieuXuat.lstdm_LoaiPhieuXuat = new List<v_dm_LoaiPhieuXuat>();
                //ct_PhieuNhap.lstdm_LoaiPhieuNhap = lstdm_LoaiPhieuNhap;
                //ct_PhieuXuat.lstdm_NhanVien = new List<ComboboxFrom>();
                //ct_PhieuXuat.lstdm_NhanVien = Utility.GetListData<ComboboxFrom>(API.dm_NhanVien, "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
                var lst = Utility.ConvertobjectToView<v_v_ct_PhieuXuat>(ct_PhieuXuat, "dd/MM/yy HH:mm");
                apiResponse.ProductCombo = Utility.GetProductInputOutput(lstOrderProduct, "Deposit_Temp");
                lst.Add(new ValueEdit { Key = "tbodyTempItemInput", Value = apiResponse.ProductCombo });
                apiResponse.Detail = lst;
                return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
                TempData["TitleError"] = API.TitleTryCatch;
                TempData["DetailError"] = ex.Message;
                return RedirectToAction("Index", "Notfound");
            }
        }

        public ActionResult Edit(string id, int type = 2)
        {
            ApiResponse apiResponse = new ApiResponse();
            Session[Sessions.IntWidth] = type;
            try
            {
                if (Utility.KiemTra())
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (!Utility.KiemTraQuyen(API.ct_PhieuXuat, API.Create))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                List<Product_Detail> lstOrderProduct = new List<Product_Detail>();
                v_v_ct_PhieuXuat ct_PhieuXuat = new v_v_ct_PhieuXuat();
                if (!string.IsNullOrEmpty(id))
                {
                    apiResponse = Utility.GetDetail<v_v_ct_PhieuXuat>(Utility.LOC_ID + "/" + id, API.ct_PhieuXuat);
                    if (!apiResponse.Success)
                    {
                        TempData["TitleError"] = apiResponse.Message;
                        return RedirectToAction("Index", "Notfound");
                    }
                    if (apiResponse.Data != null)
                        ct_PhieuXuat = apiResponse.Data as v_v_ct_PhieuXuat;

                    foreach (var itm in ct_PhieuXuat.lstct_PhieuXuat_ChiTiet)
                    {
                        lstOrderProduct.Add(Utility.ConvertobjectToProduct_Detail<v_ct_PhieuXuat_ChiTiet>(itm, new Product_Detail()));
                    }
                }

                ct_PhieuXuat.lstdm_Kho = new List<v_dm_Kho>();
                //ct_PhieuXuat.lstdm_Kho = Utility.GetListData<v_dm_Kho>(API.dm_Kho, "", "", Utility.LOC_ID).Data as List<v_dm_Kho>;
                ct_PhieuXuat.lstdm_KhachHang = new List<ComboboxFrom>();
                //ct_PhieuXuat.lstdm_KhachHang = GetDanhSachKhachHang().Data as List<v_dm_KhachHang>;
                ct_PhieuXuat.lstdm_ThueSuat = new List<v_dm_ThueSuat>();
                //ct_PhieuXuat.lstdm_ThueSuat = Utility.GetListData<v_dm_ThueSuat>(API.dm_ThueSuat, "", "", Utility.LOC_ID).Data as List<v_dm_ThueSuat>;
                ct_PhieuXuat.lstdm_NhaCungCap = new List<ComboboxFrom>();
                ct_PhieuXuat.lstdm_NhanVien = new List<ComboboxFrom>();
                Session[Sessions.lstProductInput] = lstOrderProduct;
                apiResponse.ProductCombo = Utility.GetProductInputOutput(lstOrderProduct, "Deposit_Temp");
                ViewBag.DatHang = apiResponse.ProductCombo;
                //foreach (string printer in System.Drawing.Printing.PrinterSettings.InstalledPrinters)
                //{

                //}
                return View(ct_PhieuXuat);
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
                TempData["TitleError"] = API.TitleTryCatch;
                TempData["DetailError"] = ex.Message;
                return RedirectToAction("Index", "Notfound");
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
                if (!Utility.KiemTraQuyen(API.ct_PhieuXuat, API.Edit))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                List<Product_Detail> lstOrderProduct = new List<Product_Detail>();
                v_v_ct_PhieuXuat ct_PhieuXuat = new v_v_ct_PhieuXuat();
                if (!string.IsNullOrEmpty(id))
                {
                    apiResponse = Utility.GetDetail<v_v_ct_PhieuXuat>(Utility.LOC_ID + "/" + id, API.ct_PhieuXuat);
                    if (!apiResponse.Success)
                    {
                        TempData["TitleError"] = apiResponse.Message;
                        return RedirectToAction("Index", "Notfound");
                    }
                    if (apiResponse.Data != null)
                        ct_PhieuXuat = apiResponse.Data as v_v_ct_PhieuXuat;
                }
                foreach (var itm in ct_PhieuXuat.lstct_PhieuXuat_ChiTiet)
                {
                    lstOrderProduct.Add(Utility.ConvertobjectToProduct_Detail<v_ct_PhieuXuat_ChiTiet>(itm, new Product_Detail()));
                }

                ct_PhieuXuat.lstdm_NhaCungCap = new List<ComboboxFrom>();
                var lstdm_LoaiPhieuXuat = Utility.GetListData<v_dm_LoaiPhieuXuat>(API.dm_LoaiPhieuXuat, "", "", Utility.LOC_ID).Data as List<v_dm_LoaiPhieuXuat>;
                var dm_LoaiPhieuXuat = lstdm_LoaiPhieuXuat.Where(e => e.ID == ct_PhieuXuat.ID_LOAIPHIEUXUAT).FirstOrDefault();
                if (dm_LoaiPhieuXuat == null || string.IsNullOrEmpty(dm_LoaiPhieuXuat.ID))
                {
                    TempData["TitleError"] = "Không tìm thấy loại phiếu xuất";
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                if (dm_LoaiPhieuXuat.TYPE == 1)
                {
                    ct_PhieuXuat.lstdm_NhaCungCap = Utility.GetListData<ComboboxFrom>(API.dm_NhaCungCap, "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
                    apiResponse.TYPE = "divNCCEdit";
                }

                ct_PhieuXuat.lstdm_KhachHang = new List<ComboboxFrom>();
                if (dm_LoaiPhieuXuat.TYPE == 2)
                {
                    apiResponse.TYPE = "divKHACHHANGEdit";
                    ct_PhieuXuat.lstdm_KhachHang = Utility.GetListData<ComboboxFrom>(API.dm_KhachHang, "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
                }

                ct_PhieuXuat.lstdm_NhanVien = new List<ComboboxFrom>();
                if (dm_LoaiPhieuXuat.TYPE == 3)
                {
                    apiResponse.TYPE = "divNHANVIENEdit";
                    ct_PhieuXuat.lstdm_NhanVien = Utility.GetListData<ComboboxFrom>(API.dm_NhanVien, "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
                }
                Session[Sessions.lstProductInput] = lstOrderProduct;
                ct_PhieuXuat.lstdm_Kho = new List<v_dm_Kho>();
                ct_PhieuXuat.lstdm_Kho = Utility.GetListData<v_dm_Kho>(API.dm_Kho, "", "", Utility.LOC_ID).Data as List<v_dm_Kho>;
                ct_PhieuXuat.lstdm_LoaiPhieuXuat = new List<v_dm_LoaiPhieuXuat>();
                ct_PhieuXuat.lstdm_LoaiPhieuXuat = lstdm_LoaiPhieuXuat;
                //ct_PhieuXuat.lstdm_NhanVien = new List<ComboboxFrom>();
                //ct_PhieuXuat.lstdm_NhanVien = Utility.GetListData<ComboboxFrom>(API.dm_NhanVien, "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
                var lst = Utility.ConvertobjectTo<v_v_ct_PhieuXuat>(ct_PhieuXuat);
                apiResponse.ProductCombo = Utility.GetProductInputOutput(lstOrderProduct, "Deposit_Temp");
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
        // POST: Deposit_TEMP/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID_NGUOITAO,THOIGIANTHEM,LOC_ID,ID,ID_LOAIPHIEUXUAT,ID_KHO,MAPHIEU,SOPHIEU,NGAYLAP,ID_KHACHHANG,ID_NHACUNGCAP,ID_NHANVIEN,GHICHU,TONGTHANHTIEN,CHIETKHAU,TONGTIENGIAMGIA,TONGTIENVAT,TONGTIEN,BUTTONTYPE,ADDRESS,TEL,CHUNGTUKEMTHEO")] v_v_ct_PhieuXuat ct_PhieuXuat)
        {
            try
            {
                if (Utility.KiemTra())
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (!Utility.KiemTraQuyen(API.ct_PhieuXuat, API.Edit))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                List<Product_Detail> lstOrderProduct = new List<Product_Detail>();
                ApiResponse apiResponse = new ApiResponse();
                ct_PhieuXuat.lstct_PhieuXuat_ChiTiet = new List<v_ct_PhieuXuat_ChiTiet>();
                var lstKey = Request.Form.AllKeys.Where(e => e.StartsWith("txt"));
                if (lstKey == null || lstKey.Count() == 0)
                {
                    ModelState.AddModelError("lstct_PhieuXuat_ChiTiet", "Thêm danh sách hàng hóa.");
                }
                else
                {
                    v_ct_PhieuXuat_ChiTiet ct_PhieuXuat_ChiTiet = new v_ct_PhieuXuat_ChiTiet();
                    foreach (string Key in lstKey)
                    {
                        var lstString = Key.ToString().Split('|');
                        var value = HttpContext.Request.Params.GetValues(Key.ToString());
                        string ShowSearchValue = clsMaHoa.Decrypt(lstString[1].ToString(), clsMaHoa.PassMaHoa);
                        var Checkct_PhieuXuat_ChiTiet = JsonConvert.DeserializeObject<Product_Detail>(ShowSearchValue);
                        if (ct_PhieuXuat_ChiTiet.ID != Checkct_PhieuXuat_ChiTiet.ID)
                        {
                            ct_PhieuXuat_ChiTiet = new v_ct_PhieuXuat_ChiTiet();
                            ct_PhieuXuat_ChiTiet = JsonConvert.DeserializeObject<v_ct_PhieuXuat_ChiTiet>(ShowSearchValue);
                            ct_PhieuXuat_ChiTiet.LOC_ID = ct_PhieuXuat.LOC_ID;
                            ct_PhieuXuat.lstct_PhieuXuat_ChiTiet.Add(ct_PhieuXuat_ChiTiet);
                            lstOrderProduct.Add(Checkct_PhieuXuat_ChiTiet);
                        }
                        Utility.EditObject(ct_PhieuXuat_ChiTiet, lstString[0].ToString().Substring(3, lstString[0].ToString().Length - 3), value[0]);
                    }
                }

                if (ct_PhieuXuat.BUTTONTYPE == "GetPromotion")
                {
                    // Do Next Here
                    apiResponse = Utility.Create<List<v_ct_PhieuXuat_ChiTiet>>(ct_PhieuXuat.lstct_PhieuXuat_ChiTiet, API.ct_PhieuXuat + "/" + Utility.LOC_ID);
                    lstOrderProduct = JsonConvert.DeserializeObject<List<Product_Detail>>(apiResponse.Data.ToString());
                    Session[Sessions.lstProductInput] = lstOrderProduct;
                    apiResponse.GETPROMOTION = Utility.GetProductInputOutput(lstOrderProduct, "Deposit_Temp");

                    apiResponse.SOPHIEU = ct_PhieuXuat.SOPHIEU;
                    apiResponse.NewID = ct_PhieuXuat.ID;
                    apiResponse.MAPHIEU = ct_PhieuXuat.MAPHIEU;
                }
                if (ct_PhieuXuat.BUTTONTYPE == "Save")
                {
                    if (ModelState.IsValid)
                    {
                        apiResponse = Utility.GetDetail<v_ct_PhieuXuat>(Utility.LOC_ID + "/" + ct_PhieuXuat.ID, API.ct_PhieuXuat);
                        if (!apiResponse.Success)
                        {
                            TempData["TitleError"] = apiResponse.Message;
                            return RedirectToAction("Index", "Notfound");
                        }
                        v_ct_PhieuXuat chkv_ct_PhieuXuat = null;
                        if (apiResponse.Data != null)
                            chkv_ct_PhieuXuat = apiResponse.Data as v_ct_PhieuXuat;

                        ct_PhieuXuat.LOC_ID = Utility.LOC_ID;
                        ct_PhieuXuat.ID_NGUOISUA = Session[Sessions.idUser].ToString();
                        ct_PhieuXuat.THOIGIANSUA = Utility.CurrentTime;

                        apiResponse = Utility.Edit<v_ct_PhieuXuat>(Utility.LOC_ID + "/" + ct_PhieuXuat.ID, ct_PhieuXuat, API.ct_PhieuXuat);
                        if (apiResponse.Success)
                        {
                            apiResponse.ID = ct_PhieuXuat.ID;
                            if (apiResponse.Data != null)
                                ct_PhieuXuat = JsonConvert.DeserializeObject<v_v_ct_PhieuXuat>(apiResponse.Data.ToString());

                            lstOrderProduct = new List<Product_Detail>();

                            //apiResponse.URL = Request.UrlReferrer.ToString();
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, apiResponse.Message);
                        }
                    }
                    else
                    {
                        apiResponse.Success = false;
                        apiResponse.Data = Utility.GetModelState(ModelState, API.ct_PhieuXuat);
                    }
                }
                Session[Sessions.lstProductInput] = lstOrderProduct;
                apiResponse.ID = ct_PhieuXuat.ID;
                ct_PhieuXuat.lstdm_NhaCungCap = new List<ComboboxFrom>();
                var lstdm_LoaiPhieuXuat = Utility.GetListData<v_dm_LoaiPhieuXuat>(API.dm_LoaiPhieuXuat, "", "", Utility.LOC_ID).Data as List<v_dm_LoaiPhieuXuat>;
                var dm_LoaiPhieuXuat = lstdm_LoaiPhieuXuat.Where(e => e.ID == ct_PhieuXuat.ID_LOAIPHIEUXUAT).FirstOrDefault();
                if (dm_LoaiPhieuXuat == null || string.IsNullOrEmpty(dm_LoaiPhieuXuat.ID))
                {
                    TempData["TitleError"] = "Không tìm thấy loại phiếu xuất";
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                if (dm_LoaiPhieuXuat.TYPE == 1)
                {
                    ct_PhieuXuat.lstdm_NhaCungCap = Utility.GetListData<ComboboxFrom>(API.dm_NhaCungCap, "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
                    apiResponse.TYPE = "divNCCEdit";
                }

                ct_PhieuXuat.lstdm_KhachHang = new List<ComboboxFrom>();
                if (dm_LoaiPhieuXuat.TYPE == 2)
                {
                    apiResponse.TYPE = "divKHACHHANGEdit";
                    ct_PhieuXuat.lstdm_KhachHang = Utility.GetListData<ComboboxFrom>(API.dm_KhachHang, "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
                }
                ct_PhieuXuat.lstdm_NhanVien = new List<ComboboxFrom>();
                if (dm_LoaiPhieuXuat.TYPE == 3)
                {
                    apiResponse.TYPE = "divNHANVIENEdit";
                    ct_PhieuXuat.lstdm_NhanVien = Utility.GetListData<ComboboxFrom>(API.dm_NhanVien, "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
                }
                ct_PhieuXuat.lstdm_Kho = new List<v_dm_Kho>();
                ct_PhieuXuat.lstdm_Kho = Utility.GetListData<v_dm_Kho>(API.dm_Kho, "", "", Utility.LOC_ID).Data as List<v_dm_Kho>;
                ct_PhieuXuat.lstdm_LoaiPhieuXuat = new List<v_dm_LoaiPhieuXuat>();
                ct_PhieuXuat.lstdm_LoaiPhieuXuat = lstdm_LoaiPhieuXuat;
                //ct_PhieuXuat.lstdm_NhanVien = new List<ComboboxFrom>();
                //ct_PhieuXuat.lstdm_NhanVien = Utility.GetListData<ComboboxFrom>(API.dm_NhanVien, "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
                var lst = Utility.ConvertobjectToView<v_v_ct_PhieuXuat>(ct_PhieuXuat);
                apiResponse.ProductCombo = Utility.GetProductInputOutput(lstOrderProduct, "Deposit_Temp");
                lst.Add(new ValueEdit { Key = "tbodyTempItemInputEdit", Value = apiResponse.ProductCombo });
                apiResponse.Detail = lst;
                return View(ct_PhieuXuat);
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
                TempData["TitleError"] = API.TitleTryCatch;
                TempData["DetailError"] = ex.Message;
                return RedirectToAction("Index", "Notfound");
            }
        }


        // POST: Deposit_TEMP/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult EditPopup([Bind(Include = "LOC_ID,ID,ID_LOAIPHIEUXUAT,ID_KHO,MAPHIEU,SOPHIEU,NGAYLAP,ID_KHACHHANG,ID_NHACUNGCAP,ID_NHANVIEN,GHICHU,TONGTHANHTIEN,CHIETKHAU,TONGTIENGIAMGIA,TONGTIENVAT,TONGTIEN,BUTTONTYPE,ADDRESS,TEL,CHUNGTUKEMTHEO")] v_v_ct_PhieuXuat ct_PhieuXuat)
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                if (Utility.KiemTra())
                {
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Admin");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                if (!Utility.KiemTraQuyen(API.ct_PhieuXuat, API.Create))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                List<Product_Detail> lstOrderProduct = new List<Product_Detail>();
                ct_PhieuXuat.lstct_PhieuXuat_ChiTiet = new List<v_ct_PhieuXuat_ChiTiet>();
                var lstKey = Request.Form.AllKeys.Where(e => e.StartsWith("txt"));
                if (lstKey == null || lstKey.Count() == 0)
                {
                    ModelState.AddModelError("lstct_PhieuXuat_ChiTiet", "Thêm danh sách hàng hóa.");
                }
                else
                {
                    v_ct_PhieuXuat_ChiTiet ct_PhieuXuat_ChiTiet = new v_ct_PhieuXuat_ChiTiet();
                    foreach (string Key in lstKey)
                    {
                        var lstString = Key.ToString().Split('|');
                        var value = HttpContext.Request.Params.GetValues(Key.ToString());
                        string ShowSearchValue = clsMaHoa.Decrypt(lstString[1].ToString(), clsMaHoa.PassMaHoa);
                        var Checkct_PhieuXuat_ChiTiet = JsonConvert.DeserializeObject<Product_Detail>(ShowSearchValue);
                        if (ct_PhieuXuat_ChiTiet.ID != Checkct_PhieuXuat_ChiTiet.ID)
                        {
                            ct_PhieuXuat_ChiTiet = new v_ct_PhieuXuat_ChiTiet();
                            ct_PhieuXuat_ChiTiet = JsonConvert.DeserializeObject<v_ct_PhieuXuat_ChiTiet>(ShowSearchValue);
                            ct_PhieuXuat_ChiTiet.LOC_ID = ct_PhieuXuat.LOC_ID;
                            ct_PhieuXuat.lstct_PhieuXuat_ChiTiet.Add(ct_PhieuXuat_ChiTiet);
                            lstOrderProduct.Add(Checkct_PhieuXuat_ChiTiet);
                        }
                        Utility.EditObject(ct_PhieuXuat_ChiTiet, lstString[0].ToString().Substring(3, lstString[0].ToString().Length - 3), value[0]);
                    }
                }

                if (ct_PhieuXuat.BUTTONTYPE == "GetPromotion")
                {
                    // Do Next Here
                    apiResponse = Utility.Create<List<v_ct_PhieuXuat_ChiTiet>>(ct_PhieuXuat.lstct_PhieuXuat_ChiTiet, API.ct_PhieuXuat + "/" + Utility.LOC_ID);
                    lstOrderProduct = JsonConvert.DeserializeObject<List<Product_Detail>>(apiResponse.Data.ToString());
                    Session[Sessions.lstProductInput] = lstOrderProduct;
                    apiResponse.GETPROMOTION = Utility.GetProductInputOutput(lstOrderProduct, "Deposit_Temp");

                    apiResponse.SOPHIEU = ct_PhieuXuat.SOPHIEU;
                    apiResponse.NewID = ct_PhieuXuat.ID;
                    apiResponse.MAPHIEU = ct_PhieuXuat.MAPHIEU;
                }
                if (ct_PhieuXuat.BUTTONTYPE == "Save")
                {
                    if (ModelState.IsValid)
                    {
                        apiResponse = Utility.GetDetail<v_ct_PhieuXuat>(Utility.LOC_ID + "/" + ct_PhieuXuat.ID, API.ct_PhieuXuat);
                        if (!apiResponse.Success)
                        {
                            TempData["TitleError"] = apiResponse.Message;
                            apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                            return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                        }
                        v_ct_PhieuXuat chkv_ct_PhieuXuat = null;
                        if (apiResponse.Data != null)
                            chkv_ct_PhieuXuat = apiResponse.Data as v_ct_PhieuXuat;

                        ct_PhieuXuat.LOC_ID = Utility.LOC_ID;
                        ct_PhieuXuat.ID_NGUOISUA = Session[Sessions.idUser].ToString();
                        ct_PhieuXuat.THOIGIANSUA = Utility.CurrentTime;

                        apiResponse = Utility.Edit<v_ct_PhieuXuat>(Utility.LOC_ID + "/" + ct_PhieuXuat.ID, ct_PhieuXuat, API.ct_PhieuXuat);
                        if (apiResponse.Success)
                        {
                            apiResponse.ID = ct_PhieuXuat.ID;
                            if (apiResponse.Data != null)
                                ct_PhieuXuat = JsonConvert.DeserializeObject<v_v_ct_PhieuXuat>(apiResponse.Data.ToString());

                            lstOrderProduct = new List<Product_Detail>();
                            //apiResponse.URL = Request.UrlReferrer.ToString();
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, apiResponse.Message);
                        }
                    }
                    else
                    {
                        apiResponse.Success = false;
                        apiResponse.Data = Utility.GetModelState(ModelState, API.ct_PhieuXuat);
                    }
                }
                Session[Sessions.lstProductInput] = lstOrderProduct;
                apiResponse.ID = ct_PhieuXuat.ID;
                ct_PhieuXuat.lstdm_NhaCungCap = new List<ComboboxFrom>();
                var lstdm_LoaiPhieuXuat = Utility.GetListData<v_dm_LoaiPhieuXuat>(API.dm_LoaiPhieuXuat, "", "", Utility.LOC_ID).Data as List<v_dm_LoaiPhieuXuat>;
                var dm_LoaiPhieuXuat = lstdm_LoaiPhieuXuat.Where(e => e.ID == ct_PhieuXuat.ID_LOAIPHIEUXUAT).FirstOrDefault();
                if (dm_LoaiPhieuXuat == null || string.IsNullOrEmpty(dm_LoaiPhieuXuat.ID))
                {
                    TempData["TitleError"] = "Không tìm thấy loại phiếu xuất";
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                if (dm_LoaiPhieuXuat.TYPE == 1)
                {
                    ct_PhieuXuat.lstdm_NhaCungCap = Utility.GetListData<ComboboxFrom>(API.dm_NhaCungCap, "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
                    apiResponse.TYPE = "divNCCEdit";
                }

                ct_PhieuXuat.lstdm_KhachHang = new List<ComboboxFrom>();
                if (dm_LoaiPhieuXuat.TYPE == 2)
                {
                    apiResponse.TYPE = "divKHACHHANGEdit";
                    ct_PhieuXuat.lstdm_KhachHang = Utility.GetListData<ComboboxFrom>(API.dm_KhachHang, "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
                }
                ct_PhieuXuat.lstdm_NhanVien = new List<ComboboxFrom>();
                if (dm_LoaiPhieuXuat.TYPE == 3)
                {
                    apiResponse.TYPE = "divNHANVIENEdit";
                    ct_PhieuXuat.lstdm_NhanVien = Utility.GetListData<ComboboxFrom>(API.dm_NhanVien, "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
                }
                ct_PhieuXuat.lstdm_Kho = new List<v_dm_Kho>();
                ct_PhieuXuat.lstdm_Kho = Utility.GetListData<v_dm_Kho>(API.dm_Kho, "", "", Utility.LOC_ID).Data as List<v_dm_Kho>;
                ct_PhieuXuat.lstdm_LoaiPhieuXuat = new List<v_dm_LoaiPhieuXuat>();
                ct_PhieuXuat.lstdm_LoaiPhieuXuat = lstdm_LoaiPhieuXuat;
                //ct_PhieuXuat.lstdm_NhanVien = new List<ComboboxFrom>();
                //ct_PhieuXuat.lstdm_NhanVien = Utility.GetListData<ComboboxFrom>(API.dm_NhanVien, "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
                var lst = Utility.ConvertobjectToView<v_v_ct_PhieuXuat>(ct_PhieuXuat, "dd/MM/yy HH:mm");
                apiResponse.ProductCombo = Utility.GetProductInputOutput(lstOrderProduct, "Deposit_Temp");
                lst.Add(new ValueEdit { Key = "tbodyTempItemInputEdit", Value = apiResponse.ProductCombo });
                apiResponse.Detail = lst;
                return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
                TempData["TitleError"] = API.TitleTryCatch;
                TempData["DetailError"] = ex.Message;
                return RedirectToAction("Index", "Notfound");
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
                if (!Utility.KiemTraQuyen(API.ct_PhieuXuat, API.Delete))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }

                apiResponse = Utility.Delete<v_ct_PhieuXuat>(Utility.LOC_ID + "/" + id, API.ct_PhieuXuat);
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
                v_ct_PhieuXuat PhieuNhap = new v_ct_PhieuXuat();
                SP_Parameter objParameter = new SP_Parameter();
                objParameter.ID_PHIEUXUAT = ID;
                apiResponse = Utility.ExecuteStoredProc<v_ct_PhieuXuat>(objParameter, API.Sp_Get_DanhSachPhieuXuat);
                if (!apiResponse.Success)
                {
                    apiResponse.Success = false;
                    apiResponse.Message = apiResponse.Message;
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                if (apiResponse.Data != null)
                    PhieuNhap = (apiResponse.Data as List<v_ct_PhieuXuat>).FirstOrDefault();

                SP_Parameter_Report objParameter_Report = new SP_Parameter_Report();
                objParameter_Report.LOC_ID = Utility.LOC_ID;
                objParameter_Report.ID_PHIEUXUAT = ID;
                var report = new ReportClass();
                
                apiResponse = Utility.ExecuteStoredProc<DataTable>(objParameter_Report, API.Sp_Get_DanhSachPhieuXuat_Chitiet);
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
                foreach(DataRow itm in data.Rows)
                {
                    if (data.Columns.Contains("ISKHUYENMAI") && Convert.ToBoolean(itm["ISKHUYENMAI"]) == true)
                    {
                        itm["NAME"] = "(KM)" + itm["NAME"];
                    }
                    if (data.Columns.Contains("TONGTIENGIAMGIA") && Convert.ToDecimal(itm["TONGTIENGIAMGIA"]) < 0)
                    {
                        itm["TONGTIENVAT"] = Convert.ToDecimal(itm["TONGTIENGIAMGIA"]) + Convert.ToDecimal(itm["TONGTIENVAT"]);
                        itm["TONGTIENGIAMGIA"] = 0;

                    }
                }    
                if (apiResponse.CheckValue)
                    data.Rows.Clear();

                PhieuNhap.TONGTIENNO = "";
                PhieuNhap.GHICHU = "";
                if (PhieuNhap != null && !string.IsNullOrEmpty( PhieuNhap.ID_KHACHHANG))
                {
                    SP_Parameter sp_Parameter = new SP_Parameter();
                    //ApiResponse apiResponse = new ApiResponse();
                    sp_Parameter.LOC_ID = Utility.LOC_ID;
                    sp_Parameter.ID_KHACHHANG = PhieuNhap.ID_KHACHHANG.ToString();
                    sp_Parameter.ISTHEOTHOIGIAN = false;
                    sp_Parameter.ISPHATSINHCONGNO = false;
                    sp_Parameter.ISPHATSINHCONGNOTRONGKY = false;
                    sp_Parameter.ISCONCONGNO = false;
                    apiResponse = Utility.Get_ThongKeCongNoKhachHang<v_ThongKeCongNoKhachHang>(sp_Parameter);
                    if (!apiResponse.Success)
                    {
                        TempData["TitleError"] = apiResponse.Message;
                        return RedirectToAction("Index", "Notfound");
                    }
                    if (apiResponse.Data != null)
                    {
                        var CongNo = (apiResponse.Data as List<v_ThongKeCongNoKhachHang>).FirstOrDefault();
                        apiResponse = Utility.GetDetail<v_v_ct_PhieuXuat>(Utility.LOC_ID + "/" + PhieuNhap.ID, API.ct_PhieuXuat);
                        if (!apiResponse.Success)
                        {
                            TempData["TitleError"] = apiResponse.Message;
                            return RedirectToAction("Index", "Notfound");
                        }
                        v_v_ct_PhieuXuat PhieuNhapols = new v_v_ct_PhieuXuat();
                        if (apiResponse.Data != null)
                            PhieuNhapols = apiResponse.Data as v_v_ct_PhieuXuat;
                        double CongNoMoi = PhieuNhapols.lstct_PhieuXuat_ChiTiet.Sum(e => e.TONGCONG);
                        if (CongNo != null && CongNo.TONGTIENCONGNOCUOIKY - CongNoMoi > 0)
                        {
                            PhieuNhap.TONGTIENNO = "Nợ cũ: " + (CongNo.TONGTIENCONGNOCUOIKY - CongNoMoi).ToString("N0");
                            PhieuNhap.GHICHU = "Tổng tiền: " + (CongNo.TONGTIENCONGNOCUOIKY).ToString("N0");
                        }
                    }
                }    
                
                report = Utility.GetFormulaFields(report, PhieuNhap);
                report.SetDataSource(data);
                Response.Buffer = false;
                Response.ClearContent();
                Response.ClearHeaders();
                Stream stream = report.ExportToStream(ExportFormatType.PortableDocFormat);
                Utility.Report = report;
                apiResponse = new ApiResponse();
                apiResponse.Success = true;
                string url = Request.Url.Authority;
                if(Request.Url.AbsoluteUri.StartsWith("https"))
                    apiResponse.URL = "https://" + url + "/ViewReport/VerReporte";
                else
                    apiResponse.URL = "http://" + url + "/ViewReport/VerReporte";
                apiResponse.NAME =  Utility.GetTitleFrom(API.ct_PhieuXuat) + " - " + PhieuNhap.MAPHIEU;
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