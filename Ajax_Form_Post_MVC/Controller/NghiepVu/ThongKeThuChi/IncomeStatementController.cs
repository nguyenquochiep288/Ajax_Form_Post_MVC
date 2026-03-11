using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.Hosting;
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

namespace MVC_QuanLyTHP.Controllers
{

	public class IncomeStatementController : Controller
	{
		public ActionResult Index()
		{
			try
			{
				if (Utility.KiemTra())
				{
					return RedirectToAction("Index", "Admin");
				}
				ApiResponse apiResponse = new ApiResponse();
				List<Sp_Get_ThongKeThuChi_Result> iPagedList = new List<Sp_Get_ThongKeThuChi_Result>().OrderBy((Sp_Get_ThongKeThuChi_Result s) => s.NGAYLAP).ToList();
				v_v_ThongKeThuChi v_v_ThongKeThuChi2 = new v_v_ThongKeThuChi();
				v_v_ThongKeThuChi2.IPagedList = iPagedList;
				v_v_ThongKeThuChi2.TUNGAY = DateTime.Now;
				v_v_ThongKeThuChi2.DENNGAY = DateTime.Now;
				return View(v_v_ThongKeThuChi2);
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
		public ActionResult Index(SP_Parameter_Report sp_Parameter)
		{
			try
			{
				if (Utility.KiemTra())
				{
					return RedirectToAction("Index", "Admin");
				}
				List<Sp_Get_ThongKeThuChi_Result> iPagedList = new List<Sp_Get_ThongKeThuChi_Result>().OrderBy((Sp_Get_ThongKeThuChi_Result s) => s.NGAYLAP).ToList();
				if (base.ModelState.IsValid)
				{
					ApiResponse apiResponse = new ApiResponse();
					sp_Parameter.LOC_ID = Utility.LOC_ID;
					sp_Parameter.ID_KHACHHANG = sp_Parameter.ID_KHACHHANG ?? "";
					sp_Parameter.ID_NHOMKHACHHANG = sp_Parameter.ID_NHOMKHACHHANG ?? "";
					sp_Parameter.ID_KHUVUC = sp_Parameter.ID_KHUVUC ?? "";
					apiResponse = Utility.Get_ThongKeThuChi<Sp_Get_ThongKeThuChi_Result>(sp_Parameter);
					if (!apiResponse.Success)
					{
						base.TempData["TitleError"] = apiResponse.Message;
						return RedirectToAction("Index", "Notfound");
					}
					if (apiResponse.Data != null)
					{
						iPagedList = (apiResponse.Data as List<Sp_Get_ThongKeThuChi_Result>).OrderBy((Sp_Get_ThongKeThuChi_Result s) => s.NGAYLAP).ToList();
					}
				}
				v_v_ThongKeThuChi v_v_ThongKeThuChi2 = new v_v_ThongKeThuChi();
				v_v_ThongKeThuChi2.IPagedList = iPagedList;
				v_v_ThongKeThuChi2.TUNGAY = sp_Parameter.TUNGAY ?? DateTime.Now;
				v_v_ThongKeThuChi2.DENNGAY = sp_Parameter.DENNGAY ?? DateTime.Now;
				v_v_ThongKeThuChi2.HINHTHUC_THUCHI = sp_Parameter.HINHTHUC_THUCHI;
				return View(v_v_ThongKeThuChi2);
			}
			catch (Exception ex)
			{
				Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
				base.TempData["TitleError"] = "Đã có lỗi phát sinh trong phiên làm việc!";
				base.TempData["DetailError"] = ex.Message;
				return RedirectToAction("Index", "Notfound");
			}
		}

		public ActionResult LoadDetail(string ID)
		{
			ApiResponse apiResponse = new ApiResponse();
			try
			{
				if (Utility.KiemTra())
				{
					return RedirectToAction("Index", "Admin");
				}
				string value = clsMaHoa.Decrypt(ID.Replace(" ", "+"), "tmt6364");
				SP_Parameter_Report sP_Parameter_Report = JsonConvert.DeserializeObject<SP_Parameter_Report>(value);
				apiResponse = Utility.Get_ThongKeThuChi<Sp_Get_ThongKeThuChi_Result>(sP_Parameter_Report, "Sp_Get_ThongKeThuChi");
				List<Sp_Get_ThongKeThuChi_Result> source = apiResponse.Data as List<Sp_Get_ThongKeThuChi_Result>;
				source = source.OrderBy((Sp_Get_ThongKeThuChi_Result itm) => itm.NGAYLAP).ToList();
				DataTable dataSource = Utility.ToDataTable(source);
				ReportClass reportClass = new ReportClass();
				reportClass.FileName = HostingEnvironment.MapPath("~/Report/rptThongKeThuChiChiTiet.rpt");
				reportClass.Load();
				reportClass = Utility.GetFormulaFields(reportClass, sP_Parameter_Report);
				reportClass.SetDatabaseLogon("test", "test!", "test", "test");
				reportClass.SetDataSource(dataSource);
				base.Response.Buffer = false;
				base.Response.ClearContent();
				base.Response.ClearHeaders();
				Stream stream = reportClass.ExportToStream(ExportFormatType.PortableDocFormat);
				Utility.Report = reportClass;
				apiResponse = new ApiResponse();
				apiResponse.Success = true;
				apiResponse.NAME = Utility.GetTitleFrom("IncomeStatement");
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
