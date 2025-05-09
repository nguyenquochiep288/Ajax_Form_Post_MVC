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
using System.IO;
using Newtonsoft.Json;
using System.Data.SqlClient;
using DatabaseTHP.StoredProcedure;
using DatabaseTHP.StoredProcedure.Parameter;

namespace MVC_QuanLyTHP.Controllers
{
    public class ProductController : Controller
    {

        // GET: Product
        public ActionResult Index(int Page = 1, string SearchString = "", string ShowSearchValue = "")
        {
            try
            {
                if (Utility.KiemTra())
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (!Utility.KiemTraQuyen(API.dm_HangHoa, API.Xem))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                ShowSearchValue = Utility.GetShowSearchValue<dm_HangHoa>(ShowSearchValue);
                var apiResponse = Utility.GetListData<v_dm_HangHoa>(API.dm_HangHoa, ShowSearchValue, SearchString, Utility.LOC_ID);
                if (!apiResponse.Success)
                {
                    TempData["TitleError"] = apiResponse.Message;
                    return RedirectToAction("Index", "Notfound");
                }
                IPagedList<v_dm_HangHoa> lstpage = (apiResponse.Data as List<v_dm_HangHoa>).ToPagedList(Page, Utility.GetPageSize());

                v_v_dm_HangHoa dm_HangHoa = new v_v_dm_HangHoa();
                dm_HangHoa.IPagedList = lstpage;
                dm_HangHoa.lstdm_DonViTinh = new List<v_dm_DonViTinh>();
                dm_HangHoa.lstdm_DonViTinh = Utility.GetListData<v_dm_DonViTinh>(API.dm_DonViTinh, "", "", Utility.LOC_ID).Data as List<v_dm_DonViTinh>;
                dm_HangHoa.lstdm_DonViTinh_QD = new List<v_dm_DonViTinh>();
                dm_HangHoa.lstdm_DonViTinh_QD = Utility.GetListData<v_dm_DonViTinh>(API.dm_DonViTinh, "", "", Utility.LOC_ID).Data as List<v_dm_DonViTinh>;
                dm_HangHoa.lstdm_NhaCungCap = new List<v_dm_NhaCungCap>();
                dm_HangHoa.lstdm_NhaCungCap = Utility.GetListData<v_dm_NhaCungCap>(API.dm_NhaCungCap, "", "", Utility.LOC_ID).Data as List<v_dm_NhaCungCap>;
                dm_HangHoa.lstdm_NhomHangHoa = new List<v_dm_NhomHangHoa>();
                dm_HangHoa.lstdm_NhomHangHoa = Utility.GetListData<v_dm_NhomHangHoa>(API.dm_NhomHangHoa, "", "", Utility.LOC_ID).Data as List<v_dm_NhomHangHoa>;
                dm_HangHoa.lstdm_ThueSuat = new List<v_dm_ThueSuat>();
                dm_HangHoa.lstdm_ThueSuat = Utility.GetListData<v_dm_ThueSuat>(API.dm_ThueSuat, "", "", Utility.LOC_ID).Data as List<v_dm_ThueSuat>;

                ViewBag.searchValue = SearchString;
                ViewBag.showsearchValue = ShowSearchValue;

                ViewBag.PermissionEdit = Utility.KiemTraQuyen(API.dm_HangHoa, API.Edit);
                ViewBag.PermissionDelete = Utility.KiemTraQuyen(API.dm_HangHoa, API.Delete);
                ViewBag.PermissionCreate = Utility.KiemTraQuyen(API.dm_HangHoa, API.Create);
                return View(dm_HangHoa);
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
                TempData["TitleError"] = API.TitleTryCatch;
                TempData["DetailError"] = ex.Message;
                return RedirectToAction("Index", "Notfound");
            }
        }

