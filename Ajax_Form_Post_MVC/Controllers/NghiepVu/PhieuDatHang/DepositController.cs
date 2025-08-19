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
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System.Data;
using System.IO;
using static System.Data.Entity.Infrastructure.Design.Executor;
using Syncfusion.EJ2.Maps;
using CrystalDecisions.ReportAppServer.DataDefModel;
using System.Data.SqlClient;

namespace MVC_QuanLyTHP.Controllers
{
    public class DepositController : Controller
    {
        // GET: Deposit_Temp
        #region Deposit_Temp
        public ActionResult Index(int Page = 1, string ID_DEPOT = "", string ID_KHUVUC = "", DateTime? FromDate = null, DateTime? ToDate = null, string SearchString = "", string ShowSearchValue = "", string MAPHIEU = "", string IDCODE = "")
        {
            try
            {
                if (Utility.KiemTra())
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (!Utility.KiemTraQuyen(API.ct_PhieuDatHang, API.Xem))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                ShowSearchValue = Utility.GetShowSearchValue<ct_PhieuDatHang>(ShowSearchValue);
                ApiResponse apiResponse = new ApiResponse();
                IPagedList<v_ct_PhieuDatHang> lstpage = (new List<v_ct_PhieuDatHang>()).OrderByDescending(s => s.MAPHIEU).ToList().ToPagedList(Page, Utility.GetPageSize()); ;
                if (FromDate != null || !string.IsNullOrEmpty(IDCODE))
                {
                    if (!string.IsNullOrEmpty(IDCODE))
                    {
                        apiResponse = Utility.Get_DanhSachPhieuDatHang<v_ct_PhieuDatHang>("", null, null, MAPHIEU, IDCODE,ID_KHUVUC);
                    }
                    if (FromDate != null)
                    {
                        apiResponse = Utility.Get_DanhSachPhieuDatHang<v_ct_PhieuDatHang>(ID_DEPOT, FromDate, ToDate, SearchString,"", ID_KHUVUC);
                    }
                  
                    //apiResponse = Utility.GetListDataOrder<v_ct_PhieuDatHang>(API.ct_PhieuDatHang, FromDate, ToDate, ShowSearchValue, SearchString, Utility.LOC_ID);
                    if (!apiResponse.Success)
                    {
                        TempData["TitleError"] = apiResponse.Message;
                        return RedirectToAction("Index", "Notfound");
                    }
                    if (apiResponse.Data != null)
                    {
                        var Login_Model = (Login_Model)Session[Sessions.Login_Model];
                        if (Utility.KiemTraQuyen(API.ct_PhieuDatHang, API.AllData))
                        {
                            lstpage = (apiResponse.Data as List<v_ct_PhieuDatHang>).OrderByDescending(s => s.MAPHIEU).ToList().ToPagedList(Page, Utility.GetPageSize());
                        }
                        else
                        {
                            if (Utility.KiemTraQuyen(API.ct_PhieuDatHang, API.UserData))
                                lstpage = (apiResponse.Data as List<v_ct_PhieuDatHang>).Where(s => s.ID_NHANVIEN == Login_Model.iduser).OrderByDescending(s => s.MAPHIEU).ToList().ToPagedList(Page, Utility.GetPageSize());
                        }
                    }
                }
               

                v_v_ct_PhieuDatHang ct_PhieuDatHang = new v_v_ct_PhieuDatHang();
                ct_PhieuDatHang.IPagedList = lstpage;
                ct_PhieuDatHang.lstdm_ThueSuat = new List<v_dm_ThueSuat>();
                ct_PhieuDatHang.lstdm_ThueSuat = Utility.GetListData<v_dm_ThueSuat>(API.dm_ThueSuat, "", "", Utility.LOC_ID).Data as List<v_dm_ThueSuat>;
                ct_PhieuDatHang.lstdm_Kho = new List<v_dm_Kho>();
                ct_PhieuDatHang.lstdm_Kho = Utility.GetListData<v_dm_Kho>(API.dm_Kho, "", "", Utility.LOC_ID).Data as List<v_dm_Kho>;
                ct_PhieuDatHang.lstdm_KhuVuc = new List<v_dm_KhuVuc>();
                ct_PhieuDatHang.lstdm_KhuVuc = Utility.GetListData<v_dm_KhuVuc>(API.dm_KhuVuc, "", "", Utility.LOC_ID).Data as List<v_dm_KhuVuc>;
                //ct_PhieuDatHang.lstdm_KhachHang = new List<v_dm_KhachHang>();
                //ct_PhieuDatHang.lstdm_KhachHang = Utility.GetListData<v_dm_KhachHang>(API.dm_KhachHang, "", "", Utility.LOC_ID).Data as List<v_dm_KhachHang>;
                //ct_PhieuDatHang.lstdm_NhanVien = new List<v_dm_NhanVien>();
                //ct_PhieuDatHang.lstdm_NhanVien = Utility.GetListData<v_dm_NhanVien>(API.dm_NhanVien, "", "", Utility.LOC_ID).Data as List<v_dm_NhanVien>;
                ct_PhieuDatHang.ID_KHUVUC = ID_KHUVUC;
                ViewBag.ID_KHO_DF = string.IsNullOrEmpty(ID_DEPOT) ? ct_PhieuDatHang.lstdm_Kho.FirstOrDefault(e => e.ISDEFAULT).ID : ID_DEPOT;
                ViewBag.searchValue = SearchString;
                ViewBag.showsearchValue = ShowSearchValue;
                ViewBag.fromdate = FromDate != null ? FromDate.Value.ToString("yyyy-MM-dd") : Utility.CurrentTime.ToString("yyyy-MM-dd");
                ViewBag.todate = ToDate != null ? ToDate.Value.ToString("yyyy-MM-dd") : Utility.CurrentTime.ToString("yyyy-MM-dd");
                ViewBag.PermissionEdit = Utility.KiemTraQuyen(API.ct_PhieuDatHang, API.Edit);
                ViewBag.PermissionDelete = Utility.KiemTraQuyen(API.ct_PhieuDatHang, API.Delete);
                ViewBag.PermissionCreate = Utility.KiemTraQuyen(API.ct_PhieuDatHang, API.Create);
                ViewBag.PermissionCreateInput = Utility.KiemTraQuyen(API.ct_PhieuDatHang, API.CreateInput);
                return View(ct_PhieuDatHang);
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
                if (!Utility.KiemTraQuyen(API.ct_PhieuDatHang, API.Xem))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                v_v_ct_PhieuDatHang ct_PhieuDatHang = new v_v_ct_PhieuDatHang();
                if (type == 2)
                {
                    ct_PhieuDatHang.NGAYLAP = Utility.CurrentTime;
                    ct_PhieuDatHang.LOC_ID = Utility.LOC_ID;
                    ct_PhieuDatHang.SOPHIEU = Utility.GetMaxID<ct_PhieuDatHang>(ct_PhieuDatHang, Utility.LOC_ID, ct_PhieuDatHang.NGAYLAP.ToString("yyyy-MM-dd"));
                    ct_PhieuDatHang.MAPHIEU = API.GetMaPhieu(API.ct_PhieuDatHang, ct_PhieuDatHang.NGAYLAP, ct_PhieuDatHang.SOPHIEU);
                    ct_PhieuDatHang.lstdm_ThueSuat = new List<v_dm_ThueSuat>();
                    ct_PhieuDatHang.lstdm_ThueSuat = Utility.GetListData<v_dm_ThueSuat>(API.dm_ThueSuat, "", "", Utility.LOC_ID).Data as List<v_dm_ThueSuat>;
                    ct_PhieuDatHang.lstdm_Kho = new List<v_dm_Kho>();
                    ct_PhieuDatHang.lstdm_Kho = Utility.GetListData<v_dm_Kho>(API.dm_Kho, "", "", Utility.LOC_ID).Data as List<v_dm_Kho>;
                    ct_PhieuDatHang.lstdm_KhachHang = new List<ComboboxFrom>();
                    ct_PhieuDatHang.lstdm_KhachHang = GetDanhSachKhachHangCombobox().Data as List<ComboboxFrom>;
                    ct_PhieuDatHang.lstAspNetUsers = new List<v_AspNetUsers>();
                    ct_PhieuDatHang.lstAspNetUsers = Utility.GetListData<v_AspNetUsers>(API.AspNetUser, "", "", "").Data as List<v_AspNetUsers>;
                    var KhoISDEFAULT = ct_PhieuDatHang.lstdm_Kho.Where(e => e.ISDEFAULT).FirstOrDefault();
                    if (KhoISDEFAULT != null)
                    {
                        ct_PhieuDatHang.ID_KHO = KhoISDEFAULT.ID;
                    }
                }
                else
                {
                    //ct_PhieuDatHang.NGAYLAP = Utility.CurrentTime;
                    //ct_PhieuDatHang.LOC_ID = Utility.LOC_ID;
                    //ct_PhieuDatHang.SOPHIEU = Utility.GetMaxID<ct_PhieuDatHang>(ct_PhieuDatHang, Utility.LOC_ID, ct_PhieuDatHang.NGAYLAP.ToString("yyyy-MM-dd"));
                    //ct_PhieuDatHang.MAPHIEU = API.GetMaPhieu(API.ct_PhieuDatHang, ct_PhieuDatHang.NGAYLAP, ct_PhieuDatHang.SOPHIEU);
                    ct_PhieuDatHang.lstdm_ThueSuat = new List<v_dm_ThueSuat>();
                    //ct_PhieuDatHang.lstdm_ThueSuat = Utility.GetListData<v_dm_ThueSuat>(API.dm_ThueSuat, "", "", Utility.LOC_ID).Data as List<v_dm_ThueSuat>;
                    ct_PhieuDatHang.lstdm_Kho = new List<v_dm_Kho>();
                    //ct_PhieuDatHang.lstdm_Kho = Utility.GetListData<v_dm_Kho>(API.dm_Kho, "", "", Utility.LOC_ID).Data as List<v_dm_Kho>;
                    ct_PhieuDatHang.lstdm_KhachHang = new List<ComboboxFrom>();
                    //ct_PhieuDatHang.lstdm_KhachHang = GetDanhSachKhachHang().Data as List<v_dm_KhachHang>;
                    ct_PhieuDatHang.lstAspNetUsers = new List<v_AspNetUsers>();
                    //var KhoISDEFAULT = ct_PhieuDatHang.lstdm_Kho.Where(e => e.ISDEFAULT).FirstOrDefault();
                    //if (KhoISDEFAULT != null)
                    //{
                    //    ct_PhieuDatHang.ID_KHO = KhoISDEFAULT.ID;
                    //}
                }
                List<Product_Detail> lstOrderProduct = new List<Product_Detail>();
                Session[Sessions.lstProductInput] = lstOrderProduct;
                ApiResponse apiResponse = GetDanhSachNhomSanPham();
                string sbtn = "";
                if (!apiResponse.Success)
                {
                    TempData["TitleError"] = apiResponse.Message;
                    return RedirectToAction("Index", "Notfound");
                }
                else
                {
                    var rls = apiResponse.Data as List<web_Sp_Get_DSNhomSanPham_Result>;
                    sbtn = "<button class='btnGroup active' onclick='myFunctionPage(\"all\", \"\",\"collapseOneDeposit\")' id= \"all\">Show all</button>";
                    foreach (web_Sp_Get_DSNhomSanPham_Result itm in rls)
                    {
                        sbtn += "<button class='btnGroup' onclick='myFunctionPage(\"" + itm.ID + "\", \"\",\"collapseOneDeposit\")' id= \"" + itm.ID + "\"> " + itm.NAME + "</button>";
                    }
                    if (!string.IsNullOrEmpty(sbtn))
                        sbtn += "<button class='btnGroup' onclick='myFunctionLoadGroup(\"collapseOneDeposit\")'><span class='glyphicon glyphicon-refresh'></span></button>";
                }
                sbtn = myProduct(sbtn, "collapseOneDeposit");
                ViewBag.NhomHang = sbtn;
                ViewBag.PermissionCreateUser = Utility.KiemTraQuyen(API.ct_PhieuDatHang, API.CreateUser);
                return View(ct_PhieuDatHang);
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
        public ActionResult CreatePopup([Bind(Include = "LOC_ID,ID,ID_KHO,MAPHIEU,SOPHIEU,NGAYLAP,ID_KHACHHANG,ID_NHANVIEN,GHICHU,TONGTHANHTIEN,CHIETKHAU,TONGTIENGIAMGIA,TONGTIENVAT,TONGTIEN,BUTTONTYPE,ADDRESS,TEL")] v_v_ct_PhieuDatHang ct_PhieuDatHang)
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                if (Utility.KiemTra())
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (!Utility.KiemTraQuyen(API.ct_PhieuDatHang, API.Create))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }

                ct_PhieuDatHang.lstct_PhieuDatHang_ChiTiet = new List<v_ct_PhieuDatHang_ChiTiet>();
                List<Product_Detail> lstOrderProduct = new List<Product_Detail>();
                var lstKey = Request.Form.AllKeys.Where(e => e.StartsWith("txt"));
                if (lstKey == null || lstKey.Count() == 0)
                {
                    ModelState.AddModelError("lstct_PhieuDatHang_ChiTiet", "Thêm danh sách hàng hóa.");
                }
                else
                {
                    v_ct_PhieuDatHang_ChiTiet ct_PhieuDatHang_ChiTiet = new v_ct_PhieuDatHang_ChiTiet();
                    foreach (string Key in lstKey)
                    {
                        var lstString = Key.ToString().Split('|');
                        var value = HttpContext.Request.Params.GetValues(Key.ToString());
                        string ShowSearchValue = clsMaHoa.Decrypt(lstString[1].ToString(), clsMaHoa.PassMaHoa);
                        var Checkct_PhieuDatHang_ChiTiet = JsonConvert.DeserializeObject<Product_Detail>(ShowSearchValue);
                        
                        if (ct_PhieuDatHang_ChiTiet.ID != Checkct_PhieuDatHang_ChiTiet.ID)
                        {
                            ct_PhieuDatHang_ChiTiet = new v_ct_PhieuDatHang_ChiTiet();
                            ct_PhieuDatHang_ChiTiet = JsonConvert.DeserializeObject<v_ct_PhieuDatHang_ChiTiet>(ShowSearchValue);
                            ct_PhieuDatHang_ChiTiet.LOC_ID = ct_PhieuDatHang.LOC_ID;
                            ct_PhieuDatHang.lstct_PhieuDatHang_ChiTiet.Add(ct_PhieuDatHang_ChiTiet);
                            lstOrderProduct.Add(Checkct_PhieuDatHang_ChiTiet);
                        }
                        Utility.EditObject(ct_PhieuDatHang_ChiTiet, lstString[0].ToString().Substring(3, lstString[0].ToString().Length - 3), value[0]);
                    }
                }
               
                if (ct_PhieuDatHang.BUTTONTYPE == "GetPromotion")
                {
                    // Do Next Here
                    apiResponse = Utility.Create<List<v_ct_PhieuDatHang_ChiTiet>>(ct_PhieuDatHang.lstct_PhieuDatHang_ChiTiet, API.ct_PhieuDatHang + "/" + Utility.LOC_ID);
                    lstOrderProduct = JsonConvert.DeserializeObject<List<Product_Detail>>(apiResponse.Data.ToString());
                    Session[Sessions.lstProductInput] = lstOrderProduct;
                    apiResponse.GETPROMOTION = Utility.GetProductInputOutput(lstOrderProduct, "Deposit_Temp");

                    apiResponse.SOPHIEU = ct_PhieuDatHang.SOPHIEU = Utility.GetMaxID<ct_PhieuDatHang>(ct_PhieuDatHang, Utility.LOC_ID, ct_PhieuDatHang.NGAYLAP.ToString("yyyy-MM-dd"));
                    ct_PhieuDatHang.MAPHIEU = API.GetMaPhieu(API.ct_PhieuDatHang, ct_PhieuDatHang.NGAYLAP, ct_PhieuDatHang.SOPHIEU);
                    apiResponse.NewID = ct_PhieuDatHang.ID;
                    apiResponse.MAPHIEU = ct_PhieuDatHang.MAPHIEU;
                }
                if (ct_PhieuDatHang.BUTTONTYPE == "Save")
                {
                    if(!string.IsNullOrEmpty( ct_PhieuDatHang.ID_KHACHHANG))
                    {
                        string CongNo = CheckCongNoKhachHang(ct_PhieuDatHang.ID_KHACHHANG, ct_PhieuDatHang.TONGTIEN);
                       if (!string.IsNullOrEmpty(CongNo))
                            ModelState.AddModelError("ID_KHACHHANG", "Công nợ vượt: " + CongNo);
                    }    
                    if (ModelState.IsValid)
                    {
                        ct_PhieuDatHang.NGAYLAP = ct_PhieuDatHang.NGAYLAP.AddHours(Utility.CurrentTime.Hour).AddMinutes(Utility.CurrentTime.Minute);
                        ct_PhieuDatHang.ID = Guid.NewGuid().ToString();
                        ct_PhieuDatHang.LOC_ID = Utility.LOC_ID;
                        ct_PhieuDatHang.ID_NGUOITAO = Session[Sessions.idUser].ToString();
                        ct_PhieuDatHang.THOIGIANTHEM = Utility.CurrentTime;
                        if(string.IsNullOrEmpty(ct_PhieuDatHang.ID_NHANVIEN))
                            ct_PhieuDatHang.ID_NHANVIEN = Session[Sessions.idUser].ToString();
                        apiResponse = Utility.Create<v_ct_PhieuDatHang>(ct_PhieuDatHang, API.ct_PhieuDatHang);
                        if (apiResponse.Success)
                        {
                            ct_PhieuDatHang.NGAYLAP = Utility.CurrentTime;
                            apiResponse.SOPHIEU = ct_PhieuDatHang.SOPHIEU = Utility.GetMaxID<ct_PhieuDatHang>(ct_PhieuDatHang, Utility.LOC_ID, ct_PhieuDatHang.NGAYLAP.ToString("yyyy-MM-dd"));
                            ct_PhieuDatHang.MAPHIEU = API.GetMaPhieu(API.ct_PhieuDatHang, ct_PhieuDatHang.NGAYLAP, ct_PhieuDatHang.SOPHIEU);
                            apiResponse.NewID = Guid.NewGuid().ToString();
                            apiResponse.MAPHIEU = ct_PhieuDatHang.MAPHIEU;

                            if (apiResponse.Data != null)
                                ct_PhieuDatHang = JsonConvert.DeserializeObject<v_v_ct_PhieuDatHang>(apiResponse.Data.ToString());

                            lstOrderProduct = new List<Product_Detail>();
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, apiResponse.Message);
                            if (apiResponse.CheckValue)
                            {
                                ct_PhieuDatHang.NGAYLAP = Utility.CurrentTime;
                                apiResponse.SOPHIEU = ct_PhieuDatHang.SOPHIEU = Utility.GetMaxID<ct_PhieuDatHang>(ct_PhieuDatHang, Utility.LOC_ID, ct_PhieuDatHang.NGAYLAP.ToString("yyyy-MM-dd"));
                                ct_PhieuDatHang.MAPHIEU = API.GetMaPhieu(API.ct_PhieuDatHang, ct_PhieuDatHang.NGAYLAP, ct_PhieuDatHang.SOPHIEU);
                                apiResponse.NewID = Guid.NewGuid().ToString();
                                apiResponse.MAPHIEU = ct_PhieuDatHang.MAPHIEU;
                            }
                        }
                    }
                    else
                    {
                        apiResponse.Success = false;
                        apiResponse.Data = Utility.GetModelState(ModelState, API.ct_PhieuDatHang);
                    }
                }

