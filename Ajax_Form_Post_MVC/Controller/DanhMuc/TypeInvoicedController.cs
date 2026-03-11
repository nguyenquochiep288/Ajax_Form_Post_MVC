using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using DatabaseTHP;
using DatabaseTHP.Class;
using MVC_QuanLyTHP.Class;
using MVC_QuanLyTHP.Models;
using PagedList;

namespace MVC_QuanLyTHP.Controllers
{

	public class TypeInvoicedController : Controller
	{
		public ActionResult Index(int Page = 1, string SearchString = "", string ShowSearchValue = "")
		{
			try
			{
				if (Utility.KiemTra())
				{
					return RedirectToAction("Index", "Admin");
				}
				if (!Utility.KiemTraQuyen("TypeInvoiced", "View"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				ShowSearchValue = Utility.GetShowSearchValue<dm_LoaiHoaDon>(ShowSearchValue);
				ApiResponse listData = Utility.GetListData<v_dm_LoaiHoaDon>("TypeInvoiced", ShowSearchValue, SearchString, Utility.LOC_ID);
				if (!listData.Success)
				{
					base.TempData["TitleError"] = listData.Message;
					return RedirectToAction("Index", "Notfound");
				}
				IPagedList<v_dm_LoaiHoaDon> iPagedList = (listData.Data as List<v_dm_LoaiHoaDon>).ToPagedList(Page, Utility.GetPageSize());
				v_v_dm_LoaiHoaDon v_v_dm_LoaiHoaDon2 = new v_v_dm_LoaiHoaDon();
				v_v_dm_LoaiHoaDon2.lstDanhSachMau = new List<ComboboxFrom>();
				v_v_dm_LoaiHoaDon2.IPagedList = iPagedList;
				base.ViewBag.searchValue = SearchString;
				base.ViewBag.showsearchValue = ShowSearchValue;
				base.ViewBag.PermissionEdit = Utility.KiemTraQuyen("TypeInvoiced", "Edit");
				base.ViewBag.PermissionDelete = Utility.KiemTraQuyen("TypeInvoiced", "Delete");
				base.ViewBag.PermissionCreate = Utility.KiemTraQuyen("TypeInvoiced", "Create");
				return View(v_v_dm_LoaiHoaDon2);
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
				if (!Utility.KiemTraQuyen("TypeInvoiced", "Create"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				v_v_dm_LoaiHoaDon v_v_dm_LoaiHoaDon2 = new v_v_dm_LoaiHoaDon();
				v_v_dm_LoaiHoaDon2.LOC_ID = Utility.LOC_ID;
				v_v_dm_LoaiHoaDon2.ID_NGUOITAO = base.Session["idUser"].ToString();
				v_v_dm_LoaiHoaDon2.THOIGIANTHEM = Utility.CurrentTime;
				v_v_dm_LoaiHoaDon2.ID = Guid.NewGuid().ToString();
				v_v_dm_LoaiHoaDon2.lstDanhSachMau = new List<ComboboxFrom>();
				return View(v_v_dm_LoaiHoaDon2);
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
		public ActionResult Create([Bind(Include = "LOC_ID,ID,MA,NAME,NOTE,ISACTIVE,ISDEFAULT,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO,ISSYSTEM,IPTEMPLATEID,INVSERIES")] v_v_dm_LoaiHoaDon dm_LoaiHoaDon)
		{
			try
			{
				if (Utility.KiemTra())
				{
					return RedirectToAction("Index", "Admin");
				}
				if (!Utility.KiemTraQuyen("TypeInvoiced", "Create"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				if (base.ModelState.IsValid)
				{
					dm_LoaiHoaDon.LOC_ID = Utility.LOC_ID;
					dm_LoaiHoaDon.ID_NGUOITAO = base.Session["idUser"].ToString();
					dm_LoaiHoaDon.THOIGIANTHEM = Utility.CurrentTime;
					ApiResponse apiResponse = Utility.Create((dm_LoaiHoaDon)dm_LoaiHoaDon, "TypeInvoiced");
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
				dm_LoaiHoaDon.lstDanhSachMau = Utility.DanhSachMauHoaDon();
				return View(dm_LoaiHoaDon);
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
				if (!Utility.KiemTraQuyen("TypeInvoiced", "Edit"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				v_v_dm_LoaiHoaDon v_v_dm_LoaiHoaDon2 = new v_v_dm_LoaiHoaDon();
				if (!string.IsNullOrEmpty(id))
				{
					apiResponse = Utility.GetDetail<v_v_dm_LoaiHoaDon>(Utility.LOC_ID + "/" + id, "TypeInvoiced");
					if (!apiResponse.Success)
					{
						base.TempData["TitleError"] = apiResponse.Message;
						return RedirectToAction("Index", "Notfound");
					}
					if (apiResponse.Data != null)
					{
						v_v_dm_LoaiHoaDon2 = apiResponse.Data as v_v_dm_LoaiHoaDon;
					}
				}
				v_v_dm_LoaiHoaDon2.lstDanhSachMau = new List<ComboboxFrom>();
				return View(v_v_dm_LoaiHoaDon2);
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
		public ActionResult Edit([Bind(Include = "LOC_ID,ID,MA,NAME,NOTE,ISACTIVE,ISDEFAULT,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO,ISSYSTEM,IPTEMPLATEID,INVSERIES")] v_v_dm_LoaiHoaDon dm_LoaiHoaDon)
		{
			try
			{
				if (Utility.KiemTra())
				{
					return RedirectToAction("Index", "Admin");
				}
				if (!Utility.KiemTraQuyen("TypeInvoiced", "Edit"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				if (base.ModelState.IsValid)
				{
					dm_LoaiHoaDon.LOC_ID = Utility.LOC_ID;
					dm_LoaiHoaDon.ID_NGUOISUA = base.Session["idUser"].ToString();
					dm_LoaiHoaDon.THOIGIANSUA = Utility.CurrentTime;
					ApiResponse apiResponse = Utility.Edit(Utility.LOC_ID + "/" + dm_LoaiHoaDon.MA, (v_dm_LoaiHoaDon)dm_LoaiHoaDon, "TypeInvoiced");
					if (apiResponse.Success)
					{
						return RedirectToAction("Index");
					}
					base.ModelState.AddModelError(string.Empty, apiResponse.Message);
				}
				dm_LoaiHoaDon.lstDanhSachMau = Utility.DanhSachMauHoaDon();
				return View(dm_LoaiHoaDon);
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
				if (!Utility.KiemTraQuyen("TypeInvoiced", "Delete"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				ApiResponse apiResponse = Utility.Delete<v_dm_LoaiHoaDon>(Utility.LOC_ID + "/" + id, "TypeInvoiced");
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
				if (!Utility.KiemTraQuyen("TypeInvoiced", "Create"))
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
				v_v_dm_LoaiHoaDon v_v_dm_LoaiHoaDon2 = new v_v_dm_LoaiHoaDon();
				apiResponse.Success = true;
				v_v_dm_LoaiHoaDon2.LOC_ID = Utility.LOC_ID;
				v_v_dm_LoaiHoaDon2.ID = Guid.NewGuid().ToString();
				v_v_dm_LoaiHoaDon2.lstDanhSachMau = new List<ComboboxFrom>();
				v_v_dm_LoaiHoaDon2.lstDanhSachMau = Utility.DanhSachMauHoaDon();
				apiResponse.Detail = Utility.ConvertobjectTo((dm_LoaiHoaDon)v_v_dm_LoaiHoaDon2, "yyyy-MM-dd HH:mm:ss");
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
		public ActionResult CreatePopup([Bind(Include = "LOC_ID,ID,MA,NAME,NOTE,ISACTIVE,ISDEFAULT,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO,ISSYSTEM,IPTEMPLATEID,INVSERIES")] v_v_dm_LoaiHoaDon dm_LoaiHoaDon)
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
				if (!Utility.KiemTraQuyen("TypeInvoiced", "Create"))
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
					dm_LoaiHoaDon.LOC_ID = Utility.LOC_ID;
					dm_LoaiHoaDon.ID_NGUOITAO = base.Session["idUser"].ToString();
					dm_LoaiHoaDon.THOIGIANTHEM = Utility.CurrentTime;
					apiResponse = Utility.Create((dm_LoaiHoaDon)dm_LoaiHoaDon, "TypeInvoiced");
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
					}
				}
				else
				{
					apiResponse.Success = false;
					apiResponse.Data = Utility.GetModelState(base.ModelState, "TypeInvoiced");
				}
				apiResponse.ID = dm_LoaiHoaDon.ID;
				dm_LoaiHoaDon.lstDanhSachMau = new List<ComboboxFrom>();
				dm_LoaiHoaDon.lstDanhSachMau = Utility.DanhSachMauHoaDon();
				apiResponse.Detail = Utility.ConvertobjectTo((dm_LoaiHoaDon)dm_LoaiHoaDon, "yyyy-MM-dd HH:mm:ss");
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
				if (!Utility.KiemTraQuyen("TypeInvoiced", "Edit"))
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
				v_v_dm_LoaiHoaDon dm_LoaiHoaDon2 = new v_v_dm_LoaiHoaDon();
				if (!string.IsNullOrEmpty(id))
				{
					apiResponse = Utility.GetDetail<v_v_dm_LoaiHoaDon>(Utility.LOC_ID + "/" + id, "TypeInvoiced");
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
						dm_LoaiHoaDon2 = apiResponse.Data as v_v_dm_LoaiHoaDon;
					}
				}
				apiResponse.Success = true;
				dm_LoaiHoaDon2.lstDanhSachMau = new List<ComboboxFrom>();
				dm_LoaiHoaDon2.lstDanhSachMau = Utility.DanhSachMauHoaDon();
				if (!string.IsNullOrEmpty(dm_LoaiHoaDon2.IPTEMPLATEID) && dm_LoaiHoaDon2.lstDanhSachMau.Where((ComboboxFrom s) => s.ID == dm_LoaiHoaDon2.IPTEMPLATEID).Count() == 0)
				{
					dm_LoaiHoaDon2.lstDanhSachMau.Add(new ComboboxFrom
					{
						ID = dm_LoaiHoaDon2.IPTEMPLATEID,
						NAME = dm_LoaiHoaDon2.INVSERIES
					});
				}
				apiResponse.Detail = Utility.ConvertobjectTo(dm_LoaiHoaDon2);
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
		public ActionResult EditPopup([Bind(Include = "LOC_ID,ID,MA,NAME,NOTE,ISACTIVE,ISDEFAULT,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO,ISSYSTEM,IPTEMPLATEID,INVSERIES")] v_v_dm_LoaiHoaDon dm_LoaiHoaDon)
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
				if (!Utility.KiemTraQuyen("TypeInvoiced", "Edit"))
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
					dm_LoaiHoaDon.LOC_ID = Utility.LOC_ID;
					dm_LoaiHoaDon.ID_NGUOISUA = base.Session["idUser"].ToString();
					dm_LoaiHoaDon.THOIGIANSUA = Utility.CurrentTime;
					apiResponse = Utility.Edit(Utility.LOC_ID + "/" + dm_LoaiHoaDon.MA, (v_dm_LoaiHoaDon)dm_LoaiHoaDon, "TypeInvoiced");
					if (apiResponse.Success)
					{
						apiResponse.ID = dm_LoaiHoaDon.ID;
					}
					else
					{
						base.ModelState.AddModelError(string.Empty, apiResponse.Message);
					}
				}
				else
				{
					apiResponse.Success = false;
					apiResponse.Data = Utility.GetModelState(base.ModelState, "TypeInvoiced");
				}
				dm_LoaiHoaDon.lstDanhSachMau = new List<ComboboxFrom>();
				dm_LoaiHoaDon.lstDanhSachMau = Utility.DanhSachMauHoaDon();
				apiResponse.Detail = Utility.ConvertobjectTo((dm_LoaiHoaDon)dm_LoaiHoaDon, "yyyy-MM-dd HH:mm:ss");
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
				if (!Utility.KiemTraQuyen("TypeInvoiced", "Delete"))
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
				apiResponse = Utility.Delete<v_dm_LoaiHoaDon>(Utility.LOC_ID + "/" + id, "TypeInvoiced");
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
