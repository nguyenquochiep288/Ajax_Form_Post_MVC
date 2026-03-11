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

namespace MVC_QuanLyTHP.Controllers
{

	public class ReportEmployeeController : Controller
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
				List<Sp_Get_BaoCaoTheoNhanVien_Result> iPagedList = new List<Sp_Get_BaoCaoTheoNhanVien_Result>().OrderByDescending((Sp_Get_BaoCaoTheoNhanVien_Result s) => s.TYPE).ToList();
				v_v_ThongKeTheoNhanVien v_v_ThongKeTheoNhanVien2 = new v_v_ThongKeTheoNhanVien();
				v_v_ThongKeTheoNhanVien2.IPagedList = iPagedList;
				v_v_ThongKeTheoNhanVien2.lstdm_NhanVien = new List<v_AspNetUsers>();
				v_v_ThongKeTheoNhanVien2.lstdm_NhanVien = Utility.GetListData<v_AspNetUsers>("User").Data as List<v_AspNetUsers>;
				v_v_ThongKeTheoNhanVien2.ISTHEOTHOIGIAN = true;
				v_v_ThongKeTheoNhanVien2.TUNGAY = DateTime.Now;
				v_v_ThongKeTheoNhanVien2.DENNGAY = DateTime.Now;
				return View(v_v_ThongKeTheoNhanVien2);
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
		public ActionResult Index(SP_Parameter sp_Parameter)
		{
			try
			{
				if (Utility.KiemTra())
				{
					return RedirectToAction("Index", "Admin");
				}
				List<Sp_Get_BaoCaoTheoNhanVien_Result> iPagedList = new List<Sp_Get_BaoCaoTheoNhanVien_Result>().OrderByDescending((Sp_Get_BaoCaoTheoNhanVien_Result s) => s.TYPE).ToList();
				if (base.ModelState.IsValid)
				{
					ApiResponse apiResponse = new ApiResponse();
					sp_Parameter.LOC_ID = Utility.LOC_ID;
					sp_Parameter.ID_NHANVIEN = sp_Parameter.ID_NHANVIEN ?? "";
					sp_Parameter.ISTHEOTHOIGIAN = sp_Parameter.ISTHEOTHOIGIAN == true;
					iPagedList = new List<Sp_Get_BaoCaoTheoNhanVien_Result>().OrderByDescending((Sp_Get_BaoCaoTheoNhanVien_Result s) => s.TYPE).ToList();
					apiResponse = Utility.Get_ThongKeBaoCaoNhanVien<Sp_Get_BaoCaoTheoNhanVien_Result>(sp_Parameter);
					if (!apiResponse.Success)
					{
						base.TempData["TitleError"] = apiResponse.Message;
						return RedirectToAction("Index", "Notfound");
					}
					if (apiResponse.Data != null)
					{
						iPagedList = (apiResponse.Data as List<Sp_Get_BaoCaoTheoNhanVien_Result>).OrderByDescending((Sp_Get_BaoCaoTheoNhanVien_Result s) => s.ID_NGUOITAO).ToList();
					}
				}
				v_v_ThongKeTheoNhanVien v_v_ThongKeTheoNhanVien2 = new v_v_ThongKeTheoNhanVien();
				v_v_ThongKeTheoNhanVien2.IPagedList = iPagedList;
				v_v_ThongKeTheoNhanVien2.lstdm_NhanVien = new List<v_AspNetUsers>();
				v_v_ThongKeTheoNhanVien2.lstdm_NhanVien = Utility.GetListData<v_AspNetUsers>("User").Data as List<v_AspNetUsers>;
				v_v_ThongKeTheoNhanVien2.ISTHEOTHOIGIAN = sp_Parameter.ISTHEOTHOIGIAN == true;
				v_v_ThongKeTheoNhanVien2.ID_NHANVIEN = sp_Parameter.ID_TAIKHOANNGANHANG;
				v_v_ThongKeTheoNhanVien2.TUNGAY = sp_Parameter.TUNGAY ?? DateTime.Now;
				v_v_ThongKeTheoNhanVien2.DENNGAY = sp_Parameter.DENNGAY ?? DateTime.Now;
				return View(v_v_ThongKeTheoNhanVien2);
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
				Sp_Get_BaoCaoTheoNhanVien_Result sp_Get_BaoCaoTheoNhanVien_Result = JsonConvert.DeserializeObject<Sp_Get_BaoCaoTheoNhanVien_Result>(value);
				apiResponse = Utility.GetReportEmployeeDetail<v_ThongKeCongNo_ChiTiet>(sp_Get_BaoCaoTheoNhanVien_Result, "ReportEmployee");
				List<v_ThongKeCongNo_ChiTiet> source = apiResponse.Data as List<v_ThongKeCongNo_ChiTiet>;
				source = (from itm in source
						  orderby itm.NGAY, itm.LOAIPHIEU
						  select itm).ToList();
				DataTable dataSource = Utility.ToDataTable(source);
				ReportClass report = new ReportClass();
				report = Utility.GetFormulaFields(report, sp_Get_BaoCaoTheoNhanVien_Result);
				report.SetDatabaseLogon("test", "test!", "test", "test");
				report.SetDataSource(dataSource);
				base.Response.Buffer = false;
				base.Response.ClearContent();
				base.Response.ClearHeaders();
				Stream stream = report.ExportToStream(ExportFormatType.PortableDocFormat);
				Utility.Report = report;
				apiResponse = new ApiResponse();
				apiResponse.Success = true;
				apiResponse.NAME = Utility.GetTitleFrom("MoneyFund");
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
