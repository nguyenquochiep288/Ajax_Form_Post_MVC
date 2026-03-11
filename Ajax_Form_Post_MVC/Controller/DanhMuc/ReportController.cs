using System;
using System.Collections.Generic;
using System.Linq;
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

	public class ReportController : Controller
	{
		public ActionResult Index(int Page = 1, string SearchString = "", string ShowSearchValue = "")
		{
			try
			{
				if (Utility.KiemTra())
				{
					return RedirectToAction("Index", "Admin");
				}
				if (!Utility.KiemTraQuyen("Report", "View"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				ShowSearchValue = Utility.GetShowSearchValue<web_Report>(ShowSearchValue);
				ApiResponse listData = Utility.GetListData<v_web_Report>("Report", ShowSearchValue, SearchString);
				if (!listData.Success)
				{
					base.TempData["TitleError"] = listData.Message;
					return RedirectToAction("Index", "Notfound");
				}
				IPagedList<v_web_Report> iPagedList = (listData.Data as List<v_web_Report>).ToPagedList(Page, Utility.GetPageSize());
				v_v_web_Report v_v_web_Report2 = new v_v_web_Report();
				v_v_web_Report2.IPagedList = iPagedList;
				base.ViewBag.searchValue = SearchString;
				base.ViewBag.showsearchValue = ShowSearchValue;
				base.ViewBag.PermissionEdit = Utility.KiemTraQuyen("Report", "Edit");
				base.ViewBag.PermissionDelete = Utility.KiemTraQuyen("Report", "Delete");
				base.ViewBag.PermissionCreate = Utility.KiemTraQuyen("Report", "Create");
				return View(v_v_web_Report2);
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
				if (!Utility.KiemTraQuyen("Report", "Create"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				v_v_web_Report v_v_web_Report2 = new v_v_web_Report();
				v_v_web_Report2.lstweb_Menu = new List<v_web_Menu>();
				v_v_web_Report2.lstweb_Report_Parameter = new List<v_web_Report_Parameter>();
				v_v_web_Report2.ID = Guid.NewGuid().ToString();
				return View(v_v_web_Report2);
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
		public ActionResult Create([Bind(Include = "ID_MENU,ID,MA,NAME,NAME_SP,NOTE,REPORT")] v_v_web_Report web_Report)
		{
			try
			{
				if (Utility.KiemTra())
				{
					return RedirectToAction("Index", "Admin");
				}
				if (!Utility.KiemTraQuyen("Report", "Create"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				if (base.ModelState.IsValid)
				{
					ApiResponse apiResponse = Utility.Create((web_Report)web_Report, "Report");
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
				web_Report.lstweb_Menu = new List<v_web_Menu>();
				web_Report.lstweb_Menu = Utility.GetListData<v_web_Menu>("Menu").Data as List<v_web_Menu>;
				web_Report.lstweb_Report_Parameter = new List<v_web_Report_Parameter>();
				web_Report.lstweb_Report_Parameter = Utility.GetListData<v_web_Report_Parameter>("Parameter").Data as List<v_web_Report_Parameter>;
				return View(web_Report);
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
				if (!Utility.KiemTraQuyen("Report", "Edit"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				v_v_web_Report v_v_web_Report2 = new v_v_web_Report();
				if (!string.IsNullOrEmpty(id))
				{
					apiResponse = Utility.GetDetail<v_v_web_Report>(id, "Report");
					if (!apiResponse.Success)
					{
						base.TempData["TitleError"] = apiResponse.Message;
						return RedirectToAction("Index", "Notfound");
					}
					if (apiResponse.Data != null)
					{
						v_v_web_Report2 = apiResponse.Data as v_v_web_Report;
					}
				}
				v_v_web_Report2.lstweb_Menu = new List<v_web_Menu>();
				v_v_web_Report2.lstweb_Report_Parameter = new List<v_web_Report_Parameter>();
				return View(v_v_web_Report2);
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
		public ActionResult Edit([Bind(Include = "ID_MENU,ID,MA,NAME,NAME_SP,NOTE,REPORT")] v_v_web_Report web_Report)
		{
			try
			{
				if (Utility.KiemTra())
				{
					return RedirectToAction("Index", "Admin");
				}
				if (!Utility.KiemTraQuyen("Report", "Edit"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				if (base.ModelState.IsValid)
				{
					ApiResponse apiResponse = Utility.Edit(web_Report.MA, (v_web_Report)web_Report, "Report");
					if (apiResponse.Success)
					{
						return RedirectToAction("Index");
					}
					base.ModelState.AddModelError(string.Empty, apiResponse.Message);
				}
				web_Report.lstweb_Menu = new List<v_web_Menu>();
				web_Report.lstweb_Menu = Utility.GetListData<v_web_Menu>("Menu").Data as List<v_web_Menu>;
				web_Report.lstweb_Report_Parameter = new List<v_web_Report_Parameter>();
				web_Report.lstweb_Report_Parameter = Utility.GetListData<v_web_Report_Parameter>("Parameter").Data as List<v_web_Report_Parameter>;
				return View(web_Report);
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
				if (!Utility.KiemTraQuyen("Report", "Delete"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				ApiResponse apiResponse = Utility.Delete<v_web_Report>(id, "Report");
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
				if (!Utility.KiemTraQuyen("Report", "Create"))
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
				v_v_web_Report v_v_web_Report2 = new v_v_web_Report();
				apiResponse.Success = true;
				v_v_web_Report2.ID = Guid.NewGuid().ToString();
				v_v_web_Report2.lstweb_Menu = new List<v_web_Menu>();
				v_v_web_Report2.lstweb_Menu = Utility.GetListData<v_web_Menu>("Menu").Data as List<v_web_Menu>;
				v_v_web_Report2.lstweb_Report_Parameter = new List<v_web_Report_Parameter>();
				v_v_web_Report2.lstweb_Report_Parameter = Utility.GetListData<v_web_Report_Parameter>("Parameter").Data as List<v_web_Report_Parameter>;
				List<ValueEdit> list = Utility.ConvertobjectTo(v_v_web_Report2);
				apiResponse.ProductCombo = Utility.GetParameter(v_v_web_Report2.lstweb_Report_Parameter);
				list.Add(new ValueEdit
				{
					Key = "tbodyReport_Add",
					Value = apiResponse.ProductCombo
				});
				apiResponse.Detail = list;
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
		public ActionResult CreatePopup([Bind(Include = "ID_MENU,ID,MA,NAME,NAME_SP,NOTE,REPORT")] v_v_web_Report web_Report)
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
				if (!Utility.KiemTraQuyen("Report", "Create"))
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
				IEnumerable<string> enumerable = base.Request.Form.AllKeys.Where((string e) => e.StartsWith("txt"));
				if (enumerable == null || enumerable.Count() == 0)
				{
					base.ModelState.AddModelError("lstweb_Report_Parameter", "Thêm danh sách parameter.");
				}
				if (base.ModelState.IsValid)
				{
					web_Report.lstweb_Report_Parameter = new List<v_web_Report_Parameter>();
					v_web_Report_Parameter v_web_Report_Parameter2 = new v_web_Report_Parameter();
					foreach (string item in enumerable)
					{
						string[] array = item.ToString().Split('|');
						string[] values = base.HttpContext.Request.Params.GetValues(item.ToString());
						string value = clsMaHoa.Decrypt(array[1].ToString(), "tmt6364");
						v_web_Report_Parameter v_web_Report_Parameter3 = JsonConvert.DeserializeObject<v_web_Report_Parameter>(value);
						if (v_web_Report_Parameter2.ID != v_web_Report_Parameter3.ID)
						{
							v_web_Report_Parameter2 = new v_web_Report_Parameter();
							v_web_Report_Parameter2 = JsonConvert.DeserializeObject<v_web_Report_Parameter>(value);
							web_Report.lstweb_Report_Parameter.Add(v_web_Report_Parameter2);
						}
						Utility.EditObject(v_web_Report_Parameter2, array[0].ToString().Substring(3, array[0].ToString().Length - 3), values[0]);
					}
					apiResponse = Utility.Create((v_web_Report)web_Report, "Report");
					if (apiResponse.Success)
					{
						apiResponse.NewID = Guid.NewGuid().ToString();
						if (apiResponse.Data != null && !string.IsNullOrEmpty(apiResponse.Data.ToString()))
						{
							web_Report = JsonConvert.DeserializeObject<v_v_web_Report>(apiResponse.Data.ToString());
						}
						web_Report.lstweb_Report_Parameter = new List<v_web_Report_Parameter>();
						web_Report.lstweb_Report_Parameter = Utility.GetListData<v_web_Report_Parameter>("Parameter").Data as List<v_web_Report_Parameter>;
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
					apiResponse.Data = Utility.GetModelState(base.ModelState, "Report");
				}
				apiResponse.ID = web_Report.ID;
				web_Report.lstweb_Menu = new List<v_web_Menu>();
				web_Report.lstweb_Menu = Utility.GetListData<v_web_Menu>("Menu").Data as List<v_web_Menu>;
				apiResponse.Detail = Utility.ConvertobjectToView((v_web_Report)web_Report, "dd/MM/yyyy");
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
				if (!Utility.KiemTraQuyen("Report", "Edit"))
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
				v_v_web_Report v_v_web_Report2 = new v_v_web_Report();
				if (!string.IsNullOrEmpty(id))
				{
					apiResponse = Utility.GetDetail<v_v_web_Report>(id, "Report");
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
						v_v_web_Report2 = apiResponse.Data as v_v_web_Report;
					}
				}
				apiResponse.Success = true;
				v_v_web_Report2.lstweb_Menu = new List<v_web_Menu>();
				v_v_web_Report2.lstweb_Menu = Utility.GetListData<v_web_Menu>("Menu").Data as List<v_web_Menu>;
				List<v_web_Report_Parameter> list = new List<v_web_Report_Parameter>();
				list = Utility.GetListData<v_web_Report_Parameter>("Parameter").Data as List<v_web_Report_Parameter>;
				foreach (v_web_Report_Parameter itm in list)
				{
					if (v_v_web_Report2.lstweb_Report_Parameter.Where((v_web_Report_Parameter e) => e.ID_PARAMETER == itm.ID).Count() == 0)
					{
						v_v_web_Report2.lstweb_Report_Parameter.Add(itm);
					}
				}
				List<ValueEdit> list2 = Utility.ConvertobjectTo(v_v_web_Report2);
				apiResponse.ProductCombo = Utility.GetParameter(v_v_web_Report2.lstweb_Report_Parameter);
				list2.Add(new ValueEdit
				{
					Key = "tbodyReport_Edit",
					Value = apiResponse.ProductCombo
				});
				apiResponse.Detail = list2;
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
		public ActionResult EditPopup([Bind(Include = "ID_MENU,ID,MA,NAME,NAME_SP,NOTE,REPORT")] v_v_web_Report web_Report)
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
				if (!Utility.KiemTraQuyen("Report", "Edit"))
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
				IEnumerable<string> enumerable = base.Request.Form.AllKeys.Where((string e) => e.StartsWith("txt"));
				if (enumerable == null || enumerable.Count() == 0)
				{
					base.ModelState.AddModelError("lstweb_Report_Parameter", "Thêm danh sách parameter.");
				}
				if (base.ModelState.IsValid)
				{
					web_Report.lstweb_Report_Parameter = new List<v_web_Report_Parameter>();
					v_web_Report_Parameter v_web_Report_Parameter2 = new v_web_Report_Parameter();
					foreach (string item in enumerable)
					{
						string[] array = item.ToString().Split('|');
						string[] values = base.HttpContext.Request.Params.GetValues(item.ToString());
						string value = clsMaHoa.Decrypt(array[1].ToString(), "tmt6364");
						v_web_Report_Parameter v_web_Report_Parameter3 = JsonConvert.DeserializeObject<v_web_Report_Parameter>(value);
						if (v_web_Report_Parameter2.ID != v_web_Report_Parameter3.ID)
						{
							v_web_Report_Parameter2 = new v_web_Report_Parameter();
							v_web_Report_Parameter2 = JsonConvert.DeserializeObject<v_web_Report_Parameter>(value);
							v_web_Report_Parameter2.ISACTIVE = false;
							web_Report.lstweb_Report_Parameter.Add(v_web_Report_Parameter2);
						}
						Utility.EditObject(v_web_Report_Parameter2, array[0].ToString().Substring(3, array[0].ToString().Length - 3), values[0]);
					}
					apiResponse = Utility.Edit(web_Report.MA, (v_web_Report)web_Report, "Report");
					if (apiResponse.Success)
					{
						apiResponse.ID = web_Report.ID;
						if (apiResponse.Data != null && !string.IsNullOrEmpty(apiResponse.Data.ToString()))
						{
							web_Report = JsonConvert.DeserializeObject<v_v_web_Report>(apiResponse.Data.ToString());
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
					apiResponse.Data = Utility.GetModelState(base.ModelState, "Report");
				}
				web_Report.lstweb_Menu = new List<v_web_Menu>();
				web_Report.lstweb_Menu = Utility.GetListData<v_web_Menu>("Menu").Data as List<v_web_Menu>;
				List<ValueEdit> list = Utility.ConvertobjectToView(web_Report);
				apiResponse.ProductCombo = Utility.GetParameter(web_Report.lstweb_Report_Parameter);
				list.Add(new ValueEdit
				{
					Key = "tbodyReport_Edit",
					Value = apiResponse.ProductCombo
				});
				apiResponse.Detail = list;
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
				if (!Utility.KiemTraQuyen("Report", "Delete"))
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
				apiResponse = Utility.Delete<v_web_Report>(id, "Report");
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
