using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using DatabaseTHP;
using DatabaseTHP.Class;
using DatabaseTHP.StoredProcedure;
using DatabaseTHP.StoredProcedure.Parameter;
using MVC_QuanLyTHP.Class;
using MVC_QuanLyTHP.Models;
using MVC_QuanLyTHP.Report;
using Syncfusion.EJ2.Spreadsheet;
using static System.Net.Mime.MediaTypeNames;
namespace MVC_QuanLyTHP.Controllers.NghiepVu
{
    public class ViewReportController : Controller
    {
        // GET: Report
        public ActionResult Index()
        {
            if (Utility.KiemTra())
            {
                return RedirectToAction("Index", "Admin");
            }
            //if (!Utility.KiemTraQuyen(API.BaoCao, API.Xem))
            //{
            //    TempData["TitleError"] = API.TitlePermission;
            //    return RedirectToAction("Index", "Notfound");
            //}
            v_v_web_Report web_Report = new v_v_web_Report();
            string Tree = "";
            var lstMenu = Utility.GetMenu();
            if (lstMenu != null && lstMenu.Count > 0)
            {
                var objReport = lstMenu.FirstOrDefault(e => e.CONTROLLERNAME == API.BaoCao && e.ISACTIVE);
                if (objReport != null)
                {
                    Tree = "<ul id=\"treeview\">";
                    var lstBaoCao = lstMenu.Where(e => e.ID_QUYENCHA == objReport.ID && e.ISACTIVE).OrderBy(e => e.STT).ToList();
                    if (lstBaoCao != null && lstBaoCao.Count() > 0)
                    {
                        foreach (var itm in lstBaoCao)
                        {
                            if(Utility.KiemTraQuyen(itm.CONTROLLERNAME, API.Xem))
                            {
                                Tree += GetMenuReport(lstMenu, itm);
                            }
                        }
                    }
                    Tree += " </ul>";
                }
            }
            ViewBag.ViewMenu = Tree;
            web_Report.lstValue = new List<ListValue>();
            //web_Report.lstValue.Add(new ListValue { ID = "1", NAME = "aaaa" });
            return View(web_Report);
        }

        private string GetMenuReport(List<v_web_Menu> lstMenu, v_web_Menu MenuCha)
        {
            string Tree = "<li data-icon-cls=\"" + MenuCha.ICON + "\">" + MenuCha.NAME + (!string.IsNullOrEmpty(MenuCha.CONTROLLERNAME) ? "<a style=\"margin-left:20px\" data-toggle=\"tooltip\" data-placement=\"top\" title=\"" + Utility.ThemTab + "!\" onclick=\"AddTab('" + MenuCha.ID + "')\"><i class=\"fa fa-plus-square\"></i></a>" : "");
            var lstBaoCao = lstMenu.Where(e => e.ID_QUYENCHA == MenuCha.ID).ToList();
            if (lstBaoCao != null && lstBaoCao.Count() > 0)
            {
                Tree += "<ul>";
                foreach (var itm in lstBaoCao)
                {
                    Tree += GetMenuReport(lstMenu, itm);
                }
                Tree += "</ul>";
            }
            Tree += "</li>";
            return Tree;
        }

        [HttpPost, ValidateInput(false)]
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
                    var lstMenu = Utility.GetMenu();
                    var Menu = lstMenu.FirstOrDefault(e => e.CONTROLLERNAME == objParameter.CONTROLLER && e.ISACTIVE);
                    if (Menu != null)
                        objParameter.ID_REPORT = Menu.ID_REPORT;
                }

                if (string.IsNullOrEmpty(objParameter.ID_REPORT))
                {
                    apiResponse.Success = false;
                    apiResponse.Message = "Không tìm thấy cấu hình báo cáo " + (objParameter.ID_REPORT) + "!";
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }

                apiResponse = Utility.GetDetail<v_web_Menu>(objParameter.ID_REPORT, API.web_Menu);
                v_web_Menu web_Menu = new v_web_Menu();

                if (!apiResponse.Success)
                {
                    apiResponse.Success = false;
                    apiResponse.Message = apiResponse.Message;
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }

                if (apiResponse.Data != null)
                    web_Menu = apiResponse.Data as v_web_Menu;

                if (web_Menu == null)
                {
                    apiResponse.Success = false;
                    apiResponse.Message = "Không tìm thấy cấu hình báo cáo " + (web_Menu != null ? web_Menu.NAME : objParameter.ID_REPORT) + "!";
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }

