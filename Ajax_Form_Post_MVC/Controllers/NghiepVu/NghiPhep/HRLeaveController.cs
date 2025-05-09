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

namespace MVC_QuanLyTHP.Controllers
{
    public class HRLeaveController : Controller
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
                if (!Utility.KiemTraQuyen(API.nv_NghiPhep, API.Xem))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                ApiResponse apiResponse = new ApiResponse();
                IPagedList<v_nv_NghiPhep> lstpage = (new List<v_nv_NghiPhep>()).ToList().ToPagedList(1, Utility.GetPageSize());
                v_v_nv_NghiPhep nv_NghiPhep = new v_v_nv_NghiPhep();
                nv_NghiPhep.IPagedList = lstpage;
                nv_NghiPhep.lstdm_NhanVien = new List<ComboboxFrom>();
                nv_NghiPhep.lstdm_NhanVien = Utility.GetListData<ComboboxFrom>(API.AspNetUser, "", "", "").Data as List<ComboboxFrom>;
                nv_NghiPhep.lstnv_PhepNam = new List<ComboboxFrom>();
                nv_NghiPhep.TUNGAY = DateTime.Now;
                nv_NghiPhep.DENNGAY = DateTime.Now;
                ViewBag.PermissionEdit = Utility.KiemTraQuyen(API.nv_NghiPhep, API.Edit);
                ViewBag.PermissionDelete = Utility.KiemTraQuyen(API.nv_NghiPhep, API.Delete);
                ViewBag.PermissionCreate = Utility.KiemTraQuyen(API.nv_NghiPhep, API.Create);
                ViewBag.PermissionCreateUser = Utility.KiemTraQuyen(API.nv_NghiPhep, API.CreateUser);
                ViewBag.PermissionApproveLeave = Utility.KiemTraQuyen(API.nv_NghiPhep, API.ApproveLeave);
                if (!ViewBag.PermissionCreateUser)
                    nv_NghiPhep.lstdm_NhanVien = nv_NghiPhep.lstdm_NhanVien.Where(s => s.ID == Session[Sessions.idUser].ToString()).ToList();
                return View(nv_NghiPhep);
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
                if (!Utility.KiemTraQuyen(API.nv_NghiPhep, API.Xem))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                ApiResponse apiResponse = new ApiResponse();
                IPagedList<v_nv_NghiPhep> lstpage = (new List<v_nv_NghiPhep>()).ToList().ToPagedList(1, Utility.GetPageSize());
                apiResponse = Utility.Get_DanhSachNghiPhep<v_nv_NghiPhep>(objParameter.TUNGAY, objParameter.DENNGAY, null, objParameter.KEY, objParameter.ID_NHANVIEN);
                if (!apiResponse.Success)
                {
                    TempData["TitleError"] = apiResponse.Message;
                    return RedirectToAction("Index", "Notfound");
                }

                var lst = apiResponse.Data as List<v_nv_NghiPhep>;
                if (Utility.KiemTraQuyen(API.nv_NghiPhep, API.AllData) && lst.Count() > 0)
                {
                    lstpage = lst.OrderByDescending(s => s.THOIGIANVAO).ToList().ToPagedList(1, lst.Count());
                }
                else
                {
                    var Login_Model = (Login_Model)Session[Sessions.Login_Model];
                    if (Utility.KiemTraQuyen(API.nv_NghiPhep, API.UserData) && lst.Count() > 0)
                        lstpage = lst.Where(s => s.ID_NHANVIEN == Login_Model.iduser).OrderByDescending(s => s.THOIGIANVAO).ToList().ToPagedList(1, lst.Count());
                }
                ViewBag.PermissionCreateUser = Utility.KiemTraQuyen(API.nv_NghiPhep, API.CreateUser);
                ViewBag.PermissionApproveLeave = Utility.KiemTraQuyen(API.nv_NghiPhep, API.ApproveLeave);
                //lstpage = lst.ToPagedList(1, lst.Count() > 0 ? lst.Count() : 50);
                v_v_nv_NghiPhep nv_NghiPhep = new v_v_nv_NghiPhep();
                nv_NghiPhep.IPagedList = lstpage;
                nv_NghiPhep.lstdm_NhanVien = new List<ComboboxFrom>();
                nv_NghiPhep.lstdm_NhanVien = Utility.GetListData<ComboboxFrom>(API.AspNetUser, "", "", "").Data as List<ComboboxFrom>;
                nv_NghiPhep.lstnv_PhepNam = new List<ComboboxFrom>();
                
