using DatabaseTHP;
using DatabaseTHP.Class;
using DatabaseTHP.StoredProcedure.Parameter;
using DatabaseTHP.StoredProcedure;
using MVC_QuanLyTHP.Class;
using MVC_QuanLyTHP.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web.DynamicData;
using System.Web.Mvc;
using System.Security.Cryptography;
using Microsoft.Web.Helpers;
using PagedList;
using static System.Runtime.CompilerServices.RuntimeHelpers;

namespace MVC_QuanLyTHP.Controllers
{
    public class SearchController : Controller
    {
        //
        // GET: /Partial_admin/
        [ChildActionOnly]
        public ActionResult Index()
        {
            string urlAddProductPromotion_YC = API.dm_ChuongTrinhKhuyenMai;
            string url = Request.Url.AbsolutePath;
            if (url.ToUpper().Contains(API.dm_KPI_KinhDoanh.ToUpper()))
                urlAddProductPromotion_YC = API.dm_KPI_KinhDoanh;
            ViewBag.urlAddProductPromotion_YC = urlAddProductPromotion_YC;
           v_v_Search Search = new v_v_Search();
            Search.lstdm_ThueSuat = new List<v_dm_ThueSuat>();
            Search.lstdm_ThueSuat = Utility.GetListData<v_dm_ThueSuat>(API.dm_ThueSuat, "", "", Utility.LOC_ID).Data as List<v_dm_ThueSuat>;
            Search.lstdm_DonViTinh = new List<v_dm_DonViTinh>();
            Search.lstdm_DonViTinh = Utility.GetListData<v_dm_DonViTinh>(API.dm_DonViTinh, "", "", Utility.LOC_ID).Data as List<v_dm_DonViTinh>;
            Search.lstKhuVuc = new List<v_dm_KhuVuc>();
            Search.lstKhuVuc = Utility.GetListData<v_dm_KhuVuc>(API.dm_KhuVuc, "", "", Utility.LOC_ID).Data as List<v_dm_KhuVuc>;
            if (Search.lstdm_ThueSuat != null)
                Search.lstdm_ThueSuat = Search.lstdm_ThueSuat.Where(s => s.ISACTIVE).ToList();
            else
                Search.lstdm_ThueSuat = new List<v_dm_ThueSuat>();

            if (Search.lstdm_DonViTinh != null)
                Search.lstdm_DonViTinh = Search.lstdm_DonViTinh.Where(s => s.ISACTIVE).ToList();
            else
                Search.lstdm_DonViTinh = new List<v_dm_DonViTinh>();

            if (Search.lstKhuVuc != null)
                Search.lstKhuVuc = Search.lstKhuVuc.Where(s => s.ISACTIVE).ToList();
            else
                Search.lstKhuVuc = new List<v_dm_KhuVuc>();

            ViewBag.PermissionEditPrice = Utility.KiemTraQuyen(API.ct_PhieuDatHang, API.EditPrice);
            return PartialView(Search);
        }

