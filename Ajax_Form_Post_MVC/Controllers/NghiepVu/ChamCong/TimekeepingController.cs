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
using static System.Data.Entity.Infrastructure.Design.Executor;
using System.Net.NetworkInformation;
using System.Net.Http;
using System.Threading.Tasks;
using System.Diagnostics;

namespace MVC_QuanLyTHP.Controllers
{
    public class TimekeepingController : Controller
    {

        // GET: Payment
        public ActionResult Index()
        {
            try
            {
                if (Utility.KiemTra())
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (!Utility.KiemTraQuyen(API.nv_ChamCong, API.Xem))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                ApiResponse apiResponse = new ApiResponse();
                IPagedList<v_nv_ChamCong> lstpage = (new List<v_nv_ChamCong>()).ToList().ToPagedList(1, Utility.GetPageSize());
                v_v_nv_ChamCong nv_ChamCong = new v_v_nv_ChamCong();
                nv_ChamCong.IPagedList = lstpage;
                nv_ChamCong.lstdm_NhanVien = new List<ComboboxFrom>();
                nv_ChamCong.lstdm_NhanVien = Utility.GetListData<ComboboxFrom>(API.AspNetUser, "", "", "").Data as List<ComboboxFrom>;
                nv_ChamCong.lstdm_PhongBan = new List<dm_PhongBan>();
                nv_ChamCong.lstdm_PhongBan = Utility.GetListData<dm_PhongBan>(API.dm_PhongBan, "", "", Utility.LOC_ID).Data as List<dm_PhongBan>;
                nv_ChamCong.TUNGAY = DateTime.Now;
                nv_ChamCong.DENNGAY = DateTime.Now;
                ViewBag.PermissionEdit = Utility.KiemTraQuyen(API.nv_ChamCong, API.Edit);
                ViewBag.PermissionDelete = Utility.KiemTraQuyen(API.nv_ChamCong, API.Delete);
                ViewBag.PermissionCreate = Utility.KiemTraQuyen(API.nv_ChamCong, API.Create);
                if (!ViewBag.PermissionCreate)
                    nv_ChamCong.lstdm_NhanVien = nv_ChamCong.lstdm_NhanVien.Where(s => s.ID == Session[Sessions.idUser].ToString()).ToList();
                return View(nv_ChamCong);
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
                TempData["TitleError"] = API.TitleTryCatch;
                TempData["DetailError"] = ex.Message;
                return RedirectToAction("Index", "Notfound");
            }
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Index(SP_Parameter objParameter)
        {
            try
            {
                if (Utility.KiemTra())
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (!Utility.KiemTraQuyen(API.nv_ChamCong, API.Xem))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                ApiResponse apiResponse = new ApiResponse();
                IPagedList<v_nv_ChamCong> lstpage = (new List<v_nv_ChamCong>()).ToList().ToPagedList(1, Utility.GetPageSize());
                ViewBag.PermissionCreate = Utility.KiemTraQuyen(API.nv_ChamCong, API.Create);
                if (!ViewBag.PermissionCreate)
                    objParameter.ID_NHANVIEN = Session[Sessions.idUser].ToString();
                else
                    objParameter.ID_NHANVIEN = objParameter.ID_NHANVIEN;
                apiResponse = Utility.Get_DanhSachChamCong<v_nv_ChamCong>(objParameter.TUNGAY, objParameter.DENNGAY, null, objParameter.KEY, objParameter.ID_NHANVIEN);
                if (!apiResponse.Success)
                {
                    TempData["TitleError"] = apiResponse.Message;
                    return RedirectToAction("Index", "Notfound");
                }

                var lst = apiResponse.Data as List<v_nv_ChamCong>;
                lstpage = lst.ToPagedList(1, lst.Count() > 0 ? lst.Count() : 50);
                v_v_nv_ChamCong nv_ChamCong = new v_v_nv_ChamCong();
                nv_ChamCong.IPagedList = lstpage;
                nv_ChamCong.lstdm_NhanVien = new List<ComboboxFrom>();
                nv_ChamCong.lstdm_NhanVien = Utility.GetListData<ComboboxFrom>(API.AspNetUser, "", "", "").Data as List<ComboboxFrom>;
                nv_ChamCong.lstdm_PhongBan = new List<dm_PhongBan>();
                nv_ChamCong.lstdm_PhongBan = Utility.GetListData<dm_PhongBan>(API.dm_PhongBan, "", "", Utility.LOC_ID).Data as List<dm_PhongBan>;
                nv_ChamCong.TUNGAY = objParameter.TUNGAY != null ? objParameter.TUNGAY.Value : Utility.CurrentTime;
                nv_ChamCong.DENNGAY = objParameter.DENNGAY != null ? objParameter.DENNGAY.Value : Utility.CurrentTime;
                nv_ChamCong.ID_NHANVIEN = objParameter.ID_NHANVIEN;
                nv_ChamCong.ID_PHONGBAN = objParameter.ID_PHONGBAN;
                ViewBag.PermissionEdit = Utility.KiemTraQuyen(API.nv_ChamCong, API.Edit);
                ViewBag.PermissionDelete = Utility.KiemTraQuyen(API.nv_ChamCong, API.Delete);
                if (!ViewBag.PermissionCreate)
                    nv_ChamCong.lstdm_NhanVien = nv_ChamCong.lstdm_NhanVien.Where(s => s.ID == Session[Sessions.idUser].ToString()).ToList();
                return View(nv_ChamCong);
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
                TempData["TitleError"] = API.TitleTryCatch;
                TempData["DetailError"] = ex.Message;
                return RedirectToAction("Index", "Notfound");
            }
        }

        public ActionResult TableTimekeeping()
        {
            try
            {
                if (Utility.KiemTra())
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (!Utility.KiemTraQuyen(API.nv_ChamCong, API.Xem))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                ApiResponse apiResponse = new ApiResponse();
                List<v_nv_ChamCong> lstpage = (new List<v_nv_ChamCong>()).ToList();
                v_v_nv_ChamCong nv_ChamCong = new v_v_nv_ChamCong();
                nv_ChamCong.lstnv_ChamCong_Table = lstpage;
                nv_ChamCong.lstdm_NhanVien = new List<ComboboxFrom>();
                nv_ChamCong.lstdm_NhanVien = Utility.GetListData<ComboboxFrom>(API.AspNetUser, "", "", "").Data as List<ComboboxFrom>;
                nv_ChamCong.lstdm_PhongBan = new List<dm_PhongBan>();
                nv_ChamCong.lstdm_PhongBan = Utility.GetListData<dm_PhongBan>(API.dm_PhongBan, "", "", Utility.LOC_ID).Data as List<dm_PhongBan>;
                nv_ChamCong.TUNGAY = DateTime.Now;
                nv_ChamCong.DENNGAY = DateTime.Now;
                ViewBag.PermissionEdit = Utility.KiemTraQuyen(API.nv_ChamCong, API.Edit);
                ViewBag.PermissionDelete = Utility.KiemTraQuyen(API.nv_ChamCong, API.Delete);
                ViewBag.PermissionCreate = Utility.KiemTraQuyen(API.nv_ChamCong, API.Create);
                ViewBag.IsLoad = false;
                return View(nv_ChamCong);
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
                TempData["TitleError"] = API.TitleTryCatch;
                TempData["DetailError"] = ex.Message;
                return RedirectToAction("Index", "Notfound");
            }
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult TableTimekeeping(SP_Parameter objParameter)
        {
            try
            {
                if (Utility.KiemTra())
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (!Utility.KiemTraQuyen(API.nv_ChamCong, API.Xem))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                ApiResponse apiResponse = new ApiResponse();
                List<v_nv_ChamCong> lstpage = (new List<v_nv_ChamCong>()).ToList();
                apiResponse = Utility.Get_DanhSachChamCong<v_nv_ChamCong>(objParameter.TUNGAY, objParameter.DENNGAY, null, objParameter.KEY, objParameter.ID_NHANVIEN);
                if (!apiResponse.Success)
                {
                    TempData["TitleError"] = apiResponse.Message;
                    return RedirectToAction("Index", "Notfound");
                }

                lstpage = apiResponse.Data as List<v_nv_ChamCong>;
                List<v_dm_NhanVien> lstdm_NhanVien_Table = Utility.GetListData<v_dm_NhanVien>(API.dm_NhanVien, "", "", Utility.LOC_ID).Data as List<v_dm_NhanVien>;
                if(!string.IsNullOrEmpty(objParameter.ID_NHANVIEN))
                {
                    lstdm_NhanVien_Table = lstdm_NhanVien_Table.Where(s => s.ID_TAIKHOAN == objParameter.ID_NHANVIEN).ToList();
                }
                if (!string.IsNullOrEmpty(objParameter.ID_PHONGBAN))
                {
                    lstdm_NhanVien_Table = lstdm_NhanVien_Table.Where(s => s.ID_PHONGBAN == objParameter.ID_PHONGBAN).ToList();
                }
                apiResponse = Utility.Get_DanhSachNghiPhep<v_nv_NghiPhep>(objParameter.TUNGAY, objParameter.DENNGAY, null, objParameter.KEY, objParameter.ID_NHANVIEN);
                if (!apiResponse.Success)
                {
                    TempData["TitleError"] = apiResponse.Message;
                    return RedirectToAction("Index", "Notfound");
                }

                var lst = apiResponse.Data as List<v_nv_NghiPhep>;  

                //lstpage.GroupBy(s => new { s.ID_NHANVIEN, s.NAME_NHANVIEN, s.MA_NHANVIEN})
                //.Select(s => new v_AspNetUsers
                //{ 
                //    ID = s.Key.ID_NHANVIEN,
                //    MA = s.Key.MA_NHANVIEN,
                //    NAME = s.Key.NAME_NHANVIEN
                //}).ToList();

                v_v_nv_ChamCong nv_ChamCong = new v_v_nv_ChamCong();
                nv_ChamCong.lstnv_ChamCong_Table = lstpage;
                nv_ChamCong.lstdm_NhanVien = new List<ComboboxFrom>();
                nv_ChamCong.lstdm_NhanVien = Utility.GetListData<ComboboxFrom>(API.AspNetUser, "", "", "").Data as List<ComboboxFrom>;
                nv_ChamCong.lstdm_PhongBan = new List<dm_PhongBan>();
                nv_ChamCong.lstdm_PhongBan = Utility.GetListData<dm_PhongBan>(API.dm_PhongBan, "", "", Utility.LOC_ID).Data as List<dm_PhongBan>;
                nv_ChamCong.TUNGAY = objParameter.TUNGAY != null ? objParameter.TUNGAY.Value : Utility.CurrentTime;
                nv_ChamCong.DENNGAY = objParameter.DENNGAY != null ? objParameter.DENNGAY.Value : Utility.CurrentTime;
                nv_ChamCong.ID_NHANVIEN = objParameter.ID_NHANVIEN;
                nv_ChamCong.ID_PHONGBAN = objParameter.ID_PHONGBAN;
                nv_ChamCong.lstdm_NhanVien_Table = new List<v_dm_NhanVien>();
                nv_ChamCong.lstdm_NhanVien_Table = lstdm_NhanVien_Table;
                nv_ChamCong.lstdm_ThangLuong_Table = new List<dm_ThangLuong>();
                nv_ChamCong.lstdm_ThangLuong_Table = Utility.GetListData<dm_ThangLuong>(API.dm_ThangLuong, "", "", Utility.LOC_ID).Data as List<dm_ThangLuong>;
                nv_ChamCong.lstnv_NghiPhep_Table = new List<v_nv_NghiPhep>();
                nv_ChamCong.lstnv_NghiPhep_Table = lst;
                ViewBag.PermissionEdit = Utility.KiemTraQuyen(API.nv_ChamCong, API.Edit);
                ViewBag.PermissionDelete = Utility.KiemTraQuyen(API.nv_ChamCong, API.Delete);
                ViewBag.PermissionCreate = Utility.KiemTraQuyen(API.nv_ChamCong, API.Create);
                ViewBag.IsLoad = true;
                return View(nv_ChamCong);
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
                TempData["TitleError"] = API.TitleTryCatch;
                TempData["DetailError"] = ex.Message;
                return RedirectToAction("Index", "Notfound");
            }
        }

        public ActionResult Timekeeping()
        {
            try
            {
                if (Utility.KiemTra())
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (!Utility.KiemTraQuyen(API.nv_ChamCong, API.Xem))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                var Login_Model = (Login_Model)Session[Sessions.Login_Model];
                ApiResponse apiResponse = new ApiResponse();
                apiResponse = Utility.Get_DanhSachChamCong<v_nv_ChamCong>(null, null, Utility.CurrentTime.Date, null, Login_Model != null ? Login_Model.iduser : "");
                if (!apiResponse.Success)
                {
                    TempData["TitleError"] = apiResponse.Message;
                    return RedirectToAction("Index", "Notfound");
                }
                var lst = apiResponse.Data as List<v_nv_ChamCong>;

                v_nv_ChamCong nv_ChamCong = new v_nv_ChamCong();
                if (lst != null && lst.Count > 0)
                {
                    nv_ChamCong = lst.FirstOrDefault();
                }
                else
                {
                    nv_ChamCong.ID_NHANVIEN = Login_Model != null ? Login_Model.iduser : "";
                    nv_ChamCong.NGAYCONG = Utility.CurrentTime.Date;
                    nv_ChamCong.LOC_ID = Utility.LOC_ID;
                    nv_ChamCong.ID = Guid.NewGuid().ToString();
                }

                ViewBag.NAME_NHANVIEN = Login_Model != null ? Login_Model.fullname : "";
                ViewBag.AVATAR = Login_Model != null && Login_Model.fullname != null && Login_Model.fullname.Length > 0 ? Login_Model.fullname.Substring(0, 1) : "";
                ViewBag.TIMER = Utility.CurrentTime;
                ViewBag.TYPE = nv_ChamCong.THOIGIANVAO != null ? "đăng xuất" : "đăng nhập";
                ViewBag.TYPEFORM = nv_ChamCong.THOIGIANVAO != null ? "CheckOut('" + API.nv_ChamCong + "','"+ Utility.CurrentTime.ToString("yyyy-MM-ddT00:00:00.000Z") + "','" + nv_ChamCong.ID+ "');" : "CheckIn('" + API.nv_ChamCong + "','" + Utility.CurrentTime.ToString("yyyy-MM-ddT00:00:00.000Z") + "');";
                ViewBag.LOGO = nv_ChamCong.THOIGIANVAO != null ? "logout.png" : "login.png";
                string TIMERTEXT = nv_ChamCong.THOIGIANVAO != null ? "<label name=\"TXTTHOIGIANVAO\" id=\"TXTTHOIGIANVAO\">Thời gian vào: " + nv_ChamCong.THOIGIANVAO.Value.ToString("H:mm:ss") + "</label>" : "<label name=\"TXTTHOIGIANVAO\" id=\"TXTTHOIGIANVAO\"></label>";
                TIMERTEXT += nv_ChamCong.THOIGIANRA != null ? (string.IsNullOrEmpty(TIMERTEXT) ? "" : "<br>") + "<label name=\"TXTTHOIGIANRA\" id=\"TXTTHOIGIANRA\">Thời gian ra: " + nv_ChamCong.THOIGIANRA.Value.ToString("H:mm:ss") + "</label>" : "<label name=\"TXTTHOIGIANRA\" id=\"TXTTHOIGIANRA\"></label>";
                ViewBag.TIMERTEXT = TIMERTEXT;
                return View(nv_ChamCong);
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
                TempData["TitleError"] = API.TitleTryCatch;
                TempData["DetailError"] = ex.Message;
                return RedirectToAction("Index", "Notfound");
            }
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult CheckIn(string NGAYCONG, string LATITUDELONGITUDE, string MYPUBLICIPV4)
        {
            try
            {

                ApiResponse apiResponse = new ApiResponse();
                if (Utility.KiemTra())
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (!Utility.KiemTraQuyen(API.nv_ChamCong, API.Create))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                double LATITUDE = 0;
                double LONGITUDE = 0;
                string strKhoangCach = "";
                if (!string.IsNullOrEmpty(LATITUDELONGITUDE))
                {
                    LATITUDE = Convert.ToDouble(LATITUDELONGITUDE.Split('-')[0].Replace(".", ","));
                    LONGITUDE = Convert.ToDouble(LATITUDELONGITUDE.Split('-')[1].Replace(".", ","));
                    apiResponse = Utility.GetListData<v_dm_DiaDiemChamCong>(API.dm_DiaDiemChamCong, "", "", Utility.LOC_ID);
                    if (!apiResponse.Success)
                    {
                        TempData["TitleError"] = apiResponse.Message;
                        return RedirectToAction("Index", "Notfound");
                    }
                    var lstLocation = apiResponse.Data as List<v_dm_DiaDiemChamCong>;
                    Boolean bolCheckDiaDiem = false;
                    
                    foreach(var itm in lstLocation)
                    {
                        if(itm.ISACTIVE)
                        {
                            var khoangcach = API.CalculateDistance(Convert.ToDouble(itm.LATITUDE.Replace(".", ",")), Convert.ToDouble(itm.LONGITUDE.Replace(".", ",")), LATITUDE, LONGITUDE);
                            if (khoangcach <= itm.KHOANGCACH)
                            {
                                bolCheckDiaDiem = true;
                                strKhoangCach += itm.NAME + ": " + khoangcach.ToString("N0") + " m; ";
                            }
                            else
                            {
                                strKhoangCach += itm.NAME + ": " + khoangcach.ToString("N0") + " m; ";
                            }    

                        }
                    }
                    if (!bolCheckDiaDiem)
                    {
                        ModelState.AddModelError(string.Empty, "Khảng cách xa với điểm được chỉ định chấm công!");
                        apiResponse.Message = "Khảng cách xa với điểm được chỉ định chấm công!" + strKhoangCach;
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Không lấy được địa điểm chấm công!");
                    apiResponse.Message = "Không lấy được địa điểm chấm công!";
                }    
                v_nv_ChamCong nv_ChamCong = new v_nv_ChamCong();
                nv_ChamCong.NGAYCONG = Convert.ToDateTime(NGAYCONG);
                if (nv_ChamCong.NGAYCONG.Date != Utility.CurrentTime.Date)
                {
                    ModelState.AddModelError(string.Empty, "Ngày chấm công khác với ngày hiện tại!");
                    apiResponse.Message = "Ngày chấm công khác với ngày hiện tại!";
                }

                nv_ChamCong.LOC_ID = Utility.LOC_ID;
                nv_ChamCong.ID_NHANVIEN = Session[Sessions.idUser].ToString();
                nv_ChamCong.ID = Guid.NewGuid().ToString();
                if (ModelState.IsValid)
                {
                    nv_ChamCong.ID_NGUOITAO = Session[Sessions.idUser].ToString();
                    nv_ChamCong.THOIGIANTHEM = Utility.CurrentTime;
                    nv_ChamCong.THOIGIANVAO = Utility.CurrentTime;
                    nv_ChamCong.NGAYCONG = Utility.CurrentTime.Date;
                    nv_ChamCong.IP_CHAMCONGVAO = MYPUBLICIPV4;
                    nv_ChamCong.GHICHU = strKhoangCach;
                    apiResponse = Utility.Create<nv_ChamCong>(nv_ChamCong, API.nv_ChamCong + "/PostCheckIn");
                    if (apiResponse.Success)
                    {
                        apiResponse.Message = "Chấm công vào thành công! " + nv_ChamCong.THOIGIANVAO.Value.ToString("dd/MM HH:mm:ss");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, apiResponse.Message);
                        apiResponse.Message = apiResponse.Message;
                    }
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                else
                {
                    apiResponse.Success = false;
                    apiResponse.Data = Utility.GetModelState(ModelState, API.nv_ChamCong);
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
                TempData["TitleError"] = API.TitleTryCatch;
                TempData["DetailError"] = ex.Message;
                return RedirectToAction("Index", "Notfound");
            }
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult CheckOut(string NGAYCONG, string ID, string LATITUDELONGITUDE, string MYPUBLICIPV4)
        {
            try
            {
                ApiResponse apiResponse = new ApiResponse();
                if (Utility.KiemTra())
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (!Utility.KiemTraQuyen(API.nv_ChamCong, API.Create))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                double LATITUDE = 0;
                double LONGITUDE = 0;
                //if(!string.IsNullOrEmpty(MYPUBLICIPV4))
                //{
                //    apiResponse = Utility.GetDetail<v_AspNetUsers>(Utility.LOC_ID + "/" + Session[Sessions.idUser].ToString(), API.AspNetUser);
                //    if (!apiResponse.Success)
                //    {
                //        TempData["TitleError"] = apiResponse.Message;
                //        return RedirectToAction("Index", "Notfound");
                //    }
                //}   
                //else
                //{
                //    ModelState.AddModelError(string.Empty, "Không lấy được địa điểm MYPUBLICIPV4!");
                //    apiResponse.Message = "Không lấy được địa điểm MYPUBLICIPV4!";
                //}
                string strKhoangCach = "";
                if (!string.IsNullOrEmpty(LATITUDELONGITUDE))
                {
                    LATITUDE = Convert.ToDouble(LATITUDELONGITUDE.Split('-')[0].Replace(".", ","));
                    LONGITUDE = Convert.ToDouble(LATITUDELONGITUDE.Split('-')[1].Replace(".", ","));
                    apiResponse = Utility.GetListData<v_dm_DiaDiemChamCong>(API.dm_DiaDiemChamCong, "", "", Utility.LOC_ID);
                    if (!apiResponse.Success)
                    {
                        TempData["TitleError"] = apiResponse.Message;
                        return RedirectToAction("Index", "Notfound");
                    }
                    var lstLocation = apiResponse.Data as List<v_dm_DiaDiemChamCong>;
                    Boolean bolCheckDiaDiem = false;
                   
                    foreach (var itm in lstLocation)
                    {
                        if (itm.ISACTIVE)
                        {
                            var khoangcach = API.CalculateDistance(Convert.ToDouble(itm.LATITUDE.Replace(".", ",")), Convert.ToDouble(itm.LONGITUDE.Replace(".", ",")), LATITUDE, LONGITUDE);
                            if (khoangcach <= itm.KHOANGCACH)
                            {
                                bolCheckDiaDiem = true;
                                strKhoangCach +=  itm.NAME + ": " + khoangcach.ToString("N0") + " m; ";
                            }
                            else
                            {
                                strKhoangCach += itm.NAME + ": " + khoangcach.ToString("N0") + " m; ";
                            }
                        }
                    }
                    if (!bolCheckDiaDiem)
                    {
                        ModelState.AddModelError(string.Empty, "Khảng cách xa với điểm được chỉ định chấm công!");
                        apiResponse.Message = "Khảng cách xa với điểm được chỉ định chấm công!"+ strKhoangCach;
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Không lấy được địa điểm chấm công!");
                    apiResponse.Message = "Không lấy được địa điểm chấm công!";
                }
                v_nv_ChamCong nv_ChamCong = new v_nv_ChamCong();
                nv_ChamCong.ID = ID;
                nv_ChamCong.NGAYCONG = Convert.ToDateTime(NGAYCONG);
                if (nv_ChamCong.NGAYCONG.Date != Utility.CurrentTime.Date)
                {
                    ModelState.AddModelError(string.Empty, "Ngày chấm công khác với ngày hiện tại!");
                    apiResponse.Message = "Ngày chấm công khác với ngày hiện tại!";
                }    
                    
                if (ModelState.IsValid)
                {

                    nv_ChamCong.LOC_ID = Utility.LOC_ID;
                    nv_ChamCong.ID_NHANVIEN = Session[Sessions.idUser].ToString();
                    nv_ChamCong.ID_NGUOISUA = Session[Sessions.idUser].ToString();
                    nv_ChamCong.THOIGIANSUA = Utility.CurrentTime;
                    nv_ChamCong.THOIGIANRA = Utility.CurrentTime;
                    nv_ChamCong.IP_CHAMCONGRA = MYPUBLICIPV4;
                    nv_ChamCong.GHICHU = strKhoangCach;
                    //@ConvertObjectUnicodeToTCVN3
                    apiResponse = Utility.Create<nv_ChamCong>(nv_ChamCong, API.nv_ChamCong + "/PostCheckOut");
                    if (apiResponse.Success)
                    {
                        apiResponse.Message = "Chấm công ra thành công! " + nv_ChamCong.THOIGIANRA.Value.ToString("dd/MM HH:mm:ss");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, apiResponse.Message);
                        apiResponse.Message = apiResponse.Message;
                    }
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                else
                {
                    apiResponse.Success = false;
                    apiResponse.Data = Utility.GetModelState(ModelState, API.nv_ChamCong);
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
                TempData["TitleError"] = API.TitleTryCatch;
                TempData["DetailError"] = ex.Message;
                return RedirectToAction("Index", "Notfound");
            }
        }

        // GET: Timekeeping/Create
        public ActionResult Create(int type = 2)
        {
            try
            {
                Session[Sessions.IntWidth] = type;
                if (Utility.KiemTra())
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (!Utility.KiemTraQuyen(API.nv_ChamCong, API.Create))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                v_v_nv_ChamCong nv_ChamCong = new v_v_nv_ChamCong();
                nv_ChamCong.LOC_ID = Utility.LOC_ID;
                nv_ChamCong.ID_NGUOITAO = Session[Sessions.idUser].ToString();
                nv_ChamCong.THOIGIANTHEM = Utility.CurrentTime;

                nv_ChamCong.ID = Guid.NewGuid().ToString();
                nv_ChamCong.lstdm_NhanVien = new List<ComboboxFrom>();
                nv_ChamCong.lstdm_NhanVien = Utility.GetListData<ComboboxFrom>(API.AspNetUser, "", "", "").Data as List<ComboboxFrom>;

                return View(nv_ChamCong);
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
                TempData["TitleError"] = API.TitleTryCatch;
                TempData["DetailError"] = ex.Message;
                return RedirectToAction("Index", "Notfound");
            }
        }

        // POST: Timekeeping/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "LOC_ID,ID,ID_NHANVIEN,ID_THANGLUONG,NGAYCONG,THOIGIANVAO,THOIGIANRA,SOTIENGLAMVIEC,GHICHU,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO,ISNGHIPHEP")] v_nv_ChamCong nv_ChamCong)
        {
            try
            {
                if (Utility.KiemTra())
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (!Utility.KiemTraQuyen(API.nv_ChamCong, API.Create))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                if (ModelState.IsValid)
                {
                    nv_ChamCong.LOC_ID = Utility.LOC_ID;
                    nv_ChamCong.ID_NGUOITAO = Session[Sessions.idUser].ToString();
                    nv_ChamCong.THOIGIANTHEM = Utility.CurrentTime;

                    //@ConvertObjectUnicodeToTCVN3
                    var apiResponse = Utility.Create<nv_ChamCong>(nv_ChamCong, API.nv_ChamCong);
                    if (apiResponse.Success)
                        return RedirectToAction("Index");
                    else
                    {
                        ModelState.AddModelError(string.Empty, apiResponse.Message);
                        if (apiResponse.CheckValue)
                            ViewBag.ID = Guid.NewGuid().ToString();
                    }
                }
                return View(nv_ChamCong);
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
                TempData["TitleError"] = API.TitleTryCatch;
                TempData["DetailError"] = ex.Message;
                return RedirectToAction("Index", "Notfound");
            }
        }

        // GET: Timekeeping/Edit/5
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
                if (!Utility.KiemTraQuyen(API.nv_ChamCong, API.Edit))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                v_v_nv_ChamCong nv_ChamCong = new v_v_nv_ChamCong();
                if (!string.IsNullOrEmpty(id))
                {
                    apiResponse = Utility.GetDetail<v_nv_ChamCong>(Utility.LOC_ID + "/" + id, API.nv_ChamCong);
                    if (!apiResponse.Success)
                    {
                        TempData["TitleError"] = apiResponse.Message;
                        return RedirectToAction("Index", "Notfound");
                    }
                    if (apiResponse.Data != null)
                        nv_ChamCong = apiResponse.Data as v_v_nv_ChamCong;
                }
                //@ConvertObjectTCVN3ToUnicode
                nv_ChamCong.lstdm_NhanVien = new List<ComboboxFrom>();
                nv_ChamCong.lstdm_NhanVien = Utility.GetListData<ComboboxFrom>(API.AspNetUser, "", "", "").Data as List<ComboboxFrom>;

                return View(nv_ChamCong);
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
                TempData["TitleError"] = API.TitleTryCatch;
                TempData["DetailError"] = ex.Message;
                return RedirectToAction("Index", "Notfound");
            }
        }

        // POST: Timekeeping/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "LOC_ID,ID,ID_NHANVIEN,ID_THANGLUONG,NGAYCONG,THOIGIANVAO,THOIGIANRA,SOTIENGLAMVIEC,GHICHU,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO,ISNGHIPHEP")] v_nv_ChamCong nv_ChamCong)
        {
            try
            {
                if (Utility.KiemTra())
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (!Utility.KiemTraQuyen(API.nv_ChamCong, API.Edit))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                if (ModelState.IsValid)
                {
                    nv_ChamCong.LOC_ID = Utility.LOC_ID;
                    nv_ChamCong.ID_NGUOISUA = Session[Sessions.idUser].ToString();
                    nv_ChamCong.THOIGIANSUA = Utility.CurrentTime;
                    //@ConvertObjectUnicodeToTCVN3
                    var apiResponse = Utility.Edit<v_nv_ChamCong>(Utility.LOC_ID + "/" + nv_ChamCong.ID, nv_ChamCong, API.nv_ChamCong);
                    if (apiResponse.Success)
                        return RedirectToAction("Index");
                    else
                        ModelState.AddModelError(string.Empty, apiResponse.Message);
                }
                return View(nv_ChamCong);
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
                TempData["TitleError"] = API.TitleTryCatch;
                TempData["DetailError"] = ex.Message;
                return RedirectToAction("Index", "Notfound");
            }
        }

        // POST: Timekeeping/Delete/5
        [HttpPost]
        public ActionResult Delete(string id)
        {
            try
            {
                if (Utility.KiemTra())
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (!Utility.KiemTraQuyen(API.nv_ChamCong, API.Delete))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                var apiResponse = Utility.Delete<v_nv_ChamCong>(Utility.LOC_ID + "/" + id, API.nv_ChamCong);
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
                    apiResponse.URL = Url.Action("Index", "Admin");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                if (!Utility.KiemTraQuyen(API.nv_ChamCong, API.Create))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                v_v_nv_ChamCong nv_ChamCong = new v_v_nv_ChamCong();
                apiResponse.Success = true;
                nv_ChamCong.LOC_ID = Utility.LOC_ID;
                nv_ChamCong.ID = Guid.NewGuid().ToString();
                nv_ChamCong.NGAYCONG = Utility.CurrentTime;
                nv_ChamCong.lstdm_NhanVien = new List<ComboboxFrom>();
                var lstdm_NhanVien = Utility.GetListData<ComboboxFrom>(API.AspNetUser, "", "", "").Data as List<ComboboxFrom>;
                foreach(var itm in lstdm_NhanVien)
                {
                    itm.ISACTIVE = true;
                }
                nv_ChamCong.lstdm_NhanVien = lstdm_NhanVien;
                apiResponse.Detail = Utility.ConvertobjectTo<v_v_nv_ChamCong>(nv_ChamCong);
                return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
                TempData["TitleError"] = API.TitleTryCatch;
                TempData["DetailError"] = ex.Message;
                apiResponse.URL = Url.Action("Index", "Notfound");
                return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
            }
        }
        [HttpPost]
        // GET: Menu/Create
        public ActionResult CreatePopupDate(string Date, string ID_TAIKHOAN)
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                if (Utility.KiemTra())
                {
                    apiResponse.URL = Url.Action("Index", "Admin");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                if (!Utility.KiemTraQuyen(API.nv_ChamCong, API.Create))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                v_v_nv_ChamCong nv_ChamCong = new v_v_nv_ChamCong();
                apiResponse.Success = true;
                nv_ChamCong.LOC_ID = Utility.LOC_ID;
                nv_ChamCong.ID = Guid.NewGuid().ToString();
                DateTime date = Convert.ToDateTime(Date);
                nv_ChamCong.NGAYCONG = date;
                nv_ChamCong.THOIGIANVAO = date.AddHours(8);
                nv_ChamCong.THOIGIANRA = date.AddHours(17);
                nv_ChamCong.ID_NHANVIEN = ID_TAIKHOAN;
                nv_ChamCong.lstdm_NhanVien = new List<ComboboxFrom>();
                var lstdm_NhanVien = Utility.GetListData<ComboboxFrom>(API.AspNetUser, "", "", "").Data as List<ComboboxFrom>;
                foreach (var itm in lstdm_NhanVien)
                {
                    itm.ISACTIVE = true;
                    if(itm.ID == ID_TAIKHOAN)
                        itm.ISDEFAULT = true;
                }    
                nv_ChamCong.lstdm_NhanVien = lstdm_NhanVien;
                apiResponse.Detail = Utility.ConvertobjectTo<v_v_nv_ChamCong>(nv_ChamCong);
                return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
                TempData["TitleError"] = API.TitleTryCatch;
                TempData["DetailError"] = ex.Message;
                apiResponse.URL = Url.Action("Index", "Notfound");
                return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
            }
        }

        // POST: Menu/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ValidateInput(false)]
        //[ValidateAntiForgeryToken]
        public ActionResult CreatePopup([Bind(Include = "BUTTONTYPE,LOC_ID,ID,ID_NHANVIEN,ID_THANGLUONG,NGAYCONG,THOIGIANVAO,THOIGIANRA,SOTIENGLAMVIEC,GHICHU,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO,ISNGHIPHEP")] v_nv_ChamCong nv_ChamCong)
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                if (Utility.KiemTra())
                {
                    apiResponse.URL = Url.Action("Index", "Admin");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                if (!Utility.KiemTraQuyen(API.nv_ChamCong, API.Create))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                if (ModelState.IsValid)
                {
                    nv_ChamCong.LOC_ID = Utility.LOC_ID;
                    nv_ChamCong.ID_NGUOITAO = Session[Sessions.idUser].ToString();
                    nv_ChamCong.THOIGIANTHEM = Utility.CurrentTime;

                    apiResponse = Utility.Create<nv_ChamCong>(nv_ChamCong, API.nv_ChamCong);
                    if (apiResponse.Success)
                    {
                        apiResponse.NewID = Guid.NewGuid().ToString();
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
                    apiResponse.Data = Utility.GetModelState(ModelState, API.nv_ChamCong);
                }
                apiResponse.ID = nv_ChamCong.ID;
                var lst = Utility.ConvertobjectTo<nv_ChamCong>(nv_ChamCong);
                if (nv_ChamCong.BUTTONTYPE == "TABLETIME")
                {
                    lst.Clear();
                    var lstdm_ThangLuong_Table = Utility.GetListData<dm_ThangLuong>(API.dm_ThangLuong, "", "", Utility.LOC_ID).Data as List<dm_ThangLuong>;
                    string MauVang = "#FFFF00";//Màu vàng(Đi trễ, về sớm)
                    string MauXanh = "#0033FF;color: white;";//Màu xanh(Done)
                    string Mau = "";
                    if(lstdm_ThangLuong_Table != null)
                    {
                        dm_ThangLuong v_dm_ThangLuong = lstdm_ThangLuong_Table.Where(s => s.THANG == nv_ChamCong.NGAYCONG.Month && s.NAM == nv_ChamCong.NGAYCONG.Year && s.NGAYBATDAU <= nv_ChamCong.NGAYCONG && s.NGAYKETTHUC >= nv_ChamCong.NGAYCONG && s.ISACTIVE).FirstOrDefault();
                        if (v_dm_ThangLuong != null)
                        {
                            if (v_dm_ThangLuong != null)
                            {
                                if (nv_ChamCong.THOIGIANVAO.Value.TimeOfDay > v_dm_ThangLuong.GIOBATDAU)
                                {
                                    Mau = MauVang;
                                }

                                if (nv_ChamCong.THOIGIANRA.Value.TimeOfDay < v_dm_ThangLuong.GIOKETTHUC)
                                {
                                    Mau = MauVang;
                                }

                                if (string.IsNullOrEmpty(Mau))
                                {
                                    Mau = MauXanh;
                                }
                            }
                            lst.Add(new ValueEdit { Key = nv_ChamCong.ID_NHANVIEN + "-" + nv_ChamCong.NGAYCONG.ToString("dd/MM/yyyy"), Value = ("<button style=\"width:70px;height:50px;background-color:" + Mau + ";\" id=\"" + nv_ChamCong.ID_NHANVIEN + "-" + nv_ChamCong.NGAYCONG.ToString("dd/MM/yyy") + "\" ondblclick=\"myFunctionEdit('" + API.nv_ChamCong + "','" + nv_ChamCong.ID + "')\">" + (nv_ChamCong.THOIGIANVAO.Value.ToString("HH:mm") + "<br>" + nv_ChamCong.THOIGIANRA.Value.ToString("HH:mm")) + "</button>") });
                        } 
                    }
                    apiResponse.ID = nv_ChamCong.ID_NHANVIEN;
                    apiResponse.MAPHIEU = nv_ChamCong.ID;
                }
                apiResponse.Detail = lst;
                return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
                TempData["TitleError"] = API.TitleTryCatch;
                TempData["DetailError"] = ex.Message;
                apiResponse.URL = Url.Action("Index", "Notfound");
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
                    apiResponse.URL = Url.Action("Index", "Admin");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                if (!Utility.KiemTraQuyen(API.nv_ChamCong, API.Edit))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                v_v_nv_ChamCong nv_ChamCong = new v_v_nv_ChamCong();
                if (!string.IsNullOrEmpty(id))
                {
                    apiResponse = Utility.GetDetail<v_v_nv_ChamCong>(Utility.LOC_ID + "/" + id, API.nv_ChamCong);
                    if (!apiResponse.Success)
                    {
                        TempData["TitleError"] = apiResponse.Message;
                        apiResponse.URL = Url.Action("Index", "Notfound");
                        return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                    }
                    if (apiResponse.Data != null)
                        nv_ChamCong = apiResponse.Data as v_v_nv_ChamCong;
                }
                apiResponse.Success = true;
                nv_ChamCong.lstdm_NhanVien = new List<ComboboxFrom>();
                var lstdm_NhanVien = Utility.GetListData<ComboboxFrom>(API.AspNetUser, "", "", "").Data as List<ComboboxFrom>;
                foreach (var itm in lstdm_NhanVien)
                {
                    itm.ISACTIVE = true;
                }  
                nv_ChamCong.lstdm_NhanVien = lstdm_NhanVien;
                apiResponse.Detail = Utility.ConvertobjectTo<v_v_nv_ChamCong>(nv_ChamCong);
                return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
                TempData["TitleError"] = API.TitleTryCatch;
                TempData["DetailError"] = ex.Message;
                apiResponse.URL = Url.Action("Index", "Notfound");
                return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
            }
        }

        // POST: Menu/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ValidateInput(false)]
        //[ValidateAntiForgeryToken]
        public ActionResult EditPopup([Bind(Include = "BUTTONTYPE,LOC_ID,ID,ID_NHANVIEN,ID_THANGLUONG,NGAYCONG,THOIGIANVAO,THOIGIANRA,SOTIENGLAMVIEC,GHICHU,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO,ISNGHIPHEP")] v_nv_ChamCong nv_ChamCong)
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                if (Utility.KiemTra())
                {
                    apiResponse.URL = Url.Action("Index", "Admin");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                if (!Utility.KiemTraQuyen(API.nv_ChamCong, API.Edit))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                if (ModelState.IsValid)
                {
                    nv_ChamCong.LOC_ID = Utility.LOC_ID;
                    nv_ChamCong.ID_NGUOISUA = Session[Sessions.idUser].ToString();
                    nv_ChamCong.THOIGIANSUA = Utility.CurrentTime;

                    apiResponse = Utility.Edit<v_nv_ChamCong>(Utility.LOC_ID + "/" + nv_ChamCong.ID, nv_ChamCong, API.nv_ChamCong);
                    if (apiResponse.Success)
                    {
                        //return RedirectToAction("Index");
                        apiResponse.ID = nv_ChamCong.ID;
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, apiResponse.Message);
                    }
                }
                else
                {
                    apiResponse.Success = false;
                    apiResponse.Data = Utility.GetModelState(ModelState, API.nv_ChamCong);
                }
                var lst = Utility.ConvertobjectTo<nv_ChamCong>(nv_ChamCong);
                if (nv_ChamCong.BUTTONTYPE == "TABLETIME")
                {
                    lst.Clear();
                    var lstdm_ThangLuong_Table = Utility.GetListData<dm_ThangLuong>(API.dm_ThangLuong, "", "", Utility.LOC_ID).Data as List<dm_ThangLuong>;
                    string MauVang = "#FFFF00";//Màu vàng(Đi trễ, về sớm)
                    string MauXanh = "#0033FF;color: white;";//Màu xanh(Done)
                    string Mau = "";
                    if (lstdm_ThangLuong_Table != null)
                    {
                        dm_ThangLuong v_dm_ThangLuong = lstdm_ThangLuong_Table.Where(s => s.THANG == nv_ChamCong.NGAYCONG.Month && s.NAM == nv_ChamCong.NGAYCONG.Year && s.NGAYBATDAU <= nv_ChamCong.NGAYCONG && s.NGAYKETTHUC >= nv_ChamCong.NGAYCONG && s.ISACTIVE).FirstOrDefault();
                        if (v_dm_ThangLuong != null)
                        {
                            if (v_dm_ThangLuong != null)
                            {
                                if (nv_ChamCong.THOIGIANVAO.Value.TimeOfDay > v_dm_ThangLuong.GIOBATDAU)
                                {
                                    Mau = MauVang;
                                }

                                if (nv_ChamCong.THOIGIANRA.Value.TimeOfDay < v_dm_ThangLuong.GIOKETTHUC)
                                {
                                    Mau = MauVang;
                                }

                                if (string.IsNullOrEmpty(Mau))
                                {
                                    Mau = MauXanh;
                                }
                            }
                            lst.Add(new ValueEdit { Key = nv_ChamCong.ID_NHANVIEN + "-" + nv_ChamCong.NGAYCONG.ToString("dd/MM/yyyy"), Value = ("<button style=\"width:70px;height:50px;background-color:" + Mau + ";\" id=\"" + nv_ChamCong.ID_NHANVIEN + "-" + nv_ChamCong.NGAYCONG.ToString("dd/MM/yyy") + "\" ondblclick=\"myFunctionEdit('" + API.nv_ChamCong + "','" + nv_ChamCong.ID + "')\">" + (nv_ChamCong.THOIGIANVAO.Value.ToString("HH:mm") + "<br>" + nv_ChamCong.THOIGIANRA.Value.ToString("HH:mm")) + "</button>") });
                        }
                    }
                    //lst.Add(new ValueEdit { Key = nv_ChamCong.ID_NHANVIEN + "-" + nv_ChamCong.NGAYCONG.ToString("dd/MM/yyyy"), Value = (nv_ChamCong.THOIGIANVAO.Value.ToString("HH:mm") + "<br>" + nv_ChamCong.THOIGIANRA.Value.ToString("HH:mm")) });
                    apiResponse.ID = nv_ChamCong.ID_NHANVIEN;
                    
                }
               
                apiResponse.Detail = lst;
                return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
                TempData["TitleError"] = API.TitleTryCatch;
                TempData["DetailError"] = ex.Message;
                apiResponse.URL = Url.Action("Index", "Notfound");
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
                    apiResponse.URL = Url.Action("Index", "Admin");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                if (!Utility.KiemTraQuyen(API.nv_ChamCong, API.Delete))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                apiResponse = Utility.Delete<v_nv_ChamCong>(Utility.LOC_ID + "/" + id, API.nv_ChamCong);
                apiResponse.ID = id;
                return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
                TempData["TitleError"] = API.TitleTryCatch;
                TempData["DetailError"] = ex.Message;
                apiResponse.URL = Url.Action("Index", "Notfound");
                return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
            }
        }
        #endregion
    }
}