                Session[Sessions.lstProductInput] = lstOrderProduct;
                apiResponse.ID = ct_PhieuDatHang.ID;
                ct_PhieuDatHang.lstdm_KhachHang = new List<ComboboxFrom>();
                ct_PhieuDatHang.lstdm_KhachHang = GetDanhSachKhachHangCombobox().Data as List<ComboboxFrom>;
                ct_PhieuDatHang.lstdm_Kho = new List<v_dm_Kho>();
                ct_PhieuDatHang.lstdm_Kho = Utility.GetListData<v_dm_Kho>(API.dm_Kho, "", "", Utility.LOC_ID).Data as List<v_dm_Kho>;
                ct_PhieuDatHang.lstAspNetUsers = new List<v_AspNetUsers>();
                ct_PhieuDatHang.lstAspNetUsers = Utility.GetListData<v_AspNetUsers>(API.AspNetUser, "", "", "").Data as List<v_AspNetUsers>;
                var lst = Utility.ConvertobjectToView<v_v_ct_PhieuDatHang>(ct_PhieuDatHang);

                apiResponse.ProductCombo = Utility.GetProductInputOutput(lstOrderProduct, "Deposit_Temp");

                apiResponse.GETPROMOTION = apiResponse.ProductCombo;
                lst.Add(new ValueEdit { Key = "tbodyTempItemInput", Value = apiResponse.ProductCombo });
                apiResponse.Detail = lst;
                apiResponse.TYPE = ct_PhieuDatHang.BUTTONTYPE;
                return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
            }
            catch (Exception ex)
            {
                apiResponse.Success = false;
                apiResponse.Message = ex.Message;
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
                TempData["TitleError"] = API.TitleTryCatch;
                TempData["DetailError"] = ex.Message;
                return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                //return RedirectToAction("Index", "Notfound");
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
                if (!Utility.KiemTraQuyen(API.ct_PhieuDatHang, API.Edit))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                List<Product_Detail> lstOrderProduct = new List<Product_Detail>();
                v_v_ct_PhieuDatHang ct_PhieuDatHang = new v_v_ct_PhieuDatHang();
                if (!string.IsNullOrEmpty(id))
                {
                    apiResponse = Utility.GetDetail<v_v_ct_PhieuDatHang>(Utility.LOC_ID + "/" + id, API.ct_PhieuDatHang);
                    if (!apiResponse.Success)
                    {
                        TempData["TitleError"] = apiResponse.Message;
                        return RedirectToAction("Index", "Notfound");
                    }
                    if (apiResponse.Data != null)
                        ct_PhieuDatHang = apiResponse.Data as v_v_ct_PhieuDatHang;

                    foreach (var itm in ct_PhieuDatHang.lstct_PhieuDatHang_ChiTiet)
                    {
                        lstOrderProduct.Add(Utility.ConvertobjectToProduct_Detail<v_ct_PhieuDatHang_ChiTiet>(itm, new Product_Detail()));
                    }

                    if (!string.IsNullOrEmpty(ct_PhieuDatHang.ID_PHIEUXUAT))
                    {
                        TempData["TitleError"] = "Phiếu đã thực hiện đặt hàng! Nên không thể sửa phiếu!";
                        return RedirectToAction("Index", "Notfound");
                    }

                }

                ct_PhieuDatHang.lstdm_Kho = new List<v_dm_Kho>();
                //ct_PhieuDatHang.lstdm_Kho = Utility.GetListData<v_dm_Kho>(API.dm_Kho, "", "", Utility.LOC_ID).Data as List<v_dm_Kho>;
                ct_PhieuDatHang.lstdm_KhachHang = new List<ComboboxFrom>();
                //ct_PhieuDatHang.lstdm_KhachHang = GetDanhSachKhachHang().Data as List<v_dm_KhachHang>;
                ct_PhieuDatHang.lstdm_ThueSuat = new List<v_dm_ThueSuat>();
                //ct_PhieuDatHang.lstdm_ThueSuat = Utility.GetListData<v_dm_ThueSuat>(API.dm_ThueSuat, "", "", Utility.LOC_ID).Data as List<v_dm_ThueSuat>;
                apiResponse = GetDanhSachNhomSanPham();
                string sbtn = "";
                if (!apiResponse.Success)
                {
                    TempData["TitleError"] = apiResponse.Message;
                    return RedirectToAction("Index", "Notfound");
                }
                else
                {
                    var rls = apiResponse.Data as List<web_Sp_Get_DSNhomSanPham_Result>;
                    sbtn = "<button class='btnGroup active' onclick='myFunctionPage(\"all\", \"\",\"collapseOneDepositEdit\")' id= \"all\">Show all</button>";
                    foreach (web_Sp_Get_DSNhomSanPham_Result itm in rls)
                    {
                        sbtn += "<button class='btnGroup' onclick='myFunctionPage(\"" + itm.ID + "\", \"\",\"collapseOneDepositEdit\")' id= \"" + itm.ID + "\"> " + itm.NAME + "</button>";
                    }
                    if (!string.IsNullOrEmpty(sbtn))
                        sbtn += "<button class='btnGroup' onclick='myFunctionLoadGroup(\"collapseOneDepositEdit\")'><span class='glyphicon glyphicon-refresh'></span></button>";
                }
                Session[Sessions.lstProductInput] = lstOrderProduct;
                ViewBag.NhomHang = sbtn;

                apiResponse.ProductCombo = Utility.GetProductInputOutput(lstOrderProduct, "Deposit_Temp");
                ViewBag.DatHang = apiResponse.ProductCombo;
                //foreach (string printer in System.Drawing.Printing.PrinterSettings.InstalledPrinters)
                //{

                //} 
                return View(ct_PhieuDatHang);
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
                if (!Utility.KiemTraQuyen(API.ct_PhieuDatHang, API.Edit))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                List<Product_Detail> lstOrderProduct = new List<Product_Detail>();
                v_v_ct_PhieuDatHang ct_PhieuDatHang = new v_v_ct_PhieuDatHang();
                if (!string.IsNullOrEmpty(id))
                {
                    apiResponse = Utility.GetDetail<v_v_ct_PhieuDatHang>(Utility.LOC_ID + "/" + id, API.ct_PhieuDatHang);
                    if (!apiResponse.Success)
                    {
                        TempData["TitleError"] = apiResponse.Message;
                        return RedirectToAction("Index", "Notfound");
                    }
                    if (apiResponse.Data != null)
                        ct_PhieuDatHang = apiResponse.Data as v_v_ct_PhieuDatHang;
                }
                foreach (var itm in ct_PhieuDatHang.lstct_PhieuDatHang_ChiTiet)
                {
                    lstOrderProduct.Add(Utility.ConvertobjectToProduct_Detail<v_ct_PhieuDatHang_ChiTiet>(itm, new Product_Detail()));
                }

                if (!string.IsNullOrEmpty(ct_PhieuDatHang.ID_PHIEUXUAT))
                {
                    TempData["TitleError"] = "Phiếu đã thực hiện đặt hàng! Nên không thể sửa phiếu!";
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }

                ct_PhieuDatHang.lstdm_Kho = new List<v_dm_Kho>();
                ct_PhieuDatHang.lstdm_Kho = Utility.GetListData<v_dm_Kho>(API.dm_Kho, "", "", Utility.LOC_ID).Data as List<v_dm_Kho>;
                ct_PhieuDatHang.lstdm_KhachHang = new List<ComboboxFrom>();
                ct_PhieuDatHang.lstdm_KhachHang = GetDanhSachKhachHangCombobox().Data as List<ComboboxFrom>;

                if(ct_PhieuDatHang.lstdm_KhachHang != null)
                {
                    var KhanhHang = ct_PhieuDatHang.lstdm_KhachHang.Where(e => e.ID == ct_PhieuDatHang.ID_KHACHHANG).FirstOrDefault();
                    if(KhanhHang == null)
                    {
                       var kh =  Utility.GetDetail<ComboboxFrom>(Utility.LOC_ID + "/" + ct_PhieuDatHang.ID_KHACHHANG, API.dm_KhachHang);
                        if (!kh.Success)
                        {
                            TempData["TitleError"] = kh.Message;
                            return RedirectToAction("Index", "Notfound");
                        }
                        if (kh.Data != null)
                            ct_PhieuDatHang.lstdm_KhachHang.Add(kh.Data as ComboboxFrom);
                    }
                }
                ct_PhieuDatHang.lstdm_ThueSuat = new List<v_dm_ThueSuat>();
                ct_PhieuDatHang.lstdm_ThueSuat = Utility.GetListData<v_dm_ThueSuat>(API.dm_ThueSuat, "", "", Utility.LOC_ID).Data as List<v_dm_ThueSuat>;
                ct_PhieuDatHang.lstAspNetUsers = new List<v_AspNetUsers>();
                ct_PhieuDatHang.lstAspNetUsers = Utility.GetListData<v_AspNetUsers>(API.AspNetUser, "", "", "").Data as List<v_AspNetUsers>;
                Session[Sessions.lstProductInput] = lstOrderProduct;
                var lst = Utility.ConvertobjectTo<v_v_ct_PhieuDatHang>(ct_PhieuDatHang);
                apiResponse.ProductCombo = Utility.GetProductInputOutput(lstOrderProduct, "Deposit_Temp", false, ct_PhieuDatHang.TONGTIENGIAMGIA, ct_PhieuDatHang.TONGTHANHTIEN, ct_PhieuDatHang.TONGTIENVAT, ct_PhieuDatHang.TONGTIEN);
                lst.Add(new ValueEdit { Key = "tbodyTempItemInputEdit", Value = apiResponse.ProductCombo });

                apiResponse = GetDanhSachNhomSanPham();
                string sbtn = "";
                if (!apiResponse.Success)
                {
                    TempData["TitleError"] = apiResponse.Message;
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                else
                {
                    var rls = apiResponse.Data as List<web_Sp_Get_DSNhomSanPham_Result>;
                    sbtn = "<button class='btnGroup active' onclick='myFunctionPage(\"all\", \"\",\"collapseOneDepositEdit\")' id= \"all\">Show all</button>";
                    foreach (web_Sp_Get_DSNhomSanPham_Result itm in rls)
                    {
                        sbtn += "<button class='btnGroup' onclick='myFunctionPage(\"" + itm.ID + "\", \"\",\"collapseOneDepositEdit\")' id= \"" + itm.ID + "\"> " + itm.NAME + "</button>";
                    }
                    if (!string.IsNullOrEmpty(sbtn))
                        sbtn += "<button class='btnGroup' onclick='myFunctionLoadGroup(\"collapseOneDepositEdit\")'><span class='glyphicon glyphicon-refresh'></span></button>";
                }
                ViewBag.NhomHang = sbtn;
                lst.Add(new ValueEdit { Key = "myProductEdit", Value = myProduct(sbtn, "collapseOneDepositEdit") });
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
        public ActionResult Edit([Bind(Include = "LOC_ID,ID,ID_KHO,MAPHIEU,SOPHIEU,NGAYLAP,ID_KHACHHANG,ID_NHANVIEN,GHICHU,TONGTHANHTIEN,CHIETKHAU,TONGTIENGIAMGIA,TONGTIENVAT,TONGTIEN,BUTTONTYPE,ADDRESS,TEL")] v_v_ct_PhieuDatHang ct_PhieuDatHang)
        {
            try
            {
                if (Utility.KiemTra())
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (!Utility.KiemTraQuyen(API.ct_PhieuDatHang, API.Create))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                List<Product_Detail> lstOrderProduct = new List<Product_Detail>();
                ApiResponse apiResponse = new ApiResponse();
                ct_PhieuDatHang.lstct_PhieuDatHang_ChiTiet = new List<v_ct_PhieuDatHang_ChiTiet>();
                var lstKey = Request.Form.AllKeys.Where(e => e.StartsWith("txt"));
                if (lstKey == null || lstKey.Count() == 0)
                {
                    ModelState.AddModelError("lstct_PhieuDatHang_ChiTiet", "Thêm danh sách hàng hóa.");
                }
                else
                {
                    v_ct_PhieuDatHang_ChiTiet ct_PhieuDatHang_ChiTiet = new v_ct_PhieuDatHang_ChiTiet();
                    foreach (string Key in lstKey)
                    {
                        var lstString = Key.ToString().Split('|');
                        var value = HttpContext.Request.Params.GetValues(Key.ToString());
                        string ShowSearchValue = clsMaHoa.Decrypt(lstString[1].ToString(), clsMaHoa.PassMaHoa);
                        var Checkct_PhieuDatHang_ChiTiet = JsonConvert.DeserializeObject<Product_Detail>(ShowSearchValue);
                        
                        if (ct_PhieuDatHang_ChiTiet.ID != Checkct_PhieuDatHang_ChiTiet.ID)
                        {
                            ct_PhieuDatHang_ChiTiet = new v_ct_PhieuDatHang_ChiTiet();
                            ct_PhieuDatHang_ChiTiet = JsonConvert.DeserializeObject<v_ct_PhieuDatHang_ChiTiet>(ShowSearchValue);
                            ct_PhieuDatHang_ChiTiet.LOC_ID = ct_PhieuDatHang.LOC_ID;
                            ct_PhieuDatHang.lstct_PhieuDatHang_ChiTiet.Add(ct_PhieuDatHang_ChiTiet);
                            lstOrderProduct.Add(Checkct_PhieuDatHang_ChiTiet);
                        }
                        Utility.EditObject(ct_PhieuDatHang_ChiTiet, lstString[0].ToString().Substring(3, lstString[0].ToString().Length - 3), value[0]);
                    }
                }

                if (ct_PhieuDatHang.BUTTONTYPE == "GetPromotion")
                {
                    // Do Next Here
                    apiResponse = Utility.Create<List<v_ct_PhieuDatHang_ChiTiet>>(ct_PhieuDatHang.lstct_PhieuDatHang_ChiTiet, API.ct_PhieuDatHang + "/" + Utility.LOC_ID);
                    lstOrderProduct = JsonConvert.DeserializeObject<List<Product_Detail>>(apiResponse.Data.ToString());
                    Session[Sessions.lstProductInput] = lstOrderProduct;
                    apiResponse.GETPROMOTION = Utility.GetProductInputOutput(lstOrderProduct, "Deposit_Temp");

                    apiResponse.SOPHIEU = ct_PhieuDatHang.SOPHIEU;
                    apiResponse.NewID = ct_PhieuDatHang.ID;
                    apiResponse.MAPHIEU = ct_PhieuDatHang.MAPHIEU;
                }
                if (ct_PhieuDatHang.BUTTONTYPE == "Save")
                {
                    if (ModelState.IsValid)
                    {
                        apiResponse = Utility.GetDetail<v_ct_PhieuDatHang>(Utility.LOC_ID + "/" + ct_PhieuDatHang.ID, API.ct_PhieuDatHang);
                        if (!apiResponse.Success)
                        {
                            TempData["TitleError"] = apiResponse.Message;
                            return RedirectToAction("Index", "Notfound");
                        }
                        v_ct_PhieuDatHang chkv_ct_PhieuDatHang = null;
                        if (apiResponse.Data != null)
                            chkv_ct_PhieuDatHang = apiResponse.Data as v_ct_PhieuDatHang;

                        if (chkv_ct_PhieuDatHang == null || !string.IsNullOrEmpty(chkv_ct_PhieuDatHang.ID_PHIEUXUAT))
                        {
                            TempData["TitleError"] = "Phiếu đã thực hiện đặt hàng! Nên không thể sửa phiếu!";
                            return RedirectToAction("Index", "Notfound");
                        }

                        ct_PhieuDatHang.LOC_ID = Utility.LOC_ID;
                        ct_PhieuDatHang.ID_NGUOISUA = Session[Sessions.idUser].ToString();
                        ct_PhieuDatHang.THOIGIANSUA = Utility.CurrentTime;

                        apiResponse = Utility.Edit<v_ct_PhieuDatHang>(Utility.LOC_ID + "/" + ct_PhieuDatHang.ID, ct_PhieuDatHang, API.ct_PhieuDatHang);
                        if (apiResponse.Success)
                        {
                            apiResponse.ID = ct_PhieuDatHang.ID;
                            if (apiResponse.Data != null)
                                ct_PhieuDatHang = JsonConvert.DeserializeObject<v_v_ct_PhieuDatHang>(apiResponse.Data.ToString());

                            lstOrderProduct = new List<Product_Detail>();

                            apiResponse.URL = Url.Action("Index", "Deposit", new { SearchString = "", Page = 1, ShowSearchValue = "anfACKwdLEzVMbfakvNaoA==", FromDate = DateTime.Now.ToString("yyyy-MM-dd"), ToDate = DateTime.Now.ToString("yyyy-MM-dd") });
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, apiResponse.Message);
                        }
                    }
                    else
                    {
                        apiResponse.Success = false;
                        apiResponse.Data = Utility.GetModelState(ModelState, API.ct_PhieuDatHang);
                    }
                }
                Session[Sessions.lstProductInput] = lstOrderProduct;
                apiResponse.ID = ct_PhieuDatHang.ID;
                ct_PhieuDatHang.lstdm_KhachHang = new List<ComboboxFrom>();
                ct_PhieuDatHang.lstdm_KhachHang = GetDanhSachKhachHangCombobox().Data as List<ComboboxFrom>;
                ct_PhieuDatHang.lstdm_Kho = new List<v_dm_Kho>();
                ct_PhieuDatHang.lstdm_Kho = Utility.GetListData<v_dm_Kho>(API.dm_Kho, "", "", Utility.LOC_ID).Data as List<v_dm_Kho>;
                ct_PhieuDatHang.lstAspNetUsers = new List<v_AspNetUsers>();
                ct_PhieuDatHang.lstAspNetUsers = Utility.GetListData<v_AspNetUsers>(API.AspNetUser, "", "", "").Data as List<v_AspNetUsers>;
                var lst = Utility.ConvertobjectToView<v_v_ct_PhieuDatHang>(ct_PhieuDatHang);

                apiResponse.ProductCombo = Utility.GetProductInputOutput(lstOrderProduct, "Deposit_Temp");
                lst.Add(new ValueEdit { Key = "tbodyTempItemInput", Value = apiResponse.ProductCombo });
                apiResponse.Detail = lst;
                apiResponse.TYPE = ct_PhieuDatHang.BUTTONTYPE;
                return View(ct_PhieuDatHang);
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
        public ActionResult EditPopup([Bind(Include = "LOC_ID,ID,ID_KHO,MAPHIEU,SOPHIEU,NGAYLAP,ID_KHACHHANG,ID_NHANVIEN,GHICHU,TONGTHANHTIEN,CHIETKHAU,TONGTIENGIAMGIA,TONGTIENVAT,TONGTIEN,BUTTONTYPE,ADDRESS,TEL,THOIGIANTHEM,ID_NGUOITAO")] v_v_ct_PhieuDatHang ct_PhieuDatHang)
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                if (Utility.KiemTra())
                {
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Admin");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                if (!Utility.KiemTraQuyen(API.ct_PhieuDatHang, API.Create))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                List<Product_Detail> lstOrderProduct = new List<Product_Detail>();
                ct_PhieuDatHang.lstct_PhieuDatHang_ChiTiet = new List<v_ct_PhieuDatHang_ChiTiet>();
                var lstKey = Request.Form.AllKeys.Where(e => e.StartsWith("txt"));
                if (lstKey == null || lstKey.Count() == 0)
                {
                    ModelState.AddModelError("lstct_PhieuDatHang_ChiTiet", "Thêm danh sách hàng hóa.");
                }
                else
                {
                    v_ct_PhieuDatHang_ChiTiet ct_PhieuDatHang_ChiTiet = new v_ct_PhieuDatHang_ChiTiet();
                    foreach (string Key in lstKey)
                    {
                        var lstString = Key.ToString().Split('|');
                        var value = HttpContext.Request.Params.GetValues(Key.ToString());
                        string ShowSearchValue = clsMaHoa.Decrypt(lstString[1].ToString(), clsMaHoa.PassMaHoa);
                        var Checkct_PhieuDatHang_ChiTiet = JsonConvert.DeserializeObject<Product_Detail>(ShowSearchValue);
                        
                        if (ct_PhieuDatHang_ChiTiet.ID != Checkct_PhieuDatHang_ChiTiet.ID)
                        {
                            ct_PhieuDatHang_ChiTiet = new v_ct_PhieuDatHang_ChiTiet();
                            ct_PhieuDatHang_ChiTiet = JsonConvert.DeserializeObject<v_ct_PhieuDatHang_ChiTiet>(ShowSearchValue);
                            ct_PhieuDatHang_ChiTiet.LOC_ID = ct_PhieuDatHang.LOC_ID;
                            ct_PhieuDatHang.lstct_PhieuDatHang_ChiTiet.Add(ct_PhieuDatHang_ChiTiet);
                            lstOrderProduct.Add(Checkct_PhieuDatHang_ChiTiet);
                        }
                        Utility.EditObject(ct_PhieuDatHang_ChiTiet, lstString[0].ToString().Substring(3, lstString[0].ToString().Length - 3), value[0]);
                    }
                }

                if (ct_PhieuDatHang.BUTTONTYPE == "GetPromotion")
                {
                    // Do Next Here
                    apiResponse = Utility.Create<List<v_ct_PhieuDatHang_ChiTiet>>(ct_PhieuDatHang.lstct_PhieuDatHang_ChiTiet, API.ct_PhieuDatHang + "/" + Utility.LOC_ID);
                    lstOrderProduct = JsonConvert.DeserializeObject<List<Product_Detail>>(apiResponse.Data.ToString());

                    apiResponse.GETPROMOTION = Utility.GetProductInputOutput(lstOrderProduct, "Deposit_Temp");

                    apiResponse.SOPHIEU = ct_PhieuDatHang.SOPHIEU;
                    apiResponse.NewID = ct_PhieuDatHang.ID;
                    apiResponse.MAPHIEU = ct_PhieuDatHang.MAPHIEU;
                }
                if (ct_PhieuDatHang.BUTTONTYPE == "Save")
                {
                    if (ModelState.IsValid)
                    {
                        apiResponse = Utility.GetDetail<v_ct_PhieuDatHang>(Utility.LOC_ID + "/" + ct_PhieuDatHang.ID, API.ct_PhieuDatHang);
                        if (!apiResponse.Success)
                        {
                            TempData["TitleError"] = apiResponse.Message;
                            apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                            return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                        }
                        v_ct_PhieuDatHang chkv_ct_PhieuDatHang = null;
                        if (apiResponse.Data != null)
                            chkv_ct_PhieuDatHang = apiResponse.Data as v_ct_PhieuDatHang;

                        if (chkv_ct_PhieuDatHang == null || !string.IsNullOrEmpty(chkv_ct_PhieuDatHang.ID_PHIEUXUAT))
                        {
                            //TempData["TitleError"] = "Phiếu đã thực hiện đặt hàng! Nên không thể sửa phiếu!";
                            //return RedirectToAction("Index", "Notfound");

                            apiResponse.Success = false;
                            apiResponse.Message = "Phiếu đã thực hiện đặt hàng! Nên không thể sửa phiếu!";
                            return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                        }

                        ct_PhieuDatHang.LOC_ID = Utility.LOC_ID;
                        ct_PhieuDatHang.ID_NGUOISUA = Session[Sessions.idUser].ToString();
                        ct_PhieuDatHang.THOIGIANSUA = Utility.CurrentTime;

                        apiResponse = Utility.Edit<v_ct_PhieuDatHang>(Utility.LOC_ID + "/" + ct_PhieuDatHang.ID, ct_PhieuDatHang, API.ct_PhieuDatHang);
                        if (apiResponse.Success)
                        {
                            apiResponse.ID = ct_PhieuDatHang.ID;
                            if (apiResponse.Data != null)
                                ct_PhieuDatHang = JsonConvert.DeserializeObject<v_v_ct_PhieuDatHang>(apiResponse.Data.ToString());

                            lstOrderProduct = new List<Product_Detail>();
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, apiResponse.Message);
                        }
                    }
                    else
                    {
                        apiResponse.Success = false;
                        apiResponse.Data = Utility.GetModelState(ModelState, API.ct_PhieuDatHang);
                    }
                }
                Session[Sessions.lstProductInput] = lstOrderProduct;
                apiResponse.ID = ct_PhieuDatHang.ID;
                ct_PhieuDatHang.lstdm_KhachHang = new List<ComboboxFrom>();
                ct_PhieuDatHang.lstdm_KhachHang = GetDanhSachKhachHangCombobox().Data as List<ComboboxFrom>;
                ct_PhieuDatHang.lstdm_Kho = new List<v_dm_Kho>();
                ct_PhieuDatHang.lstdm_Kho = Utility.GetListData<v_dm_Kho>(API.dm_Kho, "", "", Utility.LOC_ID).Data as List<v_dm_Kho>;
                ct_PhieuDatHang.lstAspNetUsers = new List<v_AspNetUsers>();
                ct_PhieuDatHang.lstAspNetUsers = Utility.GetListData<v_AspNetUsers>(API.AspNetUser, "", "", "").Data as List<v_AspNetUsers>;
                var lst = Utility.ConvertobjectToView<v_v_ct_PhieuDatHang>(ct_PhieuDatHang);

                apiResponse.ProductCombo = Utility.GetProductInputOutput(lstOrderProduct, "Deposit_Temp");
                lst.Add(new ValueEdit { Key = "tbodyTempItemInput", Value = apiResponse.ProductCombo });
                apiResponse.Detail = lst;
                apiResponse.TYPE = ct_PhieuDatHang.BUTTONTYPE;
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
                if (!Utility.KiemTraQuyen(API.ct_PhieuDatHang, API.Delete))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                apiResponse = Utility.GetDetail<v_ct_PhieuDatHang>(Utility.LOC_ID + "/" + id, API.ct_PhieuDatHang);
                if (!apiResponse.Success)
                {
                    TempData["TitleError"] = apiResponse.Message;
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                v_ct_PhieuDatHang chkv_ct_PhieuDatHang = null;
                if (apiResponse.Data != null)
                    chkv_ct_PhieuDatHang = apiResponse.Data as v_ct_PhieuDatHang;

                if (chkv_ct_PhieuDatHang == null || !string.IsNullOrEmpty(chkv_ct_PhieuDatHang.ID_PHIEUXUAT))
                {
                    TempData["TitleError"] = "Phiếu đã thực hiện đặt hàng! Nên không thể sửa phiếu!";
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                apiResponse = Utility.Delete<v_ct_PhieuDatHang>(Utility.LOC_ID + "/" + id, API.ct_PhieuDatHang);
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

        public ActionResult CreatePopup()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (Utility.KiemTra())
            {
                return RedirectToAction("Index", "Admin");
            }
            if (!Utility.KiemTraQuyen(API.ct_PhieuDatHang, API.Xem))
            {
                TempData["TitleError"] = API.TitlePermission;
                return RedirectToAction("Index", "Notfound");
            }
            v_v_ct_PhieuDatHang ct_PhieuDatHang = new v_v_ct_PhieuDatHang();
            ct_PhieuDatHang.NGAYLAP = Utility.CurrentTime;
            ct_PhieuDatHang.LOC_ID = Utility.LOC_ID;
            ct_PhieuDatHang.SOPHIEU = Utility.GetMaxID<ct_PhieuDatHang>(ct_PhieuDatHang, Utility.LOC_ID, ct_PhieuDatHang.NGAYLAP.ToString("yyyy-MM-dd"));
            ct_PhieuDatHang.MAPHIEU = API.GetMaPhieu(API.ct_PhieuDatHang, ct_PhieuDatHang.NGAYLAP, ct_PhieuDatHang.SOPHIEU);
            ct_PhieuDatHang.lstdm_ThueSuat = new List<v_dm_ThueSuat>();
            ct_PhieuDatHang.lstdm_ThueSuat = Utility.GetListData<v_dm_ThueSuat>(API.dm_ThueSuat, "", "", Utility.LOC_ID).Data as List<v_dm_ThueSuat>;
            ct_PhieuDatHang.lstdm_Kho = new List<v_dm_Kho>();
            ct_PhieuDatHang.lstdm_Kho = Utility.GetListData<v_dm_Kho>(API.dm_Kho, "", "", Utility.LOC_ID).Data as List<v_dm_Kho>;
            ct_PhieuDatHang.lstdm_KhachHang = new List<ComboboxFrom>();
            ct_PhieuDatHang.lstdm_KhachHang = GetDanhSachKhachHangCombobox().Data as List<ComboboxFrom>;
            ct_PhieuDatHang.lstAspNetUsers = new List<v_AspNetUsers>();
            ct_PhieuDatHang.lstAspNetUsers = Utility.GetListData<v_AspNetUsers>(API.AspNetUser, "", "", "").Data as List<v_AspNetUsers>;
            List<Product_Detail> lstOrderProduct = new List<Product_Detail>();

            var KhoISDEFAULT = ct_PhieuDatHang.lstdm_Kho.Where(e => e.ISDEFAULT).FirstOrDefault();
            if (KhoISDEFAULT != null)
            {
                ct_PhieuDatHang.ID_KHO = KhoISDEFAULT.ID;
            }
            Session[Sessions.lstProductInput] = lstOrderProduct;
            var lst = Utility.ConvertobjectTo<v_v_ct_PhieuDatHang>(ct_PhieuDatHang);
            apiResponse.Success = true;
            apiResponse.ProductCombo = Utility.GetProductInputOutput(lstOrderProduct, "Deposit_Temp");
            lst.Add(new ValueEdit { Key = "tbodyTempItemInput", Value = apiResponse.ProductCombo });
            apiResponse = GetDanhSachNhomSanPham();
            string sbtn = "";
            if (!apiResponse.Success)
            {
                TempData["TitleError"] = apiResponse.Message;
                return RedirectToAction("Index", "Notfound");
            }
            else
            {
                var rls = apiResponse.Data as List<web_Sp_Get_DSNhomSanPham_Result>;
                sbtn = "<button class='btnGroup active' onclick='myFunctionPage(\"all\", \"\",\"collapseOneDeposit\")' id= \"all\">Show all</button>";
                foreach (web_Sp_Get_DSNhomSanPham_Result itm in rls)
                {
                    sbtn += "<button class='btnGroup' onclick='myFunctionPage(\"" + itm.ID + "\", \"\",\"collapseOneDeposit\")' id= \"" + itm.ID + "\"> " + itm.NAME + "</button>";
                }
                if (!string.IsNullOrEmpty(sbtn))
                    sbtn += "<button class='btnGroup' onclick='myFunctionLoadGroup(\"collapseOneDeposit\")'><span class='glyphicon glyphicon-refresh'></span></button>";
            }
            ViewBag.NhomHang = sbtn;
            lst.Add(new ValueEdit { Key = "myProduct", Value = myProduct(sbtn, "collapseOneDeposit") });
            apiResponse.Detail = lst;
            return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
        }
        #endregion

        #region Thêm xóa sửa đặt hàng

        [HttpPost]
        public ActionResult LoadProduct_Detail()
        {
            if (Utility.KiemTra())
            {
                return RedirectToAction("Index", "Admin");
            }

            if (ModelState.IsValid)
            {
                List<Product_Detail> lstOrderProduct = new List<Product_Detail>();
                if (Session[Sessions.lstProductInput] == null)
                {

                }
                else
                {
                    lstOrderProduct = (List<Product_Detail>)Session[Sessions.lstProductInput];
                }
                return Json(Utility.GetProductInputOutput(lstOrderProduct, "Deposit_Temp"), JsonRequestBehavior.AllowGet);
            }
            else
            {
                Return newReturn = new Return();
                newReturn.DATA = "";

                return Json(newReturn, JsonRequestBehavior.AllowGet);
            }
        }

        #region Thêm sản phẩm
        [HttpPost]
        public ActionResult AddProduct_Detail(v_ct_PhieuDatHang_ChiTiet model)
        {
            if (Utility.KiemTra())
            {
                return RedirectToAction("Index", "Admin");
            }

            if (ModelState.IsValid)
            {
                ApiResponse apiResponse = new ApiResponse();
                List<Product_Detail> lstOrderProduct = new List<Product_Detail>();
                if (Session[Sessions.lstProductInput] != null)
                {
                    lstOrderProduct = (List<Product_Detail>)Session[Sessions.lstProductInput];
                }
                model.TONGSOLUONG = model.SOLUONG * model.TYLE_QD;

                var Product_Detail = lstOrderProduct.Where(s => s.ID_HANGHOAKHO == model.ID_HANGHOAKHO && string.IsNullOrEmpty(s.ID_COMBO)).FirstOrDefault();
                if (Product_Detail != null)
                {
                }
                else
                {

                }
                Return newReturn = new Return();
                newReturn.DATA = Utility.GetProductInputOutput(lstOrderProduct, "Deposit_Temp");
                newReturn.URL = apiResponse.URL;
                newReturn.Message = apiResponse.Message;
                return Json(newReturn, JsonRequestBehavior.AllowGet);
            }
            else
            {
                Return newReturn = new Return();
                newReturn.DATA = "";

                return Json(newReturn, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region Xóa sản phẩm
        [HttpPost]
        public ActionResult DeleteProduct_Detail(String id)
        {
            if (Utility.KiemTra())
            {
                return RedirectToAction("Index", "Admin");
            }

            if (ModelState.IsValid)
            {
                List<Product_Detail> lstOrderProduct = new List<Product_Detail>();
                if (Session[Sessions.lstProductInput] != null)
                {
                    lstOrderProduct = (List<Product_Detail>)Session[Sessions.lstProductInput];
                }
                Return newReturn = new Return();
                newReturn.DATA = Utility.GetProductInputOutput(lstOrderProduct, "Deposit_Temp");
                return Json(newReturn, JsonRequestBehavior.AllowGet);
            }
            else
            {
                Return newReturn = new Return();
                newReturn.DATA = "";

                return Json(newReturn, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region Xóa tất cả 
        [HttpPost]
        public ActionResult DeleteAllProduct_Detail()
        {
            if (Utility.KiemTra())
            {
                return RedirectToAction("Index", "Admin");
            }

            if (ModelState.IsValid)
            {
                List<Product_Detail> lstOrderProduct = new List<Product_Detail>();
                if (Session[Sessions.lstProductInput] != null)
                {
                    lstOrderProduct = (List<Product_Detail>)Session[Sessions.lstProductInput];
                }
                lstOrderProduct.Clear();
                return Json(Utility.GetProductInputOutput(lstOrderProduct, "Deposit_Temp"), JsonRequestBehavior.AllowGet);
            }
            else
            {
                Return newReturn = new Return();
                newReturn.DATA = "";

                return Json(newReturn, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region Cập nhật số lượng
        public ActionResult UpdateDeposit_TempProduct(String cartDeposit_Temp)
        {
            if (Utility.KiemTra())
            {
                return RedirectToAction("Index", "Admin");
            }

            if (ModelState.IsValid)
            {
                ApiResponse apiResponse = new ApiResponse();
                var lstcartDeposit_Temp = new JavaScriptSerializer().Deserialize<List<Product_Detail>>(cartDeposit_Temp);
                List<Product_Detail> lstOrderProduct = new List<Product_Detail>();
                if (Session[Sessions.lstProductInput] != null)
                {
                    lstOrderProduct = (List<Product_Detail>)Session[Sessions.lstProductInput];
                }
                Return newReturn = new Return();
                newReturn.DATA = Utility.GetProductInputOutput(lstOrderProduct, "Deposit_Temp");
                newReturn.URL = apiResponse.URL;
                newReturn.Message = apiResponse.Message;
                return Json(newReturn, JsonRequestBehavior.AllowGet);
            }
            else
            {
                Return newReturn = new Return();
                newReturn.DATA = "";
                return Json(newReturn, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion


        #region Lấy địa chỉ khách hàng
        [HttpPost]
        public ActionResult CallChangeCustomer(String id)
        {
            if (Utility.KiemTra())
            {
                return RedirectToAction("Index", "Admin");
            }

            if (ModelState.IsValid)
            {
                Return newReturn = new Return();
                ApiResponse apiResponse = Utility.GetDetail<v_dm_KhachHang>(Utility.LOC_ID + "/" + id, API.dm_KhachHang);
                if (!apiResponse.Success)
                {
                    TempData["TitleError"] = apiResponse.Message;
                    newReturn.URL = Url.Action("Index", "Notfound");
                }
                else
                {
                    var KhachHang = (apiResponse.Data as v_dm_KhachHang);
                    SP_Parameter sp_Parameter = new SP_Parameter();
                    //ApiResponse apiResponse = new ApiResponse();
                    sp_Parameter.LOC_ID = Utility.LOC_ID;
                    sp_Parameter.ID_KHACHHANG = KhachHang.ID;
                    sp_Parameter.ID_NHOMKHACHHANG = KhachHang.ID_NHOMKHACHHANG;
                    sp_Parameter.ID_KHUVUC = KhachHang.ID_KHUVUC;
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
                        if(CongNo != null)
                        {
                            KhachHang.CONGNOTHONGBAO = (KhachHang.MAX_CONGNO > 0 ?
                                (CongNo.TONGTIENCONGNOCUOIKY >= KhachHang.MAX_CONGNO ? "Công nợ: " + CongNo.TONGTIENCONGNOCUOIKY.ToString("N0") + " > " + KhachHang.MAX_CONGNO.ToString("N0")
                                : "Công nợ: " + CongNo.TONGTIENCONGNOCUOIKY.ToString("N0") + "(" + KhachHang.MAX_CONGNO.ToString("N0") + ")") : (CongNo.TONGTIENCONGNOCUOIKY > 0 ? "Công nợ: " + CongNo.TONGTIENCONGNOCUOIKY.ToString("N0") : ""));

                            KhachHang.KHONGDUOCPHEPTAO = KhachHang.MAX_CONGNO > 0 ? CongNo.TONGTIENCONGNOCUOIKY >= KhachHang.MAX_CONGNO : false;

                            if(KhachHang.LATITUDE != null && KhachHang.LONGITUDE != null)
                            {
                                KhachHang.CONTENT_MAP = "Vĩ độ: " + KhachHang.LATITUDE +"<br>Kinh độ: " + KhachHang.LONGITUDE;
                            }    
                        }
                        
                    }
                    else
                    {
                        KhachHang.CONTENT_MAP = "";
                        KhachHang.CONGNOTHONGBAO = "";
                    }
                    newReturn.DataObject = KhachHang;
                }

                return Json(newReturn, JsonRequestBehavior.AllowGet);
            }
            else
            {
                Return newReturn = new Return();
                newReturn.DATA = "";

                return Json(newReturn, JsonRequestBehavior.AllowGet);
            }
        }

        private string CheckCongNoKhachHang(string id, double TienHoaDon)
        {
            ApiResponse apiResponse = Utility.GetDetail<v_dm_KhachHang>(Utility.LOC_ID + "/" + id, API.dm_KhachHang);
            if (!apiResponse.Success)
            {
                return "-1";
            }
            else
            {
                var KhachHang = (apiResponse.Data as v_dm_KhachHang);
                SP_Parameter sp_Parameter = new SP_Parameter();
                //ApiResponse apiResponse = new ApiResponse();
                sp_Parameter.LOC_ID = Utility.LOC_ID;
                sp_Parameter.ID_KHACHHANG = KhachHang.ID;
                sp_Parameter.ID_NHOMKHACHHANG = KhachHang.ID_NHOMKHACHHANG;
                sp_Parameter.ID_KHUVUC = KhachHang.ID_KHUVUC;
                sp_Parameter.ISTHEOTHOIGIAN = false;
                sp_Parameter.ISPHATSINHCONGNO = false;
                sp_Parameter.ISPHATSINHCONGNOTRONGKY = false;
                sp_Parameter.ISCONCONGNO = false;
                apiResponse = Utility.Get_ThongKeCongNoKhachHang<v_ThongKeCongNoKhachHang>(sp_Parameter);
                if (!apiResponse.Success)
                {
                    return "-1";
                }
                if (apiResponse.Data != null)
                {
                    var CongNo = (apiResponse.Data as List<v_ThongKeCongNoKhachHang>).FirstOrDefault();
                    if (CongNo != null)
                    {
                        if (KhachHang.MAX_CONGNO > 0 && KhachHang.MAX_CONGNO < CongNo.TONGTIENCONGNOCUOIKY + TienHoaDon)
                            return (CongNo.TONGTIENCONGNOCUOIKY + TienHoaDon).ToString("N0") + " > " + KhachHang.MAX_CONGNO.ToString("N0");
                        else
                            return "";
                    }
                }
                else
                {
                    return "-1";
                }
            }
            return "-1";
        }

        [HttpPost]
        public ActionResult SaveMapCustomer(string ID, string LATITUDE, string LONGITUDE)
        {
            if (Utility.KiemTra())
            {
                return RedirectToAction("Index", "Admin");
            }

            if (ModelState.IsValid)
            {
                Return newReturn = new Return();
                v_dm_KhachHang KhachHang = new v_dm_KhachHang();
                KhachHang.ID = ID;
                KhachHang.LATITUDE = Convert.ToDouble(LATITUDE.Replace(".",","));
                KhachHang.LONGITUDE = Convert.ToDouble(LONGITUDE.Replace(".", ","));
                ApiResponse apiResponse = Utility.Save_Map(KhachHang, API.Insert_Customer_Map);
                if (!apiResponse.Success)
                {
                    TempData["TitleError"] = apiResponse.Message;
                    return RedirectToAction("Index", "Notfound");
                }
                newReturn.DataObject = KhachHang;

               
                return Json("Lưu thành công!", JsonRequestBehavior.AllowGet);
            }
            else
            {
                Return newReturn = new Return();
                newReturn.DATA = "";

                return Json(newReturn, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
        #endregion

        #region Load sản phẩm nhóm sản phẩm
        [HttpPost]
        public ActionResult LoadDanhSachSanPham(TimKiem model)
        {
            if (Utility.KiemTra())
            {
                return RedirectToAction("Index", "Admin");
            }

            if (ModelState.IsValid)
            {
                Return newReturn = new Return();
                newReturn.DATA = "";

                if (model.GroupID == "all")
                    model.GroupID = "-1";

                if (string.IsNullOrEmpty(model.keySearch))
                    model.keySearch = "";
                model.BOLTONKHO = !model.BOLTONKHO;
                ApiResponse apiResponse = GetDanhSachSanPham(model);
                if (!apiResponse.Success)
                {
                    TempData["TitleError"] = apiResponse.Message;
                    newReturn.URL = Url.Action("Index", "Notfound");
                }
                else
                {
                    var rls = apiResponse.Data as List<web_Sp_Get_DSSanPham_Result>;
                    foreach (web_Sp_Get_DSSanPham_Result itm in rls)
                    {
                        int PhanNguyen = 0;
                        string strQty = "";
                        if (itm.TYLE_QD > 1)
                        {
                            PhanNguyen = Convert.ToInt32(itm.QTY) / Convert.ToInt32(itm.TYLE_QD);
                            strQty = (PhanNguyen > 0 ? PhanNguyen.ToString("N0") + " " + itm.NAME_DVT : "") + ((itm.QTY - (PhanNguyen * itm.TYLE_QD)) > 0 ? (PhanNguyen > 0 ? "/" : "") + (itm.QTY - (PhanNguyen * itm.TYLE_QD)).ToString("N0") + " " + itm.NAME_DVT_QD : "");
                        }
                        else
                            strQty = itm.QTY.ToString("N0") + " " + itm.NAME_DVT;


                        newReturn.DATA += "<div class=\"productDeposit\">";
                        newReturn.DATA += "<button class=\"productDeposit-button\" onclick='myFunOpenProduct(this,\"" + itm.ID_HANGHOAKHO + "\")'>";
                        newReturn.DATA += "<img src=\"/Images_Upload/Product/"+ (string.IsNullOrEmpty(itm.PICTURE) ? "NoImage.png" : itm.PICTURE) + "\" "+ (!string.IsNullOrEmpty(itm.PICTURE) ? "onclick=\"showPopupDeposit('" + itm.PICTURE + "')" : "") + "\">";
                        newReturn.DATA += "<div class=\"productDeposit-details\">" + itm.NAME;
                        newReturn.DATA += "<div class=\"productDeposit-info\">";
                        if(!itm.ISKHONGHIENTHITONKHO)
                            newReturn.DATA += "<code>SL: " + strQty + "</code>-";
                        newReturn.DATA += "<code>"+ String.Format("{0:N0}", itm.GIA01)+" đ</code>";
                        newReturn.DATA += "</div></div></button></div>";
                        //newReturn.DATA += "<button style ='width: 150px;height:100px;"+ (itm.NAME.Length > 40 ? "font-size:" + (itm.NAME.Length > 60 ? "1.2ex;" : "1.5ex;" ): "")+ "' class='filterDiv active show' onclick='myFunOpenProduct(this,\"" + itm.ID_HANGHOAKHO + "\")'>" + itm.NAME + "<div style='font-size: 10px;'><code>SL: " + strQty + "</code> - <code>" + String.Format("{0:N0}", itm.GIA01) + " đ</code></div></button>";
                    }
                }
                return Json(newReturn, JsonRequestBehavior.AllowGet);
            }
            else
            {
                Return newReturn = new Return();
                newReturn.DATA = "";
                newReturn.CHUOIPHANTRANG = "";
                return Json(newReturn, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult LoadGroup(string Class)
        {
            if (Utility.KiemTra())
            {
                return RedirectToAction("Index", "Admin");
            }
            ApiResponse apiResponse = GetDanhSachNhomSanPham();
            string sbtn = "";
            if (apiResponse.Success && apiResponse.Data != null)
            {
                var rls = apiResponse.Data as List<web_Sp_Get_DSNhomSanPham_Result>;
                sbtn = "<button class='btnGroup active' onclick='myFunctionPage(\"all\", \"\", \"" + Class + "\")' id= \"all\">Show all</button>";
                foreach (web_Sp_Get_DSNhomSanPham_Result itm in rls)
                {
                    sbtn += "<button class='btnGroup' onclick='myFunctionPage(\"" + itm.ID + "\", \"\", \"" + Class + "\")' id= \"" + itm.ID + "\"> " + itm.NAME + "</button>";
                }
                if (!string.IsNullOrEmpty(sbtn))
                    sbtn += "<button class='btnGroup' onclick='myFunctionLoadGroup(\"" + Class + "\")'><span class='glyphicon glyphicon-refresh'></span></button>";
            }
            Return newReturn = new Return();
            newReturn.DATA = sbtn;
            newReturn.CHUOIPHANTRANG = Class;
            newReturn.URL = apiResponse.URL;
            return Json(newReturn, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult LoadProduct(String id)
        {
            if (Utility.KiemTra())
            {
                return RedirectToAction("Index", "Admin");
            }
            ApiResponse apiResponse = new ApiResponse();
            v_dm_HangHoa dm_HangHoa = new v_dm_HangHoa();
            if (ModelState.IsValid)
            {
                apiResponse = GetSanPham(id);
                if (apiResponse.Success)
                {
                    dm_HangHoa = (apiResponse.Data as v_dm_HangHoa);

                    if (dm_HangHoa != null)
                    {
                        dm_HangHoa.GIA = dm_HangHoa.GIA01;
                        dm_HangHoa.GIA_QD = dm_HangHoa.GIA01_QD;
                        dm_HangHoa.NAME_DVT = dm_HangHoa.NAME_DVT + " (" + dm_HangHoa.GIA01.ToString("N0") + ")";
                        dm_HangHoa.NAME_DVT_QD = dm_HangHoa.NAME_DVT_QD + " (" + dm_HangHoa.GIA01_QD.ToString("N0") + ")";
                    }
                    if (!string.IsNullOrEmpty(dm_HangHoa.ID_THUESUAT))
                    {
                        var apiResponseVAT = Utility.GetDetail<v_v_dm_ThueSuat>(Utility.LOC_ID + "/" + dm_HangHoa.ID_THUESUAT, API.dm_ThueSuat);
                        if (apiResponseVAT.Data != null)
                        {
                            v_v_dm_ThueSuat dm_ThueSuat = apiResponseVAT.Data as v_v_dm_ThueSuat;

                            if (dm_ThueSuat != null)
                            {
                                dm_HangHoa.THANHTIEN = dm_HangHoa.GIA * 1;
                                dm_HangHoa.THUESUAT = dm_ThueSuat.THUESUAT;
                                dm_HangHoa.TONGTIENVAT = dm_HangHoa.THANHTIEN * dm_HangHoa.THUESUAT / 100;
                                dm_HangHoa.TONGCONG = dm_HangHoa.THANHTIEN + dm_HangHoa.TONGTIENVAT;
                            }
                        }
                    }
                    else
                    {
                        dm_HangHoa.THANHTIEN = dm_HangHoa.GIA * 1;
                        dm_HangHoa.THUESUAT = 0;
                        dm_HangHoa.TONGTIENVAT = dm_HangHoa.THANHTIEN * dm_HangHoa.THUESUAT / 100;
                        dm_HangHoa.TONGCONG = dm_HangHoa.THANHTIEN + dm_HangHoa.TONGTIENVAT;
                    }
                }

                apiResponse.Detail = Utility.ConvertobjectTo<v_dm_HangHoa>(dm_HangHoa);

            }


            return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
        }
        #endregion

        #region Lấy danh sách khu vực
        //List<web_Sp_Get_DSKhuVuc_Result>
        private ApiResponse GetDanhSachKhuVuc()
        {
            SP_Parameter objParameter = new SP_Parameter();
            objParameter.LOC_ID = Utility.LOC_ID;
            objParameter.ID_NHOMQUYEN = Session[Sessions.idNhomQuyen].ToString();
            ApiResponse apiResponse = Utility.ExecuteStoredProc<web_Sp_Get_DSKhuVuc_Result>(objParameter, API.web_Sp_Get_DSKhuVuc);
            if (!apiResponse.Success)
            {
                apiResponse.Data = new List<web_Sp_Get_DSKhuVuc_Result>();
                TempData["TitleError"] = apiResponse.Message;
                apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
            }
            return apiResponse;
        }
        #endregion 

        #region Lấy danh sách sản phẩm
        //List<web_Sp_Get_DSSanPham_Result>
        private ApiResponse GetDanhSachSanPham(TimKiem model)
        {
            SP_Parameter objParameter = new SP_Parameter();
            ApiResponse apiResponse = new ApiResponse();
            objParameter = new SP_Parameter();
            objParameter.LOC_ID = Utility.LOC_ID;
            objParameter.ID_NHOMQUYEN = !string.IsNullOrEmpty(model.idNhomQuyen) ? model.idNhomQuyen : Session[Sessions.idNhomQuyen].ToString();
            objParameter.ID_NHOMHANGHOA = model.GroupID;
            objParameter.KEY = model.keySearch;
            objParameter.ID_KHO = model.ID_KHO;
            objParameter.BOLTONKHO = model.BOLTONKHO;
            objParameter.ID_HANGHOAKHO = "";
            apiResponse = Utility.ExecuteStoredProc<web_Sp_Get_DSSanPham_Result>(objParameter, API.web_Sp_Get_DSSanPham);
            if (!apiResponse.Success)
            {
                apiResponse.Data = new List<web_Sp_Get_DSSanPham_Result>();
                TempData["TitleError"] = apiResponse.Message;
                apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
            }
            return apiResponse;
        }
        #endregion

        #region Lấy danh sách nhóm sản phẩm
        //List<web_Sp_Get_DSNhomSanPham_Result>
        private ApiResponse GetDanhSachNhomSanPham()
        {
            SP_Parameter objParameter = new SP_Parameter();
            objParameter.LOC_ID = Utility.LOC_ID;
            objParameter.ID_NHOMQUYEN = Session[Sessions.idNhomQuyen].ToString();
            ApiResponse apiResponse = Utility.ExecuteStoredProc<web_Sp_Get_DSNhomSanPham_Result>(objParameter, API.web_Sp_Get_DSNhomSanPham);
            if (!apiResponse.Success)
            {
                apiResponse.Data = new List<web_Sp_Get_DSNhomSanPham_Result>();
                TempData["TitleError"] = apiResponse.Message;
                apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
            }
            return apiResponse;
        }
        #endregion

        #region Lấy sản phẩm
        //web_Sp_Get_SanPham_Result
        private ApiResponse GetSanPham(string idSanPham)
        {
            SP_Parameter objParameter = new SP_Parameter();
            objParameter.LOC_ID = Utility.LOC_ID;
            objParameter.ID_KHO = "";
            objParameter.BOLTONKHO = false;
            objParameter.ID_HANGHOAKHO = idSanPham;
            ApiResponse apiResponse = Utility.ExecuteStoredProc<v_dm_HangHoa>(objParameter, API.Sp_Get_DanhSachSanPhamKho);
            if (!apiResponse.Success)
            {
                apiResponse.Data = new v_dm_HangHoa();
                TempData["TitleError"] = apiResponse.Message;
                apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
            }
            else
            {

                apiResponse.Data = (apiResponse.Data as List<v_dm_HangHoa>).FirstOrDefault();
            }

            return apiResponse;
        }
        #endregion

        #region Lấy danh sách khách hàng
        //List<web_Sp_Get_DSKhachHang_Result>
        public ApiResponse GetDanhSachKhachHang<T>(string idNhomQuyen = "", string KEY = "", string LOAITIMKIEM = "")
        {
            SP_Parameter objParameter = new SP_Parameter();
            objParameter.LOC_ID = Utility.LOC_ID;
            objParameter.ID_NHOMQUYEN = !string.IsNullOrEmpty(idNhomQuyen) ? idNhomQuyen : Session[Sessions.idNhomQuyen].ToString();
            objParameter.ID_KHUVUC = "-1";
            objParameter.KEY = KEY;
            objParameter.LOAITIMKIEM = LOAITIMKIEM;
            objParameter.THU = !string.IsNullOrEmpty(idNhomQuyen) ? idNhomQuyen : Session[Sessions.idNhomQuyen].ToString();
            ApiResponse apiResponse = Utility.ExecuteStoredProc<T>(objParameter, API.web_Sp_Get_DSKhachHang);
            if (!apiResponse.Success)
            {
                apiResponse.Data = new List<v_dm_KhachHang>();
                apiResponse.URL = "";
                TempData["TitleError"] = apiResponse.Message;
                apiResponse.Success = false;
            }
            return apiResponse;
        }

        public ApiResponse GetDanhSachKhachHangCombobox(string idNhomQuyen = "", string KEY = "", string LOAITIMKIEM = "")
        {
            SP_Parameter objParameter = new SP_Parameter();
            objParameter.LOC_ID = Utility.LOC_ID;
            objParameter.ID_NHOMQUYEN = !string.IsNullOrEmpty(idNhomQuyen) ? idNhomQuyen : Session[Sessions.idNhomQuyen].ToString();
            objParameter.ID_KHUVUC = "-1";
            objParameter.KEY = KEY;
            objParameter.LOAITIMKIEM = LOAITIMKIEM;
            objParameter.THU = !string.IsNullOrEmpty(idNhomQuyen) ? idNhomQuyen : Session[Sessions.idNhomQuyen].ToString();
            ApiResponse apiResponse = new ApiResponse();
            if(objParameter.ID_NHOMQUYEN != "-1")
                apiResponse = Utility.ExecuteStoredProc<ComboboxFrom>(objParameter, API.web_Sp_Get_DSKhachHang);
            else
                apiResponse = Utility.GetListData<ComboboxFrom>(API.dm_KhachHang, "", "", Utility.LOC_ID);
            if (!apiResponse.Success)
            {
                apiResponse.Data = new List<ComboboxFrom>();
                TempData["TitleError"] = apiResponse.Message;
                apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
            }
            return apiResponse;
        }
        #endregion


        [HttpPost]
        public ActionResult UpdateAddProduct(Product_Detail Product_Detail)
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                List<Product_Detail> lstOrderProduct = new List<Product_Detail>();
                if (Session[Sessions.lstProductInput] != null)
                {
                    lstOrderProduct = (List<Product_Detail>)Session[Sessions.lstProductInput];
                }
                Utility.TinhTong(Product_Detail, null, lstOrderProduct);
                apiResponse.Success = true;
                apiResponse.Detail = Product_Detail;
                return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex, JsonConvert.SerializeObject(Product_Detail));
                TempData["TitleError"] = API.TitleTryCatch;
                TempData["DetailError"] = ex.Message;
                apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
            }
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult AddProductDeposit([Bind(Include = "ID_HANGHOA,ID_HANGHOAKHO,DONGIA,ID_DVT,SOLUONG,CHIETKHAU,TONGTIENGIAMGIA,THANHTIEN,THUESUAT,ID_THUESUAT,TONGTIENVAT,TONGCONG,ID_KHO")] Product_Detail Product_Detail)
        {
            List<Product_Detail> lstOrderProduct = new List<Product_Detail>();
            if (Session[Sessions.lstProductInput] != null)
            {
                lstOrderProduct = (List<Product_Detail>)Session[Sessions.lstProductInput];
            }
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                if (Utility.KiemTra())
                {
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Admin");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                if (ModelState.IsValid)
                {
                    v_dm_HangHoa dm_HangHoa = new v_dm_HangHoa();

                    apiResponse = Utility.Get_DanhSachSanPhamKho<v_dm_HangHoa>(Product_Detail.ID_KHO, false, Product_Detail.ID_HANGHOAKHO);

                    //apiResponse = Utility.GetDetail<v_dm_HangHoa_Kho>(Utility.LOC_ID + "/" + Product_Detail.ID_HANGHOAKHO, API.dm_HangHoa_Kho);

                    if (!apiResponse.Success)
                    {
                        apiResponse.Data = new List<v_dm_HangHoa>();
                        TempData["TitleError"] = apiResponse.Message;
                        apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                        return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                    }
                    if (apiResponse.Data != null)
                        dm_HangHoa = (apiResponse.Data as List<v_dm_HangHoa>).FirstOrDefault();

                    if (dm_HangHoa != null)
                    {
                        Product_Detail.STT = lstOrderProduct.Count() > 0 ? lstOrderProduct.Max(e => e.STT) + 1 : 1;
                        Product_Detail.ID = Guid.NewGuid().ToString();
                        Product_Detail.NAME = dm_HangHoa.NAME;
                        Product_Detail.MA = dm_HangHoa.MA;
                        Product_Detail.ID_NHOMHANGHOA = dm_HangHoa.ID_NHOMHANGHOA;
                        if (dm_HangHoa.ID_DVT == Product_Detail.ID_DVT)
                        {
                            Product_Detail.NAME_DVT = dm_HangHoa.NAME_DVT;
                            if (!string.IsNullOrEmpty(dm_HangHoa.ID_DVT_QD))
                            {
                                Product_Detail.TYLE_QD = dm_HangHoa.TYLE_QD;
                            }
                            else
                            {
                                if (dm_HangHoa.LOAIHANGHOA == ((int)API.LoaiSanPham.KhongQuanLyTonKho).ToString())
                                    Product_Detail.TYLE_QD = 0;
                                else
                                    Product_Detail.TYLE_QD = 1;

                            }
                        }
                        else if (dm_HangHoa.ID_DVT_QD == Product_Detail.ID_DVT)
                        {
                            if (!string.IsNullOrEmpty(dm_HangHoa.ID_DVT_QD))
                            {
                                Product_Detail.NAME_DVT = dm_HangHoa.NAME_DVT_QD;
                                Product_Detail.TYLE_QD = 1;
                            }
                        }
                        Product_Detail.TONGSOLUONG = Product_Detail.TYLE_QD * Product_Detail.SOLUONG;
                        //var check = lstOrderProduct.Where(e => e.ID_HANGHOAKHO == Product_Detail.ID_HANGHOAKHO && e.ID_DVT == Product_Detail.ID_DVT && e.DONGIA == Product_Detail.DONGIA).FirstOrDefault();
                        //if (check == null)
                        {
                            lstOrderProduct.Add(Product_Detail);
                            if (dm_HangHoa.LOAIHANGHOA == ((int)API.LoaiSanPham.Combo).ToString())
                            {
                                SP_Parameter objParameter = new SP_Parameter();
                                objParameter.LOC_ID = Utility.LOC_ID;
                                objParameter.ID_KHO = Product_Detail.ID_KHO;
                                objParameter.ID_COMBO = Product_Detail.ID_HANGHOA;
                                var apiResponse_Combo = Utility.ExecuteStoredProc<Product_Detail>(objParameter, API.Sp_Get_DanhSachSanPhamKho_Combo);
                                if (!apiResponse_Combo.Success)
                                {
                                    apiResponse.Data = new List<Product_Detail>();
                                    TempData["TitleError"] = apiResponse.Message;
                                    apiResponse.Success = false;
                                    apiResponse.URL = Url.Action("Index", "Notfound");
                                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                                }

                                if (apiResponse_Combo.Data != null)
                                {
                                    var lstHoangHoaCombo = (apiResponse_Combo.Data as List<Product_Detail>);
                                    foreach (Product_Detail itm in lstHoangHoaCombo)
                                    {
                                        itm.ID = Guid.NewGuid().ToString();
                                        itm.STT = Product_Detail.STT;
                                        itm.ID_DVT = itm.ID_DVT_COMBO;
                                        itm.SOLUONG = Product_Detail.SOLUONG * itm.QTY_COMBO;
                                        itm.TYLE_QD = itm.TYLE_QD_COMBO;
                                        itm.TONGSOLUONG = Product_Detail.SOLUONG * itm.QTY_TOTAL_COMBO;
                                        itm.DONGIA = 0;
                                        itm.ISCOMBO = true;
                                        itm.ID_COMBO = Product_Detail.ID_HANGHOA;

                                        Product_Detail.ID_COMBO = Product_Detail.ID_HANGHOA;
                                        lstOrderProduct.Add(itm);
                                    }
                                }
                            }

                            Session[Sessions.lstProductInput] = lstOrderProduct;
                        }
                    }
                    apiResponse.ProductCombo = Utility.GetProductInputOutput(lstOrderProduct, "Deposit_Temp");
                }
                return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex, JsonConvert.SerializeObject(Product_Detail));
                TempData["TitleError"] = API.TitleTryCatch;
                TempData["DetailError"] = ex.Message;
                apiResponse.Success = false;
                apiResponse.URL = Url.Action("Index", "Notfound");
                return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
            }
        }

        [HttpPost]
        public ActionResult UpdateProductDeposit_Temp(string ID, string TYPE, string VALUE)
        {
            List<Product_Detail> lstOrderProduct = new List<Product_Detail>();
            if (Session[Sessions.lstProductInput] != null)
            {
                lstOrderProduct = (List<Product_Detail>)Session[Sessions.lstProductInput];
            }
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                v_v_dm_HangHoa dm_HangHoa = new v_v_dm_HangHoa();
                var check = lstOrderProduct.Where(e => e.ID == ID).FirstOrDefault();
                if (check != null)
                {
                    check.TYPE = TYPE;
                    Utility.TinhTong(check, VALUE, lstOrderProduct);
                }
                if(check.TYPE != "CHIETKHAU" && check.TYPE != "TONGTIENGIAMGIA")
                    lstOrderProduct = XoaKhuyenMai(lstOrderProduct);
                Session[Sessions.lstProductInput] = lstOrderProduct;
                apiResponse.ProductCombo = Utility.GetProductInputOutput(lstOrderProduct, "Deposit_Temp");
                apiResponse.Success = true;
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

        [HttpPost]
        public ActionResult DeleteProductDeposit_Temp(string ID)
        {
            List<Product_Detail> lstOrderProduct = new List<Product_Detail>();
            if (Session[Sessions.lstProductInput] != null)
            {
                lstOrderProduct = (List<Product_Detail>)Session[Sessions.lstProductInput];
            }
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                v_v_dm_HangHoa dm_HangHoa = new v_v_dm_HangHoa();
                var check = lstOrderProduct.Where(e => e.ID == ID).FirstOrDefault();
                if (check != null && lstOrderProduct != null)
                {
                    if (!string.IsNullOrEmpty(check.ID_COMBO))
                    {
                        foreach (var itm in lstOrderProduct.Where(e => e.ID_COMBO == check.ID_COMBO).ToList())
                            lstOrderProduct.Remove(itm);
                    }
                    else
                        lstOrderProduct.Remove(check);
                }
                if(!check.ISKHUYENMAI)
                    lstOrderProduct = XoaKhuyenMai(lstOrderProduct);
                Session[Sessions.lstProductInput] = lstOrderProduct;
                apiResponse.ProductCombo = Utility.GetProductInputOutput(lstOrderProduct, "Deposit_Temp");
                apiResponse.Success = true;
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

        [HttpPost]
        public ActionResult DeleteAllProductDeposit_Temp()
        {

            ApiResponse apiResponse = new ApiResponse();
            try
            {
                List<Product_Detail> lstOrderProduct = new List<Product_Detail>();
                Session[Sessions.lstProductInput] = lstOrderProduct;
                apiResponse.ProductCombo = Utility.GetProductInputOutput(lstOrderProduct, "Deposit_Temp");
                apiResponse.Success = true;
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

        private string myProduct(string sbtnGroup, string Class)
        {
            return " <div class=\"panel-group\" id=\"accordion1\"><div class=\"panel panel-default\"><div class=\"panel-heading\">    <h1 class=\"panel-title\">        <a data-toggle=\"collapse\" data-parent=\"#accordion1\" href=\"#collapseOne1\">            NHÓM HÀNG HÓA        </a>    </h1> <div class='ckbox ckbox-default'><input type='checkbox' id='selectall' value='0'><label for='selectall'>Tất cả(Bao gồm hết tồn kho)</label></div></div><div id=\"collapseOne1\" class=\"panel-collapse collapse in\">    <div id=\"myBtnContainer\">" + sbtnGroup + "</div></div></div></div>\r\n<div class=\"panel-group\" id=\"accordion2\"><div class=\"panel panel-default\"><div class=\"panel-heading\">    <h1 class=\"panel-title\">        <a data-toggle=\"collapse\" data-parent=\"#accordion2\" href=\"#collapseOne2\">            DANH SÁCH HÀNG HÓA        </a>    </h1></div><div id=\"collapseOne2\" class=\"panel-collapse collapse in\">    <div>        <input id=\"myInput\" type=\"text\" placeholder=\""+ Utility.TimKiem + "\" class=\"form-control\" onkeyup=\"myInputOnkeyup(\'" + Class + "\', event)\" style=\"width:300px;display:inline-block\">        <button class='btn btn-default' onclick='funSearchItemProduct(\"" + Class + "\")'><span class='glyphicon glyphicon-search'></span></button>    </div><div id=\"myTest\">    </div>    <div id=\"mycontainer\" class=\"productDeposit-list\">    </div></div></div></div>";
        }

        private List<Product_Detail> XoaKhuyenMai(List<Product_Detail> lstOrderProduct)
        {
            var lstOrderProduct_Delete = new List<Product_Detail>();
            foreach (Product_Detail item in lstOrderProduct) 
            {
                if (item.ISKHUYENMAI)
                    lstOrderProduct_Delete.Add(item);
                else
                {
                    item.CHIETKHAU = 0;
                    item.TYPE = "CHIETKHAU";
                    Utility.TinhTong(item, "0", lstOrderProduct);
                    item.TONGTIENGIAMGIA = 0;
                    item.ISDALAYKHUYENMAI = false;
                    item.ID_KHUYENMAI = "";
                }
            }
            foreach (Product_Detail item in lstOrderProduct_Delete)
                lstOrderProduct.Remove(item);
            return lstOrderProduct;
        }
        #region Tạo phiếu xuất từ phiếu đặt hàng
        public ActionResult OnSubmitDeposit(String cartOrder, int HINHTHUC = 0)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (Utility.KiemTra())
            {
                apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Admin");
                return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
            }
            if (ModelState.IsValid)
            {
                Return newReturn = new Return();
                var lstcartOrder = new JavaScriptSerializer().Deserialize<List<Deposit>>(cartOrder);
                foreach (var Deposit in lstcartOrder)
                {
                    Deposit.ID_NGUOITAO = Session[Sessions.idUser].ToString();
                    Deposit.LOC_ID = Utility.LOC_ID;
                    Deposit.NGAYLAP = Utility.CurrentTime.AddDays(HINHTHUC);
                }
                apiResponse = Utility.Create<List<Deposit>>(lstcartOrder, API.ct_PhieuDatHang+ "/PostCreateOutput");
                if (apiResponse.Success)
                {
                    newReturn.Message = "Tạo phiếu xuất thành công!";
                }
                else
                {
                    ModelState.AddModelError(string.Empty, apiResponse.Message);
                    newReturn.Message = apiResponse.Message;
                }
                return Json(newReturn, JsonRequestBehavior.AllowGet);
            }
            else
            {
                Return newReturn = new Return();
                newReturn.DATA = "";
                return Json(newReturn, JsonRequestBehavior.AllowGet);
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
                v_ct_PhieuDatHang PhieuNhap = new v_ct_PhieuDatHang();
                SP_Parameter objParameter = new SP_Parameter();
                objParameter.ID_PHIEUDATHANG = ID;
                apiResponse = Utility.ExecuteStoredProc<v_ct_PhieuDatHang>(objParameter, API.Sp_Get_DanhSachPhieuDatHang);
                if (!apiResponse.Success)
                {
                    apiResponse.Success = false;
                    apiResponse.Message = apiResponse.Message;
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                if (apiResponse.Data != null)
                    PhieuNhap = (apiResponse.Data as List<v_ct_PhieuDatHang>).FirstOrDefault();

                SP_Parameter_Report objParameter_Report = new SP_Parameter_Report();
                objParameter_Report.LOC_ID = Utility.LOC_ID;
                objParameter_Report.ID_PHIEUDATHANG = ID;
                var report = new ReportClass();
                apiResponse = Utility.ExecuteStoredProc<DataTable>(objParameter_Report, API.Sp_Get_DanhSachPhieuDatHang_ChiTiet);
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
                foreach (DataRow itm in data.Rows)
                {
                    if (data.Columns.Contains("ISKHUYENMAI") && Convert.ToBoolean(itm["ISKHUYENMAI"]) == true)
                    {
                        itm["NAME"] = "(KM)" + itm["NAME"];
                    }
                }
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
                apiResponse.NAME = Utility.GetTitleFrom(API.ct_PhieuDatHang) + " - " + PhieuNhap.MAPHIEU;
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