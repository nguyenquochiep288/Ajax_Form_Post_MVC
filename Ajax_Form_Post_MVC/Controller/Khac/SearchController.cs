using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using DatabaseTHP;
using DatabaseTHP.Class;
using MVC_QuanLyTHP.Class;
using MVC_QuanLyTHP.Models;

namespace MVC_QuanLyTHP.Controllers
{

	public class SearchController : Controller
	{
		[ChildActionOnly]
		public ActionResult Index()
		{
			string text = "Promotion";
			string absolutePath = base.Request.Url.AbsolutePath;
			if (absolutePath.ToUpper().Contains("KPI_Sale".ToUpper()))
			{
				text = "KPI_Sale";
			}
			base.ViewBag.urlAddProductPromotion_YC = text;
			v_v_Search v_v_Search2 = new v_v_Search();
			v_v_Search2.lstdm_ThueSuat = new List<v_dm_ThueSuat>();
			v_v_Search2.lstdm_ThueSuat = Utility.GetListData<v_dm_ThueSuat>("Tax", "", "", Utility.LOC_ID).Data as List<v_dm_ThueSuat>;
			v_v_Search2.lstdm_DonViTinh = new List<v_dm_DonViTinh>();
			v_v_Search2.lstdm_DonViTinh = Utility.GetListData<v_dm_DonViTinh>("Unit", "", "", Utility.LOC_ID).Data as List<v_dm_DonViTinh>;
			v_v_Search2.lstKhuVuc = new List<v_dm_KhuVuc>();
			v_v_Search2.lstKhuVuc = Utility.GetListData<v_dm_KhuVuc>("Area", "", "", Utility.LOC_ID).Data as List<v_dm_KhuVuc>;
			if (v_v_Search2.lstdm_ThueSuat != null)
			{
				v_v_Search2.lstdm_ThueSuat = v_v_Search2.lstdm_ThueSuat.Where((v_dm_ThueSuat s) => s.ISACTIVE).ToList();
			}
			else
			{
				v_v_Search2.lstdm_ThueSuat = new List<v_dm_ThueSuat>();
			}
			if (v_v_Search2.lstdm_DonViTinh != null)
			{
				v_v_Search2.lstdm_DonViTinh = v_v_Search2.lstdm_DonViTinh.Where((v_dm_DonViTinh s) => s.ISACTIVE).ToList();
			}
			else
			{
				v_v_Search2.lstdm_DonViTinh = new List<v_dm_DonViTinh>();
			}
			if (v_v_Search2.lstKhuVuc != null)
			{
				v_v_Search2.lstKhuVuc = v_v_Search2.lstKhuVuc.Where((v_dm_KhuVuc s) => s.ISACTIVE).ToList();
			}
			else
			{
				v_v_Search2.lstKhuVuc = new List<v_dm_KhuVuc>();
			}
			base.ViewBag.PermissionEditPrice = Utility.KiemTraQuyen("Deposit", "EditPrice");
			return PartialView(v_v_Search2);
		}