        //HinhThucTimKiem 
        //HinhThucTimKiem = 1 Chọn 1, 2 chọn nhiều
        [HttpPost]
        public ActionResult LoadSearch(string MyModal, string ClassName = "", int HinhThucTimKiem = (int)API.HinhThucTimKiem.Chon1, string ValueField = "", string TextField = "", string ID_KHO = "", string ID_KHUVUC = "")
        {
            ApiResponse apiResponse = new ApiResponse();
            if (Utility.KiemTra())
            {
                apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Admin");
                return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
            }
            Search Search = new Search();
            Search.MyModal = MyModal;
            Search.ClassName = ClassName;
            Search.ValueField = ValueField;
            Search.TextField = TextField;
            Search.HinhThucTimKiem = HinhThucTimKiem;
            Search.TitleSearch = Utility.GetTitleFrom(ClassName);
            Search.ID_KHO = ID_KHO;
            Search.ID_KHUVUC = ID_KHUVUC;

            switch (ClassName)
            {
                case API.ct_PhieuDatHang + API.dm_KhachHang:
                    Search.ID_KHUVUC = "-1";
                    Search.TitleSearch = Utility.GetTitleFrom(API.dm_KhachHang);
                    Search.ShowSearchValue = Utility.GetShowSearchValue<dm_KhachHang>("");
                    DepositController objDeposit = new DepositController();
                    string ID_NHOMQUYEN = Session[Sessions.idNhomQuyen] != null ? Session[Sessions.idNhomQuyen].ToString() : "";
                    API.LONGITUDE = Utility.Longitude;
                    API.LATITUDE = Utility.Latitude;
                    if (ID_NHOMQUYEN != "-1")
                        apiResponse = objDeposit.GetDanhSachKhachHang<v_v_dm_KhachHang>(ID_NHOMQUYEN);
                    else
                        apiResponse = Utility.GetListData<v_v_dm_KhachHang>(API.dm_KhachHang, "", "", Utility.LOC_ID);
                    if (!apiResponse.Success)
                    {
                        apiResponse.Data = new List<v_v_dm_KhachHang>();
                        TempData["TitleError"] = apiResponse.Message;
                        apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                        break;
                    }
                    Search = GetData<v_v_dm_KhachHang>(apiResponse, Search);
                    break;
                    
                case API.ct_PhieuNhap:
                    Search.TitleSearch = Utility.GetTitleFrom(API.ct_PhieuNhap);
                    Search.ShowSearchValue = Utility.GetShowSearchValue<dm_HangHoa>("");
                    apiResponse = Utility.Get_DanhSachSanPhamKho<Product_Detail>(Search.ID_KHO, false);
                    if (!apiResponse.Success)
                    {
                        apiResponse.Data = new List<Product_Detail>();
                        TempData["TitleError"] = apiResponse.Message;
                        apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                        break;
                    }
                    Search = GetData<Product_Detail>(apiResponse, Search);
                    break;

                case API.ct_PhieuXuat:
                    Search.TitleSearch = Utility.GetTitleFrom(API.ct_PhieuXuat);
                    Search.ShowSearchValue = Utility.GetShowSearchValue<dm_HangHoa>("");
                    apiResponse = Utility.Get_DanhSachSanPhamKho<Product_Detail>(Search.ID_KHO, true);
                    if (!apiResponse.Success)
                    {
                        apiResponse.Data = new List<Product_Detail>();
                        TempData["TitleError"] = apiResponse.Message;
                        apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                        break;
                    }
                    Search = GetData<Product_Detail>(apiResponse, Search);
                    break;
                case API.dm_HangHoa:
                    Search.TitleSearch = Utility.GetTitleFrom(API.dm_HangHoa);
                    Search.ShowSearchValue = Utility.GetShowSearchValue<dm_HangHoa>("");
                    apiResponse = Utility.GetListData<v_dm_HangHoa>(ClassName, Search.ShowSearchValue, "", Utility.LOC_ID);
                    Search = GetData<v_dm_HangHoa>(apiResponse, Search);
                    break;
                case API.dm_KhachHang:
                    Search.ID_KHUVUC = "-1";
                    Search.TitleSearch = Utility.GetTitleFrom(API.dm_KhachHang);
                    Search.ShowSearchValue = Utility.GetShowSearchValue<dm_KhachHang>("");
                    apiResponse = Utility.GetListData<v_v_dm_KhachHang>(ClassName, Search.ShowSearchValue, "", Utility.LOC_ID);
                    Search = GetData<v_v_dm_KhachHang>(apiResponse, Search);
                    break;
                case API.dm_NhomHangHoa:
                    Search.TitleSearch = Utility.GetTitleFrom(API.dm_NhomHangHoa);
                    Search.ShowSearchValue = Utility.GetShowSearchValue<dm_NhomHangHoa>("");
                    apiResponse = Utility.GetListData<v_dm_NhomHangHoa>(ClassName, Search.ShowSearchValue, "", Utility.LOC_ID);
                    Search = GetData<v_dm_NhomHangHoa>(apiResponse, Search);
                    break;
                case API.dm_NhaCungCap:
                    Search.TitleSearch = Utility.GetTitleFrom(API.dm_NhaCungCap);
                    Search.ShowSearchValue = Utility.GetShowSearchValue<dm_NhaCungCap>("");
                    apiResponse = Utility.GetListData<v_dm_NhaCungCap>(ClassName, Search.ShowSearchValue, "", Utility.LOC_ID);
                    Search = GetData<v_dm_NhaCungCap>(apiResponse, Search);
                    break;
                case API.dm_NhanVien:
                    Search.TitleSearch = Utility.GetTitleFrom(API.dm_NhanVien);
                    Search.ShowSearchValue = Utility.GetShowSearchValue<dm_NhanVien>("");
                    apiResponse = Utility.GetListData<v_dm_NhanVien>(ClassName, Search.ShowSearchValue, "", Utility.LOC_ID);
                    Search = GetData<v_dm_NhanVien>(apiResponse, Search);
                    break;
                case API.dm_KhuVuc:
                    Search.TitleSearch = Utility.GetTitleFrom(API.dm_KhuVuc);
                    Search.ShowSearchValue = Utility.GetShowSearchValue<dm_KhuVuc>("");
                    apiResponse = Utility.GetListData<v_dm_KhuVuc>(ClassName, Search.ShowSearchValue, "", Utility.LOC_ID);
                    Search = GetData<v_dm_KhuVuc>(apiResponse, Search);
                    break;
                case API.dm_TaiKhoanNganHang:
                    Search.TitleSearch = Utility.GetTitleFrom(API.dm_TaiKhoanNganHang);
                    Search.ShowSearchValue = Utility.GetShowSearchValue<dm_TaiKhoanNganHang>("");
                    apiResponse = Utility.GetListData<v_dm_TaiKhoanNganHang>(ClassName, Search.ShowSearchValue, "", Utility.LOC_ID);
                    Search = GetData<v_dm_TaiKhoanNganHang>(apiResponse, Search);
                    break;
                case API.dm_Xe:
                    Search.TitleSearch = Utility.GetTitleFrom(API.dm_Xe);
                    Search.ShowSearchValue = Utility.GetShowSearchValue<dm_Xe>("");
                    apiResponse = Utility.GetListData<v_dm_Xe>(ClassName, Search.ShowSearchValue, "", Utility.LOC_ID);
                    Search = GetData<v_dm_Xe>(apiResponse, Search);
                    break;
                case API.dm_TienTe:
                    Search.TitleSearch = Utility.GetTitleFrom(API.dm_TienTe);
                    Search.ShowSearchValue = Utility.GetShowSearchValue<dm_TienTe>("");
                    apiResponse = Utility.GetListData<v_dm_TienTe>(ClassName, Search.ShowSearchValue, "", Utility.LOC_ID);
                    Search = GetData<v_dm_TienTe>(apiResponse, Search);
                    break;
                case API.dm_NhomKhachHang:
                    Search.TitleSearch = Utility.GetTitleFrom(API.dm_NhomKhachHang);
                    Search.ShowSearchValue = Utility.GetShowSearchValue<dm_NhomKhachHang>("");
                    apiResponse = Utility.GetListData<v_dm_NhomKhachHang>(ClassName, Search.ShowSearchValue, "", Utility.LOC_ID);
                    Search = GetData<v_dm_NhomKhachHang>(apiResponse, Search);
                    break;
                case API.dm_NhomNhaCungCap:
                    Search.TitleSearch = Utility.GetTitleFrom(API.dm_NhomNhaCungCap);
                    Search.ShowSearchValue = Utility.GetShowSearchValue<dm_NhomNhaCungCap>("");
                    apiResponse = Utility.GetListData<v_dm_NhomNhaCungCap>(ClassName, Search.ShowSearchValue, "", Utility.LOC_ID);
                    Search = GetData<v_dm_NhomNhaCungCap>(apiResponse, Search);
                    break;
                case API.web_Menu:
                    Search.TitleSearch = Utility.GetTitleFrom(API.web_Menu);
                    Search.ShowSearchValue = Utility.GetShowSearchValue<web_Menu>("");
                    apiResponse = Utility.GetListData<v_web_Menu>(ClassName, Search.ShowSearchValue, "", Utility.LOC_ID);
                    Search = GetData<v_web_Menu>(apiResponse, Search);
                    break;
                case API.dm_ChucVu:
                    Search.TitleSearch = Utility.GetTitleFrom(API.dm_ChucVu);
                    Search.ShowSearchValue = Utility.GetShowSearchValue<dm_ChucVu>("");
                    apiResponse = Utility.GetListData<v_dm_ChucVu>(ClassName, Search.ShowSearchValue, "", Utility.LOC_ID);
                    Search = GetData<v_dm_ChucVu>(apiResponse, Search);
                    break;
                case API.dm_ChuongTrinhKhuyenMai:
                    Search.TitleSearch = Utility.GetTitleFrom(API.dm_ChuongTrinhKhuyenMai);
                    Search.ShowSearchValue = Utility.GetShowSearchValue<dm_ChuongTrinhKhuyenMai>("");
                    apiResponse = Utility.GetListData<v_dm_ChuongTrinhKhuyenMai>(ClassName, Search.ShowSearchValue, "", Utility.LOC_ID);
                    Search = GetData<v_dm_ChuongTrinhKhuyenMai>(apiResponse, Search);
                    break;
                case API.dm_ThueSuat:
                    Search.TitleSearch = Utility.GetTitleFrom(API.dm_ThueSuat);
                    Search.ShowSearchValue = Utility.GetShowSearchValue<dm_ThueSuat>("");
                    apiResponse = Utility.GetListData<v_dm_ThueSuat>(ClassName, Search.ShowSearchValue, "", Utility.LOC_ID);
                    Search = GetData<v_dm_ThueSuat>(apiResponse, Search);
                    break;
                case API.dm_LoaiPhieuChi:
                    Search.TitleSearch = Utility.GetTitleFrom(API.dm_LoaiPhieuChi);
                    Search.ShowSearchValue = Utility.GetShowSearchValue<dm_LoaiPhieuChi>("");
                    apiResponse = Utility.GetListData<v_dm_LoaiPhieuChi>(ClassName, Search.ShowSearchValue, "", Utility.LOC_ID);
                    Search = GetData<v_dm_LoaiPhieuChi>(apiResponse, Search);
                    break;
                case API.dm_LoaiPhieuNhap:
                    Search.TitleSearch = Utility.GetTitleFrom(API.dm_LoaiPhieuNhap);
                    Search.ShowSearchValue = Utility.GetShowSearchValue<dm_LoaiPhieuNhap>("");
                    apiResponse = Utility.GetListData<v_dm_LoaiPhieuNhap>(ClassName, Search.ShowSearchValue, "", Utility.LOC_ID);
                    Search = GetData<v_dm_LoaiPhieuNhap>(apiResponse, Search);
                    break;
                case API.dm_LoaiPhieuThu:
                    Search.TitleSearch = Utility.GetTitleFrom(API.dm_LoaiPhieuThu);
                    Search.ShowSearchValue = Utility.GetShowSearchValue<dm_LoaiPhieuThu>("");
                    apiResponse = Utility.GetListData<v_dm_LoaiPhieuThu>(ClassName, Search.ShowSearchValue, "", Utility.LOC_ID);
                    Search = GetData<v_dm_LoaiPhieuThu>(apiResponse, Search);
                    break;
                case API.dm_LoaiPhieuXuat:
                    Search.TitleSearch = Utility.GetTitleFrom(API.dm_LoaiPhieuXuat);
                    Search.ShowSearchValue = Utility.GetShowSearchValue<dm_LoaiPhieuXuat>("");
                    apiResponse = Utility.GetListData<v_dm_LoaiPhieuXuat>(ClassName, Search.ShowSearchValue, "", Utility.LOC_ID);
                    Search = GetData<v_dm_LoaiPhieuXuat>(apiResponse, Search);
                    break;
                case API.dm_DonViTinh:
                    Search.TitleSearch = Utility.GetTitleFrom(API.dm_DonViTinh);
                    Search.ShowSearchValue = Utility.GetShowSearchValue<dm_DonViTinh>("");
                    apiResponse = Utility.GetListData<v_dm_DonViTinh>(ClassName, Search.ShowSearchValue, "", Utility.LOC_ID);
                    Search = GetData<v_dm_DonViTinh>(apiResponse, Search);
                    break;
                case API.AspNetUser:
                    Search.TitleSearch = Utility.GetTitleFrom(API.AspNetUser);
                    Search.ShowSearchValue = Utility.GetShowSearchValue<AspNetUsers>("");
                    apiResponse = Utility.GetListData<v_AspNetUsers>(ClassName, Search.ShowSearchValue, "", Utility.LOC_ID);
                    Search = GetData<v_AspNetUsers>(apiResponse, Search);
                    break;
                case API.dm_Kho:
                    Search.TitleSearch = Utility.GetTitleFrom(API.dm_Kho);
                    Search.ShowSearchValue = Utility.GetShowSearchValue<dm_Kho>("");
                    apiResponse = Utility.GetListData<v_dm_Kho>(ClassName, Search.ShowSearchValue, "", Utility.LOC_ID);
                    Search = GetData<v_dm_Kho>(apiResponse, Search);
                    break;
                case API.web_NhomQuyen:
                    Search.TitleSearch = Utility.GetTitleFrom(API.web_NhomQuyen);
                    Search.ShowSearchValue = Utility.GetShowSearchValue<web_NhomQuyen>("");
                    apiResponse = Utility.GetListData<v_web_NhomQuyen>(ClassName, Search.ShowSearchValue, "", Utility.LOC_ID);
                    Search = GetData<v_web_NhomQuyen>(apiResponse, Search);
                    break;
                case API.dm_PhongBan:
                    Search.TitleSearch = Utility.GetTitleFrom(API.dm_PhongBan);
                    Search.ShowSearchValue = Utility.GetShowSearchValue<dm_PhongBan>("");
                    apiResponse = Utility.GetListData<v_dm_PhongBan>(ClassName, Search.ShowSearchValue, "", Utility.LOC_ID);
                    Search = GetData<v_dm_PhongBan>(apiResponse, Search);
                    break;
            }
            Search.listSearch = Utility.listSearch;
            return Json(Search, JsonRequestBehavior.AllowGet);
        }
        
