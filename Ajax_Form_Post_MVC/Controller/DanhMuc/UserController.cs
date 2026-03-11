using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Web.Mvc;
using DatabaseTHP;
using DatabaseTHP.Class;
using MVC_QuanLyTHP.Class;
using MVC_QuanLyTHP.Models;
using Newtonsoft.Json;
using PagedList;

namespace MVC_QuanLyTHP.Controllers
{

	public class UserController : Controller
	{
		public ActionResult Index(int Page = 1, string SearchString = "", string ShowSearchValue = "")
		{
			try
			{
				if (Utility.KiemTra())
				{
					return RedirectToAction("Index", "Admin");
				}
				if (!Utility.KiemTraQuyen("User", "View"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				ShowSearchValue = Utility.GetShowSearchValue<AspNetUsers>(ShowSearchValue);
				ApiResponse listData = Utility.GetListData<v_AspNetUsers>("User", ShowSearchValue, SearchString);
				if (!listData.Success)
				{
					base.TempData["TitleError"] = listData.Message;
					return RedirectToAction("Index", "Notfound");
				}
				IPagedList<v_AspNetUsers> iPagedList = (listData.Data as List<v_AspNetUsers>).ToPagedList(Page, Utility.GetPageSize());
				v_v_AspNetUsers v_v_AspNetUsers2 = new v_v_AspNetUsers();
				v_v_AspNetUsers2.IPagedList = iPagedList;
				v_v_AspNetUsers2.lstweb_NhomQuyen = new List<web_NhomQuyen>();
				v_v_AspNetUsers2.lstweb_NhomQuyen = Utility.GetListData<web_NhomQuyen>("GroupPermissions", "", "", Utility.LOC_ID).Data as List<web_NhomQuyen>;
				base.ViewBag.searchValue = SearchString;
				base.ViewBag.showsearchValue = ShowSearchValue;
				base.ViewBag.PermissionEdit = Utility.KiemTraQuyen("User", "Edit");
				base.ViewBag.PermissionDelete = Utility.KiemTraQuyen("User", "Delete");
				base.ViewBag.PermissionCreate = Utility.KiemTraQuyen("User", "Create");
				return View(v_v_AspNetUsers2);
			}
			catch (Exception ex)
			{
				Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
				base.TempData["TitleError"] = "Đã có lỗi phát sinh trong phiên làm việc!";
				base.TempData["DetailError"] = ex.Message;
				return RedirectToAction("Index", "Notfound");
			}
		}

		public ActionResult Create(int type = 2)
		{
			try
			{
				base.Session["IntWidth"] = type;
				if (Utility.KiemTra())
				{
					return RedirectToAction("Index", "Admin");
				}
				if (!Utility.KiemTraQuyen("User", "Create"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				v_v_AspNetUsers v_v_AspNetUsers2 = new v_v_AspNetUsers();
				v_v_AspNetUsers2.ID = Guid.NewGuid().ToString();
				v_v_AspNetUsers2.lstweb_NhomQuyen = new List<web_NhomQuyen>();
				v_v_AspNetUsers2.lstweb_NhomQuyen = Utility.GetListData<web_NhomQuyen>("GroupPermissions", "", "", Utility.LOC_ID).Data as List<web_NhomQuyen>;
				return View(v_v_AspNetUsers2);
			}
			catch (Exception ex)
			{
				Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
				base.TempData["TitleError"] = "Đã có lỗi phát sinh trong phiên làm việc!";
				base.TempData["DetailError"] = ex.Message;
				return RedirectToAction("Index", "Notfound");
			}
		}

		[HttpPost]
		[ValidateInput(false)]
		[ValidateAntiForgeryToken]
		public ActionResult Create([Bind(Include = "ID,LastName,FirstName,FullName,UserName,NormalizedUserName,Email,NormalizedEmail,EmailConfirmed,PasswordDecrypt,PasswordHash,SecurityStamp,ConcurrencyStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEnd,LockoutEnabled,AccessFailedCount,ID_NHOMQUYEN,Password,IPLOCATION")] v_v_AspNetUsers AspNetUser)
		{
			try
			{
				if (Utility.KiemTra())
				{
					return RedirectToAction("Index", "Admin");
				}
				if (!Utility.KiemTraQuyen("User", "Create"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				if (base.ModelState.IsValid)
				{
					string confirmPassword = (AspNetUser.Password = clsMaHoa.Encrypt(AspNetUser.Password, "tmt6364"));
					AspNetUser.ConfirmPassword = confirmPassword;
					ApiResponse apiResponse = Utility.Create((v_AspNetUsers)AspNetUser, "Accounts/SignUp");
					if (apiResponse.Success)
					{
						return RedirectToAction("Index");
					}
					base.ModelState.AddModelError(string.Empty, apiResponse.Message);
					if (apiResponse.CheckValue)
					{
						base.ViewBag.ID = Guid.NewGuid().ToString();
					}
				}
				AspNetUser.lstweb_NhomQuyen = new List<web_NhomQuyen>();
				AspNetUser.lstweb_NhomQuyen = Utility.GetListData<web_NhomQuyen>("GroupPermissions", "", "", Utility.LOC_ID).Data as List<web_NhomQuyen>;
				return View(AspNetUser);
			}
			catch (Exception ex)
			{
				Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
				base.TempData["TitleError"] = "Đã có lỗi phát sinh trong phiên làm việc!";
				base.TempData["DetailError"] = ex.Message;
				return RedirectToAction("Index", "Notfound");
			}
		}

		public ActionResult Edit(string id, int type = 2)
		{
			ApiResponse apiResponse = new ApiResponse();
			try
			{
				base.Session["IntWidth"] = type;
				if (Utility.KiemTra())
				{
					return RedirectToAction("Index", "Admin");
				}
				if (!Utility.KiemTraQuyen("User", "Edit"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				v_v_AspNetUsers v_v_AspNetUsers2 = new v_v_AspNetUsers();
				if (!string.IsNullOrEmpty(id))
				{
					apiResponse = Utility.GetDetail<v_v_AspNetUsers>(id, "User");
					if (!apiResponse.Success)
					{
						base.TempData["TitleError"] = apiResponse.Message;
						return RedirectToAction("Index", "Notfound");
					}
					if (apiResponse.Data != null)
					{
						v_v_AspNetUsers2 = apiResponse.Data as v_v_AspNetUsers;
					}
				}
				v_v_AspNetUsers2.lstweb_NhomQuyen = new List<web_NhomQuyen>();
				v_v_AspNetUsers2.lstweb_NhomQuyen = Utility.GetListData<web_NhomQuyen>("GroupPermissions", "", "", Utility.LOC_ID).Data as List<web_NhomQuyen>;
				return View(v_v_AspNetUsers2);
			}
			catch (Exception ex)
			{
				Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
				base.TempData["TitleError"] = "Đã có lỗi phát sinh trong phiên làm việc!";
				base.TempData["DetailError"] = ex.Message;
				return RedirectToAction("Index", "Notfound");
			}
		}

		[HttpPost]
		[ValidateInput(false)]
		[ValidateAntiForgeryToken]
		public ActionResult Edit([Bind(Include = "ID,LastName,FirstName,FullName,UserName,NormalizedUserName,Email,NormalizedEmail,EmailConfirmed,PasswordDecrypt,PasswordHash,SecurityStamp,ConcurrencyStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEnd,LockoutEnabled,AccessFailedCount,ID_NHOMQUYEN,Password,IPLOCATION")] v_v_AspNetUsers AspNetUser)
		{
			try
			{
				if (Utility.KiemTra())
				{
					return RedirectToAction("Index", "Admin");
				}
				if (!Utility.KiemTraQuyen("User", "Edit"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				if (base.ModelState.IsValid)
				{
					string confirmPassword = (AspNetUser.Password = clsMaHoa.Encrypt(AspNetUser.Password, "tmt6364"));
					AspNetUser.ConfirmPassword = confirmPassword;
					ApiResponse apiResponse = Utility.Edit(AspNetUser.ID, (v_AspNetUsers)AspNetUser, "Accounts/ChangeUser");
					if (apiResponse.Success)
					{
						return RedirectToAction("Index");
					}
					base.ModelState.AddModelError(string.Empty, apiResponse.Message);
				}
				AspNetUser.lstweb_NhomQuyen = new List<web_NhomQuyen>();
				AspNetUser.lstweb_NhomQuyen = Utility.GetListData<web_NhomQuyen>("GroupPermissions", "", "", Utility.LOC_ID).Data as List<web_NhomQuyen>;
				return View(AspNetUser);
			}
			catch (Exception ex)
			{
				Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
				base.TempData["TitleError"] = "Đã có lỗi phát sinh trong phiên làm việc!";
				base.TempData["DetailError"] = ex.Message;
				return RedirectToAction("Index", "Notfound");
			}
		}

		[HttpPost]
		public ActionResult Delete(string id)
		{
			try
			{
				if (Utility.KiemTra())
				{
					return RedirectToAction("Index", "Admin");
				}
				if (!Utility.KiemTraQuyen("User", "Delete"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				if (!string.IsNullOrEmpty(id))
				{
					ApiResponse apiResponse = Utility.Delete<v_AspNetUsers>(id, "User");
					if (apiResponse.Success)
					{
						return RedirectToAction("Index");
					}
					base.ModelState.AddModelError(string.Empty, apiResponse.Message);
				}
				return View();
			}
			catch (Exception ex)
			{
				Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
				base.TempData["TitleError"] = "Đã có lỗi phát sinh trong phiên làm việc!";
				base.TempData["DetailError"] = ex.Message;
				return RedirectToAction("Index", "Notfound");
			}
		}

		public ActionResult CreatePopup()
		{
			ApiResponse apiResponse = new ApiResponse();
			try
			{
				if (Utility.KiemTra())
				{
					apiResponse.Success = false;
					apiResponse.URL = base.Url.Action("Index", "Admin");
					return new JsonResult
					{
						Data = apiResponse,
						JsonRequestBehavior = JsonRequestBehavior.AllowGet,
						MaxJsonLength = int.MaxValue
					};
				}
				if (!Utility.KiemTraQuyen("User", "Create"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					apiResponse.Success = false;
					apiResponse.URL = base.Url.Action("Index", "Notfound");
					return new JsonResult
					{
						Data = apiResponse,
						JsonRequestBehavior = JsonRequestBehavior.AllowGet,
						MaxJsonLength = int.MaxValue
					};
				}
				v_v_AspNetUsers v_v_AspNetUsers2 = new v_v_AspNetUsers();
				v_v_AspNetUsers2.lstweb_NhomQuyen = new List<web_NhomQuyen>();
				v_v_AspNetUsers2.lstweb_NhomQuyen = Utility.GetListData<web_NhomQuyen>("GroupPermissions", "", "", Utility.LOC_ID).Data as List<web_NhomQuyen>;
				apiResponse.Success = true;
				v_v_AspNetUsers2.ID = Guid.NewGuid().ToString();
				apiResponse.Detail = Utility.ConvertobjectTo((v_AspNetUsers)v_v_AspNetUsers2, "yyyy-MM-dd HH:mm:ss");
				return new JsonResult
				{
					Data = apiResponse,
					JsonRequestBehavior = JsonRequestBehavior.AllowGet,
					MaxJsonLength = int.MaxValue
				};
			}
			catch (Exception ex)
			{
				Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
				base.TempData["TitleError"] = "Đã có lỗi phát sinh trong phiên làm việc!";
				base.TempData["DetailError"] = ex.Message;
				apiResponse.Success = false;
				apiResponse.URL = base.Url.Action("Index", "Notfound");
				return new JsonResult
				{
					Data = apiResponse,
					JsonRequestBehavior = JsonRequestBehavior.AllowGet,
					MaxJsonLength = int.MaxValue
				};
			}
		}

		[HttpPost]
		[ValidateInput(false)]
		[ValidateAntiForgeryToken]
		public ActionResult CreatePopup([Bind(Include = "IPLOCATION,URL_IMAGE,ID,LastName,FirstName,FullName,UserName,NormalizedUserName,Email,NormalizedEmail,EmailConfirmed,PasswordDecrypt,PasswordHash,SecurityStamp,ConcurrencyStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEnd,LockoutEnabled,AccessFailedCount,ID_NHOMQUYEN,Password,IPLOCATION")] v_AspNetUsers AspNetUser)
		{
			ApiResponse apiResponse = new ApiResponse();
			try
			{
				if (Utility.KiemTra())
				{
					apiResponse.Success = false;
					apiResponse.URL = base.Url.Action("Index", "Admin");
					return new JsonResult
					{
						Data = apiResponse,
						JsonRequestBehavior = JsonRequestBehavior.AllowGet,
						MaxJsonLength = int.MaxValue
					};
				}
				if (!Utility.KiemTraQuyen("User", "Create"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					apiResponse.Success = false;
					apiResponse.URL = base.Url.Action("Index", "Notfound");
					return new JsonResult
					{
						Data = apiResponse,
						JsonRequestBehavior = JsonRequestBehavior.AllowGet,
						MaxJsonLength = int.MaxValue
					};
				}
				if (base.ModelState.IsValid)
				{
					if (!string.IsNullOrEmpty(AspNetUser.Password))
					{
						if (base.Request.Files["MaHinh"] != null)
						{
							string fileName = base.Request.Files["MaHinh"].FileName;
							if (fileName != "")
							{
								string text = AspNetUser.ID.ToString() + "." + fileName.Split('.')[1];
								string text2 = "/Images_Upload/User/";
								string text3 = Path.Combine(base.Server.MapPath("~" + text2), text);
								if (!Directory.Exists(base.Server.MapPath("~" + text2)))
								{
									Directory.CreateDirectory(base.Server.MapPath("~" + text2));
								}
								base.Request.Files["MaHinh"].SaveAs(text3);
								AspNetUser.URL_IMAGE = text2 + text;
								byte[] inArray = System.IO.File.ReadAllBytes(text3);
								string text4 = Convert.ToBase64String(inArray);
							}
						}
						string confirmPassword = (AspNetUser.Password = clsMaHoa.Encrypt(AspNetUser.Password, "tmt6364"));
						AspNetUser.ConfirmPassword = confirmPassword;
						apiResponse = Utility.Create(AspNetUser, "Accounts/SignUp");
						if (apiResponse.Success)
						{
							apiResponse.NewID = Guid.NewGuid().ToString();
						}
						else
						{
							base.ModelState.AddModelError(string.Empty, apiResponse.Message);
							if (apiResponse.CheckValue)
							{
								apiResponse.NewID = Guid.NewGuid().ToString();
							}
							apiResponse.Data = Utility.GetModelState(base.ModelState, "User");
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
					apiResponse.Data = Utility.GetModelState(base.ModelState, "User");
				}
				apiResponse.Detail = Utility.ConvertobjectTo(AspNetUser);
				return new JsonResult
				{
					Data = apiResponse,
					JsonRequestBehavior = JsonRequestBehavior.AllowGet,
					MaxJsonLength = int.MaxValue
				};
			}
			catch (Exception ex)
			{
				Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
				base.TempData["TitleError"] = "Đã có lỗi phát sinh trong phiên làm việc!";
				base.TempData["DetailError"] = ex.Message;
				apiResponse.Success = false;
				apiResponse.URL = base.Url.Action("Index", "Notfound");
				return new JsonResult
				{
					Data = apiResponse,
					JsonRequestBehavior = JsonRequestBehavior.AllowGet,
					MaxJsonLength = int.MaxValue
				};
			}
		}

		public ActionResult EditPopup(string id)
		{
			ApiResponse apiResponse = new ApiResponse();
			try
			{
				if (Utility.KiemTra())
				{
					apiResponse.Success = false;
					apiResponse.URL = base.Url.Action("Index", "Admin");
					return new JsonResult
					{
						Data = apiResponse,
						JsonRequestBehavior = JsonRequestBehavior.AllowGet,
						MaxJsonLength = int.MaxValue
					};
				}
				if (!Utility.KiemTraQuyen("User", "Edit"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					apiResponse.Success = false;
					apiResponse.URL = base.Url.Action("Index", "Notfound");
					return new JsonResult
					{
						Data = apiResponse,
						JsonRequestBehavior = JsonRequestBehavior.AllowGet,
						MaxJsonLength = int.MaxValue
					};
				}
				apiResponse = Utility.GetListData<web_NhomQuyen>("GroupPermissions", "", "", Utility.LOC_ID);
				if (!apiResponse.Success)
				{
					base.TempData["TitleError"] = apiResponse.Message;
					apiResponse.Success = false;
					apiResponse.URL = base.Url.Action("Index", "Notfound");
					return new JsonResult
					{
						Data = apiResponse,
						JsonRequestBehavior = JsonRequestBehavior.AllowGet,
						MaxJsonLength = int.MaxValue
					};
				}
				List<web_NhomQuyen> list = apiResponse.Data as List<web_NhomQuyen>;
				v_v_AspNetUsers v_v_AspNetUsers2 = new v_v_AspNetUsers();
				if (!string.IsNullOrEmpty(id))
				{
					apiResponse = Utility.GetDetail<v_v_AspNetUsers>(id, "User");
					if (!apiResponse.Success)
					{
						base.TempData["TitleError"] = apiResponse.Message;
						apiResponse.Success = false;
						apiResponse.URL = base.Url.Action("Index", "Notfound");
						return new JsonResult
						{
							Data = apiResponse,
							JsonRequestBehavior = JsonRequestBehavior.AllowGet,
							MaxJsonLength = int.MaxValue
						};
					}
					if (apiResponse.Data != null)
					{
						v_v_AspNetUsers2 = apiResponse.Data as v_v_AspNetUsers;
					}
				}
				v_v_AspNetUsers2.lstweb_NhomQuyen = new List<web_NhomQuyen>();
				v_v_AspNetUsers2.lstweb_NhomQuyen = Utility.GetListData<web_NhomQuyen>("GroupPermissions", "", "", Utility.LOC_ID).Data as List<web_NhomQuyen>;
				apiResponse.Success = true;
				apiResponse.PathProduct = "";
				apiResponse.Detail = Utility.ConvertobjectTo(v_v_AspNetUsers2);
				return new JsonResult
				{
					Data = apiResponse,
					JsonRequestBehavior = JsonRequestBehavior.AllowGet,
					MaxJsonLength = int.MaxValue
				};
			}
			catch (Exception ex)
			{
				Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
				base.TempData["TitleError"] = "Đã có lỗi phát sinh trong phiên làm việc!";
				base.TempData["DetailError"] = ex.Message;
				apiResponse.Success = false;
				apiResponse.URL = base.Url.Action("Index", "Notfound");
				return new JsonResult
				{
					Data = apiResponse,
					JsonRequestBehavior = JsonRequestBehavior.AllowGet,
					MaxJsonLength = int.MaxValue
				};
			}
		}

		[HttpPost]
		[ValidateInput(false)]
		[ValidateAntiForgeryToken]
		public ActionResult EditPopup([Bind(Include = "IPLOCATION,URL_IMAGE,ID,LastName,FirstName,FullName,UserName,NormalizedUserName,Email,NormalizedEmail,EmailConfirmed,PasswordDecrypt,PasswordHash,SecurityStamp,ConcurrencyStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEnd,LockoutEnabled,AccessFailedCount,ID_NHOMQUYEN,Password,IPLOCATION")] v_AspNetUsers AspNetUser)
		{
			ApiResponse apiResponse = new ApiResponse();
			try
			{
				if (Utility.KiemTra())
				{
					apiResponse.Success = false;
					apiResponse.URL = base.Url.Action("Index", "Admin");
					return new JsonResult
					{
						Data = apiResponse,
						JsonRequestBehavior = JsonRequestBehavior.AllowGet,
						MaxJsonLength = int.MaxValue
					};
				}
				if (!Utility.KiemTraQuyen("User", "Edit"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					apiResponse.Success = false;
					apiResponse.URL = base.Url.Action("Index", "Notfound");
					return new JsonResult
					{
						Data = apiResponse,
						JsonRequestBehavior = JsonRequestBehavior.AllowGet,
						MaxJsonLength = int.MaxValue
					};
				}
				if (base.ModelState.IsValid)
				{
					if (base.Request.Files["MaHinh"] != null)
					{
						string fileName = base.Request.Files["MaHinh"].FileName;
						if (fileName != "")
						{
							string text = AspNetUser.ID.Trim() + "." + fileName.Split('.')[1];
							string text2 = Path.Combine(base.Server.MapPath("~/Images_Upload/User/"), text);
							if (!Directory.Exists(base.Server.MapPath("~/Images_Upload/User/")))
							{
								Directory.CreateDirectory(base.Server.MapPath("~/Images_Upload/User/"));
							}
							if (System.IO.File.Exists(text2))
							{
								System.IO.File.Delete(text2);
							}
							base.Request.Files["MaHinh"].SaveAs(text2);
							AspNetUser.URL_IMAGE = "/Images_Upload/User/" + text;
							byte[] inArray = System.IO.File.ReadAllBytes(text2);
							string text3 = Convert.ToBase64String(inArray);
						}
					}
					v_AspNetUsers obj = AspNetUser;
					string confirmPassword = (AspNetUser.Password = clsMaHoa.Encrypt(AspNetUser.Password, "tmt6364"));
					obj.ConfirmPassword = confirmPassword;
					apiResponse = Utility.Edit(AspNetUser.ID, AspNetUser, "Accounts/ChangeUser");
					if (apiResponse.Success)
					{
						apiResponse.ID = AspNetUser.ID;
						if (apiResponse.Data != null && !string.IsNullOrEmpty(apiResponse.Data.ToString()))
						{
							AspNetUser = JsonConvert.DeserializeObject<v_AspNetUsers>(apiResponse.Data.ToString());
						}
					}
					else
					{
						base.ModelState.AddModelError(string.Empty, apiResponse.Message);
					}
				}
				else
				{
					apiResponse.Success = false;
					apiResponse.Data = Utility.GetModelState(base.ModelState, "User");
				}
				apiResponse.Detail = Utility.ConvertobjectTo(AspNetUser);
				return new JsonResult
				{
					Data = apiResponse,
					JsonRequestBehavior = JsonRequestBehavior.AllowGet,
					MaxJsonLength = int.MaxValue
				};
			}
			catch (Exception ex)
			{
				Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
				base.TempData["TitleError"] = "Đã có lỗi phát sinh trong phiên làm việc!";
				base.TempData["DetailError"] = ex.Message;
				apiResponse.Success = false;
				apiResponse.URL = base.Url.Action("Index", "Notfound");
				return new JsonResult
				{
					Data = apiResponse,
					JsonRequestBehavior = JsonRequestBehavior.AllowGet,
					MaxJsonLength = int.MaxValue
				};
			}
		}

		[HttpPost]
		public ActionResult DeletePopup(string id)
		{
			ApiResponse apiResponse = new ApiResponse();
			try
			{
				if (Utility.KiemTra())
				{
					apiResponse.Success = false;
					apiResponse.URL = base.Url.Action("Index", "Admin");
					return new JsonResult
					{
						Data = apiResponse,
						JsonRequestBehavior = JsonRequestBehavior.AllowGet,
						MaxJsonLength = int.MaxValue
					};
				}
				if (!Utility.KiemTraQuyen("User", "Delete"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					apiResponse.Success = false;
					apiResponse.URL = base.Url.Action("Index", "Notfound");
					return new JsonResult
					{
						Data = apiResponse,
						JsonRequestBehavior = JsonRequestBehavior.AllowGet,
						MaxJsonLength = int.MaxValue
					};
				}
				apiResponse = Utility.Delete<v_AspNetUsers>(id, "User");
				apiResponse.ID = id;
				return new JsonResult
				{
					Data = apiResponse,
					JsonRequestBehavior = JsonRequestBehavior.AllowGet,
					MaxJsonLength = int.MaxValue
				};
			}
			catch (Exception ex)
			{
				Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
				base.TempData["TitleError"] = "Đã có lỗi phát sinh trong phiên làm việc!";
				base.TempData["DetailError"] = ex.Message;
				apiResponse.Success = false;
				apiResponse.URL = base.Url.Action("Index", "Notfound");
				return new JsonResult
				{
					Data = apiResponse,
					JsonRequestBehavior = JsonRequestBehavior.AllowGet,
					MaxJsonLength = int.MaxValue
				};
			}
		}

		public ActionResult ChangePassword()
		{
			try
			{
				if (Utility.KiemTra())
				{
					return RedirectToAction("Index", "Admin");
				}
				ChangePassword model = new ChangePassword();
				return View(model);
			}
			catch (Exception ex)
			{
				Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
				base.TempData["TitleError"] = "Đã có lỗi phát sinh trong phiên làm việc!";
				base.TempData["DetailError"] = ex.Message;
				return RedirectToAction("Index", "Notfound");
			}
		}

		[HttpPost]
		[ValidateInput(false)]
		[ValidateAntiForgeryToken]
		public ActionResult ChangePassword([Bind(Include = "Password,NewPassword,ConfirmPassword")] ChangePassword AspNetUser)
		{
			try
			{
				if (Utility.KiemTra())
				{
					return RedirectToAction("Index", "Admin");
				}
				if (!Utility.KiemTraQuyen("User", "Edit"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				if (!string.IsNullOrEmpty(AspNetUser.NewPassword) && !string.IsNullOrEmpty(AspNetUser.ConfirmPassword) && AspNetUser.ConfirmPassword != AspNetUser.NewPassword)
				{
					base.ModelState.AddModelError("ConfirmPassword", "Mật khẩu xác nhận không trùng khớp!");
				}
				if (base.ModelState.IsValid)
				{
					AspNetUser.Password = clsMaHoa.Encrypt(AspNetUser.Password, "tmt6364");
					AspNetUser.ConfirmPassword = clsMaHoa.Encrypt(AspNetUser.ConfirmPassword, "tmt6364");
					ApiResponse apiResponse = Utility.Edit(base.Session["idUser"].ToString(), AspNetUser, "Accounts/ChangeUserPassword");
					if (apiResponse.Success)
					{
						return RedirectToAction("Index", "Admin");
					}
					AspNetUser.Password = clsMaHoa.Decrypt(AspNetUser.Password, "tmt6364");
					AspNetUser.ConfirmPassword = clsMaHoa.Decrypt(AspNetUser.ConfirmPassword, "tmt6364");
					base.ModelState.AddModelError(string.Empty, apiResponse.Message);
				}
				return View(AspNetUser);
			}
			catch (Exception ex)
			{
				Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
				base.TempData["TitleError"] = "Đã có lỗi phát sinh trong phiên làm việc!";
				base.TempData["DetailError"] = ex.Message;
				return RedirectToAction("Index", "Notfound");
			}
		}
	}
}
