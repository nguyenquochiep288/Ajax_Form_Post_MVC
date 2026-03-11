// MVC_QuanLyTHP, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVC_QuanLyTHP.Controllers.NghiepVu.ViewReportController
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
using MVC_QuanLyTHP.Controllers;
using MVC_QuanLyTHP.Models;

public class ViewReportController : Controller
{
    public ActionResult Index()
    {
        if (Utility.KiemTra())
        {
            return RedirectToAction("Index", "Admin");
        }
        v_v_web_Report v_v_web_Report2 = new v_v_web_Report();
        string text = "";
        List<v_web_Menu> menu = Utility.GetMenu();
        if (menu != null && menu.Count > 0)
        {
            v_web_Menu objReport = menu.FirstOrDefault((v_web_Menu e) => e.CONTROLLERNAME == "ViewReport" && e.ISACTIVE);
            if (objReport != null)
            {
                text = "<ul id=\"treeview\">";
                List<v_web_Menu> list = (from e in menu
                                         where e.ID_QUYENCHA == objReport.ID && e.ISACTIVE
                                         orderby e.STT
                                         select e).ToList();
                if (list != null && list.Count() > 0)
                {
                    foreach (v_web_Menu item in list)
                    {
                        if (Utility.KiemTraQuyen(item.CONTROLLERNAME, "View"))
                        {
                            text += GetMenuReport(menu, item);
                        }
                    }
                }
                text += " </ul>";
            }
        }
        base.ViewBag.ViewMenu = text;
        v_v_web_Report2.lstValue = new List<ListValue>();
        return View(v_v_web_Report2);
    }

    private string GetMenuReport(List<v_web_Menu> lstMenu, v_web_Menu MenuCha)
    {
        string text = "<li data-icon-cls=\"" + MenuCha.ICON + "\">" + MenuCha.NAME + ((!string.IsNullOrEmpty(MenuCha.CONTROLLERNAME)) ? ("<a style=\"margin-left:20px\" data-toggle=\"tooltip\" data-placement=\"top\" title=\"" + Utility.ThemTab + "!\" onclick=\"AddTab('" + MenuCha.ID + "')\"><i class=\"fa fa-plus-square\"></i></a>") : "");
        List<v_web_Menu> list = lstMenu.Where((v_web_Menu e) => e.ID_QUYENCHA == MenuCha.ID).ToList();
        if (list != null && list.Count() > 0)
        {
            text += "<ul>";
            foreach (v_web_Menu item in list)
            {
                text += GetMenuReport(lstMenu, item);
            }
            text += "</ul>";
        }
        return text + "</li>";
    }