        public Search GetData<T>(ApiResponse apiResponse, Search Search)
        {
            try
            {
                IEnumerable<PropertyInfo> props = typeof(T).GetRuntimeProperties();
               
                List<view_web_NoteClass> lstNoteClass = Utility.GetNoteClass();
                if (lstNoteClass != null)
                    lstNoteClass = lstNoteClass.Where(s => !string.IsNullOrEmpty(s.NAMECLASS) && s.NAMECLASS.ToLower() == typeof(T).Name.Replace("v_", "").ToLower() && s.ISSEARCH).ToList();


                if (lstNoteClass != null && lstNoteClass.Count > 0)
                {
                    if (Search.HinhThucTimKiem == (int)API.HinhThucTimKiem.ChonNhieu)
                    {
                        Search.TrField += "<th style=\"font-weight: bold; text-align:center; white-space: nowrap;\"></th>";
                    }
                    if (apiResponse.Data as List<v_v_dm_KhachHang> != null)
                    {
                        Search.TrField += "<th style=\"font-weight: bold; text-align:center; white-space: nowrap;\">...</th>";
                    }
                    foreach (var itmSearch in lstNoteClass.OrderBy(s => s.STT))
                    {
                        Search.TrField += "<th style=\"font-weight: bold; text-align:center; white-space: nowrap;\"> " + itmSearch.DISPLAYNAME + "</th>";
                    }
                }
                if (apiResponse.Success)
                {
                    if (apiResponse.Data != null)
                    {
                        if (lstNoteClass != null && lstNoteClass.Count > 0)
                        {
                            int total = (apiResponse.Data as List<T>).Count();


                            int i = 0;
                            bool bolKhachHang = false;
                            if(apiResponse.Data as List<v_v_dm_KhachHang> != null)
                            {
                                var lstKhachHang = (apiResponse.Data as List<v_v_dm_KhachHang>).ToList();
                                List<v_v_dm_KhachHang> lstKhachHang_Tam = new List<v_v_dm_KhachHang>();
                                List<v_v_dm_KhachHang> lstKhachHang_Tam1 = new List<v_v_dm_KhachHang>();
                                lstKhachHang_Tam = (from itm in lstKhachHang
                                                    where itm.KHOANGCACH != 0
                                                   orderby itm.KHOANGCACH, itm.NAME
                                                   select itm).ToList();
                                lstKhachHang_Tam1 = (from itm in lstKhachHang
                                                     where itm.KHOANGCACH == 0
                                                     orderby itm.KHOANGCACH, itm.NAME
                                                     select itm).ToList();
                                lstKhachHang_Tam.AddRange(lstKhachHang_Tam1);
                                apiResponse.Data = lstKhachHang_Tam;
                                bolKhachHang = true;
                            }
                            foreach (var itm in apiResponse.Data as List<T>)
                            {
                                if(total > 100 && string.IsNullOrEmpty(Search.SearchString))
                                {
                                    i += 1;
                                    if (i > 100)
                                        break;
                                }
                                Boolean bolISACTIVE = true;
                                PropertyInfo prop = props.Where(e => e.Name.ToUpper() == ("ISACTIVE").ToUpper()).FirstOrDefault();  
                                if(prop != null)
                                {
                                    var ISACTIVE = prop.GetValue(itm);
                                    if (!(Boolean)ISACTIVE)
                                        bolISACTIVE = false;
                                }
                                string ID = "";
                                if (bolISACTIVE)
                                {
                                    if (prop != null)
                                    {
                                        if (API.ct_PhieuNhap == Search.ClassName || API.ct_PhieuXuat == Search.ClassName)
                                        {
                                            prop = props.Where(e => e.Name.ToUpper() == ("ID_HANGHOAKHO").ToUpper()).FirstOrDefault();
                                            if (prop != null)
                                            {
                                                object val = prop.GetValue(itm);
                                                if (val != null)
                                                {
                                                    ID = val.ToString();
                                                }
                                            }
                                        }
                                        else
                                        {
                                            prop = props.Where(e => e.Name.ToUpper() == ("ID").ToUpper()).FirstOrDefault();
                                            if (prop != null)
                                            {
                                                object val = prop.GetValue(itm);
                                                if (val != null)
                                                {
                                                    ID = val.ToString();
                                                }
                                            }
                                        }
                                        if (!string.IsNullOrEmpty(ID))
                                        {
                                            if (Search.HinhThucTimKiem == (int)API.HinhThucTimKiem.ChonNhieu)
                                            {
                                                Search.BodyField += "<tr id=\"" + ID + "\">";
                                            }
                                            else if (Search.HinhThucTimKiem == (int)API.HinhThucTimKiem.ChonSanPhamCombo)
                                            {
                                                Search.BodyField += "<tr id=\"" + ID + "\"  ondblclick=myFunSuccessCombo(\"" + ID + "\")>";
                                            }
                                            else if (Search.HinhThucTimKiem == (int)API.HinhThucTimKiem.ChonSanPhamNhapXuatChuyen)
                                            {
                                                if(API.ct_PhieuXuat == Search.ClassName)
                                                    Search.BodyField += "<tr id=\"" + ID + "\"  ondblclick=myFunOpenProduct(null,\"" + ID + "\")>";
                                                else
                                                    Search.BodyField += "<tr id=\"" + ID + "\"  ondblclick=myFunSuccessInputOutput(\"" + ID + "\",\"" + Search.ClassName + "\",\"" + Search.ID_KHO + "\")>";
                                            }
                                            else if (Search.HinhThucTimKiem == (int)API.HinhThucTimKiem.ChonCTKM_Tang)
                                            {
                                                Search.BodyField += "<tr id=\"" + ID + "\"  ondblclick=myFunSuccessPromotion_Tang(\"" + ID + "\",\"" + Search.ClassName + "\",\"" + Search.ID_KHO + "\")>";
                                            }
                                            else if (Search.HinhThucTimKiem == (int)API.HinhThucTimKiem.ChonCTKM_YC)
                                            {
                                                Search.BodyField += "<tr id=\"" + ID + "\"  ondblclick=myFunSuccessPromotion_YC(\"" + ID + "\",\"" + Search.ClassName + "\",\"" + Search.ID_KHO + "\")>";
                                            }
                                            else if (Search.HinhThucTimKiem == (int)API.HinhThucTimKiem.ChonCTKM_YC_NHOMSANPHAM)
                                            {
                                                Search.BodyField += "<tr id=\"" + ID + "\"  ondblclick=myFunSuccessPromotionNHH_YC(\"" + ID + "\")>";
                                            }
                                            else if (Search.HinhThucTimKiem == (int)API.HinhThucTimKiem.Chon1_GiaoHang)
                                            {
                                                Search.BodyField += "<tr id=\"" + ID + "\"  ondblclick=myFunctionDelivery(\"" + API.ct_PhieuGiaoHang + "\",\"AddDeliveryShipper\",\"" + ID + "\")>";
                                            }
                                            else if (Search.HinhThucTimKiem == (int)API.HinhThucTimKiem.Chon1_NhomQuyen)
                                            {
                                                Search.BodyField += "<tr id=\"" + ID + "\"  ondblclick=myFunctionDelivery(\"" + API.dm_KPI_KinhDoanh + "\",\"AddProductPromotion_NQ\",\"" + ID + "\")>";
                                            }
                                            else if (Search.HinhThucTimKiem == (int)API.HinhThucTimKiem.Chon1_NhanVien)
                                            {
                                                Search.BodyField += "<tr id=\"" + ID + "\"  ondblclick=myFunctionDelivery(\"" + API.dm_KPI_KinhDoanh + "\",\"AddProductPromotion_NV\",\"" + ID + "\")>";
                                            }
                                            else
                                            {
                                                Search.BodyField += "<tr id=\"" + ID + "\"  ondblclick=myFunSuccess(\"" + ID + "\")>";
                                            }
                                        }
                                    }
                                    else
                                    {
                                        Search.BodyField += "<tr>";
                                    }
                                    if (lstNoteClass != null && lstNoteClass.Count > 0)
                                    {
                                        if (Search.HinhThucTimKiem == (int)API.HinhThucTimKiem.ChonNhieu)
                                        {
                                            Search.BodyField += "<td style=\"white-space: nowrap; \" id=\"" + ID + "\"><input type=\"checkbox\" name=\"TBL_ITEM\" id=\"" + ID + "\" onchange=\"checkboxChanged()\" class=\"cbx\"></td>";
                                        }
                                        if(bolKhachHang)
                                        {
                                            if((itm as v_v_dm_KhachHang).KHOANGCACH != 0)
                                            {
                                                //string origin = (API.LATITUDE.ToString().Replace(",", ".") + "," + API.LONGITUDE.ToString().Replace(",", "."));
                                                //string destination = (((itm as v_v_dm_KhachHang).LATITUDE ?? 0).ToString().Replace(",", ".") + "," + ((itm as v_v_dm_KhachHang).LONGITUDE ?? 0).ToString().Replace(",", "."));
                                                //apiResponse = Utility.GetDetail<string>(origin + "/" + destination, "Map");
                                                //if (apiResponse.Success)
                                                //{

                                                //}
                                            }


                                            Search.BodyField += "<td style=\"white-space: nowrap; \" id=\"" + prop.Name + "\">" + ((itm as v_v_dm_KhachHang).KHOANGCACH > 1000 ? ((itm as v_v_dm_KhachHang).KHOANGCACH /1000).ToString("N0") + " km" : (itm as v_v_dm_KhachHang).KHOANGCACH.ToString("N0") + " m") + "</td></a>";
                                        } 
                                        foreach (var itmSearch in lstNoteClass.OrderBy(s => s.STT))
                                        {
                                            prop = props.Where(e => e.Name.ToUpper() == (string.IsNullOrEmpty(itmSearch.REPLACESEARCH) ? itmSearch.NAMECOLUMN : itmSearch.REPLACESEARCH).ToUpper()).FirstOrDefault();
                                            if (prop != null)
                                            {
                                                object val = prop.GetValue(itm);
                                                if (val != null && val.GetType().ToString().Contains("Date"))
                                                    Search.BodyField += "<td style=\"white-space: nowrap; \" id=\"" + prop.Name + "\">" + (object)(((DateTime)val).ToString("dd/MM/yyyy")) + "</td></a>";
                                                else if (val != null && val.GetType().ToString().Contains("Bool"))
                                                    Search.BodyField += "<td style=\"white-space: nowrap; \" id=\"" + prop.Name + "\"><input " + ((Boolean)val == true ? "checked=\"checked\"" : "") + " class=\"check-box\" disabled=\"disabled\" type=\"checkbox\"></td>";
                                                else if (val != null && Utility.IsNumericType(val.GetType()))
                                                {
                                                    Decimal dec = Convert.ToDecimal(val);
                                                    if(API.ct_PhieuXuat == Search.ClassName && prop.Name == "QTY")
                                                    {
                                                       var SanPham = itm as Product_Detail;
                                                        if(SanPham.TYLE_QD == 1 || SanPham.TYLE_QD == 0)
                                                        {
                                                            Search.BodyField += "<td style=\"white-space: nowrap; \" id=\"" + prop.Name + "\">" + dec.ToString("N0") + "</td>";
                                                        }
                                                        else if (SanPham.TYLE_QD >  1)
                                                        {
                                                            var soluong = Convert.ToInt32(SanPham.QTY) / Convert.ToInt32(SanPham.TYLE_QD);
                                                            string strsolong = "";
                                                            if(soluong > 0)
                                                                strsolong = soluong.ToString("N0") + " " + SanPham.NAME_DVT;
                                                            if(SanPham.QTY - (soluong * SanPham.TYLE_QD) > 0)
                                                            {
                                                                if (string.IsNullOrEmpty(strsolong))
                                                                    strsolong += (SanPham.QTY - (soluong * SanPham.TYLE_QD)).ToString("N0") + " " + SanPham.NAME_DVT_QD;
                                                                else
                                                                    strsolong += " " + (SanPham.QTY - (soluong * SanPham.TYLE_QD)).ToString("N0") + " " + SanPham.NAME_DVT_QD;
                                                            }
                                                            Search.BodyField += "<td style=\"white-space: nowrap; \" id=\"" + prop.Name + "\">" + strsolong + "</td>";
                                                        }
                                                    }
                                                    else
                                                        Search.BodyField += "<td style=\"white-space: nowrap; \" id=\"" + prop.Name + "\">" + dec.ToString("N0") + "</td>";
                                                }
                                                else
                                                {
                                                    if (itmSearch.NAMECOLUMN.ToUpper() == "PICTURE")
                                                    {
                                                        if(val != null && !string.IsNullOrEmpty(val.ToString()))
                                                           Search.BodyField += "<td style=\"white-space: nowrap; \" id=\"" + prop.Name + "\"><div class=\"thmb-prev\"><a href = \""+ API.PathProduct + "" + val + "\" data-rel=\"prettyPhotoSearch\" rel=\"prettyPhotoSearch\"><img src=\"" + API.PathProduct + "" + val + "\" class=\"img-responsive\" alt=\"\"></a></div></td>";
                                                        else
                                                            Search.BodyField += "<td style=\"white-space: nowrap; \" id=\"" + prop.Name + "\"><div class=\"thmb-prev\"></div></td>";
                                                        //Search.BodyField += "<a><td style=\"white-space: nowrap; \" id=\"" + prop.Name + "\"><div class=\"thmb-prev\"><a href=\"/Images_Upload/" + val + "\" data-rel=\"prettyPhoto\" rel=\"prettyPhoto\"><img src=\"/Images_Upload/" + val + "\" class=\"img-responsive\" alt=\"\"></a></div></td></a>";
                                                    }
                                                    else
                                                        Search.BodyField += "<td style=\"white-space: nowrap; \" id=\"" + prop.Name + "\">" + val + "</td>";
                                                }    
                                            }
                                            else
                                            {
                                                Search.BodyField += "<td></td>";
                                            }    
                                        }
                                    }
                                    Search.BodyField += "</tr>";
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
            }
            return Search;
        }


        [HttpPost, ValidateInput(false)]
        public ActionResult Search([Bind(Include = "ID_KHUVUC,HinhThucTimKiem,MyModal,ShowSearchValue,SearchString,ClassName,ValueField,TextField,TrField,BodyField,ID_KHO")] Search Search)
        {
            Search.TitleSearch = Utility.GetTitleFrom(Search.ClassName);
            ApiResponse apiResponse = new ApiResponse();
            if (Utility.KiemTra())
            {
                apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Admin");
                return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
            }
            switch (Search.ClassName)
            {
                case API.ct_PhieuDatHang + API.dm_KhachHang:
                    Search.TitleSearch = Utility.GetTitleFrom(API.dm_KhachHang);
                    Search.ShowSearchValue = Utility.GetShowSearchValue<dm_KhachHang>(Search.ShowSearchValue);
                    DepositController objDeposit = new DepositController();
                    string ShowSearchValue_Decrypt = clsMaHoa.Decrypt(Search.ShowSearchValue, clsMaHoa.PassMaHoa);
                    string ID_NHOMQUYEN = Session[Sessions.idNhomQuyen] != null ? Session[Sessions.idNhomQuyen].ToString() : "";
                    API.LONGITUDE = Utility.Longitude;
                    API.LATITUDE = Utility.Latitude;
                    if (ID_NHOMQUYEN != "-1")
                        apiResponse = objDeposit.GetDanhSachKhachHang<v_v_dm_KhachHang>(ID_NHOMQUYEN, Search.SearchString, ShowSearchValue_Decrypt == "ALL" ? "" : ShowSearchValue_Decrypt);
                    else
                        apiResponse = Utility.GetListData<v_v_dm_KhachHang>(API.dm_KhachHang, Search.ShowSearchValue, Search.SearchString, Utility.LOC_ID);

                    if (!apiResponse.Success)
                    {
                        apiResponse.Data = new List<v_v_dm_KhachHang>();
                        TempData["TitleError"] = apiResponse.Message;
                        apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                        break;
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(Search.ID_KHUVUC) && apiResponse.Data as List<v_v_dm_KhachHang> != null)
                            apiResponse.Data = (apiResponse.Data as List<v_v_dm_KhachHang>).Where(e => e.ID_KHUVUC == Search.ID_KHUVUC).ToList();
                    }    
                    Search = GetData<v_v_dm_KhachHang>(apiResponse, Search);
                    break;
                case API.ct_PhieuNhap:
                    Search.TitleSearch = Utility.GetTitleFrom(API.ct_PhieuNhap);
                    Search.ShowSearchValue = Utility.GetShowSearchValue<dm_HangHoa>(Search.ShowSearchValue);
                    ShowSearchValue_Decrypt = clsMaHoa.Decrypt(Search.ShowSearchValue, clsMaHoa.PassMaHoa);
                    apiResponse = Utility.Get_DanhSachSanPhamKho<Product_Detail>(Search.ID_KHO, false, "", Search.SearchString, ShowSearchValue_Decrypt == "ALL" ? "" : ShowSearchValue_Decrypt);
                    if (!apiResponse.Success)
                    {
                        apiResponse.Data = new List<Product_Detail>();
                        TempData["TitleError"] = apiResponse.Message;
                        apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                        break;
                    }
                    Search = GetData<Product_Detail>(apiResponse, Search);
                    break;

                case API.ct_PhieuXuat:
                    Search.TitleSearch = Utility.GetTitleFrom(API.ct_PhieuXuat);
                    Search.ShowSearchValue = Utility.GetShowSearchValue<dm_HangHoa>(Search.ShowSearchValue);
                    ShowSearchValue_Decrypt = clsMaHoa.Decrypt(Search.ShowSearchValue, clsMaHoa.PassMaHoa);
                    apiResponse = Utility.Get_DanhSachSanPhamKho<Product_Detail>(Search.ID_KHO, true, "", Search.SearchString, ShowSearchValue_Decrypt == "ALL" ? "" : ShowSearchValue_Decrypt);
                    if (!apiResponse.Success)
                    {
                        apiResponse.Data = new List<Product_Detail>();
                        TempData["TitleError"] = apiResponse.Message;
                        apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                        break;
                    }
                    Search = GetData<Product_Detail>(apiResponse, Search);
                    break;
                case API.dm_HangHoa:
                    Search.TitleSearch = Utility.GetTitleFrom(API.dm_HangHoa);
                    Search.ShowSearchValue = Utility.GetShowSearchValue<dm_HangHoa>(Search.ShowSearchValue);
                    apiResponse = Utility.GetListData<v_dm_HangHoa>(Search.ClassName, Search.ShowSearchValue, Search.SearchString, Utility.LOC_ID);
                    Search = GetData<v_dm_HangHoa>(apiResponse, Search);
                    break;
                case API.dm_KhachHang:
                    Search.TitleSearch = Utility.GetTitleFrom(API.dm_KhachHang);
                    Search.ShowSearchValue = Utility.GetShowSearchValue<dm_KhachHang>(Search.ShowSearchValue);
                    apiResponse = Utility.GetListData<v_v_dm_KhachHang>(Search.ClassName, Search.ShowSearchValue, Search.SearchString, Utility.LOC_ID);
                    if (!string.IsNullOrEmpty(Search.ID_KHUVUC) && apiResponse.Data as List<v_v_dm_KhachHang> != null)
                        apiResponse.Data = (apiResponse.Data as List<v_v_dm_KhachHang>).Where(e => e.ID_KHUVUC == Search.ID_KHUVUC).ToList();
                    Search = GetData<v_v_dm_KhachHang>(apiResponse, Search);
                    
                    break;
                case API.dm_NhomHangHoa:
                    Search.TitleSearch = Utility.GetTitleFrom(API.dm_NhomHangHoa);
                    Search.ShowSearchValue = Utility.GetShowSearchValue<dm_NhomHangHoa>(Search.ShowSearchValue);
                    apiResponse = Utility.GetListData<v_dm_NhomHangHoa>(Search.ClassName, Search.ShowSearchValue, Search.SearchString, Utility.LOC_ID);
                    Search = GetData<v_dm_NhomHangHoa>(apiResponse, Search);
                    break;
                case API.dm_NhaCungCap:
                    Search.TitleSearch = Utility.GetTitleFrom(API.dm_NhaCungCap);
                    Search.ShowSearchValue = Utility.GetShowSearchValue<dm_NhaCungCap>(Search.ShowSearchValue);
                    apiResponse = Utility.GetListData<v_dm_NhaCungCap>(Search.ClassName, Search.ShowSearchValue, Search.SearchString, Utility.LOC_ID);
                    Search = GetData<v_dm_NhaCungCap>(apiResponse, Search);
                    break;
                case API.dm_NhanVien:
                    Search.TitleSearch = Utility.GetTitleFrom(API.dm_NhaCungCap);
                    Search.ShowSearchValue = Utility.GetShowSearchValue<dm_NhanVien>(Search.ShowSearchValue);
                    apiResponse = Utility.GetListData<v_dm_NhanVien>(Search.ClassName, Search.ShowSearchValue, Search.SearchString, Utility.LOC_ID);
                    Search = GetData<v_dm_NhanVien>(apiResponse, Search);
                    break;
                case API.dm_KhuVuc:
                    Search.TitleSearch = Utility.GetTitleFrom(API.dm_KhuVuc);
                    Search.ShowSearchValue = Utility.GetShowSearchValue<dm_KhuVuc>(Search.ShowSearchValue);
                    apiResponse = Utility.GetListData<v_dm_KhuVuc>(Search.ClassName, Search.ShowSearchValue, Search.SearchString, Utility.LOC_ID);
                    Search = GetData<v_dm_KhuVuc>(apiResponse, Search);
                    break;
                case API.dm_TaiKhoanNganHang:
                    Search.TitleSearch = Utility.GetTitleFrom(API.dm_TaiKhoanNganHang);
                    Search.ShowSearchValue = Utility.GetShowSearchValue<dm_TaiKhoanNganHang>(Search.ShowSearchValue);
                    apiResponse = Utility.GetListData<v_dm_TaiKhoanNganHang>(Search.ClassName, Search.ShowSearchValue, Search.SearchString, Utility.LOC_ID);
                    Search = GetData<v_dm_TaiKhoanNganHang>(apiResponse, Search);
                    break;
                case API.dm_Xe:
                    Search.TitleSearch = Utility.GetTitleFrom(API.dm_Xe);
                    Search.ShowSearchValue = Utility.GetShowSearchValue<dm_Xe>(Search.ShowSearchValue);
                    apiResponse = Utility.GetListData<v_dm_Xe>(Search.ClassName, Search.ShowSearchValue, Search.SearchString, Utility.LOC_ID);
                    Search = GetData<v_dm_Xe>(apiResponse, Search);
                    break;
                case API.dm_TienTe:
                    Search.TitleSearch = Utility.GetTitleFrom(API.dm_TienTe);
                    Search.ShowSearchValue = Utility.GetShowSearchValue<dm_TienTe>(Search.ShowSearchValue);
                    apiResponse = Utility.GetListData<v_dm_TienTe>(Search.ClassName, Search.ShowSearchValue, Search.SearchString, Utility.LOC_ID);
                    Search = GetData<v_dm_TienTe>(apiResponse, Search);
                    break;
                case API.dm_NhomKhachHang:
                    Search.TitleSearch = Utility.GetTitleFrom(API.dm_NhomKhachHang);
                    Search.ShowSearchValue = Utility.GetShowSearchValue<dm_NhomKhachHang>(Search.ShowSearchValue);
                    apiResponse = Utility.GetListData<v_dm_NhomKhachHang>(Search.ClassName, Search.ShowSearchValue, Search.SearchString, Utility.LOC_ID);
                    Search = GetData<v_dm_NhomKhachHang>(apiResponse, Search);
                    break;
                case API.dm_NhomNhaCungCap:
                    Search.TitleSearch = Utility.GetTitleFrom(API.dm_NhomNhaCungCap);
                    Search.ShowSearchValue = Utility.GetShowSearchValue<dm_NhomNhaCungCap>(Search.ShowSearchValue);
                    apiResponse = Utility.GetListData<v_dm_NhomNhaCungCap>(Search.ClassName, Search.ShowSearchValue, Search.SearchString, Utility.LOC_ID);
                    Search = GetData<v_dm_NhomNhaCungCap>(apiResponse, Search);
                    break;
                case API.web_Menu:
                    Search.TitleSearch = Utility.GetTitleFrom(API.web_Menu);
                    Search.ShowSearchValue = Utility.GetShowSearchValue<web_Menu>(Search.ShowSearchValue);
                    apiResponse = Utility.GetListData<v_web_Menu>(Search.ClassName, Search.ShowSearchValue, Search.SearchString, Utility.LOC_ID);
                    Search = GetData<v_web_Menu>(apiResponse, Search);
                    break;
                case API.dm_ChucVu:
                    Search.TitleSearch = Utility.GetTitleFrom(API.dm_ChucVu);
                    Search.ShowSearchValue = Utility.GetShowSearchValue<dm_ChucVu>(Search.ShowSearchValue);
                    apiResponse = Utility.GetListData<v_dm_ChucVu>(Search.ClassName, Search.ShowSearchValue, Search.SearchString, Utility.LOC_ID);
                    Search = GetData<v_dm_ChucVu>(apiResponse, Search);
                    break;
                case API.dm_ChuongTrinhKhuyenMai:
                    Search.TitleSearch = Utility.GetTitleFrom(API.dm_ChuongTrinhKhuyenMai);
                    Search.ShowSearchValue = Utility.GetShowSearchValue<dm_ChuongTrinhKhuyenMai>(Search.ShowSearchValue);
                    apiResponse = Utility.GetListData<v_dm_ChuongTrinhKhuyenMai>(Search.ClassName, Search.ShowSearchValue, Search.SearchString, Utility.LOC_ID);
                    Search = GetData<v_dm_ChuongTrinhKhuyenMai>(apiResponse, Search);
                    break;
                case API.dm_ThueSuat:
                    Search.TitleSearch = Utility.GetTitleFrom(API.dm_ThueSuat);
                    Search.ShowSearchValue = Utility.GetShowSearchValue<dm_ThueSuat>(Search.ShowSearchValue);
                    apiResponse = Utility.GetListData<v_dm_ThueSuat>(Search.ClassName, Search.ShowSearchValue, Search.SearchString, Utility.LOC_ID);
                    Search = GetData<v_dm_ThueSuat>(apiResponse, Search);
                    break;
                case API.dm_LoaiPhieuChi:
                    Search.TitleSearch = Utility.GetTitleFrom(API.dm_LoaiPhieuChi);
                    Search.ShowSearchValue = Utility.GetShowSearchValue<dm_LoaiPhieuChi>(Search.ShowSearchValue);
                    apiResponse = Utility.GetListData<v_dm_LoaiPhieuChi>(Search.ClassName, Search.ShowSearchValue, Search.SearchString, Utility.LOC_ID);
                    Search = GetData<v_dm_LoaiPhieuChi>(apiResponse, Search);
                    break;
                case API.dm_LoaiPhieuNhap:
                    Search.TitleSearch = Utility.GetTitleFrom(API.dm_LoaiPhieuNhap);
                    Search.ShowSearchValue = Utility.GetShowSearchValue<dm_LoaiPhieuNhap>(Search.ShowSearchValue);
                    apiResponse = Utility.GetListData<v_dm_LoaiPhieuNhap>(Search.ClassName, Search.ShowSearchValue, Search.SearchString, Utility.LOC_ID);
                    Search = GetData<v_dm_LoaiPhieuNhap>(apiResponse, Search);
                    break;
                case API.dm_LoaiPhieuThu:
                    Search.TitleSearch = Utility.GetTitleFrom(API.dm_LoaiPhieuThu);
                    Search.ShowSearchValue = Utility.GetShowSearchValue<dm_LoaiPhieuThu>(Search.ShowSearchValue);
                    apiResponse = Utility.GetListData<v_dm_LoaiPhieuThu>(Search.ClassName, Search.ShowSearchValue, Search.SearchString, Utility.LOC_ID);
                    Search = GetData<v_dm_LoaiPhieuThu>(apiResponse, Search);
                    break;
                case API.dm_LoaiPhieuXuat:
                    Search.TitleSearch = Utility.GetTitleFrom(API.dm_LoaiPhieuXuat);
                    Search.ShowSearchValue = Utility.GetShowSearchValue<dm_LoaiPhieuXuat>(Search.ShowSearchValue);
                    apiResponse = Utility.GetListData<v_dm_LoaiPhieuXuat>(Search.ClassName, Search.ShowSearchValue, Search.SearchString, Utility.LOC_ID);
                    Search = GetData<v_dm_LoaiPhieuXuat>(apiResponse, Search);
                    break;
                case API.dm_DonViTinh:
                    Search.TitleSearch = Utility.GetTitleFrom(API.dm_DonViTinh);
                    Search.ShowSearchValue = Utility.GetShowSearchValue<dm_DonViTinh>(Search.ShowSearchValue);
                    apiResponse = Utility.GetListData<v_dm_DonViTinh>(Search.ClassName, Search.ShowSearchValue, Search.SearchString, Utility.LOC_ID);
                    Search = GetData<v_dm_DonViTinh>(apiResponse, Search);
                    break;
                case API.AspNetUser:
                    Search.TitleSearch = Utility.GetTitleFrom(API.AspNetUser);
                    Search.ShowSearchValue = Utility.GetShowSearchValue<AspNetUsers>(Search.ShowSearchValue);
                    apiResponse = Utility.GetListData<v_AspNetUsers>(Search.ClassName, Search.ShowSearchValue, Search.SearchString, Utility.LOC_ID);
                    Search = GetData<v_AspNetUsers>(apiResponse, Search);
                    break;
                case API.dm_Kho:
                    Search.TitleSearch = Utility.GetTitleFrom(API.dm_Kho);
                    Search.ShowSearchValue = Utility.GetShowSearchValue<dm_Kho>(Search.ShowSearchValue);
                    apiResponse = Utility.GetListData<v_dm_Kho>(Search.ClassName, Search.ShowSearchValue, Search.SearchString, Utility.LOC_ID);
                    Search = GetData<v_dm_Kho>(apiResponse, Search);
                    break;
                case API.web_NhomQuyen:
                    Search.TitleSearch = Utility.GetTitleFrom(API.web_NhomQuyen);
                    Search.ShowSearchValue = Utility.GetShowSearchValue<web_NhomQuyen>(Search.ShowSearchValue);
                    apiResponse = Utility.GetListData<v_web_NhomQuyen>(Search.ClassName, Search.ShowSearchValue, Search.SearchString, Utility.LOC_ID);
                    Search = GetData<v_web_NhomQuyen>(apiResponse, Search);
                    break;
                case API.dm_PhongBan:
                    Search.TitleSearch = Utility.GetTitleFrom(API.dm_PhongBan);
                    Search.ShowSearchValue = Utility.GetShowSearchValue<dm_PhongBan>(Search.ShowSearchValue);
                    apiResponse = Utility.GetListData<v_dm_PhongBan>(Search.ClassName, Search.ShowSearchValue, Search.SearchString, Utility.LOC_ID);
                    Search = GetData<v_dm_PhongBan>(apiResponse, Search);
                    break;
            }
            Search.listSearch = Utility.listSearch;
            return Json(Search, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult SearchCode(string KeyCode)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (Utility.KiemTra())
            {
                apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Admin");
                return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
            }
            KeyCode = KeyCode.Trim().ToUpper();
            string NameController = "";
            if (KeyCode.StartsWith("PDH"))
            {
                NameController = API.ct_PhieuDatHang;
                apiResponse = GetValue<v_v_ct_PhieuDatHang>(apiResponse, NameController, KeyCode);
                if(apiResponse.Detail != null)
                   apiResponse.ID = (apiResponse.Detail as v_v_ct_PhieuDatHang).ID;
            }
            if (KeyCode.StartsWith("PT"))
            {
                NameController = API.ct_PhieuThu;
                apiResponse = GetValue<v_v_ct_PhieuThu>(apiResponse, NameController, KeyCode);
                if (apiResponse.Detail != null)
                    apiResponse.ID = (apiResponse.Detail as v_v_ct_PhieuThu).ID;
            }
            if (KeyCode.StartsWith("PC"))
            {
                NameController = API.ct_PhieuChi;
                apiResponse = GetValue<v_v_ct_PhieuChi>(apiResponse, NameController, KeyCode);
                if (apiResponse.Detail != null)
                    apiResponse.ID = (apiResponse.Detail as v_v_ct_PhieuChi).ID;
            }
            if (KeyCode.StartsWith("PN"))
            {
                NameController = API.ct_PhieuNhap;
                apiResponse = GetValue<v_v_ct_PhieuNhap>(apiResponse, NameController, KeyCode);
                if (apiResponse.Detail != null)
                    apiResponse.ID = (apiResponse.Detail as v_v_ct_PhieuNhap).ID;
            }
            if (KeyCode.StartsWith("PX"))
            {
                NameController = API.ct_PhieuXuat;
                apiResponse = GetValue<v_v_ct_PhieuXuat>(apiResponse, NameController, KeyCode);
                if (apiResponse.Detail != null)
                    apiResponse.ID =  (apiResponse.Detail as v_v_ct_PhieuXuat).ID;
            }
            if (KeyCode.StartsWith("PGH"))
            {
                NameController = API.ct_PhieuGiaoHang;
                apiResponse = GetValue<v_v_ct_PhieuGiaoHang>(apiResponse, NameController, KeyCode);
                if (apiResponse.Detail != null)
                    apiResponse.ID = (apiResponse.Detail as v_v_ct_PhieuGiaoHang).ID;
            }
            if (!string.IsNullOrEmpty(apiResponse.ID))
            {
                apiResponse.URL = Url.Action("Index", NameController, new { MAPHIEU = KeyCode, IDCODE = apiResponse.ID });
            }
            else
            {
                apiResponse.Message = "Không tìm thấy phiếu";
                apiResponse.Success = true;
            }    
                
            apiResponse.NAME = NameController;
            return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
        }

        private ApiResponse GetValue<T>(ApiResponse apiResponse, string NameController, string KeyCode)
        {
            apiResponse = Utility.GetListDataCode<T>(NameController, "MAPHIEU.ToUpper() == @0", KeyCode.ToUpper(), Utility.LOC_ID);
            if (!apiResponse.Success)
            {
                TempData["TitleError"] = apiResponse.Message;
                apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                return apiResponse;
            }
            if (apiResponse.Data != null)
            {
                List<T> lst = apiResponse.Data as List<T>;
                if (lst != null && lst.Count > 0)
                    apiResponse.Detail = lst.FirstOrDefault();
            }

            return apiResponse;
        }
    }
}
