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

	public class DebtProviderController : Controller
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
				List<v_ThongKeCongNoNhaCungCap> iPagedList = new List<v_ThongKeCongNoNhaCungCap>().OrderByDescending((v_ThongKeCongNoNhaCungCap s) => s.NAME).ToList();
				v_v_ThongKeCongNoNhaCungCap v_v_ThongKeCongNoNhaCungCap2 = new v_v_ThongKeCongNoNhaCungCap();
				v_v_ThongKeCongNoNhaCungCap2.IPagedList = iPagedList;
				v_v_ThongKeCongNoNhaCungCap2.lstdm_NhomNhaCungCap = new List<v_dm_NhomNhaCungCap>();
				v_v_ThongKeCongNoNhaCungCap2.lstdm_NhomNhaCungCap = Utility.GetListData<v_dm_NhomNhaCungCap>("GroupProvider", "", "", Utility.LOC_ID).Data as List<v_dm_NhomNhaCungCap>;
				v_v_ThongKeCongNoNhaCungCap2.lstdm_NhaCungCap = new List<v_dm_NhaCungCap>();
				v_v_ThongKeCongNoNhaCungCap2.lstdm_NhaCungCap = Utility.GetListData<v_dm_NhaCungCap>("Provider", "", "", Utility.LOC_ID).Data as List<v_dm_NhaCungCap>;
				v_v_ThongKeCongNoNhaCungCap2.ISTHEOTHOIGIAN = true;
				v_v_ThongKeCongNoNhaCungCap2.ISPHATSINHCONGNO = true;
				v_v_ThongKeCongNoNhaCungCap2.ISPHATSINHCONGNOTRONGKY = false;
				v_v_ThongKeCongNoNhaCungCap2.ISCONCONGNO = false;
				v_v_ThongKeCongNoNhaCungCap2.TUNGAY = DateTime.Now;
				v_v_ThongKeCongNoNhaCungCap2.DENNGAY = DateTime.Now;
				return View(v_v_ThongKeCongNoNhaCungCap2);
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
				List<v_ThongKeCongNoNhaCungCap> iPagedList = new List<v_ThongKeCongNoNhaCungCap>().OrderByDescending((v_ThongKeCongNoNhaCungCap s) => s.NAME).ToList();
				if (base.ModelState.IsValid)
				{
					ApiResponse apiResponse = new ApiResponse();
					sp_Parameter.LOC_ID = Utility.LOC_ID;
					sp_Parameter.ID_NHACUNGCAP = sp_Parameter.ID_NHACUNGCAP ?? "";
					sp_Parameter.ID_NHOMNCC = sp_Parameter.ID_NHOMNCC ?? "";
					sp_Parameter.ISTHEOTHOIGIAN = sp_Parameter.ISTHEOTHOIGIAN == true;
					sp_Parameter.ISPHATSINHCONGNO = sp_Parameter.ISPHATSINHCONGNO == true;
					sp_Parameter.ISPHATSINHCONGNOTRONGKY = sp_Parameter.ISPHATSINHCONGNOTRONGKY == true;
					sp_Parameter.ISCONCONGNO = sp_Parameter.ISCONCONGNO == true;
					iPagedList = new List<v_ThongKeCongNoNhaCungCap>().OrderByDescending((v_ThongKeCongNoNhaCungCap s) => s.NAME).ToList();
					apiResponse = Utility.Get_ThongKeCongNoNhaCungCap<v_ThongKeCongNoNhaCungCap>(sp_Parameter);
					if (!apiResponse.Success)
					{
						base.TempData["TitleError"] = apiResponse.Message;
						return RedirectToAction("Index", "Notfound");
					}
					if (apiResponse.Data != null)
					{
						iPagedList = (apiResponse.Data as List<v_ThongKeCongNoNhaCungCap>).OrderByDescending((v_ThongKeCongNoNhaCungCap s) => s.NAME).ToList();
					}
				}
				v_v_ThongKeCongNoNhaCungCap v_v_ThongKeCongNoNhaCungCap2 = new v_v_ThongKeCongNoNhaCungCap();
				v_v_ThongKeCongNoNhaCungCap2.IPagedList = iPagedList;
				v_v_ThongKeCongNoNhaCungCap2.lstdm_NhomNhaCungCap = new List<v_dm_NhomNhaCungCap>();
				v_v_ThongKeCongNoNhaCungCap2.lstdm_NhomNhaCungCap = Utility.GetListData<v_dm_NhomNhaCungCap>("GroupProvider", "", "", Utility.LOC_ID).Data as List<v_dm_NhomNhaCungCap>;
				v_v_ThongKeCongNoNhaCungCap2.lstdm_NhaCungCap = new List<v_dm_NhaCungCap>();
				v_v_ThongKeCongNoNhaCungCap2.lstdm_NhaCungCap = Utility.GetListData<v_dm_NhaCungCap>("Provider", "", "", Utility.LOC_ID).Data as List<v_dm_NhaCungCap>;
				v_v_ThongKeCongNoNhaCungCap2.ISTHEOTHOIGIAN = sp_Parameter.ISTHEOTHOIGIAN == true;
				v_v_ThongKeCongNoNhaCungCap2.ID_NHACUNGCAP = sp_Parameter.ID_NHACUNGCAP;
				v_v_ThongKeCongNoNhaCungCap2.ID_NHOMNCC = sp_Parameter.ID_NHOMNCC;
				v_v_ThongKeCongNoNhaCungCap2.ISPHATSINHCONGNO = sp_Parameter.ISPHATSINHCONGNO == true;
				v_v_ThongKeCongNoNhaCungCap2.ISPHATSINHCONGNOTRONGKY = sp_Parameter.ISPHATSINHCONGNOTRONGKY == true;
				v_v_ThongKeCongNoNhaCungCap2.ISCONCONGNO = sp_Parameter.ISCONCONGNO == true;
				v_v_ThongKeCongNoNhaCungCap2.TUNGAY = sp_Parameter.TUNGAY ?? DateTime.Now;
				v_v_ThongKeCongNoNhaCungCap2.DENNGAY = sp_Parameter.DENNGAY ?? DateTime.Now;
				return View(v_v_ThongKeCongNoNhaCungCap2);
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
				v_ThongKeCongNoNhaCungCap v_ThongKeCongNoNhaCungCap2 = JsonConvert.DeserializeObject<v_ThongKeCongNoNhaCungCap>(value);
				apiResponse = Utility.GetDebtCustomerDetail<v_ThongKeCongNo_ChiTiet>(v_ThongKeCongNoNhaCungCap2, "DebtProvider");
				List<v_ThongKeCongNo_ChiTiet> source = apiResponse.Data as List<v_ThongKeCongNo_ChiTiet>;
				source = (from itm in source
						  orderby itm.NGAY, itm.LOAIPHIEU
						  select itm).ToList();
				DataTable dataSource = Utility.ToDataTable(source);
				ReportClass report = new ReportClass();
				report = Utility.GetFormulaFields(report, v_ThongKeCongNoNhaCungCap2);
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
