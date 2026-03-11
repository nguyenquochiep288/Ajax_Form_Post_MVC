using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web.Mvc;
using DatabaseTHP;
using DatabaseTHP.Class;
using MVC_QuanLyTHP.Class;
using MVC_QuanLyTHP.Models;
using PagedList;

namespace MVC_QuanLyTHP.Controllers
{

	public class AnnualLeaveController : Controller
	{
		public ActionResult Index(int Page = 1, string SearchString = "", string ShowSearchValue = "")
		{
			try
			{
				if (Utility.KiemTra())
				{
					return RedirectToAction("Index", "Admin");
				}
				if (!Utility.KiemTraQuyen("AnnualLeave", "View"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				ShowSearchValue = Utility.GetShowSearchValue<nv_PhepNam>(ShowSearchValue);
				ApiResponse listData = Utility.GetListData<v_nv_PhepNam>("AnnualLeave", ShowSearchValue, SearchString, Utility.LOC_ID);
				if (!listData.Success)
				{
					base.TempData["TitleError"] = listData.Message;
					return RedirectToAction("Index", "Notfound");
				}
				IPagedList<v_nv_PhepNam> iPagedList = (listData.Data as List<v_nv_PhepNam>).ToPagedList(Page, Utility.GetPageSize());
				v_v_nv_PhepNam v_v_nv_PhepNam2 = new v_v_nv_PhepNam();
				v_v_nv_PhepNam2.IPagedList = iPagedList;
				base.ViewBag.searchValue = SearchString;
				base.ViewBag.showsearchValue = ShowSearchValue;
				base.ViewBag.PermissionEdit = Utility.KiemTraQuyen("AnnualLeave", "Edit");
				base.ViewBag.PermissionDelete = Utility.KiemTraQuyen("AnnualLeave", "Delete");
				base.ViewBag.PermissionCreate = Utility.KiemTraQuyen("AnnualLeave", "Create");
				return View(v_v_nv_PhepNam2);
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
				if (!Utility.KiemTraQuyen("AnnualLeave", "Create"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				v_v_nv_PhepNam v_v_nv_PhepNam2 = new v_v_nv_PhepNam();
				v_v_nv_PhepNam2.LOC_ID = Utility.LOC_ID;
				v_v_nv_PhepNam2.ID_NGUOITAO = base.Session["idUser"].ToString();
				v_v_nv_PhepNam2.THOIGIANTHEM = Utility.CurrentTime;
				v_v_nv_PhepNam2.ID = Guid.NewGuid().ToString();
				v_v_nv_PhepNam2.lstdm_NhanVien = new List<ComboboxFrom>();
				v_v_nv_PhepNam2.lstdm_NhanVien = Utility.GetListData<ComboboxFrom>("User").Data as List<ComboboxFrom>;
				return View(v_v_nv_PhepNam2);
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
		public ActionResult Create([Bind(Include = "LOC_ID,ID,ID_NHANVIEN,ID_THANGLUONG,NAM,NGAYBATDAU,NGAYKETTHUC,SONGAYPHEP,GHICHU,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO,SONGAYPHEPDADUNG")] v_nv_PhepNam nv_PhepNam)
		{
			try
			{
				if (Utility.KiemTra())
				{
					return RedirectToAction("Index", "Admin");
				}
				if (!Utility.KiemTraQuyen("AnnualLeave", "Create"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				if (base.ModelState.IsValid)
				{
					nv_PhepNam.LOC_ID = Utility.LOC_ID;
					nv_PhepNam.ID_NGUOITAO = base.Session["idUser"].ToString();
					nv_PhepNam.THOIGIANTHEM = Utility.CurrentTime;
					ApiResponse apiResponse = Utility.Create((nv_PhepNam)nv_PhepNam, "AnnualLeave");
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
				return View(nv_PhepNam);
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
				if (!Utility.KiemTraQuyen("AnnualLeave", "Edit"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				v_v_nv_PhepNam v_v_nv_PhepNam2 = new v_v_nv_PhepNam();
				if (!string.IsNullOrEmpty(id))
				{
					apiResponse = Utility.GetDetail<v_nv_PhepNam>(Utility.LOC_ID + "/" + id, "AnnualLeave");
					if (!apiResponse.Success)
					{
						base.TempData["TitleError"] = apiResponse.Message;
						return RedirectToAction("Index", "Notfound");
					}
					if (apiResponse.Data != null)
					{
						v_v_nv_PhepNam2 = apiResponse.Data as v_v_nv_PhepNam;
					}
				}
				v_v_nv_PhepNam2.lstdm_NhanVien = new List<ComboboxFrom>();
				v_v_nv_PhepNam2.lstdm_NhanVien = Utility.GetListData<ComboboxFrom>("User").Data as List<ComboboxFrom>;
				return View(v_v_nv_PhepNam2);
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
		public ActionResult Edit([Bind(Include = "LOC_ID,ID,ID_NHANVIEN,ID_THANGLUONG,NAM,NGAYBATDAU,NGAYKETTHUC,SONGAYPHEP,GHICHU,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO,SONGAYPHEPDADUNG")] v_nv_PhepNam nv_PhepNam)
		{
			try
			{
				if (Utility.KiemTra())
				{
					return RedirectToAction("Index", "Admin");
				}
				if (!Utility.KiemTraQuyen("AnnualLeave", "Edit"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				if (base.ModelState.IsValid)
				{
					nv_PhepNam.LOC_ID = Utility.LOC_ID;
					nv_PhepNam.ID_NGUOISUA = base.Session["idUser"].ToString();
					nv_PhepNam.THOIGIANSUA = Utility.CurrentTime;
					ApiResponse apiResponse = Utility.Edit(Utility.LOC_ID + "/" + nv_PhepNam.ID, nv_PhepNam, "AnnualLeave");
					if (apiResponse.Success)
					{
						return RedirectToAction("Index");
					}
					base.ModelState.AddModelError(string.Empty, apiResponse.Message);
				}
				return View(nv_PhepNam);
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
				if (!Utility.KiemTraQuyen("AnnualLeave", "Delete"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				ApiResponse apiResponse = Utility.Delete<v_nv_PhepNam>(Utility.LOC_ID + "/" + id, "AnnualLeave");
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
					apiResponse.URL = base.Url.Action("Index", "Admin");
					return new JsonResult
					{
						Data = apiResponse,
						JsonRequestBehavior = JsonRequestBehavior.AllowGet,
						MaxJsonLength = int.MaxValue
					};
				}
				if (!Utility.KiemTraQuyen("AnnualLeave", "Create"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					apiResponse.URL = base.Url.Action("Index", "Notfound");
					return new JsonResult
					{
						Data = apiResponse,
						JsonRequestBehavior = JsonRequestBehavior.AllowGet,
						MaxJsonLength = int.MaxValue
					};
				}
				v_v_nv_PhepNam v_v_nv_PhepNam2 = new v_v_nv_PhepNam();
				apiResponse.Success = true;
				v_v_nv_PhepNam2.LOC_ID = Utility.LOC_ID;
				v_v_nv_PhepNam2.ID = Guid.NewGuid().ToString();
				v_v_nv_PhepNam2.NAM = Utility.CurrentTime.Year;
				v_v_nv_PhepNam2.NGAYBATDAU = new DateTime(Utility.CurrentTime.Year, 1, 1);
				v_v_nv_PhepNam2.NGAYKETTHUC = new DateTime(Utility.CurrentTime.Year, 12, 31);
				v_v_nv_PhepNam2.lstdm_NhanVien = new List<ComboboxFrom>();
				List<ComboboxFrom> list = Utility.GetListData<ComboboxFrom>("User").Data as List<ComboboxFrom>;
				foreach (ComboboxFrom item in list)
				{
					item.ISACTIVE = true;
				}
				v_v_nv_PhepNam2.lstdm_NhanVien = list;
				apiResponse.Detail = Utility.ConvertobjectTo(v_v_nv_PhepNam2);
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
		public ActionResult CreatePopup([Bind(Include = "ISALL,LOC_ID,ID,ID_NHANVIEN,ID_THANGLUONG,NAM,NGAYBATDAU,NGAYKETTHUC,SONGAYPHEP,GHICHU,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO,SONGAYPHEPDADUNG")] v_nv_PhepNam nv_PhepNam)
		{
			ApiResponse apiResponse = new ApiResponse();
			try
			{
				if (Utility.KiemTra())
				{
					apiResponse.URL = base.Url.Action("Index", "Admin");
					return new JsonResult
					{
						Data = apiResponse,
						JsonRequestBehavior = JsonRequestBehavior.AllowGet,
						MaxJsonLength = int.MaxValue
					};
				}
				if (!Utility.KiemTraQuyen("AnnualLeave", "Create"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					apiResponse.URL = base.Url.Action("Index", "Notfound");
					return new JsonResult
					{
						Data = apiResponse,
						JsonRequestBehavior = JsonRequestBehavior.AllowGet,
						MaxJsonLength = int.MaxValue
					};
				}
				if (!nv_PhepNam.ISALL && string.IsNullOrEmpty(nv_PhepNam.ID_NHANVIEN))
				{
					base.ModelState.AddModelError("ID_NHANVIEN", "Vui lòng chọn nhân viên!");
				}
				if (base.ModelState.IsValid)
				{
					nv_PhepNam.LOC_ID = Utility.LOC_ID;
					nv_PhepNam.ID_NGUOITAO = base.Session["idUser"].ToString();
					nv_PhepNam.THOIGIANTHEM = Utility.CurrentTime;
					apiResponse = Utility.Create(nv_PhepNam, "AnnualLeave");
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
					apiResponse.Data = Utility.GetModelState(base.ModelState, "AnnualLeave");
				}
				apiResponse.ID = nv_PhepNam.ID;
				apiResponse.Detail = Utility.ConvertobjectTo((nv_PhepNam)nv_PhepNam, "yyyy-MM-dd HH:mm:ss");
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
					apiResponse.URL = base.Url.Action("Index", "Admin");
					return new JsonResult
					{
						Data = apiResponse,
						JsonRequestBehavior = JsonRequestBehavior.AllowGet,
						MaxJsonLength = int.MaxValue
					};
				}
				if (!Utility.KiemTraQuyen("AnnualLeave", "Edit"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					apiResponse.URL = base.Url.Action("Index", "Notfound");
					return new JsonResult
					{
						Data = apiResponse,
						JsonRequestBehavior = JsonRequestBehavior.AllowGet,
						MaxJsonLength = int.MaxValue
					};
				}
				v_v_nv_PhepNam v_v_nv_PhepNam2 = new v_v_nv_PhepNam();
				if (!string.IsNullOrEmpty(id))
				{
					apiResponse = Utility.GetDetail<v_v_nv_PhepNam>(Utility.LOC_ID + "/" + id, "AnnualLeave");
					if (!apiResponse.Success)
					{
						base.TempData["TitleError"] = apiResponse.Message;
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
						v_v_nv_PhepNam2 = apiResponse.Data as v_v_nv_PhepNam;
					}
				}
				apiResponse.Success = true;
				v_v_nv_PhepNam2.lstdm_NhanVien = new List<ComboboxFrom>();
				List<ComboboxFrom> list = Utility.GetListData<ComboboxFrom>("User").Data as List<ComboboxFrom>;
				foreach (ComboboxFrom item in list)
				{
					item.ISACTIVE = true;
				}
				v_v_nv_PhepNam2.lstdm_NhanVien = list;
				apiResponse.Detail = Utility.ConvertobjectTo(v_v_nv_PhepNam2);
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
		public ActionResult EditPopup([Bind(Include = "LOC_ID,ID,ID_NHANVIEN,ID_THANGLUONG,NAM,NGAYBATDAU,NGAYKETTHUC,SONGAYPHEP,GHICHU,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO,SONGAYPHEPDADUNG")] v_nv_PhepNam nv_PhepNam)
		{
			ApiResponse apiResponse = new ApiResponse();
			try
			{
				if (Utility.KiemTra())
				{
					apiResponse.URL = base.Url.Action("Index", "Admin");
					return new JsonResult
					{
						Data = apiResponse,
						JsonRequestBehavior = JsonRequestBehavior.AllowGet,
						MaxJsonLength = int.MaxValue
					};
				}
				if (!Utility.KiemTraQuyen("AnnualLeave", "Edit"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
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
					nv_PhepNam.LOC_ID = Utility.LOC_ID;
					nv_PhepNam.ID_NGUOISUA = base.Session["idUser"].ToString();
					nv_PhepNam.THOIGIANSUA = Utility.CurrentTime;
					apiResponse = Utility.Edit(Utility.LOC_ID + "/" + nv_PhepNam.ID_NHANVIEN + "/" + nv_PhepNam.NAM, nv_PhepNam, "AnnualLeave");
					if (apiResponse.Success)
					{
						apiResponse.ID = nv_PhepNam.ID;
					}
					else
					{
						base.ModelState.AddModelError(string.Empty, apiResponse.Message);
					}
				}
				else
				{
					apiResponse.Success = false;
					apiResponse.Data = Utility.GetModelState(base.ModelState, "AnnualLeave");
				}
				apiResponse.Detail = Utility.ConvertobjectTo(nv_PhepNam, "dd/MM/yyyy");
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
					apiResponse.URL = base.Url.Action("Index", "Admin");
					return new JsonResult
					{
						Data = apiResponse,
						JsonRequestBehavior = JsonRequestBehavior.AllowGet,
						MaxJsonLength = int.MaxValue
					};
				}
				if (!Utility.KiemTraQuyen("AnnualLeave", "Delete"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					apiResponse.URL = base.Url.Action("Index", "Notfound");
					return new JsonResult
					{
						Data = apiResponse,
						JsonRequestBehavior = JsonRequestBehavior.AllowGet,
						MaxJsonLength = int.MaxValue
					};
				}
				apiResponse = Utility.Delete<v_nv_PhepNam>(Utility.LOC_ID + "/" + id, "AnnualLeave");
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