		[HttpPost]
		public ActionResult LoadSearch(string MyModal, string ClassName = "", int HinhThucTimKiem = 0, string ValueField = "", string TextField = "", string ID_KHO = "", string ID_KHUVUC = "")
		{
			ApiResponse apiResponse = new ApiResponse();
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
			Search search = new Search();
			search.MyModal = MyModal;
			search.ClassName = ClassName;
			search.ValueField = ValueField;
			search.TextField = TextField;
			search.HinhThucTimKiem = HinhThucTimKiem;
			search.TitleSearch = Utility.GetTitleFrom(ClassName);
			search.ID_KHO = ID_KHO;
			search.ID_KHUVUC = ID_KHUVUC;
			switch (ClassName)
			{
				case "DepositCustomer":
					{
						search.ID_KHUVUC = "-1";
						search.TitleSearch = Utility.GetTitleFrom("Customer");
						search.ShowSearchValue = Utility.GetShowSearchValue<dm_KhachHang>("");
						DepositController depositController = new DepositController();
						string text = ((base.Session["idNhomQuyen"] != null) ? base.Session["idNhomQuyen"].ToString() : "");
						API.LONGITUDE = Utility.Longitude;
						API.LATITUDE = Utility.Latitude;
						apiResponse = ((!(text != "-1")) ? Utility.GetListData<v_v_dm_KhachHang>("Customer", "", "", Utility.LOC_ID) : depositController.GetDanhSachKhachHang<v_v_dm_KhachHang>(text));
						if (!apiResponse.Success)
						{
							apiResponse.Data = new List<v_v_dm_KhachHang>();
							base.TempData["TitleError"] = apiResponse.Message;
							apiResponse.Success = false;
							apiResponse.URL = base.Url.Action("Index", "Notfound");
						}
						else
						{
							search = GetData<v_v_dm_KhachHang>(apiResponse, search);
						}
						break;
					}
				case "Input":
					search.TitleSearch = Utility.GetTitleFrom("Input");
					search.ShowSearchValue = Utility.GetShowSearchValue<dm_HangHoa>("");
					apiResponse = Utility.Get_DanhSachSanPhamKho<Product_Detail>(search.ID_KHO, bolTonKho: false);
					if (!apiResponse.Success)
					{
						apiResponse.Data = new List<Product_Detail>();
						base.TempData["TitleError"] = apiResponse.Message;
						apiResponse.Success = false;
						apiResponse.URL = base.Url.Action("Index", "Notfound");
					}
					else
					{
						search = GetData<Product_Detail>(apiResponse, search);
					}
					break;
				case "Output":
					search.TitleSearch = Utility.GetTitleFrom("Output");
					search.ShowSearchValue = Utility.GetShowSearchValue<dm_HangHoa>("");
					apiResponse = Utility.Get_DanhSachSanPhamKho<Product_Detail>(search.ID_KHO, bolTonKho: true);
					if (!apiResponse.Success)
					{
						apiResponse.Data = new List<Product_Detail>();
						base.TempData["TitleError"] = apiResponse.Message;
						apiResponse.Success = false;
						apiResponse.URL = base.Url.Action("Index", "Notfound");
					}
					else
					{
						search = GetData<Product_Detail>(apiResponse, search);
					}
					break;
				case "Product":
					search.TitleSearch = Utility.GetTitleFrom("Product");
					search.ShowSearchValue = Utility.GetShowSearchValue<dm_HangHoa>("");
					apiResponse = Utility.GetListData<v_dm_HangHoa>(ClassName, search.ShowSearchValue, "", Utility.LOC_ID);
					search = GetData<v_dm_HangHoa>(apiResponse, search);
					break;
				case "Customer":
					search.ID_KHUVUC = "-1";
					search.TitleSearch = Utility.GetTitleFrom("Customer");
					search.ShowSearchValue = Utility.GetShowSearchValue<dm_KhachHang>("");
					apiResponse = Utility.GetListData<v_v_dm_KhachHang>(ClassName, search.ShowSearchValue, "", Utility.LOC_ID);
					search = GetData<v_v_dm_KhachHang>(apiResponse, search);
					break;
				case "GroupProduct":
					search.TitleSearch = Utility.GetTitleFrom("GroupProduct");
					search.ShowSearchValue = Utility.GetShowSearchValue<dm_NhomHangHoa>("");
					apiResponse = Utility.GetListData<v_dm_NhomHangHoa>(ClassName, search.ShowSearchValue, "", Utility.LOC_ID);
					search = GetData<v_dm_NhomHangHoa>(apiResponse, search);
					break;
				case "Provider":
					search.TitleSearch = Utility.GetTitleFrom("Provider");
					search.ShowSearchValue = Utility.GetShowSearchValue<dm_NhaCungCap>("");
					apiResponse = Utility.GetListData<v_dm_NhaCungCap>(ClassName, search.ShowSearchValue, "", Utility.LOC_ID);
					search = GetData<v_dm_NhaCungCap>(apiResponse, search);
					break;
				case "Employee":
					search.TitleSearch = Utility.GetTitleFrom("Employee");
					search.ShowSearchValue = Utility.GetShowSearchValue<dm_NhanVien>("");
					apiResponse = Utility.GetListData<v_dm_NhanVien>(ClassName, search.ShowSearchValue, "", Utility.LOC_ID);
					search = GetData<v_dm_NhanVien>(apiResponse, search);
					break;
				case "Area":
					search.TitleSearch = Utility.GetTitleFrom("Area");
					search.ShowSearchValue = Utility.GetShowSearchValue<dm_KhuVuc>("");
					apiResponse = Utility.GetListData<v_dm_KhuVuc>(ClassName, search.ShowSearchValue, "", Utility.LOC_ID);
					search = GetData<v_dm_KhuVuc>(apiResponse, search);
					break;
				case "BankAccount":
					search.TitleSearch = Utility.GetTitleFrom("BankAccount");
					search.ShowSearchValue = Utility.GetShowSearchValue<dm_TaiKhoanNganHang>("");
					apiResponse = Utility.GetListData<v_dm_TaiKhoanNganHang>(ClassName, search.ShowSearchValue, "", Utility.LOC_ID);
					search = GetData<v_dm_TaiKhoanNganHang>(apiResponse, search);
					break;
				case "Car":
					search.TitleSearch = Utility.GetTitleFrom("Car");
					search.ShowSearchValue = Utility.GetShowSearchValue<dm_Xe>("");
					apiResponse = Utility.GetListData<v_dm_Xe>(ClassName, search.ShowSearchValue, "", Utility.LOC_ID);
					search = GetData<v_dm_Xe>(apiResponse, search);
					break;
				case "Currency":
					search.TitleSearch = Utility.GetTitleFrom("Currency");
					search.ShowSearchValue = Utility.GetShowSearchValue<dm_TienTe>("");
					apiResponse = Utility.GetListData<v_dm_TienTe>(ClassName, search.ShowSearchValue, "", Utility.LOC_ID);
					search = GetData<v_dm_TienTe>(apiResponse, search);
					break;
				case "GroupCustomer":
					search.TitleSearch = Utility.GetTitleFrom("GroupCustomer");
					search.ShowSearchValue = Utility.GetShowSearchValue<dm_NhomKhachHang>("");
					apiResponse = Utility.GetListData<v_dm_NhomKhachHang>(ClassName, search.ShowSearchValue, "", Utility.LOC_ID);
					search = GetData<v_dm_NhomKhachHang>(apiResponse, search);
					break;
				case "GroupProvider":
					search.TitleSearch = Utility.GetTitleFrom("GroupProvider");
					search.ShowSearchValue = Utility.GetShowSearchValue<dm_NhomNhaCungCap>("");
					apiResponse = Utility.GetListData<v_dm_NhomNhaCungCap>(ClassName, search.ShowSearchValue, "", Utility.LOC_ID);
					search = GetData<v_dm_NhomNhaCungCap>(apiResponse, search);
					break;
				case "Menu":
					search.TitleSearch = Utility.GetTitleFrom("Menu");
					search.ShowSearchValue = Utility.GetShowSearchValue<web_Menu>("");
					apiResponse = Utility.GetListData<v_web_Menu>(ClassName, search.ShowSearchValue, "", Utility.LOC_ID);
					search = GetData<v_web_Menu>(apiResponse, search);
					break;
				case "Position":
					search.TitleSearch = Utility.GetTitleFrom("Position");
					search.ShowSearchValue = Utility.GetShowSearchValue<dm_ChucVu>("");
					apiResponse = Utility.GetListData<v_dm_ChucVu>(ClassName, search.ShowSearchValue, "", Utility.LOC_ID);
					search = GetData<v_dm_ChucVu>(apiResponse, search);
					break;
				case "Promotion":
					search.TitleSearch = Utility.GetTitleFrom("Promotion");
					search.ShowSearchValue = Utility.GetShowSearchValue<dm_ChuongTrinhKhuyenMai>("");
					apiResponse = Utility.GetListData<v_dm_ChuongTrinhKhuyenMai>(ClassName, search.ShowSearchValue, "", Utility.LOC_ID);
					search = GetData<v_dm_ChuongTrinhKhuyenMai>(apiResponse, search);
					break;
				case "Tax":
					search.TitleSearch = Utility.GetTitleFrom("Tax");
					search.ShowSearchValue = Utility.GetShowSearchValue<dm_ThueSuat>("");
					apiResponse = Utility.GetListData<v_dm_ThueSuat>(ClassName, search.ShowSearchValue, "", Utility.LOC_ID);
					search = GetData<v_dm_ThueSuat>(apiResponse, search);
					break;
				case "TypePayment":
					search.TitleSearch = Utility.GetTitleFrom("TypePayment");
					search.ShowSearchValue = Utility.GetShowSearchValue<dm_LoaiPhieuChi>("");
					apiResponse = Utility.GetListData<v_dm_LoaiPhieuChi>(ClassName, search.ShowSearchValue, "", Utility.LOC_ID);
					search = GetData<v_dm_LoaiPhieuChi>(apiResponse, search);
					break;
				case "TypeInput":
					search.TitleSearch = Utility.GetTitleFrom("TypeInput");
					search.ShowSearchValue = Utility.GetShowSearchValue<dm_LoaiPhieuNhap>("");
					apiResponse = Utility.GetListData<v_dm_LoaiPhieuNhap>(ClassName, search.ShowSearchValue, "", Utility.LOC_ID);
					search = GetData<v_dm_LoaiPhieuNhap>(apiResponse, search);
					break;
				case "TypeReceipt":
					search.TitleSearch = Utility.GetTitleFrom("TypeReceipt");
					search.ShowSearchValue = Utility.GetShowSearchValue<dm_LoaiPhieuThu>("");
					apiResponse = Utility.GetListData<v_dm_LoaiPhieuThu>(ClassName, search.ShowSearchValue, "", Utility.LOC_ID);
					search = GetData<v_dm_LoaiPhieuThu>(apiResponse, search);
					break;
				case "TypeOutput":
					search.TitleSearch = Utility.GetTitleFrom("TypeOutput");
					search.ShowSearchValue = Utility.GetShowSearchValue<dm_LoaiPhieuXuat>("");
					apiResponse = Utility.GetListData<v_dm_LoaiPhieuXuat>(ClassName, search.ShowSearchValue, "", Utility.LOC_ID);
					search = GetData<v_dm_LoaiPhieuXuat>(apiResponse, search);
					break;
				case "Unit":
					search.TitleSearch = Utility.GetTitleFrom("Unit");
					search.ShowSearchValue = Utility.GetShowSearchValue<dm_DonViTinh>("");
					apiResponse = Utility.GetListData<v_dm_DonViTinh>(ClassName, search.ShowSearchValue, "", Utility.LOC_ID);
					search = GetData<v_dm_DonViTinh>(apiResponse, search);
					break;
				case "User":
					search.TitleSearch = Utility.GetTitleFrom("User");
					search.ShowSearchValue = Utility.GetShowSearchValue<AspNetUsers>("");
					apiResponse = Utility.GetListData<v_AspNetUsers>(ClassName, search.ShowSearchValue, "", Utility.LOC_ID);
					search = GetData<v_AspNetUsers>(apiResponse, search);
					break;
				case "Depot":
					search.TitleSearch = Utility.GetTitleFrom("Depot");
					search.ShowSearchValue = Utility.GetShowSearchValue<dm_Kho>("");
					apiResponse = Utility.GetListData<v_dm_Kho>(ClassName, search.ShowSearchValue, "", Utility.LOC_ID);
					search = GetData<v_dm_Kho>(apiResponse, search);
					break;
				case "GroupPermissions":
					search.TitleSearch = Utility.GetTitleFrom("GroupPermissions");
					search.ShowSearchValue = Utility.GetShowSearchValue<web_NhomQuyen>("");
					apiResponse = Utility.GetListData<v_web_NhomQuyen>(ClassName, search.ShowSearchValue, "", Utility.LOC_ID);
					search = GetData<v_web_NhomQuyen>(apiResponse, search);
					break;
				case "Department":
					search.TitleSearch = Utility.GetTitleFrom("Department");
					search.ShowSearchValue = Utility.GetShowSearchValue<dm_PhongBan>("");
					apiResponse = Utility.GetListData<v_dm_PhongBan>(ClassName, search.ShowSearchValue, "", Utility.LOC_ID);
					search = GetData<v_dm_PhongBan>(apiResponse, search);
					break;
			}
			search.listSearch = Utility.listSearch;
			return Json(search, JsonRequestBehavior.AllowGet);
		}

