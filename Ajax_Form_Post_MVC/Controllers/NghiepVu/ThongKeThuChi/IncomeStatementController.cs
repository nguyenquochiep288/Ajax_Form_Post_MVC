using DatabaseTHP;
using MVC_QuanLyTHP.Models;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Configuration;
using MVC_QuanLyTHP.Class;
using System.Web.UI;
using System.Collections.Generic;
using System;
using System.Web.DynamicData;
using PagedList;
using Syncfusion.EJ2.Linq;
using System.Reflection;
using System.Web.Routing;
using DatabaseTHP.Class;
using Newtonsoft.Json;
using static System.Data.Entity.Infrastructure.Design.Executor;
using DatabaseTHP.StoredProcedure.Parameter;
using System.Data.SqlClient;
using System.Data;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System.IO;
using DatabaseTHP.StoredProcedure;

namespace MVC_QuanLyTHP.Controllers
{
    public class IncomeStatementController : Controller
    {

        // GET: IncomeStatement
        public ActionResult Index()
        {
            try
            {
                if (Utility.KiemTra())
                {
                    return RedirectToAction("Index", "Admin");
                }
                //if (!Utility.KiemTraQuyen(API.ThongKeCongNoKhachHang, API.Xem))
                //{
                //    TempData["TitleError"] = API.TitlePermission;
                //    return RedirectToAction("Index", "Notfound");
                //}
                ApiResponse apiResponse = new ApiResponse();
                List<Sp_Get_ThongKeThuChi_Result> lstpage = (new List<Sp_Get_ThongKeThuChi_Result>()).OrderBy(s => s.NGAYLAP).ToList();
                v_v_ThongKeThuChi ThongKeCongNoKhachHang = new v_v_ThongKeThuChi();
                ThongKeCongNoKhachHang.IPagedList = lstpage;
                ThongKeCongNoKhachHang.TUNGAY = DateTime.Now;
                ThongKeCongNoKhachHang.DENNGAY = DateTime.Now;
                return View(ThongKeCongNoKhachHang);
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
                TempData["TitleError"] = API.TitleTryCatch;
                TempData["DetailError"] = ex.Message;
                return RedirectToAction("Index", "Notfound");
            }
        }


        [HttpPost, ValidateInput(false)]
        public ActionResult Index(SP_Parameter_Report sp_Parameter)
        {
            try
            {
                if (Utility.KiemTra())
                {
                    return RedirectToAction("Index", "Admin");
                }
                List<Sp_Get_ThongKeThuChi_Result> lstpage = (new List<Sp_Get_ThongKeThuChi_Result>()).OrderBy(s => s.NGAYLAP).ToList();
                if (ModelState.IsValid)
                {
                    ApiResponse apiResponse = new ApiResponse();
                    sp_Parameter.LOC_ID = Utility.LOC_ID;
                    sp_Parameter.ID_KHACHHANG = sp_Parameter.ID_KHACHHANG ?? "";
                    sp_Parameter.ID_NHOMKHACHHANG = sp_Parameter.ID_NHOMKHACHHANG ?? "";
                    sp_Parameter.ID_KHUVUC = sp_Parameter.ID_KHUVUC ?? "";
                    apiResponse = Utility.Get_ThongKeThuChi<Sp_Get_ThongKeThuChi_Result>(sp_Parameter, API.Sp_Get_ThongKeThuChi_GroupBy);
                    if (!apiResponse.Success)
                    {
                        TempData["TitleError"] = apiResponse.Message;
                        return RedirectToAction("Index", "Notfound");
                    }
                    if (apiResponse.Data != null)
                    {
                        lstpage = (apiResponse.Data as List<Sp_Get_ThongKeThuChi_Result>).OrderBy(s => s.NGAYLAP).ToList();
                    }
                }
                v_v_ThongKeThuChi ThongKeCongNoKhachHang = new v_v_ThongKeThuChi();
                ThongKeCongNoKhachHang.IPagedList = lstpage;    
                ThongKeCongNoKhachHang.TUNGAY = sp_Parameter.TUNGAY ?? DateTime.Now;
                ThongKeCongNoKhachHang.DENNGAY = sp_Parameter.DENNGAY ?? DateTime.Now;
                ThongKeCongNoKhachHang.HINHTHUC_THUCHI = sp_Parameter.HINHTHUC_THUCHI;
                return View(ThongKeCongNoKhachHang);
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
                TempData["TitleError"] = API.TitleTryCatch;
                TempData["DetailError"] = ex.Message;
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
                //if (!Utility.KiemTraQuyen(API.dm_KhachHang, API.Xem))
                //{
                //    TempData["TitleError"] = API.TitlePermission;
                //    return RedirectToAction("Index", "Notfound");
                //}
                string ShowSearchValue = clsMaHoa.Decrypt(ID.Replace(" ", "+"), clsMaHoa.PassMaHoa);
                SP_Parameter_Report ThongKeCongNoKhachHang = JsonConvert.DeserializeObject<SP_Parameter_Report>(ShowSearchValue);
                apiResponse = Utility.Get_ThongKeThuChi<Sp_Get_ThongKeThuChi_Result>(ThongKeCongNoKhachHang, API.Sp_Get_ThongKeThuChi);
                List<Sp_Get_ThongKeThuChi_Result> lstThongKeCongNo_ChiTiet = apiResponse.Data as List<Sp_Get_ThongKeThuChi_Result>;

                lstThongKeCongNo_ChiTiet = (from itm in lstThongKeCongNo_ChiTiet
                                            orderby itm.NGAYLAP
                                            select itm).ToList();

                DataTable data = Utility.ToDataTable<Sp_Get_ThongKeThuChi_Result>(lstThongKeCongNo_ChiTiet);

                var report = new ReportClass();
                report.FileName = System.Web.Hosting.HostingEnvironment.MapPath("~/Report/rptThongKeThuChiChiTiet.rpt");
                report.Load();
                report = Utility.GetFormulaFields(report, ThongKeCongNoKhachHang);
                report.SetDatabaseLogon("test", "test!", "test", "test");
                report.SetDataSource(data);
                Response.Buffer = false;
                Response.ClearContent();
                Response.ClearHeaders();
                Stream stream = report.ExportToStream(ExportFormatType.PortableDocFormat);
                Utility.Report = report;
                apiResponse = new ApiResponse();
                apiResponse.Success = true;
                apiResponse.NAME = Utility.GetTitleFrom(API.ThongKeThuChi);
                return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
                TempData["TitleError"] = API.TitleTryCatch;
                TempData["DetailError"] = ex.Message;
                apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
            }
        }
    }
}