                nv_NghiPhep.TUNGAY = objParameter.TUNGAY != null ? objParameter.TUNGAY.Value : Utility.CurrentTime;
                nv_NghiPhep.DENNGAY = objParameter.DENNGAY != null ? objParameter.DENNGAY.Value : Utility.CurrentTime;
                if (!ViewBag.PermissionCreateUser)
                    nv_NghiPhep.ID_NHANVIEN = Session[Sessions.idUser].ToString();
                else
                    nv_NghiPhep.ID_NHANVIEN = objParameter.ID_NHANVIEN;
                ViewBag.PermissionEdit = Utility.KiemTraQuyen(API.nv_NghiPhep, API.Edit);
                ViewBag.PermissionDelete = Utility.KiemTraQuyen(API.nv_NghiPhep, API.Delete);
                ViewBag.PermissionCreate = Utility.KiemTraQuyen(API.nv_NghiPhep, API.Create);
                
                if (!ViewBag.PermissionCreateUser)
                {
                    nv_NghiPhep.lstdm_NhanVien = nv_NghiPhep.lstdm_NhanVien.Where(s => s.ID == Session[Sessions.idUser].ToString()).ToList();
                }
                    
                    
                return View(nv_NghiPhep);
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
                if (!Utility.KiemTraQuyen(API.nv_NghiPhep, API.Create))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                ViewBag.PermissionCreateUser = Utility.KiemTraQuyen(API.nv_NghiPhep, API.CreateUser);
                v_v_nv_NghiPhep nv_NghiPhep = new v_v_nv_NghiPhep();
                nv_NghiPhep.LOC_ID = Utility.LOC_ID;
                nv_NghiPhep.ID_NGUOITAO = Session[Sessions.idUser].ToString();
                nv_NghiPhep.THOIGIANTHEM = Utility.CurrentTime;
                nv_NghiPhep.ID_NHANVIEN = Session[Sessions.idUser].ToString();
                nv_NghiPhep.ID = Guid.NewGuid().ToString();
                nv_NghiPhep.lstdm_NhanVien = new List<ComboboxFrom>();
                nv_NghiPhep.lstnv_PhepNam = new List<ComboboxFrom>();
                ViewBag.PermissionCreateUser = Utility.KiemTraQuyen(API.nv_NghiPhep, API.CreateUser);
                return View(nv_NghiPhep);
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
        public ActionResult Create([Bind(Include = "LOC_ID,ID,ID_NHANVIEN,ID_THANGLUONG,THOIGIANVAO,THOIGIANRA,GHICHU,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO,ISNGHIPHEP,SOLUONG,ISDUYETPHEP,THOIGIANDUYETPHEP,ID_NGUOIDUYETPHEP,HINHTHUCNGHIPHEP,ID_PHEPNAM")] v_nv_NghiPhep nv_NghiPhep)
        {
            try
            {
                if (Utility.KiemTra())
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (!Utility.KiemTraQuyen(API.nv_NghiPhep, API.Create))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                if (ModelState.IsValid)
                {
                    nv_NghiPhep.LOC_ID = Utility.LOC_ID;
                    nv_NghiPhep.ID_NGUOITAO = Session[Sessions.idUser].ToString();
                    nv_NghiPhep.THOIGIANTHEM = Utility.CurrentTime;

                    //@ConvertObjectUnicodeToTCVN3
                    var apiResponse = Utility.Create<nv_NghiPhep>(nv_NghiPhep, API.nv_NghiPhep);
                    if (apiResponse.Success)
                        return RedirectToAction("Index");
                    else
                    {
                        ModelState.AddModelError(string.Empty, apiResponse.Message);
                        if (apiResponse.CheckValue)
                            ViewBag.ID = Guid.NewGuid().ToString();
                    }
                }
                return View(nv_NghiPhep);
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
                if (!Utility.KiemTraQuyen(API.nv_NghiPhep, API.Edit))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                v_v_nv_NghiPhep nv_NghiPhep = new v_v_nv_NghiPhep();
                if (!string.IsNullOrEmpty(id))
                {
                    apiResponse = Utility.GetDetail<v_nv_NghiPhep>(Utility.LOC_ID + "/" + id, API.nv_NghiPhep);
                    if (!apiResponse.Success)
                    {
                        TempData["TitleError"] = apiResponse.Message;
                        return RedirectToAction("Index", "Notfound");
                    }
                    if (apiResponse.Data != null)
                        nv_NghiPhep = apiResponse.Data as v_v_nv_NghiPhep;
                }
                //@ConvertObjectTCVN3ToUnicode
                nv_NghiPhep.lstdm_NhanVien = new List<ComboboxFrom>();
                nv_NghiPhep.lstdm_NhanVien = Utility.GetListData<ComboboxFrom>(API.AspNetUser, "", "", "").Data as List<ComboboxFrom>;
                nv_NghiPhep.lstdm_NhanVien = nv_NghiPhep.lstdm_NhanVien.Where(s => s.ID == nv_NghiPhep.ID_NHANVIEN).ToList();
                return View(nv_NghiPhep);
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
        public ActionResult Edit([Bind(Include = "LOC_ID,ID,ID_NHANVIEN,ID_THANGLUONG,THOIGIANVAO,THOIGIANRA,GHICHU,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO,ISNGHIPHEP,SOLUONG,ISDUYETPHEP,THOIGIANDUYETPHEP,ID_NGUOIDUYETPHEP,HINHTHUCNGHIPHEP,ID_PHEPNAM")] v_nv_NghiPhep nv_NghiPhep)
        {
            try
            {
                if (Utility.KiemTra())
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (!Utility.KiemTraQuyen(API.nv_NghiPhep, API.Edit))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                if (ModelState.IsValid)
                {
                    nv_NghiPhep.LOC_ID = Utility.LOC_ID;
                    nv_NghiPhep.ID_NGUOISUA = Session[Sessions.idUser].ToString();
                    nv_NghiPhep.THOIGIANSUA = Utility.CurrentTime;
                    //@ConvertObjectUnicodeToTCVN3
                    var apiResponse = Utility.Edit<v_nv_NghiPhep>(Utility.LOC_ID + "/" + nv_NghiPhep.ID, nv_NghiPhep, API.nv_NghiPhep);
                    if (apiResponse.Success)
                        return RedirectToAction("Index");
                    else
                        ModelState.AddModelError(string.Empty, apiResponse.Message);
                }
                return View(nv_NghiPhep);
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
                if (!Utility.KiemTraQuyen(API.nv_NghiPhep, API.Delete))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                var apiResponse = Utility.Delete<v_nv_NghiPhep>(Utility.LOC_ID + "/" + id, API.nv_NghiPhep);
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
                if (!Utility.KiemTraQuyen(API.nv_NghiPhep, API.Create))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                ViewBag.PermissionCreateUser = Utility.KiemTraQuyen(API.nv_NghiPhep, API.CreateUser);
                v_v_nv_NghiPhep nv_NghiPhep = new v_v_nv_NghiPhep();
                apiResponse.Success = true;
                nv_NghiPhep.LOC_ID = Utility.LOC_ID;
                nv_NghiPhep.ID = Guid.NewGuid().ToString();
                nv_NghiPhep.HINHTHUCNGHIPHEP = (int)API.HinhThucNghiPhep.NguyenNgay;
                nv_NghiPhep.THOIGIANVAO = Utility.CurrentTime;
                nv_NghiPhep.THOIGIANRA = Utility.CurrentTime;
                nv_NghiPhep.lstdm_NhanVien = new List<ComboboxFrom>();
                var lstdm_NhanVien = Utility.GetListData<ComboboxFrom>(API.AspNetUser, "", "", "").Data as List<ComboboxFrom>;
                foreach(var itm in lstdm_NhanVien)
                {
                    itm.ISACTIVE = true;
                    if (itm.ID == Session[Sessions.idUser].ToString())
                        itm.ISDEFAULT = true;
                }

                if (!ViewBag.PermissionCreateUser)
                {
                    var lstnv_PhepNam = Utility.GetListData<v_nv_PhepNam>(API.nv_PhepNam, "", "", "").Data as List<v_nv_PhepNam>;
                    if (lstnv_PhepNam != null)
                        lstnv_PhepNam = lstnv_PhepNam.Where(s => s.ID_NHANVIEN == Session[Sessions.idUser].ToString()).ToList();
                    else
                        lstnv_PhepNam = new List<v_nv_PhepNam>();
                    bool bolISDEFAULT = false;
                    var lstnv_PhepNamnew = new List<ComboboxFrom>();
                    foreach (var itm in lstnv_PhepNam.OrderBy(s => s.NAM))
                    {
                        if ((itm.SONGAYPHEP - itm.SONGAYPHEPDADUNG > 0 || itm.NAM == Utility.CurrentTime.Year) && itm.NGAYBATDAU <= Utility.CurrentTime && itm.NGAYKETTHUC >= Utility.CurrentTime)
                        {
                            ComboboxFrom newComboboxFrom = new ComboboxFrom();
                            newComboboxFrom.ID = itm.ID;
                            newComboboxFrom.NAME = itm.NAM.ToString() + "(" + (itm.SONGAYPHEP - itm.SONGAYPHEPDADUNG).ToString() + " ngày)";
                            newComboboxFrom.ISACTIVE = true;
                            if(bolISDEFAULT)
                                bolISDEFAULT = newComboboxFrom.ISDEFAULT = true;
                            lstnv_PhepNamnew.Add(newComboboxFrom);
                        }
                    }
                    nv_NghiPhep.lstnv_PhepNam = lstnv_PhepNamnew;
                    nv_NghiPhep.lstdm_NhanVien = lstdm_NhanVien.Where(s => s.ID == Session[Sessions.idUser].ToString()).ToList();
                }
                else
                {
                    nv_NghiPhep.lstdm_NhanVien = lstdm_NhanVien;
                }
                apiResponse.Detail = Utility.ConvertobjectTo<v_v_nv_NghiPhep>(nv_NghiPhep);
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
        [ValidateAntiForgeryToken]
        public ActionResult CreatePopup([Bind(Include = "LOC_ID,ID,ID_NHANVIEN,ID_THANGLUONG,THOIGIANVAO,THOIGIANRA,GHICHU,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO,ISNGHIPHEP,SOLUONG,ISDUYETPHEP,THOIGIANDUYETPHEP,ID_NGUOIDUYETPHEP,HINHTHUCNGHIPHEP,ID_PHEPNAM")] v_nv_NghiPhep nv_NghiPhep)
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                if (Utility.KiemTra())
                {
                    apiResponse.URL = Url.Action("Index", "Admin");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                if (!Utility.KiemTraQuyen(API.nv_NghiPhep, API.Create))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                if (nv_NghiPhep.HINHTHUCNGHIPHEP == (int)API.HinhThucNghiPhep.NguyenNgay && nv_NghiPhep.THOIGIANVAO > nv_NghiPhep.THOIGIANRA)
                    ModelState.AddModelError("THOIGIANRA", "Sai thời gian ra");
                if (ModelState.IsValid)
                {
                    TimeSpan variable = nv_NghiPhep.THOIGIANRA - nv_NghiPhep.THOIGIANVAO;
                    if (nv_NghiPhep.HINHTHUCNGHIPHEP == (int)API.HinhThucNghiPhep.NguyenNgay)
                        nv_NghiPhep.SOLUONG = variable.Days + 1;
                    else
                    {
                        nv_NghiPhep.THOIGIANRA = nv_NghiPhep.THOIGIANVAO;
                        nv_NghiPhep.SOLUONG = 0.5;
                    }

                    nv_NghiPhep.LOC_ID = Utility.LOC_ID;
                    nv_NghiPhep.ID_NGUOITAO = Session[Sessions.idUser].ToString();
                    nv_NghiPhep.THOIGIANTHEM = Utility.CurrentTime;

                    apiResponse = Utility.Create<nv_NghiPhep>(nv_NghiPhep, API.nv_NghiPhep);
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
                    apiResponse.Data = Utility.GetModelState(ModelState, API.nv_NghiPhep);
                }
                apiResponse.ID = nv_NghiPhep.ID;
                apiResponse.Detail = Utility.ConvertobjectTo<v_nv_NghiPhep>(nv_NghiPhep);
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
                if (!Utility.KiemTraQuyen(API.nv_NghiPhep, API.Edit))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                v_v_nv_NghiPhep nv_NghiPhep = new v_v_nv_NghiPhep();
                if (!string.IsNullOrEmpty(id))
                {
                    apiResponse = Utility.GetDetail<v_v_nv_NghiPhep>(Utility.LOC_ID + "/" + id, API.nv_NghiPhep);
                    if (!apiResponse.Success)
                    {
                        TempData["TitleError"] = apiResponse.Message;
                        apiResponse.URL = Url.Action("Index", "Notfound");
                        return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                    }
                    if (apiResponse.Data != null)
                        nv_NghiPhep = apiResponse.Data as v_v_nv_NghiPhep;
                }
                apiResponse.Success = true;
                nv_NghiPhep.lstdm_NhanVien = new List<ComboboxFrom>();
                nv_NghiPhep.lstdm_NhanVien = Utility.GetListData<ComboboxFrom>(API.AspNetUser, "", "", "").Data as List<ComboboxFrom>;
                nv_NghiPhep.lstdm_NhanVien = nv_NghiPhep.lstdm_NhanVien.Where(s => s.ID == nv_NghiPhep.ID_NHANVIEN).ToList();
                var lstnv_PhepNamnew = new List<ComboboxFrom>();
                var lstnv_PhepNam = Utility.GetListData<v_nv_PhepNam>(API.nv_PhepNam, "", "", Utility.LOC_ID).Data as List<v_nv_PhepNam>;
                if (lstnv_PhepNam != null)
                    lstnv_PhepNam = lstnv_PhepNam.Where(s => s.ID_NHANVIEN == nv_NghiPhep.ID_NHANVIEN).ToList();
                else
                    lstnv_PhepNam = new List<v_nv_PhepNam>();

                foreach (var itm in lstnv_PhepNam.OrderBy(s => s.NAM))
                {
                    ComboboxFrom newComboboxFrom = new ComboboxFrom();
                    newComboboxFrom.ID = itm.ID;
                    newComboboxFrom.NAME = itm.NAM.ToString() + "(" + (itm.SONGAYPHEP - itm.SONGAYPHEPDADUNG).ToString() + " ngày)";
                    newComboboxFrom.ISACTIVE = true;
                    lstnv_PhepNamnew.Add(newComboboxFrom);
                }
                nv_NghiPhep.lstnv_PhepNam = lstnv_PhepNamnew;
                apiResponse.Detail = Utility.ConvertobjectTo<nv_NghiPhep>(nv_NghiPhep);
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
        [ValidateAntiForgeryToken]
        public ActionResult EditPopup([Bind(Include = "LOC_ID,ID,ID_NHANVIEN,ID_THANGLUONG,THOIGIANVAO,THOIGIANRA,GHICHU,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO,ISNGHIPHEP,SOLUONG,ISDUYETPHEP,THOIGIANDUYETPHEP,ID_NGUOIDUYETPHEP,HINHTHUCNGHIPHEP,ID_PHEPNAM")] v_nv_NghiPhep nv_NghiPhep)
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                if (Utility.KiemTra())
                {
                    apiResponse.URL = Url.Action("Index", "Admin");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                if (!Utility.KiemTraQuyen(API.nv_NghiPhep, API.Edit))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                if (nv_NghiPhep.HINHTHUCNGHIPHEP == (int)API.HinhThucNghiPhep.NguyenNgay && nv_NghiPhep.THOIGIANVAO > nv_NghiPhep.THOIGIANRA)
                    ModelState.AddModelError("THOIGIANRA", "Sai thời gian ra");
                if (ModelState.IsValid)
                {
                    TimeSpan variable = nv_NghiPhep.THOIGIANRA - nv_NghiPhep.THOIGIANVAO;
                    if (nv_NghiPhep.HINHTHUCNGHIPHEP == (int)API.HinhThucNghiPhep.NguyenNgay)
                        nv_NghiPhep.SOLUONG = variable.Days + 1;
                    else
                    {
                        nv_NghiPhep.THOIGIANRA = nv_NghiPhep.THOIGIANVAO;
                        nv_NghiPhep.SOLUONG = 0.5;
                    }    
                        
                    nv_NghiPhep.LOC_ID = Utility.LOC_ID;
                    nv_NghiPhep.ID_NGUOISUA = Session[Sessions.idUser].ToString();
                    nv_NghiPhep.THOIGIANSUA = Utility.CurrentTime;
                    if(nv_NghiPhep.ISDUYETPHEP)
                        nv_NghiPhep.ID_NGUOIDUYETPHEP = Session[Sessions.idUser].ToString();
                    apiResponse = Utility.Edit<v_nv_NghiPhep>(Utility.LOC_ID + "/" + nv_NghiPhep.ID, nv_NghiPhep, API.nv_NghiPhep);
                    if (apiResponse.Success)
                    {
                        //return RedirectToAction("Index");
                        apiResponse.ID = nv_NghiPhep.ID;
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, apiResponse.Message);
                    }
                }
                else
                {
                    apiResponse.Success = false;
                    apiResponse.Data = Utility.GetModelState(ModelState, API.nv_NghiPhep);
                }
                apiResponse.Detail = Utility.ConvertobjectTo<v_nv_NghiPhep>(nv_NghiPhep, "dd/MM/yyyy");
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
                if (!Utility.KiemTraQuyen(API.nv_NghiPhep, API.Delete))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                apiResponse = Utility.Delete<v_nv_NghiPhep>(Utility.LOC_ID + "/" + id, API.nv_NghiPhep);
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
        [HttpPost]
        public ActionResult CallChangeEmployee(string id)
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                if (Utility.KiemTra())
                {
                    apiResponse.URL = Url.Action("Index", "Admin");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                var lstnv_PhepNam = Utility.GetListData<v_nv_PhepNam>(API.nv_PhepNam, "", "", Utility.LOC_ID).Data as List<v_nv_PhepNam>;
                if (lstnv_PhepNam != null)
                    lstnv_PhepNam = lstnv_PhepNam.Where(s => s.ID_NHANVIEN == id).ToList();
                else
                    lstnv_PhepNam = new List<v_nv_PhepNam>();
                var lstnv_PhepNamnew = new List<ComboboxFrom>();
                foreach (var itm in lstnv_PhepNam.OrderBy(s => s.NAM))
                {
                    if ((itm.SONGAYPHEP - itm.SONGAYPHEPDADUNG > 0 || itm.NAM == Utility.CurrentTime.Year) && itm.NGAYBATDAU <= Utility.CurrentTime && itm.NGAYKETTHUC >= Utility.CurrentTime)
                    {
                        ComboboxFrom newComboboxFrom = new ComboboxFrom();
                        newComboboxFrom.ID = itm.ID;
                        newComboboxFrom.NAME = itm.NAM.ToString() + "(" + (itm.SONGAYPHEP - itm.SONGAYPHEPDADUNG).ToString() + " ngày)";
                        newComboboxFrom.ISACTIVE = true;
                        lstnv_PhepNamnew.Add(newComboboxFrom);
                    }    
                }
                List<ValueEdit> lstValueEdit = new List<ValueEdit>();
                ValueEdit newValueEdit = new ValueEdit();
                newValueEdit.Key = "lstnv_PhepNam";
                newValueEdit.Value = lstnv_PhepNamnew;
                lstValueEdit.Add(newValueEdit);
                apiResponse.Detail = lstValueEdit;
                apiResponse.Success = true;
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

    }
}