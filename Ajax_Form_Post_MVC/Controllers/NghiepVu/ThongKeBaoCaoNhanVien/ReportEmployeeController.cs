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
    public class ReportEmployeeController : Controller
    {

        // GET: MoneyFund
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
                List<Sp_Get_BaoCaoTheoNhanVien_Result> lstpage = (new List<Sp_Get_BaoCaoTheoNhanVien_Result>()).OrderByDescending(s => s.TYPE).ToList();
                v_v_ThongKeTheoNhanVien ThongKeCongNoKhachHang = new v_v_ThongKeTheoNhanVien();
                ThongKeCongNoKhachHang.IPagedList = lstpage;
                ThongKeCongNoKhachHang.lstdm_NhanVien = new List<v_AspNetUsers>();
                ThongKeCongNoKhachHang.lstdm_NhanVien = Utility.GetListData<v_AspNetUsers>(API.AspNetUser, "", "", "").Data as List<v_AspNetUsers>;
                
                ThongKeCongNoKhachHang.ISTHEOTHOIGIAN = true;
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
        public ActionResult Index(SP_Parameter sp_Parameter)
        {
            try
            {
                if (Utility.KiemTra())
                {
                    return RedirectToAction("Index", "Admin");
                }
                //if (!Utility.KiemTraQuyen(API.dm_KhachHang, API.Create))
                //{
                //    TempData["TitleError"] = API.TitlePermission;
                //    return RedirectToAction("Index", "Notfound");
                //}
                List<Sp_Get_BaoCaoTheoNhanVien_Result> lstpage = (new List<Sp_Get_BaoCaoTheoNhanVien_Result>()).OrderByDescending(s => s.TYPE).ToList();
                if (ModelState.IsValid)
                {
                    ApiResponse apiResponse = new ApiResponse();
                    sp_Parameter.LOC_ID = Utility.LOC_ID;
                    sp_Parameter.ID_NHANVIEN = sp_Parameter.ID_NHANVIEN ?? "";
                    sp_Parameter.ISTHEOTHOIGIAN = sp_Parameter.ISTHEOTHOIGIAN ?? false;
                    lstpage = (new List<Sp_Get_BaoCaoTheoNhanVien_Result>()).OrderByDescending(s => s.TYPE).ToList();
                    apiResponse = Utility.Get_ThongKeBaoCaoNhanVien<Sp_Get_BaoCaoTheoNhanVien_Result>(sp_Parameter);
                    if (!apiResponse.Success)
                    {
                        TempData["TitleError"] = apiResponse.Message;
                        return RedirectToAction("Index", "Notfound");
                    }
                    if (apiResponse.Data != null)
                    {
                        lstpage = (apiResponse.Data as List<Sp_Get_BaoCaoTheoNhanVien_Result>).OrderByDescending(s => s.ID_NGUOITAO).ToList();
                    }
                }
                v_v_ThongKeTheoNhanVien ThongKeCongNoKhachHang = new v_v_ThongKeTheoNhanVien();
                ThongKeCongNoKhachHang.IPagedList = lstpage;
                ThongKeCongNoKhachHang.lstdm_NhanVien = new List<v_AspNetUsers>();
                ThongKeCongNoKhachHang.lstdm_NhanVien = Utility.GetListData<v_AspNetUsers>(API.AspNetUser , "", "","").Data as List<v_AspNetUsers>;
  
                ThongKeCongNoKhachHang.ISTHEOTHOIGIAN = sp_Parameter.ISTHEOTHOIGIAN ?? false;
                ThongKeCongNoKhachHang.ID_NHANVIEN = sp_Parameter.ID_TAIKHOANNGANHANG;
                ThongKeCongNoKhachHang.TUNGAY = sp_Parameter.TUNGAY ?? DateTime.Now;
                ThongKeCongNoKhachHang.DENNGAY = sp_Parameter.DENNGAY ?? DateTime.Now;
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
                Sp_Get_BaoCaoTheoNhanVien_Result ThongKeCongNoKhachHang = JsonConvert.DeserializeObject<Sp_Get_BaoCaoTheoNhanVien_Result>(ShowSearchValue);
                apiResponse = Utility.GetReportEmployeeDetail<v_ThongKeCongNo_ChiTiet>(ThongKeCongNoKhachHang, API.BaoCaoNhanVien);
                List<v_ThongKeCongNo_ChiTiet> lstThongKeCongNo_ChiTiet = apiResponse.Data as List<v_ThongKeCongNo_ChiTiet>;

                lstThongKeCongNo_ChiTiet = (from itm in lstThongKeCongNo_ChiTiet
                                            orderby itm.NGAY,itm.LOAIPHIEU
                                            select itm).ToList();

                DataTable data = Utility.ToDataTable<v_ThongKeCongNo_ChiTiet>(lstThongKeCongNo_ChiTiet);

                var report = new ReportClass();

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
                apiResponse.NAME = Utility.GetTitleFrom(API.ThongKeQuyTien);
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