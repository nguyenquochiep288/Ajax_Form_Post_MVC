using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using CrystalDecisions.CrystalReports.Engine;
using DatabaseTHP;
using DatabaseTHP.Class;
using DatabaseTHP.Class.Misa;
using DatabaseTHP.StoredProcedure;
using DatabaseTHP.StoredProcedure.Parameter;
using MVC_QuanLyTHP.Class;
using MVC_QuanLyTHP.Controllers;
using MVC_QuanLyTHP.Models;
using Newtonsoft.Json;

namespace MVC_QuanLyTHP.Class
{

	public class Utility
	{
		public static string VersionJs = "?v=8.0.1";

		private static ReportClass report;

		private static double latitude;

		private static double longitude;

		public static string ThemTab = "Thêm thẻ mới";

		public static string ThucHien = "Thực hiện";

		public static string XemBaoCaoTrenThietBiDiDong = "Xem báo cáo";

		public static string QuayLai = "Quay lại";

		public static string Dong = "Đóng";

		public static string Them = "Thêm";

		public static string Xoa = "";

		public static string Sua = "";

		public static string TimKiem = "Tìm kiếm...";

		public static string CapNhat = "Cập nhật";

		public static string In = "";

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

		public static string HoaDon = "Xuất hóa đơn";

		public static string DonDatHang = "Đơn đặt hàng";

		public static string LayKetQua = "Cập nhật kết quả hóa đơn";

		public static string LayDuLieu = "Lấy dữ liệu từ Uniben";

		public static string DaCapMa = "Đã cấp mã";

		public static string LinkTraCuư = "https://meinvoice.vn/tra-cuu/?sc=";

		public static string ChuyenKPI = "Sao chép dữ liệu -> T" + CurrentTime.Month.ToString("00") + "/" + CurrentTime.Year;

		public static string ThongTinXuatHoaDon = "THÔNG TIN XUẤT HÓA ĐƠN";

		public static string XoaHoaDon = "Xóa hóa đơn";

		public static string menu = "";

		public static string LOC_ID = "02";

		public static string OrderBy = "ASC";

		public static string TypeSeacrh = "Contains";

		private static List<Tuple<string, string, bool, int>> ListSearch;

		public static int[] arrShow = new int[5] { 50, 100, 200, 500, 1000 };

		public static string URL = clsMaHoa.Decrypt(ConfigurationManager.ConnectionStrings["httpServer"].ConnectionString, "tmt6364");

		public static string UrlWebsite = clsMaHoa.Decrypt(ConfigurationManager.ConnectionStrings["UrlWebsite"].ConnectionString, "tmt6364");

		private static int intWidth;

		public static string stypeWidth_Level1;

		public static string stypeWidth_Level2;

		public static string stypeWidth_Level3;

		public static int PageSizeDefaut = arrShow[0];

		private static DateTime expires;

		private static string token;

		private static List<v_dm_HangHoa_Combo> lstProductCombo;

		private static List<v_dm_ChuongTrinhKhuyenMai_YeuCau> lstCTKM_YeuCau;

		private static List<v_dm_BangLuong_ChiTiet> lstdm_BangLuong_ChiTiet;

		private static List<nv_BangLuong_ChiTiet> lstnv_BangLuong_ChiTiet;

		private static List<dm_HangHoa_KhungGia> lstProductPriceRange;

		private static List<v_dm_HangHoa_KhungGia_HangHoa> lstProductPriceRangeHangHoa;

		private static List<v_dm_KPI_KinhDoanh_YeuCau> lstKPISale_YeuCau;

		private static List<v_dm_KPI_KinhDoanh_NhanVien> lstKPISale_NhanVien;

		private static List<v_dm_ChuongTrinhKhuyenMai_Tang> lstCTKM_Tang;

		private static List<Product_Detail> lstProductInput;

		private static List<Product_Detail> lstProductInvoiced;

		private static List<v_ct_PhieuGiaoHang_ChiTiet> lstPhieuGiaoHang_ChiTiet;

		private static List<v_ct_PhieuGiaoHang_NhanVienGiao> lstPhieuGiaoHang_NhanVienGiao;

		private static int SoNguyenTongCong = 0;

		private static int TongCongKhac = 2;

		private static string[] ChuSo = new string[10] { " không", " một", " hai", " ba", " bốn", " năm", " sáu", " bẩy", " tám", " chín" };

		private static string[] Tien = new string[6] { "", " nghìn", " triệu", " tỷ", " nghìn tỷ", " triệu tỷ" };

		public static ReportClass Report
		{
			get
			{
				return GetReport();
			}
			set
			{
				report = value;
			}
		}

		public static double Latitude
		{
			get
			{
				return GetLatitude();
			}
			set
			{
				latitude = value;
			}
		}

		public static double Longitude
		{
			get
			{
				return GetLongitude();
			}
			set
			{
				longitude = value;
			}
		}

		public static string Menu
		{
			get
			{
				return GetMenuText();
			}
			set
			{
				menu = value;
			}
		}

		public static List<Tuple<string, string, bool, int>> listSearch
		{
			get
			{
				return GetlistSearch();
			}
			set
			{
				ListSearch = value;
			}
		}

		public static DateTime CurrentTime => DateTime.Now;

		public static int IntWidth
		{
			get
			{
				return GetIntWidth();
			}
			set
			{
				intWidth = value;
			}
		}

		public static string StypeWidth_Level1
		{
			get
			{
				return GetStyleWidth();
			}
			set
			{
				stypeWidth_Level1 = value;
			}
		}

		public static string StypeWidth_Level2
		{
			get
			{
				return GetStypeWidth_Level2();
			}
			set
			{
				stypeWidth_Level2 = value;
			}
		}

		public static string StypeWidth_Level3
		{
			get
			{
				return GetStypeWidth_Level3();
			}
			set
			{
				stypeWidth_Level3 = value;
			}
		}

		public static DateTime Expires
		{
			get
			{
				return GetExpires();
			}
			set
			{
				expires = value;
			}
		}

		public static string Token
		{
			get
			{
				return GetToken();
			}
			set
			{
				token = value;
			}
		}

		public static List<v_dm_HangHoa_Combo> LstProductCombo
		{
			get
			{
				return GetlstProductCombo();
			}
			set
			{
				lstProductCombo = value;
			}
		}

		public static List<v_dm_ChuongTrinhKhuyenMai_YeuCau> LstCTKM_YeuCau
		{
			get
			{
				return GetlstCTKM_YeuCau();
			}
			set
			{
				lstCTKM_YeuCau = value;
			}
		}

		public static List<v_dm_BangLuong_ChiTiet> Lstdm_BangLuong_ChiTiet
		{
			get
			{
				return Getlstdm_BangLuong_ChiTiet();
			}
			set
			{
				lstdm_BangLuong_ChiTiet = value;
			}
		}

		public static List<nv_BangLuong_ChiTiet> Lstnv_BangLuong_ChiTiet
		{
			get
			{
				return Getlstnv_BangLuong_ChiTiet();
			}
			set
			{
				lstnv_BangLuong_ChiTiet = value;
			}
		}

		public static List<dm_HangHoa_KhungGia> LstProductPriceRange
		{
			get
			{
				return GetlstProductPriceRange();
			}
			set
			{
				lstProductPriceRange = value;
			}
		}

		public static List<v_dm_HangHoa_KhungGia_HangHoa> LstProductPriceRangeHangHoa
		{
			get
			{
				return GetlstProductPriceRangeHangHoa();
			}
			set
			{
				lstProductPriceRangeHangHoa = value;
			}
		}

		public static List<v_dm_KPI_KinhDoanh_YeuCau> LstKPISale_YeuCau
		{
			get
			{
				return GetlstKPISale_YeuCau();
			}
			set
			{
				lstKPISale_YeuCau = value;
			}
		}

		public static List<v_dm_KPI_KinhDoanh_NhanVien> LstKPISale_NhanVien
		{
			get
			{
				return GetlstKPISale_NhanVien();
			}
			set
			{
				lstKPISale_NhanVien = value;
			}
		}

		public static List<v_dm_ChuongTrinhKhuyenMai_Tang> LstCTKM_Tang
		{
			get
			{
				return GetlstCTKM_Tang();
			}
			set
			{
				lstCTKM_Tang = value;
			}
		}

		public static List<Product_Detail> LstProductInput
		{
			get
			{
				return GetlstProductInput();
			}
			set
			{
				lstProductInput = value;
			}
		}

		public static List<Product_Detail> LstProductInvoiced
		{
			get
			{
				return GetlstProductInvoiced();
			}
			set
			{
				lstProductInvoiced = value;
			}
		}

		public static List<v_ct_PhieuGiaoHang_ChiTiet> LstPhieuGiaoHang_ChiTiet
		{
			get
			{
				return GetPhieuGiaoHang_ChiTiet();
			}
			set
			{
				lstPhieuGiaoHang_ChiTiet = value;
			}
		}

		public static List<v_ct_PhieuGiaoHang_NhanVienGiao> LstPhieuGiaoHang_NhanVienGiao
		{
			get
			{
				return GetPhieuGiaoHang_NhanVienGiao();
			}
			set
			{
				lstPhieuGiaoHang_NhanVienGiao = value;
			}
		}

		public static ReportClass GetReport()
		{
			try
			{
				if (HttpContext.Current.Session["Report"] != null && !string.IsNullOrEmpty(HttpContext.Current.Session["Report"].ToString()))
				{
					report = (ReportClass)HttpContext.Current.Session["Report"];
				}
			}
			catch (Exception e)
			{
				WriteLog("GetReport", MethodBase.GetCurrentMethod().Name, e);
			}
			return report;
		}

		public static double GetLatitude()
		{
			try
			{
				if (HttpContext.Current.Session["Latitude"] != null && !string.IsNullOrEmpty(HttpContext.Current.Session["Latitude"].ToString()))
				{
					latitude = (double)HttpContext.Current.Session["Latitude"];
				}
			}
			catch (Exception e)
			{
				WriteLog("GetLatitude", MethodBase.GetCurrentMethod().Name, e);
			}
			return latitude;
		}

		public static double GetLongitude()
		{
			try
			{
				if (HttpContext.Current.Session["Longitude"] != null && !string.IsNullOrEmpty(HttpContext.Current.Session["Longitude"].ToString()))
				{
					longitude = (double)HttpContext.Current.Session["Longitude"];
				}
			}
			catch (Exception e)
			{
				WriteLog("GetLongitude", MethodBase.GetCurrentMethod().Name, e);
			}
			return longitude;
		}

		public static string GetTitleChon(string classname)
		{
			return "--- Chọn " + GetTitleFrom(classname).ToLower() + " ---";
		}

		public static string GetMenuText()
		{
			menu = "";
			try
			{
				if (HttpContext.Current.Session["Menu"] != null && !string.IsNullOrEmpty(HttpContext.Current.Session["Menu"].ToString()))
				{
					menu = HttpContext.Current.Session["Menu"].ToString();
				}
			}
			catch (Exception e)
			{
				WriteLog("GetMenuText", MethodBase.GetCurrentMethod().Name, e);
			}
			return menu;
		}

		public static List<Tuple<string, string, bool, int>> GetlistSearch()
		{
			ListSearch = new List<Tuple<string, string, bool, int>>();
			try
			{
				if (HttpContext.Current.Session["listSearch"] != null && !string.IsNullOrEmpty(HttpContext.Current.Session["listSearch"].ToString()))
				{
					ListSearch = (List<Tuple<string, string, bool, int>>)HttpContext.Current.Session["listSearch"];
				}
			}
			catch (Exception e)
			{
				WriteLog("GetlistSearch", MethodBase.GetCurrentMethod().Name, e);
			}
			return ListSearch;
		}

		public static int GetIntWidth()
		{
			intWidth = 1;
			try
			{
				if (HttpContext.Current.Session["IntWidth"] != null && !string.IsNullOrEmpty(HttpContext.Current.Session["IntWidth"].ToString()))
				{
					intWidth = Convert.ToInt32(HttpContext.Current.Session["IntWidth"]);
				}
			}
			catch (Exception e)
			{
				intWidth = PageSizeDefaut;
				WriteLog("GetIntWidth", MethodBase.GetCurrentMethod().Name, e);
			}
			return intWidth;
		}

		public static string GetStyleWidth()
		{
			stypeWidth_Level1 = "style='width: 90%; margin-left: 5%;'";
			try
			{
				if (HttpContext.Current.Session["StypeWidth_Level1"] != null && !string.IsNullOrEmpty(HttpContext.Current.Session["StypeWidth_Level1"].ToString()))
				{
					stypeWidth_Level1 = HttpContext.Current.Session["StypeWidth_Level1"].ToString();
				}
			}
			catch (Exception e)
			{
				WriteLog("GetStyleWidth", MethodBase.GetCurrentMethod().Name, e);
			}
			return stypeWidth_Level1;
		}

		public static string GetStypeWidth_Level2()
		{
			stypeWidth_Level2 = "style='width: 84%; margin-left: 10%;'";
			try
			{
				if (HttpContext.Current.Session["StypeWidth_Level2"] != null && !string.IsNullOrEmpty(HttpContext.Current.Session["StypeWidth_Level2"].ToString()))
				{
					stypeWidth_Level2 = HttpContext.Current.Session["StypeWidth_Level2"].ToString();
				}
			}
			catch (Exception e)
			{
				WriteLog("GetStypeWidth_Level2", MethodBase.GetCurrentMethod().Name, e);
			}
			return stypeWidth_Level2;
		}

		public static string GetStypeWidth_Level3()
		{
			stypeWidth_Level3 = "style='width: 78%; margin-left: 11%;'";
			try
			{
				if (HttpContext.Current.Session["StypeWidth_Level2"] != null && !string.IsNullOrEmpty(HttpContext.Current.Session["StypeWidth_Level2"].ToString()))
				{
					stypeWidth_Level3 = HttpContext.Current.Session["StypeWidth_Level2"].ToString();
				}
			}
			catch (Exception e)
			{
				WriteLog("GetStypeWidth_Level3", MethodBase.GetCurrentMethod().Name, e);
			}
			return stypeWidth_Level3;
		}

		public static List<view_web_NoteClass> GetNoteClass(bool bolCache = false)
		{
			List<view_web_NoteClass> list = new List<view_web_NoteClass>();
			list = (List<view_web_NoteClass>)((!bolCache && HttpContext.Current.Session["lstNoteClass"] != null) ? ((List<view_web_NoteClass>)HttpContext.Current.Session["lstNoteClass"]) : (HttpContext.Current.Session["lstNoteClass"] = GetNoteClasss<view_web_NoteClass>()));
			return list ?? new List<view_web_NoteClass>();
		}

		public static List<web_ThongBao> GetThongBao(bool bolCache = false)
		{
			List<web_ThongBao> list = new List<web_ThongBao>();
			list = (List<web_ThongBao>)((!bolCache && HttpContext.Current.Session["lstThongBao"] != null) ? ((List<web_ThongBao>)HttpContext.Current.Session["lstThongBao"]) : (HttpContext.Current.Session["lstThongBao"] = GetThongBao<web_ThongBao>()));
			return list ?? new List<web_ThongBao>();
		}

		public static List<v_web_Menu> GetMenu(bool bolCache = false)
		{
			List<v_web_Menu> list = new List<v_web_Menu>();
			if (!bolCache && HttpContext.Current.Session["lstMenu"] != null)
			{
				list = (List<v_web_Menu>)HttpContext.Current.Session["lstMenu"];
			}
			else
			{
				ApiResponse listData = GetListData<v_web_Menu>("Menu");
				if (!listData.Success)
				{
					return new List<v_web_Menu>();
				}
				list = (List<v_web_Menu>)(HttpContext.Current.Session["lstMenu"] = listData.Data as List<v_web_Menu>);
			}
			return list ?? new List<v_web_Menu>();
		}

		public static List<view_web_PhanQuyen> GetPhanQuyen(bool bolCache = false)
		{
			List<view_web_PhanQuyen> list = new List<view_web_PhanQuyen>();
			if (!bolCache && HttpContext.Current.Session["lstPhanQuyen"] != null)
			{
				list = (List<view_web_PhanQuyen>)HttpContext.Current.Session["lstPhanQuyen"];
			}
			else
			{
				List<view_web_PhanQuyen> phanQuyen = GetPhanQuyen<view_web_PhanQuyen>();
				list = (List<view_web_PhanQuyen>)(HttpContext.Current.Session["lstPhanQuyen"] = phanQuyen);
			}
			return list ?? new List<view_web_PhanQuyen>();
		}

		public static List<T> GetPhanQuyen<T>()
		{
			List<T> result = new List<T>();
			HttpResponseMessage httpResponseMessage = null;
			try
			{
				HttpClient httpClient = new HttpClient();
				httpResponseMessage = httpClient.GetAsync(URL + "Accounts/GetPhanQuyen/" + LOC_ID + "/" + HttpContext.Current.Session["idNhomQuyen"].ToString()).Result;
				if (httpResponseMessage.IsSuccessStatusCode)
				{
					string result2 = httpResponseMessage.Content.ReadAsStringAsync().Result;
					ApiResponse apiResponse = JsonConvert.DeserializeObject<ApiResponse>(result2);
					result = JsonConvert.DeserializeObject<List<T>>(apiResponse.Data.ToString());
				}
			}
			catch (Exception e)
			{
				WriteLog((httpResponseMessage != null) ? httpResponseMessage.RequestMessage.RequestUri.ToString() : "GetPhanQuyen", MethodBase.GetCurrentMethod().Name, e);
				result = new List<T>();
			}
			return result;
		}

		public static string GetColumnName(string classname, string columnname)
		{
			view_web_NoteClass view_web_NoteClass2 = null;
			List<view_web_NoteClass> noteClass = GetNoteClass();
			if (noteClass != null)
			{
				view_web_NoteClass2 = noteClass.Where((view_web_NoteClass s) => !string.IsNullOrEmpty(s.NAMECOLUMN) && !string.IsNullOrEmpty(s.CONTROLLER) && s.CONTROLLER.ToLower() == classname.ToLower() && s.NAMECOLUMN.ToLower() == columnname.ToLower()).FirstOrDefault();
			}
			if (view_web_NoteClass2 != null)
			{
				return (!string.IsNullOrEmpty(view_web_NoteClass2.DISPLAYNAME)) ? view_web_NoteClass2.DISPLAYNAME : columnname;
			}
			return columnname;
		}

		public static string GetTitleFrom(string classname)
		{
			view_web_NoteClass view_web_NoteClass2 = null;
			List<view_web_NoteClass> noteClass = GetNoteClass();
			if (noteClass != null)
			{
				view_web_NoteClass2 = noteClass.Where((view_web_NoteClass s) => !string.IsNullOrEmpty(s.NAMECOLUMN) && !string.IsNullOrEmpty(s.CONTROLLER) && s.CONTROLLER.ToLower() == classname.ToLower()).FirstOrDefault();
			}
			if (view_web_NoteClass2 != null)
			{
				return (!string.IsNullOrEmpty(view_web_NoteClass2.NAMEHEADER)) ? view_web_NoteClass2.NAMEHEADER : classname;
			}
			return classname;
		}

		public static List<T> GetNoteClasss<T>()
		{
			List<T> result = new List<T>();
			try
			{
				HttpClient httpClient = new HttpClient();
				HttpResponseMessage result2 = httpClient.GetAsync(URL + "Accounts/GetNoteClass").Result;
				if (result2.IsSuccessStatusCode)
				{
					string result3 = result2.Content.ReadAsStringAsync().Result;
					ApiResponse apiResponse = JsonConvert.DeserializeObject<ApiResponse>(result3);
					result = JsonConvert.DeserializeObject<List<T>>(apiResponse.Data.ToString());
				}
			}
			catch (Exception e)
			{
				WriteLog("Utility", MethodBase.GetCurrentMethod().Name, e);
				result = new List<T>();
			}
			return result;
		}

		public static List<T> GetThongBao<T>()
		{
			List<T> result = new List<T>();
			try
			{
				HttpClient httpClient = new HttpClient();
				HttpResponseMessage result2 = httpClient.GetAsync(URL + "ThongBao").Result;
				if (result2.IsSuccessStatusCode)
				{
					string result3 = result2.Content.ReadAsStringAsync().Result;
					ApiResponse apiResponse = JsonConvert.DeserializeObject<ApiResponse>(result3);
					result = JsonConvert.DeserializeObject<List<T>>(apiResponse.Data.ToString());
				}
			}
			catch (Exception e)
			{
				WriteLog("Utility", MethodBase.GetCurrentMethod().Name, e);
				result = new List<T>();
			}
			return result;
		}

		public static int GetPageSize()
		{
			int result = PageSizeDefaut;
			try
			{
				if (HttpContext.Current.Session["PageSize"] != null && !string.IsNullOrEmpty(HttpContext.Current.Session["PageSize"].ToString()))
				{
					result = Convert.ToInt32(HttpContext.Current.Session["PageSize"]);
				}
			}
			catch (Exception e)
			{
				WriteLog("Utility", MethodBase.GetCurrentMethod().Name, e);
				result = PageSizeDefaut;
			}
			return result;
		}

		private static DateTime GetExpires()
		{
			DateTime result = default(DateTime);
			try
			{
				if (HttpContext.Current.Session["Expires"] != null && !string.IsNullOrEmpty(HttpContext.Current.Session["Expires"].ToString()))
				{
					result = Convert.ToDateTime(HttpContext.Current.Session["Expires"]);
					return result;
				}
			}
			catch (Exception e)
			{
				WriteLog("Utility", MethodBase.GetCurrentMethod().Name, e);
			}
			return result;
		}

		private static string GetToken(bool bolCache = false)
		{
			string result = string.Empty;
			try
			{
				if ((GetExpires() < CurrentTime || bolCache) && HttpContext.Current.Session["Login_Model"] != null)
				{
					Login_Model login_Model = (Login_Model)HttpContext.Current.Session["Login_Model"];
					ApiResponse apiResponse = Login(login_Model.user, login_Model.pass);
					if (apiResponse.Success)
					{
						SetSession(apiResponse, login_Model, null);
					}
				}
				if (HttpContext.Current.Session["Token"] != null && !string.IsNullOrEmpty(HttpContext.Current.Session["Token"].ToString()))
				{
					result = HttpContext.Current.Session["Token"].ToString();
				}
			}
			catch (Exception e)
			{
				WriteLog("Utility", MethodBase.GetCurrentMethod().Name, e);
			}
			return result;
		}

		public static ApiResponse Login(string username, string password)
		{
			ApiResponse apiResponse = new ApiResponse();
			HttpResponseMessage httpResponseMessage = null;
			try
			{
				SignInModel signInModel = new SignInModel();
				signInModel.UserName = username;
				signInModel.Password = clsMaHoa.Encrypt(password, "tmt6364");
				string content = JsonConvert.SerializeObject(signInModel);
				StringContent content2 = new StringContent(content, Encoding.UTF8, "application/json");
				HttpClient httpClient = new HttpClient();
				httpResponseMessage = httpClient.PostAsync(URL + "Accounts/Login", content2).Result;
				if (httpResponseMessage.IsSuccessStatusCode)
				{
					string result = httpResponseMessage.Content.ReadAsStringAsync().Result;
					apiResponse = JsonConvert.DeserializeObject<ApiResponse>(result);
				}
				else
				{
					apiResponse.Message = GetErrorServer(httpResponseMessage);
				}
			}
			catch (Exception ex)
			{
				WriteLog((httpResponseMessage != null) ? httpResponseMessage.RequestMessage.RequestUri.ToString() : "Login", MethodBase.GetCurrentMethod().Name, ex);
				apiResponse = new ApiResponse
				{
					Success = false,
					Message = ex.Message,
					Data = ""
				};
			}
			return apiResponse;
		}

		public static int GetMaxID<T>(T ovjTable, string LOC_ID = "", string NgayLap = "")
		{
			List<T> list = new List<T>();
			try
			{
				HttpClient httpClient = new HttpClient();
				httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + Token);
				HttpResponseMessage result = httpClient.GetAsync(URL + "GetIDMax/" + typeof(T).Name + "/" + LOC_ID + "/" + NgayLap.Replace("-", "")).Result;
				if (result.IsSuccessStatusCode)
				{
					string result2 = result.Content.ReadAsStringAsync().Result;
					ApiResponse apiResponse = JsonConvert.DeserializeObject<ApiResponse>(result2);
					int result3 = 0;
					if (int.TryParse(apiResponse.Data.ToString(), out result3))
					{
						return result3 + 1;
					}
					return 0;
				}
				return 0;
			}
			catch (Exception ex)
			{
				ApiResponse apiResponse2 = new ApiResponse
				{
					Success = false,
					Message = ex.Message,
					Data = ""
				};
				return 0;
			}
		}

