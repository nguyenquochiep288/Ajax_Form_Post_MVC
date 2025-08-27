using DatabaseTHP;
using DatabaseTHP.StoredProcedure.Parameter;
using MVC_QuanLyTHP.Controllers;
using Newtonsoft.Json;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.DynamicData;
using System.Web.SessionState;
using System.Linq.Dynamic;
using System.Reflection;
using DatabaseTHP.Class;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Diagnostics.Eventing.Reader;
using System.Web.Mvc;
using System.Net;
using static System.Net.WebRequestMethods;
using MVC_QuanLyTHP.Models;
using System.Globalization;
using System.Threading;
using System.Web.Caching;
using System.Web.Configuration;
using System.Diagnostics;
using System.Web.UI.WebControls;
using System.Runtime.Remoting.Contexts;
using System.Web.UI;
using System.Security.Policy;
using System.Reflection.Emit;
using System.IO;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.ReportAppServer.DataDefModel;
using static System.Net.Mime.MediaTypeNames;
using System.Data;
using System.Net.Sockets;
using MVC_QuanLyTHP.Report;
using Microsoft.VisualBasic.FileIO;
using DatabaseTHP.StoredProcedure;

namespace MVC_QuanLyTHP.Class
{
    public class Utility
    {
        private static ReportClass report;
        public static ReportClass Report
        {
            get { return GetReport(); }
            set { report = value; }
        }
        public static ReportClass GetReport()
        {
            try
            {
                if (HttpContext.Current.Session[Sessions.Report] != null && !string.IsNullOrEmpty(HttpContext.Current.Session[Sessions.Report].ToString()))
                    report = (ReportClass)HttpContext.Current.Session[Sessions.Report];
            }
            catch (Exception ex)
            {
                Utility.WriteLog("GetReport", MethodBase.GetCurrentMethod().Name, ex);
            }
            return report;
        }

        private static Double latitude;
        public static Double Latitude
        {
            get { return GetLatitude(); }
            set { latitude = value; }
        }
        public static Double GetLatitude()
        {
            try
            {
                if (HttpContext.Current.Session[Sessions.Latitude] != null && !string.IsNullOrEmpty(HttpContext.Current.Session[Sessions.Latitude].ToString()))
                    latitude = (Double)HttpContext.Current.Session[Sessions.Latitude];
            }
            catch (Exception ex)
            {
                Utility.WriteLog("GetLatitude", MethodBase.GetCurrentMethod().Name, ex);
            }
            return latitude;
        }

        private static Double longitude;
        public static Double Longitude
        {
            get { return GetLongitude(); }
            set { longitude = value; }
        }
        public static Double GetLongitude()
        {
            try
            {
                if (HttpContext.Current.Session[Sessions.Longitude] != null && !string.IsNullOrEmpty(HttpContext.Current.Session[Sessions.Longitude].ToString()))
                    longitude = (Double)HttpContext.Current.Session[Sessions.Longitude];
            }
            catch (Exception ex)
            {
                Utility.WriteLog("GetLongitude", MethodBase.GetCurrentMethod().Name, ex);
            }
            return longitude;
        }
        #region Hiển thị
        public static string ThemTab = "Thêm thẻ mới";
        public static string ThucHien = "Thực hiện";
        public static string XemBaoCaoTrenThietBiDiDong = "Xem báo cáo";
        public static string QuayLai = "Quay lại";
        public static string Dong = "Đóng";
        public static string Them = "Thêm";
        public static string Xoa = "";//"Delete";
        public static string Sua = "";//"Edit";
        public static string TimKiem = "Tìm kiếm...";
        public static string CapNhat = "Cập nhật";
        public static string In = "";//"Print";
        public static string HoanTat = "Hoàn tất";
        public static string MoHoanTat = "Mở phiếu";
        public static string GiaoHang = "Giao hàng";
        public static string DaGiaoHang = "Đã giao hàng";
        public static string ChuaGiaoHang = "Chưa giao hàng";
        public static string TraHang = "Trả hàng";
        public static string ThuTien = "Thu tiền";
        public static string XuatExcel = "Xuất Excel";
        public static string ThemHomNay = "Thêm hôm nay";
        public static string ThemNgayMai = "Thêm ngày mai";
        public static string ThemNgayMot = "Thêm ngày mốt";
        public static string PhieuChi = "Phiếu chi";
        public static string ChuyenKPI = "Sao chép dữ liệu -> T" + CurrentTime.Month.ToString("00") + "/" + CurrentTime.Year.ToString();
        public static string GetTitleChon(string classname)
        {
            return "--- Chọn " + GetTitleFrom(classname).ToLower() + " ---";
        }
        #endregion

        #region Chuỗi menu
        public static string menu = "";
        public static string Menu
        {
            get { return GetMenuText(); }
            set { menu = value; }
        }
        public static string GetMenuText()
        {
            menu = "";
            try
            {
                if (HttpContext.Current.Session[Sessions.Menu] != null && !string.IsNullOrEmpty(HttpContext.Current.Session[Sessions.Menu].ToString()))
                    menu = HttpContext.Current.Session[Sessions.Menu].ToString();
            }
            catch (Exception ex)
            {
                Utility.WriteLog("GetMenuText", MethodBase.GetCurrentMethod().Name, ex);
            }
            return menu;
        }
        #endregion

        //Công ty
        public static string LOC_ID = "02";
        //ORDER BY tăng dần ASC, giảm dần DESC
        public static string OrderBy = "ASC";
        //Contains Chứa trong, StartsWith bắt đầu
        public static string TypeSeacrh = "Contains";

        #region //Danh sách columm hiển thị trên thanh tìm kiếm
        private static List<Tuple<string, string, bool, int>> ListSearch;
        public static List<Tuple<string, string, bool, int>> listSearch
        {
            get { return GetlistSearch(); }
            set { ListSearch = value; }
        }
        public static List<Tuple<string, string, bool, int>> GetlistSearch()
        {
            ListSearch = new List<Tuple<string, string, bool, int>>();
            try
            {
                if (HttpContext.Current.Session[Sessions.listSearch] != null && !string.IsNullOrEmpty(HttpContext.Current.Session[Sessions.listSearch].ToString()))
                    ListSearch = (List<Tuple<string, string, bool, int>>)HttpContext.Current.Session[Sessions.listSearch];
            }
            catch (Exception ex)
            {
                Utility.WriteLog("GetlistSearch", MethodBase.GetCurrentMethod().Name, ex);
            }
            return ListSearch;
        }
        #endregion

        //Dãy số lượng show
        public static int[] arrShow = { 50, 100, 200, 500, 1000 };
        //Địa chỉ url server
        public static string URL = clsMaHoa.Decrypt(ConfigurationManager.ConnectionStrings["httpServer"].ConnectionString, clsMaHoa.PassMaHoa);//"http://118.69.53.15:81/api/";//"http://localhost:81/api/";// //clsMaHoa.Decrypt(ConfigurationManager.ConnectionStrings["httpServer"].ConnectionString, clsMaHoa.PassMaHoa);// +"http://localhost:81/api/";
        public static string UrlWebsite = clsMaHoa.Decrypt(ConfigurationManager.ConnectionStrings["UrlWebsite"].ConnectionString, clsMaHoa.PassMaHoa);
        public static DateTime CurrentTime { get { return DateTime.Now; } }

        #region Get int Width
        private static int intWidth;
        public static int IntWidth
        {
            get { return GetIntWidth(); }
            set { intWidth = value; }
        }
        public static int GetIntWidth()
        {
            intWidth = 1;
            try
            {
                if (HttpContext.Current.Session[Sessions.IntWidth] != null && !string.IsNullOrEmpty(HttpContext.Current.Session[Sessions.IntWidth].ToString()))
                    intWidth = Convert.ToInt32(HttpContext.Current.Session[Sessions.IntWidth]);
            }
            catch (Exception ex)
            {
                intWidth = PageSizeDefaut;
                Utility.WriteLog("GetIntWidth", MethodBase.GetCurrentMethod().Name, ex);
            }
            return intWidth;
        }
        #endregion

        #region Get styleWidth level
        public static string stypeWidth_Level1;
        public static string StypeWidth_Level1
        {
            get { return GetStyleWidth(); }
            set { stypeWidth_Level1 = value; }
        }
        public static string GetStyleWidth()
        {
            stypeWidth_Level1 = "style='width: 90%; margin-left: 5%;'";
            try
            {
                if (HttpContext.Current.Session[Sessions.StypeWidth_Level1] != null && !string.IsNullOrEmpty(HttpContext.Current.Session[Sessions.StypeWidth_Level1].ToString()))
                    stypeWidth_Level1 = HttpContext.Current.Session[Sessions.StypeWidth_Level1].ToString();
            }
            catch (Exception ex)
            {
                Utility.WriteLog("GetStyleWidth", MethodBase.GetCurrentMethod().Name, ex);
            }
            return stypeWidth_Level1;
        }

        public static string stypeWidth_Level2;
        public static string StypeWidth_Level2
        {
            get { return GetStypeWidth_Level2(); }
            set { stypeWidth_Level2 = value; }
        }

        public static string GetStypeWidth_Level2()
        {
            stypeWidth_Level2 = "style='width: 84%; margin-left: 10%;'";
            try
            {
                if (HttpContext.Current.Session[Sessions.StypeWidth_Level2] != null && !string.IsNullOrEmpty(HttpContext.Current.Session[Sessions.StypeWidth_Level2].ToString()))
                    stypeWidth_Level2 = HttpContext.Current.Session[Sessions.StypeWidth_Level2].ToString();
            }
            catch (Exception ex)
            {
                Utility.WriteLog("GetStypeWidth_Level2", MethodBase.GetCurrentMethod().Name, ex);
            }
            return stypeWidth_Level2;
        }

        public static string stypeWidth_Level3;
        public static string StypeWidth_Level3
        {
            get { return GetStypeWidth_Level3(); }
            set { stypeWidth_Level3 = value; }
        }

        public static string GetStypeWidth_Level3()
        {
            stypeWidth_Level3 = "style='width: 78%; margin-left: 11%;'";
            try
            {
                if (HttpContext.Current.Session[Sessions.StypeWidth_Level2] != null && !string.IsNullOrEmpty(HttpContext.Current.Session[Sessions.StypeWidth_Level2].ToString()))
                    stypeWidth_Level3 = HttpContext.Current.Session[Sessions.StypeWidth_Level2].ToString();
            }
            catch (Exception ex)
            {
                Utility.WriteLog("GetStypeWidth_Level3", MethodBase.GetCurrentMethod().Name, ex);
            }
            return stypeWidth_Level3;
        }
        #endregion

        #region Danh sách mô tả cột
        public static List<view_web_NoteClass> GetNoteClass(Boolean bolCache = false)
        {
            List<view_web_NoteClass> lstNoteClass = new List<view_web_NoteClass>();
            if (!bolCache && HttpContext.Current.Session[Sessions.lstNoteClass] != null)
            {
                lstNoteClass = (List<view_web_NoteClass>)HttpContext.Current.Session[Sessions.lstNoteClass];
            }
            else
            {
                HttpContext.Current.Session[Sessions.lstNoteClass] = lstNoteClass = Utility.GetNoteClasss<view_web_NoteClass>();
            }
            return lstNoteClass ?? new List<view_web_NoteClass>();
        }
        #endregion

        #region Danh sách câu thông báo
        public static List<web_ThongBao> GetThongBao(Boolean bolCache = false)
        {
            List<web_ThongBao> lstNoteClass = new List<web_ThongBao>();
            if (!bolCache && HttpContext.Current.Session[Sessions.lstThongBao] != null)
            {
                lstNoteClass = (List<web_ThongBao>)HttpContext.Current.Session[Sessions.lstThongBao];
            }
            else
            {
                HttpContext.Current.Session[Sessions.lstThongBao] = lstNoteClass = Utility.GetThongBao<web_ThongBao>();
            }
            return lstNoteClass ?? new List<web_ThongBao>();
        }
        #endregion

        #region Danh sách Menu
        public static List<v_web_Menu> GetMenu(Boolean bolCache = false)
        {
            List<v_web_Menu> lstMenu = new List<v_web_Menu>();
            if (!bolCache && HttpContext.Current.Session[Sessions.lstMenu] != null)
            {
                lstMenu = (List<v_web_Menu>)HttpContext.Current.Session[Sessions.lstMenu];
            }
            else
            {
                var apiResponse = Utility.GetListData<v_web_Menu>(API.web_Menu);
                if (!apiResponse.Success)
                {
                    return new List<v_web_Menu>();
                }
                HttpContext.Current.Session[Sessions.lstMenu] = lstMenu = apiResponse.Data as List<v_web_Menu>;
            }
            return lstMenu ?? new List<v_web_Menu>();
        }
        #endregion

        #region Danh sách ngân hàng VietQR
        //public static List<Datum> GetBankVietQR(Boolean bolCache = false)
        //{
        //    List<Datum> lstMenu = new List<Datum>();
        //    if (!bolCache && HttpContext.Current.Session[Sessions.BankVietQR] != null)
        //    {
        //        lstMenu = (List<Datum>)HttpContext.Current.Session[Sessions.BankVietQR];
        //    }
        //    else
        //    {
        //        var apiResponse = Utility.GetListData<Datum>(API.dm_TaiKhoanNganHang);
        //        if (!apiResponse.Success)
        //        {
        //            return new List<Datum>();
        //        }
        //        HttpContext.Current.Session[Sessions.BankVietQR] = lstMenu = apiResponse.Data as List<Datum>;
        //    }
        //    return lstMenu ?? new List<Datum>();
        //}
        #endregion

        #region Danh sách Phân quyền
        public static List<view_web_PhanQuyen> GetPhanQuyen(Boolean bolCache = false)
        {
            List<view_web_PhanQuyen> lstPhanQuyen = new List<view_web_PhanQuyen>();
            if (!bolCache && HttpContext.Current.Session[Sessions.lstPhanQuyen] != null)
            {
                lstPhanQuyen = (List<view_web_PhanQuyen>)HttpContext.Current.Session[Sessions.lstPhanQuyen];
            }
            else
            {
                var apiResponse = Utility.GetPhanQuyen<view_web_PhanQuyen>();

                HttpContext.Current.Session[Sessions.lstPhanQuyen] = lstPhanQuyen = apiResponse;
            }
            return lstPhanQuyen ?? new List<view_web_PhanQuyen>();
        }

        public static List<T> GetPhanQuyen<T>()
        {
            List<T> lstPage = new List<T>();
            HttpResponseMessage response = null;
            try
            {
                var client = new HttpClient();
                response = client.GetAsync(URL + "Accounts/GetPhanQuyen" + "/" + Utility.LOC_ID + "/" + HttpContext.Current.Session[Sessions.idNhomQuyen].ToString()).Result;
                if (response.IsSuccessStatusCode)
                {
                    var dataObjects = response.Content.ReadAsStringAsync().Result;
                    ApiResponse apiResponse = JsonConvert.DeserializeObject<ApiResponse>(dataObjects);
                    lstPage = JsonConvert.DeserializeObject<List<T>>(apiResponse.Data.ToString());
                }
            }
            catch (Exception ex)
            {
                Utility.WriteLog(response != null ? response.RequestMessage.RequestUri.ToString() : "GetPhanQuyen", MethodBase.GetCurrentMethod().Name, ex);
                lstPage = new List<T>();
            }
            return lstPage;
        }
        #endregion

        #region Lấy tên hiển thị
        public static string GetColumnName(string classname, string columnname)
        {

            view_web_NoteClass objNoteClass = null;
            List<view_web_NoteClass> lstNoteClass = GetNoteClass();
            if (lstNoteClass != null)
                objNoteClass = lstNoteClass.Where(s => !string.IsNullOrEmpty(s.NAMECOLUMN) && !string.IsNullOrEmpty(s.CONTROLLER) && s.CONTROLLER.ToLower() == classname.ToLower() && s.NAMECOLUMN.ToLower() == columnname.ToLower()).FirstOrDefault();
            if (objNoteClass != null)
                return !string.IsNullOrEmpty(objNoteClass.DISPLAYNAME) ? objNoteClass.DISPLAYNAME : columnname;
            return columnname;
        }

        public static string GetTitleFrom(string classname)
        {
            view_web_NoteClass objNoteClass = null;
            List<view_web_NoteClass> lstNoteClass = GetNoteClass();
            if (lstNoteClass != null)
                objNoteClass = lstNoteClass.Where(s => !string.IsNullOrEmpty(s.NAMECOLUMN) && !string.IsNullOrEmpty(s.CONTROLLER) && s.CONTROLLER.ToLower() == classname.ToLower()).FirstOrDefault();
            if (objNoteClass != null)
                return !string.IsNullOrEmpty(objNoteClass.NAMEHEADER) ? objNoteClass.NAMEHEADER : classname;
            return classname;
        }
        #endregion

        #region Lấy danh sách hiển thị trên form
        public static List<T> GetNoteClasss<T>()
        {
            List<T> lstPage = new List<T>();
            try
            {
                var client = new HttpClient();
                var response = client.GetAsync(URL + "Accounts/GetNoteClass").Result;
                if (response.IsSuccessStatusCode)
                {
                    var dataObjects = response.Content.ReadAsStringAsync().Result;
                    ApiResponse apiResponse = JsonConvert.DeserializeObject<ApiResponse>(dataObjects);
                    lstPage = JsonConvert.DeserializeObject<List<T>>(apiResponse.Data.ToString());
                }
            }
            catch (Exception ex)
            {
                Utility.WriteLog("Utility", MethodBase.GetCurrentMethod().Name, ex);
                lstPage = new List<T>();
            }
            return lstPage;
        }

        public static List<T> GetThongBao<T>()
        {
            List<T> lstPage = new List<T>();
            try
            {
                var client = new HttpClient();
                var response = client.GetAsync(URL + "ThongBao").Result;
                if (response.IsSuccessStatusCode)
                {
                    var dataObjects = response.Content.ReadAsStringAsync().Result;
                    ApiResponse apiResponse = JsonConvert.DeserializeObject<ApiResponse>(dataObjects);
                    lstPage = JsonConvert.DeserializeObject<List<T>>(apiResponse.Data.ToString());
                }
            }
            catch (Exception ex)
            {
                Utility.WriteLog("Utility", MethodBase.GetCurrentMethod().Name, ex);
                lstPage = new List<T>();
            }
            return lstPage;
        }
        #endregion

        #region Page Size
        public static int PageSizeDefaut = arrShow[0];
        public static int GetPageSize()
        {
            int PageSize = PageSizeDefaut;
            try
            {
                if (HttpContext.Current.Session[Sessions.PageSize] != null && !string.IsNullOrEmpty(HttpContext.Current.Session[Sessions.PageSize].ToString()))
                    PageSize = Convert.ToInt32(HttpContext.Current.Session[Sessions.PageSize]);
            }
            catch (Exception ex)
            {
                Utility.WriteLog("Utility", MethodBase.GetCurrentMethod().Name, ex);
                PageSize = PageSizeDefaut;
            }
            return PageSize;
        }
        #endregion

        #region Thời gian hết hạn token
        private static DateTime expires;
        public static DateTime Expires
        {
            get { return GetExpires(); }
            set { expires = value; }
        }

        private static DateTime GetExpires()
        {
            DateTime dtExpires = new DateTime();
            try
            {
                if (HttpContext.Current.Session[Sessions.Expires] != null && !string.IsNullOrEmpty(HttpContext.Current.Session[Sessions.Expires].ToString()))
                    dtExpires = Convert.ToDateTime(HttpContext.Current.Session[Sessions.Expires]);
            }
            catch (Exception ex)
            {
                Utility.WriteLog("Utility", MethodBase.GetCurrentMethod().Name, ex);
            }
            return dtExpires;
        }
        #endregion

        #region Token
        private static string token;
        public static string Token
        {
            get { return GetToken(); }
            set { token = value; }
        }

        private static string GetToken(Boolean bolCache = false)
        {
            string strToken = string.Empty;
            try
            {
                if (GetExpires() < CurrentTime || bolCache)
                {
                    if (HttpContext.Current.Session[Sessions.Login_Model] != null)
                    {
                        Login_Model model = (Login_Model)HttpContext.Current.Session[Sessions.Login_Model];
                        ApiResponse apiResponse = Utility.Login(model.user, model.pass);
                        if (apiResponse.Success)
                        {
                            SetSession(apiResponse, model, null);
                        }
                    }
                }
                if (HttpContext.Current.Session[Sessions.Token] != null && !string.IsNullOrEmpty(HttpContext.Current.Session[Sessions.Token].ToString()))
                    strToken = HttpContext.Current.Session[Sessions.Token].ToString();
            }
            catch (Exception ex)
            {
                Utility.WriteLog("Utility", MethodBase.GetCurrentMethod().Name, ex);
            }
            return strToken;
        }
        #endregion

        #region Login
        public static ApiResponse Login(string username, string password)
        {
            ApiResponse apiResponse = new ApiResponse();
            HttpResponseMessage response = null;
            try
            {
                SignInModel model = new SignInModel();
                model.UserName = username;
                model.Password = clsMaHoa.Encrypt(password, clsMaHoa.PassMaHoa);
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(model);
                var data = new System.Net.Http.StringContent(json, Encoding.UTF8, "application/json");

                var client = new HttpClient();
                response = client.PostAsync(URL + "Accounts/Login", data).Result;

                if (response.IsSuccessStatusCode)
                {
                    var dataObjects = response.Content.ReadAsStringAsync().Result;
                    apiResponse = JsonConvert.DeserializeObject<ApiResponse>(dataObjects);
                }
                else
                {
                    apiResponse.Message = GetErrorServer(response);
                }
            }
            catch (Exception ex)
            {
                Utility.WriteLog(response != null ? response.RequestMessage.RequestUri.ToString() : "Login", MethodBase.GetCurrentMethod().Name, ex, "");
                apiResponse = new ApiResponse
                {
                    Success = false,
                    Message = ex.Message,
                    Data = ""
                };
            }
            return apiResponse;
        }
        #endregion

        #region Get max ID
        public static Int32 GetMaxID<T>(T ovjTable, string LOC_ID = "", string NgayLap = "")
        {
            List<T> lstPage = new List<T>();
            try
            {

                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + Token);
                var response = client.GetAsync(URL + "GetIDMax/" + typeof(T).Name + "/" + LOC_ID + "/" + NgayLap.Replace("-","")).Result;

                if (response.IsSuccessStatusCode)
                {
                    var dataObjects = response.Content.ReadAsStringAsync().Result;
                    ApiResponse apiResponse = JsonConvert.DeserializeObject<ApiResponse>(dataObjects);

                    int n = 0;
                    if (int.TryParse(apiResponse.Data.ToString(), out n))
                    {
                        return (n + 1);
                    }
                    return 0;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                ApiResponse apiResponse = new ApiResponse
                {
                    Success = false,
                    Message = ex.Message,
                    Data = ""
                };
                return 0;
            }
        }


        #endregion

