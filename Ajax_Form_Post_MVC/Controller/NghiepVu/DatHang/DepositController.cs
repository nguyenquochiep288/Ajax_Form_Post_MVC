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

namespace MVC_QuanLyTHP.Controllers
{
    public class DepositController : Controller
    {
        // GET: Deposit_Temp
        #region Deposit_Temp
        public ActionResult Index(int Page = 1, string ID_DEPOT = "", DateTime? FromDate = null, DateTime? ToDate = null, string SearchString = "", string ShowSearchValue = "")
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
                IPagedList<v_ct_PhieuDatHang> lstpage = (new List<v_ct_PhieuDatHang>()).OrderByDescending(s => s.NGAYLAP).ToList().ToPagedList(Page, Utility.GetPageSize()); ;
                if (FromDate != null)
                {
                    apiResponse = Utility.Get_DanhSachPhieuDatHang<v_ct_PhieuDatHang>(ID_DEPOT, FromDate, ToDate, SearchString);
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
                            lstpage = (apiResponse.Data as List<v_ct_PhieuDatHang>).OrderByDescending(s => s.NGAYLAP).ToList().ToPagedList(Page, Utility.GetPageSize());
                        }
                        else
                        {
                            if (Utility.KiemTraQuyen(API.ct_PhieuDatHang, API.UserData))
                                lstpage = (apiResponse.Data as List<v_ct_PhieuDatHang>).Where(s => s.ID_NHANVIEN == Login_Model.iduser).OrderByDescending(s => s.NGAYLAP).ToList().ToPagedList(Page, Utility.GetPageSize());
                        }
                    }
                }

                v_v_ct_PhieuDatHang ct_PhieuDatHang = new v_v_ct_PhieuDatHang();
                ct_PhieuDatHang.IPagedList = lstpage;
                ct_PhieuDatHang.lstdm_ThueSuat = new List<v_dm_ThueSuat>();
                ct_PhieuDatHang.lstdm_ThueSuat = Utility.GetListData<v_dm_ThueSuat>(API.dm_ThueSuat, "", "", Utility.LOC_ID).Data as List<v_dm_ThueSuat>;
                ct_PhieuDatHang.lstdm_Kho = new List<v_dm_Kho>();
                ct_PhieuDatHang.lstdm_Kho = Utility.GetListData<v_dm_Kho>(API.dm_Kho, "", "", Utility.LOC_ID).Data as List<v_dm_Kho>;
                //ct_PhieuDatHang.lstdm_KhachHang = new List<v_dm_KhachHang>();
                //ct_PhieuDatHang.lstdm_KhachHang = Utility.GetListData<v_dm_KhachHang>(API.dm_KhachHang, "", "", Utility.LOC_ID).Data as List<v_dm_KhachHang>;
                //ct_PhieuDatHang.lstdm_NhanVien = new List<v_dm_NhanVien>();
                //ct_PhieuDatHang.lstdm_NhanVien = Utility.GetListData<v_dm_NhanVien>(API.dm_NhanVien, "", "", Utility.LOC_ID).Data as List<v_dm_NhanVien>;

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
                //ct_PhieuDatHang.NGAYLAP = Utility.CurrentTime;
                //ct_PhieuDatHang.LOC_ID = Utility.LOC_ID;
                //ct_PhieuDatHang.SOPHIEU = Utility.GetMaxID<ct_PhieuDatHang>(ct_PhieuDatHang, Utility.LOC_ID, ct_PhieuDatHang.NGAYLAP.ToString("yyyy-MM-dd"));
                //ct_PhieuDatHang.MAPHIEU = API.GetMaPhieu(API.ct_PhieuDatHang, ct_PhieuDatHang.NGAYLAP, ct_PhieuDatHang.SOPHIEU);
                ct_PhieuDatHang.lstdm_ThueSuat = new List<v_dm_ThueSuat>();
                //ct_PhieuDatHang.lstdm_ThueSuat = Utility.GetListData<v_dm_ThueSuat>(API.dm_ThueSuat, "", "", Utility.LOC_ID).Data as List<v_dm_ThueSuat>;
                ct_PhieuDatHang.lstdm_Kho = new List<v_dm_Kho>();
                //ct_PhieuDatHang.lstdm_Kho = Utility.GetListData<v_dm_Kho>(API.dm_Kho, "", "", Utility.LOC_ID).Data as List<v_dm_Kho>;
                ct_PhieuDatHang.lstdm_KhachHang = new List<v_dm_KhachHang>();
                //ct_PhieuDatHang.lstdm_KhachHang = GetDanhSachKhachHang().Data as List<v_dm_KhachHang>;
                List<Product_Detail> lstOrderProduct = new List<Product_Detail>();

                //var KhoISDEFAULT = ct_PhieuDatHang.lstdm_Kho.Where(e => e.ISDEFAULT).FirstOrDefault();
                //if (KhoISDEFAULT != null)
                //{
                //    ct_PhieuDatHang.ID_KHO = KhoISDEFAULT.ID;
                //}
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
                ViewBag.NhomHang = sbtn;
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
                ApiResponse apiResponse = new ApiResponse();
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
                    if (ModelState.IsValid)
                    {
                        ct_PhieuDatHang.ID = Guid.NewGuid().ToString();
                        ct_PhieuDatHang.LOC_ID = Utility.LOC_ID;
                        ct_PhieuDatHang.ID_NGUOITAO = Session[Sessions.idUser].ToString();
                        ct_PhieuDatHang.THOIGIANTHEM = Utility.CurrentTime;
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
                ct_PhieuDatHang.lstdm_KhachHang = new List<v_dm_KhachHang>();
                ct_PhieuDatHang.lstdm_KhachHang = GetDanhSachKhachHang().Data as List<v_dm_KhachHang>;
                ct_PhieuDatHang.lstdm_Kho = new List<v_dm_Kho>();
                ct_PhieuDatHang.lstdm_Kho = Utility.GetListData<v_dm_Kho>(API.dm_Kho, "", "", Utility.LOC_ID).Data as List<v_dm_Kho>;
                var lst = Utility.ConvertobjectToView<v_v_ct_PhieuDatHang>(ct_PhieuDatHang);

                apiResponse.ProductCombo = Utility.GetProductInputOutput(lstOrderProduct, "Deposit_Temp");

                apiResponse.GETPROMOTION = apiResponse.ProductCombo;
                lst.Add(new ValueEdit { Key = "tbodyTempItemInput", Value = apiResponse.ProductCombo });
                apiResponse.Detail = lst;
                return Json(apiResponse, JsonRequestBehavior.AllowGet);
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
                if (!Utility.KiemTraQuyen(API.ct_PhieuDatHang, API.Create))
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
                ct_PhieuDatHang.lstdm_KhachHang = new List<v_dm_KhachHang>();
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
                    return Json(apiResponse, JsonRequestBehavior.AllowGet);
                }
                if (!Utility.KiemTraQuyen(API.ct_PhieuNhap, API.Edit))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                    return Json(apiResponse, JsonRequestBehavior.AllowGet);
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
                    return Json(apiResponse, JsonRequestBehavior.AllowGet);
                }

                ct_PhieuDatHang.lstdm_Kho = new List<v_dm_Kho>();
                ct_PhieuDatHang.lstdm_Kho = Utility.GetListData<v_dm_Kho>(API.dm_Kho, "", "", Utility.LOC_ID).Data as List<v_dm_Kho>;
                ct_PhieuDatHang.lstdm_KhachHang = new List<v_dm_KhachHang>();
                ct_PhieuDatHang.lstdm_KhachHang = GetDanhSachKhachHang().Data as List<v_dm_KhachHang>;

                if(ct_PhieuDatHang.lstdm_KhachHang != null)
                {
                    var KhanhHang = ct_PhieuDatHang.lstdm_KhachHang.Where(e => e.ID == ct_PhieuDatHang.ID_KHACHHANG).FirstOrDefault();
                    if(KhanhHang == null)
                    {
                       var kh =  Utility.GetDetail<v_dm_KhachHang>(Utility.LOC_ID + "/" + id, API.dm_KhachHang);
                        if (!kh.Success)
                        {
                            TempData["TitleError"] = kh.Message;
                            return RedirectToAction("Index", "Notfound");
                        }
                        if (kh.Data != null)
                            ct_PhieuDatHang.lstdm_KhachHang.Add(kh.Data as v_dm_KhachHang);
                    }
                }
                ct_PhieuDatHang.lstdm_ThueSuat = new List<v_dm_ThueSuat>();
                ct_PhieuDatHang.lstdm_ThueSuat = Utility.GetListData<v_dm_ThueSuat>(API.dm_ThueSuat, "", "", Utility.LOC_ID).Data as List<v_dm_ThueSuat>;
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
                    return Json(apiResponse, JsonRequestBehavior.AllowGet);
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
              
                return Json(apiResponse, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
                TempData["TitleError"] = API.TitleTryCatch;
                TempData["DetailError"] = ex.Message;
                apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                return Json(apiResponse, JsonRequestBehavior.AllowGet);
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
                ct_PhieuDatHang.lstdm_KhachHang = new List<v_dm_KhachHang>();
                ct_PhieuDatHang.lstdm_KhachHang = GetDanhSachKhachHang().Data as List<v_dm_KhachHang>;
                ct_PhieuDatHang.lstdm_Kho = new List<v_dm_Kho>();
                ct_PhieuDatHang.lstdm_Kho = Utility.GetListData<v_dm_Kho>(API.dm_Kho, "", "", Utility.LOC_ID).Data as List<v_dm_Kho>;
                var lst = Utility.ConvertobjectToView<v_v_ct_PhieuDatHang>(ct_PhieuDatHang);

                apiResponse.ProductCombo = Utility.GetProductInputOutput(lstOrderProduct, "Deposit_Temp");
                lst.Add(new ValueEdit { Key = "tbodyTempItemInput", Value = apiResponse.ProductCombo });
                apiResponse.Detail = lst;
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
        public ActionResult EditPopup([Bind(Include = "LOC_ID,ID,ID_KHO,MAPHIEU,SOPHIEU,NGAYLAP,ID_KHACHHANG,ID_NHANVIEN,GHICHU,TONGTHANHTIEN,CHIETKHAU,TONGTIENGIAMGIA,TONGTIENVAT,TONGTIEN,BUTTONTYPE,ADDRESS,TEL")] v_v_ct_PhieuDatHang ct_PhieuDatHang)
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                if (Utility.KiemTra())
                {
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Admin");
                    return Json(apiResponse, JsonRequestBehavior.AllowGet);
                }
                if (!Utility.KiemTraQuyen(API.ct_PhieuDatHang, API.Create))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                    return Json(apiResponse, JsonRequestBehavior.AllowGet);
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
                            return Json(apiResponse, JsonRequestBehavior.AllowGet);
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
                            return Json(apiResponse, JsonRequestBehavior.AllowGet);
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
                ct_PhieuDatHang.lstdm_KhachHang = new List<v_dm_KhachHang>();
                ct_PhieuDatHang.lstdm_KhachHang = GetDanhSachKhachHang().Data as List<v_dm_KhachHang>;
                ct_PhieuDatHang.lstdm_Kho = new List<v_dm_Kho>();
                ct_PhieuDatHang.lstdm_Kho = Utility.GetListData<v_dm_Kho>(API.dm_Kho, "", "", Utility.LOC_ID).Data as List<v_dm_Kho>;
                var lst = Utility.ConvertobjectToView<v_v_ct_PhieuDatHang>(ct_PhieuDatHang);

                apiResponse.ProductCombo = Utility.GetProductInputOutput(lstOrderProduct, "Deposit_Temp");
                lst.Add(new ValueEdit { Key = "tbodyTempItemInput", Value = apiResponse.ProductCombo });
                apiResponse.Detail = lst;
                return Json(apiResponse, JsonRequestBehavior.AllowGet);
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
                    return Json(apiResponse, JsonRequestBehavior.AllowGet);
                }
                if (!Utility.KiemTraQuyen(API.ct_PhieuDatHang, API.Delete))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                    return Json(apiResponse, JsonRequestBehavior.AllowGet);
                }
                apiResponse = Utility.GetDetail<v_ct_PhieuDatHang>(Utility.LOC_ID + "/" + id, API.ct_PhieuDatHang);
                if (!apiResponse.Success)
                {
                    TempData["TitleError"] = apiResponse.Message;
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                    return Json(apiResponse, JsonRequestBehavior.AllowGet);
                }
                v_ct_PhieuDatHang chkv_ct_PhieuDatHang = null;
                if (apiResponse.Data != null)
                    chkv_ct_PhieuDatHang = apiResponse.Data as v_ct_PhieuDatHang;

                if (chkv_ct_PhieuDatHang == null || !string.IsNullOrEmpty(chkv_ct_PhieuDatHang.ID_PHIEUXUAT))
                {
                    TempData["TitleError"] = "Phiếu đã thực hiện đặt hàng! Nên không thể sửa phiếu!";
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                    return Json(apiResponse, JsonRequestBehavior.AllowGet);
                }
                apiResponse = Utility.Delete<v_ct_PhieuDatHang>(Utility.LOC_ID + "/" + id, API.ct_PhieuDatHang);
                apiResponse.ID = id;
                return Json(apiResponse, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
                TempData["TitleError"] = API.TitleTryCatch;
                TempData["DetailError"] = ex.Message;
                apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                return Json(apiResponse, JsonRequestBehavior.AllowGet);
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
            ct_PhieuDatHang.lstdm_KhachHang = new List<v_dm_KhachHang>();
            ct_PhieuDatHang.lstdm_KhachHang = GetDanhSachKhachHang().Data as List<v_dm_KhachHang>;
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
            return Json(apiResponse, JsonRequestBehavior.AllowGet);
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
                    newReturn.DataObject = (apiResponse.Data as v_dm_KhachHang);
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
                            strQty = PhanNguyen.ToString("N0") + " " + itm.NAME_DVT + ((itm.QTY - (PhanNguyen * itm.TYLE_QD)) > 0 ? "/" + (itm.QTY - (PhanNguyen * itm.TYLE_QD)).ToString("N0") + " " + itm.NAME_DVT_QD : "");
                        }
                        else
                            strQty = itm.QTY.ToString("N0") + " " + itm.NAME_DVT_QD;



                        newReturn.DATA += "<button style ='width: 150px;height:100px;' class='filterDiv active show' onclick='myFunOpenProduct(this,\"" + itm.ID_HANGHOAKHO + "\")'>" + itm.NAME + "<div><code>SL: " + strQty + "</code> - <code>" + String.Format("{0:N0}", itm.GIA01) + " đ</code></div></button>";
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


            return Json(apiResponse, JsonRequestBehavior.AllowGet);
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
            objParameter.BOLTONKHO = true;
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
        private ApiResponse GetDanhSachKhachHang(string idNhomQuyen = "")
        {
            SP_Parameter objParameter = new SP_Parameter();
            objParameter.LOC_ID = Utility.LOC_ID;
            objParameter.ID_NHOMQUYEN = !string.IsNullOrEmpty(idNhomQuyen) ? idNhomQuyen : Session[Sessions.idNhomQuyen].ToString();
            objParameter.ID_KHUVUC = "-1";
            ApiResponse apiResponse = Utility.ExecuteStoredProc<v_dm_KhachHang>(objParameter, API.web_Sp_Get_DSKhachHang);
            if (!apiResponse.Success)
            {
                apiResponse.Data = new List<v_dm_KhachHang>();
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
                return Json(apiResponse, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex, JsonConvert.SerializeObject(Product_Detail));
                TempData["TitleError"] = API.TitleTryCatch;
                TempData["DetailError"] = ex.Message;
                apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                return Json(apiResponse, JsonRequestBehavior.AllowGet);
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
                    return Json(apiResponse, JsonRequestBehavior.AllowGet);
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
                        return Json(apiResponse, JsonRequestBehavior.AllowGet);
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
                                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                                    return Json(apiResponse, JsonRequestBehavior.AllowGet);
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
                return Json(apiResponse, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex, JsonConvert.SerializeObject(Product_Detail));
                TempData["TitleError"] = API.TitleTryCatch;
                TempData["DetailError"] = ex.Message;
                apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                return Json(apiResponse, JsonRequestBehavior.AllowGet);
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

                Session[Sessions.lstProductInput] = lstOrderProduct;
                apiResponse.ProductCombo = Utility.GetProductInputOutput(lstOrderProduct, "Deposit_Temp");
                apiResponse.Success = true;
                return Json(apiResponse, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
                TempData["TitleError"] = API.TitleTryCatch;
                TempData["DetailError"] = ex.Message;
                apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                return Json(apiResponse, JsonRequestBehavior.AllowGet);
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
                Session[Sessions.lstProductInput] = lstOrderProduct;
                apiResponse.ProductCombo = Utility.GetProductInputOutput(lstOrderProduct, "Deposit_Temp");
                apiResponse.Success = true;
                return Json(apiResponse, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
                TempData["TitleError"] = API.TitleTryCatch;
                TempData["DetailError"] = ex.Message;
                apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                return Json(apiResponse, JsonRequestBehavior.AllowGet);
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
                return Json(apiResponse, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
                TempData["TitleError"] = API.TitleTryCatch;
                TempData["DetailError"] = ex.Message;
                apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                return Json(apiResponse, JsonRequestBehavior.AllowGet);
            }
        }

        private string myProduct(string sbtnGroup, string Class)
        {
            return " <div class=\"panel-group\" id=\"accordion1\"><div class=\"panel panel-default\"><div class=\"panel-heading\">    <h1 class=\"panel-title\">        <a data-toggle=\"collapse\" data-parent=\"#accordion1\" href=\"#collapseOne1\">            NHÓM HÀNG HÓA        </a>    </h1></div><div id=\"collapseOne1\" class=\"panel-collapse collapse in\">    <div id=\"myBtnContainer\">"+ sbtnGroup + "</div></div></div></div>\r\n<div class=\"panel-group\" id=\"accordion2\"><div class=\"panel panel-default\"><div class=\"panel-heading\">    <h1 class=\"panel-title\">        <a data-toggle=\"collapse\" data-parent=\"#accordion2\" href=\"#collapseOne2\">            DANH SÁCH HÀNG HÓA        </a>    </h1></div><div id=\"collapseOne2\" class=\"panel-collapse collapse in\">    <div>        <input id=\"myInput\" type=\"text\" placeholder=\""+ Utility.TimKiem + "\" class=\"form-control\" onkeyup=\"myInputOnkeyup(\'" + Class + "\', event)\" style=\"width:300px;display:inline-block\">        <button class='btnGroup' onclick='funSearchItemProduct(\"" + Class + "\")'><span class='glyphicon glyphicon-search'></span></button>    </div><div id=\"myTest\">    </div>    <div id=\"mycontainer\" class=\"container\">    </div></div></div></div>";
        }

        #region Tạo phiếu xuất từ phiếu đặt hàng
        public ActionResult OnSubmitDeposit(String cartOrder)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (Utility.KiemTra())
            {
                apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Admin");
                return Json(apiResponse, JsonRequestBehavior.AllowGet);
            }
            if (ModelState.IsValid)
            {
                Return newReturn = new Return();
                var lstcartOrder = new JavaScriptSerializer().Deserialize<List<Deposit>>(cartOrder);
                foreach (var Deposit in lstcartOrder)
                {
                    Deposit.ID_NGUOITAO = Session[Sessions.idUser].ToString();
                    Deposit.LOC_ID = Utility.LOC_ID;
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
    }
}