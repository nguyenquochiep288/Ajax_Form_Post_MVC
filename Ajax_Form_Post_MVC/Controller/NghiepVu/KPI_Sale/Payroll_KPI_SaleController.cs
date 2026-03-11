using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using DatabaseTHP;
using DatabaseTHP.Class;
using DatabaseTHP.StoredProcedure.Parameter;
using MVC_QuanLyTHP.Class;
using MVC_QuanLyTHP.Models;
using Newtonsoft.Json;
using PagedList;

namespace MVC_QuanLyTHP.Controllers
{

	public class Payroll_KPI_SaleController : Controller
	{
		public ActionResult Index()
		{
			try
			{
				if (Utility.KiemTra())
				{
					return RedirectToAction("Index", "Admin");
				}
				if (!Utility.KiemTraQuyen("Payment", "View"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				ApiResponse apiResponse = new ApiResponse();
				IPagedList<v_Tinh_KPI_KinhDoanh> iPagedList = new List<v_Tinh_KPI_KinhDoanh>().ToList().ToPagedList(1, Utility.GetPageSize());
				v_v_Tinh_KPI_KinhDoanh v_v_Tinh_KPI_KinhDoanh2 = new v_v_Tinh_KPI_KinhDoanh();
				v_v_Tinh_KPI_KinhDoanh2.IPagedList = iPagedList;
				v_v_Tinh_KPI_KinhDoanh2.lstdm_NhanVien = new List<v_dm_NhanVien>();
				v_v_Tinh_KPI_KinhDoanh2.lstdm_NhanVien = Utility.GetListData<v_dm_NhanVien>("Employee", "", "", Utility.LOC_ID).Data as List<v_dm_NhanVien>;
				v_v_Tinh_KPI_KinhDoanh2.lstweb_NhomQuyen = new List<v_web_NhomQuyen>();
				v_v_Tinh_KPI_KinhDoanh2.lstweb_NhomQuyen = Utility.GetListData<v_web_NhomQuyen>("GroupPermissions", "", "", Utility.LOC_ID).Data as List<v_web_NhomQuyen>;
				v_v_Tinh_KPI_KinhDoanh2.TUNGAY = DateTime.Now;
				v_v_Tinh_KPI_KinhDoanh2.DENNGAY = DateTime.Now;
				base.ViewBag.PermissionEdit = Utility.KiemTraQuyen("Payment", "Edit");
				base.ViewBag.PermissionDelete = Utility.KiemTraQuyen("Payment", "Delete");
				base.ViewBag.PermissionCreate = Utility.KiemTraQuyen("Payment", "Create");
				return View(v_v_Tinh_KPI_KinhDoanh2);
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
		public ActionResult Index(SP_Parameter objParameter)
		{
			try
			{
				if (Utility.KiemTra())
				{
					return RedirectToAction("Index", "Admin");
				}
				if (!Utility.KiemTraQuyen("Payment", "View"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				ApiResponse apiResponse = new ApiResponse();
				IPagedList<v_Tinh_KPI_KinhDoanh> pagedList = new List<v_Tinh_KPI_KinhDoanh>().ToList().ToPagedList(1, Utility.GetPageSize());
				objParameter.LOC_ID = Utility.LOC_ID;
				apiResponse = Utility.Edit("", objParameter, "KPI_Sale");
				if (!apiResponse.Success)
				{
					base.TempData["TitleError"] = apiResponse.Message;
					return RedirectToAction("Index", "Notfound");
				}
				List<v_Tinh_KPI_KinhDoanh> list = JsonConvert.DeserializeObject<List<v_Tinh_KPI_KinhDoanh>>(apiResponse.Data.ToString());
				pagedList = list.ToPagedList(1, (list.Count() > 0) ? list.Count() : 50);
				v_v_Tinh_KPI_KinhDoanh v_v_Tinh_KPI_KinhDoanh2 = new v_v_Tinh_KPI_KinhDoanh();
				v_v_Tinh_KPI_KinhDoanh2.IPagedList = pagedList;
				v_v_Tinh_KPI_KinhDoanh2.lstdm_NhanVien = new List<v_dm_NhanVien>();
				v_v_Tinh_KPI_KinhDoanh2.lstdm_NhanVien = Utility.GetListData<v_dm_NhanVien>("Employee", "", "", Utility.LOC_ID).Data as List<v_dm_NhanVien>;
				v_v_Tinh_KPI_KinhDoanh2.lstweb_NhomQuyen = new List<v_web_NhomQuyen>();
				v_v_Tinh_KPI_KinhDoanh2.lstweb_NhomQuyen = Utility.GetListData<v_web_NhomQuyen>("GroupPermissions", "", "", Utility.LOC_ID).Data as List<v_web_NhomQuyen>;
				v_v_Tinh_KPI_KinhDoanh2.TUNGAY = DateTime.Now;
				v_v_Tinh_KPI_KinhDoanh2.DENNGAY = DateTime.Now;
				base.ViewBag.PermissionEdit = Utility.KiemTraQuyen("Payment", "Edit");
				base.ViewBag.PermissionDelete = Utility.KiemTraQuyen("Payment", "Delete");
				base.ViewBag.PermissionCreate = Utility.KiemTraQuyen("Payment", "Create");
				return View(v_v_Tinh_KPI_KinhDoanh2);
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