        #region Insert, Update, Delete, GetDetail, GetListData
        public static ApiResponse GetListDataCode<T>(string name = "Books", string ShowSearchValue = "", string SearchString = "", string LOC_ID = "")
        {
            ApiResponse apiResponse = new ApiResponse();
            List<T> lstPage = new List<T>();
            var client = new HttpClient();
            HttpResponseMessage response = null;
            try
            {
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + Token);
                response = client.GetAsync(URL + name + "/" + LOC_ID + "/1/" + ShowSearchValue + "/"+ SearchString.ToLower()).Result;

                if (response.IsSuccessStatusCode)
                {
                    var dataObjects = response.Content.ReadAsStringAsync().Result;
                    apiResponse = JsonConvert.DeserializeObject<ApiResponse>(dataObjects);
                    if (apiResponse.Data != null && !string.IsNullOrEmpty(apiResponse.Data.ToString()))
                        lstPage = JsonConvert.DeserializeObject<List<T>>(apiResponse.Data.ToString());
                    apiResponse.Data = lstPage;
                }
                else
                {
                    apiResponse.Message = GetErrorServer(response);
                }
            }
            catch (Exception ex)
            {
                Utility.WriteLog(response != null ? response.RequestMessage.RequestUri.ToString() : "GetListData", MethodBase.GetCurrentMethod().Name, ex, "");
                apiResponse = new ApiResponse
                {
                    Success = false,
                    Message = ex.Message,
                    Data = ""
                };
            }
            return apiResponse;
        }

