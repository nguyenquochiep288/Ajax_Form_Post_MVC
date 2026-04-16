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

	public class InputController : Controller
	{
		public ActionResult Index(int Page = 1, string ID_DEPOT = "", DateTime? FromDate = null, DateTime? ToDate = null, string SearchString = "", string MAPHIEU = "", string IDCODE = "")
		{
			try
			{
				if (Utility.KiemTra())
				{
					return RedirectToAction("Index", "Admin");
				}
				if (!Utility.KiemTraQuyen("Input", "View"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				string text = "";
				ApiResponse apiResponse = new ApiResponse();
				IPagedList<v_ct_PhieuNhap> iPagedList = new List<v_ct_PhieuNhap>().ToList().ToPagedList(Page, Utility.GetPageSize());
				if (FromDate.HasValue || !string.IsNullOrEmpty(IDCODE) || !string.IsNullOrEmpty(MAPHIEU))
				{
					if (!string.IsNullOrEmpty(IDCODE) || !string.IsNullOrEmpty(MAPHIEU))
					{
						apiResponse = Utility.Get_DanhSachPhieuNhap<v_ct_PhieuNhap>(ID_DEPOT, null, null, MAPHIEU, IDCODE);
					}
					if (FromDate.HasValue)
					{
						apiResponse = Utility.Get_DanhSachPhieuNhap<v_ct_PhieuNhap>(ID_DEPOT, FromDate, ToDate, SearchString);
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
					text = (apiResponse.Data as List<v_ct_PhieuNhap>).Sum((v_ct_PhieuNhap s) => s.TONGTIEN).ToString("N0").Replace(".", ",");
					iPagedList = (apiResponse.Data as List<v_ct_PhieuNhap>).ToPagedList(Page, Utility.GetPageSize());
				}
				v_v_ct_PhieuNhap v_v_ct_PhieuNhap2 = new v_v_ct_PhieuNhap();
				v_v_ct_PhieuNhap2.IPagedList = iPagedList;
				v_v_ct_PhieuNhap2.lstdm_NhaCungCap = new List<ComboboxFrom>();
				v_v_ct_PhieuNhap2.lstdm_KhachHang = new List<ComboboxFrom>();
				v_v_ct_PhieuNhap2.lstdm_Kho = new List<v_dm_Kho>();
				v_v_ct_PhieuNhap2.lstdm_LoaiPhieuNhap = new List<v_dm_LoaiPhieuNhap>();
				if (Utility.GetListData<v_dm_LoaiPhieuNhap>("TypeInput", "", "", Utility.LOC_ID).Data is List<v_dm_LoaiPhieuNhap> source)
				{
					v_v_ct_PhieuNhap2.lstdm_LoaiPhieuNhap = (from e in source
															 where e.ISACTIVE
															 orderby e.TYPE
															 select e).ToList();
				}
				else
				{
					v_v_ct_PhieuNhap2.lstdm_LoaiPhieuNhap = new List<v_dm_LoaiPhieuNhap>();
				}
				v_v_ct_PhieuNhap2.lstdm_NhanVien = new List<ComboboxFrom>();
				v_v_ct_PhieuNhap2.lstdm_NhanVien = Utility.GetListData<ComboboxFrom>("Employee", "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
				v_v_ct_PhieuNhap2.lstdm_Kho = new List<v_dm_Kho>();
				v_v_ct_PhieuNhap2.lstdm_Kho = Utility.GetListData<v_dm_Kho>("Depot", "", "", Utility.LOC_ID).Data as List<v_dm_Kho>;
				base.ViewBag.ID_KHO_DF = (string.IsNullOrEmpty(ID_DEPOT) ? v_v_ct_PhieuNhap2.lstdm_Kho.FirstOrDefault((v_dm_Kho e) => e.ISDEFAULT).ID : ID_DEPOT);
				base.ViewBag.searchValue = SearchString;
				base.ViewBag.TotalSum = text;
				base.ViewBag.PermissionEdit = Utility.KiemTraQuyen("Input", "Edit");
				base.ViewBag.PermissionDelete = Utility.KiemTraQuyen("Input", "Delete");
				base.ViewBag.PermissionCreate = Utility.KiemTraQuyen("Input", "Create");
				base.ViewBag.fromdate = (FromDate.HasValue ? FromDate.Value.ToString("yyyy-MM-dd") : Utility.CurrentTime.AddMonths(-1).ToString("yyyy-MM-dd"));
				base.ViewBag.todate = (ToDate.HasValue ? ToDate.Value.ToString("yyyy-MM-dd") : Utility.CurrentTime.ToString("yyyy-MM-dd"));
				return View(v_v_ct_PhieuNhap2);
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
				if (!Utility.KiemTraQuyen("Delivery", "Delivery_CreateReturn") && !Utility.KiemTraQuyen("Input", "Create"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				v_v_ct_PhieuNhap v_v_ct_PhieuNhap2 = new v_v_ct_PhieuNhap();
				v_v_ct_PhieuNhap2.LOC_ID = Utility.LOC_ID;
				v_v_ct_PhieuNhap2.ID_NGUOITAO = base.Session["idUser"].ToString();
				v_v_ct_PhieuNhap2.THOIGIANTHEM = Utility.CurrentTime;
				v_v_ct_PhieuNhap2.NGAYLAP = Utility.CurrentTime;
				v_v_ct_PhieuNhap2.SOPHIEU = Utility.GetMaxID((ct_PhieuNhap)v_v_ct_PhieuNhap2, Utility.LOC_ID, v_v_ct_PhieuNhap2.NGAYLAP.ToString("yyyy-MM-dd"));
				v_v_ct_PhieuNhap2.MAPHIEU = API.GetMaPhieu("Input", v_v_ct_PhieuNhap2.NGAYLAP, v_v_ct_PhieuNhap2.SOPHIEU);
				v_v_ct_PhieuNhap2.lstdm_KhachHang = new List<ComboboxFrom>();
				v_v_ct_PhieuNhap2.ID = Guid.NewGuid().ToString();
				v_v_ct_PhieuNhap2.lstdm_NhaCungCap = new List<ComboboxFrom>();
				v_v_ct_PhieuNhap2.lstdm_Kho = new List<v_dm_Kho>();
				v_v_ct_PhieuNhap2.lstdm_LoaiPhieuNhap = new List<v_dm_LoaiPhieuNhap>();
				v_v_ct_PhieuNhap2.lstdm_NhanVien = new List<ComboboxFrom>();
				base.ViewBag.myModalAdd = myModalAdd;
				v_v_ct_PhieuNhap2.myModalAdd = myModalAdd;
				base.ViewBag.bolHienThi = !(myModalAdd == "myModalAddInput");
				return View(v_v_ct_PhieuNhap2);
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
		public ActionResult Create([Bind(Include = "LOC_ID,ID,ID_LOAIPHIEUNHAP,ID_KHO,MAPHIEU,SOPHIEU,NGAYLAP,ID_NHACUNGCAP,ID_KHACHHANG,ID_NHANVIEN,GHICHU,TONGTHANHTIEN,CHIETKHAU,TONGTIENGIAMGIA,TONGTIENVAT,TONGTIEN,CHUNGTUKEMTHEO")] v_ct_PhieuNhap ct_PhieuNhap)
		{
			try
			{
				if (Utility.KiemTra())
				{
					return RedirectToAction("Index", "Admin");
				}
				if (!Utility.KiemTraQuyen("Input", "Create"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				if (base.ModelState.IsValid)
				{
					ct_PhieuNhap.LOC_ID = Utility.LOC_ID;
					ct_PhieuNhap.ID_NGUOITAO = base.Session["idUser"].ToString();
					ct_PhieuNhap.THOIGIANTHEM = Utility.CurrentTime;
					ApiResponse apiResponse = Utility.Create((ct_PhieuNhap)ct_PhieuNhap, "Input");
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
				return View(ct_PhieuNhap);
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
				if (!Utility.KiemTraQuyen("Input", "Edit"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				v_v_ct_PhieuNhap v_v_ct_PhieuNhap2 = new v_v_ct_PhieuNhap();
				if (!string.IsNullOrEmpty(id))
				{
					apiResponse = Utility.GetDetail<v_ct_PhieuNhap>(Utility.LOC_ID + "/" + id, "Input");
					if (!apiResponse.Success)
					{
						base.TempData["TitleError"] = apiResponse.Message;
						return RedirectToAction("Index", "Notfound");
					}
					if (apiResponse.Data != null)
					{
						v_v_ct_PhieuNhap2 = apiResponse.Data as v_v_ct_PhieuNhap;
					}
				}
				v_v_ct_PhieuNhap2.lstdm_KhachHang = new List<ComboboxFrom>();
				v_v_ct_PhieuNhap2.lstdm_NhaCungCap = new List<ComboboxFrom>();
				v_v_ct_PhieuNhap2.lstdm_Kho = new List<v_dm_Kho>();
				v_v_ct_PhieuNhap2.lstdm_LoaiPhieuNhap = new List<v_dm_LoaiPhieuNhap>();
				v_v_ct_PhieuNhap2.lstdm_NhanVien = new List<ComboboxFrom>();
				return View(v_v_ct_PhieuNhap2);
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
		public ActionResult Edit([Bind(Include = "LOC_ID,ID,ID_LOAIPHIEUNHAP,ID_KHO,MAPHIEU,SOPHIEU,NGAYLAP,ID_NHACUNGCAP,ID_KHACHHANG,ID_NHANVIEN,GHICHU,TONGTHANHTIEN,CHIETKHAU,TONGTIENGIAMGIA,TONGTIENVAT,TONGTIEN,CHUNGTUKEMTHEO")] v_ct_PhieuNhap ct_PhieuNhap)
		{
			try
			{
				if (Utility.KiemTra())
				{
					return RedirectToAction("Index", "Admin");
				}
				if (!Utility.KiemTraQuyen("Input", "Edit"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				if (base.ModelState.IsValid)
				{
					ct_PhieuNhap.LOC_ID = Utility.LOC_ID;
					ct_PhieuNhap.ID_NGUOISUA = base.Session["idUser"].ToString();
					ct_PhieuNhap.THOIGIANSUA = Utility.CurrentTime;
					ApiResponse apiResponse = Utility.Edit(Utility.LOC_ID + "/" + ct_PhieuNhap.ID, ct_PhieuNhap, "Input");
					if (apiResponse.Success)
					{
						return RedirectToAction("Index");
					}
					base.ModelState.AddModelError(string.Empty, apiResponse.Message);
				}
				return View(ct_PhieuNhap);
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
				if (!Utility.KiemTraQuyen("Input", "Delete"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				ApiResponse apiResponse = Utility.Delete<v_ct_PhieuNhap>(Utility.LOC_ID + "/" + id, "Input");
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

		public ActionResult CreatePopup(string ID, string ID_LOAIPHIEU, string ID_KHACHAHANG = "", string CHUNGTUKEMTHEO = "")
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
				if (!Utility.KiemTraQuyen("Delivery", "Delivery_CreateReturn") && !Utility.KiemTraQuyen("Input", "Create"))
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
				v_v_ct_PhieuNhap ct_PhieuNhap2 = new v_v_ct_PhieuNhap();
				apiResponse.Success = true;
				ct_PhieuNhap2.ID_LOAIPHIEUNHAP = ID_LOAIPHIEU;
				ct_PhieuNhap2.LOC_ID = Utility.LOC_ID;
				ct_PhieuNhap2.ID = Guid.NewGuid().ToString();
				ct_PhieuNhap2.NGAYLAP = Utility.CurrentTime;
				ct_PhieuNhap2.CHUNGTUKEMTHEO = CHUNGTUKEMTHEO;
				ct_PhieuNhap2.lstdm_NhaCungCap = new List<ComboboxFrom>();
				if (v_dm_LoaiPhieuNhap2.TYPE == 1)
				{
					ct_PhieuNhap2.lstdm_NhaCungCap = Utility.GetListData<ComboboxFrom>("Provider", "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
					ct_PhieuNhap2.ID_NHACUNGCAP = ID_KHACHAHANG;
					apiResponse.TYPE = "divNCCAdd";
					foreach (ComboboxFrom item in ct_PhieuNhap2.lstdm_NhaCungCap.Where((ComboboxFrom s) => s.ISDEFAULT))
					{
						item.ISDEFAULT = false;
					}
					ComboboxFrom comboboxFrom = ct_PhieuNhap2.lstdm_NhaCungCap.Where((ComboboxFrom s) => s.ID == ct_PhieuNhap2.ID_NHACUNGCAP).FirstOrDefault();
					if (comboboxFrom != null)
					{
						comboboxFrom.ISDEFAULT = true;
					}
				}
				ct_PhieuNhap2.lstdm_KhachHang = new List<ComboboxFrom>();
				if (v_dm_LoaiPhieuNhap2.TYPE == 2)
				{
					apiResponse.TYPE = "divKHACHHANGAdd";
					ct_PhieuNhap2.ID_KHACHHANG = ID_KHACHAHANG;
					ct_PhieuNhap2.lstdm_KhachHang = Utility.GetListData<ComboboxFrom>("Customer", "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
					foreach (ComboboxFrom item2 in ct_PhieuNhap2.lstdm_KhachHang.Where((ComboboxFrom s) => s.ISDEFAULT))
					{
						item2.ISDEFAULT = false;
					}
					ComboboxFrom comboboxFrom2 = ct_PhieuNhap2.lstdm_KhachHang.Where((ComboboxFrom s) => s.ID == ct_PhieuNhap2.ID_KHACHHANG).FirstOrDefault();
					if (comboboxFrom2 != null)
					{
						comboboxFrom2.ISDEFAULT = true;
					}
				}
				ct_PhieuNhap2.lstdm_NhanVien = new List<ComboboxFrom>();
				if (v_dm_LoaiPhieuNhap2.TYPE == 3)
				{
					apiResponse.TYPE = "divNHANVIENAdd";
					ct_PhieuNhap2.lstdm_NhanVien = Utility.GetListData<ComboboxFrom>("Employee", "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
					ct_PhieuNhap2.ID_NHANVIEN = ID_KHACHAHANG;
					foreach (ComboboxFrom item3 in ct_PhieuNhap2.lstdm_NhanVien.Where((ComboboxFrom s) => s.ISDEFAULT))
					{
						item3.ISDEFAULT = false;
					}
					ComboboxFrom comboboxFrom3 = ct_PhieuNhap2.lstdm_NhanVien.Where((ComboboxFrom s) => s.ID == ct_PhieuNhap2.ID_NHANVIEN).FirstOrDefault();
					if (comboboxFrom3 != null)
					{
						comboboxFrom3.ISDEFAULT = true;
					}
				}
				ct_PhieuNhap2.lstdm_Kho = new List<v_dm_Kho>();
				ct_PhieuNhap2.lstdm_Kho = Utility.GetListData<v_dm_Kho>("Depot", "", "", Utility.LOC_ID).Data as List<v_dm_Kho>;
				ct_PhieuNhap2.lstdm_LoaiPhieuNhap = new List<v_dm_LoaiPhieuNhap>();
				ct_PhieuNhap2.lstdm_LoaiPhieuNhap = Utility.GetListData<v_dm_LoaiPhieuNhap>("TypeInput", "", "", Utility.LOC_ID).Data as List<v_dm_LoaiPhieuNhap>;
				v_v_ct_PhieuXuat ct_PhieuXuat2 = new v_v_ct_PhieuXuat();
				List<Product_Detail> list = new List<Product_Detail>();
				if (!string.IsNullOrEmpty(ID))
				{
					apiResponse = Utility.GetDetail<v_v_ct_PhieuXuat>(Utility.LOC_ID + "/" + ID, "Output");
					if (!apiResponse.Success)
					{
						base.TempData["TitleError"] = apiResponse.Message;
						return RedirectToAction("Index", "Notfound");
					}
					if (apiResponse.Data != null)
					{
						ct_PhieuXuat2 = apiResponse.Data as v_v_ct_PhieuXuat;
						ct_PhieuNhap2.NGAYLAP = ct_PhieuXuat2.NGAYLAP;
					}
					foreach (v_ct_PhieuXuat_ChiTiet item4 in ct_PhieuXuat2.lstct_PhieuXuat_ChiTiet)
					{
						Product_Detail product_Detail = Utility.ConvertobjectToProduct_Detail(item4, new Product_Detail());
						product_Detail.ID = Guid.NewGuid().ToString();
						list.Add(product_Detail);
					}
					ct_PhieuNhap2.lstdm_Kho = ct_PhieuNhap2.lstdm_Kho.Where((v_dm_Kho s) => s.ID == ct_PhieuXuat2.ID_KHO).ToList();
					foreach (v_dm_Kho item5 in ct_PhieuNhap2.lstdm_Kho)
					{
						item5.ISDEFAULT = true;
					}
				}
				ct_PhieuNhap2.SOPHIEU = Utility.GetMaxID((ct_PhieuNhap)ct_PhieuNhap2, Utility.LOC_ID, ct_PhieuNhap2.NGAYLAP.ToString("yyyy-MM-dd"));
				ct_PhieuNhap2.MAPHIEU = API.GetMaPhieu("Input", ct_PhieuNhap2.NGAYLAP, ct_PhieuNhap2.SOPHIEU);
				base.Session["lstProductInput"] = list;
				List<ValueEdit> list2 = Utility.ConvertobjectTo(ct_PhieuNhap2);
				apiResponse.ProductCombo = Utility.GetProductInputOutput(list, "InputOutput", bolTinhLai: true, 0.0, 0.0, 0.0, 0.0, bolSuaSoLuong: true, bolSuaDonGia: true, !string.IsNullOrEmpty(ID));
				list2.Add(new ValueEdit
				{
					Key = "tbodyTempItemInput",
					Value = apiResponse.ProductCombo
				});
				list2.Add(new ValueEdit
				{
					Key = "lblName",
					Value = v_dm_LoaiPhieuNhap2.NAME.ToUpper()
				});
				apiResponse.Detail = list2;
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

		public ActionResult CreatePopupNCC(string ID, string ID_LOAIPHIEU = "", string ID_KHACHAHANG = "", string CHUNGTUKEMTHEO = "")
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
				if (!Utility.KiemTraQuyen("Order_Provider", "CreateInput"))
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
				v_dm_LoaiPhieuNhap v_dm_LoaiPhieuNhap2 = ((!string.IsNullOrEmpty(ID_LOAIPHIEU)) ? source.Where((v_dm_LoaiPhieuNhap e) => e.ID == ID_LOAIPHIEU).FirstOrDefault() : null);
				v_v_ct_PhieuNhap ct_PhieuNhap2 = new v_v_ct_PhieuNhap();
				apiResponse.Success = true;
				ct_PhieuNhap2.ID_LOAIPHIEUNHAP = ID_LOAIPHIEU;
				ct_PhieuNhap2.LOC_ID = Utility.LOC_ID;
				ct_PhieuNhap2.ID = Guid.NewGuid().ToString();
				ct_PhieuNhap2.NGAYLAP = Utility.CurrentTime;
				ct_PhieuNhap2.CHUNGTUKEMTHEO = CHUNGTUKEMTHEO;
				ct_PhieuNhap2.lstdm_Kho = new List<v_dm_Kho>();
				ct_PhieuNhap2.lstdm_Kho = Utility.GetListData<v_dm_Kho>("Depot", "", "", Utility.LOC_ID).Data as List<v_dm_Kho>;
				ct_PhieuNhap2.lstdm_LoaiPhieuNhap = new List<v_dm_LoaiPhieuNhap>();
				ct_PhieuNhap2.lstdm_LoaiPhieuNhap = Utility.GetListData<v_dm_LoaiPhieuNhap>("TypeInput", "", "", Utility.LOC_ID).Data as List<v_dm_LoaiPhieuNhap>;
				v_v_ct_PhieuDatHangNCC ct_PhieuXuat2 = new v_v_ct_PhieuDatHangNCC();
				List<Product_Detail> list = new List<Product_Detail>();
				if (!string.IsNullOrEmpty(ID))
				{
					apiResponse = Utility.GetDetail<v_v_ct_PhieuDatHangNCC>(Utility.LOC_ID + "/" + ID, "Order_Provider");
					if (!apiResponse.Success)
					{
						base.TempData["TitleError"] = apiResponse.Message;
						return RedirectToAction("Index", "Notfound");
					}
					if (apiResponse.Data != null)
					{
						ct_PhieuXuat2 = apiResponse.Data as v_v_ct_PhieuDatHangNCC;
						ct_PhieuNhap2.ID_LOAIPHIEUNHAP = ct_PhieuXuat2.ID_LOAIPHIEUNHAP;
						ct_PhieuNhap2.ID_KHO = ct_PhieuXuat2.ID_KHO;
						string text = (ct_PhieuNhap2.ID_NHACUNGCAP = ct_PhieuXuat2.ID_NHACUNGCAP);
						ID_KHACHAHANG = text;
						ct_PhieuNhap2.ID_NHANVIEN = ct_PhieuXuat2.ID_NHANVIEN;
						ct_PhieuNhap2.CHUNGTUKEMTHEO = ct_PhieuXuat2.MAPHIEU;
						v_dm_LoaiPhieuNhap2 = source.Where((v_dm_LoaiPhieuNhap e) => e.ID == ct_PhieuXuat2.ID_LOAIPHIEUNHAP).FirstOrDefault();
					}
					foreach (v_ct_PhieuDatHangNCC_ChiTiet item in ct_PhieuXuat2.lstct_PhieuNhap_ChiTiet)
					{
						Product_Detail product_Detail = Utility.ConvertobjectToProduct_Detail(item, new Product_Detail());
						product_Detail.ID = Guid.NewGuid().ToString();
						list.Add(product_Detail);
					}
					ct_PhieuNhap2.lstdm_Kho = ct_PhieuNhap2.lstdm_Kho.Where((v_dm_Kho s) => s.ID == ct_PhieuXuat2.ID_KHO).ToList();
					foreach (v_dm_Kho item2 in ct_PhieuNhap2.lstdm_Kho)
					{
						item2.ISDEFAULT = true;
					}
				}
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
				ct_PhieuNhap2.lstdm_NhaCungCap = new List<ComboboxFrom>();
				if (v_dm_LoaiPhieuNhap2.TYPE == 1)
				{
					ct_PhieuNhap2.lstdm_NhaCungCap = Utility.GetListData<ComboboxFrom>("Provider", "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
					ct_PhieuNhap2.ID_NHACUNGCAP = ID_KHACHAHANG;
					apiResponse.TYPE = "divNCCAdd";
					foreach (ComboboxFrom item3 in ct_PhieuNhap2.lstdm_NhaCungCap.Where((ComboboxFrom s) => s.ISDEFAULT))
					{
						item3.ISDEFAULT = false;
					}
					ComboboxFrom comboboxFrom = ct_PhieuNhap2.lstdm_NhaCungCap.Where((ComboboxFrom s) => s.ID == ct_PhieuNhap2.ID_NHACUNGCAP).FirstOrDefault();
					if (comboboxFrom != null)
					{
						comboboxFrom.ISDEFAULT = true;
					}
				}
				ct_PhieuNhap2.lstdm_KhachHang = new List<ComboboxFrom>();
				if (v_dm_LoaiPhieuNhap2.TYPE == 2)
				{
					apiResponse.TYPE = "divKHACHHANGAdd";
					ct_PhieuNhap2.ID_KHACHHANG = ID_KHACHAHANG;
					ct_PhieuNhap2.lstdm_KhachHang = Utility.GetListData<ComboboxFrom>("Customer", "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
					foreach (ComboboxFrom item4 in ct_PhieuNhap2.lstdm_KhachHang.Where((ComboboxFrom s) => s.ISDEFAULT))
					{
						item4.ISDEFAULT = false;
					}
					ComboboxFrom comboboxFrom2 = ct_PhieuNhap2.lstdm_KhachHang.Where((ComboboxFrom s) => s.ID == ct_PhieuNhap2.ID_KHACHHANG).FirstOrDefault();
					if (comboboxFrom2 != null)
					{
						comboboxFrom2.ISDEFAULT = true;
					}
				}
				ct_PhieuNhap2.lstdm_NhanVien = new List<ComboboxFrom>();
				if (v_dm_LoaiPhieuNhap2.TYPE == 3)
				{
					apiResponse.TYPE = "divNHANVIENAdd";
					ct_PhieuNhap2.lstdm_NhanVien = Utility.GetListData<ComboboxFrom>("Employee", "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
					ct_PhieuNhap2.ID_NHANVIEN = ID_KHACHAHANG;
					foreach (ComboboxFrom item5 in ct_PhieuNhap2.lstdm_NhanVien.Where((ComboboxFrom s) => s.ISDEFAULT))
					{
						item5.ISDEFAULT = false;
					}
					ComboboxFrom comboboxFrom3 = ct_PhieuNhap2.lstdm_NhanVien.Where((ComboboxFrom s) => s.ID == ct_PhieuNhap2.ID_NHANVIEN).FirstOrDefault();
					if (comboboxFrom3 != null)
					{
						comboboxFrom3.ISDEFAULT = true;
					}
				}
				ct_PhieuNhap2.SOPHIEU = Utility.GetMaxID((ct_PhieuNhap)ct_PhieuNhap2, Utility.LOC_ID, ct_PhieuNhap2.NGAYLAP.ToString("yyyy-MM-dd"));
				ct_PhieuNhap2.MAPHIEU = API.GetMaPhieu("Input", ct_PhieuNhap2.NGAYLAP, ct_PhieuNhap2.SOPHIEU);
				base.Session["lstProductInput"] = list;
				List<ValueEdit> list2 = Utility.ConvertobjectTo(ct_PhieuNhap2);
				apiResponse.ProductCombo = Utility.GetProductInputOutput(list, "InputOutput", bolTinhLai: true, 0.0, 0.0, 0.0, 0.0, bolSuaSoLuong: true, bolSuaDonGia: true);
				list2.Add(new ValueEdit
				{
					Key = "tbodyTempItemInput",
					Value = apiResponse.ProductCombo
				});
				list2.Add(new ValueEdit
				{
					Key = "lblName",
					Value = v_dm_LoaiPhieuNhap2.NAME.ToUpper()
				});
				apiResponse.Detail = list2;
				apiResponse.NAME = "myModalAdd";
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
		public ActionResult CreatePopup([Bind(Include = "LOC_ID,ID,ID_LOAIPHIEUNHAP,ID_KHO,MAPHIEU,SOPHIEU,NGAYLAP,ID_NHACUNGCAP,ID_KHACHHANG,ID_NHANVIEN,GHICHU,TONGTHANHTIEN,CHIETKHAU,TONGTIENGIAMGIA,TONGTIENVAT,TONGTIEN,CHUNGTUKEMTHEO,myModalAdd")] v_v_ct_PhieuNhap ct_PhieuNhap)
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
				if (!Utility.KiemTraQuyen("Delivery", "Delivery_CreateReturn") && !Utility.KiemTraQuyen("Input", "Create"))
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
					base.ModelState.AddModelError("lstct_PhieuNhap_ChiTiet", "Thêm danh sách hàng hóa.");
				}
				if (base.ModelState.IsValid)
				{
					ct_PhieuNhap.NGAYLAP = ct_PhieuNhap.NGAYLAP.AddHours(Utility.CurrentTime.Hour).AddMinutes(Utility.CurrentTime.Minute);
					ct_PhieuNhap.LOC_ID = Utility.LOC_ID;
					ct_PhieuNhap.ID_NGUOITAO = base.Session["idUser"].ToString();
					ct_PhieuNhap.THOIGIANTHEM = Utility.CurrentTime;
					ct_PhieuNhap.lstct_PhieuNhap_ChiTiet = new List<v_ct_PhieuNhap_ChiTiet>();
					v_ct_PhieuNhap_ChiTiet v_ct_PhieuNhap_ChiTiet2 = new v_ct_PhieuNhap_ChiTiet();
					foreach (string item in enumerable)
					{
						string[] array = item.ToString().Split('|');
						string[] values = base.HttpContext.Request.Params.GetValues(item.ToString());
						string value = clsMaHoa.Decrypt(array[1].ToString(), "tmt6364");
						v_ct_PhieuNhap_ChiTiet v_ct_PhieuNhap_ChiTiet3 = JsonConvert.DeserializeObject<v_ct_PhieuNhap_ChiTiet>(value);
						if (v_ct_PhieuNhap_ChiTiet2.ID != v_ct_PhieuNhap_ChiTiet3.ID)
						{
							v_ct_PhieuNhap_ChiTiet2 = new v_ct_PhieuNhap_ChiTiet();
							v_ct_PhieuNhap_ChiTiet2 = JsonConvert.DeserializeObject<v_ct_PhieuNhap_ChiTiet>(value);
							v_ct_PhieuNhap_ChiTiet2.LOC_ID = ct_PhieuNhap.LOC_ID;
							ct_PhieuNhap.lstct_PhieuNhap_ChiTiet.Add(v_ct_PhieuNhap_ChiTiet2);
						}
						Utility.EditObject(v_ct_PhieuNhap_ChiTiet2, array[0].ToString().Substring(3, array[0].ToString().Length - 3), values[0]);
					}
					apiResponse = Utility.Create((v_ct_PhieuNhap)ct_PhieuNhap, "Input");
					if (apiResponse.Success)
					{
						ct_PhieuNhap.NGAYLAP = Utility.CurrentTime;
						ApiResponse apiResponse2 = apiResponse;
						int sOPHIEU = (ct_PhieuNhap.SOPHIEU = Utility.GetMaxID((ct_PhieuNhap)ct_PhieuNhap, Utility.LOC_ID, ct_PhieuNhap.NGAYLAP.ToString("yyyy-MM-dd")));
						apiResponse2.SOPHIEU = sOPHIEU;
						ct_PhieuNhap.MAPHIEU = API.GetMaPhieu("Input", ct_PhieuNhap.NGAYLAP, ct_PhieuNhap.SOPHIEU);
						apiResponse.NewID = Guid.NewGuid().ToString();
						apiResponse.MAPHIEU = ct_PhieuNhap.MAPHIEU;
						if (apiResponse.Data != null)
						{
							ct_PhieuNhap = JsonConvert.DeserializeObject<v_v_ct_PhieuNhap>(apiResponse.Data.ToString());
						}
					}
					else
					{
						base.ModelState.AddModelError(string.Empty, apiResponse.Message);
						if (apiResponse.CheckValue)
						{
							ct_PhieuNhap.NGAYLAP = Utility.CurrentTime;
							ApiResponse apiResponse3 = apiResponse;
							int sOPHIEU = (ct_PhieuNhap.SOPHIEU = Utility.GetMaxID((ct_PhieuNhap)ct_PhieuNhap, Utility.LOC_ID, ct_PhieuNhap.NGAYLAP.ToString("yyyy-MM-dd")));
							apiResponse3.SOPHIEU = sOPHIEU;
							ct_PhieuNhap.MAPHIEU = API.GetMaPhieu("Input", ct_PhieuNhap.NGAYLAP, ct_PhieuNhap.SOPHIEU);
							apiResponse.NewID = Guid.NewGuid().ToString();
							apiResponse.MAPHIEU = ct_PhieuNhap.MAPHIEU;
						}
					}
				}
				else
				{
					apiResponse.Success = false;
					apiResponse.Data = Utility.GetModelState(base.ModelState, "Input");
				}
				apiResponse.ID = ct_PhieuNhap.ID;
				List<v_dm_LoaiPhieuNhap> source = Utility.GetListData<v_dm_LoaiPhieuNhap>("TypeInput", "", "", Utility.LOC_ID).Data as List<v_dm_LoaiPhieuNhap>;
				v_dm_LoaiPhieuNhap v_dm_LoaiPhieuNhap2 = source.Where((v_dm_LoaiPhieuNhap e) => e.ID == ct_PhieuNhap.ID_LOAIPHIEUNHAP).FirstOrDefault();
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
				ct_PhieuNhap.lstdm_NhaCungCap = new List<ComboboxFrom>();
				if (v_dm_LoaiPhieuNhap2.TYPE == 1)
				{
					ct_PhieuNhap.lstdm_NhaCungCap = Utility.GetListData<ComboboxFrom>("Provider", "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
					apiResponse.TYPE = "divNCCAdd";
				}
				ct_PhieuNhap.lstdm_KhachHang = new List<ComboboxFrom>();
				if (v_dm_LoaiPhieuNhap2.TYPE == 2)
				{
					apiResponse.TYPE = "divKHACHHANGAdd";
					ct_PhieuNhap.lstdm_KhachHang = Utility.GetListData<ComboboxFrom>("Customer", "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
				}
				ct_PhieuNhap.lstdm_NhanVien = new List<ComboboxFrom>();
				if (v_dm_LoaiPhieuNhap2.TYPE == 3)
				{
					apiResponse.TYPE = "divNHANVIENAdd";
					ct_PhieuNhap.lstdm_NhanVien = Utility.GetListData<ComboboxFrom>("Employee", "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
				}
				ct_PhieuNhap.lstdm_Kho = new List<v_dm_Kho>();
				ct_PhieuNhap.lstdm_Kho = Utility.GetListData<v_dm_Kho>("Depot", "", "", Utility.LOC_ID).Data as List<v_dm_Kho>;
				ct_PhieuNhap.lstdm_LoaiPhieuNhap = new List<v_dm_LoaiPhieuNhap>();
				List<ValueEdit> list = Utility.ConvertobjectToView(ct_PhieuNhap);
				apiResponse.ProductCombo = Utility.GetProductInputOutput(new List<Product_Detail>(), "InputOutput");
				list.Add(new ValueEdit
				{
					Key = "tbodyTempItemInput",
					Value = apiResponse.ProductCombo
				});
				apiResponse.Detail = list;
				apiResponse.NAME = ct_PhieuNhap.myModalAdd;
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
				if (!Utility.KiemTraQuyen("Input", "Edit"))
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
				v_v_ct_PhieuNhap ct_PhieuNhap2 = new v_v_ct_PhieuNhap();
				if (!string.IsNullOrEmpty(id))
				{
					apiResponse = Utility.GetDetail<v_v_ct_PhieuNhap>(Utility.LOC_ID + "/" + id, "Input");
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
						ct_PhieuNhap2 = apiResponse.Data as v_v_ct_PhieuNhap;
					}
				}
				ct_PhieuNhap2.lstdm_NhaCungCap = new List<ComboboxFrom>();
				apiResponse.Success = true;
				List<v_dm_LoaiPhieuNhap> list = Utility.GetListData<v_dm_LoaiPhieuNhap>("TypeInput", "", "", Utility.LOC_ID).Data as List<v_dm_LoaiPhieuNhap>;
				v_dm_LoaiPhieuNhap v_dm_LoaiPhieuNhap2 = list.Where((v_dm_LoaiPhieuNhap e) => e.ID == ct_PhieuNhap2.ID_LOAIPHIEUNHAP).FirstOrDefault();
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
					ct_PhieuNhap2.lstdm_NhaCungCap = Utility.GetListData<ComboboxFrom>("Provider", "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
					apiResponse.TYPE = "divNCCEdit";
				}
				ct_PhieuNhap2.lstdm_KhachHang = new List<ComboboxFrom>();
				if (v_dm_LoaiPhieuNhap2.TYPE == 2)
				{
					apiResponse.TYPE = "divKHACHHANGEdit";
					ct_PhieuNhap2.lstdm_KhachHang = Utility.GetListData<ComboboxFrom>("Customer", "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
				}
				ct_PhieuNhap2.lstdm_NhanVien = new List<ComboboxFrom>();
				if (v_dm_LoaiPhieuNhap2.TYPE == 3)
				{
					apiResponse.TYPE = "divNHANVIENEdit";
					ct_PhieuNhap2.lstdm_NhanVien = Utility.GetListData<ComboboxFrom>("Employee", "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
				}
				ct_PhieuNhap2.lstdm_Kho = new List<v_dm_Kho>();
				ct_PhieuNhap2.lstdm_Kho = Utility.GetListData<v_dm_Kho>("Depot", "", "", Utility.LOC_ID).Data as List<v_dm_Kho>;
				ct_PhieuNhap2.lstdm_Kho = ct_PhieuNhap2.lstdm_Kho.Where((v_dm_Kho s) => s.ID == ct_PhieuNhap2.ID_KHO).ToList();
				foreach (v_dm_Kho item in ct_PhieuNhap2.lstdm_Kho)
				{
					item.ISDEFAULT = true;
				}
				ct_PhieuNhap2.lstdm_LoaiPhieuNhap = new List<v_dm_LoaiPhieuNhap>();
				ct_PhieuNhap2.lstdm_LoaiPhieuNhap = list;
				List<Product_Detail> list2 = new List<Product_Detail>();
				foreach (v_ct_PhieuNhap_ChiTiet item2 in ct_PhieuNhap2.lstct_PhieuNhap_ChiTiet)
				{
					list2.Add(Utility.ConvertobjectToProduct_Detail(item2, new Product_Detail()));
				}
				base.Session["lstProductInput"] = list2;
				List<ValueEdit> list3 = Utility.ConvertobjectTo(ct_PhieuNhap2);
				apiResponse.ProductCombo = Utility.GetProductInputOutput(list2, "InputOutput", bolTinhLai: false, ct_PhieuNhap2.TONGTIENGIAMGIA, ct_PhieuNhap2.TONGTHANHTIEN, ct_PhieuNhap2.TONGTIENVAT, ct_PhieuNhap2.TONGTIEN, bolSuaSoLuong: true, bolSuaDonGia: true);
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
		public ActionResult EditPopup([Bind(Include = "ID_NGUOITAO,THOIGIANTHEM,LOC_ID,ID,ID_LOAIPHIEUNHAP,ID_KHO,MAPHIEU,SOPHIEU,NGAYLAP,ID_NHACUNGCAP,ID_KHACHHANG,ID_NHANVIEN,GHICHU,TONGTHANHTIEN,CHIETKHAU,TONGTIENGIAMGIA,TONGTIENVAT,TONGTIEN,CHUNGTUKEMTHEO")] v_v_ct_PhieuNhap ct_PhieuNhap)
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
				if (!Utility.KiemTraQuyen("Input", "Edit"))
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
					base.ModelState.AddModelError("lstct_PhieuNhap_ChiTiet", "Thêm danh sách hàng hóa.");
				}
				if (base.ModelState.IsValid)
				{
					ct_PhieuNhap.LOC_ID = Utility.LOC_ID;
					ct_PhieuNhap.ID_NGUOISUA = base.Session["idUser"].ToString();
					ct_PhieuNhap.THOIGIANSUA = Utility.CurrentTime;
					ct_PhieuNhap.lstct_PhieuNhap_ChiTiet = new List<v_ct_PhieuNhap_ChiTiet>();
					v_ct_PhieuNhap_ChiTiet v_ct_PhieuNhap_ChiTiet2 = new v_ct_PhieuNhap_ChiTiet();
					foreach (string item in enumerable)
					{
						string[] array = item.ToString().Split('|');
						string[] values = base.HttpContext.Request.Params.GetValues(item.ToString());
						string value = clsMaHoa.Decrypt(array[1].ToString(), "tmt6364");
						v_ct_PhieuNhap_ChiTiet v_ct_PhieuNhap_ChiTiet3 = JsonConvert.DeserializeObject<v_ct_PhieuNhap_ChiTiet>(value);
						if (v_ct_PhieuNhap_ChiTiet2.ID != v_ct_PhieuNhap_ChiTiet3.ID)
						{
							v_ct_PhieuNhap_ChiTiet2 = new v_ct_PhieuNhap_ChiTiet();
							v_ct_PhieuNhap_ChiTiet2 = JsonConvert.DeserializeObject<v_ct_PhieuNhap_ChiTiet>(value);
							v_ct_PhieuNhap_ChiTiet2.LOC_ID = ct_PhieuNhap.LOC_ID;
							ct_PhieuNhap.lstct_PhieuNhap_ChiTiet.Add(v_ct_PhieuNhap_ChiTiet2);
						}
						Utility.EditObject(v_ct_PhieuNhap_ChiTiet2, array[0].ToString().Substring(3, array[0].ToString().Length - 3), values[0]);
					}
					apiResponse = Utility.Edit(Utility.LOC_ID + "/" + ct_PhieuNhap.ID, (v_ct_PhieuNhap)ct_PhieuNhap, "Input");
					if (apiResponse.Success)
					{
						apiResponse.ID = ct_PhieuNhap.ID;
						if (apiResponse.Data != null)
						{
							ct_PhieuNhap = JsonConvert.DeserializeObject<v_v_ct_PhieuNhap>(apiResponse.Data.ToString());
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
					apiResponse.Data = Utility.GetModelState(base.ModelState, "Input");
				}
				ct_PhieuNhap.lstdm_NhaCungCap = new List<ComboboxFrom>();
				List<v_dm_LoaiPhieuNhap> list = Utility.GetListData<v_dm_LoaiPhieuNhap>("TypeInput", "", "", Utility.LOC_ID).Data as List<v_dm_LoaiPhieuNhap>;
				v_dm_LoaiPhieuNhap v_dm_LoaiPhieuNhap2 = list.Where((v_dm_LoaiPhieuNhap e) => e.ID == ct_PhieuNhap.ID_LOAIPHIEUNHAP).FirstOrDefault();
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
					ct_PhieuNhap.lstdm_NhaCungCap = Utility.GetListData<ComboboxFrom>("Provider", "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
					apiResponse.TYPE = "divNCCEdit";
				}
				ct_PhieuNhap.lstdm_KhachHang = new List<ComboboxFrom>();
				if (v_dm_LoaiPhieuNhap2.TYPE == 2)
				{
					apiResponse.TYPE = "divKHACHHANGEdit";
					ct_PhieuNhap.lstdm_KhachHang = Utility.GetListData<ComboboxFrom>("Customer", "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
				}
				ct_PhieuNhap.lstdm_NhanVien = new List<ComboboxFrom>();
				if (v_dm_LoaiPhieuNhap2.TYPE == 3)
				{
					apiResponse.TYPE = "divNHANVIENEdit";
					ct_PhieuNhap.lstdm_NhanVien = Utility.GetListData<ComboboxFrom>("Employee", "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
				}
				ct_PhieuNhap.lstdm_Kho = new List<v_dm_Kho>();
				ct_PhieuNhap.lstdm_Kho = Utility.GetListData<v_dm_Kho>("Depot", "", "", Utility.LOC_ID).Data as List<v_dm_Kho>;
				ct_PhieuNhap.lstdm_LoaiPhieuNhap = new List<v_dm_LoaiPhieuNhap>();
				ct_PhieuNhap.lstdm_LoaiPhieuNhap = list;
				ct_PhieuNhap.lstdm_NhanVien = new List<ComboboxFrom>();
				ct_PhieuNhap.lstdm_NhanVien = Utility.GetListData<ComboboxFrom>("Employee", "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
				apiResponse.Detail = Utility.ConvertobjectToView(ct_PhieuNhap);
				List<Product_Detail> list2 = new List<Product_Detail>();
				list2 = Utility.GetlstProductInput();
				List<ValueEdit> list3 = Utility.ConvertobjectToView(ct_PhieuNhap);
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
				if (!Utility.KiemTraQuyen("Input", "Delete"))
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
				apiResponse = Utility.Delete<v_ct_PhieuNhap>(Utility.LOC_ID + "/" + id, "Input");
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
				v_ct_PhieuNhap v_ct_PhieuNhap2 = new v_ct_PhieuNhap();
				SP_Parameter sP_Parameter = new SP_Parameter();
				sP_Parameter.ID_PHIEUNHAP = ID;
				apiResponse = Utility.ExecuteStoredProc<v_ct_PhieuNhap>(sP_Parameter, "Sp_Get_DanhSachPhieuNhap");
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
					v_ct_PhieuNhap2 = (apiResponse.Data as List<v_ct_PhieuNhap>).FirstOrDefault();
				}
				SP_Parameter_Report sP_Parameter_Report = new SP_Parameter_Report();
				sP_Parameter_Report.LOC_ID = Utility.LOC_ID;
				sP_Parameter_Report.ID_PHIEUNHAP = ID;
				ReportClass report = new ReportClass();
				apiResponse = Utility.ExecuteStoredProc<DataTable>(sP_Parameter_Report, "Sp_Get_DanhSachPhieuNhap_Chitiet");
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
				report = Utility.GetFormulaFields(report, v_ct_PhieuNhap2);
				report.SetDataSource(dataTable);
				base.Response.Buffer = false;
				base.Response.ClearContent();
				base.Response.ClearHeaders();
				Stream stream = report.ExportToStream(ExportFormatType.PortableDocFormat);
				Utility.Report = report;
				apiResponse = new ApiResponse();
				apiResponse.Success = true;
				apiResponse.NAME = Utility.GetTitleFrom("Input") + " - " + v_ct_PhieuNhap2.MAPHIEU;
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
