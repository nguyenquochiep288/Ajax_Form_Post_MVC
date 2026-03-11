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
using DatabaseTHP.StoredProcedure.Parameter;
using MVC_QuanLyTHP.Class;
using MVC_QuanLyTHP.Models;
using Newtonsoft.Json;
using PagedList;

namespace MVC_QuanLyTHP.Controllers
{

	public class OutputController : Controller
	{
		public ActionResult Index(int Page = 1, string ID_DEPOT = "", DateTime? FromDate = null, DateTime? ToDate = null, string SearchString = "", string ShowSearchValue = "", string MAPHIEU = "", string IDCODE = "")
		{
			try
			{
				if (Utility.KiemTra())
				{
					return RedirectToAction("Index", "Admin");
				}
				if (!Utility.KiemTraQuyen("Output", "View"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				string text = "";
				ShowSearchValue = Utility.GetShowSearchValue<ct_PhieuXuat>(ShowSearchValue);
				ApiResponse apiResponse = new ApiResponse();
				IPagedList<v_ct_PhieuXuat> iPagedList = new List<v_ct_PhieuXuat>().OrderByDescending((v_ct_PhieuXuat s) => s.NGAYLAP).ToList().ToPagedList(Page, Utility.GetPageSize());
				if (FromDate.HasValue || !string.IsNullOrEmpty(IDCODE) || !string.IsNullOrEmpty(MAPHIEU))
				{
					if (!string.IsNullOrEmpty(IDCODE) || !string.IsNullOrEmpty(MAPHIEU))
					{
						apiResponse = Utility.Get_DanhSachPhieuXuat<v_ct_PhieuXuat>(ID_DEPOT, null, null, MAPHIEU, IDCODE);
					}
					if (FromDate.HasValue)
					{
						apiResponse = Utility.Get_DanhSachPhieuXuat<v_ct_PhieuXuat>(ID_DEPOT, FromDate, ToDate, SearchString);
					}
					if (!apiResponse.Success)
					{
						base.TempData["TitleError"] = apiResponse.Message;
						return RedirectToAction("Index", "Notfound");
					}
					if (apiResponse.Data != null)
					{
						Login_Model Login_Model = (Login_Model)base.Session["Login_Model"];
						if (Utility.KiemTraQuyen("Output", "AllData"))
						{
							iPagedList = ((string.IsNullOrEmpty(SearchString) || (!(SearchString.ToUpper() == "ĐÃ XUẤT") && !(SearchString.ToUpper() == "CHƯA XUẤT"))) ? (apiResponse.Data as List<v_ct_PhieuXuat>).OrderByDescending((v_ct_PhieuXuat s) => s.NGAYLAP).ToList().ToPagedList(Page, Utility.GetPageSize()) : (apiResponse.Data as List<v_ct_PhieuXuat>).ToList().ToPagedList(Page, Utility.GetPageSize()));
							text = (apiResponse.Data as List<v_ct_PhieuXuat>).Sum((v_ct_PhieuXuat s) => s.TONGTIEN).ToString("N0");
						}
						else if (Utility.KiemTraQuyen("Output", "UserData"))
						{
							iPagedList = (from s in apiResponse.Data as List<v_ct_PhieuXuat>
										  where s.ID_NHANVIEN == Login_Model.iduser
										  orderby s.NGAYLAP descending
										  select s).ToList().ToPagedList(Page, Utility.GetPageSize());
							text = (apiResponse.Data as List<v_ct_PhieuXuat>).Sum((v_ct_PhieuXuat s) => s.TONGTIEN).ToString("N0");
						}
					}
				}
				v_v_ct_PhieuXuat v_v_ct_PhieuXuat2 = new v_v_ct_PhieuXuat();
				v_v_ct_PhieuXuat2.IPagedList = iPagedList;
				v_v_ct_PhieuXuat2.lstdm_ThueSuat = new List<v_dm_ThueSuat>();
				v_v_ct_PhieuXuat2.lstdm_ThueSuat = Utility.GetListData<v_dm_ThueSuat>("Tax", "", "", Utility.LOC_ID).Data as List<v_dm_ThueSuat>;
				v_v_ct_PhieuXuat2.lstdm_Kho = new List<v_dm_Kho>();
				v_v_ct_PhieuXuat2.lstdm_Kho = Utility.GetListData<v_dm_Kho>("Depot", "", "", Utility.LOC_ID).Data as List<v_dm_Kho>;
				v_v_ct_PhieuXuat2.lstdm_LoaiPhieuXuat = new List<v_dm_LoaiPhieuXuat>();
				if (Utility.GetListData<v_dm_LoaiPhieuXuat>("TypeOutput", "", "", Utility.LOC_ID).Data is List<v_dm_LoaiPhieuXuat> source)
				{
					v_v_ct_PhieuXuat2.lstdm_LoaiPhieuXuat = (from e in source
															 where e.ISACTIVE
															 orderby e.TYPE
															 select e).ToList();
				}
				else
				{
					v_v_ct_PhieuXuat2.lstdm_LoaiPhieuXuat = new List<v_dm_LoaiPhieuXuat>();
				}
				v_v_ct_PhieuXuat2.lstdm_LoaiHoaDon = new List<v_dm_LoaiHoaDon>();
				if (Utility.GetListData<v_dm_LoaiHoaDon>("TypeInvoiced", "", "", Utility.LOC_ID).Data is List<v_dm_LoaiHoaDon> source2)
				{
					v_v_ct_PhieuXuat2.lstdm_LoaiHoaDon = source2.Where((v_dm_LoaiHoaDon e) => e.ISACTIVE).ToList();
				}
				else
				{
					v_v_ct_PhieuXuat2.lstdm_LoaiHoaDon = new List<v_dm_LoaiHoaDon>();
				}
				base.ViewBag.ID_KHO_DF = (string.IsNullOrEmpty(ID_DEPOT) ? v_v_ct_PhieuXuat2.lstdm_Kho.FirstOrDefault((v_dm_Kho e) => e.ISDEFAULT).ID : ID_DEPOT);
				base.ViewBag.TotalSum = text;
				base.ViewBag.searchValue = SearchString;
				base.ViewBag.showsearchValue = ShowSearchValue;
				base.ViewBag.fromdate = (FromDate.HasValue ? FromDate.Value.ToString("yyyy-MM-dd") : Utility.CurrentTime.ToString("yyyy-MM-dd"));
				base.ViewBag.todate = (ToDate.HasValue ? ToDate.Value.ToString("yyyy-MM-dd") : Utility.CurrentTime.ToString("yyyy-MM-dd"));
				base.ViewBag.PermissionEdit = Utility.KiemTraQuyen("Output", "Edit");
				base.ViewBag.PermissionDelete = Utility.KiemTraQuyen("Output", "Delete");
				base.ViewBag.PermissionCreate = Utility.KiemTraQuyen("Output", "Create");
				base.ViewBag.PermissionCreateInvoiced = Utility.KiemTraQuyen("Output", "CreateInput");
				return View(v_v_ct_PhieuXuat2);
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
				if (!Utility.KiemTraQuyen("Output", "Create"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				v_v_ct_PhieuXuat v_v_ct_PhieuXuat2 = new v_v_ct_PhieuXuat();
				v_v_ct_PhieuXuat2.lstdm_ThueSuat = new List<v_dm_ThueSuat>();
				v_v_ct_PhieuXuat2.lstdm_Kho = new List<v_dm_Kho>();
				v_v_ct_PhieuXuat2.lstdm_KhachHang = new List<ComboboxFrom>();
				v_v_ct_PhieuXuat2.lstdm_NhaCungCap = new List<ComboboxFrom>();
				v_v_ct_PhieuXuat2.lstdm_NhanVien = new List<ComboboxFrom>();
				List<Product_Detail> value = new List<Product_Detail>();
				base.Session["lstProductInput"] = value;
				return View(v_v_ct_PhieuXuat2);
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
				if (!Utility.KiemTraQuyen("Output", "Create"))
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
				List<v_dm_LoaiPhieuXuat> source = Utility.GetListData<v_dm_LoaiPhieuXuat>("TypeOutput", "", "", Utility.LOC_ID).Data as List<v_dm_LoaiPhieuXuat>;
				v_dm_LoaiPhieuXuat v_dm_LoaiPhieuXuat2 = source.Where((v_dm_LoaiPhieuXuat e) => e.ID == ID_LOAIPHIEU).FirstOrDefault();
				if (v_dm_LoaiPhieuXuat2 == null || string.IsNullOrEmpty(v_dm_LoaiPhieuXuat2.ID))
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
				v_v_ct_PhieuXuat v_v_ct_PhieuXuat2 = new v_v_ct_PhieuXuat();
				apiResponse.Success = true;
				v_v_ct_PhieuXuat2.ID_LOAIPHIEUXUAT = ID_LOAIPHIEU;
				v_v_ct_PhieuXuat2.LOC_ID = Utility.LOC_ID;
				v_v_ct_PhieuXuat2.ID = Guid.NewGuid().ToString();
				v_v_ct_PhieuXuat2.NGAYLAP = Utility.CurrentTime;
				v_v_ct_PhieuXuat2.SOPHIEU = Utility.GetMaxID((ct_PhieuXuat)v_v_ct_PhieuXuat2, Utility.LOC_ID, v_v_ct_PhieuXuat2.NGAYLAP.ToString("yyyy-MM-dd"));
				v_v_ct_PhieuXuat2.MAPHIEU = API.GetMaPhieu("Output", v_v_ct_PhieuXuat2.NGAYLAP, v_v_ct_PhieuXuat2.SOPHIEU);
				v_v_ct_PhieuXuat2.CHUNGTUKEMTHEO = CHUNGTUKEMTHEO;
				v_v_ct_PhieuXuat2.lstdm_NhaCungCap = new List<ComboboxFrom>();
				if (v_dm_LoaiPhieuXuat2.TYPE == 1)
				{
					v_v_ct_PhieuXuat2.lstdm_NhaCungCap = Utility.GetListData<ComboboxFrom>("Provider", "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
					v_v_ct_PhieuXuat2.ID_NHACUNGCAP = ID_KHACHAHANG;
					apiResponse.TYPE = "divNCCAdd";
				}
				v_v_ct_PhieuXuat2.lstdm_KhachHang = new List<ComboboxFrom>();
				if (v_dm_LoaiPhieuXuat2.TYPE == 2)
				{
					apiResponse.TYPE = "divKHACHHANGAdd";
					v_v_ct_PhieuXuat2.ID_KHACHHANG = ID_KHACHAHANG;
					v_v_ct_PhieuXuat2.lstdm_KhachHang = Utility.GetListData<ComboboxFrom>("Customer", "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
				}
				v_v_ct_PhieuXuat2.lstdm_NhanVien = new List<ComboboxFrom>();
				if (v_dm_LoaiPhieuXuat2.TYPE == 3)
				{
					apiResponse.TYPE = "divNHANVIENAdd";
					v_v_ct_PhieuXuat2.lstdm_NhanVien = Utility.GetListData<ComboboxFrom>("Employee", "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
					v_v_ct_PhieuXuat2.ID_NHANVIEN = ID_KHACHAHANG;
				}
				v_v_ct_PhieuXuat2.lstdm_Kho = new List<v_dm_Kho>();
				v_v_ct_PhieuXuat2.lstdm_Kho = Utility.GetListData<v_dm_Kho>("Depot", "", "", Utility.LOC_ID).Data as List<v_dm_Kho>;
				v_v_ct_PhieuXuat2.lstdm_LoaiPhieuXuat = new List<v_dm_LoaiPhieuXuat>();
				v_v_ct_PhieuXuat2.lstdm_LoaiPhieuXuat = Utility.GetListData<v_dm_LoaiPhieuXuat>("TypeOutput", "", "", Utility.LOC_ID).Data as List<v_dm_LoaiPhieuXuat>;
				base.Session["lstProductInput"] = new List<Product_Detail>();
				List<ValueEdit> list = Utility.ConvertobjectTo(v_v_ct_PhieuXuat2);
				apiResponse.ProductCombo = Utility.GetProductInputOutput(new List<Product_Detail>(), "Deposit_Temp");
				list.Add(new ValueEdit
				{
					Key = "tbodyTempItemInput",
					Value = apiResponse.ProductCombo
				});
				list.Add(new ValueEdit
				{
					Key = "lblName",
					Value = v_dm_LoaiPhieuXuat2.NAME.ToUpper()
				});
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

		[HttpPost]
		[ValidateInput(false)]
		[ValidateAntiForgeryToken]
		public ActionResult CreatePopup([Bind(Include = "LOC_ID,ID,ID_LOAIPHIEUXUAT,ID_KHO,MAPHIEU,SOPHIEU,NGAYLAP,ID_KHACHHANG,ID_NHACUNGCAP,ID_NHANVIEN,GHICHU,TONGTHANHTIEN,CHIETKHAU,TONGTIENGIAMGIA,TONGTIENVAT,TONGTIEN,BUTTONTYPE,ADDRESS,TEL,CHUNGTUKEMTHEO")] v_v_ct_PhieuXuat ct_PhieuXuat)
		{
			try
			{
				if (Utility.KiemTra())
				{
					return RedirectToAction("Index", "Admin");
				}
				if (!Utility.KiemTraQuyen("Output", "Create"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				ct_PhieuXuat.lstct_PhieuXuat_ChiTiet = new List<v_ct_PhieuXuat_ChiTiet>();
				List<Product_Detail> list = new List<Product_Detail>();
				IEnumerable<string> enumerable = base.Request.Form.AllKeys.Where((string e) => e.StartsWith("txt"));
				if (enumerable == null || enumerable.Count() == 0)
				{
					base.ModelState.AddModelError("lstct_PhieuXuat_ChiTiet", "Thêm danh sách hàng hóa.");
				}
				else
				{
					v_ct_PhieuXuat_ChiTiet v_ct_PhieuXuat_ChiTiet2 = new v_ct_PhieuXuat_ChiTiet();
					foreach (string item in enumerable)
					{
						string[] array = item.ToString().Split('|');
						string[] values = base.HttpContext.Request.Params.GetValues(item.ToString());
						string value = clsMaHoa.Decrypt(array[1].ToString(), "tmt6364");
						Product_Detail product_Detail = JsonConvert.DeserializeObject<Product_Detail>(value);
						if (v_ct_PhieuXuat_ChiTiet2.ID != product_Detail.ID)
						{
							v_ct_PhieuXuat_ChiTiet2 = new v_ct_PhieuXuat_ChiTiet();
							v_ct_PhieuXuat_ChiTiet2 = JsonConvert.DeserializeObject<v_ct_PhieuXuat_ChiTiet>(value);
							v_ct_PhieuXuat_ChiTiet2.LOC_ID = ct_PhieuXuat.LOC_ID;
							ct_PhieuXuat.lstct_PhieuXuat_ChiTiet.Add(v_ct_PhieuXuat_ChiTiet2);
							list.Add(product_Detail);
						}
						Utility.EditObject(v_ct_PhieuXuat_ChiTiet2, array[0].ToString().Substring(3, array[0].ToString().Length - 3), values[0]);
					}
				}
				ApiResponse apiResponse = new ApiResponse();
				if (ct_PhieuXuat.BUTTONTYPE == "GetPromotion")
				{
					apiResponse = Utility.Create(ct_PhieuXuat.lstct_PhieuXuat_ChiTiet, "Output/" + Utility.LOC_ID);
					list = JsonConvert.DeserializeObject<List<Product_Detail>>(apiResponse.Data.ToString());
					apiResponse.GETPROMOTION = Utility.GetProductInputOutput(list, "Deposit_Temp");
					ApiResponse apiResponse2 = apiResponse;
					int sOPHIEU = (ct_PhieuXuat.SOPHIEU = Utility.GetMaxID((ct_PhieuXuat)ct_PhieuXuat, Utility.LOC_ID, ct_PhieuXuat.NGAYLAP.ToString("yyyy-MM-dd")));
					apiResponse2.SOPHIEU = sOPHIEU;
					ct_PhieuXuat.MAPHIEU = API.GetMaPhieu("Output", ct_PhieuXuat.NGAYLAP, ct_PhieuXuat.SOPHIEU);
					apiResponse.NewID = ct_PhieuXuat.ID;
					apiResponse.MAPHIEU = ct_PhieuXuat.MAPHIEU;
				}
				if (ct_PhieuXuat.BUTTONTYPE == "Save")
				{
					if (base.ModelState.IsValid)
					{
						ct_PhieuXuat.NGAYLAP = ct_PhieuXuat.NGAYLAP.AddHours(Utility.CurrentTime.Hour).AddMinutes(Utility.CurrentTime.Minute);
						ct_PhieuXuat.ID = Guid.NewGuid().ToString();
						ct_PhieuXuat.LOC_ID = Utility.LOC_ID;
						ct_PhieuXuat.ID_NGUOITAO = base.Session["idUser"].ToString();
						ct_PhieuXuat.THOIGIANTHEM = Utility.CurrentTime;
						ct_PhieuXuat.ID_NHANVIEN = base.Session["idUser"].ToString();
						apiResponse = Utility.Create((v_ct_PhieuXuat)ct_PhieuXuat, "Output");
						if (apiResponse.Success)
						{
							ct_PhieuXuat.NGAYLAP = Utility.CurrentTime;
							ApiResponse apiResponse3 = apiResponse;
							int sOPHIEU = (ct_PhieuXuat.SOPHIEU = Utility.GetMaxID((ct_PhieuXuat)ct_PhieuXuat, Utility.LOC_ID, ct_PhieuXuat.NGAYLAP.ToString("yyyy-MM-dd")));
							apiResponse3.SOPHIEU = sOPHIEU;
							ct_PhieuXuat.MAPHIEU = API.GetMaPhieu("Output", ct_PhieuXuat.NGAYLAP, ct_PhieuXuat.SOPHIEU);
							apiResponse.NewID = Guid.NewGuid().ToString();
							apiResponse.MAPHIEU = ct_PhieuXuat.MAPHIEU;
							if (apiResponse.Data != null)
							{
								ct_PhieuXuat = JsonConvert.DeserializeObject<v_v_ct_PhieuXuat>(apiResponse.Data.ToString());
							}
							list = new List<Product_Detail>();
						}
						else
						{
							base.ModelState.AddModelError(string.Empty, apiResponse.Message);
							if (apiResponse.CheckValue)
							{
								ct_PhieuXuat.NGAYLAP = Utility.CurrentTime;
								ApiResponse apiResponse4 = apiResponse;
								int sOPHIEU = (ct_PhieuXuat.SOPHIEU = Utility.GetMaxID((ct_PhieuXuat)ct_PhieuXuat, Utility.LOC_ID, ct_PhieuXuat.NGAYLAP.ToString("yyyy-MM-dd")));
								apiResponse4.SOPHIEU = sOPHIEU;
								ct_PhieuXuat.MAPHIEU = API.GetMaPhieu("Output", ct_PhieuXuat.NGAYLAP, ct_PhieuXuat.SOPHIEU);
								apiResponse.NewID = Guid.NewGuid().ToString();
								apiResponse.MAPHIEU = ct_PhieuXuat.MAPHIEU;
							}
						}
					}
					else
					{
						apiResponse.Success = false;
						apiResponse.Data = Utility.GetModelState(base.ModelState, "Output");
					}
				}
				base.Session["lstProductInput"] = list;
				apiResponse.ID = ct_PhieuXuat.ID;
				List<v_dm_LoaiPhieuXuat> source = Utility.GetListData<v_dm_LoaiPhieuXuat>("TypeOutput", "", "", Utility.LOC_ID).Data as List<v_dm_LoaiPhieuXuat>;
				v_dm_LoaiPhieuXuat v_dm_LoaiPhieuXuat2 = source.Where((v_dm_LoaiPhieuXuat e) => e.ID == ct_PhieuXuat.ID_LOAIPHIEUXUAT).FirstOrDefault();
				if (v_dm_LoaiPhieuXuat2 == null || string.IsNullOrEmpty(v_dm_LoaiPhieuXuat2.ID))
				{
					base.TempData["TitleError"] = "Không tìm thấy loại phiếu xuất";
					apiResponse.Success = false;
					apiResponse.URL = base.Url.Action("Index", "Notfound");
					return new JsonResult
					{
						Data = apiResponse,
						JsonRequestBehavior = JsonRequestBehavior.AllowGet,
						MaxJsonLength = int.MaxValue
					};
				}
				ct_PhieuXuat.lstdm_NhaCungCap = new List<ComboboxFrom>();
				if (v_dm_LoaiPhieuXuat2.TYPE == 1)
				{
					ct_PhieuXuat.lstdm_NhaCungCap = Utility.GetListData<ComboboxFrom>("Provider", "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
					apiResponse.TYPE = "divNCCAdd";
				}
				ct_PhieuXuat.lstdm_KhachHang = new List<ComboboxFrom>();
				if (v_dm_LoaiPhieuXuat2.TYPE == 2)
				{
					apiResponse.TYPE = "divKHACHHANGAdd";
					ct_PhieuXuat.lstdm_KhachHang = Utility.GetListData<ComboboxFrom>("Customer", "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
				}
				ct_PhieuXuat.lstdm_NhanVien = new List<ComboboxFrom>();
				if (v_dm_LoaiPhieuXuat2.TYPE == 3)
				{
					apiResponse.TYPE = "divNHANVIENAdd";
					ct_PhieuXuat.lstdm_NhanVien = Utility.GetListData<ComboboxFrom>("Employee", "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
				}
				ct_PhieuXuat.lstdm_Kho = new List<v_dm_Kho>();
				ct_PhieuXuat.lstdm_Kho = Utility.GetListData<v_dm_Kho>("Depot", "", "", Utility.LOC_ID).Data as List<v_dm_Kho>;
				ct_PhieuXuat.lstdm_LoaiPhieuXuat = new List<v_dm_LoaiPhieuXuat>();
				List<ValueEdit> list2 = Utility.ConvertobjectToView(ct_PhieuXuat, "dd/MM/yy HH:mm");
				apiResponse.ProductCombo = Utility.GetProductInputOutput(list, "Deposit_Temp");
				list2.Add(new ValueEdit
				{
					Key = "tbodyTempItemInput",
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
				if (!Utility.KiemTraQuyen("Output", "Create"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				List<Product_Detail> list = new List<Product_Detail>();
				v_v_ct_PhieuXuat v_v_ct_PhieuXuat2 = new v_v_ct_PhieuXuat();
				if (!string.IsNullOrEmpty(id))
				{
					apiResponse = Utility.GetDetail<v_v_ct_PhieuXuat>(Utility.LOC_ID + "/" + id, "Output");
					if (!apiResponse.Success)
					{
						base.TempData["TitleError"] = apiResponse.Message;
						return RedirectToAction("Index", "Notfound");
					}
					if (apiResponse.Data != null)
					{
						v_v_ct_PhieuXuat2 = apiResponse.Data as v_v_ct_PhieuXuat;
					}
					foreach (v_ct_PhieuXuat_ChiTiet item in v_v_ct_PhieuXuat2.lstct_PhieuXuat_ChiTiet)
					{
						list.Add(Utility.ConvertobjectToProduct_Detail(item, new Product_Detail()));
					}
				}
				v_v_ct_PhieuXuat2.lstdm_Kho = new List<v_dm_Kho>();
				v_v_ct_PhieuXuat2.lstdm_KhachHang = new List<ComboboxFrom>();
				v_v_ct_PhieuXuat2.lstdm_ThueSuat = new List<v_dm_ThueSuat>();
				v_v_ct_PhieuXuat2.lstdm_NhaCungCap = new List<ComboboxFrom>();
				v_v_ct_PhieuXuat2.lstdm_NhanVien = new List<ComboboxFrom>();
				base.Session["lstProductInput"] = list;
				apiResponse.ProductCombo = Utility.GetProductInputOutput(list, "Deposit_Temp");
				base.ViewBag.DatHang = apiResponse.ProductCombo;
				return View(v_v_ct_PhieuXuat2);
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
				if (!Utility.KiemTraQuyen("Output", "Edit"))
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
				v_v_ct_PhieuXuat ct_PhieuXuat2 = new v_v_ct_PhieuXuat();
				if (!string.IsNullOrEmpty(id))
				{
					apiResponse = Utility.GetDetail<v_v_ct_PhieuXuat>(Utility.LOC_ID + "/" + id, "Output");
					if (!apiResponse.Success)
					{
						base.TempData["TitleError"] = apiResponse.Message;
						return RedirectToAction("Index", "Notfound");
					}
					if (apiResponse.Data != null)
					{
						ct_PhieuXuat2 = apiResponse.Data as v_v_ct_PhieuXuat;
					}
				}
				foreach (v_ct_PhieuXuat_ChiTiet item in ct_PhieuXuat2.lstct_PhieuXuat_ChiTiet)
				{
					list.Add(Utility.ConvertobjectToProduct_Detail(item, new Product_Detail()));
				}
				ct_PhieuXuat2.lstdm_NhaCungCap = new List<ComboboxFrom>();
				List<v_dm_LoaiPhieuXuat> list2 = Utility.GetListData<v_dm_LoaiPhieuXuat>("TypeOutput", "", "", Utility.LOC_ID).Data as List<v_dm_LoaiPhieuXuat>;
				v_dm_LoaiPhieuXuat v_dm_LoaiPhieuXuat2 = list2.Where((v_dm_LoaiPhieuXuat e) => e.ID == ct_PhieuXuat2.ID_LOAIPHIEUXUAT).FirstOrDefault();
				if (v_dm_LoaiPhieuXuat2 == null || string.IsNullOrEmpty(v_dm_LoaiPhieuXuat2.ID))
				{
					base.TempData["TitleError"] = "Không tìm thấy loại phiếu xuất";
					apiResponse.Success = false;
					apiResponse.URL = base.Url.Action("Index", "Notfound");
					return new JsonResult
					{
						Data = apiResponse,
						JsonRequestBehavior = JsonRequestBehavior.AllowGet,
						MaxJsonLength = int.MaxValue
					};
				}
				if (v_dm_LoaiPhieuXuat2.TYPE == 1)
				{
					ct_PhieuXuat2.lstdm_NhaCungCap = Utility.GetListData<ComboboxFrom>("Provider", "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
					apiResponse.TYPE = "divNCCEdit";
				}
				ct_PhieuXuat2.lstdm_KhachHang = new List<ComboboxFrom>();
				if (v_dm_LoaiPhieuXuat2.TYPE == 2)
				{
					apiResponse.TYPE = "divKHACHHANGEdit";
					ct_PhieuXuat2.lstdm_KhachHang = Utility.GetListData<ComboboxFrom>("Customer", "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
				}
				ct_PhieuXuat2.lstdm_NhanVien = new List<ComboboxFrom>();
				if (v_dm_LoaiPhieuXuat2.TYPE == 3)
				{
					apiResponse.TYPE = "divNHANVIENEdit";
					ct_PhieuXuat2.lstdm_NhanVien = Utility.GetListData<ComboboxFrom>("Employee", "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
				}
				base.Session["lstProductInput"] = list;
				ct_PhieuXuat2.lstdm_Kho = new List<v_dm_Kho>();
				ct_PhieuXuat2.lstdm_Kho = Utility.GetListData<v_dm_Kho>("Depot", "", "", Utility.LOC_ID).Data as List<v_dm_Kho>;
				ct_PhieuXuat2.lstdm_Kho = ct_PhieuXuat2.lstdm_Kho.Where((v_dm_Kho s) => s.ID == ct_PhieuXuat2.ID_KHO).ToList();
				foreach (v_dm_Kho item2 in ct_PhieuXuat2.lstdm_Kho)
				{
					item2.ISDEFAULT = true;
				}
				ct_PhieuXuat2.lstdm_LoaiPhieuXuat = new List<v_dm_LoaiPhieuXuat>();
				ct_PhieuXuat2.lstdm_LoaiPhieuXuat = list2;
				List<ValueEdit> list3 = Utility.ConvertobjectTo(ct_PhieuXuat2);
				apiResponse.ProductCombo = Utility.GetProductInputOutput(list, "Deposit_Temp");
				list3.Add(new ValueEdit
				{
					Key = "tbodyTempItemInputEdit",
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
		public ActionResult Edit([Bind(Include = "ID_NGUOITAO,THOIGIANTHEM,LOC_ID,ID,ID_LOAIPHIEUXUAT,ID_KHO,MAPHIEU,SOPHIEU,NGAYLAP,ID_KHACHHANG,ID_NHACUNGCAP,ID_NHANVIEN,GHICHU,TONGTHANHTIEN,CHIETKHAU,TONGTIENGIAMGIA,TONGTIENVAT,TONGTIEN,BUTTONTYPE,ADDRESS,TEL,CHUNGTUKEMTHEO")] v_v_ct_PhieuXuat ct_PhieuXuat)
		{
			try
			{
				if (Utility.KiemTra())
				{
					return RedirectToAction("Index", "Admin");
				}
				if (!Utility.KiemTraQuyen("Output", "Edit"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				List<Product_Detail> list = new List<Product_Detail>();
				ApiResponse apiResponse = new ApiResponse();
				ct_PhieuXuat.lstct_PhieuXuat_ChiTiet = new List<v_ct_PhieuXuat_ChiTiet>();
				IEnumerable<string> enumerable = base.Request.Form.AllKeys.Where((string e) => e.StartsWith("txt"));
				if (enumerable == null || enumerable.Count() == 0)
				{
					base.ModelState.AddModelError("lstct_PhieuXuat_ChiTiet", "Thêm danh sách hàng hóa.");
				}
				else
				{
					v_ct_PhieuXuat_ChiTiet v_ct_PhieuXuat_ChiTiet2 = new v_ct_PhieuXuat_ChiTiet();
					foreach (string item in enumerable)
					{
						string[] array = item.ToString().Split('|');
						string[] values = base.HttpContext.Request.Params.GetValues(item.ToString());
						string value = clsMaHoa.Decrypt(array[1].ToString(), "tmt6364");
						Product_Detail product_Detail = JsonConvert.DeserializeObject<Product_Detail>(value);
						if (v_ct_PhieuXuat_ChiTiet2.ID != product_Detail.ID)
						{
							v_ct_PhieuXuat_ChiTiet2 = new v_ct_PhieuXuat_ChiTiet();
							v_ct_PhieuXuat_ChiTiet2 = JsonConvert.DeserializeObject<v_ct_PhieuXuat_ChiTiet>(value);
							v_ct_PhieuXuat_ChiTiet2.LOC_ID = ct_PhieuXuat.LOC_ID;
							ct_PhieuXuat.lstct_PhieuXuat_ChiTiet.Add(v_ct_PhieuXuat_ChiTiet2);
							list.Add(product_Detail);
						}
						Utility.EditObject(v_ct_PhieuXuat_ChiTiet2, array[0].ToString().Substring(3, array[0].ToString().Length - 3), values[0]);
					}
				}
				if (ct_PhieuXuat.BUTTONTYPE == "GetPromotion")
				{
					apiResponse = Utility.Create(ct_PhieuXuat.lstct_PhieuXuat_ChiTiet, "Output/" + Utility.LOC_ID);
					list = JsonConvert.DeserializeObject<List<Product_Detail>>(apiResponse.Data.ToString());
					base.Session["lstProductInput"] = list;
					apiResponse.GETPROMOTION = Utility.GetProductInputOutput(list, "Deposit_Temp");
					apiResponse.SOPHIEU = ct_PhieuXuat.SOPHIEU;
					apiResponse.NewID = ct_PhieuXuat.ID;
					apiResponse.MAPHIEU = ct_PhieuXuat.MAPHIEU;
				}
				if (ct_PhieuXuat.BUTTONTYPE == "Save")
				{
					if (base.ModelState.IsValid)
					{
						apiResponse = Utility.GetDetail<v_ct_PhieuXuat>(Utility.LOC_ID + "/" + ct_PhieuXuat.ID, "Output");
						if (!apiResponse.Success)
						{
							base.TempData["TitleError"] = apiResponse.Message;
							return RedirectToAction("Index", "Notfound");
						}
						v_ct_PhieuXuat v_ct_PhieuXuat2 = null;
						if (apiResponse.Data != null)
						{
							v_ct_PhieuXuat2 = apiResponse.Data as v_ct_PhieuXuat;
						}
						ct_PhieuXuat.LOC_ID = Utility.LOC_ID;
						ct_PhieuXuat.ID_NGUOISUA = base.Session["idUser"].ToString();
						ct_PhieuXuat.THOIGIANSUA = Utility.CurrentTime;
						apiResponse = Utility.Edit(Utility.LOC_ID + "/" + ct_PhieuXuat.ID, (v_ct_PhieuXuat)ct_PhieuXuat, "Output");
						if (apiResponse.Success)
						{
							apiResponse.ID = ct_PhieuXuat.ID;
							if (apiResponse.Data != null)
							{
								ct_PhieuXuat = JsonConvert.DeserializeObject<v_v_ct_PhieuXuat>(apiResponse.Data.ToString());
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
						apiResponse.Data = Utility.GetModelState(base.ModelState, "Output");
					}
				}
				base.Session["lstProductInput"] = list;
				apiResponse.ID = ct_PhieuXuat.ID;
				ct_PhieuXuat.lstdm_NhaCungCap = new List<ComboboxFrom>();
				List<v_dm_LoaiPhieuXuat> list2 = Utility.GetListData<v_dm_LoaiPhieuXuat>("TypeOutput", "", "", Utility.LOC_ID).Data as List<v_dm_LoaiPhieuXuat>;
				v_dm_LoaiPhieuXuat v_dm_LoaiPhieuXuat2 = list2.Where((v_dm_LoaiPhieuXuat e) => e.ID == ct_PhieuXuat.ID_LOAIPHIEUXUAT).FirstOrDefault();
				if (v_dm_LoaiPhieuXuat2 == null || string.IsNullOrEmpty(v_dm_LoaiPhieuXuat2.ID))
				{
					base.TempData["TitleError"] = "Không tìm thấy loại phiếu xuất";
					apiResponse.Success = false;
					apiResponse.URL = base.Url.Action("Index", "Notfound");
					return new JsonResult
					{
						Data = apiResponse,
						JsonRequestBehavior = JsonRequestBehavior.AllowGet,
						MaxJsonLength = int.MaxValue
					};
				}
				if (v_dm_LoaiPhieuXuat2.TYPE == 1)
				{
					ct_PhieuXuat.lstdm_NhaCungCap = Utility.GetListData<ComboboxFrom>("Provider", "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
					apiResponse.TYPE = "divNCCEdit";
				}
				ct_PhieuXuat.lstdm_KhachHang = new List<ComboboxFrom>();
				if (v_dm_LoaiPhieuXuat2.TYPE == 2)
				{
					apiResponse.TYPE = "divKHACHHANGEdit";
					ct_PhieuXuat.lstdm_KhachHang = Utility.GetListData<ComboboxFrom>("Customer", "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
				}
				ct_PhieuXuat.lstdm_NhanVien = new List<ComboboxFrom>();
				if (v_dm_LoaiPhieuXuat2.TYPE == 3)
				{
					apiResponse.TYPE = "divNHANVIENEdit";
					ct_PhieuXuat.lstdm_NhanVien = Utility.GetListData<ComboboxFrom>("Employee", "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
				}
				ct_PhieuXuat.lstdm_Kho = new List<v_dm_Kho>();
				ct_PhieuXuat.lstdm_Kho = Utility.GetListData<v_dm_Kho>("Depot", "", "", Utility.LOC_ID).Data as List<v_dm_Kho>;
				ct_PhieuXuat.lstdm_LoaiPhieuXuat = new List<v_dm_LoaiPhieuXuat>();
				ct_PhieuXuat.lstdm_LoaiPhieuXuat = list2;
				List<ValueEdit> list3 = Utility.ConvertobjectToView(ct_PhieuXuat);
				apiResponse.ProductCombo = Utility.GetProductInputOutput(list, "Deposit_Temp");
				list3.Add(new ValueEdit
				{
					Key = "tbodyTempItemInputEdit",
					Value = apiResponse.ProductCombo
				});
				apiResponse.Detail = list3;
				return View(ct_PhieuXuat);
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
		public ActionResult EditPopup([Bind(Include = "LOC_ID,ID,ID_LOAIPHIEUXUAT,ID_KHO,MAPHIEU,SOPHIEU,NGAYLAP,ID_KHACHHANG,ID_NHACUNGCAP,ID_NHANVIEN,GHICHU,TONGTHANHTIEN,CHIETKHAU,TONGTIENGIAMGIA,TONGTIENVAT,TONGTIEN,BUTTONTYPE,ADDRESS,TEL,CHUNGTUKEMTHEO")] v_v_ct_PhieuXuat ct_PhieuXuat)
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
				if (!Utility.KiemTraQuyen("Output", "Create"))
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
				ct_PhieuXuat.lstct_PhieuXuat_ChiTiet = new List<v_ct_PhieuXuat_ChiTiet>();
				IEnumerable<string> enumerable = base.Request.Form.AllKeys.Where((string e) => e.StartsWith("txt"));
				if (enumerable == null || enumerable.Count() == 0)
				{
					base.ModelState.AddModelError("lstct_PhieuXuat_ChiTiet", "Thêm danh sách hàng hóa.");
				}
				else
				{
					v_ct_PhieuXuat_ChiTiet v_ct_PhieuXuat_ChiTiet2 = new v_ct_PhieuXuat_ChiTiet();
					foreach (string item in enumerable)
					{
						string[] array = item.ToString().Split('|');
						string[] values = base.HttpContext.Request.Params.GetValues(item.ToString());
						string value = clsMaHoa.Decrypt(array[1].ToString(), "tmt6364");
						Product_Detail product_Detail = JsonConvert.DeserializeObject<Product_Detail>(value);
						if (v_ct_PhieuXuat_ChiTiet2.ID != product_Detail.ID)
						{
							v_ct_PhieuXuat_ChiTiet2 = new v_ct_PhieuXuat_ChiTiet();
							v_ct_PhieuXuat_ChiTiet2 = JsonConvert.DeserializeObject<v_ct_PhieuXuat_ChiTiet>(value);
							v_ct_PhieuXuat_ChiTiet2.LOC_ID = ct_PhieuXuat.LOC_ID;
							ct_PhieuXuat.lstct_PhieuXuat_ChiTiet.Add(v_ct_PhieuXuat_ChiTiet2);
							list.Add(product_Detail);
						}
						Utility.EditObject(v_ct_PhieuXuat_ChiTiet2, array[0].ToString().Substring(3, array[0].ToString().Length - 3), values[0]);
					}
				}
				if (ct_PhieuXuat.BUTTONTYPE == "GetPromotion")
				{
					apiResponse = Utility.Create(ct_PhieuXuat.lstct_PhieuXuat_ChiTiet, "Output/" + Utility.LOC_ID);
					list = JsonConvert.DeserializeObject<List<Product_Detail>>(apiResponse.Data.ToString());
					base.Session["lstProductInput"] = list;
					apiResponse.GETPROMOTION = Utility.GetProductInputOutput(list, "Deposit_Temp");
					apiResponse.SOPHIEU = ct_PhieuXuat.SOPHIEU;
					apiResponse.NewID = ct_PhieuXuat.ID;
					apiResponse.MAPHIEU = ct_PhieuXuat.MAPHIEU;
				}
				if (ct_PhieuXuat.BUTTONTYPE == "Save")
				{
					if (base.ModelState.IsValid)
					{
						apiResponse = Utility.GetDetail<v_ct_PhieuXuat>(Utility.LOC_ID + "/" + ct_PhieuXuat.ID, "Output");
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
						v_ct_PhieuXuat v_ct_PhieuXuat2 = null;
						if (apiResponse.Data != null)
						{
							v_ct_PhieuXuat2 = apiResponse.Data as v_ct_PhieuXuat;
						}
						ct_PhieuXuat.LOC_ID = Utility.LOC_ID;
						ct_PhieuXuat.ID_NGUOISUA = base.Session["idUser"].ToString();
						ct_PhieuXuat.THOIGIANSUA = Utility.CurrentTime;
						apiResponse = Utility.Edit(Utility.LOC_ID + "/" + ct_PhieuXuat.ID, (v_ct_PhieuXuat)ct_PhieuXuat, "Output");
						if (apiResponse.Success)
						{
							apiResponse.ID = ct_PhieuXuat.ID;
							if (apiResponse.Data != null)
							{
								ct_PhieuXuat = JsonConvert.DeserializeObject<v_v_ct_PhieuXuat>(apiResponse.Data.ToString());
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
						apiResponse.Data = Utility.GetModelState(base.ModelState, "Output");
					}
				}
				base.Session["lstProductInput"] = list;
				apiResponse.ID = ct_PhieuXuat.ID;
				ct_PhieuXuat.lstdm_NhaCungCap = new List<ComboboxFrom>();
				List<v_dm_LoaiPhieuXuat> list2 = Utility.GetListData<v_dm_LoaiPhieuXuat>("TypeOutput", "", "", Utility.LOC_ID).Data as List<v_dm_LoaiPhieuXuat>;
				v_dm_LoaiPhieuXuat v_dm_LoaiPhieuXuat2 = list2.Where((v_dm_LoaiPhieuXuat e) => e.ID == ct_PhieuXuat.ID_LOAIPHIEUXUAT).FirstOrDefault();
				if (v_dm_LoaiPhieuXuat2 == null || string.IsNullOrEmpty(v_dm_LoaiPhieuXuat2.ID))
				{
					base.TempData["TitleError"] = "Không tìm thấy loại phiếu xuất";
					apiResponse.Success = false;
					apiResponse.URL = base.Url.Action("Index", "Notfound");
					return new JsonResult
					{
						Data = apiResponse,
						JsonRequestBehavior = JsonRequestBehavior.AllowGet,
						MaxJsonLength = int.MaxValue
					};
				}
				if (v_dm_LoaiPhieuXuat2.TYPE == 1)
				{
					ct_PhieuXuat.lstdm_NhaCungCap = Utility.GetListData<ComboboxFrom>("Provider", "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
					apiResponse.TYPE = "divNCCEdit";
				}
				ct_PhieuXuat.lstdm_KhachHang = new List<ComboboxFrom>();
				if (v_dm_LoaiPhieuXuat2.TYPE == 2)
				{
					apiResponse.TYPE = "divKHACHHANGEdit";
					ct_PhieuXuat.lstdm_KhachHang = Utility.GetListData<ComboboxFrom>("Customer", "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
				}
				ct_PhieuXuat.lstdm_NhanVien = new List<ComboboxFrom>();
				if (v_dm_LoaiPhieuXuat2.TYPE == 3)
				{
					apiResponse.TYPE = "divNHANVIENEdit";
					ct_PhieuXuat.lstdm_NhanVien = Utility.GetListData<ComboboxFrom>("Employee", "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
				}
				ct_PhieuXuat.lstdm_Kho = new List<v_dm_Kho>();
				ct_PhieuXuat.lstdm_Kho = Utility.GetListData<v_dm_Kho>("Depot", "", "", Utility.LOC_ID).Data as List<v_dm_Kho>;
				ct_PhieuXuat.lstdm_LoaiPhieuXuat = new List<v_dm_LoaiPhieuXuat>();
				ct_PhieuXuat.lstdm_LoaiPhieuXuat = list2;
				List<ValueEdit> list3 = Utility.ConvertobjectToView(ct_PhieuXuat, "dd/MM/yy HH:mm");
				apiResponse.ProductCombo = Utility.GetProductInputOutput(list, "Deposit_Temp");
				list3.Add(new ValueEdit
				{
					Key = "tbodyTempItemInputEdit",
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
				if (!Utility.KiemTraQuyen("Output", "Delete"))
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
				apiResponse = Utility.Delete<v_ct_PhieuXuat>(Utility.LOC_ID + "/" + id, "Output");
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
				v_ct_PhieuXuat v_ct_PhieuXuat2 = new v_ct_PhieuXuat();
				SP_Parameter sP_Parameter = new SP_Parameter();
				sP_Parameter.ID_PHIEUXUAT = ID;
				apiResponse = Utility.ExecuteStoredProc<v_ct_PhieuXuat>(sP_Parameter, "Sp_Get_DanhSachPhieuXuat");
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
					v_ct_PhieuXuat2 = (apiResponse.Data as List<v_ct_PhieuXuat>).FirstOrDefault();
				}
				SP_Parameter_Report sP_Parameter_Report = new SP_Parameter_Report();
				sP_Parameter_Report.LOC_ID = Utility.LOC_ID;
				sP_Parameter_Report.ID_PHIEUXUAT = ID;
				ReportClass report = new ReportClass();
				apiResponse = Utility.ExecuteStoredProc<DataTable>(sP_Parameter_Report, "Sp_Get_DanhSachPhieuXuat_ChiTiet");
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
					if (dataTable.Columns.Contains("TONGTIENGIAMGIA") && Convert.ToDecimal(row["TONGTIENGIAMGIA"]) < 0m)
					{
						row["TONGTIENVAT"] = Convert.ToDecimal(row["TONGTIENGIAMGIA"]) + Convert.ToDecimal(row["TONGTIENVAT"]);
						row["TONGTIENGIAMGIA"] = 0;
					}
				}
				if (apiResponse.CheckValue)
				{
					dataTable.Rows.Clear();
				}
				v_ct_PhieuXuat2.TONGTIENNO = "";
				v_ct_PhieuXuat2.GHICHU = "";
				if (v_ct_PhieuXuat2 != null && !string.IsNullOrEmpty(v_ct_PhieuXuat2.ID_KHACHHANG))
				{
					SP_Parameter sP_Parameter2 = new SP_Parameter();
					sP_Parameter2.LOC_ID = Utility.LOC_ID;
					sP_Parameter2.ID_KHACHHANG = v_ct_PhieuXuat2.ID_KHACHHANG.ToString();
					sP_Parameter2.ISTHEOTHOIGIAN = false;
					sP_Parameter2.ISPHATSINHCONGNO = false;
					sP_Parameter2.ISPHATSINHCONGNOTRONGKY = false;
					sP_Parameter2.ISCONCONGNO = false;
					apiResponse = Utility.Get_ThongKeCongNoKhachHang<v_ThongKeCongNoKhachHang>(sP_Parameter2);
					if (!apiResponse.Success)
					{
						base.TempData["TitleError"] = apiResponse.Message;
						return RedirectToAction("Index", "Notfound");
					}
					if (apiResponse.Data != null)
					{
						v_ThongKeCongNoKhachHang v_ThongKeCongNoKhachHang2 = (apiResponse.Data as List<v_ThongKeCongNoKhachHang>).FirstOrDefault();
						apiResponse = Utility.GetDetail<v_v_ct_PhieuXuat>(Utility.LOC_ID + "/" + v_ct_PhieuXuat2.ID, "Output");
						if (!apiResponse.Success)
						{
							base.TempData["TitleError"] = apiResponse.Message;
							return RedirectToAction("Index", "Notfound");
						}
						v_v_ct_PhieuXuat v_v_ct_PhieuXuat2 = new v_v_ct_PhieuXuat();
						if (apiResponse.Data != null)
						{
							v_v_ct_PhieuXuat2 = apiResponse.Data as v_v_ct_PhieuXuat;
						}
						double num = v_v_ct_PhieuXuat2.lstct_PhieuXuat_ChiTiet.Sum((v_ct_PhieuXuat_ChiTiet e) => e.TONGCONG);
						if (v_ThongKeCongNoKhachHang2 != null && v_ThongKeCongNoKhachHang2.TONGTIENCONGNOCUOIKY - num > 0.0)
						{
							v_ct_PhieuXuat2.TONGTIENNO = "Nợ cũ: " + (v_ThongKeCongNoKhachHang2.TONGTIENCONGNOCUOIKY - num).ToString("N0");
							v_ct_PhieuXuat2.GHICHU = "Tổng tiền: " + v_ThongKeCongNoKhachHang2.TONGTIENCONGNOCUOIKY.ToString("N0");
						}
					}
				}
				report = Utility.GetFormulaFields(report, v_ct_PhieuXuat2);
				string text = Path.Combine(base.Server.MapPath("~/Images_Upload/Logo/"), "040937143939.png");
				report.DataDefinition.FormulaFields["QRCode1"].Text = "'" + text + "'";
				text = Path.Combine(base.Server.MapPath("~/Images_Upload/Logo/"), "117000052509.png");
				report.DataDefinition.FormulaFields["QRCode2"].Text = "'" + text + "'";
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
				apiResponse.NAME = Utility.GetTitleFrom("Output") + " - " + v_ct_PhieuXuat2.MAPHIEU;
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
