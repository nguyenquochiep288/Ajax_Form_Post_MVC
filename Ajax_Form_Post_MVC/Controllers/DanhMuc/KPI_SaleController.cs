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
using DatabaseTHP.StoredProcedure.Parameter;
using Newtonsoft.Json;
using static DatabaseTHP.Class.API;
using System.Web.Script.Serialization;

namespace MVC_QuanLyTHP.Controllers
{
    public class KPI_SaleController : Controller
    {

        // GET: KPI_Sale
        public ActionResult Index(int Page = 1, string SearchString = "", string ShowSearchValue = "")
        {
            try
            {
                if (Utility.KiemTra())
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (!Utility.KiemTraQuyen(API.dm_KPI_KinhDoanh, API.Xem))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                ShowSearchValue = Utility.GetShowSearchValue<dm_KPI_KinhDoanh>(ShowSearchValue);
                string ShowSearchValue_Temp = ShowSearchValue;
                string SearchString_Temp = SearchString;
                if (clsMaHoa.Decrypt(ShowSearchValue, clsMaHoa.PassMaHoa) == "TUNGAY" || clsMaHoa.Decrypt(ShowSearchValue, clsMaHoa.PassMaHoa) == "DENNGAY")
                {
                    ShowSearchValue = "";
                    SearchString = "";
                }
                    
               
                var apiResponse = Utility.GetListData<v_dm_KPI_KinhDoanh>(API.dm_KPI_KinhDoanh, ShowSearchValue, SearchString, Utility.LOC_ID);
                if (!apiResponse.Success)
                {
                    TempData["TitleError"] = apiResponse.Message;
                    return RedirectToAction("Index", "Notfound");
                }
                List<v_dm_KPI_KinhDoanh> lstv_dm_KPI_KinhDoanh = new List<v_dm_KPI_KinhDoanh>();
                if (clsMaHoa.Decrypt(ShowSearchValue_Temp, clsMaHoa.PassMaHoa) == "TUNGAY" || clsMaHoa.Decrypt(ShowSearchValue_Temp, clsMaHoa.PassMaHoa) == "DENNGAY")
                {
                    if(clsMaHoa.Decrypt(ShowSearchValue_Temp, clsMaHoa.PassMaHoa) == "TUNGAY")
                    {
                        DateTime myDate = DateTime.ParseExact(SearchString_Temp, "dd/MM/yyyy",
                                      System.Globalization.CultureInfo.InvariantCulture);
                        lstv_dm_KPI_KinhDoanh = (apiResponse.Data as List<v_dm_KPI_KinhDoanh>).Where(s => s.TUNGAY == myDate).OrderByDescending(s => s.DENNGAY).ToList();
                    }
                    if (clsMaHoa.Decrypt(ShowSearchValue_Temp, clsMaHoa.PassMaHoa) == "DENNGAY")
                    {
                        DateTime myDate = DateTime.ParseExact(SearchString_Temp, "dd/MM/yyyy",
                                       System.Globalization.CultureInfo.InvariantCulture);
                        lstv_dm_KPI_KinhDoanh = (apiResponse.Data as List<v_dm_KPI_KinhDoanh>).Where(s => s.DENNGAY >= myDate).OrderByDescending(s => s.DENNGAY).ToList();
                    }
                }
                else
                {
                    lstv_dm_KPI_KinhDoanh = (apiResponse.Data as List<v_dm_KPI_KinhDoanh>).OrderByDescending(s => s.DENNGAY).ToList();
                }
                    
                IPagedList<v_dm_KPI_KinhDoanh> lstpage = (lstv_dm_KPI_KinhDoanh).ToPagedList(Page, Utility.GetPageSize());

                v_v_dm_KPI_KinhDoanh dm_KPI_KinhDoanh = new v_v_dm_KPI_KinhDoanh();
                dm_KPI_KinhDoanh.IPagedList = lstpage;
                dm_KPI_KinhDoanh.lstdm_DonViTinh = new List<v_dm_DonViTinh>();
                dm_KPI_KinhDoanh.lstdm_DonViTinh = Utility.GetListData<v_dm_DonViTinh>(API.dm_DonViTinh, "", "", Utility.LOC_ID).Data as List<v_dm_DonViTinh>;


                ViewBag.searchValue = SearchString;
                ViewBag.showsearchValue = ShowSearchValue;

                ViewBag.PermissionEdit = Utility.KiemTraQuyen(API.dm_KPI_KinhDoanh, API.Edit);
                ViewBag.PermissionDelete = Utility.KiemTraQuyen(API.dm_KPI_KinhDoanh, API.Delete);
                ViewBag.PermissionCreate = Utility.KiemTraQuyen(API.dm_KPI_KinhDoanh, API.Create);
                return View(dm_KPI_KinhDoanh);
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
                TempData["TitleError"] = API.TitleTryCatch;
                TempData["DetailError"] = ex.Message;
                return RedirectToAction("Index", "Notfound");
            }
        }

        // GET: Promotion/Create
        public ActionResult Create(int type = 2)
        {
            try
            {
                Session[Sessions.IntWidth] = type;
                if (Utility.KiemTra())
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (!Utility.KiemTraQuyen(API.dm_KPI_KinhDoanh, API.Create))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                v_v_dm_KPI_KinhDoanh dm_KPI_KinhDoanh = new v_v_dm_KPI_KinhDoanh();
                dm_KPI_KinhDoanh.LOC_ID = Utility.LOC_ID;
                dm_KPI_KinhDoanh.ID_NGUOITAO = Session[Sessions.idUser].ToString();
                dm_KPI_KinhDoanh.THOIGIANTHEM = Utility.CurrentTime;

                dm_KPI_KinhDoanh.ID = Guid.NewGuid().ToString();
                dm_KPI_KinhDoanh.lstdm_DonViTinh = new List<v_dm_DonViTinh>();
                dm_KPI_KinhDoanh.lstdm_DonViTinh = Utility.GetListData<v_dm_DonViTinh>(API.dm_DonViTinh, "", "", Utility.LOC_ID).Data as List<v_dm_DonViTinh>;

                return View(dm_KPI_KinhDoanh);
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
                TempData["TitleError"] = API.TitleTryCatch;
                TempData["DetailError"] = ex.Message;
                return RedirectToAction("Index", "Notfound");
            }
        }

