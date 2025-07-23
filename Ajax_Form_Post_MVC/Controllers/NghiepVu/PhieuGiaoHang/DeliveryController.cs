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
using System.Web.Script.Serialization;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using DatabaseTHP.StoredProcedure.Parameter;
using System.Data;
using System.IO;
using Syncfusion.EJ2.BarcodeGenerator;
using System.Diagnostics;
using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;
using CrystalDecisions.ReportAppServer.ReportDefModel;
using System.Drawing.Printing;
using System.Web;
using System.Xml.Linq;

namespace MVC_QuanLyTHP.Controllers
{
    public class DeliveryController : Controller
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
                if (!Utility.KiemTraQuyen(API.ct_PhieuGiaoHang, API.Xem))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                string TotalSum = "";
                // ShowSearchValue = Utility.GetShowSearchValue<ct_PhieuGiaoHang>(ShowSearchValue);
                ApiResponse apiResponse = new ApiResponse();
                IPagedList<v_ct_PhieuGiaoHang> lstpage = (new List<v_ct_PhieuGiaoHang>()).ToList().ToPagedList(Page, Utility.GetPageSize());
                if (FromDate != null || !string.IsNullOrEmpty(IDCODE) || !string.IsNullOrEmpty(MAPHIEU))
                {
                    if (!string.IsNullOrEmpty(IDCODE) || !string.IsNullOrEmpty(MAPHIEU))
                    {
                        apiResponse = Utility.Get_DanhSachPhieuGiaoHang<v_ct_PhieuGiaoHang>("", null, null, MAPHIEU, IDCODE);
                    }
                    if (FromDate != null)
                    {
                        apiResponse = Utility.Get_DanhSachPhieuGiaoHang<v_ct_PhieuGiaoHang>("", FromDate, ToDate, SearchString, "");
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
                    TotalSum = (apiResponse.Data as List<v_ct_PhieuGiaoHang>).Sum(s => s.SOTIENGIAOHANG).ToString("N0");
                    lstpage = (apiResponse.Data as List<v_ct_PhieuGiaoHang>).ToPagedList(Page, Utility.GetPageSize());
                }
                v_v_ct_PhieuGiaoHang ct_PhieuGiaoHang = new v_v_ct_PhieuGiaoHang();
                ct_PhieuGiaoHang.IPagedList = lstpage;
                ct_PhieuGiaoHang.lstdm_Xe = new List<v_dm_Xe>();
                ct_PhieuGiaoHang.lstdm_KhuVuc = new List<v_dm_KhuVuc>();
                ct_PhieuGiaoHang.lstdm_KhuVuc = Utility.GetListData<v_dm_KhuVuc>(API.dm_KhuVuc, "", "", Utility.LOC_ID).Data as List<v_dm_KhuVuc>;

                ViewBag.searchValue = SearchString;
                ViewBag.TotalSum = TotalSum;
                ViewBag.PermissionEdit = Utility.KiemTraQuyen(API.ct_PhieuGiaoHang, API.Edit);
                ViewBag.PermissionDelete = Utility.KiemTraQuyen(API.ct_PhieuGiaoHang, API.Delete);
                ViewBag.PermissionCreate = Utility.KiemTraQuyen(API.ct_PhieuGiaoHang, API.Create);
                ViewBag.PermissionDelivery = Utility.KiemTraQuyen(API.ct_PhieuGiaoHang, API.Delivery);
                ViewBag.fromdate = FromDate != null ? FromDate.Value.ToString("yyyy-MM-dd") : Utility.CurrentTime.AddMonths(-1).ToString("yyyy-MM-dd");
                ViewBag.todate = ToDate != null ? ToDate.Value.ToString("yyyy-MM-dd") : Utility.CurrentTime.ToString("yyyy-MM-dd");
                
                return View(ct_PhieuGiaoHang);
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
        public ActionResult Create(int type = 2)
        {
            try
            {
                Session[Sessions.IntWidth] = type;
                if (Utility.KiemTra())
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (!Utility.KiemTraQuyen(API.ct_PhieuGiaoHang, API.Create))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                v_v_ct_PhieuGiaoHang ct_PhieuGiaoHang = new v_v_ct_PhieuGiaoHang();
                ct_PhieuGiaoHang.LOC_ID = Utility.LOC_ID;
                ct_PhieuGiaoHang.ID_NGUOITAO = Session[Sessions.idUser].ToString();
                ct_PhieuGiaoHang.THOIGIANTHEM = Utility.CurrentTime;
                ct_PhieuGiaoHang.NGAYLAP = Utility.CurrentTime;
                ct_PhieuGiaoHang.SOPHIEU = Utility.GetMaxID<ct_PhieuGiaoHang>(ct_PhieuGiaoHang, Utility.LOC_ID, ct_PhieuGiaoHang.NGAYLAP.ToString("yyyy-MM-dd"));
                ct_PhieuGiaoHang.MAPHIEU = API.GetMaPhieu(API.ct_PhieuGiaoHang, ct_PhieuGiaoHang.NGAYLAP, ct_PhieuGiaoHang.SOPHIEU);
                ct_PhieuGiaoHang.lstdm_Xe = new List<v_dm_Xe>();

                ct_PhieuGiaoHang.ID = Guid.NewGuid().ToString();
                return View(ct_PhieuGiaoHang);
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
        public ActionResult Create([Bind(Include = "LOC_ID,ID,ID_XEGIAOHANG,MAPHIEU,SOPHIEU,NGAYLAP,GHICHU,ISHOANTAT,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO")] v_ct_PhieuGiaoHang ct_PhieuGiaoHang)
        {
            try
            {
                if (Utility.KiemTra())
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (!Utility.KiemTraQuyen(API.ct_PhieuGiaoHang, API.Create))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                if (ModelState.IsValid)
                {
                    ct_PhieuGiaoHang.LOC_ID = Utility.LOC_ID;
                    ct_PhieuGiaoHang.ID_NGUOITAO = Session[Sessions.idUser].ToString();
                    ct_PhieuGiaoHang.THOIGIANTHEM = Utility.CurrentTime;

                    //@ConvertObjectUnicodeToTCVN3
                    var apiResponse = Utility.Create<ct_PhieuGiaoHang>(ct_PhieuGiaoHang, API.ct_PhieuGiaoHang);
                    if (apiResponse.Success)
                        return RedirectToAction("Index");
                    else
                    {
                        ModelState.AddModelError(string.Empty, apiResponse.Message);
                        if (apiResponse.CheckValue)
                            ViewBag.ID = Guid.NewGuid().ToString();
                    }
                }
                return View(ct_PhieuGiaoHang);
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
                if (!Utility.KiemTraQuyen(API.ct_PhieuGiaoHang, API.Edit))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                v_v_ct_PhieuGiaoHang ct_PhieuGiaoHang = new v_v_ct_PhieuGiaoHang();
                if (!string.IsNullOrEmpty(id))
                {
                    apiResponse = Utility.GetDetail<v_ct_PhieuGiaoHang>(Utility.LOC_ID + "/" + id, API.ct_PhieuGiaoHang);
                    if (!apiResponse.Success)
                    {
                        TempData["TitleError"] = apiResponse.Message;
                        return RedirectToAction("Index", "Notfound");
                    }
                    if (apiResponse.Data != null)
                        ct_PhieuGiaoHang = apiResponse.Data as v_v_ct_PhieuGiaoHang;
                }
                //@ConvertObjectTCVN3ToUnicode
                ct_PhieuGiaoHang.lstdm_Xe = new List<v_dm_Xe>();

                return View(ct_PhieuGiaoHang);
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
        public ActionResult Edit([Bind(Include = "LOC_ID,ID,ID_XEGIAOHANG,MAPHIEU,SOPHIEU,NGAYLAP,GHICHU,ISHOANTAT,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO")] v_ct_PhieuGiaoHang ct_PhieuGiaoHang)
        {
            try
            {
                if (Utility.KiemTra())
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (!Utility.KiemTraQuyen(API.ct_PhieuGiaoHang, API.Edit))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                if (ModelState.IsValid)
                {
                    ct_PhieuGiaoHang.LOC_ID = Utility.LOC_ID;
                    ct_PhieuGiaoHang.ID_NGUOISUA = Session[Sessions.idUser].ToString();
                    ct_PhieuGiaoHang.THOIGIANSUA = Utility.CurrentTime;
                    //@ConvertObjectUnicodeToTCVN3
                    var apiResponse = Utility.Edit<v_ct_PhieuGiaoHang>(Utility.LOC_ID + "/" + ct_PhieuGiaoHang.ID, ct_PhieuGiaoHang, API.ct_PhieuGiaoHang);
                    if (apiResponse.Success)
                        return RedirectToAction("Index");
                    else
                        ModelState.AddModelError(string.Empty, apiResponse.Message);
                }
                return View(ct_PhieuGiaoHang);
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
                if (!Utility.KiemTraQuyen(API.ct_PhieuGiaoHang, API.Delete))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                var apiResponse = Utility.Delete<v_ct_PhieuGiaoHang>(Utility.LOC_ID + "/" + id, API.ct_PhieuGiaoHang);
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
        public ActionResult CreatePopup(int HINHTHUC = 0)
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                if (Utility.KiemTra())
                {
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Admin");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                if (!Utility.KiemTraQuyen(API.ct_PhieuGiaoHang, API.Create))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }

                v_v_ct_PhieuGiaoHang ct_PhieuGiaoHang = new v_v_ct_PhieuGiaoHang();
                apiResponse.Success = true;
                ct_PhieuGiaoHang.LOC_ID = Utility.LOC_ID;
                ct_PhieuGiaoHang.ID = Guid.NewGuid().ToString();
                ct_PhieuGiaoHang.NGAYLAP = Utility.CurrentTime.AddDays(HINHTHUC);
                ct_PhieuGiaoHang.SOPHIEU = Utility.GetMaxID<ct_PhieuGiaoHang>(ct_PhieuGiaoHang, Utility.LOC_ID, ct_PhieuGiaoHang.NGAYLAP.ToString("yyyy-MM-dd"));
                ct_PhieuGiaoHang.MAPHIEU = API.GetMaPhieu(API.ct_PhieuGiaoHang, ct_PhieuGiaoHang.NGAYLAP, ct_PhieuGiaoHang.SOPHIEU);

                ct_PhieuGiaoHang.lstdm_Xe = new List<v_dm_Xe>();
                ct_PhieuGiaoHang.lstdm_Xe = Utility.GetListData<v_dm_Xe>(API.dm_Xe, "", "", Utility.LOC_ID).Data as List<v_dm_Xe>;

                Session[Sessions.lstDelivery_Detail] = new List<v_ct_PhieuGiaoHang_ChiTiet>();
                Session[Sessions.lstDelivery_Shipper] = new List<v_ct_PhieuGiaoHang_NhanVienGiao>();
                ct_PhieuGiaoHang.lstct_PhieuGiaoHang_ChiTiet = new List<v_ct_PhieuGiaoHang_ChiTiet>();
                ct_PhieuGiaoHang.lstct_PhieuGiaoHang_NhanVienGiao = new List<v_ct_PhieuGiaoHang_NhanVienGiao>();

                var lst = Utility.ConvertobjectTo<v_v_ct_PhieuGiaoHang>(ct_PhieuGiaoHang);
                apiResponse.ProductCombo = Utility.GetDelivery_Detail(new List<v_ct_PhieuGiaoHang_ChiTiet>());
                lst.Add(new ValueEdit { Key = "lstct_PhieuGiaoHang_ChiTiet", Value = apiResponse.ProductCombo });
                apiResponse.ProductCombo = Utility.GetDelivery_Shipper(new List<v_ct_PhieuGiaoHang_NhanVienGiao>());
                lst.Add(new ValueEdit { Key = "lstct_PhieuGiaoHang_NhanVienGiao", Value = apiResponse.ProductCombo });
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

        // POST: Menu/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult CreatePopup([Bind(Include = "LOC_ID,ID,ID_XEGIAOHANG,MAPHIEU,SOPHIEU,NGAYLAP,GHICHU,ISHOANTAT,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO")] v_v_ct_PhieuGiaoHang ct_PhieuGiaoHang)
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                if (Utility.KiemTra())
                {
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Admin");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                if (!Utility.KiemTraQuyen(API.ct_PhieuGiaoHang, API.Create))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }

                var lstKey = Request.Form.AllKeys.Where(e => e.StartsWith("txtDetail"));
                if (lstKey == null || lstKey.Count() == 0)
                {

                    ModelState.AddModelError("lstct_PhieuGiaoHang_ChiTiet", "Thêm danh sách phiếu xuất.");
                }

                var lstKey_Shipper = Request.Form.AllKeys.Where(e => e.StartsWith("txtShipper"));
                if (lstKey == null || lstKey.Count() == 0)
                {

                    ModelState.AddModelError("lstct_PhieuGiaoHang_NhanVienGiao", "Thêm nhân viên giao.");
                }
                if (ModelState.IsValid)
                {
                    ct_PhieuGiaoHang.NGAYLAP = ct_PhieuGiaoHang.NGAYLAP.AddHours(Utility.CurrentTime.Hour).AddMinutes(Utility.CurrentTime.Minute);
                    ct_PhieuGiaoHang.LOC_ID = Utility.LOC_ID;
                    ct_PhieuGiaoHang.ID_NGUOITAO = Session[Sessions.idUser].ToString();
                    ct_PhieuGiaoHang.THOIGIANTHEM = Utility.CurrentTime;
                    ct_PhieuGiaoHang.lstct_PhieuGiaoHang_ChiTiet = new List<v_ct_PhieuGiaoHang_ChiTiet>();
                    ct_PhieuGiaoHang.lstct_PhieuGiaoHang_NhanVienGiao = new List<v_ct_PhieuGiaoHang_NhanVienGiao>();
                    v_ct_PhieuGiaoHang_ChiTiet ct_PhieuGiaoHang_ChiTiet = new v_ct_PhieuGiaoHang_ChiTiet();

                    foreach (string Key in lstKey)
                    {
                        var lstString = Key.ToString().Split('|');
                        var value = HttpContext.Request.Params.GetValues(Key.ToString());
                        string ShowSearchValue = clsMaHoa.Decrypt(lstString[1].ToString(), clsMaHoa.PassMaHoa);
                        var Checkct_PhieuGiaoHang_ChiTiet = JsonConvert.DeserializeObject<v_ct_PhieuGiaoHang_ChiTiet>(ShowSearchValue);
                        if (ct_PhieuGiaoHang_ChiTiet.ID != Checkct_PhieuGiaoHang_ChiTiet.ID)
                        {
                            ct_PhieuGiaoHang_ChiTiet = new v_ct_PhieuGiaoHang_ChiTiet();
                            ct_PhieuGiaoHang_ChiTiet = JsonConvert.DeserializeObject<v_ct_PhieuGiaoHang_ChiTiet>(ShowSearchValue);
                            ct_PhieuGiaoHang_ChiTiet.LOC_ID = ct_PhieuGiaoHang.LOC_ID;
                            ct_PhieuGiaoHang.lstct_PhieuGiaoHang_ChiTiet.Add(ct_PhieuGiaoHang_ChiTiet);
                        }
                        Utility.EditObject(ct_PhieuGiaoHang_ChiTiet, lstString[0].ToString().Substring(3, lstString[0].ToString().Length - 3), value[0]);
                    }

                    v_ct_PhieuGiaoHang_NhanVienGiao ct_PhieuGiaoHang_NhanVienGiao = new v_ct_PhieuGiaoHang_NhanVienGiao();
                    foreach (string Key in lstKey_Shipper)
                    {
                        var lstString = Key.ToString().Split('|');
                        var value = HttpContext.Request.Params.GetValues(Key.ToString());
                        string ShowSearchValue = clsMaHoa.Decrypt(lstString[1].ToString(), clsMaHoa.PassMaHoa);
                        var Checkct_PhieuGiaoHang_ChiTiet = JsonConvert.DeserializeObject<v_ct_PhieuGiaoHang_NhanVienGiao>(ShowSearchValue);
                        if (ct_PhieuGiaoHang_NhanVienGiao.ID != Checkct_PhieuGiaoHang_ChiTiet.ID)
                        {
                            ct_PhieuGiaoHang_NhanVienGiao = new v_ct_PhieuGiaoHang_NhanVienGiao();
                            ct_PhieuGiaoHang_NhanVienGiao = JsonConvert.DeserializeObject<v_ct_PhieuGiaoHang_NhanVienGiao>(ShowSearchValue);
                            ct_PhieuGiaoHang_NhanVienGiao.LOC_ID = ct_PhieuGiaoHang.LOC_ID;
                            ct_PhieuGiaoHang.lstct_PhieuGiaoHang_NhanVienGiao.Add(ct_PhieuGiaoHang_NhanVienGiao);
                        }
                        Utility.EditObject(ct_PhieuGiaoHang_NhanVienGiao, lstString[0].ToString().Substring(3, lstString[0].ToString().Length - 3), value[0]);
                    }

                    apiResponse = Utility.Create<v_ct_PhieuGiaoHang>(ct_PhieuGiaoHang, API.ct_PhieuGiaoHang);
                    if (apiResponse.Success)
                    {
                        ct_PhieuGiaoHang.NGAYLAP = Utility.CurrentTime;
                        apiResponse.SOPHIEU = ct_PhieuGiaoHang.SOPHIEU = Utility.GetMaxID<ct_PhieuGiaoHang>(ct_PhieuGiaoHang, Utility.LOC_ID, ct_PhieuGiaoHang.NGAYLAP.ToString("yyyy-MM-dd"));
                        ct_PhieuGiaoHang.MAPHIEU = API.GetMaPhieu(API.ct_PhieuGiaoHang, ct_PhieuGiaoHang.NGAYLAP, ct_PhieuGiaoHang.SOPHIEU);
                        apiResponse.NewID = Guid.NewGuid().ToString();
                        apiResponse.MAPHIEU = ct_PhieuGiaoHang.MAPHIEU;

                        if (apiResponse.Data != null)
                            ct_PhieuGiaoHang = JsonConvert.DeserializeObject<v_v_ct_PhieuGiaoHang>(apiResponse.Data.ToString());

                        ct_PhieuGiaoHang.lstct_PhieuGiaoHang_ChiTiet = new List<v_ct_PhieuGiaoHang_ChiTiet>();
                        ct_PhieuGiaoHang.lstct_PhieuGiaoHang_NhanVienGiao = new List<v_ct_PhieuGiaoHang_NhanVienGiao>();
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, apiResponse.Message);
                        if (apiResponse.CheckValue)
                        {
                            ct_PhieuGiaoHang.NGAYLAP = Utility.CurrentTime;
                            apiResponse.SOPHIEU = ct_PhieuGiaoHang.SOPHIEU = Utility.GetMaxID<ct_PhieuGiaoHang>(ct_PhieuGiaoHang, Utility.LOC_ID, ct_PhieuGiaoHang.NGAYLAP.ToString("yyyy-MM-dd"));
                            ct_PhieuGiaoHang.MAPHIEU = API.GetMaPhieu(API.ct_PhieuGiaoHang, ct_PhieuGiaoHang.NGAYLAP, ct_PhieuGiaoHang.SOPHIEU);
                            apiResponse.NewID = Guid.NewGuid().ToString();
                            apiResponse.MAPHIEU = ct_PhieuGiaoHang.MAPHIEU;
                        }
                    }
                }
                else
                {
                    apiResponse.Success = false;
                    apiResponse.Data = Utility.GetModelState(ModelState, API.ct_PhieuGiaoHang);
                }
                apiResponse.ID = ct_PhieuGiaoHang.ID;

                ct_PhieuGiaoHang.lstdm_Xe = new List<v_dm_Xe>();
                ct_PhieuGiaoHang.lstdm_Xe = Utility.GetListData<v_dm_Xe>(API.dm_Xe, "", "", Utility.LOC_ID).Data as List<v_dm_Xe>;


                var lst = Utility.ConvertobjectToView<v_v_ct_PhieuGiaoHang>(ct_PhieuGiaoHang);
                apiResponse.ProductCombo = Utility.GetDelivery_Detail(ct_PhieuGiaoHang.lstct_PhieuGiaoHang_ChiTiet);
                lst.Add(new ValueEdit { Key = "lstct_PhieuGiaoHang_ChiTiet", Value = apiResponse.ProductCombo });
                apiResponse.ProductCombo = Utility.GetDelivery_Shipper(ct_PhieuGiaoHang.lstct_PhieuGiaoHang_NhanVienGiao);
                lst.Add(new ValueEdit { Key = "lstct_PhieuGiaoHang_NhanVienGiao", Value = apiResponse.ProductCombo });
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
                if (!Utility.KiemTraQuyen(API.ct_PhieuGiaoHang, API.Edit))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }

                v_v_ct_PhieuGiaoHang ct_PhieuGiaoHang = new v_v_ct_PhieuGiaoHang();
                if (!string.IsNullOrEmpty(id))
                {
                    apiResponse = Utility.GetDetail<v_v_ct_PhieuGiaoHang>(Utility.LOC_ID + "/" + id, API.ct_PhieuGiaoHang);
                    if (!apiResponse.Success)
                    {
                        TempData["TitleError"] = apiResponse.Message;
                        apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                        return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                    }
                    if (apiResponse.Data != null)
                        ct_PhieuGiaoHang = apiResponse.Data as v_v_ct_PhieuGiaoHang;
                }

                ct_PhieuGiaoHang.lstdm_Xe = new List<v_dm_Xe>();
                ct_PhieuGiaoHang.lstdm_Xe = Utility.GetListData<v_dm_Xe>(API.dm_Xe, "", "", Utility.LOC_ID).Data as List<v_dm_Xe>;

                List<v_ct_PhieuGiaoHang_ChiTiet> lstct_PhieuGiaoHang_ChiTiet = new List<v_ct_PhieuGiaoHang_ChiTiet>();
                foreach (var itm in ct_PhieuGiaoHang.lstct_PhieuGiaoHang_ChiTiet)
                {
                    lstct_PhieuGiaoHang_ChiTiet.Add(itm);
                }

                List<v_ct_PhieuGiaoHang_NhanVienGiao> lstct_PhieuGiaoHang_NhanVienGiao = new List<v_ct_PhieuGiaoHang_NhanVienGiao>();
                foreach (var itm in ct_PhieuGiaoHang.lstct_PhieuGiaoHang_NhanVienGiao)
                {
                    lstct_PhieuGiaoHang_NhanVienGiao.Add(itm);
                }
                Session[Sessions.lstDelivery_Detail] = lstct_PhieuGiaoHang_ChiTiet;
                Session[Sessions.lstDelivery_Shipper] = lstct_PhieuGiaoHang_NhanVienGiao;

                var lst = Utility.ConvertobjectTo<v_v_ct_PhieuGiaoHang>(ct_PhieuGiaoHang);
                apiResponse.ProductCombo = Utility.GetDelivery_Detail(lstct_PhieuGiaoHang_ChiTiet);
                lst.Add(new ValueEdit { Key = "lstct_PhieuGiaoHang_ChiTietEdit", Value = apiResponse.ProductCombo });
                apiResponse.ProductCombo = Utility.GetDelivery_Shipper(lstct_PhieuGiaoHang_NhanVienGiao);
                lst.Add(new ValueEdit { Key = "lstct_PhieuGiaoHang_NhanVienGiaoEdit", Value = apiResponse.ProductCombo });
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
        public ActionResult EditPopup([Bind(Include = "LOC_ID,ID,ID_XEGIAOHANG,MAPHIEU,SOPHIEU,NGAYLAP,GHICHU,ISHOANTAT,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO")] v_v_ct_PhieuGiaoHang ct_PhieuGiaoHang)
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                if (Utility.KiemTra())
                {
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Admin");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                if (!Utility.KiemTraQuyen(API.ct_PhieuGiaoHang, API.Edit))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }

                var lstKey = Request.Form.AllKeys.Where(e => e.StartsWith("txtDetail"));
                if (lstKey == null || lstKey.Count() == 0)
                {

                    ModelState.AddModelError("lstct_PhieuGiaoHang_ChiTiet", "Thêm danh sách phiếu xuất.");
                }

                var lstKey_Shipper = Request.Form.AllKeys.Where(e => e.StartsWith("txtShipper"));
                if (lstKey == null || lstKey.Count() == 0)
                {

                    ModelState.AddModelError("lstct_PhieuGiaoHang_NhanVienGiao", "Thêm nhân viên giao.");
                }

                if (ModelState.IsValid)
                {
                    ct_PhieuGiaoHang.LOC_ID = Utility.LOC_ID;
                    ct_PhieuGiaoHang.ID_NGUOISUA = Session[Sessions.idUser].ToString();
                    ct_PhieuGiaoHang.THOIGIANSUA = Utility.CurrentTime;
                    ct_PhieuGiaoHang.lstct_PhieuGiaoHang_ChiTiet = new List<v_ct_PhieuGiaoHang_ChiTiet>();
                    ct_PhieuGiaoHang.lstct_PhieuGiaoHang_NhanVienGiao = new List<v_ct_PhieuGiaoHang_NhanVienGiao>();
                    v_ct_PhieuGiaoHang_ChiTiet ct_PhieuGiaoHang_ChiTiet = new v_ct_PhieuGiaoHang_ChiTiet();

                    foreach (string Key in lstKey)
                    {
                        var lstString = Key.ToString().Split('|');
                        var value = HttpContext.Request.Params.GetValues(Key.ToString());
                        string ShowSearchValue = clsMaHoa.Decrypt(lstString[1].ToString(), clsMaHoa.PassMaHoa);
                        var Checkct_PhieuGiaoHang_ChiTiet = JsonConvert.DeserializeObject<v_ct_PhieuGiaoHang_ChiTiet>(ShowSearchValue);
                        if (ct_PhieuGiaoHang_ChiTiet.ID != Checkct_PhieuGiaoHang_ChiTiet.ID)
                        {
                            ct_PhieuGiaoHang_ChiTiet = new v_ct_PhieuGiaoHang_ChiTiet();
                            ct_PhieuGiaoHang_ChiTiet = JsonConvert.DeserializeObject<v_ct_PhieuGiaoHang_ChiTiet>(ShowSearchValue);
                            ct_PhieuGiaoHang_ChiTiet.LOC_ID = ct_PhieuGiaoHang.LOC_ID;
                            ct_PhieuGiaoHang.lstct_PhieuGiaoHang_ChiTiet.Add(ct_PhieuGiaoHang_ChiTiet);
                        }
                        Utility.EditObject(ct_PhieuGiaoHang_ChiTiet, lstString[0].ToString().Substring(3, lstString[0].ToString().Length - 3), value[0]);
                    }

                    v_ct_PhieuGiaoHang_NhanVienGiao ct_PhieuGiaoHang_NhanVienGiao = new v_ct_PhieuGiaoHang_NhanVienGiao();
                    foreach (string Key in lstKey_Shipper)
                    {
                        var lstString = Key.ToString().Split('|');
                        var value = HttpContext.Request.Params.GetValues(Key.ToString());
                        string ShowSearchValue = clsMaHoa.Decrypt(lstString[1].ToString(), clsMaHoa.PassMaHoa);
                        var Checkct_PhieuGiaoHang_ChiTiet = JsonConvert.DeserializeObject<v_ct_PhieuGiaoHang_NhanVienGiao>(ShowSearchValue);
                        if (ct_PhieuGiaoHang_NhanVienGiao.ID != Checkct_PhieuGiaoHang_ChiTiet.ID)
                        {
                            ct_PhieuGiaoHang_NhanVienGiao = new v_ct_PhieuGiaoHang_NhanVienGiao();
                            ct_PhieuGiaoHang_NhanVienGiao = JsonConvert.DeserializeObject<v_ct_PhieuGiaoHang_NhanVienGiao>(ShowSearchValue);
                            ct_PhieuGiaoHang_NhanVienGiao.LOC_ID = ct_PhieuGiaoHang.LOC_ID;
                            ct_PhieuGiaoHang.lstct_PhieuGiaoHang_NhanVienGiao.Add(ct_PhieuGiaoHang_NhanVienGiao);
                        }
                        Utility.EditObject(ct_PhieuGiaoHang_NhanVienGiao, lstString[0].ToString().Substring(3, lstString[0].ToString().Length - 3), value[0]);
                    }
                    apiResponse = Utility.Edit<v_ct_PhieuGiaoHang>(Utility.LOC_ID + "/" + ct_PhieuGiaoHang.ID, ct_PhieuGiaoHang, API.ct_PhieuGiaoHang);
                    if (apiResponse.Success)
                    {
                        //return RedirectToAction("Index");
                        apiResponse.ID = ct_PhieuGiaoHang.ID;
                        if (apiResponse.Data != null)
                            ct_PhieuGiaoHang = JsonConvert.DeserializeObject<v_v_ct_PhieuGiaoHang>(apiResponse.Data.ToString());
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, apiResponse.Message);
                    }
                }
                else
                {
                    apiResponse.Success = false;
                    apiResponse.Data = Utility.GetModelState(ModelState, API.ct_PhieuGiaoHang);
                }

                ct_PhieuGiaoHang.lstdm_Xe = new List<v_dm_Xe>();
                ct_PhieuGiaoHang.lstdm_Xe = Utility.GetListData<v_dm_Xe>(API.dm_Xe, "", "", Utility.LOC_ID).Data as List<v_dm_Xe>;

                apiResponse.Detail = Utility.ConvertobjectToView<v_v_ct_PhieuGiaoHang>(ct_PhieuGiaoHang);
                var lst = Utility.ConvertobjectToView<v_v_ct_PhieuGiaoHang>(ct_PhieuGiaoHang);
                apiResponse.ProductCombo = Utility.GetDelivery_Detail(ct_PhieuGiaoHang.lstct_PhieuGiaoHang_ChiTiet);
                lst.Add(new ValueEdit { Key = "lstct_PhieuGiaoHang_ChiTietEdit", Value = apiResponse.ProductCombo });
                apiResponse.ProductCombo = Utility.GetDelivery_Shipper(ct_PhieuGiaoHang.lstct_PhieuGiaoHang_NhanVienGiao);
                lst.Add(new ValueEdit { Key = "lstct_PhieuGiaoHang_NhanVienGiaoEdit", Value = apiResponse.ProductCombo });
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
                if (!Utility.KiemTraQuyen(API.ct_PhieuGiaoHang, API.Delete))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                apiResponse = Utility.Delete<v_ct_PhieuGiaoHang>(Utility.LOC_ID + "/" + id, API.ct_PhieuGiaoHang);
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

        #region Chi tiết phiếu xuất phiếu giao hàng 
        [HttpGet]
        public ActionResult AddDeliveryDetail(String cartOrder)
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                if (Utility.KiemTra())
                {
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Admin");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                v_v_ct_PhieuXuat ct_PhieuXuat = new v_v_ct_PhieuXuat();
                var lstProduct = Utility.LstPhieuGiaoHang_ChiTiet;
                Return newReturn = new Return();
                var lstcartOrder = new JavaScriptSerializer().Deserialize<List<Deposit>>(cartOrder);
                foreach (var Deposit in lstcartOrder)
                {
                    if (lstProduct.Where(e => e.ID_PHIEUXUAT == Deposit.ID).Count() > 0)
                    {
                        apiResponse.Success = true;
                    }
                    else
                    {
                        apiResponse = Utility.GetDetail<v_v_ct_PhieuXuat>(Utility.LOC_ID + "/" + Deposit.ID, API.ct_PhieuXuat);
                        if (!apiResponse.Success)
                        {
                            TempData["TitleError"] = apiResponse.Message;
                            return RedirectToAction("Index", "Notfound");
                        }
                        if (apiResponse.Data != null)
                            ct_PhieuXuat = apiResponse.Data as v_v_ct_PhieuXuat;

                        v_ct_PhieuGiaoHang_ChiTiet newv_ct_PhieuGiaoHang_ChiTiet = new v_ct_PhieuGiaoHang_ChiTiet();
                        newv_ct_PhieuGiaoHang_ChiTiet.ID = Guid.NewGuid().ToString();
                        newv_ct_PhieuGiaoHang_ChiTiet.ID_PHIEUXUAT = ct_PhieuXuat.ID;
                        newv_ct_PhieuGiaoHang_ChiTiet.MAPHIEUXUAT = ct_PhieuXuat.MAPHIEU;
                        newv_ct_PhieuGiaoHang_ChiTiet.NGAYLAP = ct_PhieuXuat.NGAYLAP;
                        newv_ct_PhieuGiaoHang_ChiTiet.ID_KHACHHANG_NCC = ct_PhieuXuat.ID_KHACHHANG;
                        newv_ct_PhieuGiaoHang_ChiTiet.NAME_KHACHHANG_NCC = ct_PhieuXuat.NAME_KHACHHANG_NCC;
                        newv_ct_PhieuGiaoHang_ChiTiet.SOTIENGIAOHANG = ct_PhieuXuat.TONGTIEN;
                        newv_ct_PhieuGiaoHang_ChiTiet.TONGSOLUONG = ct_PhieuXuat.lstct_PhieuXuat_ChiTiet.Sum(e => e.SOLUONG);
                        newv_ct_PhieuGiaoHang_ChiTiet.TONGKHOILUONG = ct_PhieuXuat.lstct_PhieuXuat_ChiTiet.Sum(e => e.TONGSOLUONG * e.TRONGLUONG);
                        lstProduct.Add(newv_ct_PhieuGiaoHang_ChiTiet);
                    }
                }
                Session[Sessions.lstDelivery_Detail] = lstProduct;
                List<ValueEdit> lst = new List<ValueEdit>();
                apiResponse.ProductCombo = Utility.GetDelivery_Detail(lstProduct);
                lst.Add(new ValueEdit { Key = "lstct_PhieuGiaoHang_ChiTietEdit", Value = apiResponse.ProductCombo });
                lst.Add(new ValueEdit { Key = "lstct_PhieuGiaoHang_ChiTiet", Value = apiResponse.ProductCombo });
                apiResponse.Detail = lst;
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

        [HttpGet]
        public ActionResult AddDeliveryShipper(string ID)
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                if (Utility.KiemTra())
                {
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Admin");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                v_dm_NhanVien dm_NhanVien = new v_dm_NhanVien();
                var lstProduct = Utility.LstPhieuGiaoHang_NhanVienGiao;
                if (lstProduct.Where(e => e.ID_NHANVIENGIAO == ID).Count() > 0)
                {

                }
                else
                {
                    apiResponse = Utility.GetDetail<v_dm_NhanVien>(Utility.LOC_ID + "/" + ID, API.dm_NhanVien);
                    if (!apiResponse.Success)
                    {
                        TempData["TitleError"] = apiResponse.Message;
                        return RedirectToAction("Index", "Notfound");
                    }
                    if (apiResponse.Data != null)
                        dm_NhanVien = apiResponse.Data as v_dm_NhanVien;

                    v_ct_PhieuGiaoHang_NhanVienGiao newv_ct_PhieuGiaoHang_NhanVienGiao = new v_ct_PhieuGiaoHang_NhanVienGiao();
                    newv_ct_PhieuGiaoHang_NhanVienGiao.ID = Guid.NewGuid().ToString();
                    newv_ct_PhieuGiaoHang_NhanVienGiao.ID_NHANVIENGIAO = dm_NhanVien.ID;
                    newv_ct_PhieuGiaoHang_NhanVienGiao.MA_NHANVIEN = dm_NhanVien.MA;
                    newv_ct_PhieuGiaoHang_NhanVienGiao.NAME_NHANVIEN = dm_NhanVien.NAME;
                    lstProduct.Add(newv_ct_PhieuGiaoHang_NhanVienGiao);
                }

                List<ValueEdit> lst = new List<ValueEdit>();
                Session[Sessions.lstDelivery_Shipper] = lstProduct;

                apiResponse.ProductCombo = Utility.GetDelivery_Shipper(lstProduct);
                lst.Add(new ValueEdit { Key = "lstct_PhieuGiaoHang_NhanVienGiaoEdit", Value = apiResponse.ProductCombo });
                lst.Add(new ValueEdit { Key = "lstct_PhieuGiaoHang_NhanVienGiao", Value = apiResponse.ProductCombo });
                apiResponse.Detail = lst;
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

        [HttpGet]
        public ActionResult DeleteDeliveryDetail(string ID)
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                if (Utility.KiemTra())
                {
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Admin");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                var lstProduct = Utility.LstPhieuGiaoHang_ChiTiet;
                var ChiTiet = lstProduct.FirstOrDefault(e => e.ID_PHIEUXUAT == ID);
                if (ChiTiet != null)
                {
                    lstProduct.Remove(ChiTiet);
                }
                Session[Sessions.lstDelivery_Detail] = lstProduct;

                List<ValueEdit> lst = new List<ValueEdit>();
                apiResponse.ProductCombo = Utility.GetDelivery_Detail(lstProduct);
                lst.Add(new ValueEdit { Key = "lstct_PhieuGiaoHang_ChiTietEdit", Value = apiResponse.ProductCombo });
                lst.Add(new ValueEdit { Key = "lstct_PhieuGiaoHang_ChiTiet", Value = apiResponse.ProductCombo });
                apiResponse.Success = true;
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

        [HttpGet]
        public ActionResult DeleteDeliveryShipper(string ID)
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                if (Utility.KiemTra())
                {
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Admin");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                var lstProduct = Utility.LstPhieuGiaoHang_NhanVienGiao;
                var ChiTiet = lstProduct.FirstOrDefault(e => e.ID_NHANVIENGIAO == ID);
                if (ChiTiet != null)
                {
                    lstProduct.Remove(ChiTiet);
                }
                Session[Sessions.lstDelivery_Shipper] = lstProduct;
                List<ValueEdit> lst = new List<ValueEdit>();
                apiResponse.ProductCombo = Utility.GetDelivery_Shipper(lstProduct);
                lst.Add(new ValueEdit { Key = "lstct_PhieuGiaoHang_NhanVienGiaoEdit", Value = apiResponse.ProductCombo });
                lst.Add(new ValueEdit { Key = "lstct_PhieuGiaoHang_NhanVienGiao", Value = apiResponse.ProductCombo });
                apiResponse.Detail = lst;
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

        public ActionResult Search(DateTime? FromDate = null, DateTime? ToDate = null, string SearchString = "", string ID_KHUVUC = "")
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                if (Utility.KiemTra())
                {
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Admin");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                if (!Utility.KiemTraQuyen(API.ct_PhieuXuat, API.Xem))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    apiResponse.Success = false;
                    apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                //ShowSearchValue = Utility.GetShowSearchValue<ct_PhieuXuat>(ShowSearchValue);

                List<v_ct_PhieuXuat> lstpage = new List<v_ct_PhieuXuat>();
                string TrField = "";
                string BodyField = "";
                if (FromDate != null)
                {
                    apiResponse = Utility.Get_DanhSachPhieuXuat_TimKiem<v_ct_PhieuXuat>("", FromDate, ToDate, SearchString, "", ID_KHUVUC);
                    if (!apiResponse.Success)
                    {
                        TempData["TitleError"] = apiResponse.Message;
                        return RedirectToAction("Index", "Notfound");
                    }

                    if (apiResponse.Data != null)
                    {
                        var Login_Model = (Login_Model)Session[Sessions.Login_Model];
                        lstpage = (apiResponse.Data as List<v_ct_PhieuXuat>).Where(s => s.ISHOANTAT == false && !string.IsNullOrEmpty(s.ID_KHACHHANG)).OrderByDescending(s => s.NGAYLAP).ToList();
                        IEnumerable<PropertyInfo> props = typeof(v_ct_PhieuXuat).GetRuntimeProperties();

                        List<view_web_NoteClass> lstNoteClass = Utility.GetNoteClass();
                        if (lstNoteClass != null)
                            lstNoteClass = lstNoteClass.Where(s => !string.IsNullOrEmpty(s.NAMECLASS) && s.NAMECLASS.ToLower() == typeof(v_ct_PhieuXuat).Name.Replace("v_", "").ToLower() && s.ISSEARCH).ToList();

                        if (lstNoteClass != null && lstNoteClass.Count > 0)
                        {
                            TrField += "<th style=\"font-weight: bold; text-align:center; white-space: nowrap;\">";
                            TrField += "<input type=\"checkbox\" onchange=\"OnchangeCheckbox(event, 'tbodySearchDelivery')\" />";
                            TrField += "</th>";
                            foreach (var itmSearch in lstNoteClass.OrderBy(s => s.STT))
                            {
                                TrField += "<th style=\"font-weight: bold; text-align:center; white-space: nowrap;\"> " + itmSearch.DISPLAYNAME + "</th>";
                            }

                            foreach (var itm in lstpage)
                            {
                                BodyField += "<tr id=\"" + itm.ID + "\">";
                                BodyField += "<td style=\"white-space: nowrap; \" id=\"" + itm.ID + "\"><input type=\"checkbox\" id=\"" + itm.ID + "\" name=\"TBL_ITEM\" onchange=\"checkboxChanged()\" class=\"cbx\"></td>";
                                foreach (var itmSearch in lstNoteClass.OrderBy(s => s.STT))
                                {
                                    PropertyInfo prop = props.Where(e => e.Name.ToUpper() == (string.IsNullOrEmpty(itmSearch.REPLACESEARCH) ? itmSearch.NAMECOLUMN : itmSearch.REPLACESEARCH).ToUpper()).FirstOrDefault();
                                    if (prop != null)
                                    {
                                        object val = prop.GetValue(itm);
                                        if (val != null && val.GetType().ToString().Contains("Date"))
                                            BodyField += "<td style=\"white-space: nowrap; \" id=\"" + prop.Name + "\">" + (object)(((DateTime)val).ToString("dd/MM/yyyy")) + "</td></a>";
                                        else if (val != null && val.GetType().ToString().Contains("Bool"))
                                            BodyField += "<td style=\"white-space: nowrap; \" id=\"" + prop.Name + "\"><input " + ((Boolean)val == true ? "checked=\"checked\"" : "") + " class=\"check-box\" disabled=\"disabled\" type=\"checkbox\"></td>";
                                        else if (val != null && Utility.IsNumericType(val.GetType()))
                                        {
                                            Decimal dec = Convert.ToDecimal(val);
                                            BodyField += "<td style=\"white-space: nowrap; \" id=\"" + prop.Name + "\">" + dec.ToString("N0") + "</td>";
                                        }
                                        else
                                        {
                                            BodyField += "<td style=\"white-space: nowrap; \" id=\"" + prop.Name + "\">" + val + "</td>";
                                        }
                                    }
                                    else
                                    {
                                        BodyField += "<td></td>";
                                    }
                                }
                                BodyField += "</tr>";
                            }
                        }
                    }
                }

                List<ValueEdit> lst = new List<ValueEdit>();
                lst.Add(new ValueEdit { Key = "tbodySearchDelivery", Value = BodyField });
                lst.Add(new ValueEdit { Key = "trSearchDelivery", Value = TrField });

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
        #endregion

        #region CheckData
        public ActionResult CheckData(string ID = "")
        {
            try
            {
                if (Utility.KiemTra())
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (!Utility.KiemTraQuyen(API.ct_PhieuGiaoHang, API.Xem))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                ApiResponse apiResponse = new ApiResponse();

                v_ct_PhieuGiaoHang ct_PhieuGiaoHang = new v_ct_PhieuGiaoHang();
                if (!string.IsNullOrEmpty(ID))
                {
                    apiResponse = Utility.GetDetail<v_v_ct_PhieuGiaoHang>(Utility.LOC_ID + "/" + ID, API.ct_PhieuGiaoHang);
                    if (!apiResponse.Success)
                    {
                        TempData["TitleError"] = apiResponse.Message;
                        apiResponse.Success = false;
                        apiResponse.URL = Url.Action("Index", "Notfound");
                        return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                    }
                    if (apiResponse.Data != null)
                        ct_PhieuGiaoHang = apiResponse.Data as v_ct_PhieuGiaoHang;
                }

                ViewBag.PermissionEdit = Utility.KiemTraQuyen(API.ct_PhieuGiaoHang, API.Edit);
                ViewBag.PermissionDelete = Utility.KiemTraQuyen(API.ct_PhieuGiaoHang, API.Delete);
                ViewBag.PermissionCreate = Utility.KiemTraQuyen(API.ct_PhieuChi, API.Create);

                ViewBag.PermissionDelivery_CreateReceipt = Utility.KiemTraQuyen(API.ct_PhieuGiaoHang, API.Delivery_CreateReceipt);
                ViewBag.PermissionDelivery_CreateReturn = Utility.KiemTraQuyen(API.ct_PhieuGiaoHang, API.Delivery_CreateReturn);
                var lstdm_LoaiPhieuThu = Utility.GetListData<v_dm_LoaiPhieuThu>(API.dm_LoaiPhieuThu, "", "", Utility.LOC_ID).Data as List<v_dm_LoaiPhieuThu>;
                var LoaiPhieuThu = lstdm_LoaiPhieuThu.FirstOrDefault(e => e.MA == API.PTKH);

                var lstdm_LoaiPhieuNhap = Utility.GetListData<v_dm_LoaiPhieuNhap>(API.dm_LoaiPhieuNhap, "", "", Utility.LOC_ID).Data as List<v_dm_LoaiPhieuNhap>;
                var LoaiPhieuNhap = lstdm_LoaiPhieuNhap.FirstOrDefault(e => e.MA == API.NTHKH);

                ct_PhieuGiaoHang.lstdm_LoaiPhieuChi = new List<v_dm_LoaiPhieuChi>();
                var lstdm_LoaiPhieuChi = Utility.GetListData<v_dm_LoaiPhieuChi>(API.dm_LoaiPhieuChi, "", "", Utility.LOC_ID).Data as List<v_dm_LoaiPhieuChi>;
                if (lstdm_LoaiPhieuChi != null)
                {
                    ct_PhieuGiaoHang.lstdm_LoaiPhieuChi = lstdm_LoaiPhieuChi.Where(e => e.ISACTIVE == true && (e.TYPE == 3 || e.TYPE == 4)).OrderBy(e => e.TYPE).ToList();
                }
                else
                {
                    ct_PhieuGiaoHang.lstdm_LoaiPhieuChi = new List<v_dm_LoaiPhieuChi>();
                }
                var LoaiPhieuChi = ct_PhieuGiaoHang.lstdm_LoaiPhieuChi.FirstOrDefault(e => e.MA == API.PCGCNKHCNV);
                ct_PhieuGiaoHang.lstdm_LoaiPhieuThu = new List<v_dm_LoaiPhieuThu>();
                if (lstdm_LoaiPhieuThu != null)
                {
                    ct_PhieuGiaoHang.lstdm_LoaiPhieuThu = lstdm_LoaiPhieuThu.Where(e => e.ISACTIVE == true && (e.TYPE == 2 || e.TYPE == 3)).OrderBy(e => e.TYPE).ToList();
                }
                else
                {
                    ct_PhieuGiaoHang.lstdm_LoaiPhieuThu = new List<v_dm_LoaiPhieuThu>();
                }
                ViewBag.ID_LOAIPHIEUTHU = LoaiPhieuThu != null ? LoaiPhieuThu.ID : "";
                ViewBag.ID_LOAIPHIEUNHAP = LoaiPhieuNhap != null ? LoaiPhieuNhap.ID : "";
                ViewBag.ID_LOAIPHIEUCHI = LoaiPhieuChi != null ? LoaiPhieuChi.ID : "";
                ct_PhieuGiaoHang.SOLAN = ct_PhieuGiaoHang.lstct_PhieuGiaoHang_ChiTiet.Max(s => s.SOLAN);
                return View(ct_PhieuGiaoHang);
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
                TempData["TitleError"] = API.TitleTryCatch;
                TempData["DetailError"] = ex.Message;
                return RedirectToAction("Index", "Notfound");
            }
        }

        public ActionResult Completed_Detail(string ID = "", string TRANGTHAI = "")
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                if (Utility.KiemTra())
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (!Utility.KiemTraQuyen(API.ct_PhieuGiaoHang, API.Xem))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }

                v_ct_PhieuGiaoHang ct_PhieuGiaoHang = new v_ct_PhieuGiaoHang();
                v_ct_PhieuGiaoHang_ChiTiet ct_PhieuGiaoHang_ChiTiet = new v_ct_PhieuGiaoHang_ChiTiet();
                if (!string.IsNullOrEmpty(ID))
                {
                    apiResponse = Utility.GetDetail<v_ct_PhieuGiaoHang_ChiTiet>(Utility.LOC_ID + "/" + ID, API.ct_PhieuGiaoHang_ChiTiet);
                    if (!apiResponse.Success)
                    {
                        TempData["TitleError"] = apiResponse.Message;
                        apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                        return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                    }
                    if (apiResponse.Data != null)
                        ct_PhieuGiaoHang_ChiTiet = apiResponse.Data as v_ct_PhieuGiaoHang_ChiTiet;

                    if (TRANGTHAI != null && TRANGTHAI.Contains("1"))
                        ct_PhieuGiaoHang_ChiTiet.ISDAGIAOHANG = true;
                    else
                        ct_PhieuGiaoHang_ChiTiet.ISDAGIAOHANG = false;

                    apiResponse = Utility.Edit<v_ct_PhieuGiaoHang_ChiTiet>(Utility.LOC_ID + "/" + ID, ct_PhieuGiaoHang_ChiTiet, API.ct_PhieuGiaoHang_ChiTiet);
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

        public ActionResult Completed(string ID = "", string TRANGTHAI = "")
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                if (Utility.KiemTra())
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (!Utility.KiemTraQuyen(API.ct_PhieuGiaoHang, API.Xem))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }

                v_ct_PhieuGiaoHang ct_PhieuGiaoHang = new v_ct_PhieuGiaoHang();
                if (!string.IsNullOrEmpty(ID))
                {
                    apiResponse = Utility.GetDetail<v_ct_PhieuGiaoHang>(Utility.LOC_ID + "/" + ID, API.ct_PhieuGiaoHang);
                    if (!apiResponse.Success)
                    {
                        TempData["TitleError"] = apiResponse.Message;
                        apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                        return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                    }
                    if (apiResponse.Data != null)
                        ct_PhieuGiaoHang = apiResponse.Data as v_ct_PhieuGiaoHang;


                    if (TRANGTHAI != null && TRANGTHAI.Contains("1"))
                    {
                        foreach (var itm in ct_PhieuGiaoHang.lstct_PhieuGiaoHang_ChiTiet)
                        {
                            if (!itm.ISDAGIAOHANG)
                            {
                                apiResponse.Success = false;
                                apiResponse.Message = "Chưa giao hàng phiếu xuất " + itm.MAPHIEUXUAT + "!";

                            }
                        }
                        ct_PhieuGiaoHang.ISHOANTAT = true;
                    }
                    else
                        ct_PhieuGiaoHang.ISHOANTAT = false;

                    apiResponse = Utility.Edit<v_ct_PhieuGiaoHang>(Utility.LOC_ID + "/" + ID, ct_PhieuGiaoHang, API.ct_PhieuGiaoHang);
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
        #endregion

        #region ViewReport
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
                v_ct_PhieuGiaoHang PhieuNhap = new v_ct_PhieuGiaoHang();
                SP_Parameter objParameter = new SP_Parameter();
                objParameter.LOC_ID = Utility.LOC_ID;
                objParameter.ID_PHIEUGIAOHANG = ID;
                apiResponse = Utility.ExecuteStoredProc<v_ct_PhieuGiaoHang>(objParameter, API.Sp_Get_DanhSachPhieuGiaoHang);
                if (!apiResponse.Success)
                {
                    apiResponse.Success = false;
                    apiResponse.Message = apiResponse.Message;
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                if (apiResponse.Data != null)
                    PhieuNhap = (apiResponse.Data as List<v_ct_PhieuGiaoHang>).FirstOrDefault();

                SP_Parameter_Report objParameter_Report = new SP_Parameter_Report();
                objParameter_Report.NAME_SP = API.Sp_Get_DanhSachPhieuGiaoHang_PhieuXuat;
                objParameter_Report.LOC_ID = Utility.LOC_ID;
                objParameter_Report.ID_PHIEUGIAOHANG = ID;
                var report = new ReportClass();
                apiResponse = Utility.ExecuteStoredProc<DataTable>(objParameter_Report, API.SP_GetReport);
                if (!apiResponse.Success)
                {
                    apiResponse.Success = false;
                    apiResponse.Message = apiResponse.Message;
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }

                //byte[] BinaryData = System.Text.Encoding.UTF8.GetBytes("https://ironsoftware.com/csharp/barcode/");
                // WRITE QR with Binary Content
                //String fullpath = Path.Combine(Server.MapPath("~" + API.PathProduct), "MyBinaryQR.png");
                String fullpathLogo = Path.Combine(Server.MapPath("~" + API.PathLogo), "logoTrangHiepPhat.jpg");
                //QRCodeLogo qrCodeLogo = new QRCodeLogo(fullpathLogo);
                //GeneratedBarcode MyVerifiedQR = QRCodeWriter.CreateQrCodeWithLogo(BinaryData, qrCodeLogo, 500);
                //MyVerifiedQR.ResizeTo(500, 500).SetMargins(10).ChangeBarCodeColor(Color.DarkGreen);
                //MyVerifiedQR.SaveAsImage(fullpath);
                string linkdata = Utility.UrlWebsite + "/Delivery/CheckData?ID=" + (PhieuNhap != null ? PhieuNhap.ID : "");
                QRCoder.QRCodeGenerator qrGenerator = new QRCoder.QRCodeGenerator();
                Bitmap borderedLogo = new Bitmap(fullpathLogo);
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(linkdata, QRCoder.QRCodeGenerator.ECCLevel.Q);
                QRCode qrCode = new QRCode(qrCodeData);

                Bitmap qrCodeImage = qrCode.GetGraphic(9, Color.Black, Color.White, borderedLogo);

                // Save the QR code as a PNG image file inside the specified folder
                String fullpath = Path.Combine(Server.MapPath("~" + API.PathProduct), "MyBinaryQR.png");
                //string fileName = Path.Combine(folderPath, linkdata + "_" + "QRCode.png");
                qrCodeImage.Save(fullpath, System.Drawing.Imaging.ImageFormat.Png);

                // Display the QR code image using an image viewer application
                DisplayQRCodeImage(fullpath);

                DataTable data = (apiResponse.Data as DataTable);
                if (apiResponse.CheckValue)
                    data.Rows.Clear();

                if (data.Columns.Contains("QR_CODE"))
                {
                    foreach (DataRow dr in data.Rows)
                    {
                        dr["QR_CODE"] = Utility.UrlWebsite + "/Output/Edit?ID=" + dr["ID"];
                    }
                }
                if (PhieuNhap != null)
                    report = Utility.GetFormulaFields(report, PhieuNhap);
                report.DataDefinition.FormulaFields["QRCode"].Text = "'" + fullpath + "'";
                report.SetDataSource(data);
                Response.Buffer = false;
                Response.ClearContent();
                Response.ClearHeaders();

                //PrinterSettings getprinterName = new PrinterSettings();
                //report.PrintOptions.PrinterName = getprinterName.PrinterName;
                //report.PrintToPrinter(1, true, 1, 1);

                //((CrystalDecisions.CrystalReports.Engine.TextObject)report.ReportDefinition.ReportObjects["VerReporte"]).Text = "Changed Header";
                Stream stream = report.ExportToStream(ExportFormatType.PortableDocFormat);
                //CrystalDecisions.Shared.ExportOptions exportOptions = new CrystalDecisions.Shared.ExportOptions();
                //exportOptions.ExportFormatType = ExportFormatType.PortableDocFormat;
                //HttpResponse response = System.Web.HttpContext.Current.Response;
                //report.ExportToHttpResponse(exportOptions, response, true, "DirectAccessReport.pdf");
                Utility.Report = report;
                apiResponse = new ApiResponse();
                apiResponse.Success = true;
                apiResponse.NAME = Utility.GetTitleFrom(API.ct_PhieuGiaoHang) + " - " + (PhieuNhap != null ? PhieuNhap.MAPHIEU : "");
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

        static void DisplayQRCodeImage(string imagePath)
        {
            try
            {
                // Check if the file exists
                if (System.IO.File.Exists(imagePath))
                {
                    // Use the default image viewer to open and display the QR code image
                    //ProcessStartInfo psi = new ProcessStartInfo
                    //{
                    //    FileName = imagePath,
                    //    UseShellExecute = true
                    //};
                    //Process.Start(psi);
                }
                else
                {
                    Console.WriteLine("QR code image not found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public ActionResult ViewReportType(string ID, string LOAIPHIEUIN, int SOLAN = -1)
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {

                if (Utility.KiemTra())
                {
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Admin");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                v_ct_PhieuGiaoHang PhieuNhap = new v_ct_PhieuGiaoHang();
                SP_Parameter objParameter = new SP_Parameter();
                objParameter.LOC_ID = Utility.LOC_ID;
                objParameter.ID_PHIEUGIAOHANG = ID;
                apiResponse = Utility.ExecuteStoredProc<v_ct_PhieuGiaoHang>(objParameter, API.Sp_Get_DanhSachPhieuGiaoHang);
                if (!apiResponse.Success)
                {
                    apiResponse.Success = false;
                    apiResponse.Message = apiResponse.Message;
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                if (apiResponse.Data != null)
                    PhieuNhap = (apiResponse.Data as List<v_ct_PhieuGiaoHang>).FirstOrDefault();

                SP_Parameter objParameter_Report = new SP_Parameter();
                objParameter_Report.LOC_ID = Utility.LOC_ID;
                objParameter_Report.ID_PHIEUGIAOHANG = ID;
                objParameter_Report.SOLAN = SOLAN;
                var report = new ReportClass();

                apiResponse = Utility.ExecuteStoredProc<Sp_Get_DanhSachPhieuGiaoHang_In>(objParameter_Report, LOAIPHIEUIN == "3" ? API.Sp_Get_DanhSachPhieuGiaoHang_InPhieuGiao : API.Sp_Get_DanhSachPhieuGiaoHang_In);
                if (!apiResponse.Success)
                {
                    apiResponse.Success = false;
                    apiResponse.Message = apiResponse.Message;
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                string PathMap = "~/Report/rptDanhSachPhieuGiaoHang_InGroupType.rpt";
                List<Sp_Get_DanhSachPhieuGiaoHang_In> lstSp_Get_DanhSachPhieuGiaoHang_In = (apiResponse.Data as List<Sp_Get_DanhSachPhieuGiaoHang_In>);
                List<v_PhieuGioaHang_InTheoGroup> lstv_PhieuGioaHang_InTheoGroup = new List<v_PhieuGioaHang_InTheoGroup>();
                //if(LOAIPHIEUIN == "1")
                //{
                //    PathMap = "~/Report/rptDanhSachPhieuGiaoHang_InGroupType.rpt";
                //    lstv_PhieuGioaHang_InTheoGroup = lstSp_Get_DanhSachPhieuGiaoHang_In.GroupBy(s => new { s.NAME_NHOMHANGHOA, s.MA, s.NAME, s.NAME_DVT, s.NAME_DVT_QD, s.TYLE_QD_HH, s.ISKHUYENMAI })
                //    .Select(s => new v_PhieuGioaHang_InTheoGroup
                //    {
                //        NAME_GROUP = s.Key.NAME_NHOMHANGHOA,
                //        MA_HANGHOA = s.Key.MA,
                //        NAME_HANGHOA = (s.Key.ISKHUYENMAI ? "(KM)" : "") + s.Key.NAME,
                //        NAME_DVT = s.Key.NAME_DVT,
                //        NAME_DVT_QD = s.Key.NAME_DVT_QD,
                //        TYLE_QD = s.Key.TYLE_QD_HH,
                //        TONGSOLUONG = s.Sum(x => Math.Round(Convert.ToDecimal(x.SOLUONG * x.TYLE_QD), 0))
                //    }).ToList();
                //}
                int TONGSODONHANG = 0;
                string MAPHIEU = "";
                string NAME_KHUVUC = "";
                if (LOAIPHIEUIN == "1")
                {
                    v_v_ct_PhieuGiaoHang ct_PhieuGiaoHang = new v_v_ct_PhieuGiaoHang();
                    if (!string.IsNullOrEmpty(ID))
                    {
                        apiResponse = Utility.GetDetail<v_v_ct_PhieuGiaoHang>(Utility.LOC_ID + "/" + ID, API.ct_PhieuGiaoHang);
                        if (!apiResponse.Success)
                        {
                            TempData["TitleError"] = apiResponse.Message;
                            return RedirectToAction("Index", "Notfound");
                        }
                        if (apiResponse.Data != null)
                            ct_PhieuGiaoHang = apiResponse.Data as v_v_ct_PhieuGiaoHang;
                    }
                    PathMap = "~/Report/rptBaoCaoPhieuDatHang.rpt";
                    SP_Parameter_Report objParameter_BC = new SP_Parameter_Report();
                    objParameter_BC.ID_PHIEUGIAOHANG = ID;
                    objParameter_BC.SOLAN = SOLAN;
                    apiResponse = Utility.ExecuteStoredProcT<v_ct_PhieuDatHang_ChiTiet_BaoCao>(objParameter_BC, API.Sp_Get_DanhSachPhieuGiaoHang_ChiTiet_BaoCao);
                    List<v_ct_PhieuDatHang_ChiTiet_BaoCao> lstSp_Get_DanhSachPhieuGiaoHang_In_BC = (apiResponse.Data as List<v_ct_PhieuDatHang_ChiTiet_BaoCao>);
                    if (!apiResponse.Success)
                    {
                        apiResponse.Success = false;
                        apiResponse.Message = apiResponse.Message;
                        return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                    }

                    if (lstSp_Get_DanhSachPhieuGiaoHang_In_BC != null)
                    {
                        MAPHIEU = ct_PhieuGiaoHang.MAPHIEU;
                        TONGSODONHANG = lstSp_Get_DanhSachPhieuGiaoHang_In_BC.GroupBy(s => new { s.MAPHIEU }).Count();
                        NAME_KHUVUC = string.Join(";", lstSp_Get_DanhSachPhieuGiaoHang_In_BC.GroupBy(s => new { s.NAME_KHUVUC }).Select(s => s.Key.NAME_KHUVUC));
                        lstv_PhieuGioaHang_InTheoGroup = lstSp_Get_DanhSachPhieuGiaoHang_In_BC.GroupBy(s => new { s.TYLE_QD, s.TRONGLUONG, s.NAME, s.MA, s.NAME_DVT, s.NAME_DVT_QD, s.NAME_NHOMHANGHOA })
                                                .Select(s => new v_PhieuGioaHang_InTheoGroup
                                                {
                                                    MAPHIEUXUAT = s.Key.NAME_NHOMHANGHOA,
                                                    MA_HANGHOA = s.Key.MA,
                                                    NAME_HANGHOA = s.Key.NAME,
                                                    NAME_DVT = s.Key.NAME_DVT,
                                                    CHIETKHAU = s.Sum(x => Math.Round(x.CHIETKHAU, 0)),
                                                    TONGTIENGIAMGIA = s.Sum(x => Math.Round(x.TONGTIENGIAMGIA, 0)),
                                                    THANHTIEN = s.Sum(x => Math.Round(x.THANHTIEN, 0)),
                                                    THUESUAT = s.Sum(x => Math.Round(x.THUESUAT, 0)),
                                                    TONGTIENVAT = s.Sum(x => Math.Round(x.TONGTIENVAT, 0)),
                                                    TONGCONG = s.Sum(x => Math.Round(x.TONGCONG, 0)),
                                                    TONGSOLUONG = Convert.ToDecimal(s.Sum(x => Math.Round(x.TONGSOLUONG, 0))),
                                                    TONGTRONGLUONG = Convert.ToDecimal(s.Key.TRONGLUONG * s.Sum(x => Math.Round(x.TONGSOLUONG, 0))),
                                                    NAME_DVT_QD = s.Key.NAME_DVT_QD,
                                                    TYLE_QD = s.Key.TYLE_QD
                                                }).ToList();
                    }


                }

                if (LOAIPHIEUIN == "4")
                {
                    v_v_ct_PhieuGiaoHang ct_PhieuGiaoHang = new v_v_ct_PhieuGiaoHang();
                    if (!string.IsNullOrEmpty(ID))
                    {
                        apiResponse = Utility.GetDetail<v_v_ct_PhieuGiaoHang>(Utility.LOC_ID + "/" + ID, API.ct_PhieuGiaoHang);
                        if (!apiResponse.Success)
                        {
                            TempData["TitleError"] = apiResponse.Message;
                            return RedirectToAction("Index", "Notfound");
                        }
                        if (apiResponse.Data != null)
                            ct_PhieuGiaoHang = apiResponse.Data as v_v_ct_PhieuGiaoHang;
                    }
                    PathMap = "~/Report/rptBaoCaoPhieuDatHang.rpt";
                    SP_Parameter_Report objParameter_BC = new SP_Parameter_Report();
                    objParameter_BC.ID_PHIEUGIAOHANG = ID;
                    objParameter_BC.SOLAN = SOLAN;
                    apiResponse = Utility.ExecuteStoredProcT<v_ct_PhieuDatHang_ChiTiet_BaoCao>(objParameter_BC, API.Sp_Get_DanhSachPhieuGiaoHang_ChiTiet_BaoCao);
                    List<v_ct_PhieuDatHang_ChiTiet_BaoCao> lstSp_Get_DanhSachPhieuGiaoHang_In_BC = (apiResponse.Data as List<v_ct_PhieuDatHang_ChiTiet_BaoCao>);
                    if (!apiResponse.Success)
                    {
                        apiResponse.Success = false;
                        apiResponse.Message = apiResponse.Message;
                        return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                    }

                    if (lstSp_Get_DanhSachPhieuGiaoHang_In_BC != null)
                    {
                        MAPHIEU = ct_PhieuGiaoHang.MAPHIEU;
                        TONGSODONHANG = lstSp_Get_DanhSachPhieuGiaoHang_In_BC.GroupBy(s => new { s.MAPHIEU }).Count();
                        lstv_PhieuGioaHang_InTheoGroup = lstSp_Get_DanhSachPhieuGiaoHang_In_BC.GroupBy(s => new { s.NAME_KHUVUC, s.TYLE_QD, s.TRONGLUONG, s.NAME, s.MA, s.NAME_DVT, s.NAME_DVT_QD, s.NAME_NHOMHANGHOA })
                                                .Select(s => new v_PhieuGioaHang_InTheoGroup
                                                {
                                                    ID_KHACHHANG = "",
                                                    NAME_GROUP = s.Key.NAME_KHUVUC,
                                                    MAPHIEUXUAT = s.Key.NAME_NHOMHANGHOA,
                                                    MA_HANGHOA = s.Key.MA,
                                                    NAME_HANGHOA = s.Key.NAME,
                                                    NAME_DVT = s.Key.NAME_DVT,
                                                    CHIETKHAU = s.Sum(x => Math.Round(x.CHIETKHAU, 0)),
                                                    TONGTIENGIAMGIA = s.Sum(x => Math.Round(x.TONGTIENGIAMGIA, 0)),
                                                    THANHTIEN = s.Sum(x => Math.Round(x.THANHTIEN, 0)),
                                                    THUESUAT = s.Sum(x => Math.Round(x.THUESUAT, 0)),
                                                    TONGTIENVAT = s.Sum(x => Math.Round(x.TONGTIENVAT, 0)),
                                                    TONGCONG = s.Sum(x => Math.Round(x.TONGCONG, 0)),
                                                    TONGSOLUONG = Convert.ToDecimal(s.Sum(x => Math.Round(x.TONGSOLUONG, 0))),
                                                    TONGTRONGLUONG = Convert.ToDecimal(s.Key.TRONGLUONG * s.Sum(x => Math.Round(x.TONGSOLUONG, 0))),
                                                    NAME_DVT_QD = s.Key.NAME_DVT_QD,
                                                    TYLE_QD = s.Key.TYLE_QD
                                                }).ToList();
                    }


                }

                if (LOAIPHIEUIN == "2")
                {
                    PathMap = "~/Report/rptDanhSachPhieuGiaoHang_InGroupType.rpt";
                    lstv_PhieuGioaHang_InTheoGroup = lstSp_Get_DanhSachPhieuGiaoHang_In.GroupBy(s => new { s.NAME_NCC, s.MA, s.NAME, s.NAME_DVT, s.NAME_DVT_QD, s.TYLE_QD_HH, s.ISKHUYENMAI })
                    .Select(s => new v_PhieuGioaHang_InTheoGroup
                    {
                        ID_KHACHHANG = "",
                        NAME_GROUP = s.Key.NAME_NCC,
                        MA_HANGHOA = s.Key.MA,
                        NAME_HANGHOA = (s.Key.ISKHUYENMAI ? "(KM)" : "") + s.Key.NAME,
                        NAME_DVT = s.Key.NAME_DVT,
                        NAME_DVT_QD = s.Key.NAME_DVT_QD,
                        TYLE_QD = s.Key.TYLE_QD_HH,
                        TONGSOLUONG = Convert.ToDecimal(s.Sum(x => Math.Round(x.TONGSOLUONG, 0)))
                    }).ToList();
                }

                if (LOAIPHIEUIN == "3")
                {
                    PathMap = "~/Report/rptDanhSachPhieuGiaoHang_InGroupBy.rpt";
                    lstv_PhieuGioaHang_InTheoGroup = lstSp_Get_DanhSachPhieuGiaoHang_In
                    .Select(s => new v_PhieuGioaHang_InTheoGroup
                    {
                        ID_KHACHHANG = s.ID_KHACHHANG_NCC,
                        NAME_GROUP = "Họ tên khách hàng: " + s.NAME_KHACHHANG_NCC + (string.IsNullOrEmpty(s.TEL_KHACHHANG_NCC) ? "" : Environment.NewLine + "Điện thoại: ") + s.TEL_KHACHHANG_NCC + (string.IsNullOrEmpty(s.DIACHI_KHACHHANG_NCC) ? "" : Environment.NewLine + "Địa chỉ: ") + s.DIACHI_KHACHHANG_NCC,
                        MAPHIEU_GROUP = String.Join(",", lstSp_Get_DanhSachPhieuGiaoHang_In.Where(x => x.ID_KHACHHANG_NCC == s.ID_KHACHHANG_NCC).GroupBy(x => new { x.MAPHIEU }).Select(x => x.Key.MAPHIEU).ToList()),
                        MAPHIEUXUAT = (lstSp_Get_DanhSachPhieuGiaoHang_In.Where(x => x.ID_KHACHHANG_NCC == s.ID_KHACHHANG_NCC).GroupBy(x => new { x.MAPHIEU }).Count() > 1 ? s.MAPHIEU : ""),
                        //MA_HANGHOA = s.MA,
                        NAME_HANGHOA = (s.ISKHUYENMAI ? "(KM)" : "") + s.NAME,
                        NAME_DVT = s.NAME_DVT,
                        SOLUONG = s.SOLUONG,
                        DONGIA = s.DONGIA,
                        CHIETKHAU = s.CHIETKHAU,
                        TONGTIENGIAMGIA = (s.TONGTIENGIAMGIA > 0 ? s.TONGTIENGIAMGIA : 0),
                        THANHTIEN = s.THANHTIEN,
                        THUESUAT = s.THUESUAT,
                        TONGTIENVAT = (s.TONGTIENGIAMGIA < 0 ? s.TONGTIENVAT - s.TONGTIENGIAMGIA : s.TONGTIENVAT),
                        TONGCONG = s.TONGCONG,
                        TONGSOLUONG = 0,
                        TYLE_QD = 1,
                    }).ToList();

                    var lst = lstv_PhieuGioaHang_InTheoGroup.GroupBy(s => new { s.ID_KHACHHANG }).Select(s => s.Key).ToList();
                    foreach (var itm in lst)
                    {
                        SP_Parameter sp_Parameter = new SP_Parameter();
                        //ApiResponse apiResponse = new ApiResponse();
                        sp_Parameter.LOC_ID = Utility.LOC_ID;
                        sp_Parameter.ID_KHACHHANG = itm.ID_KHACHHANG.ToString();
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
                            double CongNoMoi = lstv_PhieuGioaHang_InTheoGroup.Where(e => e.ID_KHACHHANG == sp_Parameter.ID_KHACHHANG).Sum(e => e.TONGCONG);
                            var CongNo = (apiResponse.Data as List<v_ThongKeCongNoKhachHang>).FirstOrDefault();
                            if (CongNo != null && CongNo.TONGTIENCONGNOCUOIKY - CongNoMoi > 0)
                            {
                                foreach (var GoiHang in lstv_PhieuGioaHang_InTheoGroup.Where(e => e.ID_KHACHHANG == sp_Parameter.ID_KHACHHANG))
                                {
                                    GoiHang.NAME_DVT_QD = "Nợ cũ: " + (CongNo.TONGTIENCONGNOCUOIKY - CongNoMoi).ToString("N0");
                                    GoiHang.MA_HANGHOA = "Tổng tiền: " + (CongNo.TONGTIENCONGNOCUOIKY).ToString("N0");
                                }
                            }
                        }
                    }
                }
                
                DataTable data = Utility.ToDataTable<v_PhieuGioaHang_InTheoGroup>(lstv_PhieuGioaHang_InTheoGroup);
                report = Utility.GetFormulaFields(report, PhieuNhap, PathMap);
                if (LOAIPHIEUIN == "1")
                {
                    v_v_ct_PhieuGiaoHang ct_PhieuGiaoHang = new v_v_ct_PhieuGiaoHang();
                    if (!string.IsNullOrEmpty(ID))
                    {
                        apiResponse = Utility.GetDetail<v_v_ct_PhieuGiaoHang>(Utility.LOC_ID + "/" + ID, API.ct_PhieuGiaoHang);
                        if (!apiResponse.Success)
                        {
                            TempData["TitleError"] = apiResponse.Message;
                            return RedirectToAction("Index", "Notfound");
                        }
                        if (apiResponse.Data != null)
                            ct_PhieuGiaoHang = apiResponse.Data as v_v_ct_PhieuGiaoHang;
                    }
                    report.DataDefinition.FormulaFields["TIEUDE"].Text = "'" + "BÁO CÁO GIAO HÀNG THEO NHÓM HÀNG" + "'";
                    report.DataDefinition.FormulaFields["TONGCONG"].Text = "'" + lstv_PhieuGioaHang_InTheoGroup.Sum(s => s.TONGCONG).ToString("N0") + "'";
                    report.DataDefinition.FormulaFields["TONGTRONGLUONG"].Text = "'" + lstv_PhieuGioaHang_InTheoGroup.Sum(s => s.TONGTRONGLUONG/1000).ToString("N0") + "'";
                    report.DataDefinition.FormulaFields["TONGSODONHANG"].Text = "'" + TONGSODONHANG.ToString("N0") + "'";
                    //if (objParameter.ID_KHUVUC != null && lstv_PhieuGioaHang_InTheoGroup.Count > 0)
                    //    report.DataDefinition.FormulaFields["MAPHIEU"].Text = "'" + lstv_PhieuGioaHang_InTheoGroup.FirstOrDefault().NAME_GROUP + "'";
                    //else
                    //    report.DataDefinition.FormulaFields["MAPHIEU"].Text = "'" + "Tất cả khu vực" + "'";
                    report.DataDefinition.FormulaFields["MAPHIEU"].Text = "'" + MAPHIEU+ "'";
                    report.DataDefinition.FormulaFields["KHUVUC"].Text = "'" + "Khu vực: " + NAME_KHUVUC + "'";
                }
                if(LOAIPHIEUIN == "3")
                {
                    v_v_ct_PhieuGiaoHang ct_PhieuGiaoHang = new v_v_ct_PhieuGiaoHang();
                    if (!string.IsNullOrEmpty(ID))
                    {
                        apiResponse = Utility.GetDetail<v_v_ct_PhieuGiaoHang>(Utility.LOC_ID + "/" + ID, API.ct_PhieuGiaoHang);
                        if (!apiResponse.Success)
                        {
                            TempData["TitleError"] = apiResponse.Message;
                            return RedirectToAction("Index", "Notfound");
                        }
                        if (apiResponse.Data != null)
                            ct_PhieuGiaoHang = apiResponse.Data as v_v_ct_PhieuGiaoHang;
                    }
                    string THONGTINTHEM = "Nhân viên giao hàng: ";
                    foreach(var itm in ct_PhieuGiaoHang.lstct_PhieuGiaoHang_NhanVienGiao)
                    {
                        THONGTINTHEM += itm.NAME_NHANVIEN + "; ";
                    }
                    
                    report.DataDefinition.FormulaFields["THONGTINTHEM"].Text = "'" + THONGTINTHEM + "'";
                    String fullpath = Path.Combine(Server.MapPath("~" + API.PathLogo), "040937143939.png");
                    report.DataDefinition.FormulaFields["QRCode1"].Text = "'" + fullpath + "'";
                    fullpath = Path.Combine(Server.MapPath("~" + API.PathLogo), "117000052509.png");
                    report.DataDefinition.FormulaFields["QRCode2"].Text = "'" + fullpath + "'";
                }
                report.SetDataSource(data);
                //report.Database.Tables[0].SetDataSource(data);
                //report.Database.Tables[1].SetDataSource(dataTable2);
                Response.Buffer = false;
                Response.ClearContent();
                Response.ClearHeaders();

                //PrinterSettings getprinterName = new PrinterSettings();
                //report.PrintOptions.PrinterName = getprinterName.PrinterName;
                //report.PrintToPrinter(1, true, 1, 1);

                //((CrystalDecisions.CrystalReports.Engine.TextObject)report.ReportDefinition.ReportObjects["VerReporte"]).Text = "Changed Header";
                Stream stream = report.ExportToStream(ExportFormatType.PortableDocFormat);
                //CrystalDecisions.Shared.ExportOptions exportOptions = new CrystalDecisions.Shared.ExportOptions();
                //exportOptions.ExportFormatType = ExportFormatType.PortableDocFormat;
                //HttpResponse response = System.Web.HttpContext.Current.Response;
                //report.ExportToHttpResponse(exportOptions, response, true, "DirectAccessReport.pdf");
                Utility.Report = report;
                apiResponse = new ApiResponse();
                apiResponse.Success = true;
                apiResponse.NAME = Utility.GetTitleFrom(API.ct_PhieuGiaoHang) + " - " + PhieuNhap.MAPHIEU;
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

        public ActionResult GetImageDelivery(string ID = "", string ID_PHIEUXUAT = "")
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                if (Utility.KiemTra())
                {
                    return RedirectToAction("Index", "Admin");
                }

                List<v_ct_PhieuGiaoHang_HinhAnh> lstct_PhieuGiaoHang = new List<v_ct_PhieuGiaoHang_HinhAnh>();
                if (!string.IsNullOrEmpty(ID) || !string.IsNullOrEmpty(ID_PHIEUXUAT))
                {
                    apiResponse = Utility.GetDetail<List<v_ct_PhieuGiaoHang_HinhAnh>>(Utility.LOC_ID + "/" + (!string.IsNullOrEmpty(ID_PHIEUXUAT) ? ID_PHIEUXUAT : ID), API.ct_PhieuGiaoHang_HinhAnh);
                    if (!apiResponse.Success)
                    {
                        TempData["TitleError"] = apiResponse.Message;
                        apiResponse.Success = false;
                        apiResponse.URL = Url.Action("Index", "Notfound");
                        return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                    }
                    if (apiResponse.Data != null)
                        lstct_PhieuGiaoHang = apiResponse.Data as List<v_ct_PhieuGiaoHang_HinhAnh>;

                    foreach (v_ct_PhieuGiaoHang_HinhAnh itm in lstct_PhieuGiaoHang)
                    {
                        var lst = itm.URL_IMAGE.Split('/');
                        string url = Request.Url.Authority;
                        string URL_IMAGE = "";
                        if (Request.Url.AbsoluteUri.StartsWith("https"))
                            URL_IMAGE = "https://" + url + itm.URL_IMAGE;
                        else
                            URL_IMAGE = "http://" + url + itm.URL_IMAGE;
                        apiResponse.CONTENT += "<div class='col-xs-6 col-sm-4 col-md-3 image' id='" + itm.ID + "'>";
                        apiResponse.CONTENT += "<div class='thmb'>";
                        apiResponse.CONTENT += "<div class='ckbox ckbox-default'>";
                        apiResponse.CONTENT += "</div>";
                        apiResponse.CONTENT += "<div class='btn-group fm-group'>";
                        apiResponse.CONTENT += "</div><!-- btn-group -->";
                        apiResponse.CONTENT += "<div class='thmb-prev'>";
                        apiResponse.CONTENT += ("<a href='" + URL_IMAGE + "' data-rel='prettyPhoto'>");
                        apiResponse.CONTENT += ("<img src='" + URL_IMAGE + "' class='img-responsive' alt='' />");
                        apiResponse.CONTENT += "</a>";
                        apiResponse.CONTENT += "</div>";
                        apiResponse.CONTENT += "<h5 class='fm-title'><a href='#'>" + itm.NAME_NGUOITAO + "</a></h5> ";
                        apiResponse.CONTENT += "<small class=\"text-muted\"><a href=\"#\" style=\"color:red\" onclick=\"myFunctionPopupImage('" + API.ct_PhieuGiaoHang_HinhAnh + "','" + itm.ID + "')\"><i class=\"glyphicon glyphicon-trash\" style=\"margin-right:5px\" ></i>" + Utility.Xoa + "\r\n</a></small>";
                        apiResponse.CONTENT += "<small class='text-muted'>" + itm.NGAYTAO.ToString("dd/MM/yyyy HH:mm") + "</small>";
                        apiResponse.CONTENT += "</div><!-- thmb -->";
                        apiResponse.CONTENT += "</div><!-- col-xs-6 -->";
                    }
                    apiResponse.ID = ID;
                    apiResponse.ID_PHIEUXUAT = ID_PHIEUXUAT;
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

