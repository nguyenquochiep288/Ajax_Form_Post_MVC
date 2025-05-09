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
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System.Data;
using System.IO;

namespace MVC_QuanLyTHP.Controllers
{
    public class DebtProviderController : Controller
    {

        // GET: DebtProvider
        public ActionResult Index()
        {
            try
            {
                if (Utility.KiemTra())
                {
                    return RedirectToAction("Index", "Admin");
                }
                //if (!Utility.KiemTraQuyen(API.ThongKeCongNoNhaCungCap, API.Xem))
                //{
                //    TempData["TitleError"] = API.TitlePermission;
                //    return RedirectToAction("Index", "Notfound");
                //}
                ApiResponse apiResponse = new ApiResponse();
                List<v_ThongKeCongNoNhaCungCap> lstpage = (new List<v_ThongKeCongNoNhaCungCap>()).OrderByDescending(s => s.NAME).ToList();               
                v_v_ThongKeCongNoNhaCungCap ThongKeCongNoNhaCungCap = new v_v_ThongKeCongNoNhaCungCap();
                ThongKeCongNoNhaCungCap.IPagedList = lstpage;
               
                ThongKeCongNoNhaCungCap.lstdm_NhomNhaCungCap = new List<v_dm_NhomNhaCungCap>();
                ThongKeCongNoNhaCungCap.lstdm_NhomNhaCungCap = Utility.GetListData<v_dm_NhomNhaCungCap>(API.dm_NhomNhaCungCap, "", "", Utility.LOC_ID).Data as List<v_dm_NhomNhaCungCap>;
                ThongKeCongNoNhaCungCap.lstdm_NhaCungCap = new List<v_dm_NhaCungCap>();
                ThongKeCongNoNhaCungCap.lstdm_NhaCungCap = Utility.GetListData<v_dm_NhaCungCap>(API.dm_NhaCungCap, "", "", Utility.LOC_ID).Data as List<v_dm_NhaCungCap>;
                ThongKeCongNoNhaCungCap.ISTHEOTHOIGIAN = true;
                ThongKeCongNoNhaCungCap.ISPHATSINHCONGNO = true;
                ThongKeCongNoNhaCungCap.ISPHATSINHCONGNOTRONGKY = false;
                ThongKeCongNoNhaCungCap.ISCONCONGNO = false;
                ThongKeCongNoNhaCungCap.TUNGAY = DateTime.Now;
                ThongKeCongNoNhaCungCap.DENNGAY = DateTime.Now;
                return View(ThongKeCongNoNhaCungCap);
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
                List<v_ThongKeCongNoNhaCungCap> lstpage = (new List<v_ThongKeCongNoNhaCungCap>()).OrderByDescending(s => s.NAME).ToList();
                if (ModelState.IsValid)
                {
                    ApiResponse apiResponse = new ApiResponse();
                    sp_Parameter.LOC_ID = Utility.LOC_ID;
                    sp_Parameter.ID_NHACUNGCAP = sp_Parameter.ID_NHACUNGCAP ?? "";
                    sp_Parameter.ID_NHOMNCC = sp_Parameter.ID_NHOMNCC ?? "";
                    sp_Parameter.ISTHEOTHOIGIAN = sp_Parameter.ISTHEOTHOIGIAN ?? false;
                    sp_Parameter.ISPHATSINHCONGNO = sp_Parameter.ISPHATSINHCONGNO ?? false;
                    sp_Parameter.ISPHATSINHCONGNOTRONGKY = sp_Parameter.ISPHATSINHCONGNOTRONGKY ?? false;
                    sp_Parameter.ISCONCONGNO = sp_Parameter.ISCONCONGNO ?? false;

                    lstpage = (new List<v_ThongKeCongNoNhaCungCap>()).OrderByDescending(s => s.NAME).ToList();
                    apiResponse = Utility.Get_ThongKeCongNoNhaCungCap<v_ThongKeCongNoNhaCungCap>(sp_Parameter);
                    if (!apiResponse.Success)
                    {
                        TempData["TitleError"] = apiResponse.Message;
                        return RedirectToAction("Index", "Notfound");
                    }
                    if (apiResponse.Data != null)
                    {
                        lstpage = (apiResponse.Data as List<v_ThongKeCongNoNhaCungCap>).OrderByDescending(s => s.NAME).ToList();
                    }
                }
                v_v_ThongKeCongNoNhaCungCap ThongKeCongNoNhaCungCap = new v_v_ThongKeCongNoNhaCungCap();
                ThongKeCongNoNhaCungCap.IPagedList = lstpage;
                ThongKeCongNoNhaCungCap.lstdm_NhomNhaCungCap = new List<v_dm_NhomNhaCungCap>();
                ThongKeCongNoNhaCungCap.lstdm_NhomNhaCungCap = Utility.GetListData<v_dm_NhomNhaCungCap>(API.dm_NhomNhaCungCap, "", "", Utility.LOC_ID).Data as List<v_dm_NhomNhaCungCap>;
                ThongKeCongNoNhaCungCap.lstdm_NhaCungCap = new List<v_dm_NhaCungCap>();
                ThongKeCongNoNhaCungCap.lstdm_NhaCungCap = Utility.GetListData<v_dm_NhaCungCap>(API.dm_NhaCungCap, "", "", Utility.LOC_ID).Data as List<v_dm_NhaCungCap>;
                ThongKeCongNoNhaCungCap.ISTHEOTHOIGIAN = sp_Parameter.ISTHEOTHOIGIAN ?? false;
                ThongKeCongNoNhaCungCap.ID_NHACUNGCAP = sp_Parameter.ID_NHACUNGCAP;
                ThongKeCongNoNhaCungCap.ID_NHOMNCC = sp_Parameter.ID_NHOMNCC;
                ThongKeCongNoNhaCungCap.ISPHATSINHCONGNO = sp_Parameter.ISPHATSINHCONGNO ?? false;
                ThongKeCongNoNhaCungCap.ISPHATSINHCONGNOTRONGKY = sp_Parameter.ISPHATSINHCONGNOTRONGKY ?? false;
                ThongKeCongNoNhaCungCap.ISCONCONGNO = sp_Parameter.ISCONCONGNO ?? false;
                ThongKeCongNoNhaCungCap.TUNGAY = sp_Parameter.TUNGAY ?? DateTime.Now;
                ThongKeCongNoNhaCungCap.DENNGAY = sp_Parameter.DENNGAY ?? DateTime.Now;
                return View(ThongKeCongNoNhaCungCap);
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
                v_ThongKeCongNoNhaCungCap ThongKeCongNoNhaCungCap = JsonConvert.DeserializeObject<v_ThongKeCongNoNhaCungCap>(ShowSearchValue);
                apiResponse = Utility.GetDebtCustomerDetail<v_ThongKeCongNo_ChiTiet>(ThongKeCongNoNhaCungCap, API.ThongKeCongNoNhaCungCap);
                List<v_ThongKeCongNo_ChiTiet> lstThongKeCongNo_ChiTiet = apiResponse.Data as List<v_ThongKeCongNo_ChiTiet>;

                lstThongKeCongNo_ChiTiet = (from itm in lstThongKeCongNo_ChiTiet
                                            orderby itm.NGAY, itm.LOAIPHIEU
                                            select itm).ToList();

                DataTable data = Utility.ToDataTable<v_ThongKeCongNo_ChiTiet>(lstThongKeCongNo_ChiTiet);

                var report = new ReportClass();

                report = Utility.GetFormulaFields(report, ThongKeCongNoNhaCungCap);
                report.SetDatabaseLogon("test", "test!", "test", "test");
                report.SetDataSource(data);
                Response.Buffer = false;
                Response.ClearContent();
                Response.ClearHeaders();
                Stream stream = report.ExportToStream(ExportFormatType.PortableDocFormat);
                Utility.Report = report;
                apiResponse = new ApiResponse();
                apiResponse.Success = true;
                apiResponse.NAME = Utility.GetTitleFrom(API.ThongKeCongNoNhaCungCap);
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