    [HttpGet]
    public ActionResult ExportExcel()
    {
        Stream stream = Utility.Report.ExportToStream(ExportFormatType.ExcelWorkbook);
        stream.Seek(0L, SeekOrigin.Begin);
        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "report.xlsx");
    }

    [HttpPost]
    [ValidateInput(false)]
    public ActionResult VerReporte(SP_Parameter_Report objParameter)
    {
        try
        {
            ApiResponse apiResponse = new ApiResponse();
            if (Utility.KiemTra())
            {
                return RedirectToAction("Index", "Admin");
            }
            if (!string.IsNullOrEmpty(objParameter.CONTROLLER))
            {
                List<v_web_Menu> menu = Utility.GetMenu();
                v_web_Menu v_web_Menu2 = menu.FirstOrDefault((v_web_Menu e) => e.CONTROLLERNAME == objParameter.CONTROLLER && e.ISACTIVE);
                if (v_web_Menu2 != null)
                {
                    objParameter.ID_REPORT = v_web_Menu2.ID_REPORT;
                }
            }
            if (string.IsNullOrEmpty(objParameter.ID_REPORT))
            {
                apiResponse.Success = false;
                apiResponse.Message = "Không tìm thấy cấu hình báo cáo " + objParameter.ID_REPORT + "!";
                return new JsonResult
                {
                    Data = apiResponse,
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                    MaxJsonLength = int.MaxValue
                };
            }
            apiResponse = Utility.GetDetail<v_web_Menu>(objParameter.ID_REPORT, "Menu");
            v_web_Menu v_web_Menu3 = new v_web_Menu();
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
                v_web_Menu3 = apiResponse.Data as v_web_Menu;
            }
            if (v_web_Menu3 == null)
            {
                apiResponse.Success = false;
                apiResponse.Message = "Không tìm thấy cấu hình báo cáo " + ((v_web_Menu3 != null) ? v_web_Menu3.NAME : objParameter.ID_REPORT) + "!";
                return new JsonResult
                {
                    Data = apiResponse,
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                    MaxJsonLength = int.MaxValue
                };
            }
            v_v_web_Report v_v_web_Report2 = new v_v_web_Report();
            apiResponse = Utility.GetDetail<v_v_web_Report>(v_web_Menu3.ID_REPORT, "Report");
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
                v_v_web_Report2 = apiResponse.Data as v_v_web_Report;
            }
            string text = Path.Combine(base.Server.MapPath("~/Images_Upload/Product/"), "MyBinaryQR.png");
            string text2 = Path.Combine(base.Server.MapPath("~/Images_Upload/Logo/"), "logoTrangHiepPhat.jpg");
            v_v_dm_CongTy v_v_dm_CongTy2 = new v_v_dm_CongTy();
            apiResponse = Utility.GetDetail<v_v_dm_CongTy>(Utility.LOC_ID, "Company");
            if (apiResponse.Data != null)
            {
                v_v_dm_CongTy2 = apiResponse.Data as v_v_dm_CongTy;
            }
            objParameter.NAME_SP = v_v_web_Report2.NAME_SP;
            objParameter.LOC_ID = Utility.LOC_ID;
            ReportClass reportClass = new ReportClass();
            string text3 = v_v_web_Report2.REPORT;
            if (text3 == "~/Report/rptBaoCaoTaiChinh.rpt" && objParameter.HINHTHUC_BAOCAOTAICHINH == 2)
            {
                text3 = "~/Report/rptBaoCaoTaiChinh_KhanhHang.rpt";
            }
            reportClass.FileName = base.Server.MapPath(text3);
            DataTable dataTable = new DataTable();
            if (v_v_web_Report2.NAME_SP != "Sp_Get_DanhSachHangHoa_BanChay" && !objParameter.HINHTHUC_PHIEUXUATHANG_KHUYENMAI.HasValue && !objParameter.HINHTHUC_BAOCAOTAICHINH.HasValue && !objParameter.HINHTHUC.HasValue)
            {
                apiResponse = Utility.ExecuteStoredProc<DataTable>(objParameter, "SP_GetReport");
                if (!apiResponse.Success)
                {
                    base.TempData["TitleError"] = apiResponse.Message;
                    return RedirectToAction("Index", "Notfound");
                }
                dataTable = apiResponse.Data as DataTable;
                if (apiResponse.CheckValue)
                {
                    dataTable.Rows.Clear();
                }
            }
            reportClass.Load();
            reportClass.DataDefinition.FormulaFields["TENCONGTY"].Text = "'" + v_v_dm_CongTy2.NAME + "'";
            reportClass.DataDefinition.FormulaFields["DIACHI"].Text = "'" + v_v_dm_CongTy2.ADDRESS + "'";
            reportClass.DataDefinition.FormulaFields["DIENTHOAI"].Text = "'" + v_v_dm_CongTy2.TEL + "'";
            reportClass.DataDefinition.FormulaFields["ICON"].Text = "'" + text2 + "'";
            switch (v_v_web_Report2.REPORT)
            {
                case "~/Report/rptBaoCaoPhieuDatHang.rpt":
                    {
                        string name = ((!(v_v_web_Report2.NAME_SP == "Sp_Get_DanhSachPhieuNhap_ChiTiet_BaoCao") && !(v_v_web_Report2.NAME_SP == "Sp_Get_DanhSachPhieuNhapTraHang_ChiTiet_BaoCao")) ? "Sp_Get_BaoCaoPhieuDatHang" : ((v_v_web_Report2.NAME_SP == "Sp_Get_DanhSachPhieuNhapTraHang_ChiTiet_BaoCao") ? "Sp_Get_DanhSachPhieuNhapTraHang_ChiTiet_BaoCao" : "Sp_Get_DanhSachPhieuNhap_ChiTiet_BaoCao"));
                        apiResponse = Utility.ExecuteStoredProcT<v_ct_PhieuDatHang_ChiTiet_BaoCao>(objParameter, name);
                        List<v_ct_PhieuDatHang_ChiTiet_BaoCao> list2 = apiResponse.Data as List<v_ct_PhieuDatHang_ChiTiet_BaoCao>;
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
                        int num = 0;
                        List<v_PhieuGioaHang_InTheoGroup> list3 = new List<v_PhieuGioaHang_InTheoGroup>();
                        if (list2 != null)
                        {
                            num = (from s in list2
                                   group s by new { s.MAPHIEU }).Count();
                            if (v_v_web_Report2.NAME_SP == "Sp_Get_DanhSachPhieuNhap_ChiTiet_BaoCao")
                            {
                                if (objParameter.HINHTHUC == 1)
                                {
                                    reportClass.DataDefinition.FormulaFields["TIEUDE"].Text = "'BÁO CÁO NHẬP HÀNG THEO NHÓM HÀNG'";
                                    list3 = (from s in list2
                                             group s by new { s.NAME_KHUVUC, s.TYLE_QD, s.TRONGLUONG, s.NAME, s.MA, s.NAME_DVT, s.NAME_DVT_QD, s.NAME_NHOMHANGHOA } into s
                                             select new v_PhieuGioaHang_InTheoGroup
                                             {
                                                 NAME_GROUP = s.Key.NAME_NHOMHANGHOA,
                                                 MAPHIEUXUAT = "",
                                                 MA_HANGHOA = s.Key.MA,
                                                 TONGTRONGLUONG = Convert.ToDecimal(s.Key.TRONGLUONG * s.Sum((v_ct_PhieuDatHang_ChiTiet_BaoCao x) => Math.Round(x.TONGSOLUONG, 0))),
                                                 NAME_HANGHOA = s.Key.NAME,
                                                 NAME_DVT = s.Key.NAME_DVT,
                                                 CHIETKHAU = s.Sum((v_ct_PhieuDatHang_ChiTiet_BaoCao x) => Math.Round(x.CHIETKHAU, 0)),
                                                 TONGTIENGIAMGIA = s.Sum((v_ct_PhieuDatHang_ChiTiet_BaoCao x) => Math.Round(x.TONGTIENGIAMGIA, 0)),
                                                 THANHTIEN = s.Sum((v_ct_PhieuDatHang_ChiTiet_BaoCao x) => Math.Round(x.THANHTIEN, 0)),
                                                 THUESUAT = s.Sum((v_ct_PhieuDatHang_ChiTiet_BaoCao x) => Math.Round(x.THUESUAT, 0)),
                                                 TONGTIENVAT = s.Sum((v_ct_PhieuDatHang_ChiTiet_BaoCao x) => Math.Round(x.TONGTIENVAT, 0)),
                                                 TONGCONG = s.Sum((v_ct_PhieuDatHang_ChiTiet_BaoCao x) => Math.Round(x.TONGCONG, 0)),
                                                 TONGSOLUONG = Convert.ToDecimal(s.Sum((v_ct_PhieuDatHang_ChiTiet_BaoCao x) => Math.Round(x.TONGSOLUONG, 0))),
                                                 NAME_DVT_QD = s.Key.NAME_DVT_QD,
                                                 TYLE_QD = s.Key.TYLE_QD
                                             }).ToList();
                                }
                                else if (objParameter.HINHTHUC == 3)
                                {
                                    reportClass.DataDefinition.FormulaFields["TIEUDE"].Text = "'BÁO CÁO NHẬP HÀNG THEO NHÀ CUNG CẤP'";
                                    list3 = (from s in list2
                                             group s by new { s.NAME_KHUVUC, s.TYLE_QD, s.TRONGLUONG, s.NAME, s.MA, s.NAME_DVT, s.NAME_DVT_QD, s.NAME_KHACHHANG, s.TEL_KHACHHANG } into s
                                             select new v_PhieuGioaHang_InTheoGroup
                                             {
                                                 NAME_GROUP = s.Key.NAME_KHACHHANG + (string.IsNullOrEmpty(s.Key.TEL_KHACHHANG) ? "" : (Environment.NewLine + "Điện thoại: ")) + s.Key.TEL_KHACHHANG,
                                                 MAPHIEUXUAT = s.Key.NAME_KHUVUC,
                                                 MA_HANGHOA = s.Key.MA,
                                                 NAME_HANGHOA = s.Key.NAME,
                                                 NAME_DVT = s.Key.NAME_DVT,
                                                 CHIETKHAU = s.Sum((v_ct_PhieuDatHang_ChiTiet_BaoCao x) => Math.Round(x.CHIETKHAU, 0)),
                                                 TONGTIENGIAMGIA = s.Sum((v_ct_PhieuDatHang_ChiTiet_BaoCao x) => Math.Round(x.TONGTIENGIAMGIA, 0)),
                                                 THANHTIEN = s.Sum((v_ct_PhieuDatHang_ChiTiet_BaoCao x) => Math.Round(x.THANHTIEN, 0)),
                                                 THUESUAT = s.Sum((v_ct_PhieuDatHang_ChiTiet_BaoCao x) => Math.Round(x.THUESUAT, 0)),
                                                 TONGTIENVAT = s.Sum((v_ct_PhieuDatHang_ChiTiet_BaoCao x) => Math.Round(x.TONGTIENVAT, 0)),
                                                 TONGCONG = s.Sum((v_ct_PhieuDatHang_ChiTiet_BaoCao x) => Math.Round(x.TONGCONG, 0)),
                                                 TONGSOLUONG = Convert.ToDecimal(s.Sum((v_ct_PhieuDatHang_ChiTiet_BaoCao x) => Math.Round(x.TONGSOLUONG, 0))),
                                                 TONGTRONGLUONG = Convert.ToDecimal(s.Key.TRONGLUONG * s.Sum((v_ct_PhieuDatHang_ChiTiet_BaoCao x) => Math.Round(x.TONGSOLUONG, 0))),
                                                 NAME_DVT_QD = s.Key.NAME_DVT_QD,
                                                 TYLE_QD = s.Key.TYLE_QD
                                             }).ToList();
                                }
                            }
                            else if (v_v_web_Report2.NAME_SP == "Sp_Get_DanhSachPhieuNhapTraHang_ChiTiet_BaoCao")
                            {
                                if (objParameter.HINHTHUC == 1)
                                {
                                    reportClass.DataDefinition.FormulaFields["TIEUDE"].Text = "'BÁO CÁO NHẬP TRẢ HÀNG THEO LOẠI PHIẾU NHẬP'";
                                    list3 = (from s in list2
                                             group s by new { s.NAME_KHUVUC, s.TYLE_QD, s.TRONGLUONG, s.NAME, s.MA, s.NAME_DVT, s.NAME_DVT_QD, s.NAME_NHOMHANGHOA } into s
                                             select new v_PhieuGioaHang_InTheoGroup
                                             {
                                                 NAME_GROUP = s.Key.NAME_KHUVUC,
                                                 MAPHIEUXUAT = s.Key.NAME_NHOMHANGHOA,
                                                 MA_HANGHOA = s.Key.MA,
                                                 NAME_HANGHOA = s.Key.NAME,
                                                 NAME_DVT = s.Key.NAME_DVT,
                                                 CHIETKHAU = s.Sum((v_ct_PhieuDatHang_ChiTiet_BaoCao x) => Math.Round(x.CHIETKHAU, 0)),
                                                 TONGTIENGIAMGIA = s.Sum((v_ct_PhieuDatHang_ChiTiet_BaoCao x) => Math.Round(x.TONGTIENGIAMGIA, 0)),
                                                 THANHTIEN = s.Sum((v_ct_PhieuDatHang_ChiTiet_BaoCao x) => Math.Round(x.THANHTIEN, 0)),
                                                 THUESUAT = s.Sum((v_ct_PhieuDatHang_ChiTiet_BaoCao x) => Math.Round(x.THUESUAT, 0)),
                                                 TONGTIENVAT = s.Sum((v_ct_PhieuDatHang_ChiTiet_BaoCao x) => Math.Round(x.TONGTIENVAT, 0)),
                                                 TONGCONG = s.Sum((v_ct_PhieuDatHang_ChiTiet_BaoCao x) => Math.Round(x.TONGCONG, 0)),
                                                 TONGSOLUONG = Convert.ToDecimal(s.Sum((v_ct_PhieuDatHang_ChiTiet_BaoCao x) => Math.Round(x.TONGSOLUONG, 0))),
                                                 TONGTRONGLUONG = Convert.ToDecimal(s.Key.TRONGLUONG * s.Sum((v_ct_PhieuDatHang_ChiTiet_BaoCao x) => Math.Round(x.TONGSOLUONG, 0))),
                                                 NAME_DVT_QD = s.Key.NAME_DVT_QD,
                                                 TYLE_QD = s.Key.TYLE_QD
                                             }).ToList();
                                }
                                else if (objParameter.HINHTHUC == 3)
                                {
                                    reportClass.DataDefinition.FormulaFields["TIEUDE"].Text = "'BÁO CÁO NHẬP TRẢ HÀNG THEO KHÁCH HÀNG'";
                                    list3 = (from s in list2
                                             group s by new { s.NAME_KHUVUC, s.TYLE_QD, s.TRONGLUONG, s.NAME, s.MA, s.NAME_DVT, s.NAME_DVT_QD, s.NAME_KHACHHANG, s.TEL_KHACHHANG } into s
                                             select new v_PhieuGioaHang_InTheoGroup
                                             {
                                                 NAME_GROUP = s.Key.NAME_KHUVUC,
                                                 MAPHIEUXUAT = s.Key.NAME_KHACHHANG + (string.IsNullOrEmpty(s.Key.TEL_KHACHHANG) ? "" : (Environment.NewLine + "Điện thoại: ")) + s.Key.TEL_KHACHHANG,
                                                 MA_HANGHOA = s.Key.MA,
                                                 NAME_HANGHOA = s.Key.NAME,
                                                 NAME_DVT = s.Key.NAME_DVT,
                                                 CHIETKHAU = s.Sum((v_ct_PhieuDatHang_ChiTiet_BaoCao x) => Math.Round(x.CHIETKHAU, 0)),
                                                 TONGTIENGIAMGIA = s.Sum((v_ct_PhieuDatHang_ChiTiet_BaoCao x) => Math.Round(x.TONGTIENGIAMGIA, 0)),
                                                 THANHTIEN = s.Sum((v_ct_PhieuDatHang_ChiTiet_BaoCao x) => Math.Round(x.THANHTIEN, 0)),
                                                 THUESUAT = s.Sum((v_ct_PhieuDatHang_ChiTiet_BaoCao x) => Math.Round(x.THUESUAT, 0)),
                                                 TONGTIENVAT = s.Sum((v_ct_PhieuDatHang_ChiTiet_BaoCao x) => Math.Round(x.TONGTIENVAT, 0)),
                                                 TONGCONG = s.Sum((v_ct_PhieuDatHang_ChiTiet_BaoCao x) => Math.Round(x.TONGCONG, 0)),
                                                 TONGSOLUONG = Convert.ToDecimal(s.Sum((v_ct_PhieuDatHang_ChiTiet_BaoCao x) => Math.Round(x.TONGSOLUONG, 0))),
                                                 TONGTRONGLUONG = Convert.ToDecimal(s.Key.TRONGLUONG * s.Sum((v_ct_PhieuDatHang_ChiTiet_BaoCao x) => Math.Round(x.TONGSOLUONG, 0))),
                                                 NAME_DVT_QD = s.Key.NAME_DVT_QD,
                                                 TYLE_QD = s.Key.TYLE_QD
                                             }).ToList();
                                }
                            }
                            else if (objParameter.HINHTHUC == 1)
                            {
                                reportClass.DataDefinition.FormulaFields["TIEUDE"].Text = "'BÁO CÁO ĐẶT HÀNG THEO NHÓM HÀNG'";
                                list3 = (from s in list2
                                         group s by new { s.NAME_KHUVUC, s.TYLE_QD, s.TRONGLUONG, s.NAME, s.MA, s.NAME_DVT, s.NAME_DVT_QD, s.NAME_NHOMHANGHOA } into s
                                         select new v_PhieuGioaHang_InTheoGroup
                                         {
                                             NAME_GROUP = s.Key.NAME_KHUVUC,
                                             MAPHIEUXUAT = s.Key.NAME_NHOMHANGHOA,
                                             MA_HANGHOA = s.Key.MA,
                                             TONGTRONGLUONG = Convert.ToDecimal(s.Key.TRONGLUONG * s.Sum((v_ct_PhieuDatHang_ChiTiet_BaoCao x) => Math.Round(x.TONGSOLUONG, 0))),
                                             NAME_HANGHOA = s.Key.NAME + " - (" + Math.Round(Convert.ToDecimal(s.Key.TRONGLUONG * s.Sum((v_ct_PhieuDatHang_ChiTiet_BaoCao x) => Math.Round(x.TONGSOLUONG, 0))) / 1000m, 1).ToString("N0") + " Kg)",
                                             NAME_DVT = s.Key.NAME_DVT,
                                             CHIETKHAU = s.Sum((v_ct_PhieuDatHang_ChiTiet_BaoCao x) => Math.Round(x.CHIETKHAU, 0)),
                                             TONGTIENGIAMGIA = s.Sum((v_ct_PhieuDatHang_ChiTiet_BaoCao x) => Math.Round(x.TONGTIENGIAMGIA, 0)),
                                             THANHTIEN = s.Sum((v_ct_PhieuDatHang_ChiTiet_BaoCao x) => Math.Round(x.THANHTIEN, 0)),
                                             THUESUAT = s.Sum((v_ct_PhieuDatHang_ChiTiet_BaoCao x) => Math.Round(x.THUESUAT, 0)),
                                             TONGTIENVAT = s.Sum((v_ct_PhieuDatHang_ChiTiet_BaoCao x) => Math.Round(x.TONGTIENVAT, 0)),
                                             TONGCONG = s.Sum((v_ct_PhieuDatHang_ChiTiet_BaoCao x) => Math.Round(x.TONGCONG, 0)),
                                             TONGSOLUONG = Convert.ToDecimal(s.Sum((v_ct_PhieuDatHang_ChiTiet_BaoCao x) => Math.Round(x.TONGSOLUONG, 0))),
                                             NAME_DVT_QD = s.Key.NAME_DVT_QD,
                                             TYLE_QD = s.Key.TYLE_QD
                                         }).ToList();
                            }
                            else if (objParameter.HINHTHUC == 2)
                            {
                                reportClass.DataDefinition.FormulaFields["TIEUDE"].Text = "'BÁO CÁO ĐẶT HÀNG THEO NHÂN VIÊN'";
                                list3 = (from s in list2
                                         group s by new { s.NAME_KHUVUC, s.TYLE_QD, s.TRONGLUONG, s.NAME, s.MA, s.NAME_DVT, s.NAME_DVT_QD, s.NAME_NHANVIEN } into s
                                         select new v_PhieuGioaHang_InTheoGroup
                                         {
                                             NAME_GROUP = s.Key.NAME_NHANVIEN,
                                             MAPHIEUXUAT = s.Key.NAME_KHUVUC,
                                             MA_HANGHOA = s.Key.MA,
                                             NAME_HANGHOA = s.Key.NAME,
                                             NAME_DVT = s.Key.NAME_DVT,
                                             CHIETKHAU = s.Sum((v_ct_PhieuDatHang_ChiTiet_BaoCao x) => Math.Round(x.CHIETKHAU, 0)),
                                             TONGTIENGIAMGIA = s.Sum((v_ct_PhieuDatHang_ChiTiet_BaoCao x) => Math.Round(x.TONGTIENGIAMGIA, 0)),
                                             THANHTIEN = s.Sum((v_ct_PhieuDatHang_ChiTiet_BaoCao x) => Math.Round(x.THANHTIEN, 0)),
                                             THUESUAT = s.Sum((v_ct_PhieuDatHang_ChiTiet_BaoCao x) => Math.Round(x.THUESUAT, 0)),
                                             TONGTIENVAT = s.Sum((v_ct_PhieuDatHang_ChiTiet_BaoCao x) => Math.Round(x.TONGTIENVAT, 0)),
                                             TONGCONG = s.Sum((v_ct_PhieuDatHang_ChiTiet_BaoCao x) => Math.Round(x.TONGCONG, 0)),
                                             TONGSOLUONG = Convert.ToDecimal(s.Sum((v_ct_PhieuDatHang_ChiTiet_BaoCao x) => Math.Round(x.TONGSOLUONG, 0))),
                                             TONGTRONGLUONG = Convert.ToDecimal(s.Key.TRONGLUONG * s.Sum((v_ct_PhieuDatHang_ChiTiet_BaoCao x) => Math.Round(x.TONGSOLUONG, 0))),
                                             NAME_DVT_QD = s.Key.NAME_DVT_QD,
                                             TYLE_QD = s.Key.TYLE_QD
                                         }).ToList();
                            }
                            else if (objParameter.HINHTHUC == 3)
                            {
                                reportClass.DataDefinition.FormulaFields["TIEUDE"].Text = "'BÁO CÁO ĐẶT HÀNG THEO KHÁCH HÀNG'";
                                list3 = (from s in list2
                                         group s by new { s.NAME_KHUVUC, s.TYLE_QD, s.TRONGLUONG, s.NAME, s.MA, s.NAME_DVT, s.NAME_DVT_QD, s.NAME_KHACHHANG, s.TEL_KHACHHANG } into s
                                         select new v_PhieuGioaHang_InTheoGroup
                                         {
                                             NAME_GROUP = s.Key.NAME_KHUVUC,
                                             MAPHIEUXUAT = s.Key.NAME_KHACHHANG + (string.IsNullOrEmpty(s.Key.TEL_KHACHHANG) ? "" : (Environment.NewLine + "Điện thoại: ")) + s.Key.TEL_KHACHHANG,
                                             MA_HANGHOA = s.Key.MA,
                                             NAME_HANGHOA = s.Key.NAME,
                                             NAME_DVT = s.Key.NAME_DVT,
                                             CHIETKHAU = s.Sum((v_ct_PhieuDatHang_ChiTiet_BaoCao x) => Math.Round(x.CHIETKHAU, 0)),
                                             TONGTIENGIAMGIA = s.Sum((v_ct_PhieuDatHang_ChiTiet_BaoCao x) => Math.Round(x.TONGTIENGIAMGIA, 0)),
                                             THANHTIEN = s.Sum((v_ct_PhieuDatHang_ChiTiet_BaoCao x) => Math.Round(x.THANHTIEN, 0)),
                                             THUESUAT = s.Sum((v_ct_PhieuDatHang_ChiTiet_BaoCao x) => Math.Round(x.THUESUAT, 0)),
                                             TONGTIENVAT = s.Sum((v_ct_PhieuDatHang_ChiTiet_BaoCao x) => Math.Round(x.TONGTIENVAT, 0)),
                                             TONGCONG = s.Sum((v_ct_PhieuDatHang_ChiTiet_BaoCao x) => Math.Round(x.TONGCONG, 0)),
                                             TONGSOLUONG = Convert.ToDecimal(s.Sum((v_ct_PhieuDatHang_ChiTiet_BaoCao x) => Math.Round(x.TONGSOLUONG, 0))),
                                             TONGTRONGLUONG = Convert.ToDecimal(s.Key.TRONGLUONG * s.Sum((v_ct_PhieuDatHang_ChiTiet_BaoCao x) => Math.Round(x.TONGSOLUONG, 0))),
                                             NAME_DVT_QD = s.Key.NAME_DVT_QD,
                                             TYLE_QD = s.Key.TYLE_QD
                                         }).ToList();
                            }
                        }
                        dataTable = Utility.ToDataTable(list3);
                        reportClass.DataDefinition.FormulaFields["TONGCONG"].Text = "'" + list3.Sum((v_PhieuGioaHang_InTheoGroup s) => s.TONGCONG).ToString("N0") + "'";
                        reportClass.DataDefinition.FormulaFields["TONGTRONGLUONG"].Text = "'" + list3.Sum((v_PhieuGioaHang_InTheoGroup s) => s.TONGTRONGLUONG / 1000m).ToString("N0") + "'";
                        reportClass.DataDefinition.FormulaFields["TONGSODONHANG"].Text = "'" + num.ToString("N0") + "'";
                        if (objParameter.ID_KHUVUC != null && list3.Count > 0)
                        {
                            reportClass.DataDefinition.FormulaFields["MAPHIEU"].Text = "'" + list3.FirstOrDefault().NAME_GROUP + "'";
                        }
                        else
                        {
                            reportClass.DataDefinition.FormulaFields["MAPHIEU"].Text = "'Tất cả khu vực'";
                        }
                        break;
                    }
                case "~/Report/rptBaoCaoTaiChinh.rpt":
                    {
                        List<Sp_Get_BaoCaoTaiChinh_Result> list7 = new List<Sp_Get_BaoCaoTaiChinh_Result>();
                        apiResponse = Utility.ExecuteStoredProcT<Sp_Get_BaoCaoTaiChinh_Result>(objParameter, "Sp_Get_BaoCaoTaiChinh");
                        List<Sp_Get_BaoCaoTaiChinh_Result> list8 = apiResponse.Data as List<Sp_Get_BaoCaoTaiChinh_Result>;
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
                        reportClass.DataDefinition.FormulaFields["TIEUDE"].Text = "'BÁO CÁO TÀI CHÍNH'";
                        if (objParameter.ID_KHUVUC != null && list8.Count > 0)
                        {
                            reportClass.DataDefinition.FormulaFields["MAPHIEU"].Text = "'" + list8.FirstOrDefault().NAME_KHUVUC + "'";
                        }
                        else
                        {
                            reportClass.DataDefinition.FormulaFields["MAPHIEU"].Text = "'Tất cả khu vực'";
                        }
                        if (objParameter.HINHTHUC_BAOCAOTAICHINH == 1)
                        {
                            list7 = (from s in list8
                                     group s by new { s.NAME_KHUVUC, s.ID_KHUVUC, s.NGAYLAP, s.NGAYLAP_TEXT } into s
                                     select new Sp_Get_BaoCaoTaiChinh_Result
                                     {
                                         ID_KHUVUC = s.Key.ID_KHUVUC,
                                         NAME_KHUVUC = s.Key.NAME_KHUVUC,
                                         NGAYLAP = s.Key.NGAYLAP,
                                         NGAYLAP_TEXT = s.Key.NGAYLAP_TEXT,
                                         SOLUONG_DONHANG = s.Sum((Sp_Get_BaoCaoTaiChinh_Result x) => Math.Round(x.SOLUONG_DONHANG, 0)),
                                         TONGTIENGIAMGIA = s.Sum((Sp_Get_BaoCaoTaiChinh_Result x) => Math.Round(x.TONGTIENGIAMGIA, 0)),
                                         TONGTHANHTIEN = s.Sum((Sp_Get_BaoCaoTaiChinh_Result x) => Math.Round(x.TONGTHANHTIEN, 0)),
                                         TONGTIENVAT = s.Sum((Sp_Get_BaoCaoTaiChinh_Result x) => Math.Round(x.TONGTIENVAT, 0)),
                                         TONGCONG = s.Sum((Sp_Get_BaoCaoTaiChinh_Result x) => Math.Round(x.TONGCONG, 0)),
                                         TONGCONG_HANGROT = s.Sum((Sp_Get_BaoCaoTaiChinh_Result x) => Math.Round(x.TONGCONG_HANGROT, 0)),
                                         SOLUONG_DATHU = s.Sum((Sp_Get_BaoCaoTaiChinh_Result x) => Math.Round(x.SOLUONG_DATHU, 0)),
                                         TONGCONG_DATHU = s.Sum((Sp_Get_BaoCaoTaiChinh_Result x) => Math.Round(x.TONGCONG_DATHU, 0)),
                                         SOLUONG_THUNO = s.Sum((Sp_Get_BaoCaoTaiChinh_Result x) => Math.Round(x.SOLUONG_THUNO, 0)),
                                         TONGCONG_THUNO = s.Sum((Sp_Get_BaoCaoTaiChinh_Result x) => Math.Round(x.TONGCONG_THUNO, 0)),
                                         THUKHAC = s.Sum((Sp_Get_BaoCaoTaiChinh_Result x) => Math.Round(x.THUKHAC, 0)),
                                         CHIKHAC = s.Sum((Sp_Get_BaoCaoTaiChinh_Result x) => Math.Round(x.CHIKHAC, 0))
                                     }).ToList();
                        }
                        else if (objParameter.HINHTHUC_BAOCAOTAICHINH == 2)
                        {
                            list7 = (from s in list8
                                     group s by new { s.NAME_KHUVUC, s.ID_KHUVUC, s.NGAYLAP, s.NGAYLAP_TEXT, s.ID, s.MA, s.NAME, s.TEL, s.ADDRESS } into s
                                     select new Sp_Get_BaoCaoTaiChinh_Result
                                     {
                                         ID_KHUVUC = s.Key.ID_KHUVUC,
                                         NAME_KHUVUC = s.Key.NAME_KHUVUC,
                                         NGAYLAP = s.Key.NGAYLAP,
                                         NGAYLAP_TEXT = s.Key.NGAYLAP_TEXT,
                                         ID = s.Key.ID,
                                         MA = s.Key.MA,
                                         NAME = s.Key.NAME,
                                         TEL = s.Key.TEL,
                                         ADDRESS = s.Key.ADDRESS,
                                         SOLUONG_DONHANG = s.Sum((Sp_Get_BaoCaoTaiChinh_Result x) => Math.Round(x.SOLUONG_DONHANG, 0)),
                                         TONGTIENGIAMGIA = s.Sum((Sp_Get_BaoCaoTaiChinh_Result x) => Math.Round(x.TONGTIENGIAMGIA, 0)),
                                         TONGTHANHTIEN = s.Sum((Sp_Get_BaoCaoTaiChinh_Result x) => Math.Round(x.TONGTHANHTIEN, 0)),
                                         TONGTIENVAT = s.Sum((Sp_Get_BaoCaoTaiChinh_Result x) => Math.Round(x.TONGTIENVAT, 0)),
                                         TONGCONG = s.Sum((Sp_Get_BaoCaoTaiChinh_Result x) => Math.Round(x.TONGCONG, 0)),
                                         TONGCONG_HANGROT = s.Sum((Sp_Get_BaoCaoTaiChinh_Result x) => Math.Round(x.TONGCONG_HANGROT, 0)),
                                         SOLUONG_DATHU = s.Sum((Sp_Get_BaoCaoTaiChinh_Result x) => Math.Round(x.SOLUONG_DATHU, 0)),
                                         TONGCONG_DATHU = s.Sum((Sp_Get_BaoCaoTaiChinh_Result x) => Math.Round(x.TONGCONG_DATHU, 0)),
                                         SOLUONG_THUNO = s.Sum((Sp_Get_BaoCaoTaiChinh_Result x) => Math.Round(x.SOLUONG_THUNO, 0)),
                                         TONGCONG_THUNO = s.Sum((Sp_Get_BaoCaoTaiChinh_Result x) => Math.Round(x.TONGCONG_THUNO, 0)),
                                         THUKHAC = s.Sum((Sp_Get_BaoCaoTaiChinh_Result x) => Math.Round(x.THUKHAC, 0)),
                                         CHIKHAC = s.Sum((Sp_Get_BaoCaoTaiChinh_Result x) => Math.Round(x.CHIKHAC, 0))
                                     }).ToList();
                        }
                        dataTable = Utility.ToDataTable(list7);
                        break;
                    }
                case "~/Report/rptDanhSachPhieuXuatHang_KhuyenMai.rpt":
                    {
                        apiResponse = Utility.ExecuteStoredProcT<Sp_Get_DanhSachPhieuXuatHang_KhuyenMai>(objParameter, "Sp_Get_DanhSachPhieuXuatHang_KhuyenMai");
                        List<Sp_Get_DanhSachPhieuXuatHang_KhuyenMai> list4 = apiResponse.Data as List<Sp_Get_DanhSachPhieuXuatHang_KhuyenMai>;
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
                        List<v_PhieuXuatHangKhuyenMai_InTheoGroup> list5 = new List<v_PhieuXuatHangKhuyenMai_InTheoGroup>();
                        if (list4 != null)
                        {
                            if (objParameter.HINHTHUC_PHIEUXUATHANG_KHUYENMAI == 1)
                            {
                                reportClass.DataDefinition.FormulaFields["TIEUDE"].Text = "'BÁO CÁO XUẤT HÀNG KHUYẾN MÃI THEO CHƯƠNG TRÌNH KHUYẾN MÃI'";
                                list5 = (from s in list4
                                         group s by new { s.MA_CHUONGTRINHKHUYENMAI, s.NAME_CHUONGTRINHKHUYENMAI, s.TYLE_QD, s.NAME_HANGHOA, s.MA_HANGHOA, s.NAME_DVT, s.NAME_DVT_QD } into s
                                         select new v_PhieuXuatHangKhuyenMai_InTheoGroup
                                         {
                                             NAME_GROUP = s.Key.MA_CHUONGTRINHKHUYENMAI + "-" + s.Key.NAME_CHUONGTRINHKHUYENMAI,
                                             MA_HANGHOA = s.Key.MA_HANGHOA,
                                             NAME_HANGHOA = s.Key.NAME_HANGHOA,
                                             TONGSOLUONG = Convert.ToDecimal(s.Sum((Sp_Get_DanhSachPhieuXuatHang_KhuyenMai x) => Math.Round(x.ISKHUYENMAI ? x.TONGSOLUONG : 0.0, 0))),
                                             TONGTIENGIAMGIA = Convert.ToDecimal(s.Sum((Sp_Get_DanhSachPhieuXuatHang_KhuyenMai x) => Math.Round(x.TONGTIENGIAMGIA, 0))),
                                             TONGSOLUONG_TRAHANG = Convert.ToDecimal(s.Sum((Sp_Get_DanhSachPhieuXuatHang_KhuyenMai x) => Math.Round(x.ISKHUYENMAI ? x.TONGSOLUONG_TRAHANG : 0.0, 0))),
                                             TONGTIENGIAMGIA_TRAHANG = Convert.ToDecimal(s.Sum((Sp_Get_DanhSachPhieuXuatHang_KhuyenMai x) => Math.Round(x.TONGTIENGIAMGIA_TRAHANG, 0))),
                                             NAME_DVT = s.Key.NAME_DVT,
                                             NAME_DVT_QD = s.Key.NAME_DVT_QD,
                                             TYLE_QD = s.Key.TYLE_QD
                                         }).ToList();
                            }
                            else if (objParameter.HINHTHUC_PHIEUXUATHANG_KHUYENMAI == 2)
                            {
                                reportClass.DataDefinition.FormulaFields["TIEUDE"].Text = "'BÁO CÁO XUẤT HÀNG KHUYẾN MÃI THEO NHÓM HÀNG HÓA'";
                                list5 = (from s in list4
                                         group s by new { s.MA_NHOMHANGHOA, s.NAME_NHOMHANGHOA, s.TYLE_QD, s.NAME_HANGHOA, s.MA_HANGHOA, s.NAME_DVT, s.NAME_DVT_QD } into s
                                         select new v_PhieuXuatHangKhuyenMai_InTheoGroup
                                         {
                                             NAME_GROUP = s.Key.MA_NHOMHANGHOA + "-" + s.Key.NAME_NHOMHANGHOA,
                                             MA_HANGHOA = s.Key.MA_HANGHOA,
                                             NAME_HANGHOA = s.Key.NAME_HANGHOA,
                                             TONGSOLUONG = Convert.ToDecimal(s.Sum((Sp_Get_DanhSachPhieuXuatHang_KhuyenMai x) => Math.Round(x.ISKHUYENMAI ? x.TONGSOLUONG : 0.0, 0))),
                                             TONGTIENGIAMGIA = Convert.ToDecimal(s.Sum((Sp_Get_DanhSachPhieuXuatHang_KhuyenMai x) => Math.Round(x.TONGTIENGIAMGIA, 0))),
                                             TONGSOLUONG_TRAHANG = Convert.ToDecimal(s.Sum((Sp_Get_DanhSachPhieuXuatHang_KhuyenMai x) => Math.Round(x.ISKHUYENMAI ? x.TONGSOLUONG_TRAHANG : 0.0, 0))),
                                             TONGTIENGIAMGIA_TRAHANG = Convert.ToDecimal(s.Sum((Sp_Get_DanhSachPhieuXuatHang_KhuyenMai x) => Math.Round(x.TONGTIENGIAMGIA_TRAHANG, 0))),
                                             NAME_DVT = s.Key.NAME_DVT,
                                             NAME_DVT_QD = s.Key.NAME_DVT_QD,
                                             TYLE_QD = s.Key.TYLE_QD
                                         }).ToList();
                            }
                            else if (objParameter.HINHTHUC_PHIEUXUATHANG_KHUYENMAI == 3)
                            {
                                reportClass.DataDefinition.FormulaFields["TIEUDE"].Text = "'BÁO CÁO XUẤT HÀNG KHUYẾN MÃI THEO KHU VỰC'";
                                list5 = (from s in list4
                                         group s by new { s.MA_KHUVUC, s.NAME_KHUVUC, s.TYLE_QD, s.NAME_HANGHOA, s.MA_HANGHOA, s.NAME_DVT, s.NAME_DVT_QD } into s
                                         select new v_PhieuXuatHangKhuyenMai_InTheoGroup
                                         {
                                             NAME_GROUP = s.Key.MA_KHUVUC + "-" + s.Key.NAME_KHUVUC,
                                             MA_HANGHOA = s.Key.MA_HANGHOA,
                                             NAME_HANGHOA = s.Key.NAME_HANGHOA,
                                             TONGSOLUONG = Convert.ToDecimal(s.Sum((Sp_Get_DanhSachPhieuXuatHang_KhuyenMai x) => Math.Round(x.ISKHUYENMAI ? x.TONGSOLUONG : 0.0, 0))),
                                             TONGTIENGIAMGIA = Convert.ToDecimal(s.Sum((Sp_Get_DanhSachPhieuXuatHang_KhuyenMai x) => Math.Round(x.TONGTIENGIAMGIA, 0))),
                                             TONGSOLUONG_TRAHANG = Convert.ToDecimal(s.Sum((Sp_Get_DanhSachPhieuXuatHang_KhuyenMai x) => Math.Round(x.ISKHUYENMAI ? x.TONGSOLUONG_TRAHANG : 0.0, 0))),
                                             TONGTIENGIAMGIA_TRAHANG = Convert.ToDecimal(s.Sum((Sp_Get_DanhSachPhieuXuatHang_KhuyenMai x) => Math.Round(x.TONGTIENGIAMGIA_TRAHANG, 0))),
                                             NAME_DVT = s.Key.NAME_DVT,
                                             NAME_DVT_QD = s.Key.NAME_DVT_QD,
                                             TYLE_QD = s.Key.TYLE_QD
                                         }).ToList();
                            }
                        }
                        dataTable = Utility.ToDataTable(list5);
                        break;
                    }
                case "~/Report/rptDanhSachPhieuGiaoHangNhanVien_KPI.rpt":
                    {
                        apiResponse = Utility.ExecuteStoredProcT<Sp_Get_DanhSachPhieuGiaoHang_KPI_Result>(objParameter, "Sp_Get_DanhSachPhieuGiaoHang_KPI");
                        List<Sp_Get_DanhSachPhieuGiaoHang_KPI_Result> list9 = apiResponse.Data as List<Sp_Get_DanhSachPhieuGiaoHang_KPI_Result>;
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
                        List<DanhSachPhieuGiaoHangNhanVien_KPI> list10 = new List<DanhSachPhieuGiaoHangNhanVien_KPI>();
                        if (list9 != null)
                        {
                            list10 = (from s in list9
                                      group s by new { s.NAME_PHONGBAN, s.NAME_NHANVIEN, s.MA_PHONGBAN, s.MA_NHANVIEN } into s
                                      select new DanhSachPhieuGiaoHangNhanVien_KPI
                                      {
                                          NAME_GROUP = s.Key.NAME_PHONGBAN,
                                          NAME_NHANVIEN = s.Key.NAME_NHANVIEN,
                                          MA_NHANVIEN = s.Key.MA_NHANVIEN,
                                          SOLUONG_DONHANG = Convert.ToDecimal(s.Select((Sp_Get_DanhSachPhieuGiaoHang_KPI_Result x) => x.ID_PHIEUXUAT).Count()),
                                          SOLUONG_GIAOHANG = Convert.ToDecimal(s.Count((Sp_Get_DanhSachPhieuGiaoHang_KPI_Result x) => x.ISDAGIAOHANG)),
                                          SOLUONG_TRAHANG = Convert.ToDecimal(s.Count((Sp_Get_DanhSachPhieuGiaoHang_KPI_Result x) => x.ISTRAHANG)),
                                          TONGTIEN = Convert.ToDecimal(s.Sum((Sp_Get_DanhSachPhieuGiaoHang_KPI_Result x) => Math.Round(x.TONGTHANHTIEN, 0)))
                                      }).ToList();
                        }
                        dataTable = Utility.ToDataTable(list10);
                        if (objParameter.ID_PHONGBAN != null && list10.Count > 0)
                        {
                            reportClass.DataDefinition.FormulaFields["MAPHIEU"].Text = "'" + list10.FirstOrDefault().NAME_GROUP + "'";
                        }
                        else
                        {
                            reportClass.DataDefinition.FormulaFields["MAPHIEU"].Text = "'Tất cả phòng ban'";
                        }
                        break;
                    }
                case "~/Report/rpt_DanhSachHangHoa_NV_KH.rpt":
                    {
                        apiResponse = Utility.ExecuteStoredProcT<Sp_Get_DanhSachHangHoa_Result>(objParameter, objParameter.NAME_SP);
                        List<Sp_Get_DanhSachHangHoa_Result> list6 = apiResponse.Data as List<Sp_Get_DanhSachHangHoa_Result>;
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
                        if (list6 == null)
                        {
                            list6 = new List<Sp_Get_DanhSachHangHoa_Result>();
                        }
                        dataTable = Utility.ToDataTable(list6);
                        break;
                    }
                case "~/Report/rpt_DanhSachHangHoa_BanChay.rpt":
                    {
                        apiResponse = Utility.ExecuteStoredProcT<Sp_Get_DanhSachHangHoa_Result>(objParameter, objParameter.NAME_SP);
                        List<Sp_Get_DanhSachHangHoa_Result> list6 = apiResponse.Data as List<Sp_Get_DanhSachHangHoa_Result>;
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
                        if (list6 == null)
                        {
                            list6 = new List<Sp_Get_DanhSachHangHoa_Result>();
                        }
                        dataTable = Utility.ToDataTable(list6);
                        break;
                    }
                case "~/Report/rpt_DanhSachHangHoa.rpt":
                    {
                        apiResponse = Utility.ExecuteStoredProcT<Sp_Get_DanhSachHangHoa>(objParameter, objParameter.NAME_SP);
                        List<Sp_Get_DanhSachHangHoa> list = apiResponse.Data as List<Sp_Get_DanhSachHangHoa>;
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
                        if (list == null)
                        {
                            list = new List<Sp_Get_DanhSachHangHoa>();
                        }
                        dataTable = Utility.ToDataTable(list);
                        break;
                    }
            }
            reportClass.SetDatabaseLogon("test", "test!", "test", "test");
            reportClass.SetDataSource(dataTable);
            base.Response.Buffer = false;
            base.Response.ClearContent();
            base.Response.ClearHeaders();
            Stream stream = reportClass.ExportToStream(ExportFormatType.PortableDocFormat);
            Utility.Report = reportClass;
            return Json(objParameter.ID_REPORT, JsonRequestBehavior.AllowGet);
        }
        catch (Exception ex)
        {
            Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
            base.TempData["TitleError"] = "Đã có lỗi phát sinh trong phiên làm việc!";
            base.TempData["DetailError"] = ex.Message;
            return RedirectToAction("Index", "Notfound");
        }
    }

    public ActionResult VerReporte()
    {
        if (Utility.KiemTra())
        {
            return RedirectToAction("Index", "Admin");
        }
        if (Utility.Report != null)
        {
            Stream fileStream = Utility.Report.ExportToStream(ExportFormatType.PortableDocFormat);
            return new FileStreamResult(fileStream, "application/pdf");
        }
        base.TempData["TitleError"] = "Không có dữ liệu!";
        return RedirectToAction("Index", "Notfound");
    }

    public FileResult GetReport()
    {
        Stream stream = Utility.Report.ExportToStream(ExportFormatType.PortableDocFormat);
        byte[] fileContents = ReadToEnd(stream);
        return File(fileContents, "application/pdf");
    }

    public ActionResult Download()
    {
        Stream stream = Utility.Report.ExportToStream(ExportFormatType.PortableDocFormat);
        byte[] fileContents = ReadToEnd(stream);
        return File(fileContents, "application/octet-stream", "yourfile.pdf");
    }

    public static byte[] ReadToEnd(Stream stream)
    {
        long position = 0L;
        if (stream.CanSeek)
        {
            position = stream.Position;
            stream.Position = 0L;
        }
        try
        {
            byte[] array = new byte[4096];
            int num = 0;
            int num2;
            while ((num2 = stream.Read(array, num, array.Length - num)) > 0)
            {
                num += num2;
                if (num == array.Length)
                {
                    int num3 = stream.ReadByte();
                    if (num3 != -1)
                    {
                        byte[] array2 = new byte[array.Length * 2];
                        Buffer.BlockCopy(array, 0, array2, 0, array.Length);
                        Buffer.SetByte(array2, num, (byte)num3);
                        array = array2;
                        num++;
                    }
                }
            }
            byte[] array3 = array;
            if (array.Length != num)
            {
                array3 = new byte[num];
                Buffer.BlockCopy(array, 0, array3, 0, num);
            }
            return array3;
        }
        finally
        {
            if (stream.CanSeek)
            {
                stream.Position = position;
            }
        }
    }

    public ActionResult AddTab(string ID)
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
            apiResponse = Utility.GetDetail<v_web_Menu>(ID, "Menu");
            v_web_Menu v_web_Menu2 = new v_web_Menu();
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
                v_web_Menu2 = apiResponse.Data as v_web_Menu;
            }
            if (v_web_Menu2 == null || string.IsNullOrEmpty(v_web_Menu2.ID_REPORT))
            {
                apiResponse.Success = false;
                apiResponse.Message = "Không tìm thấy cấu hình báo cáo " + ((v_web_Menu2 != null) ? v_web_Menu2.NAME : ID) + "!";
                return new JsonResult
                {
                    Data = apiResponse,
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                    MaxJsonLength = int.MaxValue
                };
            }
            string text = "";
            List<ValueEdit> list = new List<ValueEdit>();
            List<ValueEdit> list2 = new List<ValueEdit>();
            v_v_web_Report v_v_web_Report2 = new v_v_web_Report();
            apiResponse = Utility.GetDetail<v_v_web_Report>(v_web_Menu2.ID_REPORT, "Report");
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
                v_v_web_Report2 = apiResponse.Data as v_v_web_Report;
            }
            if (v_v_web_Report2.lstweb_Report_Parameter != null)
            {
                foreach (v_web_Report_Parameter item in from e in v_v_web_Report2.lstweb_Report_Parameter
                                                        where e.ISACTIVE
                                                        orderby e.STT
                                                        select e)
                {
                    if (item.VALUE != null && item.TYPE == API.Input)
                    {
                        text = ((!(item.MA_PARAMETER == "ID_REPORT")) ? ((!(item.MA_PARAMETER == "TUNGAY")) ? ((!(item.MA_PARAMETER == "DENNGAY")) ? (text + item.VALUE.ToString()) : (text + item.VALUE.ToString().Replace("@Value", DateTime.Now.ToString("yyyy-MM-dd")))) : (text + item.VALUE.ToString().Replace("@Value", DateTime.Now.AddMonths(-1).ToString("yyyy-MM-dd")))) : (text + item.VALUE.ToString().Replace("@Value", v_web_Menu2.ID)));
                    }
                    else if (item.VALUE != null && item.TYPE == API.ListValue)
                    {
                        ApiResponse listData = Utility.GetListData<ListValue>(item.VALUE, "", "", Utility.LOC_ID);
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
                        if (listData.Data != null)
                        {
                            list.Add(new ValueEdit
                            {
                                Key = item.MA_PARAMETER,
                                Name = item.NAME,
                                Controller = item.VALUE,
                                Value = (listData.Data as List<ListValue>)
                            });
                        }
                    }
                    else if (item.VALUE != null && item.TYPE == API.Checkbox)
                    {
                        list2.Add(new ValueEdit
                        {
                            Key = item.MA_PARAMETER,
                            Name = item.NAME,
                            Controller = item.VALUE
                        });
                    }
                }
            }
            string tAB = "<li class=\"active\" name=\"" + v_web_Menu2.ID + "\" id=\"tab" + v_web_Menu2.ID + "\"><a href=\"#content" + v_web_Menu2.ID + "\" data-toggle=\"tab\" class=\"dropdown-toggle\" style=\"display:table-cell;\"><strong> " + v_web_Menu2.NAME.ToUpper() + "</strong></a><a href=\"#\" onclick=\"DeleteTab('" + v_web_Menu2.ID + "')\" style=\"display:table-cell;\"><i class=\"glyphicon glyphicon-remove\"></i></a></li>";
            bool flag = false;
            if (v_web_Menu2.CONTROLLERNAME == "Sp_Get_DanhSachPhieuDatHang")
            {
                flag = Utility.KiemTraQuyen("Sp_Get_DanhSachPhieuDatHang", "CreateUser");
                if (Utility.KiemTraQuyenAdmin())
                {
                    flag = false;
                }
            }
            foreach (ValueEdit item2 in list)
            {
                if (flag && item2.Key == "ID_NHANVIEN")
                {
                    text = text + "<input type=\"hidden\" id=\"ID_NHANVIEN\" name=\"ID_NHANVIEN\" value=\"" + base.Session["idUser"].ToString() + "\">";
                    continue;
                }
                text = text + "<div class=\"form-group\"><label class=\"col-sm-2 control-label\" for=\"" + item2.Key + "\">" + item2.Name?.ToString() + "</label><div class=\"col-sm-4\"><div class=\"input-group mb15\">";
                text = text + "<span class=\"input-group-btn\"><button type=\"button\" class=\"btn btn-default\" onclick=\"myFunOpenSearch('content" + v_web_Menu2.ID + "', '" + item2.Controller + "', '" + API.Chon1 + "', '" + item2.Key + "', '')\"><span class=\"glyphicon glyphicon-search\"></span></button></span>";
                text = text + "<select class=\"form-control chosen-select\" data-placeholder=\"" + Utility.GetTitleChon(item2.Controller) + "\" id=\"" + item2.Key + "\" name=\"" + item2.Key + "\">";
                text = text + "<option value=\"\">" + Utility.GetTitleChon(item2.Controller) + "</option>";
                foreach (ListValue item3 in item2.Value as List<ListValue>)
                {
                    if (!item3.ISACTIVE.HasValue || item3.ISACTIVE == true)
                    {
                        text = text + "<option value=\"" + item3.ID + "\">" + item3.NAME?.ToString() + "</option>";
                    }
                }
                text += "</select>";
                text += "</div></div></div>";
            }
            foreach (ValueEdit item4 in list2)
            {
                text = text + "<div class=\"form-group\"><label class=\"col-sm-2 control-label\" for=\"" + item4.Key + "\">" + item4.Name?.ToString() + "</label><div class=\"col-sm-4\">";
                text += item4.Controller;
                text += "</div></div>";
            }
            apiResponse.ID = ID;
            apiResponse.TAB = tAB;
            apiResponse.CONTENT = text;
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
}