        // GET: Product/Create
        public ActionResult Create(int type = 2)
        {
            try
            {
                Session[Sessions.IntWidth] = type;
                if (Utility.KiemTra())
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (!Utility.KiemTraQuyen(API.dm_HangHoa, API.Create))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                v_v_dm_HangHoa dm_HangHoa = new v_v_dm_HangHoa();
                dm_HangHoa.LOC_ID = Utility.LOC_ID;
                dm_HangHoa.ID_NGUOITAO = Session[Sessions.idUser].ToString();
                dm_HangHoa.THOIGIANTHEM = Utility.CurrentTime;

                dm_HangHoa.ID = Guid.NewGuid().ToString();
                dm_HangHoa.lstdm_DonViTinh = new List<v_dm_DonViTinh>();
                dm_HangHoa.lstdm_DonViTinh = Utility.GetListData<v_dm_DonViTinh>(API.dm_DonViTinh, "", "", Utility.LOC_ID).Data as List<v_dm_DonViTinh>;
                dm_HangHoa.lstdm_DonViTinh_QD = new List<v_dm_DonViTinh>();
                dm_HangHoa.lstdm_DonViTinh_QD = Utility.GetListData<v_dm_DonViTinh>(API.dm_DonViTinh, "", "", Utility.LOC_ID).Data as List<v_dm_DonViTinh>;
                dm_HangHoa.lstdm_NhaCungCap = new List<v_dm_NhaCungCap>();
                dm_HangHoa.lstdm_NhaCungCap = Utility.GetListData<v_dm_NhaCungCap>(API.dm_NhaCungCap, "", "", Utility.LOC_ID).Data as List<v_dm_NhaCungCap>;
                dm_HangHoa.lstdm_NhomHangHoa = new List<v_dm_NhomHangHoa>();
                dm_HangHoa.lstdm_NhomHangHoa = Utility.GetListData<v_dm_NhomHangHoa>(API.dm_NhomHangHoa, "", "", Utility.LOC_ID).Data as List<v_dm_NhomHangHoa>;
                dm_HangHoa.lstdm_ThueSuat = new List<v_dm_ThueSuat>();
                dm_HangHoa.lstdm_ThueSuat = Utility.GetListData<v_dm_ThueSuat>(API.dm_ThueSuat, "", "", Utility.LOC_ID).Data as List<v_dm_ThueSuat>;
                return View(dm_HangHoa);
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
                TempData["TitleError"] = API.TitleTryCatch;
                TempData["DetailError"] = ex.Message;
                return RedirectToAction("Index", "Notfound");
            }
        }

        // POST: Product/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ISKHONGHIENTHITONKHO,LOC_ID,ID,BARCODE,MA,NAME,PICTURE,GIA01,GIA02,GIA03,GIA01_QD,GIA02_QD,ID_NHOMHANGHOA,ISACTIVE,LOAIHANGHOA,ISCOMBO,ID_DVT,STATUS_QD,ID_DVT_QD,TYLE_QD,TRONGLUONG,STATUS_HIENTHI,ID_NCC,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO,BAOGOMTHUESUAT,ID_THUESUAT,GIA03_QD")] v_v_dm_HangHoa dm_HangHoa)
        {
            try
            {
                if (Utility.KiemTra())
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (!Utility.KiemTraQuyen(API.dm_HangHoa, API.Create))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                if (ModelState.IsValid)
                {
                    dm_HangHoa.LOC_ID = Utility.LOC_ID;
                    dm_HangHoa.ID_NGUOITAO = Session[Sessions.idUser].ToString();
                    dm_HangHoa.THOIGIANTHEM = Utility.CurrentTime;

                    if (Request.Files["MaHinh"] != null)//Nếu có uploads
                    {
                        String fulName = Request.Files["MaHinh"].FileName;
                        if (fulName != "")
                        {
                            String Name = Guid.NewGuid().ToString() + fulName.Split('.')[1];
                            String fullpath = Path.Combine(Server.MapPath("~"+ API.PathProduct), Name);
                            Request.Files["MaHinh"].SaveAs(fullpath);

                            dm_HangHoa.PICTURE = Name;//cập nhật tên file ảnh
                            Byte[] AsBytes = System.IO.File.ReadAllBytes(fullpath);
                            String AsBase64String = Convert.ToBase64String(AsBytes);
                            dm_HangHoa.FILEBASE64 = AsBase64String;
                        }
                    }

                    //@ConvertObjectUnicodeToTCVN3
                    var apiResponse = Utility.Create<dm_HangHoa>(dm_HangHoa, API.dm_HangHoa);
                    if (apiResponse.Success)
                        return RedirectToAction("Index");
                    else
                    {
                        ModelState.AddModelError(string.Empty, apiResponse.Message);
                        if (apiResponse.CheckValue)
                            ViewBag.ID = Guid.NewGuid().ToString();
                    }
                }
                return View(dm_HangHoa);
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
                TempData["TitleError"] = API.TitleTryCatch;
                TempData["DetailError"] = ex.Message;
                return RedirectToAction("Index", "Notfound");
            }
        }

