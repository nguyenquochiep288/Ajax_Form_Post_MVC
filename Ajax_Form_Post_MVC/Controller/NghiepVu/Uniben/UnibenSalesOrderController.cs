using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using DatabaseTHP;
using DatabaseTHP.Class;
using DatabaseTHP.Class.Uniben;
using MVC_QuanLyTHP.Class;
using MVC_QuanLyTHP.Models;
using Newtonsoft.Json;
using PagedList;

namespace MVC_QuanLyTHP.Controllers.NghiepVu.Uniben
{

	public class UnibenSalesOrderController : Controller
	{
		public ActionResult Index(int Page = 1, DateTime? FromDate = null, DateTime? ToDate = null, string SearchString = "")
		{
			try
			{
				if (Utility.KiemTra())
				{
					return RedirectToAction("Index", "Admin");
				}
				ApiResponse apiResponse = new ApiResponse();
				v_v_UnibenOrderData v_v_UnibenOrderData2 = new v_v_UnibenOrderData();
				string text = "";
				string text2 = "";
				string text3 = "";
				IPagedList<DatabaseTHP.Class.Uniben.Uniben.UnibenOrderData> pagedList = new List<DatabaseTHP.Class.Uniben.Uniben.UnibenOrderData>().ToList().ToPagedList(Page, Utility.GetPageSize());
				if (FromDate.HasValue)
				{
					apiResponse = Utility.GetListData<DatabaseTHP.Class.Uniben.Uniben.UnibenOrderData>("Uniben", "", "", Utility.LOC_ID + "/" + (ToDate.HasValue ? ToDate.Value.ToString("yyyy-MM-dd") : DateTime.Now.ToString("yyyy-MM-dd")) + "/" + (FromDate.HasValue ? FromDate.Value.ToString("yyyy-MM-dd") : DateTime.Now.ToString("yyyy-MM-dd")) + "/" + (string.IsNullOrEmpty(SearchString) ? "%" : SearchString));
					if (!apiResponse.Success)
					{
						base.TempData["TitleError"] = apiResponse.Message;
						return RedirectToAction("Index", "Notfound");
					}
					pagedList = (apiResponse.Data as List<DatabaseTHP.Class.Uniben.Uniben.UnibenOrderData>).ToPagedList(Page, Utility.GetPageSize());
					v_v_UnibenOrderData2.IPagedList = pagedList;
					text = (apiResponse.Data as List<DatabaseTHP.Class.Uniben.Uniben.UnibenOrderData>).Sum((DatabaseTHP.Class.Uniben.Uniben.UnibenOrderData s) => s.totalAmount).ToString("N0").Replace(".", ",");
					text2 = (apiResponse.Data as List<DatabaseTHP.Class.Uniben.Uniben.UnibenOrderData>).Where((DatabaseTHP.Class.Uniben.Uniben.UnibenOrderData s) => string.IsNullOrEmpty(s.MAPHIEUDATHANG)).Count().ToString("N0").Replace(".", ",");
					text3 = (apiResponse.Data as List<DatabaseTHP.Class.Uniben.Uniben.UnibenOrderData>).Where((DatabaseTHP.Class.Uniben.Uniben.UnibenOrderData s) => !string.IsNullOrEmpty(s.MAPHIEUDATHANG)).Count().ToString("N0").Replace(".", ",");
				}
				string text4 = "";
				if (!string.IsNullOrEmpty(text3) || !string.IsNullOrEmpty(text2))
				{
					text4 = "     <a class=\"label label-success\" href=\"#\"><i class=\"fa fa-check-square-o\" style=\"margin-right:5px\"></i>Đã đồng bộ " + ((!string.IsNullOrEmpty(text3)) ? (" (" + text3 + ")") : "") + "</a>";
					text4 = text4 + "     <a class=\"label label-warning\" href=\"#\"><i class=\"fa fa-square-o\" style=\"margin-right:5px\"></i>Chưa đồng bộ " + ((!string.IsNullOrEmpty(text2)) ? (" (" + text2 + ")") : "") + "</a>";
				}
				base.ViewBag.searchValue = SearchString;
				base.ViewBag.TotalSum = text + text4;
				base.ViewBag.PermissionEdit = Utility.KiemTraQuyen("UnibenSalesOrder", "Edit");
				base.ViewBag.PermissionDelete = Utility.KiemTraQuyen("UnibenSalesOrder", "Delete");
				base.ViewBag.PermissionCreate = Utility.KiemTraQuyen("UnibenSalesOrder", "Create");
				base.ViewBag.fromdate = (FromDate.HasValue ? FromDate.Value.ToString("yyyy-MM-dd") : Utility.CurrentTime.ToString("yyyy-MM-dd"));
				base.ViewBag.todate = (ToDate.HasValue ? ToDate.Value.ToString("yyyy-MM-dd") : Utility.CurrentTime.ToString("yyyy-MM-dd"));
				return View(v_v_UnibenOrderData2);
			}
			catch (Exception ex)
			{
				Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
				base.TempData["TitleError"] = "Đã có lỗi phát sinh trong phiên làm việc!";
				base.TempData["DetailError"] = ex.Message;
				return RedirectToAction("Index", "Notfound");
			}
		}

