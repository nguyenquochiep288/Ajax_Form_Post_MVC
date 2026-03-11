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

	public class CustomerController : Controller
	{
		public ActionResult Index(int Page = 1, string SearchString = "", string ShowSearchValue = "")
		{
			try
			{
				if (Utility.KiemTra())
				{
					return RedirectToAction("Index", "Admin");
				}
				if (!Utility.KiemTraQuyen("Customer", "View"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				ShowSearchValue = Utility.GetShowSearchValue<dm_KhachHang>(ShowSearchValue);
				ApiResponse listData = Utility.GetListData<v_dm_KhachHang>("Customer", ShowSearchValue, SearchString, Utility.LOC_ID);
				if (!listData.Success)
				{
					base.TempData["TitleError"] = listData.Message;
					return RedirectToAction("Index", "Notfound");
				}
				IPagedList<v_dm_KhachHang> iPagedList = (listData.Data as List<v_dm_KhachHang>).ToPagedList(Page, Utility.GetPageSize());
				v_v_dm_KhachHang v_v_dm_KhachHang2 = new v_v_dm_KhachHang();
				v_v_dm_KhachHang2.IPagedList = iPagedList;
				v_v_dm_KhachHang2.lstdm_KhuVuc = new List<v_dm_KhuVuc>();
				v_v_dm_KhachHang2.lstdm_KhuVuc = Utility.GetListData<v_dm_KhuVuc>("Area", "", "", Utility.LOC_ID).Data as List<v_dm_KhuVuc>;
				v_v_dm_KhachHang2.lstdm_NhomKhachHang = new List<v_dm_NhomKhachHang>();
				v_v_dm_KhachHang2.lstdm_NhomKhachHang = Utility.GetListData<v_dm_NhomKhachHang>("GroupCustomer", "", "", Utility.LOC_ID).Data as List<v_dm_NhomKhachHang>;
				base.ViewBag.searchValue = SearchString;
				base.ViewBag.showsearchValue = ShowSearchValue;
				base.ViewBag.PermissionEdit = Utility.KiemTraQuyen("Customer", "Edit");
				base.ViewBag.PermissionDelete = Utility.KiemTraQuyen("Customer", "Delete");
				base.ViewBag.PermissionCreate = Utility.KiemTraQuyen("Customer", "Create");
				return View(v_v_dm_KhachHang2);
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
				if (!Utility.KiemTraQuyen("Customer", "Create"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				v_v_dm_KhachHang v_v_dm_KhachHang2 = new v_v_dm_KhachHang();
				v_v_dm_KhachHang2.LOC_ID = Utility.LOC_ID;
				v_v_dm_KhachHang2.ID_NGUOITAO = base.Session["idUser"].ToString();
				v_v_dm_KhachHang2.THOIGIANTHEM = Utility.CurrentTime;
				v_v_dm_KhachHang2.ID = Guid.NewGuid().ToString();
				v_v_dm_KhachHang2.lstdm_KhuVuc = new List<v_dm_KhuVuc>();
				v_v_dm_KhachHang2.lstdm_KhuVuc = Utility.GetListData<v_dm_KhuVuc>("Area", "", "", Utility.LOC_ID).Data as List<v_dm_KhuVuc>;
				v_v_dm_KhachHang2.lstdm_NhomKhachHang = new List<v_dm_NhomKhachHang>();
				v_v_dm_KhachHang2.lstdm_NhomKhachHang = Utility.GetListData<v_dm_NhomKhachHang>("GroupCustomer", "", "", Utility.LOC_ID).Data as List<v_dm_NhomKhachHang>;
				return View(v_v_dm_KhachHang2);
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
		public ActionResult Create([Bind(Include = "LOC_ID,ID,MA,NAME,ADDRESS,TEL,FAX,EMAIL,NGAYSINH,DIS,RATE,ID_NHOMKHACHHANG,MAX_CONGNO,SONGAY,MAHANG_KH_LK,LEVEL_PRICE,ID_KHUVUC,ISACTIVE,ISDEFAULT,CONGNODAUKY,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO,TENKHACHHANG,TENDONVI,DIACHI,MASOTHUE,CCCD")] v_v_dm_KhachHang dm_KhachHang)
		{
			try
			{
				if (Utility.KiemTra())
				{
					return RedirectToAction("Index", "Admin");
				}
				if (!Utility.KiemTraQuyen("Customer", "Create"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				if (base.ModelState.IsValid)
				{
					dm_KhachHang.LOC_ID = Utility.LOC_ID;
					dm_KhachHang.ID_NGUOITAO = base.Session["idUser"].ToString();
					dm_KhachHang.THOIGIANTHEM = Utility.CurrentTime;
					ApiResponse apiResponse = Utility.Create((dm_KhachHang)dm_KhachHang, "Customer");
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
				return View(dm_KhachHang);
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
				if (!Utility.KiemTraQuyen("Customer", "Edit"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				v_v_dm_KhachHang v_v_dm_KhachHang2 = new v_v_dm_KhachHang();
				if (!string.IsNullOrEmpty(id))
				{
					apiResponse = Utility.GetDetail<v_v_dm_KhachHang>(Utility.LOC_ID + "/" + id, "Customer");
					if (!apiResponse.Success)
					{
						base.TempData["TitleError"] = apiResponse.Message;
						return RedirectToAction("Index", "Notfound");
					}
					if (apiResponse.Data != null)
					{
						v_v_dm_KhachHang2 = apiResponse.Data as v_v_dm_KhachHang;
					}
				}
				v_v_dm_KhachHang2.lstdm_KhuVuc = new List<v_dm_KhuVuc>();
				v_v_dm_KhachHang2.lstdm_KhuVuc = Utility.GetListData<v_dm_KhuVuc>("Area", "", "", Utility.LOC_ID).Data as List<v_dm_KhuVuc>;
				v_v_dm_KhachHang2.lstdm_NhomKhachHang = new List<v_dm_NhomKhachHang>();
				v_v_dm_KhachHang2.lstdm_NhomKhachHang = Utility.GetListData<v_dm_NhomKhachHang>("GroupCustomer", "", "", Utility.LOC_ID).Data as List<v_dm_NhomKhachHang>;
				return View(v_v_dm_KhachHang2);
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
		public ActionResult Edit([Bind(Include = "LOC_ID,ID,MA,NAME,ADDRESS,TEL,FAX,EMAIL,NGAYSINH,DIS,RATE,ID_NHOMKHACHHANG,MAX_CONGNO,SONGAY,MAHANG_KH_LK,LEVEL_PRICE,ID_KHUVUC,ISACTIVE,ISDEFAULT,CONGNODAUKY,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO,TENKHACHHANG,TENDONVI,DIACHI,MASOTHUE,CCCD")] v_v_dm_KhachHang dm_KhachHang)
		{
			try
			{
				if (Utility.KiemTra())
				{
					return RedirectToAction("Index", "Admin");
				}
				if (!Utility.KiemTraQuyen("Customer", "Edit"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				if (base.ModelState.IsValid)
				{
					dm_KhachHang.LOC_ID = Utility.LOC_ID;
					dm_KhachHang.ID_NGUOISUA = base.Session["idUser"].ToString();
					dm_KhachHang.THOIGIANSUA = Utility.CurrentTime;
					ApiResponse apiResponse = Utility.Edit(Utility.LOC_ID + "/" + dm_KhachHang.MA, (v_dm_KhachHang)dm_KhachHang, "Customer");
					if (apiResponse.Success)
					{
						return RedirectToAction("Index");
					}
					base.ModelState.AddModelError(string.Empty, apiResponse.Message);
				}
				return View(dm_KhachHang);
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
				if (!Utility.KiemTraQuyen("Customer", "Delete"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				ApiResponse apiResponse = Utility.Delete<v_dm_KhachHang>(Utility.LOC_ID + "/" + id, "Customer");
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
				if (!Utility.KiemTraQuyen("Customer", "Create"))
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
				v_v_dm_KhachHang v_v_dm_KhachHang2 = new v_v_dm_KhachHang();
				v_v_dm_KhachHang2.lstdm_KhuVuc = new List<v_dm_KhuVuc>();
				v_v_dm_KhachHang2.lstdm_KhuVuc = Utility.GetListData<v_dm_KhuVuc>("Area", "", "", Utility.LOC_ID).Data as List<v_dm_KhuVuc>;
				v_v_dm_KhachHang2.lstdm_NhomKhachHang = new List<v_dm_NhomKhachHang>();
				v_v_dm_KhachHang2.lstdm_NhomKhachHang = Utility.GetListData<v_dm_NhomKhachHang>("GroupCustomer", "", "", Utility.LOC_ID).Data as List<v_dm_NhomKhachHang>;
				apiResponse.Success = true;
				v_v_dm_KhachHang2.LOC_ID = Utility.LOC_ID;
				v_v_dm_KhachHang2.ID = Guid.NewGuid().ToString();
				apiResponse.Detail = Utility.ConvertobjectTo((dm_KhachHang)v_v_dm_KhachHang2, "yyyy-MM-dd HH:mm:ss");
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
		public ActionResult CreatePopup([Bind(Include = "LOC_ID,ID,MA,NAME,ADDRESS,TEL,FAX,EMAIL,NGAYSINH,DIS,RATE,ID_NHOMKHACHHANG,MAX_CONGNO,SONGAY,MAHANG_KH_LK,LEVEL_PRICE,ID_KHUVUC,ISACTIVE,ISDEFAULT,CONGNODAUKY,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO,TENKHACHHANG,TENDONVI,DIACHI,MASOTHUE,CCCD")] v_v_dm_KhachHang dm_KhachHang)
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
				if (!Utility.KiemTraQuyen("Customer", "Create"))
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
					dm_KhachHang.LOC_ID = Utility.LOC_ID;
					dm_KhachHang.ID_NGUOITAO = base.Session["idUser"].ToString();
					dm_KhachHang.THOIGIANTHEM = Utility.CurrentTime;
					apiResponse = Utility.Create((dm_KhachHang)dm_KhachHang, "Customer");
					if (apiResponse.Success)
					{
						apiResponse.NewID = Guid.NewGuid().ToString();
						if (apiResponse.Data != null)
						{
							dm_KhachHang = JsonConvert.DeserializeObject<v_v_dm_KhachHang>(apiResponse.Data.ToString());
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
					apiResponse.Data = Utility.GetModelState(base.ModelState, "Customer");
				}
				apiResponse.ID = dm_KhachHang.ID;
				apiResponse.Detail = Utility.ConvertobjectToView((dm_KhachHang)dm_KhachHang, "dd/MM/yyyy");
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
				if (!Utility.KiemTraQuyen("Customer", "Edit"))
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
				v_v_dm_KhachHang v_v_dm_KhachHang2 = new v_v_dm_KhachHang();
				if (!string.IsNullOrEmpty(id))
				{
					apiResponse = Utility.GetDetail<v_v_dm_KhachHang>(Utility.LOC_ID + "/" + id, "Customer");
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
						v_v_dm_KhachHang2 = apiResponse.Data as v_v_dm_KhachHang;
					}
				}
				v_v_dm_KhachHang2.lstdm_KhuVuc = new List<v_dm_KhuVuc>();
				v_v_dm_KhachHang2.lstdm_KhuVuc = Utility.GetListData<v_dm_KhuVuc>("Area", "", "", Utility.LOC_ID).Data as List<v_dm_KhuVuc>;
				v_v_dm_KhachHang2.lstdm_NhomKhachHang = new List<v_dm_NhomKhachHang>();
				v_v_dm_KhachHang2.lstdm_NhomKhachHang = Utility.GetListData<v_dm_NhomKhachHang>("GroupCustomer", "", "", Utility.LOC_ID).Data as List<v_dm_NhomKhachHang>;
				apiResponse.Success = true;
				apiResponse.Detail = Utility.ConvertobjectTo((dm_KhachHang)v_v_dm_KhachHang2, "yyyy-MM-dd HH:mm:ss");
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
		public ActionResult EditPopup([Bind(Include = "LOC_ID,ID,MA,NAME,ADDRESS,TEL,FAX,EMAIL,NGAYSINH,DIS,RATE,ID_NHOMKHACHHANG,MAX_CONGNO,SONGAY,MAHANG_KH_LK,LEVEL_PRICE,ID_KHUVUC,ISACTIVE,ISDEFAULT,CONGNODAUKY,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO,TENKHACHHANG,TENDONVI,DIACHI,MASOTHUE,CCCD")] v_v_dm_KhachHang dm_KhachHang)
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
				if (!Utility.KiemTraQuyen("Customer", "Edit"))
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
					dm_KhachHang.LOC_ID = Utility.LOC_ID;
					dm_KhachHang.ID_NGUOISUA = base.Session["idUser"].ToString();
					dm_KhachHang.THOIGIANSUA = Utility.CurrentTime;
					apiResponse = Utility.Edit(Utility.LOC_ID + "/" + dm_KhachHang.MA, (v_dm_KhachHang)dm_KhachHang, "Customer");
					if (apiResponse.Success)
					{
						apiResponse.ID = dm_KhachHang.ID;
						if (apiResponse.Data != null)
						{
							dm_KhachHang = JsonConvert.DeserializeObject<v_v_dm_KhachHang>(apiResponse.Data.ToString());
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
					apiResponse.Data = Utility.GetModelState(base.ModelState, "Customer");
				}
				apiResponse.Detail = Utility.ConvertobjectToView((v_dm_KhachHang)dm_KhachHang, "dd/MM/yyyy");
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
				if (!Utility.KiemTraQuyen("Customer", "Delete"))
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
				apiResponse = Utility.Delete<v_dm_KhachHang>(Utility.LOC_ID + "/" + id, "Customer");
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