        // POST: Promotion/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "LOC_ID,ID,MA,NAME,TUNGAY,DENNGAY,CHIETKHAU,TIENGIAM,IS_YEUCAUCHITIET,TONGTIEN_DATKM,SOLUONG_DATKM,ID_DVT_DATKM,ISACTIVE,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO,ISTINHLUYTUYEN,ISTONGHOADON,CAPDO")] v_dm_KPI_KinhDoanh dm_KPI_KinhDoanh)
        {
            try
            {
                if (Utility.KiemTra())
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (!Utility.KiemTraQuyen(API.dm_KPI_KinhDoanh, API.Create))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                if (ModelState.IsValid)
                {
                    dm_KPI_KinhDoanh.LOC_ID = Utility.LOC_ID;
                    dm_KPI_KinhDoanh.ID_NGUOITAO = Session[Sessions.idUser].ToString();
                    dm_KPI_KinhDoanh.THOIGIANTHEM = Utility.CurrentTime;

                    //@ConvertObjectUnicodeToTCVN3
                    var apiResponse = Utility.Create<dm_KPI_KinhDoanh>(dm_KPI_KinhDoanh, API.dm_KPI_KinhDoanh);
                    if (apiResponse.Success)
                        return RedirectToAction("Index");
                    else
                    {
                        ModelState.AddModelError(string.Empty, apiResponse.Message);
                        if (apiResponse.CheckValue)
                            ViewBag.ID = Guid.NewGuid().ToString();
                    }
                }
                return View(dm_KPI_KinhDoanh);
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
                TempData["TitleError"] = API.TitleTryCatch;
                TempData["DetailError"] = ex.Message;
                return RedirectToAction("Index", "Notfound");
            }
        }

        // GET: Promotion/Edit/5
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
                if (!Utility.KiemTraQuyen(API.dm_KPI_KinhDoanh, API.Edit))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                v_v_dm_KPI_KinhDoanh dm_KPI_KinhDoanh = new v_v_dm_KPI_KinhDoanh();
                if (!string.IsNullOrEmpty(id))
                {
                    apiResponse = Utility.GetDetail<v_dm_KPI_KinhDoanh>(Utility.LOC_ID + "/" + id, API.dm_KPI_KinhDoanh);
                    if (!apiResponse.Success)
                    {
                        TempData["TitleError"] = apiResponse.Message;
                        return RedirectToAction("Index", "Notfound");
                    }
                    if (apiResponse.Data != null)
                        dm_KPI_KinhDoanh = apiResponse.Data as v_v_dm_KPI_KinhDoanh;
                }
                //@ConvertObjectTCVN3ToUnicode
                dm_KPI_KinhDoanh.lstdm_DonViTinh = new List<v_dm_DonViTinh>();
                dm_KPI_KinhDoanh.lstdm_DonViTinh = Utility.GetListData<v_dm_DonViTinh>(API.dm_DonViTinh, "", "", Utility.LOC_ID).Data as List<v_dm_DonViTinh>;