		public ActionResult LinkUniben(string Type = "Customer", string TypeGet = "", string SearchString = "")
		{
			try
			{
				if (Utility.KiemTra())
				{
					return RedirectToAction("Index", "Admin");
				}
				ApiResponse apiResponse = new ApiResponse();
				v_v_UnibenOrderListResponse v_v_UnibenOrderListResponse2 = new v_v_UnibenOrderListResponse();
				v_v_UnibenOrderListResponse2.lstdm_HangHoa = new List<ComboboxFrom>();
				v_v_UnibenOrderListResponse2.lstdm_KhachHang = new List<ComboboxFrom>();
				v_v_UnibenOrderListResponse2.lstAspNetUsers = new List<ComboboxFrom>();
				apiResponse = ((!(TypeGet == "GetUniben")) ? Utility.GetListData<v_v_UnibenOrderListResponse>("Uniben", "", "", Utility.LOC_ID + "/" + Type + "/" + (string.IsNullOrEmpty(SearchString) ? "%" : SearchString)) : Utility.GetListData<v_v_UnibenOrderListResponse>("Uniben", "", "", Utility.LOC_ID + "/" + Type));
				if (!apiResponse.Success)
				{
					base.TempData["TitleError"] = apiResponse.Message;
					return RedirectToAction("Index", "Notfound");
				}
				v_v_UnibenOrderListResponse2 = (apiResponse.Data as List<v_v_UnibenOrderListResponse>)[0];
				switch (Type)
				{
					case "Product":
						v_v_UnibenOrderListResponse2.lstdm_HangHoa = Utility.GetListData<ComboboxFrom>("Product", "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
						break;
					case "Customer":
						v_v_UnibenOrderListResponse2.lstdm_KhachHang = Utility.GetListData<ComboboxFrom>("Customer", "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
						break;
					case "Employee":
						v_v_UnibenOrderListResponse2.lstAspNetUsers = Utility.GetListData<ComboboxFrom>("User").Data as List<ComboboxFrom>;
						break;
				}
				base.ViewBag.searchValue = SearchString;
				base.ViewBag.type = Type;
				return View(v_v_UnibenOrderListResponse2);
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
		public ActionResult LoadData(string cartOrder)
		{
			try
			{
				if (Utility.KiemTra())
				{
					return RedirectToAction("Index", "Admin");
				}
				if (base.ModelState.IsValid)
				{
					List<Deposit> model = new JavaScriptSerializer().Deserialize<List<Deposit>>(cartOrder);
					v_v_UnibenOrderListResponse v_v_UnibenOrderListResponse2 = new v_v_UnibenOrderListResponse();
					ApiResponse apiResponse = Utility.Create(model, "Uniben/" + Utility.LOC_ID);
					if (apiResponse.Success)
					{
						v_v_UnibenOrderListResponse2 = JsonConvert.DeserializeObject<v_v_UnibenOrderListResponse>(apiResponse.Data.ToString());
						v_v_UnibenOrderListResponse2.lstdm_KhachHang = Utility.GetListData<ComboboxFrom>("Customer", "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
						v_v_UnibenOrderListResponse2.lstdm_HangHoa = Utility.GetListData<ComboboxFrom>("Product", "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
						v_v_UnibenOrderListResponse2.lstAspNetUsers = Utility.GetListData<ComboboxFrom>("User").Data as List<ComboboxFrom>;
						if (v_v_UnibenOrderListResponse2 != null)
						{
							return View("LoadData", v_v_UnibenOrderListResponse2);
						}
						base.ModelState.AddModelError(string.Empty, apiResponse.Message);
					}
					else
					{
						base.ModelState.AddModelError(string.Empty, apiResponse.Message);
					}
					return View("LoadData", v_v_UnibenOrderListResponse2);
				}
				base.TempData["TitleError"] = "Đã có lỗi phát sinh trong phiên làm việc!";
				return RedirectToAction("Index", "Notfound");
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
		public ActionResult CreateCustomer()
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
				IEnumerable<string> enumerable = base.Request.Form.AllKeys.Where((string e) => e.StartsWith("Hidden"));
				IEnumerable<string> enumerable2 = base.Request.Form.AllKeys.Where((string e) => e.StartsWith("lstdm_KhachHang"));
				if (enumerable2 == null || enumerable2.Count() == 0 || enumerable == null || enumerable.Count() == 0)
				{
					base.ModelState.AddModelError("lstct_PhieuDatHang_ChiTiet", "Thêm danh sách.");
				}
				else
				{
					foreach (string item in enumerable)
					{
						string[] values = base.HttpContext.Request.Params.GetValues(item.ToString());
						string value = clsMaHoa.Decrypt(values[0], "tmt6364");
						if (string.IsNullOrEmpty(value))
						{
							continue;
						}
						List<v_v_uniben_dm_LienKet_KhachHang> list = JsonConvert.DeserializeObject<List<v_v_uniben_dm_LienKet_KhachHang>>(value);
						foreach (string item2 in enumerable2)
						{
							string[] lstString = item2.ToString().Split('|');
							if (lstString.Length == 2)
							{
								v_v_uniben_dm_LienKet_KhachHang v_v_uniben_dm_LienKet_KhachHang2 = list.Where((v_v_uniben_dm_LienKet_KhachHang s) => s.ID_UNIBEN == lstString[1].ToString()).FirstOrDefault();
								string[] values2 = base.HttpContext.Request.Params.GetValues(item2.ToString());
								if (v_v_uniben_dm_LienKet_KhachHang2 != null && values2 != null && values2.Length != 0)
								{
									v_v_uniben_dm_LienKet_KhachHang2.ID_KHACHHANG = values2[0].ToString();
								}
							}
						}
						apiResponse = Utility.Create(list, "Uniben/Customer/" + Utility.LOC_ID);
					}
				}
				if (!base.ModelState.IsValid)
				{
					apiResponse.Success = false;
					apiResponse.Data = Utility.GetModelState(base.ModelState, "Deposit");
				}
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
				return RedirectToAction("Index", "Notfound");
			}
		}

		[HttpPost]
		[ValidateInput(false)]
		[ValidateAntiForgeryToken]
		public ActionResult CreateProduct()
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
				IEnumerable<string> enumerable = base.Request.Form.AllKeys.Where((string e) => e.StartsWith("Hidden"));
				IEnumerable<string> enumerable2 = base.Request.Form.AllKeys.Where((string e) => e.StartsWith("lstdm_HangHoa"));
				if (enumerable2 == null || enumerable2.Count() == 0 || enumerable == null || enumerable.Count() == 0)
				{
					base.ModelState.AddModelError("lstct_PhieuDatHang_ChiTiet", "Thêm danh sách.");
				}
				else
				{
					foreach (string item in enumerable)
					{
						string[] values = base.HttpContext.Request.Params.GetValues(item.ToString());
						string value = clsMaHoa.Decrypt(values[0], "tmt6364");
						if (string.IsNullOrEmpty(value))
						{
							continue;
						}
						List<v_v_uniben_dm_LienKet_HangHoa> list = JsonConvert.DeserializeObject<List<v_v_uniben_dm_LienKet_HangHoa>>(value);
						foreach (string item2 in enumerable2)
						{
							string[] lstString = item2.ToString().Split('|');
							if (lstString.Length == 3)
							{
								v_v_uniben_dm_LienKet_HangHoa v_v_uniben_dm_LienKet_HangHoa2 = list.Where((v_v_uniben_dm_LienKet_HangHoa s) => s.ID_UNIBEN == lstString[1].ToString() && s.ISKHUYENMAI == Convert.ToBoolean(lstString[2])).FirstOrDefault();
								string[] values2 = base.HttpContext.Request.Params.GetValues(item2.ToString());
								if (v_v_uniben_dm_LienKet_HangHoa2 != null && values2 != null && values2.Length != 0)
								{
									v_v_uniben_dm_LienKet_HangHoa2.ID_HANGHOA = values2[0].ToString();
								}
							}
						}
						apiResponse = Utility.Create(list, "Uniben/Product/" + Utility.LOC_ID);
					}
				}
				if (!base.ModelState.IsValid)
				{
					apiResponse.Success = false;
					apiResponse.Data = Utility.GetModelState(base.ModelState, "Deposit");
				}
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
				return RedirectToAction("Index", "Notfound");
			}
		}

		[HttpPost]
		[ValidateInput(false)]
		[ValidateAntiForgeryToken]
		public ActionResult CreateEmployee()
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
				IEnumerable<string> enumerable = base.Request.Form.AllKeys.Where((string e) => e.StartsWith("Hidden"));
				IEnumerable<string> enumerable2 = base.Request.Form.AllKeys.Where((string e) => e.StartsWith("lstAspNetUsers"));
				if (enumerable2 == null || enumerable2.Count() == 0 || enumerable == null || enumerable.Count() == 0)
				{
					base.ModelState.AddModelError("lstct_PhieuDatHang_ChiTiet", "Thêm danh sách.");
				}
				else
				{
					foreach (string item in enumerable)
					{
						string[] values = base.HttpContext.Request.Params.GetValues(item.ToString());
						string value = clsMaHoa.Decrypt(values[0], "tmt6364");
						if (string.IsNullOrEmpty(value))
						{
							continue;
						}
						List<v_v_uniben_dm_LienKet_NhanVien> list = JsonConvert.DeserializeObject<List<v_v_uniben_dm_LienKet_NhanVien>>(value);
						foreach (string item2 in enumerable2)
						{
							string[] lstString = item2.ToString().Split('|');
							if (lstString.Length == 2)
							{
								v_v_uniben_dm_LienKet_NhanVien v_v_uniben_dm_LienKet_NhanVien2 = list.Where((v_v_uniben_dm_LienKet_NhanVien s) => s.ID_UNIBEN == lstString[1].ToString()).FirstOrDefault();
								string[] values2 = base.HttpContext.Request.Params.GetValues(item2.ToString());
								if (v_v_uniben_dm_LienKet_NhanVien2 != null && values2 != null && values2.Length != 0)
								{
									v_v_uniben_dm_LienKet_NhanVien2.ID_NHANVIEN = values2[0].ToString();
								}
							}
						}
						apiResponse = Utility.Create(list, "Uniben/Employee/" + Utility.LOC_ID);
					}
				}
				if (!base.ModelState.IsValid)
				{
					apiResponse.Success = false;
					apiResponse.Data = Utility.GetModelState(base.ModelState, "Deposit");
				}
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
				return RedirectToAction("Index", "Notfound");
			}
		}

		[HttpPost]
		[ValidateInput(false)]
		[ValidateAntiForgeryToken]
		public ActionResult CreateUnibenSalesOrder()
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
				IEnumerable<string> enumerable = base.Request.Form.AllKeys.Where((string e) => e.StartsWith("chk|"));
				if (enumerable == null || enumerable.Count() == 0)
				{
					base.ModelState.AddModelError("lstct_PhieuDatHang_ChiTiet", "Thêm danh sách.");
				}
				else
				{
					List<Deposit> list = new List<Deposit>();
					foreach (string item in enumerable)
					{
						string[] array = item.ToString().Split('|');
						string[] values = base.HttpContext.Request.Params.GetValues(item.ToString());
						if (array.Length == 2 && values != null && Convert.ToBoolean(values[0]))
						{
							Deposit deposit = new Deposit();
							deposit.ID_NGUOITAO = base.Session["idUser"].ToString();
							deposit.LOC_ID = Utility.LOC_ID;
							deposit.ID = array[1].ToString();
							list.Add(deposit);
						}
					}
					if (list.Count > 0)
					{
						apiResponse = Utility.Create(list, "Uniben/UnibenSalesOrder/" + Utility.LOC_ID);
					}
				}
				if (!base.ModelState.IsValid)
				{
					apiResponse.Success = false;
					apiResponse.Data = Utility.GetModelState(base.ModelState, "Deposit");
				}
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
				return RedirectToAction("Index", "Notfound");
			}
		}
	}
}