        // GET: Product/Edit/5
        public ActionResult Edit(string id = "", int type = 2)
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                Session[Sessions.IntWidth] = type;
                if (Utility.KiemTra())
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (!Utility.KiemTraQuyen(API.dm_HangHoa, API.Edit))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                v_v_dm_HangHoa dm_HangHoa = new v_v_dm_HangHoa();
                if (!string.IsNullOrEmpty(id))
                {
                    apiResponse = Utility.GetDetail<v_v_dm_HangHoa>(Utility.LOC_ID + "/" + id, API.dm_HangHoa);
                    if (!apiResponse.Success)
                    {
                        TempData["TitleError"] = apiResponse.Message;
                        return RedirectToAction("Index", "Notfound");
                    }
                    if (apiResponse.Data != null)
                        dm_HangHoa = apiResponse.Data as v_v_dm_HangHoa;
                }
                //@ConvertObjectTCVN3ToUnicode
                dm_HangHoa.lstdm_DonViTinh = new List<v_dm_DonViTinh>();
                dm_HangHoa.lstdm_DonViTinh = Utility.GetListData<v_dm_DonViTinh>(API.dm_DonViTinh, "", "", Utility.LOC_ID).Data as List<v_dm_DonViTinh>;
                dm_HangHoa.lstdm_DonViTinh_QD = new List<v_dm_DonViTinh>();
                dm_HangHoa.lstdm_DonViTinh_QD = Utility.GetListData<v_dm_DonViTinh>(API.dm_DonViTinh, "", "", Utility.LOC_ID).Data as List<v_dm_DonViTinh>;
                dm_HangHoa.lstdm_NhaCungCap = new List<v_dm_NhaCungCap>();
                dm_HangHoa.lstdm_NhaCungCap = Utility.GetListData<v_dm_NhaCungCap>(API.dm_NhaCungCap, "", "", Utility.LOC_ID).Data as List<v_dm_NhaCungCap>;
                dm_HangHoa.lstdm_NhomHangHoa = new List<v_dm_NhomHangHoa>();
                dm_HangHoa.lstdm_NhomHangHoa = Utility.GetListData<v_dm_NhomHangHoa>(API.dm_NhomHangHoa, "", "", Utility.LOC_ID).Data as List<v_dm_NhomHangHoa>;
                dm_HangHoa.lstdm_ThueSuat = new List<v_dm_ThueSuat>();
                dm_HangHoa.lstdm_ThueSuat = Utility.GetListData<v_dm_ThueSuat>(API.dm_ThueSuat, "", "", Utility.LOC_ID).Data as List<v_dm_ThueSuat>;
                return View(dm_HangHoa);
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
                TempData["TitleError"] = API.TitleTryCatch;
                TempData["DetailError"] = ex.Message;
                return RedirectToAction("Index", "Notfound");
            }
        }

        // POST: Product/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ISKHONGHIENTHITONKHO,LOC_ID,ID,BARCODE,MA,NAME,PICTURE,GIA01,GIA02,GIA03,GIA01_QD,GIA02_QD,ID_NHOMHANGHOA,ISACTIVE,LOAIHANGHOA,ISCOMBO,ID_DVT,STATUS_QD,ID_DVT_QD,TYLE_QD,TRONGLUONG,STATUS_HIENTHI,ID_NCC,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO,BAOGOMTHUESUAT,ID_THUESUAT,GIA03_QD")] v_v_dm_HangHoa dm_HangHoa)
        {
            try
            {
                if (Utility.KiemTra())
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (!Utility.KiemTraQuyen(API.dm_HangHoa, API.Edit))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                if (ModelState.IsValid)
                {
                    dm_HangHoa.LOC_ID = Utility.LOC_ID;
                    dm_HangHoa.ID_NGUOISUA = Session[Sessions.idUser].ToString();
                    dm_HangHoa.THOIGIANSUA = Utility.CurrentTime;
                    //@ConvertObjectUnicodeToTCVN3
                    var apiResponse = Utility.Edit<v_dm_HangHoa>(Utility.LOC_ID + "/" + dm_HangHoa.MA, dm_HangHoa, API.dm_HangHoa);
                    if (apiResponse.Success)
                        return RedirectToAction("Index");
                    else
                        ModelState.AddModelError(string.Empty, apiResponse.Message);
                }
                return View(dm_HangHoa);
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
                TempData["TitleError"] = API.TitleTryCatch;
                TempData["DetailError"] = ex.Message;
                return RedirectToAction("Index", "Notfound");
            }
        }

        // POST: Product/Delete/5
        [HttpPost]
        public ActionResult Delete(string id)
        {
            try
            {
                if (Utility.KiemTra())
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (!Utility.KiemTraQuyen(API.dm_HangHoa, API.Delete))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                var apiResponse = Utility.Delete<v_dm_HangHoa>(Utility.LOC_ID + "/" + id, API.dm_HangHoa);
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
                if (!Utility.KiemTraQuyen(API.dm_HangHoa, API.Create))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                v_v_dm_HangHoa dm_HangHoa = new v_v_dm_HangHoa();
                apiResponse.Success = true;
                dm_HangHoa.LOC_ID = Utility.LOC_ID;
                dm_HangHoa.ID = Guid.NewGuid().ToString();
                dm_HangHoa.lstdm_DonViTinh = new List<v_dm_DonViTinh>();
                dm_HangHoa.lstdm_DonViTinh = Utility.GetListData<v_dm_DonViTinh>(API.dm_DonViTinh, "", "", Utility.LOC_ID).Data as List<v_dm_DonViTinh>;
                dm_HangHoa.lstdm_DonViTinh_QD = new List<v_dm_DonViTinh>();
                dm_HangHoa.lstdm_DonViTinh_QD = Utility.GetListData<v_dm_DonViTinh>(API.dm_DonViTinh, "", "", Utility.LOC_ID).Data as List<v_dm_DonViTinh>;
                dm_HangHoa.lstdm_NhaCungCap = new List<v_dm_NhaCungCap>();
                dm_HangHoa.lstdm_NhaCungCap = Utility.GetListData<v_dm_NhaCungCap>(API.dm_NhaCungCap, "", "", Utility.LOC_ID).Data as List<v_dm_NhaCungCap>;
                dm_HangHoa.lstdm_NhomHangHoa = new List<v_dm_NhomHangHoa>();
                dm_HangHoa.lstdm_NhomHangHoa = Utility.GetListData<v_dm_NhomHangHoa>(API.dm_NhomHangHoa, "", "", Utility.LOC_ID).Data as List<v_dm_NhomHangHoa>;
                dm_HangHoa.lstdm_ThueSuat = new List<v_dm_ThueSuat>();
                dm_HangHoa.lstdm_ThueSuat = Utility.GetListData<v_dm_ThueSuat>(API.dm_ThueSuat, "", "", Utility.LOC_ID).Data as List<v_dm_ThueSuat>;
                dm_HangHoa.BAOGOMTHUESUAT = true;
                apiResponse.Detail = Utility.ConvertobjectTo<v_v_dm_HangHoa>(dm_HangHoa);
                apiResponse.PathProduct = API.PathProduct;
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
        public ActionResult CreatePopup([Bind(Include = "ISKHONGHIENTHITONKHO,LOC_ID,ID,BARCODE,MA,NAME,PICTURE,GIA01,GIA02,GIA03,GIA01_QD,GIA02_QD,ID_NHOMHANGHOA,ISACTIVE,LOAIHANGHOA,ISCOMBO,ID_DVT,STATUS_QD,ID_DVT_QD,TYLE_QD,TRONGLUONG,STATUS_HIENTHI,ID_NCC,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO,BAOGOMTHUESUAT,ID_THUESUAT,GIA03_QD,GIAMUA,GIAMUA_QD")] v_dm_HangHoa dm_HangHoa)
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                if (Utility.KiemTra())
                {
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Admin");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                if (!Utility.KiemTraQuyen(API.dm_HangHoa, API.Create))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                var lstKey = Request.Form.AllKeys.Where(e => e.StartsWith("txtQuantity|"));
                if (dm_HangHoa.LOAIHANGHOA == ((int)API.LoaiSanPham.Combo).ToString())
                {
                    if (lstKey == null || lstKey.Count() == 0)
                    {
                        ModelState.AddModelError("lstdm_HangHoa_Combo", "Thêm sản phẩm trong combo.");
                    }
                }
                else
                {
                    if (dm_HangHoa.STATUS_QD)
                    {
                        if (dm_HangHoa.TYLE_QD <= 0)
                        {
                            ModelState.AddModelError("TYLE_QD", "The TYLE_QD field is required.");
                        }

                        if (string.IsNullOrEmpty(dm_HangHoa.ID_DVT_QD))
                        {
                            ModelState.AddModelError("ID_DVT_QD", "The ID_DVT_QD field is required.");
                        }
                    }
                }
                if (ModelState.IsValid)
                {
                    if (dm_HangHoa.LOAIHANGHOA == ((int)API.LoaiSanPham.Combo).ToString())
                    {
                        dm_HangHoa.lstdm_HangHoa_Combo = new List<v_dm_HangHoa_Combo>();
                        if (lstKey != null)
                        {
                            foreach (var itm in lstKey)
                            {
                                var lstString = itm.ToString().Split('|');
                                var value = HttpContext.Request.Params.GetValues(itm.ToString());
                                if (lstString != null && lstString.Length > 3)
                                {
                                    v_dm_HangHoa_Combo newv_dm_HangHoa_Combo = new v_dm_HangHoa_Combo();
                                    newv_dm_HangHoa_Combo.ID_HANGHOA = lstString[1];
                                    newv_dm_HangHoa_Combo.ID_DVT = lstString[2];
                                    newv_dm_HangHoa_Combo.TYLE_QD = Utility.ConvertStringToDouble(lstString[3]);
                                    newv_dm_HangHoa_Combo.QTY = Utility.ConvertStringToDouble(value[0]);
                                    dm_HangHoa.lstdm_HangHoa_Combo.Add(newv_dm_HangHoa_Combo);
                                }
                            }
                        }
                    }
                    dm_HangHoa.LOC_ID = Utility.LOC_ID;
                    dm_HangHoa.ID_NGUOITAO = Session[Sessions.idUser].ToString();
                    dm_HangHoa.THOIGIANTHEM = Utility.CurrentTime;

                    if (Request.Files["MaHinh"] != null)//Nếu có uploads
                    {
                        String fulName = Request.Files["MaHinh"].FileName;
                        if (fulName != "")
                        {
                            String Name = Guid.NewGuid().ToString() + "." + fulName.Split('.')[1];
                            String fullpath = Path.Combine(Server.MapPath("~" + API.PathProduct), Name);
                            if (!System.IO.Directory.Exists(Server.MapPath("~" + API.PathProduct)))
                            {
                                System.IO.Directory.CreateDirectory(Server.MapPath("~" + API.PathProduct));
                            }
                            Request.Files["MaHinh"].SaveAs(fullpath);
                            dm_HangHoa.PICTURE = Name;//cập nhật tên file ảnh
                            Byte[] AsBytes = System.IO.File.ReadAllBytes(fullpath);
                            String AsBase64String = Convert.ToBase64String(AsBytes);
                            dm_HangHoa.FILEBASE64 = AsBase64String;
                        }
                    }
                    apiResponse = Utility.Create<v_dm_HangHoa>(dm_HangHoa, API.dm_HangHoa);
                    if (apiResponse.Success)
                    {
                        apiResponse.NewID = Guid.NewGuid().ToString();
                        if (apiResponse.Data != null)
                            dm_HangHoa = JsonConvert.DeserializeObject<v_v_dm_HangHoa>(apiResponse.Data.ToString());
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
                    apiResponse.Data = Utility.GetModelState(ModelState, API.dm_HangHoa);
                }
                apiResponse.ID = dm_HangHoa.ID;
                apiResponse.Detail = Utility.ConvertobjectToView<v_dm_HangHoa>(dm_HangHoa);
                apiResponse.PathProduct = API.PathProduct;
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
                if (!Utility.KiemTraQuyen(API.dm_HangHoa, API.Edit))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                v_v_dm_HangHoa dm_HangHoa = new v_v_dm_HangHoa();
                if (!string.IsNullOrEmpty(id))
                {
                    apiResponse = Utility.GetDetail<v_v_dm_HangHoa>(Utility.LOC_ID + "/" + id, API.dm_HangHoa);
                    if (!apiResponse.Success)
                    {
                        TempData["TitleError"] = apiResponse.Message;
                        apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                        return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                    }
                    if (apiResponse.Data != null)
                        dm_HangHoa = apiResponse.Data as v_v_dm_HangHoa;
                }
                dm_HangHoa.lstdm_DonViTinh = new List<v_dm_DonViTinh>();
                dm_HangHoa.lstdm_DonViTinh = Utility.GetListData<v_dm_DonViTinh>(API.dm_DonViTinh, "", "", Utility.LOC_ID).Data as List<v_dm_DonViTinh>;
                dm_HangHoa.lstdm_DonViTinh_QD = new List<v_dm_DonViTinh>();
                dm_HangHoa.lstdm_DonViTinh_QD = Utility.GetListData<v_dm_DonViTinh>(API.dm_DonViTinh, "", "", Utility.LOC_ID).Data as List<v_dm_DonViTinh>;
                dm_HangHoa.lstdm_NhaCungCap = new List<v_dm_NhaCungCap>();
                dm_HangHoa.lstdm_NhaCungCap = Utility.GetListData<v_dm_NhaCungCap>(API.dm_NhaCungCap, "", "", Utility.LOC_ID).Data as List<v_dm_NhaCungCap>;
                dm_HangHoa.lstdm_NhomHangHoa = new List<v_dm_NhomHangHoa>();
                dm_HangHoa.lstdm_NhomHangHoa = Utility.GetListData<v_dm_NhomHangHoa>(API.dm_NhomHangHoa, "", "", Utility.LOC_ID).Data as List<v_dm_NhomHangHoa>;
                dm_HangHoa.lstdm_ThueSuat = new List<v_dm_ThueSuat>();
                dm_HangHoa.lstdm_ThueSuat = Utility.GetListData<v_dm_ThueSuat>(API.dm_ThueSuat, "", "", Utility.LOC_ID).Data as List<v_dm_ThueSuat>;
                dm_HangHoa.lstdm_HangHoa_Combo = new List<v_dm_HangHoa_Combo>();
                dm_HangHoa.lstdm_HangHoa_Combo = Utility.GetListData<v_dm_HangHoa_Combo>(API.dm_HangHoa_Combo, "", "", Utility.LOC_ID + "/" + dm_HangHoa.ID).Data as List<v_dm_HangHoa_Combo>;
                Session[Sessions.lstProductCombo] = dm_HangHoa.lstdm_HangHoa_Combo;

                apiResponse.Success = true;
                apiResponse.ProductCombo = Utility.GetProductCombo();
                var lst = Utility.ConvertobjectTo<v_dm_HangHoa>(dm_HangHoa);
                lst.Add(new ValueEdit { Key = "tbodyTempItemComboEdit", Value = apiResponse.ProductCombo });
                apiResponse.Detail = lst;
                apiResponse.PathProduct = API.PathProduct;
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
        public ActionResult EditPopup([Bind(Include = "ISKHONGHIENTHITONKHO,LOC_ID,ID,BARCODE,MA,NAME,PICTURE,GIA01,GIA02,GIA03,GIA01_QD,GIA02_QD,GIA03_QD,ID_NHOMHANGHOA,ISACTIVE,LOAIHANGHOA,ISCOMBO,ID_DVT,STATUS_QD,ID_DVT_QD,TYLE_QD,TRONGLUONG,STATUS_HIENTHI,ID_NCC,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO,BAOGOMTHUESUAT,ID_THUESUAT,GIAMUA,GIAMUA_QD ")] v_v_dm_HangHoa dm_HangHoa)
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                if (Utility.KiemTra())
                {
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Admin");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                if (!Utility.KiemTraQuyen(API.dm_HangHoa, API.Edit))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                var lstKey = Request.Form.AllKeys.Where(e => e.StartsWith("txtQuantity|"));
                if (dm_HangHoa.LOAIHANGHOA == ((int)API.LoaiSanPham.Combo).ToString())
                {
                    if (lstKey == null || lstKey.Count() == 0)
                    {
                        ModelState.AddModelError("lstdm_HangHoa_Combo", "Thêm hàng hóa trong combo.");
                    }
                }
                else
                {
                    if (dm_HangHoa.STATUS_QD)
                    {
                        if (dm_HangHoa.TYLE_QD <= 0)
                        {
                            ModelState.AddModelError("TYLE_QD", "The TYLE_QD field is required.");
                        }

                        if (string.IsNullOrEmpty(dm_HangHoa.ID_DVT_QD))
                        {
                            ModelState.AddModelError("ID_DVT_QD", "The ID_DVT_QD field is required.");
                        }
                    }
                }
                if (ModelState.IsValid)
                {
                    if (dm_HangHoa.LOAIHANGHOA == ((int)API.LoaiSanPham.Combo).ToString())
                    {
                        dm_HangHoa.lstdm_HangHoa_Combo = new List<v_dm_HangHoa_Combo>();
                        if (lstKey != null)
                        {
                            foreach (var itm in lstKey)
                            {
                                var lstString = itm.ToString().Split('|');
                                var value = HttpContext.Request.Params.GetValues(itm.ToString());
                                if (lstString != null && lstString.Length > 3)
                                {
                                    v_dm_HangHoa_Combo newv_dm_HangHoa_Combo = new v_dm_HangHoa_Combo();
                                    newv_dm_HangHoa_Combo.ID_HANGHOA = lstString[1];
                                    newv_dm_HangHoa_Combo.ID_DVT = lstString[2];
                                    newv_dm_HangHoa_Combo.TYLE_QD = Utility.ConvertStringToDouble(lstString[3]);
                                    newv_dm_HangHoa_Combo.QTY = Utility.ConvertStringToDouble(value[0]);
                                    newv_dm_HangHoa_Combo.THOIGIANTHEM = Utility.CurrentTime;
                                    newv_dm_HangHoa_Combo.ID_NGUOITAO = Session[Sessions.idUser].ToString();
                                    dm_HangHoa.lstdm_HangHoa_Combo.Add(newv_dm_HangHoa_Combo);
                                }
                            }
                        }
                    }
                    dm_HangHoa.LOC_ID = Utility.LOC_ID;
                    dm_HangHoa.ID_NGUOISUA = Session[Sessions.idUser].ToString();
                    dm_HangHoa.THOIGIANSUA = Utility.CurrentTime;
                    if (Request.Files["MaHinh"] != null)//Nếu có uploads
                    {
                        String fulName = Request.Files["MaHinh"].FileName;
                        if (fulName != "")
                        {
                            String Name = dm_HangHoa.ID.Trim() + "." + fulName.Split('.')[1];
                            String fullpath = Path.Combine(Server.MapPath("~" + API.PathProduct), Name);
                            if (!System.IO.Directory.Exists(Server.MapPath("~" + API.PathProduct)))
                            {
                                System.IO.Directory.CreateDirectory(Server.MapPath("~" + API.PathProduct));
                            }
                            if (System.IO.File.Exists(fullpath))
                            {
                                System.IO.File.Delete(fullpath);
                            }
                            Request.Files["MaHinh"].SaveAs(fullpath);
                            dm_HangHoa.PICTURE = Name;//cập nhật tên file ảnh
                            Byte[] AsBytes = System.IO.File.ReadAllBytes(fullpath);
                            String AsBase64String = Convert.ToBase64String(AsBytes);
                            dm_HangHoa.FILEBASE64 = AsBase64String;
                            dm_HangHoa.FILENEW = true;
                        }
                    }
                   
                    apiResponse = Utility.Edit<v_dm_HangHoa>(Utility.LOC_ID + "/" + dm_HangHoa.MA, dm_HangHoa, API.dm_HangHoa);
                    if (apiResponse.Success)
                    {
                        //return RedirectToAction("Index");
                        apiResponse.ID = dm_HangHoa.ID;
                        if (apiResponse.Data != null)
                            dm_HangHoa = JsonConvert.DeserializeObject<v_v_dm_HangHoa>(apiResponse.Data.ToString());
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, apiResponse.Message);
                    }
                }
                else
                {
                    apiResponse.Success = false;
                    apiResponse.Data = Utility.GetModelState(ModelState, API.dm_HangHoa);
                }
                apiResponse.Detail = Utility.ConvertobjectToView<v_dm_HangHoa>(dm_HangHoa);
                apiResponse.PathProduct = API.PathProduct;
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
                if (!Utility.KiemTraQuyen(API.dm_HangHoa, API.Delete))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                apiResponse = Utility.Delete<v_dm_HangHoa>(Utility.LOC_ID + "/" + id, API.dm_HangHoa);
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

        [HttpGet]
        public ActionResult LoadProduct(string ID, string Type)
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                if (Utility.KiemTra())
                {
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Admin");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                v_v_dm_HangHoa dm_HangHoa = new v_v_dm_HangHoa();

                apiResponse = Utility.GetDetail<v_v_dm_HangHoa>(Utility.LOC_ID + "/" + ID, API.dm_HangHoa);
                if (!apiResponse.Success)
                {
                    apiResponse.Data = new List<v_dm_HangHoa>();
                    TempData["TitleError"] = apiResponse.Message;
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                if (apiResponse.Data != null)
                    dm_HangHoa = (apiResponse.Data as v_v_dm_HangHoa);
                if (dm_HangHoa != null)
                {
                    switch (Type)
                    {
                        case API.ct_PhieuNhap:
                            dm_HangHoa.GIA = dm_HangHoa.GIAMUA;
                            dm_HangHoa.GIA_QD = dm_HangHoa.GIAMUA_QD;
                            break;
                        case API.ct_PhieuNhapKhac:
                            dm_HangHoa.GIA = dm_HangHoa.GIAMUA;
                            dm_HangHoa.GIA_QD = dm_HangHoa.GIAMUA_QD;
                            break;
                        case API.ct_PhieuXuat:
                            dm_HangHoa.GIA = dm_HangHoa.GIA01;
                            dm_HangHoa.GIA_QD = dm_HangHoa.GIA01_QD;
                            break;
                        case API.ct_PhieuXuatKhac:
                            dm_HangHoa.GIA = dm_HangHoa.GIA01;
                            dm_HangHoa.GIA_QD = dm_HangHoa.GIA01_QD;
                            break;
                        case API.ct_PhieuChuyen:
                            dm_HangHoa.GIA = 0;
                            dm_HangHoa.GIA_QD = 0;
                            break;
                        case API.dm_HangHoa:
                            dm_HangHoa.GIA = dm_HangHoa.GIA01;
                            dm_HangHoa.GIA_QD = dm_HangHoa.GIA01_QD;
                            break;
                        default:
                            // code block
                            break;
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
                }


                apiResponse.Detail = Utility.ConvertobjectTo<v_dm_HangHoa>(dm_HangHoa);
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

        #region Nhập hàng
        [HttpGet]
        public ActionResult LoadProductKho(string ID, string Type, string ID_KHO)
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                if (Utility.KiemTra())
                {
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Admin");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                v_dm_HangHoa dm_HangHoa = new v_dm_HangHoa();

                apiResponse = Utility.Get_DanhSachSanPhamKho<v_dm_HangHoa>(ID_KHO, false, ID);
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
                    switch (Type)
                    {
                        case API.ct_PhieuNhap:
                            dm_HangHoa.GIA = dm_HangHoa.GIAMUA;
                            dm_HangHoa.GIA_QD = dm_HangHoa.GIAMUA_QD;
                            break;
                        case API.ct_PhieuNhapKhac:
                            dm_HangHoa.GIA = dm_HangHoa.GIAMUA;
                            dm_HangHoa.GIA_QD = dm_HangHoa.GIAMUA_QD;
                            break;
                        case API.ct_PhieuXuat:
                            dm_HangHoa.GIA = dm_HangHoa.GIA01;
                            dm_HangHoa.GIA_QD = dm_HangHoa.GIA01_QD;
                            break;
                        case API.ct_PhieuXuatKhac:
                            dm_HangHoa.GIA = dm_HangHoa.GIA01;
                            dm_HangHoa.GIA_QD = dm_HangHoa.GIA01_QD;
                            break;
                        case API.ct_PhieuChuyen:
                            dm_HangHoa.GIA = 0;
                            dm_HangHoa.GIA_QD = 0;
                            break;
                        default:
                            // code block
                            break;
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
                        else
                        {
                            dm_HangHoa.THANHTIEN = dm_HangHoa.GIA * 1;
                            dm_HangHoa.THUESUAT = 0;
                            dm_HangHoa.TONGTIENVAT = dm_HangHoa.THANHTIEN * dm_HangHoa.THUESUAT / 100;
                            dm_HangHoa.TONGCONG = dm_HangHoa.THANHTIEN + dm_HangHoa.TONGTIENVAT;
                        }
                    }
                }


                apiResponse.Detail = Utility.ConvertobjectTo<v_dm_HangHoa>(dm_HangHoa);
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

        [HttpPost, ValidateInput(false)]
        public ActionResult AddProductInputOutput([Bind(Include = "ID_HANGHOA,ID_HANGHOAKHO,DONGIA,ID_DVT,SOLUONG,CHIETKHAU,TONGTIENGIAMGIA,THANHTIEN,THUESUAT,ID_THUESUAT,TONGTIENVAT,TONGCONG,ID_KHO")] Product_Detail Product_Detail)
        {

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
                        Product_Detail.ID = Guid.NewGuid().ToString();
                        Product_Detail.NAME = dm_HangHoa.NAME;
                        Product_Detail.MA = dm_HangHoa.MA;
                        if (dm_HangHoa.ID_DVT == Product_Detail.ID_DVT)
                        {
                            Product_Detail.NAME_DVT = dm_HangHoa.NAME_DVT;
                            if (!string.IsNullOrEmpty(dm_HangHoa.ID_DVT_QD))
                            {
                                Product_Detail.TYLE_QD = dm_HangHoa.TYLE_QD;
                            }
                            else
                            {
                                if(dm_HangHoa.LOAIHANGHOA == ((int)API.LoaiSanPham.KhongQuanLyTonKho).ToString())
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

                        var check = Utility.LstProductInput.Where(e => e.ID_HANGHOAKHO == Product_Detail.ID_HANGHOAKHO && e.ID_DVT == Product_Detail.ID_DVT && e.DONGIA == Product_Detail.DONGIA).FirstOrDefault();
                        if (check == null)
                        {
                            var LstProduct = Utility.LstProductInput;
                            LstProduct.Add(Product_Detail);
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
                                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                                }

                                if (apiResponse_Combo.Data != null)
                                {
                                    var lstHoangHoaCombo = (apiResponse_Combo.Data as List<Product_Detail>);
                                    foreach (Product_Detail itm in lstHoangHoaCombo)
                                    {
                                        itm.ID = Guid.NewGuid().ToString();
                                        itm.ID_DVT = itm.ID_DVT_COMBO;
                                        itm.SOLUONG = Product_Detail.SOLUONG * itm.QTY_COMBO;
                                        itm.TYLE_QD = itm.TYLE_QD_COMBO;
                                        itm.TONGSOLUONG = Product_Detail.SOLUONG * itm.QTY_TOTAL_COMBO;
                                        itm.DONGIA = 0;
                                        itm.ISCOMBO = true;
                                        itm.ID_COMBO = Product_Detail.ID_HANGHOA;

                                        Product_Detail.ID_COMBO = Product_Detail.ID_HANGHOA;
                                        LstProduct.Add(itm);
                                    }
                                }
                            }

                            Session[Sessions.lstProductInput] = LstProduct;
                        }
                    }
                    var lstProduct = Utility.LstProductInput;
                    string url = Request.Url.AbsolutePath;
                    apiResponse.ProductCombo = Utility.GetProductInputOutput(lstProduct, "InputOutput", true, 0,0,0,0, false, url.Contains(API.ct_PhieuNhap));
                }
               
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

        [HttpPost]
        public ActionResult UpdateAddProduct(Product_Detail Product_Detail)
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                var lstProduct = Utility.LstProductInput;
                Utility.TinhTong(Product_Detail, null, lstProduct);
                apiResponse.Success = true;
                apiResponse.Detail = Product_Detail;
                return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex , JsonConvert.SerializeObject(Product_Detail));
                TempData["TitleError"] = API.TitleTryCatch;
                TempData["DetailError"] = ex.Message;
                apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
            }
        }

        [HttpPost]
        public ActionResult DeleteProductInputOutput(string ID)
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                v_v_dm_HangHoa dm_HangHoa = new v_v_dm_HangHoa();
                var lstProduct = Utility.LstProductInput;
                var check = Utility.LstProductInput.Where(e => e.ID == ID).FirstOrDefault();
                if (check != null && lstProduct != null)
                {
                    if (!string.IsNullOrEmpty(check.ID_COMBO))
                    {
                        foreach (var itm in lstProduct.Where(e => e.ID_COMBO == check.ID_COMBO).ToList())
                            lstProduct.Remove(itm);
                    }
                    else
                        lstProduct.Remove(check);
                }
                Session[Sessions.lstProductInput] = lstProduct;
                string url = Request.Url.AbsolutePath;
                apiResponse.ProductCombo = Utility.GetProductInputOutput(lstProduct, "InputOutput", true, 0, 0, 0, 0, false, url.Contains(API.ct_PhieuNhap));
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
        public ActionResult UpdateProductInputOutput(string ID, string TYPE, string VALUE)
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                v_v_dm_HangHoa dm_HangHoa = new v_v_dm_HangHoa();
                var lstProduct = Utility.LstProductInput;
                var check = Utility.LstProductInput.Where(e => e.ID == ID).FirstOrDefault();
                if (check != null)
                {
                    check.TYPE = TYPE;
                    Utility.TinhTong(check, VALUE, lstProduct);
                }
                string url = Request.Url.AbsolutePath;
                Session[Sessions.lstProductInput] = lstProduct;
                apiResponse.ProductCombo = Utility.GetProductInputOutput(lstProduct, "InputOutput", true, 0, 0, 0, 0, url.Contains(API.ct_PhieuNhap), url.Contains(API.ct_PhieuNhap));
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

        #endregion
    }
}