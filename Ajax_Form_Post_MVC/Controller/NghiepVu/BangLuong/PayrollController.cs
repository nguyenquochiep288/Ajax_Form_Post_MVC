using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using DatabaseTHP;
using DatabaseTHP.Class;
using DatabaseTHP.StoredProcedure;
using DatabaseTHP.StoredProcedure.Parameter;
using MVC_QuanLyTHP.Class;
using MVC_QuanLyTHP.Models;
using Newtonsoft.Json;
using PagedList;

namespace MVC_QuanLyTHP.Controllers
{

	public class PayrollController : Controller
	{
		public ActionResult Index(int Page = 1, DateTime? FromDate = null, DateTime? ToDate = null, string SearchString = "", string MAPHIEU = "", string IDCODE = "")
		{
			try
			{
				if (Utility.KiemTra())
				{
					return RedirectToAction("Index", "Admin");
				}
				if (!Utility.KiemTraQuyen("Payroll", "View"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				base.ViewBag.PermissionCreate = Utility.KiemTraQuyen("Payroll", "Create");
				ApiResponse apiResponse = new ApiResponse();
				IPagedList<v_nv_BangLuong> iPagedList = new List<v_nv_BangLuong>().ToList().ToPagedList(Page, Utility.GetPageSize());
				if ((FromDate.HasValue || !string.IsNullOrEmpty(IDCODE) || !string.IsNullOrEmpty(MAPHIEU)) && (FromDate.HasValue || !string.IsNullOrEmpty(IDCODE) || !string.IsNullOrEmpty(MAPHIEU)))
				{
					if (!string.IsNullOrEmpty(IDCODE) || !string.IsNullOrEmpty(MAPHIEU))
					{
						apiResponse = Utility.Get_DanhSachPhieuLuong<v_nv_BangLuong>(null, null, MAPHIEU, IDCODE);
					}
					if (FromDate.HasValue)
					{
						apiResponse = Utility.Get_DanhSachPhieuLuong<v_nv_BangLuong>(FromDate, ToDate, SearchString);
					}
					if (!apiResponse.Success)
					{
						apiResponse.Data = new List<v_dm_HangHoa>();
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
					if (!apiResponse.Success)
					{
						base.TempData["TitleError"] = apiResponse.Message;
						return RedirectToAction("Index", "Notfound");
					}
					List<v_nv_BangLuong> list = new List<v_nv_BangLuong>();
					list = ((!((!base.ViewBag.PermissionCreate) ? true : false)) ? (apiResponse.Data as List<v_nv_BangLuong>).ToList() : (apiResponse.Data as List<v_nv_BangLuong>).Where((v_nv_BangLuong s) => s.ID_NHANVIEN == base.Session["idUser"].ToString()).ToList());
					iPagedList = list.ToPagedList(Page, Utility.GetPageSize());
				}
				v_v_nv_BangLuong v_v_nv_BangLuong2 = new v_v_nv_BangLuong();
				v_v_nv_BangLuong2.IPagedList = iPagedList;
				v_v_nv_BangLuong2.lstdm_NhanVien = new List<ComboboxFrom>();
				base.ViewBag.searchValue = SearchString;
				base.ViewBag.PermissionEdit = Utility.KiemTraQuyen("Payroll", "Edit");
				base.ViewBag.PermissionDelete = Utility.KiemTraQuyen("Payroll", "Delete");
				base.ViewBag.fromdate = (FromDate.HasValue ? FromDate.Value.ToString("yyyy-MM-dd") : Utility.CurrentTime.AddMonths(-1).ToString("yyyy-MM-dd"));
				base.ViewBag.todate = (ToDate.HasValue ? ToDate.Value.ToString("yyyy-MM-dd") : Utility.CurrentTime.ToString("yyyy-MM-dd"));
				return View(v_v_nv_BangLuong2);
			}
			catch (Exception ex)
			{
				Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
				base.TempData["TitleError"] = "Đã có lỗi phát sinh trong phiên làm việc!";
				base.TempData["DetailError"] = ex.Message;
				return RedirectToAction("Index", "Notfound");
			}
		}

		public ActionResult Create(int type = 2, string myModalAdd = "myModalAdd")
		{
			try
			{
				base.Session["IntWidth"] = type;
				if (Utility.KiemTra())
				{
					return RedirectToAction("Index", "Admin");
				}
				if (!Utility.KiemTraQuyen("Payroll", "Create"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				v_v_nv_BangLuong v_v_nv_BangLuong2 = new v_v_nv_BangLuong();
				v_v_nv_BangLuong2.LOC_ID = Utility.LOC_ID;
				v_v_nv_BangLuong2.ID_NGUOITAO = base.Session["idUser"].ToString();
				v_v_nv_BangLuong2.THOIGIANTHEM = Utility.CurrentTime;
				v_v_nv_BangLuong2.NGAYLAP = Utility.CurrentTime;
				v_v_nv_BangLuong2.SOPHIEU = Utility.GetMaxID((nv_BangLuong)v_v_nv_BangLuong2, Utility.LOC_ID, v_v_nv_BangLuong2.NGAYLAP.ToString("yyyy-MM-dd"));
				v_v_nv_BangLuong2.MAPHIEU = API.GetMaPhieu("Payroll", v_v_nv_BangLuong2.NGAYLAP, v_v_nv_BangLuong2.SOPHIEU);
				v_v_nv_BangLuong2.ID = Guid.NewGuid().ToString();
				v_v_nv_BangLuong2.lstdm_ThangLuong = new List<ComboboxFrom>();
				v_v_nv_BangLuong2.lstdm_NhanVien = new List<ComboboxFrom>();
				base.ViewBag.myModalAdd = myModalAdd;
				return View(v_v_nv_BangLuong2);
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
		public ActionResult Create([Bind(Include = "LOC_ID,ID,ID_THANGLUONG,ID_NHANVIEN,SONGAYCONG,MUCLUONG,SONGAYLAMVIEC,SONGAYNGHIPHEP,TIENLUONG,TIENLUONGKHAC,TIENGIAM,TIENTHUCNHAN,GHICHU,NGAYLAP,ISTINHLUONG,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO,MAPHIEU,SOPHIEU,SONGAYNGHIKHONGPHEP")] v_nv_BangLuong nv_BangLuong)
		{
			try
			{
				if (Utility.KiemTra())
				{
					return RedirectToAction("Index", "Admin");
				}
				if (!Utility.KiemTraQuyen("Payroll", "Create"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				if (base.ModelState.IsValid)
				{
					nv_BangLuong.LOC_ID = Utility.LOC_ID;
					nv_BangLuong.ID_NGUOITAO = base.Session["idUser"].ToString();
					nv_BangLuong.THOIGIANTHEM = Utility.CurrentTime;
					ApiResponse apiResponse = Utility.Create((nv_BangLuong)nv_BangLuong, "Payroll");
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
				return View(nv_BangLuong);
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
				if (!Utility.KiemTraQuyen("Payroll", "Edit"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				v_v_nv_BangLuong v_v_nv_BangLuong2 = new v_v_nv_BangLuong();
				if (!string.IsNullOrEmpty(id))
				{
					apiResponse = Utility.GetDetail<v_nv_BangLuong>(Utility.LOC_ID + "/" + id, "Payroll");
					if (!apiResponse.Success)
					{
						base.TempData["TitleError"] = apiResponse.Message;
						return RedirectToAction("Index", "Notfound");
					}
					if (apiResponse.Data != null)
					{
						v_v_nv_BangLuong2 = apiResponse.Data as v_v_nv_BangLuong;
					}
				}
				v_v_nv_BangLuong2.lstdm_NhanVien = new List<ComboboxFrom>();
				v_v_nv_BangLuong2.lstdm_ThangLuong = new List<ComboboxFrom>();
				return View(v_v_nv_BangLuong2);
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
		public ActionResult Edit([Bind(Include = "LOC_ID,ID,ID_THANGLUONG,ID_NHANVIEN,SONGAYCONG,MUCLUONG,SONGAYLAMVIEC,SONGAYNGHIPHEP,TIENLUONG,TIENLUONGKHAC,TIENGIAM,TIENTHUCNHAN,GHICHU,NGAYLAP,ISTINHLUONG,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO,MAPHIEU,SOPHIEU,SONGAYNGHIKHONGPHEP")] v_nv_BangLuong nv_BangLuong)
		{
			try
			{
				if (Utility.KiemTra())
				{
					return RedirectToAction("Index", "Admin");
				}
				if (!Utility.KiemTraQuyen("Payroll", "Edit"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				if (base.ModelState.IsValid)
				{
					nv_BangLuong.LOC_ID = Utility.LOC_ID;
					nv_BangLuong.ID_NGUOISUA = base.Session["idUser"].ToString();
					nv_BangLuong.THOIGIANSUA = Utility.CurrentTime;
					ApiResponse apiResponse = Utility.Edit(Utility.LOC_ID + "/" + nv_BangLuong.ID, nv_BangLuong, "Payroll");
					if (apiResponse.Success)
					{
						return RedirectToAction("Index");
					}
					base.ModelState.AddModelError(string.Empty, apiResponse.Message);
				}
				return View(nv_BangLuong);
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
				if (!Utility.KiemTraQuyen("Payroll", "Delete"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				ApiResponse apiResponse = Utility.Delete<v_nv_BangLuong>(Utility.LOC_ID + "/" + id, "Payroll");
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
				if (!Utility.KiemTraQuyen("Payroll", "Create"))
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
				v_v_nv_BangLuong v_v_nv_BangLuong2 = new v_v_nv_BangLuong();
				apiResponse.Success = true;
				v_v_nv_BangLuong2.LOC_ID = Utility.LOC_ID;
				v_v_nv_BangLuong2.ID = Guid.NewGuid().ToString();
				v_v_nv_BangLuong2.NGAYLAP = Utility.CurrentTime;
				v_v_nv_BangLuong2.SOPHIEU = Utility.GetMaxID((nv_BangLuong)v_v_nv_BangLuong2, Utility.LOC_ID, v_v_nv_BangLuong2.NGAYLAP.ToString("yyyy-MM-dd"));
				v_v_nv_BangLuong2.MAPHIEU = API.GetMaPhieu("Payroll", v_v_nv_BangLuong2.NGAYLAP, v_v_nv_BangLuong2.SOPHIEU);
				v_v_nv_BangLuong2.lstdm_NhanVien = new List<ComboboxFrom>();
				v_v_nv_BangLuong2.lstdm_NhanVien = Utility.GetListData<ComboboxFrom>("Employee", "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
				v_v_nv_BangLuong2.lstdm_ThangLuong = new List<ComboboxFrom>();
				v_v_nv_BangLuong2.lstdm_ThangLuong = Utility.GetListData<ComboboxFrom>("MonthlySalary", "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
				List<ValueEdit> detail = Utility.ConvertobjectTo(v_v_nv_BangLuong2);
				base.Session["lstnv_BangLuong_ChiTiet"] = new List<nv_BangLuong_ChiTiet>();
				apiResponse.Detail = detail;
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

		private ApiResponse GetValue<T>(ApiResponse apiResponse, string NameController, string KeyCode)
		{
			apiResponse = Utility.GetListDataCode<T>(NameController, "MAPHIEU.ToUpper() == @0", KeyCode.ToUpper(), Utility.LOC_ID);
			if (!apiResponse.Success)
			{
				base.TempData["TitleError"] = apiResponse.Message;
				apiResponse.Success = false;
				apiResponse.URL = base.Url.Action("Index", "Notfound");
				return apiResponse;
			}
            if (apiResponse.Data != null && apiResponse.Data is List<T> list && list.Count > 0)
            {
                apiResponse.Detail = list.FirstOrDefault();
            }
            return apiResponse;
		}

		[HttpPost]
		[ValidateInput(false)]
		[ValidateAntiForgeryToken]
		public ActionResult CreatePopup([Bind(Include = "LOC_ID,ID,ID_THANGLUONG,ID_NHANVIEN,SONGAYCONG,MUCLUONG,SONGAYLAMVIEC,SONGAYNGHIPHEP,TIENLUONG,TIENLUONGKHAC,TIENGIAM,TIENTHUCNHAN,GHICHU,NGAYLAP,ISTINHLUONG,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO,MAPHIEU,SOPHIEU,SONGAYNGHIKHONGPHEP,BUTTONTYPE,TIENDAUKY")] v_v_nv_BangLuong nv_BangLuong)
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
				if (!Utility.KiemTraQuyen("Payroll", "Create"))
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
				nv_BangLuong.lstnv_BangLuong_ChiTiet = new List<nv_BangLuong_ChiTiet>();
				List<nv_BangLuong_ChiTiet> list = new List<nv_BangLuong_ChiTiet>();
				IEnumerable<string> enumerable = base.Request.Form.AllKeys.Where((string e) => e.StartsWith("txt"));
				if (enumerable == null || enumerable.Count() == 0)
				{
					base.ModelState.AddModelError("lstnv_BangLuong_ChiTiet", "Thêm danh sách hàng hóa.");
				}
				else
				{
					v_nv_BangLuong_ChiTiet v_nv_BangLuong_ChiTiet2 = new v_nv_BangLuong_ChiTiet();
					foreach (string item in enumerable)
					{
						string[] array = item.ToString().Split('|');
						string[] values = base.HttpContext.Request.Params.GetValues(item.ToString());
						string value = clsMaHoa.Decrypt(array[1].ToString(), "tmt6364");
						nv_BangLuong_ChiTiet nv_BangLuong_ChiTiet2 = JsonConvert.DeserializeObject<nv_BangLuong_ChiTiet>(value);
						if (v_nv_BangLuong_ChiTiet2.ID != nv_BangLuong_ChiTiet2.ID)
						{
							v_nv_BangLuong_ChiTiet2 = new v_nv_BangLuong_ChiTiet();
							v_nv_BangLuong_ChiTiet2 = JsonConvert.DeserializeObject<v_nv_BangLuong_ChiTiet>(value);
							v_nv_BangLuong_ChiTiet2.LOC_ID = Utility.LOC_ID;
							nv_BangLuong.lstnv_BangLuong_ChiTiet.Add(v_nv_BangLuong_ChiTiet2);
							list.Add(nv_BangLuong_ChiTiet2);
						}
						Utility.EditObject(v_nv_BangLuong_ChiTiet2, array[0].ToString().Substring(3, array[0].ToString().Length - 3), values[0]);
					}
				}
				if (base.ModelState.IsValid)
				{
					nv_BangLuong.NGAYLAP = nv_BangLuong.NGAYLAP.AddHours(Utility.CurrentTime.Hour).AddMinutes(Utility.CurrentTime.Minute);
					nv_BangLuong.LOC_ID = Utility.LOC_ID;
					nv_BangLuong.ID_NGUOITAO = base.Session["idUser"].ToString();
					nv_BangLuong.THOIGIANTHEM = Utility.CurrentTime;
					apiResponse = Utility.Create((v_nv_BangLuong)nv_BangLuong, "Payroll");
					if (apiResponse.Success)
					{
						nv_BangLuong.NGAYLAP = Utility.CurrentTime;
						ApiResponse apiResponse2 = apiResponse;
						int sOPHIEU = (nv_BangLuong.SOPHIEU = Utility.GetMaxID((nv_BangLuong)nv_BangLuong, Utility.LOC_ID, nv_BangLuong.NGAYLAP.ToString("yyyy-MM-dd")));
						apiResponse2.SOPHIEU = sOPHIEU;
						nv_BangLuong.MAPHIEU = API.GetMaPhieu("Payroll", nv_BangLuong.NGAYLAP, nv_BangLuong.SOPHIEU);
						apiResponse.NewID = Guid.NewGuid().ToString();
						apiResponse.MAPHIEU = nv_BangLuong.MAPHIEU;
						if (apiResponse.Data != null)
						{
							nv_BangLuong = JsonConvert.DeserializeObject<v_v_nv_BangLuong>(apiResponse.Data.ToString());
						}
						list = new List<nv_BangLuong_ChiTiet>();
					}
					else
					{
						base.ModelState.AddModelError(string.Empty, apiResponse.Message);
						if (apiResponse.CheckValue)
						{
							nv_BangLuong.NGAYLAP = Utility.CurrentTime;
							ApiResponse apiResponse3 = apiResponse;
							int sOPHIEU = (nv_BangLuong.SOPHIEU = Utility.GetMaxID((nv_BangLuong)nv_BangLuong, Utility.LOC_ID, nv_BangLuong.NGAYLAP.ToString("yyyy-MM-dd")));
							apiResponse3.SOPHIEU = sOPHIEU;
							nv_BangLuong.MAPHIEU = API.GetMaPhieu("Payroll", nv_BangLuong.NGAYLAP, nv_BangLuong.SOPHIEU);
							apiResponse.NewID = Guid.NewGuid().ToString();
							apiResponse.MAPHIEU = nv_BangLuong.MAPHIEU;
						}
					}
				}
				else
				{
					apiResponse.Success = false;
					apiResponse.Data = Utility.GetModelState(base.ModelState, "Payroll");
				}
				base.Session["lstnv_BangLuong_ChiTiet"] = list;
				apiResponse.ID = nv_BangLuong.ID;
				nv_BangLuong.lstdm_NhanVien = new List<ComboboxFrom>();
				nv_BangLuong.lstdm_NhanVien = Utility.GetListData<ComboboxFrom>("Employee", "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
				nv_BangLuong.lstdm_ThangLuong = new List<ComboboxFrom>();
				nv_BangLuong.lstdm_ThangLuong = Utility.GetListData<ComboboxFrom>("MonthlySalary", "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
				List<ValueEdit> list2 = Utility.ConvertobjectToView(nv_BangLuong);
				List<v_dm_LoaiLuong> lstLoaiLuong = Utility.GetListData<v_dm_LoaiLuong>("TypePayroll", "", "", Utility.LOC_ID).Data as List<v_dm_LoaiLuong>;
				apiResponse.ProductCombo = Utility.GetPayrollDetail(list, lstLoaiLuong);
				list2.Add(new ValueEdit
				{
					Key = "tbodyReport_Add",
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
				if (!Utility.KiemTraQuyen("Payroll", "Edit"))
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
				v_v_nv_BangLuong v_v_nv_BangLuong2 = new v_v_nv_BangLuong();
				if (!string.IsNullOrEmpty(id))
				{
					apiResponse = Utility.GetDetail<v_v_nv_BangLuong>(Utility.LOC_ID + "/" + id, "Payroll");
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
						v_v_nv_BangLuong2 = apiResponse.Data as v_v_nv_BangLuong;
					}
				}
				v_v_nv_BangLuong2.lstdm_NhanVien = new List<ComboboxFrom>();
				v_v_nv_BangLuong2.lstdm_NhanVien = Utility.GetListData<ComboboxFrom>("Employee", "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
				v_v_nv_BangLuong2.lstdm_ThangLuong = new List<ComboboxFrom>();
				v_v_nv_BangLuong2.lstdm_ThangLuong = Utility.GetListData<ComboboxFrom>("MonthlySalary", "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
				List<ValueEdit> list = Utility.ConvertobjectTo(v_v_nv_BangLuong2);
				List<v_dm_LoaiLuong> lstLoaiLuong = Utility.GetListData<v_dm_LoaiLuong>("TypePayroll", "", "", Utility.LOC_ID).Data as List<v_dm_LoaiLuong>;
				apiResponse.ProductCombo = Utility.GetPayrollDetail(v_v_nv_BangLuong2.lstnv_BangLuong_ChiTiet, lstLoaiLuong);
				list.Add(new ValueEdit
				{
					Key = "tbodyReport_Edit",
					Value = apiResponse.ProductCombo
				});
				base.Session["lstnv_BangLuong_ChiTiet"] = v_v_nv_BangLuong2.lstnv_BangLuong_ChiTiet;
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
		public ActionResult EditPopup([Bind(Include = "LOC_ID,ID,ID_THANGLUONG,ID_NHANVIEN,SONGAYCONG,MUCLUONG,SONGAYLAMVIEC,SONGAYNGHIPHEP,TIENLUONG,TIENLUONGKHAC,TIENGIAM,TIENTHUCNHAN,GHICHU,NGAYLAP,ISTINHLUONG,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO,MAPHIEU,SOPHIEU,SONGAYNGHIKHONGPHEP,BUTTONTYPE,TIENDAUKY")] v_v_nv_BangLuong nv_BangLuong)
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
				if (!Utility.KiemTraQuyen("Payroll", "Edit"))
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
				nv_BangLuong.lstnv_BangLuong_ChiTiet = new List<nv_BangLuong_ChiTiet>();
				List<nv_BangLuong_ChiTiet> list = new List<nv_BangLuong_ChiTiet>();
				IEnumerable<string> enumerable = base.Request.Form.AllKeys.Where((string e) => e.StartsWith("txt"));
				if (enumerable == null || enumerable.Count() == 0)
				{
					base.ModelState.AddModelError("lstnv_BangLuong_ChiTiet", "Thêm danh sách hàng hóa.");
				}
				else
				{
					v_nv_BangLuong_ChiTiet v_nv_BangLuong_ChiTiet2 = new v_nv_BangLuong_ChiTiet();
					foreach (string item in enumerable)
					{
						string[] array = item.ToString().Split('|');
						string[] values = base.HttpContext.Request.Params.GetValues(item.ToString());
						string value = clsMaHoa.Decrypt(array[1].ToString(), "tmt6364");
						nv_BangLuong_ChiTiet nv_BangLuong_ChiTiet2 = JsonConvert.DeserializeObject<nv_BangLuong_ChiTiet>(value);
						if (v_nv_BangLuong_ChiTiet2.ID != nv_BangLuong_ChiTiet2.ID)
						{
							v_nv_BangLuong_ChiTiet2 = new v_nv_BangLuong_ChiTiet();
							v_nv_BangLuong_ChiTiet2 = JsonConvert.DeserializeObject<v_nv_BangLuong_ChiTiet>(value);
							v_nv_BangLuong_ChiTiet2.LOC_ID = Utility.LOC_ID;
							nv_BangLuong.lstnv_BangLuong_ChiTiet.Add(v_nv_BangLuong_ChiTiet2);
							list.Add(nv_BangLuong_ChiTiet2);
						}
						Utility.EditObject(v_nv_BangLuong_ChiTiet2, array[0].ToString().Substring(3, array[0].ToString().Length - 3), values[0]);
					}
				}
				if (base.ModelState.IsValid)
				{
					nv_BangLuong.LOC_ID = Utility.LOC_ID;
					nv_BangLuong.ID_NGUOISUA = base.Session["idUser"].ToString();
					nv_BangLuong.THOIGIANSUA = Utility.CurrentTime;
					apiResponse = Utility.Edit(Utility.LOC_ID + "/" + nv_BangLuong.ID, (v_nv_BangLuong)nv_BangLuong, "Payroll");
					if (apiResponse.Success)
					{
						apiResponse.ID = nv_BangLuong.ID;
						if (apiResponse.Data != null)
						{
							nv_BangLuong = JsonConvert.DeserializeObject<v_v_nv_BangLuong>(apiResponse.Data.ToString());
						}
						list = new List<nv_BangLuong_ChiTiet>();
					}
					else
					{
						base.ModelState.AddModelError(string.Empty, apiResponse.Message);
					}
				}
				else
				{
					apiResponse.Success = false;
					apiResponse.Data = Utility.GetModelState(base.ModelState, "Payroll");
				}
				base.Session["lstnv_BangLuong_ChiTiet"] = list;
				nv_BangLuong.lstdm_NhanVien = new List<ComboboxFrom>();
				nv_BangLuong.lstdm_NhanVien = Utility.GetListData<ComboboxFrom>("Employee", "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
				nv_BangLuong.lstdm_ThangLuong = new List<ComboboxFrom>();
				nv_BangLuong.lstdm_ThangLuong = Utility.GetListData<ComboboxFrom>("MonthlySalary", "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
				List<ValueEdit> detail = Utility.ConvertobjectToView(nv_BangLuong);
				apiResponse.Detail = detail;
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
				if (!Utility.KiemTraQuyen("Payroll", "Delete"))
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
				apiResponse = Utility.Delete<v_nv_BangLuong>(Utility.LOC_ID + "/" + id, "Payroll");
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
		public ActionResult CallChangePayroll(string id, string type)
		{
			if (Utility.KiemTra())
			{
				return RedirectToAction("Index", "Admin");
			}
			if (base.ModelState.IsValid)
			{
				Return obj = new Return();
				ApiResponse apiResponse = new ApiResponse();
				switch (type)
				{
					case "dm_ThangLuong":
						apiResponse = Utility.GetDetail<v_dm_ThangLuong>(Utility.LOC_ID + "/" + id, "MonthlySalary");
						break;
					case "dm_NhanVien":
						apiResponse = Utility.GetDetail<v_dm_NhanVien>(Utility.LOC_ID + "/" + id, "Employee");
						break;
					case "NGAYLAP":
						apiResponse.Success = true;
						break;
				}
				if (!apiResponse.Success)
				{
					base.TempData["TitleError"] = apiResponse.Message;
					obj.URL = base.Url.Action("Index", "Notfound");
				}
				else
				{
					switch (type)
					{
						case "dm_ThangLuong":
							{
								v_dm_ThangLuong dataObject2 = apiResponse.Data as v_dm_ThangLuong;
								obj.DataObject = dataObject2;
								break;
							}
						case "dm_NhanVien":
							{
								v_dm_NhanVien dataObject = apiResponse.Data as v_dm_NhanVien;
								obj.DataObject = dataObject;
								break;
							}
						case "NGAYLAP":
							{
								nv_BangLuong nv_BangLuong2 = new nv_BangLuong();
								nv_BangLuong2.NGAYLAP = Convert.ToDateTime(id);
								nv_BangLuong2.SOPHIEU = Utility.GetMaxID(nv_BangLuong2, Utility.LOC_ID, nv_BangLuong2.NGAYLAP.ToString("yyyy-MM-dd"));
								nv_BangLuong2.MAPHIEU = API.GetMaPhieu("Payroll", nv_BangLuong2.NGAYLAP, nv_BangLuong2.SOPHIEU);
								obj.DataObject = nv_BangLuong2;
								break;
							}
					}
				}
				obj.DATA = type;
				return Json(obj, JsonRequestBehavior.AllowGet);
			}
			Return obj2 = new Return();
			obj2.DATA = "";
			return Json(obj2, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		public ActionResult GetPayrollDetail(string ID_THANGLUONG, string ID_NHANVIEN, string ID)
		{
			ApiResponse data = new ApiResponse();
			if (Utility.KiemTra())
			{
				return RedirectToAction("Index", "Admin");
			}
			if (base.ModelState.IsValid)
			{
				v_nv_BangLuong v_nv_BangLuong2 = new v_nv_BangLuong();
				v_nv_BangLuong2.LOC_ID = Utility.LOC_ID;
				v_nv_BangLuong2.ID_NHANVIEN = ID_NHANVIEN;
				v_nv_BangLuong2.ID_THANGLUONG = ID_THANGLUONG;
				v_nv_BangLuong2.ID = ID;
				data = Utility.Create(v_nv_BangLuong2, "Payroll/" + Utility.LOC_ID);
				v_nv_BangLuong2 = JsonConvert.DeserializeObject<v_v_nv_BangLuong>(data.Data.ToString());
				if (data.Success)
				{
					v_v_dm_ThangLuong v_v_dm_ThangLuong2 = new v_v_dm_ThangLuong();
					double num = 0.0;
					data = Utility.GetDetail<v_v_dm_ThangLuong>(Utility.LOC_ID + "/" + ID_THANGLUONG, "MonthlySalary");
					if (!data.Success)
					{
						base.TempData["TitleError"] = data.Message;
						return RedirectToAction("Index", "Notfound");
					}
					if (data.Data != null)
					{
						v_v_dm_ThangLuong2 = data.Data as v_v_dm_ThangLuong;
					}
					SP_Parameter sP_Parameter = new SP_Parameter();
					sP_Parameter.LOC_ID = Utility.LOC_ID;
					sP_Parameter.ID_NHANVIEN = ID_NHANVIEN;
					sP_Parameter.ISTHEOTHOIGIAN = true;
					sP_Parameter.TUNGAY = v_v_dm_ThangLuong2.NGAYBATDAU;
					sP_Parameter.DENNGAY = v_v_dm_ThangLuong2.NGAYKETTHUC;
					sP_Parameter.ISPHATSINHCONGNO = false;
					sP_Parameter.ISPHATSINHCONGNOTRONGKY = false;
					sP_Parameter.ISCONCONGNO = false;
					data = Utility.Get_ThongKeCongNoNhanVien<v_ThongKeCongNoNhanVien>(sP_Parameter);
					if (!data.Success)
					{
						base.TempData["TitleError"] = data.Message;
						return RedirectToAction("Index", "Notfound");
					}
					if (data.Data != null)
					{
						v_ThongKeCongNoNhanVien v_ThongKeCongNoNhanVien2 = (data.Data as List<v_ThongKeCongNoNhanVien>).FirstOrDefault();
						if (v_ThongKeCongNoNhanVien2 != null)
						{
							num = v_ThongKeCongNoNhanVien2.TONGTIENCONGNOCUOIKY;
						}
					}
					if (v_nv_BangLuong2.lstnv_BangLuong_ChiTiet != null && v_nv_BangLuong2.lstnv_BangLuong_ChiTiet.Count > 0)
					{
						List<v_dm_LoaiLuong> lstLoaiLuong = Utility.GetListData<v_dm_LoaiLuong>("TypePayroll", "", "", Utility.LOC_ID).Data as List<v_dm_LoaiLuong>;
						data.ProductCombo = Utility.GetPayrollDetail(v_nv_BangLuong2.lstnv_BangLuong_ChiTiet, lstLoaiLuong);
						List<ValueEdit> list = new List<ValueEdit>();
						list.Add(new ValueEdit
						{
							Key = "SONGAYCONG",
							Value = v_nv_BangLuong2.SONGAYCONG
						});
						list.Add(new ValueEdit
						{
							Key = "SONGAYLAMVIEC",
							Value = v_nv_BangLuong2.SONGAYLAMVIEC
						});
						list.Add(new ValueEdit
						{
							Key = "SONGAYNGHIPHEP",
							Value = v_nv_BangLuong2.SONGAYNGHIPHEP
						});
						list.Add(new ValueEdit
						{
							Key = "SONGAYNGHIKHONGPHEP",
							Value = v_nv_BangLuong2.SONGAYNGHIKHONGPHEP
						});
						list.Add(new ValueEdit
						{
							Key = "TIENLUONG",
							Value = v_nv_BangLuong2.TIENLUONG
						});
						list.Add(new ValueEdit
						{
							Key = "TIENGIAM",
							Value = v_nv_BangLuong2.TIENGIAM
						});
						list.Add(new ValueEdit
						{
							Key = "TIENTHUCNHAN",
							Value = v_nv_BangLuong2.TIENTHUCNHAN
						});
						list.Add(new ValueEdit
						{
							Key = "GHICHU",
							Value = v_nv_BangLuong2.GHICHU
						});
						list.Add(new ValueEdit
						{
							Key = "TIENDAUKY",
							Value = num
						});
						base.Session["lstnv_BangLuong_ChiTiet"] = v_nv_BangLuong2.lstnv_BangLuong_ChiTiet;
						data.Detail = list;
					}
				}
				return new JsonResult
				{
					Data = data,
					JsonRequestBehavior = JsonRequestBehavior.AllowGet,
					MaxJsonLength = int.MaxValue
				};
			}
			return new JsonResult
			{
				Data = data,
				JsonRequestBehavior = JsonRequestBehavior.AllowGet,
				MaxJsonLength = int.MaxValue
			};
		}

		[HttpPost]
		public ActionResult AddPayroll()
		{
			ApiResponse apiResponse = new ApiResponse();
			v_nv_BangLuong_ChiTiet v_nv_BangLuong_ChiTiet2 = new v_nv_BangLuong_ChiTiet();
			v_nv_BangLuong_ChiTiet2.ID = Guid.NewGuid().ToString();
			Utility.Lstnv_BangLuong_ChiTiet.Add(v_nv_BangLuong_ChiTiet2);
			List<v_dm_LoaiLuong> lstLoaiLuong = Utility.GetListData<v_dm_LoaiLuong>("TypePayroll", "", "", Utility.LOC_ID).Data as List<v_dm_LoaiLuong>;
			apiResponse.ProductCombo = Utility.GetPayrollDetail(Utility.Lstnv_BangLuong_ChiTiet, lstLoaiLuong);
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
			List<nv_BangLuong_ChiTiet> lstnv_BangLuong_ChiTiet = Utility.Lstnv_BangLuong_ChiTiet;
			nv_BangLuong_ChiTiet nv_BangLuong_ChiTiet2 = Utility.Lstnv_BangLuong_ChiTiet.Where((nv_BangLuong_ChiTiet e) => e.ID == ID).FirstOrDefault();
			if (nv_BangLuong_ChiTiet2 != null)
			{
				lstnv_BangLuong_ChiTiet.Remove(nv_BangLuong_ChiTiet2);
			}
			base.Session["lstnv_BangLuong_ChiTiet"] = lstnv_BangLuong_ChiTiet;
			List<v_dm_LoaiLuong> lstLoaiLuong = Utility.GetListData<v_dm_LoaiLuong>("TypePayroll", "", "", Utility.LOC_ID).Data as List<v_dm_LoaiLuong>;
			apiResponse.ProductCombo = Utility.GetPayrollDetail(Utility.Lstnv_BangLuong_ChiTiet, lstLoaiLuong);
			apiResponse.Success = true;
			return new JsonResult
			{
				Data = apiResponse,
				JsonRequestBehavior = JsonRequestBehavior.AllowGet,
				MaxJsonLength = int.MaxValue
			};
		}

		public ActionResult ViewReport(string ID)
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
				view_nv_BangLuong_ChiTiet view_nv_BangLuong_ChiTiet2 = new view_nv_BangLuong_ChiTiet();
				apiResponse = Utility.Create<view_nv_BangLuong>(null, "Payroll/" + Utility.LOC_ID + "/" + ID);
				if (!apiResponse.Success)
				{
					apiResponse.Success = false;
					apiResponse.Message = apiResponse.Message;
					return new JsonResult
					{
						Data = apiResponse,
						JsonRequestBehavior = JsonRequestBehavior.AllowGet,
						MaxJsonLength = int.MaxValue
					};
				}
				if (apiResponse.Data != null)
				{
					view_nv_BangLuong_ChiTiet2 = JsonConvert.DeserializeObject<List<view_nv_BangLuong_ChiTiet>>(apiResponse.Data.ToString()).FirstOrDefault();
				}
				ReportClass reportClass = new ReportClass();
				reportClass.FileName = base.Server.MapPath("~/Report/rptPhieuLuong.rpt");
				List<view_nv_BangLuong_ChiTiet> list = new List<view_nv_BangLuong_ChiTiet>();
				list = (from s in JsonConvert.DeserializeObject<List<view_nv_BangLuong_ChiTiet>>(apiResponse.Data.ToString())
						orderby s.TYPE
						select s).ToList();
				if (list == null)
				{
					list = new List<view_nv_BangLuong_ChiTiet>();
				}
				List<view_nv_BangLuong_ChiTiet> list2 = (from itm in list
														 orderby itm.TYPE, itm.SOTIEN descending
														 select itm).ToList();
				DataTable dataTable = Utility.ToDataTable(list2);
				if (apiResponse.CheckValue)
				{
					dataTable.Rows.Clear();
				}
				reportClass = Utility.GetFormulaFields(reportClass, view_nv_BangLuong_ChiTiet2);
				reportClass.SetDataSource(dataTable);
				base.Response.Buffer = false;
				base.Response.ClearContent();
				base.Response.ClearHeaders();
				Stream stream = reportClass.ExportToStream(ExportFormatType.PortableDocFormat);
				Utility.Report = reportClass;
				apiResponse = new ApiResponse();
				apiResponse.Success = true;
				apiResponse.NAME = Utility.GetTitleFrom("Payroll") + " - " + view_nv_BangLuong_ChiTiet2.MAPHIEU;
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
