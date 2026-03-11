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

	public class CategoryPayrollController : Controller
	{
		public ActionResult Index(int Page = 1, string SearchString = "", string ShowSearchValue = "")
		{
			try
			{
				if (Utility.KiemTra())
				{
					return RedirectToAction("Index", "Admin");
				}
				if (!Utility.KiemTraQuyen("CategoryPayroll", "View"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				ShowSearchValue = Utility.GetShowSearchValue<dm_BangLuong>(ShowSearchValue);
				ApiResponse listData = Utility.GetListData<v_dm_BangLuong>("CategoryPayroll", ShowSearchValue, SearchString);
				if (!listData.Success)
				{
					base.TempData["TitleError"] = listData.Message;
					return RedirectToAction("Index", "Notfound");
				}
				IPagedList<v_dm_BangLuong> iPagedList = (listData.Data as List<v_dm_BangLuong>).ToPagedList(Page, Utility.GetPageSize());
				v_v_dm_BangLuong v_v_dm_BangLuong2 = new v_v_dm_BangLuong();
				v_v_dm_BangLuong2.IPagedList = iPagedList;
				base.ViewBag.searchValue = SearchString;
				base.ViewBag.showsearchValue = ShowSearchValue;
				base.ViewBag.PermissionEdit = Utility.KiemTraQuyen("CategoryPayroll", "Edit");
				base.ViewBag.PermissionDelete = Utility.KiemTraQuyen("CategoryPayroll", "Delete");
				base.ViewBag.PermissionCreate = Utility.KiemTraQuyen("CategoryPayroll", "Create");
				return View(v_v_dm_BangLuong2);
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
				if (!Utility.KiemTraQuyen("CategoryPayroll", "Create"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				v_v_dm_BangLuong v_v_dm_BangLuong2 = new v_v_dm_BangLuong();
				v_v_dm_BangLuong2.lstdm_BangLuong_ChiTiet = new List<v_dm_BangLuong_ChiTiet>();
				v_v_dm_BangLuong2.lstdm_PhongBan = new List<v_dm_PhongBan>();
				v_v_dm_BangLuong2.ID = Guid.NewGuid().ToString();
				return View(v_v_dm_BangLuong2);
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
		public ActionResult Create([Bind(Include = "LOC_ID,ID,MA,NAME,ISACTIVE,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO,ID_PHONGBAN")] v_v_dm_BangLuong dm_BangLuong)
		{
			try
			{
				if (Utility.KiemTra())
				{
					return RedirectToAction("Index", "Admin");
				}
				if (!Utility.KiemTraQuyen("CategoryPayroll", "Create"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				if (base.ModelState.IsValid)
				{
					ApiResponse apiResponse = Utility.Create((dm_BangLuong)dm_BangLuong, "CategoryPayroll");
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
				return View(dm_BangLuong);
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
				if (!Utility.KiemTraQuyen("CategoryPayroll", "Edit"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				v_v_dm_BangLuong v_v_dm_BangLuong2 = new v_v_dm_BangLuong();
				if (!string.IsNullOrEmpty(id))
				{
					apiResponse = Utility.GetDetail<v_v_dm_BangLuong>(id, "CategoryPayroll");
					if (!apiResponse.Success)
					{
						base.TempData["TitleError"] = apiResponse.Message;
						return RedirectToAction("Index", "Notfound");
					}
					if (apiResponse.Data != null)
					{
						v_v_dm_BangLuong2 = apiResponse.Data as v_v_dm_BangLuong;
					}
				}
				v_v_dm_BangLuong2.lstdm_PhongBan = new List<v_dm_PhongBan>();
				return View(v_v_dm_BangLuong2);
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
		public ActionResult Edit([Bind(Include = "LOC_ID,ID,MA,NAME,ISACTIVE,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO,ID_PHONGBAN")] v_v_dm_BangLuong dm_BangLuong)
		{
			try
			{
				if (Utility.KiemTra())
				{
					return RedirectToAction("Index", "Admin");
				}
				if (!Utility.KiemTraQuyen("CategoryPayroll", "Edit"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				if (base.ModelState.IsValid)
				{
					ApiResponse apiResponse = Utility.Edit(dm_BangLuong.MA, (v_dm_BangLuong)dm_BangLuong, "CategoryPayroll");
					if (apiResponse.Success)
					{
						return RedirectToAction("Index");
					}
					base.ModelState.AddModelError(string.Empty, apiResponse.Message);
				}
				return View(dm_BangLuong);
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
				if (!Utility.KiemTraQuyen("CategoryPayroll", "Delete"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				ApiResponse apiResponse = Utility.Delete<v_dm_BangLuong>(id, "CategoryPayroll");
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
				if (!Utility.KiemTraQuyen("CategoryPayroll", "Create"))
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
				v_v_dm_BangLuong v_v_dm_BangLuong2 = new v_v_dm_BangLuong();
				apiResponse.Success = true;
				v_v_dm_BangLuong2.ID = Guid.NewGuid().ToString();
				v_v_dm_BangLuong2.lstdm_BangLuong_ChiTiet = new List<v_dm_BangLuong_ChiTiet>();
				base.Session["lstdm_LuongThang_ChiTiet"] = v_v_dm_BangLuong2.lstdm_BangLuong_ChiTiet;
				v_v_dm_BangLuong2.lstdm_PhongBan = new List<v_dm_PhongBan>();
				v_v_dm_BangLuong2.lstdm_PhongBan = Utility.GetListData<v_dm_PhongBan>("Department", "", "", Utility.LOC_ID).Data as List<v_dm_PhongBan>;
				List<v_dm_LoaiLuong> lstLoaiLuong = Utility.GetListData<v_dm_LoaiLuong>("TypePayroll", "", "", Utility.LOC_ID).Data as List<v_dm_LoaiLuong>;
				List<ValueEdit> list = Utility.ConvertobjectTo(v_v_dm_BangLuong2);
				v_dm_BangLuong_ChiTiet v_dm_BangLuong_ChiTiet2 = new v_dm_BangLuong_ChiTiet();
				v_dm_BangLuong_ChiTiet2.ID = Guid.NewGuid().ToString();
				v_v_dm_BangLuong2.lstdm_BangLuong_ChiTiet.Add(v_dm_BangLuong_ChiTiet2);
				apiResponse.ProductCombo = Utility.GetCategoryPayroll(v_v_dm_BangLuong2.lstdm_BangLuong_ChiTiet, lstLoaiLuong);
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
		public ActionResult CreatePopup([Bind(Include = "LOC_ID,ID,MA,NAME,ISACTIVE,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO,ID_PHONGBAN")] v_v_dm_BangLuong dm_BangLuong)
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
				if (!Utility.KiemTraQuyen("CategoryPayroll", "Create"))
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
				IEnumerable<string> source = base.Request.Form.AllKeys.Where((string e) => e.StartsWith("ID_LOAILUONG|"));
				IEnumerable<string> source2 = base.Request.Form.AllKeys.Where((string e) => e.StartsWith("TYPE_LUONG|"));
				IEnumerable<string> source3 = base.Request.Form.AllKeys.Where((string e) => e.StartsWith("TYPE_QUYTACTINHLUONG|"));
				IEnumerable<string> enumerable = base.Request.Form.AllKeys.Where((string e) => e.StartsWith("txtSOTIEN|"));
				if (enumerable == null || enumerable.Count() == 0)
				{
					base.ModelState.AddModelError("lstdm_BangLuong_ChiTiet", "Thêm danh sách parameter.");
				}
				if (base.ModelState.IsValid)
				{
					dm_BangLuong.lstdm_BangLuong_ChiTiet = new List<v_dm_BangLuong_ChiTiet>();
					int num = 0;
					foreach (string item in enumerable)
					{
						string[] array = item.ToString().Split('|');
						string[] values = base.HttpContext.Request.Params.GetValues(item.ToString());
						string[] values2 = base.HttpContext.Request.Params.GetValues(source.ToList()[num].ToString());
						string[] values3 = base.HttpContext.Request.Params.GetValues(source2.ToList()[num].ToString());
						string[] values4 = base.HttpContext.Request.Params.GetValues(source3.ToList()[num].ToString());
						string value = clsMaHoa.Decrypt(array[1].ToString(), "tmt6364");
						v_dm_BangLuong_ChiTiet v_dm_BangLuong_ChiTiet2 = JsonConvert.DeserializeObject<v_dm_BangLuong_ChiTiet>(value);
						if (array != null)
						{
							if (string.IsNullOrEmpty(v_dm_BangLuong_ChiTiet2.ID))
							{
								v_dm_BangLuong_ChiTiet2.ID = Guid.NewGuid().ToString();
							}
							v_dm_BangLuong_ChiTiet2.LOC_ID = Utility.LOC_ID;
							v_dm_BangLuong_ChiTiet2.ID_BANGLUONG = dm_BangLuong.ID;
							v_dm_BangLuong_ChiTiet2.SOTIEN = Utility.ConvertStringToDouble(values[0]);
							v_dm_BangLuong_ChiTiet2.ID_LOAILUONG = values2[0];
							v_dm_BangLuong_ChiTiet2.TYPE_LUONG = Convert.ToInt32(Utility.ConvertStringToDouble(values3[0]));
							v_dm_BangLuong_ChiTiet2.TYPE_QUYTACTINHLUONG = Convert.ToInt32(Utility.ConvertStringToDouble(values4[0]));
							dm_BangLuong.lstdm_BangLuong_ChiTiet.Add(v_dm_BangLuong_ChiTiet2);
						}
						num++;
					}
					dm_BangLuong.LOC_ID = Utility.LOC_ID;
					dm_BangLuong.ID_NGUOITAO = base.Session["idUser"].ToString();
					dm_BangLuong.THOIGIANTHEM = Utility.CurrentTime;
					apiResponse = Utility.Create((v_dm_BangLuong)dm_BangLuong, "CategoryPayroll");
					if (apiResponse.Success)
					{
						apiResponse.NewID = Guid.NewGuid().ToString();
						if (apiResponse.Data != null && !string.IsNullOrEmpty(apiResponse.Data.ToString()))
						{
							dm_BangLuong = JsonConvert.DeserializeObject<v_v_dm_BangLuong>(apiResponse.Data.ToString());
						}
						dm_BangLuong.lstdm_BangLuong_ChiTiet = new List<v_dm_BangLuong_ChiTiet>();
						dm_BangLuong.lstdm_BangLuong_ChiTiet = dm_BangLuong.lstdm_BangLuong_ChiTiet;
						base.Session["lstdm_LuongThang_ChiTiet"] = dm_BangLuong.lstdm_BangLuong_ChiTiet;
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
					apiResponse.Data = Utility.GetModelState(base.ModelState, "CategoryPayroll");
				}
				dm_BangLuong.lstdm_PhongBan = new List<v_dm_PhongBan>();
				dm_BangLuong.lstdm_PhongBan = Utility.GetListData<v_dm_PhongBan>("Department", "", "", Utility.LOC_ID).Data as List<v_dm_PhongBan>;
				apiResponse.ID = dm_BangLuong.ID;
				apiResponse.Detail = Utility.ConvertobjectToView((v_dm_BangLuong)dm_BangLuong, "dd/MM/yyyy");
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
				if (!Utility.KiemTraQuyen("CategoryPayroll", "Edit"))
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
				v_v_dm_BangLuong v_v_dm_BangLuong2 = new v_v_dm_BangLuong();
				if (!string.IsNullOrEmpty(id))
				{
					apiResponse = Utility.GetDetail<v_v_dm_BangLuong>(id, "CategoryPayroll");
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
						v_v_dm_BangLuong2 = apiResponse.Data as v_v_dm_BangLuong;
					}
				}
				apiResponse.Success = true;
				v_v_dm_BangLuong2.lstdm_PhongBan = new List<v_dm_PhongBan>();
				v_v_dm_BangLuong2.lstdm_PhongBan = Utility.GetListData<v_dm_PhongBan>("Department", "", "", Utility.LOC_ID).Data as List<v_dm_PhongBan>;
				List<v_dm_LoaiLuong> lstLoaiLuong = Utility.GetListData<v_dm_LoaiLuong>("TypePayroll", "", "", Utility.LOC_ID).Data as List<v_dm_LoaiLuong>;
				List<ValueEdit> list = Utility.ConvertobjectTo(v_v_dm_BangLuong2);
				base.Session["lstdm_LuongThang_ChiTiet"] = v_v_dm_BangLuong2.lstdm_BangLuong_ChiTiet;
				apiResponse.ProductCombo = Utility.GetCategoryPayroll(v_v_dm_BangLuong2.lstdm_BangLuong_ChiTiet, lstLoaiLuong);
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
		[ValidateInput(false)]
		[ValidateAntiForgeryToken]
		public ActionResult EditPopup([Bind(Include = "LOC_ID,ID,MA,NAME,ISACTIVE,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO,ID_PHONGBAN")] v_v_dm_BangLuong dm_BangLuong)
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
				if (!Utility.KiemTraQuyen("CategoryPayroll", "Edit"))
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
				IEnumerable<string> source = base.Request.Form.AllKeys.Where((string e) => e.StartsWith("ID_LOAILUONG|"));
				IEnumerable<string> source2 = base.Request.Form.AllKeys.Where((string e) => e.StartsWith("TYPE_LUONG|"));
				IEnumerable<string> source3 = base.Request.Form.AllKeys.Where((string e) => e.StartsWith("TYPE_QUYTACTINHLUONG|"));
				IEnumerable<string> enumerable = base.Request.Form.AllKeys.Where((string e) => e.StartsWith("txtSOTIEN|"));
				if (enumerable == null || enumerable.Count() == 0)
				{
					base.ModelState.AddModelError("lstdm_BangLuong_ChiTiet", "Thêm danh sách.");
				}
				if (base.ModelState.IsValid)
				{
					dm_BangLuong.lstdm_BangLuong_ChiTiet = new List<v_dm_BangLuong_ChiTiet>();
					int num = 0;
					foreach (string item in enumerable)
					{
						string[] array = item.ToString().Split('|');
						string[] values = base.HttpContext.Request.Params.GetValues(item.ToString());
						string[] values2 = base.HttpContext.Request.Params.GetValues(source.ToList()[num].ToString());
						string[] values3 = base.HttpContext.Request.Params.GetValues(source2.ToList()[num].ToString());
						string[] values4 = base.HttpContext.Request.Params.GetValues(source3.ToList()[num].ToString());
						string value = clsMaHoa.Decrypt(array[1].ToString(), "tmt6364");
						v_dm_BangLuong_ChiTiet v_dm_BangLuong_ChiTiet2 = JsonConvert.DeserializeObject<v_dm_BangLuong_ChiTiet>(value);
						if (array != null)
						{
							if (string.IsNullOrEmpty(v_dm_BangLuong_ChiTiet2.ID))
							{
								v_dm_BangLuong_ChiTiet2.ID = Guid.NewGuid().ToString();
							}
							v_dm_BangLuong_ChiTiet2.LOC_ID = Utility.LOC_ID;
							v_dm_BangLuong_ChiTiet2.ID_BANGLUONG = dm_BangLuong.ID;
							v_dm_BangLuong_ChiTiet2.SOTIEN = Utility.ConvertStringToDouble(values[0]);
							v_dm_BangLuong_ChiTiet2.ID_LOAILUONG = values2[0];
							v_dm_BangLuong_ChiTiet2.TYPE_LUONG = Convert.ToInt32(Utility.ConvertStringToDouble(values3[0]));
							v_dm_BangLuong_ChiTiet2.TYPE_QUYTACTINHLUONG = Convert.ToInt32(Utility.ConvertStringToDouble(values4[0]));
							dm_BangLuong.lstdm_BangLuong_ChiTiet.Add(v_dm_BangLuong_ChiTiet2);
						}
						num++;
					}
					dm_BangLuong.ID_NGUOISUA = base.Session["idUser"].ToString();
					dm_BangLuong.THOIGIANSUA = Utility.CurrentTime;
					apiResponse = Utility.Edit(dm_BangLuong.MA, (v_dm_BangLuong)dm_BangLuong, "CategoryPayroll");
					if (apiResponse.Success)
					{
						apiResponse.ID = dm_BangLuong.ID;
						if (apiResponse.Data != null && !string.IsNullOrEmpty(apiResponse.Data.ToString()))
						{
							dm_BangLuong = JsonConvert.DeserializeObject<v_v_dm_BangLuong>(apiResponse.Data.ToString());
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
					apiResponse.Data = Utility.GetModelState(base.ModelState, "CategoryPayroll");
				}
				dm_BangLuong.lstdm_PhongBan = new List<v_dm_PhongBan>();
				dm_BangLuong.lstdm_PhongBan = Utility.GetListData<v_dm_PhongBan>("Department", "", "", Utility.LOC_ID).Data as List<v_dm_PhongBan>;
				List<ValueEdit> list = Utility.ConvertobjectToView(dm_BangLuong);
				List<v_dm_LoaiLuong> lstLoaiLuong = Utility.GetListData<v_dm_LoaiLuong>("TypePayroll", "", "", Utility.LOC_ID).Data as List<v_dm_LoaiLuong>;
				apiResponse.ProductCombo = Utility.GetCategoryPayroll(dm_BangLuong.lstdm_BangLuong_ChiTiet, lstLoaiLuong);
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
				if (!Utility.KiemTraQuyen("CategoryPayroll", "Delete"))
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
				apiResponse = Utility.Delete<v_dm_BangLuong>(id, "CategoryPayroll");
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

		[HttpPost]
		public ActionResult AddPayroll()
		{
			ApiResponse apiResponse = new ApiResponse();
			v_dm_BangLuong_ChiTiet v_dm_BangLuong_ChiTiet2 = new v_dm_BangLuong_ChiTiet();
			v_dm_BangLuong_ChiTiet2.ID = Guid.NewGuid().ToString();
			Utility.Lstdm_BangLuong_ChiTiet.Add(v_dm_BangLuong_ChiTiet2);
			List<v_dm_LoaiLuong> lstLoaiLuong = Utility.GetListData<v_dm_LoaiLuong>("TypePayroll", "", "", Utility.LOC_ID).Data as List<v_dm_LoaiLuong>;
			apiResponse.ProductCombo = Utility.GetCategoryPayroll(Utility.Lstdm_BangLuong_ChiTiet, lstLoaiLuong);
			apiResponse.Success = true;
			return new JsonResult
			{
				Data = apiResponse,
				JsonRequestBehavior = JsonRequestBehavior.AllowGet,
				MaxJsonLength = int.MaxValue
			};
		}

		[HttpPost]
		public ActionResult RemovePayroll(string ID)
		{
			ApiResponse apiResponse = new ApiResponse();
			v_v_dm_HangHoa v_v_dm_HangHoa2 = new v_v_dm_HangHoa();
			List<v_dm_BangLuong_ChiTiet> lstdm_BangLuong_ChiTiet = Utility.Lstdm_BangLuong_ChiTiet;
			v_dm_BangLuong_ChiTiet v_dm_BangLuong_ChiTiet2 = Utility.Lstdm_BangLuong_ChiTiet.Where((v_dm_BangLuong_ChiTiet e) => e.ID == ID).FirstOrDefault();
			if (v_dm_BangLuong_ChiTiet2 != null)
			{
				lstdm_BangLuong_ChiTiet.Remove(v_dm_BangLuong_ChiTiet2);
			}
			base.Session["lstdm_LuongThang_ChiTiet"] = lstdm_BangLuong_ChiTiet;
			List<v_dm_LoaiLuong> lstLoaiLuong = Utility.GetListData<v_dm_LoaiLuong>("TypePayroll", "", "", Utility.LOC_ID).Data as List<v_dm_LoaiLuong>;
			apiResponse.ProductCombo = Utility.GetCategoryPayroll(Utility.Lstdm_BangLuong_ChiTiet, lstLoaiLuong);
			apiResponse.Success = true;
			return new JsonResult
			{
				Data = apiResponse,
				JsonRequestBehavior = JsonRequestBehavior.AllowGet,
				MaxJsonLength = int.MaxValue
			};
		}
	}
}
