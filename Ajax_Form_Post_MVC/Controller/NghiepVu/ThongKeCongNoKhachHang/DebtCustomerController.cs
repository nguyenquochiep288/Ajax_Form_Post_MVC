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

	public class DebtCustomerController : Controller
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
				List<v_ThongKeCongNoKhachHang> iPagedList = new List<v_ThongKeCongNoKhachHang>().OrderByDescending((v_ThongKeCongNoKhachHang s) => s.NAME).ToList();
				v_v_ThongKeCongNoKhachHang v_v_ThongKeCongNoKhachHang2 = new v_v_ThongKeCongNoKhachHang();
				v_v_ThongKeCongNoKhachHang2.IPagedList = iPagedList;
				v_v_ThongKeCongNoKhachHang2.lstdm_KhuVuc = new List<v_dm_KhuVuc>();
				v_v_ThongKeCongNoKhachHang2.lstdm_KhuVuc = Utility.GetListData<v_dm_KhuVuc>("Area", "", "", Utility.LOC_ID).Data as List<v_dm_KhuVuc>;
				v_v_ThongKeCongNoKhachHang2.lstdm_NhomKhachHang = new List<v_dm_NhomKhachHang>();
				v_v_ThongKeCongNoKhachHang2.lstdm_NhomKhachHang = Utility.GetListData<v_dm_NhomKhachHang>("GroupCustomer", "", "", Utility.LOC_ID).Data as List<v_dm_NhomKhachHang>;
				v_v_ThongKeCongNoKhachHang2.lstdm_KhachHang = new List<v_dm_KhachHang>();
				v_v_ThongKeCongNoKhachHang2.lstdm_KhachHang = Utility.GetListData<v_dm_KhachHang>("Customer", "", "", Utility.LOC_ID).Data as List<v_dm_KhachHang>;
				v_v_ThongKeCongNoKhachHang2.ISTHEOTHOIGIAN = true;
				v_v_ThongKeCongNoKhachHang2.ISPHATSINHCONGNO = true;
				v_v_ThongKeCongNoKhachHang2.ISPHATSINHCONGNOTRONGKY = false;
				v_v_ThongKeCongNoKhachHang2.ISCONCONGNO = false;
                v_v_ThongKeCongNoKhachHang2.ISNGAYQUAHAN = false;
                v_v_ThongKeCongNoKhachHang2.SONGAYQUAHAN = 7;
                v_v_ThongKeCongNoKhachHang2.TUNGAY = DateTime.Now;
				v_v_ThongKeCongNoKhachHang2.DENNGAY = DateTime.Now;
				return View(v_v_ThongKeCongNoKhachHang2);
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
				List<v_ThongKeCongNoKhachHang> iPagedList = new List<v_ThongKeCongNoKhachHang>().OrderByDescending((v_ThongKeCongNoKhachHang s) => s.NAME).ToList();
				if (base.ModelState.IsValid)
				{
					ApiResponse apiResponse = new ApiResponse();
					sp_Parameter.LOC_ID = Utility.LOC_ID;
					sp_Parameter.ID_KHACHHANG = sp_Parameter.ID_KHACHHANG ?? "";
					sp_Parameter.ID_NHOMKHACHHANG = sp_Parameter.ID_NHOMKHACHHANG ?? "";
					sp_Parameter.ID_KHUVUC = sp_Parameter.ID_KHUVUC ?? "";
					sp_Parameter.ISTHEOTHOIGIAN = sp_Parameter.ISTHEOTHOIGIAN == true;
					sp_Parameter.ISPHATSINHCONGNO = sp_Parameter.ISPHATSINHCONGNO == true;
					sp_Parameter.ISPHATSINHCONGNOTRONGKY = sp_Parameter.ISPHATSINHCONGNOTRONGKY == true;
					sp_Parameter.ISCONCONGNO = sp_Parameter.ISCONCONGNO == true;
					iPagedList = new List<v_ThongKeCongNoKhachHang>().OrderByDescending((v_ThongKeCongNoKhachHang s) => s.NAME).ToList();
					apiResponse = Utility.Get_ThongKeCongNoKhachHang<v_ThongKeCongNoKhachHang>(sp_Parameter);
					if (!apiResponse.Success)
					{
						base.TempData["TitleError"] = apiResponse.Message;
						return RedirectToAction("Index", "Notfound");
					}
					if (apiResponse.Data != null)
					{
						var lst = (apiResponse.Data as List<v_ThongKeCongNoKhachHang>).OrderByDescending((v_ThongKeCongNoKhachHang s) => s.NAME).ToList();
						if(sp_Parameter.ISNGAYQUAHAN == true)
						{
                            lst = lst.Where(s =>s.NGAY_PHIEUXUAT_CUOI.HasValue &&
								(DateTime.Now.Date - s.NGAY_PHIEUXUAT_CUOI.Value.Date).Days > sp_Parameter.SONGAYQUAHAN).OrderByDescending(s =>(DateTime.Now.Date - s.NGAY_PHIEUXUAT_CUOI.Value.Date).Days).ToList();
                        }
                        iPagedList = lst;
                    }
				}
				v_v_ThongKeCongNoKhachHang v_v_ThongKeCongNoKhachHang2 = new v_v_ThongKeCongNoKhachHang();
				v_v_ThongKeCongNoKhachHang2.IPagedList = iPagedList;
				v_v_ThongKeCongNoKhachHang2.lstdm_KhuVuc = new List<v_dm_KhuVuc>();
				v_v_ThongKeCongNoKhachHang2.lstdm_KhuVuc = Utility.GetListData<v_dm_KhuVuc>("Area", "", "", Utility.LOC_ID).Data as List<v_dm_KhuVuc>;
				v_v_ThongKeCongNoKhachHang2.lstdm_NhomKhachHang = new List<v_dm_NhomKhachHang>();
				v_v_ThongKeCongNoKhachHang2.lstdm_NhomKhachHang = Utility.GetListData<v_dm_NhomKhachHang>("GroupCustomer", "", "", Utility.LOC_ID).Data as List<v_dm_NhomKhachHang>;
				v_v_ThongKeCongNoKhachHang2.lstdm_KhachHang = new List<v_dm_KhachHang>();
				v_v_ThongKeCongNoKhachHang2.lstdm_KhachHang = Utility.GetListData<v_dm_KhachHang>("Customer", "", "", Utility.LOC_ID).Data as List<v_dm_KhachHang>;
				v_v_ThongKeCongNoKhachHang2.ISTHEOTHOIGIAN = sp_Parameter.ISTHEOTHOIGIAN == true;
				v_v_ThongKeCongNoKhachHang2.ID_KHACHHANG = sp_Parameter.ID_KHACHHANG;
				v_v_ThongKeCongNoKhachHang2.ID_NHOMKHACHHANG = sp_Parameter.ID_NHOMKHACHHANG;
				v_v_ThongKeCongNoKhachHang2.ID_KHUVUC = sp_Parameter.ID_KHUVUC;
				v_v_ThongKeCongNoKhachHang2.ISPHATSINHCONGNO = sp_Parameter.ISPHATSINHCONGNO == true;
				v_v_ThongKeCongNoKhachHang2.ISPHATSINHCONGNOTRONGKY = sp_Parameter.ISPHATSINHCONGNOTRONGKY == true;
				v_v_ThongKeCongNoKhachHang2.ISCONCONGNO = sp_Parameter.ISCONCONGNO == true;
				v_v_ThongKeCongNoKhachHang2.ISNGAYQUAHAN = sp_Parameter.ISNGAYQUAHAN == true;
				v_v_ThongKeCongNoKhachHang2.SONGAYQUAHAN = sp_Parameter.SONGAYQUAHAN;
				v_v_ThongKeCongNoKhachHang2.TUNGAY = sp_Parameter.TUNGAY ?? DateTime.Now;
				v_v_ThongKeCongNoKhachHang2.DENNGAY = sp_Parameter.DENNGAY ?? DateTime.Now;
				return View(v_v_ThongKeCongNoKhachHang2);
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
				v_ThongKeCongNoKhachHang v_ThongKeCongNoKhachHang2 = JsonConvert.DeserializeObject<v_ThongKeCongNoKhachHang>(value);
				apiResponse = Utility.GetDebtCustomerDetail<v_ThongKeCongNo_ChiTiet>(v_ThongKeCongNoKhachHang2, "DebtCustomer");
				List<v_ThongKeCongNo_ChiTiet> source = apiResponse.Data as List<v_ThongKeCongNo_ChiTiet>;
				source = (from itm in source
						  orderby itm.NGAY, itm.LOAIPHIEU
						  select itm).ToList();
				DataTable dataSource = Utility.ToDataTable(source);
				ReportClass report = new ReportClass();
				report = Utility.GetFormulaFields(report, v_ThongKeCongNoKhachHang2);
				report.SetDatabaseLogon("test", "test!", "test", "test");
				report.SetDataSource(dataSource);
				base.Response.Buffer = false;
				base.Response.ClearContent();
				base.Response.ClearHeaders();
				Stream stream = report.ExportToStream(ExportFormatType.PortableDocFormat);
				Utility.Report = report;
				apiResponse = new ApiResponse();
				apiResponse.Success = true;
				apiResponse.NAME = Utility.GetTitleFrom("DebtCustomer");
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
