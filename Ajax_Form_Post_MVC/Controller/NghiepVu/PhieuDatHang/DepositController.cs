using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using DatabaseTHP;
using DatabaseTHP.Class;
using DatabaseTHP.StoredProcedure;
using DatabaseTHP.StoredProcedure.Parameter;
using MVC_QuanLyTHP.Class;
using MVC_QuanLyTHP.Models;
using Newtonsoft.Json;
using PagedList;

namespace MVC_QuanLyTHP.Controllers
{

	public class DepositController : Controller
	{
		public ActionResult Index(int Page = 1, string ID_DEPOT = "", string ID_KHUVUC = "", DateTime? FromDate = null, DateTime? ToDate = null, string SearchString = "", string ShowSearchValue = "", string MAPHIEU = "", string IDCODE = "")
		{
			try
			{
				if (Utility.KiemTra())
				{
					return RedirectToAction("Index", "Admin");
				}
				if (!Utility.KiemTraQuyen("Deposit", "View"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				ShowSearchValue = Utility.GetShowSearchValue<ct_PhieuDatHang>(ShowSearchValue);
				ApiResponse apiResponse = new ApiResponse();
				IPagedList<v_ct_PhieuDatHang> iPagedList = new List<v_ct_PhieuDatHang>().OrderByDescending((v_ct_PhieuDatHang s) => s.MAPHIEU).ToList().ToPagedList(Page, Utility.GetPageSize());
				if (FromDate.HasValue || !string.IsNullOrEmpty(IDCODE))
				{
					if (!string.IsNullOrEmpty(IDCODE))
					{
						apiResponse = Utility.Get_DanhSachPhieuDatHang<v_ct_PhieuDatHang>("", null, null, MAPHIEU, IDCODE, ID_KHUVUC);
					}
					if (FromDate.HasValue)
					{
						apiResponse = Utility.Get_DanhSachPhieuDatHang<v_ct_PhieuDatHang>(ID_DEPOT, FromDate, ToDate, SearchString, "", ID_KHUVUC);
					}
					if (!apiResponse.Success)
					{
						base.TempData["TitleError"] = apiResponse.Message;
						return RedirectToAction("Index", "Notfound");
					}
					if (apiResponse.Data != null)
					{
						Login_Model Login_Model = (Login_Model)base.Session["Login_Model"];
						if (Utility.KiemTraQuyen("Deposit", "AllData"))
						{
							iPagedList = (apiResponse.Data as List<v_ct_PhieuDatHang>).OrderByDescending((v_ct_PhieuDatHang s) => s.MAPHIEU).ToList().ToPagedList(Page, Utility.GetPageSize());
						}
						else if (Utility.KiemTraQuyen("Deposit", "UserData"))
						{
							iPagedList = (from s in apiResponse.Data as List<v_ct_PhieuDatHang>
										  where s.ID_NHANVIEN == Login_Model.iduser
										  orderby s.MAPHIEU descending
										  select s).ToList().ToPagedList(Page, Utility.GetPageSize());
						}
					}
				}
				v_v_ct_PhieuDatHang v_v_ct_PhieuDatHang2 = new v_v_ct_PhieuDatHang();
				v_v_ct_PhieuDatHang2.IPagedList = iPagedList;
				v_v_ct_PhieuDatHang2.lstdm_ThueSuat = new List<v_dm_ThueSuat>();
				v_v_ct_PhieuDatHang2.lstdm_ThueSuat = Utility.GetListData<v_dm_ThueSuat>("Tax", "", "", Utility.LOC_ID).Data as List<v_dm_ThueSuat>;
				v_v_ct_PhieuDatHang2.lstdm_Kho = new List<v_dm_Kho>();
				v_v_ct_PhieuDatHang2.lstdm_Kho = Utility.GetListData<v_dm_Kho>("Depot", "", "", Utility.LOC_ID).Data as List<v_dm_Kho>;
				v_v_ct_PhieuDatHang2.lstdm_KhuVuc = new List<v_dm_KhuVuc>();
				v_v_ct_PhieuDatHang2.lstdm_KhuVuc = Utility.GetListData<v_dm_KhuVuc>("Area", "", "", Utility.LOC_ID).Data as List<v_dm_KhuVuc>;
				v_v_ct_PhieuDatHang2.ID_KHUVUC = ID_KHUVUC;
				base.ViewBag.ID_KHO_DF = (string.IsNullOrEmpty(ID_DEPOT) ? v_v_ct_PhieuDatHang2.lstdm_Kho.FirstOrDefault((v_dm_Kho e) => e.ISDEFAULT).ID : ID_DEPOT);
				base.ViewBag.searchValue = SearchString;
				base.ViewBag.showsearchValue = ShowSearchValue;
				base.ViewBag.fromdate = (FromDate.HasValue ? FromDate.Value.ToString("yyyy-MM-dd") : Utility.CurrentTime.ToString("yyyy-MM-dd"));
				base.ViewBag.todate = (ToDate.HasValue ? ToDate.Value.ToString("yyyy-MM-dd") : Utility.CurrentTime.ToString("yyyy-MM-dd"));
				base.ViewBag.PermissionEdit = Utility.KiemTraQuyen("Deposit", "Edit");
				base.ViewBag.PermissionDelete = Utility.KiemTraQuyen("Deposit", "Delete");
				base.ViewBag.PermissionCreate = Utility.KiemTraQuyen("Deposit", "Create");
				base.ViewBag.PermissionCreateInput = Utility.KiemTraQuyen("Deposit", "CreateInput");
				return View(v_v_ct_PhieuDatHang2);
			}
			catch (Exception ex)
			{
				Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
				base.TempData["TitleError"] = "Đã có lỗi phát sinh trong phiên làm việc!";
				base.TempData["DetailError"] = ex.Message;
				return RedirectToAction("Index", "Notfound");
			}
		}

		public ActionResult Create(int type = 2)
		{
			base.Session["IntWidth"] = type;
			try
			{
				if (Utility.KiemTra())
				{
					return RedirectToAction("Index", "Admin");
				}
				if (!Utility.KiemTraQuyen("Deposit", "View"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				v_v_ct_PhieuDatHang v_v_ct_PhieuDatHang2 = new v_v_ct_PhieuDatHang();
				if (type == 2)
				{
					v_v_ct_PhieuDatHang2.NGAYLAP = Utility.CurrentTime;
					v_v_ct_PhieuDatHang2.LOC_ID = Utility.LOC_ID;
					v_v_ct_PhieuDatHang2.SOPHIEU = Utility.GetMaxID((ct_PhieuDatHang)v_v_ct_PhieuDatHang2, Utility.LOC_ID, v_v_ct_PhieuDatHang2.NGAYLAP.ToString("yyyy-MM-dd"));
					v_v_ct_PhieuDatHang2.MAPHIEU = API.GetMaPhieu("Deposit", v_v_ct_PhieuDatHang2.NGAYLAP, v_v_ct_PhieuDatHang2.SOPHIEU);
					v_v_ct_PhieuDatHang2.lstdm_ThueSuat = new List<v_dm_ThueSuat>();
					v_v_ct_PhieuDatHang2.lstdm_ThueSuat = Utility.GetListData<v_dm_ThueSuat>("Tax", "", "", Utility.LOC_ID).Data as List<v_dm_ThueSuat>;
					v_v_ct_PhieuDatHang2.lstdm_Kho = new List<v_dm_Kho>();
					v_v_ct_PhieuDatHang2.lstdm_Kho = Utility.GetListData<v_dm_Kho>("Depot", "", "", Utility.LOC_ID).Data as List<v_dm_Kho>;
					v_v_ct_PhieuDatHang2.lstdm_KhachHang = new List<ComboboxFrom>();
					v_v_ct_PhieuDatHang2.lstdm_KhachHang = GetDanhSachKhachHangCombobox().Data as List<ComboboxFrom>;
					v_v_ct_PhieuDatHang2.lstAspNetUsers = new List<v_AspNetUsers>();
					v_v_ct_PhieuDatHang2.lstAspNetUsers = Utility.GetListData<v_AspNetUsers>("User").Data as List<v_AspNetUsers>;
					v_dm_Kho v_dm_Kho2 = v_v_ct_PhieuDatHang2.lstdm_Kho.Where((v_dm_Kho e) => e.ISDEFAULT).FirstOrDefault();
					if (v_dm_Kho2 != null)
					{
						v_v_ct_PhieuDatHang2.ID_KHO = v_dm_Kho2.ID;
					}
				}
				else
				{
					v_v_ct_PhieuDatHang2.lstdm_ThueSuat = new List<v_dm_ThueSuat>();
					v_v_ct_PhieuDatHang2.lstdm_Kho = new List<v_dm_Kho>();
					v_v_ct_PhieuDatHang2.lstdm_KhachHang = new List<ComboboxFrom>();
					v_v_ct_PhieuDatHang2.lstAspNetUsers = new List<v_AspNetUsers>();
				}
				List<Product_Detail> value = new List<Product_Detail>();
				base.Session["lstProductInput"] = value;
				ApiResponse danhSachNhomSanPham = GetDanhSachNhomSanPham();
				string text = "";
				if (!danhSachNhomSanPham.Success)
				{
					base.TempData["TitleError"] = danhSachNhomSanPham.Message;
					return RedirectToAction("Index", "Notfound");
				}
				List<web_Sp_Get_DSNhomSanPham_Result> list = danhSachNhomSanPham.Data as List<web_Sp_Get_DSNhomSanPham_Result>;
				text = "<button class='btnGroup active' onclick='myFunctionPage(\"all\", \"\",\"collapseOneDeposit\")' id= \"all\">Show all</button>";
				foreach (web_Sp_Get_DSNhomSanPham_Result item in list)
				{
					text = text + "<button class='btnGroup' onclick='myFunctionPage(\"" + item.ID + "\", \"\",\"collapseOneDeposit\")' id= \"" + item.ID + "\"> " + item.NAME + "</button>";
				}
				if (!string.IsNullOrEmpty(text))
				{
					text += "<button class='btnGroup' onclick='myFunctionLoadGroup(\"collapseOneDeposit\")'><span class='glyphicon glyphicon-refresh'></span></button>";
				}
				text = myProduct(text, "collapseOneDeposit");
				base.ViewBag.NhomHang = text;
				base.ViewBag.PermissionCreateUser = Utility.KiemTraQuyen("Deposit", "CreateUser");
				return View(v_v_ct_PhieuDatHang2);
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
		[ValidateAntiForgeryToken]
		public ActionResult CreatePopup([Bind(Include = "LOC_ID,ID,ID_KHO,MAPHIEU,SOPHIEU,NGAYLAP,ID_KHACHHANG,ID_NHANVIEN,GHICHU,TONGTHANHTIEN,CHIETKHAU,TONGTIENGIAMGIA,TONGTIENVAT,TONGTIEN,BUTTONTYPE,ADDRESS,TEL")] v_v_ct_PhieuDatHang ct_PhieuDatHang)
		{
			ApiResponse apiResponse = new ApiResponse();
			try
			{
				if (Utility.KiemTra())
				{
					return RedirectToAction("Index", "Admin");
				}
				if (!Utility.KiemTraQuyen("Deposit", "Create"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				ct_PhieuDatHang.lstct_PhieuDatHang_ChiTiet = new List<v_ct_PhieuDatHang_ChiTiet>();
				List<Product_Detail> list = new List<Product_Detail>();
				IEnumerable<string> enumerable = base.Request.Form.AllKeys.Where((string e) => e.StartsWith("txt"));
				if (enumerable == null || enumerable.Count() == 0)
				{
					base.ModelState.AddModelError("lstct_PhieuDatHang_ChiTiet", "Thêm danh sách hàng hóa.");
				}
				else
				{
					v_ct_PhieuDatHang_ChiTiet v_ct_PhieuDatHang_ChiTiet2 = new v_ct_PhieuDatHang_ChiTiet();
					foreach (string item in enumerable)
					{
						string[] array = item.ToString().Split('|');
						string[] values = base.HttpContext.Request.Params.GetValues(item.ToString());
						string value = clsMaHoa.Decrypt(array[1].ToString(), "tmt6364");
						Product_Detail product_Detail = JsonConvert.DeserializeObject<Product_Detail>(value);
						if (v_ct_PhieuDatHang_ChiTiet2.ID != product_Detail.ID)
						{
							v_ct_PhieuDatHang_ChiTiet2 = new v_ct_PhieuDatHang_ChiTiet();
							v_ct_PhieuDatHang_ChiTiet2 = JsonConvert.DeserializeObject<v_ct_PhieuDatHang_ChiTiet>(value);
							v_ct_PhieuDatHang_ChiTiet2.LOC_ID = ct_PhieuDatHang.LOC_ID;
							ct_PhieuDatHang.lstct_PhieuDatHang_ChiTiet.Add(v_ct_PhieuDatHang_ChiTiet2);
							list.Add(product_Detail);
						}
						Utility.EditObject(v_ct_PhieuDatHang_ChiTiet2, array[0].ToString().Substring(3, array[0].ToString().Length - 3), values[0]);
					}
				}
				if (ct_PhieuDatHang.BUTTONTYPE == "GetPromotion")
				{
					apiResponse = Utility.Create(ct_PhieuDatHang.lstct_PhieuDatHang_ChiTiet, "Deposit/" + Utility.LOC_ID);
					list = JsonConvert.DeserializeObject<List<Product_Detail>>(apiResponse.Data.ToString());
					base.Session["lstProductInput"] = list;
					apiResponse.GETPROMOTION = Utility.GetProductInputOutput(list, "Deposit_Temp");
					ApiResponse apiResponse2 = apiResponse;
					int sOPHIEU = (ct_PhieuDatHang.SOPHIEU = Utility.GetMaxID((ct_PhieuDatHang)ct_PhieuDatHang, Utility.LOC_ID, ct_PhieuDatHang.NGAYLAP.ToString("yyyy-MM-dd")));
					apiResponse2.SOPHIEU = sOPHIEU;
					ct_PhieuDatHang.MAPHIEU = API.GetMaPhieu("Deposit", ct_PhieuDatHang.NGAYLAP, ct_PhieuDatHang.SOPHIEU);
					apiResponse.NewID = ct_PhieuDatHang.ID;
					apiResponse.MAPHIEU = ct_PhieuDatHang.MAPHIEU;
				}
				if (ct_PhieuDatHang.BUTTONTYPE == "Save")
				{
					if (!string.IsNullOrEmpty(ct_PhieuDatHang.ID_KHACHHANG))
					{
						string text = CheckCongNoKhachHang(ct_PhieuDatHang.ID_KHACHHANG, ct_PhieuDatHang.TONGTIEN);
						if (!string.IsNullOrEmpty(text))
						{
							base.ModelState.AddModelError("ID_KHACHHANG", "Công nợ vượt: " + text);
						}
					}
					if (base.ModelState.IsValid)
					{
						ct_PhieuDatHang.NGAYLAP = ct_PhieuDatHang.NGAYLAP.AddHours(Utility.CurrentTime.Hour).AddMinutes(Utility.CurrentTime.Minute);
						ct_PhieuDatHang.ID = Guid.NewGuid().ToString();
						ct_PhieuDatHang.LOC_ID = Utility.LOC_ID;
						ct_PhieuDatHang.ID_NGUOITAO = base.Session["idUser"].ToString();
						ct_PhieuDatHang.THOIGIANTHEM = Utility.CurrentTime;
						if (string.IsNullOrEmpty(ct_PhieuDatHang.ID_NHANVIEN))
						{
							ct_PhieuDatHang.ID_NHANVIEN = base.Session["idUser"].ToString();
						}
						apiResponse = Utility.Create((v_ct_PhieuDatHang)ct_PhieuDatHang, "Deposit");
						if (apiResponse.Success)
						{
							ct_PhieuDatHang.NGAYLAP = Utility.CurrentTime;
							ApiResponse apiResponse3 = apiResponse;
							int sOPHIEU = (ct_PhieuDatHang.SOPHIEU = Utility.GetMaxID((ct_PhieuDatHang)ct_PhieuDatHang, Utility.LOC_ID, ct_PhieuDatHang.NGAYLAP.ToString("yyyy-MM-dd")));
							apiResponse3.SOPHIEU = sOPHIEU;
							ct_PhieuDatHang.MAPHIEU = API.GetMaPhieu("Deposit", ct_PhieuDatHang.NGAYLAP, ct_PhieuDatHang.SOPHIEU);
							apiResponse.NewID = Guid.NewGuid().ToString();
							apiResponse.MAPHIEU = ct_PhieuDatHang.MAPHIEU;
							if (apiResponse.Data != null)
							{
								ct_PhieuDatHang = JsonConvert.DeserializeObject<v_v_ct_PhieuDatHang>(apiResponse.Data.ToString());
							}
							list = new List<Product_Detail>();
						}
						else
						{
							base.ModelState.AddModelError(string.Empty, apiResponse.Message);
							if (apiResponse.CheckValue)
							{
								ct_PhieuDatHang.NGAYLAP = Utility.CurrentTime;
								ApiResponse apiResponse4 = apiResponse;
								int sOPHIEU = (ct_PhieuDatHang.SOPHIEU = Utility.GetMaxID((ct_PhieuDatHang)ct_PhieuDatHang, Utility.LOC_ID, ct_PhieuDatHang.NGAYLAP.ToString("yyyy-MM-dd")));
								apiResponse4.SOPHIEU = sOPHIEU;
								ct_PhieuDatHang.MAPHIEU = API.GetMaPhieu("Deposit", ct_PhieuDatHang.NGAYLAP, ct_PhieuDatHang.SOPHIEU);
								apiResponse.NewID = Guid.NewGuid().ToString();
								apiResponse.MAPHIEU = ct_PhieuDatHang.MAPHIEU;
							}
						}
					}
					else
					{
						apiResponse.Success = false;
						apiResponse.Data = Utility.GetModelState(base.ModelState, "Deposit");
					}
				}
				base.Session["lstProductInput"] = list;
				apiResponse.ID = ct_PhieuDatHang.ID;
				ct_PhieuDatHang.lstdm_KhachHang = new List<ComboboxFrom>();
				ct_PhieuDatHang.lstdm_KhachHang = GetDanhSachKhachHangCombobox().Data as List<ComboboxFrom>;
				ct_PhieuDatHang.lstdm_Kho = new List<v_dm_Kho>();
				ct_PhieuDatHang.lstdm_Kho = Utility.GetListData<v_dm_Kho>("Depot", "", "", Utility.LOC_ID).Data as List<v_dm_Kho>;
				ct_PhieuDatHang.lstAspNetUsers = new List<v_AspNetUsers>();
				ct_PhieuDatHang.lstAspNetUsers = Utility.GetListData<v_AspNetUsers>("User").Data as List<v_AspNetUsers>;
				List<ValueEdit> list2 = Utility.ConvertobjectToView(ct_PhieuDatHang);
				apiResponse.ProductCombo = Utility.GetProductInputOutput(list, "Deposit_Temp");
				apiResponse.GETPROMOTION = apiResponse.ProductCombo;
				list2.Add(new ValueEdit
				{
					Key = "tbodyTempItemInput",
					Value = apiResponse.ProductCombo
				});
				apiResponse.Detail = list2;
				apiResponse.TYPE = ct_PhieuDatHang.BUTTONTYPE;
				return new JsonResult
				{
					Data = apiResponse,
					JsonRequestBehavior = JsonRequestBehavior.AllowGet,
					MaxJsonLength = int.MaxValue
				};
			}
			catch (Exception ex)
			{
				apiResponse.Success = false;
				apiResponse.Message = ex.Message;
				Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
				base.TempData["TitleError"] = "Đã có lỗi phát sinh trong phiên làm việc!";
				base.TempData["DetailError"] = ex.Message;
				return new JsonResult
				{
					Data = apiResponse,
					JsonRequestBehavior = JsonRequestBehavior.AllowGet,
					MaxJsonLength = int.MaxValue
				};
			}
		}

		public ActionResult Edit(string id, int type = 2)
		{
			ApiResponse apiResponse = new ApiResponse();
			base.Session["IntWidth"] = type;
			try
			{
				if (Utility.KiemTra())
				{
					return RedirectToAction("Index", "Admin");
				}
				if (!Utility.KiemTraQuyen("Deposit", "Edit"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				List<Product_Detail> list = new List<Product_Detail>();
				v_v_ct_PhieuDatHang v_v_ct_PhieuDatHang2 = new v_v_ct_PhieuDatHang();
				if (!string.IsNullOrEmpty(id))
				{
					apiResponse = Utility.GetDetail<v_v_ct_PhieuDatHang>(Utility.LOC_ID + "/" + id, "Deposit");
					if (!apiResponse.Success)
					{
						base.TempData["TitleError"] = apiResponse.Message;
						return RedirectToAction("Index", "Notfound");
					}
					if (apiResponse.Data != null)
					{
						v_v_ct_PhieuDatHang2 = apiResponse.Data as v_v_ct_PhieuDatHang;
					}
					foreach (v_ct_PhieuDatHang_ChiTiet item in v_v_ct_PhieuDatHang2.lstct_PhieuDatHang_ChiTiet)
					{
						list.Add(Utility.ConvertobjectToProduct_Detail(item, new Product_Detail()));
					}
					if (!string.IsNullOrEmpty(v_v_ct_PhieuDatHang2.ID_PHIEUXUAT))
					{
						base.TempData["TitleError"] = "Phiếu đã thực hiện đặt hàng! Nên không thể sửa phiếu!";
						return RedirectToAction("Index", "Notfound");
					}
				}
				v_v_ct_PhieuDatHang2.lstdm_Kho = new List<v_dm_Kho>();
				v_v_ct_PhieuDatHang2.lstdm_KhachHang = new List<ComboboxFrom>();
				v_v_ct_PhieuDatHang2.lstdm_ThueSuat = new List<v_dm_ThueSuat>();
				apiResponse = GetDanhSachNhomSanPham();
				string text = "";
				if (!apiResponse.Success)
				{
					base.TempData["TitleError"] = apiResponse.Message;
					return RedirectToAction("Index", "Notfound");
				}
				List<web_Sp_Get_DSNhomSanPham_Result> list2 = apiResponse.Data as List<web_Sp_Get_DSNhomSanPham_Result>;
				text = "<button class='btnGroup active' onclick='myFunctionPage(\"all\", \"\",\"collapseOneDepositEdit\")' id= \"all\">Show all</button>";
				foreach (web_Sp_Get_DSNhomSanPham_Result item2 in list2)
				{
					text = text + "<button class='btnGroup' onclick='myFunctionPage(\"" + item2.ID + "\", \"\",\"collapseOneDepositEdit\")' id= \"" + item2.ID + "\"> " + item2.NAME + "</button>";
				}
				if (!string.IsNullOrEmpty(text))
				{
					text += "<button class='btnGroup' onclick='myFunctionLoadGroup(\"collapseOneDepositEdit\")'><span class='glyphicon glyphicon-refresh'></span></button>";
				}
				base.Session["lstProductInput"] = list;
				base.ViewBag.NhomHang = text;
				apiResponse.ProductCombo = Utility.GetProductInputOutput(list, "Deposit_Temp");
				base.ViewBag.DatHang = apiResponse.ProductCombo;
				return View(v_v_ct_PhieuDatHang2);
			}
			catch (Exception ex)
			{
				Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
				base.TempData["TitleError"] = "Đã có lỗi phát sinh trong phiên làm việc!";
				base.TempData["DetailError"] = ex.Message;
				return RedirectToAction("Index", "Notfound");
			}
		}

		public ActionResult EditPopup(string id)
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
				if (!Utility.KiemTraQuyen("Deposit", "Edit"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					apiResponse.Success = false;
					apiResponse.URL = base.Url.Action("Index", "Notfound");
					return new JsonResult
					{
						Data = apiResponse,
						JsonRequestBehavior = JsonRequestBehavior.AllowGet,
						MaxJsonLength = int.MaxValue
					};
				}
				List<Product_Detail> list = new List<Product_Detail>();
				v_v_ct_PhieuDatHang ct_PhieuDatHang2 = new v_v_ct_PhieuDatHang();
				if (!string.IsNullOrEmpty(id))
				{
					apiResponse = Utility.GetDetail<v_v_ct_PhieuDatHang>(Utility.LOC_ID + "/" + id, "Deposit");
					if (!apiResponse.Success)
					{
						base.TempData["TitleError"] = apiResponse.Message;
						return RedirectToAction("Index", "Notfound");
					}
					if (apiResponse.Data != null)
					{
						ct_PhieuDatHang2 = apiResponse.Data as v_v_ct_PhieuDatHang;
					}
				}
				foreach (v_ct_PhieuDatHang_ChiTiet item in ct_PhieuDatHang2.lstct_PhieuDatHang_ChiTiet)
				{
					list.Add(Utility.ConvertobjectToProduct_Detail(item, new Product_Detail()));
				}
				if (!string.IsNullOrEmpty(ct_PhieuDatHang2.ID_PHIEUXUAT))
				{
					base.TempData["TitleError"] = "Phiếu đã thực hiện đặt hàng! Nên không thể sửa phiếu!";
					apiResponse.Success = false;
					apiResponse.URL = base.Url.Action("Index", "Notfound");
					return new JsonResult
					{
						Data = apiResponse,
						JsonRequestBehavior = JsonRequestBehavior.AllowGet,
						MaxJsonLength = int.MaxValue
					};
				}
				ct_PhieuDatHang2.lstdm_Kho = new List<v_dm_Kho>();
				ct_PhieuDatHang2.lstdm_Kho = Utility.GetListData<v_dm_Kho>("Depot", "", "", Utility.LOC_ID).Data as List<v_dm_Kho>;
				ct_PhieuDatHang2.lstdm_KhachHang = new List<ComboboxFrom>();
				ct_PhieuDatHang2.lstdm_KhachHang = GetDanhSachKhachHangCombobox().Data as List<ComboboxFrom>;
				if (ct_PhieuDatHang2.lstdm_KhachHang != null)
				{
					ComboboxFrom comboboxFrom = ct_PhieuDatHang2.lstdm_KhachHang.Where((ComboboxFrom e) => e.ID == ct_PhieuDatHang2.ID_KHACHHANG).FirstOrDefault();
					if (comboboxFrom == null)
					{
						ApiResponse detail = Utility.GetDetail<ComboboxFrom>(Utility.LOC_ID + "/" + ct_PhieuDatHang2.ID_KHACHHANG, "Customer");
						if (!detail.Success)
						{
							base.TempData["TitleError"] = detail.Message;
							return RedirectToAction("Index", "Notfound");
						}
						if (detail.Data != null)
						{
							ct_PhieuDatHang2.lstdm_KhachHang.Add(detail.Data as ComboboxFrom);
						}
					}
				}
				ct_PhieuDatHang2.lstdm_ThueSuat = new List<v_dm_ThueSuat>();
				ct_PhieuDatHang2.lstdm_ThueSuat = Utility.GetListData<v_dm_ThueSuat>("Tax", "", "", Utility.LOC_ID).Data as List<v_dm_ThueSuat>;
				ct_PhieuDatHang2.lstAspNetUsers = new List<v_AspNetUsers>();
				ct_PhieuDatHang2.lstAspNetUsers = Utility.GetListData<v_AspNetUsers>("User").Data as List<v_AspNetUsers>;
				base.Session["lstProductInput"] = list;
				List<ValueEdit> list2 = Utility.ConvertobjectTo(ct_PhieuDatHang2);
				apiResponse.ProductCombo = Utility.GetProductInputOutput(list, "Deposit_Temp", bolTinhLai: false, ct_PhieuDatHang2.TONGTIENGIAMGIA, ct_PhieuDatHang2.TONGTHANHTIEN, ct_PhieuDatHang2.TONGTIENVAT, ct_PhieuDatHang2.TONGTIEN);
				list2.Add(new ValueEdit
				{
					Key = "tbodyTempItemInputEdit",
					Value = apiResponse.ProductCombo
				});
				apiResponse = GetDanhSachNhomSanPham();
				string text = "";
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
				List<web_Sp_Get_DSNhomSanPham_Result> list3 = apiResponse.Data as List<web_Sp_Get_DSNhomSanPham_Result>;
				text = "<button class='btnGroup active' onclick='myFunctionPage(\"all\", \"\",\"collapseOneDepositEdit\")' id= \"all\">Show all</button>";
				foreach (web_Sp_Get_DSNhomSanPham_Result item2 in list3)
				{
					text = text + "<button class='btnGroup' onclick='myFunctionPage(\"" + item2.ID + "\", \"\",\"collapseOneDepositEdit\")' id= \"" + item2.ID + "\"> " + item2.NAME + "</button>";
				}
				if (!string.IsNullOrEmpty(text))
				{
					text += "<button class='btnGroup' onclick='myFunctionLoadGroup(\"collapseOneDepositEdit\")'><span class='glyphicon glyphicon-refresh'></span></button>";
				}
				base.ViewBag.NhomHang = text;
				list2.Add(new ValueEdit
				{
					Key = "myProductEdit",
					Value = myProduct(text, "collapseOneDepositEdit")
				});
				apiResponse.Detail = list2;
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

		[HttpPost]
		[ValidateInput(false)]
		[ValidateAntiForgeryToken]
		public ActionResult Edit([Bind(Include = "LOC_ID,ID,ID_KHO,MAPHIEU,SOPHIEU,NGAYLAP,ID_KHACHHANG,ID_NHANVIEN,GHICHU,TONGTHANHTIEN,CHIETKHAU,TONGTIENGIAMGIA,TONGTIENVAT,TONGTIEN,BUTTONTYPE,ADDRESS,TEL")] v_v_ct_PhieuDatHang ct_PhieuDatHang)
		{
			try
			{
				if (Utility.KiemTra())
				{
					return RedirectToAction("Index", "Admin");
				}
				if (!Utility.KiemTraQuyen("Deposit", "Create"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				List<Product_Detail> list = new List<Product_Detail>();
				ApiResponse apiResponse = new ApiResponse();
				ct_PhieuDatHang.lstct_PhieuDatHang_ChiTiet = new List<v_ct_PhieuDatHang_ChiTiet>();
				IEnumerable<string> enumerable = base.Request.Form.AllKeys.Where((string e) => e.StartsWith("txt"));
				if (enumerable == null || enumerable.Count() == 0)
				{
					base.ModelState.AddModelError("lstct_PhieuDatHang_ChiTiet", "Thêm danh sách hàng hóa.");
				}
				else
				{
					v_ct_PhieuDatHang_ChiTiet v_ct_PhieuDatHang_ChiTiet2 = new v_ct_PhieuDatHang_ChiTiet();
					foreach (string item in enumerable)
					{
						string[] array = item.ToString().Split('|');
						string[] values = base.HttpContext.Request.Params.GetValues(item.ToString());
						string value = clsMaHoa.Decrypt(array[1].ToString(), "tmt6364");
						Product_Detail product_Detail = JsonConvert.DeserializeObject<Product_Detail>(value);
						if (v_ct_PhieuDatHang_ChiTiet2.ID != product_Detail.ID)
						{
							v_ct_PhieuDatHang_ChiTiet2 = new v_ct_PhieuDatHang_ChiTiet();
							v_ct_PhieuDatHang_ChiTiet2 = JsonConvert.DeserializeObject<v_ct_PhieuDatHang_ChiTiet>(value);
							v_ct_PhieuDatHang_ChiTiet2.LOC_ID = ct_PhieuDatHang.LOC_ID;
							ct_PhieuDatHang.lstct_PhieuDatHang_ChiTiet.Add(v_ct_PhieuDatHang_ChiTiet2);
							list.Add(product_Detail);
						}
						Utility.EditObject(v_ct_PhieuDatHang_ChiTiet2, array[0].ToString().Substring(3, array[0].ToString().Length - 3), values[0]);
					}
				}
				if (ct_PhieuDatHang.BUTTONTYPE == "GetPromotion")
				{
					apiResponse = Utility.Create(ct_PhieuDatHang.lstct_PhieuDatHang_ChiTiet, "Deposit/" + Utility.LOC_ID);
					list = JsonConvert.DeserializeObject<List<Product_Detail>>(apiResponse.Data.ToString());
					base.Session["lstProductInput"] = list;
					apiResponse.GETPROMOTION = Utility.GetProductInputOutput(list, "Deposit_Temp");
					apiResponse.SOPHIEU = ct_PhieuDatHang.SOPHIEU;
					apiResponse.NewID = ct_PhieuDatHang.ID;
					apiResponse.MAPHIEU = ct_PhieuDatHang.MAPHIEU;
				}
				if (ct_PhieuDatHang.BUTTONTYPE == "Save")
				{
					if (base.ModelState.IsValid)
					{
						apiResponse = Utility.GetDetail<v_ct_PhieuDatHang>(Utility.LOC_ID + "/" + ct_PhieuDatHang.ID, "Deposit");
						if (!apiResponse.Success)
						{
							base.TempData["TitleError"] = apiResponse.Message;
							return RedirectToAction("Index", "Notfound");
						}
						v_ct_PhieuDatHang v_ct_PhieuDatHang2 = null;
						if (apiResponse.Data != null)
						{
							v_ct_PhieuDatHang2 = apiResponse.Data as v_ct_PhieuDatHang;
						}
						if (v_ct_PhieuDatHang2 == null || !string.IsNullOrEmpty(v_ct_PhieuDatHang2.ID_PHIEUXUAT))
						{
							base.TempData["TitleError"] = "Phiếu đã thực hiện đặt hàng! Nên không thể sửa phiếu!";
							return RedirectToAction("Index", "Notfound");
						}
						ct_PhieuDatHang.LOC_ID = Utility.LOC_ID;
						ct_PhieuDatHang.ID_NGUOISUA = base.Session["idUser"].ToString();
						ct_PhieuDatHang.THOIGIANSUA = Utility.CurrentTime;
						apiResponse = Utility.Edit(Utility.LOC_ID + "/" + ct_PhieuDatHang.ID, (v_ct_PhieuDatHang)ct_PhieuDatHang, "Deposit");
						if (apiResponse.Success)
						{
							apiResponse.ID = ct_PhieuDatHang.ID;
							if (apiResponse.Data != null)
							{
								ct_PhieuDatHang = JsonConvert.DeserializeObject<v_v_ct_PhieuDatHang>(apiResponse.Data.ToString());
							}
							list = new List<Product_Detail>();
							apiResponse.URL = base.Url.Action("Index", "Deposit", new
							{
								SearchString = "",
								Page = 1,
								ShowSearchValue = "anfACKwdLEzVMbfakvNaoA==",
								FromDate = DateTime.Now.ToString("yyyy-MM-dd"),
								ToDate = DateTime.Now.ToString("yyyy-MM-dd")
							});
						}
						else
						{
							base.ModelState.AddModelError(string.Empty, apiResponse.Message);
						}
					}
					else
					{
						apiResponse.Success = false;
						apiResponse.Data = Utility.GetModelState(base.ModelState, "Deposit");
					}
				}
				base.Session["lstProductInput"] = list;
				apiResponse.ID = ct_PhieuDatHang.ID;
				ct_PhieuDatHang.lstdm_KhachHang = new List<ComboboxFrom>();
				ct_PhieuDatHang.lstdm_KhachHang = GetDanhSachKhachHangCombobox().Data as List<ComboboxFrom>;
				ct_PhieuDatHang.lstdm_Kho = new List<v_dm_Kho>();
				ct_PhieuDatHang.lstdm_Kho = Utility.GetListData<v_dm_Kho>("Depot", "", "", Utility.LOC_ID).Data as List<v_dm_Kho>;
				ct_PhieuDatHang.lstAspNetUsers = new List<v_AspNetUsers>();
				ct_PhieuDatHang.lstAspNetUsers = Utility.GetListData<v_AspNetUsers>("User").Data as List<v_AspNetUsers>;
				List<ValueEdit> list2 = Utility.ConvertobjectToView(ct_PhieuDatHang);
				apiResponse.ProductCombo = Utility.GetProductInputOutput(list, "Deposit_Temp");
				list2.Add(new ValueEdit
				{
					Key = "tbodyTempItemInput",
					Value = apiResponse.ProductCombo
				});
				apiResponse.Detail = list2;
				apiResponse.TYPE = ct_PhieuDatHang.BUTTONTYPE;
				return View(ct_PhieuDatHang);
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
		[ValidateAntiForgeryToken]
		public ActionResult EditPopup([Bind(Include = "LOC_ID,ID,ID_KHO,MAPHIEU,SOPHIEU,NGAYLAP,ID_KHACHHANG,ID_NHANVIEN,GHICHU,TONGTHANHTIEN,CHIETKHAU,TONGTIENGIAMGIA,TONGTIENVAT,TONGTIEN,BUTTONTYPE,ADDRESS,TEL,THOIGIANTHEM,ID_NGUOITAO")] v_v_ct_PhieuDatHang ct_PhieuDatHang)
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
				if (!Utility.KiemTraQuyen("Deposit", "Create"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					apiResponse.Success = false;
					apiResponse.URL = base.Url.Action("Index", "Notfound");
					return new JsonResult
					{
						Data = apiResponse,
						JsonRequestBehavior = JsonRequestBehavior.AllowGet,
						MaxJsonLength = int.MaxValue
					};
				}
				List<Product_Detail> list = new List<Product_Detail>();
				ct_PhieuDatHang.lstct_PhieuDatHang_ChiTiet = new List<v_ct_PhieuDatHang_ChiTiet>();
				IEnumerable<string> enumerable = base.Request.Form.AllKeys.Where((string e) => e.StartsWith("txt"));
				if (enumerable == null || enumerable.Count() == 0)
				{
					base.ModelState.AddModelError("lstct_PhieuDatHang_ChiTiet", "Thêm danh sách hàng hóa.");
				}
				else
				{
					v_ct_PhieuDatHang_ChiTiet v_ct_PhieuDatHang_ChiTiet2 = new v_ct_PhieuDatHang_ChiTiet();
					foreach (string item in enumerable)
					{
						string[] array = item.ToString().Split('|');
						string[] values = base.HttpContext.Request.Params.GetValues(item.ToString());
						string value = clsMaHoa.Decrypt(array[1].ToString(), "tmt6364");
						Product_Detail product_Detail = JsonConvert.DeserializeObject<Product_Detail>(value);
						if (v_ct_PhieuDatHang_ChiTiet2.ID != product_Detail.ID)
						{
							v_ct_PhieuDatHang_ChiTiet2 = new v_ct_PhieuDatHang_ChiTiet();
							v_ct_PhieuDatHang_ChiTiet2 = JsonConvert.DeserializeObject<v_ct_PhieuDatHang_ChiTiet>(value);
							v_ct_PhieuDatHang_ChiTiet2.LOC_ID = ct_PhieuDatHang.LOC_ID;
							ct_PhieuDatHang.lstct_PhieuDatHang_ChiTiet.Add(v_ct_PhieuDatHang_ChiTiet2);
							list.Add(product_Detail);
						}
						Utility.EditObject(v_ct_PhieuDatHang_ChiTiet2, array[0].ToString().Substring(3, array[0].ToString().Length - 3), values[0]);
					}
				}
				if (ct_PhieuDatHang.BUTTONTYPE == "GetPromotion")
				{
					apiResponse = Utility.Create(ct_PhieuDatHang.lstct_PhieuDatHang_ChiTiet, "Deposit/" + Utility.LOC_ID);
					list = JsonConvert.DeserializeObject<List<Product_Detail>>(apiResponse.Data.ToString());
					apiResponse.GETPROMOTION = Utility.GetProductInputOutput(list, "Deposit_Temp");
					apiResponse.SOPHIEU = ct_PhieuDatHang.SOPHIEU;
					apiResponse.NewID = ct_PhieuDatHang.ID;
					apiResponse.MAPHIEU = ct_PhieuDatHang.MAPHIEU;
				}
				if (ct_PhieuDatHang.BUTTONTYPE == "Save")
				{
					if (base.ModelState.IsValid)
					{
						apiResponse = Utility.GetDetail<v_ct_PhieuDatHang>(Utility.LOC_ID + "/" + ct_PhieuDatHang.ID, "Deposit");
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
						v_ct_PhieuDatHang v_ct_PhieuDatHang2 = null;
						if (apiResponse.Data != null)
						{
							v_ct_PhieuDatHang2 = apiResponse.Data as v_ct_PhieuDatHang;
						}
						if (v_ct_PhieuDatHang2 == null || !string.IsNullOrEmpty(v_ct_PhieuDatHang2.ID_PHIEUXUAT))
						{
							apiResponse.Success = false;
							apiResponse.Message = "Phiếu đã thực hiện đặt hàng! Nên không thể sửa phiếu!";
							return new JsonResult
							{
								Data = apiResponse,
								JsonRequestBehavior = JsonRequestBehavior.AllowGet,
								MaxJsonLength = int.MaxValue
							};
						}
						ct_PhieuDatHang.LOC_ID = Utility.LOC_ID;
						ct_PhieuDatHang.ID_NGUOISUA = base.Session["idUser"].ToString();
						ct_PhieuDatHang.THOIGIANSUA = Utility.CurrentTime;
						apiResponse = Utility.Edit(Utility.LOC_ID + "/" + ct_PhieuDatHang.ID, (v_ct_PhieuDatHang)ct_PhieuDatHang, "Deposit");
						if (apiResponse.Success)
						{
							apiResponse.ID = ct_PhieuDatHang.ID;
							if (apiResponse.Data != null)
							{
								ct_PhieuDatHang = JsonConvert.DeserializeObject<v_v_ct_PhieuDatHang>(apiResponse.Data.ToString());
							}
							list = new List<Product_Detail>();
						}
						else
						{
							base.ModelState.AddModelError(string.Empty, apiResponse.Message);
						}
					}
					else
					{
						apiResponse.Success = false;
						apiResponse.Data = Utility.GetModelState(base.ModelState, "Deposit");
					}
				}
				base.Session["lstProductInput"] = list;
				apiResponse.ID = ct_PhieuDatHang.ID;
				ct_PhieuDatHang.lstdm_KhachHang = new List<ComboboxFrom>();
				ct_PhieuDatHang.lstdm_KhachHang = GetDanhSachKhachHangCombobox().Data as List<ComboboxFrom>;
				ct_PhieuDatHang.lstdm_Kho = new List<v_dm_Kho>();
				ct_PhieuDatHang.lstdm_Kho = Utility.GetListData<v_dm_Kho>("Depot", "", "", Utility.LOC_ID).Data as List<v_dm_Kho>;
				ct_PhieuDatHang.lstAspNetUsers = new List<v_AspNetUsers>();
				ct_PhieuDatHang.lstAspNetUsers = Utility.GetListData<v_AspNetUsers>("User").Data as List<v_AspNetUsers>;
				List<ValueEdit> list2 = Utility.ConvertobjectToView(ct_PhieuDatHang);
				apiResponse.ProductCombo = Utility.GetProductInputOutput(list, "Deposit_Temp");
				list2.Add(new ValueEdit
				{
					Key = "tbodyTempItemInput",
					Value = apiResponse.ProductCombo
				});
				apiResponse.Detail = list2;
				apiResponse.TYPE = ct_PhieuDatHang.BUTTONTYPE;
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
				return RedirectToAction("Index", "Notfound");
			}
		}

		[HttpPost]
		public ActionResult DeletePopup(string id)
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
				if (!Utility.KiemTraQuyen("Deposit", "Delete"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					apiResponse.Success = false;
					apiResponse.URL = base.Url.Action("Index", "Notfound");
					return new JsonResult
					{
						Data = apiResponse,
						JsonRequestBehavior = JsonRequestBehavior.AllowGet,
						MaxJsonLength = int.MaxValue
					};
				}
				apiResponse = Utility.GetDetail<v_ct_PhieuDatHang>(Utility.LOC_ID + "/" + id, "Deposit");
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
				v_ct_PhieuDatHang v_ct_PhieuDatHang2 = null;
				if (apiResponse.Data != null)
				{
					v_ct_PhieuDatHang2 = apiResponse.Data as v_ct_PhieuDatHang;
				}
				if (v_ct_PhieuDatHang2 == null || !string.IsNullOrEmpty(v_ct_PhieuDatHang2.ID_PHIEUXUAT))
				{
					base.TempData["TitleError"] = "Phiếu đã thực hiện đặt hàng! Nên không thể sửa phiếu!";
					apiResponse.Success = false;
					apiResponse.URL = base.Url.Action("Index", "Notfound");
					return new JsonResult
					{
						Data = apiResponse,
						JsonRequestBehavior = JsonRequestBehavior.AllowGet,
						MaxJsonLength = int.MaxValue
					};
				}
				apiResponse = Utility.Delete<v_ct_PhieuDatHang>(Utility.LOC_ID + "/" + id, "Deposit");
				apiResponse.ID = id;
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

		public ActionResult CreatePopup()
		{
			ApiResponse apiResponse = new ApiResponse();
			if (Utility.KiemTra())
			{
				return RedirectToAction("Index", "Admin");
			}
			if (!Utility.KiemTraQuyen("Deposit", "View"))
			{
				base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
				return RedirectToAction("Index", "Notfound");
			}
			v_v_ct_PhieuDatHang v_v_ct_PhieuDatHang2 = new v_v_ct_PhieuDatHang();
			v_v_ct_PhieuDatHang2.NGAYLAP = Utility.CurrentTime;
			v_v_ct_PhieuDatHang2.LOC_ID = Utility.LOC_ID;
			v_v_ct_PhieuDatHang2.SOPHIEU = Utility.GetMaxID((ct_PhieuDatHang)v_v_ct_PhieuDatHang2, Utility.LOC_ID, v_v_ct_PhieuDatHang2.NGAYLAP.ToString("yyyy-MM-dd"));
			v_v_ct_PhieuDatHang2.MAPHIEU = API.GetMaPhieu("Deposit", v_v_ct_PhieuDatHang2.NGAYLAP, v_v_ct_PhieuDatHang2.SOPHIEU);
			v_v_ct_PhieuDatHang2.lstdm_ThueSuat = new List<v_dm_ThueSuat>();
			v_v_ct_PhieuDatHang2.lstdm_ThueSuat = Utility.GetListData<v_dm_ThueSuat>("Tax", "", "", Utility.LOC_ID).Data as List<v_dm_ThueSuat>;
			v_v_ct_PhieuDatHang2.lstdm_Kho = new List<v_dm_Kho>();
			v_v_ct_PhieuDatHang2.lstdm_Kho = Utility.GetListData<v_dm_Kho>("Depot", "", "", Utility.LOC_ID).Data as List<v_dm_Kho>;
			v_v_ct_PhieuDatHang2.lstdm_KhachHang = new List<ComboboxFrom>();
			v_v_ct_PhieuDatHang2.lstdm_KhachHang = GetDanhSachKhachHangCombobox().Data as List<ComboboxFrom>;
			v_v_ct_PhieuDatHang2.lstAspNetUsers = new List<v_AspNetUsers>();
			v_v_ct_PhieuDatHang2.lstAspNetUsers = Utility.GetListData<v_AspNetUsers>("User").Data as List<v_AspNetUsers>;
			List<Product_Detail> list = new List<Product_Detail>();
			v_dm_Kho v_dm_Kho2 = v_v_ct_PhieuDatHang2.lstdm_Kho.Where((v_dm_Kho e) => e.ISDEFAULT).FirstOrDefault();
			if (v_dm_Kho2 != null)
			{
				v_v_ct_PhieuDatHang2.ID_KHO = v_dm_Kho2.ID;
			}
			base.Session["lstProductInput"] = list;
			List<ValueEdit> list2 = Utility.ConvertobjectTo(v_v_ct_PhieuDatHang2);
			apiResponse.Success = true;
			apiResponse.ProductCombo = Utility.GetProductInputOutput(list, "Deposit_Temp");
			list2.Add(new ValueEdit
			{
				Key = "tbodyTempItemInput",
				Value = apiResponse.ProductCombo
			});
			apiResponse = GetDanhSachNhomSanPham();
			string text = "";
			if (!apiResponse.Success)
			{
				base.TempData["TitleError"] = apiResponse.Message;
				return RedirectToAction("Index", "Notfound");
			}
			List<web_Sp_Get_DSNhomSanPham_Result> list3 = apiResponse.Data as List<web_Sp_Get_DSNhomSanPham_Result>;
			text = "<button class='btnGroup active' onclick='myFunctionPage(\"all\", \"\",\"collapseOneDeposit\")' id= \"all\">Show all</button>";
			foreach (web_Sp_Get_DSNhomSanPham_Result item in list3)
			{
				text = text + "<button class='btnGroup' onclick='myFunctionPage(\"" + item.ID + "\", \"\",\"collapseOneDeposit\")' id= \"" + item.ID + "\"> " + item.NAME + "</button>";
			}
			if (!string.IsNullOrEmpty(text))
			{
				text += "<button class='btnGroup' onclick='myFunctionLoadGroup(\"collapseOneDeposit\")'><span class='glyphicon glyphicon-refresh'></span></button>";
			}
			base.ViewBag.NhomHang = text;
			list2.Add(new ValueEdit
			{
				Key = "myProduct",
				Value = myProduct(text, "collapseOneDeposit")
			});
			apiResponse.Detail = list2;
			return new JsonResult
			{
				Data = apiResponse,
				JsonRequestBehavior = JsonRequestBehavior.AllowGet,
				MaxJsonLength = int.MaxValue
			};
		}

		[HttpPost]
		public ActionResult LoadProduct_Detail()
		{
			if (Utility.KiemTra())
			{
				return RedirectToAction("Index", "Admin");
			}
			if (base.ModelState.IsValid)
			{
				List<Product_Detail> lstProduct = new List<Product_Detail>();
				if (base.Session["lstProductInput"] != null)
				{
					lstProduct = (List<Product_Detail>)base.Session["lstProductInput"];
				}
				return Json(Utility.GetProductInputOutput(lstProduct, "Deposit_Temp"), JsonRequestBehavior.AllowGet);
			}
			Return obj = new Return();
			obj.DATA = "";
			return Json(obj, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		public ActionResult AddProduct_Detail(v_ct_PhieuDatHang_ChiTiet model)
		{
			if (Utility.KiemTra())
			{
				return RedirectToAction("Index", "Admin");
			}
			if (base.ModelState.IsValid)
			{
				ApiResponse apiResponse = new ApiResponse();
				List<Product_Detail> list = new List<Product_Detail>();
				if (base.Session["lstProductInput"] != null)
				{
					list = (List<Product_Detail>)base.Session["lstProductInput"];
				}
				model.TONGSOLUONG = model.SOLUONG * model.TYLE_QD;
				Product_Detail product_Detail = list.Where((Product_Detail s) => s.ID_HANGHOAKHO == model.ID_HANGHOAKHO && string.IsNullOrEmpty(s.ID_COMBO)).FirstOrDefault();
				if (product_Detail != null)
				{
				}
				Return obj = new Return();
				obj.DATA = Utility.GetProductInputOutput(list, "Deposit_Temp");
				obj.URL = apiResponse.URL;
				obj.Message = apiResponse.Message;
				return Json(obj, JsonRequestBehavior.AllowGet);
			}
			Return obj2 = new Return();
			obj2.DATA = "";
			return Json(obj2, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		public ActionResult DeleteProduct_Detail(string id)
		{
			if (Utility.KiemTra())
			{
				return RedirectToAction("Index", "Admin");
			}
			if (base.ModelState.IsValid)
			{
				List<Product_Detail> lstProduct = new List<Product_Detail>();
				if (base.Session["lstProductInput"] != null)
				{
					lstProduct = (List<Product_Detail>)base.Session["lstProductInput"];
				}
				Return obj = new Return();
				obj.DATA = Utility.GetProductInputOutput(lstProduct, "Deposit_Temp");
				return Json(obj, JsonRequestBehavior.AllowGet);
			}
			Return obj2 = new Return();
			obj2.DATA = "";
			return Json(obj2, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		public ActionResult DeleteAllProduct_Detail()
		{
			if (Utility.KiemTra())
			{
				return RedirectToAction("Index", "Admin");
			}
			if (base.ModelState.IsValid)
			{
				List<Product_Detail> list = new List<Product_Detail>();
				if (base.Session["lstProductInput"] != null)
				{
					list = (List<Product_Detail>)base.Session["lstProductInput"];
				}
				list.Clear();
				return Json(Utility.GetProductInputOutput(list, "Deposit_Temp"), JsonRequestBehavior.AllowGet);
			}
			Return obj = new Return();
			obj.DATA = "";
			return Json(obj, JsonRequestBehavior.AllowGet);
		}

		public ActionResult UpdateDeposit_TempProduct(string cartDeposit_Temp)
		{
			if (Utility.KiemTra())
			{
				return RedirectToAction("Index", "Admin");
			}
			if (base.ModelState.IsValid)
			{
				ApiResponse apiResponse = new ApiResponse();
				List<Product_Detail> list = new JavaScriptSerializer().Deserialize<List<Product_Detail>>(cartDeposit_Temp);
				List<Product_Detail> lstProduct = new List<Product_Detail>();
				if (base.Session["lstProductInput"] != null)
				{
					lstProduct = (List<Product_Detail>)base.Session["lstProductInput"];
				}
				Return obj = new Return();
				obj.DATA = Utility.GetProductInputOutput(lstProduct, "Deposit_Temp");
				obj.URL = apiResponse.URL;
				obj.Message = apiResponse.Message;
				return Json(obj, JsonRequestBehavior.AllowGet);
			}
			Return obj2 = new Return();
			obj2.DATA = "";
			return Json(obj2, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		public ActionResult CallChangeCustomer(string id)
		{
			if (Utility.KiemTra())
			{
				return RedirectToAction("Index", "Admin");
			}
			if (base.ModelState.IsValid)
			{
				Return obj = new Return();
				ApiResponse detail = Utility.GetDetail<v_dm_KhachHang>(Utility.LOC_ID + "/" + id, "Customer");
				if (!detail.Success)
				{
					base.TempData["TitleError"] = detail.Message;
					obj.URL = base.Url.Action("Index", "Notfound");
				}
				else
				{
					v_dm_KhachHang v_dm_KhachHang2 = detail.Data as v_dm_KhachHang;
					SP_Parameter sP_Parameter = new SP_Parameter();
					sP_Parameter.LOC_ID = Utility.LOC_ID;
					sP_Parameter.ID_KHACHHANG = v_dm_KhachHang2.ID;
					sP_Parameter.ID_NHOMKHACHHANG = v_dm_KhachHang2.ID_NHOMKHACHHANG;
					sP_Parameter.ID_KHUVUC = v_dm_KhachHang2.ID_KHUVUC;
					sP_Parameter.ISTHEOTHOIGIAN = false;
					sP_Parameter.ISPHATSINHCONGNO = false;
					sP_Parameter.ISPHATSINHCONGNOTRONGKY = false;
					sP_Parameter.ISCONCONGNO = false;
					detail = Utility.Get_ThongKeCongNoKhachHang<v_ThongKeCongNoKhachHang>(sP_Parameter);
					if (!detail.Success)
					{
						base.TempData["TitleError"] = detail.Message;
						return RedirectToAction("Index", "Notfound");
					}
					if (detail.Data != null)
					{
						v_ThongKeCongNoKhachHang v_ThongKeCongNoKhachHang2 = (detail.Data as List<v_ThongKeCongNoKhachHang>).FirstOrDefault();
						if (v_ThongKeCongNoKhachHang2 != null)
						{
							v_dm_KhachHang2.CONGNOTHONGBAO = ((!(v_dm_KhachHang2.MAX_CONGNO > 0.0)) ? ((v_ThongKeCongNoKhachHang2.TONGTIENCONGNOCUOIKY > 0.0) ? ("Công nợ: " + v_ThongKeCongNoKhachHang2.TONGTIENCONGNOCUOIKY.ToString("N0")) : "") : ((v_ThongKeCongNoKhachHang2.TONGTIENCONGNOCUOIKY >= v_dm_KhachHang2.MAX_CONGNO) ? ("Công nợ: " + v_ThongKeCongNoKhachHang2.TONGTIENCONGNOCUOIKY.ToString("N0") + " > " + v_dm_KhachHang2.MAX_CONGNO.ToString("N0")) : ("Công nợ: " + v_ThongKeCongNoKhachHang2.TONGTIENCONGNOCUOIKY.ToString("N0") + "(" + v_dm_KhachHang2.MAX_CONGNO.ToString("N0") + ")")));
							v_dm_KhachHang2.KHONGDUOCPHEPTAO = v_dm_KhachHang2.MAX_CONGNO > 0.0 && v_ThongKeCongNoKhachHang2.TONGTIENCONGNOCUOIKY >= v_dm_KhachHang2.MAX_CONGNO;
							if (v_dm_KhachHang2.LATITUDE.HasValue && v_dm_KhachHang2.LONGITUDE.HasValue)
							{
								v_dm_KhachHang2.CONTENT_MAP = "Vĩ độ: " + v_dm_KhachHang2.LATITUDE + "<br>Kinh độ: " + v_dm_KhachHang2.LONGITUDE;
							}
						}
					}
					else
					{
						v_dm_KhachHang2.CONTENT_MAP = "";
						v_dm_KhachHang2.CONGNOTHONGBAO = "";
					}
					obj.DataObject = v_dm_KhachHang2;
				}
				return Json(obj, JsonRequestBehavior.AllowGet);
			}
			Return obj2 = new Return();
			obj2.DATA = "";
			return Json(obj2, JsonRequestBehavior.AllowGet);
		}

		private string CheckCongNoKhachHang(string id, double TienHoaDon)
		{
			ApiResponse detail = Utility.GetDetail<v_dm_KhachHang>(Utility.LOC_ID + "/" + id, "Customer");
			if (!detail.Success)
			{
				return "-1";
			}
			v_dm_KhachHang v_dm_KhachHang2 = detail.Data as v_dm_KhachHang;
			SP_Parameter sP_Parameter = new SP_Parameter();
			sP_Parameter.LOC_ID = Utility.LOC_ID;
			sP_Parameter.ID_KHACHHANG = v_dm_KhachHang2.ID;
			sP_Parameter.ID_NHOMKHACHHANG = v_dm_KhachHang2.ID_NHOMKHACHHANG;
			sP_Parameter.ID_KHUVUC = v_dm_KhachHang2.ID_KHUVUC;
			sP_Parameter.ISTHEOTHOIGIAN = false;
			sP_Parameter.ISPHATSINHCONGNO = false;
			sP_Parameter.ISPHATSINHCONGNOTRONGKY = false;
			sP_Parameter.ISCONCONGNO = false;
			detail = Utility.Get_ThongKeCongNoKhachHang<v_ThongKeCongNoKhachHang>(sP_Parameter);
			if (!detail.Success)
			{
				return "-1";
			}
			if (detail.Data != null)
			{
				v_ThongKeCongNoKhachHang v_ThongKeCongNoKhachHang2 = (detail.Data as List<v_ThongKeCongNoKhachHang>).FirstOrDefault();
				if (v_ThongKeCongNoKhachHang2 != null)
				{
					if (v_dm_KhachHang2.MAX_CONGNO > 0.0 && v_dm_KhachHang2.MAX_CONGNO < v_ThongKeCongNoKhachHang2.TONGTIENCONGNOCUOIKY + TienHoaDon)
					{
						return (v_ThongKeCongNoKhachHang2.TONGTIENCONGNOCUOIKY + TienHoaDon).ToString("N0") + " > " + v_dm_KhachHang2.MAX_CONGNO.ToString("N0").Replace(".", ",");
					}
					return "";
				}
				return "-1";
			}
			return "-1";
		}

		[HttpPost]
		public ActionResult SaveMapCustomer(string ID, string LATITUDE, string LONGITUDE)
		{
			if (Utility.KiemTra())
			{
				return RedirectToAction("Index", "Admin");
			}
			if (base.ModelState.IsValid)
			{
				Return obj = new Return();
				v_dm_KhachHang v_dm_KhachHang2 = new v_dm_KhachHang();
				v_dm_KhachHang2.ID = ID;
				v_dm_KhachHang2.LATITUDE = Convert.ToDouble(LATITUDE.Replace(".", ","));
				v_dm_KhachHang2.LONGITUDE = Convert.ToDouble(LONGITUDE.Replace(".", ","));
				ApiResponse apiResponse = Utility.Save_Map(v_dm_KhachHang2, "Insert_Customer_Map");
				if (!apiResponse.Success)
				{
					base.TempData["TitleError"] = apiResponse.Message;
					return RedirectToAction("Index", "Notfound");
				}
				obj.DataObject = v_dm_KhachHang2;
				return Json("Lưu thành công!", JsonRequestBehavior.AllowGet);
			}
			Return obj2 = new Return();
			obj2.DATA = "";
			return Json(obj2, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		public ActionResult LoadDanhSachSanPham(TimKiem model)
		{
			if (Utility.KiemTra())
			{
				return RedirectToAction("Index", "Admin");
			}
			if (base.ModelState.IsValid)
			{
				Return obj = new Return();
				obj.DATA = "";
				if (model.GroupID == "all")
				{
					model.GroupID = "-1";
				}
				if (string.IsNullOrEmpty(model.keySearch))
				{
					model.keySearch = "";
				}
				model.BOLTONKHO = !model.BOLTONKHO;
				ApiResponse danhSachSanPham = GetDanhSachSanPham(model);
				if (!danhSachSanPham.Success)
				{
					base.TempData["TitleError"] = danhSachSanPham.Message;
					obj.URL = base.Url.Action("Index", "Notfound");
				}
				else
				{
					List<web_Sp_Get_DSSanPham_Result> list = danhSachSanPham.Data as List<web_Sp_Get_DSSanPham_Result>;
					foreach (web_Sp_Get_DSSanPham_Result item in list)
					{
						int num = 0;
						string text = "";
						if (item.TYLE_QD > 1.0)
						{
							num = Convert.ToInt32(item.QTY) / Convert.ToInt32(item.TYLE_QD);
							text = ((num > 0) ? (num.ToString("N0") + " " + item.NAME_DVT) : "") + ((item.QTY - (double)num * item.TYLE_QD > 0.0) ? (((num > 0) ? "/" : "") + (item.QTY - (double)num * item.TYLE_QD).ToString("N0") + " " + item.NAME_DVT_QD) : "");
						}
						else
						{
							text = item.QTY.ToString("N0") + " " + item.NAME_DVT;
						}
						obj.DATA += "<div class=\"productDeposit\">";
						obj.DATA = obj.DATA + "<button class=\"productDeposit-button\" onclick='myFunOpenProduct(this,\"" + item.ID_HANGHOAKHO + "\")'>";
						Return obj2 = obj;
						obj2.DATA = obj2.DATA + "<img src=\"/Images_Upload/Product/" + (string.IsNullOrEmpty(item.PICTURE) ? "NoImage.png" : item.PICTURE) + "\" " + ((!string.IsNullOrEmpty(item.PICTURE)) ? ("onclick=\"showPopupDeposit('" + item.PICTURE + "')") : "") + "\">";
						obj.DATA = obj.DATA + "<div class=\"productDeposit-details\">" + item.NAME;
						obj.DATA += "<div class=\"productDeposit-info\">";
						if (!item.ISKHONGHIENTHITONKHO)
						{
							obj.DATA = obj.DATA + "<code>SL: " + text + "</code>-";
						}
						obj.DATA = obj.DATA + "<code>" + $"{item.GIA01:N0}" + " đ</code>";
						obj.DATA += "</div></div></button></div>";
					}
				}
				return Json(obj, JsonRequestBehavior.AllowGet);
			}
			Return obj3 = new Return();
			obj3.DATA = "";
			obj3.CHUOIPHANTRANG = "";
			return Json(obj3, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		public ActionResult LoadGroup(string Class)
		{
			if (Utility.KiemTra())
			{
				return RedirectToAction("Index", "Admin");
			}
			ApiResponse danhSachNhomSanPham = GetDanhSachNhomSanPham();
			string text = "";
			if (danhSachNhomSanPham.Success && danhSachNhomSanPham.Data != null)
			{
				List<web_Sp_Get_DSNhomSanPham_Result> list = danhSachNhomSanPham.Data as List<web_Sp_Get_DSNhomSanPham_Result>;
				text = "<button class='btnGroup active' onclick='myFunctionPage(\"all\", \"\", \"" + Class + "\")' id= \"all\">Show all</button>";
				foreach (web_Sp_Get_DSNhomSanPham_Result item in list)
				{
					text = text + "<button class='btnGroup' onclick='myFunctionPage(\"" + item.ID + "\", \"\", \"" + Class + "\")' id= \"" + item.ID + "\"> " + item.NAME + "</button>";
				}
				if (!string.IsNullOrEmpty(text))
				{
					text = text + "<button class='btnGroup' onclick='myFunctionLoadGroup(\"" + Class + "\")'><span class='glyphicon glyphicon-refresh'></span></button>";
				}
			}
			Return obj = new Return();
			obj.DATA = text;
			obj.CHUOIPHANTRANG = Class;
			obj.URL = danhSachNhomSanPham.URL;
			return Json(obj, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		public ActionResult LoadProduct(string id)
		{
			if (Utility.KiemTra())
			{
				return RedirectToAction("Index", "Admin");
			}
			ApiResponse apiResponse = new ApiResponse();
			v_dm_HangHoa v_dm_HangHoa2 = new v_dm_HangHoa();
			if (base.ModelState.IsValid)
			{
				apiResponse = GetSanPham(id);
				if (apiResponse.Success)
				{
					v_dm_HangHoa2 = apiResponse.Data as v_dm_HangHoa;
					if (v_dm_HangHoa2 != null)
					{
						v_dm_HangHoa2.GIA = v_dm_HangHoa2.GIA01;
						v_dm_HangHoa2.GIA_QD = v_dm_HangHoa2.GIA01_QD;
						v_dm_HangHoa2.NAME_DVT = v_dm_HangHoa2.NAME_DVT + " (" + v_dm_HangHoa2.GIA01.ToString("N0") + ")";
						v_dm_HangHoa2.NAME_DVT_QD = v_dm_HangHoa2.NAME_DVT_QD + " (" + v_dm_HangHoa2.GIA01_QD.ToString("N0") + ")";
					}
					if (!string.IsNullOrEmpty(v_dm_HangHoa2.ID_THUESUAT))
					{
						ApiResponse detail = Utility.GetDetail<v_v_dm_ThueSuat>(Utility.LOC_ID + "/" + v_dm_HangHoa2.ID_THUESUAT, "Tax");
						if (detail.Data != null && detail.Data is v_v_dm_ThueSuat v_v_dm_ThueSuat2)
						{
							v_dm_HangHoa2.THANHTIEN = v_dm_HangHoa2.GIA * 1.0;
							v_dm_HangHoa2.THUESUAT = v_v_dm_ThueSuat2.THUESUAT;
							v_dm_HangHoa2.TONGTIENVAT = v_dm_HangHoa2.THANHTIEN * v_dm_HangHoa2.THUESUAT / 100.0;
							v_dm_HangHoa2.TONGCONG = v_dm_HangHoa2.THANHTIEN + v_dm_HangHoa2.TONGTIENVAT;
						}
					}
					else
					{
						v_dm_HangHoa2.THANHTIEN = v_dm_HangHoa2.GIA * 1.0;
						v_dm_HangHoa2.THUESUAT = 0.0;
						v_dm_HangHoa2.TONGTIENVAT = v_dm_HangHoa2.THANHTIEN * v_dm_HangHoa2.THUESUAT / 100.0;
						v_dm_HangHoa2.TONGCONG = v_dm_HangHoa2.THANHTIEN + v_dm_HangHoa2.TONGTIENVAT;
					}
				}
				List<ValueEdit> list = Utility.ConvertobjectTo(v_dm_HangHoa2);
				list.Add(new ValueEdit
				{
					Key = "PriceProductDeposit_Temp",
					Value = (v_dm_HangHoa2.MA == API.TINHTHUE_KM)
				});
				apiResponse.Detail = list;
			}
			return new JsonResult
			{
				Data = apiResponse,
				JsonRequestBehavior = JsonRequestBehavior.AllowGet,
				MaxJsonLength = int.MaxValue
			};
		}

		private ApiResponse GetDanhSachKhuVuc()
		{
			SP_Parameter sP_Parameter = new SP_Parameter();
			sP_Parameter.LOC_ID = Utility.LOC_ID;
			sP_Parameter.ID_NHOMQUYEN = base.Session["idNhomQuyen"].ToString();
			ApiResponse apiResponse = Utility.ExecuteStoredProc<web_Sp_Get_DSKhuVuc_Result>(sP_Parameter, "web_Sp_Get_DSKhuVuc");
			if (!apiResponse.Success)
			{
				apiResponse.Data = new List<web_Sp_Get_DSKhuVuc_Result>();
				base.TempData["TitleError"] = apiResponse.Message;
				apiResponse.Success = false;
				apiResponse.URL = base.Url.Action("Index", "Notfound");
			}
			return apiResponse;
		}

		private ApiResponse GetDanhSachSanPham(TimKiem model)
		{
			SP_Parameter sP_Parameter = new SP_Parameter();
			ApiResponse apiResponse = new ApiResponse();
			sP_Parameter = new SP_Parameter();
			sP_Parameter.LOC_ID = Utility.LOC_ID;
			sP_Parameter.ID_NHOMQUYEN = ((!string.IsNullOrEmpty(model.idNhomQuyen)) ? model.idNhomQuyen : base.Session["idNhomQuyen"].ToString());
			sP_Parameter.ID_NHOMHANGHOA = model.GroupID;
			sP_Parameter.KEY = model.keySearch;
			sP_Parameter.ID_KHO = model.ID_KHO;
			sP_Parameter.BOLTONKHO = model.BOLTONKHO;
			sP_Parameter.ID_HANGHOAKHO = "";
			apiResponse = Utility.ExecuteStoredProc<web_Sp_Get_DSSanPham_Result>(sP_Parameter, "web_Sp_Get_DSSanPham");
			if (!apiResponse.Success)
			{
				apiResponse.Data = new List<web_Sp_Get_DSSanPham_Result>();
				base.TempData["TitleError"] = apiResponse.Message;
				apiResponse.Success = false;
				apiResponse.URL = base.Url.Action("Index", "Notfound");
			}
			return apiResponse;
		}

		private ApiResponse GetDanhSachNhomSanPham()
		{
			SP_Parameter sP_Parameter = new SP_Parameter();
			sP_Parameter.LOC_ID = Utility.LOC_ID;
			sP_Parameter.ID_NHOMQUYEN = base.Session["idNhomQuyen"].ToString();
			ApiResponse apiResponse = Utility.ExecuteStoredProc<web_Sp_Get_DSNhomSanPham_Result>(sP_Parameter, "web_Sp_Get_DSNhomSanPham");
			if (!apiResponse.Success)
			{
				apiResponse.Data = new List<web_Sp_Get_DSNhomSanPham_Result>();
				base.TempData["TitleError"] = apiResponse.Message;
				apiResponse.Success = false;
				apiResponse.URL = base.Url.Action("Index", "Notfound");
			}
			return apiResponse;
		}

		private ApiResponse GetSanPham(string idSanPham)
		{
			SP_Parameter sP_Parameter = new SP_Parameter();
			sP_Parameter.LOC_ID = Utility.LOC_ID;
			sP_Parameter.ID_KHO = "";
			sP_Parameter.BOLTONKHO = false;
			sP_Parameter.ID_HANGHOAKHO = idSanPham;
			ApiResponse apiResponse = Utility.ExecuteStoredProc<v_dm_HangHoa>(sP_Parameter, "Sp_Get_DanhSachSanPhamKho");
			if (!apiResponse.Success)
			{
				apiResponse.Data = new v_dm_HangHoa();
				base.TempData["TitleError"] = apiResponse.Message;
				apiResponse.Success = false;
				apiResponse.URL = base.Url.Action("Index", "Notfound");
			}
			else
			{
				apiResponse.Data = (apiResponse.Data as List<v_dm_HangHoa>).FirstOrDefault();
			}
			return apiResponse;
		}

		public ApiResponse GetDanhSachKhachHang<T>(string idNhomQuyen = "", string KEY = "", string LOAITIMKIEM = "")
		{
			SP_Parameter sP_Parameter = new SP_Parameter();
			sP_Parameter.LOC_ID = Utility.LOC_ID;
			sP_Parameter.ID_NHOMQUYEN = ((!string.IsNullOrEmpty(idNhomQuyen)) ? idNhomQuyen : base.Session["idNhomQuyen"].ToString());
			sP_Parameter.ID_KHUVUC = "-1";
			sP_Parameter.KEY = KEY;
			sP_Parameter.LOAITIMKIEM = LOAITIMKIEM;
			sP_Parameter.THU = ((!string.IsNullOrEmpty(idNhomQuyen)) ? idNhomQuyen : base.Session["idNhomQuyen"].ToString());
			ApiResponse apiResponse = Utility.ExecuteStoredProc<T>(sP_Parameter, "web_Sp_Get_DSKhachHang");
			if (!apiResponse.Success)
			{
				apiResponse.Data = new List<v_dm_KhachHang>();
				apiResponse.URL = "";
				base.TempData["TitleError"] = apiResponse.Message;
				apiResponse.Success = false;
			}
			return apiResponse;
		}

		public ApiResponse GetDanhSachKhachHangCombobox(string idNhomQuyen = "", string KEY = "", string LOAITIMKIEM = "")
		{
			SP_Parameter sP_Parameter = new SP_Parameter();
			sP_Parameter.LOC_ID = Utility.LOC_ID;
			sP_Parameter.ID_NHOMQUYEN = ((!string.IsNullOrEmpty(idNhomQuyen)) ? idNhomQuyen : base.Session["idNhomQuyen"].ToString());
			sP_Parameter.ID_KHUVUC = "-1";
			sP_Parameter.KEY = KEY;
			sP_Parameter.LOAITIMKIEM = LOAITIMKIEM;
			sP_Parameter.THU = ((!string.IsNullOrEmpty(idNhomQuyen)) ? idNhomQuyen : base.Session["idNhomQuyen"].ToString());
			ApiResponse apiResponse = new ApiResponse();
			apiResponse = ((!(sP_Parameter.ID_NHOMQUYEN != "-1")) ? Utility.GetListData<ComboboxFrom>("Customer", "", "", Utility.LOC_ID) : Utility.ExecuteStoredProc<ComboboxFrom>(sP_Parameter, "web_Sp_Get_DSKhachHang"));
			if (!apiResponse.Success)
			{
				apiResponse.Data = new List<ComboboxFrom>();
				base.TempData["TitleError"] = apiResponse.Message;
				apiResponse.Success = false;
				apiResponse.URL = base.Url.Action("Index", "Notfound");
			}
			return apiResponse;
		}

		[HttpPost]
		public ActionResult UpdateAddProduct(Product_Detail Product_Detail)
		{
			ApiResponse apiResponse = new ApiResponse();
			try
			{
				List<Product_Detail> lstProduct = new List<Product_Detail>();
				if (base.Session["lstProductInput"] != null)
				{
					lstProduct = (List<Product_Detail>)base.Session["lstProductInput"];
				}
				double dONGIA = Product_Detail.DONGIA;
				Utility.TinhTong(Product_Detail, null, lstProduct);
				apiResponse.Success = true;
				apiResponse.Detail = Product_Detail;
				apiResponse.Message = ((dONGIA != Product_Detail.DONGIA) ? (Product_Detail.NAME + " Cập nhật đơn giá từ " + dONGIA.ToString("N0") + " thành " + Product_Detail.DONGIA.ToString("N0")) : "");
				return new JsonResult
				{
					Data = apiResponse,
					JsonRequestBehavior = JsonRequestBehavior.AllowGet,
					MaxJsonLength = int.MaxValue
				};
			}
			catch (Exception ex)
			{
				Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex, JsonConvert.SerializeObject(Product_Detail));
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

		[HttpPost]
		[ValidateInput(false)]
		public ActionResult AddProductDeposit([Bind(Include = "ID_HANGHOA,ID_HANGHOAKHO,DONGIA,ID_DVT,SOLUONG,CHIETKHAU,TONGTIENGIAMGIA,THANHTIEN,THUESUAT,ID_THUESUAT,TONGTIENVAT,TONGCONG,ID_KHO")] Product_Detail Product_Detail)
		{
			List<Product_Detail> list = new List<Product_Detail>();
			if (base.Session["lstProductInput"] != null)
			{
				list = (List<Product_Detail>)base.Session["lstProductInput"];
			}
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
				if (base.ModelState.IsValid)
				{
					v_dm_HangHoa v_dm_HangHoa2 = new v_dm_HangHoa();
					apiResponse = Utility.Get_DanhSachSanPhamKho<v_dm_HangHoa>(Product_Detail.ID_KHO, bolTonKho: false, Product_Detail.ID_HANGHOAKHO);
					if (!apiResponse.Success)
					{
						apiResponse.Data = new List<v_dm_HangHoa>();
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
						v_dm_HangHoa2 = (apiResponse.Data as List<v_dm_HangHoa>).FirstOrDefault();
					}
					if (v_dm_HangHoa2 != null)
					{
						Product_Detail.STT = ((list.Count() <= 0) ? 1 : (list.Max((Product_Detail e) => e.STT) + 1));
						Product_Detail.ID = Guid.NewGuid().ToString();
						Product_Detail.NAME = v_dm_HangHoa2.NAME;
						Product_Detail.MA = v_dm_HangHoa2.MA;
						Product_Detail.ID_NHOMHANGHOA = v_dm_HangHoa2.ID_NHOMHANGHOA;
						if (v_dm_HangHoa2.ID_DVT == Product_Detail.ID_DVT)
						{
							Product_Detail.NAME_DVT = v_dm_HangHoa2.NAME_DVT;
							if (!string.IsNullOrEmpty(v_dm_HangHoa2.ID_DVT_QD))
							{
								Product_Detail.TYLE_QD = v_dm_HangHoa2.TYLE_QD;
							}
							else if (v_dm_HangHoa2.LOAIHANGHOA == 2.ToString())
							{
								Product_Detail.TYLE_QD = 0.0;
							}
							else
							{
								Product_Detail.TYLE_QD = 1.0;
							}
						}
						else if (v_dm_HangHoa2.ID_DVT_QD == Product_Detail.ID_DVT && !string.IsNullOrEmpty(v_dm_HangHoa2.ID_DVT_QD))
						{
							Product_Detail.NAME_DVT = v_dm_HangHoa2.NAME_DVT_QD;
							Product_Detail.TYLE_QD = 1.0;
						}
						Product_Detail.TONGSOLUONG = Product_Detail.TYLE_QD * Product_Detail.SOLUONG;
						list.Add(Product_Detail);
						double dONGIA = Product_Detail.DONGIA;
						Product_Detail.TYPE = "SOLUONG";
						Utility.TinhTong(Product_Detail, null, list);
						apiResponse.Message = ((dONGIA != Product_Detail.DONGIA) ? (Product_Detail.NAME + " Cập nhật đơn giá từ " + dONGIA.ToString("N0") + " thành " + Product_Detail.DONGIA.ToString("N0")) : "");
						if (v_dm_HangHoa2.LOAIHANGHOA == 1.ToString())
						{
							SP_Parameter sP_Parameter = new SP_Parameter();
							sP_Parameter.LOC_ID = Utility.LOC_ID;
							sP_Parameter.ID_KHO = Product_Detail.ID_KHO;
							sP_Parameter.ID_COMBO = Product_Detail.ID_HANGHOA;
							ApiResponse apiResponse2 = Utility.ExecuteStoredProc<Product_Detail>(sP_Parameter, "Sp_Get_DanhSachSanPhamKho_Combo");
							if (!apiResponse2.Success)
							{
								apiResponse.Data = new List<Product_Detail>();
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
							if (apiResponse2.Data != null)
							{
								List<Product_Detail> list2 = apiResponse2.Data as List<Product_Detail>;
								foreach (Product_Detail item in list2)
								{
									item.ID = Guid.NewGuid().ToString();
									item.STT = Product_Detail.STT;
									item.ID_DVT = item.ID_DVT_COMBO;
									item.SOLUONG = Product_Detail.SOLUONG * item.QTY_COMBO;
									item.TYLE_QD = item.TYLE_QD_COMBO;
									item.TONGSOLUONG = Product_Detail.SOLUONG * item.QTY_TOTAL_COMBO;
									item.DONGIA = 0.0;
									item.ISCOMBO = true;
									item.ID_COMBO = Product_Detail.ID_HANGHOA;
									Product_Detail.ID_COMBO = Product_Detail.ID_HANGHOA;
									list.Add(item);
								}
							}
						}
						base.Session["lstProductInput"] = list;
					}
					apiResponse.ProductCombo = Utility.GetProductInputOutput(list, "Deposit_Temp");
				}
				return new JsonResult
				{
					Data = apiResponse,
					JsonRequestBehavior = JsonRequestBehavior.AllowGet,
					MaxJsonLength = int.MaxValue
				};
			}
			catch (Exception ex)
			{
				Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex, JsonConvert.SerializeObject(Product_Detail));
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

		[HttpPost]
		public ActionResult UpdateProductDeposit_Temp(string ID, string TYPE, string VALUE)
		{
			List<Product_Detail> list = new List<Product_Detail>();
			if (base.Session["lstProductInput"] != null)
			{
				list = (List<Product_Detail>)base.Session["lstProductInput"];
			}
			ApiResponse apiResponse = new ApiResponse();
			try
			{
				v_v_dm_HangHoa v_v_dm_HangHoa2 = new v_v_dm_HangHoa();
				Product_Detail product_Detail = list.Where((Product_Detail e) => e.ID == ID).FirstOrDefault();
				if (product_Detail != null)
				{
					double dONGIA = product_Detail.DONGIA;
					product_Detail.TYPE = TYPE;
					Utility.TinhTong(product_Detail, VALUE, list);
					apiResponse.Message = ((dONGIA != product_Detail.DONGIA) ? (product_Detail.NAME + " Cập nhật đơn giá từ " + dONGIA.ToString("N0") + " thành " + product_Detail.DONGIA.ToString("N0")) : "");
					if (product_Detail.TYPE != "CHIETKHAU" && product_Detail.TYPE != "TONGTIENGIAMGIA")
					{
						list = XoaKhuyenMai(list);
					}
				}
				base.Session["lstProductInput"] = list;
				apiResponse.ProductCombo = Utility.GetProductInputOutput(list, "Deposit_Temp");
				apiResponse.Success = true;
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

		[HttpPost]
		public ActionResult DeleteProductDeposit_Temp(string ID)
		{
			List<Product_Detail> list = new List<Product_Detail>();
			if (base.Session["lstProductInput"] != null)
			{
				list = (List<Product_Detail>)base.Session["lstProductInput"];
			}
			ApiResponse apiResponse = new ApiResponse();
			try
			{
				v_v_dm_HangHoa v_v_dm_HangHoa2 = new v_v_dm_HangHoa();
				Product_Detail check = list.Where((Product_Detail e) => e.ID == ID).FirstOrDefault();
				if (check != null && list != null)
				{
					if (!string.IsNullOrEmpty(check.ID_COMBO))
					{
						foreach (Product_Detail item in list.Where((Product_Detail e) => e.ID_COMBO == check.ID_COMBO).ToList())
						{
							list.Remove(item);
						}
					}
					else
					{
						list.Remove(check);
					}
				}
				if (!check.ISKHUYENMAI)
				{
					list = XoaKhuyenMai(list);
				}
				base.Session["lstProductInput"] = list;
				apiResponse.ProductCombo = Utility.GetProductInputOutput(list, "Deposit_Temp");
				apiResponse.Success = true;
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

		[HttpPost]
		public ActionResult DeleteAllProductDeposit_Temp()
		{
			ApiResponse apiResponse = new ApiResponse();
			try
			{
				List<Product_Detail> list = new List<Product_Detail>();
				base.Session["lstProductInput"] = list;
				apiResponse.ProductCombo = Utility.GetProductInputOutput(list, "Deposit_Temp");
				apiResponse.Success = true;
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

		private string myProduct(string sbtnGroup, string Class)
		{
			return " <div class=\"panel-group\" id=\"accordion1\"><div class=\"panel panel-default\"><div class=\"panel-heading\">    <h1 class=\"panel-title\">        <a data-toggle=\"collapse\" data-parent=\"#accordion1\" href=\"#collapseOne1\">            NHÓM HÀNG HÓA        </a>    </h1> <div class='ckbox ckbox-default'><input type='checkbox' id='selectall' value='0'><label for='selectall'>Tất cả(Bao gồm hết tồn kho)</label></div></div><div id=\"collapseOne1\" class=\"panel-collapse collapse in\">    <div id=\"myBtnContainer\">" + sbtnGroup + "</div></div></div></div>\r\n<div class=\"panel-group\" id=\"accordion2\"><div class=\"panel panel-default\"><div class=\"panel-heading\">    <h1 class=\"panel-title\">        <a data-toggle=\"collapse\" data-parent=\"#accordion2\" href=\"#collapseOne2\">            DANH SÁCH HÀNG HÓA        </a>    </h1></div><div id=\"collapseOne2\" class=\"panel-collapse collapse in\">    <div>        <input id=\"myInput\" type=\"text\" placeholder=\"" + Utility.TimKiem + "\" class=\"form-control\" onkeyup=\"myInputOnkeyup('" + Class + "', event)\" style=\"width:300px;display:inline-block\">        <button class='btn btn-default' onclick='funSearchItemProduct(\"" + Class + "\")'><span class='glyphicon glyphicon-search'></span></button>    </div><div id=\"myTest\">    </div>    <div id=\"mycontainer\" class=\"productDeposit-list\">    </div></div></div></div>";
		}

		private List<Product_Detail> XoaKhuyenMai(List<Product_Detail> lstOrderProduct)
		{
			List<Product_Detail> list = new List<Product_Detail>();
			foreach (Product_Detail item in lstOrderProduct)
			{
				if (item.ISKHUYENMAI)
				{
					list.Add(item);
					continue;
				}
				item.CHIETKHAU = 0.0;
				item.TYPE = "CHIETKHAU";
				Utility.TinhTong(item, "0", lstOrderProduct);
				item.TONGTIENGIAMGIA = 0.0;
				item.ISDALAYKHUYENMAI = false;
				item.ID_KHUYENMAI = "";
			}
			foreach (Product_Detail item2 in list)
			{
				lstOrderProduct.Remove(item2);
			}
			return lstOrderProduct;
		}

		[HttpPost]
		public ActionResult OnSubmitDeposit(string cartOrder, int HINHTHUC = 0)
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
			if (base.ModelState.IsValid)
			{
				Return obj = new Return();
				List<Deposit> list = new JavaScriptSerializer().Deserialize<List<Deposit>>(cartOrder);
				foreach (Deposit item in list)
				{
					item.ID_NGUOITAO = base.Session["idUser"].ToString();
					item.LOC_ID = Utility.LOC_ID;
					item.NGAYLAP = Utility.CurrentTime.AddDays(HINHTHUC);
				}
				apiResponse = Utility.Create(list, "Deposit/PostCreateOutput");
				if (apiResponse.Success)
				{
					obj.Message = "Tạo phiếu xuất thành công!";
				}
				else
				{
					base.ModelState.AddModelError(string.Empty, apiResponse.Message);
					obj.Message = apiResponse.Message;
				}
				return Json(obj, JsonRequestBehavior.AllowGet);
			}
			Return obj2 = new Return();
			obj2.DATA = "";
			return Json(obj2, JsonRequestBehavior.AllowGet);
		}

		public ActionResult ViewReport(string ID)
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
				v_ct_PhieuDatHang v_ct_PhieuDatHang2 = new v_ct_PhieuDatHang();
				SP_Parameter sP_Parameter = new SP_Parameter();
				sP_Parameter.ID_PHIEUDATHANG = ID;
				apiResponse = Utility.ExecuteStoredProc<v_ct_PhieuDatHang>(sP_Parameter, "Sp_Get_DanhSachPhieuDatHang");
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
					v_ct_PhieuDatHang2 = (apiResponse.Data as List<v_ct_PhieuDatHang>).FirstOrDefault();
				}
				SP_Parameter_Report sP_Parameter_Report = new SP_Parameter_Report();
				sP_Parameter_Report.LOC_ID = Utility.LOC_ID;
				sP_Parameter_Report.ID_PHIEUDATHANG = ID;
				ReportClass report = new ReportClass();
				apiResponse = Utility.ExecuteStoredProc<DataTable>(sP_Parameter_Report, "Sp_Get_DanhSachPhieuDatHang_ChiTiet");
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
				DataTable dataTable = apiResponse.Data as DataTable;
				foreach (DataRow row in dataTable.Rows)
				{
					if (dataTable.Columns.Contains("ISKHUYENMAI") && Convert.ToBoolean(row["ISKHUYENMAI"]))
					{
						row["NAME"] = "(KM)" + row["NAME"];
					}
				}
				if (apiResponse.CheckValue)
				{
					dataTable.Rows.Clear();
				}
				report = Utility.GetFormulaFields(report, v_ct_PhieuDatHang2);
				report.SetDataSource(dataTable);
				base.Response.Buffer = false;
				base.Response.ClearContent();
				base.Response.ClearHeaders();
				Stream stream = report.ExportToStream(ExportFormatType.PortableDocFormat);
				Utility.Report = report;
				apiResponse = new ApiResponse();
				apiResponse.Success = true;
				apiResponse.NAME = Utility.GetTitleFrom("Deposit") + " - " + v_ct_PhieuDatHang2.MAPHIEU;
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