                return View(dm_KPI_KinhDoanh);
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
                TempData["TitleError"] = API.TitleTryCatch;
                TempData["DetailError"] = ex.Message;
                return RedirectToAction("Index", "Notfound");
            }
        }

        // POST: Promotion/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "LOC_ID,ID,MA,NAME,TUNGAY,DENNGAY,CHIETKHAU,TIENGIAM,IS_YEUCAUCHITIET,TONGTIEN_DATKM,SOLUONG_DATKM,ID_DVT_DATKM,ISACTIVE,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO,ISTINHLUYTUYEN,ISTONGHOADON,CAPDO")] v_dm_KPI_KinhDoanh dm_KPI_KinhDoanh)
        {
            try
            {
                if (Utility.KiemTra())
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (!Utility.KiemTraQuyen(API.dm_KPI_KinhDoanh, API.Edit))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                if (ModelState.IsValid)
                {
                    dm_KPI_KinhDoanh.LOC_ID = Utility.LOC_ID;
                    dm_KPI_KinhDoanh.ID_NGUOISUA = Session[Sessions.idUser].ToString();
                    dm_KPI_KinhDoanh.THOIGIANSUA = Utility.CurrentTime;
                    //@ConvertObjectUnicodeToTCVN3
                    var apiResponse = Utility.Edit<v_dm_KPI_KinhDoanh>(Utility.LOC_ID + "/" + dm_KPI_KinhDoanh.MA, dm_KPI_KinhDoanh, API.dm_KPI_KinhDoanh);
                    if (apiResponse.Success)
                        return RedirectToAction("Index");
                    else
                        ModelState.AddModelError(string.Empty, apiResponse.Message);
                }
                return View(dm_KPI_KinhDoanh);
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
                TempData["TitleError"] = API.TitleTryCatch;
                TempData["DetailError"] = ex.Message;
                return RedirectToAction("Index", "Notfound");
            }
        }

        // POST: Promotion/Delete/5
        [HttpPost]
        public ActionResult Delete(string id)
        {
            try
            {
                if (Utility.KiemTra())
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (!Utility.KiemTraQuyen(API.dm_KPI_KinhDoanh, API.Delete))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                var apiResponse = Utility.Delete<v_dm_KPI_KinhDoanh>(Utility.LOC_ID + "/" + id, API.dm_KPI_KinhDoanh);
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
                if (!Utility.KiemTraQuyen(API.dm_KPI_KinhDoanh, API.Create))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                v_v_dm_KPI_KinhDoanh dm_KPI_KinhDoanh = new v_v_dm_KPI_KinhDoanh();
                apiResponse.Success = true;
                dm_KPI_KinhDoanh.LOC_ID = Utility.LOC_ID;
                dm_KPI_KinhDoanh.TUNGAY = Utility.CurrentTime;
                dm_KPI_KinhDoanh.DENNGAY = Utility.CurrentTime.AddMonths(1);
                dm_KPI_KinhDoanh.ID = Guid.NewGuid().ToString();
                dm_KPI_KinhDoanh.lstdm_DonViTinh = new List<v_dm_DonViTinh>();
                dm_KPI_KinhDoanh.lstdm_DonViTinh = Utility.GetListData<v_dm_DonViTinh>(API.dm_DonViTinh, "", "", Utility.LOC_ID).Data as List<v_dm_DonViTinh>;
                List<v_dm_KPI_KinhDoanh_YeuCau> lstCTKM_YC = new List<v_dm_KPI_KinhDoanh_YeuCau>();
                List<v_dm_KPI_KinhDoanh_NhanVien> lstCTKM_NV = new List<v_dm_KPI_KinhDoanh_NhanVien>();
                Session[Sessions.lstKPISale_YeuCau] = lstCTKM_YC;
                Session[Sessions.lstKPISale_NhanVien] = lstCTKM_NV;
                var lst = Utility.ConvertobjectTo<v_v_dm_KPI_KinhDoanh>(dm_KPI_KinhDoanh);
                apiResponse.ProductCombo = Utility.GetKPISale_YeuCau();
                lst.Add(new ValueEdit { Key = "tbodyTempItemdivPromotion_YC", Value = apiResponse.ProductCombo });
                apiResponse.ProductCombo = Utility.GetKPISale_NhanVien();
                lst.Add(new ValueEdit { Key = "tbodyTempItemdivPromotion_Tang", Value = apiResponse.ProductCombo });
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
        public ActionResult CreatePopup([Bind(Include = "LOC_ID,ID,MA,NAME,TUNGAY,DENNGAY,CHIETKHAU,TIENGIAM,IS_YEUCAUCHITIET,TONGTIEN_DATKM,SOLUONG_DATKM,ID_DVT_DATKM,ISACTIVE,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO,ISTINHLUYTUYEN,ISTONGHOADON,CAPDO")] v_dm_KPI_KinhDoanh dm_KPI_KinhDoanh)
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                bool bolAddSuccess = false;
                if (Utility.KiemTra())
                {
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Admin");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                if (!Utility.KiemTraQuyen(API.dm_KPI_KinhDoanh, API.Create))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                if (ModelState.IsValid)
                {
                    dm_KPI_KinhDoanh.LOC_ID = Utility.LOC_ID;
                    dm_KPI_KinhDoanh.ID_NGUOITAO = Session[Sessions.idUser].ToString();
                    dm_KPI_KinhDoanh.THOIGIANTHEM = Utility.CurrentTime;
                    dm_KPI_KinhDoanh.lstdm_KPI_KinhDoanh_YeuCau = new List<v_dm_KPI_KinhDoanh_YeuCau>();
                    dm_KPI_KinhDoanh.lstdm_KPI_KinhDoanh_NhanVien = new List<v_dm_KPI_KinhDoanh_NhanVien>();
                    var lstKey_HINHTHUC_TINHKPI = Request.Form.AllKeys.Where(e => e.StartsWith("HINHTHUC_TINHKPI|"));
                    var lstKey = Request.Form.AllKeys.Where(e => e.StartsWith("txtMoney_YC|"));
                    var lstKey_YC = Request.Form.AllKeys.Where(e => e.StartsWith("txtQuantity_YC|"));
                    var lstKeyCHIETKHAU_YC = Request.Form.AllKeys.Where(e => e.StartsWith("txtCHIETKHAU_YC|"));
                    var lstKeyTIENGIAM_YC = Request.Form.AllKeys.Where(e => e.StartsWith("txtTIENGIAM_YC|"));
                    if (lstKey == null || lstKey.Count() == 0 
                        || lstKey_YC == null || lstKey_YC.Count() == 0 
                        || lstKeyCHIETKHAU_YC == null || lstKeyCHIETKHAU_YC.Count() == 0
                        || lstKeyTIENGIAM_YC == null || lstKeyTIENGIAM_YC.Count() == 0)
                    {
                        ModelState.AddModelError("lstdm_KPI_KinhDoanh_YeuCau", "Thêm sản phẩm.");
                    }
                    else
                    {
                       
                        int i = 0;
                        foreach (var itm in lstKey)
                        {
                            var lstString = itm.ToString().Split('|');
                            var value_st = HttpContext.Request.Params.GetValues(itm.ToString());
                            var value_sl = HttpContext.Request.Params.GetValues(lstKey_YC.ToList()[i].ToString());
                            var value_ck = HttpContext.Request.Params.GetValues(lstKeyCHIETKHAU_YC.ToList()[i].ToString());
                            var value_tg = HttpContext.Request.Params.GetValues(lstKeyTIENGIAM_YC.ToList()[i].ToString());
                            var value_tinhkpi = HttpContext.Request.Params.GetValues(lstKey_HINHTHUC_TINHKPI.ToList()[i].ToString());
                            string ShowSearchValue = clsMaHoa.Decrypt(lstString[1].ToString(), clsMaHoa.PassMaHoa);
                            var dm_KPI_KinhDoanh_YeuCau = JsonConvert.DeserializeObject<v_dm_KPI_KinhDoanh_YeuCau>(ShowSearchValue);
                            if (lstString != null)
                            {
                                if(string.IsNullOrEmpty(dm_KPI_KinhDoanh_YeuCau.ID))
                                    dm_KPI_KinhDoanh_YeuCau.ID = Guid.NewGuid().ToString();
                                dm_KPI_KinhDoanh_YeuCau.LOC_ID = Utility.LOC_ID;
                                dm_KPI_KinhDoanh_YeuCau.ID_KPI_KINHDOANH = dm_KPI_KinhDoanh.ID;
                                dm_KPI_KinhDoanh_YeuCau.SOTIEN = Utility.ConvertStringToDouble(value_st[0]);
                                dm_KPI_KinhDoanh_YeuCau.SOLUONG = Utility.ConvertStringToDouble(value_sl[0]);
                                dm_KPI_KinhDoanh_YeuCau.CHIETKHAU = Utility.ConvertStringToDouble(value_ck[0]);
                                dm_KPI_KinhDoanh_YeuCau.TIENGIAM = Utility.ConvertStringToDouble(value_tg[0]);
                                dm_KPI_KinhDoanh_YeuCau.HINHTHUC_TINHKPI = Convert.ToInt32(Utility.ConvertStringToDouble(value_tinhkpi[0]));
                                dm_KPI_KinhDoanh.lstdm_KPI_KinhDoanh_YeuCau.Add(dm_KPI_KinhDoanh_YeuCau);
                            }

                            i += 1;
                        }
                    }

                    var lstKey_Tang = Request.Form.AllKeys.Where(e => e.StartsWith("txtISACTIVE|"));
                    if (lstKey_Tang == null || lstKey_Tang.Count() == 0)
                    {
                        ModelState.AddModelError("lstdm_KPI_KinhDoanh_NhanVien", "Thêm nhân viên.");
                    }
                    else
                    {
                        int i = 0;
                        foreach (var itm in lstKey_Tang)
                        {
                            var lstString = itm.ToString().Split('|');
                            var value = HttpContext.Request.Params.GetValues(itm.ToString());
                            string ShowSearchValue = clsMaHoa.Decrypt(lstString[1].ToString(), clsMaHoa.PassMaHoa);
                            var dm_ChuongTrinhKhuyenMai_Tang = JsonConvert.DeserializeObject<v_dm_KPI_KinhDoanh_NhanVien>(ShowSearchValue);
                            if (lstString != null)
                            {
                                if (string.IsNullOrEmpty(dm_ChuongTrinhKhuyenMai_Tang.ID))
                                    dm_ChuongTrinhKhuyenMai_Tang.ID = Guid.NewGuid().ToString();
                                dm_ChuongTrinhKhuyenMai_Tang.LOC_ID = Utility.LOC_ID;
                                dm_ChuongTrinhKhuyenMai_Tang.ID_KPI_KINHDOANH = dm_KPI_KinhDoanh.ID;
                                dm_KPI_KinhDoanh.lstdm_KPI_KinhDoanh_NhanVien.Add(dm_ChuongTrinhKhuyenMai_Tang);
                            }
                            i += 1;
                        }
                    }
                    apiResponse = Utility.Create<v_dm_KPI_KinhDoanh>(dm_KPI_KinhDoanh, API.dm_KPI_KinhDoanh);
                    if (apiResponse.Success)
                    {
                        if (apiResponse.Data != null)
                            dm_KPI_KinhDoanh = JsonConvert.DeserializeObject<v_dm_KPI_KinhDoanh>(apiResponse.Data.ToString());
                        apiResponse.NewID = Guid.NewGuid().ToString();
                        List<v_dm_KPI_KinhDoanh_YeuCau> lstCTKM_YC = new List<v_dm_KPI_KinhDoanh_YeuCau>();
                        List<v_dm_KPI_KinhDoanh_NhanVien> lstCTKM_NV = new List<v_dm_KPI_KinhDoanh_NhanVien>();
                        Session[Sessions.lstKPISale_YeuCau] = lstCTKM_YC;
                        Session[Sessions.lstKPISale_NhanVien] = lstCTKM_NV;
                        bolAddSuccess = true;
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
                    apiResponse.Data = Utility.GetModelState(ModelState, API.dm_KPI_KinhDoanh);
                }
                apiResponse.ID = dm_KPI_KinhDoanh.ID;
                var lst = Utility.ConvertobjectTo<v_dm_KPI_KinhDoanh>(dm_KPI_KinhDoanh);
                if(bolAddSuccess)
                {
                    apiResponse.ProductCombo = Utility.GetKPISale_YeuCau();
                    lst.Add(new ValueEdit { Key = "tbodyTempItemdivPromotion_YC", Value = apiResponse.ProductCombo });
                    apiResponse.ProductCombo = Utility.GetKPISale_NhanVien();
                    lst.Add(new ValueEdit { Key = "tbodyTempItemdivPromotion_Tang", Value = apiResponse.ProductCombo });
                }    
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
                if (!Utility.KiemTraQuyen(API.dm_KPI_KinhDoanh, API.Edit))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                v_v_dm_KPI_KinhDoanh dm_KPI_KinhDoanh = new v_v_dm_KPI_KinhDoanh();
                if (!string.IsNullOrEmpty(id))
                {
                    apiResponse = Utility.GetDetail<v_v_dm_KPI_KinhDoanh>(Utility.LOC_ID + "/" + id, API.dm_KPI_KinhDoanh);
                    if (!apiResponse.Success)
                    {
                        TempData["TitleError"] = apiResponse.Message;
                        apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                        return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                    }
                    if (apiResponse.Data != null)
                        dm_KPI_KinhDoanh = apiResponse.Data as v_v_dm_KPI_KinhDoanh;
                }
                apiResponse.Success = true;
                dm_KPI_KinhDoanh.lstdm_DonViTinh = new List<v_dm_DonViTinh>();
                dm_KPI_KinhDoanh.lstdm_DonViTinh = Utility.GetListData<v_dm_DonViTinh>(API.dm_DonViTinh, "", "", Utility.LOC_ID).Data as List<v_dm_DonViTinh>;
                List<v_dm_KPI_KinhDoanh_YeuCau> lstCTKM_YC = new List<v_dm_KPI_KinhDoanh_YeuCau>();
                List<v_dm_KPI_KinhDoanh_NhanVien> lstCTKM_NV = new List<v_dm_KPI_KinhDoanh_NhanVien>();
                foreach (var itm in dm_KPI_KinhDoanh.lstdm_KPI_KinhDoanh_YeuCau)
                {
                    lstCTKM_YC.Add(itm);
                }
                foreach (var itm in dm_KPI_KinhDoanh.lstdm_KPI_KinhDoanh_NhanVien)
                {
                    lstCTKM_NV.Add(itm);
                }
                Session[Sessions.lstKPISale_YeuCau] = lstCTKM_YC;
                Session[Sessions.lstKPISale_NhanVien] = lstCTKM_NV;
                var lst = Utility.ConvertobjectTo<v_v_dm_KPI_KinhDoanh>(dm_KPI_KinhDoanh);
                apiResponse.ProductCombo = Utility.GetKPISale_YeuCau();
                lst.Add(new ValueEdit { Key = "tbodyTempItemdivPromotion_YCEdit", Value = apiResponse.ProductCombo });
                apiResponse.ProductCombo = Utility.GetKPISale_NhanVien();
                lst.Add(new ValueEdit { Key = "tbodyTempItemdivPromotion_TangEdit", Value = apiResponse.ProductCombo });
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
        public ActionResult EditPopup([Bind(Include = "LOC_ID,ID,MA,NAME,TUNGAY,DENNGAY,CHIETKHAU,TIENGIAM,IS_YEUCAUCHITIET,TONGTIEN_DATKM,SOLUONG_DATKM,ID_DVT_DATKM,ISACTIVE,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO,ISTINHLUYTUYEN,ISTONGHOADON,CAPDO")] v_dm_KPI_KinhDoanh dm_KPI_KinhDoanh)
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                if (Utility.KiemTra())
                {
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Admin");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                if (!Utility.KiemTraQuyen(API.dm_KPI_KinhDoanh, API.Edit))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                if (ModelState.IsValid)
                {
                    dm_KPI_KinhDoanh.LOC_ID = Utility.LOC_ID;
                    dm_KPI_KinhDoanh.ID_NGUOISUA = Session[Sessions.idUser].ToString();
                    dm_KPI_KinhDoanh.THOIGIANSUA = Utility.CurrentTime;
                    dm_KPI_KinhDoanh.lstdm_KPI_KinhDoanh_YeuCau = new List<v_dm_KPI_KinhDoanh_YeuCau>();
                    dm_KPI_KinhDoanh.lstdm_KPI_KinhDoanh_NhanVien = new List<v_dm_KPI_KinhDoanh_NhanVien>();
                    var lstKey_HINHTHUC_TINHKPI = Request.Form.AllKeys.Where(e => e.StartsWith("HINHTHUC_TINHKPI|"));
                    var lstKey = Request.Form.AllKeys.Where(e => e.StartsWith("txtMoney_YC|"));
                    var lstKey_YC = Request.Form.AllKeys.Where(e => e.StartsWith("txtQuantity_YC|"));
                    var lstKeyCHIETKHAU_YC = Request.Form.AllKeys.Where(e => e.StartsWith("txtCHIETKHAU_YC|"));
                    var lstKeyTIENGIAM_YC = Request.Form.AllKeys.Where(e => e.StartsWith("txtTIENGIAM_YC|"));
                    if (lstKey == null || lstKey.Count() == 0
                         || lstKey_YC == null || lstKey_YC.Count() == 0
                         || lstKeyCHIETKHAU_YC == null || lstKeyCHIETKHAU_YC.Count() == 0
                         || lstKeyTIENGIAM_YC == null || lstKeyTIENGIAM_YC.Count() == 0)
                    {
                        ModelState.AddModelError("lstdm_KPI_KinhDoanh_YeuCau", "Thêm sản phẩm trong yêu cầu.");
                    }
                    else
                    {

                        int i = 0;
                        foreach (var itm in lstKey)
                        {
                            var lstString = itm.ToString().Split('|');
                            var value_st = HttpContext.Request.Params.GetValues(itm.ToString());
                            string ShowSearchValue = clsMaHoa.Decrypt(lstString[1].ToString(), clsMaHoa.PassMaHoa);
                            var value_sl = HttpContext.Request.Params.GetValues(lstKey_YC.ToList()[i].ToString());
                            var value_ck = HttpContext.Request.Params.GetValues(lstKeyCHIETKHAU_YC.ToList()[i].ToString());
                            var value_tg = HttpContext.Request.Params.GetValues(lstKeyTIENGIAM_YC.ToList()[i].ToString());
                            var value_tinhkpi = HttpContext.Request.Params.GetValues(lstKey_HINHTHUC_TINHKPI.ToList()[i].ToString());
                            var dm_KPI_KinhDoanh_YeuCau = JsonConvert.DeserializeObject<v_dm_KPI_KinhDoanh_YeuCau>(ShowSearchValue);
                            if (lstString != null)
                            {
                                if (string.IsNullOrEmpty(dm_KPI_KinhDoanh_YeuCau.ID))
                                    dm_KPI_KinhDoanh_YeuCau.ID = Guid.NewGuid().ToString();
                                dm_KPI_KinhDoanh_YeuCau.LOC_ID = Utility.LOC_ID;
                                dm_KPI_KinhDoanh_YeuCau.ID_KPI_KINHDOANH = dm_KPI_KinhDoanh.ID;
                                dm_KPI_KinhDoanh_YeuCau.SOLUONG = Utility.ConvertStringToDouble(value_sl[0]);
                                dm_KPI_KinhDoanh_YeuCau.SOTIEN = Utility.ConvertStringToDouble(value_st[0]);
                                dm_KPI_KinhDoanh_YeuCau.CHIETKHAU = Utility.ConvertStringToDouble(value_ck[0]);
                                dm_KPI_KinhDoanh_YeuCau.TIENGIAM = Utility.ConvertStringToDouble(value_tg[0]);
                                dm_KPI_KinhDoanh_YeuCau.HINHTHUC_TINHKPI = Convert.ToInt32(Utility.ConvertStringToDouble(value_tinhkpi[0]));
                                dm_KPI_KinhDoanh.lstdm_KPI_KinhDoanh_YeuCau.Add(dm_KPI_KinhDoanh_YeuCau);
                            }

                            i += 1;
                        }
                    }

                    var lstKey_Tang = Request.Form.AllKeys.Where(e => e.StartsWith("txtISACTIVE|"));
                    if (lstKey_Tang == null || lstKey_Tang.Count() == 0)
                    {
                        ModelState.AddModelError("lstdm_KPI_KinhDoanh_NhanVien", "Thêm nhân viên .");
                    }
                    else
                    {
                        int i = 0;
                        foreach (var itm in lstKey_Tang)
                        {
                            var lstString = itm.ToString().Split('|');
                            var value = HttpContext.Request.Params.GetValues(itm.ToString());
                            string ShowSearchValue = clsMaHoa.Decrypt(lstString[1].ToString(), clsMaHoa.PassMaHoa);
                            var dm_ChuongTrinhKhuyenMai_Tang = JsonConvert.DeserializeObject<v_dm_KPI_KinhDoanh_NhanVien>(ShowSearchValue);
                            if (lstString != null)
                            {
                                if (string.IsNullOrEmpty(dm_ChuongTrinhKhuyenMai_Tang.ID))
                                    dm_ChuongTrinhKhuyenMai_Tang.ID = Guid.NewGuid().ToString();
                                dm_ChuongTrinhKhuyenMai_Tang.LOC_ID = Utility.LOC_ID;
                                dm_ChuongTrinhKhuyenMai_Tang.ID_KPI_KINHDOANH = dm_KPI_KinhDoanh.ID;
                                dm_KPI_KinhDoanh.lstdm_KPI_KinhDoanh_NhanVien.Add(dm_ChuongTrinhKhuyenMai_Tang);
                            }
                            i += 1;
                        }
                    }

                    apiResponse = Utility.Edit<v_dm_KPI_KinhDoanh>(Utility.LOC_ID + "/" + dm_KPI_KinhDoanh.MA, dm_KPI_KinhDoanh, API.dm_KPI_KinhDoanh);
                    if (apiResponse.Success)
                    {
                        //return RedirectToAction("Index");
                        if (apiResponse.Data != null)
                            dm_KPI_KinhDoanh = JsonConvert.DeserializeObject<v_dm_KPI_KinhDoanh>(apiResponse.Data.ToString());
                        apiResponse.ID = dm_KPI_KinhDoanh.ID;
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, apiResponse.Message);
                    }
                }
                else
                {
                    apiResponse.Success = false;
                    apiResponse.Data = Utility.GetModelState(ModelState, API.dm_KPI_KinhDoanh);
                }
                apiResponse.Detail = Utility.ConvertobjectToView<dm_KPI_KinhDoanh>(dm_KPI_KinhDoanh);
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
                if (!Utility.KiemTraQuyen(API.dm_KPI_KinhDoanh, API.Delete))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                apiResponse = Utility.Delete<v_dm_KPI_KinhDoanh>(Utility.LOC_ID + "/" + id, API.dm_KPI_KinhDoanh);
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

        [HttpPost, ValidateInput(false)]
        public ActionResult AddProductPromotion_YC([Bind(Include = "ID_HANGHOA,SOLUONG,ID_DVT")] v_v_dm_KPI_KinhDoanh_YeuCau dm_HangHoa_Combo)
        {
            ApiResponse apiResponse = new ApiResponse();
            v_v_dm_HangHoa dm_HangHoa = new v_v_dm_HangHoa();
            apiResponse = Utility.GetDetail<v_v_dm_HangHoa>(Utility.LOC_ID + "/" + dm_HangHoa_Combo.ID_HANGHOA, API.dm_HangHoa);
            if (!apiResponse.Success)
            {
                TempData["TitleError"] = apiResponse.Message;
                apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
            }
            if (apiResponse.Data != null)
                dm_HangHoa = apiResponse.Data as v_v_dm_HangHoa;

            if (dm_HangHoa != null)
            {
                dm_HangHoa_Combo.HINHTHUC = (int)HinhThucKhuyenMai.SanPham;
                dm_HangHoa_Combo.NAME = dm_HangHoa.NAME;
                dm_HangHoa_Combo.MA = dm_HangHoa.MA;
                if (dm_HangHoa.ID_DVT == dm_HangHoa_Combo.ID_DVT)
                {
                    dm_HangHoa_Combo.NAME_DVT = dm_HangHoa.NAME_DVT;
                    if (!string.IsNullOrEmpty(dm_HangHoa.ID_DVT_QD))
                    {
                        dm_HangHoa_Combo.TYLE_QD = dm_HangHoa.TYLE_QD;
                    }
                    else
                    {
                        if (dm_HangHoa.LOAIHANGHOA == ((int)API.LoaiSanPham.KhongQuanLyTonKho).ToString())
                            dm_HangHoa_Combo.TYLE_QD = 0;
                        else
                            dm_HangHoa_Combo.TYLE_QD = 1;

                    }
                }
                else if (dm_HangHoa.ID_DVT_QD == dm_HangHoa_Combo.ID_DVT)
                {
                    if (!string.IsNullOrEmpty(dm_HangHoa.ID_DVT_QD))
                    {
                        dm_HangHoa_Combo.NAME_DVT = dm_HangHoa.NAME_DVT_QD;
                        dm_HangHoa_Combo.TYLE_QD = 1;
                    }
                }
                var check = Utility.LstKPISale_YeuCau.Where(e => e.ID_HANGHOA == dm_HangHoa_Combo.ID_HANGHOA && e.ID_DVT == dm_HangHoa_Combo.ID_DVT).FirstOrDefault();
                if (check == null)
                {
                    var LstKPISale_YeuCau = Utility.LstKPISale_YeuCau;
                    LstKPISale_YeuCau.Add(dm_HangHoa_Combo);
                    Session[Sessions.lstKPISale_YeuCau] = LstKPISale_YeuCau;
                }
                else
                {
                    check.SOLUONG = dm_HangHoa_Combo.SOLUONG;
                    check.SOTIEN = dm_HangHoa_Combo.SOTIEN;
                }
            }
            apiResponse.ProductCombo = Utility.GetKPISale_YeuCau();
            return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult AddProductPromotionNHH_YC([Bind(Include = "ID_HANGHOA,SOLUONG,ID_DVT")] v_v_dm_KPI_KinhDoanh_YeuCau dm_CTKM_YC)
        {
            ApiResponse apiResponse = new ApiResponse();
            v_v_dm_NhomHangHoa dm_NhomHangHoa = new v_v_dm_NhomHangHoa();
            v_v_dm_DonViTinh dm_DonViTinh = new v_v_dm_DonViTinh();
            apiResponse = Utility.GetDetail<v_v_dm_NhomHangHoa>(Utility.LOC_ID + "/" + dm_CTKM_YC.ID_HANGHOA, API.dm_NhomHangHoa);
            if (!apiResponse.Success)
            {
                TempData["TitleError"] = apiResponse.Message;
                apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
            }
            if (apiResponse.Data != null)
                dm_NhomHangHoa = apiResponse.Data as v_v_dm_NhomHangHoa;

            apiResponse = Utility.GetDetail<v_v_dm_DonViTinh>(Utility.LOC_ID + "/" + dm_CTKM_YC.ID_DVT, API.dm_DonViTinh);
            if (!apiResponse.Success)
            {
                TempData["TitleError"] = apiResponse.Message;
                apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
            }
            if (apiResponse.Data != null)
                dm_DonViTinh = apiResponse.Data as v_v_dm_DonViTinh;

            if (dm_NhomHangHoa != null && dm_DonViTinh != null)
            {
                dm_CTKM_YC.HINHTHUC = (int)HinhThucKhuyenMai.NhomSanPham;
                dm_CTKM_YC.ID_HANGHOA = dm_NhomHangHoa.ID;
                dm_CTKM_YC.NAME = dm_NhomHangHoa.NAME;
                dm_CTKM_YC.MA = dm_NhomHangHoa.MA;
                dm_CTKM_YC.NAME_DVT = dm_DonViTinh.NAME;
                var check = Utility.LstKPISale_YeuCau.Where(e => e.ID_HANGHOA == dm_CTKM_YC.ID_HANGHOA && e.ID_DVT == dm_CTKM_YC.ID_DVT).FirstOrDefault();
                if (check == null)
                {
                    var LstKPISale_YeuCau = Utility.LstKPISale_YeuCau;
                    LstKPISale_YeuCau.Add(dm_CTKM_YC);
                    Session[Sessions.lstKPISale_YeuCau] = LstKPISale_YeuCau;
                }
                else
                {
                    check.SOLUONG = dm_CTKM_YC.SOLUONG;
                    check.SOTIEN = dm_CTKM_YC.SOTIEN;
                }
            }
            apiResponse.ProductCombo = Utility.GetKPISale_YeuCau();
            return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
        }

        [HttpPost]
        public ActionResult DeleteProductPromotion_YC(string ID_HANGHOA, string ID_DVT)
        {
            ApiResponse apiResponse = new ApiResponse();
            v_v_dm_HangHoa dm_HangHoa = new v_v_dm_HangHoa();
            var LstKPISale_YeuCau = Utility.LstKPISale_YeuCau;
            var check = Utility.LstKPISale_YeuCau.Where(e => e.ID_HANGHOA == ID_HANGHOA && e.ID_DVT == ID_DVT).FirstOrDefault();
            if (check != null)
                LstKPISale_YeuCau.Remove(check);

            Session[Sessions.lstKPISale_YeuCau] = LstKPISale_YeuCau;
            apiResponse.ProductCombo = Utility.GetKPISale_YeuCau();
            apiResponse.Success = true;
            return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
        }

        [HttpGet]
        public ActionResult AddProductPromotion_NQ(string ID)
        {
            ApiResponse apiResponse = new ApiResponse();
            v_v_web_NhomQuyen dm_NhomHangHoa = new v_v_web_NhomQuyen();
            apiResponse = Utility.GetDetail<v_v_web_NhomQuyen>(Utility.LOC_ID + "/" + ID, API.web_NhomQuyen);
            if (!apiResponse.Success)
            {
                TempData["TitleError"] = apiResponse.Message;
                apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
            }
            if (apiResponse.Data != null)
                dm_NhomHangHoa = apiResponse.Data as v_v_web_NhomQuyen;

            if (dm_NhomHangHoa != null)
            {
                v_dm_KPI_KinhDoanh_NhanVien dm_CTKM_YC = new v_dm_KPI_KinhDoanh_NhanVien();
                dm_CTKM_YC.HINHTHUC = (int)HinhThucKhuyenMai.NhomSanPham;
                dm_CTKM_YC.ID_NHANVIEN = ID;
                dm_CTKM_YC.NAME = dm_NhomHangHoa.NAME;
                dm_CTKM_YC.MA = dm_NhomHangHoa.MA;
                var check = Utility.LstKPISale_NhanVien.Where(e => e.ID_NHANVIEN == dm_CTKM_YC.ID_NHANVIEN && e.HINHTHUC == (int)API.HinhThucKhuyenMai.NhomSanPham).FirstOrDefault();
                if (check == null)
                {
                    var LstKPISale_NhanVien = Utility.LstKPISale_NhanVien;
                    LstKPISale_NhanVien.Add(dm_CTKM_YC);
                    Session[Sessions.lstKPISale_NhanVien] = LstKPISale_NhanVien;
                }
            }
            apiResponse.ProductCombo = Utility.GetKPISale_NhanVien();
            List<ValueEdit> lst = new List<ValueEdit>();
            apiResponse.ProductCombo = Utility.GetKPISale_NhanVien();
            lst.Add(new ValueEdit { Key = "tbodyTempItemdivPromotion_TangEdit", Value = apiResponse.ProductCombo });
            lst.Add(new ValueEdit { Key = "tbodyTempItemdivPromotion_Tang", Value = apiResponse.ProductCombo });
            apiResponse.Detail = lst;
            apiResponse.Success = true;
            return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
        }

        [HttpGet]
        public ActionResult AddProductPromotion_NV(string ID)
        {
            ApiResponse apiResponse = new ApiResponse();
            v_v_dm_NhanVien dm_NhomHangHoa = new v_v_dm_NhanVien();
            apiResponse = Utility.GetDetail<v_v_dm_NhanVien>(Utility.LOC_ID + "/" + ID, API.dm_NhanVien);
            if (!apiResponse.Success)
            {
                TempData["TitleError"] = apiResponse.Message;
                apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
            }
            if (apiResponse.Data != null)
                dm_NhomHangHoa = apiResponse.Data as v_v_dm_NhanVien;

            if (dm_NhomHangHoa != null)
            {
                v_dm_KPI_KinhDoanh_NhanVien dm_CTKM_YC = new v_dm_KPI_KinhDoanh_NhanVien();
                dm_CTKM_YC.HINHTHUC = (int)HinhThucKhuyenMai.SanPham;
                dm_CTKM_YC.ID_NHANVIEN = ID;
                dm_CTKM_YC.NAME = dm_NhomHangHoa.NAME;
                dm_CTKM_YC.MA = dm_NhomHangHoa.MA;
                var check = Utility.LstKPISale_NhanVien.Where(e => e.ID_NHANVIEN == dm_CTKM_YC.ID_NHANVIEN && e.HINHTHUC == dm_CTKM_YC.HINHTHUC).FirstOrDefault();
                if (check == null)
                {
                    var LstKPISale_NhanVien = Utility.LstKPISale_NhanVien;
                    LstKPISale_NhanVien.Add(dm_CTKM_YC);
                    Session[Sessions.lstKPISale_NhanVien] = LstKPISale_NhanVien;
                }
            }
            apiResponse.ProductCombo = Utility.GetKPISale_NhanVien();
            List<ValueEdit> lst = new List<ValueEdit>();
            apiResponse.ProductCombo = Utility.GetKPISale_NhanVien();
            lst.Add(new ValueEdit { Key = "tbodyTempItemdivPromotion_TangEdit", Value = apiResponse.ProductCombo });
            lst.Add(new ValueEdit { Key = "tbodyTempItemdivPromotion_Tang", Value = apiResponse.ProductCombo });
            apiResponse.Detail = lst;
            apiResponse.Success = true;
            return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
        }

        [HttpPost]
        public ActionResult DeleteProductPromotion_Tang(string ID_HANGHOA, string ID_DVT)
        {
            ApiResponse apiResponse = new ApiResponse();
            v_v_dm_HangHoa dm_HangHoa = new v_v_dm_HangHoa();
            var LstKPISale_NhanVien = Utility.LstKPISale_NhanVien;
            var check = Utility.LstKPISale_NhanVien.Where(e => e.ID_NHANVIEN == ID_HANGHOA && e.HINHTHUC.ToString() == ID_DVT).FirstOrDefault();
            if (check != null)
                LstKPISale_NhanVien.Remove(check);

            Session[Sessions.lstKPISale_NhanVien] = LstKPISale_NhanVien;
            List<ValueEdit> lst = new List<ValueEdit>();
            apiResponse.ProductCombo = Utility.GetKPISale_NhanVien();
            lst.Add(new ValueEdit { Key = "tbodyTempItemdivPromotion_TangEdit", Value = apiResponse.ProductCombo });
            lst.Add(new ValueEdit { Key = "tbodyTempItemdivPromotion_Tang", Value = apiResponse.ProductCombo });
            apiResponse.Detail = lst;
            apiResponse.Success = true;
            return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
        }

        #region Tạo phiếu xuất từ phiếu đặt hàng
        public ActionResult OnSubmitKPI_Sale(String cartOrder)
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
                    Deposit.NGAYLAP = Utility.CurrentTime;
                }
                apiResponse = Utility.Create<List<Deposit>>(lstcartOrder, API.dm_KPI_KinhDoanh + "/PostCreateKPI_Sale");
                if (apiResponse.Success)
                {
                    newReturn.Message = "Tạo chương trình thành công!";
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