        public static ApiResponse GetListData<T>(string name = "Books", string ShowSearchValue = "", string SearchString = "", string LOC_ID = "", string TypeSearch = "")
        {
            ApiResponse apiResponse = new ApiResponse();
            List<T> lstPage = new List<T>();
            var client = new HttpClient();
            HttpResponseMessage response = null;
            try
            {
                string ShowSearchValue_Decrypt = clsMaHoa.Decrypt(ShowSearchValue, clsMaHoa.PassMaHoa);
                string KeyWhere = string.Empty;
                List<view_web_NoteClass> lstNoteClass = GetNoteClass();
                if (lstNoteClass != null)
                    lstNoteClass = lstNoteClass.Where(s => !string.IsNullOrEmpty(s.NAMECLASS) && s.NAMECLASS.ToLower() == typeof(T).Name.Replace("v_", "").ToLower() && s.ISSEARCH).ToList();
                if (!string.IsNullOrEmpty(ShowSearchValue_Decrypt) && !string.IsNullOrEmpty(SearchString))
                {
                    if (string.IsNullOrEmpty(TypeSearch))
                    {
                        var properties = typeof(T).GetProperties();

                        if (ShowSearchValue_Decrypt != "ALL")
                        {
                            PropertyInfo property = null;
                            var valu = lstNoteClass.Where(e => e.NAMECOLUMN.ToLower() == ShowSearchValue_Decrypt.ToLower()).FirstOrDefault();
                            if (valu != null && !string.IsNullOrEmpty(valu.REPLACESEARCH))
                            {
                                property = properties.Where(s => s.Name.ToUpper() == valu.REPLACESEARCH.ToUpper()).FirstOrDefault();
                            }

                            if (property == null)
                                property = properties.Where(s => s.Name.ToUpper() == ShowSearchValue_Decrypt.ToUpper()).FirstOrDefault();
                            else
                                ShowSearchValue_Decrypt = valu.REPLACESEARCH;

                            string value = property.PropertyType.GenericTypeArguments.Count() > 0 ? property.PropertyType.GenericTypeArguments[0].Name.ToUpper() : property.PropertyType.Name.ToUpper();
                            switch (value)
                            {
                                case "STRING":
                                    KeyWhere = ShowSearchValue_Decrypt + ".ToLower()." + Utility.TypeSeacrh + "(@0)";
                                    break;
                                case "BOOLEAN":
                                    int bol = 0;
                                    int.TryParse(SearchString, out bol);

                                    if (bol == 0)
                                        SearchString = SearchString.Replace("0", "false");
                                    else if (bol == 1)
                                        SearchString = SearchString.Replace("1", "true");

                                    KeyWhere = ShowSearchValue_Decrypt + ".ToString().ToLower().Contains(@0)";
                                    break;
                                case "INT32":
                                    KeyWhere = ShowSearchValue_Decrypt + " == @0";
                                    int n = 0;
                                    int.TryParse(SearchString, out n);
                                    SearchString = n.ToString();
                                    break;
                                case "DOUBLE":
                                    double d = 0;
                                    double.TryParse(SearchString, out d);
                                    SearchString = d.ToString();
                                    break;
                                case "DATETIME":
                                    DateTime date = CurrentTime;
                                    DateTime.TryParse(SearchString, out date);
                                    KeyWhere = ShowSearchValue_Decrypt + " >= @0";
                                    SearchString = date.ToString("dd/MM/yyyy");
                                    break;
                                default:
                                    KeyWhere = ShowSearchValue_Decrypt + ".ToString().ToLower()." + Utility.TypeSeacrh + "(@0)";
                                    break;
                            }
                        }
                        else
                        {
                            if (lstNoteClass == null)
                            {
                                foreach (var itm in properties)
                                {
                                    string value = itm.PropertyType.GenericTypeArguments.Count() > 0 ? itm.PropertyType.GenericTypeArguments[0].Name.ToUpper() : itm.PropertyType.Name.ToUpper();
                                    switch (value)
                                    {
                                        case "STRING":
                                            KeyWhere += itm.Name + ".ToLower()." + Utility.TypeSeacrh + "(@0) || ";
                                            break;
                                    }
                                }
                                ShowSearchValue_Decrypt = "";
                            }
                            else
                            {
                                foreach (var itm in lstNoteClass)
                                {
                                    PropertyInfo property = null;
                                    var value1 = lstNoteClass.Where(e => e.NAMECOLUMN.ToLower() == itm.NAMECOLUMN.ToLower()).FirstOrDefault();
                                    if (value1 != null && !string.IsNullOrEmpty(value1.REPLACESEARCH))
                                    {
                                        property = properties.Where(s => s.Name.ToUpper() == value1.REPLACESEARCH.ToUpper()).FirstOrDefault();
                                    }

                                    if (property == null)
                                        property = properties.Where(s => s.Name.ToUpper() == itm.NAMECOLUMN.ToUpper()).FirstOrDefault();

                                    //var property = properties.Where(s => s.Name.ToUpper() == itm.NAMECOLUMN.ToUpper()).FirstOrDefault();
                                    if (property != null)
                                    {
                                        string value = property.PropertyType.GenericTypeArguments.Count() > 0 ? property.PropertyType.GenericTypeArguments[0].Name.ToUpper() : property.PropertyType.Name.ToUpper();
                                        switch (value)
                                        {
                                            case "STRING":
                                                KeyWhere += property.Name + ".ToLower()." + Utility.TypeSeacrh + "(@0) || ";
                                                break;
                                        }
                                    }
                                }
                                var valu = lstNoteClass.Where(e => e.ISSORT).FirstOrDefault();
                                ShowSearchValue_Decrypt = (valu != null ? valu.NAMECOLUMN : "");
                            }
                            if (!string.IsNullOrEmpty(KeyWhere))
                                KeyWhere = KeyWhere.Substring(0, KeyWhere.Length - 4);


                        }
                    }
                    else
                    {
                        KeyWhere = ShowSearchValue_Decrypt + " == @0";
                        SearchString = clsMaHoa.Decrypt(SearchString, clsMaHoa.PassMaHoa);
                    }

                }
                else
                {
                    if (ShowSearchValue_Decrypt.Contains("ALL"))
                        ShowSearchValue_Decrypt = "";
                }



                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + Token);
                SearchString = SearchString.Replace("/", "%2f");

                response = client.GetAsync(URL + name + (string.IsNullOrEmpty(LOC_ID) ? "" : "/" + LOC_ID) + (string.IsNullOrEmpty(SearchString) || string.IsNullOrEmpty(KeyWhere) ? "" : "/1/" + KeyWhere + "/" + SearchString.ToLower())).Result;

                if (response.IsSuccessStatusCode)
                {
                    var dataObjects = response.Content.ReadAsStringAsync().Result;
                    apiResponse = JsonConvert.DeserializeObject<ApiResponse>(dataObjects);
                    if (apiResponse.Data != null && !string.IsNullOrEmpty(apiResponse.Data.ToString()))
                        lstPage = JsonConvert.DeserializeObject<List<T>>(apiResponse.Data.ToString());
                    if (!string.IsNullOrEmpty(ShowSearchValue_Decrypt))
                        lstPage = lstPage.OrderBy(ShowSearchValue_Decrypt + " " + OrderBy).ToList();
                    apiResponse.Data = lstPage;
                }
                else
                {
                    apiResponse.Message = GetErrorServer(response);
                }
            }
            catch (Exception ex)
            {
                Utility.WriteLog(response != null ? response.RequestMessage.RequestUri.ToString() : "GetListData", MethodBase.GetCurrentMethod().Name, ex, "");
                apiResponse = new ApiResponse
                {
                    Success = false,
                    Message = ex.Message,
                    Data = new List<T>()
                };
            }
            return apiResponse;
        }

        public static ApiResponse GetDetail<T>(string GetValue, string name = "Books")
        {
            ApiResponse apiResponse = new ApiResponse();
            HttpResponseMessage response = null;
            try
            {

                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + Token);
                response = client.GetAsync(URL + name + "/" + GetValue).Result;

                if (response.IsSuccessStatusCode)
                {
                    var dataObjects = response.Content.ReadAsStringAsync().Result;
                    apiResponse = JsonConvert.DeserializeObject<ApiResponse>(dataObjects);
                    if (apiResponse.Data != null)
                        apiResponse.Data = JsonConvert.DeserializeObject<T>(apiResponse.Data.ToString());
                }
                else
                {
                    apiResponse.Message = GetErrorServer(response);
                }
            }
            catch (Exception ex)
            {
                Utility.WriteLog(response != null ? response.RequestMessage.RequestUri.ToString() : "GetDetail", MethodBase.GetCurrentMethod().Name, ex, "");
                apiResponse = new ApiResponse
                {
                    Success = false,
                    Message = ex.Message,
                    Data = ""
                };
            }
            return apiResponse;
        }

        public static ApiResponse Edit<T>(string GetValue, T model, string name = "Books")
        {
            T obj = default(T);
            obj = JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(model));
            ApiResponse apiResponse = new ApiResponse();
            StringContent content = null;
            string strcontent = "";
            HttpResponseMessage response = null;
            try
            {
                strcontent = JsonConvert.SerializeObject(obj);
                content = new StringContent(strcontent, Encoding.UTF8, "application/json");
                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + Token);
                response = client.PutAsync(URL + name + "/" + GetValue, content).Result;

                if (response.IsSuccessStatusCode)
                {
                    var dataObjects = response.Content.ReadAsStringAsync().Result;
                    apiResponse = JsonConvert.DeserializeObject<ApiResponse>(dataObjects);
                }
                else
                {
                    apiResponse.Message = GetErrorServer(response);
                }
            }
            catch (Exception ex)
            {
                Utility.WriteLog(response != null ? response.RequestMessage.RequestUri.ToString() : "Edit", MethodBase.GetCurrentMethod().Name, ex, strcontent);
                apiResponse = new ApiResponse
                {
                    Success = false,
                    Message = ex.Message,
                    Data = ""
                };
            }
            return apiResponse;
        }

        public static ApiResponse Create<T>(T model, string name = "Books")
        {
            T obj = default(T);

            obj = JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(model));
            ApiResponse apiResponse = new ApiResponse();
            StringContent content = null;
            HttpResponseMessage response = null;
            string strcontent = "";

            try
            {
                strcontent = JsonConvert.SerializeObject(obj);
                content = new StringContent(strcontent, Encoding.UTF8, "application/json");
                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + Token);
                response = client.PostAsync(URL + name, content).Result;

                if (response.IsSuccessStatusCode)
                {
                    var dataObjects = response.Content.ReadAsStringAsync().Result;
                    apiResponse = JsonConvert.DeserializeObject<ApiResponse>(dataObjects);
                }
                else
                {
                    apiResponse.Message = GetErrorServer(response);
                }
            }
            catch (Exception ex)
            {
                Utility.WriteLog(response != null ? response.RequestMessage.RequestUri.ToString() : "Create", MethodBase.GetCurrentMethod().Name, ex, strcontent);
                apiResponse = new ApiResponse
                {
                    Success = false,
                    Message = ex.Message,
                    Data = ""
                };
            }
            return apiResponse;
        }

        public static ApiResponse Delete<T>(string GetValue, string name = "Books")
        {
            ApiResponse apiResponse = new ApiResponse();
            HttpResponseMessage response = null;
            try
            {
                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + Token);
                response = client.DeleteAsync(URL + name + "/" + GetValue).Result;

                if (response.IsSuccessStatusCode)
                {
                    var dataObjects = response.Content.ReadAsStringAsync().Result;
                    apiResponse = JsonConvert.DeserializeObject<ApiResponse>(dataObjects);
                }
                else
                {
                    apiResponse.Message = GetErrorServer(response);
                }
            }
            catch (Exception ex)
            {
                Utility.WriteLog(response != null ? response.RequestMessage.RequestUri.ToString() : "Delete", MethodBase.GetCurrentMethod().Name, ex, "");
                apiResponse = new ApiResponse
                {
                    Success = false,
                    Message = ex.Message,
                    Data = ""
                };
            }
            return apiResponse;
        }
        #endregion

        #region Order
        public static ApiResponse GetListDataOrder<T>(string name, DateTime? FromDate, DateTime? ToDate, string ShowSearchValue = "", string SearchString = "", string LOC_ID = "", string TypeSearch = "")
        {
            ApiResponse apiResponse = new ApiResponse();
            List<T> lstPage = new List<T>();
            try
            {
                string ShowSearchValue_Decrypt = clsMaHoa.Decrypt(ShowSearchValue, clsMaHoa.PassMaHoa);
                string KeyWhere = string.Empty;
                if (!string.IsNullOrEmpty(ShowSearchValue_Decrypt) && !string.IsNullOrEmpty(SearchString))
                {
                    if (string.IsNullOrEmpty(TypeSearch))
                    {
                        var properties = typeof(T).GetProperties();
                        var property = properties.Where(s => s.Name.ToUpper() == ShowSearchValue_Decrypt.ToUpper()).FirstOrDefault();
                        string value = property.PropertyType.GenericTypeArguments.Count() > 0 ? property.PropertyType.GenericTypeArguments[0].Name.ToUpper() : property.PropertyType.Name.ToUpper();
                        switch (value)
                        {
                            case "STRING":
                                KeyWhere = ShowSearchValue_Decrypt + ".ToLower()." + Utility.TypeSeacrh + "(@0)";
                                break;
                            case "BOOLEAN":
                                int bol = 0;
                                int.TryParse(SearchString, out bol);

                                if (bol == 0)
                                    SearchString = SearchString.Replace("0", "false");
                                else if (bol == 1)
                                    SearchString = SearchString.Replace("1", "true");

                                KeyWhere = ShowSearchValue_Decrypt + ".ToString().ToLower().Contains(@0)";
                                break;
                            case "INT32":
                                KeyWhere = ShowSearchValue_Decrypt + " == @0";
                                int n = 0;
                                int.TryParse(SearchString, out n);
                                SearchString = n.ToString();
                                break;
                            case "DOUBLE":
                                double d = 0;
                                double.TryParse(SearchString, out d);
                                SearchString = d.ToString();
                                break;
                            case "DATETIME":
                                DateTime date = CurrentTime;
                                DateTime.TryParse(SearchString, out date);
                                KeyWhere = ShowSearchValue_Decrypt + " >= @0";
                                SearchString = date.ToString("dd/MM/yyyy");
                                break;
                            default:
                                KeyWhere = ShowSearchValue_Decrypt + ".ToString().ToLower()." + Utility.TypeSeacrh + "(@0)";
                                break;
                        }

                    }
                    else
                    {
                        KeyWhere = ShowSearchValue_Decrypt + " == @0";
                        SearchString = clsMaHoa.Decrypt(SearchString, clsMaHoa.PassMaHoa);
                    }

                }

                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + Token);
                var response = client.GetAsync(URL + name + "/" + LOC_ID + "/" + FromDate.Value.ToString("yyyy-MM-ddT10:00:00.000Z") + "/" + ToDate.Value.ToString("yyyy-MM-ddT10:00:00.000Z") + (string.IsNullOrEmpty(SearchString) || string.IsNullOrEmpty(KeyWhere) ? "" : "/1/" + KeyWhere + "/" + SearchString.ToLower())).Result;

                if (response.IsSuccessStatusCode)
                {
                    var dataObjects = response.Content.ReadAsStringAsync().Result;
                    apiResponse = JsonConvert.DeserializeObject<ApiResponse>(dataObjects);
                    if (apiResponse.Data != null && !string.IsNullOrEmpty(apiResponse.Data.ToString()))
                        lstPage = JsonConvert.DeserializeObject<List<T>>(apiResponse.Data.ToString());
                    if (!string.IsNullOrEmpty(ShowSearchValue_Decrypt))
                        lstPage = lstPage.OrderBy(ShowSearchValue_Decrypt + " " + OrderBy).ToList();
                    apiResponse.Data = lstPage;
                }
                else
                {
                    apiResponse.Message = GetErrorServer(response);
                }
            }
            catch (Exception ex)
            {
                apiResponse = new ApiResponse
                {
                    Success = false,
                    Message = ex.Message,
                    Data = ""
                };
            }
            return apiResponse;
        }

        public static ApiResponse CreateINV_DEPOSIT<T>(List<T> model, string name = "Books")
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + Token);
                //+ "/" + JsonConvert.SerializeObject((model as INV_DEPOSIT_TEMP).lstINV_DEPOSIT_DTL_TEMP)
                var response = client.PostAsync(URL + name, content).Result;

                if (response.IsSuccessStatusCode)
                {
                    var dataObjects = response.Content.ReadAsStringAsync().Result;
                    apiResponse = JsonConvert.DeserializeObject<ApiResponse>(dataObjects);
                }
                else
                {
                    apiResponse.Message = GetErrorServer(response);
                }
            }
            catch (Exception ex)
            {
                apiResponse = new ApiResponse
                {
                    Success = false,
                    Message = ex.Message,
                    Data = ""
                };
            }
            return apiResponse;
        }

        public static ApiResponse CreateINV_DEPOSIT_TEMP<T>(T model, string name = "Books")
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + Token);
                //+ "/" + JsonConvert.SerializeObject((model as INV_DEPOSIT_TEMP).lstINV_DEPOSIT_DTL_TEMP)
                var response = client.PostAsync(URL + name, content).Result;

                if (response.IsSuccessStatusCode)
                {
                    var dataObjects = response.Content.ReadAsStringAsync().Result;
                    apiResponse = JsonConvert.DeserializeObject<ApiResponse>(dataObjects);
                }
                else
                {
                    apiResponse.Message = GetErrorServer(response);
                }
            }
            catch (Exception ex)
            {
                apiResponse = new ApiResponse
                {
                    Success = false,
                    Message = ex.Message,
                    Data = ""
                };
            }
            return apiResponse;
        }

        public static ApiResponse EditINV_DEPOSIT_TEMP<T>(T model, string name = "Books")
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + Token);
                //+ "/" + JsonConvert.SerializeObject((model as INV_DEPOSIT_TEMP).lstINV_DEPOSIT_DTL_TEMP)
                var response = client.PutAsync(URL + name, content).Result;

                if (response.IsSuccessStatusCode)
                {
                    var dataObjects = response.Content.ReadAsStringAsync().Result;
                    apiResponse = JsonConvert.DeserializeObject<ApiResponse>(dataObjects);
                }
                else
                {
                    apiResponse.Message = GetErrorServer(response);
                }
            }
            catch (Exception ex)
            {
                apiResponse = new ApiResponse
                {
                    Success = false,
                    Message = ex.Message,
                    Data = ""
                };
            }
            return apiResponse;
        }
        #endregion

        #region Execute StoredProcedure
        public static ApiResponse ExecuteStoredProc<T>(SP_Parameter model, string name = "Books")
        {
            ApiResponse apiResponse = new ApiResponse();
            HttpResponseMessage response = null;
            StringContent content = null;
            List<T> lstPage = new List<T>();
            string strcontent = "";
            try
            {
                strcontent = JsonConvert.SerializeObject(model);
                content = new StringContent(strcontent, Encoding.UTF8, "application/json");
                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + Token);
                response = client.PostAsync(URL + name, content).Result;

                if (response.IsSuccessStatusCode)
                {
                    var dataObjects = response.Content.ReadAsStringAsync().Result;
                    apiResponse = JsonConvert.DeserializeObject<ApiResponse>(dataObjects);
                    if (apiResponse.Data != null)
                        lstPage = JsonConvert.DeserializeObject<List<T>>(apiResponse.Data.ToString());
                    apiResponse.Data = lstPage;
                }
                else
                {
                    apiResponse.Message = GetErrorServer(response);
                }
            }
            catch (Exception ex)
            {
                Utility.WriteLog(response != null ? response.RequestMessage.RequestUri.ToString() : "ExecuteStoredProc", MethodBase.GetCurrentMethod().Name, ex, strcontent);
                apiResponse = new ApiResponse
                {
                    Success = false,
                    Message = ex.Message,
                    Data = ""
                };
            }
            return apiResponse;
        }

        public static ApiResponse Save_Map(v_dm_KhachHang model, string name = "Books")
        {
            ApiResponse apiResponse = new ApiResponse();
            HttpResponseMessage response = null;
            StringContent content = null;
            string strcontent = "";
            try
            {
                strcontent = JsonConvert.SerializeObject(model);
                content = new StringContent(strcontent, Encoding.UTF8, "application/json");
                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + Token);
                response = client.PostAsync(URL + name, content).Result;

                if (response.IsSuccessStatusCode)
                {
                    var dataObjects = response.Content.ReadAsStringAsync().Result;
                    apiResponse = JsonConvert.DeserializeObject<ApiResponse>(dataObjects);
                }
                else
                {
                    apiResponse.Message = GetErrorServer(response);
                }
            }
            catch (Exception ex)
            {
                Utility.WriteLog(response != null ? response.RequestMessage.RequestUri.ToString() : "ExecuteStoredProc", MethodBase.GetCurrentMethod().Name, ex, strcontent);
                apiResponse = new ApiResponse
                {
                    Success = false,
                    Message = ex.Message,
                    Data = ""
                };
            }
            return apiResponse;
        }
        #endregion

        #region Execute StoredProcedure Report
        public static ApiResponse ExecuteStoredProc<T>(SP_Parameter_Report model, string name = "Books")
        {
            ApiResponse apiResponse = new ApiResponse();
            HttpResponseMessage response = null;
            StringContent content = null;
            DataTable lstPage = new DataTable();
            string strcontent = "";
            try
            {
                strcontent = JsonConvert.SerializeObject(model);
                content = new StringContent(strcontent, Encoding.UTF8, "application/json");
                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + Token);
                response = client.PostAsync(URL + name, content).Result;

                if (response.IsSuccessStatusCode)
                {
                    var dataObjects = response.Content.ReadAsStringAsync().Result;
                    apiResponse = JsonConvert.DeserializeObject<ApiResponse>(dataObjects);
                    if (apiResponse.Data != null)
                        lstPage = JsonConvert.DeserializeObject<DataTable>(apiResponse.Data.ToString());
                    apiResponse.Data = lstPage;
                }
                else
                {
                    apiResponse.Message = GetErrorServer(response);
                }
            }
            catch (Exception ex)
            {
                Utility.WriteLog(response != null ? response.RequestMessage.RequestUri.ToString() : "ExecuteStoredProc", MethodBase.GetCurrentMethod().Name, ex, strcontent);
                apiResponse = new ApiResponse
                {
                    Success = false,
                    Message = ex.Message,
                    Data = ""
                };
            }
            return apiResponse;
        }

        public static ApiResponse ExecuteStoredProcT<T>(SP_Parameter_Report model, string name = "Books")
        {
            ApiResponse apiResponse = new ApiResponse();
            HttpResponseMessage response = null;
            StringContent content = null;
            List<T> lstPage = new List<T>();
            string strcontent = "";
            try
            {
                strcontent = JsonConvert.SerializeObject(model);
                content = new StringContent(strcontent, Encoding.UTF8, "application/json");
                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + Token);
                response = client.PostAsync(URL + name, content).Result;

                if (response.IsSuccessStatusCode)
                {
                    var dataObjects = response.Content.ReadAsStringAsync().Result;
                    apiResponse = JsonConvert.DeserializeObject<ApiResponse>(dataObjects);
                    if (apiResponse.Data != null)
                        lstPage = JsonConvert.DeserializeObject<List<T>>(apiResponse.Data.ToString());
                    apiResponse.Data = lstPage;
                }
                else
                {
                    apiResponse.Message = GetErrorServer(response);
                }
            }
            catch (Exception ex)
            {
                Utility.WriteLog(response != null ? response.RequestMessage.RequestUri.ToString() : "ExecuteStoredProc", MethodBase.GetCurrentMethod().Name, ex, strcontent);
                apiResponse = new ApiResponse
                {
                    Success = false,
                    Message = ex.Message,
                    Data = ""
                };
            }
            return apiResponse;
        }
        #endregion

        #region Kiểm tra thông tin đăng nhập
        public static Boolean KiemTra(Boolean bolCach = false)
        {
            //en-US
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("vi-VN");
            if (string.IsNullOrEmpty(HttpContext.Current.Session[Sessions.User] == null ? "" : HttpContext.Current.Session[Sessions.User].ToString()) || bolCach)
            {
                string userName = string.Empty;
                string passWord = string.Empty;
                Boolean bolGhiNho = false;

                HttpCookie cookie = HttpContext.Current.Request.Cookies[Cookies.Name];
                if ((cookie != null) && (cookie.Value != ""))
                {
                    string MaHoa = clsMaHoa.Decrypt(cookie.Values[Cookies.User].ToString(), clsMaHoa.PassMaHoa);
                    var lst = MaHoa.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                    if (lst != null && lst.Count() == 2)
                    {
                        userName = lst[0];
                        passWord = lst[1];
                        bolGhiNho = true;
                    }
                }
                if (!string.IsNullOrEmpty(passWord))
                {
                    Login_Model model = new Login_Model
                    {
                        user = userName,
                        pass = passWord,
                        check = bolGhiNho
                    };
                    ApiResponse apiResponse = Utility.Login(model.user, model.pass);
                    if (apiResponse.Success)
                    {
                        SetSession(apiResponse, model, cookie);
                    }
                    else
                    {
                        HttpContext.Current.Session.Clear();
                        return true;
                    }
                }
                if (string.IsNullOrEmpty(HttpContext.Current.Session[Sessions.User] == null ? "" : HttpContext.Current.Session[Sessions.User].ToString()))
                    return true;
                else
                    return false;
            }
            else
            {
                return false;
            }
        }

        public static Boolean KiemTraQuyenMoKhoa()
        {
            if (HttpContext.Current.Session[Sessions.idNhomQuyen] != null && HttpContext.Current.Session[Sessions.idNhomQuyen].ToString() == "-1")
            {
                return true;
            }
            return false;
        }
        #endregion

        #region Ghi log
        public static void WriteLog(object sCls, string MethodName, Exception e, string data = "")
        {
            try
            {
                string IPHost = Dns.GetHostName();
                string ip_Local = "";
                if (!string.IsNullOrEmpty(IPHost))
                    ip_Local = Dns.GetHostByName(IPHost).AddressList[0].ToString();

                string ip_public = GetIPAddress();

                string sCName = "";
                string FullName = "";
                int ix = 0;
                if (sCls != null)
                {
                    if (sCls as string != null)
                        sCName = sCls.ToString();
                    else
                        sCName = sCls.GetType().ToString();
                }
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(e, true);
                try
                {
                    StackFrame stf = trace.GetFrame(0);
                    FullName = ((System.Reflection.MemberInfo)(stf.GetMethod())).DeclaringType != null ? ((System.Reflection.MemberInfo)(stf.GetMethod())).DeclaringType.FullName : "";
                    ix = stf.GetFileLineNumber();
                }
                catch
                {

                }
                LogError newLogError = new LogError();
                newLogError.LOC_ID = LOC_ID;
                newLogError.ID = Guid.NewGuid().ToString();
                newLogError.FULLNAME = FullName;
                newLogError.METHODNAME = MethodName + " - " + sCName;
                newLogError.DATA = data;
                newLogError.MESSAGE = e.Message + " - " + ix;
                newLogError.ID_USER = HttpContext.Current.Session[Sessions.idUser] != null ? HttpContext.Current.Session[Sessions.idUser].ToString() : "";
                newLogError.TIME = CurrentTime;
                newLogError.IP = IPHost + "-" + ip_Local + "-" + ip_public;
                var apiResponse = Utility.Create<LogError>(newLogError, API.LogError);
            }
            catch
            {

            }
        }
        public static void WriteLog(object sCls, string MethodName, string data = "")
        {
            try
            {
                string IPHost = Dns.GetHostName();
                string ip_Local = "";
                if (!string.IsNullOrEmpty(IPHost))
                    ip_Local = Dns.GetHostByName(IPHost).AddressList[0].ToString();

                string ip_public = GetIPAddress();

                string sCName = "";
                string FullName = "";
                int ix = 0;
                if (sCls != null)
                {
                    if (sCls as string != null)
                        sCName = sCls.ToString();
                    else
                        sCName = sCls.GetType().ToString();
                }
               
               
                LogError newLogError = new LogError();
                newLogError.LOC_ID = LOC_ID;
                newLogError.ID = Guid.NewGuid().ToString();
                newLogError.FULLNAME = FullName;
                newLogError.METHODNAME = MethodName + " - " + sCName;
                newLogError.DATA = data;
                newLogError.MESSAGE = data;
                newLogError.ID_USER = HttpContext.Current.Session[Sessions.idUser] != null ? HttpContext.Current.Session[Sessions.idUser].ToString() : "";
                newLogError.TIME = CurrentTime;
                newLogError.IP = IPHost + "-" + ip_Local + "-" + ip_public;
                var apiResponse = Utility.Create<LogError>(newLogError, API.LogError);
            }
            catch
            {

            }
        }
        static string GetIPAddress()
        {
            String address = "";
            try
            {
                WebRequest request = WebRequest.Create("http://checkip.dyndns.org/");
                using (WebResponse response = request.GetResponse())
                using (StreamReader stream = new StreamReader(response.GetResponseStream()))
                {
                    address = stream.ReadToEnd();
                }

                int first = address.IndexOf("Address: ") + 9;
                int last = address.LastIndexOf("</body>");
                address = address.Substring(first, last - first);
            }
            catch
            {

            }
            return address;
        }
        #endregion

        #region Kiểm tra quyền
        public static Boolean KiemTraQuyenAdmin()
        {
            if (HttpContext.Current.Session[Sessions.idNhomQuyen] != null && HttpContext.Current.Session[Sessions.idNhomQuyen].ToString() == "-1")
            {
                return true;
            }
            return false;
        }

        public static Boolean KiemTraQuyen(string MaForm, string MaQuyen, v_web_Menu web_Menu = null)
        {
            if (HttpContext.Current.Session[Sessions.idNhomQuyen] != null && HttpContext.Current.Session[Sessions.idNhomQuyen].ToString() == "-1")
            {
                return true;
            }
            var lstPhanQuyen = Utility.GetPhanQuyen();
            view_web_PhanQuyen PhanQuyen;
            if (web_Menu != null)
                PhanQuyen = lstPhanQuyen.Where(s => s.ID_MENU.Trim() == web_Menu.ID.Trim() && s.MAQUYEN.ToUpper() == MaQuyen.ToUpper() && s.ID_NHOMQUYEN == HttpContext.Current.Session[Sessions.idNhomQuyen].ToString()).FirstOrDefault();
            else
                PhanQuyen = lstPhanQuyen.Where(s => !string.IsNullOrEmpty(s.CONTROLLERNAME) && (s.CONTROLLERNAME ?? "").ToUpper() == MaForm.ToUpper() && s.MAQUYEN.ToUpper() == MaQuyen.ToUpper() && s.ID_NHOMQUYEN == HttpContext.Current.Session[Sessions.idNhomQuyen].ToString()).FirstOrDefault();

            if (PhanQuyen != null)
            {
                return PhanQuyen.TRANGTHAI;
            }
            return false;
        }
        #endregion

        #region Gắn Combobox tìm kiếm
        public static string GetShowSearchValue<T>(string ShowSearchValue)
        {
            try
            {
                ShowSearchValue = ShowSearchValue.Replace(" ", "+");
                List<view_web_NoteClass> lstNoteClass = GetNoteClass();
                if (lstNoteClass != null)
                    lstNoteClass = lstNoteClass.Where(s => !string.IsNullOrEmpty(s.NAMECLASS) && s.NAMECLASS.ToLower() == typeof(T).Name.Replace("v_", "").ToLower() && s.ISSEARCH).ToList();

                if (lstNoteClass == null)
                {
                    var properties = typeof(T).GetProperties();
                    List<Tuple<string, string, bool, int>> newlistSearch = new List<Tuple<string, string, bool, int>>();
                    Tuple<string, string, bool, int> all = new Tuple<string, string, bool, int>(clsMaHoa.Encrypt("ALL", clsMaHoa.PassMaHoa), "Tất cả", false, 0);
                    newlistSearch.Add(all);
                    if (string.IsNullOrEmpty(ShowSearchValue))
                        ShowSearchValue = clsMaHoa.Encrypt("ALL", clsMaHoa.PassMaHoa);
                    int order = 1;
                    foreach (var item in properties)
                    {
                        if (string.IsNullOrEmpty(ShowSearchValue))
                            ShowSearchValue = clsMaHoa.Encrypt(item.Name, clsMaHoa.PassMaHoa);
                        var isVirtual = item.GetAccessors()[0].IsVirtual;
                        order += 1;
                        Tuple<string, string, bool, int> t = new Tuple<string, string, bool, int>(clsMaHoa.Encrypt(item.Name, clsMaHoa.PassMaHoa), item.Name, isVirtual, order);
                        newlistSearch.Add(t);
                    }
                    HttpContext.Current.Session[Sessions.listSearch] = newlistSearch;
                }
                else
                {
                    List<Tuple<string, string, bool, int>> newlistSearch = new List<Tuple<string, string, bool, int>>();
                    Tuple<string, string, bool, int> all = new Tuple<string, string, bool, int>(clsMaHoa.Encrypt("ALL", clsMaHoa.PassMaHoa), "Tất cả", false, 0);
                    newlistSearch.Add(all);
                    if (string.IsNullOrEmpty(ShowSearchValue))
                        ShowSearchValue = clsMaHoa.Encrypt("ALL", clsMaHoa.PassMaHoa);
                    int order = 1;
                    var properties = typeof(T).GetProperties();
                    foreach (var item in lstNoteClass.OrderBy(s => s.STT))
                    {
                        if (string.IsNullOrEmpty(ShowSearchValue))
                            ShowSearchValue = clsMaHoa.Encrypt(item.NAMECOLUMN, clsMaHoa.PassMaHoa);
                        order += 1;
                        Tuple<string, string, bool, int> t = new Tuple<string, string, bool, int>(clsMaHoa.Encrypt(item.NAMECOLUMN, clsMaHoa.PassMaHoa), !string.IsNullOrEmpty(item.DISPLAYNAME) ? item.DISPLAYNAME : item.NAMECOLUMN, true, order);
                        newlistSearch.Add(t);
                    }
                    HttpContext.Current.Session[Sessions.listSearch] = newlistSearch;
                }
            }
            catch (Exception ex)
            {
                Utility.WriteLog("GetShowSearchValue", MethodBase.GetCurrentMethod().Name, ex);
            }
            return ShowSearchValue;
        }
        #endregion

        #region Set Session
        public static void SetSession(ApiResponse apiResponse, Login_Model model, HttpCookie cookie)
        {
            if (model.check && cookie != null)
            {
                cookie = new HttpCookie(Cookies.Name);
                string MaHoa = clsMaHoa.Encrypt(model.user + Environment.NewLine + model.pass, clsMaHoa.PassMaHoa);
                cookie.Values[Cookies.User] = MaHoa;
                cookie.Expires = CurrentTime.AddDays(90);
                HttpContext.Current.Response.Cookies.Add(cookie);
            }

            HttpContext.Current.Session[Sessions.Token] = apiResponse.Data;
            HttpContext.Current.Session[Sessions.Expires] = apiResponse.Expires;
            ApiResponseUser apiResponseUser = JsonConvert.DeserializeObject<ApiResponseUser>(apiResponse.Detail != null ? apiResponse.Detail.ToString() : "{}");
            model.fullname = apiResponseUser.FullName;
            model.iduser = apiResponseUser.idUser;
            HttpContext.Current.Session[Sessions.idUser] = apiResponseUser.idUser;
            HttpContext.Current.Session[Sessions.idNhomQuyen] = apiResponseUser.idNhomQuyen;
            HttpContext.Current.Session[Sessions.Login_Model] = model;
            HttpContext.Current.Session[Sessions.User] = apiResponseUser.UserName;
            ResetCach();
        }
        #endregion

        #region ResetCach
        public static void ResetCach()
        {
            Utility.Reset();
            Utility.GetNoteClass(true);
            Utility.GetMenu(true);
            Utility.GetPhanQuyen(true);
            Utility.GetThongBao(true);
            //Utility.GetBankVietQR(true);
        }
        #endregion

        #region Lấy code lỗi
        private static string GetErrorServer(HttpResponseMessage response)
        {
            string Message = "";
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                GetToken(true);
                Message = "Authorization bị sai! " + response.StatusCode.ToString() + response.ReasonPhrase.ToString();
            }
            else if (response.StatusCode == HttpStatusCode.NotFound)
                Message = "Lỗi hệ thống server! " + response.StatusCode.ToString();
            else if (response.StatusCode == HttpStatusCode.RequestTimeout)
                Message = "Kết nối server Timeout! " + response.StatusCode.ToString();
            else if (response.StatusCode == HttpStatusCode.InternalServerError)
                Message = "500 (Internal Server Error)!" + response.StatusCode.ToString();
            else
                Message = "Lỗi không xác định! " + response.StatusCode.ToString();
            return Message;
        }
        #endregion

        #region Reset 
        public static void Reset()
        {
            HttpContext.Current.Session[Sessions.Menu] = "";
        }
        #endregion

        #region Hàm lấy tiếng việt hiển thị lỗi
        public static List<modelState> GetModelState(ModelStateDictionary ModelState, string Name)
        {
            List<web_ThongBao> lstThongBao = GetThongBao();
            List<view_web_NoteClass> lstNoteClass = GetNoteClass();
            List<modelState> lstmodelState = new List<modelState>();
            int i = 0;
            foreach (var Key in ModelState.Keys)
            {
                modelState objmodelState = new modelState();
                objmodelState.Key = Key;
                int j = 0;
                foreach (var itm in ModelState.Values)
                {
                    if (j == i)
                    {
                        var errors = itm.Errors;
                        if (errors.Any())
                        {
                            foreach (ModelError error in errors)
                            {
                                if (!string.IsNullOrEmpty(error.ErrorMessage))
                                {
                                    string strerror = string.IsNullOrEmpty(objmodelState.Key) ? error.ErrorMessage : error.ErrorMessage.Replace(objmodelState.Key, "'...'");
                                    var ThongBao = lstThongBao.Where(e => e.DISPLAYNAME.ToLower() == strerror.ToLower()).FirstOrDefault();
                                    if (ThongBao != null)
                                    {
                                        if (ThongBao.VN != null)
                                        {
                                            var NoteClass = lstNoteClass.Where(s => !string.IsNullOrEmpty(s.CONTROLLER) && s.CONTROLLER.ToLower() == Name.ToLower() && s.NAMECOLUMN.ToLower() == objmodelState.Key.ToLower()).FirstOrDefault();
                                            if (NoteClass != null)
                                                objmodelState.Error += ThongBao.VN.Replace("...", NoteClass.DISPLAYNAME);
                                            else
                                            {
                                                objmodelState.Error += ThongBao.VN.Replace("...", objmodelState.Key);
                                            }
                                        }
                                        else
                                        {
                                            objmodelState.Error += ThongBao.DISPLAYNAME;
                                        }
                                    }
                                    else
                                    {
                                        web_ThongBao web_ThongBao = new web_ThongBao();
                                        web_ThongBao.ID = Guid.NewGuid().ToString();
                                        web_ThongBao.DISPLAYNAME = strerror;
                                        var apiResponse = Utility.Create<web_ThongBao>(web_ThongBao, API.ThongBao);
                                        objmodelState.Error += error.ErrorMessage;
                                    }
                                }
                            }
                        }
                    }
                    j++;
                }
                lstmodelState.Add(objmodelState);
                i++;
            }
            return lstmodelState;
        }
        #endregion

        #region Convert dữ liệu sang string hiển thị
        public static List<ValueEdit> ConvertobjectTo<T>(T objectTo, string FomatDate = "yyyy-MM-dd HH:mm:ss")
        {
            List<ValueEdit> lstValueEdit = new List<ValueEdit>();
            if (objectTo != null)
            {
                var properties = objectTo.GetType().GetProperties();
                foreach (var itmPropertyInfo in properties)
                {
                    if (itmPropertyInfo != null)
                    {
                        object val = itmPropertyInfo.GetValue(objectTo);
                        ValueEdit objValueEdit = new ValueEdit();
                        objValueEdit.Key = itmPropertyInfo.Name;
                        if (val != null && val.GetType().ToString().Contains("Date"))
                        {
                            objValueEdit.Value = (object)(((DateTime)val).ToString(FomatDate));
                        }
                        else if(val != null && val.GetType().ToString().Contains("Time"))
                        {
                            objValueEdit.Value = val.ToString();
                        }
                        else
                            objValueEdit.Value = val;
                        lstValueEdit.Add(objValueEdit);
                    }
                }
                //List<view_web_NoteClass> lstNoteClass = GetNoteClass();
                //if (lstNoteClass != null)
                //    lstNoteClass = lstNoteClass.Where(s => !string.IsNullOrEmpty(s.NAMECLASS) && s.NAMECLASS.ToLower() == typeof(T).Name.ToLower() && (s.ISVIEW || s.NAMECOLUMN.ToUpper() == "LOC_ID" || s.NAMECOLUMN.ToUpper() == "ID")).ToList();

                //if (lstNoteClass != null)
                //{
                //    foreach (view_web_NoteClass itm in lstNoteClass.OrderBy(s => s.STT))
                //    {
                //        PropertyInfo itmPropertyInfo = properties.Where(s => s.Name.ToUpper() == itm.NAMECOLUMN.ToUpper()).FirstOrDefault();
                //        if(itmPropertyInfo != null)
                //        {
                //            object val = itmPropertyInfo.GetValue(objectTo);
                //            ValueEdit objValueEdit = new ValueEdit();
                //            objValueEdit.Key = itmPropertyInfo.Name;
                //            objValueEdit.Value = val;
                //            lstValueEdit.Add(objValueEdit);
                //        }
                //    }
                //}
            }


            return lstValueEdit;
        }

        public static List<ValueEdit> ConvertobjectToView<T>(T objectTo, string strDatetime = "dd/MM/yyyy")
        {
            List<ValueEdit> lstValueEdit = new List<ValueEdit>();
            try
            {
                var properties = objectTo.GetType().GetProperties();
                foreach (var itmPropertyInfo in properties)
                {
                    if (itmPropertyInfo != null)
                    {
                        object val = itmPropertyInfo.GetValue(objectTo);
                        ValueEdit objValueEdit = new ValueEdit();
                        objValueEdit.Key = itmPropertyInfo.Name;
                        if (val != null && val.GetType().ToString().Contains("Date"))
                        {
                            objValueEdit.Value = (object)(((DateTime)val).ToString(strDatetime));
                        }
                        else if (val != null && IsNumericType(val.GetType()))
                        {
                            if (Type.GetTypeCode(val.GetType()).ToString().Contains("Int"))
                                objValueEdit.Value = (object)((int)val).ToString("N0");
                            else
                            {
                                if (val.ToString().Contains(","))
                                {
                                    var length = val.ToString().Split(',')[1].Length;
                                    objValueEdit.Value = (object)((Double)val).ToString("N" + length);
                                }
                                else
                                {
                                    objValueEdit.Value = (object)((Double)val).ToString("N0");
                                }
                            }

                        }
                        else

                            objValueEdit.Value = val;
                        lstValueEdit.Add(objValueEdit);
                    }
                }
                return lstValueEdit;
            }
            catch (Exception ex)
            {

                Utility.WriteLog("ConvertobjectToView", MethodBase.GetCurrentMethod().Name, ex, JsonConvert.SerializeObject(objectTo));
                return lstValueEdit;
            }
        }

        public static bool IsNumericType(Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                default:
                    return false;
            }
        }

        public static Product_Detail ConvertobjectToProduct_Detail<T>(T objectFrom, Product_Detail objectTo, string FomatDate = "yyyy-MM-dd HH:mm:ss")
        {
            var properties = objectFrom.GetType().GetProperties();
            foreach (var itmPropertyInfo in properties)
            {
                if (itmPropertyInfo != null)
                {
                    object val = itmPropertyInfo.GetValue(objectFrom);
                    PropertyInfo piShared = objectTo.GetType().GetProperty(itmPropertyInfo.Name);
                    if (piShared != null)
                        piShared.SetValue(objectTo, val);
                }
            }
            return objectTo;
        }

        public static T EditObject<T>(T InputOutput, string TYPE, object VALUE = null)
        {
            var properties = InputOutput.GetType().GetProperties();
            foreach (var itmPropertyInfo in properties)
            {
                if (itmPropertyInfo != null && TYPE.ToLower() == itmPropertyInfo.Name.ToLower())
                {
                    object val = itmPropertyInfo.GetValue(InputOutput);
                    if (val != null && val.GetType().ToString().Contains("Date"))
                    {
                        itmPropertyInfo.SetValue(InputOutput, (Convert.ToDateTime(VALUE)), null);
                    }
                    else if (val != null && IsNumericType(val.GetType()))
                    {
                        if (Type.GetTypeCode(val.GetType()) == TypeCode.Int32)
                            itmPropertyInfo.SetValue(InputOutput, (Convert.ToInt32(VALUE)), null);
                        else
                            itmPropertyInfo.SetValue(InputOutput, (Convert.ToDouble(VALUE.ToString().Replace('.', ','))), null);
                    }
                    else if (val != null && Type.GetTypeCode(val.GetType()) == TypeCode.Boolean)
                    {
                        itmPropertyInfo.SetValue(InputOutput, Convert.ToBoolean((VALUE.ToString() == "on") ? true : false), null);
                    }
                    else
                        itmPropertyInfo.SetValue(InputOutput, VALUE, null);
                }
            }
            return InputOutput;
        }
        #endregion

        #region Get max ID Deposit_TEMP
        public static int GetMaxIDDeposit_TEMP<T>(T ovjTable, string IDName, string LOC_ID = "")
        {
            List<T> lstPage = new List<T>();
            try
            {
                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + Token);
                var response = client.GetAsync(URL + "GetIDMax/" + ovjTable.GetType().BaseType.Name + "/" + IDName + (string.IsNullOrEmpty(LOC_ID) ? "" : "/" + LOC_ID)).Result;

                if (response.IsSuccessStatusCode)
                {
                    var dataObjects = response.Content.ReadAsStringAsync().Result;
                    ApiResponse apiResponse = JsonConvert.DeserializeObject<ApiResponse>(dataObjects);

                    int n = 0;
                    if (int.TryParse(apiResponse.Data.ToString(), out n))
                    {
                        return (n + 1);
                    }
                    return 1;
                }
                else
                {
                    return 1;
                }
            }
            catch (Exception ex)
            {
                ApiResponse apiResponse = new ApiResponse
                {
                    Success = false,
                    Message = ex.Message,
                    Data = ""
                };
                return 1;
            }
        }
        #endregion

        #region Lấy danh hàng hóa combo
        private static List<v_dm_HangHoa_Combo> lstProductCombo;
        public static List<v_dm_HangHoa_Combo> LstProductCombo
        {
            get { return GetlstProductCombo(); }
            set { lstProductCombo = value; }
        }
        public static List<v_dm_HangHoa_Combo> GetlstProductCombo()
        {
            lstProductCombo = new List<v_dm_HangHoa_Combo>();
            try
            {
                if (HttpContext.Current.Session[Sessions.lstProductCombo] != null && !string.IsNullOrEmpty(HttpContext.Current.Session[Sessions.lstProductCombo].ToString()))
                    lstProductCombo = (List<v_dm_HangHoa_Combo>)HttpContext.Current.Session[Sessions.lstProductCombo];
            }
            catch (Exception ex)
            {
                Utility.WriteLog("GetlstProductCombo", MethodBase.GetCurrentMethod().Name, ex);
            }
            return lstProductCombo;
        }
        #endregion

        #region Lấy danh hàng hóa chương trình khuyến mãi yêu cầu
        private static List<v_dm_ChuongTrinhKhuyenMai_YeuCau> lstCTKM_YeuCau;
        public static List<v_dm_ChuongTrinhKhuyenMai_YeuCau> LstCTKM_YeuCau
        {
            get { return GetlstCTKM_YeuCau(); }
            set { lstCTKM_YeuCau = value; }
        }
        public static List<v_dm_ChuongTrinhKhuyenMai_YeuCau> GetlstCTKM_YeuCau()
        {
            lstCTKM_YeuCau = new List<v_dm_ChuongTrinhKhuyenMai_YeuCau>();
            try
            {
                if (HttpContext.Current.Session[Sessions.lstCTKM_YeuCau] != null && !string.IsNullOrEmpty(HttpContext.Current.Session[Sessions.lstCTKM_YeuCau].ToString()))
                    lstCTKM_YeuCau = (List<v_dm_ChuongTrinhKhuyenMai_YeuCau>)HttpContext.Current.Session[Sessions.lstCTKM_YeuCau];
            }
            catch (Exception ex)
            {
                Utility.WriteLog("GetlstCTKM_YeuCau", MethodBase.GetCurrentMethod().Name, ex);
            }
            return lstCTKM_YeuCau;
        }
        #endregion

        #region Lấy danh hàng hóa KPI kinh doanh
        private static List<v_dm_BangLuong_ChiTiet> lstdm_BangLuong_ChiTiet;
        public static List<v_dm_BangLuong_ChiTiet> Lstdm_BangLuong_ChiTiet
        {
            get { return Getlstdm_BangLuong_ChiTiet(); }
            set { lstdm_BangLuong_ChiTiet = value; }
        }
        public static List<v_dm_BangLuong_ChiTiet> Getlstdm_BangLuong_ChiTiet()
        {
            lstdm_BangLuong_ChiTiet = new List<v_dm_BangLuong_ChiTiet>();
            try
            {
                if (HttpContext.Current.Session[Sessions.lstdm_LuongThang_ChiTiet] != null && !string.IsNullOrEmpty(HttpContext.Current.Session[Sessions.lstdm_LuongThang_ChiTiet].ToString()))
                    lstdm_BangLuong_ChiTiet = (List<v_dm_BangLuong_ChiTiet>)HttpContext.Current.Session[Sessions.lstdm_LuongThang_ChiTiet];
            }
            catch (Exception ex)
            {
                Utility.WriteLog("GetlstKPISale_YeuCau", MethodBase.GetCurrentMethod().Name, ex);
            }
            return lstdm_BangLuong_ChiTiet;
        }
        #endregion

        #region Lấy danh hàng hóa KPI kinh doanh
        private static List<nv_BangLuong_ChiTiet> lstnv_BangLuong_ChiTiet;
        public static List<nv_BangLuong_ChiTiet> Lstnv_BangLuong_ChiTiet
        {
            get { return Getlstnv_BangLuong_ChiTiet(); }
            set { lstnv_BangLuong_ChiTiet = value; }
        }
        public static List<nv_BangLuong_ChiTiet> Getlstnv_BangLuong_ChiTiet()
        {
            lstnv_BangLuong_ChiTiet = new List<nv_BangLuong_ChiTiet>();
            try
            {
                if (HttpContext.Current.Session[Sessions.lstnv_BangLuong_ChiTiet] != null && !string.IsNullOrEmpty(HttpContext.Current.Session[Sessions.lstnv_BangLuong_ChiTiet].ToString()))
                    lstnv_BangLuong_ChiTiet = (List<nv_BangLuong_ChiTiet>)HttpContext.Current.Session[Sessions.lstnv_BangLuong_ChiTiet];
            }
            catch (Exception ex)
            {
                Utility.WriteLog("Getlstnv_BangLuong_ChiTiet", MethodBase.GetCurrentMethod().Name, ex);
            }
            return lstnv_BangLuong_ChiTiet;
        }
        #endregion

        #region Lấy danh hàng hóa KPI kinh doanh
        private static List<v_dm_KPI_KinhDoanh_YeuCau> lstKPISale_YeuCau;
        public static List<v_dm_KPI_KinhDoanh_YeuCau> LstKPISale_YeuCau
        {
            get { return GetlstKPISale_YeuCau(); }
            set { lstKPISale_YeuCau = value; }
        }
        public static List<v_dm_KPI_KinhDoanh_YeuCau> GetlstKPISale_YeuCau()
        {
            lstKPISale_YeuCau = new List<v_dm_KPI_KinhDoanh_YeuCau>();
            try
            {
                if (HttpContext.Current.Session[Sessions.lstKPISale_YeuCau] != null && !string.IsNullOrEmpty(HttpContext.Current.Session[Sessions.lstKPISale_YeuCau].ToString()))
                    lstKPISale_YeuCau = (List<v_dm_KPI_KinhDoanh_YeuCau>)HttpContext.Current.Session[Sessions.lstKPISale_YeuCau];
            }
            catch (Exception ex)
            {
                Utility.WriteLog("GetlstKPISale_YeuCau", MethodBase.GetCurrentMethod().Name, ex);
            }
            return lstKPISale_YeuCau;
        }

        private static List<v_dm_KPI_KinhDoanh_NhanVien> lstKPISale_NhanVien;
        public static List<v_dm_KPI_KinhDoanh_NhanVien> LstKPISale_NhanVien
        {
            get { return GetlstKPISale_NhanVien(); }
            set { lstKPISale_NhanVien = value; }
        }
        public static List<v_dm_KPI_KinhDoanh_NhanVien> GetlstKPISale_NhanVien()
        {
            lstKPISale_NhanVien = new List<v_dm_KPI_KinhDoanh_NhanVien>();
            try
            {
                if (HttpContext.Current.Session[Sessions.lstKPISale_NhanVien] != null && !string.IsNullOrEmpty(HttpContext.Current.Session[Sessions.lstKPISale_NhanVien].ToString()))
                    lstKPISale_NhanVien = (List<v_dm_KPI_KinhDoanh_NhanVien>)HttpContext.Current.Session[Sessions.lstKPISale_NhanVien];
            }
            catch (Exception ex)
            {
                Utility.WriteLog("GetlstKPISale_NhanVien", MethodBase.GetCurrentMethod().Name, ex);
            }
            return lstKPISale_NhanVien;
        }
        #endregion

        #region Lấy danh hàng hóa chương trình khuyến mãi tặng
        private static List<v_dm_ChuongTrinhKhuyenMai_Tang> lstCTKM_Tang;
        public static List<v_dm_ChuongTrinhKhuyenMai_Tang> LstCTKM_Tang
        {
            get { return GetlstCTKM_Tang(); }
            set { lstCTKM_Tang = value; }
        }
        public static List<v_dm_ChuongTrinhKhuyenMai_Tang> GetlstCTKM_Tang()
        {
            lstCTKM_Tang = new List<v_dm_ChuongTrinhKhuyenMai_Tang>();
            try
            {
                if (HttpContext.Current.Session[Sessions.lstCTKM_Tang] != null && !string.IsNullOrEmpty(HttpContext.Current.Session[Sessions.lstCTKM_Tang].ToString()))
                    lstCTKM_Tang = (List<v_dm_ChuongTrinhKhuyenMai_Tang>)HttpContext.Current.Session[Sessions.lstCTKM_Tang];
            }
            catch (Exception ex)
            {
                Utility.WriteLog("GetlstCTKM_Tang", MethodBase.GetCurrentMethod().Name, ex);
            }
            return lstCTKM_Tang;
        }
        #endregion

        #region Lấy danh hàng hóa input
        private static List<Product_Detail> lstProductInput;
        public static List<Product_Detail> LstProductInput
        {
            get { return GetlstProductInput(); }
            set { lstProductInput = value; }
        }
        public static List<Product_Detail> GetlstProductInput()
        {
            lstProductInput = new List<Product_Detail>();
            try
            {
                if (HttpContext.Current.Session[Sessions.lstProductInput] != null && !string.IsNullOrEmpty(HttpContext.Current.Session[Sessions.lstProductInput].ToString()))
                    lstProductInput = (List<Product_Detail>)HttpContext.Current.Session[Sessions.lstProductInput];
            }
            catch (Exception ex)
            {
                Utility.WriteLog("GetLstProductInput", MethodBase.GetCurrentMethod().Name, ex);
            }
            return lstProductInput;
        }
        #endregion

        #region Lấy danh sách giao hàng phiếu xuất
        private static List<v_ct_PhieuGiaoHang_ChiTiet> lstPhieuGiaoHang_ChiTiet;
        public static List<v_ct_PhieuGiaoHang_ChiTiet> LstPhieuGiaoHang_ChiTiet
        {
            get { return GetPhieuGiaoHang_ChiTiet(); }
            set { lstPhieuGiaoHang_ChiTiet = value; }
        }
        public static List<v_ct_PhieuGiaoHang_ChiTiet> GetPhieuGiaoHang_ChiTiet()
        {
            lstPhieuGiaoHang_ChiTiet = new List<v_ct_PhieuGiaoHang_ChiTiet>();
            try
            {
                if (HttpContext.Current.Session[Sessions.lstDelivery_Detail] != null && !string.IsNullOrEmpty(HttpContext.Current.Session[Sessions.lstDelivery_Detail].ToString()))
                    lstPhieuGiaoHang_ChiTiet = (List<v_ct_PhieuGiaoHang_ChiTiet>)HttpContext.Current.Session[Sessions.lstDelivery_Detail];
            }
            catch (Exception ex)
            {
                Utility.WriteLog("GetPhieuGiaoHang_ChiTiet", MethodBase.GetCurrentMethod().Name, ex);
            }
            return lstPhieuGiaoHang_ChiTiet;
        }
        #endregion

        #region Lấy danh sách giao hàng nhân viên giao 
        private static List<v_ct_PhieuGiaoHang_NhanVienGiao> lstPhieuGiaoHang_NhanVienGiao;
        public static List<v_ct_PhieuGiaoHang_NhanVienGiao> LstPhieuGiaoHang_NhanVienGiao
        {
            get { return GetPhieuGiaoHang_NhanVienGiao(); }
            set { lstPhieuGiaoHang_NhanVienGiao = value; }
        }
        public static List<v_ct_PhieuGiaoHang_NhanVienGiao> GetPhieuGiaoHang_NhanVienGiao()
        {
            lstPhieuGiaoHang_NhanVienGiao = new List<v_ct_PhieuGiaoHang_NhanVienGiao>();
            try
            {
                if (HttpContext.Current.Session[Sessions.lstDelivery_Shipper] != null && !string.IsNullOrEmpty(HttpContext.Current.Session[Sessions.lstDelivery_Shipper].ToString()))
                    lstPhieuGiaoHang_NhanVienGiao = (List<v_ct_PhieuGiaoHang_NhanVienGiao>)HttpContext.Current.Session[Sessions.lstDelivery_Shipper];
            }
            catch (Exception ex)
            {
                Utility.WriteLog("GetPhieuGiaoHang_NhanVienGiao", MethodBase.GetCurrentMethod().Name, ex);
            }
            return lstPhieuGiaoHang_NhanVienGiao;
        }
        #endregion

        #region Convert Số sang Value
        public static string ConvertNumberToString(object value, int? sole = null)
        {
            try
            {
                if (sole != null)
                {
                    return Convert.ToDecimal(value).ToString("N" + sole.Value.ToString()).Replace(",", ".");
                }
                else
                    return value.ToString().Replace(",", ".");
            }
            catch (Exception ex)
            {
                Utility.WriteLog("ConvertNumberToString", MethodBase.GetCurrentMethod().Name, ex);
                return "0";
            }
        }
        #endregion

        #region Convert String sang Double
        public static Double ConvertStringToDouble(object value)
        {
            try
            {
                return Convert.ToDouble(value.ToString().Replace("'", ""));//.Replace(".",",")
            }
            catch (Exception ex)
            {
                Utility.WriteLog("ConvertStringToDouble", MethodBase.GetCurrentMethod().Name, ex);
                return 0;
            }
        }
        #endregion

        #region Sản phẩm combo
        public static string GetProductCombo()
        {
            string BodyField = "";
            foreach (var itm in Utility.LstProductCombo)
            {
                BodyField += "<tr id=\"" + itm.ID + "\">";
                BodyField += "<td style=\"white-space: nowrap; \" id=\"MA\">" + itm.MA + "</td>";
                BodyField += "<td style=\"white-space: nowrap; \" id=\"NAME\">" + itm.NAME + "</td>";
                BodyField += "<td style=\"white-space: nowrap; \" id=\"QTY\"><input type=\"number\" class=\"form-control maskinput\" data-type=\"currency\"  name=\"txtQuantity|" + itm.ID_HANGHOA + "|" + itm.ID_DVT + "|" + itm.TYLE_QD + "\" min=\"0.10\" data-id=\"" + itm.ID + "\" step=\"any\" value=\"" + Utility.ConvertNumberToString(itm.QTY) + "\" style=\"width:80px\" min=\"0\"/></td>";
                BodyField += "<td style=\"white-space: nowrap; \" id=\"NAME_DVT\">" + itm.NAME_DVT + "</td>";
                BodyField += "<td style=\"white-space: nowrap; \"><a class=\"label label-danger\" onclick=\"myFunctionDeleteProdcutCombo('" + API.dm_HangHoa_Combo + "','" + itm.ID_HANGHOA + "','" + itm.ID_DVT + "')\" href=\"#\"><i class=\"glyphicon glyphicon-trash\" style=\"margin-right:5px\"></i>" + Utility.Xoa + "</a></td>";
                BodyField += "</tr>";
            }
            return BodyField;
        }
        #endregion

        #region Chương trinh khuyến mãi
        public static string GetCTKM_YeuCau()
        {
            string BodyField = "";
            foreach (var itm in Utility.LstCTKM_YeuCau)
            {
                string ShowSearchValue = clsMaHoa.Encrypt(JsonConvert.SerializeObject(itm), clsMaHoa.PassMaHoa);
                BodyField += "<tr id=\"" + itm.ID + "\">";
                BodyField += "<td style=\"white-space: nowrap; \" id=\"MA\">" + itm.NAME_HINHTHUC + "</td>";
                BodyField += "<td style=\"white-space: nowrap; \" id=\"MA\">" + itm.MA + "</td>";
                BodyField += "<td style=\"white-space: nowrap; \" id=\"NAME\">" + itm.NAME + "</td>";
                BodyField += "<td style=\"white-space: nowrap; \" id=\"MONEY\"><input type=\"number\" class=\"form-control maskinput\" data-type=\"currency\"  name=\"txtMoney_YC|" + ShowSearchValue + "\"  step=\"any\" value=\"" + Utility.ConvertNumberToString(itm.SOTIEN) + "\" style=\"width:100%\" min=\"0\"/></td>";
                BodyField += "<td style=\"white-space: nowrap; \" id=\"QTY\"><input type=\"number\" class=\"form-control maskinput\" data-type=\"currency\" name=\"txtQuantity_YC|" + ShowSearchValue + "\" step=\"any\" value=\"" + Utility.ConvertNumberToString(itm.SOLUONG) + "\" style=\"width:100%\" min=\"0\"/></td>";
                BodyField += "<td style=\"white-space: nowrap; \" id=\"NAME_DVT\">" + itm.NAME_DVT + "</td>";
                BodyField += "<td style=\"white-space: nowrap; \" id=\"CHIETKHAU\"><input type=\"number\" class=\"form-control maskinput\" data-type=\"currency\" name=\"txtCHIETKHAU_YC|" + ShowSearchValue + "\" step=\"any\" value=\"" + Utility.ConvertNumberToString(itm.CHIETKHAU) + "\" style=\"width:100%\" min=\"0\" max=\"100\"/></td>";
                BodyField += "<td style=\"white-space: nowrap; \" id=\"TIENGIAM\"><input type=\"number\" class=\"form-control maskinput\" data-type=\"currency\" name=\"txtTIENGIAM_YC|" + ShowSearchValue + "\" step=\"any\" value=\"" + Utility.ConvertNumberToString(itm.TIENGIAM) + "\" style=\"width:100%\" min=\"0\"/></td>";
                BodyField += "<td style=\"white-space: nowrap; \" id=\"ISBATBUOC\"><input type=\"checkbox\" class=\"form-control\" name=\"txtISBATBUOC|" + ShowSearchValue + "\" id=\"ISBATBUOC\" " + (itm.ISBATBUOC ? "checked" : "") + "/></td>";
                BodyField += "<td style=\"white-space: nowrap; \" id=\"SOLUONG_BATBUOC\"><input type=\"number\" class=\"form-control maskinput\" data-type=\"currency\" name=\"txtSOLUONG_BATBUOC|" + ShowSearchValue + "\" step=\"any\" value=\"" + Utility.ConvertNumberToString(itm.SOLUONG_BATBUOC) + "\" style=\"width:100%\" min=\"0\"/></td>";
                BodyField += "<td style=\"white-space: nowrap; \"><a class=\"label label-danger\" onclick=\"myFunctionDeletePromotion_YC('" + API.dm_ChuongTrinhKhuyenMai + "','" + itm.ID_HANGHOA + "','" + itm.ID_DVT + "')\" href=\"#\"><i class=\"glyphicon glyphicon-trash\" style=\"margin-right:5px\"></i>" + @Utility.Xoa + "</a></td>";
                BodyField += "</tr>";
            }
            return BodyField;
        }

        public static string GetCTKM_Tang()
        {
            string BodyField = "";
            foreach (var itm in Utility.LstCTKM_Tang)
            {
                string ShowSearchValue = clsMaHoa.Encrypt(JsonConvert.SerializeObject(itm), clsMaHoa.PassMaHoa);
                BodyField += "<tr id=\"" + itm.ID + "\">";
                BodyField += "<td style=\"white-space: nowrap; \" id=\"MA\">" + itm.MA + "</td>";
                BodyField += "<td style=\"white-space: nowrap; \" id=\"NAME\">" + itm.NAME + "</td>";
                BodyField += "<td style=\"white-space: nowrap; \" id=\"MONEY\"><input type=\"number\" class=\"form-control maskinput\" data-type=\"currency\" name=\"txtMoney_Tang|" + ShowSearchValue + "\"  step=\"any\" value=\"" + Utility.ConvertNumberToString(itm.SOTIEN) + "\" style=\"width:100%\" min=\"0\"/></td>";
                BodyField += "<td style=\"white-space: nowrap; \" id=\"QTY\"><input type=\"number\" class=\"form-control maskinput\" data-type=\"currency\" name=\"txtQuantity_Tang|" + ShowSearchValue + "\" step=\"any\" value=\"" + Utility.ConvertNumberToString(itm.SOLUONG) + "\" style=\"width:100%\" min=\"0\"/></td>";
                BodyField += "<td style=\"white-space: nowrap; \" id=\"NAME_DVT\">" + itm.NAME_DVT + "</td>";
                BodyField += "<td style=\"white-space: nowrap; \"><a class=\"label label-danger\" onclick=\"myFunctionDeletePromotion_Tang('" + API.dm_ChuongTrinhKhuyenMai + "','" + itm.ID_HANGHOA + "','" + itm.ID_DVT + "')\" href=\"#\"><i class=\"glyphicon glyphicon-trash\" style=\"margin-right:5px\"></i>" + @Utility.Xoa + "</a></td>";
                BodyField += "</tr>";
            }
            return BodyField;
        }
        #endregion

        #region KPI Kinh doanh
        public static string GetKPISale_YeuCau()
        {
            string BodyField = "";
            foreach (var itm in Utility.LstKPISale_YeuCau)
            {
                string ShowSearchValue = clsMaHoa.Encrypt(JsonConvert.SerializeObject(itm), clsMaHoa.PassMaHoa);
                BodyField += "<tr id=\"" + itm.ID + "\">";
                BodyField += "<td style=\"white-space: nowrap; \" id=\"HINHTHUC_TINHKPI\"><select class=\"form-control chosen-select\" name=\"HINHTHUC_TINHKPI|" + ShowSearchValue + "\" id=\"HINHTHUC_TINHKPI\" style=\"width:150px\">";
                BodyField += "<option value>Chọn hình thức tính</option>";
                 foreach(var s in API.lstHinhThucTinhKPI())
                {
                   BodyField += "<option value = \"" + s.ID + "\" " + (s.ID == itm.HINHTHUC_TINHKPI ? "selected" : "") +"> " + s.NAME + " </option>";
                }
                BodyField += "</select></td>";
                BodyField += "<td style=\"white-space: nowrap; \" id=\"MA\">" + itm.NAME_HINHTHUC + "</td>";
                BodyField += "<td style=\"white-space: nowrap; \" id=\"MA\">" + itm.MA + "</td>";
                BodyField += "<td style=\"white-space: nowrap; \" id=\"NAME\">" + itm.NAME + "</td>";
                BodyField += "<td style=\"white-space: nowrap; \" id=\"MONEY\"><input type=\"number\" class=\"form-control maskinput\" data-type=\"currency\" name=\"txtMoney_YC|" + ShowSearchValue + "\"  step=\"any\" value=\"" + Utility.ConvertNumberToString(itm.SOTIEN) + "\" style=\"width:100%\" min=\"0\"/></td>";
                BodyField += "<td style=\"white-space: nowrap; \" id=\"QTY\"><input type=\"number\" class=\"form-control maskinput\" data-type=\"currency\" name=\"txtQuantity_YC|" + ShowSearchValue + "\" step=\"any\" value=\"" + Utility.ConvertNumberToString(itm.SOLUONG) + "\" style=\"width:100%\" min=\"0\"/></td>";
                BodyField += "<td style=\"white-space: nowrap; \" id=\"NAME_DVT\">" + itm.NAME_DVT + "</td>";
                BodyField += "<td style=\"white-space: nowrap; \" id=\"CHIETKHAU\"><input type=\"number\" class=\"form-control maskinput\" data-type=\"currency\" name=\"txtCHIETKHAU_YC|" + ShowSearchValue + "\" step=\"any\" value=\"" + Utility.ConvertNumberToString(itm.CHIETKHAU) + "\" style=\"width:100%\" min=\"0\" max=\"100\"/></td>";
                BodyField += "<td style=\"white-space: nowrap; \" id=\"TIENGIAM\"><input type=\"number\" class=\"form-control maskinput\" data-type=\"currency\" name=\"txtTIENGIAM_YC|" + ShowSearchValue + "\" step=\"any\" value=\"" + Utility.ConvertNumberToString(itm.TIENGIAM) + "\" style=\"width:100%\" min=\"0\"/></td>";
                BodyField += "<td style=\"white-space: nowrap; \"><a class=\"label label-danger\" onclick=\"myFunctionDeletePromotion_YC('" + API.dm_KPI_KinhDoanh + "','" + itm.ID_HANGHOA + "','" + itm.ID_DVT + "')\" href=\"#\"><i class=\"glyphicon glyphicon-trash\" style=\"margin-right:5px\"></i>" + @Utility.Xoa + "</a></td>";
                BodyField += "</tr>";
            }
            return BodyField;
        }

        public static string GetKPISale_NhanVien()
        {
            string BodyField = "";
            foreach (var itm in Utility.LstKPISale_NhanVien)
            {
                string ShowSearchValue = clsMaHoa.Encrypt(JsonConvert.SerializeObject(itm), clsMaHoa.PassMaHoa);
                BodyField += "<tr id=\"" + itm.ID + "\">";
                BodyField += "<td style=\"white-space: nowrap; \" id=\"MA\">" + itm.NAME_HINHTHUC + "</td>";
                BodyField += "<td style=\"white-space: nowrap; \" id=\"MA\">" + itm.MA + "</td>";
                BodyField += "<td style=\"white-space: nowrap; \" id=\"NAME\">" + itm.NAME + "</td>";
                BodyField += "<td style=\"white-space: nowrap;display: none; \" id=\"ISACTIVE\" ><input type=\"checkbox\" class=\"form-control\" name=\"txtISACTIVE|" + ShowSearchValue + "\" id=\"ISACTIVE\" checked/></td>";
                BodyField += "<td style=\"white-space: nowrap; \"><a class=\"label label-danger\" onclick=\"myFunctionDeletePromotion_Tang('" + API.dm_KPI_KinhDoanh + "','" + itm.ID_NHANVIEN + "','" + itm.HINHTHUC.ToString() + "')\" href=\"#\"><i class=\"glyphicon glyphicon-trash\" style=\"margin-right:5px\"></i>" + @Utility.Xoa + "</a></td>";
                BodyField += "</tr>";
            }
            return BodyField;
        }
        #endregion

        #region Xuất, đặt hàng
        public static string GetProductInputOutput(List<Product_Detail> lstProduct, string name, bool bolTinhLai = true, double TONGTIENGIAMGIA = 0, double TONGTHANHTIEN = 0, double TONGTIENVAT = 0, double TONGTIEN = 0, bool bolSuaSoLuong = false,bool bolSuaDonGia = false)
        {
            string option = "";
            if (lstProduct == null) return "";
            try
            {
                var apiResponse = Utility.GetListData<v_dm_ThueSuat>(API.dm_ThueSuat, "", "", Utility.LOC_ID);
                List<v_dm_ThueSuat> lstThueSuat = (apiResponse.Data as List<v_dm_ThueSuat>);
                foreach (var thuesuat in lstThueSuat)
                {
                    option += "<option value = \"" + thuesuat.ID + "\"> " + thuesuat.NAME + " </option>";
                }
                string BodyField = null;
                foreach (var itm in lstProduct)
                {
                    if (bolTinhLai)
                    {
                        TONGTIENGIAMGIA += itm.TONGTIENGIAMGIA;
                        TONGTHANHTIEN += itm.THANHTIEN;
                        TONGTIENVAT += itm.TONGTIENVAT;
                        TONGTIEN += itm.TONGCONG;
                    }
                    
                    BodyField += "<tr id=\"" + itm.ID + "\">";
                    BodyField += "<td style=\"white-space: nowrap; \" id=\"MA\">" + itm.MA + "</td>";
                    BodyField += "<td style=\"white-space: nowrap; \" id=\"NAME\">" + (itm.ISKHUYENMAI ? "(KM)" : "") + itm.NAME +"</td>";
                    BodyField += "<td style=\"white-space: nowrap; \" id=\"NAME_DVT\">" + itm.NAME_DVT + "</td>";
                    BodyField += Get_tdInput(name, "SOLUONG", "txtSOLUONG", itm, itm.SOLUONG, "100", itm.ISKHUYENMAI ? !bolSuaSoLuong : (itm.ISCOMBO ? itm.ISCOMBO : (itm.MA == API.GTBH ? true : false)), "0");
                    BodyField += Get_tdInput(name, "DONGIA", "txtDONGIA", itm, itm.DONGIA, "100px", itm.ISKHUYENMAI ? true : !bolSuaDonGia);
                    BodyField += Get_tdInput(name, "CHIETKHAU", "txtCHIETKHAU", itm, itm.CHIETKHAU, "100px", itm.ISKHUYENMAI ? true : itm.ISCOMBO);
                    BodyField += Get_tdInput(name, "TONGTIENGIAMGIA", "txtTONGTIENGIAMGIA", itm, itm.TONGTIENGIAMGIA, "100px", itm.ISKHUYENMAI ? true : itm.ISCOMBO, "-10000000");

                    BodyField += Get_tdInput(name, "THANHTIEN", "txtTHANHTIEN", itm, itm.THANHTIEN, "100px", true);

                    if (itm.ISCOMBO || (itm.ISKHUYENMAI))
                        BodyField += "<td></td>";
                    else
                    {
                        BodyField += "<td style=\"white-space: nowrap; \" id=\"ID_THUESUAT\"><select class=\"form-control chosen-select\" name=\"ThueSuat|" + itm.ID + "\" id=\"ID_THUESUAT\" style=\"width:80px\" onchange = \"updateInputOutput('" + itm.ID + "',this)\">";
                        BodyField += "<option value>" + Utility.GetTitleChon(API.dm_ThueSuat) + "</option>";
                        BodyField += option.Replace("option value = \"" + itm.ID_THUESUAT + "\"", "option value = \"" + itm.ID_THUESUAT + "\" selected");
                        BodyField += "</select>";
                    }

                    BodyField += Get_tdInput(name, "TONGTIENVAT", "txtTONGTIENVAT", itm, itm.TONGTIENVAT, "100px", itm.ISKHUYENMAI ? true : itm.ISCOMBO);
                    BodyField += Get_tdInput(name, "TONGCONG", "txtTONGCONG", itm, itm.TONGCONG, "100px", true);

                    if (itm.ISCOMBO)
                        BodyField += "<td></td>";
                    else
                        BodyField += "<td style=\"white-space: nowrap; \" class=\"fix\"><a class=\"label label-danger\" onclick=\"myFunctionDeleteProdcut" + name + "('" + API.dm_HangHoa + "','" + itm.ID + "')\" href=\"#\"><i class=\"glyphicon glyphicon-trash\" style=\"margin-right:5px\"></i>" + @Utility.Xoa + "</a></td>";
                    BodyField += "</tr>";
                }
                if (!string.IsNullOrEmpty(BodyField))
                {
                    BodyField += "<tr>" +
                                      "<td style=\"font-weight: bold; text-align:center; white-space: nowrap;\" colspan=\"6\">" +
                                      "   <label class=\"col-sm-2 control-label\" for=\"T_ng_ti_n\" style=\"font-weight: bold; text-align:center; white-space: nowrap;float:right;\">Tổng tiền</label>" +
                                      "</td>" +
                                      "<td style=\"font-weight: bold; text-align:center; white-space: nowrap;\">" +
                                      "   <input class=\"form-control maskinput\" data-type=\"currency\" min=\"-10000000\" step=\"any\" data-val=\"true\" data-val-number=\"The field TONGTIENGIAMGIA must be a number.\" data-val-required=\"The TONGTIENGIAMGIA field is required.\" id=\"TONGTIENGIAMGIA\" name=\"TONGTIENGIAMGIA\" type=\"number\" value=\"" + Utility.ConvertNumberToString(TONGTIENGIAMGIA) + "\" style=\"width:100%\">" +
                                      "    <span class=\"field-validation-valid text-danger\" data-valmsg-for=\"TONGTIENGIAMGIA\" data-valmsg-replace=\"true\"></span>" +
                                      "</td>" +
                                      "<td style=\"font-weight: bold; text-align:center; white-space: nowrap;\">" +
                                      "    <input class=\"form-control maskinput\" data-type=\"currency\" min=\"0\" step=\"any\" data-val=\"true\" data-val-number=\"The field TONGTHANHTIEN must be a number.\" data-val-required=\"The TONGTHANHTIEN field is required.\" id=\"TONGTHANHTIEN\" name=\"TONGTHANHTIEN\" type=\"number\" value=\"" + Utility.ConvertNumberToString(TONGTHANHTIEN) + "\" style=\"width:100%\">" +
                                      "   <span class=\"field-validation-valid text-danger\" data-valmsg-for=\"TONGTHANHTIEN\" data-valmsg-replace=\"true\"></span>" +
                                      "</td>" +
                                      "<td style=\"font-weight: bold; text-align:center; white-space: nowrap;\" colspan=\"2\">" +
                                      "    <input class=\"form-control maskinput\" data-type=\"currency\" min=\"0\" step=\"any\" data-val=\"true\" data-val-number=\"The field TONGTIENVAT must be a number.\" data-val-required=\"The TONGTIENVAT field is required.\" id=\"TONGTIENVAT\" name=\"TONGTIENVAT\" type=\"number\" value=\"" + Utility.ConvertNumberToString(TONGTIENVAT) + "\" style=\"width:100%\">" +
                                      "    <span class=\"field-validation-valid text-danger\" data-valmsg-for=\"TONGTIENVAT\" data-valmsg-replace=\"true\"></span>" +
                                      "</td>" +
                                      "<td style=\"font-weight: bold; text-align:center; white-space: nowrap;\">" +
                                      "   <input class=\"form-control maskinput\" data-type=\"currency\" min=\"0\" step=\"any\" data-val=\"true\" data-val-number=\"The field TONGTIEN must be a number.\" data-val-required=\"The TONGTIEN field is required.\" id=\"TONGTIEN\" name=\"TONGTIEN\" type=\"number\" value=\"" + Utility.ConvertNumberToString(TONGTIEN) + "\" style=\"width:100%\">" +
                                      "   <span class=\"field-validation-valid text-danger\" data-valmsg-for=\"TONGTIEN\" data-valmsg-replace=\"true\"></span>" +
                                      "</td>" +
                                      "<td>" +
                                      "</td>" +
                                  "</tr>";
                }

                return BodyField;
            }
            catch (Exception ex)
            {
                Utility.WriteLog("GetProductInputOutput", MethodBase.GetCurrentMethod().Name, ex, JsonConvert.SerializeObject(lstProduct));
                return "";
            }
        }

        private static string Get_tdInput(string name, string nameinput, string txt, Product_Detail Product_Detail, double value, string width = "50px", bool bolreadonly = false, string Min = "")
        {
            if (name == "Deposit_Temp" && nameinput == "DONGIA")
            {
                bolreadonly = !Utility.KiemTraQuyen(API.ct_PhieuDatHang, API.EditPrice);
            }
            if (name == "Deposit_Temp" && nameinput == "DONGIA")
            {
                bolreadonly = !Utility.KiemTraQuyen(API.ct_PhieuDatHang, API.EditPrice);
            }
            string ShowSearchValue = clsMaHoa.Encrypt(JsonConvert.SerializeObject(Product_Detail), clsMaHoa.PassMaHoa);
            string td = "";
            td = "<td style=\"white-space: nowrap; \" id=\"" + nameinput + "\"><input type=\"number\" class=\"form-control maskinput\" data-type=\"currency\" name=\"" + txt + "|" + ShowSearchValue + "\" id=\"" + nameinput + "\" min=\"" + (!string.IsNullOrEmpty(Min) ? Min : "0") + "\" step=\"any\" value=\"" + Utility.ConvertNumberToString(value) + "\" style=\"width:" + width + "\" min=\"0\" onchange=\"update" + name + "('" + Product_Detail.ID + "',this)\" "+ (bolreadonly ? "" : "") +" " + (bolreadonly ? "readonly = \"readonly\"" : "") + "/></td>"; //onkeyup =\"this.onchange();\"
            //td += "<input type=\"text\" class=\"form-control mask\" name=\"" + txt + "|" + ID_HANGHOAKHO + "|" + ID_DVT + "|" + TYLE_QD + "\" id=\"txtQty|" + ID_HANGHOAKHO + "|" + ID_DVT + "|" + TYLE_QD + "\" min=\"0.10\" step=\"any\" value=\"" + Utility.ConvertNumberToString(value) + "\" style=\"width:" + width + "\" min=\"0\"/></td>";
            return td;
        }
        #endregion

        #region Parameter
        public static string GetParameter(List<v_web_Report_Parameter> lstProduct)
        {
            try
            {
                string BodyField = null;
                foreach (var itm in lstProduct.OrderBy(e => e.STT))
                {
                    if (string.IsNullOrEmpty(itm.ID_PARAMETER))
                        itm.ID_PARAMETER = itm.ID;
                    string ShowSearchValue = clsMaHoa.Encrypt(JsonConvert.SerializeObject(itm), clsMaHoa.PassMaHoa);
                    BodyField += "<tr id=\"" + itm.ID + "\">";
                    BodyField += "<td style=\"white-space: nowrap; \" id=\"NAME_PARAMETER\">" + (!string.IsNullOrEmpty(itm.NAME_PARAMETER) ? itm.NAME_PARAMETER : itm.NAME) + "</td>";
                    BodyField += "<td style=\"white-space: nowrap; \" id=\"STT\"><input type=\"number\" class=\"form-control maskinput\" data-type=\"currency\" name=\"txtSTT|" + ShowSearchValue + "\" id=\"STT\" min=\"0\" step=\"any\" value=\"" + Utility.ConvertNumberToString(itm.STT) + "\" style=\"width:80px\" min=\"0\"/></td>";
                    BodyField += "<td style=\"white-space: nowrap; \" id=\"ISACTIVE\"><input type=\"checkbox\" class=\"form-control\" name=\"txtISACTIVE|" + ShowSearchValue + "\" id=\"ISACTIVE\" " + (itm.ISACTIVE ? "checked" : "") + "/></td>";
                    BodyField += "<td style=\"white-space: nowrap; \" id=\"VALUE_REPORT\"><textarea class=\"form-control\" runat=\"server\" cols=\"20\" id=\"VALUE_REPORT\" name=\"txtVALUE_REPORT|" + ShowSearchValue + "\" rows=\"3\"> " + itm.VALUE_REPORT + "</textarea></td>";
                }
                return BodyField;
            }
            catch (Exception ex)
            {
                Utility.WriteLog("GetParameter", MethodBase.GetCurrentMethod().Name, ex, JsonConvert.SerializeObject(lstProduct));
                return "";
            }

        }
        #endregion


        #region CategoryPayroll
        public static string GetCategoryPayroll(List<v_dm_BangLuong_ChiTiet> lstProduct, List<v_dm_LoaiLuong> lstLoaiLuong)
        {
            try
            {
                string BodyField = null;
                foreach (var itm in lstProduct)
                {
                    string ShowSearchValue = clsMaHoa.Encrypt(JsonConvert.SerializeObject(itm), clsMaHoa.PassMaHoa);
                    
                    BodyField += "<tr id=\"" + itm.ID + "\">";
                    BodyField += "<td style=\"white-space: nowrap; \" id=\"ID_LOAILUONG\"><select class=\"form-control chosen-select\" name=\"ID_LOAILUONG|" + ShowSearchValue + "\" id=\"ID_LOAILUONG\" style=\"width:150px\">";
                    BodyField += "<option value>--Chọn loại lương--</option>";
                    foreach (var s in lstLoaiLuong)
                    {
                        BodyField += "<option value = \"" + s.ID + "\" " + (s.ID == itm.ID_LOAILUONG ? "selected" : "") + "> " + s.NAME + " </option>";
                    }
                    BodyField += "<td style=\"white-space: nowrap; \" id=\"TYPE_LUONG\"><select class=\"form-control chosen-select\" name=\"TYPE_LUONG|" + ShowSearchValue + "\" id=\"TYPE_LUONG\" style=\"width:150px\">";
                    BodyField += "<option value>--Chọn hình thức tính--</option>";
                    foreach (var s in API.lstTYPELuong())
                    {
                        BodyField += "<option value = \"" + s.ID + "\" " + (s.ID == itm.TYPE_LUONG ? "selected" : "") + "> " + s.NAME + " </option>";
                    }
                    BodyField += "</select></td>";

                    BodyField += "<td style=\"white-space: nowrap; \" id=\"TYPE_QUYTACTINHLUONG\"><select class=\"form-control chosen-select\" name=\"TYPE_QUYTACTINHLUONG|" + ShowSearchValue + "\" id=\"TYPE_QUYTACTINHLUONG\" style=\"width:150px\">";
                    BodyField += "<option value>--Chọn quy tắc tính lương--</option>";
                    foreach (var s in API.lstTYPEQuyTacTinhLuong())
                    {
                        BodyField += "<option value = \"" + s.ID + "\" " + (s.ID == itm.TYPE_QUYTACTINHLUONG ? "selected" : "") + "> " + s.NAME + " </option>";
                    }
                    BodyField += "</select></td>";
                    BodyField += "<td style=\"white-space: nowrap; \" id=\"SOTIEN\"><input type=\"number\" class=\"form-control maskinput\" data-type=\"currency\" name=\"txtSOTIEN|" + ShowSearchValue + "\" step=\"any\" value=\"" + Utility.ConvertNumberToString(itm.SOTIEN) + "\" style=\"width:100%\" min=\"0\"/></td>";
                    BodyField += "<td style=\"white-space: nowrap; \"><a class=\"label label-danger\" onclick=\"myFunctionDeletePayroll('" + itm.ID + "','CategoryPayroll')\" href=\"#\"><i class=\"glyphicon glyphicon-trash\" style=\"margin-right:5px\"></i>" + Utility.Xoa + "</a></td>";
                    BodyField += "</tr>";
                   
                }
                return BodyField;
            }
            catch (Exception ex)
            {
                Utility.WriteLog("GetCategoryPayroll", MethodBase.GetCurrentMethod().Name, ex, JsonConvert.SerializeObject(lstProduct));
                return "";
            }

        }
        #endregion

        #region Payroll Detail
        public static string GetPayrollDetail(List<nv_BangLuong_ChiTiet> lstProduct, List<v_dm_LoaiLuong> lstLoaiLuong)
        {
            try
            {
                string BodyField = null;
                foreach (var itm in lstProduct.OrderByDescending(s => s.SOTIEN))
                {
                    string ShowSearchValue = clsMaHoa.Encrypt(JsonConvert.SerializeObject(itm), clsMaHoa.PassMaHoa);

                    BodyField += "<tr id=\"" + itm.ID + "\">";
                    BodyField += "<td style=\"white-space: nowrap; \" id=\"txtID_LOAILUONG\"><select class=\"form-control chosen-select\" name=\"txtID_LOAILUONG|" + ShowSearchValue + "\" id=\"txtID_LOAILUONG\" style=\"width:150px\">";
                    BodyField += "<option value>--Chọn loại lương--</option>";
                    foreach (var s in lstLoaiLuong)
                    {
                        BodyField += "<option value = \"" + s.ID + "\" " + (s.ID == itm.ID_LOAILUONG ? "selected" : "") + "> " + s.NAME + " </option>";
                    }
                    BodyField += "<td style=\"white-space: nowrap; \" id=\"txtTYPE\"><select class=\"form-control chosen-select\" name=\"txtTYPE|" + ShowSearchValue + "\" id=\"txtTYPE\" style=\"width:150px\">";
                    BodyField += "<option value>--Chọn hình thức tính--</option>";
                    foreach (var s in API.lstTYPELoaiLuong())
                    {
                        BodyField += "<option value = \"" + s.ID + "\" " + (s.ID.ToString() == itm.TYPE ? "selected" : "") + "> " + s.NAME + " </option>";
                    }
                    BodyField += "</select></td>";
                    BodyField += "<td style=\"white-space: nowrap; \" id=\"txtSOTIEN\"><input type=\"number\" class=\"form-control maskinput\" data-type=\"currency\" name=\"txtSOTIEN|" + ShowSearchValue + "\" step=\"any\" value=\"" + Utility.ConvertNumberToString(itm.SOTIEN) + "\" style=\"width:100%\" min=\"0\"/></td>";
                    BodyField += "<td style=\"white-space: nowrap; \"><a class=\"label label-danger\" onclick=\"myFunctionDeletePayroll('" + itm.ID + "','Payroll')\" href=\"#\"><i class=\"glyphicon glyphicon-trash\" style=\"margin-right:5px\"></i>" + Utility.Xoa + "</a></td>";
                    BodyField += "</tr>";

                }
                return BodyField;
            }
            catch (Exception ex)
            {
                Utility.WriteLog("GetPayrollDetail", MethodBase.GetCurrentMethod().Name, ex, JsonConvert.SerializeObject(lstProduct));
                return "";
            }

        }
        #endregion

        #region Giao hàng
        public static string GetDelivery_Detail(List<v_ct_PhieuGiaoHang_ChiTiet> lstProduct, bool bolEdit = false)
        {
            try
            {
                string BodyField = null;
                string BodyField0 = null;
                if (lstProduct == null) return "";
                string ID_KHACHHANG_NCC = "";

                foreach (var itm in lstProduct.OrderBy(s => s.ID_KHACHHANG_NCC))
                {
                    if (ID_KHACHHANG_NCC != itm.ID_KHACHHANG_NCC && lstProduct.Where(s => s.ID_KHACHHANG_NCC == itm.ID_KHACHHANG_NCC).ToList().Count > 1)
                    {
                        BodyField += "<tr data-id=\"" + itm.ID_KHACHHANG_NCC + "\" data-parent=\"0\" data-level=\"1\" id=\"" + itm.ID_KHACHHANG_NCC + "\">";
                        BodyField += "<td data-column=\"name\" colspan=\"3\" style=\"font-weight: bold;\">" + itm.NAME_KHACHHANG_NCC + "</td>";
                        BodyField += "<td style=\"font-weight: bold;text-align: right;\">" + lstProduct.Where(s => s.ID_KHACHHANG_NCC == itm.ID_KHACHHANG_NCC).Sum(s => s.TONGSOLUONG).ToString("N0") + "</td>";
                        BodyField += "<td style=\"font-weight: bold;text-align: right;\">" + lstProduct.Where(s => s.ID_KHACHHANG_NCC == itm.ID_KHACHHANG_NCC).Sum(s => s.TONGKHOILUONG).ToString("N0") + "</td>";
                        BodyField += "<td style=\"font-weight: bold;text-align: right;\">" + lstProduct.Where(s => s.ID_KHACHHANG_NCC == itm.ID_KHACHHANG_NCC).Sum(s => s.SOTIENGIAOHANG).ToString("N0") + "</td>";
                        BodyField += "</tr>";
                    }
                    ID_KHACHHANG_NCC = itm.ID_KHACHHANG_NCC;
                    string ShowSearchValue = clsMaHoa.Encrypt(JsonConvert.SerializeObject(itm), clsMaHoa.PassMaHoa);
                    if (lstProduct.Where(s => s.ID_KHACHHANG_NCC == itm.ID_KHACHHANG_NCC).ToList().Count > 1)
                    {
                        BodyField += "<tr data-id=\"" + itm.ID_PHIEUXUAT + "\" data-parent=\"" + itm.ID_KHACHHANG_NCC + "\" data-level=\"2\" id=\"" + itm.ID_PHIEUXUAT + "\">";
                        BodyField += "<td style=\"white-space: nowrap; \" id=\"NGAYLAP\">" + itm.NGAYLAP.ToString("dd/MM/yyyy") + "</td>";
                        BodyField += "<td style=\"white-space: nowrap; \" id=\"MAPHIEU\">" + itm.MAPHIEUXUAT + " (" + (itm.SOLAN > 0 ? itm.SOLAN : (lstProduct.Max(s => s.SOLAN) > 0 ? (lstProduct.Max(s => s.SOLAN) + 1) : 1)).ToString() + ")"+ "</td>";
                        BodyField += "<td style=\"white-space: nowrap; \" id=\"NAME_KHACHHANG_NCC\"></td>";
                        BodyField += "<td style=\"white-space: nowrap;text-align: right; \" id=\"TONGSOLUONG\">" + itm.TONGSOLUONG.ToString("N0") + "</td>";
                        BodyField += "<td style=\"white-space: nowrap;text-align: right; \" id=\"TONGKHOILUONG\">" + itm.TONGKHOILUONG.ToString("N0") + "</td>";
                        BodyField += "<td style=\"white-space: nowrap;text-align: right; \" id=\"SOTIENGIAOHANG\">" + itm.SOTIENGIAOHANG.ToString("N0") + "</td>";
                        BodyField += "<td style=\"white-space: nowrap; \"><a class=\"label label-default\" onclick=\"myFunctionViewReport('" + API.ct_PhieuXuat + "','" + itm.ID_PHIEUXUAT + "')\" href=\"#\"><i class=\"fa fa-print\" style=\"margin-right:5px\"></i></a></td>";
                        BodyField += "<td style=\"visibility: hidden; display: none;\" id=\"Detail\"><input type=\"checkbox\" class=\"form-control\" name=\"txtDetail|" + ShowSearchValue + "\" id=\"Detail\" checked/></td>";
                        BodyField += "<td style=\"white-space: nowrap; \"><a class=\"label label-danger\" onclick=\"myFunctionDelivery('" + API.ct_PhieuGiaoHang + "','DeleteDeliveryDetail','" + itm.ID_PHIEUXUAT + "')\" href=\"#\"><i class=\"glyphicon glyphicon-trash\" style=\"margin-right:5px\"></i>" + Utility.Xoa + "</a></td>";
                        BodyField += "</tr>";
                    }
                    else
                    {
                        BodyField0 += "<tr data-id=\"" + itm.ID_KHACHHANG_NCC + "\" data-parent=\"0\" data-level=\"1\" id=\"" + itm.ID_KHACHHANG_NCC + "\">";
                        BodyField0 += "<td style=\"white-space: nowrap; \" id=\"NGAYLAP\">" + itm.NGAYLAP.ToString("dd/MM/yyyy") + "</td>";
                        BodyField0 += "<td style=\"white-space: nowrap; \" id=\"MAPHIEU\">" + itm.MAPHIEUXUAT + " (" + (itm.SOLAN > 0 ? itm.SOLAN : (lstProduct.Max(s => s.SOLAN) > 0 ? (lstProduct.Max(s => s.SOLAN) + 1) : 1)).ToString() + ")" + "</td>";
                        BodyField0 += "<td style=\"white-space: nowrap; \" id=\"NAME_KHACHHANG_NCC\">" + itm.NAME_KHACHHANG_NCC + "</td>";
                        BodyField0 += "<td style=\"white-space: nowrap;text-align: right; \" id=\"TONGSOLUONG\">" + itm.TONGSOLUONG.ToString("N0") + "</td>";
                        BodyField0 += "<td style=\"white-space: nowrap;text-align: right; \" id=\"TONGKHOILUONG\">" + itm.TONGKHOILUONG.ToString("N0") + "</td>";
                        BodyField0 += "<td style=\"white-space: nowrap;text-align: right; \" id=\"SOTIENGIAOHANG\">" + itm.SOTIENGIAOHANG.ToString("N0") + "</td>";
                        BodyField0 += "<td style=\"white-space: nowrap; \"><a class=\"label label-default\" onclick=\"myFunctionViewReport('" + API.ct_PhieuXuat + "','" + itm.ID_PHIEUXUAT + "')\" href=\"#\"><i class=\"fa fa-print\" style=\"margin-right:5px\"></i></a></td>";
                        BodyField0 += "<td style=\"visibility: hidden; display: none;\" id=\"Detail\"><input type=\"checkbox\" class=\"form-control\" name=\"txtDetail|" + ShowSearchValue + "\" id=\"Detail\" checked/></td>";
                        BodyField0 += "<td style=\"white-space: nowrap; \"><a class=\"label label-danger\" onclick=\"myFunctionDelivery('" + API.ct_PhieuGiaoHang + "','DeleteDeliveryDetail','" + itm.ID_PHIEUXUAT + "')\" href=\"#\"><i class=\"glyphicon glyphicon-trash\" style=\"margin-right:5px\"></i>" + Utility.Xoa + "</a></td>";
                        BodyField0 += "</tr>";
                    }

                    //BodyField += "<tr id=\"" + itm.ID_PHIEUXUAT + "\">";

                    //if (bolEdit)
                    //{

                    //}
                    //else
                    //{
                    //    //BodyField += "<td style=\"white-space: nowrap; \"></td>";
                    //}

                }

                BodyField += "<tbody><tr style=\"color: red;\">";
                BodyField += "<td colspan=\"2\" style=\"font-weight: bold;text-align: center;\">TỔNG:</td>";
                BodyField += "<td style=\"font-weight: bold;text-align: right;\">" + lstProduct.GroupBy(s => s.ID_KHACHHANG_NCC).Count().ToString("N0") + "</td>";
                BodyField += "<td style=\"font-weight: bold;text-align: right;\">" + lstProduct.Sum(s => s.TONGSOLUONG).ToString("N0") + "</td>";
                BodyField += "<td style=\"font-weight: bold;text-align: right;\">" + lstProduct.Sum(s => s.TONGKHOILUONG).ToString("N0") + "</td>";
                BodyField += "<td style=\"font-weight: bold;text-align: right;\">" + lstProduct.Sum(s => s.SOTIENGIAOHANG).ToString("N0") + "</td>";
                BodyField += "</tr></tbody>";
                return BodyField0 + BodyField;
            }
            catch (Exception ex)
            {
                Utility.WriteLog("GetDelivery_Detail", MethodBase.GetCurrentMethod().Name, ex, JsonConvert.SerializeObject(lstProduct));
                return "";
            }
        }

        public static string GetDelivery_Shipper(List<v_ct_PhieuGiaoHang_NhanVienGiao> lstProduct)
        {
            try
            {
                string BodyField = null;
                if (lstProduct == null) return "";
                foreach (var itm in lstProduct)
                {
                    string ShowSearchValue = clsMaHoa.Encrypt(JsonConvert.SerializeObject(itm), clsMaHoa.PassMaHoa);
                    BodyField += "<tr id=\"" + itm.ID_NHANVIENGIAO + "\">";
                    BodyField += "<td style=\"white-space: nowrap; \" id=\"MA_NHANVIEN\">" + itm.MA_NHANVIEN + "</td>";
                    BodyField += "<td style=\"white-space: nowrap; \" id=\"NAME_NHANVIEN\">" + itm.NAME_NHANVIEN + "</td>";
                    BodyField += "<td style=\"visibility: hidden; display: none;\" id=\"Shipper\"><input type=\"checkbox\" class=\"form-control\" name=\"txtShipper|" + ShowSearchValue + "\" id=\"Shipper\" checked/></td>";
                    BodyField += "<td style=\"white-space: nowrap; \"><a class=\"label label-danger\" onclick=\"myFunctionDelivery('" + API.ct_PhieuGiaoHang + "','DeleteDeliveryShipper','" + itm.ID_NHANVIENGIAO + "')\" href=\"#\"><i class=\"glyphicon glyphicon-trash\" style=\"margin-right:5px\"></i>" + Utility.Xoa + "</a></td>";
                }
                return BodyField;
            }
            catch (Exception ex)
            {
                Utility.WriteLog("GetDelivery_Shipper", MethodBase.GetCurrentMethod().Name, ex, JsonConvert.SerializeObject(lstProduct));
                return "";
            }
        }
        #endregion

        #region Lấy danh sách sản phẩm kho
        public static ApiResponse Get_DanhSachSanPhamKho<T>(string ID_KHO, bool bolTonKho, string ID_HANGHOAKHO = "", string KEY = "", string LOAITIMKIEM = "")
        {
            SP_Parameter objParameter = new SP_Parameter();
            objParameter.LOC_ID = Utility.LOC_ID;
            objParameter.ID_KHO = ID_KHO;
            objParameter.BOLTONKHO = bolTonKho;
            objParameter.ID_HANGHOAKHO = ID_HANGHOAKHO;
            objParameter.KEY = KEY;
            objParameter.LOAITIMKIEM = LOAITIMKIEM;
            ApiResponse apiResponse = Utility.ExecuteStoredProc<T>(objParameter, API.Sp_Get_DanhSachSanPhamKho);
            return apiResponse;
        }
        #endregion

        #region Lấy danh sách phiếu nhập
        public static ApiResponse Get_DanhSachPhieuNhap<T>(string ID_KHO, DateTime? TUNGAY, DateTime? DENNAY, string SearchString = "", string IDPHIEU = "")
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                SP_Parameter objParameter = new SP_Parameter();
                objParameter.LOC_ID = Utility.LOC_ID;
                objParameter.ID_KHO = ID_KHO;
                objParameter.TUNGAY = TUNGAY;
                objParameter.DENNGAY = DENNAY;
                objParameter.KEY = SearchString;
                objParameter.ID_PHIEUNHAP = IDPHIEU;
                if (SearchString.StartsWith("PGH"))
                    apiResponse = Utility.ExecuteStoredProc<T>(objParameter, API.Sp_Get_DanhSachPhieuNhap_PhieuGiaoHang);
                else
                    apiResponse = Utility.ExecuteStoredProc<T>(objParameter, API.Sp_Get_DanhSachPhieuNhap);
                return apiResponse;
            }
            catch (Exception ex)
            {
                //Utility.WriteLog(response != null ? response.RequestMessage.RequestUri.ToString() : "GetListData", MethodBase.GetCurrentMethod().Name, ex, "");
                return apiResponse = new ApiResponse
                {
                    Success = false,
                    Message = ex.Message,
                    Data = ""
                };
            }
        }
        #endregion

        #region Lấy danh sách phiếu đặt hàng NCC
        public static ApiResponse Get_DanhSachPhieuDatHangNCC<T>(string ID_KHO, DateTime? TUNGAY, DateTime? DENNAY, string SearchString = "", string IDPHIEU = "")
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                SP_Parameter objParameter = new SP_Parameter();
                objParameter.LOC_ID = Utility.LOC_ID;
                objParameter.ID_KHO = ID_KHO;
                objParameter.TUNGAY = TUNGAY;
                objParameter.DENNGAY = DENNAY;
                objParameter.KEY = SearchString;
                objParameter.ID_PHIEUNHAP = IDPHIEU;
                
                apiResponse = Utility.ExecuteStoredProc<T>(objParameter, API.Sp_Get_DanhSachPhieuDatHangNCC);
                return apiResponse;
            }
            catch (Exception ex)
            {
                //Utility.WriteLog(response != null ? response.RequestMessage.RequestUri.ToString() : "GetListData", MethodBase.GetCurrentMethod().Name, ex, "");
                return apiResponse = new ApiResponse
                {
                    Success = false,
                    Message = ex.Message,
                    Data = ""
                };
            }
        }
        #endregion

        #region Lấy danh sách phiếu xuất
        public static ApiResponse Get_DanhSachPhieuXuat<T>(string ID_KHO, DateTime? TUNGAY, DateTime? DENNAY, string SearchString = "", string IDPHIEU = "", string ID_KHUVUC = "")
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                SP_Parameter objParameter = new SP_Parameter();
                objParameter.LOC_ID = Utility.LOC_ID;
                objParameter.ID_KHO = ID_KHO;
                objParameter.TUNGAY = TUNGAY;
                objParameter.DENNGAY = DENNAY;
                objParameter.ID_KHUVUC = ID_KHUVUC;
                objParameter.KEY = SearchString;
                objParameter.ID_PHIEUXUAT = IDPHIEU;
                if (SearchString.StartsWith("PGH"))
                    apiResponse = Utility.ExecuteStoredProc<T>(objParameter, API.Sp_Get_DanhSachPhieuXuat_PhieuGiaoHang);
                else
                    apiResponse = Utility.ExecuteStoredProc<T>(objParameter, API.Sp_Get_DanhSachPhieuXuat);
                return apiResponse;
            }
            catch (Exception ex)
            {
                //Utility.WriteLog(response != null ? response.RequestMessage.RequestUri.ToString() : "GetListData", MethodBase.GetCurrentMethod().Name, ex, "");
                return apiResponse = new ApiResponse
                {
                    Success = false,
                    Message = ex.Message,
                    Data = ""
                };
            }
        }

        public static ApiResponse Get_DanhSachPhieuXuat_TimKiem<T>(string ID_KHO, DateTime? TUNGAY, DateTime? DENNAY, string SearchString = "", string TypeSearch = "", string ID_KHUVUC = "")
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                SP_Parameter objParameter = new SP_Parameter();
                objParameter.LOC_ID = Utility.LOC_ID;
                objParameter.ID_KHO = ID_KHO;
                objParameter.TUNGAY = TUNGAY;
                objParameter.DENNGAY = DENNAY;
                objParameter.ID_KHUVUC = ID_KHUVUC;
                objParameter.KEY = SearchString;
                apiResponse = Utility.ExecuteStoredProc<T>(objParameter, API.Sp_Get_DanhSachPhieuXuat_TimKiem);
                return apiResponse;
            }
            catch (Exception ex)
            {
                //Utility.WriteLog(response != null ? response.RequestMessage.RequestUri.ToString() : "GetListData", MethodBase.GetCurrentMethod().Name, ex, "");
                return apiResponse = new ApiResponse
                {
                    Success = false,
                    Message = ex.Message,
                    Data = ""
                };
            }
        }
        #endregion

        #region Lấy danh sách phiếu thu
        public static ApiResponse Get_DanhSachPhieuThu<T>(string ID_KHO, DateTime? TUNGAY, DateTime? DENNAY, string SearchString = "", string IDPHIEU = "")
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                SP_Parameter objParameter = new SP_Parameter();
                objParameter.LOC_ID = Utility.LOC_ID;
                objParameter.ID_KHO = ID_KHO;
                objParameter.TUNGAY = TUNGAY;
                objParameter.DENNGAY = DENNAY;
                objParameter.KEY = SearchString;
                objParameter.ID_PHIEUTHU = IDPHIEU;
                if(SearchString.StartsWith("PGH"))
                    apiResponse = Utility.ExecuteStoredProc<T>(objParameter, API.Sp_Get_DanhSachPhieuThu_PhieuGiaoHang);
                else
                    apiResponse = Utility.ExecuteStoredProc<T>(objParameter, API.Sp_Get_DanhSachPhieuThu);
                return apiResponse;
            }
            catch (Exception ex)
            {
                //Utility.WriteLog(response != null ? response.RequestMessage.RequestUri.ToString() : "GetListData", MethodBase.GetCurrentMethod().Name, ex, "");
                return apiResponse = new ApiResponse
                {
                    Success = false,
                    Message = ex.Message,
                    Data = ""
                };
            }
        }
        #endregion

        #region Lấy danh sách phiếu chi
        public static ApiResponse Get_DanhSachPhieuChi<T>(string ID_KHO, DateTime? TUNGAY, DateTime? DENNAY, string SearchString = "", string IDPHIEU = "")
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                SP_Parameter objParameter = new SP_Parameter();
                objParameter.LOC_ID = Utility.LOC_ID;
                objParameter.ID_KHO = ID_KHO;
                objParameter.TUNGAY = TUNGAY;
                objParameter.DENNGAY = DENNAY;
                objParameter.KEY = SearchString;
                objParameter.ID_PHIEUCHI = IDPHIEU;
                if (SearchString.StartsWith("PGH"))
                    apiResponse = Utility.ExecuteStoredProc<T>(objParameter, API.Sp_Get_DanhSachPhieuChi_PhieuGiaoHang);
                else
                    apiResponse = Utility.ExecuteStoredProc<T>(objParameter, API.Sp_Get_DanhSachPhieuChi);
                return apiResponse;
            }
            catch (Exception ex)
            {
                //Utility.WriteLog(response != null ? response.RequestMessage.RequestUri.ToString() : "GetListData", MethodBase.GetCurrentMethod().Name, ex, "");
                return apiResponse = new ApiResponse
                {
                    Success = false,
                    Message = ex.Message,
                    Data = ""
                };
            }
        }
        #endregion

        #region Lấy danh sách phiếu lương
        public static ApiResponse Get_DanhSachPhieuLuong<T>(DateTime? TUNGAY, DateTime? DENNAY, string SearchString = "", string IDPHIEU = "")
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                SP_Parameter objParameter = new SP_Parameter();
                objParameter.LOC_ID = Utility.LOC_ID;
                objParameter.TUNGAY = TUNGAY;
                objParameter.DENNGAY = DENNAY;
                objParameter.KEY = SearchString;
                apiResponse = Utility.ExecuteStoredProc<T>(objParameter, API.Sp_Get_DanhSachBangLuong);
                return apiResponse;
            }
            catch (Exception ex)
            {
                //Utility.WriteLog(response != null ? response.RequestMessage.RequestUri.ToString() : "GetListData", MethodBase.GetCurrentMethod().Name, ex, "");
                return apiResponse = new ApiResponse
                {
                    Success = false,
                    Message = ex.Message,
                    Data = ""
                };
            }
        }
        #endregion

        #region Lấy danh sách phiếu đặt hàng
        public static ApiResponse Get_DanhSachPhieuDatHang<T>(string ID_KHO, DateTime? TUNGAY, DateTime? DENNAY, string SearchString = "", string IDPHIEU = "",string ID_KHUVUC = "")
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                SP_Parameter objParameter = new SP_Parameter();
                objParameter.LOC_ID = Utility.LOC_ID;
                objParameter.ID_KHO = ID_KHO;
                objParameter.TUNGAY = TUNGAY;
                objParameter.DENNGAY = DENNAY;
                objParameter.KEY = SearchString;
                objParameter.ID_PHIEUDATHANG = IDPHIEU;
                objParameter.ID_KHUVUC = ID_KHUVUC;
                apiResponse = Utility.ExecuteStoredProc<T>(objParameter, API.Sp_Get_DanhSachPhieuDatHang);
                return apiResponse;
            }
            catch (Exception ex)
            {
                //Utility.WriteLog(response != null ? response.RequestMessage.RequestUri.ToString() : "GetListData", MethodBase.GetCurrentMethod().Name, ex, "");
                return apiResponse = new ApiResponse
                {
                    Success = false,
                    Message = ex.Message,
                    Data = ""
                };
            }
        }
        #endregion

        #region Lấy danh sách phiếu giao hàng
        public static ApiResponse Get_DanhSachPhieuGiaoHang<T>(string ID_KHO, DateTime? TUNGAY, DateTime? DENNAY, string SearchString = "", string TypeSearch = "")
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                SP_Parameter objParameter = new SP_Parameter();
                objParameter.LOC_ID = Utility.LOC_ID;
                objParameter.TUNGAY = TUNGAY;
                objParameter.DENNGAY = DENNAY;
                objParameter.KEY = SearchString;
                apiResponse = Utility.ExecuteStoredProc<T>(objParameter, API.Sp_Get_DanhSachPhieuGiaoHang);
                return apiResponse;
            }
            catch (Exception ex)
            {
                //Utility.WriteLog(response != null ? response.RequestMessage.RequestUri.ToString() : "GetListData", MethodBase.GetCurrentMethod().Name, ex, "");
                return apiResponse = new ApiResponse
                {
                    Success = false,
                    Message = ex.Message,
                    Data = ""
                };
            }
        }
        #endregion

        #region Lấy danh sách công nợ 
        public static ApiResponse Get_ThongKeCongNoKhachHang<T>(SP_Parameter objParameter)
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                apiResponse = Utility.ExecuteStoredProc<T>(objParameter, API.Sp_Get_ThongKeCongNoKhachHang);
                return apiResponse;
            }
            catch (Exception ex)
            {
                //Utility.WriteLog(response != null ? response.RequestMessage.RequestUri.ToString() : "GetListData", MethodBase.GetCurrentMethod().Name, ex, "");
                return apiResponse = new ApiResponse
                {
                    Success = false,
                    Message = ex.Message,
                    Data = ""
                };
            }
        }
        public static ApiResponse Get_ThongKeCongNoNhaCungCap<T>(SP_Parameter objParameter)
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                apiResponse = Utility.ExecuteStoredProc<T>(objParameter, API.Sp_Get_ThongKeCongNoNhaCungCap);
                return apiResponse;
            }
            catch (Exception ex)
            {
                //Utility.WriteLog(response != null ? response.RequestMessage.RequestUri.ToString() : "GetListData", MethodBase.GetCurrentMethod().Name, ex, "");
                return apiResponse = new ApiResponse
                {
                    Success = false,
                    Message = ex.Message,
                    Data = ""
                };
            }
        }
        public static ApiResponse Get_ThongKeCongNoNhanVien<T>(SP_Parameter objParameter)
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                apiResponse = Utility.ExecuteStoredProc<T>(objParameter, API.Sp_Get_ThongKeCongNoNhanVien);
                return apiResponse;
            }
            catch (Exception ex)
            {
                //Utility.WriteLog(response != null ? response.RequestMessage.RequestUri.ToString() : "GetListData", MethodBase.GetCurrentMethod().Name, ex, "");
                return apiResponse = new ApiResponse
                {
                    Success = false,
                    Message = ex.Message,
                    Data = ""
                };
            }
        }
        public static ApiResponse Get_ThongKeTonKhoHangHoa<T>(SP_Parameter objParameter)
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                apiResponse = Utility.ExecuteStoredProc<T>(objParameter, API.Sp_Get_ThongKeTonKhoHangHoa);              
                return apiResponse;
            }
            catch (Exception ex)
            {
                //Utility.WriteLog(response != null ? response.RequestMessage.RequestUri.ToString() : "GetListData", MethodBase.GetCurrentMethod().Name, ex, "");
                return apiResponse = new ApiResponse
                {
                    Success = false,
                    Message = ex.Message,
                    Data = ""
                };
            }
        }
        #endregion

        #region Quỹ tiền
        public static ApiResponse Get_ThongKeQuyTien<T>(SP_Parameter objParameter)
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                apiResponse = Utility.ExecuteStoredProc<T>(objParameter, API.Sp_Get_ThongKeQuyTien);
                return apiResponse;
            }
            catch (Exception ex)
            {
                //Utility.WriteLog(response != null ? response.RequestMessage.RequestUri.ToString() : "GetListData", MethodBase.GetCurrentMethod().Name, ex, "");
                return apiResponse = new ApiResponse
                {
                    Success = false,
                    Message = ex.Message,
                    Data = ""
                };
            }
        }

        public static ApiResponse GetMoneyFundDetail<T>(v_ThongKeQuyTien model, string name = "Books")
        {
            ApiResponse apiResponse = new ApiResponse();
            HttpResponseMessage response = null;
            StringContent content = null;
            List<T> lstPage = new List<T>();
            string strcontent = "";
            try
            {
                strcontent = JsonConvert.SerializeObject(model);
                content = new StringContent(strcontent, Encoding.UTF8, "application/json");
                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + Token);
                response = client.PostAsync(URL + name, content).Result;

                if (response.IsSuccessStatusCode)
                {
                    var dataObjects = response.Content.ReadAsStringAsync().Result;
                    apiResponse = JsonConvert.DeserializeObject<ApiResponse>(dataObjects);
                    if (apiResponse.Data != null)
                        lstPage = JsonConvert.DeserializeObject<List<T>>(apiResponse.Data.ToString());
                    apiResponse.Data = lstPage;
                }
                else
                {
                    apiResponse.Message = GetErrorServer(response);
                }
            }
            catch (Exception ex)
            {
                Utility.WriteLog(response != null ? response.RequestMessage.RequestUri.ToString() : "GetDebtDetail", MethodBase.GetCurrentMethod().Name, ex, strcontent);
                apiResponse = new ApiResponse
                {
                    Success = false,
                    Message = ex.Message,
                    Data = ""
                };
            }
            return apiResponse;
        }
        #endregion

        #region Báo cáo nhân viên
        public static ApiResponse Get_ThongKeBaoCaoNhanVien<T>(SP_Parameter objParameter)
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                apiResponse = Utility.ExecuteStoredProc<T>(objParameter, API.Sp_Get_BaoCaoTheoNhanVien);
                return apiResponse;
            }
            catch (Exception ex)
            {
                //Utility.WriteLog(response != null ? response.RequestMessage.RequestUri.ToString() : "GetListData", MethodBase.GetCurrentMethod().Name, ex, "");
                return apiResponse = new ApiResponse
                {
                    Success = false,
                    Message = ex.Message,
                    Data = ""
                };
            }
        }

        public static ApiResponse GetReportEmployeeDetail<T>(Sp_Get_BaoCaoTheoNhanVien_Result model, string name = "Books")
        {
            ApiResponse apiResponse = new ApiResponse();
            HttpResponseMessage response = null;
            StringContent content = null;
            List<T> lstPage = new List<T>();
            string strcontent = "";
            try
            {
                strcontent = JsonConvert.SerializeObject(model);
                content = new StringContent(strcontent, Encoding.UTF8, "application/json");
                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + Token);
                response = client.PostAsync(URL + name, content).Result;

                if (response.IsSuccessStatusCode)
                {
                    var dataObjects = response.Content.ReadAsStringAsync().Result;
                    apiResponse = JsonConvert.DeserializeObject<ApiResponse>(dataObjects);
                    if (apiResponse.Data != null)
                        lstPage = JsonConvert.DeserializeObject<List<T>>(apiResponse.Data.ToString());
                    apiResponse.Data = lstPage;
                }
                else
                {
                    apiResponse.Message = GetErrorServer(response);
                }
            }
            catch (Exception ex)
            {
                Utility.WriteLog(response != null ? response.RequestMessage.RequestUri.ToString() : "GetDebtDetail", MethodBase.GetCurrentMethod().Name, ex, strcontent);
                apiResponse = new ApiResponse
                {
                    Success = false,
                    Message = ex.Message,
                    Data = ""
                };
            }
            return apiResponse;
        }
        #endregion

        #region Thông kê thu chi
        public static ApiResponse Get_ThongKeThuChi<T>(SP_Parameter_Report objParameter, string Name = API.Sp_Get_ThongKeThuChi_GroupBy)
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                apiResponse = Utility.ExecuteStoredProcT<T>(objParameter, Name);
                return apiResponse;
            }
            catch (Exception ex)
            {
                //Utility.WriteLog(response != null ? response.RequestMessage.RequestUri.ToString() : "GetListData", MethodBase.GetCurrentMethod().Name, ex, "");
                return apiResponse = new ApiResponse
                {
                    Success = false,
                    Message = ex.Message,
                    Data = ""
                };
            }
        }
        #endregion

        #region Tính tổng 
        private static int SoNguyenTongCong = 0;
        private static int TongCongKhac = 2;
        public static void TinhTong(Product_Detail Product_Detail, string VALUE = null, List<Product_Detail> lstProduct = null)
        {
            if (Product_Detail.TYPE == "ID_THUESUAT")
            {
                if (VALUE != null)
                    Product_Detail.ID_THUESUAT = VALUE;
                if (string.IsNullOrEmpty(Product_Detail.ID_THUESUAT))
                {
                    Product_Detail.THUESUAT = 0;
                    Product_Detail.TONGTIENVAT = 0;
                    Product_Detail.TONGCONG = Product_Detail.THANHTIEN + Product_Detail.TONGTIENVAT;
                }
                else
                {
                    var apiResponseVAT = Utility.GetDetail<v_v_dm_ThueSuat>(Utility.LOC_ID + "/" + Product_Detail.ID_THUESUAT, API.dm_ThueSuat);
                    if (apiResponseVAT.Data != null)
                    {
                        v_v_dm_ThueSuat dm_ThueSuat = apiResponseVAT.Data as v_v_dm_ThueSuat;

                        if (dm_ThueSuat != null)
                        {
                            Product_Detail.THUESUAT = dm_ThueSuat.THUESUAT;
                            Product_Detail.TONGTIENVAT = Product_Detail.THANHTIEN * Product_Detail.THUESUAT / 100;
                            Product_Detail.TONGCONG = Product_Detail.THANHTIEN + Product_Detail.TONGTIENVAT;
                        }
                    }
                }
            }
            else if (Product_Detail.TYPE == "SOLUONG")
            {
                if (!string.IsNullOrEmpty(VALUE))
                    Product_Detail.SOLUONG = Math.Round( Utility.ConvertStringToDouble(VALUE), TongCongKhac);
                Product_Detail.TONGTIENGIAMGIA = Math.Round((Product_Detail.SOLUONG * Product_Detail.DONGIA) * Product_Detail.CHIETKHAU / 100, TongCongKhac);
                Product_Detail.THANHTIEN = Math.Round((Product_Detail.SOLUONG * Product_Detail.DONGIA) - Product_Detail.TONGTIENGIAMGIA, TongCongKhac);
                Product_Detail.TONGTIENVAT = Math.Round(Product_Detail.THANHTIEN * Product_Detail.THUESUAT / 100, TongCongKhac);
                Product_Detail.TONGCONG = Math.Round(Product_Detail.THANHTIEN + Product_Detail.TONGTIENVAT, SoNguyenTongCong);
            }
            else if (Product_Detail.TYPE == "DONGIA")
            {
                if (!string.IsNullOrEmpty(VALUE))
                    Product_Detail.DONGIA = Math.Round(Utility.ConvertStringToDouble(VALUE), TongCongKhac);
                Product_Detail.TONGTIENGIAMGIA = Math.Round((Product_Detail.SOLUONG * Product_Detail.DONGIA) * Product_Detail.CHIETKHAU / 100, TongCongKhac);
                Product_Detail.THANHTIEN = Math.Round((Product_Detail.SOLUONG * Product_Detail.DONGIA) - Product_Detail.TONGTIENGIAMGIA, TongCongKhac);
                Product_Detail.TONGTIENVAT = Math.Round(Product_Detail.THANHTIEN * Product_Detail.THUESUAT / 100, TongCongKhac);
                Product_Detail.TONGCONG = Math.Round(Product_Detail.THANHTIEN + Product_Detail.TONGTIENVAT, SoNguyenTongCong);
            }
            else if (Product_Detail.TYPE == "TONGTIENGIAMGIA")
            {
                if (!string.IsNullOrEmpty(VALUE))
                    Product_Detail.TONGTIENGIAMGIA = Math.Round(Utility.ConvertStringToDouble(VALUE), TongCongKhac);
                Product_Detail.THANHTIEN = Math.Round((Product_Detail.SOLUONG * Product_Detail.DONGIA) - Product_Detail.TONGTIENGIAMGIA, TongCongKhac);
                Product_Detail.TONGTIENVAT = Math.Round(Product_Detail.THANHTIEN * Product_Detail.THUESUAT / 100, TongCongKhac);
                Product_Detail.TONGCONG = Math.Round(Product_Detail.THANHTIEN + Product_Detail.TONGTIENVAT, SoNguyenTongCong);
            }
            else if (Product_Detail.TYPE == "CHIETKHAU")
            {
                if (!string.IsNullOrEmpty(VALUE))
                    Product_Detail.CHIETKHAU = Math.Round(Utility.ConvertStringToDouble(VALUE), TongCongKhac);
                Product_Detail.TONGTIENGIAMGIA = Math.Round((Product_Detail.SOLUONG * Product_Detail.DONGIA) * Product_Detail.CHIETKHAU / 100, TongCongKhac);
                Product_Detail.THANHTIEN = Math.Round((Product_Detail.SOLUONG * Product_Detail.DONGIA) - Product_Detail.TONGTIENGIAMGIA, TongCongKhac);
                Product_Detail.TONGTIENVAT = Math.Round(Product_Detail.THANHTIEN * Product_Detail.THUESUAT / 100, TongCongKhac);
                Product_Detail.TONGCONG = Math.Round(Product_Detail.THANHTIEN + Product_Detail.TONGTIENVAT, SoNguyenTongCong);
            }
            else if (Product_Detail.TYPE == "TONGTIENVAT")
            {
                if (!string.IsNullOrEmpty(VALUE))
                    Product_Detail.TONGTIENVAT = Math.Round(Utility.ConvertStringToDouble(VALUE), TongCongKhac);
                Product_Detail.TONGCONG = Math.Round(Product_Detail.THANHTIEN + Product_Detail.TONGTIENVAT, SoNguyenTongCong);
            }
            else if (Product_Detail.TYPE == "THANHTIEN")
            {
                if (!string.IsNullOrEmpty(VALUE))
                    Product_Detail.THANHTIEN = Math.Round(Utility.ConvertStringToDouble(VALUE), TongCongKhac);
                Product_Detail.TONGTIENVAT = Math.Round(Product_Detail.THANHTIEN * Product_Detail.THUESUAT / 100, TongCongKhac);
                Product_Detail.TONGCONG = Math.Round(Product_Detail.THANHTIEN + Product_Detail.TONGTIENVAT, SoNguyenTongCong);
            }
            else if (Product_Detail.TYPE == "DONGIA")
            {
                if (!string.IsNullOrEmpty(VALUE))
                    Product_Detail.DONGIA = Math.Round(Utility.ConvertStringToDouble(VALUE), TongCongKhac);

                Product_Detail.TONGTIENGIAMGIA = Math.Round((Product_Detail.SOLUONG * Product_Detail.DONGIA) * Product_Detail.CHIETKHAU / 100, TongCongKhac);
                Product_Detail.THANHTIEN = Math.Round((Product_Detail.SOLUONG * Product_Detail.DONGIA) - Product_Detail.TONGTIENGIAMGIA, TongCongKhac);
                Product_Detail.TONGTIENVAT = Math.Round(Product_Detail.THANHTIEN * Product_Detail.THUESUAT / 100, TongCongKhac);
                Product_Detail.TONGCONG = Math.Round(Product_Detail.THANHTIEN + Product_Detail.TONGTIENVAT, SoNguyenTongCong);
            }
            else if (Product_Detail.TYPE == "TONGCONG")
            {
                if (!string.IsNullOrEmpty(VALUE))
                    Product_Detail.TONGCONG = Math.Round(Utility.ConvertStringToDouble(VALUE), TongCongKhac);
            }
            else
            {
                Product_Detail.TONGTIENGIAMGIA = Math.Round((Product_Detail.SOLUONG * Product_Detail.DONGIA) * Product_Detail.CHIETKHAU / 100, TongCongKhac);
                Product_Detail.THANHTIEN = Math.Round((Product_Detail.SOLUONG * Product_Detail.DONGIA) - Product_Detail.TONGTIENGIAMGIA, TongCongKhac);
                Product_Detail.TONGTIENVAT = Math.Round(Product_Detail.THANHTIEN * Product_Detail.THUESUAT / 100, TongCongKhac);
                Product_Detail.TONGCONG = Math.Round(Product_Detail.THANHTIEN + Product_Detail.TONGTIENVAT, SoNguyenTongCong);
            }

            if (!string.IsNullOrEmpty(Product_Detail.ID_COMBO))
            {
                var lstProductCombo = lstProduct.Where(e => e.ID_COMBO == Product_Detail.ID_COMBO && e.ISCOMBO);
                foreach (var itm in lstProductCombo)
                {
                    itm.ID_DVT = itm.ID_DVT_COMBO;
                    itm.SOLUONG = Product_Detail.SOLUONG * itm.QTY_COMBO;
                    itm.TYLE_QD = itm.TYLE_QD_COMBO;
                    itm.TONGSOLUONG = Product_Detail.SOLUONG * itm.QTY_TOTAL_COMBO;
                    itm.DONGIA = 0;
                    itm.ISCOMBO = true;
                    itm.ID_COMBO = Product_Detail.ID_HANGHOA;
                }
            }
        }
        #endregion

        #region Đọc tiền bằng chữ 
        private static string[] ChuSo = new string[10] { " không", " một", " hai", " ba", " bốn", " năm", " sáu", " bẩy", " tám", " chín" };
        private static string[] Tien = new string[6] { "", " nghìn", " triệu", " tỷ", " nghìn tỷ", " triệu tỷ" };
        // Hàm đọc số thành chữ
        public static string DocTienBangChu(long SoTien, string strTail = " đồng")
        {
            int lan, i;
            long so;
            string KetQua = "", tmp = "";
            int[] ViTri = new int[6];
            if (SoTien < 0) return "Số tiền âm !";
            if (SoTien == 0) return "Không đồng !";
            if (SoTien > 0)
            {
                so = SoTien;
            }
            else
            {
                so = -SoTien;
            }
            //Kiểm tra số quá lớn
            if (SoTien > 8999999999999999)
            {
                SoTien = 0;
                return "";
            }
            ViTri[5] = (int)(so / 1000000000000000);
            so = so - long.Parse(ViTri[5].ToString()) * 1000000000000000;
            ViTri[4] = (int)(so / 1000000000000);
            so = so - long.Parse(ViTri[4].ToString()) * +1000000000000;
            ViTri[3] = (int)(so / 1000000000);
            so = so - long.Parse(ViTri[3].ToString()) * 1000000000;
            ViTri[2] = (int)(so / 1000000);
            ViTri[1] = (int)((so % 1000000) / 1000);
            ViTri[0] = (int)(so % 1000);
            if (ViTri[5] > 0)
            {
                lan = 5;
            }
            else if (ViTri[4] > 0)
            {
                lan = 4;
            }
            else if (ViTri[3] > 0)
            {
                lan = 3;
            }
            else if (ViTri[2] > 0)
            {
                lan = 2;
            }
            else if (ViTri[1] > 0)
            {
                lan = 1;
            }
            else
            {
                lan = 0;
            }
            for (i = lan; i >= 0; i--)
            {
                tmp = DocSo3ChuSo(ViTri[i]);
                KetQua += tmp;
                if (ViTri[i] != 0) KetQua += Tien[i];
                if ((i > 0) && (!string.IsNullOrEmpty(tmp))) KetQua += "";//&& (!string.IsNullOrEmpty(tmp))
            }
            if (KetQua.Substring(KetQua.Length - 1, 1) == ",") KetQua = KetQua.Substring(0, KetQua.Length - 1);
            KetQua = KetQua.Trim() + strTail;
            return KetQua.Substring(0, 1).ToUpper() + KetQua.Substring(1);
        }
        // Hàm đọc số có 3 chữ số
        private static string DocSo3ChuSo(int baso)
        {
            int tram, chuc, donvi;
            string KetQua = "";
            tram = (int)(baso / 100);
            chuc = (int)((baso % 100) / 10);
            donvi = baso % 10;
            if ((tram == 0) && (chuc == 0) && (donvi == 0)) return "";
            if (tram != 0)
            {
                KetQua += ChuSo[tram] + " trăm";
                if ((chuc == 0) && (donvi != 0)) KetQua += " linh";
            }
            if ((chuc != 0) && (chuc != 1))
            {
                KetQua += ChuSo[chuc] + " mươi";
                if ((chuc == 0) && (donvi != 0)) KetQua = KetQua + " linh";
            }
            if (chuc == 1) KetQua += " mười";
            switch (donvi)
            {
                case 1:
                    if ((chuc != 0) && (chuc != 1))
                    {
                        KetQua += " mốt";
                    }
                    else
                    {
                        KetQua += ChuSo[donvi];
                    }
                    break;
                case 5:
                    if (chuc == 0)
                    {
                        KetQua += ChuSo[donvi];
                    }
                    else
                    {
                        KetQua += " lăm";
                    }
                    break;
                default:
                    if (donvi != 0)
                    {
                        KetQua += ChuSo[donvi];
                    }
                    break;
            }
            return KetQua;
        }
        #endregion

        #region Phiếu in
        public static ReportClass GetFormulaFields(ReportClass report, object Master = null, string MapPath = "")
        {
            if (Master == null) return report;
            ApiResponse apiResponse = new ApiResponse();
            v_v_dm_CongTy dm_CongTy = new v_v_dm_CongTy();
            apiResponse = Utility.GetDetail<v_v_dm_CongTy>(Utility.LOC_ID, API.dm_CongTy);
            if (apiResponse.Data != null)
                dm_CongTy = apiResponse.Data as v_v_dm_CongTy;

            switch (Master.GetType().Name)
            {
                case "v_ct_PhieuChi":
                    report.FileName = System.Web.Hosting.HostingEnvironment.MapPath("~/Report/rptPhieuChi.rpt");
                    report.Load();
                    v_ct_PhieuChi PhieuChi = (v_ct_PhieuChi)Master;
                    report.DataDefinition.FormulaFields["NGAYLAP"].Text = "'" + "Ngày " + PhieuChi.NGAYLAP.Day.ToString() + " tháng " + PhieuChi.NGAYLAP.Month.ToString() + " năm " + PhieuChi.NGAYLAP.Year.ToString() + "'";
                    report.DataDefinition.FormulaFields["SOTIENBANGCHU"].Text = "'" + Utility.DocTienBangChu((long)PhieuChi.SOTIEN) + "'";
                    break;
                case "v_ct_PhieuThu":

                    report.FileName = System.Web.Hosting.HostingEnvironment.MapPath("~/Report/rptPhieuThu.rpt");
                    report.Load();
                    v_ct_PhieuThu PhieuThu = (v_ct_PhieuThu)Master;
                    report.DataDefinition.FormulaFields["NGAYLAP"].Text = "'" + "Ngày " + PhieuThu.NGAYLAP.Day.ToString() + " tháng " + PhieuThu.NGAYLAP.Month.ToString() + " năm " + PhieuThu.NGAYLAP.Year.ToString() + "'";
                    report.DataDefinition.FormulaFields["SOTIENBANGCHU"].Text = "'" + Utility.DocTienBangChu((long)PhieuThu.SOTIEN) + "'";
                    break;
                case "v_ct_PhieuNhap":

                    report.FileName = System.Web.Hosting.HostingEnvironment.MapPath("~/Report/rptPhieuNhap.rpt");
                    report.Load();
                    v_ct_PhieuNhap PhieuNhap = (v_ct_PhieuNhap)Master;
                    report.DataDefinition.FormulaFields["MAPHIEU"].Text = "'" + PhieuNhap.MAPHIEU + "'";
                    report.DataDefinition.FormulaFields["NGAYLAP"].Text = "'" + "Ngày " + PhieuNhap.NGAYLAP.Day.ToString() + " tháng " + PhieuNhap.NGAYLAP.Month.ToString() + " năm " + PhieuNhap.NGAYLAP.Year.ToString() + "'";
                    report.DataDefinition.FormulaFields["TENNGUOIMUA"].Text = "'" + CovertText(PhieuNhap.NAME_KHACHHANG_NCC.Replace("'", "")) + "'";
                    report.DataDefinition.FormulaFields["DIACHINGUOIMUA"].Text = "''";
                    report.DataDefinition.FormulaFields["TENKHONHAP"].Text = "'" + CovertText(PhieuNhap.NAME_KHO) + "'";
                    report.DataDefinition.FormulaFields["LOAIPHIEUNHAP"].Text = "'" + CovertText(PhieuNhap.NAME_LOAIPHIEUNHAP) + "'";
                    report.DataDefinition.FormulaFields["GHICHU"].Text = "'" + CovertText(PhieuNhap.GHICHU.Replace("'", "")) + "'";
                    break;

                case "v_ct_PhieuDatHangNCC":

                    report.FileName = System.Web.Hosting.HostingEnvironment.MapPath("~/Report/rptPhieuDatHangNCC.rpt");
                    report.Load();
                    v_ct_PhieuDatHangNCC PhieuDatHangNCC = (v_ct_PhieuDatHangNCC)Master;
                    report.DataDefinition.FormulaFields["MAPHIEU"].Text = "'" + PhieuDatHangNCC.MAPHIEU + "'";
                    report.DataDefinition.FormulaFields["NGAYLAP"].Text = "'" + "Ngày " + PhieuDatHangNCC.NGAYLAP.Day.ToString() + " tháng " + PhieuDatHangNCC.NGAYLAP.Month.ToString() + " năm " + PhieuDatHangNCC.NGAYLAP.Year.ToString() + "'";
                    report.DataDefinition.FormulaFields["TENNGUOIMUA"].Text = "'" + CovertText(PhieuDatHangNCC.NAME_KHACHHANG_NCC.Replace("'", "")) + "'";
                    report.DataDefinition.FormulaFields["DIACHINGUOIMUA"].Text = "''";
                    report.DataDefinition.FormulaFields["TENKHONHAP"].Text = "'" + CovertText(PhieuDatHangNCC.NAME_KHO) + "'";
                    report.DataDefinition.FormulaFields["LOAIPHIEUNHAP"].Text = "'" + CovertText(PhieuDatHangNCC.NAME_LOAIPHIEUNHAP) + "'";
                    report.DataDefinition.FormulaFields["GHICHU"].Text = "'" + CovertText(PhieuDatHangNCC.GHICHU.Replace("'", "")) + "'";
                    break;
                case "v_ct_PhieuXuat":

                    report.FileName = System.Web.Hosting.HostingEnvironment.MapPath("~/Report/rptPhieuXuat.rpt");
                    report.Load();
                    v_ct_PhieuXuat PhieuXuat = (v_ct_PhieuXuat)Master;
                    report.DataDefinition.FormulaFields["MAPHIEU"].Text = "'" + CovertText(PhieuXuat.MAPHIEU) + "'";
                    report.DataDefinition.FormulaFields["NGAYLAP"].Text = "'" + "Ngày " + PhieuXuat.NGAYLAP.Day.ToString() + " tháng " + PhieuXuat.NGAYLAP.Month.ToString() + " năm " + PhieuXuat.NGAYLAP.Year.ToString() + "'";
                    report.DataDefinition.FormulaFields["TENNGUOIMUA"].Text = "'" + CovertText(PhieuXuat.NAME_KHACHHANG_NCC.Replace("'", "")) + "'";
                    report.DataDefinition.FormulaFields["DIACHINGUOIMUA"].Text = "'" + CovertText(PhieuXuat.DIACHI_KHACHHANG_NCC.Replace("'", "")) + "'";
                    report.DataDefinition.FormulaFields["TENKHONHAP"].Text = "'" + CovertText(PhieuXuat.NAME_KHO) + "'";
                    report.DataDefinition.FormulaFields["LOAIPHIEUNHAP"].Text = "'" + CovertText(PhieuXuat.NAME_LOAIPHIEUXUAT) + "'";
                    report.DataDefinition.FormulaFields["GHICHU"].Text = "'" + CovertText(PhieuXuat.GHICHU) + "'";
                    report.DataDefinition.FormulaFields["SOTIENBANGCHU"].Text = "'" + CovertText(PhieuXuat.GHICHU) + "'";
                    
                    break;
                case "v_ct_PhieuDatHang":

                    report.FileName = System.Web.Hosting.HostingEnvironment.MapPath("~/Report/rptPhieuDatHang.rpt");
                    report.Load();
                    v_ct_PhieuDatHang PhieuDatHang = (v_ct_PhieuDatHang)Master;
                    report.DataDefinition.FormulaFields["MAPHIEU"].Text = "'" + CovertText(PhieuDatHang.MAPHIEU) + "'";
                    report.DataDefinition.FormulaFields["NGAYLAP"].Text = "'" + "Ngày " + PhieuDatHang.NGAYLAP.Day.ToString() + " tháng " + PhieuDatHang.NGAYLAP.Month.ToString() + " năm " + PhieuDatHang.NGAYLAP.Year.ToString() + "'";
                    report.DataDefinition.FormulaFields["TENNGUOIMUA"].Text = "'" + CovertText(PhieuDatHang.NAME_KHACHHANG.Replace("'", "")) + "'";
                    report.DataDefinition.FormulaFields["DIACHINGUOIMUA"].Text = "'" + CovertText(PhieuDatHang.ADDRESS.Replace("'", "")) + "'";
                    report.DataDefinition.FormulaFields["TENKHONHAP"].Text = "'" + CovertText(PhieuDatHang.NAME_KHO.Replace("'", "")) + "'";
                    report.DataDefinition.FormulaFields["GHICHU"].Text = "'" + CovertText(PhieuDatHang.GHICHU) + "'";
                    break;
                case "v_ct_PhieuGiaoHang":
                    if (!string.IsNullOrEmpty(MapPath))
                        report.FileName = System.Web.Hosting.HostingEnvironment.MapPath(MapPath);
                    else
                        report.FileName = System.Web.Hosting.HostingEnvironment.MapPath("~/Report/rptPhieuGiaoHang.rpt");
                    report.Load();
                    v_ct_PhieuGiaoHang PhieuGiaoHang = (v_ct_PhieuGiaoHang)Master;
                    report.DataDefinition.FormulaFields["MAPHIEU"].Text = "'" + PhieuGiaoHang.MAPHIEU + "'";
                    report.DataDefinition.FormulaFields["NGAYLAP"].Text = "'" + "Ngày " + PhieuGiaoHang.NGAYLAP.Day.ToString() + " tháng " + PhieuGiaoHang.NGAYLAP.Month.ToString() + " năm " + PhieuGiaoHang.NGAYLAP.Year.ToString() + "'";
                    break;
                case "v_ThongKeCongNoKhachHang":

                    report.FileName = System.Web.Hosting.HostingEnvironment.MapPath("~/Report/rptChiTietCongNo.rpt");
                    report.Load();
                    v_ThongKeCongNoKhachHang ThongKeCongNoKhachHang = (v_ThongKeCongNoKhachHang)Master;
                    report.DataDefinition.FormulaFields["HOVATEN"].Text = "'Khách hàng: " + CovertText(ThongKeCongNoKhachHang.NAME.Replace("'", "")) + "'";
                    report.DataDefinition.FormulaFields["DIACHI_CN"].Text = "'" + CovertText(ThongKeCongNoKhachHang.ADDRESS.Replace("'", "")) + "'";
                    report.DataDefinition.FormulaFields["DIENTHOAI_CN"].Text = "'" + CovertText(ThongKeCongNoKhachHang.TEL) + "'";
                    report.DataDefinition.FormulaFields["DAUKY"].Text = "'" + ThongKeCongNoKhachHang.TONGTIENCONGNODAUKY.ToString("N0") + "'";
                    report.DataDefinition.FormulaFields["CUOIKY"].Text = "'" + ThongKeCongNoKhachHang.TONGTIENCONGNOCUOIKY.ToString("N0") + "'";
                    break;
                case "v_ThongKeCongNoNhaCungCap":

                    report.FileName = System.Web.Hosting.HostingEnvironment.MapPath("~/Report/rptChiTietCongNo.rpt");
                    report.Load();
                    v_ThongKeCongNoNhaCungCap ThongKeCongNoNhaCungCap = (v_ThongKeCongNoNhaCungCap)Master;
                    report.DataDefinition.FormulaFields["HOVATEN"].Text = "'Nhà cung cấp: " + CovertText(ThongKeCongNoNhaCungCap.NAME.Replace("'", "")) + "'";
                    report.DataDefinition.FormulaFields["DIACHI_CN"].Text = "'" + CovertText(ThongKeCongNoNhaCungCap.ADDRESS.Replace("'", "")) + "'";
                    report.DataDefinition.FormulaFields["DIENTHOAI_CN"].Text = "'" + CovertText(ThongKeCongNoNhaCungCap.TEL) + "'";
                    report.DataDefinition.FormulaFields["DAUKY"].Text = "'" + ThongKeCongNoNhaCungCap.TONGTIENCONGNODAUKY.ToString("N0") + "'";
                    report.DataDefinition.FormulaFields["CUOIKY"].Text = "'" + ThongKeCongNoNhaCungCap.TONGTIENCONGNOCUOIKY.ToString("N0") + "'";
                    break;
                case "v_ThongKeCongNoNhanVien":

                    report.FileName = System.Web.Hosting.HostingEnvironment.MapPath("~/Report/rptChiTietCongNo.rpt");
                    report.Load();
                    v_ThongKeCongNoNhanVien ThongKeCongNoNhanVien = (v_ThongKeCongNoNhanVien)Master;
                    report.DataDefinition.FormulaFields["HOVATEN"].Text = "'Nhân viên: " + CovertText(ThongKeCongNoNhanVien.NAME.Replace("'", "")) + "'";
                    report.DataDefinition.FormulaFields["DIACHI_CN"].Text = "'" + CovertText(ThongKeCongNoNhanVien.ADDRESS.Replace("'", "")) + "'";
                    report.DataDefinition.FormulaFields["DIENTHOAI_CN"].Text = "'" + CovertText(ThongKeCongNoNhanVien.TEL) + "'";
                    report.DataDefinition.FormulaFields["DAUKY"].Text = "'" + ThongKeCongNoNhanVien.TONGTIENCONGNODAUKY.ToString("N0") + "'";
                    report.DataDefinition.FormulaFields["CUOIKY"].Text = "'" + ThongKeCongNoNhanVien.TONGTIENCONGNOCUOIKY.ToString("N0") + "'";
                    break;
                case "Sp_Get_BaoCaoGiaoHang_Result":

                    report.FileName = System.Web.Hosting.HostingEnvironment.MapPath("~/Report/rptChiTietCongNo.rpt");
                    report.Load();
                    Sp_Get_BaoCaoGiaoHang_Result Sp_Get_BaoCaoGiaoHang_Result = (Sp_Get_BaoCaoGiaoHang_Result)Master;
                    report.DataDefinition.FormulaFields["HOVATEN"].Text = "'Mã phiếu: " + Sp_Get_BaoCaoGiaoHang_Result.MAPHIEU + "'";
                    report.DataDefinition.FormulaFields["DIACHI_CN"].Text = "''";
                    report.DataDefinition.FormulaFields["DIENTHOAI_CN"].Text = "''";
                    report.DataDefinition.FormulaFields["DAUKY"].Text = "''";
                    report.DataDefinition.FormulaFields["CUOIKY"].Text = "''";
                    break;

                case "v_ThongKeQuyTien":

                    report.FileName = System.Web.Hosting.HostingEnvironment.MapPath("~/Report/rptChiTietQuyTien.rpt");
                    report.Load();
                    v_ThongKeQuyTien ThongKeQuyTien = (v_ThongKeQuyTien)Master;
                    report.DataDefinition.FormulaFields["HOVATEN"].Text = "'Tài khoản: " + CovertText(ThongKeQuyTien.NAME.Replace("'", "")) + "'";
                    report.DataDefinition.FormulaFields["DIACHI_CN"].Text = "'" + CovertText(ThongKeQuyTien.CHUTAIKHOAN + (!string.IsNullOrEmpty(ThongKeQuyTien.SOTAIKHOAN) ? ":" + ThongKeQuyTien.SOTAIKHOAN : "")) + "'";
                    report.DataDefinition.FormulaFields["DIENTHOAI_CN"].Text = "'" + CovertText(ThongKeQuyTien.MANGANHANG + (!string.IsNullOrEmpty(ThongKeQuyTien.TENNGANHANG) ? " - " + ThongKeQuyTien.TENNGANHANG + " " + ThongKeQuyTien.TINHTP : "")) + "'";
                    report.DataDefinition.FormulaFields["DAUKY"].Text = "'" + ThongKeQuyTien.TONGTIENCONGNODAUKY.ToString("N0") + "'";
                    report.DataDefinition.FormulaFields["CUOIKY"].Text = "'" + ThongKeQuyTien.TONGTIENCONGNOCUOIKY.ToString("N0") + "'";
                    break;

                case "Sp_Get_BaoCaoTheoNhanVien_Result":

                    report.FileName = System.Web.Hosting.HostingEnvironment.MapPath("~/Report/rptBaoCaoNhanVien.rpt");
                    report.Load();
                    Sp_Get_BaoCaoTheoNhanVien_Result Sp_Get_BaoCaoTheoNhanVien_Result = (Sp_Get_BaoCaoTheoNhanVien_Result)Master;
                    report.DataDefinition.FormulaFields["HOVATEN"].Text = "'Tài khoản: " + CovertText(Sp_Get_BaoCaoTheoNhanVien_Result.NAME_NHANVIEN.Replace("'", "")) + "'";
                    report.DataDefinition.FormulaFields["DIACHI_CN"].Text = "'" + CovertText(Sp_Get_BaoCaoTheoNhanVien_Result.NAME_LOAIPHIEU.Replace("'", "")) + "'";
                    report.DataDefinition.FormulaFields["DIENTHOAI_CN"].Text = "''";
                    report.DataDefinition.FormulaFields["DAUKY"].Text = "''";
                    report.DataDefinition.FormulaFields["CUOIKY"].Text = "''";
                    break;
            }
            report.DataDefinition.FormulaFields["TENCONGTY"].Text = "'" + CovertText(dm_CongTy.NAME) + "'";
            report.DataDefinition.FormulaFields["DIACHI"].Text = "'" + CovertText(dm_CongTy.ADDRESS) + "'";
            report.DataDefinition.FormulaFields["DIENTHOAI"].Text = "'" + CovertText(dm_CongTy.TEL) + "'";
            report.DataDefinition.FormulaFields["ICON"].Text = "'" + CovertText(dm_CongTy.LOGO) + "'";
            report.SetDatabaseLogon("test", "test@", "test", "test");
            return report;
        }
        private static string CovertText(string text)
        {
            if (string.IsNullOrEmpty(text)) return "";
            text = text.Replace("'", "");
            text = text.Replace("\r\n", " ");
            return text;
        }
        #endregion

        #region Lấy danh sách chi tiết công nợ
        public static ApiResponse GetDebtCustomerDetail<T>(v_ThongKeCongNoKhachHang model, string name = "Books")
        {
            ApiResponse apiResponse = new ApiResponse();
            HttpResponseMessage response = null;
            StringContent content = null;
            List<T> lstPage = new List<T>();
            string strcontent = "";
            try
            {
                strcontent = JsonConvert.SerializeObject(model);
                content = new StringContent(strcontent, Encoding.UTF8, "application/json");
                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + Token);
                response = client.PostAsync(URL + name, content).Result;

                if (response.IsSuccessStatusCode)
                {
                    var dataObjects = response.Content.ReadAsStringAsync().Result;
                    apiResponse = JsonConvert.DeserializeObject<ApiResponse>(dataObjects);
                    if (apiResponse.Data != null)
                        lstPage = JsonConvert.DeserializeObject<List<T>>(apiResponse.Data.ToString());
                    apiResponse.Data = lstPage;
                }
                else
                {
                    apiResponse.Message = GetErrorServer(response);
                }
            }
            catch (Exception ex)
            {
                Utility.WriteLog(response != null ? response.RequestMessage.RequestUri.ToString() : "GetDebtDetail", MethodBase.GetCurrentMethod().Name, ex, strcontent);
                apiResponse = new ApiResponse
                {
                    Success = false,
                    Message = ex.Message,
                    Data = ""
                };
            }
            return apiResponse;
        }

        public static ApiResponse GetDebtCustomerDetail<T>(v_ThongKeCongNoNhaCungCap model, string name = "Books")
        {
            ApiResponse apiResponse = new ApiResponse();
            HttpResponseMessage response = null;
            StringContent content = null;
            List<T> lstPage = new List<T>();
            string strcontent = "";
            try
            {
                strcontent = JsonConvert.SerializeObject(model);
                content = new StringContent(strcontent, Encoding.UTF8, "application/json");
                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + Token);
                response = client.PostAsync(URL + name, content).Result;

                if (response.IsSuccessStatusCode)
                {
                    var dataObjects = response.Content.ReadAsStringAsync().Result;
                    apiResponse = JsonConvert.DeserializeObject<ApiResponse>(dataObjects);
                    if (apiResponse.Data != null)
                        lstPage = JsonConvert.DeserializeObject<List<T>>(apiResponse.Data.ToString());
                    apiResponse.Data = lstPage;
                }
                else
                {
                    apiResponse.Message = GetErrorServer(response);
                }
            }
            catch (Exception ex)
            {
                Utility.WriteLog(response != null ? response.RequestMessage.RequestUri.ToString() : "GetDebtDetail", MethodBase.GetCurrentMethod().Name, ex, strcontent);
                apiResponse = new ApiResponse
                {
                    Success = false,
                    Message = ex.Message,
                    Data = ""
                };
            }
            return apiResponse;
        }

        public static ApiResponse DebtEmployeeDetail<T>(v_ThongKeCongNoNhanVien model, string name = "Books")
        {
            ApiResponse apiResponse = new ApiResponse();
            HttpResponseMessage response = null;
            StringContent content = null;
            List<T> lstPage = new List<T>();
            string strcontent = "";
            try
            {
                strcontent = JsonConvert.SerializeObject(model);
                content = new StringContent(strcontent, Encoding.UTF8, "application/json");
                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + Token);
                response = client.PostAsync(URL + name, content).Result;

                if (response.IsSuccessStatusCode)
                {
                    var dataObjects = response.Content.ReadAsStringAsync().Result;
                    apiResponse = JsonConvert.DeserializeObject<ApiResponse>(dataObjects);
                    if (apiResponse.Data != null)
                        lstPage = JsonConvert.DeserializeObject<List<T>>(apiResponse.Data.ToString());
                    apiResponse.Data = lstPage;
                }
                else
                {
                    apiResponse.Message = GetErrorServer(response);
                }
            }
            catch (Exception ex)
            {
                Utility.WriteLog(response != null ? response.RequestMessage.RequestUri.ToString() : "GetDebtDetail", MethodBase.GetCurrentMethod().Name, ex, strcontent);
                apiResponse = new ApiResponse
                {
                    Success = false,
                    Message = ex.Message,
                    Data = ""
                };
            }
            return apiResponse;
        }

        #endregion

        #region Convert T sang DataTable View Report
        public static DataTable ToDataTable<T>(List<T> list)
        {
            DataTable table = new DataTable(typeof(T).Name);

            //Get Properites of List Fiels
            PropertyInfo[] props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            //Create Columns as Fields of List
            foreach (PropertyInfo propertyInfo in props)
            {
                var column = new DataColumn
                {
                    ColumnName = propertyInfo.Name,
                    DataType = propertyInfo.PropertyType.Name.Contains("Nullable") ? typeof(string) : propertyInfo.PropertyType
                };

                table.Columns.Add(column);
            }
            int STT = 1;
            //Fill DataTable with Rows of List
            foreach (var item in list)
            {
                var values = new object[props.Length];

                for (var i = 0; i < props.Length; i++)
                {
                    if (table.Columns[i].ColumnName == "STT")
                    {
                        values[i] = STT++;
                    }
                    else
                        values[i] = props[i].GetValue(item, null);
                }

                table.Rows.Add(values);
            }


            return table;
        }
        #endregion

        #region Lấy báo cáo giao hàng
        public static ApiResponse Get_BaoCaoGiaoHang<T>(SP_Parameter objParameter)
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                apiResponse = Utility.ExecuteStoredProc<T>(objParameter, API.Sp_Get_BaoCaoGiaoHang);
                return apiResponse;
            }
            catch (Exception ex)
            {
                //Utility.WriteLog(response != null ? response.RequestMessage.RequestUri.ToString() : "GetListData", MethodBase.GetCurrentMethod().Name, ex, "");
                return apiResponse = new ApiResponse
                {
                    Success = false,
                    Message = ex.Message,
                    Data = ""
                };
            }
        }

        public static ApiResponse Get_BaoCaoGiaoHangDetail<T>(Sp_Get_BaoCaoGiaoHang_Result model, string name = "Books")
        {
            ApiResponse apiResponse = new ApiResponse();
            HttpResponseMessage response = null;
            StringContent content = null;
            List<T> lstPage = new List<T>();
            string strcontent = "";
            try
            {
                strcontent = JsonConvert.SerializeObject(model);
                content = new StringContent(strcontent, Encoding.UTF8, "application/json");
                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + Token);
                response = client.PostAsync(URL + name, content).Result;

                if (response.IsSuccessStatusCode)
                {
                    var dataObjects = response.Content.ReadAsStringAsync().Result;
                    apiResponse = JsonConvert.DeserializeObject<ApiResponse>(dataObjects);
                    if (apiResponse.Data != null)
                        lstPage = JsonConvert.DeserializeObject<List<T>>(apiResponse.Data.ToString());
                    apiResponse.Data = lstPage;
                }
                else
                {
                    apiResponse.Message = GetErrorServer(response);
                }
            }
            catch (Exception ex)
            {
                Utility.WriteLog(response != null ? response.RequestMessage.RequestUri.ToString() : "GetDebtDetail", MethodBase.GetCurrentMethod().Name, ex, strcontent);
                apiResponse = new ApiResponse
                {
                    Success = false,
                    Message = ex.Message,
                    Data = ""
                };
            }
            return apiResponse;
        }
        #endregion

        #region Lấy danh sách chấm công
        public static ApiResponse Get_DanhSachChamCong<T>(DateTime? TUNGAY, DateTime? DENNAY, DateTime? NGAYCONG, string SearchString = "", string ID_NHANVIEN = "")
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                SP_Parameter objParameter = new SP_Parameter();
                objParameter.LOC_ID = Utility.LOC_ID;
                objParameter.TUNGAY = TUNGAY;
                objParameter.DENNGAY = DENNAY;
                objParameter.KEY = SearchString;
                objParameter.NGAYCONG = NGAYCONG;
                if(NGAYCONG != null)
                    objParameter.ISTHEOTHOIGIAN = false;
                objParameter.ID_NHANVIEN = ID_NHANVIEN;
                
                apiResponse = Utility.ExecuteStoredProc<T>(objParameter, API.Sp_Get_DanhSachChamCong);
                return apiResponse;
            }
            catch (Exception ex)
            {
                //Utility.WriteLog(response != null ? response.RequestMessage.RequestUri.ToString() : "GetListData", MethodBase.GetCurrentMethod().Name, ex, "");
                return apiResponse = new ApiResponse
                {
                    Success = false,
                    Message = ex.Message,
                    Data = ""
                };
            }
        }
        #endregion

        #region Lấy danh sách nghỉ phép
        public static ApiResponse Get_DanhSachNghiPhep<T>(DateTime? TUNGAY, DateTime? DENNAY, DateTime? NGAYCONG, string SearchString = "", string ID_NHANVIEN = "")
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                SP_Parameter objParameter = new SP_Parameter();
                objParameter.LOC_ID = Utility.LOC_ID;
                objParameter.TUNGAY = TUNGAY;
                objParameter.DENNGAY = DENNAY;
                objParameter.KEY = SearchString;
                objParameter.NGAYCONG = NGAYCONG;
                if (NGAYCONG != null)
                    objParameter.ISTHEOTHOIGIAN = false;
                objParameter.ID_NHANVIEN = ID_NHANVIEN;

                apiResponse = Utility.ExecuteStoredProc<T>(objParameter, API.Sp_Get_DanhSachNghiPhep);
                return apiResponse;
            }
            catch (Exception ex)
            {
                //Utility.WriteLog(response != null ? response.RequestMessage.RequestUri.ToString() : "GetListData", MethodBase.GetCurrentMethod().Name, ex, "");
                return apiResponse = new ApiResponse
                {
                    Success = false,
                    Message = ex.Message,
                    Data = ""
                };
            }
        }
        #endregion
        public static string GetDayOfWeekInVietnamese(DateTime date)
        {
            switch (date.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    return "T2";
                case DayOfWeek.Tuesday:
                    return "T3";
                case DayOfWeek.Wednesday:
                    return "T4";
                case DayOfWeek.Thursday:
                    return "T5";
                case DayOfWeek.Friday:
                    return "T6";
                case DayOfWeek.Saturday:
                    return "T7";
                case DayOfWeek.Sunday:
                    return "CN";
                default:
                    return "Không xác định"; // Trường hợp không xác định
            }
        }
    }
}