		public static ApiResponse GetListDataCode<T>(string name = "Books", string ShowSearchValue = "", string SearchString = "", string LOC_ID = "")
		{
			ApiResponse apiResponse = new ApiResponse();
			List<T> data = new List<T>();
			HttpClient httpClient = new HttpClient();
			HttpResponseMessage httpResponseMessage = null;
			try
			{
				httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + Token);
				httpResponseMessage = httpClient.GetAsync(URL + name + "/" + LOC_ID + "/1/" + ShowSearchValue + "/" + SearchString.ToLower()).Result;
				if (httpResponseMessage.IsSuccessStatusCode)
				{
					string result = httpResponseMessage.Content.ReadAsStringAsync().Result;
					apiResponse = JsonConvert.DeserializeObject<ApiResponse>(result);
					if (apiResponse.Data != null && !string.IsNullOrEmpty(apiResponse.Data.ToString()))
					{
						data = JsonConvert.DeserializeObject<List<T>>(apiResponse.Data.ToString());
					}
					apiResponse.Data = data;
				}
				else
				{
					apiResponse.Message = GetErrorServer(httpResponseMessage);
				}
			}
			catch (Exception ex)
			{
				WriteLog((httpResponseMessage != null) ? httpResponseMessage.RequestMessage.RequestUri.ToString() : "GetListData", MethodBase.GetCurrentMethod().Name, ex);
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
			List<T> list = new List<T>();
			HttpClient httpClient = new HttpClient();
			HttpResponseMessage httpResponseMessage = null;
			try
			{
				string ShowSearchValue_Decrypt = clsMaHoa.Decrypt(ShowSearchValue, "tmt6364");
				string text = string.Empty;
				List<view_web_NoteClass> list2 = GetNoteClass();
				if (list2 != null)
				{
					list2 = list2.Where((view_web_NoteClass s) => !string.IsNullOrEmpty(s.NAMECLASS) && s.NAMECLASS.ToLower() == typeof(T).Name.Replace("v_view_", "").Replace("v_", "").ToLower() && s.ISSEARCH).ToList();
				}
				if (!string.IsNullOrEmpty(ShowSearchValue_Decrypt) && !string.IsNullOrEmpty(SearchString))
				{
					if (string.IsNullOrEmpty(TypeSearch))
					{
						PropertyInfo[] properties = typeof(T).GetProperties();
						if (ShowSearchValue_Decrypt != "ALL")
						{
							PropertyInfo propertyInfo = null;
							view_web_NoteClass valu = list2.Where((view_web_NoteClass e) => e.NAMECOLUMN.ToLower() == ShowSearchValue_Decrypt.ToLower()).FirstOrDefault();
							if (valu != null && !string.IsNullOrEmpty(valu.REPLACESEARCH))
							{
								propertyInfo = properties.Where((PropertyInfo s) => s.Name.ToUpper() == valu.REPLACESEARCH.ToUpper()).FirstOrDefault();
							}
							if (propertyInfo == null)
							{
								propertyInfo = properties.Where((PropertyInfo s) => s.Name.ToUpper() == ShowSearchValue_Decrypt.ToUpper()).FirstOrDefault();
							}
							else
							{
								ShowSearchValue_Decrypt = valu.REPLACESEARCH;
							}
							switch ((propertyInfo.PropertyType.GenericTypeArguments.Count() > 0) ? propertyInfo.PropertyType.GenericTypeArguments[0].Name.ToUpper() : propertyInfo.PropertyType.Name.ToUpper())
							{
								case "STRING":
									text = ShowSearchValue_Decrypt + ".ToLower()." + TypeSeacrh + "(@0)";
									break;
								case "BOOLEAN":
									{
										int result4 = 0;
										int.TryParse(SearchString, out result4);
										switch (result4)
										{
											case 0:
												SearchString = SearchString.Replace("0", "false");
												break;
											case 1:
												SearchString = SearchString.Replace("1", "true");
												break;
										}
										text = ShowSearchValue_Decrypt + ".ToString().ToLower().Contains(@0)";
										break;
									}
								case "INT32":
									{
										text = ShowSearchValue_Decrypt + " == @0";
										int result3 = 0;
										int.TryParse(SearchString, out result3);
										SearchString = result3.ToString();
										break;
									}
								case "DOUBLE":
									{
										double result2 = 0.0;
										double.TryParse(SearchString, out result2);
										SearchString = result2.ToString();
										break;
									}
								case "DATETIME":
									{
										DateTime result = CurrentTime;
										DateTime.TryParse(SearchString, out result);
										text = ShowSearchValue_Decrypt + " >= @0";
										SearchString = result.ToString("dd/MM/yyyy");
										break;
									}
								default:
									text = ShowSearchValue_Decrypt + ".ToString().ToLower()." + TypeSeacrh + "(@0)";
									break;
							}
						}
						else
						{
							if (list2 == null)
							{
								PropertyInfo[] array = properties;
								foreach (PropertyInfo propertyInfo2 in array)
								{
									string text2 = ((propertyInfo2.PropertyType.GenericTypeArguments.Count() > 0) ? propertyInfo2.PropertyType.GenericTypeArguments[0].Name.ToUpper() : propertyInfo2.PropertyType.Name.ToUpper());
									string text3 = text2;
									string text4 = text3;
									if (text4 == "STRING")
									{
										text = text + propertyInfo2.Name + ".ToLower()." + TypeSeacrh + "(@0) || ";
									}
								}
								ShowSearchValue_Decrypt = "";
							}
							else
							{
								foreach (view_web_NoteClass itm in list2)
								{
									PropertyInfo propertyInfo3 = null;
									view_web_NoteClass value1 = list2.Where((view_web_NoteClass e) => e.NAMECOLUMN.ToLower() == itm.NAMECOLUMN.ToLower()).FirstOrDefault();
									if (value1 != null && !string.IsNullOrEmpty(value1.REPLACESEARCH))
									{
										propertyInfo3 = properties.Where((PropertyInfo s) => s.Name.ToUpper() == value1.REPLACESEARCH.ToUpper()).FirstOrDefault();
									}
									if (propertyInfo3 == null)
									{
										propertyInfo3 = properties.Where((PropertyInfo s) => s.Name.ToUpper() == itm.NAMECOLUMN.ToUpper()).FirstOrDefault();
									}
									if (propertyInfo3 != null)
									{
										string text5 = ((propertyInfo3.PropertyType.GenericTypeArguments.Count() > 0) ? propertyInfo3.PropertyType.GenericTypeArguments[0].Name.ToUpper() : propertyInfo3.PropertyType.Name.ToUpper());
										string text6 = text5;
										string text7 = text6;
										if (text7 == "STRING")
										{
											text = text + propertyInfo3.Name + ".ToLower()." + TypeSeacrh + "(@0) || ";
										}
									}
								}
								view_web_NoteClass view_web_NoteClass2 = list2.Where((view_web_NoteClass e) => e.ISSORT).FirstOrDefault();
								ShowSearchValue_Decrypt = ((view_web_NoteClass2 != null) ? view_web_NoteClass2.NAMECOLUMN : "");
							}
							if (!string.IsNullOrEmpty(text))
							{
								text = text.Substring(0, text.Length - 4);
							}
						}
					}
					else
					{
						text = ShowSearchValue_Decrypt + " == @0";
						SearchString = clsMaHoa.Decrypt(SearchString, "tmt6364");
					}
				}
				else if (ShowSearchValue_Decrypt.Contains("ALL"))
				{
					ShowSearchValue_Decrypt = "";
				}
				httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + Token);
				SearchString = SearchString.Replace("/", "%2f");
				if (name.Contains("Uniben"))
				{
					httpClient.Timeout = TimeSpan.FromSeconds(120.0);
				}
				httpResponseMessage = httpClient.GetAsync(URL + name + (string.IsNullOrEmpty(LOC_ID) ? "" : ("/" + LOC_ID)) + ((string.IsNullOrEmpty(SearchString) || string.IsNullOrEmpty(text)) ? "" : ("/1/" + text + "/" + SearchString.ToLower()))).Result;
				if (httpResponseMessage.IsSuccessStatusCode)
				{
					string result5 = httpResponseMessage.Content.ReadAsStringAsync().Result;
					apiResponse = JsonConvert.DeserializeObject<ApiResponse>(result5);
					if (apiResponse.Data != null && !string.IsNullOrEmpty(apiResponse.Data.ToString()))
					{
						list = JsonConvert.DeserializeObject<List<T>>(apiResponse.Data.ToString());
					}
					if (!string.IsNullOrEmpty(ShowSearchValue_Decrypt))
					{
						list = list.OrderBy(ShowSearchValue_Decrypt + " " + OrderBy).ToList();
					}
					apiResponse.Data = list;
				}
				else
				{
					apiResponse.Message = GetErrorServer(httpResponseMessage);
				}
			}
			catch (Exception ex)
			{
				WriteLog((httpResponseMessage != null) ? httpResponseMessage.RequestMessage.RequestUri.ToString() : "GetListData", MethodBase.GetCurrentMethod().Name, ex);
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
			HttpResponseMessage httpResponseMessage = null;
			try
			{
				HttpClient httpClient = new HttpClient();
				httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + Token);
				httpResponseMessage = httpClient.GetAsync(URL + name + "/" + GetValue).Result;
				if (httpResponseMessage.IsSuccessStatusCode)
				{
					string result = httpResponseMessage.Content.ReadAsStringAsync().Result;
					apiResponse = JsonConvert.DeserializeObject<ApiResponse>(result);
					if (apiResponse.Data != null)
					{
						apiResponse.Data = JsonConvert.DeserializeObject<T>(apiResponse.Data.ToString());
					}
				}
				else
				{
					apiResponse.Message = GetErrorServer(httpResponseMessage);
				}
			}
			catch (Exception ex)
			{
				WriteLog((httpResponseMessage != null) ? httpResponseMessage.RequestMessage.RequestUri.ToString() : "GetDetail", MethodBase.GetCurrentMethod().Name, ex);
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
			T val = default(T);
			if (model != null)
			{
				val = JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(model));
			}
			ApiResponse apiResponse = new ApiResponse();
			StringContent content = null;
			string text = "";
			HttpResponseMessage httpResponseMessage = null;
			try
			{
				if (model != null)
				{
					text = JsonConvert.SerializeObject(val);
					content = new StringContent(text, Encoding.UTF8, "application/json");
				}
				HttpClient httpClient = new HttpClient();
				httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + Token);
				httpResponseMessage = httpClient.PutAsync(URL + name + "/" + GetValue, content).Result;
				if (httpResponseMessage.IsSuccessStatusCode)
				{
					string result = httpResponseMessage.Content.ReadAsStringAsync().Result;
					apiResponse = JsonConvert.DeserializeObject<ApiResponse>(result);
				}
				else
				{
					apiResponse.Message = GetErrorServer(httpResponseMessage);
				}
			}
			catch (Exception ex)
			{
				WriteLog((httpResponseMessage != null) ? httpResponseMessage.RequestMessage.RequestUri.ToString() : "Edit", MethodBase.GetCurrentMethod().Name, ex, text);
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
			T val = default(T);
			val = JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(model));
			ApiResponse apiResponse = new ApiResponse();
			StringContent stringContent = null;
			HttpResponseMessage httpResponseMessage = null;
			string text = "";
			try
			{
				text = JsonConvert.SerializeObject(val);
				stringContent = new StringContent(text, Encoding.UTF8, "application/json");
				HttpClient httpClient = new HttpClient();
				httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + Token);
				httpResponseMessage = httpClient.PostAsync(URL + name, stringContent).Result;
				if (httpResponseMessage.IsSuccessStatusCode)
				{
					string result = httpResponseMessage.Content.ReadAsStringAsync().Result;
					apiResponse = JsonConvert.DeserializeObject<ApiResponse>(result);
				}
				else
				{
					apiResponse.Message = GetErrorServer(httpResponseMessage);
					string result2 = httpResponseMessage.Content.ReadAsStringAsync().Result;
					apiResponse.Message += result2;
				}
			}
			catch (Exception ex)
			{
				WriteLog((httpResponseMessage != null) ? httpResponseMessage.RequestMessage.RequestUri.ToString() : "Create", MethodBase.GetCurrentMethod().Name, ex, text);
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
			HttpResponseMessage httpResponseMessage = null;
			try
			{
				HttpClient httpClient = new HttpClient();
				httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + Token);
				httpResponseMessage = httpClient.DeleteAsync(URL + name + "/" + GetValue).Result;
				if (httpResponseMessage.IsSuccessStatusCode)
				{
					string result = httpResponseMessage.Content.ReadAsStringAsync().Result;
					apiResponse = JsonConvert.DeserializeObject<ApiResponse>(result);
				}
				else
				{
					apiResponse.Message = GetErrorServer(httpResponseMessage);
				}
			}
			catch (Exception ex)
			{
				WriteLog((httpResponseMessage != null) ? httpResponseMessage.RequestMessage.RequestUri.ToString() : "Delete", MethodBase.GetCurrentMethod().Name, ex);
				apiResponse = new ApiResponse
				{
					Success = false,
					Message = ex.Message,
					Data = ""
				};
			}
			return apiResponse;
		}

		public static ApiResponse Delete<T>(string GetValue, string name = "Books", T model = null) where T : class
		{
			ApiResponse apiResponse = new ApiResponse();
			HttpResponseMessage httpResponseMessage = null;
			try
			{
				StringContent content = CreateContent(model);
				HttpClient httpClient = new HttpClient();
				httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + Token);
				HttpRequestMessage request = new HttpRequestMessage
				{
					Method = HttpMethod.Delete,
					RequestUri = new Uri(URL + name + "/" + GetValue),
					Content = content
				};
				httpResponseMessage = httpClient.SendAsync(request).Result;
				if (httpResponseMessage.IsSuccessStatusCode)
				{
					string result = httpResponseMessage.Content.ReadAsStringAsync().Result;
					apiResponse = JsonConvert.DeserializeObject<ApiResponse>(result);
				}
				else
				{
					apiResponse.Message = GetErrorServer(httpResponseMessage);
				}
			}
			catch (Exception ex)
			{
				WriteLog(httpResponseMessage?.RequestMessage?.RequestUri?.ToString() ?? "Delete", MethodBase.GetCurrentMethod().Name, ex);
				apiResponse = new ApiResponse
				{
					Success = false,
					Message = ex.Message,
					Data = ""
				};
			}
			return apiResponse;
		}

		private static StringContent CreateContent<T>(T model) where T : class
		{
			if (model == null)
			{
				return null;
			}
			string content = JsonConvert.SerializeObject(model);
			return new StringContent(content, Encoding.UTF8, "application/json");
		}

		public static ApiResponse GetListDataOrder<T>(string name, DateTime? FromDate, DateTime? ToDate, string ShowSearchValue = "", string SearchString = "", string LOC_ID = "", string TypeSearch = "")
		{
			ApiResponse apiResponse = new ApiResponse();
			List<T> list = new List<T>();
			try
			{
				string ShowSearchValue_Decrypt = clsMaHoa.Decrypt(ShowSearchValue, "tmt6364");
				string text = string.Empty;
				if (!string.IsNullOrEmpty(ShowSearchValue_Decrypt) && !string.IsNullOrEmpty(SearchString))
				{
					if (string.IsNullOrEmpty(TypeSearch))
					{
						PropertyInfo[] properties = typeof(T).GetProperties();
						PropertyInfo propertyInfo = properties.Where((PropertyInfo s) => s.Name.ToUpper() == ShowSearchValue_Decrypt.ToUpper()).FirstOrDefault();
						switch ((propertyInfo.PropertyType.GenericTypeArguments.Count() > 0) ? propertyInfo.PropertyType.GenericTypeArguments[0].Name.ToUpper() : propertyInfo.PropertyType.Name.ToUpper())
						{
							case "STRING":
								text = ShowSearchValue_Decrypt + ".ToLower()." + TypeSeacrh + "(@0)";
								break;
							case "BOOLEAN":
								{
									int result4 = 0;
									int.TryParse(SearchString, out result4);
									switch (result4)
									{
										case 0:
											SearchString = SearchString.Replace("0", "false");
											break;
										case 1:
											SearchString = SearchString.Replace("1", "true");
											break;
									}
									text = ShowSearchValue_Decrypt + ".ToString().ToLower().Contains(@0)";
									break;
								}
							case "INT32":
								{
									text = ShowSearchValue_Decrypt + " == @0";
									int result3 = 0;
									int.TryParse(SearchString, out result3);
									SearchString = result3.ToString();
									break;
								}
							case "DOUBLE":
								{
									double result2 = 0.0;
									double.TryParse(SearchString, out result2);
									SearchString = result2.ToString();
									break;
								}
							case "DATETIME":
								{
									DateTime result = CurrentTime;
									DateTime.TryParse(SearchString, out result);
									text = ShowSearchValue_Decrypt + " >= @0";
									SearchString = result.ToString("dd/MM/yyyy");
									break;
								}
							default:
								text = ShowSearchValue_Decrypt + ".ToString().ToLower()." + TypeSeacrh + "(@0)";
								break;
						}
					}
					else
					{
						text = ShowSearchValue_Decrypt + " == @0";
						SearchString = clsMaHoa.Decrypt(SearchString, "tmt6364");
					}
				}
				HttpClient httpClient = new HttpClient();
				httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + Token);
				HttpResponseMessage result5 = httpClient.GetAsync(URL + name + "/" + LOC_ID + "/" + FromDate.Value.ToString("yyyy-MM-ddT10:00:00.000Z") + "/" + ToDate.Value.ToString("yyyy-MM-ddT10:00:00.000Z") + ((string.IsNullOrEmpty(SearchString) || string.IsNullOrEmpty(text)) ? "" : ("/1/" + text + "/" + SearchString.ToLower()))).Result;
				if (result5.IsSuccessStatusCode)
				{
					string result6 = result5.Content.ReadAsStringAsync().Result;
					apiResponse = JsonConvert.DeserializeObject<ApiResponse>(result6);
					if (apiResponse.Data != null && !string.IsNullOrEmpty(apiResponse.Data.ToString()))
					{
						list = JsonConvert.DeserializeObject<List<T>>(apiResponse.Data.ToString());
					}
					if (!string.IsNullOrEmpty(ShowSearchValue_Decrypt))
					{
						list = list.OrderBy(ShowSearchValue_Decrypt + " " + OrderBy).ToList();
					}
					apiResponse.Data = list;
				}
				else
				{
					apiResponse.Message = GetErrorServer(result5);
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
				HttpClient httpClient = new HttpClient();
				httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + Token);
				HttpResponseMessage result = httpClient.PostAsync(URL + name, content).Result;
				if (result.IsSuccessStatusCode)
				{
					string result2 = result.Content.ReadAsStringAsync().Result;
					apiResponse = JsonConvert.DeserializeObject<ApiResponse>(result2);
				}
				else
				{
					apiResponse.Message = GetErrorServer(result);
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
				HttpClient httpClient = new HttpClient();
				httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + Token);
				HttpResponseMessage result = httpClient.PostAsync(URL + name, content).Result;
				if (result.IsSuccessStatusCode)
				{
					string result2 = result.Content.ReadAsStringAsync().Result;
					apiResponse = JsonConvert.DeserializeObject<ApiResponse>(result2);
				}
				else
				{
					apiResponse.Message = GetErrorServer(result);
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
				HttpClient httpClient = new HttpClient();
				httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + Token);
				HttpResponseMessage result = httpClient.PutAsync(URL + name, content).Result;
				if (result.IsSuccessStatusCode)
				{
					string result2 = result.Content.ReadAsStringAsync().Result;
					apiResponse = JsonConvert.DeserializeObject<ApiResponse>(result2);
				}
				else
				{
					apiResponse.Message = GetErrorServer(result);
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

		public static ApiResponse ExecuteStoredProc<T>(SP_Parameter model, string name = "Books")
		{
			ApiResponse apiResponse = new ApiResponse();
			HttpResponseMessage httpResponseMessage = null;
			StringContent stringContent = null;
			List<T> data = new List<T>();
			string text = "";
			try
			{
				text = JsonConvert.SerializeObject(model);
				stringContent = new StringContent(text, Encoding.UTF8, "application/json");
				HttpClient httpClient = new HttpClient();
				httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + Token);
				httpResponseMessage = httpClient.PostAsync(URL + name, stringContent).Result;
				if (httpResponseMessage.IsSuccessStatusCode)
				{
					string result = httpResponseMessage.Content.ReadAsStringAsync().Result;
					apiResponse = JsonConvert.DeserializeObject<ApiResponse>(result);
					if (apiResponse.Data != null)
					{
						data = JsonConvert.DeserializeObject<List<T>>(apiResponse.Data.ToString());
					}
					apiResponse.Data = data;
				}
				else
				{
					apiResponse.Message = GetErrorServer(httpResponseMessage);
				}
			}
			catch (Exception ex)
			{
				WriteLog((httpResponseMessage != null) ? httpResponseMessage.RequestMessage.RequestUri.ToString() : "ExecuteStoredProc", MethodBase.GetCurrentMethod().Name, ex, text);
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
			HttpResponseMessage httpResponseMessage = null;
			StringContent stringContent = null;
			string text = "";
			try
			{
				text = JsonConvert.SerializeObject(model);
				stringContent = new StringContent(text, Encoding.UTF8, "application/json");
				HttpClient httpClient = new HttpClient();
				httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + Token);
				httpResponseMessage = httpClient.PostAsync(URL + name, stringContent).Result;
				if (httpResponseMessage.IsSuccessStatusCode)
				{
					string result = httpResponseMessage.Content.ReadAsStringAsync().Result;
					apiResponse = JsonConvert.DeserializeObject<ApiResponse>(result);
				}
				else
				{
					apiResponse.Message = GetErrorServer(httpResponseMessage);
				}
			}
			catch (Exception ex)
			{
				WriteLog((httpResponseMessage != null) ? httpResponseMessage.RequestMessage.RequestUri.ToString() : "ExecuteStoredProc", MethodBase.GetCurrentMethod().Name, ex, text);
				apiResponse = new ApiResponse
				{
					Success = false,
					Message = ex.Message,
					Data = ""
				};
			}
			return apiResponse;
		}

		public static ApiResponse ExecuteStoredProc<T>(SP_Parameter_Report model, string name = "Books")
		{
			ApiResponse apiResponse = new ApiResponse();
			HttpResponseMessage httpResponseMessage = null;
			StringContent stringContent = null;
			DataTable data = new DataTable();
			string text = "";
			try
			{
				text = JsonConvert.SerializeObject(model);
				stringContent = new StringContent(text, Encoding.UTF8, "application/json");
				HttpClient httpClient = new HttpClient();
				httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + Token);
				httpResponseMessage = httpClient.PostAsync(URL + name, stringContent).Result;
				if (httpResponseMessage.IsSuccessStatusCode)
				{
					string result = httpResponseMessage.Content.ReadAsStringAsync().Result;
					apiResponse = JsonConvert.DeserializeObject<ApiResponse>(result);
					if (apiResponse.Data != null)
					{
						data = JsonConvert.DeserializeObject<DataTable>(apiResponse.Data.ToString());
					}
					apiResponse.Data = data;
				}
				else
				{
					apiResponse.Message = GetErrorServer(httpResponseMessage);
				}
			}
			catch (Exception ex)
			{
				WriteLog((httpResponseMessage != null) ? httpResponseMessage.RequestMessage.RequestUri.ToString() : "ExecuteStoredProc", MethodBase.GetCurrentMethod().Name, ex, text);
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
			HttpResponseMessage httpResponseMessage = null;
			StringContent stringContent = null;
			List<T> data = new List<T>();
			string text = "";
			try
			{
				text = JsonConvert.SerializeObject(model);
				stringContent = new StringContent(text, Encoding.UTF8, "application/json");
				HttpClient httpClient = new HttpClient();
				httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + Token);
				httpResponseMessage = httpClient.PostAsync(URL + name, stringContent).Result;
				if (httpResponseMessage.IsSuccessStatusCode)
				{
					string result = httpResponseMessage.Content.ReadAsStringAsync().Result;
					apiResponse = JsonConvert.DeserializeObject<ApiResponse>(result);
					if (apiResponse.Data != null)
					{
						data = JsonConvert.DeserializeObject<List<T>>(apiResponse.Data.ToString());
					}
					apiResponse.Data = data;
				}
				else
				{
					apiResponse.Message = GetErrorServer(httpResponseMessage);
				}
			}
			catch (Exception ex)
			{
				WriteLog((httpResponseMessage != null) ? httpResponseMessage.RequestMessage.RequestUri.ToString() : "ExecuteStoredProc", MethodBase.GetCurrentMethod().Name, ex, text);
				apiResponse = new ApiResponse
				{
					Success = false,
					Message = ex.Message,
					Data = ""
				};
			}
			return apiResponse;
		}

		public static bool KiemTra(bool bolCach = false)
		{
			Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("vi-VN");
			if (string.IsNullOrEmpty((HttpContext.Current.Session["User"] == null) ? "" : HttpContext.Current.Session["User"].ToString()) || bolCach)
			{
				string user = string.Empty;
				string text = string.Empty;
				bool check = false;
				HttpCookie httpCookie = HttpContext.Current.Request.Cookies["THP"];
				if (httpCookie != null && httpCookie.Value != "")
				{
					string text2 = clsMaHoa.Decrypt(httpCookie.Values["Us"].ToString(), "tmt6364");
					string[] array = text2.Split(new string[1] { Environment.NewLine }, StringSplitOptions.None);
					if (array != null && array.Count() == 2)
					{
						user = array[0];
						text = array[1];
						check = true;
					}
				}
				if (!string.IsNullOrEmpty(text))
				{
					Login_Model login_Model = new Login_Model
					{
						user = user,
						pass = text,
						check = check
					};
					ApiResponse apiResponse = Login(login_Model.user, login_Model.pass);
					if (!apiResponse.Success)
					{
						HttpContext.Current.Session.Clear();
						return true;
					}
					SetSession(apiResponse, login_Model, httpCookie);
				}
				if (string.IsNullOrEmpty((HttpContext.Current.Session["User"] == null) ? "" : HttpContext.Current.Session["User"].ToString()))
				{
					return true;
				}
				return false;
			}
			return false;
		}

		public static bool KiemTraQuyenMoKhoa()
		{
			if (HttpContext.Current.Session["idNhomQuyen"] != null && HttpContext.Current.Session["idNhomQuyen"].ToString() == "-1")
			{
				return true;
			}
			return false;
		}

		public static void WriteLog(object sCls, string MethodName, Exception e, string data = "")
		{
			try
			{
				string hostName = Dns.GetHostName();
				string text = "";
				if (!string.IsNullOrEmpty(hostName))
				{
					text = Dns.GetHostByName(hostName).AddressList[0].ToString();
				}
				string iPAddress = GetIPAddress();
				string text2 = "";
				string fULLNAME = "";
				int num = 0;
				if (sCls != null)
				{
					text2 = ((!(sCls is string)) ? sCls.GetType().ToString() : sCls.ToString());
				}
				StackTrace stackTrace = new StackTrace(e, fNeedFileInfo: true);
				try
				{
					StackFrame frame = stackTrace.GetFrame(0);
					fULLNAME = ((frame.GetMethod().DeclaringType != null) ? frame.GetMethod().DeclaringType.FullName : "");
					num = frame.GetFileLineNumber();
				}
				catch
				{
				}
				LogError logError = new LogError();
				logError.LOC_ID = LOC_ID;
				logError.ID = Guid.NewGuid().ToString();
				logError.FULLNAME = fULLNAME;
				logError.METHODNAME = MethodName + " - " + text2;
				logError.DATA = data;
				logError.MESSAGE = e.Message + " - " + num;
				logError.ID_USER = ((HttpContext.Current.Session["idUser"] != null) ? HttpContext.Current.Session["idUser"].ToString() : "");
				logError.TIME = CurrentTime;
				logError.IP = hostName + "-" + text + "-" + iPAddress;
				ApiResponse apiResponse = Create(logError, "LogError");
			}
			catch
			{
			}
		}

		public static void WriteLog(object sCls, string MethodName, string data = "")
		{
			try
			{
				string hostName = Dns.GetHostName();
				string text = "";
				if (!string.IsNullOrEmpty(hostName))
				{
					text = Dns.GetHostByName(hostName).AddressList[0].ToString();
				}
				string iPAddress = GetIPAddress();
				string text2 = "";
				string fULLNAME = "";
				int num = 0;
				if (sCls != null)
				{
					text2 = ((!(sCls is string)) ? sCls.GetType().ToString() : sCls.ToString());
				}
				LogError logError = new LogError();
				logError.LOC_ID = LOC_ID;
				logError.ID = Guid.NewGuid().ToString();
				logError.FULLNAME = fULLNAME;
				logError.METHODNAME = MethodName + " - " + text2;
				logError.DATA = data;
				logError.MESSAGE = data;
				logError.ID_USER = ((HttpContext.Current.Session["idUser"] != null) ? HttpContext.Current.Session["idUser"].ToString() : "");
				logError.TIME = CurrentTime;
				logError.IP = hostName + "-" + text + "-" + iPAddress;
				ApiResponse apiResponse = Create(logError, "LogError");
			}
			catch
			{
			}
		}

		private static string GetIPAddress()
		{
			string text = "";
			try
			{
				WebRequest webRequest = WebRequest.Create("http://checkip.dyndns.org/");
				using (WebResponse webResponse = webRequest.GetResponse())
				{
					StreamReader streamReader = new StreamReader(webResponse.GetResponseStream());
					text = streamReader.ReadToEnd();
				}
				int num = text.IndexOf("Address: ") + 9;
				int num2 = text.LastIndexOf("</body>");
				text = text.Substring(num, num2 - num);
			}
			catch
			{
			}
			return text;
		}

		public static bool KiemTraQuyenAdmin()
		{
			if (HttpContext.Current.Session["idNhomQuyen"] != null && HttpContext.Current.Session["idNhomQuyen"].ToString() == "-1")
			{
				return true;
			}
			return false;
		}

		public static bool KiemTraQuyen(string MaForm, string MaQuyen, v_web_Menu web_Menu = null)
		{
			if (HttpContext.Current.Session["idNhomQuyen"] != null && HttpContext.Current.Session["idNhomQuyen"].ToString() == "-1")
			{
				return true;
			}
			List<view_web_PhanQuyen> phanQuyen = GetPhanQuyen();
			return ((web_Menu == null) ? phanQuyen.Where((view_web_PhanQuyen s) => !string.IsNullOrEmpty(s.CONTROLLERNAME) && (s.CONTROLLERNAME ?? "").ToUpper() == MaForm.ToUpper() && s.MAQUYEN.ToUpper() == MaQuyen.ToUpper() && s.ID_NHOMQUYEN == HttpContext.Current.Session["idNhomQuyen"].ToString()).FirstOrDefault() : phanQuyen.Where((view_web_PhanQuyen s) => s.ID_MENU.Trim() == web_Menu.ID.Trim() && s.MAQUYEN.ToUpper() == MaQuyen.ToUpper() && s.ID_NHOMQUYEN == HttpContext.Current.Session["idNhomQuyen"].ToString()).FirstOrDefault())?.TRANGTHAI ?? false;
		}

		public static string GetShowSearchValue<T>(string ShowSearchValue)
		{
			try
			{
				ShowSearchValue = ShowSearchValue.Replace(" ", "+");
				List<view_web_NoteClass> list = GetNoteClass();
				if (list != null)
				{
					list = list.Where((view_web_NoteClass s) => !string.IsNullOrEmpty(s.NAMECLASS) && s.NAMECLASS.ToLower() == typeof(T).Name.Replace("v_", "").ToLower() && s.ISSEARCH).ToList();
				}
				if (list == null)
				{
					PropertyInfo[] properties = typeof(T).GetProperties();
					List<Tuple<string, string, bool, int>> list2 = new List<Tuple<string, string, bool, int>>();
					Tuple<string, string, bool, int> item = new Tuple<string, string, bool, int>(clsMaHoa.Encrypt("ALL", "tmt6364"), "Tất cả", item3: false, 0);
					list2.Add(item);
					if (string.IsNullOrEmpty(ShowSearchValue))
					{
						ShowSearchValue = clsMaHoa.Encrypt("ALL", "tmt6364");
					}
					int num = 1;
					PropertyInfo[] array = properties;
					foreach (PropertyInfo propertyInfo in array)
					{
						if (string.IsNullOrEmpty(ShowSearchValue))
						{
							ShowSearchValue = clsMaHoa.Encrypt(propertyInfo.Name, "tmt6364");
						}
						bool isVirtual = propertyInfo.GetAccessors()[0].IsVirtual;
						num++;
						Tuple<string, string, bool, int> item2 = new Tuple<string, string, bool, int>(clsMaHoa.Encrypt(propertyInfo.Name, "tmt6364"), propertyInfo.Name, isVirtual, num);
						list2.Add(item2);
					}
					HttpContext.Current.Session["listSearch"] = list2;
				}
				else
				{
					List<Tuple<string, string, bool, int>> list3 = new List<Tuple<string, string, bool, int>>();
					Tuple<string, string, bool, int> item3 = new Tuple<string, string, bool, int>(clsMaHoa.Encrypt("ALL", "tmt6364"), "Tất cả", item3: false, 0);
					list3.Add(item3);
					if (string.IsNullOrEmpty(ShowSearchValue))
					{
						ShowSearchValue = clsMaHoa.Encrypt("ALL", "tmt6364");
					}
					int num3 = 1;
					PropertyInfo[] properties2 = typeof(T).GetProperties();
					foreach (view_web_NoteClass item5 in list.OrderBy((view_web_NoteClass s) => s.STT))
					{
						if (string.IsNullOrEmpty(ShowSearchValue))
						{
							ShowSearchValue = clsMaHoa.Encrypt(item5.NAMECOLUMN, "tmt6364");
						}
						num3++;
						Tuple<string, string, bool, int> item4 = new Tuple<string, string, bool, int>(clsMaHoa.Encrypt(item5.NAMECOLUMN, "tmt6364"), (!string.IsNullOrEmpty(item5.DISPLAYNAME)) ? item5.DISPLAYNAME : item5.NAMECOLUMN, item3: true, num3);
						list3.Add(item4);
					}
					HttpContext.Current.Session["listSearch"] = list3;
				}
			}
			catch (Exception e)
			{
				WriteLog("GetShowSearchValue", MethodBase.GetCurrentMethod().Name, e);
			}
			return ShowSearchValue;
		}

		public static void SetSession(ApiResponse apiResponse, Login_Model model, HttpCookie cookie)
		{
			if (model.check && cookie != null)
			{
				cookie = new HttpCookie("THP");
				string value = clsMaHoa.Encrypt(model.user + Environment.NewLine + model.pass, "tmt6364");
				cookie.Values["Us"] = value;
				cookie.Expires = CurrentTime.AddDays(90.0);
				HttpContext.Current.Response.Cookies.Add(cookie);
			}
			HttpContext.Current.Session["Token"] = apiResponse.Data;
			HttpContext.Current.Session["Expires"] = apiResponse.Expires;
			ApiResponseUser apiResponseUser = JsonConvert.DeserializeObject<ApiResponseUser>((apiResponse.Detail != null) ? apiResponse.Detail.ToString() : "{}");
			model.fullname = apiResponseUser.FullName;
			model.iduser = apiResponseUser.idUser;
			HttpContext.Current.Session["idUser"] = apiResponseUser.idUser;
			HttpContext.Current.Session["idNhomQuyen"] = apiResponseUser.idNhomQuyen;
			HttpContext.Current.Session["Login_Model"] = model;
			HttpContext.Current.Session["User"] = apiResponseUser.UserName;
			ResetCach();
		}

		public static void ResetCach()
		{
			Reset();
			GetNoteClass(bolCache: true);
			GetMenu(bolCache: true);
			GetPhanQuyen(bolCache: true);
			GetThongBao(bolCache: true);
		}

		private static string GetErrorServer(HttpResponseMessage response)
		{
			string text = "";
			if (response.StatusCode == HttpStatusCode.Unauthorized)
			{
				GetToken(bolCache: true);
				return "Authorization bị sai! " + response.StatusCode.ToString() + response.ReasonPhrase.ToString();
			}
			if (response.StatusCode == HttpStatusCode.NotFound)
			{
				return "Lỗi hệ thống server! " + response.StatusCode;
			}
			if (response.StatusCode == HttpStatusCode.RequestTimeout)
			{
				return "Kết nối server Timeout! " + response.StatusCode;
			}
			if (response.StatusCode == HttpStatusCode.InternalServerError)
			{
				return "500 (Internal Server Error)!" + response.StatusCode;
			}
			return "Lỗi không xác định! " + response.StatusCode;
		}

		public static void Reset()
		{
			HttpContext.Current.Session["Menu"] = "";
		}

		public static List<modelState> GetModelState(ModelStateDictionary ModelState, string Name)
		{
			List<web_ThongBao> thongBao = GetThongBao();
			List<view_web_NoteClass> noteClass = GetNoteClass();
			List<modelState> list = new List<modelState>();
			int num = 0;
			foreach (string key in ModelState.Keys)
			{
				modelState objmodelState = new modelState();
				objmodelState.Key = key;
				int num2 = 0;
				foreach (ModelState value in ModelState.Values)
				{
					if (num2 == num)
					{
						ModelErrorCollection errors = value.Errors;
						if (errors.Any())
						{
							foreach (ModelError item in errors)
							{
								if (string.IsNullOrEmpty(item.ErrorMessage))
								{
									continue;
								}
								string strerror = (string.IsNullOrEmpty(objmodelState.Key) ? item.ErrorMessage : item.ErrorMessage.Replace(objmodelState.Key, "'...'"));
								web_ThongBao web_ThongBao2 = thongBao.Where((web_ThongBao e) => e.DISPLAYNAME.ToLower() == strerror.ToLower()).FirstOrDefault();
								if (web_ThongBao2 != null)
								{
									if (web_ThongBao2.VN != null)
									{
										view_web_NoteClass view_web_NoteClass2 = noteClass.Where((view_web_NoteClass s) => !string.IsNullOrEmpty(s.CONTROLLER) && s.CONTROLLER.ToLower() == Name.ToLower() && s.NAMECOLUMN.ToLower() == objmodelState.Key.ToLower()).FirstOrDefault();
										if (view_web_NoteClass2 != null)
										{
											objmodelState.Error += web_ThongBao2.VN.Replace("...", view_web_NoteClass2.DISPLAYNAME);
										}
										else
										{
											objmodelState.Error += web_ThongBao2.VN.Replace("...", objmodelState.Key);
										}
									}
									else
									{
										objmodelState.Error += web_ThongBao2.DISPLAYNAME;
									}
								}
								else
								{
									web_ThongBao web_ThongBao3 = new web_ThongBao();
									web_ThongBao3.ID = Guid.NewGuid().ToString();
									web_ThongBao3.DISPLAYNAME = strerror;
									ApiResponse apiResponse = Create(web_ThongBao3, "ThongBao");
									objmodelState.Error += item.ErrorMessage;
								}
							}
						}
					}
					num2++;
				}
				list.Add(objmodelState);
				num++;
			}
			return list;
		}

		public static List<ValueEdit> ConvertobjectTo<T>(T objectTo, string FomatDate = "yyyy-MM-dd HH:mm:ss")
		{
			List<ValueEdit> list = new List<ValueEdit>();
			if (objectTo != null)
			{
				PropertyInfo[] properties = objectTo.GetType().GetProperties();
				PropertyInfo[] array = properties;
				foreach (PropertyInfo propertyInfo in array)
				{
					if (propertyInfo != null)
					{
						object value = propertyInfo.GetValue(objectTo);
						ValueEdit valueEdit = new ValueEdit();
						valueEdit.Key = propertyInfo.Name;
						if (value != null && value.GetType().ToString().Contains("Date"))
						{
							valueEdit.Value = ((DateTime)value).ToString(FomatDate);
						}
						else if (value != null && value.GetType().ToString().Contains("Time"))
						{
							valueEdit.Value = value.ToString();
						}
						else
						{
							valueEdit.Value = value;
						}
						list.Add(valueEdit);
					}
				}
			}
			return list;
		}

		public static List<ValueEdit> ConvertobjectToView<T>(T objectTo, string strDatetime = "dd/MM/yyyy")
		{
			List<ValueEdit> list = new List<ValueEdit>();
			try
			{
				PropertyInfo[] properties = objectTo.GetType().GetProperties();
				PropertyInfo[] array = properties;
				foreach (PropertyInfo propertyInfo in array)
				{
					if (!(propertyInfo != null))
					{
						continue;
					}
					object value = propertyInfo.GetValue(objectTo);
					ValueEdit valueEdit = new ValueEdit();
					valueEdit.Key = propertyInfo.Name;
					if (value != null && value.GetType().ToString().Contains("Date"))
					{
						valueEdit.Value = ((DateTime)value).ToString(strDatetime);
					}
					else if (value != null && IsNumericType(value.GetType()))
					{
						if (Type.GetTypeCode(value.GetType()).ToString().Contains("Int"))
						{
							valueEdit.Value = ((int)value).ToString("N0");
						}
						else if (value.ToString().Contains(","))
						{
							int length = value.ToString().Split(',')[1].Length;
							valueEdit.Value = ((double)value).ToString("N" + length);
						}
						else
						{
							valueEdit.Value = ((double)value).ToString("N0");
						}
					}
					else
					{
						valueEdit.Value = value;
					}
					list.Add(valueEdit);
				}
				return list;
			}
			catch (Exception e)
			{
				WriteLog("ConvertobjectToView", MethodBase.GetCurrentMethod().Name, e, JsonConvert.SerializeObject(objectTo));
				return list;
			}
		}

		public static bool IsNumericType(Type type)
		{
			TypeCode typeCode = Type.GetTypeCode(type);
			TypeCode typeCode2 = typeCode;
			if ((uint)(typeCode2 - 5) <= 10u)
			{
				return true;
			}
			return false;
		}

		public static Product_Detail ConvertobjectToProduct_Detail<T>(T objectFrom, Product_Detail objectTo, string FomatDate = "yyyy-MM-dd HH:mm:ss")
		{
			PropertyInfo[] properties = objectFrom.GetType().GetProperties();
			PropertyInfo[] array = properties;
			foreach (PropertyInfo propertyInfo in array)
			{
				if (propertyInfo != null)
				{
					object value = propertyInfo.GetValue(objectFrom);
					PropertyInfo property = objectTo.GetType().GetProperty(propertyInfo.Name);
					if (property != null)
					{
						property.SetValue(objectTo, value);
					}
				}
			}
			return objectTo;
		}

		public static T EditObject<T>(T InputOutput, string TYPE, object VALUE = null)
		{
			PropertyInfo[] properties = InputOutput.GetType().GetProperties();
			PropertyInfo[] array = properties;
			foreach (PropertyInfo propertyInfo in array)
			{
				if (!(propertyInfo != null) || !(TYPE.ToLower() == propertyInfo.Name.ToLower()))
				{
					continue;
				}
				object value = propertyInfo.GetValue(InputOutput);
				if (value != null && value.GetType().ToString().Contains("Date"))
				{
					propertyInfo.SetValue(InputOutput, Convert.ToDateTime(VALUE), null);
				}
				else if (value != null && IsNumericType(value.GetType()))
				{
					if (Type.GetTypeCode(value.GetType()) == TypeCode.Int32)
					{
						propertyInfo.SetValue(InputOutput, Convert.ToInt32(VALUE), null);
					}
					else
					{
						propertyInfo.SetValue(InputOutput, ConvertStringToDouble(VALUE.ToString()), null);
					}
				}
				else if (value != null && Type.GetTypeCode(value.GetType()) == TypeCode.Boolean)
				{
					propertyInfo.SetValue(InputOutput, Convert.ToBoolean(VALUE.ToString() == "on"), null);
				}
				else
				{
					propertyInfo.SetValue(InputOutput, VALUE, null);
				}
			}
			return InputOutput;
		}

		public static int GetMaxIDDeposit_TEMP<T>(T ovjTable, string IDName, string LOC_ID = "")
		{
			List<T> list = new List<T>();
			try
			{
				HttpClient httpClient = new HttpClient();
				httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + Token);
				HttpResponseMessage result = httpClient.GetAsync(URL + "GetIDMax/" + ovjTable.GetType().BaseType.Name + "/" + IDName + (string.IsNullOrEmpty(LOC_ID) ? "" : ("/" + LOC_ID))).Result;
				if (result.IsSuccessStatusCode)
				{
					string result2 = result.Content.ReadAsStringAsync().Result;
					ApiResponse apiResponse = JsonConvert.DeserializeObject<ApiResponse>(result2);
					int result3 = 0;
					if (int.TryParse(apiResponse.Data.ToString(), out result3))
					{
						return result3 + 1;
					}
					return 1;
				}
				return 1;
			}
			catch (Exception ex)
			{
				ApiResponse apiResponse2 = new ApiResponse
				{
					Success = false,
					Message = ex.Message,
					Data = ""
				};
				return 1;
			}
		}

		public static List<v_dm_HangHoa_Combo> GetlstProductCombo()
		{
			lstProductCombo = new List<v_dm_HangHoa_Combo>();
			try
			{
				if (HttpContext.Current.Session["lstProductCombo"] != null && !string.IsNullOrEmpty(HttpContext.Current.Session["lstProductCombo"].ToString()))
				{
					lstProductCombo = (List<v_dm_HangHoa_Combo>)HttpContext.Current.Session["lstProductCombo"];
				}
			}
			catch (Exception e)
			{
				WriteLog("GetlstProductCombo", MethodBase.GetCurrentMethod().Name, e);
			}
			return lstProductCombo;
		}

		public static List<v_dm_ChuongTrinhKhuyenMai_YeuCau> GetlstCTKM_YeuCau()
		{
			lstCTKM_YeuCau = new List<v_dm_ChuongTrinhKhuyenMai_YeuCau>();
			try
			{
				if (HttpContext.Current.Session["lstCTKM_YeuCau"] != null && !string.IsNullOrEmpty(HttpContext.Current.Session["lstCTKM_YeuCau"].ToString()))
				{
					lstCTKM_YeuCau = (List<v_dm_ChuongTrinhKhuyenMai_YeuCau>)HttpContext.Current.Session["lstCTKM_YeuCau"];
				}
			}
			catch (Exception e)
			{
				WriteLog("GetlstCTKM_YeuCau", MethodBase.GetCurrentMethod().Name, e);
			}
			return lstCTKM_YeuCau;
		}

		public static List<v_dm_BangLuong_ChiTiet> Getlstdm_BangLuong_ChiTiet()
		{
			lstdm_BangLuong_ChiTiet = new List<v_dm_BangLuong_ChiTiet>();
			try
			{
				if (HttpContext.Current.Session["lstdm_LuongThang_ChiTiet"] != null && !string.IsNullOrEmpty(HttpContext.Current.Session["lstdm_LuongThang_ChiTiet"].ToString()))
				{
					lstdm_BangLuong_ChiTiet = (List<v_dm_BangLuong_ChiTiet>)HttpContext.Current.Session["lstdm_LuongThang_ChiTiet"];
				}
			}
			catch (Exception e)
			{
				WriteLog("GetlstKPISale_YeuCau", MethodBase.GetCurrentMethod().Name, e);
			}
			return lstdm_BangLuong_ChiTiet;
		}

		public static List<nv_BangLuong_ChiTiet> Getlstnv_BangLuong_ChiTiet()
		{
			lstnv_BangLuong_ChiTiet = new List<nv_BangLuong_ChiTiet>();
			try
			{
				if (HttpContext.Current.Session["lstnv_BangLuong_ChiTiet"] != null && !string.IsNullOrEmpty(HttpContext.Current.Session["lstnv_BangLuong_ChiTiet"].ToString()))
				{
					lstnv_BangLuong_ChiTiet = (List<nv_BangLuong_ChiTiet>)HttpContext.Current.Session["lstnv_BangLuong_ChiTiet"];
				}
			}
			catch (Exception e)
			{
				WriteLog("Getlstnv_BangLuong_ChiTiet", MethodBase.GetCurrentMethod().Name, e);
			}
			return lstnv_BangLuong_ChiTiet;
		}

		public static List<dm_HangHoa_KhungGia> GetlstProductPriceRange()
		{
			lstProductPriceRange = new List<dm_HangHoa_KhungGia>();
			try
			{
				if (HttpContext.Current.Session["lstProductPriceRange"] != null && !string.IsNullOrEmpty(HttpContext.Current.Session["lstProductPriceRange"].ToString()))
				{
					lstProductPriceRange = (List<dm_HangHoa_KhungGia>)HttpContext.Current.Session["lstProductPriceRange"];
				}
			}
			catch (Exception e)
			{
				WriteLog("lstProductPriceRange", MethodBase.GetCurrentMethod().Name, e);
			}
			return lstProductPriceRange;
		}

		public static List<v_dm_HangHoa_KhungGia_HangHoa> GetlstProductPriceRangeHangHoa()
		{
			lstProductPriceRangeHangHoa = new List<v_dm_HangHoa_KhungGia_HangHoa>();
			try
			{
				if (HttpContext.Current.Session["lstProductPriceRangeHangHoa"] != null && !string.IsNullOrEmpty(HttpContext.Current.Session["lstProductPriceRangeHangHoa"].ToString()))
				{
					lstProductPriceRangeHangHoa = (List<v_dm_HangHoa_KhungGia_HangHoa>)HttpContext.Current.Session["lstProductPriceRangeHangHoa"];
				}
			}
			catch (Exception e)
			{
				WriteLog("lstProductPriceRange", MethodBase.GetCurrentMethod().Name, e);
			}
			return lstProductPriceRangeHangHoa;
		}

		public static List<v_dm_KPI_KinhDoanh_YeuCau> GetlstKPISale_YeuCau()
		{
			lstKPISale_YeuCau = new List<v_dm_KPI_KinhDoanh_YeuCau>();
			try
			{
				if (HttpContext.Current.Session["lstKPISale_YeuCau"] != null && !string.IsNullOrEmpty(HttpContext.Current.Session["lstKPISale_YeuCau"].ToString()))
				{
					lstKPISale_YeuCau = (List<v_dm_KPI_KinhDoanh_YeuCau>)HttpContext.Current.Session["lstKPISale_YeuCau"];
				}
			}
			catch (Exception e)
			{
				WriteLog("GetlstKPISale_YeuCau", MethodBase.GetCurrentMethod().Name, e);
			}
			return lstKPISale_YeuCau;
		}

		public static List<v_dm_KPI_KinhDoanh_NhanVien> GetlstKPISale_NhanVien()
		{
			lstKPISale_NhanVien = new List<v_dm_KPI_KinhDoanh_NhanVien>();
			try
			{
				if (HttpContext.Current.Session["lstKPISale_NhanVien"] != null && !string.IsNullOrEmpty(HttpContext.Current.Session["lstKPISale_NhanVien"].ToString()))
				{
					lstKPISale_NhanVien = (List<v_dm_KPI_KinhDoanh_NhanVien>)HttpContext.Current.Session["lstKPISale_NhanVien"];
				}
			}
			catch (Exception e)
			{
				WriteLog("GetlstKPISale_NhanVien", MethodBase.GetCurrentMethod().Name, e);
			}
			return lstKPISale_NhanVien;
		}

		public static List<v_dm_ChuongTrinhKhuyenMai_Tang> GetlstCTKM_Tang()
		{
			lstCTKM_Tang = new List<v_dm_ChuongTrinhKhuyenMai_Tang>();
			try
			{
				if (HttpContext.Current.Session["lstCTKM_Tang"] != null && !string.IsNullOrEmpty(HttpContext.Current.Session["lstCTKM_Tang"].ToString()))
				{
					lstCTKM_Tang = (List<v_dm_ChuongTrinhKhuyenMai_Tang>)HttpContext.Current.Session["lstCTKM_Tang"];
				}
			}
			catch (Exception e)
			{
				WriteLog("GetlstCTKM_Tang", MethodBase.GetCurrentMethod().Name, e);
			}
			return lstCTKM_Tang;
		}

		public static List<Product_Detail> GetlstProductInput()
		{
			lstProductInput = new List<Product_Detail>();
			try
			{
				if (HttpContext.Current.Session["lstProductInput"] != null && !string.IsNullOrEmpty(HttpContext.Current.Session["lstProductInput"].ToString()))
				{
					lstProductInput = (List<Product_Detail>)HttpContext.Current.Session["lstProductInput"];
				}
			}
			catch (Exception e)
			{
				WriteLog("GetLstProductInput", MethodBase.GetCurrentMethod().Name, e);
			}
			return lstProductInput;
		}

		public static List<Product_Detail> GetlstProductInvoiced()
		{
			lstProductInvoiced = new List<Product_Detail>();
			try
			{
				if (HttpContext.Current.Session["lstProductInvoiced"] != null && !string.IsNullOrEmpty(HttpContext.Current.Session["lstProductInvoiced"].ToString()))
				{
					lstProductInvoiced = (List<Product_Detail>)HttpContext.Current.Session["lstProductInvoiced"];
				}
			}
			catch (Exception e)
			{
				WriteLog("lstProductInvoiced", MethodBase.GetCurrentMethod().Name, e);
			}
			return lstProductInvoiced;
		}

		public static List<v_ct_PhieuGiaoHang_ChiTiet> GetPhieuGiaoHang_ChiTiet()
		{
			lstPhieuGiaoHang_ChiTiet = new List<v_ct_PhieuGiaoHang_ChiTiet>();
			try
			{
				if (HttpContext.Current.Session["lstDelivery_Detail"] != null && !string.IsNullOrEmpty(HttpContext.Current.Session["lstDelivery_Detail"].ToString()))
				{
					lstPhieuGiaoHang_ChiTiet = (List<v_ct_PhieuGiaoHang_ChiTiet>)HttpContext.Current.Session["lstDelivery_Detail"];
				}
			}
			catch (Exception e)
			{
				WriteLog("GetPhieuGiaoHang_ChiTiet", MethodBase.GetCurrentMethod().Name, e);
			}
			return lstPhieuGiaoHang_ChiTiet;
		}

		public static List<v_ct_PhieuGiaoHang_NhanVienGiao> GetPhieuGiaoHang_NhanVienGiao()
		{
			lstPhieuGiaoHang_NhanVienGiao = new List<v_ct_PhieuGiaoHang_NhanVienGiao>();
			try
			{
				if (HttpContext.Current.Session["lstDelivery_Shipper"] != null && !string.IsNullOrEmpty(HttpContext.Current.Session["lstDelivery_Shipper"].ToString()))
				{
					lstPhieuGiaoHang_NhanVienGiao = (List<v_ct_PhieuGiaoHang_NhanVienGiao>)HttpContext.Current.Session["lstDelivery_Shipper"];
				}
			}
			catch (Exception e)
			{
				WriteLog("GetPhieuGiaoHang_NhanVienGiao", MethodBase.GetCurrentMethod().Name, e);
			}
			return lstPhieuGiaoHang_NhanVienGiao;
		}

		public static string ConvertNumberToString(object value, int? sole = null)
		{
			try
			{
				if (sole.HasValue)
				{
					return Convert.ToDecimal(value).ToString("N" + sole.Value).Replace(",", ".");
				}
				return value.ToString().Replace(",", ".");
			}
			catch (Exception e)
			{
				WriteLog("ConvertNumberToString", MethodBase.GetCurrentMethod().Name, e);
				return "0";
			}
		}

		public static double ConvertStringToDouble(object value, bool bolForm = true)
		{
			try
			{
				if (bolForm)
				{
					return Convert.ToDouble(value.ToString().Replace("'", "").Replace(".", ","));
				}
				return Convert.ToDouble(value.ToString().Replace("'", "").Replace(",", "."));
			}
			catch (Exception e)
			{
				WriteLog("ConvertStringToDouble", MethodBase.GetCurrentMethod().Name, e);
				return 0.0;
			}
		}

		public static string GetProductCombo()
		{
			string text = "";
			foreach (v_dm_HangHoa_Combo item in LstProductCombo)
			{
				text = text + "<tr id=\"" + item.ID + "\">";
				text = text + "<td style=\"white-space: nowrap; \" id=\"MA\">" + item.MA + "</td>";
				text = text + "<td style=\"white-space: nowrap; \" id=\"NAME\">" + item.NAME + "</td>";
				text = text + "<td style=\"white-space: nowrap; \" id=\"QTY\"><input type=\"number\" class=\"form-control maskinput\" data-type=\"currency\"  name=\"txtQuantity|" + item.ID_HANGHOA + "|" + item.ID_DVT + "|" + item.TYLE_QD + "\" min=\"0.10\" data-id=\"" + item.ID + "\" step=\"any\" value=\"" + ConvertNumberToString(item.QTY) + "\" style=\"width:80px\" min=\"0\"/></td>";
				text = text + "<td style=\"white-space: nowrap; \" id=\"NAME_DVT\">" + item.NAME_DVT + "</td>";
				text = text + "<td style=\"white-space: nowrap; \"><a class=\"label label-danger\" onclick=\"myFunctionDeleteProdcutCombo('Product_Combo','" + item.ID_HANGHOA + "','" + item.ID_DVT + "')\" href=\"#\"><i class=\"glyphicon glyphicon-trash\" style=\"margin-right:5px\"></i>" + Xoa + "</a></td>";
				text += "</tr>";
			}
			return text;
		}

		public static string GetCTKM_YeuCau()
		{
			string text = "";
			foreach (v_dm_ChuongTrinhKhuyenMai_YeuCau item in LstCTKM_YeuCau)
			{
				string text2 = clsMaHoa.Encrypt(JsonConvert.SerializeObject(item), "tmt6364");
				text = text + "<tr id=\"" + item.ID + "\">";
				text = text + "<td style=\"white-space: nowrap; \" id=\"MA\">" + item.NAME_HINHTHUC + "</td>";
				text = text + "<td style=\"white-space: nowrap; \" id=\"MA\">" + item.MA + "</td>";
				text = text + "<td style=\"white-space: nowrap; \" id=\"NAME\">" + item.NAME + "</td>";
				text = text + "<td style=\"white-space: nowrap; \" id=\"MONEY\"><input type=\"number\" class=\"form-control maskinput\" data-type=\"currency\"  name=\"txtMoney_YC|" + text2 + "\"  step=\"any\" value=\"" + ConvertNumberToString(item.SOTIEN) + "\" style=\"width:100%\" min=\"0\"/></td>";
				text = text + "<td style=\"white-space: nowrap; \" id=\"QTY\"><input type=\"number\" class=\"form-control maskinput\" data-type=\"currency\" name=\"txtQuantity_YC|" + text2 + "\" step=\"any\" value=\"" + ConvertNumberToString(item.SOLUONG) + "\" style=\"width:100%\" min=\"0\"/></td>";
				text = text + "<td style=\"white-space: nowrap; \" id=\"NAME_DVT\">" + item.NAME_DVT + "</td>";
				text = text + "<td style=\"white-space: nowrap; \" id=\"CHIETKHAU\"><input type=\"number\" class=\"form-control maskinput\" data-type=\"currency\" name=\"txtCHIETKHAU_YC|" + text2 + "\" step=\"any\" value=\"" + ConvertNumberToString(item.CHIETKHAU) + "\" style=\"width:100%\" min=\"0\" max=\"100\"/></td>";
				text = text + "<td style=\"white-space: nowrap; \" id=\"TIENGIAM\"><input type=\"number\" class=\"form-control maskinput\" data-type=\"currency\" name=\"txtTIENGIAM_YC|" + text2 + "\" step=\"any\" value=\"" + ConvertNumberToString(item.TIENGIAM) + "\" style=\"width:100%\" min=\"0\"/></td>";
				text = text + "<td style=\"white-space: nowrap; \" id=\"ISBATBUOC\"><input type=\"checkbox\" class=\"form-control\" name=\"txtISBATBUOC|" + text2 + "\" id=\"ISBATBUOC\" " + (item.ISBATBUOC ? "checked" : "") + "/></td>";
				text = text + "<td style=\"white-space: nowrap; \" id=\"SOLUONG_BATBUOC\"><input type=\"number\" class=\"form-control maskinput\" data-type=\"currency\" name=\"txtSOLUONG_BATBUOC|" + text2 + "\" step=\"any\" value=\"" + ConvertNumberToString(item.SOLUONG_BATBUOC) + "\" style=\"width:100%\" min=\"0\"/></td>";
				text = text + "<td style=\"white-space: nowrap; \"><a class=\"label label-danger\" onclick=\"myFunctionDeletePromotion_YC('Promotion','" + item.ID_HANGHOA + "','" + item.ID_DVT + "')\" href=\"#\"><i class=\"glyphicon glyphicon-trash\" style=\"margin-right:5px\"></i>" + Xoa + "</a></td>";
				text += "</tr>";
			}
			return text;
		}

		public static string GetCTKM_Tang()
		{
			string text = "";
			foreach (v_dm_ChuongTrinhKhuyenMai_Tang item in LstCTKM_Tang)
			{
				string text2 = clsMaHoa.Encrypt(JsonConvert.SerializeObject(item), "tmt6364");
				text = text + "<tr id=\"" + item.ID + "\">";
				text = text + "<td style=\"white-space: nowrap; \" id=\"MA\">" + item.MA + "</td>";
				text = text + "<td style=\"white-space: nowrap; \" id=\"NAME\">" + item.NAME + "</td>";
				text = text + "<td style=\"white-space: nowrap; \" id=\"MONEY\"><input type=\"number\" class=\"form-control maskinput\" data-type=\"currency\" name=\"txtMoney_Tang|" + text2 + "\"  step=\"any\" value=\"" + ConvertNumberToString(item.SOTIEN) + "\" style=\"width:100%\" min=\"0\"/></td>";
				text = text + "<td style=\"white-space: nowrap; \" id=\"QTY\"><input type=\"number\" class=\"form-control maskinput\" data-type=\"currency\" name=\"txtQuantity_Tang|" + text2 + "\" step=\"any\" value=\"" + ConvertNumberToString(item.SOLUONG) + "\" style=\"width:100%\" min=\"0\"/></td>";
				text = text + "<td style=\"white-space: nowrap; \" id=\"NAME_DVT\">" + item.NAME_DVT + "</td>";
				text = text + "<td style=\"white-space: nowrap; \"><a class=\"label label-danger\" onclick=\"myFunctionDeletePromotion_Tang('Promotion','" + item.ID_HANGHOA + "','" + item.ID_DVT + "')\" href=\"#\"><i class=\"glyphicon glyphicon-trash\" style=\"margin-right:5px\"></i>" + Xoa + "</a></td>";
				text += "</tr>";
			}
			return text;
		}

		public static string GetKPISale_YeuCau()
		{
			string text = "";
			foreach (v_dm_KPI_KinhDoanh_YeuCau item in LstKPISale_YeuCau)
			{
				string text2 = clsMaHoa.Encrypt(JsonConvert.SerializeObject(item), "tmt6364");
				text = text + "<tr id=\"" + item.ID + "\">";
				text = text + "<td style=\"white-space: nowrap; \" id=\"HINHTHUC_TINHKPI\"><select class=\"form-control chosen-select\" name=\"HINHTHUC_TINHKPI|" + text2 + "\" id=\"HINHTHUC_TINHKPI\" style=\"width:150px\">";
				text += "<option value>Chọn hình thức tính</option>";
				foreach (API.LoaiHangHoa item2 in API.lstHinhThucTinhKPI())
				{
					text = text + "<option value = \"" + item2.ID + "\" " + ((item2.ID == item.HINHTHUC_TINHKPI) ? "selected" : "") + "> " + item2.NAME + " </option>";
				}
				text += "</select></td>";
				text = text + "<td style=\"white-space: nowrap; \" id=\"MA\">" + item.NAME_HINHTHUC + "</td>";
				text = text + "<td style=\"white-space: nowrap; \" id=\"MA\">" + item.MA + "</td>";
				text = text + "<td style=\"white-space: nowrap; \" id=\"NAME\">" + item.NAME + "</td>";
				text = text + "<td style=\"white-space: nowrap; \" id=\"MONEY\"><input type=\"number\" class=\"form-control maskinput\" data-type=\"currency\" name=\"txtMoney_YC|" + text2 + "\"  step=\"any\" value=\"" + ConvertNumberToString(item.SOTIEN) + "\" style=\"width:100%\" min=\"0\"/></td>";
				text = text + "<td style=\"white-space: nowrap; \" id=\"QTY\"><input type=\"number\" class=\"form-control maskinput\" data-type=\"currency\" name=\"txtQuantity_YC|" + text2 + "\" step=\"any\" value=\"" + ConvertNumberToString(item.SOLUONG) + "\" style=\"width:100%\" min=\"0\"/></td>";
				text = text + "<td style=\"white-space: nowrap; \" id=\"NAME_DVT\">" + item.NAME_DVT + "</td>";
				text = text + "<td style=\"white-space: nowrap; \" id=\"CHIETKHAU\"><input type=\"number\" class=\"form-control maskinput\" data-type=\"currency\" name=\"txtCHIETKHAU_YC|" + text2 + "\" step=\"any\" value=\"" + ConvertNumberToString(item.CHIETKHAU) + "\" style=\"width:100%\" min=\"0\" max=\"100\"/></td>";
				text = text + "<td style=\"white-space: nowrap; \" id=\"TIENGIAM\"><input type=\"number\" class=\"form-control maskinput\" data-type=\"currency\" name=\"txtTIENGIAM_YC|" + text2 + "\" step=\"any\" value=\"" + ConvertNumberToString(item.TIENGIAM) + "\" style=\"width:100%\" min=\"0\"/></td>";
				text = text + "<td style=\"white-space: nowrap; \"><a class=\"label label-danger\" onclick=\"myFunctionDeletePromotion_YC('KPI_Sale','" + item.ID_HANGHOA + "','" + item.ID_DVT + "')\" href=\"#\"><i class=\"glyphicon glyphicon-trash\" style=\"margin-right:5px\"></i>" + Xoa + "</a></td>";
				text += "</tr>";
			}
			return text;
		}

		public static string GetKPISale_NhanVien()
		{
			string text = "";
			foreach (v_dm_KPI_KinhDoanh_NhanVien item in LstKPISale_NhanVien)
			{
				string text2 = clsMaHoa.Encrypt(JsonConvert.SerializeObject(item), "tmt6364");
				text = text + "<tr id=\"" + item.ID + "\">";
				text = text + "<td style=\"white-space: nowrap; \" id=\"MA\">" + item.NAME_HINHTHUC + "</td>";
				text = text + "<td style=\"white-space: nowrap; \" id=\"MA\">" + item.MA + "</td>";
				text = text + "<td style=\"white-space: nowrap; \" id=\"NAME\">" + item.NAME + "</td>";
				text = text + "<td style=\"white-space: nowrap;display: none; \" id=\"ISACTIVE\" ><input type=\"checkbox\" class=\"form-control\" name=\"txtISACTIVE|" + text2 + "\" id=\"ISACTIVE\" checked/></td>";
				text = text + "<td style=\"white-space: nowrap; \"><a class=\"label label-danger\" onclick=\"myFunctionDeletePromotion_Tang('KPI_Sale','" + item.ID_NHANVIEN + "','" + item.HINHTHUC + "')\" href=\"#\"><i class=\"glyphicon glyphicon-trash\" style=\"margin-right:5px\"></i>" + Xoa + "</a></td>";
				text += "</tr>";
			}
			return text;
		}

		public static string GetProductPriceRange(List<v_dm_DonViTinh> lstdm_DonViTinh)
		{
			string text = "";
			foreach (dm_HangHoa_KhungGia item in LstProductPriceRange)
			{
				string text2 = clsMaHoa.Encrypt(JsonConvert.SerializeObject(item), "tmt6364");
				text = text + "<tr id=\"" + item.ID + "\">";
				text = text + "<td style=\"white-space: nowrap; \" id=\"TU\"><input type=\"number\" class=\"form-control maskinput\" data-type=\"currency\" name=\"txtTU|" + text2 + "\"  step=\"any\" value=\"" + ConvertNumberToString(item.TU) + "\" style=\"width:70px;\" min=\"0\"/></td>";
				text = text + "<td style=\"white-space: nowrap; \" id=\"DEN\"><input type=\"number\" class=\"form-control maskinput\" data-type=\"currency\" name=\"txtDEN|" + text2 + "\" step=\"any\" value=\"" + ConvertNumberToString(item.DEN) + "\" style=\"width:70px;\" min=\"0\"/></td>";
				text = text + "<td style=\"white-space: nowrap; \" id=\"ID_DVT\"><select class=\"form-control chosen-select\" name=\"ID_DVT|" + text2 + "\" id=\"ID_DVT\" style=\"width:150px\">";
				text += "<option value>Chọn ĐVT</option>";
				foreach (v_dm_DonViTinh item2 in lstdm_DonViTinh)
				{
					text = text + "<option value = \"" + item2.ID + "\" " + ((item2.ID == item.ID_DVT) ? "selected" : "") + "> " + item2.NAME + " </option>";
				}
				text += "</select></td>";
				text = text + "<td style=\"white-space: nowrap; \" id=\"DONGIA\"><input type=\"number\" class=\"form-control maskinput\" data-type=\"currency\" name=\"txtDONGIA|" + text2 + "\" step=\"any\" value=\"" + ConvertNumberToString(item.DONGIA) + "\" style=\"width:150px;\" min=\"0\"/></td>";
				text = text + "<td style=\"white-space: nowrap; \" id=\"HINHTHUC_TINHKPI\"><select class=\"form-control chosen-select\" name=\"HINHTHUC_TINHKPI|" + text2 + "\" id=\"HINHTHUC_TINHKPI\" style=\"width:150px\">";
				text += "<option value>Chọn hình thức tính</option>";
				foreach (API.LoaiHangHoa item3 in API.lstHinhThucTinhKPI())
				{
					text = text + "<option value = \"" + item3.ID + "\" " + ((item3.ID == item.HINHTHUC_TINHKPI) ? "selected" : "") + "> " + item3.NAME + " </option>";
				}
				text += "</select></td>";
				text = text + "<td style=\"white-space: nowrap; \" id=\"TIEN_KPI\"><input type=\"number\" class=\"form-control maskinput\" data-type=\"currency\" name=\"txtTIEN_KPI|" + text2 + "\" step=\"any\" value=\"" + ConvertNumberToString(item.TIEN_KPI) + "\" style=\"width:100%\" min=\"0\"/></td>";
				text = text + "<td style=\"white-space: nowrap; \" id=\"CK_KPI\"><input type=\"number\" class=\"form-control maskinput\" data-type=\"currency\" name=\"txtCK_KPI|" + text2 + "\" step=\"any\" value=\"" + ConvertNumberToString(item.CK_KPI) + "\" style=\"width:100%\"/></td>";
				text = text + "<td style=\"white-space: nowrap; \"><a class=\"label label-danger\" onclick=\"myFunctionDeletePromotion_YC('ProductPriceRange','','" + item.ID + "')\" href=\"#\"><i class=\"glyphicon glyphicon-trash\" style=\"margin-right:5px\"></i>" + Xoa + "</a></td>";
				text += "</tr>";
			}
			return text;
		}

		public static string GetProductPriceRange_HangHoa()
		{
			string text = "";
			foreach (v_dm_HangHoa_KhungGia_HangHoa item in LstProductPriceRangeHangHoa)
			{
				string text2 = clsMaHoa.Encrypt(JsonConvert.SerializeObject(item), "tmt6364");
				text = text + "<tr id=\"" + item.ID + "\">";
				text = text + "<td style=\"white-space: nowrap; \" id=\"MA\">" + item.MA + "</td>";
				text = text + "<td style=\"white-space: nowrap; \" id=\"NAME\">" + item.NAME + "</td>";
				text = text + "<td style=\"white-space: nowrap;display: none; \" id=\"txtISACTIVE\" ><input type=\"checkbox\" class=\"form-control\" name=\"txtISACTIVE|" + text2 + "\" id=\"ISACTIVE\" checked/></td>";
				text = text + "<td style=\"white-space: nowrap; \"><a class=\"label label-danger\" onclick=\"myFunctionDeletePromotion_Tang('ProductPriceRange','" + item.ID_HANGHOA + "','')\" href=\"#\"><i class=\"glyphicon glyphicon-trash\" style=\"margin-right:5px\"></i>" + Xoa + "</a></td>";
				text += "</tr>";
			}
			return text;
		}

		public static string GetProductInvoiced(List<Product_Detail> lstProduct, string name, bool bolTinhLai = true, v_ct_HoaDon HoaDon = null, string myBtn = "myBtnAdd")
		{
			string text = "";
			string text2 = "";
			if (lstProduct == null)
			{
				return "";
			}
			try
			{
				ApiResponse listData = GetListData<v_dm_ThueSuat>("Tax", "", "", LOC_ID);
				List<v_dm_ThueSuat> list = listData.Data as List<v_dm_ThueSuat>;
				v_dm_ThueSuat v_dm_ThueSuat2 = list.Where((v_dm_ThueSuat s) => s.MA == "0").FirstOrDefault();
				foreach (v_dm_ThueSuat item in list)
				{
					text = text + "<option value = \"" + item.ID + "\"> " + item.NAME + " </option>";
				}
				List<ComboboxFrom> list2 = DachSachTinhChat();
				foreach (ComboboxFrom item2 in list2)
				{
					text2 = text2 + "<option value = \"" + item2.ID + "\" " + (item2.ISDEFAULT ? "selected" : "") + "> " + item2.NAME + " </option>";
				}
				double num = 0.0;
				double num2 = 0.0;
				double num3 = 0.0;
				double num4 = 0.0;
				if (!bolTinhLai)
				{
					num = HoaDon.TONGTIENGIAMGIA;
					num2 = HoaDon.TONGTHANHTIEN;
					num3 = HoaDon.TONGTIENVAT;
					num4 = HoaDon.TONGTIEN;
				}
				string text3 = null;
				foreach (Product_Detail item3 in lstProduct.OrderBy((Product_Detail s) => s.STT))
				{
					if (bolTinhLai)
					{
						if (item3.TINHCHAT == 3)
						{
							num -= item3.TONGTIENGIAMGIA;
							num2 -= item3.THANHTIEN;
							num3 -= item3.TONGTIENVAT;
							num4 -= item3.TONGCONG;
						}
						else if (item3.TINHCHAT == 1)
						{
							num += item3.TONGTIENGIAMGIA;
							num2 += item3.THANHTIEN;
							num3 += item3.TONGTIENVAT;
							num4 += item3.TONGCONG;
						}
					}
					if (string.IsNullOrEmpty(item3.ID_THUESUAT))
					{
						item3.ID_THUESUAT = v_dm_ThueSuat2?.ID;
						item3.THUESUAT = v_dm_ThueSuat2?.THUESUAT ?? 0.0;
					}
					text3 = text3 + "<tr id=\"" + item3.ID + "\">";
					ChiTietTam chiTietTam = new ChiTietTam();
					chiTietTam.ID_HANGHOAKHO = item3.ID_HANGHOAKHO;
					chiTietTam.ID = item3.ID;
					chiTietTam.STT = item3.STT;
					string text4 = clsMaHoa.Encrypt(JsonConvert.SerializeObject(chiTietTam), "tmt6364");
					string text5 = ",'" + myBtn + "'";
					text3 = text3 + "<td style=\"white-space: nowrap; \" id=\"STT\"><input name=\"txtSTT|" + text4 + "\" id=\"STT\"  style=\"width:40px;display:inline-block\" type=\"number\" value='" + item3.STT + "'></td>";
					text3 = text3 + "<td style=\"white-space: nowrap; \" id=\"TINHCHAT\"><select class=\"form-control chosen-select\" onchange=\"update" + name + "('" + item3.ID + "',this" + text5 + ")\" name=\"txtTINHCHAT|" + text4 + "\" id=\"TINHCHAT\" style=\"width:150px\" >";
					text3 = text3 + "<option value>" + GetTitleChon("Tax") + "</option>";
					text3 = ((item3.TINHCHAT <= 0) ? (text3 + text2) : (text3 + text2.Replace("option value = \"" + item3.TINHCHAT + "\"", "option value = \"" + item3.TINHCHAT + "\" selected")));
					text3 += "</select>";
					string tENHANGHOA = item3.TENHANGHOA;
					text3 = text3 + "<td style=\"white-space: nowrap; \" id=\"MAHANGHOA\"><textarea class=\"form-control\" name=\"txtMAHANGHOA|" + text4 + "\" id=\"MAHANGHOA\"  style=\"width:100px;display:inline-block\" type=\"text\" rows='3' onchange = \"update" + name + "('" + item3.ID + "',this, '" + myBtn + "')\">" + item3.MAHANGHOA + "</textarea></td>";
					text3 = text3 + "<td style=\"white-space: nowrap; \" id=\"TENHANGHOA\"><textarea class=\"form-control\" name=\"txtTENHANGHOA|" + text4 + "\" id=\"TENHANGHOA\"  style=\"width:150px;display:inline-block\" type=\"text\" rows='3' onchange = \"update" + name + "('" + item3.ID + "',this, '" + myBtn + "')\">" + tENHANGHOA + "</textarea></td>";
					text3 = text3 + "<td style=\"white-space: nowrap; \" id=\"DVT\"><input class=\"form-control\" name=\"txtDVT|" + text4 + "\" id=\"DVT\"  style=\"width:80px;display:inline-block\" type=\"text\" value='" + item3.DVT + "' onchange = \"update" + name + "('" + item3.ID + "',this, '" + myBtn + "')\"></td>";
					text3 += Get_tdInput(name, "SOLUONG", "txtSOLUONG", chiTietTam, item3.SOLUONG, "100", bolreadonly: false, "0", myBtn);
					text3 += Get_tdInput(name, "DONGIA", "txtDONGIA", chiTietTam, item3.DONGIA, "100px", bolreadonly: false, "", myBtn);
					if (item3.TINHCHAT == 3)
					{
						text3 += "<td></td>";
						text3 += "<td></td>";
					}
					else
					{
						text3 += Get_tdInput(name, "CHIETKHAU", "txtCHIETKHAU", chiTietTam, item3.CHIETKHAU, "100px", bolreadonly: false, "", myBtn);
						text3 += Get_tdInput(name, "TONGTIENGIAMGIA", "txtTONGTIENGIAMGIA", chiTietTam, item3.TONGTIENGIAMGIA, "100px", bolreadonly: false, "-10000000", myBtn);
					}
					text3 += Get_tdInput(name, "THANHTIEN", "txtTHANHTIEN", chiTietTam, item3.THANHTIEN, "100px", bolreadonly: false, "", myBtn);
					text3 = text3 + "<td style=\"white-space: nowrap; \" id=\"ID_THUESUAT\"><select class=\"form-control chosen-select\" name=\"txtID_THUESUAT|" + text4 + "\" id=\"ID_THUESUAT\" style=\"width:80px\" onchange = \"update" + name + "('" + item3.ID + "',this, '" + myBtn + "')\">";
					text3 = text3 + "<option value>" + GetTitleChon("Tax") + "</option>";
					text3 += text.Replace("option value = \"" + item3.ID_THUESUAT + "\"", "option value = \"" + item3.ID_THUESUAT + "\" selected");
					text3 += "</select>";
					text3 += Get_tdInput(name, "TONGTIENVAT", "txtTONGTIENVAT", chiTietTam, item3.TONGTIENVAT, "100px", bolreadonly: false, "", myBtn);
					text3 += Get_tdInput(name, "TONGCONG", "txtTONGCONG", chiTietTam, item3.TONGCONG, "100px", bolreadonly: false, "", myBtn);
					text3 = ((!item3.ISCOMBO) ? (text3 + "<td style=\"white-space: nowrap; \" class=\"fix\"><a class=\"label label-danger\" onclick=\"myFunctionDeleteProdcut" + name + "('Invoiced','" + item3.ID + "', '" + myBtn + "')\" href=\"#\"><i class=\"glyphicon glyphicon-trash\" style=\"margin-right:5px\"></i>" + Xoa + "</a></td>") : (text3 + "<td></td>"));
					text3 += "</tr>";
				}
				if (!string.IsNullOrEmpty(text3))
				{
					text3 = text3 + "<tr><td style=\"font-weight: bold; text-align:center; white-space: nowrap;\" colspan=\"8\">   <label class=\"col-sm-2 control-label\" for=\"T_ng_ti_n\" style=\"font-weight: bold; text-align:center; white-space: nowrap;float:right;\">Tổng tiền</label></td><td style=\"font-weight: bold; text-align:center; white-space: nowrap;\">   <input class=\"form-control maskinput\" data-type=\"currency\" min=\"-10000000\" step=\"any\" data-val=\"true\" data-val-number=\"The field TONGTIENGIAMGIA must be a number.\" data-val-required=\"The TONGTIENGIAMGIA field is required.\" id=\"TONGTIENGIAMGIA\" name=\"TONGTIENGIAMGIA\" type=\"number\" value=\"" + ConvertNumberToString(num) + "\" style=\"width:100%\">    <span class=\"field-validation-valid text-danger\" data-valmsg-for=\"TONGTIENGIAMGIA\" data-valmsg-replace=\"true\"></span></td><td style=\"font-weight: bold; text-align:center; white-space: nowrap;\">    <input class=\"form-control maskinput\" data-type=\"currency\" min=\"0\" step=\"any\" data-val=\"true\" data-val-number=\"The field TONGTHANHTIEN must be a number.\" data-val-required=\"The TONGTHANHTIEN field is required.\" id=\"TONGTHANHTIEN\" name=\"TONGTHANHTIEN\" type=\"number\" value=\"" + ConvertNumberToString(num2) + "\" style=\"width:100%\">   <span class=\"field-validation-valid text-danger\" data-valmsg-for=\"TONGTHANHTIEN\" data-valmsg-replace=\"true\"></span></td><td style=\"font-weight: bold; text-align:center; white-space: nowrap;\" colspan=\"2\">    <input class=\"form-control maskinput\" data-type=\"currency\" min=\"0\" step=\"any\" data-val=\"true\" data-val-number=\"The field TONGTIENVAT must be a number.\" data-val-required=\"The TONGTIENVAT field is required.\" id=\"TONGTIENVAT\" name=\"TONGTIENVAT\" type=\"number\" value=\"" + ConvertNumberToString(num3) + "\" style=\"width:100%\">    <span class=\"field-validation-valid text-danger\" data-valmsg-for=\"TONGTIENVAT\" data-valmsg-replace=\"true\"></span></td><td style=\"font-weight: bold; text-align:center; white-space: nowrap;\">   <input class=\"form-control maskinput\" data-type=\"currency\" min=\"0\" step=\"any\" data-val=\"true\" data-val-number=\"The field TONGTIEN must be a number.\" data-val-required=\"The TONGTIEN field is required.\" id=\"TONGTIEN\" name=\"TONGTIEN\" type=\"number\" value=\"" + ConvertNumberToString(num4) + "\" style=\"width:100%\">   <span class=\"field-validation-valid text-danger\" data-valmsg-for=\"TONGTIEN\" data-valmsg-replace=\"true\"></span></td><td></td></tr>";
				}
				return text3;
			}
			catch (Exception e)
			{
				WriteLog("GetProductInputOutput", MethodBase.GetCurrentMethod().Name, e, JsonConvert.SerializeObject(lstProduct));
				return "";
			}
		}

		public static string GetProductInputOutput(List<Product_Detail> lstProduct, string name, bool bolTinhLai = true, double TONGTIENGIAMGIA = 0.0, double TONGTHANHTIEN = 0.0, double TONGTIENVAT = 0.0, double TONGTIEN = 0.0, bool bolSuaSoLuong = false, bool bolSuaDonGia = false, bool bolSuaGiamGia = false)
		{
			string text = "";
			if (lstProduct == null)
			{
				return "";
			}
			try
			{
				ApiResponse listData = GetListData<v_dm_ThueSuat>("Tax", "", "", LOC_ID);
				if (!(listData.Data is List<v_dm_ThueSuat> list))
				{
					return "";
				}
				foreach (v_dm_ThueSuat item in list)
				{
					text = text + "<option value = \"" + item.ID + "\"> " + item.NAME + " </option>";
				}
				string text2 = null;
				foreach (Product_Detail item2 in lstProduct)
				{
					if (bolTinhLai)
					{
						TONGTIENGIAMGIA += item2.TONGTIENGIAMGIA;
						TONGTHANHTIEN += item2.THANHTIEN;
						TONGTIENVAT += item2.TONGTIENVAT;
						TONGTIEN += item2.TONGCONG;
					}
					string text3 = clsMaHoa.Encrypt(JsonConvert.SerializeObject(item2), "tmt6364");
					text2 = text2 + "<tr id=\"" + item2.ID + "\">";
					text2 = text2 + "<td style=\"white-space: nowrap; \" id=\"MA\">" + item2.MA + "</td>";
					text2 = text2 + "<td style=\"white-space: nowrap; \" id=\"NAME\">" + (item2.ISKHUYENMAI ? "(KM)" : "") + item2.NAME + "</td>";
					text2 = text2 + "<td style=\"white-space: nowrap; \" id=\"NAME_DVT\">" + item2.NAME_DVT + "</td>";
					text2 += Get_tdInput(name, "SOLUONG", "txtSOLUONG", item2, item2.SOLUONG, "100", item2.ISKHUYENMAI ? (!bolSuaSoLuong) : (item2.ISCOMBO ? item2.ISCOMBO : ((item2.MA == API.GTBH) ? true : false)), "0");
					text2 += Get_tdInput(name, "DONGIA", "txtDONGIA", item2, item2.DONGIA, "100px", (!item2.ISKHUYENMAI) ? (!bolSuaDonGia) : (!bolSuaGiamGia), "-10000000");
					text2 += Get_tdInput(name, "CHIETKHAU", "txtCHIETKHAU", item2, item2.CHIETKHAU, "100px", (!item2.ISKHUYENMAI) ? item2.ISCOMBO : (!bolSuaGiamGia));
					text2 += Get_tdInput(name, "TONGTIENGIAMGIA", "txtTONGTIENGIAMGIA", item2, item2.TONGTIENGIAMGIA, "100px", (!item2.ISKHUYENMAI) ? item2.ISCOMBO : (!bolSuaGiamGia), "-10000000");
					text2 += Get_tdInput(name, "THANHTIEN", "txtTHANHTIEN", item2, item2.THANHTIEN, "100px", bolreadonly: true, "-10000000");
					if (item2.ISCOMBO || item2.ISKHUYENMAI)
					{
						text2 += "<td></td>";
					}
					else
					{
						text2 = text2 + "<td style=\"white-space: nowrap; \" id=\"ID_THUESUAT\"><select class=\"form-control chosen-select\" name=\"txtID_THUESUAT|" + text3 + "\" id=\"ID_THUESUAT\" style=\"width:80px\" onchange = \"updateInputOutput('" + item2.ID + "',this)\">";
						text2 = text2 + "<option value>" + GetTitleChon("Tax") + "</option>";
						text2 += text.Replace("option value = \"" + item2.ID_THUESUAT + "\"", "option value = \"" + item2.ID_THUESUAT + "\" selected");
						text2 += "</select>";
					}
					text2 += Get_tdInput(name, "TONGTIENVAT", "txtTONGTIENVAT", item2, item2.TONGTIENVAT, "100px", item2.ISKHUYENMAI || item2.ISCOMBO);
					text2 += Get_tdInput(name, "TONGCONG", "txtTONGCONG", item2, item2.TONGCONG, "100px", bolreadonly: true, "-10000000");
					text2 = ((!item2.ISCOMBO) ? (text2 + "<td style=\"white-space: nowrap; \" class=\"fix\"><a class=\"label label-danger\" onclick=\"myFunctionDeleteProdcut" + name + "('Product','" + item2.ID + "')\" href=\"#\"><i class=\"glyphicon glyphicon-trash\" style=\"margin-right:5px\"></i>" + Xoa + "</a></td>") : (text2 + "<td></td>"));
					text2 += "</tr>";
				}
				if (!string.IsNullOrEmpty(text2))
				{
					text2 = text2 + "<tr><td style=\"font-weight: bold; text-align:center; white-space: nowrap;\" colspan=\"6\">   <label class=\"col-sm-2 control-label\" for=\"T_ng_ti_n\" style=\"font-weight: bold; text-align:center; white-space: nowrap;float:right;\">Tổng tiền</label></td><td style=\"font-weight: bold; text-align:center; white-space: nowrap;\">   <input class=\"form-control maskinput\" data-type=\"currency\" min=\"-10000000\" step=\"any\" data-val=\"true\" data-val-number=\"The field TONGTIENGIAMGIA must be a number.\" data-val-required=\"The TONGTIENGIAMGIA field is required.\" id=\"TONGTIENGIAMGIA\" name=\"TONGTIENGIAMGIA\" type=\"number\" value=\"" + ConvertNumberToString(TONGTIENGIAMGIA) + "\" style=\"width:100%\">    <span class=\"field-validation-valid text-danger\" data-valmsg-for=\"TONGTIENGIAMGIA\" data-valmsg-replace=\"true\"></span></td><td style=\"font-weight: bold; text-align:center; white-space: nowrap;\">    <input class=\"form-control maskinput\" data-type=\"currency\" min=\"0\" step=\"any\" data-val=\"true\" data-val-number=\"The field TONGTHANHTIEN must be a number.\" data-val-required=\"The TONGTHANHTIEN field is required.\" id=\"TONGTHANHTIEN\" name=\"TONGTHANHTIEN\" type=\"number\" value=\"" + ConvertNumberToString(TONGTHANHTIEN) + "\" style=\"width:100%\">   <span class=\"field-validation-valid text-danger\" data-valmsg-for=\"TONGTHANHTIEN\" data-valmsg-replace=\"true\"></span></td><td style=\"font-weight: bold; text-align:center; white-space: nowrap;\" colspan=\"2\">    <input class=\"form-control maskinput\" data-type=\"currency\" min=\"0\" step=\"any\" data-val=\"true\" data-val-number=\"The field TONGTIENVAT must be a number.\" data-val-required=\"The TONGTIENVAT field is required.\" id=\"TONGTIENVAT\" name=\"TONGTIENVAT\" type=\"number\" value=\"" + ConvertNumberToString(TONGTIENVAT) + "\" style=\"width:100%\">    <span class=\"field-validation-valid text-danger\" data-valmsg-for=\"TONGTIENVAT\" data-valmsg-replace=\"true\"></span></td><td style=\"font-weight: bold; text-align:center; white-space: nowrap;\">   <input class=\"form-control maskinput\" data-type=\"currency\" min=\"0\" step=\"any\" data-val=\"true\" data-val-number=\"The field TONGTIEN must be a number.\" data-val-required=\"The TONGTIEN field is required.\" id=\"TONGTIEN\" name=\"TONGTIEN\" type=\"number\" value=\"" + ConvertNumberToString(TONGTIEN) + "\" style=\"width:100%\">   <span class=\"field-validation-valid text-danger\" data-valmsg-for=\"TONGTIEN\" data-valmsg-replace=\"true\"></span></td><td></td></tr>";
				}
				return text2;
			}
			catch (Exception e)
			{
				WriteLog("GetProductInputOutput", MethodBase.GetCurrentMethod().Name, e, JsonConvert.SerializeObject(lstProduct));
				return "";
			}
		}

		private static string Get_tdInput(string name, string nameinput, string txt, Product_Detail Product_Detail, double value, string width = "50px", bool bolreadonly = false, string Min = "", string myBtn = "")
		{
			if (name == "Deposit_Temp" && nameinput == "DONGIA")
			{
				bolreadonly = !KiemTraQuyen("Deposit", "EditPrice");
				if (Product_Detail.MA == API.TINHTHUE_KM || Product_Detail.MAHANGHOA == API.TINHTHUE_KM)
				{
					bolreadonly = false;
				}
			}
			if (!string.IsNullOrEmpty(myBtn))
			{
				myBtn = ",'" + myBtn + "'";
			}
			string text = clsMaHoa.Encrypt(JsonConvert.SerializeObject(Product_Detail), "tmt6364");
			string text2 = "";
			return "<td style=\"white-space: nowrap; \" id=\"" + nameinput + "\"><input type=\"number\" class=\"form-control maskinput\" data-type=\"currency\" name=\"" + txt + "|" + text + "\" id=\"" + nameinput + "\" min=\"" + ((!string.IsNullOrEmpty(Min)) ? Min : "0") + "\" step=\"any\" value=\"" + ConvertNumberToString(value) + "\" style=\"width:" + width + "\" min=\"0\" onchange=\"update" + name + "('" + Product_Detail.ID + "',this" + myBtn + ")\" " + (bolreadonly ? "" : "") + " " + (bolreadonly ? "readonly = \"readonly\"" : "") + "/></td>";
		}

		private static string Get_tdInput(string name, string nameinput, string txt, ChiTietTam Product_Detail, double value, string width = "50px", bool bolreadonly = false, string Min = "", string myBtn = "")
		{
			if (name == "Deposit_Temp" && nameinput == "DONGIA")
			{
				bolreadonly = !KiemTraQuyen("Deposit", "EditPrice");
			}
			if (name == "Deposit_Temp" && nameinput == "DONGIA")
			{
				bolreadonly = !KiemTraQuyen("Deposit", "EditPrice");
			}
			if (!string.IsNullOrEmpty(myBtn))
			{
				myBtn = ",'" + myBtn + "'";
			}
			string text = clsMaHoa.Encrypt(JsonConvert.SerializeObject(Product_Detail), "tmt6364");
			string text2 = "";
			return "<td style=\"white-space: nowrap; \" id=\"" + nameinput + "\"><input type=\"number\" class=\"form-control maskinput\" data-type=\"currency\" name=\"" + txt + "|" + text + "\" id=\"" + nameinput + "\" min=\"" + ((!string.IsNullOrEmpty(Min)) ? Min : "0") + "\" step=\"any\" value=\"" + ConvertNumberToString(value) + "\" style=\"width:" + width + "\" min=\"0\" onchange=\"update" + name + "('" + Product_Detail.ID + "',this" + myBtn + ")\" " + (bolreadonly ? "" : "") + " " + (bolreadonly ? "readonly = \"readonly\"" : "") + "/></td>";
		}

		public static string GetParameter(List<v_web_Report_Parameter> lstProduct)
		{
			try
			{
				string text = null;
				foreach (v_web_Report_Parameter item in lstProduct.OrderBy((v_web_Report_Parameter v_web_Report_Parameter2) => v_web_Report_Parameter2.STT))
				{
					if (string.IsNullOrEmpty(item.ID_PARAMETER))
					{
						item.ID_PARAMETER = item.ID;
					}
					string text2 = clsMaHoa.Encrypt(JsonConvert.SerializeObject(item), "tmt6364");
					text = text + "<tr id=\"" + item.ID + "\">";
					text = text + "<td style=\"white-space: nowrap; \" id=\"NAME_PARAMETER\">" + ((!string.IsNullOrEmpty(item.NAME_PARAMETER)) ? item.NAME_PARAMETER : item.NAME) + "</td>";
					text = text + "<td style=\"white-space: nowrap; \" id=\"STT\"><input type=\"number\" class=\"form-control maskinput\" data-type=\"currency\" name=\"txtSTT|" + text2 + "\" id=\"STT\" min=\"0\" step=\"any\" value=\"" + ConvertNumberToString(item.STT) + "\" style=\"width:80px\" min=\"0\"/></td>";
					text = text + "<td style=\"white-space: nowrap; \" id=\"ISACTIVE\"><input type=\"checkbox\" class=\"form-control\" name=\"txtISACTIVE|" + text2 + "\" id=\"ISACTIVE\" " + (item.ISACTIVE ? "checked" : "") + "/></td>";
					text = text + "<td style=\"white-space: nowrap; \" id=\"VALUE_REPORT\"><textarea class=\"form-control\" runat=\"server\" cols=\"20\" id=\"VALUE_REPORT\" name=\"txtVALUE_REPORT|" + text2 + "\" rows=\"3\"> " + item.VALUE_REPORT + "</textarea></td>";
				}
				return text;
			}
			catch (Exception e)
			{
				WriteLog("GetParameter", MethodBase.GetCurrentMethod().Name, e, JsonConvert.SerializeObject(lstProduct));
				return "";
			}
		}

		public static string GetCategoryPayroll(List<v_dm_BangLuong_ChiTiet> lstProduct, List<v_dm_LoaiLuong> lstLoaiLuong)
		{
			try
			{
				string text = null;
				foreach (v_dm_BangLuong_ChiTiet item in lstProduct)
				{
					string text2 = clsMaHoa.Encrypt(JsonConvert.SerializeObject(item), "tmt6364");
					text = text + "<tr id=\"" + item.ID + "\">";
					text = text + "<td style=\"white-space: nowrap; \" id=\"ID_LOAILUONG\"><select class=\"form-control chosen-select\" name=\"ID_LOAILUONG|" + text2 + "\" id=\"ID_LOAILUONG\" style=\"width:150px\">";
					text += "<option value>--Chọn loại lương--</option>";
					foreach (v_dm_LoaiLuong item2 in lstLoaiLuong)
					{
						text = text + "<option value = \"" + item2.ID + "\" " + ((item2.ID == item.ID_LOAILUONG) ? "selected" : "") + "> " + item2.NAME + " </option>";
					}
					text = text + "<td style=\"white-space: nowrap; \" id=\"TYPE_LUONG\"><select class=\"form-control chosen-select\" name=\"TYPE_LUONG|" + text2 + "\" id=\"TYPE_LUONG\" style=\"width:150px\">";
					text += "<option value>--Chọn hình thức tính--</option>";
					foreach (API.LoaiHangHoa item3 in API.lstTYPELuong())
					{
						text = text + "<option value = \"" + item3.ID + "\" " + ((item3.ID == item.TYPE_LUONG) ? "selected" : "") + "> " + item3.NAME + " </option>";
					}
					text += "</select></td>";
					text = text + "<td style=\"white-space: nowrap; \" id=\"TYPE_QUYTACTINHLUONG\"><select class=\"form-control chosen-select\" name=\"TYPE_QUYTACTINHLUONG|" + text2 + "\" id=\"TYPE_QUYTACTINHLUONG\" style=\"width:150px\">";
					text += "<option value>--Chọn quy tắc tính lương--</option>";
					foreach (API.LoaiHangHoa item4 in API.lstTYPEQuyTacTinhLuong())
					{
						text = text + "<option value = \"" + item4.ID + "\" " + ((item4.ID == item.TYPE_QUYTACTINHLUONG) ? "selected" : "") + "> " + item4.NAME + " </option>";
					}
					text += "</select></td>";
					text = text + "<td style=\"white-space: nowrap; \" id=\"SOTIEN\"><input type=\"number\" class=\"form-control maskinput\" data-type=\"currency\" name=\"txtSOTIEN|" + text2 + "\" step=\"any\" value=\"" + ConvertNumberToString(item.SOTIEN) + "\" style=\"width:100%\" min=\"0\"/></td>";
					text = text + "<td style=\"white-space: nowrap; \"><a class=\"label label-danger\" onclick=\"myFunctionDeletePayroll('" + item.ID + "','CategoryPayroll')\" href=\"#\"><i class=\"glyphicon glyphicon-trash\" style=\"margin-right:5px\"></i>" + Xoa + "</a></td>";
					text += "</tr>";
				}
				return text;
			}
			catch (Exception e)
			{
				WriteLog("GetCategoryPayroll", MethodBase.GetCurrentMethod().Name, e, JsonConvert.SerializeObject(lstProduct));
				return "";
			}
		}

		public static string GetPayrollDetail(List<nv_BangLuong_ChiTiet> lstProduct, List<v_dm_LoaiLuong> lstLoaiLuong)
		{
			try
			{
				string text = null;
				foreach (nv_BangLuong_ChiTiet item in lstProduct.OrderByDescending((nv_BangLuong_ChiTiet s) => s.SOTIEN))
				{
					string text2 = clsMaHoa.Encrypt(JsonConvert.SerializeObject(item), "tmt6364");
					text = text + "<tr id=\"" + item.ID + "\">";
					text = text + "<td style=\"white-space: nowrap; \" id=\"txtID_LOAILUONG\"><select class=\"form-control chosen-select\" name=\"txtID_LOAILUONG|" + text2 + "\" id=\"txtID_LOAILUONG\" style=\"width:150px\">";
					text += "<option value>--Chọn loại lương--</option>";
					foreach (v_dm_LoaiLuong item2 in lstLoaiLuong)
					{
						text = text + "<option value = \"" + item2.ID + "\" " + ((item2.ID == item.ID_LOAILUONG) ? "selected" : "") + "> " + item2.NAME + " </option>";
					}
					text = text + "<td style=\"white-space: nowrap; \" id=\"txtTYPE\"><select class=\"form-control chosen-select\" name=\"txtTYPE|" + text2 + "\" id=\"txtTYPE\" style=\"width:150px\">";
					text += "<option value>--Chọn hình thức tính--</option>";
					foreach (API.LoaiHangHoa item3 in API.lstTYPELoaiLuong())
					{
						text = text + "<option value = \"" + item3.ID + "\" " + ((item3.ID.ToString() == item.TYPE) ? "selected" : "") + "> " + item3.NAME + " </option>";
					}
					text += "</select></td>";
					text = text + "<td style=\"white-space: nowrap; \" id=\"txtSOTIEN\"><input type=\"number\" class=\"form-control maskinput\" data-type=\"currency\" name=\"txtSOTIEN|" + text2 + "\" step=\"any\" value=\"" + ConvertNumberToString(item.SOTIEN) + "\" style=\"width:100%\" min=\"0\"/></td>";
					text = text + "<td style=\"white-space: nowrap; \"><a class=\"label label-danger\" onclick=\"myFunctionDeletePayroll('" + item.ID + "','Payroll')\" href=\"#\"><i class=\"glyphicon glyphicon-trash\" style=\"margin-right:5px\"></i>" + Xoa + "</a></td>";
					text += "</tr>";
				}
				return text;
			}
			catch (Exception e)
			{
				WriteLog("GetPayrollDetail", MethodBase.GetCurrentMethod().Name, e, JsonConvert.SerializeObject(lstProduct));
				return "";
			}
		}

		public static string GetDelivery_Detail(List<v_ct_PhieuGiaoHang_ChiTiet> lstProduct, bool bolEdit = false)
		{
			try
			{
				string text = null;
				string text2 = null;
				if (lstProduct == null)
				{
					return "";
				}
				string text3 = "";
				foreach (v_ct_PhieuGiaoHang_ChiTiet itm in lstProduct.OrderBy((v_ct_PhieuGiaoHang_ChiTiet s) => s.ID_KHACHHANG_NCC))
				{
					if (text3 != itm.ID_KHACHHANG_NCC && lstProduct.Where((v_ct_PhieuGiaoHang_ChiTiet s) => s.ID_KHACHHANG_NCC == itm.ID_KHACHHANG_NCC).ToList().Count > 1)
					{
						text = text + "<tr data-id=\"" + itm.ID_KHACHHANG_NCC + "\" data-parent=\"0\" data-level=\"1\" id=\"" + itm.ID_KHACHHANG_NCC + "\">";
						text = text + "<td data-column=\"name\" colspan=\"3\" style=\"font-weight: bold;\">" + itm.NAME_KHACHHANG_NCC + "</td>";
						text = text + "<td style=\"font-weight: bold;text-align: right;\">" + lstProduct.Where((v_ct_PhieuGiaoHang_ChiTiet s) => s.ID_KHACHHANG_NCC == itm.ID_KHACHHANG_NCC).Sum((v_ct_PhieuGiaoHang_ChiTiet s) => s.TONGSOLUONG).ToString("N0") + "</td>";
						text = text + "<td style=\"font-weight: bold;text-align: right;\">" + lstProduct.Where((v_ct_PhieuGiaoHang_ChiTiet s) => s.ID_KHACHHANG_NCC == itm.ID_KHACHHANG_NCC).Sum((v_ct_PhieuGiaoHang_ChiTiet s) => s.TONGKHOILUONG).ToString("N0") + "</td>";
						text = text + "<td style=\"font-weight: bold;text-align: right;\">" + lstProduct.Where((v_ct_PhieuGiaoHang_ChiTiet s) => s.ID_KHACHHANG_NCC == itm.ID_KHACHHANG_NCC).Sum((v_ct_PhieuGiaoHang_ChiTiet s) => s.SOTIENGIAOHANG).ToString("N0") + "</td>";
						text += "</tr>";
					}
					text3 = itm.ID_KHACHHANG_NCC;
					string text4 = clsMaHoa.Encrypt(JsonConvert.SerializeObject(itm), "tmt6364");
					if (lstProduct.Where((v_ct_PhieuGiaoHang_ChiTiet s) => s.ID_KHACHHANG_NCC == itm.ID_KHACHHANG_NCC).ToList().Count > 1)
					{
						text = text + "<tr data-id=\"" + itm.ID_PHIEUXUAT + "\" data-parent=\"" + itm.ID_KHACHHANG_NCC + "\" data-level=\"2\" id=\"" + itm.ID_PHIEUXUAT + "\">";
						text = text + "<td style=\"white-space: nowrap; \" id=\"NGAYLAP\">" + itm.NGAYLAP.ToString("dd/MM/yyyy") + "</td>";
						text = text + "<td style=\"white-space: nowrap; \" id=\"MAPHIEU\">" + itm.MAPHIEUXUAT + " (" + ((itm.SOLAN > 0) ? itm.SOLAN : ((lstProduct.Max((v_ct_PhieuGiaoHang_ChiTiet s) => s.SOLAN) <= 0) ? 1 : (lstProduct.Max((v_ct_PhieuGiaoHang_ChiTiet s) => s.SOLAN) + 1))) + ")</td>";
						text += "<td style=\"white-space: nowrap; \" id=\"NAME_KHACHHANG_NCC\"></td>";
						text = text + "<td style=\"white-space: nowrap;text-align: right; \" id=\"TONGSOLUONG\">" + itm.TONGSOLUONG.ToString("N0") + "</td>";
						text = text + "<td style=\"white-space: nowrap;text-align: right; \" id=\"TONGKHOILUONG\">" + itm.TONGKHOILUONG.ToString("N0") + "</td>";
						text = text + "<td style=\"white-space: nowrap;text-align: right; \" id=\"SOTIENGIAOHANG\">" + itm.SOTIENGIAOHANG.ToString("N0") + "</td>";
						text = text + "<td style=\"white-space: nowrap; \"><a class=\"label label-default\" onclick=\"myFunctionViewReport('Output','" + itm.ID_PHIEUXUAT + "')\" href=\"#\"><i class=\"fa fa-print\" style=\"margin-right:5px\"></i></a></td>";
						text = text + "<td style=\"visibility: hidden; display: none;\" id=\"Detail\"><input type=\"checkbox\" class=\"form-control\" name=\"txtDetail|" + text4 + "\" id=\"Detail\" checked/></td>";
						text = text + "<td style=\"white-space: nowrap; \"><a class=\"label label-danger\" onclick=\"myFunctionDelivery('Delivery','DeleteDeliveryDetail','" + itm.ID_PHIEUXUAT + "')\" href=\"#\"><i class=\"glyphicon glyphicon-trash\" style=\"margin-right:5px\"></i>" + Xoa + "</a></td>";
						text += "</tr>";
					}
					else
					{
						text2 = text2 + "<tr data-id=\"" + itm.ID_KHACHHANG_NCC + "\" data-parent=\"0\" data-level=\"1\" id=\"" + itm.ID_KHACHHANG_NCC + "\">";
						text2 = text2 + "<td style=\"white-space: nowrap; \" id=\"NGAYLAP\">" + itm.NGAYLAP.ToString("dd/MM/yyyy") + "</td>";
						text2 = text2 + "<td style=\"white-space: nowrap; \" id=\"MAPHIEU\">" + itm.MAPHIEUXUAT + " (" + ((itm.SOLAN > 0) ? itm.SOLAN : ((lstProduct.Max((v_ct_PhieuGiaoHang_ChiTiet s) => s.SOLAN) <= 0) ? 1 : (lstProduct.Max((v_ct_PhieuGiaoHang_ChiTiet s) => s.SOLAN) + 1))) + ")</td>";
						text2 = text2 + "<td style=\"white-space: nowrap; \" id=\"NAME_KHACHHANG_NCC\">" + itm.NAME_KHACHHANG_NCC + "</td>";
						text2 = text2 + "<td style=\"white-space: nowrap;text-align: right; \" id=\"TONGSOLUONG\">" + itm.TONGSOLUONG.ToString("N0") + "</td>";
						text2 = text2 + "<td style=\"white-space: nowrap;text-align: right; \" id=\"TONGKHOILUONG\">" + itm.TONGKHOILUONG.ToString("N0") + "</td>";
						text2 = text2 + "<td style=\"white-space: nowrap;text-align: right; \" id=\"SOTIENGIAOHANG\">" + itm.SOTIENGIAOHANG.ToString("N0") + "</td>";
						text2 = text2 + "<td style=\"white-space: nowrap; \"><a class=\"label label-default\" onclick=\"myFunctionViewReport('Output','" + itm.ID_PHIEUXUAT + "')\" href=\"#\"><i class=\"fa fa-print\" style=\"margin-right:5px\"></i></a></td>";
						text2 = text2 + "<td style=\"visibility: hidden; display: none;\" id=\"Detail\"><input type=\"checkbox\" class=\"form-control\" name=\"txtDetail|" + text4 + "\" id=\"Detail\" checked/></td>";
						text2 = text2 + "<td style=\"white-space: nowrap; \"><a class=\"label label-danger\" onclick=\"myFunctionDelivery('Delivery','DeleteDeliveryDetail','" + itm.ID_PHIEUXUAT + "')\" href=\"#\"><i class=\"glyphicon glyphicon-trash\" style=\"margin-right:5px\"></i>" + Xoa + "</a></td>";
						text2 += "</tr>";
					}
				}
				text += "<tbody><tr style=\"color: red;\">";
				text += "<td colspan=\"2\" style=\"font-weight: bold;text-align: center;\">TỔNG:</td>";
				text = text + "<td style=\"font-weight: bold;text-align: right;\">" + (from s in lstProduct
																					   group s by s.ID_KHACHHANG_NCC).Count().ToString("N0") + "</td>";
				text = text + "<td style=\"font-weight: bold;text-align: right;\">" + lstProduct.Sum((v_ct_PhieuGiaoHang_ChiTiet s) => s.TONGSOLUONG).ToString("N0") + "</td>";
				text = text + "<td style=\"font-weight: bold;text-align: right;\">" + lstProduct.Sum((v_ct_PhieuGiaoHang_ChiTiet s) => s.TONGKHOILUONG).ToString("N0") + "</td>";
				text = text + "<td style=\"font-weight: bold;text-align: right;\">" + lstProduct.Sum((v_ct_PhieuGiaoHang_ChiTiet s) => s.SOTIENGIAOHANG).ToString("N0") + "</td>";
				text += "</tr></tbody>";
				return text2 + text;
			}
			catch (Exception e)
			{
				WriteLog("GetDelivery_Detail", MethodBase.GetCurrentMethod().Name, e, JsonConvert.SerializeObject(lstProduct));
				return "";
			}
		}

		public static string GetDelivery_Shipper(List<v_ct_PhieuGiaoHang_NhanVienGiao> lstProduct)
		{
			try
			{
				string text = null;
				if (lstProduct == null)
				{
					return "";
				}
				foreach (v_ct_PhieuGiaoHang_NhanVienGiao item in lstProduct)
				{
					string text2 = clsMaHoa.Encrypt(JsonConvert.SerializeObject(item), "tmt6364");
					text = text + "<tr id=\"" + item.ID_NHANVIENGIAO + "\">";
					text = text + "<td style=\"white-space: nowrap; \" id=\"MA_NHANVIEN\">" + item.MA_NHANVIEN + "</td>";
					text = text + "<td style=\"white-space: nowrap; \" id=\"NAME_NHANVIEN\">" + item.NAME_NHANVIEN + "</td>";
					text = text + "<td style=\"visibility: hidden; display: none;\" id=\"Shipper\"><input type=\"checkbox\" class=\"form-control\" name=\"txtShipper|" + text2 + "\" id=\"Shipper\" checked/></td>";
					text = text + "<td style=\"white-space: nowrap; \"><a class=\"label label-danger\" onclick=\"myFunctionDelivery('Delivery','DeleteDeliveryShipper','" + item.ID_NHANVIENGIAO + "')\" href=\"#\"><i class=\"glyphicon glyphicon-trash\" style=\"margin-right:5px\"></i>" + Xoa + "</a></td>";
				}
				return text;
			}
			catch (Exception e)
			{
				WriteLog("GetDelivery_Shipper", MethodBase.GetCurrentMethod().Name, e, JsonConvert.SerializeObject(lstProduct));
				return "";
			}
		}

		public static ApiResponse Get_DanhSachSanPhamKho<T>(string ID_KHO, bool bolTonKho, string ID_HANGHOAKHO = "", string KEY = "", string LOAITIMKIEM = "")
		{
			SP_Parameter sP_Parameter = new SP_Parameter();
			sP_Parameter.LOC_ID = LOC_ID;
			sP_Parameter.ID_KHO = ID_KHO;
			sP_Parameter.BOLTONKHO = bolTonKho;
			sP_Parameter.ID_HANGHOAKHO = ID_HANGHOAKHO;
			sP_Parameter.KEY = KEY;
			sP_Parameter.LOAITIMKIEM = LOAITIMKIEM;
			return ExecuteStoredProc<T>(sP_Parameter, "Sp_Get_DanhSachSanPhamKho");
		}

		public static ApiResponse Get_DanhSachPhieuNhap<T>(string ID_KHO, DateTime? TUNGAY, DateTime? DENNAY, string SearchString = "", string IDPHIEU = "")
		{
			ApiResponse apiResponse = new ApiResponse();
			try
			{
				SP_Parameter sP_Parameter = new SP_Parameter();
				sP_Parameter.LOC_ID = LOC_ID;
				sP_Parameter.ID_KHO = ID_KHO;
				sP_Parameter.TUNGAY = TUNGAY;
				sP_Parameter.DENNGAY = DENNAY;
				sP_Parameter.KEY = SearchString;
				sP_Parameter.ID_PHIEUNHAP = IDPHIEU;
				return (!SearchString.StartsWith("PGH")) ? ExecuteStoredProc<T>(sP_Parameter, "Sp_Get_DanhSachPhieuNhap") : ExecuteStoredProc<T>(sP_Parameter, "Sp_Get_DanhSachPhieuNhap_PhieuGiaoHang");
			}
			catch (Exception ex)
			{
				ApiResponse obj = new ApiResponse
				{
					Success = false,
					Message = ex.Message,
					Data = ""
				};
				apiResponse = obj;
				return obj;
			}
		}

		public static ApiResponse Get_DanhSachPhieuDatHangNCC<T>(string ID_KHO, DateTime? TUNGAY, DateTime? DENNAY, string SearchString = "", string IDPHIEU = "")
		{
			ApiResponse apiResponse = new ApiResponse();
			try
			{
				SP_Parameter sP_Parameter = new SP_Parameter();
				sP_Parameter.LOC_ID = LOC_ID;
				sP_Parameter.ID_KHO = ID_KHO;
				sP_Parameter.TUNGAY = TUNGAY;
				sP_Parameter.DENNGAY = DENNAY;
				sP_Parameter.KEY = SearchString;
				sP_Parameter.ID_PHIEUNHAP = IDPHIEU;
				return ExecuteStoredProc<T>(sP_Parameter, "Sp_Get_DanhSachPhieuDatHangNCC");
			}
			catch (Exception ex)
			{
				ApiResponse obj = new ApiResponse
				{
					Success = false,
					Message = ex.Message,
					Data = ""
				};
				apiResponse = obj;
				return obj;
			}
		}

		public static ApiResponse Get_DanhSachPhieuXuat<T>(string ID_KHO, DateTime? TUNGAY, DateTime? DENNAY, string SearchString = "", string IDPHIEU = "", string ID_KHUVUC = "")
		{
			ApiResponse apiResponse = new ApiResponse();
			try
			{
				SP_Parameter sP_Parameter = new SP_Parameter();
				sP_Parameter.LOC_ID = LOC_ID;
				sP_Parameter.ID_KHO = ID_KHO;
				sP_Parameter.TUNGAY = TUNGAY;
				sP_Parameter.DENNGAY = DENNAY;
				sP_Parameter.ID_KHUVUC = ID_KHUVUC;
				sP_Parameter.KEY = SearchString;
				sP_Parameter.ID_PHIEUXUAT = IDPHIEU;
				return (!SearchString.StartsWith("PGH")) ? ExecuteStoredProc<T>(sP_Parameter, "Sp_Get_DanhSachPhieuXuat") : ExecuteStoredProc<T>(sP_Parameter, "Sp_Get_DanhSachPhieuXuat_PhieuGiaoHang");
			}
			catch (Exception ex)
			{
				ApiResponse obj = new ApiResponse
				{
					Success = false,
					Message = ex.Message,
					Data = ""
				};
				apiResponse = obj;
				return obj;
			}
		}

		public static ApiResponse Get_DanhSachPhieuXuat_TimKiem<T>(string ID_KHO, DateTime? TUNGAY, DateTime? DENNAY, string SearchString = "", string TypeSearch = "", string ID_KHUVUC = "")
		{
			ApiResponse apiResponse = new ApiResponse();
			try
			{
				SP_Parameter sP_Parameter = new SP_Parameter();
				sP_Parameter.LOC_ID = LOC_ID;
				sP_Parameter.ID_KHO = ID_KHO;
				sP_Parameter.TUNGAY = TUNGAY;
				sP_Parameter.DENNGAY = DENNAY;
				sP_Parameter.ID_KHUVUC = ID_KHUVUC;
				sP_Parameter.KEY = SearchString;
				return ExecuteStoredProc<T>(sP_Parameter, "Sp_Get_DanhSachPhieuXuat_TimKiem");
			}
			catch (Exception ex)
			{
				ApiResponse obj = new ApiResponse
				{
					Success = false,
					Message = ex.Message,
					Data = ""
				};
				apiResponse = obj;
				return obj;
			}
		}

		public static ApiResponse Get_DanhSachHoaDon<T>(DateTime? TUNGAY, DateTime? DENNAY, string SearchString = "", string IDPHIEU = "")
		{
			ApiResponse apiResponse = new ApiResponse();
			try
			{
				SP_Parameter sP_Parameter = new SP_Parameter();
				sP_Parameter.LOC_ID = LOC_ID;
				sP_Parameter.TUNGAY = TUNGAY;
				sP_Parameter.DENNGAY = DENNAY;
				sP_Parameter.KEY = SearchString;
				sP_Parameter.ID_HOADON = IDPHIEU;
				return ExecuteStoredProc<T>(sP_Parameter, "Sp_Get_DanhSachHoaDon");
			}
			catch (Exception ex)
			{
				ApiResponse obj = new ApiResponse
				{
					Success = false,
					Message = ex.Message,
					Data = ""
				};
				apiResponse = obj;
				return obj;
			}
		}

		public static ApiResponse Get_DanhSachPhieuThu<T>(string ID_KHO, DateTime? TUNGAY, DateTime? DENNAY, string SearchString = "", string IDPHIEU = "")
		{
			ApiResponse apiResponse = new ApiResponse();
			try
			{
				SP_Parameter sP_Parameter = new SP_Parameter();
				sP_Parameter.LOC_ID = LOC_ID;
				sP_Parameter.ID_KHO = ID_KHO;
				sP_Parameter.TUNGAY = TUNGAY;
				sP_Parameter.DENNGAY = DENNAY;
				sP_Parameter.KEY = SearchString;
				sP_Parameter.ID_PHIEUTHU = IDPHIEU;
				return (!SearchString.StartsWith("PGH")) ? ExecuteStoredProc<T>(sP_Parameter, "Sp_Get_DanhSachPhieuThu") : ExecuteStoredProc<T>(sP_Parameter, "Sp_Get_DanhSachPhieuThu_PhieuGiaoHang");
			}
			catch (Exception ex)
			{
				ApiResponse obj = new ApiResponse
				{
					Success = false,
					Message = ex.Message,
					Data = ""
				};
				apiResponse = obj;
				return obj;
			}
		}

		public static ApiResponse Get_DanhSachPhieuChi<T>(string ID_KHO, DateTime? TUNGAY, DateTime? DENNAY, string SearchString = "", string IDPHIEU = "")
		{
			ApiResponse apiResponse = new ApiResponse();
			try
			{
				SP_Parameter sP_Parameter = new SP_Parameter();
				sP_Parameter.LOC_ID = LOC_ID;
				sP_Parameter.ID_KHO = ID_KHO;
				sP_Parameter.TUNGAY = TUNGAY;
				sP_Parameter.DENNGAY = DENNAY;
				sP_Parameter.KEY = SearchString;
				sP_Parameter.ID_PHIEUCHI = IDPHIEU;
				return (!SearchString.StartsWith("PGH")) ? ExecuteStoredProc<T>(sP_Parameter, "Sp_Get_DanhSachPhieuChi") : ExecuteStoredProc<T>(sP_Parameter, "Sp_Get_DanhSachPhieuChi_PhieuGiaoHang");
			}
			catch (Exception ex)
			{
				ApiResponse obj = new ApiResponse
				{
					Success = false,
					Message = ex.Message,
					Data = ""
				};
				apiResponse = obj;
				return obj;
			}
		}

		public static ApiResponse Get_DanhSachPhieuLuong<T>(DateTime? TUNGAY, DateTime? DENNAY, string SearchString = "", string IDPHIEU = "")
		{
			ApiResponse apiResponse = new ApiResponse();
			try
			{
				SP_Parameter sP_Parameter = new SP_Parameter();
				sP_Parameter.LOC_ID = LOC_ID;
				sP_Parameter.TUNGAY = TUNGAY;
				sP_Parameter.DENNGAY = DENNAY;
				sP_Parameter.KEY = SearchString;
				return ExecuteStoredProc<T>(sP_Parameter, "Sp_Get_DanhSachBangLuong");
			}
			catch (Exception ex)
			{
				ApiResponse obj = new ApiResponse
				{
					Success = false,
					Message = ex.Message,
					Data = ""
				};
				apiResponse = obj;
				return obj;
			}
		}

		public static ApiResponse Get_DanhSachPhieuDatHang<T>(string ID_KHO, DateTime? TUNGAY, DateTime? DENNAY, string SearchString = "", string IDPHIEU = "", string ID_KHUVUC = "")
		{
			ApiResponse apiResponse = new ApiResponse();
			try
			{
				SP_Parameter sP_Parameter = new SP_Parameter();
				sP_Parameter.LOC_ID = LOC_ID;
				sP_Parameter.ID_KHO = ID_KHO;
				sP_Parameter.TUNGAY = TUNGAY;
				sP_Parameter.DENNGAY = DENNAY;
				sP_Parameter.KEY = SearchString;
				sP_Parameter.ID_PHIEUDATHANG = IDPHIEU;
				sP_Parameter.ID_KHUVUC = ID_KHUVUC;
				return ExecuteStoredProc<T>(sP_Parameter, "Sp_Get_DanhSachPhieuDatHang");
			}
			catch (Exception ex)
			{
				ApiResponse obj = new ApiResponse
				{
					Success = false,
					Message = ex.Message,
					Data = ""
				};
				apiResponse = obj;
				return obj;
			}
		}

		public static ApiResponse Get_DanhSachPhieuGiaoHang<T>(string ID_KHO, DateTime? TUNGAY, DateTime? DENNAY, string SearchString = "", string TypeSearch = "")
		{
			ApiResponse apiResponse = new ApiResponse();
			try
			{
				SP_Parameter sP_Parameter = new SP_Parameter();
				sP_Parameter.LOC_ID = LOC_ID;
				sP_Parameter.TUNGAY = TUNGAY;
				sP_Parameter.DENNGAY = DENNAY;
				sP_Parameter.KEY = SearchString;
				return ExecuteStoredProc<T>(sP_Parameter, "Sp_Get_DanhSachPhieuGiaoHang");
			}
			catch (Exception ex)
			{
				ApiResponse obj = new ApiResponse
				{
					Success = false,
					Message = ex.Message,
					Data = ""
				};
				apiResponse = obj;
				return obj;
			}
		}

		public static ApiResponse Get_DanhSachPhieuXuat_ChiTiet<T>(SP_Parameter objParameter)
		{
			ApiResponse apiResponse = new ApiResponse();
			try
			{
				return ExecuteStoredProc<T>(objParameter, "Sp_Get_DanhSachPhieuXuat_ChiTiet_BC");
			}
			catch (Exception ex)
			{
				ApiResponse obj = new ApiResponse
				{
					Success = false,
					Message = ex.Message,
					Data = ""
				};
				apiResponse = obj;
				return obj;
			}
		}

		public static ApiResponse Get_ThongKeCongNoKhachHang<T>(SP_Parameter objParameter)
		{
			ApiResponse apiResponse = new ApiResponse();
			try
			{
				return ExecuteStoredProc<T>(objParameter, "Sp_Get_ThongKeCongNoKhachHang");
			}
			catch (Exception ex)
			{
				ApiResponse obj = new ApiResponse
				{
					Success = false,
					Message = ex.Message,
					Data = ""
				};
				apiResponse = obj;
				return obj;
			}
		}

		public static ApiResponse Get_ThongKeCongNoNhaCungCap<T>(SP_Parameter objParameter)
		{
			ApiResponse apiResponse = new ApiResponse();
			try
			{
				return ExecuteStoredProc<T>(objParameter, "Sp_Get_ThongKeCongNoNhaCungCap");
			}
			catch (Exception ex)
			{
				ApiResponse obj = new ApiResponse
				{
					Success = false,
					Message = ex.Message,
					Data = ""
				};
				apiResponse = obj;
				return obj;
			}
		}

		public static ApiResponse Get_ThongKeCongNoNhanVien<T>(SP_Parameter objParameter)
		{
			ApiResponse apiResponse = new ApiResponse();
			try
			{
				return ExecuteStoredProc<T>(objParameter, "Sp_Get_ThongKeCongNoNhanVien");
			}
			catch (Exception ex)
			{
				ApiResponse obj = new ApiResponse
				{
					Success = false,
					Message = ex.Message,
					Data = ""
				};
				apiResponse = obj;
				return obj;
			}
		}

		public static ApiResponse Get_ThongKeTonKhoHangHoa<T>(SP_Parameter objParameter)
		{
			ApiResponse apiResponse = new ApiResponse();
			try
			{
				return ExecuteStoredProc<T>(objParameter, "Sp_Get_ThongKeTonKhoHangHoa");
			}
			catch (Exception ex)
			{
				ApiResponse obj = new ApiResponse
				{
					Success = false,
					Message = ex.Message,
					Data = ""
				};
				apiResponse = obj;
				return obj;
			}
		}

		public static ApiResponse Get_ThongKeQuyTien<T>(SP_Parameter objParameter)
		{
			ApiResponse apiResponse = new ApiResponse();
			try
			{
				return ExecuteStoredProc<T>(objParameter, "Sp_Get_ThongKeQuyTien");
			}
			catch (Exception ex)
			{
				ApiResponse obj = new ApiResponse
				{
					Success = false,
					Message = ex.Message,
					Data = ""
				};
				apiResponse = obj;
				return obj;
			}
		}

		public static ApiResponse GetMoneyFundDetail<T>(v_ThongKeQuyTien model, string name = "Books")
		{
			ApiResponse apiResponse = new ApiResponse();
			HttpResponseMessage httpResponseMessage = null;
			StringContent stringContent = null;
			List<T> data = new List<T>();
			string text = "";
			try
			{
				text = JsonConvert.SerializeObject(model);
				stringContent = new StringContent(text, Encoding.UTF8, "application/json");
				HttpClient httpClient = new HttpClient();
				httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + Token);
				httpResponseMessage = httpClient.PostAsync(URL + name, stringContent).Result;
				if (httpResponseMessage.IsSuccessStatusCode)
				{
					string result = httpResponseMessage.Content.ReadAsStringAsync().Result;
					apiResponse = JsonConvert.DeserializeObject<ApiResponse>(result);
					if (apiResponse.Data != null)
					{
						data = JsonConvert.DeserializeObject<List<T>>(apiResponse.Data.ToString());
					}
					apiResponse.Data = data;
				}
				else
				{
					apiResponse.Message = GetErrorServer(httpResponseMessage);
				}
			}
			catch (Exception ex)
			{
				WriteLog((httpResponseMessage != null) ? httpResponseMessage.RequestMessage.RequestUri.ToString() : "GetDebtDetail", MethodBase.GetCurrentMethod().Name, ex, text);
				apiResponse = new ApiResponse
				{
					Success = false,
					Message = ex.Message,
					Data = ""
				};
			}
			return apiResponse;
		}

		public static ApiResponse Get_ThongKeBaoCaoNhanVien<T>(SP_Parameter objParameter)
		{
			ApiResponse apiResponse = new ApiResponse();
			try
			{
				return ExecuteStoredProc<T>(objParameter, "Sp_Get_BaoCaoTheoNhanVien");
			}
			catch (Exception ex)
			{
				ApiResponse obj = new ApiResponse
				{
					Success = false,
					Message = ex.Message,
					Data = ""
				};
				apiResponse = obj;
				return obj;
			}
		}

		public static ApiResponse GetReportEmployeeDetail<T>(Sp_Get_BaoCaoTheoNhanVien_Result model, string name = "Books")
		{
			ApiResponse apiResponse = new ApiResponse();
			HttpResponseMessage httpResponseMessage = null;
			StringContent stringContent = null;
			List<T> data = new List<T>();
			string text = "";
			try
			{
				text = JsonConvert.SerializeObject(model);
				stringContent = new StringContent(text, Encoding.UTF8, "application/json");
				HttpClient httpClient = new HttpClient();
				httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + Token);
				httpResponseMessage = httpClient.PostAsync(URL + name, stringContent).Result;
				if (httpResponseMessage.IsSuccessStatusCode)
				{
					string result = httpResponseMessage.Content.ReadAsStringAsync().Result;
					apiResponse = JsonConvert.DeserializeObject<ApiResponse>(result);
					if (apiResponse.Data != null)
					{
						data = JsonConvert.DeserializeObject<List<T>>(apiResponse.Data.ToString());
					}
					apiResponse.Data = data;
				}
				else
				{
					apiResponse.Message = GetErrorServer(httpResponseMessage);
				}
			}
			catch (Exception ex)
			{
				WriteLog((httpResponseMessage != null) ? httpResponseMessage.RequestMessage.RequestUri.ToString() : "GetDebtDetail", MethodBase.GetCurrentMethod().Name, ex, text);
				apiResponse = new ApiResponse
				{
					Success = false,
					Message = ex.Message,
					Data = ""
				};
			}
			return apiResponse;
		}

		public static ApiResponse Get_ThongKeThuChi<T>(SP_Parameter_Report objParameter, string Name = "Sp_Get_ThongKeThuChi_GroupBy")
		{
			ApiResponse apiResponse = new ApiResponse();
			try
			{
				return ExecuteStoredProcT<T>(objParameter, Name);
			}
			catch (Exception ex)
			{
				ApiResponse obj = new ApiResponse
				{
					Success = false,
					Message = ex.Message,
					Data = ""
				};
				apiResponse = obj;
				return obj;
			}
		}

		public static void TinhTong(Product_Detail Product_Detail, string VALUE = null, List<Product_Detail> lstProduct = null)
		{
			if (Product_Detail.TYPE == "ID_THUESUAT")
			{
				if (VALUE != null)
				{
					Product_Detail.ID_THUESUAT = VALUE;
				}
				if (string.IsNullOrEmpty(Product_Detail.ID_THUESUAT))
				{
					Product_Detail.THUESUAT = 0.0;
					Product_Detail.TONGTIENVAT = 0.0;
					Product_Detail.TONGCONG = Product_Detail.THANHTIEN + Product_Detail.TONGTIENVAT;
				}
				else
				{
					ApiResponse detail = GetDetail<v_v_dm_ThueSuat>(LOC_ID + "/" + Product_Detail.ID_THUESUAT, "Tax");
					if (detail.Data != null && detail.Data is v_v_dm_ThueSuat v_v_dm_ThueSuat2)
					{
						Product_Detail.THUESUAT = v_v_dm_ThueSuat2.THUESUAT;
						Product_Detail.TONGTIENVAT = Product_Detail.THANHTIEN * Product_Detail.THUESUAT / 100.0;
						Product_Detail.TONGCONG = Product_Detail.THANHTIEN + Product_Detail.TONGTIENVAT;
					}
				}
			}
			else if (Product_Detail.TYPE == "SOLUONG")
			{
				if (!string.IsNullOrEmpty(VALUE))
				{
					Product_Detail.SOLUONG = Math.Round(ConvertStringToDouble(VALUE, bolForm: false), TongCongKhac);
				}
				List<v_ChiTietHoaDon> list = new List<v_ChiTietHoaDon>();
				if (lstProduct != null)
				{
					foreach (Product_Detail item in lstProduct)
					{
						v_ChiTietHoaDon v_ChiTietHoaDon2 = new v_ChiTietHoaDon();
						v_ChiTietHoaDon2.ID_HANGHOA = item.ID_HANGHOA;
						v_ChiTietHoaDon2.SOLUONG = item.SOLUONG;
						v_ChiTietHoaDon2.ID_DVT = item.ID_DVT;
						v_ChiTietHoaDon2.DONGIA = item.DONGIA;
						list.Add(v_ChiTietHoaDon2);
					}
					List<v_ChiTietHoaDon> list2 = LayDonGia_KhungGia(Product_Detail.ID_HANGHOA, Product_Detail.ID_DVT, Product_Detail.SOLUONG, list);
					if (list2 != null && list2.Count > 0)
					{
						foreach (v_ChiTietHoaDon itm in list2)
						{
							IEnumerable<Product_Detail> enumerable = lstProduct.Where((Product_Detail s) => s.ID_HANGHOA == itm.ID_HANGHOA && s.ID_DVT == itm.ID_DVT);
							if (enumerable == null)
							{
								continue;
							}
							foreach (Product_Detail item2 in enumerable)
							{
								item2.DONGIA = ((itm.DONGIAMOI != 0.0) ? itm.DONGIAMOI : item2.DONGIA);
							}
						}
					}
				}
				Product_Detail.TONGTIENGIAMGIA = Math.Round(Product_Detail.SOLUONG * Product_Detail.DONGIA * Product_Detail.CHIETKHAU / 100.0, SoNguyenTongCong);
				Product_Detail.THANHTIEN = Math.Round(Product_Detail.SOLUONG * Product_Detail.DONGIA - Product_Detail.TONGTIENGIAMGIA, SoNguyenTongCong);
				Product_Detail.TONGTIENVAT = Math.Round(Product_Detail.THANHTIEN * Product_Detail.THUESUAT / 100.0, SoNguyenTongCong);
				Product_Detail.TONGCONG = Math.Round(Product_Detail.THANHTIEN + Product_Detail.TONGTIENVAT, SoNguyenTongCong);
			}
			else if (Product_Detail.TYPE == "DONGIA")
			{
				if (!string.IsNullOrEmpty(VALUE))
				{
					Product_Detail.DONGIA = Math.Round(ConvertStringToDouble(VALUE, bolForm: false), TongCongKhac);
				}
				Product_Detail.TONGTIENGIAMGIA = Math.Round(Product_Detail.SOLUONG * Product_Detail.DONGIA * Product_Detail.CHIETKHAU / 100.0, SoNguyenTongCong);
				Product_Detail.THANHTIEN = Math.Round(Product_Detail.SOLUONG * Product_Detail.DONGIA - Product_Detail.TONGTIENGIAMGIA, SoNguyenTongCong);
				Product_Detail.TONGTIENVAT = Math.Round(Product_Detail.THANHTIEN * Product_Detail.THUESUAT / 100.0, SoNguyenTongCong);
				Product_Detail.TONGCONG = Math.Round(Product_Detail.THANHTIEN + Product_Detail.TONGTIENVAT, SoNguyenTongCong);
			}
			else if (Product_Detail.TYPE == "TONGTIENGIAMGIA")
			{
				if (!string.IsNullOrEmpty(VALUE))
				{
					Product_Detail.TONGTIENGIAMGIA = Math.Round(ConvertStringToDouble(VALUE, bolForm: false), SoNguyenTongCong);
				}
				Product_Detail.THANHTIEN = Math.Round(Product_Detail.SOLUONG * Product_Detail.DONGIA - Product_Detail.TONGTIENGIAMGIA, SoNguyenTongCong);
				Product_Detail.TONGTIENVAT = Math.Round(Product_Detail.THANHTIEN * Product_Detail.THUESUAT / 100.0, SoNguyenTongCong);
				Product_Detail.TONGCONG = Math.Round(Product_Detail.THANHTIEN + Product_Detail.TONGTIENVAT, SoNguyenTongCong);
			}
			else if (Product_Detail.TYPE == "CHIETKHAU")
			{
				if (!string.IsNullOrEmpty(VALUE))
				{
					Product_Detail.CHIETKHAU = Math.Round(ConvertStringToDouble(VALUE, bolForm: false), 1);
				}
				Product_Detail.TONGTIENGIAMGIA = Math.Round(Product_Detail.SOLUONG * Product_Detail.DONGIA * Product_Detail.CHIETKHAU / 100.0, SoNguyenTongCong);
				Product_Detail.THANHTIEN = Math.Round(Product_Detail.SOLUONG * Product_Detail.DONGIA - Product_Detail.TONGTIENGIAMGIA, SoNguyenTongCong);
				Product_Detail.TONGTIENVAT = Math.Round(Product_Detail.THANHTIEN * Product_Detail.THUESUAT / 100.0, SoNguyenTongCong);
				Product_Detail.TONGCONG = Math.Round(Product_Detail.THANHTIEN + Product_Detail.TONGTIENVAT, SoNguyenTongCong);
			}
			else if (Product_Detail.TYPE == "TONGTIENVAT")
			{
				if (!string.IsNullOrEmpty(VALUE))
				{
					Product_Detail.TONGTIENVAT = Math.Round(ConvertStringToDouble(VALUE, bolForm: false), SoNguyenTongCong);
				}
				Product_Detail.TONGCONG = Math.Round(Product_Detail.THANHTIEN + Product_Detail.TONGTIENVAT, SoNguyenTongCong);
			}
			else if (Product_Detail.TYPE == "THANHTIEN")
			{
				if (!string.IsNullOrEmpty(VALUE))
				{
					Product_Detail.THANHTIEN = Math.Round(ConvertStringToDouble(VALUE, bolForm: false), SoNguyenTongCong);
				}
				Product_Detail.TONGTIENVAT = Math.Round(Product_Detail.THANHTIEN * Product_Detail.THUESUAT / 100.0, SoNguyenTongCong);
				Product_Detail.TONGCONG = Math.Round(Product_Detail.THANHTIEN + Product_Detail.TONGTIENVAT, SoNguyenTongCong);
			}
			else if (Product_Detail.TYPE == "DONGIA")
			{
				if (!string.IsNullOrEmpty(VALUE))
				{
					Product_Detail.DONGIA = Math.Round(ConvertStringToDouble(VALUE, bolForm: false), TongCongKhac);
				}
				Product_Detail.TONGTIENGIAMGIA = Math.Round(Product_Detail.SOLUONG * Product_Detail.DONGIA * Product_Detail.CHIETKHAU / 100.0, SoNguyenTongCong);
				Product_Detail.THANHTIEN = Math.Round(Product_Detail.SOLUONG * Product_Detail.DONGIA - Product_Detail.TONGTIENGIAMGIA, SoNguyenTongCong);
				Product_Detail.TONGTIENVAT = Math.Round(Product_Detail.THANHTIEN * Product_Detail.THUESUAT / 100.0, SoNguyenTongCong);
				Product_Detail.TONGCONG = Math.Round(Product_Detail.THANHTIEN + Product_Detail.TONGTIENVAT, SoNguyenTongCong);
			}
			else if (Product_Detail.TYPE == "TONGCONG")
			{
				if (!string.IsNullOrEmpty(VALUE))
				{
					Product_Detail.TONGCONG = Math.Round(ConvertStringToDouble(VALUE, bolForm: false), SoNguyenTongCong);
				}
			}
			else
			{
				Product_Detail.TONGTIENGIAMGIA = Math.Round(Product_Detail.SOLUONG * Product_Detail.DONGIA * Product_Detail.CHIETKHAU / 100.0, SoNguyenTongCong);
				Product_Detail.THANHTIEN = Math.Round(Product_Detail.SOLUONG * Product_Detail.DONGIA - Product_Detail.TONGTIENGIAMGIA, SoNguyenTongCong);
				Product_Detail.TONGTIENVAT = Math.Round(Product_Detail.THANHTIEN * Product_Detail.THUESUAT / 100.0, SoNguyenTongCong);
				Product_Detail.TONGCONG = Math.Round(Product_Detail.THANHTIEN + Product_Detail.TONGTIENVAT, SoNguyenTongCong);
			}
			if (string.IsNullOrEmpty(Product_Detail.ID_COMBO))
			{
				return;
			}
			IEnumerable<Product_Detail> enumerable2 = lstProduct.Where((Product_Detail e) => e.ID_COMBO == Product_Detail.ID_COMBO && e.ISCOMBO);
			foreach (Product_Detail item3 in enumerable2)
			{
				item3.ID_DVT = item3.ID_DVT_COMBO;
				item3.SOLUONG = Product_Detail.SOLUONG * item3.QTY_COMBO;
				item3.TYLE_QD = item3.TYLE_QD_COMBO;
				item3.TONGSOLUONG = Product_Detail.SOLUONG * item3.QTY_TOTAL_COMBO;
				item3.DONGIA = 0.0;
				item3.ISCOMBO = true;
				item3.ID_COMBO = Product_Detail.ID_HANGHOA;
			}
		}

		public static void TinhTongVAT(Product_Detail Product_Detail, string VALUE = null, List<Product_Detail> lstProduct = null)
		{
			if (Product_Detail.TYPE == "MAHANGHOA")
			{
				Product_Detail.MAHANGHOA = VALUE;
			}
			else if (Product_Detail.TYPE == "TENHANGHOA")
			{
				Product_Detail.TENHANGHOA = VALUE;
			}
			else if (Product_Detail.TYPE == "DVT")
			{
				Product_Detail.DVT = VALUE;
			}
			else if (Product_Detail.TYPE == "TINHCHAT")
			{
				Product_Detail.TINHCHAT = Convert.ToInt16(VALUE);
				if (Product_Detail.TINHCHAT == 3 && Product_Detail.THANHTIEN < 0.0)
				{
					Product_Detail.THANHTIEN = Math.Round(Product_Detail.SOLUONG * Product_Detail.DONGIA - Product_Detail.TONGTIENGIAMGIA, SoNguyenTongCong);
				}
				if (Product_Detail.TINHCHAT == 3)
				{
					Product_Detail.CHIETKHAU = 0.0;
					Product_Detail.TONGTIENGIAMGIA = 0.0;
				}
				Product_Detail.TONGTIENVAT = Math.Round(Product_Detail.THANHTIEN * Product_Detail.THUESUAT / 100.0, SoNguyenTongCong);
				Product_Detail.TONGCONG = Math.Round(Product_Detail.THANHTIEN + Product_Detail.TONGTIENVAT, SoNguyenTongCong);
			}
			else if (Product_Detail.TYPE == "ID_THUESUAT")
			{
				if (VALUE != null)
				{
					Product_Detail.ID_THUESUAT = VALUE;
				}
				if (string.IsNullOrEmpty(Product_Detail.ID_THUESUAT))
				{
					Product_Detail.THUESUAT = 0.0;
					Product_Detail.TONGTIENVAT = 0.0;
					Product_Detail.TONGCONG = Product_Detail.THANHTIEN + Product_Detail.TONGTIENVAT;
					return;
				}
				ApiResponse detail = GetDetail<v_v_dm_ThueSuat>(LOC_ID + "/" + Product_Detail.ID_THUESUAT, "Tax");
				if (detail.Data != null && detail.Data is v_v_dm_ThueSuat v_v_dm_ThueSuat2)
				{
					Product_Detail.THUESUAT = v_v_dm_ThueSuat2.THUESUAT;
					Product_Detail.TONGTIENVAT = Product_Detail.THANHTIEN * Product_Detail.THUESUAT / 100.0;
					Product_Detail.TONGCONG = Product_Detail.THANHTIEN + Product_Detail.TONGTIENVAT;
				}
			}
			else if (Product_Detail.TYPE == "SOLUONG")
			{
				if (!string.IsNullOrEmpty(VALUE))
				{
					Product_Detail.SOLUONG = Math.Round(ConvertStringToDouble(VALUE, bolForm: false), TongCongKhac);
				}
				Product_Detail.TONGTIENGIAMGIA = Math.Round(Product_Detail.SOLUONG * Product_Detail.DONGIA * Product_Detail.CHIETKHAU / 100.0, SoNguyenTongCong);
				Product_Detail.THANHTIEN = Math.Round(Product_Detail.SOLUONG * Product_Detail.DONGIA - Product_Detail.TONGTIENGIAMGIA, SoNguyenTongCong);
				if (Product_Detail.TINHCHAT == 3 && Product_Detail.THANHTIEN < 0.0)
				{
					Product_Detail.THANHTIEN *= -1.0;
				}
				if (Product_Detail.TINHCHAT == 3)
				{
					Product_Detail.CHIETKHAU = 0.0;
					Product_Detail.TONGTIENGIAMGIA = 0.0;
				}
				Product_Detail.TONGTIENVAT = Math.Round(Product_Detail.THANHTIEN * Product_Detail.THUESUAT / 100.0, SoNguyenTongCong);
				Product_Detail.TONGCONG = Math.Round(Product_Detail.THANHTIEN + Product_Detail.TONGTIENVAT, SoNguyenTongCong);
			}
			else if (Product_Detail.TYPE == "DONGIA")
			{
				if (!string.IsNullOrEmpty(VALUE))
				{
					Product_Detail.DONGIA = Math.Round(ConvertStringToDouble(VALUE, bolForm: false), TongCongKhac);
				}
				Product_Detail.TONGTIENGIAMGIA = Math.Round(Product_Detail.SOLUONG * Product_Detail.DONGIA * Product_Detail.CHIETKHAU / 100.0, SoNguyenTongCong);
				Product_Detail.THANHTIEN = Math.Round(Product_Detail.SOLUONG * Product_Detail.DONGIA - Product_Detail.TONGTIENGIAMGIA, SoNguyenTongCong);
				if (Product_Detail.TINHCHAT == 3 && Product_Detail.THANHTIEN < 0.0)
				{
					Product_Detail.THANHTIEN *= -1.0;
				}
				if (Product_Detail.TINHCHAT == 3)
				{
					Product_Detail.CHIETKHAU = 0.0;
					Product_Detail.TONGTIENGIAMGIA = 0.0;
				}
				Product_Detail.TONGTIENVAT = Math.Round(Product_Detail.THANHTIEN * Product_Detail.THUESUAT / 100.0, SoNguyenTongCong);
				Product_Detail.TONGCONG = Math.Round(Product_Detail.THANHTIEN + Product_Detail.TONGTIENVAT, SoNguyenTongCong);
			}
			else if (Product_Detail.TYPE == "TONGTIENGIAMGIA")
			{
				if (!string.IsNullOrEmpty(VALUE))
				{
					Product_Detail.TONGTIENGIAMGIA = Math.Round(ConvertStringToDouble(VALUE, bolForm: false), SoNguyenTongCong);
				}
				Product_Detail.THANHTIEN = Math.Round(Product_Detail.SOLUONG * Product_Detail.DONGIA - Product_Detail.TONGTIENGIAMGIA, SoNguyenTongCong);
				if (Product_Detail.TINHCHAT == 3 && Product_Detail.THANHTIEN < 0.0)
				{
					Product_Detail.THANHTIEN *= -1.0;
				}
				if (Product_Detail.TINHCHAT == 3)
				{
					Product_Detail.CHIETKHAU = 0.0;
					Product_Detail.TONGTIENGIAMGIA = 0.0;
				}
				Product_Detail.TONGTIENVAT = Math.Round(Product_Detail.THANHTIEN * Product_Detail.THUESUAT / 100.0, SoNguyenTongCong);
				Product_Detail.TONGCONG = Math.Round(Product_Detail.THANHTIEN + Product_Detail.TONGTIENVAT, SoNguyenTongCong);
			}
			else if (Product_Detail.TYPE == "CHIETKHAU")
			{
				if (!string.IsNullOrEmpty(VALUE))
				{
					Product_Detail.CHIETKHAU = Math.Round(ConvertStringToDouble(VALUE, bolForm: false), 1);
				}
				Product_Detail.TONGTIENGIAMGIA = Math.Round(Product_Detail.SOLUONG * Product_Detail.DONGIA * Product_Detail.CHIETKHAU / 100.0, SoNguyenTongCong);
				Product_Detail.THANHTIEN = Math.Round(Product_Detail.SOLUONG * Product_Detail.DONGIA - Product_Detail.TONGTIENGIAMGIA, SoNguyenTongCong);
				if (Product_Detail.TINHCHAT == 3 && Product_Detail.THANHTIEN < 0.0)
				{
					Product_Detail.THANHTIEN *= -1.0;
				}
				if (Product_Detail.TINHCHAT == 3)
				{
					Product_Detail.CHIETKHAU = 0.0;
					Product_Detail.TONGTIENGIAMGIA = 0.0;
				}
				Product_Detail.TONGTIENVAT = Math.Round(Product_Detail.THANHTIEN * Product_Detail.THUESUAT / 100.0, SoNguyenTongCong);
				Product_Detail.TONGCONG = Math.Round(Product_Detail.THANHTIEN + Product_Detail.TONGTIENVAT, SoNguyenTongCong);
			}
			else if (Product_Detail.TYPE == "TONGTIENVAT")
			{
				if (!string.IsNullOrEmpty(VALUE))
				{
					Product_Detail.TONGTIENVAT = Math.Round(ConvertStringToDouble(VALUE, bolForm: false), SoNguyenTongCong);
				}
				Product_Detail.TONGCONG = Math.Round(Product_Detail.THANHTIEN + Product_Detail.TONGTIENVAT, SoNguyenTongCong);
			}
			else if (Product_Detail.TYPE == "THANHTIEN")
			{
				if (!string.IsNullOrEmpty(VALUE))
				{
					Product_Detail.THANHTIEN = Math.Round(ConvertStringToDouble(VALUE, bolForm: false), SoNguyenTongCong);
				}
				if (Product_Detail.TINHCHAT == 3 && Product_Detail.THANHTIEN < 0.0)
				{
					Product_Detail.THANHTIEN *= -1.0;
				}
				if (Product_Detail.TINHCHAT == 3)
				{
					Product_Detail.CHIETKHAU = 0.0;
					Product_Detail.TONGTIENGIAMGIA = 0.0;
				}
				Product_Detail.TONGTIENVAT = Math.Round(Product_Detail.THANHTIEN * Product_Detail.THUESUAT / 100.0, SoNguyenTongCong);
				Product_Detail.TONGCONG = Math.Round(Product_Detail.THANHTIEN + Product_Detail.TONGTIENVAT, SoNguyenTongCong);
			}
			else if (Product_Detail.TYPE == "TONGCONG")
			{
				if (!string.IsNullOrEmpty(VALUE))
				{
					Product_Detail.TONGCONG = Math.Round(ConvertStringToDouble(VALUE, bolForm: false), SoNguyenTongCong);
				}
			}
			else
			{
				Product_Detail.TONGTIENGIAMGIA = Math.Round(Product_Detail.SOLUONG * Product_Detail.DONGIA * Product_Detail.CHIETKHAU / 100.0, SoNguyenTongCong);
				if (Product_Detail.TINHCHAT == 3 && Product_Detail.THANHTIEN < 0.0)
				{
					Product_Detail.THANHTIEN *= -1.0;
				}
				if (Product_Detail.TINHCHAT == 3)
				{
					Product_Detail.CHIETKHAU = 0.0;
					Product_Detail.TONGTIENGIAMGIA = 0.0;
				}
				Product_Detail.THANHTIEN = Math.Round(Product_Detail.SOLUONG * Product_Detail.DONGIA - Product_Detail.TONGTIENGIAMGIA, SoNguyenTongCong);
				Product_Detail.TONGTIENVAT = Math.Round(Product_Detail.THANHTIEN * Product_Detail.THUESUAT / 100.0, SoNguyenTongCong);
				Product_Detail.TONGCONG = Math.Round(Product_Detail.THANHTIEN + Product_Detail.TONGTIENVAT, SoNguyenTongCong);
			}
		}

		public static string DocTienBangChu(long SoTien, string strTail = " đồng")
		{
			string text = "";
			string text2 = "";
			int[] array = new int[6];
			if (SoTien < 0)
			{
				return "Số tiền âm !";
			}
			if (SoTien == 0)
			{
				return "Không đồng !";
			}
			long num = ((SoTien <= 0) ? (-SoTien) : SoTien);
			if (SoTien > 8999999999999999L)
			{
				SoTien = 0L;
				return "";
			}
			array[5] = (int)(num / 1000000000000000L);
			num -= long.Parse(array[5].ToString()) * 1000000000000000L;
			array[4] = (int)(num / 1000000000000L);
			num -= long.Parse(array[4].ToString()) * 1000000000000L;
			array[3] = (int)(num / 1000000000);
			num -= long.Parse(array[3].ToString()) * 1000000000;
			array[2] = (int)(num / 1000000);
			array[1] = (int)(num % 1000000 / 1000);
			array[0] = (int)(num % 1000);
			int num2 = ((array[5] > 0) ? 5 : ((array[4] > 0) ? 4 : ((array[3] > 0) ? 3 : ((array[2] > 0) ? 2 : ((array[1] > 0) ? 1 : 0)))));
			for (int num3 = num2; num3 >= 0; num3--)
			{
				text2 = DocSo3ChuSo(array[num3]);
				text += text2;
				if (array[num3] != 0)
				{
					text += Tien[num3];
				}
				if (num3 > 0 && !string.IsNullOrEmpty(text2))
				{
					text = text ?? "";
				}
			}
			if (text.Substring(text.Length - 1, 1) == ",")
			{
				text = text.Substring(0, text.Length - 1);
			}
			text = text.Trim() + strTail;
			return text.Substring(0, 1).ToUpper() + text.Substring(1);
		}

		private static string DocSo3ChuSo(int baso)
		{
			string text = "";
			int num = baso / 100;
			int num2 = baso % 100 / 10;
			int num3 = baso % 10;
			if (num == 0 && num2 == 0 && num3 == 0)
			{
				return "";
			}
			if (num != 0)
			{
				text = text + ChuSo[num] + " trăm";
				if (num2 == 0 && num3 != 0)
				{
					text += " linh";
				}
			}
			if (num2 != 0 && num2 != 1)
			{
				text = text + ChuSo[num2] + " mươi";
				if (num2 == 0 && num3 != 0)
				{
					text += " linh";
				}
			}
			if (num2 == 1)
			{
				text += " mười";
			}
			switch (num3)
			{
				case 1:
					text = ((num2 == 0 || num2 == 1) ? (text + ChuSo[num3]) : (text + " mốt"));
					break;
				case 5:
					text = ((num2 != 0) ? (text + " lăm") : (text + ChuSo[num3]));
					break;
				default:
					if (num3 != 0)
					{
						text += ChuSo[num3];
					}
					break;
			}
			return text;
		}

		public static ReportClass GetFormulaFields(ReportClass report, object Master = null, string MapPath = "")
		{
			if (Master == null)
			{
				return report;
			}
			ApiResponse apiResponse = new ApiResponse();
			v_v_dm_CongTy v_v_dm_CongTy2 = new v_v_dm_CongTy();
			apiResponse = GetDetail<v_v_dm_CongTy>(LOC_ID, "Company");
			if (apiResponse.Data != null)
			{
				v_v_dm_CongTy2 = apiResponse.Data as v_v_dm_CongTy;
			}
			switch (Master.GetType().Name)
			{
				case "v_ct_PhieuChi":
					{
						report.FileName = HostingEnvironment.MapPath("~/Report/rptPhieuChi.rpt");
						report.Load();
						v_ct_PhieuChi v_ct_PhieuChi2 = (v_ct_PhieuChi)Master;
						report.DataDefinition.FormulaFields["NGAYLAP"].Text = "'Ngày " + v_ct_PhieuChi2.NGAYLAP.Day + " tháng " + v_ct_PhieuChi2.NGAYLAP.Month + " năm " + v_ct_PhieuChi2.NGAYLAP.Year + "'";
						report.DataDefinition.FormulaFields["SOTIENBANGCHU"].Text = "'" + DocTienBangChu((long)v_ct_PhieuChi2.SOTIEN) + "'";
						break;
					}
				case "v_ct_PhieuThu":
					{
						report.FileName = HostingEnvironment.MapPath("~/Report/rptPhieuThu.rpt");
						report.Load();
						v_ct_PhieuThu v_ct_PhieuThu2 = (v_ct_PhieuThu)Master;
						report.DataDefinition.FormulaFields["NGAYLAP"].Text = "'Ngày " + v_ct_PhieuThu2.NGAYLAP.Day + " tháng " + v_ct_PhieuThu2.NGAYLAP.Month + " năm " + v_ct_PhieuThu2.NGAYLAP.Year + "'";
						report.DataDefinition.FormulaFields["SOTIENBANGCHU"].Text = "'" + DocTienBangChu((long)v_ct_PhieuThu2.SOTIEN) + "'";
						break;
					}
				case "v_ct_PhieuNhap":
					{
						report.FileName = HostingEnvironment.MapPath("~/Report/rptPhieuNhap.rpt");
						report.Load();
						v_ct_PhieuNhap v_ct_PhieuNhap2 = (v_ct_PhieuNhap)Master;
						report.DataDefinition.FormulaFields["MAPHIEU"].Text = "'" + v_ct_PhieuNhap2.MAPHIEU + "'";
						report.DataDefinition.FormulaFields["NGAYLAP"].Text = "'Ngày " + v_ct_PhieuNhap2.NGAYLAP.Day + " tháng " + v_ct_PhieuNhap2.NGAYLAP.Month + " năm " + v_ct_PhieuNhap2.NGAYLAP.Year + "'";
						report.DataDefinition.FormulaFields["TENNGUOIMUA"].Text = "'" + CovertText(v_ct_PhieuNhap2.NAME_KHACHHANG_NCC.Replace("'", "")) + "'";
						report.DataDefinition.FormulaFields["DIACHINGUOIMUA"].Text = "''";
						report.DataDefinition.FormulaFields["TENKHONHAP"].Text = "'" + CovertText(v_ct_PhieuNhap2.NAME_KHO) + "'";
						report.DataDefinition.FormulaFields["LOAIPHIEUNHAP"].Text = "'" + CovertText(v_ct_PhieuNhap2.NAME_LOAIPHIEUNHAP) + "'";
						report.DataDefinition.FormulaFields["GHICHU"].Text = "'" + CovertText(v_ct_PhieuNhap2.GHICHU.Replace("'", "")) + "'";
						break;
					}
				case "v_ct_PhieuDatHangNCC":
					{
						report.FileName = HostingEnvironment.MapPath("~/Report/rptPhieuDatHangNCC.rpt");
						report.Load();
						v_ct_PhieuDatHangNCC v_ct_PhieuDatHangNCC2 = (v_ct_PhieuDatHangNCC)Master;
						report.DataDefinition.FormulaFields["MAPHIEU"].Text = "'" + v_ct_PhieuDatHangNCC2.MAPHIEU + "'";
						report.DataDefinition.FormulaFields["NGAYLAP"].Text = "'Ngày " + v_ct_PhieuDatHangNCC2.NGAYLAP.Day + " tháng " + v_ct_PhieuDatHangNCC2.NGAYLAP.Month + " năm " + v_ct_PhieuDatHangNCC2.NGAYLAP.Year + "'";
						report.DataDefinition.FormulaFields["TENNGUOIMUA"].Text = "'" + CovertText(v_ct_PhieuDatHangNCC2.NAME_KHACHHANG_NCC.Replace("'", "")) + "'";
						report.DataDefinition.FormulaFields["DIACHINGUOIMUA"].Text = "''";
						report.DataDefinition.FormulaFields["TENKHONHAP"].Text = "'" + CovertText(v_ct_PhieuDatHangNCC2.NAME_KHO) + "'";
						report.DataDefinition.FormulaFields["LOAIPHIEUNHAP"].Text = "'" + CovertText(v_ct_PhieuDatHangNCC2.NAME_LOAIPHIEUNHAP) + "'";
						report.DataDefinition.FormulaFields["GHICHU"].Text = "'" + CovertText(v_ct_PhieuDatHangNCC2.GHICHU.Replace("'", "")) + "'";
						break;
					}
				case "v_ct_PhieuXuat":
					{
						report.FileName = HostingEnvironment.MapPath("~/Report/rptPhieuXuat.rpt");
						report.Load();
						v_ct_PhieuXuat v_ct_PhieuXuat2 = (v_ct_PhieuXuat)Master;
						report.DataDefinition.FormulaFields["MAPHIEU"].Text = "'" + CovertText(v_ct_PhieuXuat2.MAPHIEU) + "'";
						report.DataDefinition.FormulaFields["NGAYLAP"].Text = "'Ngày " + v_ct_PhieuXuat2.NGAYLAP.Day + " tháng " + v_ct_PhieuXuat2.NGAYLAP.Month + " năm " + v_ct_PhieuXuat2.NGAYLAP.Year + "'";
						report.DataDefinition.FormulaFields["TENNGUOIMUA"].Text = "'" + CovertText(v_ct_PhieuXuat2.NAME_KHACHHANG_NCC.Replace("'", "")) + "'";
						report.DataDefinition.FormulaFields["DIACHINGUOIMUA"].Text = "'" + CovertText(v_ct_PhieuXuat2.DIACHI_KHACHHANG_NCC.Replace("'", "")) + "'";
						report.DataDefinition.FormulaFields["TENKHONHAP"].Text = "'" + CovertText(v_ct_PhieuXuat2.NAME_KHO) + "'";
						report.DataDefinition.FormulaFields["LOAIPHIEUNHAP"].Text = "'" + CovertText(v_ct_PhieuXuat2.NAME_LOAIPHIEUXUAT) + "'";
						report.DataDefinition.FormulaFields["GHICHU"].Text = "'" + CovertText(v_ct_PhieuXuat2.GHICHU) + "'";
						report.DataDefinition.FormulaFields["SOTIENBANGCHU"].Text = "'" + CovertText(v_ct_PhieuXuat2.GHICHU) + "'";
						break;
					}
				case "v_ct_PhieuDatHang":
					{
						report.FileName = HostingEnvironment.MapPath("~/Report/rptPhieuDatHang.rpt");
						report.Load();
						v_ct_PhieuDatHang v_ct_PhieuDatHang2 = (v_ct_PhieuDatHang)Master;
						report.DataDefinition.FormulaFields["MAPHIEU"].Text = "'" + CovertText(v_ct_PhieuDatHang2.MAPHIEU) + "'";
						report.DataDefinition.FormulaFields["NGAYLAP"].Text = "'Ngày " + v_ct_PhieuDatHang2.NGAYLAP.Day + " tháng " + v_ct_PhieuDatHang2.NGAYLAP.Month + " năm " + v_ct_PhieuDatHang2.NGAYLAP.Year + "'";
						report.DataDefinition.FormulaFields["TENNGUOIMUA"].Text = "'" + CovertText(v_ct_PhieuDatHang2.NAME_KHACHHANG.Replace("'", "")) + "'";
						report.DataDefinition.FormulaFields["DIACHINGUOIMUA"].Text = "'" + CovertText(v_ct_PhieuDatHang2.ADDRESS.Replace("'", "")) + "'";
						report.DataDefinition.FormulaFields["TENKHONHAP"].Text = "'" + CovertText(v_ct_PhieuDatHang2.NAME_KHO.Replace("'", "")) + "'";
						report.DataDefinition.FormulaFields["GHICHU"].Text = "'" + CovertText(v_ct_PhieuDatHang2.GHICHU) + "'";
						break;
					}
				case "v_ct_PhieuGiaoHang":
					{
						if (!string.IsNullOrEmpty(MapPath))
						{
							report.FileName = HostingEnvironment.MapPath(MapPath);
						}
						else
						{
							report.FileName = HostingEnvironment.MapPath("~/Report/rptPhieuGiaoHang.rpt");
						}
						report.Load();
						v_ct_PhieuGiaoHang v_ct_PhieuGiaoHang2 = (v_ct_PhieuGiaoHang)Master;
						report.DataDefinition.FormulaFields["MAPHIEU"].Text = "'" + v_ct_PhieuGiaoHang2.MAPHIEU + "'";
						report.DataDefinition.FormulaFields["NGAYLAP"].Text = "'Ngày " + v_ct_PhieuGiaoHang2.NGAYLAP.Day + " tháng " + v_ct_PhieuGiaoHang2.NGAYLAP.Month + " năm " + v_ct_PhieuGiaoHang2.NGAYLAP.Year + "'";
						break;
					}
				case "v_ThongKeCongNoKhachHang":
					{
						report.FileName = HostingEnvironment.MapPath("~/Report/rptChiTietCongNo.rpt");
						report.Load();
						v_ThongKeCongNoKhachHang v_ThongKeCongNoKhachHang2 = (v_ThongKeCongNoKhachHang)Master;
						report.DataDefinition.FormulaFields["HOVATEN"].Text = "'Khách hàng: " + CovertText(v_ThongKeCongNoKhachHang2.NAME.Replace("'", "")) + "'";
						report.DataDefinition.FormulaFields["DIACHI_CN"].Text = "'" + CovertText(v_ThongKeCongNoKhachHang2.ADDRESS.Replace("'", "")) + "'";
						report.DataDefinition.FormulaFields["DIENTHOAI_CN"].Text = "'" + CovertText(v_ThongKeCongNoKhachHang2.TEL) + "'";
						report.DataDefinition.FormulaFields["DAUKY"].Text = "'" + v_ThongKeCongNoKhachHang2.TONGTIENCONGNODAUKY.ToString("N0") + "'";
						report.DataDefinition.FormulaFields["CUOIKY"].Text = "'" + v_ThongKeCongNoKhachHang2.TONGTIENCONGNOCUOIKY.ToString("N0") + "'";
						break;
					}
				case "v_ThongKeCongNoNhaCungCap":
					{
						report.FileName = HostingEnvironment.MapPath("~/Report/rptChiTietCongNo.rpt");
						report.Load();
						v_ThongKeCongNoNhaCungCap v_ThongKeCongNoNhaCungCap2 = (v_ThongKeCongNoNhaCungCap)Master;
						report.DataDefinition.FormulaFields["HOVATEN"].Text = "'Nhà cung cấp: " + CovertText(v_ThongKeCongNoNhaCungCap2.NAME.Replace("'", "")) + "'";
						report.DataDefinition.FormulaFields["DIACHI_CN"].Text = "'" + CovertText(v_ThongKeCongNoNhaCungCap2.ADDRESS.Replace("'", "")) + "'";
						report.DataDefinition.FormulaFields["DIENTHOAI_CN"].Text = "'" + CovertText(v_ThongKeCongNoNhaCungCap2.TEL) + "'";
						report.DataDefinition.FormulaFields["DAUKY"].Text = "'" + v_ThongKeCongNoNhaCungCap2.TONGTIENCONGNODAUKY.ToString("N0") + "'";
						report.DataDefinition.FormulaFields["CUOIKY"].Text = "'" + v_ThongKeCongNoNhaCungCap2.TONGTIENCONGNOCUOIKY.ToString("N0") + "'";
						break;
					}
				case "v_ThongKeCongNoNhanVien":
					{
						report.FileName = HostingEnvironment.MapPath("~/Report/rptChiTietCongNo.rpt");
						report.Load();
						v_ThongKeCongNoNhanVien v_ThongKeCongNoNhanVien2 = (v_ThongKeCongNoNhanVien)Master;
						report.DataDefinition.FormulaFields["HOVATEN"].Text = "'Nhân viên: " + CovertText(v_ThongKeCongNoNhanVien2.NAME.Replace("'", "")) + "'";
						report.DataDefinition.FormulaFields["DIACHI_CN"].Text = "'" + CovertText(v_ThongKeCongNoNhanVien2.ADDRESS.Replace("'", "")) + "'";
						report.DataDefinition.FormulaFields["DIENTHOAI_CN"].Text = "'" + CovertText(v_ThongKeCongNoNhanVien2.TEL) + "'";
						report.DataDefinition.FormulaFields["DAUKY"].Text = "'" + v_ThongKeCongNoNhanVien2.TONGTIENCONGNODAUKY.ToString("N0") + "'";
						report.DataDefinition.FormulaFields["CUOIKY"].Text = "'" + v_ThongKeCongNoNhanVien2.TONGTIENCONGNOCUOIKY.ToString("N0") + "'";
						break;
					}
				case "Sp_Get_BaoCaoGiaoHang_Result":
					{
						report.FileName = HostingEnvironment.MapPath("~/Report/rptChiTietCongNo.rpt");
						report.Load();
						Sp_Get_BaoCaoGiaoHang_Result sp_Get_BaoCaoGiaoHang_Result = (Sp_Get_BaoCaoGiaoHang_Result)Master;
						report.DataDefinition.FormulaFields["HOVATEN"].Text = "'Mã phiếu: " + sp_Get_BaoCaoGiaoHang_Result.MAPHIEU + "'";
						report.DataDefinition.FormulaFields["DIACHI_CN"].Text = "''";
						report.DataDefinition.FormulaFields["DIENTHOAI_CN"].Text = "''";
						report.DataDefinition.FormulaFields["DAUKY"].Text = "''";
						report.DataDefinition.FormulaFields["CUOIKY"].Text = "''";
						break;
					}
				case "v_ThongKeQuyTien":
					{
						report.FileName = HostingEnvironment.MapPath("~/Report/rptChiTietQuyTien.rpt");
						report.Load();
						v_ThongKeQuyTien v_ThongKeQuyTien2 = (v_ThongKeQuyTien)Master;
						report.DataDefinition.FormulaFields["HOVATEN"].Text = "'Tài khoản: " + CovertText(v_ThongKeQuyTien2.NAME.Replace("'", "")) + "'";
						report.DataDefinition.FormulaFields["DIACHI_CN"].Text = "'" + CovertText(v_ThongKeQuyTien2.CHUTAIKHOAN + ((!string.IsNullOrEmpty(v_ThongKeQuyTien2.SOTAIKHOAN)) ? (":" + v_ThongKeQuyTien2.SOTAIKHOAN) : "")) + "'";
						report.DataDefinition.FormulaFields["DIENTHOAI_CN"].Text = "'" + CovertText(v_ThongKeQuyTien2.MANGANHANG + ((!string.IsNullOrEmpty(v_ThongKeQuyTien2.TENNGANHANG)) ? (" - " + v_ThongKeQuyTien2.TENNGANHANG + " " + v_ThongKeQuyTien2.TINHTP) : "")) + "'";
						report.DataDefinition.FormulaFields["DAUKY"].Text = "'" + v_ThongKeQuyTien2.TONGTIENCONGNODAUKY.ToString("N0") + "'";
						report.DataDefinition.FormulaFields["CUOIKY"].Text = "'" + v_ThongKeQuyTien2.TONGTIENCONGNOCUOIKY.ToString("N0") + "'";
						break;
					}
				case "Sp_Get_BaoCaoTheoNhanVien_Result":
					{
						report.FileName = HostingEnvironment.MapPath("~/Report/rptBaoCaoNhanVien.rpt");
						report.Load();
						Sp_Get_BaoCaoTheoNhanVien_Result sp_Get_BaoCaoTheoNhanVien_Result = (Sp_Get_BaoCaoTheoNhanVien_Result)Master;
						report.DataDefinition.FormulaFields["HOVATEN"].Text = "'Tài khoản: " + CovertText(sp_Get_BaoCaoTheoNhanVien_Result.NAME_NHANVIEN.Replace("'", "")) + "'";
						report.DataDefinition.FormulaFields["DIACHI_CN"].Text = "'" + CovertText(sp_Get_BaoCaoTheoNhanVien_Result.NAME_LOAIPHIEU.Replace("'", "")) + "'";
						report.DataDefinition.FormulaFields["DIENTHOAI_CN"].Text = "''";
						report.DataDefinition.FormulaFields["DAUKY"].Text = "''";
						report.DataDefinition.FormulaFields["CUOIKY"].Text = "''";
						break;
					}
			}
			report.DataDefinition.FormulaFields["TENCONGTY"].Text = "'" + CovertText(v_v_dm_CongTy2.NAME) + "'";
			report.DataDefinition.FormulaFields["DIACHI"].Text = "'" + CovertText(v_v_dm_CongTy2.ADDRESS) + "'";
			report.DataDefinition.FormulaFields["DIENTHOAI"].Text = "'" + CovertText(v_v_dm_CongTy2.TEL) + "'";
			report.DataDefinition.FormulaFields["ICON"].Text = "'" + CovertText(v_v_dm_CongTy2.LOGO) + "'";
			report.SetDatabaseLogon("test", "test@", "test", "test");
			return report;
		}

		private static string CovertText(string text)
		{
			if (string.IsNullOrEmpty(text))
			{
				return "";
			}
			text = text.Replace("'", "");
			text = text.Replace("\r\n", " ");
			return text;
		}

		public static ApiResponse GetDebtCustomerDetail<T>(v_ThongKeCongNoKhachHang model, string name = "Books")
		{
			ApiResponse apiResponse = new ApiResponse();
			HttpResponseMessage httpResponseMessage = null;
			StringContent stringContent = null;
			List<T> data = new List<T>();
			string text = "";
			try
			{
				text = JsonConvert.SerializeObject(model);
				stringContent = new StringContent(text, Encoding.UTF8, "application/json");
				HttpClient httpClient = new HttpClient();
				httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + Token);
				httpResponseMessage = httpClient.PostAsync(URL + name, stringContent).Result;
				if (httpResponseMessage.IsSuccessStatusCode)
				{
					string result = httpResponseMessage.Content.ReadAsStringAsync().Result;
					apiResponse = JsonConvert.DeserializeObject<ApiResponse>(result);
					if (apiResponse.Data != null)
					{
						data = JsonConvert.DeserializeObject<List<T>>(apiResponse.Data.ToString());
					}
					apiResponse.Data = data;
				}
				else
				{
					apiResponse.Message = GetErrorServer(httpResponseMessage);
				}
			}
			catch (Exception ex)
			{
				WriteLog((httpResponseMessage != null) ? httpResponseMessage.RequestMessage.RequestUri.ToString() : "GetDebtDetail", MethodBase.GetCurrentMethod().Name, ex, text);
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
			HttpResponseMessage httpResponseMessage = null;
			StringContent stringContent = null;
			List<T> data = new List<T>();
			string text = "";
			try
			{
				text = JsonConvert.SerializeObject(model);
				stringContent = new StringContent(text, Encoding.UTF8, "application/json");
				HttpClient httpClient = new HttpClient();
				httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + Token);
				httpResponseMessage = httpClient.PostAsync(URL + name, stringContent).Result;
				if (httpResponseMessage.IsSuccessStatusCode)
				{
					string result = httpResponseMessage.Content.ReadAsStringAsync().Result;
					apiResponse = JsonConvert.DeserializeObject<ApiResponse>(result);
					if (apiResponse.Data != null)
					{
						data = JsonConvert.DeserializeObject<List<T>>(apiResponse.Data.ToString());
					}
					apiResponse.Data = data;
				}
				else
				{
					apiResponse.Message = GetErrorServer(httpResponseMessage);
				}
			}
			catch (Exception ex)
			{
				WriteLog((httpResponseMessage != null) ? httpResponseMessage.RequestMessage.RequestUri.ToString() : "GetDebtDetail", MethodBase.GetCurrentMethod().Name, ex, text);
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
			HttpResponseMessage httpResponseMessage = null;
			StringContent stringContent = null;
			List<T> data = new List<T>();
			string text = "";
			try
			{
				text = JsonConvert.SerializeObject(model);
				stringContent = new StringContent(text, Encoding.UTF8, "application/json");
				HttpClient httpClient = new HttpClient();
				httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + Token);
				httpResponseMessage = httpClient.PostAsync(URL + name, stringContent).Result;
				if (httpResponseMessage.IsSuccessStatusCode)
				{
					string result = httpResponseMessage.Content.ReadAsStringAsync().Result;
					apiResponse = JsonConvert.DeserializeObject<ApiResponse>(result);
					if (apiResponse.Data != null)
					{
						data = JsonConvert.DeserializeObject<List<T>>(apiResponse.Data.ToString());
					}
					apiResponse.Data = data;
				}
				else
				{
					apiResponse.Message = GetErrorServer(httpResponseMessage);
				}
			}
			catch (Exception ex)
			{
				WriteLog((httpResponseMessage != null) ? httpResponseMessage.RequestMessage.RequestUri.ToString() : "GetDebtDetail", MethodBase.GetCurrentMethod().Name, ex, text);
				apiResponse = new ApiResponse
				{
					Success = false,
					Message = ex.Message,
					Data = ""
				};
			}
			return apiResponse;
		}

		public static DataTable ToDataTable<T>(List<T> list)
		{
			DataTable dataTable = new DataTable(typeof(T).Name);
			PropertyInfo[] properties = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public);
			PropertyInfo[] array = properties;
			foreach (PropertyInfo propertyInfo in array)
			{
				DataColumn dataColumn = new DataColumn();
				dataColumn.ColumnName = propertyInfo.Name;
				dataColumn.DataType = (propertyInfo.PropertyType.Name.Contains("Nullable") ? typeof(string) : propertyInfo.PropertyType);
				DataColumn column = dataColumn;
				dataTable.Columns.Add(column);
			}
			int num = 1;
			foreach (T item in list)
			{
				object[] array2 = new object[properties.Length];
				for (int j = 0; j < properties.Length; j++)
				{
					if (dataTable.Columns[j].ColumnName == "STT")
					{
						array2[j] = num++;
					}
					else
					{
						array2[j] = properties[j].GetValue(item, null);
					}
				}
				dataTable.Rows.Add(array2);
			}
			return dataTable;
		}

		public static ApiResponse Get_BaoCaoGiaoHang<T>(SP_Parameter objParameter)
		{
			ApiResponse apiResponse = new ApiResponse();
			try
			{
				return ExecuteStoredProc<T>(objParameter, "Sp_Get_BaoCaoGiaoHang");
			}
			catch (Exception ex)
			{
				ApiResponse obj = new ApiResponse
				{
					Success = false,
					Message = ex.Message,
					Data = ""
				};
				apiResponse = obj;
				return obj;
			}
		}

		public static ApiResponse Get_BaoCaoGiaoHangDetail<T>(Sp_Get_BaoCaoGiaoHang_Result model, string name = "Books")
		{
			ApiResponse apiResponse = new ApiResponse();
			HttpResponseMessage httpResponseMessage = null;
			StringContent stringContent = null;
			List<T> data = new List<T>();
			string text = "";
			try
			{
				text = JsonConvert.SerializeObject(model);
				stringContent = new StringContent(text, Encoding.UTF8, "application/json");
				HttpClient httpClient = new HttpClient();
				httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + Token);
				httpResponseMessage = httpClient.PostAsync(URL + name, stringContent).Result;
				if (httpResponseMessage.IsSuccessStatusCode)
				{
					string result = httpResponseMessage.Content.ReadAsStringAsync().Result;
					apiResponse = JsonConvert.DeserializeObject<ApiResponse>(result);
					if (apiResponse.Data != null)
					{
						data = JsonConvert.DeserializeObject<List<T>>(apiResponse.Data.ToString());
					}
					apiResponse.Data = data;
				}
				else
				{
					apiResponse.Message = GetErrorServer(httpResponseMessage);
				}
			}
			catch (Exception ex)
			{
				WriteLog((httpResponseMessage != null) ? httpResponseMessage.RequestMessage.RequestUri.ToString() : "GetDebtDetail", MethodBase.GetCurrentMethod().Name, ex, text);
				apiResponse = new ApiResponse
				{
					Success = false,
					Message = ex.Message,
					Data = ""
				};
			}
			return apiResponse;
		}

		public static ApiResponse Get_DanhSachChamCong<T>(DateTime? TUNGAY, DateTime? DENNAY, DateTime? NGAYCONG, string SearchString = "", string ID_NHANVIEN = "")
		{
			ApiResponse apiResponse = new ApiResponse();
			try
			{
				SP_Parameter sP_Parameter = new SP_Parameter();
				sP_Parameter.LOC_ID = LOC_ID;
				sP_Parameter.TUNGAY = TUNGAY;
				sP_Parameter.DENNGAY = DENNAY;
				sP_Parameter.KEY = SearchString;
				sP_Parameter.NGAYCONG = NGAYCONG;
				if (NGAYCONG.HasValue)
				{
					sP_Parameter.ISTHEOTHOIGIAN = false;
				}
				sP_Parameter.ID_NHANVIEN = ID_NHANVIEN;
				return ExecuteStoredProc<T>(sP_Parameter, "Sp_Get_DanhSachChamCong");
			}
			catch (Exception ex)
			{
				ApiResponse obj = new ApiResponse
				{
					Success = false,
					Message = ex.Message,
					Data = ""
				};
				apiResponse = obj;
				return obj;
			}
		}

		public static ApiResponse Get_DanhSachNghiPhep<T>(DateTime? TUNGAY, DateTime? DENNAY, DateTime? NGAYCONG, string SearchString = "", string ID_NHANVIEN = "")
		{
			ApiResponse apiResponse = new ApiResponse();
			try
			{
				SP_Parameter sP_Parameter = new SP_Parameter();
				sP_Parameter.LOC_ID = LOC_ID;
				sP_Parameter.TUNGAY = TUNGAY;
				sP_Parameter.DENNGAY = DENNAY;
				sP_Parameter.KEY = SearchString;
				sP_Parameter.NGAYCONG = NGAYCONG;
				if (NGAYCONG.HasValue)
				{
					sP_Parameter.ISTHEOTHOIGIAN = false;
				}
				sP_Parameter.ID_NHANVIEN = ID_NHANVIEN;
				return ExecuteStoredProc<T>(sP_Parameter, "Sp_Get_DanhSachNghiPhep");
			}
			catch (Exception ex)
			{
				ApiResponse obj = new ApiResponse
				{
					Success = false,
					Message = ex.Message,
					Data = ""
				};
				apiResponse = obj;
				return obj;
			}
		}

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
					return "Không xác định";
			}
		}

		public static List<ComboboxFrom> DachSachTinhChat()
		{
			List<ComboboxFrom> list = new List<ComboboxFrom>();
			list.Add(new ComboboxFrom
			{
				ID = 1.ToString(),
				MA = 1.ToString(),
				NAME = "Hàng hóa/dịch vụ",
				ISACTIVE = true,
				ISDEFAULT = true
			});
			list.Add(new ComboboxFrom
			{
				ID = 2.ToString(),
				MA = 2.ToString(),
				NAME = "Khuyến mại",
				ISACTIVE = true
			});
			list.Add(new ComboboxFrom
			{
				ID = 3.ToString(),
				MA = 3.ToString(),
				NAME = "Chiết khấu thương mại",
				ISACTIVE = true
			});
			list.Add(new ComboboxFrom
			{
				ID = 4.ToString(),
				MA = 4.ToString(),
				NAME = "Ghi chú/diễn giải",
				ISACTIVE = true
			});
			return list;
		}

		public static List<ComboboxFrom> DachSachHinhThucThanhToan()
		{
			List<ComboboxFrom> list = new List<ComboboxFrom>();
			list.Add(new ComboboxFrom
			{
				ID = "TM/CK",
				MA = "TM/CK",
				NAME = "TM/CK",
				ISACTIVE = true,
				ISDEFAULT = true
			});
			list.Add(new ComboboxFrom
			{
				ID = "TM",
				MA = "TM",
				NAME = "TM",
				ISACTIVE = true
			});
			list.Add(new ComboboxFrom
			{
				ID = "CK",
				MA = "CK",
				NAME = "CK",
				ISACTIVE = true
			});
			return list;
		}

		public static List<ComboboxFrom> DanhSachMauHoaDon()
		{
			List<ComboboxFrom> list = new List<ComboboxFrom>();
			if (GetListData<MisaInvoiceTemplate>("Invoiced_Misa", "", "", LOC_ID).Data is List<MisaInvoiceTemplate> list2)
			{
				foreach (MisaInvoiceTemplate item in list2)
				{
					list.Add(new ComboboxFrom
					{
						ID = item.IPTemplateID.ToString(),
						MA = item.IPTemplateID.ToString(),
						NAME = item.InvSeries,
						ISACTIVE = true
					});
				}
			}
			return list;
		}

		public static string GetEnumDescription(Enum value)
		{
			FieldInfo field = value.GetType().GetField(value.ToString());
			DescriptionAttribute[] array = (DescriptionAttribute[])field.GetCustomAttributes(typeof(DescriptionAttribute), inherit: false);
			return (array.Length != 0) ? array[0].Description : value.ToString();
		}

		public static List<v_ChiTietHoaDon> LayDonGia_KhungGia(string ID_HANGHOA, string ID_DVT, double SOLUONG, List<v_ChiTietHoaDon> lstChiTietHoaDon)
		{
			ApiResponse apiResponse = Create(lstChiTietHoaDon, "ProductPriceRange/" + LOC_ID + "/" + ID_HANGHOA + "/" + ID_DVT);
			if (apiResponse.Data != null)
			{
				List<v_ChiTietHoaDon> list = JsonConvert.DeserializeObject<List<v_ChiTietHoaDon>>(apiResponse.Data.ToString());
				if (list != null && list.Count > 0)
				{
					return list;
				}
				return new List<v_ChiTietHoaDon>();
			}
			return new List<v_ChiTietHoaDon>();
		}
	}
}