		public Search GetData<T>(ApiResponse apiResponse, Search Search)
		{
			try
			{
				IEnumerable<PropertyInfo> runtimeProperties = typeof(T).GetRuntimeProperties();
				List<view_web_NoteClass> list = Utility.GetNoteClass();
				if (list != null)
				{
					list = list.Where((view_web_NoteClass s) => !string.IsNullOrEmpty(s.NAMECLASS) && s.NAMECLASS.ToLower() == typeof(T).Name.Replace("v_", "").ToLower() && s.ISSEARCH).ToList();
				}
				if (list != null && list.Count > 0)
				{
					if (Search.HinhThucTimKiem == 3)
					{
						Search.TrField += "<th style=\"font-weight: bold; text-align:center; white-space: nowrap;\"></th>";
					}
					if (apiResponse.Data is List<v_v_dm_KhachHang>)
					{
						Search.TrField += "<th style=\"font-weight: bold; text-align:center; white-space: nowrap;\">...</th>";
					}
					foreach (view_web_NoteClass item in list.OrderBy((view_web_NoteClass s) => s.STT))
					{
						Search.TrField = Search.TrField + "<th style=\"font-weight: bold; text-align:center; white-space: nowrap;\"> " + item.DISPLAYNAME + "</th>";
					}
				}
				if (apiResponse.Success && apiResponse.Data != null && list != null && list.Count > 0)
				{
					int num = (apiResponse.Data as List<T>).Count();
					int num2 = 0;
					bool flag = false;
					if (apiResponse.Data is List<v_v_dm_KhachHang>)
					{
						List<v_v_dm_KhachHang> source = (apiResponse.Data as List<v_v_dm_KhachHang>).ToList();
						List<v_v_dm_KhachHang> list2 = new List<v_v_dm_KhachHang>();
						List<v_v_dm_KhachHang> list3 = new List<v_v_dm_KhachHang>();
						list2 = (from itm in source
								 where itm.KHOANGCACH != 0.0
								 orderby itm.KHOANGCACH, itm.NAME
								 select itm).ToList();
						list3 = (from itm in source
								 where itm.KHOANGCACH == 0.0
								 orderby itm.KHOANGCACH, itm.NAME
								 select itm).ToList();
						list2.AddRange(list3);
						apiResponse.Data = list2;
						flag = true;
					}
					foreach (T item2 in apiResponse.Data as List<T>)
					{
						if (num > 100 && string.IsNullOrEmpty(Search.SearchString))
						{
							num2++;
							if (num2 > 100)
							{
								break;
							}
						}
						bool flag2 = true;
						PropertyInfo propertyInfo = runtimeProperties.Where((PropertyInfo propertyInfo2) => propertyInfo2.Name.ToUpper() == "ISACTIVE".ToUpper()).FirstOrDefault();
						if (propertyInfo != null)
						{
							object value = propertyInfo.GetValue(item2);
							if (!(bool)value)
							{
								flag2 = false;
							}
						}
						string text = "";
						string stringToEscape = "";
						if (!flag2)
						{
							continue;
						}
						if (propertyInfo != null)
						{
							if ("Input" == Search.ClassName || "Output" == Search.ClassName)
							{
								propertyInfo = runtimeProperties.Where((PropertyInfo propertyInfo2) => propertyInfo2.Name.ToUpper() == "ID_HANGHOAKHO".ToUpper()).FirstOrDefault();
								if (propertyInfo != null)
								{
									object value2 = propertyInfo.GetValue(item2);
									if (value2 != null)
									{
										text = value2.ToString();
									}
								}
							}
							else
							{
								propertyInfo = runtimeProperties.Where((PropertyInfo propertyInfo2) => propertyInfo2.Name.ToUpper() == "ID".ToUpper()).FirstOrDefault();
								if (propertyInfo != null)
								{
									object value3 = propertyInfo.GetValue(item2);
									if (value3 != null)
									{
										text = value3.ToString();
									}
								}
								propertyInfo = runtimeProperties.Where((PropertyInfo propertyInfo2) => propertyInfo2.Name.ToUpper() == "NAME".ToUpper()).FirstOrDefault();
								if (propertyInfo != null)
								{
									object value4 = propertyInfo.GetValue(item2);
									if (value4 != null)
									{
										stringToEscape = value4.ToString();
									}
								}
							}
							if (!string.IsNullOrEmpty(text))
							{
								if (Search.HinhThucTimKiem == 3)
								{
									Search.BodyField = Search.BodyField + "<tr id=\"" + text + "\">";
								}
								else if (Search.HinhThucTimKiem == 1)
								{
									Search search = Search;
									search.BodyField = search.BodyField + "<tr id=\"" + text + "\"  ondblclick=myFunSuccessCombo(\"" + text + "\")>";
								}
								else if (Search.HinhThucTimKiem == 2)
								{
									if ("Output" == Search.ClassName)
									{
										Search search = Search;
										search.BodyField = search.BodyField + "<tr id=\"" + text + "\"  ondblclick=myFunOpenProduct(null,\"" + text + "\")>";
									}
									else
									{
										Search search = Search;
										search.BodyField = search.BodyField + "<tr id=\"" + text + "\"  ondblclick=myFunSuccessInputOutput(\"" + text + "\",\"" + Search.ClassName + "\",\"" + Search.ID_KHO + "\")>";
									}
								}
								else if (Search.HinhThucTimKiem == 5)
								{
									Search search = Search;
									search.BodyField = search.BodyField + "<tr id=\"" + text + "\"  ondblclick=myFunSuccessPromotion_Tang(\"" + text + "\",\"" + Search.ClassName + "\",\"" + Search.ID_KHO + "\")>";
								}
								else if (Search.HinhThucTimKiem == 4)
								{
									Search search = Search;
									search.BodyField = search.BodyField + "<tr id=\"" + text + "\"  ondblclick=myFunSuccessPromotion_YC(\"" + text + "\",\"" + Search.ClassName + "\",\"" + Search.ID_KHO + "\")>";
								}
								else if (Search.HinhThucTimKiem == 6)
								{
									Search search = Search;
									search.BodyField = search.BodyField + "<tr id=\"" + text + "\"  ondblclick=myFunSuccessPromotionNHH_YC(\"" + text + "\")>";
								}
								else if (Search.HinhThucTimKiem == 7)
								{
									Search search = Search;
									search.BodyField = search.BodyField + "<tr id=\"" + text + "\"  ondblclick=myFunctionDelivery(\"Delivery\",\"AddDeliveryShipper\",\"" + text + "\")>";
								}
								else if (Search.HinhThucTimKiem == 9)
								{
									Search search = Search;
									search.BodyField = search.BodyField + "<tr id=\"" + text + "\"  ondblclick=myFunctionDelivery(\"KPI_Sale\",\"AddProductPromotion_NQ\",\"" + text + "\")>";
								}
								else if (Search.HinhThucTimKiem == 8)
								{
									Search search = Search;
									search.BodyField = search.BodyField + "<tr id=\"" + text + "\"  ondblclick=myFunctionDelivery(\"KPI_Sale\",\"AddProductPromotion_NV\",\"" + text + "\")>";
								}
								else
								{
									string text2 = Uri.EscapeDataString(stringToEscape);
									Search search = Search;
									search.BodyField = search.BodyField + "<tr id='" + text + "' ondblclick=\"myFunSuccess('" + text + "', decodeURIComponent('" + text2 + "'))\">";
								}
							}
						}
						else
						{
							Search.BodyField += "<tr>";
						}
						if (list != null && list.Count > 0)
						{
							if (Search.HinhThucTimKiem == 3)
							{
								Search search = Search;
								search.BodyField = search.BodyField + "<td style=\"white-space: nowrap; \" id=\"" + text + "\"><input type=\"checkbox\" name=\"TBL_ITEM\" id=\"" + text + "\" onchange=\"checkboxChanged()\" class=\"cbx\"></td>";
							}
							if (flag)
							{
								if ((item2 as v_v_dm_KhachHang).KHOANGCACH != 0.0)
								{
								}
								Search search = Search;
								search.BodyField = search.BodyField + "<td style=\"white-space: nowrap; \" id=\"" + propertyInfo.Name + "\">" + (((item2 as v_v_dm_KhachHang).KHOANGCACH > 1000.0) ? (((item2 as v_v_dm_KhachHang).KHOANGCACH / 1000.0).ToString("N0") + " km") : ((item2 as v_v_dm_KhachHang).KHOANGCACH.ToString("N0") + " m")) + "</td></a>";
							}
							foreach (view_web_NoteClass itmSearch in list.OrderBy((view_web_NoteClass s) => s.STT))
							{
								propertyInfo = runtimeProperties.Where((PropertyInfo propertyInfo2) => propertyInfo2.Name.ToUpper() == (string.IsNullOrEmpty(itmSearch.REPLACESEARCH) ? itmSearch.NAMECOLUMN : itmSearch.REPLACESEARCH).ToUpper()).FirstOrDefault();
								if (propertyInfo != null)
								{
									object value5 = propertyInfo.GetValue(item2);
									if (value5 != null && value5.GetType().ToString().Contains("Date"))
									{
										Search search = Search;
										search.BodyField = search.BodyField + "<td style=\"white-space: nowrap; \" id=\"" + propertyInfo.Name + "\">" + ((DateTime)value5).ToString("dd/MM/yyyy") + "</td></a>";
									}
									else if (value5 != null && value5.GetType().ToString().Contains("Bool"))
									{
										Search search = Search;
										search.BodyField = search.BodyField + "<td style=\"white-space: nowrap; \" id=\"" + propertyInfo.Name + "\"><input " + (((bool)value5) ? "checked=\"checked\"" : "") + " class=\"check-box\" disabled=\"disabled\" type=\"checkbox\"></td>";
									}
									else if (value5 != null && Utility.IsNumericType(value5.GetType()))
									{
										decimal num3 = Convert.ToDecimal(value5);
										if ("Output" == Search.ClassName && propertyInfo.Name == "QTY")
										{
											Product_Detail product_Detail = item2 as Product_Detail;
											if (product_Detail.TYLE_QD == 1.0 || product_Detail.TYLE_QD == 0.0)
											{
												Search search = Search;
												search.BodyField = search.BodyField + "<td style=\"white-space: nowrap; \" id=\"" + propertyInfo.Name + "\">" + num3.ToString("N0") + "</td>";
											}
											else if (product_Detail.TYLE_QD > 1.0)
											{
												int num4 = Convert.ToInt32(product_Detail.QTY) / Convert.ToInt32(product_Detail.TYLE_QD);
												string text3 = "";
												if (num4 > 0)
												{
													text3 = num4.ToString("N0") + " " + product_Detail.NAME_DVT;
												}
												if (product_Detail.QTY - (double)num4 * product_Detail.TYLE_QD > 0.0)
												{
													text3 = ((!string.IsNullOrEmpty(text3)) ? (text3 + " " + (product_Detail.QTY - (double)num4 * product_Detail.TYLE_QD).ToString("N0") + " " + product_Detail.NAME_DVT_QD) : (text3 + (product_Detail.QTY - (double)num4 * product_Detail.TYLE_QD).ToString("N0") + " " + product_Detail.NAME_DVT_QD));
												}
												Search search = Search;
												search.BodyField = search.BodyField + "<td style=\"white-space: nowrap; \" id=\"" + propertyInfo.Name + "\">" + text3 + "</td>";
											}
										}
										else
										{
											Search search = Search;
											search.BodyField = search.BodyField + "<td style=\"white-space: nowrap; \" id=\"" + propertyInfo.Name + "\">" + num3.ToString("N0") + "</td>";
										}
									}
									else if (itmSearch.NAMECOLUMN.ToUpper() == "PICTURE")
									{
										if (value5 != null && !string.IsNullOrEmpty(value5.ToString()))
										{
											Search search = Search;
											search.BodyField = search.BodyField + "<td style=\"white-space: nowrap; \" id=\"" + propertyInfo.Name + "\"><div class=\"thmb-prev\"><a href = \"/Images_Upload/Product/" + value5?.ToString() + "\" data-rel=\"prettyPhotoSearch\" rel=\"prettyPhotoSearch\"><img src=\"/Images_Upload/Product/" + value5?.ToString() + "\" class=\"img-responsive\" alt=\"\"></a></div></td>";
										}
										else
										{
											Search.BodyField = Search.BodyField + "<td style=\"white-space: nowrap; \" id=\"" + propertyInfo.Name + "\"><div class=\"thmb-prev\"></div></td>";
										}
									}
									else
									{
										Search search = Search;
										search.BodyField = search.BodyField + "<td style=\"white-space: nowrap; \" id=\"" + propertyInfo.Name + "\">" + value5?.ToString() + "</td>";
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
			catch (Exception e)
			{
				Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, e);
			}
			return Search;
		}

		[HttpPost]
		[ValidateInput(false)]
		public ActionResult Search([Bind(Include = "ID_KHUVUC,HinhThucTimKiem,MyModal,ShowSearchValue,SearchString,ClassName,ValueField,TextField,TrField,BodyField,ID_KHO")] Search Search)
		{
			Search.TitleSearch = Utility.GetTitleFrom(Search.ClassName);
			ApiResponse apiResponse = new ApiResponse();
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
			switch (Search.ClassName)
			{
				case "DepositCustomer":
					{
						Search.TitleSearch = Utility.GetTitleFrom("Customer");
						Search.ShowSearchValue = Utility.GetShowSearchValue<dm_KhachHang>(Search.ShowSearchValue);
						DepositController depositController = new DepositController();
						string text = clsMaHoa.Decrypt(Search.ShowSearchValue, "tmt6364");
						string text2 = ((base.Session["idNhomQuyen"] != null) ? base.Session["idNhomQuyen"].ToString() : "");
						API.LONGITUDE = Utility.Longitude;
						API.LATITUDE = Utility.Latitude;
						apiResponse = ((!(text2 != "-1")) ? Utility.GetListData<v_v_dm_KhachHang>("Customer", Search.ShowSearchValue, Search.SearchString, Utility.LOC_ID) : depositController.GetDanhSachKhachHang<v_v_dm_KhachHang>(text2, Search.SearchString, (text == "ALL") ? "" : text));
						if (!apiResponse.Success)
						{
							apiResponse.Data = new List<v_v_dm_KhachHang>();
							base.TempData["TitleError"] = apiResponse.Message;
							apiResponse.Success = false;
							apiResponse.URL = base.Url.Action("Index", "Notfound");
							break;
						}
						if (!string.IsNullOrEmpty(Search.ID_KHUVUC) && apiResponse.Data is List<v_v_dm_KhachHang>)
						{
							apiResponse.Data = (apiResponse.Data as List<v_v_dm_KhachHang>).Where((v_v_dm_KhachHang e) => e.ID_KHUVUC == Search.ID_KHUVUC).ToList();
						}
						Search = GetData<v_v_dm_KhachHang>(apiResponse, Search);
						break;
					}
				case "Input":
					{
						Search.TitleSearch = Utility.GetTitleFrom("Input");
						Search.ShowSearchValue = Utility.GetShowSearchValue<dm_HangHoa>(Search.ShowSearchValue);
						string text = clsMaHoa.Decrypt(Search.ShowSearchValue, "tmt6364");
						apiResponse = Utility.Get_DanhSachSanPhamKho<Product_Detail>(Search.ID_KHO, bolTonKho: false, "", Search.SearchString, (text == "ALL") ? "" : text);
						if (!apiResponse.Success)
						{
							apiResponse.Data = new List<Product_Detail>();
							base.TempData["TitleError"] = apiResponse.Message;
							apiResponse.Success = false;
							apiResponse.URL = base.Url.Action("Index", "Notfound");
						}
						else
						{
							Search = GetData<Product_Detail>(apiResponse, Search);
						}
						break;
					}
				case "Output":
					{
						Search.TitleSearch = Utility.GetTitleFrom("Output");
						Search.ShowSearchValue = Utility.GetShowSearchValue<dm_HangHoa>(Search.ShowSearchValue);
						string text = clsMaHoa.Decrypt(Search.ShowSearchValue, "tmt6364");
						apiResponse = Utility.Get_DanhSachSanPhamKho<Product_Detail>(Search.ID_KHO, bolTonKho: true, "", Search.SearchString, (text == "ALL") ? "" : text);
						if (!apiResponse.Success)
						{
							apiResponse.Data = new List<Product_Detail>();
							base.TempData["TitleError"] = apiResponse.Message;
							apiResponse.Success = false;
							apiResponse.URL = base.Url.Action("Index", "Notfound");
						}
						else
						{
							Search = GetData<Product_Detail>(apiResponse, Search);
						}
						break;
					}
				case "Product":
					Search.TitleSearch = Utility.GetTitleFrom("Product");
					Search.ShowSearchValue = Utility.GetShowSearchValue<dm_HangHoa>(Search.ShowSearchValue);
					apiResponse = Utility.GetListData<v_dm_HangHoa>(Search.ClassName, Search.ShowSearchValue, Search.SearchString, Utility.LOC_ID);
					Search = GetData<v_dm_HangHoa>(apiResponse, Search);
					break;
				case "Customer":
					Search.TitleSearch = Utility.GetTitleFrom("Customer");
					Search.ShowSearchValue = Utility.GetShowSearchValue<dm_KhachHang>(Search.ShowSearchValue);
					apiResponse = Utility.GetListData<v_v_dm_KhachHang>(Search.ClassName, Search.ShowSearchValue, Search.SearchString, Utility.LOC_ID);
					if (!string.IsNullOrEmpty(Search.ID_KHUVUC) && apiResponse.Data is List<v_v_dm_KhachHang>)
					{
						apiResponse.Data = (apiResponse.Data as List<v_v_dm_KhachHang>).Where((v_v_dm_KhachHang e) => e.ID_KHUVUC == Search.ID_KHUVUC).ToList();
					}
					Search = GetData<v_v_dm_KhachHang>(apiResponse, Search);
					break;
				case "GroupProduct":
					Search.TitleSearch = Utility.GetTitleFrom("GroupProduct");
					Search.ShowSearchValue = Utility.GetShowSearchValue<dm_NhomHangHoa>(Search.ShowSearchValue);
					apiResponse = Utility.GetListData<v_dm_NhomHangHoa>(Search.ClassName, Search.ShowSearchValue, Search.SearchString, Utility.LOC_ID);
					Search = GetData<v_dm_NhomHangHoa>(apiResponse, Search);
					break;
				case "Provider":
					Search.TitleSearch = Utility.GetTitleFrom("Provider");
					Search.ShowSearchValue = Utility.GetShowSearchValue<dm_NhaCungCap>(Search.ShowSearchValue);
					apiResponse = Utility.GetListData<v_dm_NhaCungCap>(Search.ClassName, Search.ShowSearchValue, Search.SearchString, Utility.LOC_ID);
					Search = GetData<v_dm_NhaCungCap>(apiResponse, Search);
					break;
				case "Employee":
					Search.TitleSearch = Utility.GetTitleFrom("Provider");
					Search.ShowSearchValue = Utility.GetShowSearchValue<dm_NhanVien>(Search.ShowSearchValue);
					apiResponse = Utility.GetListData<v_dm_NhanVien>(Search.ClassName, Search.ShowSearchValue, Search.SearchString, Utility.LOC_ID);
					Search = GetData<v_dm_NhanVien>(apiResponse, Search);
					break;
				case "Area":
					Search.TitleSearch = Utility.GetTitleFrom("Area");
					Search.ShowSearchValue = Utility.GetShowSearchValue<dm_KhuVuc>(Search.ShowSearchValue);
					apiResponse = Utility.GetListData<v_dm_KhuVuc>(Search.ClassName, Search.ShowSearchValue, Search.SearchString, Utility.LOC_ID);
					Search = GetData<v_dm_KhuVuc>(apiResponse, Search);
					break;
				case "BankAccount":
					Search.TitleSearch = Utility.GetTitleFrom("BankAccount");
					Search.ShowSearchValue = Utility.GetShowSearchValue<dm_TaiKhoanNganHang>(Search.ShowSearchValue);
					apiResponse = Utility.GetListData<v_dm_TaiKhoanNganHang>(Search.ClassName, Search.ShowSearchValue, Search.SearchString, Utility.LOC_ID);
					Search = GetData<v_dm_TaiKhoanNganHang>(apiResponse, Search);
					break;
				case "Car":
					Search.TitleSearch = Utility.GetTitleFrom("Car");
					Search.ShowSearchValue = Utility.GetShowSearchValue<dm_Xe>(Search.ShowSearchValue);
					apiResponse = Utility.GetListData<v_dm_Xe>(Search.ClassName, Search.ShowSearchValue, Search.SearchString, Utility.LOC_ID);
					Search = GetData<v_dm_Xe>(apiResponse, Search);
					break;
				case "Currency":
					Search.TitleSearch = Utility.GetTitleFrom("Currency");
					Search.ShowSearchValue = Utility.GetShowSearchValue<dm_TienTe>(Search.ShowSearchValue);
					apiResponse = Utility.GetListData<v_dm_TienTe>(Search.ClassName, Search.ShowSearchValue, Search.SearchString, Utility.LOC_ID);
					Search = GetData<v_dm_TienTe>(apiResponse, Search);
					break;
				case "GroupCustomer":
					Search.TitleSearch = Utility.GetTitleFrom("GroupCustomer");
					Search.ShowSearchValue = Utility.GetShowSearchValue<dm_NhomKhachHang>(Search.ShowSearchValue);
					apiResponse = Utility.GetListData<v_dm_NhomKhachHang>(Search.ClassName, Search.ShowSearchValue, Search.SearchString, Utility.LOC_ID);
					Search = GetData<v_dm_NhomKhachHang>(apiResponse, Search);
					break;
				case "GroupProvider":
					Search.TitleSearch = Utility.GetTitleFrom("GroupProvider");
					Search.ShowSearchValue = Utility.GetShowSearchValue<dm_NhomNhaCungCap>(Search.ShowSearchValue);
					apiResponse = Utility.GetListData<v_dm_NhomNhaCungCap>(Search.ClassName, Search.ShowSearchValue, Search.SearchString, Utility.LOC_ID);
					Search = GetData<v_dm_NhomNhaCungCap>(apiResponse, Search);
					break;
				case "Menu":
					Search.TitleSearch = Utility.GetTitleFrom("Menu");
					Search.ShowSearchValue = Utility.GetShowSearchValue<web_Menu>(Search.ShowSearchValue);
					apiResponse = Utility.GetListData<v_web_Menu>(Search.ClassName, Search.ShowSearchValue, Search.SearchString, Utility.LOC_ID);
					Search = GetData<v_web_Menu>(apiResponse, Search);
					break;
				case "Position":
					Search.TitleSearch = Utility.GetTitleFrom("Position");
					Search.ShowSearchValue = Utility.GetShowSearchValue<dm_ChucVu>(Search.ShowSearchValue);
					apiResponse = Utility.GetListData<v_dm_ChucVu>(Search.ClassName, Search.ShowSearchValue, Search.SearchString, Utility.LOC_ID);
					Search = GetData<v_dm_ChucVu>(apiResponse, Search);
					break;
				case "Promotion":
					Search.TitleSearch = Utility.GetTitleFrom("Promotion");
					Search.ShowSearchValue = Utility.GetShowSearchValue<dm_ChuongTrinhKhuyenMai>(Search.ShowSearchValue);
					apiResponse = Utility.GetListData<v_dm_ChuongTrinhKhuyenMai>(Search.ClassName, Search.ShowSearchValue, Search.SearchString, Utility.LOC_ID);
					Search = GetData<v_dm_ChuongTrinhKhuyenMai>(apiResponse, Search);
					break;
				case "Tax":
					Search.TitleSearch = Utility.GetTitleFrom("Tax");
					Search.ShowSearchValue = Utility.GetShowSearchValue<dm_ThueSuat>(Search.ShowSearchValue);
					apiResponse = Utility.GetListData<v_dm_ThueSuat>(Search.ClassName, Search.ShowSearchValue, Search.SearchString, Utility.LOC_ID);
					Search = GetData<v_dm_ThueSuat>(apiResponse, Search);
					break;
				case "TypePayment":
					Search.TitleSearch = Utility.GetTitleFrom("TypePayment");
					Search.ShowSearchValue = Utility.GetShowSearchValue<dm_LoaiPhieuChi>(Search.ShowSearchValue);
					apiResponse = Utility.GetListData<v_dm_LoaiPhieuChi>(Search.ClassName, Search.ShowSearchValue, Search.SearchString, Utility.LOC_ID);
					Search = GetData<v_dm_LoaiPhieuChi>(apiResponse, Search);
					break;
				case "TypeInput":
					Search.TitleSearch = Utility.GetTitleFrom("TypeInput");
					Search.ShowSearchValue = Utility.GetShowSearchValue<dm_LoaiPhieuNhap>(Search.ShowSearchValue);
					apiResponse = Utility.GetListData<v_dm_LoaiPhieuNhap>(Search.ClassName, Search.ShowSearchValue, Search.SearchString, Utility.LOC_ID);
					Search = GetData<v_dm_LoaiPhieuNhap>(apiResponse, Search);
					break;
				case "TypeReceipt":
					Search.TitleSearch = Utility.GetTitleFrom("TypeReceipt");
					Search.ShowSearchValue = Utility.GetShowSearchValue<dm_LoaiPhieuThu>(Search.ShowSearchValue);
					apiResponse = Utility.GetListData<v_dm_LoaiPhieuThu>(Search.ClassName, Search.ShowSearchValue, Search.SearchString, Utility.LOC_ID);
					Search = GetData<v_dm_LoaiPhieuThu>(apiResponse, Search);
					break;
				case "TypeOutput":
					Search.TitleSearch = Utility.GetTitleFrom("TypeOutput");
					Search.ShowSearchValue = Utility.GetShowSearchValue<dm_LoaiPhieuXuat>(Search.ShowSearchValue);
					apiResponse = Utility.GetListData<v_dm_LoaiPhieuXuat>(Search.ClassName, Search.ShowSearchValue, Search.SearchString, Utility.LOC_ID);
					Search = GetData<v_dm_LoaiPhieuXuat>(apiResponse, Search);
					break;
				case "Unit":
					Search.TitleSearch = Utility.GetTitleFrom("Unit");
					Search.ShowSearchValue = Utility.GetShowSearchValue<dm_DonViTinh>(Search.ShowSearchValue);
					apiResponse = Utility.GetListData<v_dm_DonViTinh>(Search.ClassName, Search.ShowSearchValue, Search.SearchString, Utility.LOC_ID);
					Search = GetData<v_dm_DonViTinh>(apiResponse, Search);
					break;
				case "User":
					Search.TitleSearch = Utility.GetTitleFrom("User");
					Search.ShowSearchValue = Utility.GetShowSearchValue<AspNetUsers>(Search.ShowSearchValue);
					apiResponse = Utility.GetListData<v_AspNetUsers>(Search.ClassName, Search.ShowSearchValue, Search.SearchString, Utility.LOC_ID);
					Search = GetData<v_AspNetUsers>(apiResponse, Search);
					break;
				case "Depot":
					Search.TitleSearch = Utility.GetTitleFrom("Depot");
					Search.ShowSearchValue = Utility.GetShowSearchValue<dm_Kho>(Search.ShowSearchValue);
					apiResponse = Utility.GetListData<v_dm_Kho>(Search.ClassName, Search.ShowSearchValue, Search.SearchString, Utility.LOC_ID);
					Search = GetData<v_dm_Kho>(apiResponse, Search);
					break;
				case "GroupPermissions":
					Search.TitleSearch = Utility.GetTitleFrom("GroupPermissions");
					Search.ShowSearchValue = Utility.GetShowSearchValue<web_NhomQuyen>(Search.ShowSearchValue);
					apiResponse = Utility.GetListData<v_web_NhomQuyen>(Search.ClassName, Search.ShowSearchValue, Search.SearchString, Utility.LOC_ID);
					Search = GetData<v_web_NhomQuyen>(apiResponse, Search);
					break;
				case "Department":
					Search.TitleSearch = Utility.GetTitleFrom("Department");
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
				apiResponse.Success = false;
				apiResponse.URL = base.Url.Action("Index", "Admin");
				return new JsonResult
				{
					Data = apiResponse,
					JsonRequestBehavior = JsonRequestBehavior.AllowGet,
					MaxJsonLength = int.MaxValue
				};
			}
			KeyCode = KeyCode.Trim().ToUpper();
			string text = "";
			if (KeyCode.StartsWith("PDH"))
			{
				text = "Deposit";
				apiResponse = GetValue<v_v_ct_PhieuDatHang>(apiResponse, text, KeyCode);
				if (apiResponse.Detail != null)
				{
					apiResponse.ID = (apiResponse.Detail as v_v_ct_PhieuDatHang).ID;
				}
			}
			if (KeyCode.StartsWith("PT"))
			{
				text = "Receipt";
				apiResponse = GetValue<v_v_ct_PhieuThu>(apiResponse, text, KeyCode);
				if (apiResponse.Detail != null)
				{
					apiResponse.ID = (apiResponse.Detail as v_v_ct_PhieuThu).ID;
				}
			}
			if (KeyCode.StartsWith("PC"))
			{
				text = "Payment";
				apiResponse = GetValue<v_v_ct_PhieuChi>(apiResponse, text, KeyCode);
				if (apiResponse.Detail != null)
				{
					apiResponse.ID = (apiResponse.Detail as v_v_ct_PhieuChi).ID;
				}
			}
			if (KeyCode.StartsWith("PN"))
			{
				text = "Input";
				apiResponse = GetValue<v_v_ct_PhieuNhap>(apiResponse, text, KeyCode);
				if (apiResponse.Detail != null)
				{
					apiResponse.ID = (apiResponse.Detail as v_v_ct_PhieuNhap).ID;
				}
			}
			if (KeyCode.StartsWith("PX"))
			{
				text = "Output";
				apiResponse = GetValue<v_v_ct_PhieuXuat>(apiResponse, text, KeyCode);
				if (apiResponse.Detail != null)
				{
					apiResponse.ID = (apiResponse.Detail as v_v_ct_PhieuXuat).ID;
				}
			}
			if (KeyCode.StartsWith("PGH"))
			{
				text = "Delivery";
				apiResponse = GetValue<v_v_ct_PhieuGiaoHang>(apiResponse, text, KeyCode);
				if (apiResponse.Detail != null)
				{
					apiResponse.ID = (apiResponse.Detail as v_v_ct_PhieuGiaoHang).ID;
				}
			}
			if (!string.IsNullOrEmpty(apiResponse.ID))
			{
				apiResponse.URL = base.Url.Action("Index", text, new
				{
					MAPHIEU = KeyCode,
					IDCODE = apiResponse.ID
				});
			}
			else
			{
				apiResponse.Message = "Không tìm thấy phiếu";
				apiResponse.Success = true;
			}
			apiResponse.NAME = text;
			return new JsonResult
			{
				Data = apiResponse,
				JsonRequestBehavior = JsonRequestBehavior.AllowGet,
				MaxJsonLength = int.MaxValue
			};
		}

		private ApiResponse GetValue<T>(ApiResponse apiResponse, string NameController, string KeyCode)
		{
			apiResponse = Utility.GetListDataCode<T>(NameController, "MAPHIEU.ToUpper() == @0", KeyCode.ToUpper(), Utility.LOC_ID);
			if (!apiResponse.Success)
			{
				base.TempData["TitleError"] = apiResponse.Message;
				apiResponse.Success = false;
				apiResponse.URL = base.Url.Action("Index", "Notfound");
				return apiResponse;
			}
            if (apiResponse.Data != null && apiResponse.Data is List<T> list && list.Count > 0)
            {
                apiResponse.Detail = list.FirstOrDefault();
            }
            return apiResponse;
		}
	}
}
