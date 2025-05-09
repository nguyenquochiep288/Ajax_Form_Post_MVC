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
using System.Windows.Documents;
using System.Windows.Input;

namespace MVC_QuanLyTHP.Controllers
{
    public class PromotionController : Controller
    {

        // GET: Promotion
        public ActionResult Index(int Page = 1, string SearchString = "", string ShowSearchValue = "")
        {
            try
            {
                if (Utility.KiemTra())
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (!Utility.KiemTraQuyen(API.dm_ChuongTrinhKhuyenMai, API.Xem))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                ShowSearchValue = Utility.GetShowSearchValue<dm_ChuongTrinhKhuyenMai>(ShowSearchValue);
                var apiResponse = Utility.GetListData<v_dm_ChuongTrinhKhuyenMai>(API.dm_ChuongTrinhKhuyenMai, ShowSearchValue, SearchString, Utility.LOC_ID);
                if (!apiResponse.Success)
                {
                    TempData["TitleError"] = apiResponse.Message;
                    return RedirectToAction("Index", "Notfound");
                }
                IPagedList<v_dm_ChuongTrinhKhuyenMai> lstpage = (apiResponse.Data as List<v_dm_ChuongTrinhKhuyenMai>).ToPagedList(Page, Utility.GetPageSize());

                v_v_dm_ChuongTrinhKhuyenMai dm_ChuongTrinhKhuyenMai = new v_v_dm_ChuongTrinhKhuyenMai();
                dm_ChuongTrinhKhuyenMai.IPagedList = lstpage;
                dm_ChuongTrinhKhuyenMai.lstdm_DonViTinh = new List<v_dm_DonViTinh>();
                dm_ChuongTrinhKhuyenMai.lstdm_DonViTinh = Utility.GetListData<v_dm_DonViTinh>(API.dm_DonViTinh, "", "", Utility.LOC_ID).Data as List<v_dm_DonViTinh>;


                ViewBag.searchValue = SearchString;
                ViewBag.showsearchValue = ShowSearchValue;

                ViewBag.PermissionEdit = Utility.KiemTraQuyen(API.dm_ChuongTrinhKhuyenMai, API.Edit);
                ViewBag.PermissionDelete = Utility.KiemTraQuyen(API.dm_ChuongTrinhKhuyenMai, API.Delete);
                ViewBag.PermissionCreate = Utility.KiemTraQuyen(API.dm_ChuongTrinhKhuyenMai, API.Create);
                return View(dm_ChuongTrinhKhuyenMai);
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
                if (!Utility.KiemTraQuyen(API.dm_ChuongTrinhKhuyenMai, API.Create))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                v_v_dm_ChuongTrinhKhuyenMai dm_ChuongTrinhKhuyenMai = new v_v_dm_ChuongTrinhKhuyenMai();
                dm_ChuongTrinhKhuyenMai.LOC_ID = Utility.LOC_ID;
                dm_ChuongTrinhKhuyenMai.ID_NGUOITAO = Session[Sessions.idUser].ToString();
                dm_ChuongTrinhKhuyenMai.THOIGIANTHEM = Utility.CurrentTime;

                dm_ChuongTrinhKhuyenMai.ID = Guid.NewGuid().ToString();
                dm_ChuongTrinhKhuyenMai.lstdm_DonViTinh = new List<v_dm_DonViTinh>();
                dm_ChuongTrinhKhuyenMai.lstdm_DonViTinh = Utility.GetListData<v_dm_DonViTinh>(API.dm_DonViTinh, "", "", Utility.LOC_ID).Data as List<v_dm_DonViTinh>;

                return View(dm_ChuongTrinhKhuyenMai);
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
        public ActionResult Create([Bind(Include = "LOC_ID,ID,MA,NAME,TUNGAY,DENNGAY,CHIETKHAU,TIENGIAM,IS_YEUCAUCHITIET,TONGTIEN_DATKM,SOLUONG_DATKM,ID_DVT_DATKM,ISACTIVE,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO,ISTINHLUYTUYEN,ISTONGHOADON,SOLUONG_DATKM_DEN,TONGTIEN_DATKM_DEN,HINHTHUC_TINHKPI")] v_dm_ChuongTrinhKhuyenMai dm_ChuongTrinhKhuyenMai)
        {
            try
            {
                if (Utility.KiemTra())
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (!Utility.KiemTraQuyen(API.dm_ChuongTrinhKhuyenMai, API.Create))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                if (ModelState.IsValid)
                {
                    dm_ChuongTrinhKhuyenMai.LOC_ID = Utility.LOC_ID;
                    dm_ChuongTrinhKhuyenMai.ID_NGUOITAO = Session[Sessions.idUser].ToString();
                    dm_ChuongTrinhKhuyenMai.THOIGIANTHEM = Utility.CurrentTime;

                    //@ConvertObjectUnicodeToTCVN3
                    var apiResponse = Utility.Create<dm_ChuongTrinhKhuyenMai>(dm_ChuongTrinhKhuyenMai, API.dm_ChuongTrinhKhuyenMai);
                    if (apiResponse.Success)
                        return RedirectToAction("Index");
                    else
                    {
                        ModelState.AddModelError(string.Empty, apiResponse.Message);
                        if (apiResponse.CheckValue)
                            ViewBag.ID = Guid.NewGuid().ToString();
                    }
                }
                return View(dm_ChuongTrinhKhuyenMai);
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
                if (!Utility.KiemTraQuyen(API.dm_ChuongTrinhKhuyenMai, API.Edit))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                v_v_dm_ChuongTrinhKhuyenMai dm_ChuongTrinhKhuyenMai = new v_v_dm_ChuongTrinhKhuyenMai();
                if (!string.IsNullOrEmpty(id))
                {
                    apiResponse = Utility.GetDetail<v_dm_ChuongTrinhKhuyenMai>(Utility.LOC_ID + "/" + id, API.dm_ChuongTrinhKhuyenMai);
                    if (!apiResponse.Success)
                    {
                        TempData["TitleError"] = apiResponse.Message;
                        return RedirectToAction("Index", "Notfound");
                    }
                    if (apiResponse.Data != null)
                        dm_ChuongTrinhKhuyenMai = apiResponse.Data as v_v_dm_ChuongTrinhKhuyenMai;
                }
                //@ConvertObjectTCVN3ToUnicode
                dm_ChuongTrinhKhuyenMai.lstdm_DonViTinh = new List<v_dm_DonViTinh>();
                dm_ChuongTrinhKhuyenMai.lstdm_DonViTinh = Utility.GetListData<v_dm_DonViTinh>(API.dm_DonViTinh, "", "", Utility.LOC_ID).Data as List<v_dm_DonViTinh>;

                return View(dm_ChuongTrinhKhuyenMai);
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
        public ActionResult Edit([Bind(Include = "LOC_ID,ID,MA,NAME,TUNGAY,DENNGAY,CHIETKHAU,TIENGIAM,IS_YEUCAUCHITIET,TONGTIEN_DATKM,SOLUONG_DATKM,ID_DVT_DATKM,ISACTIVE,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO,ISTINHLUYTUYEN,ISTONGHOADON,SOLUONG_DATKM_DEN,TONGTIEN_DATKM_DEN,HINHTHUC_TINHKPI")] v_dm_ChuongTrinhKhuyenMai dm_ChuongTrinhKhuyenMai)
        {
            try
            {
                if (Utility.KiemTra())
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (!Utility.KiemTraQuyen(API.dm_ChuongTrinhKhuyenMai, API.Edit))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                if (ModelState.IsValid)
                {
                    dm_ChuongTrinhKhuyenMai.LOC_ID = Utility.LOC_ID;
                    dm_ChuongTrinhKhuyenMai.ID_NGUOISUA = Session[Sessions.idUser].ToString();
                    dm_ChuongTrinhKhuyenMai.THOIGIANSUA = Utility.CurrentTime;
                    //@ConvertObjectUnicodeToTCVN3
                    var apiResponse = Utility.Edit<v_dm_ChuongTrinhKhuyenMai>(Utility.LOC_ID + "/" + dm_ChuongTrinhKhuyenMai.MA, dm_ChuongTrinhKhuyenMai, API.dm_ChuongTrinhKhuyenMai);
                    if (apiResponse.Success)
                        return RedirectToAction("Index");
                    else
                        ModelState.AddModelError(string.Empty, apiResponse.Message);
                }
                return View(dm_ChuongTrinhKhuyenMai);
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
                if (!Utility.KiemTraQuyen(API.dm_ChuongTrinhKhuyenMai, API.Delete))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                var apiResponse = Utility.Delete<v_dm_ChuongTrinhKhuyenMai>(Utility.LOC_ID + "/" + id, API.dm_ChuongTrinhKhuyenMai);
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
                if (!Utility.KiemTraQuyen(API.dm_ChuongTrinhKhuyenMai, API.Create))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                List<v_dm_ChuongTrinhKhuyenMai_YeuCau> lstCTKM_YC = new List<v_dm_ChuongTrinhKhuyenMai_YeuCau>();
                Session[Sessions.lstCTKM_YeuCau] = lstCTKM_YC;

                List<v_dm_ChuongTrinhKhuyenMai_Tang> lstCTKM_Tang = new List<v_dm_ChuongTrinhKhuyenMai_Tang>();
                Session[Sessions.lstCTKM_Tang] = lstCTKM_Tang;


                v_v_dm_ChuongTrinhKhuyenMai dm_ChuongTrinhKhuyenMai = new v_v_dm_ChuongTrinhKhuyenMai();
                apiResponse.Success = true;
                dm_ChuongTrinhKhuyenMai.LOC_ID = Utility.LOC_ID;
                dm_ChuongTrinhKhuyenMai.TUNGAY = Utility.CurrentTime;
                dm_ChuongTrinhKhuyenMai.DENNGAY = Utility.CurrentTime.AddMonths(1);
                dm_ChuongTrinhKhuyenMai.ID = Guid.NewGuid().ToString();
                dm_ChuongTrinhKhuyenMai.lstdm_DonViTinh = new List<v_dm_DonViTinh>();
                dm_ChuongTrinhKhuyenMai.lstdm_DonViTinh = Utility.GetListData<v_dm_DonViTinh>(API.dm_DonViTinh, "", "", Utility.LOC_ID).Data as List<v_dm_DonViTinh>;
                var lst = Utility.ConvertobjectToView<v_v_dm_ChuongTrinhKhuyenMai>(dm_ChuongTrinhKhuyenMai);
                apiResponse.ProductCombo = Utility.GetCTKM_YeuCau();
                lst.Add(new ValueEdit { Key = "tbodyTempItemdivPromotion_YC", Value = apiResponse.ProductCombo });
                apiResponse.ProductCombo = Utility.GetCTKM_Tang();
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
        public ActionResult CreatePopup([Bind(Include = "LOC_ID,ID,MA,NAME,TUNGAY,DENNGAY,CHIETKHAU,TIENGIAM,IS_YEUCAUCHITIET,TONGTIEN_DATKM,SOLUONG_DATKM,ID_DVT_DATKM,ISACTIVE,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO,ISTINHLUYTUYEN,ISTONGHOADON,SOLUONG_DATKM_DEN,TONGTIEN_DATKM_DEN,HINHTHUC_TINHKPI")] v_dm_ChuongTrinhKhuyenMai dm_ChuongTrinhKhuyenMai)
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                if (Utility.KiemTra())
                {
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Admin");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                if (!Utility.KiemTraQuyen(API.dm_ChuongTrinhKhuyenMai, API.Create))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                bool bolAddSuccess = false;
                if (ModelState.IsValid)
                {
                    dm_ChuongTrinhKhuyenMai.LOC_ID = Utility.LOC_ID;
                    dm_ChuongTrinhKhuyenMai.ID_NGUOITAO = Session[Sessions.idUser].ToString();
                    dm_ChuongTrinhKhuyenMai.THOIGIANTHEM = Utility.CurrentTime;
                    dm_ChuongTrinhKhuyenMai.lstdm_ChuongTrinhKhuyenMai_YeuCau = new List<v_dm_ChuongTrinhKhuyenMai_YeuCau>();
                    dm_ChuongTrinhKhuyenMai.lstdm_ChuongTrinhKhuyenMai_Tang = new List<v_dm_ChuongTrinhKhuyenMai_Tang>();

                    var lstKey = Request.Form.AllKeys.Where(e => e.StartsWith("txt") && !e.StartsWith("txtQuantity_Tang") && !e.StartsWith("txtMoney_Tang"));
                    //var lstKey = Request.Form.AllKeys.Where(e => e.StartsWith("txtMoney_YC|"));
                    //var lstKey_YC = Request.Form.AllKeys.Where(e => e.StartsWith("txtQuantity_YC|"));
                    //var lstKeyCHIETKHAU_YC = Request.Form.AllKeys.Where(e => e.StartsWith("txtCHIETKHAU_YC|"));
                    //var lstKeyTIENGIAM_YC = Request.Form.AllKeys.Where(e => e.StartsWith("txtTIENGIAM_YC|"));
                    //var lstKeyISBATBUOC = Request.Form.AllKeys.Where(e => e.StartsWith("txtISBATBUOC|"));
                    //var lstKeySOLUONG_BATBUOC = Request.Form.AllKeys.Where(e => e.StartsWith("txtSOLUONG_BATBUOC|"));
                    //if (lstKey == null || lstKey.Count() == 0 || lstKey_YC == null || lstKey_YC.Count() == 0)
                    if (lstKey == null)
                    {
                        ModelState.AddModelError("lstdm_ChuongTrinhKhuyenMai_YeuCau", "Thêm sản phẩm trong yêu cầu.");
                    }
                    else
                    {
                        dm_ChuongTrinhKhuyenMai.lstdm_ChuongTrinhKhuyenMai_YeuCau = new List<v_dm_ChuongTrinhKhuyenMai_YeuCau>();
                        v_dm_ChuongTrinhKhuyenMai_YeuCau v_dm_ChuongTrinhKhuyenMai_YeuCau = new v_dm_ChuongTrinhKhuyenMai_YeuCau();
                        //int i = 0;
                        foreach (var itm in lstKey)
                        {
                            var lstString = itm.ToString().Split('|');
                            var value = HttpContext.Request.Params.GetValues(itm.ToString());
                            string ShowSearchValue = clsMaHoa.Decrypt(lstString[1].ToString(), clsMaHoa.PassMaHoa);
                            //var value_st = HttpContext.Request.Params.GetValues(itm.ToString());
                            //string ShowSearchValue = clsMaHoa.Decrypt(lstString[1].ToString(), clsMaHoa.PassMaHoa);
                            //var value_sl = HttpContext.Request.Params.GetValues(lstKey_YC.ToList()[i].ToString());
                            //var value_ck = HttpContext.Request.Params.GetValues(lstKeyCHIETKHAU_YC.ToList()[i].ToString());
                            //var value_tg = HttpContext.Request.Params.GetValues(lstKeyTIENGIAM_YC.ToList()[i].ToString());
                            //var value_sl_bb = HttpContext.Request.Params.GetValues(lstKeySOLUONG_BATBUOC.ToList()[i].ToString());
                            //var value_isbatbuoc = HttpContext.Request.Params.GetValues(lstKeyISBATBUOC.ToList()[i].ToString());
                            var dm_ChuongTrinhKhuyenMai_YeuCau = JsonConvert.DeserializeObject<v_dm_ChuongTrinhKhuyenMai_YeuCau>(ShowSearchValue);
                            if (v_dm_ChuongTrinhKhuyenMai_YeuCau.ID != dm_ChuongTrinhKhuyenMai_YeuCau.ID)
                            {
                                v_dm_ChuongTrinhKhuyenMai_YeuCau = new v_dm_ChuongTrinhKhuyenMai_YeuCau();
                                v_dm_ChuongTrinhKhuyenMai_YeuCau = JsonConvert.DeserializeObject<v_dm_ChuongTrinhKhuyenMai_YeuCau>(ShowSearchValue);
                                v_dm_ChuongTrinhKhuyenMai_YeuCau.ISBATBUOC = false;
                                v_dm_ChuongTrinhKhuyenMai_YeuCau.ID_CHUONGTRINHKHUYENMAI = dm_ChuongTrinhKhuyenMai.ID;
                                v_dm_ChuongTrinhKhuyenMai_YeuCau.LOC_ID = dm_ChuongTrinhKhuyenMai.LOC_ID;
                                dm_ChuongTrinhKhuyenMai.lstdm_ChuongTrinhKhuyenMai_YeuCau.Add(v_dm_ChuongTrinhKhuyenMai_YeuCau);
                            }
                            string TYPE = lstString[0].ToString().Substring(3, lstString[0].ToString().Length - 3);
                            if (TYPE == "Money_YC")
                                TYPE = "SOTIEN";
                            if (TYPE == "Quantity_YC")
                                TYPE = "SOLUONG";
                            if (TYPE == "CHIETKHAU_YC")
                                TYPE = "CHIETKHAU";
                            if (TYPE == "TIENGIAM_YC")
                                TYPE = "TIENGIAM";
                            Utility.EditObject(v_dm_ChuongTrinhKhuyenMai_YeuCau, TYPE, value[0]);
                        //    int i = 0;
                        //foreach (var itm in lstKey)
                        //{
                        //    var lstString = itm.ToString().Split('|');
                        //    //var value_st = HttpContext.Request.Params.GetValues(itm.ToString());
                        //    //var value_sl = HttpContext.Request.Params.GetValues(lstKey_YC.ToList()[i].ToString());
                        //    //var value_ck = HttpContext.Request.Params.GetValues(lstKeyCHIETKHAU_YC.ToList()[i].ToString());
                        //    //var value_tg = HttpContext.Request.Params.GetValues(lstKeyTIENGIAM_YC.ToList()[i].ToString());
                        //    //var value_sl_bb = HttpContext.Request.Params.GetValues(lstKeySOLUONG_BATBUOC.ToList()[i].ToString());
                        //    //var value_isbatbuoc = HttpContext.Request.Params.GetValues(lstKeyISBATBUOC.ToList()[i].ToString());
                        //    //string ShowSearchValue = clsMaHoa.Decrypt(lstString[1].ToString(), clsMaHoa.PassMaHoa);
                        //    var dm_ChuongTrinhKhuyenMai_YeuCau = JsonConvert.DeserializeObject<v_dm_ChuongTrinhKhuyenMai_YeuCau>(ShowSearchValue);
                        //    if (lstString != null)
                        //    {
                        //        if(string.IsNullOrEmpty(dm_ChuongTrinhKhuyenMai_YeuCau.ID))
                        //            dm_ChuongTrinhKhuyenMai_YeuCau.ID = Guid.NewGuid().ToString();
                        //        dm_ChuongTrinhKhuyenMai_YeuCau.LOC_ID = Utility.LOC_ID;
                        //        dm_ChuongTrinhKhuyenMai_YeuCau.ID_CHUONGTRINHKHUYENMAI = dm_ChuongTrinhKhuyenMai.ID;
                        //        dm_ChuongTrinhKhuyenMai_YeuCau.SOTIEN = Utility.ConvertStringToDouble(value_st[0]);
                        //        dm_ChuongTrinhKhuyenMai_YeuCau.SOLUONG = Utility.ConvertStringToDouble(value_sl[0]);
                        //        dm_ChuongTrinhKhuyenMai_YeuCau.CHIETKHAU = Utility.ConvertStringToDouble(value_ck[0]);
                        //        dm_ChuongTrinhKhuyenMai_YeuCau.TIENGIAM = Utility.ConvertStringToDouble(value_tg[0]);
                        //        dm_ChuongTrinhKhuyenMai_YeuCau.ISBATBUOC = value_isbatbuoc[0].ToString() == "on" ? true : false;
                        //        dm_ChuongTrinhKhuyenMai_YeuCau.SOLUONG_BATBUOC = Utility.ConvertStringToDouble(value_sl_bb[0]);
                        //        dm_ChuongTrinhKhuyenMai.lstdm_ChuongTrinhKhuyenMai_YeuCau.Add(dm_ChuongTrinhKhuyenMai_YeuCau);
                        //    }

                        //    i += 1;
                        }
                    }    

                    var lstKey_Tang = Request.Form.AllKeys.Where(e => e.StartsWith("txtQuantity_Tang|"));
                    var lstKeyMoney_Tang = Request.Form.AllKeys.Where(e => e.StartsWith("txtMoney_Tang|"));
                    if (lstKey_Tang == null || lstKey_Tang.Count() == 0 || lstKeyMoney_Tang == null || lstKeyMoney_Tang.Count() == 0)
                    {
                        //ModelState.AddModelError("lstdm_ChuongTrinhKhuyenMai_Tang", "Thêm sản phẩm trong .");
                    }
                    else
                    {
                        int i = 0;
                        foreach (var itm in lstKey_Tang)
                        {
                            var lstString = itm.ToString().Split('|');
                            var value = HttpContext.Request.Params.GetValues(itm.ToString());
                            var value_sl = HttpContext.Request.Params.GetValues(lstKeyMoney_Tang.ToList()[i].ToString());
                            string ShowSearchValue = clsMaHoa.Decrypt(lstString[1].ToString(), clsMaHoa.PassMaHoa);
                            var dm_ChuongTrinhKhuyenMai_Tang = JsonConvert.DeserializeObject<v_dm_ChuongTrinhKhuyenMai_Tang>(ShowSearchValue);
                            if (lstString != null)
                            {
                                if (string.IsNullOrEmpty(dm_ChuongTrinhKhuyenMai_Tang.ID))
                                    dm_ChuongTrinhKhuyenMai_Tang.ID = Guid.NewGuid().ToString();
                                dm_ChuongTrinhKhuyenMai_Tang.LOC_ID = Utility.LOC_ID;
                                dm_ChuongTrinhKhuyenMai_Tang.ID_CHUONGTRINHKHUYENMAI = dm_ChuongTrinhKhuyenMai.ID;
                                dm_ChuongTrinhKhuyenMai_Tang.SOLUONG = Utility.ConvertStringToDouble(value[0]);
                                dm_ChuongTrinhKhuyenMai_Tang.SOTIEN = Utility.ConvertStringToDouble(value_sl[0]);
                                dm_ChuongTrinhKhuyenMai.lstdm_ChuongTrinhKhuyenMai_Tang.Add(dm_ChuongTrinhKhuyenMai_Tang);
                            }
                            i += 1;
                        }
                    }

                    apiResponse = Utility.Create<v_dm_ChuongTrinhKhuyenMai>(dm_ChuongTrinhKhuyenMai, API.dm_ChuongTrinhKhuyenMai);
                    if (apiResponse.Success)
                    {
                        if (apiResponse.Data != null)
                            dm_ChuongTrinhKhuyenMai = JsonConvert.DeserializeObject<v_dm_ChuongTrinhKhuyenMai>(apiResponse.Data.ToString());
                        apiResponse.NewID = Guid.NewGuid().ToString();

                        List<v_dm_ChuongTrinhKhuyenMai_YeuCau> lstCTKM_YC = new List<v_dm_ChuongTrinhKhuyenMai_YeuCau>();
                        Session[Sessions.lstCTKM_YeuCau] = lstCTKM_YC;

                        List<v_dm_ChuongTrinhKhuyenMai_Tang> lstCTKM_Tang = new List<v_dm_ChuongTrinhKhuyenMai_Tang>();
                        Session[Sessions.lstCTKM_Tang] = lstCTKM_Tang;

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
                    apiResponse.Data = Utility.GetModelState(ModelState, API.dm_ChuongTrinhKhuyenMai);
                }
                apiResponse.ID = dm_ChuongTrinhKhuyenMai.ID;
                var lst = Utility.ConvertobjectToView<dm_ChuongTrinhKhuyenMai>(dm_ChuongTrinhKhuyenMai);
                if (bolAddSuccess)
                {
                    apiResponse.ProductCombo = Utility.GetCTKM_YeuCau();
                    lst.Add(new ValueEdit { Key = "tbodyTempItemdivPromotion_YC", Value = apiResponse.ProductCombo });
                    apiResponse.ProductCombo = Utility.GetCTKM_Tang();
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
                if (!Utility.KiemTraQuyen(API.dm_ChuongTrinhKhuyenMai, API.Edit))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                v_v_dm_ChuongTrinhKhuyenMai dm_ChuongTrinhKhuyenMai = new v_v_dm_ChuongTrinhKhuyenMai();
                if (!string.IsNullOrEmpty(id))
                {
                    apiResponse = Utility.GetDetail<v_v_dm_ChuongTrinhKhuyenMai>(Utility.LOC_ID + "/" + id, API.dm_ChuongTrinhKhuyenMai);
                    if (!apiResponse.Success)
                    {
                        TempData["TitleError"] = apiResponse.Message;
                        apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                        return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                    }
                    if (apiResponse.Data != null)
                        dm_ChuongTrinhKhuyenMai = apiResponse.Data as v_v_dm_ChuongTrinhKhuyenMai;
                }
                apiResponse.Success = true;
                apiResponse.Success = true;
                dm_ChuongTrinhKhuyenMai.lstdm_DonViTinh = new List<v_dm_DonViTinh>();
                dm_ChuongTrinhKhuyenMai.lstdm_DonViTinh = Utility.GetListData<v_dm_DonViTinh>(API.dm_DonViTinh, "", "", Utility.LOC_ID).Data as List<v_dm_DonViTinh>;
                List<v_dm_ChuongTrinhKhuyenMai_YeuCau> lstCTKM_YC = new List<v_dm_ChuongTrinhKhuyenMai_YeuCau>();
                foreach (var itm in dm_ChuongTrinhKhuyenMai.lstdm_ChuongTrinhKhuyenMai_YeuCau)
                {
                    lstCTKM_YC.Add(itm);
                }
                Session[Sessions.lstCTKM_YeuCau] = lstCTKM_YC;

                List<v_dm_ChuongTrinhKhuyenMai_Tang> lstCTKM_Tang = new List<v_dm_ChuongTrinhKhuyenMai_Tang>();
                foreach (var itm in dm_ChuongTrinhKhuyenMai.lstdm_ChuongTrinhKhuyenMai_Tang)
                {
                    lstCTKM_Tang.Add(itm);
                }
                Session[Sessions.lstCTKM_Tang] = lstCTKM_Tang;

                var lst = Utility.ConvertobjectTo<v_v_dm_ChuongTrinhKhuyenMai>(dm_ChuongTrinhKhuyenMai);
                apiResponse.ProductCombo = Utility.GetCTKM_YeuCau();
                lst.Add(new ValueEdit { Key = "tbodyTempItemdivPromotion_YCEdit", Value = apiResponse.ProductCombo });
                apiResponse.ProductCombo = Utility.GetCTKM_Tang();
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
        public ActionResult EditPopup([Bind(Include = "LOC_ID,ID,MA,NAME,TUNGAY,DENNGAY,CHIETKHAU,TIENGIAM,IS_YEUCAUCHITIET,TONGTIEN_DATKM,SOLUONG_DATKM,ID_DVT_DATKM,ISACTIVE,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO,ISTINHLUYTUYEN,ISTONGHOADON,SOLUONG_DATKM_DEN,TONGTIEN_DATKM_DEN,HINHTHUC_TINHKPI")] v_dm_ChuongTrinhKhuyenMai dm_ChuongTrinhKhuyenMai)
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                if (Utility.KiemTra())
                {
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Admin");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                if (!Utility.KiemTraQuyen(API.dm_ChuongTrinhKhuyenMai, API.Edit))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                if (ModelState.IsValid)
                {
                    dm_ChuongTrinhKhuyenMai.LOC_ID = Utility.LOC_ID;
                    dm_ChuongTrinhKhuyenMai.ID_NGUOISUA = Session[Sessions.idUser].ToString();
                    dm_ChuongTrinhKhuyenMai.THOIGIANSUA = Utility.CurrentTime;
                    dm_ChuongTrinhKhuyenMai.lstdm_ChuongTrinhKhuyenMai_YeuCau = new List<v_dm_ChuongTrinhKhuyenMai_YeuCau>();
                    dm_ChuongTrinhKhuyenMai.lstdm_ChuongTrinhKhuyenMai_Tang = new List<v_dm_ChuongTrinhKhuyenMai_Tang>();
                    var lstKey = Request.Form.AllKeys.Where(e => e.StartsWith("txt") && !e.StartsWith("txtQuantity_Tang") && !e.StartsWith("txtMoney_Tang"));
                    //var lstKey = Request.Form.AllKeys.Where(e => e.StartsWith("txtMoney_YC|"));
                    //var lstKey_YC = Request.Form.AllKeys.Where(e => e.StartsWith("txtQuantity_YC|"));
                    //var lstKeyCHIETKHAU_YC = Request.Form.AllKeys.Where(e => e.StartsWith("txtCHIETKHAU_YC|"));
                    //var lstKeyTIENGIAM_YC = Request.Form.AllKeys.Where(e => e.StartsWith("txtTIENGIAM_YC|"));
                    //var lstKeyISBATBUOC = Request.Form.AllKeys.Where(e => e.StartsWith("txtISBATBUOC|"));
                    //var lstKeySOLUONG_BATBUOC = Request.Form.AllKeys.Where(e => e.StartsWith("txtSOLUONG_BATBUOC|"));
                    if (lstKey == null)
                    {
                        ModelState.AddModelError("lstdm_ChuongTrinhKhuyenMai_YeuCau", "Thêm sản phẩm trong yêu cầu.");
                    }
                    else
                    {
                        dm_ChuongTrinhKhuyenMai.lstdm_ChuongTrinhKhuyenMai_YeuCau = new List<v_dm_ChuongTrinhKhuyenMai_YeuCau>();
                        v_dm_ChuongTrinhKhuyenMai_YeuCau v_dm_ChuongTrinhKhuyenMai_YeuCau = new v_dm_ChuongTrinhKhuyenMai_YeuCau();
                        //int i = 0;
                        foreach (var itm in lstKey)
                        {
                            var lstString = itm.ToString().Split('|');
                            var value = HttpContext.Request.Params.GetValues(itm.ToString());
                            string ShowSearchValue = clsMaHoa.Decrypt(lstString[1].ToString(), clsMaHoa.PassMaHoa);
                            //var value_st = HttpContext.Request.Params.GetValues(itm.ToString());
                            //string ShowSearchValue = clsMaHoa.Decrypt(lstString[1].ToString(), clsMaHoa.PassMaHoa);
                            //var value_sl = HttpContext.Request.Params.GetValues(lstKey_YC.ToList()[i].ToString());
                            //var value_ck = HttpContext.Request.Params.GetValues(lstKeyCHIETKHAU_YC.ToList()[i].ToString());
                            //var value_tg = HttpContext.Request.Params.GetValues(lstKeyTIENGIAM_YC.ToList()[i].ToString());
                            //var value_sl_bb = HttpContext.Request.Params.GetValues(lstKeySOLUONG_BATBUOC.ToList()[i].ToString());
                            //var value_isbatbuoc = HttpContext.Request.Params.GetValues(lstKeyISBATBUOC.ToList()[i].ToString());
                            var dm_ChuongTrinhKhuyenMai_YeuCau = JsonConvert.DeserializeObject<v_dm_ChuongTrinhKhuyenMai_YeuCau>(ShowSearchValue);
                            if (v_dm_ChuongTrinhKhuyenMai_YeuCau.ID != dm_ChuongTrinhKhuyenMai_YeuCau.ID)
                            {
                                v_dm_ChuongTrinhKhuyenMai_YeuCau = new v_dm_ChuongTrinhKhuyenMai_YeuCau();
                                v_dm_ChuongTrinhKhuyenMai_YeuCau = JsonConvert.DeserializeObject<v_dm_ChuongTrinhKhuyenMai_YeuCau>(ShowSearchValue);
                                v_dm_ChuongTrinhKhuyenMai_YeuCau.ISBATBUOC = false;
                                v_dm_ChuongTrinhKhuyenMai_YeuCau.ID_CHUONGTRINHKHUYENMAI = dm_ChuongTrinhKhuyenMai.ID;
                                v_dm_ChuongTrinhKhuyenMai_YeuCau.LOC_ID = dm_ChuongTrinhKhuyenMai.LOC_ID;
                                dm_ChuongTrinhKhuyenMai.lstdm_ChuongTrinhKhuyenMai_YeuCau.Add(v_dm_ChuongTrinhKhuyenMai_YeuCau);
                            }
                            string TYPE = lstString[0].ToString().Substring(3, lstString[0].ToString().Length - 3);
                            if (TYPE == "Money_YC")
                                TYPE = "SOTIEN";
                            if (TYPE == "Quantity_YC")
                                TYPE = "SOLUONG";
                            if (TYPE == "CHIETKHAU_YC")
                                TYPE = "CHIETKHAU";
                            if (TYPE == "TIENGIAM_YC")
                                TYPE = "TIENGIAM";
                            Utility.EditObject(v_dm_ChuongTrinhKhuyenMai_YeuCau, TYPE, value[0]);
                            //if (lstString != null)
                            //{
                            //    if (string.IsNullOrEmpty(dm_ChuongTrinhKhuyenMai_YeuCau.ID))
                            //        dm_ChuongTrinhKhuyenMai_YeuCau.ID = Guid.NewGuid().ToString();
                            //    dm_ChuongTrinhKhuyenMai_YeuCau.LOC_ID = Utility.LOC_ID;
                            //    dm_ChuongTrinhKhuyenMai_YeuCau.ID_CHUONGTRINHKHUYENMAI = dm_ChuongTrinhKhuyenMai.ID;
                            //    dm_ChuongTrinhKhuyenMai_YeuCau.SOLUONG = Utility.ConvertStringToDouble(value_sl[0]);
                            //    dm_ChuongTrinhKhuyenMai_YeuCau.SOTIEN = Utility.ConvertStringToDouble(value_st[0]);
                            //    dm_ChuongTrinhKhuyenMai_YeuCau.CHIETKHAU = Utility.ConvertStringToDouble(value_ck[0]);
                            //    dm_ChuongTrinhKhuyenMai_YeuCau.TIENGIAM = Utility.ConvertStringToDouble(value_tg[0]);
                            //    if (value_isbatbuoc != null && value_isbatbuoc.Count() > 0)
                            //        dm_ChuongTrinhKhuyenMai_YeuCau.ISBATBUOC = value_isbatbuoc[0].ToString() == "on" ? true : false;
                            //    else
                            //        dm_ChuongTrinhKhuyenMai_YeuCau.ISBATBUOC = false;
                            //    dm_ChuongTrinhKhuyenMai_YeuCau.SOLUONG_BATBUOC = Utility.ConvertStringToDouble(value_sl_bb[0]);
                            //    dm_ChuongTrinhKhuyenMai.lstdm_ChuongTrinhKhuyenMai_YeuCau.Add(dm_ChuongTrinhKhuyenMai_YeuCau);
                            //}

                            //i += 1;
                        }
                    }

                    var lstKey_Tang = Request.Form.AllKeys.Where(e => e.StartsWith("txtQuantity_Tang|"));
                    var lstKeyMoney_Tang = Request.Form.AllKeys.Where(e => e.StartsWith("txtMoney_Tang|"));
                    if (lstKey_Tang == null || lstKey_Tang.Count() == 0)
                    {
                        //ModelState.AddModelError("lstdm_ChuongTrinhKhuyenMai_Tang", "Thêm sản phẩm trong .");
                    }
                    else
                    {
                        int i = 0;
                        foreach (var itm in lstKey_Tang)
                        {
                            var lstString = itm.ToString().Split('|');
                            var value = HttpContext.Request.Params.GetValues(itm.ToString());
                            var value_st = HttpContext.Request.Params.GetValues(lstKeyMoney_Tang.ToList()[i].ToString());
                            string ShowSearchValue = clsMaHoa.Decrypt(lstString[1].ToString(), clsMaHoa.PassMaHoa);
                            var dm_ChuongTrinhKhuyenMai_Tang = JsonConvert.DeserializeObject<v_dm_ChuongTrinhKhuyenMai_Tang>(ShowSearchValue);
                            if (lstString != null)
                            {
                                if (string.IsNullOrEmpty(dm_ChuongTrinhKhuyenMai_Tang.ID))
                                    dm_ChuongTrinhKhuyenMai_Tang.ID = Guid.NewGuid().ToString();
                                dm_ChuongTrinhKhuyenMai_Tang.LOC_ID = Utility.LOC_ID;
                                dm_ChuongTrinhKhuyenMai_Tang.ID_CHUONGTRINHKHUYENMAI = dm_ChuongTrinhKhuyenMai.ID;
                                dm_ChuongTrinhKhuyenMai_Tang.SOLUONG = Utility.ConvertStringToDouble(value[0]);
                                dm_ChuongTrinhKhuyenMai_Tang.SOTIEN = Utility.ConvertStringToDouble(value_st[0]);
                                dm_ChuongTrinhKhuyenMai.lstdm_ChuongTrinhKhuyenMai_Tang.Add(dm_ChuongTrinhKhuyenMai_Tang);
                            }
                            i += 1;
                        }
                    }
                    apiResponse = Utility.Edit<v_dm_ChuongTrinhKhuyenMai>(Utility.LOC_ID + "/" + dm_ChuongTrinhKhuyenMai.MA, dm_ChuongTrinhKhuyenMai, API.dm_ChuongTrinhKhuyenMai);
                    if (apiResponse.Success)
                    {
                        //return RedirectToAction("Index");
                        if (apiResponse.Data != null)
                            dm_ChuongTrinhKhuyenMai = JsonConvert.DeserializeObject<v_dm_ChuongTrinhKhuyenMai>(apiResponse.Data.ToString());
                        apiResponse.ID = dm_ChuongTrinhKhuyenMai.ID;
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, apiResponse.Message);
                    }
                }
                else
                {
                    apiResponse.Success = false;
                    apiResponse.Data = Utility.GetModelState(ModelState, API.dm_ChuongTrinhKhuyenMai);
                }
                apiResponse.Detail = Utility.ConvertobjectToView<dm_ChuongTrinhKhuyenMai>(dm_ChuongTrinhKhuyenMai);
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
                if (!Utility.KiemTraQuyen(API.dm_ChuongTrinhKhuyenMai, API.Delete))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                apiResponse = Utility.Delete<v_dm_ChuongTrinhKhuyenMai>(Utility.LOC_ID + "/" + id, API.dm_ChuongTrinhKhuyenMai);
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
        public ActionResult AddProductPromotion_YC([Bind(Include = "ID_HANGHOA,SOLUONG,ID_DVT")] v_v_dm_ChuongTrinhKhuyenMai_YeuCau dm_HangHoa_Combo)
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
                dm_HangHoa_Combo.ID = Guid.NewGuid().ToString();
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
                var check = Utility.LstCTKM_YeuCau.Where(e => e.ID_HANGHOA == dm_HangHoa_Combo.ID_HANGHOA && e.ID_DVT == dm_HangHoa_Combo.ID_DVT).FirstOrDefault();
                if (check == null)
                {
                    var LstCTKM_YeuCau = Utility.LstCTKM_YeuCau;
                    LstCTKM_YeuCau.Add(dm_HangHoa_Combo);
                    Session[Sessions.lstCTKM_YeuCau] = LstCTKM_YeuCau;
                }
                else
                {
                    check.SOLUONG = dm_HangHoa_Combo.SOLUONG;
                    check.SOTIEN = dm_HangHoa_Combo.SOTIEN;
                }
            }
            apiResponse.ProductCombo = Utility.GetCTKM_YeuCau();
            return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult AddProductPromotionNHH_YC([Bind(Include = "ID_HANGHOA,SOLUONG,ID_DVT")] v_v_dm_ChuongTrinhKhuyenMai_YeuCau dm_CTKM_YC)
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
                dm_CTKM_YC.ID = Guid.NewGuid().ToString();
                dm_CTKM_YC.HINHTHUC = (int)HinhThucKhuyenMai.NhomSanPham;
                dm_CTKM_YC.ID_HANGHOA = dm_NhomHangHoa.ID;
                dm_CTKM_YC.NAME = dm_NhomHangHoa.NAME;
                dm_CTKM_YC.MA = dm_NhomHangHoa.MA;
                dm_CTKM_YC.ID_DVT = dm_CTKM_YC.ID_DVT;
                dm_CTKM_YC.NAME_DVT = dm_DonViTinh.NAME;
                var check = Utility.LstCTKM_YeuCau.Where(e => e.ID_HANGHOA == dm_CTKM_YC.ID_HANGHOA && e.ID_DVT == dm_CTKM_YC.ID_DVT).FirstOrDefault();
                if (check == null)
                {
                    var LstCTKM_YeuCau = Utility.LstCTKM_YeuCau;
                    LstCTKM_YeuCau.Add(dm_CTKM_YC);
                    Session[Sessions.lstCTKM_YeuCau] = LstCTKM_YeuCau;
                }
                else
                {
                    check.SOLUONG = dm_CTKM_YC.SOLUONG;
                    check.SOTIEN = dm_CTKM_YC.SOTIEN;
                }
            }
            apiResponse.ProductCombo = Utility.GetCTKM_YeuCau();
            return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
        }

        [HttpPost]
        public ActionResult DeleteProductPromotion_YC(string ID_HANGHOA, string ID_DVT)
        {
            ApiResponse apiResponse = new ApiResponse();
            v_v_dm_HangHoa dm_HangHoa = new v_v_dm_HangHoa();
            var LstCTKM_YeuCau = Utility.LstCTKM_YeuCau;
            var check = Utility.LstCTKM_YeuCau.Where(e => e.ID_HANGHOA == ID_HANGHOA && e.ID_DVT == ID_DVT).FirstOrDefault();
            if (check != null)
                LstCTKM_YeuCau.Remove(check);

            Session[Sessions.lstCTKM_YeuCau] = LstCTKM_YeuCau;
            apiResponse.ProductCombo = Utility.GetCTKM_YeuCau();
            apiResponse.Success = true;
            return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
        }


        [HttpPost, ValidateInput(false)]
        public ActionResult AddProductPromotion_Tang([Bind(Include = "ID_HANGHOA,SOLUONG,ID_DVT")] v_v_dm_ChuongTrinhKhuyenMai_Tang dm_HangHoa_Combo)
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
                dm_HangHoa_Combo.NAME = dm_HangHoa.NAME;
                dm_HangHoa_Combo.MA = dm_HangHoa.MA;
                if (dm_HangHoa.ID_DVT == dm_HangHoa_Combo.ID_DVT)
                {
                    dm_HangHoa_Combo.NAME_DVT = dm_HangHoa.NAME_DVT;
                    if (!string.IsNullOrEmpty(dm_HangHoa.ID_DVT_QD))
                    {
                        dm_HangHoa_Combo.TYLE_QD = dm_HangHoa.TYLE_QD;
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

                var check = Utility.LstCTKM_Tang.Where(e => e.ID_HANGHOA == dm_HangHoa_Combo.ID_HANGHOA && e.ID_DVT == dm_HangHoa_Combo.ID_DVT).FirstOrDefault();
                if (check == null)
                {
                    var LstCTKM_Tang = Utility.LstCTKM_Tang;
                    LstCTKM_Tang.Add(dm_HangHoa_Combo);
                    Session[Sessions.lstCTKM_Tang] = LstCTKM_Tang;
                }
                else
                {
                    check.SOLUONG = dm_HangHoa_Combo.SOLUONG;
                }
            }
            apiResponse.ProductCombo = Utility.GetCTKM_Tang();
            return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
        }

        [HttpPost]
        public ActionResult DeleteProductPromotion_Tang(string ID_HANGHOA, string ID_DVT)
        {
            ApiResponse apiResponse = new ApiResponse();
            v_v_dm_HangHoa dm_HangHoa = new v_v_dm_HangHoa();
            var LstCTKM_Tang = Utility.LstCTKM_Tang;
            var check = Utility.LstCTKM_Tang.Where(e => e.ID_HANGHOA == ID_HANGHOA && e.ID_DVT == ID_DVT).FirstOrDefault();
            if (check != null)
                LstCTKM_Tang.Remove(check);

            Session[Sessions.lstCTKM_Tang] = LstCTKM_Tang;
            apiResponse.ProductCombo = Utility.GetCTKM_Tang();
            apiResponse.Success = true;
            return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
        }
    }
}