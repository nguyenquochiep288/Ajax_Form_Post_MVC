using System;
using System.Collections.Generic;
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

	public class DepartmentController : Controller
	{
		public ActionResult Index(int Page = 1, string SearchString = "", string ShowSearchValue = "")
		{
			try
			{
				if (Utility.KiemTra())
				{
					return RedirectToAction("Index", "Admin");
				}
				if (!Utility.KiemTraQuyen("Department", "View"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				ShowSearchValue = Utility.GetShowSearchValue<dm_PhongBan>(ShowSearchValue);
				ApiResponse listData = Utility.GetListData<v_dm_PhongBan>("Department", ShowSearchValue, SearchString, Utility.LOC_ID);
				if (!listData.Success)
				{
					base.TempData["TitleError"] = listData.Message;
					return RedirectToAction("Index", "Notfound");
				}
				IPagedList<v_dm_PhongBan> iPagedList = (listData.Data as List<v_dm_PhongBan>).ToPagedList(Page, Utility.GetPageSize());
				v_v_dm_PhongBan v_v_dm_PhongBan2 = new v_v_dm_PhongBan();
				v_v_dm_PhongBan2.IPagedList = iPagedList;
				v_v_dm_PhongBan2.lstdm_PhongBan = new List<v_dm_PhongBan>();
				v_v_dm_PhongBan2.lstdm_PhongBan = Utility.GetListData<v_dm_PhongBan>("Department", "", "", Utility.LOC_ID).Data as List<v_dm_PhongBan>;
				base.ViewBag.searchValue = SearchString;
				base.ViewBag.showsearchValue = ShowSearchValue;
				base.ViewBag.PermissionEdit = Utility.KiemTraQuyen("Department", "Edit");
				base.ViewBag.PermissionDelete = Utility.KiemTraQuyen("Department", "Delete");
				base.ViewBag.PermissionCreate = Utility.KiemTraQuyen("Department", "Create");
				return View(v_v_dm_PhongBan2);
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
				if (!Utility.KiemTraQuyen("Department", "Create"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				v_v_dm_PhongBan v_v_dm_PhongBan2 = new v_v_dm_PhongBan();
				v_v_dm_PhongBan2.LOC_ID = Utility.LOC_ID;
				v_v_dm_PhongBan2.ID_NGUOITAO = base.Session["idUser"].ToString();
				v_v_dm_PhongBan2.THOIGIANTHEM = Utility.CurrentTime;
				v_v_dm_PhongBan2.ID = Guid.NewGuid().ToString();
				v_v_dm_PhongBan2.lstdm_PhongBan = new List<v_dm_PhongBan>();
				v_v_dm_PhongBan2.lstdm_PhongBan = Utility.GetListData<v_dm_PhongBan>("Department", "", "", Utility.LOC_ID).Data as List<v_dm_PhongBan>;
				return View(v_v_dm_PhongBan2);
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
		public ActionResult Create([Bind(Include = "LOC_ID,ID,MA,NAME,ID_PHONGBANCHA,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO,ISACTIVE")] v_v_dm_PhongBan dm_PhongBan)
		{
			try
			{
				if (Utility.KiemTra())
				{
					return RedirectToAction("Index", "Admin");
				}
				if (!Utility.KiemTraQuyen("Department", "Create"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				if (base.ModelState.IsValid)
				{
					dm_PhongBan.LOC_ID = Utility.LOC_ID;
					dm_PhongBan.ID_NGUOITAO = base.Session["idUser"].ToString();
					dm_PhongBan.THOIGIANTHEM = Utility.CurrentTime;
					ApiResponse apiResponse = Utility.Create((dm_PhongBan)dm_PhongBan, "Department");
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
				return View(dm_PhongBan);
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
				if (!Utility.KiemTraQuyen("Department", "Edit"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				v_v_dm_PhongBan v_v_dm_PhongBan2 = new v_v_dm_PhongBan();
				if (!string.IsNullOrEmpty(id))
				{
					apiResponse = Utility.GetDetail<v_v_dm_PhongBan>(Utility.LOC_ID + "/" + id, "Department");
					if (!apiResponse.Success)
					{
						base.TempData["TitleError"] = apiResponse.Message;
						return RedirectToAction("Index", "Notfound");
					}
					if (apiResponse.Data != null)
					{
						v_v_dm_PhongBan2 = apiResponse.Data as v_v_dm_PhongBan;
					}
				}
				v_v_dm_PhongBan2.lstdm_PhongBan = new List<v_dm_PhongBan>();
				v_v_dm_PhongBan2.lstdm_PhongBan = Utility.GetListData<v_dm_PhongBan>("Department", "", "", Utility.LOC_ID).Data as List<v_dm_PhongBan>;
				return View(v_v_dm_PhongBan2);
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
		public ActionResult Edit([Bind(Include = "LOC_ID,ID,MA,NAME,ID_PHONGBANCHA,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO,ISACTIVE")] v_v_dm_PhongBan dm_PhongBan)
		{
			try
			{
				if (Utility.KiemTra())
				{
					return RedirectToAction("Index", "Admin");
				}
				if (!Utility.KiemTraQuyen("Department", "Edit"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				if (base.ModelState.IsValid)
				{
					dm_PhongBan.LOC_ID = Utility.LOC_ID;
					dm_PhongBan.ID_NGUOISUA = base.Session["idUser"].ToString();
					dm_PhongBan.THOIGIANSUA = Utility.CurrentTime;
					ApiResponse apiResponse = Utility.Edit(Utility.LOC_ID + "/" + dm_PhongBan.MA, (v_dm_PhongBan)dm_PhongBan, "Department");
					if (apiResponse.Success)
					{
						return RedirectToAction("Index");
					}
					base.ModelState.AddModelError(string.Empty, apiResponse.Message);
				}
				return View(dm_PhongBan);
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
				if (!Utility.KiemTraQuyen("Department", "Delete"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				ApiResponse apiResponse = Utility.Delete<v_dm_PhongBan>(Utility.LOC_ID + "/" + id, "Department");
				if (apiResponse.Success)
				{
					return RedirectToAction("Index");
				}
				base.ModelState.AddModelError(string.Empty, apiResponse.Message);
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
				if (!Utility.KiemTraQuyen("Department", "Create"))
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
				v_v_dm_PhongBan v_v_dm_PhongBan2 = new v_v_dm_PhongBan();
				v_v_dm_PhongBan2.lstdm_PhongBan = new List<v_dm_PhongBan>();
				v_v_dm_PhongBan2.lstdm_PhongBan = Utility.GetListData<v_dm_PhongBan>("Department", "", "", Utility.LOC_ID).Data as List<v_dm_PhongBan>;
				apiResponse.Success = true;
				v_v_dm_PhongBan2.LOC_ID = Utility.LOC_ID;
				v_v_dm_PhongBan2.ID = Guid.NewGuid().ToString();
				apiResponse.Detail = Utility.ConvertobjectTo((dm_PhongBan)v_v_dm_PhongBan2, "yyyy-MM-dd HH:mm:ss");
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
		public ActionResult CreatePopup([Bind(Include = "LOC_ID,ID,MA,NAME,ID_PHONGBANCHA,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO,ISACTIVE")] v_v_dm_PhongBan dm_PhongBan)
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
				if (!Utility.KiemTraQuyen("Department", "Create"))
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
					dm_PhongBan.LOC_ID = Utility.LOC_ID;
					dm_PhongBan.ID_NGUOITAO = base.Session["idUser"].ToString();
					dm_PhongBan.THOIGIANTHEM = Utility.CurrentTime;
					apiResponse = Utility.Create((dm_PhongBan)dm_PhongBan, "Department");
					if (apiResponse.Success)
					{
						apiResponse.NewID = Guid.NewGuid().ToString();
						if (apiResponse.Data != null)
						{
							dm_PhongBan = JsonConvert.DeserializeObject<v_v_dm_PhongBan>(apiResponse.Data.ToString());
						}
					}
					else
					{
						base.ModelState.AddModelError(string.Empty, apiResponse.Message);
						if (apiResponse.CheckValue)
						{
							apiResponse.NewID = Guid.NewGuid().ToString();
						}
					}
				}
				else
				{
					apiResponse.Success = false;
					apiResponse.Data = Utility.GetModelState(base.ModelState, "Department");
				}
				apiResponse.ID = dm_PhongBan.ID;
				apiResponse.Detail = Utility.ConvertobjectToView((v_dm_PhongBan)dm_PhongBan, "dd/MM/yyyy");
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
				if (!Utility.KiemTraQuyen("Department", "Edit"))
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
				v_v_dm_PhongBan v_v_dm_PhongBan2 = new v_v_dm_PhongBan();
				if (!string.IsNullOrEmpty(id))
				{
					apiResponse = Utility.GetDetail<v_v_dm_PhongBan>(Utility.LOC_ID + "/" + id, "Department");
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
						v_v_dm_PhongBan2 = apiResponse.Data as v_v_dm_PhongBan;
					}
				}
				v_v_dm_PhongBan2.lstdm_PhongBan = new List<v_dm_PhongBan>();
				v_v_dm_PhongBan2.lstdm_PhongBan = Utility.GetListData<v_dm_PhongBan>("Department", "", "", Utility.LOC_ID).Data as List<v_dm_PhongBan>;
				apiResponse.Success = true;
				apiResponse.Detail = Utility.ConvertobjectTo(v_v_dm_PhongBan2);
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
		public ActionResult EditPopup([Bind(Include = "LOC_ID,ID,MA,NAME,ID_PHONGBANCHA,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO,ISACTIVE")] v_v_dm_PhongBan dm_PhongBan)
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
				if (!Utility.KiemTraQuyen("Department", "Edit"))
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
					dm_PhongBan.LOC_ID = Utility.LOC_ID;
					dm_PhongBan.ID_NGUOISUA = base.Session["idUser"].ToString();
					dm_PhongBan.THOIGIANSUA = Utility.CurrentTime;
					apiResponse = Utility.Edit(Utility.LOC_ID + "/" + dm_PhongBan.MA, (v_dm_PhongBan)dm_PhongBan, "Department");
					if (apiResponse.Success)
					{
						apiResponse.ID = dm_PhongBan.ID;
						if (apiResponse.Data != null)
						{
							dm_PhongBan = JsonConvert.DeserializeObject<v_v_dm_PhongBan>(apiResponse.Data.ToString());
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
					apiResponse.Data = Utility.GetModelState(base.ModelState, "Department");
				}
				apiResponse.Detail = Utility.ConvertobjectToView((v_dm_PhongBan)dm_PhongBan, "dd/MM/yyyy");
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
				if (!Utility.KiemTraQuyen("Department", "Delete"))
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
				apiResponse = Utility.Delete<v_dm_PhongBan>(Utility.LOC_ID + "/" + id, "Department");
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
	}
}
