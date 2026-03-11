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
using DatabaseTHP.StoredProcedure.Parameter;
using MVC_QuanLyTHP.Class;
using MVC_QuanLyTHP.Models;
using Newtonsoft.Json;

namespace MVC_QuanLyTHP.Controllers
{

	public class DebtEmployeeController : Controller
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
				List<v_ThongKeCongNoNhanVien> iPagedList = new List<v_ThongKeCongNoNhanVien>().OrderByDescending((v_ThongKeCongNoNhanVien s) => s.NAME).ToList();
				v_v_ThongKeCongNoNhanVien v_v_ThongKeCongNoNhanVien2 = new v_v_ThongKeCongNoNhanVien();
				v_v_ThongKeCongNoNhanVien2.IPagedList = iPagedList;
				v_v_ThongKeCongNoNhanVien2.lstdm_PhongBan = new List<v_dm_PhongBan>();
				v_v_ThongKeCongNoNhanVien2.lstdm_PhongBan = Utility.GetListData<v_dm_PhongBan>("Department", "", "", Utility.LOC_ID).Data as List<v_dm_PhongBan>;
				v_v_ThongKeCongNoNhanVien2.lstdm_NhanVien = new List<v_dm_NhanVien>();
				v_v_ThongKeCongNoNhanVien2.lstdm_NhanVien = Utility.GetListData<v_dm_NhanVien>("Employee", "", "", Utility.LOC_ID).Data as List<v_dm_NhanVien>;
				v_v_ThongKeCongNoNhanVien2.ISTHEOTHOIGIAN = true;
				v_v_ThongKeCongNoNhanVien2.ISPHATSINHCONGNO = true;
				v_v_ThongKeCongNoNhanVien2.ISPHATSINHCONGNOTRONGKY = false;
				v_v_ThongKeCongNoNhanVien2.ISCONCONGNO = false;
				v_v_ThongKeCongNoNhanVien2.TUNGAY = DateTime.Now;
				v_v_ThongKeCongNoNhanVien2.DENNGAY = DateTime.Now;
				return View(v_v_ThongKeCongNoNhanVien2);
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
				List<v_ThongKeCongNoNhanVien> iPagedList = new List<v_ThongKeCongNoNhanVien>().OrderByDescending((v_ThongKeCongNoNhanVien s) => s.NAME).ToList();
				if (base.ModelState.IsValid)
				{
					ApiResponse apiResponse = new ApiResponse();
					sp_Parameter.LOC_ID = Utility.LOC_ID;
					sp_Parameter.ID_NHANVIEN = sp_Parameter.ID_NHANVIEN ?? "";
					sp_Parameter.ID_PHONGBAN = sp_Parameter.ID_PHONGBAN ?? "";
					sp_Parameter.ISTHEOTHOIGIAN = sp_Parameter.ISTHEOTHOIGIAN == true;
					sp_Parameter.ISPHATSINHCONGNO = sp_Parameter.ISPHATSINHCONGNO == true;
					sp_Parameter.ISPHATSINHCONGNOTRONGKY = sp_Parameter.ISPHATSINHCONGNOTRONGKY == true;
					sp_Parameter.ISCONCONGNO = sp_Parameter.ISCONCONGNO == true;
					iPagedList = new List<v_ThongKeCongNoNhanVien>().OrderByDescending((v_ThongKeCongNoNhanVien s) => s.NAME).ToList();
					apiResponse = Utility.Get_ThongKeCongNoNhanVien<v_ThongKeCongNoNhanVien>(sp_Parameter);
					if (!apiResponse.Success)
					{
						base.TempData["TitleError"] = apiResponse.Message;
						return RedirectToAction("Index", "Notfound");
					}
					if (apiResponse.Data != null)
					{
						iPagedList = (apiResponse.Data as List<v_ThongKeCongNoNhanVien>).OrderByDescending((v_ThongKeCongNoNhanVien s) => s.NAME).ToList();
					}
				}
				v_v_ThongKeCongNoNhanVien v_v_ThongKeCongNoNhanVien2 = new v_v_ThongKeCongNoNhanVien();
				v_v_ThongKeCongNoNhanVien2.IPagedList = iPagedList;
				v_v_ThongKeCongNoNhanVien2.lstdm_PhongBan = new List<v_dm_PhongBan>();
				v_v_ThongKeCongNoNhanVien2.lstdm_PhongBan = Utility.GetListData<v_dm_PhongBan>("Department", "", "", Utility.LOC_ID).Data as List<v_dm_PhongBan>;
				v_v_ThongKeCongNoNhanVien2.lstdm_NhanVien = new List<v_dm_NhanVien>();
				v_v_ThongKeCongNoNhanVien2.lstdm_NhanVien = Utility.GetListData<v_dm_NhanVien>("Employee", "", "", Utility.LOC_ID).Data as List<v_dm_NhanVien>;
				v_v_ThongKeCongNoNhanVien2.ISTHEOTHOIGIAN = sp_Parameter.ISTHEOTHOIGIAN == true;
				v_v_ThongKeCongNoNhanVien2.ID_PHONGBAN = sp_Parameter.ID_PHONGBAN;
				v_v_ThongKeCongNoNhanVien2.ID_NHANVIEN = sp_Parameter.ID_NHANVIEN;
				v_v_ThongKeCongNoNhanVien2.ISPHATSINHCONGNO = sp_Parameter.ISPHATSINHCONGNO == true;
				v_v_ThongKeCongNoNhanVien2.ISPHATSINHCONGNOTRONGKY = sp_Parameter.ISPHATSINHCONGNOTRONGKY == true;
				v_v_ThongKeCongNoNhanVien2.ISCONCONGNO = sp_Parameter.ISCONCONGNO == true;
				v_v_ThongKeCongNoNhanVien2.TUNGAY = sp_Parameter.TUNGAY ?? DateTime.Now;
				v_v_ThongKeCongNoNhanVien2.DENNGAY = sp_Parameter.DENNGAY ?? DateTime.Now;
				return View(v_v_ThongKeCongNoNhanVien2);
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
				v_ThongKeCongNoNhanVien v_ThongKeCongNoNhanVien2 = JsonConvert.DeserializeObject<v_ThongKeCongNoNhanVien>(value);
				v_ThongKeCongNoNhanVien2.ID_CHUCVU = "1";
				v_ThongKeCongNoNhanVien2.GIOITINH = "1";
				apiResponse = Utility.DebtEmployeeDetail<v_ThongKeCongNo_ChiTiet>(v_ThongKeCongNoNhanVien2, "DebtEmployee");
				List<v_ThongKeCongNo_ChiTiet> source = apiResponse.Data as List<v_ThongKeCongNo_ChiTiet>;
				source = (from itm in source
						  orderby itm.NGAY, itm.LOAIPHIEU
						  select itm).ToList();
				DataTable dataSource = Utility.ToDataTable(source);
				ReportClass report = new ReportClass();
				report = Utility.GetFormulaFields(report, v_ThongKeCongNoNhanVien2);
				report.SetDatabaseLogon("test", "test!", "test", "test");
				report.SetDataSource(dataSource);
				base.Response.Buffer = false;
				base.Response.ClearContent();
				base.Response.ClearHeaders();
				Stream stream = report.ExportToStream(ExportFormatType.PortableDocFormat);
				Utility.Report = report;
				apiResponse = new ApiResponse();
				apiResponse.Success = true;
				apiResponse.NAME = Utility.GetTitleFrom("DebtProvider");
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
