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
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace MVC_QuanLyTHP.Controllers
{
    public class UserController : Controller
    {

        // GET: User
        public ActionResult Index(int Page = 1, string SearchString = "", string ShowSearchValue = "")
        {
            try
            {
                if (Utility.KiemTra())
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (!Utility.KiemTraQuyen(API.AspNetUser, API.Xem))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                ShowSearchValue = Utility.GetShowSearchValue<AspNetUsers>(ShowSearchValue);
                var apiResponse = Utility.GetListData<v_AspNetUsers>(API.AspNetUser, ShowSearchValue, SearchString);
                if (!apiResponse.Success)
                {
                    TempData["TitleError"] = apiResponse.Message;
                    return RedirectToAction("Index", "Notfound");
                }
                IPagedList<v_AspNetUsers> lstpage = (apiResponse.Data as List<v_AspNetUsers>).ToPagedList(Page, Utility.GetPageSize());

                v_v_AspNetUsers AspNetUser = new v_v_AspNetUsers();
                AspNetUser.IPagedList = lstpage;
                AspNetUser.lstweb_NhomQuyen = new List<web_NhomQuyen>();
                AspNetUser.lstweb_NhomQuyen = Utility.GetListData<web_NhomQuyen>(API.web_NhomQuyen, "", "", Utility.LOC_ID).Data as List<web_NhomQuyen>;


                ViewBag.searchValue = SearchString;
                ViewBag.showsearchValue = ShowSearchValue;

                ViewBag.PermissionEdit = Utility.KiemTraQuyen(API.AspNetUser, API.Edit);
                ViewBag.PermissionDelete = Utility.KiemTraQuyen(API.AspNetUser, API.Delete);
                ViewBag.PermissionCreate = Utility.KiemTraQuyen(API.AspNetUser, API.Create);
                return View(AspNetUser);
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
                TempData["TitleError"] = API.TitleTryCatch;
                TempData["DetailError"] = ex.Message;
                return RedirectToAction("Index", "Notfound");
            }
        }

     
        // GET: User/Create
        public ActionResult Create(int type = 2)
        {
            try
            {
                Session[Sessions.IntWidth] = type;
                if (Utility.KiemTra())
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (!Utility.KiemTraQuyen(API.AspNetUser, API.Create))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                v_v_AspNetUsers AspNetUser = new v_v_AspNetUsers();

                AspNetUser.ID = Guid.NewGuid().ToString();
                AspNetUser.lstweb_NhomQuyen = new List<web_NhomQuyen>();
                AspNetUser.lstweb_NhomQuyen = Utility.GetListData<web_NhomQuyen>(API.web_NhomQuyen, "", "", Utility.LOC_ID).Data as List<web_NhomQuyen>;
                return View(AspNetUser);
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
                TempData["TitleError"] = API.TitleTryCatch;
                TempData["DetailError"] = ex.Message;
                return RedirectToAction("Index", "Notfound");
            }
        }

        // POST: User/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,LastName,FirstName,FullName,UserName,NormalizedUserName,Email,NormalizedEmail,EmailConfirmed,PasswordDecrypt,PasswordHash,SecurityStamp,ConcurrencyStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEnd,LockoutEnabled,AccessFailedCount,ID_NHOMQUYEN,Password,IPLOCATION")] v_v_AspNetUsers AspNetUser)
        {
            try
            {
                if (Utility.KiemTra())
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (!Utility.KiemTraQuyen(API.AspNetUser, API.Create))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                if (ModelState.IsValid)
                {

                    AspNetUser.ConfirmPassword = AspNetUser.Password = clsMaHoa.Encrypt(AspNetUser.Password, clsMaHoa.PassMaHoa);
                    var apiResponse = Utility.Create<v_AspNetUsers>(AspNetUser, API.Account + "/SignUp");
                    if (apiResponse.Success)
                        return RedirectToAction("Index");
                    else
                    {
                        ModelState.AddModelError(string.Empty, apiResponse.Message);
                        if (apiResponse.CheckValue)
                            ViewBag.ID = Guid.NewGuid().ToString();
                    }
                }
                AspNetUser.lstweb_NhomQuyen = new List<web_NhomQuyen>();
                AspNetUser.lstweb_NhomQuyen = Utility.GetListData<web_NhomQuyen>(API.web_NhomQuyen, "", "", Utility.LOC_ID).Data as List<web_NhomQuyen>;
                return View(AspNetUser);
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
                TempData["TitleError"] = API.TitleTryCatch;
                TempData["DetailError"] = ex.Message;
                return RedirectToAction("Index", "Notfound");
            }
        }

        // GET: User/Edit/5
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
                if (!Utility.KiemTraQuyen(API.AspNetUser, API.Edit))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                v_v_AspNetUsers AspNetUser = new v_v_AspNetUsers();
                if (!string.IsNullOrEmpty(id))
                {
                    apiResponse = Utility.GetDetail<v_v_AspNetUsers>(id, API.AspNetUser);
                    if (!apiResponse.Success)
                    {
                        TempData["TitleError"] = apiResponse.Message;
                        return RedirectToAction("Index", "Notfound");
                    }
                    if (apiResponse.Data != null)
                        AspNetUser = apiResponse.Data as v_v_AspNetUsers;
                }
                //@ConvertObjectTCVN3ToUnicode
                AspNetUser.lstweb_NhomQuyen = new List<web_NhomQuyen>();
                AspNetUser.lstweb_NhomQuyen = Utility.GetListData<web_NhomQuyen>(API.web_NhomQuyen, "", "", Utility.LOC_ID).Data as List<web_NhomQuyen>;
                return View(AspNetUser);
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
                TempData["TitleError"] = API.TitleTryCatch;
                TempData["DetailError"] = ex.Message;
                return RedirectToAction("Index", "Notfound");
            }
        }

        // POST: User/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,LastName,FirstName,FullName,UserName,NormalizedUserName,Email,NormalizedEmail,EmailConfirmed,PasswordDecrypt,PasswordHash,SecurityStamp,ConcurrencyStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEnd,LockoutEnabled,AccessFailedCount,ID_NHOMQUYEN,Password,IPLOCATION")] v_v_AspNetUsers AspNetUser)
        {
            try
            {
                if (Utility.KiemTra())
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (!Utility.KiemTraQuyen(API.AspNetUser, API.Edit))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                if (ModelState.IsValid)
                {
                    //@ConvertObjectUnicodeToTCVN3
                    AspNetUser.ConfirmPassword = AspNetUser.Password = clsMaHoa.Encrypt(AspNetUser.Password, clsMaHoa.PassMaHoa);
                    var apiResponse = Utility.Edit<v_AspNetUsers>(AspNetUser.ID, AspNetUser, API.Account+ "/ChangeUser");
                    if (apiResponse.Success)
                        return RedirectToAction("Index");
                    else
                        ModelState.AddModelError(string.Empty, apiResponse.Message);
                }
                AspNetUser.lstweb_NhomQuyen = new List<web_NhomQuyen>();
                AspNetUser.lstweb_NhomQuyen = Utility.GetListData<web_NhomQuyen>(API.web_NhomQuyen, "", "", Utility.LOC_ID).Data as List<web_NhomQuyen>;
                return View(AspNetUser);
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
                TempData["TitleError"] = API.TitleTryCatch;
                TempData["DetailError"] = ex.Message;
                return RedirectToAction("Index", "Notfound");
            }
        }

        // POST: User/Delete/5
        [HttpPost]
        public ActionResult Delete(string id)
        {
            try
            {
                if (Utility.KiemTra())
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (!Utility.KiemTraQuyen(API.AspNetUser, API.Delete))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                if (!string.IsNullOrEmpty(id))
                {
                    var apiResponse = Utility.Delete<v_AspNetUsers>(id, API.AspNetUser);
                    if (apiResponse.Success)
                        return RedirectToAction("Index");
                    else
                        ModelState.AddModelError(string.Empty, apiResponse.Message);
                }
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
                if (!Utility.KiemTraQuyen(API.AspNetUser, API.Create))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                v_v_AspNetUsers AspNetUser = new v_v_AspNetUsers();
                AspNetUser.lstweb_NhomQuyen = new List<web_NhomQuyen>();
                AspNetUser.lstweb_NhomQuyen = Utility.GetListData<web_NhomQuyen>(API.web_NhomQuyen, "", "", Utility.LOC_ID).Data as List<web_NhomQuyen>;
                apiResponse.Success = true;
                AspNetUser.ID = Guid.NewGuid().ToString();
                apiResponse.Detail = Utility.ConvertobjectTo<v_AspNetUsers>(AspNetUser);
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
        public ActionResult CreatePopup([Bind(Include = "IPLOCATION,URL_IMAGE,ID,LastName,FirstName,FullName,UserName,NormalizedUserName,Email,NormalizedEmail,EmailConfirmed,PasswordDecrypt,PasswordHash,SecurityStamp,ConcurrencyStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEnd,LockoutEnabled,AccessFailedCount,ID_NHOMQUYEN,Password,IPLOCATION")] v_AspNetUsers AspNetUser)
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                if (Utility.KiemTra())
                {
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Admin");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                if (!Utility.KiemTraQuyen(API.AspNetUser, API.Create))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                if (ModelState.IsValid)
                {
                    if(!string.IsNullOrEmpty(AspNetUser.Password))
                    {
                        if (Request.Files["MaHinh"] != null)
                        {
                            String fulName = Request.Files["MaHinh"].FileName;
                            if (fulName != "")
                            {
                                String Name = AspNetUser.ID.ToString() + "." + fulName.Split('.')[1];
                                String path = API.PathUser;
                                String fullpath = Path.Combine(Server.MapPath("~" + path), Name);
                                if (!System.IO.Directory.Exists(Server.MapPath("~" + path)))
                                {
                                    System.IO.Directory.CreateDirectory(Server.MapPath("~" + path));
                                }
                                Request.Files["MaHinh"].SaveAs(fullpath);
                                AspNetUser.URL_IMAGE = path + Name;//cập nhật tên file ảnh
                                Byte[] AsBytes = System.IO.File.ReadAllBytes(fullpath);
                                String AsBase64String = Convert.ToBase64String(AsBytes);
                                //dm_HangHoa.FILEBASE64 = AsBase64String;
                            }
                        }

                        AspNetUser.ConfirmPassword = AspNetUser.Password = clsMaHoa.Encrypt(AspNetUser.Password, clsMaHoa.PassMaHoa);
                        apiResponse = Utility.Create<v_AspNetUsers>(AspNetUser, API.Account + "/SignUp");
                        if (apiResponse.Success)
                        {
                            apiResponse.NewID = Guid.NewGuid().ToString();
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, apiResponse.Message);
                            if (apiResponse.CheckValue)
                                apiResponse.NewID = Guid.NewGuid().ToString();

                            apiResponse.Data = Utility.GetModelState(ModelState, API.AspNetUser);
                        }
                    }
                    else
                    {
                        apiResponse.Message = "Vui lòng nhập mật khẩu!";
                    }
                }
                else
                {
                    apiResponse.Success = false;
                    apiResponse.Data = Utility.GetModelState(ModelState, API.AspNetUser);
                }
                apiResponse.Detail = Utility.ConvertobjectTo<v_AspNetUsers>(AspNetUser);
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
                if (!Utility.KiemTraQuyen(API.AspNetUser, API.Edit))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                apiResponse = Utility.GetListData<web_NhomQuyen>(API.web_NhomQuyen, "", "", Utility.LOC_ID);
                if (!apiResponse.Success)
                {
                    TempData["TitleError"] = apiResponse.Message;
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                List<web_NhomQuyen> lstpage = (apiResponse.Data as List<web_NhomQuyen>);
                v_v_AspNetUsers AspNetUser = new v_v_AspNetUsers();
                if (!string.IsNullOrEmpty(id))
                {
                    apiResponse = Utility.GetDetail<v_v_AspNetUsers>(id, API.AspNetUser);
                    if (!apiResponse.Success)
                    {
                        TempData["TitleError"] = apiResponse.Message;
                        apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                        return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                    }
                    if (apiResponse.Data != null)
                        AspNetUser = apiResponse.Data as v_v_AspNetUsers;
                }
                AspNetUser.lstweb_NhomQuyen = new List<web_NhomQuyen>();
                AspNetUser.lstweb_NhomQuyen = Utility.GetListData<web_NhomQuyen>(API.web_NhomQuyen, "", "", Utility.LOC_ID).Data as List<web_NhomQuyen>;
                apiResponse.Success = true;
                apiResponse.PathProduct = "";
                apiResponse.Detail = Utility.ConvertobjectTo<v_v_AspNetUsers>(AspNetUser);
                
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
        public ActionResult EditPopup([Bind(Include = "IPLOCATION,URL_IMAGE,ID,LastName,FirstName,FullName,UserName,NormalizedUserName,Email,NormalizedEmail,EmailConfirmed,PasswordDecrypt,PasswordHash,SecurityStamp,ConcurrencyStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEnd,LockoutEnabled,AccessFailedCount,ID_NHOMQUYEN,Password,IPLOCATION")] v_AspNetUsers AspNetUser)
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                if (Utility.KiemTra())
                {
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Admin");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                if (!Utility.KiemTraQuyen(API.AspNetUser, API.Edit))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                if (ModelState.IsValid)
                {
                    if(Request.Files["MaHinh"] != null)
                    {
                        String fulName = Request.Files["MaHinh"].FileName;
                        if (fulName != "")
                        {
                            String Name = AspNetUser.ID.Trim() + "." + fulName.Split('.')[1];
                            String fullpath = Path.Combine(Server.MapPath("~" + API.PathUser), Name);
                            if (!System.IO.Directory.Exists(Server.MapPath("~" + API.PathUser)))
                            {
                                System.IO.Directory.CreateDirectory(Server.MapPath("~" + API.PathUser));
                            }
                            if (System.IO.File.Exists(fullpath))
                            {
                                System.IO.File.Delete(fullpath);
                            }
                            Request.Files["MaHinh"].SaveAs(fullpath);
                            AspNetUser.URL_IMAGE = API.PathUser + Name;//cập nhật tên file ảnh
                            Byte[] AsBytes = System.IO.File.ReadAllBytes(fullpath);
                            String AsBase64String = Convert.ToBase64String(AsBytes);
                            //dm_HangHoa.FILEBASE64 = AsBase64String;
                            //dm_HangHoa.FILENEW = true;
                        }
                    }


                    AspNetUser.ConfirmPassword = AspNetUser.Password = clsMaHoa.Encrypt(AspNetUser.Password, clsMaHoa.PassMaHoa);
                    apiResponse = Utility.Edit<v_AspNetUsers>(AspNetUser.ID, AspNetUser, API.Account + "/ChangeUser");
                    if (apiResponse.Success)
                    {
                        //return RedirectToAction("Index");
                        apiResponse.ID = AspNetUser.ID;
                        if (apiResponse.Data != null && !string.IsNullOrEmpty(apiResponse.Data.ToString()))
                            AspNetUser = JsonConvert.DeserializeObject<v_AspNetUsers>(apiResponse.Data.ToString());
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, apiResponse.Message);
                    }
                }
                else
                {
                    apiResponse.Success = false;
                    apiResponse.Data = Utility.GetModelState(ModelState, API.AspNetUser);
                }
                apiResponse.Detail = Utility.ConvertobjectTo<v_AspNetUsers>(AspNetUser);
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
                if (!Utility.KiemTraQuyen(API.AspNetUser, API.Delete))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                apiResponse = Utility.Delete<v_AspNetUsers>(id, API.AspNetUser);
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


        public ActionResult ChangePassword()
        {
            try
            {

                if (Utility.KiemTra())
                {
                    return RedirectToAction("Index", "Admin");
                }
                ChangePassword AspNetUser = new ChangePassword();
                return View(AspNetUser);
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
                TempData["TitleError"] = API.TitleTryCatch;
                TempData["DetailError"] = ex.Message;
                return RedirectToAction("Index", "Notfound");
            }
        }

        // POST: User/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword([Bind(Include = "Password,NewPassword,ConfirmPassword")] ChangePassword AspNetUser)
        {
            try
            {
                if (Utility.KiemTra())
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (!Utility.KiemTraQuyen(API.AspNetUser, API.Edit))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }

                if (!string.IsNullOrEmpty(AspNetUser.NewPassword) && 
                    !string.IsNullOrEmpty(AspNetUser.ConfirmPassword) && 
                    AspNetUser.ConfirmPassword != AspNetUser.NewPassword)
                {
                    ModelState.AddModelError("ConfirmPassword", "Mật khẩu xác nhận không trùng khớp!");
                }

                if (ModelState.IsValid)
                {
                    //@ConvertObjectUnicodeToTCVN3
                    AspNetUser.Password = clsMaHoa.Encrypt(AspNetUser.Password, clsMaHoa.PassMaHoa);
                    AspNetUser.ConfirmPassword = clsMaHoa.Encrypt(AspNetUser.ConfirmPassword, clsMaHoa.PassMaHoa);
                    var apiResponse = Utility.Edit<ChangePassword>(Session[Sessions.idUser].ToString(), AspNetUser, API.Account + "/ChangeUserPassword");
                    if (apiResponse.Success)
                        return RedirectToAction("Index", "Admin");
                    else
                    {
                        AspNetUser.Password = clsMaHoa.Decrypt(AspNetUser.Password, clsMaHoa.PassMaHoa);
                        AspNetUser.ConfirmPassword = clsMaHoa.Decrypt(AspNetUser.ConfirmPassword, clsMaHoa.PassMaHoa);
                        ModelState.AddModelError(string.Empty, apiResponse.Message);
                    }    
                        
                }

                return View(AspNetUser);
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
                TempData["TitleError"] = API.TitleTryCatch;
                TempData["DetailError"] = ex.Message;
                return RedirectToAction("Index", "Notfound");
            }
        }
    }
  
    public class ChangePassword
    {
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu cũ.")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu mới.")]
        [RegularExpression(@"^(?=.*[!@#$%^&*(),.?"":{}|<>])(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[a-zA-Z\d!@#$%^&*(),.?"":{}|<>]{10,}$",ErrorMessage = "Mật khẩu phải có ít nhất 10 ký tự, trong đó có ít nhất 1 chữ hoa, ít nhất 1 chữ thường, ít nhất một số và một ký tự đặc biệt.")]
        public string NewPassword { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu xác nhận.")]
        public string ConfirmPassword { get; set; }
    }
}

