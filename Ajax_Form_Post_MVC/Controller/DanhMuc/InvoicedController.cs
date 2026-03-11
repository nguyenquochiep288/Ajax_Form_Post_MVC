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
using DatabaseTHP.StoredProcedure.Parameter;
using MVC_QuanLyTHP.Class;
using MVC_QuanLyTHP.Models;
using Newtonsoft.Json;
using PagedList;

namespace MVC_QuanLyTHP.Controllers
{

	public class InvoicedController : Controller
	{
		private string myModalEdit = "myModalEdit";

		private string myModalAdd = "myModalAdd";

		public ActionResult Index(int Page = 1, DateTime? FromDate = null, DateTime? ToDate = null, string SearchString = "", string ShowSearchValue = "", string MAPHIEU = "", string IDCODE = "")
		{
			try
			{
				if (Utility.KiemTra())
				{
					return RedirectToAction("Index", "Admin");
				}
				if (!Utility.KiemTraQuyen("Invoiced", "View"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				string text = "";
				int num = 0;
				int num2 = 0;
				int num3 = 0;
				ShowSearchValue = Utility.GetShowSearchValue<ct_HoaDon>(ShowSearchValue);
				ApiResponse apiResponse = new ApiResponse();
				IPagedList<v_ct_HoaDon> iPagedList = new List<v_ct_HoaDon>().OrderByDescending((v_ct_HoaDon s) => s.NGAYLAP).ToList().ToPagedList(Page, Utility.GetPageSize());
				if (FromDate.HasValue || !string.IsNullOrEmpty(IDCODE) || !string.IsNullOrEmpty(MAPHIEU))
				{
					if (!string.IsNullOrEmpty(IDCODE) || !string.IsNullOrEmpty(MAPHIEU))
					{
						apiResponse = Utility.Get_DanhSachHoaDon<v_ct_HoaDon>(null, null, MAPHIEU, IDCODE);
					}
					if (FromDate.HasValue)
					{
						apiResponse = Utility.Get_DanhSachHoaDon<v_ct_HoaDon>(FromDate, ToDate, SearchString);
					}
					if (!apiResponse.Success)
					{
						base.TempData["TitleError"] = apiResponse.Message;
						return RedirectToAction("Index", "Notfound");
					}
					if (apiResponse.Data != null)
					{
						iPagedList = (apiResponse.Data as List<v_ct_HoaDon>).OrderByDescending((v_ct_HoaDon s) => s.NGAYLAP).ToList().ToPagedList(Page, Utility.GetPageSize());
						text = (apiResponse.Data as List<v_ct_HoaDon>).Sum((v_ct_HoaDon s) => s.TONGTIEN).ToString("N0");
						num = (apiResponse.Data as List<v_ct_HoaDon>).Where((v_ct_HoaDon s) => s.ISXUATHOADON && !string.IsNullOrEmpty(s.MACQT)).Count();
						num2 = (apiResponse.Data as List<v_ct_HoaDon>).Where((v_ct_HoaDon s) => s.ISXUATHOADON && string.IsNullOrEmpty(s.MACQT)).Count();
						num3 = (apiResponse.Data as List<v_ct_HoaDon>).Where((v_ct_HoaDon s) => !s.ISXUATHOADON).Count();
					}
				}
				v_v_ct_HoaDon v_v_ct_HoaDon2 = new v_v_ct_HoaDon();
				v_v_ct_HoaDon2.IPagedList = iPagedList;
				v_v_ct_HoaDon2.lstdm_ThueSuat = new List<v_dm_ThueSuat>();
				v_v_ct_HoaDon2.lstdm_ThueSuat = Utility.GetListData<v_dm_ThueSuat>("Tax", "", "", Utility.LOC_ID).Data as List<v_dm_ThueSuat>;
				v_v_ct_HoaDon2.lstdm_LoaiHoaDon = new List<v_dm_LoaiHoaDon>();
				if (Utility.GetListData<v_dm_LoaiHoaDon>("TypeInvoiced", "", "", Utility.LOC_ID).Data is List<v_dm_LoaiHoaDon> source)
				{
					v_v_ct_HoaDon2.lstdm_LoaiHoaDon = source.Where((v_dm_LoaiHoaDon e) => e.ISACTIVE).ToList();
				}
				else
				{
					v_v_ct_HoaDon2.lstdm_LoaiHoaDon = new List<v_dm_LoaiHoaDon>();
				}
				v_v_ct_HoaDon2.lstdm_HTTT = Utility.DachSachHinhThucThanhToan();
				v_v_ct_HoaDon2.lstdm_TienTe = Utility.GetListData<v_dm_TienTe>("Currency", "", "", Utility.LOC_ID).Data as List<v_dm_TienTe>;
				foreach (v_dm_TienTe item in v_v_ct_HoaDon2.lstdm_TienTe)
				{
					item.ID = item.MA;
				}
				v_v_ct_HoaDon2.TYGIA = 1.0;
				text = text + "     <a class=\"label label-success\" href=\"#\" onclick=\"redirectWithSearch('" + Utility.DaCapMa.ToUpper() + "')\"><i class=\"fa fa-check-square-o\" style=\"margin-right:5px\"></i>" + Utility.DaCapMa + ((string.IsNullOrEmpty(SearchString) || SearchString.ToUpper().Trim() == Utility.DaCapMa.ToUpper()) ? (" (" + num.ToString("N0") + ")") : "") + "</a>";
				text = text + "     <a class=\"label label-warning\" href=\"#\" onclick=\"redirectWithSearch('CHỜ CẤP MÃ')\"><i class=\"fa fa-square-o\" style=\"margin-right:5px\"></i>Chờ cấp mã" + ((string.IsNullOrEmpty(SearchString) || SearchString.ToUpper().Trim() == "CHỜ CẤP MÃ") ? (" (" + num2.ToString("N0") + ")") : "") + "</a>";
				text = text + "     <a class=\"label label-default\" href=\"#\" onclick=\"redirectWithSearch('CHƯA XUẤT')\"><i class=\"fa fa-share-square-o\" style=\"margin-right:5px\"></i>Chưa xuất hóa đơn MISA" + ((string.IsNullOrEmpty(SearchString) || SearchString.ToUpper().Trim() == "CHƯA XUẤT") ? (" (" + num3.ToString("N0") + ")") : "") + "</a>";
				base.ViewBag.TotalSum = text;
				base.ViewBag.searchValue = SearchString;
				base.ViewBag.showsearchValue = ShowSearchValue;
				base.ViewBag.fromdate = (FromDate.HasValue ? FromDate.Value.ToString("yyyy-MM-dd") : Utility.CurrentTime.ToString("yyyy-MM-dd"));
				base.ViewBag.todate = (ToDate.HasValue ? ToDate.Value.ToString("yyyy-MM-dd") : Utility.CurrentTime.ToString("yyyy-MM-dd"));
				base.ViewBag.PermissionEdit = Utility.KiemTraQuyen("Invoiced", "Edit");
				base.ViewBag.PermissionDelete = Utility.KiemTraQuyen("Invoiced", "Delete");
				base.ViewBag.PermissionCreate = Utility.KiemTraQuyen("Invoiced", "Create") || Utility.KiemTraQuyen("Output", "CreateInput");
				base.ViewBag.PermissionCreateInvoiced = Utility.KiemTraQuyen("Invoiced", "CreateInput");
				return View(v_v_ct_HoaDon2);
			}
			catch (Exception ex)
			{
				Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
				base.TempData["TitleError"] = "Đã có lỗi phát sinh trong phiên làm việc!";
				base.TempData["DetailError"] = ex.Message;
				return RedirectToAction("Index", "Notfound");
			}
		}

		public ActionResult Create(int type = 2, string myModalAdd = "myModalAdd")
		{
			base.Session["IntWidth"] = type;
			try
			{
				if (Utility.KiemTra())
				{
					return RedirectToAction("Index", "Admin");
				}
				if (!Utility.KiemTraQuyen("Invoiced", "Create") && !Utility.KiemTraQuyen("Output", "CreateInput"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				v_v_ct_HoaDon v_v_ct_HoaDon2 = new v_v_ct_HoaDon();
				v_v_ct_HoaDon2.lstdm_ThueSuat = new List<v_dm_ThueSuat>();
				v_v_ct_HoaDon2.lstdm_KhachHang = new List<ComboboxFrom>();
				v_v_ct_HoaDon2.lstdm_HTTT = new List<ComboboxFrom>();
				v_v_ct_HoaDon2.lstdm_TienTe = new List<v_dm_TienTe>();
				base.ViewBag.myModalAdd = myModalAdd;
				List<Product_Detail> value = new List<Product_Detail>();
				base.Session["lstProductInvoiced"] = value;
				return View(v_v_ct_HoaDon2);
			}
			catch (Exception ex)
			{
				Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
				base.TempData["TitleError"] = "Đã có lỗi phát sinh trong phiên làm việc!";
				base.TempData["DetailError"] = ex.Message;
				return RedirectToAction("Index", "Notfound");
			}
		}

		public ActionResult CreatePopup(string ID_LOAIPHIEU, string ID_KHACHAHANG = "", string CHUNGTUKEMTHEO = "")
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
				if (!Utility.KiemTraQuyen("Invoiced", "Create") && Utility.KiemTraQuyen("Output", "CreateInput"))
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
				List<v_dm_LoaiHoaDon> source = Utility.GetListData<v_dm_LoaiHoaDon>("TypeInvoiced", "", "", Utility.LOC_ID).Data as List<v_dm_LoaiHoaDon>;
				v_dm_LoaiHoaDon v_dm_LoaiHoaDon2 = source.Where((v_dm_LoaiHoaDon e) => e.ID == ID_LOAIPHIEU).FirstOrDefault();
				if (v_dm_LoaiHoaDon2 == null || string.IsNullOrEmpty(v_dm_LoaiHoaDon2.ID))
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
				v_v_ct_HoaDon ct_HoaDon2 = new v_v_ct_HoaDon();
				apiResponse.Success = true;
				ct_HoaDon2.ID_LOAIHOADON = ID_LOAIPHIEU;
				ct_HoaDon2.LOC_ID = Utility.LOC_ID;
				ct_HoaDon2.ID = Guid.NewGuid().ToString();
				v_v_ct_HoaDon obj = ct_HoaDon2;
				DateTime nGAYHOADON = (ct_HoaDon2.NGAYLAP = Utility.CurrentTime);
				obj.NGAYHOADON = nGAYHOADON;
				ct_HoaDon2.SOPHIEU = Utility.GetMaxID((ct_HoaDon)ct_HoaDon2, Utility.LOC_ID, ct_HoaDon2.NGAYLAP.ToString("yyyy-MM-dd"));
				ct_HoaDon2.MAPHIEU = API.GetMaPhieu("Invoiced", ct_HoaDon2.NGAYLAP, ct_HoaDon2.SOPHIEU);
				ct_HoaDon2.CHUNGTUKEMTHEO = CHUNGTUKEMTHEO;
				ct_HoaDon2.ID_KHACHHANG = ID_KHACHAHANG;
				ct_HoaDon2.lstdm_KhachHang = new List<ComboboxFrom>();
				ct_HoaDon2.lstdm_KhachHang = Utility.GetListData<ComboboxFrom>("Customer", "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
				ct_HoaDon2.lstdm_LoaiHoaDon = new List<v_dm_LoaiHoaDon>();
				ct_HoaDon2.lstdm_LoaiHoaDon = Utility.GetListData<v_dm_LoaiHoaDon>("TypeInvoiced", "", "", Utility.LOC_ID).Data as List<v_dm_LoaiHoaDon>;
				ct_HoaDon2.lstdm_HTTT = Utility.DachSachHinhThucThanhToan();
				ct_HoaDon2.lstdm_TienTe = Utility.GetListData<v_dm_TienTe>("Currency", "", "", Utility.LOC_ID).Data as List<v_dm_TienTe>;
				foreach (v_dm_TienTe item in ct_HoaDon2.lstdm_TienTe)
				{
					item.ID = item.MA;
				}
				ct_HoaDon2.TYGIA = 1.0;
				base.Session["lstProductInvoiced"] = new List<Product_Detail>();
				List<Product_Detail> list = new List<Product_Detail>();
				if (!string.IsNullOrEmpty(ID_KHACHAHANG) || !string.IsNullOrEmpty(CHUNGTUKEMTHEO))
				{
					if (string.IsNullOrEmpty(ID_KHACHAHANG))
					{
						ID_KHACHAHANG = "-1";
					}
					if (string.IsNullOrEmpty(CHUNGTUKEMTHEO))
					{
						CHUNGTUKEMTHEO = "-1";
					}
					foreach (ComboboxFrom item2 in ct_HoaDon2.lstdm_KhachHang.Where((ComboboxFrom s) => s.ISDEFAULT))
					{
						item2.ISDEFAULT = false;
					}
					ComboboxFrom comboboxFrom = ct_HoaDon2.lstdm_KhachHang.Where((ComboboxFrom s) => s.ID == ct_HoaDon2.ID_KHACHHANG).FirstOrDefault();
					if (comboboxFrom != null)
					{
						comboboxFrom.ISDEFAULT = true;
					}
					ApiResponse detail = Utility.GetDetail<v_ct_HoaDon>(Utility.LOC_ID + "/" + ID_KHACHAHANG + "/" + CHUNGTUKEMTHEO, "Invoiced");
					if (detail.Data != null && detail.Data is v_ct_HoaDon v_ct_HoaDon2)
					{
						ct_HoaDon2.ID_KHACHHANG = v_ct_HoaDon2.ID_KHACHHANG;
						ct_HoaDon2.TENKHACHHANG = v_ct_HoaDon2.TENKHACHHANG;
						ct_HoaDon2.TENDONVI = v_ct_HoaDon2.TENDONVI;
						ct_HoaDon2.DIACHI = v_ct_HoaDon2.DIACHI;
						ct_HoaDon2.MASOTHUE = v_ct_HoaDon2.MASOTHUE;
						ct_HoaDon2.DIENTHOAI = v_ct_HoaDon2.DIENTHOAI;
						ct_HoaDon2.EMAIL = v_ct_HoaDon2.EMAIL;
						ct_HoaDon2.CCCD = v_ct_HoaDon2.CCCD;
						list = v_ct_HoaDon2.lstct_HoaDon_ChiTiet_TraVe.ToList();
						ct_HoaDon2.TONGTHANHTIEN = v_ct_HoaDon2.TONGTHANHTIEN;
						ct_HoaDon2.TONGTIENVAT = v_ct_HoaDon2.TONGTIENVAT;
						ct_HoaDon2.TONGTIENGIAMGIA = v_ct_HoaDon2.TONGTIENGIAMGIA;
						ct_HoaDon2.TONGTIEN = v_ct_HoaDon2.TONGTIEN;
					}
				}
				if (!string.IsNullOrEmpty(ID_KHACHAHANG) || !string.IsNullOrEmpty(CHUNGTUKEMTHEO))
				{
					v_v_ct_HoaDon obj2 = ct_HoaDon2;
					string text = (apiResponse.NAME = (myModalAdd = "myModalAddInvoiced"));
					obj2.myModalAdd = text;
				}
				else
				{
					v_v_ct_HoaDon obj3 = ct_HoaDon2;
					string text = (apiResponse.NAME = "myModalAdd");
					obj3.myModalAdd = text;
				}
				List<ValueEdit> list2 = Utility.ConvertobjectTo(ct_HoaDon2);
				apiResponse.ProductCombo = Utility.GetProductInvoiced(list, "Invoiced", bolTinhLai: false, ct_HoaDon2, myModalAdd);
				base.Session["lstProductInvoiced"] = list;
				list2.Add(new ValueEdit
				{
					Key = "tbodyTempItemInvoiced",
					Value = apiResponse.ProductCombo
				});
				list2.Add(new ValueEdit
				{
					Key = "lblName",
					Value = v_dm_LoaiHoaDon2.NAME.ToUpper()
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
		public ActionResult CreatePopup([Bind(Include = "myModalAdd,LOC_ID,ID,ID_LOAIHOADON,MAPHIEU,SOPHIEU,NGAYLAP,ID_KHACHHANG,GHICHU,TONGTHANHTIEN,CHIETKHAU,TONGTIENGIAMGIA,TONGTIENVAT,TONGTIEN,BUTTONTYPE,ADDRESS,TEL,CHUNGTUKEMTHEO,MASOTHUE,TENKHACHHANG,TENDONVI,DIACHI,CCCD,DIENTHOAI,EMAIL,HTTT,LOAITIEN,TYGIA,NGAYHOADON")] v_v_ct_HoaDon ct_HoaDon)
		{
			try
			{
				if (Utility.KiemTra())
				{
					return RedirectToAction("Index", "Admin");
				}
				if (!Utility.KiemTraQuyen("Invoiced", "Create"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				ct_HoaDon.lstct_HoaDon_ChiTiet = new List<v_ct_HoaDon_ChiTiet>();
				List<Product_Detail> list = new List<Product_Detail>();
				IEnumerable<string> enumerable = base.Request.Form.AllKeys.Where((string e) => e.StartsWith("txt"));
				if (enumerable == null || enumerable.Count() == 0)
				{
					base.ModelState.AddModelError("lstct_HoaDon_ChiTiet", "Thêm danh sách hàng hóa.");
				}
				else
				{
					v_ct_HoaDon_ChiTiet v_ct_HoaDon_ChiTiet2 = new v_ct_HoaDon_ChiTiet();
					foreach (string item in enumerable)
					{
						string[] array = item.ToString().Split('|');
						string[] values = base.HttpContext.Request.Params.GetValues(item.ToString());
						string value = clsMaHoa.Decrypt(array[1].ToString(), "tmt6364");
						Product_Detail product_Detail = JsonConvert.DeserializeObject<Product_Detail>(value);
						if (v_ct_HoaDon_ChiTiet2.ID != product_Detail.ID)
						{
							v_ct_HoaDon_ChiTiet2 = new v_ct_HoaDon_ChiTiet();
							v_ct_HoaDon_ChiTiet2 = JsonConvert.DeserializeObject<v_ct_HoaDon_ChiTiet>(value);
							v_ct_HoaDon_ChiTiet2.LOC_ID = ct_HoaDon.LOC_ID;
							ct_HoaDon.lstct_HoaDon_ChiTiet.Add(v_ct_HoaDon_ChiTiet2);
							list.Add(product_Detail);
						}
						Utility.EditObject(v_ct_HoaDon_ChiTiet2, array[0].ToString().Substring(3, array[0].ToString().Length - 3), values[0]);
					}
				}
				ApiResponse apiResponse = new ApiResponse();
				if (base.ModelState.IsValid)
				{
					ct_HoaDon.NGAYLAP = ct_HoaDon.NGAYLAP.AddHours(Utility.CurrentTime.Hour).AddMinutes(Utility.CurrentTime.Minute);
					ct_HoaDon.ID = Guid.NewGuid().ToString();
					ct_HoaDon.LOC_ID = Utility.LOC_ID;
					ct_HoaDon.ID_NGUOITAO = base.Session["idUser"].ToString();
					ct_HoaDon.THOIGIANTHEM = Utility.CurrentTime;
					ct_HoaDon.lstct_HoaDon_ChiTiet_TraVe = new List<Product_Detail>();
					apiResponse = Utility.Create((v_ct_HoaDon)ct_HoaDon, "Invoiced");
					if (apiResponse.Success)
					{
						ct_HoaDon.NGAYLAP = Utility.CurrentTime;
						ApiResponse apiResponse2 = apiResponse;
						int sOPHIEU = (ct_HoaDon.SOPHIEU = Utility.GetMaxID((ct_HoaDon)ct_HoaDon, Utility.LOC_ID, ct_HoaDon.NGAYLAP.ToString("yyyy-MM-dd")));
						apiResponse2.SOPHIEU = sOPHIEU;
						ct_HoaDon.MAPHIEU = API.GetMaPhieu("Invoiced", ct_HoaDon.NGAYLAP, ct_HoaDon.SOPHIEU);
						apiResponse.NewID = Guid.NewGuid().ToString();
						if (ct_HoaDon.myModalAdd != "myModalAddInvoiced")
						{
							apiResponse.MAPHIEU = ct_HoaDon.MAPHIEU;
						}
						if (apiResponse.Data != null)
						{
							ct_HoaDon = JsonConvert.DeserializeObject<v_v_ct_HoaDon>(apiResponse.Data.ToString());
						}
						list = new List<Product_Detail>();
					}
					else
					{
						base.ModelState.AddModelError(string.Empty, apiResponse.Message);
						if (apiResponse.CheckValue)
						{
							ct_HoaDon.NGAYLAP = Utility.CurrentTime;
							ApiResponse apiResponse3 = apiResponse;
							int sOPHIEU = (ct_HoaDon.SOPHIEU = Utility.GetMaxID((ct_HoaDon)ct_HoaDon, Utility.LOC_ID, ct_HoaDon.NGAYLAP.ToString("yyyy-MM-dd")));
							apiResponse3.SOPHIEU = sOPHIEU;
							ct_HoaDon.MAPHIEU = API.GetMaPhieu("Invoiced", ct_HoaDon.NGAYLAP, ct_HoaDon.SOPHIEU);
							apiResponse.NewID = Guid.NewGuid().ToString();
							if (myModalAdd != "myModalAddInvoiced")
							{
								apiResponse.MAPHIEU = ct_HoaDon.MAPHIEU;
							}
						}
					}
				}
				else
				{
					apiResponse.Success = false;
					apiResponse.Data = Utility.GetModelState(base.ModelState, "Invoiced");
				}
				base.Session["lstProductInvoiced"] = list;
				apiResponse.ID = ct_HoaDon.ID;
				List<v_dm_LoaiHoaDon> list2 = Utility.GetListData<v_dm_LoaiHoaDon>("TypeInvoiced", "", "", Utility.LOC_ID).Data as List<v_dm_LoaiHoaDon>;
				ct_HoaDon.lstdm_KhachHang = new List<ComboboxFrom>();
				ct_HoaDon.lstdm_KhachHang = Utility.GetListData<ComboboxFrom>("Customer", "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
				ct_HoaDon.lstdm_LoaiHoaDon = new List<v_dm_LoaiHoaDon>();
				ct_HoaDon.lstdm_HTTT = Utility.DachSachHinhThucThanhToan();
				ct_HoaDon.lstdm_TienTe = Utility.GetListData<v_dm_TienTe>("Currency", "", "", Utility.LOC_ID).Data as List<v_dm_TienTe>;
				foreach (v_dm_TienTe item2 in ct_HoaDon.lstdm_TienTe)
				{
					item2.ID = item2.MA;
				}
				ct_HoaDon.TYGIA = 1.0;
				List<ValueEdit> list3 = Utility.ConvertobjectToView(ct_HoaDon, "dd/MM/yy HH:mm");
				apiResponse.ProductCombo = Utility.GetProductInvoiced(list, "Invoiced", bolTinhLai: false, ct_HoaDon, ct_HoaDon.myModalAdd);
				list3.Add(new ValueEdit
				{
					Key = "tbodyTempItemInvoiced",
					Value = apiResponse.ProductCombo
				});
				apiResponse.Detail = list3;
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
				if (!Utility.KiemTraQuyen("Invoiced", "Edit"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				List<Product_Detail> list = new List<Product_Detail>();
				v_v_ct_HoaDon v_v_ct_HoaDon2 = new v_v_ct_HoaDon();
				if (!string.IsNullOrEmpty(id))
				{
					apiResponse = Utility.GetDetail<v_v_ct_HoaDon>(Utility.LOC_ID + "/" + id, "Invoiced");
					if (!apiResponse.Success)
					{
						base.TempData["TitleError"] = apiResponse.Message;
						return RedirectToAction("Index", "Notfound");
					}
					if (apiResponse.Data != null)
					{
						v_v_ct_HoaDon2 = apiResponse.Data as v_v_ct_HoaDon;
					}
					foreach (v_ct_HoaDon_ChiTiet item in v_v_ct_HoaDon2.lstct_HoaDon_ChiTiet)
					{
						list.Add(Utility.ConvertobjectToProduct_Detail(item, new Product_Detail()));
					}
				}
				v_v_ct_HoaDon2.lstdm_KhachHang = new List<ComboboxFrom>();
				v_v_ct_HoaDon2.lstdm_ThueSuat = new List<v_dm_ThueSuat>();
				v_v_ct_HoaDon2.lstdm_HTTT = new List<ComboboxFrom>();
				v_v_ct_HoaDon2.lstdm_TienTe = new List<v_dm_TienTe>();
				base.Session["lstProductInvoiced"] = list;
				apiResponse.ProductCombo = Utility.GetProductInvoiced(list, "Invoiced", bolTinhLai: false, v_v_ct_HoaDon2, myModalEdit);
				base.ViewBag.DatHang = apiResponse.ProductCombo;
				return View(v_v_ct_HoaDon2);
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
				if (!Utility.KiemTraQuyen("Invoiced", "Edit"))
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
				v_v_ct_HoaDon ct_HoaDon2 = new v_v_ct_HoaDon();
				if (!string.IsNullOrEmpty(id))
				{
					apiResponse = Utility.GetDetail<v_v_ct_HoaDon>(Utility.LOC_ID + "/" + id, "Invoiced");
					if (!apiResponse.Success)
					{
						base.TempData["TitleError"] = apiResponse.Message;
						return RedirectToAction("Index", "Notfound");
					}
					if (apiResponse.Data != null)
					{
						ct_HoaDon2 = apiResponse.Data as v_v_ct_HoaDon;
					}
				}
				foreach (v_ct_HoaDon_ChiTiet item in ct_HoaDon2.lstct_HoaDon_ChiTiet)
				{
					list.Add(Utility.ConvertobjectToProduct_Detail(item, new Product_Detail()));
				}
				List<v_dm_LoaiHoaDon> list2 = Utility.GetListData<v_dm_LoaiHoaDon>("TypeInvoiced", "", "", Utility.LOC_ID).Data as List<v_dm_LoaiHoaDon>;
				v_dm_LoaiHoaDon v_dm_LoaiHoaDon2 = list2.Where((v_dm_LoaiHoaDon e) => e.ID == ct_HoaDon2.ID_LOAIHOADON).FirstOrDefault();
				if (v_dm_LoaiHoaDon2 == null || string.IsNullOrEmpty(v_dm_LoaiHoaDon2.ID))
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
				ct_HoaDon2.lstdm_KhachHang = new List<ComboboxFrom>();
				ct_HoaDon2.lstdm_KhachHang = Utility.GetListData<ComboboxFrom>("Customer", "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
				base.Session["lstProductInvoiced"] = list;
				ct_HoaDon2.lstdm_LoaiHoaDon = new List<v_dm_LoaiHoaDon>();
				ct_HoaDon2.lstdm_LoaiHoaDon = list2;
				ct_HoaDon2.lstdm_HTTT = Utility.DachSachHinhThucThanhToan();
				ct_HoaDon2.lstdm_TienTe = Utility.GetListData<v_dm_TienTe>("Currency", "", "", Utility.LOC_ID).Data as List<v_dm_TienTe>;
				foreach (v_dm_TienTe item2 in ct_HoaDon2.lstdm_TienTe)
				{
					item2.ID = item2.MA;
				}
				List<ValueEdit> list3 = Utility.ConvertobjectTo(ct_HoaDon2);
				apiResponse.ProductCombo = Utility.GetProductInvoiced(list, "Invoiced", bolTinhLai: false, ct_HoaDon2, myModalEdit);
				list3.Add(new ValueEdit
				{
					Key = "tbodyTempItemInvoicedEdit",
					Value = apiResponse.ProductCombo
				});
				list3.Add(new ValueEdit
				{
					Key = "lblName",
					Value = v_dm_LoaiHoaDon2.NAME.ToUpper() + "(" + ct_HoaDon2.MAPHIEU + ")"
				});
				apiResponse.Detail = list3;
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
		public ActionResult Edit([Bind(Include = "ID_NGUOITAO,THOIGIANTHEM,LOC_ID,ID,ID_LOAIHOADON,MAPHIEU,SOPHIEU,NGAYLAP,ID_KHACHHANG,GHICHU,TONGTHANHTIEN,CHIETKHAU,TONGTIENGIAMGIA,TONGTIENVAT,TONGTIEN,BUTTONTYPE,ADDRESS,TEL,CHUNGTUKEMTHEO,MASOTHUE,TENKHACHHANG,TENDONVI,DIACHI,CCCD,DIENTHOAI,EMAIL,HTTT,LOAITIEN,TYGIA,NGAYHOADON")] v_v_ct_HoaDon ct_HoaDon)
		{
			try
			{
				if (Utility.KiemTra())
				{
					return RedirectToAction("Index", "Admin");
				}
				if (!Utility.KiemTraQuyen("Invoiced", "Edit"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				List<Product_Detail> list = new List<Product_Detail>();
				ApiResponse apiResponse = new ApiResponse();
				ct_HoaDon.lstct_HoaDon_ChiTiet = new List<v_ct_HoaDon_ChiTiet>();
				IEnumerable<string> enumerable = base.Request.Form.AllKeys.Where((string e) => e.StartsWith("txt"));
				if (enumerable == null || enumerable.Count() == 0)
				{
					base.ModelState.AddModelError("lstct_HoaDon_ChiTiet", "Thêm danh sách hàng hóa.");
				}
				else
				{
					v_ct_HoaDon_ChiTiet v_ct_HoaDon_ChiTiet2 = new v_ct_HoaDon_ChiTiet();
					foreach (string item in enumerable)
					{
						string[] array = item.ToString().Split('|');
						string[] values = base.HttpContext.Request.Params.GetValues(item.ToString());
						string value = clsMaHoa.Decrypt(array[1].ToString(), "tmt6364");
						Product_Detail product_Detail = JsonConvert.DeserializeObject<Product_Detail>(value);
						if (v_ct_HoaDon_ChiTiet2.ID != product_Detail.ID)
						{
							v_ct_HoaDon_ChiTiet2 = new v_ct_HoaDon_ChiTiet();
							v_ct_HoaDon_ChiTiet2 = JsonConvert.DeserializeObject<v_ct_HoaDon_ChiTiet>(value);
							v_ct_HoaDon_ChiTiet2.LOC_ID = ct_HoaDon.LOC_ID;
							ct_HoaDon.lstct_HoaDon_ChiTiet.Add(v_ct_HoaDon_ChiTiet2);
							list.Add(product_Detail);
						}
						Utility.EditObject(v_ct_HoaDon_ChiTiet2, array[0].ToString().Substring(3, array[0].ToString().Length - 3), values[0]);
					}
				}
				if (base.ModelState.IsValid)
				{
					apiResponse = Utility.GetDetail<v_ct_HoaDon>(Utility.LOC_ID + "/" + ct_HoaDon.ID, "Invoiced");
					if (!apiResponse.Success)
					{
						base.TempData["TitleError"] = apiResponse.Message;
						return RedirectToAction("Index", "Notfound");
					}
					v_ct_HoaDon v_ct_HoaDon2 = null;
					if (apiResponse.Data != null)
					{
						v_ct_HoaDon2 = apiResponse.Data as v_ct_HoaDon;
					}
					ct_HoaDon.LOC_ID = Utility.LOC_ID;
					ct_HoaDon.ID_NGUOISUA = base.Session["idUser"].ToString();
					ct_HoaDon.THOIGIANSUA = Utility.CurrentTime;
					apiResponse = Utility.Edit(Utility.LOC_ID + "/" + ct_HoaDon.ID, (v_ct_HoaDon)ct_HoaDon, "Invoiced");
					if (apiResponse.Success)
					{
						apiResponse.ID = ct_HoaDon.ID;
						if (apiResponse.Data != null)
						{
							ct_HoaDon = JsonConvert.DeserializeObject<v_v_ct_HoaDon>(apiResponse.Data.ToString());
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
					apiResponse.Data = Utility.GetModelState(base.ModelState, "Invoiced");
				}
				base.Session["lstProductInvoiced"] = list;
				apiResponse.ID = ct_HoaDon.ID;
				List<v_dm_LoaiHoaDon> lstdm_LoaiHoaDon = Utility.GetListData<v_dm_LoaiHoaDon>("TypeInvoiced", "", "", Utility.LOC_ID).Data as List<v_dm_LoaiHoaDon>;
				ct_HoaDon.lstdm_KhachHang = new List<ComboboxFrom>();
				ct_HoaDon.lstdm_KhachHang = Utility.GetListData<ComboboxFrom>("Customer", "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
				ct_HoaDon.lstdm_LoaiHoaDon = new List<v_dm_LoaiHoaDon>();
				ct_HoaDon.lstdm_LoaiHoaDon = lstdm_LoaiHoaDon;
				ct_HoaDon.lstdm_HTTT = Utility.DachSachHinhThucThanhToan();
				ct_HoaDon.lstdm_TienTe = Utility.GetListData<v_dm_TienTe>("Currency", "", "", Utility.LOC_ID).Data as List<v_dm_TienTe>;
				foreach (v_dm_TienTe item2 in ct_HoaDon.lstdm_TienTe)
				{
					item2.ID = item2.MA;
				}
				List<ValueEdit> list2 = Utility.ConvertobjectToView(ct_HoaDon);
				apiResponse.ProductCombo = Utility.GetProductInvoiced(list, "Invoiced", bolTinhLai: false, ct_HoaDon, myModalEdit);
				list2.Add(new ValueEdit
				{
					Key = "tbodyTempItemInvoicedEdit",
					Value = apiResponse.ProductCombo
				});
				apiResponse.Detail = list2;
				return View(ct_HoaDon);
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
		public ActionResult EditPopup([Bind(Include = "ID_NGUOITAO,THOIGIANTHEM,LOC_ID,ID,ID_LOAIHOADON,MAPHIEU,SOPHIEU,NGAYLAP,ID_KHACHHANG,GHICHU,TONGTHANHTIEN,CHIETKHAU,TONGTIENGIAMGIA,TONGTIENVAT,TONGTIEN,BUTTONTYPE,ADDRESS,TEL,CHUNGTUKEMTHEO,MASOTHUE,TENKHACHHANG,TENDONVI,DIACHI,CCCD,DIENTHOAI,EMAIL,HTTT,LOAITIEN,TYGIA,NGAYHOADON")] v_v_ct_HoaDon ct_HoaDon)
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
				if (!Utility.KiemTraQuyen("Invoiced", "Create"))
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
				ct_HoaDon.lstct_HoaDon_ChiTiet = new List<v_ct_HoaDon_ChiTiet>();
				IEnumerable<string> enumerable = base.Request.Form.AllKeys.Where((string e) => e.StartsWith("txt"));
				if (enumerable == null || enumerable.Count() == 0)
				{
					base.ModelState.AddModelError("lstct_HoaDon_ChiTiet", "Thêm danh sách hàng hóa.");
				}
				else
				{
					v_ct_HoaDon_ChiTiet v_ct_HoaDon_ChiTiet2 = new v_ct_HoaDon_ChiTiet();
					foreach (string item in enumerable)
					{
						string[] array = item.ToString().Split('|');
						string[] values = base.HttpContext.Request.Params.GetValues(item.ToString());
						string value = clsMaHoa.Decrypt(array[1].ToString(), "tmt6364");
						Product_Detail product_Detail = JsonConvert.DeserializeObject<Product_Detail>(value);
						if (v_ct_HoaDon_ChiTiet2.ID != product_Detail.ID)
						{
							v_ct_HoaDon_ChiTiet2 = new v_ct_HoaDon_ChiTiet();
							v_ct_HoaDon_ChiTiet2 = JsonConvert.DeserializeObject<v_ct_HoaDon_ChiTiet>(value);
							v_ct_HoaDon_ChiTiet2.LOC_ID = ct_HoaDon.LOC_ID;
							ct_HoaDon.lstct_HoaDon_ChiTiet.Add(v_ct_HoaDon_ChiTiet2);
							list.Add(product_Detail);
						}
						Utility.EditObject(v_ct_HoaDon_ChiTiet2, array[0].ToString().Substring(3, array[0].ToString().Length - 3), values[0]);
					}
				}
				if (base.ModelState.IsValid)
				{
					apiResponse = Utility.GetDetail<v_ct_HoaDon>(Utility.LOC_ID + "/" + ct_HoaDon.ID, "Invoiced");
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
					v_ct_HoaDon v_ct_HoaDon2 = null;
					if (apiResponse.Data != null)
					{
						v_ct_HoaDon2 = apiResponse.Data as v_ct_HoaDon;
					}
					ct_HoaDon.LOC_ID = Utility.LOC_ID;
					ct_HoaDon.ID_NGUOISUA = base.Session["idUser"].ToString();
					ct_HoaDon.THOIGIANSUA = Utility.CurrentTime;
					apiResponse = Utility.Edit(Utility.LOC_ID + "/" + ct_HoaDon.ID, (v_ct_HoaDon)ct_HoaDon, "Invoiced");
					if (apiResponse.Success)
					{
						apiResponse.ID = ct_HoaDon.ID;
						if (apiResponse.Data != null)
						{
							ct_HoaDon = JsonConvert.DeserializeObject<v_v_ct_HoaDon>(apiResponse.Data.ToString());
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
					apiResponse.Data = Utility.GetModelState(base.ModelState, "Invoiced");
				}
				base.Session["lstProductInvoiced"] = list;
				apiResponse.ID = ct_HoaDon.ID;
				List<v_dm_LoaiHoaDon> lstdm_LoaiHoaDon = Utility.GetListData<v_dm_LoaiHoaDon>("TypeInvoiced", "", "", Utility.LOC_ID).Data as List<v_dm_LoaiHoaDon>;
				ct_HoaDon.lstdm_LoaiHoaDon = new List<v_dm_LoaiHoaDon>();
				ct_HoaDon.lstdm_LoaiHoaDon = lstdm_LoaiHoaDon;
				ct_HoaDon.lstdm_HTTT = Utility.DachSachHinhThucThanhToan();
				ct_HoaDon.lstdm_TienTe = Utility.GetListData<v_dm_TienTe>("Currency", "", "", Utility.LOC_ID).Data as List<v_dm_TienTe>;
				foreach (v_dm_TienTe item2 in ct_HoaDon.lstdm_TienTe)
				{
					item2.ID = item2.MA;
				}
				List<ValueEdit> list2 = Utility.ConvertobjectToView(ct_HoaDon, "dd/MM/yy HH:mm");
				apiResponse.ProductCombo = Utility.GetProductInvoiced(list, "Invoiced", bolTinhLai: false, ct_HoaDon, myModalEdit);
				list2.Add(new ValueEdit
				{
					Key = "tbodyTempItemInvoicedEdit",
					Value = apiResponse.ProductCombo
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
				if (!Utility.KiemTraQuyen("Invoiced", "Delete"))
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
				apiResponse = Utility.Delete<v_ct_HoaDon>(Utility.LOC_ID + "/" + id, "Invoiced");
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

		[HttpPost]
		public ActionResult OnSubmitDeposit(string cartOrder, string HINHTHUC = "")
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
					item.NGAYLAP = Utility.CurrentTime;
					item.ID_LOAIHOADON = HINHTHUC;
				}
				apiResponse = Utility.Create(list, "Invoiced/PostCreateOutput");
				if (apiResponse.Success)
				{
					obj.Message = "Tạo hóa đơn thành công!";
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

		[HttpPost]
		public ActionResult UpdateProductDeposit_Temp(string ID, string TYPE, string VALUE, bool bolTinhLai, double TONGTHANHTIEN, double TONGTIENGIAMGIA, double TONGTIENVAT, double TONGTIEN, string MYBTN)
		{
			List<Product_Detail> list = new List<Product_Detail>();
			if (base.Session["lstProductInvoiced"] != null)
			{
				list = (List<Product_Detail>)base.Session["lstProductInvoiced"];
			}
			ApiResponse apiResponse = new ApiResponse();
			try
			{
				v_v_dm_HangHoa v_v_dm_HangHoa2 = new v_v_dm_HangHoa();
				v_ct_HoaDon v_ct_HoaDon2 = new v_ct_HoaDon();
				Product_Detail product_Detail = list.Where((Product_Detail e) => e.ID == ID).FirstOrDefault();
				if (product_Detail != null && bolTinhLai)
				{
					product_Detail.TYPE = TYPE;
					Utility.TinhTongVAT(product_Detail, VALUE, list);
				}
				else
				{
					v_ct_HoaDon2.TONGTIENGIAMGIA = TONGTIENGIAMGIA;
					v_ct_HoaDon2.TONGTHANHTIEN = TONGTHANHTIEN;
					v_ct_HoaDon2.TONGTIENVAT = TONGTIENVAT;
					v_ct_HoaDon2.TONGTIEN = TONGTIEN;
				}
				base.Session["lstProductInvoiced"] = list;
				apiResponse.ProductCombo = Utility.GetProductInvoiced(list, "Invoiced", bolTinhLai, v_ct_HoaDon2, MYBTN);
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
		public ActionResult AddInvoiced(double TONGTHANHTIEN, double TONGTIENGIAMGIA, double TONGTIENVAT, double TONGTIEN, string MYBTN)
		{
			List<Product_Detail> list = new List<Product_Detail>();
			if (base.Session["lstProductInvoiced"] != null)
			{
				list = (List<Product_Detail>)base.Session["lstProductInvoiced"];
			}
			ApiResponse apiResponse = new ApiResponse();
			try
			{
				v_v_dm_HangHoa v_v_dm_HangHoa2 = new v_v_dm_HangHoa();
				v_ct_HoaDon v_ct_HoaDon2 = new v_ct_HoaDon();
				int sTT = list.Max((Product_Detail e) => e.STT) + 1;
				v_ct_HoaDon2.TONGTIENGIAMGIA = TONGTIENGIAMGIA;
				v_ct_HoaDon2.TONGTHANHTIEN = TONGTHANHTIEN;
				v_ct_HoaDon2.TONGTIENVAT = TONGTIENVAT;
				v_ct_HoaDon2.TONGTIEN = TONGTIEN;
				Product_Detail product_Detail = new Product_Detail();
				product_Detail.STT = sTT;
				product_Detail.ID = Guid.NewGuid().ToString();
				product_Detail.TINHCHAT = 1;
				list.Add(product_Detail);
				base.Session["lstProductInvoiced"] = list;
				apiResponse.ProductCombo = Utility.GetProductInvoiced(list, "Invoiced", bolTinhLai: false, v_ct_HoaDon2, MYBTN);
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
		public ActionResult CallChangeCustomer(string id, string myModal)
		{
			if (Utility.KiemTra())
			{
				return RedirectToAction("Index", "Admin");
			}
			ApiResponse data = new ApiResponse();
			if (base.ModelState.IsValid)
			{
				data = Utility.GetDetail<v_dm_KhachHang>(Utility.LOC_ID + "/" + id, "Customer");
				if (!data.Success)
				{
					base.TempData["TitleError"] = data.Message;
				}
				else
				{
					v_dm_KhachHang v_dm_KhachHang2 = data.Data as v_dm_KhachHang;
					List<ValueEdit> list = new List<ValueEdit>();
					list.Add(new ValueEdit
					{
						Key = "TENDONVI",
						Value = ((!string.IsNullOrEmpty(v_dm_KhachHang2.TENDONVI)) ? v_dm_KhachHang2.TENDONVI : "")
					});
					list.Add(new ValueEdit
					{
						Key = "MASOTHUE",
						Value = ((!string.IsNullOrEmpty(v_dm_KhachHang2.MASOTHUE)) ? v_dm_KhachHang2.MASOTHUE : "")
					});
					list.Add(new ValueEdit
					{
						Key = "DIACHI",
						Value = ((!string.IsNullOrEmpty(v_dm_KhachHang2.DIACHI)) ? v_dm_KhachHang2.DIACHI : "")
					});
					list.Add(new ValueEdit
					{
						Key = "TEL",
						Value = ((!string.IsNullOrEmpty(v_dm_KhachHang2.TEL)) ? v_dm_KhachHang2.TEL : "")
					});
					list.Add(new ValueEdit
					{
						Key = "EMAIL",
						Value = ((!string.IsNullOrEmpty(v_dm_KhachHang2.EMAIL)) ? v_dm_KhachHang2.EMAIL : "")
					});
					list.Add(new ValueEdit
					{
						Key = "CCCD",
						Value = ((!string.IsNullOrEmpty(v_dm_KhachHang2.CCCD)) ? v_dm_KhachHang2.CCCD : "")
					});
					list.Add(new ValueEdit
					{
						Key = "TENKHACHHANG",
						Value = ((!string.IsNullOrEmpty(v_dm_KhachHang2.TENKHACHHANG)) ? v_dm_KhachHang2.TENKHACHHANG : "")
					});
					data.Detail = list;
				}
				data.NAME = myModal;
				return new JsonResult
				{
					Data = data,
					JsonRequestBehavior = JsonRequestBehavior.AllowGet,
					MaxJsonLength = int.MaxValue
				};
			}
			return new JsonResult
			{
				Data = data,
				JsonRequestBehavior = JsonRequestBehavior.AllowGet,
				MaxJsonLength = int.MaxValue
			};
		}

		[HttpPost]
		public ActionResult Invoiced(string id)
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
				if (!Utility.KiemTraQuyen("Invoiced", "Create") && !Utility.KiemTraQuyen("Output", "CreateInput"))
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
				List<Deposit> list = new List<Deposit>();
				Deposit deposit = new Deposit();
				deposit.ID_NGUOITAO = base.Session["idUser"].ToString();
				deposit.LOC_ID = Utility.LOC_ID;
				deposit.NGAYLAP = Utility.CurrentTime;
				deposit.ID = id;
				list.Add(deposit);
				apiResponse = Utility.Create(list, "Invoiced_Misa/" + Utility.LOC_ID);
				if (apiResponse.Success)
				{
					apiResponse.Message = "Tạo hóa đơn thành công!";
				}
				else
				{
					base.ModelState.AddModelError(string.Empty, apiResponse.Message);
				}
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

		[HttpPost]
		public ActionResult InvoicedList(string cartOrder)
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
					item.NGAYLAP = Utility.CurrentTime;
				}
				apiResponse = Utility.Create(list, "Invoiced_Misa/" + Utility.LOC_ID);
				if (apiResponse.Success)
				{
					obj.Message = "Tạo hóa đơn thành công!";
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

		[HttpPost]
		public ActionResult DeleteInvoicedList(string cartOrder)
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
					item.NGAYLAP = Utility.CurrentTime;
				}
				apiResponse = Utility.Edit(Utility.LOC_ID, list, "Invoiced");
				if (apiResponse.Success)
				{
					obj.Message = "Xóa hóa đơn thành công!";
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

		[HttpPost]
		public ActionResult GetInvoicedList(string cartOrder)
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
				List<Deposit> model = new JavaScriptSerializer().Deserialize<List<Deposit>>(cartOrder);
				apiResponse = Utility.Edit(Utility.LOC_ID, model, "Invoiced_Misa");
				if (apiResponse.Success)
				{
					apiResponse.Message = "Cập nhật " + apiResponse.Data?.ToString() + " hóa đơn thành công!";
				}
				else
				{
					base.ModelState.AddModelError(string.Empty, apiResponse.Message);
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
		public ActionResult DeleteProductInputOutput(string ID, bool bolTinhLai, double TONGTHANHTIEN, double TONGTIENGIAMGIA, double TONGTIENVAT, double TONGTIEN, string MYBTN)
		{
			ApiResponse apiResponse = new ApiResponse();
			try
			{
				v_v_dm_HangHoa v_v_dm_HangHoa2 = new v_v_dm_HangHoa();
				List<Product_Detail> lstProductInvoiced = Utility.LstProductInvoiced;
				Product_Detail check = Utility.LstProductInvoiced.Where((Product_Detail e) => e.ID == ID).FirstOrDefault();
				if (check != null && lstProductInvoiced != null)
				{
					if (!string.IsNullOrEmpty(check.ID_COMBO))
					{
						foreach (Product_Detail item in lstProductInvoiced.Where((Product_Detail e) => e.ID_COMBO == check.ID_COMBO).ToList())
						{
							lstProductInvoiced.Remove(item);
						}
					}
					else
					{
						lstProductInvoiced.Remove(check);
					}
				}
				base.Session["lstProductInvoiced"] = lstProductInvoiced;
				v_ct_HoaDon v_ct_HoaDon2 = new v_ct_HoaDon();
				if (!bolTinhLai)
				{
					v_ct_HoaDon2.TONGTIENGIAMGIA = TONGTIENGIAMGIA;
					v_ct_HoaDon2.TONGTHANHTIEN = TONGTHANHTIEN;
					v_ct_HoaDon2.TONGTIENVAT = TONGTIENVAT;
					v_ct_HoaDon2.TONGTIEN = TONGTIEN;
				}
				string absolutePath = base.Request.Url.AbsolutePath;
				apiResponse.ProductCombo = Utility.GetProductInvoiced(lstProductInvoiced, "Invoiced", bolTinhLai, v_ct_HoaDon2, MYBTN);
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
				v_ct_HoaDon v_ct_HoaDon2 = new v_ct_HoaDon();
				SP_Parameter sP_Parameter = new SP_Parameter();
				sP_Parameter.ID_HOADON = ID;
				apiResponse = Utility.ExecuteStoredProc<v_ct_HoaDon>(sP_Parameter, "Sp_Get_DanhSachHoaDon");
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
					v_ct_HoaDon2 = (apiResponse.Data as List<v_ct_HoaDon>).FirstOrDefault();
				}
				SP_Parameter_Report sP_Parameter_Report = new SP_Parameter_Report();
				sP_Parameter_Report.LOC_ID = Utility.LOC_ID;
				sP_Parameter_Report.ID_HOADON = ID;
				ReportClass report = new ReportClass();
				apiResponse = Utility.ExecuteStoredProc<DataTable>(sP_Parameter_Report, "Sp_Get_DanhSachHoaDon_Chitiet");
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
				if (apiResponse.CheckValue)
				{
					dataTable.Rows.Clear();
				}
				v_ct_HoaDon2.GHICHU = "";
				report = Utility.GetFormulaFields(report, v_ct_HoaDon2);
				report.SetDataSource(dataTable);
				base.Response.Buffer = false;
				base.Response.ClearContent();
				base.Response.ClearHeaders();
				Stream stream = report.ExportToStream(ExportFormatType.PortableDocFormat);
				Utility.Report = report;
				apiResponse = new ApiResponse();
				apiResponse.Success = true;
				string authority = base.Request.Url.Authority;
				if (base.Request.Url.AbsoluteUri.StartsWith("https"))
				{
					apiResponse.URL = "https://" + authority + "/ViewReport/VerReporte";
				}
				else
				{
					apiResponse.URL = "http://" + authority + "/ViewReport/VerReporte";
				}
				apiResponse.NAME = Utility.GetTitleFrom("Invoiced");
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