                v_v_web_Report web_Report = new v_v_web_Report();
                apiResponse = Utility.GetDetail<v_v_web_Report>(web_Menu.ID_REPORT, API.web_Report);
                if (!apiResponse.Success)
                {
                    TempData["TitleError"] = apiResponse.Message;
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                if (apiResponse.Data != null)
                    web_Report = apiResponse.Data as v_v_web_Report;

                //byte[] BinaryData = System.Text.Encoding.UTF8.GetBytes("https://ironsoftware.com/csharp/barcode/");
                // WRITE QR with Binary Content
                String fullpath = Path.Combine(Server.MapPath("~" + API.PathProduct), "MyBinaryQR.png");
                String fullpathLogo = Path.Combine(Server.MapPath("~" + API.PathLogo), "logoTrangHiepPhat.jpg");

                v_v_dm_CongTy dm_CongTy = new v_v_dm_CongTy();
                apiResponse = Utility.GetDetail<v_v_dm_CongTy>(Utility.LOC_ID, API.dm_CongTy);
                if (apiResponse.Data != null)
                    dm_CongTy = apiResponse.Data as v_v_dm_CongTy;

                objParameter.NAME_SP = web_Report.NAME_SP;
                objParameter.LOC_ID = Utility.LOC_ID;
                var report = new ReportClass();
                string fileReport = web_Report.REPORT;
                if (fileReport == "~/Report/rptBaoCaoTaiChinh.rpt" && objParameter.HINHTHUC_BAOCAOTAICHINH == 2)
                {
                    fileReport = "~/Report/rptBaoCaoTaiChinh_KhanhHang.rpt";
                }
                report.FileName = Server.MapPath(fileReport);
                DataTable data = new DataTable();
                if (objParameter.HINHTHUC_PHIEUXUATHANG_KHUYENMAI == null && objParameter.HINHTHUC_BAOCAOTAICHINH == null && objParameter.HINHTHUC == null)
                {
                    apiResponse = Utility.ExecuteStoredProc<DataTable>(objParameter, API.SP_GetReport);
                    if (!apiResponse.Success)
                    {
                        TempData["TitleError"] = apiResponse.Message;
                        return RedirectToAction("Index", "Notfound");
                    }

                    data = (apiResponse.Data as DataTable);
                    if (apiResponse.CheckValue)
                        data.Rows.Clear();
                }


                //foreach (DataRow itm in data.Rows)
                //{
                //    itm["ID_LOAIPHIEUNHAP"] = "https://phamthihuonghuyen.click/Delivery/CheckData?ID=894f8dfa-4f35-4b81-b1b2-9c17db12fe14";
                //}

                report.Load();
                report.DataDefinition.FormulaFields["TENCONGTY"].Text = "'" + dm_CongTy.NAME + "'";
                report.DataDefinition.FormulaFields["DIACHI"].Text = "'" + dm_CongTy.ADDRESS + "'";
                report.DataDefinition.FormulaFields["DIENTHOAI"].Text = "'" + dm_CongTy.TEL + "'";
                report.DataDefinition.FormulaFields["ICON"].Text = "'" + fullpathLogo + "'";
                //report.DataDefinition.FormulaFields["QRCode"].Text = "'" + fullpath + "'";
                switch (web_Report.REPORT)
                {
                    #region Phiếu đặt hàng
                    case "~/Report/rptBaoCaoPhieuDatHang.rpt":
                        string NAME = web_Report.NAME_SP == API.Sp_Get_DanhSachPhieuNhap_ChiTiet_BaoCao || web_Report.NAME_SP == API.Sp_Get_DanhSachPhieuNhapTraHang_ChiTiet_BaoCao ? web_Report.NAME_SP == API.Sp_Get_DanhSachPhieuNhapTraHang_ChiTiet_BaoCao ? API.Sp_Get_DanhSachPhieuNhapTraHang_ChiTiet_BaoCao : API.Sp_Get_DanhSachPhieuNhap_ChiTiet_BaoCao : API.Sp_Get_BaoCaoPhieuDatHang;
                        apiResponse = Utility.ExecuteStoredProcT<v_ct_PhieuDatHang_ChiTiet_BaoCao>(objParameter, NAME);
                        List<v_ct_PhieuDatHang_ChiTiet_BaoCao> lstSp_Get_DanhSachPhieuGiaoHang_In = (apiResponse.Data as List<v_ct_PhieuDatHang_ChiTiet_BaoCao>);
                        if (!apiResponse.Success)
                        {
                            apiResponse.Success = false;
                            apiResponse.Message = apiResponse.Message;
                            return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                        }
                        int TONGSODONHANG = 0;
                        List<v_PhieuGioaHang_InTheoGroup> lstv_PhieuGioaHang_InTheoGroup = new List<v_PhieuGioaHang_InTheoGroup>();
                        if (lstSp_Get_DanhSachPhieuGiaoHang_In != null)
                        {
                            TONGSODONHANG = lstSp_Get_DanhSachPhieuGiaoHang_In.GroupBy(s => new { s.MAPHIEU }).Count();
                            if (web_Report.NAME_SP == "Sp_Get_DanhSachPhieuNhap_ChiTiet_BaoCao")
                            {
                                if (objParameter.HINHTHUC == 1)
                                {
                                    report.DataDefinition.FormulaFields["TIEUDE"].Text = "'" + "BÁO CÁO NHẬP HÀNG THEO NHÓM HÀNG" + "'";
                                    lstv_PhieuGioaHang_InTheoGroup = lstSp_Get_DanhSachPhieuGiaoHang_In.GroupBy(s => new { s.NAME_KHUVUC, s.TYLE_QD, s.TRONGLUONG, s.NAME, s.MA, s.NAME_DVT, s.NAME_DVT_QD, s.NAME_NHOMHANGHOA })
                                                            .Select(s => new v_PhieuGioaHang_InTheoGroup
                                                            {
                                                                NAME_GROUP = s.Key.NAME_NHOMHANGHOA,
                                                                MAPHIEUXUAT = "",
                                                                MA_HANGHOA = s.Key.MA,
                                                                NAME_HANGHOA = s.Key.NAME,
                                                                NAME_DVT = s.Key.NAME_DVT,
                                                                CHIETKHAU = s.Sum(x => Math.Round(x.CHIETKHAU, 0)),
                                                                TONGTIENGIAMGIA = s.Sum(x => Math.Round(x.TONGTIENGIAMGIA, 0)),
                                                                THANHTIEN = s.Sum(x => Math.Round(x.THANHTIEN, 0)),
                                                                THUESUAT = s.Sum(x => Math.Round(x.THUESUAT, 0)),
                                                                TONGTIENVAT = s.Sum(x => Math.Round(x.TONGTIENVAT, 0)),
                                                                TONGCONG = s.Sum(x => Math.Round(x.TONGCONG, 0)),
                                                                TONGSOLUONG = Convert.ToDecimal(s.Sum(x => Math.Round(x.TONGSOLUONG, 0))),
                                                                TONGTRONGLUONG = Convert.ToDecimal(s.Key.TRONGLUONG * s.Sum(x => Math.Round(x.TONGSOLUONG, 0))),
                                                                NAME_DVT_QD = s.Key.NAME_DVT_QD,
                                                                TYLE_QD = s.Key.TYLE_QD
                                                            }).ToList();
                                }
                                else if (objParameter.HINHTHUC == 3)
                                {
                                    report.DataDefinition.FormulaFields["TIEUDE"].Text = "'" + "BÁO CÁO NHẬP HÀNG THEO NHÀ CUNG CẤP" + "'";
                                    lstv_PhieuGioaHang_InTheoGroup = lstSp_Get_DanhSachPhieuGiaoHang_In.GroupBy(s => new { s.NAME_KHUVUC, s.TYLE_QD, s.TRONGLUONG, s.NAME, s.MA, s.NAME_DVT, s.NAME_DVT_QD, s.NAME_KHACHHANG, s.TEL_KHACHHANG })
                                                            .Select(s => new v_PhieuGioaHang_InTheoGroup
                                                            {
                                                                NAME_GROUP = s.Key.NAME_KHACHHANG + (string.IsNullOrEmpty(s.Key.TEL_KHACHHANG) ? "" : Environment.NewLine + "Điện thoại: ") + s.Key.TEL_KHACHHANG,
                                                                MAPHIEUXUAT = s.Key.NAME_KHUVUC,
                                                                MA_HANGHOA = s.Key.MA,
                                                                NAME_HANGHOA = s.Key.NAME,
                                                                NAME_DVT = s.Key.NAME_DVT,
                                                                CHIETKHAU = s.Sum(x => Math.Round(x.CHIETKHAU, 0)),
                                                                TONGTIENGIAMGIA = s.Sum(x => Math.Round(x.TONGTIENGIAMGIA, 0)),
                                                                THANHTIEN = s.Sum(x => Math.Round(x.THANHTIEN, 0)),
                                                                THUESUAT = s.Sum(x => Math.Round(x.THUESUAT, 0)),
                                                                TONGTIENVAT = s.Sum(x => Math.Round(x.TONGTIENVAT, 0)),
                                                                TONGCONG = s.Sum(x => Math.Round(x.TONGCONG, 0)),
                                                                TONGSOLUONG = Convert.ToDecimal(s.Sum(x => Math.Round(x.TONGSOLUONG, 0))),
                                                                TONGTRONGLUONG = Convert.ToDecimal(s.Key.TRONGLUONG * s.Sum(x => Math.Round(x.TONGSOLUONG, 0))),
                                                                NAME_DVT_QD = s.Key.NAME_DVT_QD,
                                                                TYLE_QD = s.Key.TYLE_QD
                                                            }).ToList();
                                }
                            }
                            else if (web_Report.NAME_SP == API.Sp_Get_DanhSachPhieuNhapTraHang_ChiTiet_BaoCao)
                            {
                                if (objParameter.HINHTHUC == 1)
                                {
                                    report.DataDefinition.FormulaFields["TIEUDE"].Text = "'" + "BÁO CÁO NHẬP TRẢ HÀNG THEO LOẠI PHIẾU NHẬP" + "'";
                                    lstv_PhieuGioaHang_InTheoGroup = lstSp_Get_DanhSachPhieuGiaoHang_In.GroupBy(s => new { s.NAME_KHUVUC, s.TYLE_QD, s.TRONGLUONG, s.NAME, s.MA, s.NAME_DVT, s.NAME_DVT_QD, s.NAME_NHOMHANGHOA })
                                                            .Select(s => new v_PhieuGioaHang_InTheoGroup
                                                            {
                                                                NAME_GROUP = s.Key.NAME_KHUVUC,
                                                                MAPHIEUXUAT = s.Key.NAME_NHOMHANGHOA,
                                                                MA_HANGHOA = s.Key.MA,
                                                                NAME_HANGHOA = s.Key.NAME,
                                                                NAME_DVT = s.Key.NAME_DVT,
                                                                CHIETKHAU = s.Sum(x => Math.Round(x.CHIETKHAU, 0)),
                                                                TONGTIENGIAMGIA = s.Sum(x => Math.Round(x.TONGTIENGIAMGIA, 0)),
                                                                THANHTIEN = s.Sum(x => Math.Round(x.THANHTIEN, 0)),
                                                                THUESUAT = s.Sum(x => Math.Round(x.THUESUAT, 0)),
                                                                TONGTIENVAT = s.Sum(x => Math.Round(x.TONGTIENVAT, 0)),
                                                                TONGCONG = s.Sum(x => Math.Round(x.TONGCONG, 0)),
                                                                TONGSOLUONG = Convert.ToDecimal(s.Sum(x => Math.Round(x.TONGSOLUONG, 0))),
                                                                TONGTRONGLUONG = Convert.ToDecimal(s.Key.TRONGLUONG * s.Sum(x => Math.Round(x.TONGSOLUONG, 0))),
                                                                NAME_DVT_QD = s.Key.NAME_DVT_QD,
                                                                TYLE_QD = s.Key.TYLE_QD
                                                            }).ToList();
                                }
                                else if (objParameter.HINHTHUC == 3)
                                {
                                    report.DataDefinition.FormulaFields["TIEUDE"].Text = "'" + "BÁO CÁO NHẬP TRẢ HÀNG THEO KHÁCH HÀNG" + "'";
                                    lstv_PhieuGioaHang_InTheoGroup = lstSp_Get_DanhSachPhieuGiaoHang_In.GroupBy(s => new { s.NAME_KHUVUC, s.TYLE_QD, s.TRONGLUONG, s.NAME, s.MA, s.NAME_DVT, s.NAME_DVT_QD, s.NAME_KHACHHANG, s.TEL_KHACHHANG })
                                                            .Select(s => new v_PhieuGioaHang_InTheoGroup
                                                            {
                                                                NAME_GROUP = s.Key.NAME_KHUVUC,
                                                                MAPHIEUXUAT = s.Key.NAME_KHACHHANG + (string.IsNullOrEmpty(s.Key.TEL_KHACHHANG) ? "" : Environment.NewLine + "Điện thoại: ") + s.Key.TEL_KHACHHANG,
                                                                MA_HANGHOA = s.Key.MA,
                                                                NAME_HANGHOA = s.Key.NAME,
                                                                NAME_DVT = s.Key.NAME_DVT,
                                                                CHIETKHAU = s.Sum(x => Math.Round(x.CHIETKHAU, 0)),
                                                                TONGTIENGIAMGIA = s.Sum(x => Math.Round(x.TONGTIENGIAMGIA, 0)),
                                                                THANHTIEN = s.Sum(x => Math.Round(x.THANHTIEN, 0)),
                                                                THUESUAT = s.Sum(x => Math.Round(x.THUESUAT, 0)),
                                                                TONGTIENVAT = s.Sum(x => Math.Round(x.TONGTIENVAT, 0)),
                                                                TONGCONG = s.Sum(x => Math.Round(x.TONGCONG, 0)),
                                                                TONGSOLUONG = Convert.ToDecimal(s.Sum(x => Math.Round(x.TONGSOLUONG, 0))),
                                                                TONGTRONGLUONG = Convert.ToDecimal(s.Key.TRONGLUONG * s.Sum(x => Math.Round(x.TONGSOLUONG, 0))),
                                                                NAME_DVT_QD = s.Key.NAME_DVT_QD,
                                                                TYLE_QD = s.Key.TYLE_QD
                                                            }).ToList();
                                }
                            }
                            else
                            {
                                if (objParameter.HINHTHUC == 1)
                                {
                                    report.DataDefinition.FormulaFields["TIEUDE"].Text = "'" + "BÁO CÁO ĐẶT HÀNG THEO NHÓM HÀNG" + "'";
                                    lstv_PhieuGioaHang_InTheoGroup = lstSp_Get_DanhSachPhieuGiaoHang_In.GroupBy(s => new { s.NAME_KHUVUC, s.TYLE_QD, s.TRONGLUONG, s.NAME, s.MA, s.NAME_DVT, s.NAME_DVT_QD, s.NAME_NHOMHANGHOA })
                                                            .Select(s => new v_PhieuGioaHang_InTheoGroup
                                                            {
                                                                NAME_GROUP = s.Key.NAME_KHUVUC,
                                                                MAPHIEUXUAT = s.Key.NAME_NHOMHANGHOA,
                                                                MA_HANGHOA = s.Key.MA,
                                                                NAME_HANGHOA = s.Key.NAME,
                                                                NAME_DVT = s.Key.NAME_DVT,
                                                                CHIETKHAU = s.Sum(x => Math.Round(x.CHIETKHAU, 0)),
                                                                TONGTIENGIAMGIA = s.Sum(x => Math.Round(x.TONGTIENGIAMGIA, 0)),
                                                                THANHTIEN = s.Sum(x => Math.Round(x.THANHTIEN, 0)),
                                                                THUESUAT = s.Sum(x => Math.Round(x.THUESUAT, 0)),
                                                                TONGTIENVAT = s.Sum(x => Math.Round(x.TONGTIENVAT, 0)),
                                                                TONGCONG = s.Sum(x => Math.Round(x.TONGCONG, 0)),
                                                                TONGSOLUONG = Convert.ToDecimal(s.Sum(x => Math.Round(x.TONGSOLUONG, 0))),
                                                                TONGTRONGLUONG = Convert.ToDecimal(s.Key.TRONGLUONG * s.Sum(x => Math.Round(x.TONGSOLUONG, 0))),
                                                                NAME_DVT_QD = s.Key.NAME_DVT_QD,
                                                                TYLE_QD = s.Key.TYLE_QD
                                                            }).ToList();
                                }
                                else if (objParameter.HINHTHUC == 2)
                                {
                                    report.DataDefinition.FormulaFields["TIEUDE"].Text = "'" + "BÁO CÁO ĐẶT HÀNG THEO NHÂN VIÊN" + "'";
                                    lstv_PhieuGioaHang_InTheoGroup = lstSp_Get_DanhSachPhieuGiaoHang_In.GroupBy(s => new { s.NAME_KHUVUC, s.TYLE_QD, s.TRONGLUONG, s.NAME, s.MA, s.NAME_DVT, s.NAME_DVT_QD, s.NAME_NHANVIEN })
                                                            .Select(s => new v_PhieuGioaHang_InTheoGroup
                                                            {
                                                                NAME_GROUP = s.Key.NAME_NHANVIEN,
                                                                MAPHIEUXUAT = s.Key.NAME_KHUVUC,
                                                                MA_HANGHOA = s.Key.MA,
                                                                NAME_HANGHOA = s.Key.NAME,
                                                                NAME_DVT = s.Key.NAME_DVT,
                                                                CHIETKHAU = s.Sum(x => Math.Round(x.CHIETKHAU, 0)),
                                                                TONGTIENGIAMGIA = s.Sum(x => Math.Round(x.TONGTIENGIAMGIA, 0)),
                                                                THANHTIEN = s.Sum(x => Math.Round(x.THANHTIEN, 0)),
                                                                THUESUAT = s.Sum(x => Math.Round(x.THUESUAT, 0)),
                                                                TONGTIENVAT = s.Sum(x => Math.Round(x.TONGTIENVAT, 0)),
                                                                TONGCONG = s.Sum(x => Math.Round(x.TONGCONG, 0)),
                                                                TONGSOLUONG = Convert.ToDecimal(s.Sum(x => Math.Round(x.TONGSOLUONG, 0))),
                                                                TONGTRONGLUONG = Convert.ToDecimal(s.Key.TRONGLUONG * s.Sum(x => Math.Round(x.TONGSOLUONG, 0))),
                                                                NAME_DVT_QD = s.Key.NAME_DVT_QD,
                                                                TYLE_QD = s.Key.TYLE_QD
                                                            }).ToList();
                                }
                                else if (objParameter.HINHTHUC == 3)
                                {
                                    report.DataDefinition.FormulaFields["TIEUDE"].Text = "'" + "BÁO CÁO ĐẶT HÀNG THEO KHÁCH HÀNG" + "'";
                                    lstv_PhieuGioaHang_InTheoGroup = lstSp_Get_DanhSachPhieuGiaoHang_In.GroupBy(s => new { s.NAME_KHUVUC, s.TYLE_QD, s.TRONGLUONG, s.NAME, s.MA, s.NAME_DVT, s.NAME_DVT_QD, s.NAME_KHACHHANG, s.TEL_KHACHHANG })
                                                            .Select(s => new v_PhieuGioaHang_InTheoGroup
                                                            {
                                                                NAME_GROUP = s.Key.NAME_KHUVUC,
                                                                MAPHIEUXUAT = s.Key.NAME_KHACHHANG + (string.IsNullOrEmpty(s.Key.TEL_KHACHHANG) ? "" : Environment.NewLine + "Điện thoại: ") + s.Key.TEL_KHACHHANG,
                                                                MA_HANGHOA = s.Key.MA,
                                                                NAME_HANGHOA = s.Key.NAME,
                                                                NAME_DVT = s.Key.NAME_DVT,
                                                                CHIETKHAU = s.Sum(x => Math.Round(x.CHIETKHAU, 0)),
                                                                TONGTIENGIAMGIA = s.Sum(x => Math.Round(x.TONGTIENGIAMGIA, 0)),
                                                                THANHTIEN = s.Sum(x => Math.Round(x.THANHTIEN, 0)),
                                                                THUESUAT = s.Sum(x => Math.Round(x.THUESUAT, 0)),
                                                                TONGTIENVAT = s.Sum(x => Math.Round(x.TONGTIENVAT, 0)),
                                                                TONGCONG = s.Sum(x => Math.Round(x.TONGCONG, 0)),
                                                                TONGSOLUONG = Convert.ToDecimal(s.Sum(x => Math.Round(x.TONGSOLUONG, 0))),
                                                                TONGTRONGLUONG = Convert.ToDecimal(s.Key.TRONGLUONG * s.Sum(x => Math.Round(x.TONGSOLUONG, 0))),
                                                                NAME_DVT_QD = s.Key.NAME_DVT_QD,
                                                                TYLE_QD = s.Key.TYLE_QD
                                                            }).ToList();
                                }
                            }
                        }


                        data = Utility.ToDataTable<v_PhieuGioaHang_InTheoGroup>(lstv_PhieuGioaHang_InTheoGroup);

                        report.DataDefinition.FormulaFields["TONGCONG"].Text = "'" + lstv_PhieuGioaHang_InTheoGroup.Sum(s => s.TONGCONG).ToString("N0") + "'";
                        report.DataDefinition.FormulaFields["TONGTRONGLUONG"].Text = "'" + lstv_PhieuGioaHang_InTheoGroup.Sum(s => s.TONGTRONGLUONG).ToString("N0") + "'";
                        report.DataDefinition.FormulaFields["TONGSODONHANG"].Text = "'" + TONGSODONHANG.ToString("N0") + "'";
                        if (objParameter.ID_KHUVUC != null && lstv_PhieuGioaHang_InTheoGroup.Count > 0)
                            report.DataDefinition.FormulaFields["MAPHIEU"].Text = "'" + lstv_PhieuGioaHang_InTheoGroup.FirstOrDefault().NAME_GROUP + "'";
                        else
                            report.DataDefinition.FormulaFields["MAPHIEU"].Text = "'" + "Tất cả khu vực" + "'";


                        break;
                    #endregion

                    #region Báo cáo tài chính
                    case "~/Report/rptBaoCaoTaiChinh.rpt":
                        List<Sp_Get_BaoCaoTaiChinh_Result> lstSp_Get_BaoCaoTaiChinh_ResultGroup = new List<Sp_Get_BaoCaoTaiChinh_Result>();
                        apiResponse = Utility.ExecuteStoredProcT<Sp_Get_BaoCaoTaiChinh_Result>(objParameter, API.Sp_Get_BaoCaoTaiChinh);
                        List<Sp_Get_BaoCaoTaiChinh_Result> lstSp_Get_BaoCaoTaiChinh_Result = (apiResponse.Data as List<Sp_Get_BaoCaoTaiChinh_Result>);
                        if (!apiResponse.Success)
                        {
                            apiResponse.Success = false;
                            apiResponse.Message = apiResponse.Message;
                            return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                        }
                        report.DataDefinition.FormulaFields["TIEUDE"].Text = "'" + "BÁO CÁO TÀI CHÍNH" + "'";
                        if (objParameter.ID_KHUVUC != null && lstSp_Get_BaoCaoTaiChinh_Result.Count > 0)
                            report.DataDefinition.FormulaFields["MAPHIEU"].Text = "'" + lstSp_Get_BaoCaoTaiChinh_Result.FirstOrDefault().NAME_KHUVUC + "'";
                        else
                            report.DataDefinition.FormulaFields["MAPHIEU"].Text = "'" + "Tất cả khu vực" + "'";
                        if (objParameter.HINHTHUC_BAOCAOTAICHINH == 1)
                        {

                            lstSp_Get_BaoCaoTaiChinh_ResultGroup = lstSp_Get_BaoCaoTaiChinh_Result.GroupBy(s => new { s.NAME_KHUVUC, s.ID_KHUVUC, s.NGAYLAP, s.NGAYLAP_TEXT })
                                                        .Select(s => new Sp_Get_BaoCaoTaiChinh_Result
                                                        {
                                                            ID_KHUVUC = s.Key.ID_KHUVUC,
                                                            NAME_KHUVUC = s.Key.NAME_KHUVUC,
                                                            NGAYLAP = s.Key.NGAYLAP,
                                                            NGAYLAP_TEXT = s.Key.NGAYLAP_TEXT,
                                                            SOLUONG_DONHANG = s.Sum(x => Math.Round(x.SOLUONG_DONHANG, 0)),
                                                            TONGTIENGIAMGIA = s.Sum(x => Math.Round(x.TONGTIENGIAMGIA, 0)),
                                                            TONGTHANHTIEN = s.Sum(x => Math.Round(x.TONGTHANHTIEN, 0)),
                                                            TONGTIENVAT = s.Sum(x => Math.Round(x.TONGTIENVAT, 0)),
                                                            TONGCONG = s.Sum(x => Math.Round(x.TONGCONG, 0)),
                                                            TONGCONG_HANGROT = s.Sum(x => Math.Round(x.TONGCONG_HANGROT, 0)),
                                                            SOLUONG_DATHU = s.Sum(x => Math.Round(x.SOLUONG_DATHU, 0)),
                                                            TONGCONG_DATHU = s.Sum(x => Math.Round(x.TONGCONG_DATHU, 0)),
                                                            SOLUONG_THUNO = s.Sum(x => Math.Round(x.SOLUONG_THUNO, 0)),
                                                            TONGCONG_THUNO = s.Sum(x => Math.Round(x.TONGCONG_THUNO, 0)),
                                                            THUKHAC = s.Sum(x => Math.Round(x.THUKHAC, 0)),
                                                            CHIKHAC = s.Sum(x => Math.Round(x.CHIKHAC, 0))
                                                        }).ToList();
                        }
                        else if (objParameter.HINHTHUC_BAOCAOTAICHINH == 2)
                        {
                            lstSp_Get_BaoCaoTaiChinh_ResultGroup = lstSp_Get_BaoCaoTaiChinh_Result.GroupBy(s => new { s.NAME_KHUVUC, s.ID_KHUVUC, s.NGAYLAP, s.NGAYLAP_TEXT, s.ID, s.MA, s.NAME, s.TEL, s.ADDRESS })
                                                        .Select(s => new Sp_Get_BaoCaoTaiChinh_Result
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
                                                            SOLUONG_DONHANG = s.Sum(x => Math.Round(x.SOLUONG_DONHANG, 0)),
                                                            TONGTIENGIAMGIA = s.Sum(x => Math.Round(x.TONGTIENGIAMGIA, 0)),
                                                            TONGTHANHTIEN = s.Sum(x => Math.Round(x.TONGTHANHTIEN, 0)),
                                                            TONGTIENVAT = s.Sum(x => Math.Round(x.TONGTIENVAT, 0)),
                                                            TONGCONG = s.Sum(x => Math.Round(x.TONGCONG, 0)),
                                                            TONGCONG_HANGROT = s.Sum(x => Math.Round(x.TONGCONG_HANGROT, 0)),
                                                            SOLUONG_DATHU = s.Sum(x => Math.Round(x.SOLUONG_DATHU, 0)),
                                                            TONGCONG_DATHU = s.Sum(x => Math.Round(x.TONGCONG_DATHU, 0)),
                                                            SOLUONG_THUNO = s.Sum(x => Math.Round(x.SOLUONG_THUNO, 0)),
                                                            TONGCONG_THUNO = s.Sum(x => Math.Round(x.TONGCONG_THUNO, 0)),
                                                            THUKHAC = s.Sum(x => Math.Round(x.THUKHAC, 0)),
                                                            CHIKHAC = s.Sum(x => Math.Round(x.CHIKHAC, 0))
                                                        }).ToList();
                        }
                        data = Utility.ToDataTable<Sp_Get_BaoCaoTaiChinh_Result>(lstSp_Get_BaoCaoTaiChinh_ResultGroup);
                        break;
                    #endregion

                    #region Phiếu xuất hàng khuyến mãi
                    case "~/Report/rptDanhSachPhieuXuatHang_KhuyenMai.rpt":
                        apiResponse = Utility.ExecuteStoredProcT<Sp_Get_DanhSachPhieuXuatHang_KhuyenMai>(objParameter, API.Sp_Get_DanhSachPhieuXuatHang_KhuyenMai);
                        List<Sp_Get_DanhSachPhieuXuatHang_KhuyenMai> lstSp_Get_DanhSachPhieuXuatHang_KhuyenMai = (apiResponse.Data as List<Sp_Get_DanhSachPhieuXuatHang_KhuyenMai>);
                        if (!apiResponse.Success)
                        {
                            apiResponse.Success = false;
                            apiResponse.Message = apiResponse.Message;
                            return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                        }
                        List<v_PhieuXuatHangKhuyenMai_InTheoGroup> lstSp_Get_DanhSachPhieuXuatHang_KhuyenMai_InTheoGroup = new List<v_PhieuXuatHangKhuyenMai_InTheoGroup>();
                        if (lstSp_Get_DanhSachPhieuXuatHang_KhuyenMai != null)
                        {
                            if (objParameter.HINHTHUC_PHIEUXUATHANG_KHUYENMAI == 1)
                            {
                                report.DataDefinition.FormulaFields["TIEUDE"].Text = "'" + "BÁO CÁO XUẤT HÀNG KHUYẾN MÃI THEO CHƯƠNG TRÌNH KHUYẾN MÃI" + "'";
                                lstSp_Get_DanhSachPhieuXuatHang_KhuyenMai_InTheoGroup = lstSp_Get_DanhSachPhieuXuatHang_KhuyenMai.GroupBy(s => new { s.MA_CHUONGTRINHKHUYENMAI, s.NAME_CHUONGTRINHKHUYENMAI, s.TYLE_QD, s.NAME_HANGHOA, s.MA_HANGHOA, s.NAME_DVT, s.NAME_DVT_QD })
                                                        .Select(s => new v_PhieuXuatHangKhuyenMai_InTheoGroup
                                                        {
                                                            NAME_GROUP = s.Key.MA_CHUONGTRINHKHUYENMAI + "-" + s.Key.NAME_CHUONGTRINHKHUYENMAI,
                                                            MA_HANGHOA = s.Key.MA_HANGHOA,
                                                            NAME_HANGHOA = s.Key.NAME_HANGHOA,
                                                            TONGSOLUONG = Convert.ToDecimal(s.Sum(x => Math.Round(x.ISKHUYENMAI ? x.TONGSOLUONG : 0, 0))),
                                                            TONGTIENGIAMGIA = Convert.ToDecimal(s.Sum(x => Math.Round(x.TONGTIENGIAMGIA, 0))),
                                                            TONGSOLUONG_TRAHANG = Convert.ToDecimal(s.Sum(x => Math.Round(x.ISKHUYENMAI ? x.TONGSOLUONG_TRAHANG : 0, 0))),
                                                            TONGTIENGIAMGIA_TRAHANG = Convert.ToDecimal(s.Sum(x => Math.Round(x.TONGTIENGIAMGIA_TRAHANG, 0))),
                                                            NAME_DVT = s.Key.NAME_DVT,
                                                            NAME_DVT_QD = s.Key.NAME_DVT_QD,
                                                            TYLE_QD = s.Key.TYLE_QD
                                                        }).ToList();
                            }
                            else if (objParameter.HINHTHUC_PHIEUXUATHANG_KHUYENMAI == 2)
                            {
                                report.DataDefinition.FormulaFields["TIEUDE"].Text = "'" + "BÁO CÁO XUẤT HÀNG KHUYẾN MÃI THEO NHÓM HÀNG HÓA" + "'";
                                lstSp_Get_DanhSachPhieuXuatHang_KhuyenMai_InTheoGroup = lstSp_Get_DanhSachPhieuXuatHang_KhuyenMai.GroupBy(s => new { s.MA_NHOMHANGHOA, s.NAME_NHOMHANGHOA, s.TYLE_QD, s.NAME_HANGHOA, s.MA_HANGHOA, s.NAME_DVT, s.NAME_DVT_QD })
                                                        .Select(s => new v_PhieuXuatHangKhuyenMai_InTheoGroup
                                                        {
                                                            NAME_GROUP = s.Key.MA_NHOMHANGHOA + "-" + s.Key.NAME_NHOMHANGHOA,
                                                            MA_HANGHOA = s.Key.MA_HANGHOA,
                                                            NAME_HANGHOA = s.Key.NAME_HANGHOA,
                                                            TONGSOLUONG = Convert.ToDecimal(s.Sum(x => Math.Round(x.ISKHUYENMAI ? x.TONGSOLUONG : 0, 0))),
                                                            TONGTIENGIAMGIA = Convert.ToDecimal(s.Sum(x => Math.Round(x.TONGTIENGIAMGIA, 0))),
                                                            TONGSOLUONG_TRAHANG = Convert.ToDecimal(s.Sum(x => Math.Round(x.ISKHUYENMAI ? x.TONGSOLUONG_TRAHANG : 0, 0))),
                                                            TONGTIENGIAMGIA_TRAHANG = Convert.ToDecimal(s.Sum(x => Math.Round(x.TONGTIENGIAMGIA_TRAHANG, 0))),
                                                            NAME_DVT = s.Key.NAME_DVT,
                                                            NAME_DVT_QD = s.Key.NAME_DVT_QD,
                                                            TYLE_QD = s.Key.TYLE_QD
                                                        }).ToList();
                            }
                            else if (objParameter.HINHTHUC_PHIEUXUATHANG_KHUYENMAI == 3)
                            {
                                report.DataDefinition.FormulaFields["TIEUDE"].Text = "'" + "BÁO CÁO XUẤT HÀNG KHUYẾN MÃI THEO KHU VỰC" + "'";
                                lstSp_Get_DanhSachPhieuXuatHang_KhuyenMai_InTheoGroup = lstSp_Get_DanhSachPhieuXuatHang_KhuyenMai.GroupBy(s => new { s.MA_KHUVUC, s.NAME_KHUVUC, s.TYLE_QD, s.NAME_HANGHOA, s.MA_HANGHOA, s.NAME_DVT, s.NAME_DVT_QD })
                                                        .Select(s => new v_PhieuXuatHangKhuyenMai_InTheoGroup
                                                        {
                                                            NAME_GROUP = s.Key.MA_KHUVUC + "-" + s.Key.NAME_KHUVUC,
                                                            MA_HANGHOA = s.Key.MA_HANGHOA,
                                                            NAME_HANGHOA = s.Key.NAME_HANGHOA,
                                                            TONGSOLUONG = Convert.ToDecimal(s.Sum(x => Math.Round(x.ISKHUYENMAI ? x.TONGSOLUONG : 0, 0))),
                                                            TONGTIENGIAMGIA = Convert.ToDecimal(s.Sum(x => Math.Round(x.TONGTIENGIAMGIA, 0))),
                                                            TONGSOLUONG_TRAHANG = Convert.ToDecimal(s.Sum(x => Math.Round(x.ISKHUYENMAI ? x.TONGSOLUONG_TRAHANG : 0, 0))),
                                                            TONGTIENGIAMGIA_TRAHANG = Convert.ToDecimal(s.Sum(x => Math.Round(x.TONGTIENGIAMGIA_TRAHANG, 0))),
                                                            NAME_DVT = s.Key.NAME_DVT,
                                                            NAME_DVT_QD = s.Key.NAME_DVT_QD,
                                                            TYLE_QD = s.Key.TYLE_QD
                                                        }).ToList();
                            }
                        }


                        data = Utility.ToDataTable<v_PhieuXuatHangKhuyenMai_InTheoGroup>(lstSp_Get_DanhSachPhieuXuatHang_KhuyenMai_InTheoGroup);
                        break;
                    #endregion

                    #region Phiếu giao hàng nhân viên KPI
                    case "~/Report/rptDanhSachPhieuGiaoHangNhanVien_KPI.rpt":
                        apiResponse = Utility.ExecuteStoredProcT<Sp_Get_DanhSachPhieuGiaoHang_KPI_Result>(objParameter, API.Sp_Get_DanhSachPhieuGiaoHang_KPI);
                        List<Sp_Get_DanhSachPhieuGiaoHang_KPI_Result> lstSp_Get_DanhSachPhieuGiaoHang_KPI_Result = (apiResponse.Data as List<Sp_Get_DanhSachPhieuGiaoHang_KPI_Result>);
                        if (!apiResponse.Success)
                        {
                            apiResponse.Success = false;
                            apiResponse.Message = apiResponse.Message;
                            return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                        }
                        List<DanhSachPhieuGiaoHangNhanVien_KPI> lstDanhSachPhieuDatHang_ChiTiet_KPI = new List<DanhSachPhieuGiaoHangNhanVien_KPI>();
                        if (lstSp_Get_DanhSachPhieuGiaoHang_KPI_Result != null)
                        {
                            lstDanhSachPhieuDatHang_ChiTiet_KPI = lstSp_Get_DanhSachPhieuGiaoHang_KPI_Result.GroupBy(s => new { s.NAME_PHONGBAN, s.NAME_NHANVIEN, s.MA_PHONGBAN, s.MA_NHANVIEN })
                                                    .Select(s => new DanhSachPhieuGiaoHangNhanVien_KPI
                                                    {
                                                        NAME_GROUP = s.Key.NAME_PHONGBAN,
                                                        NAME_NHANVIEN = s.Key.NAME_NHANVIEN,
                                                        MA_NHANVIEN = s.Key.MA_NHANVIEN,
                                                        SOLUONG_DONHANG = Convert.ToDecimal(s.Select(x => x.ID_PHIEUXUAT).Count()),
                                                        SOLUONG_GIAOHANG = Convert.ToDecimal(s.Count(x => x.ISDAGIAOHANG)),
                                                        SOLUONG_TRAHANG = Convert.ToDecimal(s.Count(x => x.ISTRAHANG)),
                                                        TONGTIEN = Convert.ToDecimal(s.Sum(x => Math.Round(x.TONGTHANHTIEN, 0)))
                                                    }).ToList();
                        }


                        data = Utility.ToDataTable<DanhSachPhieuGiaoHangNhanVien_KPI>(lstDanhSachPhieuDatHang_ChiTiet_KPI);
                        if (objParameter.ID_PHONGBAN != null && lstDanhSachPhieuDatHang_ChiTiet_KPI.Count > 0)
                            report.DataDefinition.FormulaFields["MAPHIEU"].Text = "'" + lstDanhSachPhieuDatHang_ChiTiet_KPI.FirstOrDefault().NAME_GROUP + "'";
                        else
                            report.DataDefinition.FormulaFields["MAPHIEU"].Text = "'" + "Tất cả phòng ban" + "'";


                        break;
                    #endregion

                    #region Báo cáo hàng hóa nhân viên, khách hàng
                    case "~/Report/rpt_DanhSachHangHoa_NV_KH.rpt":
                        apiResponse = Utility.ExecuteStoredProcT<Sp_Get_DanhSachHangHoa_Result>(objParameter, objParameter.NAME_SP);
                        List<Sp_Get_DanhSachHangHoa_Result> lstSp_Get_DanhSachHangHoa_Result = (apiResponse.Data as List<Sp_Get_DanhSachHangHoa_Result>);
                        if (!apiResponse.Success)
                        {
                            apiResponse.Success = false;
                            apiResponse.Message = apiResponse.Message;
                            return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                        }
                        if (lstSp_Get_DanhSachHangHoa_Result == null)
                            lstSp_Get_DanhSachHangHoa_Result = new List<Sp_Get_DanhSachHangHoa_Result>();
                        data = Utility.ToDataTable<Sp_Get_DanhSachHangHoa_Result>(lstSp_Get_DanhSachHangHoa_Result);
                        break;
                    #endregion

                    #region Báo cáo hàng hóa nhân viên, khách hàng
                    case "~/Report/rpt_DanhSachHangHoa.rpt":
                        apiResponse = Utility.ExecuteStoredProcT<Sp_Get_DanhSachHangHoa>(objParameter, objParameter.NAME_SP);
                        List<Sp_Get_DanhSachHangHoa> lstSp_Get_DanhSachHangHoa_Result1 = (apiResponse.Data as List<Sp_Get_DanhSachHangHoa>);
                        if (!apiResponse.Success)
                        {
                            apiResponse.Success = false;
                            apiResponse.Message = apiResponse.Message;
                            return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                        }
                        if (lstSp_Get_DanhSachHangHoa_Result1 == null)
                            lstSp_Get_DanhSachHangHoa_Result1 = new List<Sp_Get_DanhSachHangHoa>();
                        data = Utility.ToDataTable<Sp_Get_DanhSachHangHoa>(lstSp_Get_DanhSachHangHoa_Result1);
                        break;
                        #endregion
                }
                report.SetDatabaseLogon("test", "test!", "test", "test");
                report.SetDataSource(data);
                Response.Buffer = false;
                Response.ClearContent();
                Response.ClearHeaders();
                Stream stream = report.ExportToStream(ExportFormatType.PortableDocFormat);
                Utility.Report = report;
                return Json(objParameter.ID_REPORT, JsonRequestBehavior.AllowGet); //new FileStreamResult(stream, "application/pdf");
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
                TempData["TitleError"] = API.TitleTryCatch;
                TempData["DetailError"] = ex.Message;
                return RedirectToAction("Index", "Notfound");
            }

        }
        //public ActionResult TestReport()
        //{
        //    if (Utility.KiemTra())
        //    {
        //        return RedirectToAction("Index", "Admin");
        //    }
        //    if (!Utility.KiemTraQuyen(API.ct_PhieuNhap, API.Xem))
        //    {
        //        TempData["TitleError"] = API.TitlePermission;
        //        return RedirectToAction("Index", "Notfound");
        //    }
        //    var report = new ReportClass();
        //    report.FileName = Server.MapPath("~/Report/CrystalReport1.rpt");

        //    List<v_dm_ChucVu> allCustomer = new List<v_dm_ChucVu>();
        //    var apiResponse = Utility.GetListData<v_dm_ChucVu>(API.dm_ChucVu, "", "", Utility.LOC_ID);
        //    if (!apiResponse.Success)
        //    {
        //        TempData["TitleError"] = apiResponse.Message;
        //        return RedirectToAction("Index", "Notfound");
        //    }
        //    allCustomer = (apiResponse.Data as List<v_dm_ChucVu>);
        //    DataTable data = ToDataTable<v_dm_ChucVu>(allCustomer);
        //    report.Load();
        //    report.SetDatabaseLogon("sa", "aasdsfsdf!", "qqqq", "qqq");
        //    report.SetDataSource(data);
        //    Response.Buffer = false;
        //    Response.ClearContent();
        //    Response.ClearHeaders();

        //    Stream stream = report.ExportToStream(ExportFormatType.PortableDocFormat);
        //    return new FileStreamResult(stream, "application/pdf");
        //}

        public ActionResult VerReporte()
        {
            if (Utility.KiemTra())
            {
                return RedirectToAction("Index", "Admin");
            }
            if (Utility.Report != null)
            {
                Stream stream = Utility.Report.ExportToStream(ExportFormatType.PortableDocFormat);
                return new FileStreamResult(stream, "application/pdf");
            }
            else
            {
                TempData["TitleError"] = "Không có dữ liệu!";
                return RedirectToAction("Index", "Notfound");
            }
        }


        public FileResult GetReport()
        {
            Stream stream = Utility.Report.ExportToStream(ExportFormatType.PortableDocFormat);
            byte[] FileBytes = ReadToEnd(stream);
            return File(FileBytes, "application/pdf");
        }

        public ActionResult Download()
        {
            Stream stream = Utility.Report.ExportToStream(ExportFormatType.PortableDocFormat);
            byte[] fileBytes = ReadToEnd(stream);
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "yourfile.pdf");
        }
        public static byte[] ReadToEnd(System.IO.Stream stream)
        {
            long originalPosition = 0;

            if (stream.CanSeek)
            {
                originalPosition = stream.Position;
                stream.Position = 0;
            }

            try
            {
                byte[] readBuffer = new byte[4096];

                int totalBytesRead = 0;
                int bytesRead;

                while ((bytesRead = stream.Read(readBuffer, totalBytesRead, readBuffer.Length - totalBytesRead)) > 0)
                {
                    totalBytesRead += bytesRead;

                    if (totalBytesRead == readBuffer.Length)
                    {
                        int nextByte = stream.ReadByte();
                        if (nextByte != -1)
                        {
                            byte[] temp = new byte[readBuffer.Length * 2];
                            Buffer.BlockCopy(readBuffer, 0, temp, 0, readBuffer.Length);
                            Buffer.SetByte(temp, totalBytesRead, (byte)nextByte);
                            readBuffer = temp;
                            totalBytesRead++;
                        }
                    }
                }

                byte[] buffer = readBuffer;
                if (readBuffer.Length != totalBytesRead)
                {
                    buffer = new byte[totalBytesRead];
                    Buffer.BlockCopy(readBuffer, 0, buffer, 0, totalBytesRead);
                }
                return buffer;
            }
            finally
            {
                if (stream.CanSeek)
                {
                    stream.Position = originalPosition;
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
                    apiResponse.URL = Url.Action("Index", "Admin");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                apiResponse = Utility.GetDetail<v_web_Menu>(ID, API.web_Menu);
                v_web_Menu web_Menu = new v_web_Menu();
                if (!apiResponse.Success)
                {
                    apiResponse.Success = false;
                    apiResponse.Message = apiResponse.Message;
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                if (apiResponse.Data != null)
                    web_Menu = apiResponse.Data as v_web_Menu;

                if (web_Menu == null || string.IsNullOrEmpty(web_Menu.ID_REPORT))
                {
                    apiResponse.Success = false;
                    apiResponse.Message = "Không tìm thấy cấu hình báo cáo " + (web_Menu != null ? web_Menu.NAME : ID) + "!";
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }

                string content = "";
                List<ValueEdit> lst = new List<ValueEdit>();
                List<ValueEdit> lstCheckbox = new List<ValueEdit>();
                v_v_web_Report web_Report = new v_v_web_Report();
                apiResponse = Utility.GetDetail<v_v_web_Report>(web_Menu.ID_REPORT, API.web_Report);
                if (!apiResponse.Success)
                {
                    TempData["TitleError"] = apiResponse.Message;
                    apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                    return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                }
                if (apiResponse.Data != null)
                    web_Report = apiResponse.Data as v_v_web_Report;
                if (web_Report.lstweb_Report_Parameter != null)
                {
                    foreach (var itm in web_Report.lstweb_Report_Parameter.Where(e => e.ISACTIVE).OrderBy(e => e.STT))
                    {
                        if (itm.VALUE != null && itm.TYPE == API.Input)
                        {
                            if (itm.MA_PARAMETER == "ID_REPORT")
                            {
                                content += itm.VALUE.ToString().Replace("@Value", web_Menu.ID);
                            }
                            else if (itm.MA_PARAMETER == "TUNGAY")
                            {
                                content += itm.VALUE.ToString().Replace("@Value", DateTime.Now.AddMonths(-1).ToString("yyyy-MM-dd"));
                            }
                            else if (itm.MA_PARAMETER == "DENNGAY")
                            {
                                content += itm.VALUE.ToString().Replace("@Value", DateTime.Now.ToString("yyyy-MM-dd"));
                            }
                            else
                            {
                                content += itm.VALUE.ToString();
                            }
                        }
                        else if (itm.VALUE != null && itm.TYPE == API.ListValue)
                        {

                            var apiResponseLst = Utility.GetListData<ListValue>(itm.VALUE, "", "", Utility.LOC_ID);
                            if (!apiResponse.Success)
                            {
                                TempData["TitleError"] = apiResponse.Message;
                                apiResponse.Success = false;
                                apiResponse.URL = Url.Action("Index", "Notfound");
                                return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                            }
                            if (apiResponseLst.Data != null)
                                lst.Add(new ValueEdit { Key = itm.MA_PARAMETER, Name = itm.NAME, Controller = itm.VALUE, Value = apiResponseLst.Data as List<ListValue> });
                        }
                        else if (itm.VALUE != null && itm.TYPE == API.Checkbox)
                        {
                            lstCheckbox.Add(new ValueEdit { Key = itm.MA_PARAMETER, Name = itm.NAME, Controller = itm.VALUE });
                        }
                    }
                }

                string tab = "<li class=\"active\" name=\"" + web_Menu.ID + "\" id=\"tab" + web_Menu.ID + "\">" +
                    // "<div class=\"dropdown\" style=\"padding:14px 20px \">" +
                    "<a href=\"#content" + web_Menu.ID + "\" data-toggle=\"tab\" class=\"dropdown-toggle\" style=\"display:table-cell;\"><strong> " + web_Menu.NAME.ToUpper() + "</strong></a><a href=\"#\" onclick=\"DeleteTab('" + web_Menu.ID + "')\" style=\"display:table-cell;\"><i class=\"glyphicon glyphicon-remove\"></i>" +
                    "</a>" +
                    //"</div>" +
                    "</li>";

                //content += "<select class=\"form-control chosen-select\" data-placeholder=\"Choose a Country...\"><option value=\"\"></option>\r\n                                    <option value=\"United States\">United States</option>\r\n                                    <option value=\"United Kingdom\">United Kingdom</option>\r\n                                    <option value=\"Afghanistan\">Afghanistan</option>\r\n                                    <option value=\"Aland Islands\">Aland Islands</option>\r\n                                    <option value=\"Albania\">Albania</option>\r\n                                    <option value=\"Algeria\">Algeria</option>\r\n                                    <option value=\"American Samoa\">American Samoa</option>\r\n                                    <option value=\"Andorra\">Andorra</option>\r\n                                    <option value=\"Angola\">Angola</option>\r\n                                    <option value=\"Anguilla\">Anguilla</option>\r\n                                    <option value=\"Antarctica\">Antarctica</option>\r\n                                    <option value=\"Antigua and Barbuda\">Antigua and Barbuda</option>\r\n                                    <option value=\"Argentina\">Argentina</option>\r\n                                    <option value=\"Armenia\">Armenia</option>\r\n                                    <option value=\"Aruba\">Aruba</option>\r\n                                    <option value=\"Australia\">Australia</option>\r\n                                    <option value=\"Austria\">Austria</option>\r\n                                    <option value=\"Azerbaijan\">Azerbaijan</option>\r\n                                    <option value=\"Bahamas\">Bahamas</option>\r\n                                    <option value=\"Bahrain\">Bahrain</option>\r\n                                    <option value=\"Bangladesh\">Bangladesh</option>\r\n                                    <option value=\"Barbados\">Barbados</option>\r\n                                    <option value=\"Belarus\">Belarus</option>\r\n                                    <option value=\"Belgium\">Belgium</option>\r\n                                    <option value=\"Belize\">Belize</option>\r\n                                    <option value=\"Benin\">Benin</option>\r\n                                    <option value=\"Bermuda\">Bermuda</option>\r\n                                    <option value=\"Bhutan\">Bhutan</option>\r\n                                    <option value=\"Bolivia, Plurinational State of\">Bolivia, Plurinational State of</option>\r\n                                    <option value=\"Bonaire, Sint Eustatius and Saba\">Bonaire, Sint Eustatius and Saba</option>\r\n                                    <option value=\"Bosnia and Herzegovina\">Bosnia and Herzegovina</option>\r\n                                    <option value=\"Botswana\">Botswana</option>\r\n                                    <option value=\"Bouvet Island\">Bouvet Island</option>\r\n                                    <option value=\"Brazil\">Brazil</option>\r\n                                    <option value=\"British Indian Ocean Territory\">British Indian Ocean Territory</option>\r\n                                    <option value=\"Brunei Darussalam\">Brunei Darussalam</option>\r\n                                    <option value=\"Bulgaria\">Bulgaria</option>\r\n                                    <option value=\"Burkina Faso\">Burkina Faso</option>\r\n                                    <option value=\"Burundi\">Burundi</option>\r\n                                    <option value=\"Cambodia\">Cambodia</option>\r\n                                    <option value=\"Cameroon\">Cameroon</option>\r\n                                    <option value=\"Canada\">Canada</option>\r\n                                    <option value=\"Cape Verde\">Cape Verde</option>\r\n                                    <option value=\"Cayman Islands\">Cayman Islands</option>\r\n                                    <option value=\"Central African Republic\">Central African Republic</option>\r\n                                    <option value=\"Chad\">Chad</option>\r\n                                    <option value=\"Chile\">Chile</option>\r\n                                    <option value=\"China\">China</option>\r\n                                    <option value=\"Christmas Island\">Christmas Island</option>\r\n                                    <option value=\"Cocos (Keeling) Islands\">Cocos (Keeling) Islands</option>\r\n                                    <option value=\"Colombia\">Colombia</option>\r\n                                    <option value=\"Comoros\">Comoros</option>\r\n                                    <option value=\"Congo\">Congo</option>\r\n                                    <option value=\"Congo, The Democratic Republic of The\">Congo, The Democratic Republic of The</option>\r\n                                    <option value=\"Cook Islands\">Cook Islands</option>\r\n                                    <option value=\"Costa Rica\">Costa Rica</option>\r\n                                    <option value=\"Cote D'ivoire\">Cote D'ivoire</option>\r\n                                    <option value=\"Croatia\">Croatia</option>\r\n                                    <option value=\"Cuba\">Cuba</option>\r\n                                    <option value=\"Curacao\">Curacao</option>\r\n                                    <option value=\"Cyprus\">Cyprus</option>\r\n                                    <option value=\"Czech Republic\">Czech Republic</option>\r\n                                    <option value=\"Denmark\">Denmark</option>\r\n                                    <option value=\"Djibouti\">Djibouti</option>\r\n                                    <option value=\"Dominica\">Dominica</option>\r\n                                    <option value=\"Dominican Republic\">Dominican Republic</option>\r\n                                    <option value=\"Ecuador\">Ecuador</option>\r\n                                    <option value=\"Egypt\">Egypt</option>\r\n                                    <option value=\"El Salvador\">El Salvador</option>\r\n                                    <option value=\"Equatorial Guinea\">Equatorial Guinea</option>\r\n                                    <option value=\"Eritrea\">Eritrea</option>\r\n                                    <option value=\"Estonia\">Estonia</option>\r\n                                    <option value=\"Ethiopia\">Ethiopia</option>\r\n                                    <option value=\"Falkland Islands (Malvinas)\">Falkland Islands (Malvinas)</option>\r\n                                    <option value=\"Faroe Islands\">Faroe Islands</option>\r\n                                    <option value=\"Fiji\">Fiji</option>\r\n                                    <option value=\"Finland\">Finland</option>\r\n                                    <option value=\"France\">France</option>\r\n                                    <option value=\"French Guiana\">French Guiana</option>\r\n                                    <option value=\"French Polynesia\">French Polynesia</option>\r\n                                    <option value=\"French Southern Territories\">French Southern Territories</option>\r\n                                    <option value=\"Gabon\">Gabon</option>\r\n                                    <option value=\"Gambia\">Gambia</option>\r\n                                    <option value=\"Georgia\">Georgia</option>\r\n                                    <option value=\"Germany\">Germany</option>\r\n                                    <option value=\"Ghana\">Ghana</option>\r\n                                    <option value=\"Gibraltar\">Gibraltar</option>\r\n                                    <option value=\"Greece\">Greece</option>\r\n                                    <option value=\"Greenland\">Greenland</option>\r\n                                    <option value=\"Grenada\">Grenada</option>\r\n                                    <option value=\"Guadeloupe\">Guadeloupe</option>\r\n                                    <option value=\"Guam\">Guam</option>\r\n                                    <option value=\"Guatemala\">Guatemala</option>\r\n                                    <option value=\"Guernsey\">Guernsey</option>\r\n                                    <option value=\"Guinea\">Guinea</option>\r\n                                    <option value=\"Guinea-bissau\">Guinea-bissau</option>\r\n                                    <option value=\"Guyana\">Guyana</option>\r\n                                    <option value=\"Haiti\">Haiti</option>\r\n                                    <option value=\"Heard Island and Mcdonald Islands\">Heard Island and Mcdonald Islands</option>\r\n                                    <option value=\"Holy See (Vatican City State)\">Holy See (Vatican City State)</option>\r\n                                    <option value=\"Honduras\">Honduras</option>\r\n                                    <option value=\"Hong Kong\">Hong Kong</option>\r\n                                    <option value=\"Hungary\">Hungary</option>\r\n                                    <option value=\"Iceland\">Iceland</option>\r\n                                    <option value=\"India\">India</option>\r\n                                    <option value=\"Indonesia\">Indonesia</option>\r\n                                    <option value=\"Iran, Islamic Republic of\">Iran, Islamic Republic of</option>\r\n                                    <option value=\"Iraq\">Iraq</option>\r\n                                    <option value=\"Ireland\">Ireland</option>\r\n                                    <option value=\"Isle of Man\">Isle of Man</option>\r\n                                    <option value=\"Israel\">Israel</option>\r\n                                    <option value=\"Italy\">Italy</option>\r\n                                    <option value=\"Jamaica\">Jamaica</option>\r\n                                    <option value=\"Japan\">Japan</option>\r\n                                    <option value=\"Jersey\">Jersey</option>\r\n                                    <option value=\"Jordan\">Jordan</option>\r\n                                    <option value=\"Kazakhstan\">Kazakhstan</option>\r\n                                    <option value=\"Kenya\">Kenya</option>\r\n                                    <option value=\"Kiribati\">Kiribati</option>\r\n                                    <option value=\"Korea, Democratic People's Republic of\">Korea, Democratic People's Republic of</option>\r\n                                    <option value=\"Korea, Republic of\">Korea, Republic of</option>\r\n                                    <option value=\"Kuwait\">Kuwait</option>\r\n                                    <option value=\"Kyrgyzstan\">Kyrgyzstan</option>\r\n                                    <option value=\"Lao People's Democratic Republic\">Lao People's Democratic Republic</option>\r\n                                    <option value=\"Latvia\">Latvia</option>\r\n                                    <option value=\"Lebanon\">Lebanon</option>\r\n                                    <option value=\"Lesotho\">Lesotho</option>\r\n                                    <option value=\"Liberia\">Liberia</option>\r\n                                    <option value=\"Libya\">Libya</option>\r\n                                    <option value=\"Liechtenstein\">Liechtenstein</option>\r\n                                    <option value=\"Lithuania\">Lithuania</option>\r\n                                    <option value=\"Luxembourg\">Luxembourg</option>\r\n                                    <option value=\"Macao\">Macao</option>\r\n                                    <option value=\"Macedonia, The Former Yugoslav Republic of\">Macedonia, The Former Yugoslav Republic of</option>\r\n                                    <option value=\"Madagascar\">Madagascar</option>\r\n                                    <option value=\"Malawi\">Malawi</option>\r\n                                    <option value=\"Malaysia\">Malaysia</option>\r\n                                    <option value=\"Maldives\">Maldives</option>\r\n                                    <option value=\"Mali\">Mali</option>\r\n                                    <option value=\"Malta\">Malta</option>\r\n                                    <option value=\"Marshall Islands\">Marshall Islands</option>\r\n                                    <option value=\"Martinique\">Martinique</option>\r\n                                    <option value=\"Mauritania\">Mauritania</option>\r\n                                    <option value=\"Mauritius\">Mauritius</option>\r\n                                    <option value=\"Mayotte\">Mayotte</option>\r\n                                    <option value=\"Mexico\">Mexico</option>\r\n                                    <option value=\"Micronesia, Federated States of\">Micronesia, Federated States of</option>\r\n                                    <option value=\"Moldova, Republic of\">Moldova, Republic of</option>\r\n                                    <option value=\"Monaco\">Monaco</option>\r\n                                    <option value=\"Mongolia\">Mongolia</option>\r\n                                    <option value=\"Montenegro\">Montenegro</option>\r\n                                    <option value=\"Montserrat\">Montserrat</option>\r\n                                    <option value=\"Morocco\">Morocco</option>\r\n                                    <option value=\"Mozambique\">Mozambique</option>\r\n                                    <option value=\"Myanmar\">Myanmar</option>\r\n                                    <option value=\"Namibia\">Namibia</option>\r\n                                    <option value=\"Nauru\">Nauru</option>\r\n                                    <option value=\"Nepal\">Nepal</option>\r\n                                    <option value=\"Netherlands\">Netherlands</option>\r\n                                    <option value=\"New Caledonia\">New Caledonia</option>\r\n                                    <option value=\"New Zealand\">New Zealand</option>\r\n                                    <option value=\"Nicaragua\">Nicaragua</option>\r\n                                    <option value=\"Niger\">Niger</option>\r\n                                    <option value=\"Nigeria\">Nigeria</option>\r\n                                    <option value=\"Niue\">Niue</option>\r\n                                    <option value=\"Norfolk Island\">Norfolk Island</option>\r\n                                    <option value=\"Northern Mariana Islands\">Northern Mariana Islands</option>\r\n                                    <option value=\"Norway\">Norway</option>\r\n                                    <option value=\"Oman\">Oman</option>\r\n                                    <option value=\"Pakistan\">Pakistan</option>\r\n                                    <option value=\"Palau\">Palau</option>\r\n                                    <option value=\"Palestinian Territory, Occupied\">Palestinian Territory, Occupied</option>\r\n                                    <option value=\"Panama\">Panama</option>\r\n                                    <option value=\"Papua New Guinea\">Papua New Guinea</option>\r\n                                    <option value=\"Paraguay\">Paraguay</option>\r\n                                    <option value=\"Peru\">Peru</option>\r\n                                    <option value=\"Philippines\">Philippines</option>\r\n                                    <option value=\"Pitcairn\">Pitcairn</option>\r\n                                    <option value=\"Poland\">Poland</option>\r\n                                    <option value=\"Portugal\">Portugal</option>\r\n                                    <option value=\"Puerto Rico\">Puerto Rico</option>\r\n                                    <option value=\"Qatar\">Qatar</option>\r\n                                    <option value=\"Reunion\">Reunion</option>\r\n                                    <option value=\"Romania\">Romania</option>\r\n                                    <option value=\"Russian Federation\">Russian Federation</option>\r\n                                    <option value=\"Rwanda\">Rwanda</option>\r\n                                    <option value=\"Saint Barthelemy\">Saint Barthelemy</option>\r\n                                    <option value=\"Saint Helena, Ascension and Tristan da Cunha\">Saint Helena, Ascension and Tristan da Cunha</option>\r\n                                    <option value=\"Saint Kitts and Nevis\">Saint Kitts and Nevis</option>\r\n                                    <option value=\"Saint Lucia\">Saint Lucia</option>\r\n                                    <option value=\"Saint Martin (French part)\">Saint Martin (French part)</option>\r\n                                    <option value=\"Saint Pierre and Miquelon\">Saint Pierre and Miquelon</option>\r\n                                    <option value=\"Saint Vincent and The Grenadines\">Saint Vincent and The Grenadines</option>\r\n                                    <option value=\"Samoa\">Samoa</option>\r\n                                    <option value=\"San Marino\">San Marino</option>\r\n                                    <option value=\"Sao Tome and Principe\">Sao Tome and Principe</option>\r\n                                    <option value=\"Saudi Arabia\">Saudi Arabia</option>\r\n                                    <option value=\"Senegal\">Senegal</option>\r\n                                    <option value=\"Serbia\">Serbia</option>\r\n                                    <option value=\"Seychelles\">Seychelles</option>\r\n                                    <option value=\"Sierra Leone\">Sierra Leone</option>\r\n                                    <option value=\"Singapore\">Singapore</option>\r\n                                    <option value=\"Sint Maarten (Dutch part)\">Sint Maarten (Dutch part)</option>\r\n                                    <option value=\"Slovakia\">Slovakia</option>\r\n                                    <option value=\"Slovenia\">Slovenia</option>\r\n                                    <option value=\"Solomon Islands\">Solomon Islands</option>\r\n                                    <option value=\"Somalia\">Somalia</option>\r\n                                    <option value=\"South Africa\">South Africa</option>\r\n                                    <option value=\"South Georgia and The South Sandwich Islands\">South Georgia and The South Sandwich Islands</option>\r\n                                    <option value=\"South Sudan\">South Sudan</option>\r\n                                    <option value=\"Spain\">Spain</option>\r\n                                    <option value=\"Sri Lanka\">Sri Lanka</option>\r\n                                    <option value=\"Sudan\">Sudan</option>\r\n                                    <option value=\"Suriname\">Suriname</option>\r\n                                    <option value=\"Svalbard and Jan Mayen\">Svalbard and Jan Mayen</option>\r\n                                    <option value=\"Swaziland\">Swaziland</option>\r\n                                    <option value=\"Sweden\">Sweden</option>\r\n                                    <option value=\"Switzerland\">Switzerland</option>\r\n                                    <option value=\"Syrian Arab Republic\">Syrian Arab Republic</option>\r\n                                    <option value=\"Taiwan, Province of China\">Taiwan, Province of China</option>\r\n                                    <option value=\"Tajikistan\">Tajikistan</option>\r\n                                    <option value=\"Tanzania, United Republic of\">Tanzania, United Republic of</option>\r\n                                    <option value=\"Thailand\">Thailand</option>\r\n                                    <option value=\"Timor-leste\">Timor-leste</option>\r\n                                    <option value=\"Togo\">Togo</option>\r\n                                    <option value=\"Tokelau\">Tokelau</option>\r\n                                    <option value=\"Tonga\">Tonga</option>\r\n                                    <option value=\"Trinidad and Tobago\">Trinidad and Tobago</option>\r\n                                    <option value=\"Tunisia\">Tunisia</option>\r\n                                    <option value=\"Turkey\">Turkey</option>\r\n                                    <option value=\"Turkmenistan\">Turkmenistan</option>\r\n                                    <option value=\"Turks and Caicos Islands\">Turks and Caicos Islands</option>\r\n                                    <option value=\"Tuvalu\">Tuvalu</option>\r\n                                    <option value=\"Uganda\">Uganda</option>\r\n                                    <option value=\"Ukraine\">Ukraine</option>\r\n                                    <option value=\"United Arab Emirates\">United Arab Emirates</option>\r\n                                    <option value=\"United Kingdom\">United Kingdom</option>\r\n                                    <option value=\"United States\">United States</option>\r\n                                    <option value=\"United States Minor Outlying Islands\">United States Minor Outlying Islands</option>\r\n                                    <option value=\"Uruguay\">Uruguay</option>\r\n                                    <option value=\"Uzbekistan\">Uzbekistan</option>\r\n                                    <option value=\"Vanuatu\">Vanuatu</option>\r\n                                    <option value=\"Venezuela, Bolivarian Republic of\">Venezuela, Bolivarian Republic of</option>\r\n                                    <option value=\"Viet Nam\">Viet Nam</option>\r\n                                    <option value=\"Virgin Islands, British\">Virgin Islands, British</option>\r\n                                    <option value=\"Virgin Islands, U.S.\">Virgin Islands, U.S.</option>\r\n                                    <option value=\"Wallis and Futuna\">Wallis and Futuna</option>\r\n                                    <option value=\"Western Sahara\">Western Sahara</option>\r\n                                    <option value=\"Yemen\">Yemen</option>\r\n                                    <option value=\"Zambia\">Zambia</option>\r\n                                    <option value=\"Zimbabwe\">Zimbabwe</option>\r\n                                </select>";
                bool bolPhanQuyenUser = false;
                if(web_Menu.CONTROLLERNAME == "Sp_Get_DanhSachPhieuDatHang")
                {
                    bolPhanQuyenUser = Utility.KiemTraQuyen(API.Sp_Get_DanhSachPhieuDatHang, API.CreateUser);
                }
               
                foreach (var item in lst)
                {
                    if(bolPhanQuyenUser == true && item.Key == "ID_NHANVIEN")
                    {
                        content += "<input type=\"hidden\" id=\"ID_NHANVIEN\" name=\"ID_NHANVIEN\" value=\""+ Session[Sessions.idUser].ToString() + "\">";
                        continue;
                    }    
                    content += "<div class=\"form-group\">" +
                           "<label class=\"col-sm-2 control-label\" for=\"" + item.Key + "\">" + item.Name + "</label>" +
                           "<div class=\"col-sm-4\"><div class=\"input-group mb15\">";

                    content += "<span class=\"input-group-btn\"><button type=\"button\" class=\"btn btn-default\" onclick=\"myFunOpenSearch('content" + web_Menu.ID + "', '" + item.Controller + "', '" + API.Chon1 + "', '" + item.Key + "', '')\"><span class=\"glyphicon glyphicon-search\"></span></button></span>";
                    content += "<select class=\"form-control chosen-select\" data-placeholder=\"" + Utility.GetTitleChon(item.Controller) + "\" id=\"" + item.Key + "\" name=\"" + item.Key + "\">";
                    content += "<option value=\"\">" + Utility.GetTitleChon(item.Controller) + "</option>";
                    foreach (var value in item.Value as List<ListValue>)
                    {
                        if (value.ISACTIVE == null || value.ISACTIVE == true)
                            content += "<option value=\"" + value.ID + "\">" + value.NAME + "</option>";
                    }
                    content += "</select>";
                    content += "</div>" + "</div>" + "</div>";
                }

                foreach (var item in lstCheckbox)
                {
                    content += "<div class=\"form-group\">" +
                           "<label class=\"col-sm-2 control-label\" for=\"" + item.Key + "\">" + item.Name + "</label>" +
                           "<div class=\"col-sm-4\">";
                    content += item.Controller;
                    content += "</div>" + "</div>";
                }

                apiResponse.ID = ID;
                apiResponse.TAB = tab;
                apiResponse.CONTENT = content;
                apiResponse.Detail = lst;
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