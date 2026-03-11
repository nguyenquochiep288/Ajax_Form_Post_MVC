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

	public class Order_ProviderController : Controller
	{
		public ActionResult Index(int Page = 1, string ID_DEPOT = "", DateTime? FromDate = null, DateTime? ToDate = null, string SearchString = "", string MAPHIEU = "", string IDCODE = "")
		{
			try
			{
				if (Utility.KiemTra())
				{
					return RedirectToAction("Index", "Admin");
				}
				if (!Utility.KiemTraQuyen("Order_Provider", "View"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				ApiResponse apiResponse = new ApiResponse();
				IPagedList<v_ct_PhieuDatHangNCC> iPagedList = new List<v_ct_PhieuDatHangNCC>().ToList().ToPagedList(Page, Utility.GetPageSize());
				if (FromDate.HasValue || !string.IsNullOrEmpty(IDCODE))
				{
					if (!string.IsNullOrEmpty(IDCODE))
					{
						apiResponse = Utility.Get_DanhSachPhieuDatHangNCC<v_ct_PhieuDatHangNCC>(ID_DEPOT, null, null, MAPHIEU, IDCODE);
					}
					if (FromDate.HasValue)
					{
						apiResponse = Utility.Get_DanhSachPhieuDatHangNCC<v_ct_PhieuDatHangNCC>(ID_DEPOT, FromDate, ToDate, SearchString);
					}
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
					if (!apiResponse.Success)
					{
						base.TempData["TitleError"] = apiResponse.Message;
						return RedirectToAction("Index", "Notfound");
					}
					iPagedList = (apiResponse.Data as List<v_ct_PhieuDatHangNCC>).ToPagedList(Page, Utility.GetPageSize());
				}
				v_v_ct_PhieuDatHangNCC v_v_ct_PhieuDatHangNCC2 = new v_v_ct_PhieuDatHangNCC();
				v_v_ct_PhieuDatHangNCC2.IPagedList = iPagedList;
				v_v_ct_PhieuDatHangNCC2.lstdm_NhaCungCap = new List<ComboboxFrom>();
				v_v_ct_PhieuDatHangNCC2.lstdm_Kho = new List<v_dm_Kho>();
				v_v_ct_PhieuDatHangNCC2.lstdm_Kho = Utility.GetListData<v_dm_Kho>("Depot", "", "", Utility.LOC_ID).Data as List<v_dm_Kho>;
				v_v_ct_PhieuDatHangNCC2.lstdm_LoaiPhieuNhap = new List<v_dm_LoaiPhieuNhap>();
				if (Utility.GetListData<v_dm_LoaiPhieuNhap>("TypeInput", "", "", Utility.LOC_ID).Data is List<v_dm_LoaiPhieuNhap> source)
				{
					v_v_ct_PhieuDatHangNCC2.lstdm_LoaiPhieuNhap = (from e in source
																   where e.ISACTIVE && e.TYPE == 1
																   orderby e.TYPE
																   select e).ToList();
				}
				else
				{
					v_v_ct_PhieuDatHangNCC2.lstdm_LoaiPhieuNhap = new List<v_dm_LoaiPhieuNhap>();
				}
				v_v_ct_PhieuDatHangNCC2.lstdm_NhanVien = new List<ComboboxFrom>();
				v_v_ct_PhieuDatHangNCC2.lstdm_NhanVien = Utility.GetListData<ComboboxFrom>("Employee", "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
				base.ViewBag.searchValue = SearchString;
				base.ViewBag.ID_KHO_DF = (string.IsNullOrEmpty(ID_DEPOT) ? v_v_ct_PhieuDatHangNCC2.lstdm_Kho.FirstOrDefault((v_dm_Kho e) => e.ISDEFAULT).ID : ID_DEPOT);
				base.ViewBag.PermissionEdit = Utility.KiemTraQuyen("Order_Provider", "Edit");
				base.ViewBag.PermissionDelete = Utility.KiemTraQuyen("Order_Provider", "Delete");
				base.ViewBag.PermissionCreate = Utility.KiemTraQuyen("Order_Provider", "Create");
				base.ViewBag.PermissionCreateInput = Utility.KiemTraQuyen("Order_Provider", "CreateInput");
				base.ViewBag.fromdate = (FromDate.HasValue ? FromDate.Value.ToString("yyyy-MM-dd") : Utility.CurrentTime.AddMonths(-1).ToString("yyyy-MM-dd"));
				base.ViewBag.todate = (ToDate.HasValue ? ToDate.Value.ToString("yyyy-MM-dd") : Utility.CurrentTime.ToString("yyyy-MM-dd"));
				return View(v_v_ct_PhieuDatHangNCC2);
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
			try
			{
				base.Session["IntWidth"] = type;
				if (Utility.KiemTra())
				{
					return RedirectToAction("Index", "Admin");
				}
				if (!Utility.KiemTraQuyen("Order_Provider", "Create"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				v_v_ct_PhieuDatHangNCC v_v_ct_PhieuDatHangNCC2 = new v_v_ct_PhieuDatHangNCC();
				v_v_ct_PhieuDatHangNCC2.LOC_ID = Utility.LOC_ID;
				v_v_ct_PhieuDatHangNCC2.ID_NGUOITAO = base.Session["idUser"].ToString();
				v_v_ct_PhieuDatHangNCC2.THOIGIANTHEM = Utility.CurrentTime;
				v_v_ct_PhieuDatHangNCC2.NGAYLAP = Utility.CurrentTime;
				v_v_ct_PhieuDatHangNCC2.SOPHIEU = Utility.GetMaxID((ct_PhieuDatHangNCC)v_v_ct_PhieuDatHangNCC2, Utility.LOC_ID, v_v_ct_PhieuDatHangNCC2.NGAYLAP.ToString("yyyy-MM-dd"));
				v_v_ct_PhieuDatHangNCC2.MAPHIEU = API.GetMaPhieu("Order_Provider", v_v_ct_PhieuDatHangNCC2.NGAYLAP, v_v_ct_PhieuDatHangNCC2.SOPHIEU);
				v_v_ct_PhieuDatHangNCC2.ID = Guid.NewGuid().ToString();
				v_v_ct_PhieuDatHangNCC2.lstdm_NhaCungCap = new List<ComboboxFrom>();
				v_v_ct_PhieuDatHangNCC2.lstdm_Kho = new List<v_dm_Kho>();
				v_v_ct_PhieuDatHangNCC2.lstdm_LoaiPhieuNhap = new List<v_dm_LoaiPhieuNhap>();
				v_v_ct_PhieuDatHangNCC2.lstdm_NhanVien = new List<ComboboxFrom>();
				base.ViewBag.myModalAdd = myModalAdd;
				v_v_ct_PhieuDatHangNCC2.myModalAdd = myModalAdd;
				return View(v_v_ct_PhieuDatHangNCC2);
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
		public ActionResult Create([Bind(Include = "LOC_ID,ID,ID_LOAIPHIEUNHAP,ID_KHO,MAPHIEU,SOPHIEU,NGAYLAP,ID_NHACUNGCAP,ID_KHACHHANG,ID_NHANVIEN,GHICHU,TONGTHANHTIEN,CHIETKHAU,TONGTIENGIAMGIA,TONGTIENVAT,TONGTIEN,CHUNGTUKEMTHEO")] v_ct_PhieuDatHangNCC ct_PhieuDatHangNCC)
		{
			try
			{
				if (Utility.KiemTra())
				{
					return RedirectToAction("Index", "Admin");
				}
				if (!Utility.KiemTraQuyen("Order_Provider", "Create"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				if (base.ModelState.IsValid)
				{
					ct_PhieuDatHangNCC.LOC_ID = Utility.LOC_ID;
					ct_PhieuDatHangNCC.ID_NGUOITAO = base.Session["idUser"].ToString();
					ct_PhieuDatHangNCC.THOIGIANTHEM = Utility.CurrentTime;
					ApiResponse apiResponse = Utility.Create((ct_PhieuDatHangNCC)ct_PhieuDatHangNCC, "Order_Provider");
					if (apiResponse.Success)
					{
						return RedirectToAction("Index");
					}
					base.ModelState.AddModelError(string.Empty, apiResponse.Message);
					if (apiResponse.CheckValue)
					{
						base.ViewBag.ID = Guid.NewGuid().ToString();
					}
				}
				return View(ct_PhieuDatHangNCC);
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
			try
			{
				base.Session["IntWidth"] = type;
				if (Utility.KiemTra())
				{
					return RedirectToAction("Index", "Admin");
				}
				if (!Utility.KiemTraQuyen("Order_Provider", "Edit"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				v_v_ct_PhieuDatHangNCC v_v_ct_PhieuDatHangNCC2 = new v_v_ct_PhieuDatHangNCC();
				if (!string.IsNullOrEmpty(id))
				{
					apiResponse = Utility.GetDetail<v_ct_PhieuDatHangNCC>(Utility.LOC_ID + "/" + id, "Order_Provider");
					if (!apiResponse.Success)
					{
						base.TempData["TitleError"] = apiResponse.Message;
						return RedirectToAction("Index", "Notfound");
					}
					if (apiResponse.Data != null)
					{
						v_v_ct_PhieuDatHangNCC2 = apiResponse.Data as v_v_ct_PhieuDatHangNCC;
					}
				}
				v_v_ct_PhieuDatHangNCC2.lstdm_NhaCungCap = new List<ComboboxFrom>();
				v_v_ct_PhieuDatHangNCC2.lstdm_Kho = new List<v_dm_Kho>();
				v_v_ct_PhieuDatHangNCC2.lstdm_LoaiPhieuNhap = new List<v_dm_LoaiPhieuNhap>();
				v_v_ct_PhieuDatHangNCC2.lstdm_NhanVien = new List<ComboboxFrom>();
				return View(v_v_ct_PhieuDatHangNCC2);
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
		public ActionResult Edit([Bind(Include = "LOC_ID,ID,ID_LOAIPHIEUNHAP,ID_KHO,MAPHIEU,SOPHIEU,NGAYLAP,ID_NHACUNGCAP,ID_KHACHHANG,ID_NHANVIEN,GHICHU,TONGTHANHTIEN,CHIETKHAU,TONGTIENGIAMGIA,TONGTIENVAT,TONGTIEN,CHUNGTUKEMTHEO")] v_ct_PhieuDatHangNCC ct_PhieuDatHangNCC)
		{
			try
			{
				if (Utility.KiemTra())
				{
					return RedirectToAction("Index", "Admin");
				}
				if (!Utility.KiemTraQuyen("Order_Provider", "Edit"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				if (base.ModelState.IsValid)
				{
					ct_PhieuDatHangNCC.LOC_ID = Utility.LOC_ID;
					ct_PhieuDatHangNCC.ID_NGUOISUA = base.Session["idUser"].ToString();
					ct_PhieuDatHangNCC.THOIGIANSUA = Utility.CurrentTime;
					ApiResponse apiResponse = Utility.Edit(Utility.LOC_ID + "/" + ct_PhieuDatHangNCC.ID, ct_PhieuDatHangNCC, "Order_Provider");
					if (apiResponse.Success)
					{
						return RedirectToAction("Index");
					}
					base.ModelState.AddModelError(string.Empty, apiResponse.Message);
				}
				return View(ct_PhieuDatHangNCC);
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
		public ActionResult Delete(string id)
		{
			try
			{
				if (Utility.KiemTra())
				{
					return RedirectToAction("Index", "Admin");
				}
				if (!Utility.KiemTraQuyen("Order_Provider", "Delete"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				ApiResponse apiResponse = Utility.Delete<v_ct_PhieuDatHangNCC>(Utility.LOC_ID + "/" + id, "Order_Provider");
				if (apiResponse.Success)
				{
					return RedirectToAction("Index");
				}
				base.ModelState.AddModelError(string.Empty, apiResponse.Message);
				return View();
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
				if (!Utility.KiemTraQuyen("Order_Provider", "Create"))
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
				List<v_dm_LoaiPhieuNhap> source = Utility.GetListData<v_dm_LoaiPhieuNhap>("TypeInput", "", "", Utility.LOC_ID).Data as List<v_dm_LoaiPhieuNhap>;
				v_dm_LoaiPhieuNhap v_dm_LoaiPhieuNhap2 = source.Where((v_dm_LoaiPhieuNhap e) => e.ID == ID_LOAIPHIEU).FirstOrDefault();
				if (v_dm_LoaiPhieuNhap2 == null || string.IsNullOrEmpty(v_dm_LoaiPhieuNhap2.ID))
				{
					base.TempData["TitleError"] = "Không tìm thấy loại phiếu nhập";
					apiResponse.Success = false;
					apiResponse.URL = base.Url.Action("Index", "Notfound");
					return new JsonResult
					{
						Data = apiResponse,
						JsonRequestBehavior = JsonRequestBehavior.AllowGet,
						MaxJsonLength = int.MaxValue
					};
				}
				v_v_ct_PhieuDatHangNCC v_v_ct_PhieuDatHangNCC2 = new v_v_ct_PhieuDatHangNCC();
				apiResponse.Success = true;
				v_v_ct_PhieuDatHangNCC2.ID_LOAIPHIEUNHAP = ID_LOAIPHIEU;
				v_v_ct_PhieuDatHangNCC2.LOC_ID = Utility.LOC_ID;
				v_v_ct_PhieuDatHangNCC2.ID = Guid.NewGuid().ToString();
				v_v_ct_PhieuDatHangNCC2.NGAYLAP = Utility.CurrentTime;
				v_v_ct_PhieuDatHangNCC2.SOPHIEU = Utility.GetMaxID((ct_PhieuDatHangNCC)v_v_ct_PhieuDatHangNCC2, Utility.LOC_ID, v_v_ct_PhieuDatHangNCC2.NGAYLAP.ToString("yyyy-MM-dd"));
				v_v_ct_PhieuDatHangNCC2.MAPHIEU = API.GetMaPhieu("Order_Provider", v_v_ct_PhieuDatHangNCC2.NGAYLAP, v_v_ct_PhieuDatHangNCC2.SOPHIEU);
				v_v_ct_PhieuDatHangNCC2.CHUNGTUKEMTHEO = CHUNGTUKEMTHEO;
				v_v_ct_PhieuDatHangNCC2.lstdm_NhaCungCap = new List<ComboboxFrom>();
				if (v_dm_LoaiPhieuNhap2.TYPE == 1)
				{
					v_v_ct_PhieuDatHangNCC2.lstdm_NhaCungCap = Utility.GetListData<ComboboxFrom>("Provider", "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
					v_v_ct_PhieuDatHangNCC2.ID_NHACUNGCAP = ID_KHACHAHANG;
					apiResponse.TYPE = "divNCCAdd";
				}
				v_v_ct_PhieuDatHangNCC2.lstdm_Kho = new List<v_dm_Kho>();
				v_v_ct_PhieuDatHangNCC2.lstdm_Kho = Utility.GetListData<v_dm_Kho>("Depot", "", "", Utility.LOC_ID).Data as List<v_dm_Kho>;
				v_v_ct_PhieuDatHangNCC2.lstdm_LoaiPhieuNhap = new List<v_dm_LoaiPhieuNhap>();
				v_v_ct_PhieuDatHangNCC2.lstdm_LoaiPhieuNhap = Utility.GetListData<v_dm_LoaiPhieuNhap>("TypeInput", "", "", Utility.LOC_ID).Data as List<v_dm_LoaiPhieuNhap>;
				v_v_ct_PhieuDatHangNCC2.lstdm_NhanVien = new List<ComboboxFrom>();
				v_v_ct_PhieuDatHangNCC2.lstdm_NhanVien = Utility.GetListData<ComboboxFrom>("Employee", "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
				base.Session["lstProductInput"] = new List<Product_Detail>();
				List<ValueEdit> list = Utility.ConvertobjectTo(v_v_ct_PhieuDatHangNCC2);
				apiResponse.ProductCombo = Utility.GetProductInputOutput(new List<Product_Detail>(), "InputOutput");
				list.Add(new ValueEdit
				{
					Key = "tbodyTempItemInput",
					Value = apiResponse.ProductCombo
				});
				list.Add(new ValueEdit
				{
					Key = "lblName",
					Value = v_dm_LoaiPhieuNhap2.NAME.ToUpper()
				});
				apiResponse.Detail = list;
				if (!string.IsNullOrEmpty(ID_KHACHAHANG))
				{
					apiResponse.NAME = "myModalAddInput";
				}
				else
				{
					apiResponse.NAME = "myModalAdd";
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
		[ValidateInput(false)]
		[ValidateAntiForgeryToken]
		public ActionResult CreatePopup([Bind(Include = "LOC_ID,ID,ID_LOAIPHIEUNHAP,ID_KHO,MAPHIEU,SOPHIEU,NGAYLAP,ID_NHACUNGCAP,ID_KHACHHANG,ID_NHANVIEN,GHICHU,TONGTHANHTIEN,CHIETKHAU,TONGTIENGIAMGIA,TONGTIENVAT,TONGTIEN,CHUNGTUKEMTHEO,myModalAdd")] v_v_ct_PhieuDatHangNCC ct_PhieuDatHangNCC)
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
				if (!Utility.KiemTraQuyen("Order_Provider", "Create"))
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
				IEnumerable<string> enumerable = base.Request.Form.AllKeys.Where((string e) => e.StartsWith("txt"));
				if (enumerable == null || enumerable.Count() == 0)
				{
					base.ModelState.AddModelError("lstct_PhieuDatHangNCC_ChiTiet", "Thêm danh sách hàng hóa.");
				}
				if (base.ModelState.IsValid)
				{
					if (ct_PhieuDatHangNCC.MAPHIEU.Contains("PN-"))
					{
						v_v_ct_PhieuNhap v_v_ct_PhieuNhap2 = new v_v_ct_PhieuNhap();
						string value = JsonConvert.SerializeObject(ct_PhieuDatHangNCC);
						v_v_ct_PhieuNhap2 = JsonConvert.DeserializeObject<v_v_ct_PhieuNhap>(value) ?? new v_v_ct_PhieuNhap();
						v_v_ct_PhieuNhap2.NGAYLAP = v_v_ct_PhieuNhap2.NGAYLAP.AddHours(Utility.CurrentTime.Hour).AddMinutes(Utility.CurrentTime.Minute);
						v_v_ct_PhieuNhap2.LOC_ID = Utility.LOC_ID;
						v_v_ct_PhieuNhap2.ID_NGUOITAO = base.Session["idUser"].ToString();
						v_v_ct_PhieuNhap2.THOIGIANTHEM = Utility.CurrentTime;
						v_v_ct_PhieuNhap2.lstct_PhieuNhap_ChiTiet = new List<v_ct_PhieuNhap_ChiTiet>();
						v_ct_PhieuNhap_ChiTiet v_ct_PhieuNhap_ChiTiet2 = new v_ct_PhieuNhap_ChiTiet();
						foreach (string item in enumerable)
						{
							string[] array = item.ToString().Split('|');
							string[] values = base.HttpContext.Request.Params.GetValues(item.ToString());
							string value2 = clsMaHoa.Decrypt(array[1].ToString(), "tmt6364");
							v_ct_PhieuNhap_ChiTiet v_ct_PhieuNhap_ChiTiet3 = JsonConvert.DeserializeObject<v_ct_PhieuNhap_ChiTiet>(value2);
							if (v_ct_PhieuNhap_ChiTiet2.ID != v_ct_PhieuNhap_ChiTiet3.ID)
							{
								v_ct_PhieuNhap_ChiTiet2 = new v_ct_PhieuNhap_ChiTiet();
								v_ct_PhieuNhap_ChiTiet2 = JsonConvert.DeserializeObject<v_ct_PhieuNhap_ChiTiet>(value2);
								v_ct_PhieuNhap_ChiTiet2.LOC_ID = v_v_ct_PhieuNhap2.LOC_ID;
								v_v_ct_PhieuNhap2.lstct_PhieuNhap_ChiTiet.Add(v_ct_PhieuNhap_ChiTiet2);
							}
							Utility.EditObject(v_ct_PhieuNhap_ChiTiet2, array[0].ToString().Substring(3, array[0].ToString().Length - 3), values[0]);
						}
						apiResponse = Utility.Create((v_ct_PhieuNhap)v_v_ct_PhieuNhap2, "Input");
						if (apiResponse.Success)
						{
							if (apiResponse.Data != null)
							{
								ct_PhieuDatHangNCC = JsonConvert.DeserializeObject<v_v_ct_PhieuDatHangNCC>(apiResponse.Data.ToString());
							}
						}
						else
						{
							base.ModelState.AddModelError(string.Empty, apiResponse.Message);
							if (apiResponse.CheckValue)
							{
								v_v_ct_PhieuNhap obj = v_v_ct_PhieuNhap2;
								DateTime nGAYLAP = (ct_PhieuDatHangNCC.NGAYLAP = Utility.CurrentTime);
								obj.NGAYLAP = nGAYLAP;
								ApiResponse apiResponse2 = apiResponse;
								int sOPHIEU = (v_v_ct_PhieuNhap2.SOPHIEU = Utility.GetMaxID((ct_PhieuNhap)v_v_ct_PhieuNhap2, Utility.LOC_ID, v_v_ct_PhieuNhap2.NGAYLAP.ToString("yyyy-MM-dd")));
								apiResponse2.SOPHIEU = sOPHIEU;
								v_v_ct_PhieuNhap obj2 = v_v_ct_PhieuNhap2;
								string mAPHIEU = (ct_PhieuDatHangNCC.MAPHIEU = API.GetMaPhieu("Input", v_v_ct_PhieuNhap2.NGAYLAP, v_v_ct_PhieuNhap2.SOPHIEU));
								obj2.MAPHIEU = mAPHIEU;
								apiResponse.NewID = Guid.NewGuid().ToString();
								apiResponse.MAPHIEU = v_v_ct_PhieuNhap2.MAPHIEU;
							}
						}
					}
					else
					{
						ct_PhieuDatHangNCC.NGAYLAP = ct_PhieuDatHangNCC.NGAYLAP.AddHours(Utility.CurrentTime.Hour).AddMinutes(Utility.CurrentTime.Minute);
						ct_PhieuDatHangNCC.LOC_ID = Utility.LOC_ID;
						ct_PhieuDatHangNCC.ID_NGUOITAO = base.Session["idUser"].ToString();
						ct_PhieuDatHangNCC.THOIGIANTHEM = Utility.CurrentTime;
						ct_PhieuDatHangNCC.lstct_PhieuNhap_ChiTiet = new List<v_ct_PhieuDatHangNCC_ChiTiet>();
						v_ct_PhieuDatHangNCC_ChiTiet v_ct_PhieuDatHangNCC_ChiTiet2 = new v_ct_PhieuDatHangNCC_ChiTiet();
						foreach (string item2 in enumerable)
						{
							string[] array2 = item2.ToString().Split('|');
							string[] values2 = base.HttpContext.Request.Params.GetValues(item2.ToString());
							string value3 = clsMaHoa.Decrypt(array2[1].ToString(), "tmt6364");
							v_ct_PhieuDatHangNCC_ChiTiet v_ct_PhieuDatHangNCC_ChiTiet3 = JsonConvert.DeserializeObject<v_ct_PhieuDatHangNCC_ChiTiet>(value3);
							if (v_ct_PhieuDatHangNCC_ChiTiet2.ID != v_ct_PhieuDatHangNCC_ChiTiet3.ID)
							{
								v_ct_PhieuDatHangNCC_ChiTiet2 = new v_ct_PhieuDatHangNCC_ChiTiet();
								v_ct_PhieuDatHangNCC_ChiTiet2 = JsonConvert.DeserializeObject<v_ct_PhieuDatHangNCC_ChiTiet>(value3);
								v_ct_PhieuDatHangNCC_ChiTiet2.LOC_ID = ct_PhieuDatHangNCC.LOC_ID;
								ct_PhieuDatHangNCC.lstct_PhieuNhap_ChiTiet.Add(v_ct_PhieuDatHangNCC_ChiTiet2);
							}
							Utility.EditObject(v_ct_PhieuDatHangNCC_ChiTiet2, array2[0].ToString().Substring(3, array2[0].ToString().Length - 3), values2[0]);
						}
						apiResponse = Utility.Create((v_ct_PhieuDatHangNCC)ct_PhieuDatHangNCC, "Order_Provider");
						if (apiResponse.Success)
						{
							ct_PhieuDatHangNCC.NGAYLAP = Utility.CurrentTime;
							ApiResponse apiResponse3 = apiResponse;
							int sOPHIEU = (ct_PhieuDatHangNCC.SOPHIEU = Utility.GetMaxID((ct_PhieuDatHangNCC)ct_PhieuDatHangNCC, Utility.LOC_ID, ct_PhieuDatHangNCC.NGAYLAP.ToString("yyyy-MM-dd")));
							apiResponse3.SOPHIEU = sOPHIEU;
							ct_PhieuDatHangNCC.MAPHIEU = API.GetMaPhieu("Order_Provider", ct_PhieuDatHangNCC.NGAYLAP, ct_PhieuDatHangNCC.SOPHIEU);
							apiResponse.NewID = Guid.NewGuid().ToString();
							apiResponse.MAPHIEU = ct_PhieuDatHangNCC.MAPHIEU;
							if (apiResponse.Data != null)
							{
								ct_PhieuDatHangNCC = JsonConvert.DeserializeObject<v_v_ct_PhieuDatHangNCC>(apiResponse.Data.ToString());
							}
						}
						else
						{
							base.ModelState.AddModelError(string.Empty, apiResponse.Message);
							if (apiResponse.CheckValue)
							{
								ct_PhieuDatHangNCC.NGAYLAP = Utility.CurrentTime;
								ApiResponse apiResponse4 = apiResponse;
								int sOPHIEU = (ct_PhieuDatHangNCC.SOPHIEU = Utility.GetMaxID((ct_PhieuDatHangNCC)ct_PhieuDatHangNCC, Utility.LOC_ID, ct_PhieuDatHangNCC.NGAYLAP.ToString("yyyy-MM-dd")));
								apiResponse4.SOPHIEU = sOPHIEU;
								ct_PhieuDatHangNCC.MAPHIEU = API.GetMaPhieu("Order_Provider", ct_PhieuDatHangNCC.NGAYLAP, ct_PhieuDatHangNCC.SOPHIEU);
								apiResponse.NewID = Guid.NewGuid().ToString();
								apiResponse.MAPHIEU = ct_PhieuDatHangNCC.MAPHIEU;
							}
						}
					}
				}
				else
				{
					apiResponse.Success = false;
					apiResponse.Data = Utility.GetModelState(base.ModelState, "Order_Provider");
				}
				apiResponse.ID = ct_PhieuDatHangNCC.ID;
				List<v_dm_LoaiPhieuNhap> source = Utility.GetListData<v_dm_LoaiPhieuNhap>("TypeInput", "", "", Utility.LOC_ID).Data as List<v_dm_LoaiPhieuNhap>;
				v_dm_LoaiPhieuNhap v_dm_LoaiPhieuNhap2 = source.Where((v_dm_LoaiPhieuNhap e) => e.ID == ct_PhieuDatHangNCC.ID_LOAIPHIEUNHAP).FirstOrDefault();
				if (v_dm_LoaiPhieuNhap2 == null || string.IsNullOrEmpty(v_dm_LoaiPhieuNhap2.ID))
				{
					base.TempData["TitleError"] = "Không tìm thấy loại phiếu nhập";
					apiResponse.Success = false;
					apiResponse.URL = base.Url.Action("Index", "Notfound");
					return new JsonResult
					{
						Data = apiResponse,
						JsonRequestBehavior = JsonRequestBehavior.AllowGet,
						MaxJsonLength = int.MaxValue
					};
				}
				ct_PhieuDatHangNCC.lstdm_NhaCungCap = new List<ComboboxFrom>();
				if (v_dm_LoaiPhieuNhap2.TYPE == 1)
				{
					ct_PhieuDatHangNCC.lstdm_NhaCungCap = Utility.GetListData<ComboboxFrom>("Provider", "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
					apiResponse.TYPE = "divNCCAdd";
				}
				ct_PhieuDatHangNCC.lstdm_Kho = new List<v_dm_Kho>();
				ct_PhieuDatHangNCC.lstdm_Kho = Utility.GetListData<v_dm_Kho>("Depot", "", "", Utility.LOC_ID).Data as List<v_dm_Kho>;
				ct_PhieuDatHangNCC.lstdm_LoaiPhieuNhap = new List<v_dm_LoaiPhieuNhap>();
				ct_PhieuDatHangNCC.lstdm_NhanVien = new List<ComboboxFrom>();
				ct_PhieuDatHangNCC.lstdm_NhanVien = Utility.GetListData<ComboboxFrom>("Employee", "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
				List<ValueEdit> list = Utility.ConvertobjectToView(ct_PhieuDatHangNCC);
				apiResponse.ProductCombo = Utility.GetProductInputOutput(new List<Product_Detail>(), "InputOutput");
				list.Add(new ValueEdit
				{
					Key = "tbodyTempItemInput",
					Value = apiResponse.ProductCombo
				});
				apiResponse.Detail = list;
				apiResponse.NAME = ct_PhieuDatHangNCC.myModalAdd;
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
				if (!Utility.KiemTraQuyen("Order_Provider", "Edit"))
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
				v_v_ct_PhieuDatHangNCC ct_PhieuDatHangNCC2 = new v_v_ct_PhieuDatHangNCC();
				if (!string.IsNullOrEmpty(id))
				{
					apiResponse = Utility.GetDetail<v_v_ct_PhieuDatHangNCC>(Utility.LOC_ID + "/" + id, "Order_Provider");
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
						ct_PhieuDatHangNCC2 = apiResponse.Data as v_v_ct_PhieuDatHangNCC;
					}
				}
				if (ct_PhieuDatHangNCC2.ISHOANTAT)
				{
					base.TempData["TitleError"] = "Phiếu " + ct_PhieuDatHangNCC2.MAPHIEU + " đã hoàn thành! Vui lòng kiểm tra lại!";
					apiResponse.Success = false;
					apiResponse.URL = base.Url.Action("Index", "Notfound");
					return new JsonResult
					{
						Data = apiResponse,
						JsonRequestBehavior = JsonRequestBehavior.AllowGet,
						MaxJsonLength = int.MaxValue
					};
				}
				ct_PhieuDatHangNCC2.lstdm_NhaCungCap = new List<ComboboxFrom>();
				apiResponse.Success = true;
				List<v_dm_LoaiPhieuNhap> list = Utility.GetListData<v_dm_LoaiPhieuNhap>("TypeInput", "", "", Utility.LOC_ID).Data as List<v_dm_LoaiPhieuNhap>;
				v_dm_LoaiPhieuNhap v_dm_LoaiPhieuNhap2 = list.Where((v_dm_LoaiPhieuNhap e) => e.ID == ct_PhieuDatHangNCC2.ID_LOAIPHIEUNHAP).FirstOrDefault();
				if (v_dm_LoaiPhieuNhap2 == null || string.IsNullOrEmpty(v_dm_LoaiPhieuNhap2.ID))
				{
					base.TempData["TitleError"] = "Không tìm thấy loại phiếu nhập";
					apiResponse.Success = false;
					apiResponse.URL = base.Url.Action("Index", "Notfound");
					return new JsonResult
					{
						Data = apiResponse,
						JsonRequestBehavior = JsonRequestBehavior.AllowGet,
						MaxJsonLength = int.MaxValue
					};
				}
				if (v_dm_LoaiPhieuNhap2.TYPE == 1)
				{
					ct_PhieuDatHangNCC2.lstdm_NhaCungCap = Utility.GetListData<ComboboxFrom>("Provider", "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
					apiResponse.TYPE = "divNCCEdit";
				}
				ct_PhieuDatHangNCC2.lstdm_Kho = new List<v_dm_Kho>();
				ct_PhieuDatHangNCC2.lstdm_Kho = Utility.GetListData<v_dm_Kho>("Depot", "", "", Utility.LOC_ID).Data as List<v_dm_Kho>;
				ct_PhieuDatHangNCC2.lstdm_Kho = ct_PhieuDatHangNCC2.lstdm_Kho.Where((v_dm_Kho s) => s.ID == ct_PhieuDatHangNCC2.ID_KHO).ToList();
				foreach (v_dm_Kho item in ct_PhieuDatHangNCC2.lstdm_Kho)
				{
					item.ISDEFAULT = true;
				}
				ct_PhieuDatHangNCC2.lstdm_LoaiPhieuNhap = new List<v_dm_LoaiPhieuNhap>();
				ct_PhieuDatHangNCC2.lstdm_LoaiPhieuNhap = list;
				ct_PhieuDatHangNCC2.lstdm_NhanVien = new List<ComboboxFrom>();
				ct_PhieuDatHangNCC2.lstdm_NhanVien = Utility.GetListData<ComboboxFrom>("Employee", "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
				List<Product_Detail> list2 = new List<Product_Detail>();
				foreach (v_ct_PhieuDatHangNCC_ChiTiet item2 in ct_PhieuDatHangNCC2.lstct_PhieuNhap_ChiTiet)
				{
					list2.Add(Utility.ConvertobjectToProduct_Detail(item2, new Product_Detail()));
				}
				base.Session["lstProductInput"] = list2;
				List<ValueEdit> list3 = Utility.ConvertobjectTo(ct_PhieuDatHangNCC2);
				apiResponse.ProductCombo = Utility.GetProductInputOutput(list2, "InputOutput", bolTinhLai: false, ct_PhieuDatHangNCC2.TONGTIENGIAMGIA, ct_PhieuDatHangNCC2.TONGTHANHTIEN, ct_PhieuDatHangNCC2.TONGTIENVAT, ct_PhieuDatHangNCC2.TONGTIEN);
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
		public ActionResult EditPopup([Bind(Include = "LOC_ID,ID,ID_LOAIPHIEUNHAP,ID_KHO,MAPHIEU,SOPHIEU,NGAYLAP,ID_NHACUNGCAP,ID_KHACHHANG,ID_NHANVIEN,GHICHU,TONGTHANHTIEN,CHIETKHAU,TONGTIENGIAMGIA,TONGTIENVAT,TONGTIEN,CHUNGTUKEMTHEO")] v_v_ct_PhieuDatHangNCC ct_PhieuDatHangNCC)
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
				if (!Utility.KiemTraQuyen("Order_Provider", "Edit"))
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
				IEnumerable<string> enumerable = base.Request.Form.AllKeys.Where((string e) => e.StartsWith("txt"));
				if (enumerable == null || enumerable.Count() == 0)
				{
					base.ModelState.AddModelError("lstct_PhieuDatHangNCC_ChiTiet", "Thêm danh sách hàng hóa.");
				}
				if (base.ModelState.IsValid)
				{
					ct_PhieuDatHangNCC.LOC_ID = Utility.LOC_ID;
					ct_PhieuDatHangNCC.ID_NGUOISUA = base.Session["idUser"].ToString();
					ct_PhieuDatHangNCC.THOIGIANSUA = Utility.CurrentTime;
					ct_PhieuDatHangNCC.lstct_PhieuNhap_ChiTiet = new List<v_ct_PhieuDatHangNCC_ChiTiet>();
					v_ct_PhieuDatHangNCC_ChiTiet v_ct_PhieuDatHangNCC_ChiTiet2 = new v_ct_PhieuDatHangNCC_ChiTiet();
					foreach (string item in enumerable)
					{
						string[] array = item.ToString().Split('|');
						string[] values = base.HttpContext.Request.Params.GetValues(item.ToString());
						string value = clsMaHoa.Decrypt(array[1].ToString(), "tmt6364");
						v_ct_PhieuDatHangNCC_ChiTiet v_ct_PhieuDatHangNCC_ChiTiet3 = JsonConvert.DeserializeObject<v_ct_PhieuDatHangNCC_ChiTiet>(value);
						if (v_ct_PhieuDatHangNCC_ChiTiet2.ID != v_ct_PhieuDatHangNCC_ChiTiet3.ID)
						{
							v_ct_PhieuDatHangNCC_ChiTiet2 = new v_ct_PhieuDatHangNCC_ChiTiet();
							v_ct_PhieuDatHangNCC_ChiTiet2 = JsonConvert.DeserializeObject<v_ct_PhieuDatHangNCC_ChiTiet>(value);
							v_ct_PhieuDatHangNCC_ChiTiet2.LOC_ID = ct_PhieuDatHangNCC.LOC_ID;
							ct_PhieuDatHangNCC.lstct_PhieuNhap_ChiTiet.Add(v_ct_PhieuDatHangNCC_ChiTiet2);
						}
						Utility.EditObject(v_ct_PhieuDatHangNCC_ChiTiet2, array[0].ToString().Substring(3, array[0].ToString().Length - 3), values[0]);
					}
					apiResponse = Utility.Edit(Utility.LOC_ID + "/" + ct_PhieuDatHangNCC.ID, (v_ct_PhieuDatHangNCC)ct_PhieuDatHangNCC, "Order_Provider");
					if (apiResponse.Success)
					{
						apiResponse.ID = ct_PhieuDatHangNCC.ID;
						if (apiResponse.Data != null)
						{
							ct_PhieuDatHangNCC = JsonConvert.DeserializeObject<v_v_ct_PhieuDatHangNCC>(apiResponse.Data.ToString());
						}
					}
					else
					{
						base.ModelState.AddModelError(string.Empty, apiResponse.Message);
					}
				}
				else
				{
					apiResponse.Success = false;
					apiResponse.Data = Utility.GetModelState(base.ModelState, "Order_Provider");
				}
				ct_PhieuDatHangNCC.lstdm_NhaCungCap = new List<ComboboxFrom>();
				List<v_dm_LoaiPhieuNhap> list = Utility.GetListData<v_dm_LoaiPhieuNhap>("TypeInput", "", "", Utility.LOC_ID).Data as List<v_dm_LoaiPhieuNhap>;
				v_dm_LoaiPhieuNhap v_dm_LoaiPhieuNhap2 = list.Where((v_dm_LoaiPhieuNhap e) => e.ID == ct_PhieuDatHangNCC.ID_LOAIPHIEUNHAP).FirstOrDefault();
				if (v_dm_LoaiPhieuNhap2 == null || string.IsNullOrEmpty(v_dm_LoaiPhieuNhap2.ID))
				{
					base.TempData["TitleError"] = "Không tìm thấy loại phiếu nhập";
					apiResponse.Success = false;
					apiResponse.URL = base.Url.Action("Index", "Notfound");
					return new JsonResult
					{
						Data = apiResponse,
						JsonRequestBehavior = JsonRequestBehavior.AllowGet,
						MaxJsonLength = int.MaxValue
					};
				}
				if (v_dm_LoaiPhieuNhap2.TYPE == 1)
				{
					ct_PhieuDatHangNCC.lstdm_NhaCungCap = Utility.GetListData<ComboboxFrom>("Provider", "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
					apiResponse.TYPE = "divNCCEdit";
				}
				ct_PhieuDatHangNCC.lstdm_Kho = new List<v_dm_Kho>();
				ct_PhieuDatHangNCC.lstdm_Kho = Utility.GetListData<v_dm_Kho>("Depot", "", "", Utility.LOC_ID).Data as List<v_dm_Kho>;
				ct_PhieuDatHangNCC.lstdm_LoaiPhieuNhap = new List<v_dm_LoaiPhieuNhap>();
				ct_PhieuDatHangNCC.lstdm_LoaiPhieuNhap = list;
				ct_PhieuDatHangNCC.lstdm_NhanVien = new List<ComboboxFrom>();
				ct_PhieuDatHangNCC.lstdm_NhanVien = Utility.GetListData<ComboboxFrom>("Employee", "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
				apiResponse.Detail = Utility.ConvertobjectToView(ct_PhieuDatHangNCC);
				List<Product_Detail> list2 = new List<Product_Detail>();
				list2 = Utility.GetlstProductInput();
				List<ValueEdit> list3 = Utility.ConvertobjectToView(ct_PhieuDatHangNCC);
				apiResponse.ProductCombo = Utility.GetProductInputOutput(list2, "InputOutput");
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
				if (!Utility.KiemTraQuyen("Order_Provider", "Delete"))
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
				apiResponse = Utility.Delete<v_ct_PhieuDatHangNCC>(Utility.LOC_ID + "/" + id, "Order_Provider");
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
				v_ct_PhieuDatHangNCC v_ct_PhieuDatHangNCC2 = new v_ct_PhieuDatHangNCC();
				SP_Parameter sP_Parameter = new SP_Parameter();
				sP_Parameter.ID_PHIEUNHAP = ID;
				apiResponse = Utility.ExecuteStoredProc<v_ct_PhieuDatHangNCC>(sP_Parameter, "Sp_Get_DanhSachPhieuDatHangNCC");
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
					v_ct_PhieuDatHangNCC2 = (apiResponse.Data as List<v_ct_PhieuDatHangNCC>).FirstOrDefault();
				}
				SP_Parameter_Report sP_Parameter_Report = new SP_Parameter_Report();
				sP_Parameter_Report.LOC_ID = Utility.LOC_ID;
				sP_Parameter_Report.ID_PHIEUNHAP = ID;
				ReportClass report = new ReportClass();
				apiResponse = Utility.ExecuteStoredProc<DataTable>(sP_Parameter_Report, "Sp_Get_DanhSachPhieuDatHangNCC_Chitiet");
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
				report = Utility.GetFormulaFields(report, v_ct_PhieuDatHangNCC2);
				report.SetDataSource(dataTable);
				base.Response.Buffer = false;
				base.Response.ClearContent();
				base.Response.ClearHeaders();
				Stream stream = report.ExportToStream(ExportFormatType.PortableDocFormat);
				Utility.Report = report;
				apiResponse = new ApiResponse();
				apiResponse.Success = true;
				apiResponse.NAME = Utility.GetTitleFrom("Order_Provider") + " - " + v_ct_PhieuDatHangNCC2.MAPHIEU;
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

		public ActionResult Completed(string ID = "", string TRANGTHAI = "")
		{
			ApiResponse apiResponse = new ApiResponse();
			try
			{
				if (Utility.KiemTra())
				{
					return RedirectToAction("Index", "Admin");
				}
				if (!Utility.KiemTraQuyen("Order_Provider", "View"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				v_ct_PhieuDatHangNCC v_ct_PhieuDatHangNCC2 = new v_ct_PhieuDatHangNCC();
				if (!string.IsNullOrEmpty(ID))
				{
					apiResponse = Utility.GetDetail<v_ct_PhieuDatHangNCC>(Utility.LOC_ID + "/" + ID, "Order_Provider");
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
						v_ct_PhieuDatHangNCC2 = apiResponse.Data as v_ct_PhieuDatHangNCC;
					}
					v_ct_PhieuDatHangNCC2.ISHOANTAT = TRANGTHAI == "1";
					apiResponse = Utility.Edit<v_ct_PhieuDatHangNCC>(Utility.LOC_ID + "/" + ID + "/" + TRANGTHAI, null, "Order_Provider");
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
	}
}
