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

	public class ProviderController : Controller
	{
		public ActionResult Index(int Page = 1, string SearchString = "", string ShowSearchValue = "")
		{
			try
			{
				if (Utility.KiemTra())
				{
					return RedirectToAction("Index", "Admin");
				}
				if (!Utility.KiemTraQuyen("Provider", "View"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				ShowSearchValue = Utility.GetShowSearchValue<dm_NhaCungCap>(ShowSearchValue);
				ApiResponse listData = Utility.GetListData<v_dm_NhaCungCap>("Provider", ShowSearchValue, SearchString, Utility.LOC_ID);
				if (!listData.Success)
				{
					base.TempData["TitleError"] = listData.Message;
					return RedirectToAction("Index", "Notfound");
				}
				IPagedList<v_dm_NhaCungCap> iPagedList = (listData.Data as List<v_dm_NhaCungCap>).ToPagedList(Page, Utility.GetPageSize());
				v_v_dm_NhaCungCap v_v_dm_NhaCungCap2 = new v_v_dm_NhaCungCap();
				v_v_dm_NhaCungCap2.IPagedList = iPagedList;
				v_v_dm_NhaCungCap2.lstdm_NhomNhaCungCap = new List<v_dm_NhomNhaCungCap>();
				v_v_dm_NhaCungCap2.lstdm_NhomNhaCungCap = Utility.GetListData<v_dm_NhomNhaCungCap>("GroupProvider", "", "", Utility.LOC_ID).Data as List<v_dm_NhomNhaCungCap>;
				base.ViewBag.searchValue = SearchString;
				base.ViewBag.showsearchValue = ShowSearchValue;
				base.ViewBag.PermissionEdit = Utility.KiemTraQuyen("Provider", "Edit");
				base.ViewBag.PermissionDelete = Utility.KiemTraQuyen("Provider", "Delete");
				base.ViewBag.PermissionCreate = Utility.KiemTraQuyen("Provider", "Create");
				return View(v_v_dm_NhaCungCap2);
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
				if (!Utility.KiemTraQuyen("Provider", "Create"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				v_v_dm_NhaCungCap v_v_dm_NhaCungCap2 = new v_v_dm_NhaCungCap();
				v_v_dm_NhaCungCap2.LOC_ID = Utility.LOC_ID;
				v_v_dm_NhaCungCap2.ID_NGUOITAO = base.Session["idUser"].ToString();
				v_v_dm_NhaCungCap2.THOIGIANTHEM = Utility.CurrentTime;
				v_v_dm_NhaCungCap2.ID = Guid.NewGuid().ToString();
				v_v_dm_NhaCungCap2.lstdm_NhomNhaCungCap = new List<v_dm_NhomNhaCungCap>();
				v_v_dm_NhaCungCap2.lstdm_NhomNhaCungCap = Utility.GetListData<v_dm_NhomNhaCungCap>("GroupProvider", "", "", Utility.LOC_ID).Data as List<v_dm_NhomNhaCungCap>;
				return View(v_v_dm_NhaCungCap2);
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
		public ActionResult Create([Bind(Include = "ID,LOC_ID,MA,NAME,ADDRESS,TEL,FAX,EMAIL,ID_NHOMNCC,ISACTIVE,ISDEFAULT,CONGNODAUKY,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO,MASOTHUE,TENNGANHANG,CHUTAIKHOAN,SOTAIKHOAN")] v_v_dm_NhaCungCap dm_NhaCungCap)
		{
			try
			{
				if (Utility.KiemTra())
				{
					return RedirectToAction("Index", "Admin");
				}
				if (!Utility.KiemTraQuyen("Provider", "Create"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				if (base.ModelState.IsValid)
				{
					dm_NhaCungCap.LOC_ID = Utility.LOC_ID;
					dm_NhaCungCap.ID_NGUOITAO = base.Session["idUser"].ToString();
					dm_NhaCungCap.THOIGIANTHEM = Utility.CurrentTime;
					ApiResponse apiResponse = Utility.Create((dm_NhaCungCap)dm_NhaCungCap, "Provider");
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
				return View(dm_NhaCungCap);
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
				if (!Utility.KiemTraQuyen("Provider", "Edit"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				v_v_dm_NhaCungCap v_v_dm_NhaCungCap2 = new v_v_dm_NhaCungCap();
				if (!string.IsNullOrEmpty(id))
				{
					apiResponse = Utility.GetDetail<v_v_dm_NhaCungCap>(Utility.LOC_ID + "/" + id, "Provider");
					if (!apiResponse.Success)
					{
						base.TempData["TitleError"] = apiResponse.Message;
						return RedirectToAction("Index", "Notfound");
					}
					if (apiResponse.Data != null)
					{
						v_v_dm_NhaCungCap2 = apiResponse.Data as v_v_dm_NhaCungCap;
					}
				}
				v_v_dm_NhaCungCap2.lstdm_NhomNhaCungCap = new List<v_dm_NhomNhaCungCap>();
				v_v_dm_NhaCungCap2.lstdm_NhomNhaCungCap = Utility.GetListData<v_dm_NhomNhaCungCap>("GroupProvider", "", "", Utility.LOC_ID).Data as List<v_dm_NhomNhaCungCap>;
				return View(v_v_dm_NhaCungCap2);
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
		public ActionResult Edit([Bind(Include = "ID,LOC_ID,MA,NAME,ADDRESS,TEL,FAX,EMAIL,ID_NHOMNCC,ISACTIVE,ISDEFAULT,CONGNODAUKY,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO,MASOTHUE,TENNGANHANG,CHUTAIKHOAN,SOTAIKHOAN")] v_v_dm_NhaCungCap dm_NhaCungCap)
		{
			try
			{
				if (Utility.KiemTra())
				{
					return RedirectToAction("Index", "Admin");
				}
				if (!Utility.KiemTraQuyen("Provider", "Edit"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				if (base.ModelState.IsValid)
				{
					dm_NhaCungCap.LOC_ID = Utility.LOC_ID;
					dm_NhaCungCap.ID_NGUOISUA = base.Session["idUser"].ToString();
					dm_NhaCungCap.THOIGIANSUA = Utility.CurrentTime;
					ApiResponse apiResponse = Utility.Edit(Utility.LOC_ID + "/" + dm_NhaCungCap.MA, (v_dm_NhaCungCap)dm_NhaCungCap, "Provider");
					if (apiResponse.Success)
					{
						return RedirectToAction("Index");
					}
					base.ModelState.AddModelError(string.Empty, apiResponse.Message);
				}
				return View(dm_NhaCungCap);
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
				if (!Utility.KiemTraQuyen("Provider", "Delete"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				ApiResponse apiResponse = Utility.Delete<v_dm_NhaCungCap>(Utility.LOC_ID + "/" + id, "Provider");
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
				if (!Utility.KiemTraQuyen("Provider", "Create"))
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
				v_v_dm_NhaCungCap v_v_dm_NhaCungCap2 = new v_v_dm_NhaCungCap();
				v_v_dm_NhaCungCap2.lstdm_NhomNhaCungCap = new List<v_dm_NhomNhaCungCap>();
				v_v_dm_NhaCungCap2.lstdm_NhomNhaCungCap = Utility.GetListData<v_dm_NhomNhaCungCap>("GroupProvider", "", "", Utility.LOC_ID).Data as List<v_dm_NhomNhaCungCap>;
				apiResponse.Success = true;
				v_v_dm_NhaCungCap2.LOC_ID = Utility.LOC_ID;
				v_v_dm_NhaCungCap2.ID = Guid.NewGuid().ToString();
				apiResponse.Detail = Utility.ConvertobjectTo((dm_NhaCungCap)v_v_dm_NhaCungCap2, "yyyy-MM-dd HH:mm:ss");
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
		public ActionResult CreatePopup([Bind(Include = "ID,LOC_ID,MA,NAME,ADDRESS,TEL,FAX,EMAIL,ID_NHOMNCC,ISACTIVE,ISDEFAULT,CONGNODAUKY,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO,MASOTHUE,TENNGANHANG,CHUTAIKHOAN,SOTAIKHOAN")] v_v_dm_NhaCungCap dm_NhaCungCap)
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
				if (!Utility.KiemTraQuyen("Provider", "Create"))
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
					dm_NhaCungCap.LOC_ID = Utility.LOC_ID;
					dm_NhaCungCap.ID_NGUOITAO = base.Session["idUser"].ToString();
					dm_NhaCungCap.THOIGIANTHEM = Utility.CurrentTime;
					apiResponse = Utility.Create((dm_NhaCungCap)dm_NhaCungCap, "Provider");
					if (apiResponse.Success)
					{
						apiResponse.NewID = Guid.NewGuid().ToString();
						if (apiResponse.Data != null && !string.IsNullOrEmpty(apiResponse.Data.ToString()))
						{
							dm_NhaCungCap = JsonConvert.DeserializeObject<v_v_dm_NhaCungCap>(apiResponse.Data.ToString());
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
					apiResponse.Data = Utility.GetModelState(base.ModelState, "Provider");
				}
				apiResponse.ID = dm_NhaCungCap.ID;
				apiResponse.Detail = Utility.ConvertobjectToView((dm_NhaCungCap)dm_NhaCungCap, "dd/MM/yyyy");
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
				if (!Utility.KiemTraQuyen("Provider", "Edit"))
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
				v_v_dm_NhaCungCap v_v_dm_NhaCungCap2 = new v_v_dm_NhaCungCap();
				if (!string.IsNullOrEmpty(id))
				{
					apiResponse = Utility.GetDetail<v_v_dm_NhaCungCap>(Utility.LOC_ID + "/" + id, "Provider");
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
						v_v_dm_NhaCungCap2 = apiResponse.Data as v_v_dm_NhaCungCap;
					}
				}
				v_v_dm_NhaCungCap2.lstdm_NhomNhaCungCap = new List<v_dm_NhomNhaCungCap>();
				v_v_dm_NhaCungCap2.lstdm_NhomNhaCungCap = Utility.GetListData<v_dm_NhomNhaCungCap>("GroupProvider", "", "", Utility.LOC_ID).Data as List<v_dm_NhomNhaCungCap>;
				apiResponse.Success = true;
				apiResponse.Detail = Utility.ConvertobjectTo(v_v_dm_NhaCungCap2);
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
		public ActionResult EditPopup([Bind(Include = "ID,LOC_ID,MA,NAME,ADDRESS,TEL,FAX,EMAIL,ID_NHOMNCC,ISACTIVE,ISDEFAULT,CONGNODAUKY,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO,MASOTHUE,TENNGANHANG,CHUTAIKHOAN,SOTAIKHOAN")] v_v_dm_NhaCungCap dm_NhaCungCap)
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
				if (!Utility.KiemTraQuyen("Provider", "Edit"))
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
					dm_NhaCungCap.LOC_ID = Utility.LOC_ID;
					dm_NhaCungCap.ID_NGUOISUA = base.Session["idUser"].ToString();
					dm_NhaCungCap.THOIGIANSUA = Utility.CurrentTime;
					apiResponse = Utility.Edit(Utility.LOC_ID + "/" + dm_NhaCungCap.MA, (v_dm_NhaCungCap)dm_NhaCungCap, "Provider");
					if (apiResponse.Success)
					{
						apiResponse.ID = dm_NhaCungCap.ID;
						if (apiResponse.Data != null && !string.IsNullOrEmpty(apiResponse.Data.ToString()))
						{
							dm_NhaCungCap = JsonConvert.DeserializeObject<v_v_dm_NhaCungCap>(apiResponse.Data.ToString());
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
					apiResponse.Data = Utility.GetModelState(base.ModelState, "Provider");
				}
				apiResponse.Detail = Utility.ConvertobjectToView((v_dm_NhaCungCap)dm_NhaCungCap, "dd/MM/yyyy");
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
				if (!Utility.KiemTraQuyen("Provider", "Delete"))
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
				apiResponse = Utility.Delete<v_dm_NhaCungCap>(Utility.LOC_ID + "/" + id, "Provider");
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
