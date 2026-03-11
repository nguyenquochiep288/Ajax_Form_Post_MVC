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

	public class PaymentController : Controller
	{
		public ActionResult Index(int Page = 1, DateTime? FromDate = null, DateTime? ToDate = null, string SearchString = "", string MAPHIEU = "", string IDCODE = "")
		{
			try
			{
				if (Utility.KiemTra())
				{
					return RedirectToAction("Index", "Admin");
				}
				if (!Utility.KiemTraQuyen("Payment", "View"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				ApiResponse apiResponse = new ApiResponse();
				string text = "";
				IPagedList<v_ct_PhieuChi> iPagedList = new List<v_ct_PhieuChi>().ToList().ToPagedList(Page, Utility.GetPageSize());
				if ((FromDate.HasValue || !string.IsNullOrEmpty(IDCODE) || !string.IsNullOrEmpty(MAPHIEU)) && (FromDate.HasValue || !string.IsNullOrEmpty(IDCODE) || !string.IsNullOrEmpty(MAPHIEU)))
				{
					if (!string.IsNullOrEmpty(IDCODE) || !string.IsNullOrEmpty(MAPHIEU))
					{
						apiResponse = Utility.Get_DanhSachPhieuChi<v_ct_PhieuChi>("", null, null, MAPHIEU, IDCODE);
					}
					if (FromDate.HasValue)
					{
						apiResponse = Utility.Get_DanhSachPhieuChi<v_ct_PhieuChi>("", FromDate, ToDate, SearchString);
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
					text = (apiResponse.Data as List<v_ct_PhieuChi>).Sum((v_ct_PhieuChi s) => s.SOTIEN).ToString("N0");
					iPagedList = (apiResponse.Data as List<v_ct_PhieuChi>).ToPagedList(Page, Utility.GetPageSize());
				}
				v_v_ct_PhieuChi v_v_ct_PhieuChi2 = new v_v_ct_PhieuChi();
				v_v_ct_PhieuChi2.IPagedList = iPagedList;
				v_v_ct_PhieuChi2.lstdm_NhaCungCap = new List<ComboboxFrom>();
				v_v_ct_PhieuChi2.lstdm_KhachHang = new List<ComboboxFrom>();
				v_v_ct_PhieuChi2.lstdm_LoaiPhieuChi = new List<v_dm_LoaiPhieuChi>();
				v_v_ct_PhieuChi2.lstdm_TaiKhoanNganHang = new List<v_dm_TaiKhoanNganHang>();
				if (Utility.GetListData<v_dm_LoaiPhieuChi>("TypePayment", "", "", Utility.LOC_ID).Data is List<v_dm_LoaiPhieuChi> source)
				{
					v_v_ct_PhieuChi2.lstdm_LoaiPhieuChi = (from e in source
														   where e.ISACTIVE
														   orderby e.TYPE
														   select e).ToList();
				}
				else
				{
					v_v_ct_PhieuChi2.lstdm_LoaiPhieuChi = new List<v_dm_LoaiPhieuChi>();
				}
				v_v_ct_PhieuChi2.lstdm_NhanVien = new List<ComboboxFrom>();
				v_v_ct_PhieuChi2.lstdm_Xe = new List<ComboboxFrom>();
				base.ViewBag.searchValue = SearchString;
				base.ViewBag.TotalSum = text;
				base.ViewBag.PermissionEdit = Utility.KiemTraQuyen("Payment", "Edit");
				base.ViewBag.PermissionDelete = Utility.KiemTraQuyen("Payment", "Delete");
				base.ViewBag.PermissionCreate = Utility.KiemTraQuyen("Payment", "Create");
				base.ViewBag.fromdate = (FromDate.HasValue ? FromDate.Value.ToString("yyyy-MM-dd") : Utility.CurrentTime.AddMonths(-1).ToString("yyyy-MM-dd"));
				base.ViewBag.todate = (ToDate.HasValue ? ToDate.Value.ToString("yyyy-MM-dd") : Utility.CurrentTime.ToString("yyyy-MM-dd"));
				return View(v_v_ct_PhieuChi2);
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
				if (!Utility.KiemTraQuyen("Payment", "Create"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				v_v_ct_PhieuChi v_v_ct_PhieuChi2 = new v_v_ct_PhieuChi();
				v_v_ct_PhieuChi2.LOC_ID = Utility.LOC_ID;
				v_v_ct_PhieuChi2.ID_NGUOITAO = base.Session["idUser"].ToString();
				v_v_ct_PhieuChi2.THOIGIANTHEM = Utility.CurrentTime;
				v_v_ct_PhieuChi2.NGAYLAP = Utility.CurrentTime;
				v_v_ct_PhieuChi2.SOPHIEU = Utility.GetMaxID((ct_PhieuChi)v_v_ct_PhieuChi2, Utility.LOC_ID, v_v_ct_PhieuChi2.NGAYLAP.ToString("yyyy-MM-dd"));
				v_v_ct_PhieuChi2.MAPHIEU = API.GetMaPhieu("Payment", v_v_ct_PhieuChi2.NGAYLAP, v_v_ct_PhieuChi2.SOPHIEU);
				v_v_ct_PhieuChi2.ID = Guid.NewGuid().ToString();
				v_v_ct_PhieuChi2.lstdm_NhaCungCap = new List<ComboboxFrom>();
				v_v_ct_PhieuChi2.lstdm_KhachHang = new List<ComboboxFrom>();
				v_v_ct_PhieuChi2.lstdm_LoaiPhieuChi = new List<v_dm_LoaiPhieuChi>();
				v_v_ct_PhieuChi2.lstdm_NhanVien = new List<ComboboxFrom>();
				v_v_ct_PhieuChi2.lstdm_Xe = new List<ComboboxFrom>();
				v_v_ct_PhieuChi2.lstdm_TaiKhoanNganHang = new List<v_dm_TaiKhoanNganHang>();
				base.ViewBag.myModalAdd = myModalAdd;
				v_v_ct_PhieuChi2.myModalAdd = myModalAdd;
				return View(v_v_ct_PhieuChi2);
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
		public ActionResult Create([Bind(Include = "LOC_ID,ID,NAME_LOAIPHIEUCHI,ID_LOAIPHIEUCHI,NGAYLAP,MAPHIEU,SOPHIEU,NAME_KHACHHANG_NCC_NHANVIEN,ID_KHACHHANG,ID_NHACUNGCAP,ID_NHANVIEN,TENNGUOINHAN,DIACHI,NGUOICHITIEN,SOTIEN,LYDO,CHUNGTUKEMTHEO,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO,ID_TAIKHOANNGANHANG")] v_ct_PhieuChi ct_PhieuChi)
		{
			try
			{
				if (Utility.KiemTra())
				{
					return RedirectToAction("Index", "Admin");
				}
				if (!Utility.KiemTraQuyen("Payment", "Create"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				if (base.ModelState.IsValid)
				{
					ct_PhieuChi.LOC_ID = Utility.LOC_ID;
					ct_PhieuChi.ID_NGUOITAO = base.Session["idUser"].ToString();
					ct_PhieuChi.THOIGIANTHEM = Utility.CurrentTime;
					ApiResponse apiResponse = Utility.Create((ct_PhieuChi)ct_PhieuChi, "Payment");
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
				return View(ct_PhieuChi);
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
				if (!Utility.KiemTraQuyen("Payment", "Edit"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				v_v_ct_PhieuChi v_v_ct_PhieuChi2 = new v_v_ct_PhieuChi();
				if (!string.IsNullOrEmpty(id))
				{
					apiResponse = Utility.GetDetail<v_ct_PhieuChi>(Utility.LOC_ID + "/" + id, "Payment");
					if (!apiResponse.Success)
					{
						base.TempData["TitleError"] = apiResponse.Message;
						return RedirectToAction("Index", "Notfound");
					}
					if (apiResponse.Data != null)
					{
						v_v_ct_PhieuChi2 = apiResponse.Data as v_v_ct_PhieuChi;
					}
				}
				v_v_ct_PhieuChi2.lstdm_KhachHang = new List<ComboboxFrom>();
				v_v_ct_PhieuChi2.lstdm_NhaCungCap = new List<ComboboxFrom>();
				v_v_ct_PhieuChi2.lstdm_LoaiPhieuChi = new List<v_dm_LoaiPhieuChi>();
				v_v_ct_PhieuChi2.lstdm_NhanVien = new List<ComboboxFrom>();
				v_v_ct_PhieuChi2.lstdm_Xe = new List<ComboboxFrom>();
				v_v_ct_PhieuChi2.lstdm_TaiKhoanNganHang = new List<v_dm_TaiKhoanNganHang>();
				return View(v_v_ct_PhieuChi2);
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
		public ActionResult Edit([Bind(Include = "LOC_ID,ID,NAME_LOAIPHIEUCHI,ID_LOAIPHIEUCHI,NGAYLAP,MAPHIEU,SOPHIEU,NAME_KHACHHANG_NCC_NHANVIEN,ID_KHACHHANG,ID_NHACUNGCAP,ID_NHANVIEN,TENNGUOINHAN,DIACHI,NGUOICHITIEN,SOTIEN,LYDO,CHUNGTUKEMTHEO,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO,ID_TAIKHOANNGANHANG")] v_ct_PhieuChi ct_PhieuChi)
		{
			try
			{
				if (Utility.KiemTra())
				{
					return RedirectToAction("Index", "Admin");
				}
				if (!Utility.KiemTraQuyen("Payment", "Edit"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				if (base.ModelState.IsValid)
				{
					ct_PhieuChi.LOC_ID = Utility.LOC_ID;
					ct_PhieuChi.ID_NGUOISUA = base.Session["idUser"].ToString();
					ct_PhieuChi.THOIGIANSUA = Utility.CurrentTime;
					ApiResponse apiResponse = Utility.Edit(Utility.LOC_ID + "/" + ct_PhieuChi.ID, ct_PhieuChi, "Payment");
					if (apiResponse.Success)
					{
						return RedirectToAction("Index");
					}
					base.ModelState.AddModelError(string.Empty, apiResponse.Message);
				}
				return View(ct_PhieuChi);
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
				if (!Utility.KiemTraQuyen("Payment", "Delete"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				ApiResponse apiResponse = Utility.Delete<v_ct_PhieuChi>(Utility.LOC_ID + "/" + id, "Payment");
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
				if (!Utility.KiemTraQuyen("Payment", "Create"))
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
				List<v_dm_LoaiPhieuChi> source = Utility.GetListData<v_dm_LoaiPhieuChi>("TypePayment", "", "", Utility.LOC_ID).Data as List<v_dm_LoaiPhieuChi>;
				v_dm_LoaiPhieuChi v_dm_LoaiPhieuChi2 = source.Where((v_dm_LoaiPhieuChi e) => e.ID == ID_LOAIPHIEU).FirstOrDefault();
				if (v_dm_LoaiPhieuChi2 == null || string.IsNullOrEmpty(v_dm_LoaiPhieuChi2.ID))
				{
					base.TempData["TitleError"] = "Không tìm thấy loại phiếu chi";
					apiResponse.Success = false;
					apiResponse.URL = base.Url.Action("Index", "Notfound");
					return new JsonResult
					{
						Data = apiResponse,
						JsonRequestBehavior = JsonRequestBehavior.AllowGet,
						MaxJsonLength = int.MaxValue
					};
				}
				v_v_ct_PhieuChi ct_PhieuChi2 = new v_v_ct_PhieuChi();
				apiResponse.Success = true;
				ct_PhieuChi2.ID_LOAIPHIEUCHI = ID_LOAIPHIEU;
				ct_PhieuChi2.LOC_ID = Utility.LOC_ID;
				ct_PhieuChi2.ID = Guid.NewGuid().ToString();
				ct_PhieuChi2.NGAYLAP = Utility.CurrentTime;
				ct_PhieuChi2.CHUNGTUKEMTHEO = CHUNGTUKEMTHEO;
				ct_PhieuChi2.lstdm_NhaCungCap = new List<ComboboxFrom>();
				if (v_dm_LoaiPhieuChi2.TYPE == 1)
				{
					ct_PhieuChi2.lstdm_NhaCungCap = Utility.GetListData<ComboboxFrom>("Provider", "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
					ct_PhieuChi2.ID_NHACUNGCAP = ID_KHACHAHANG;
					apiResponse.TYPE = "divNCCAdd";
					foreach (ComboboxFrom item in ct_PhieuChi2.lstdm_NhaCungCap.Where((ComboboxFrom s) => s.ISDEFAULT))
					{
						item.ISDEFAULT = false;
					}
					ComboboxFrom comboboxFrom = ct_PhieuChi2.lstdm_NhaCungCap.Where((ComboboxFrom s) => s.ID == ct_PhieuChi2.ID_NHANVIEN).FirstOrDefault();
					if (comboboxFrom != null)
					{
						comboboxFrom.ISDEFAULT = true;
					}
				}
				ct_PhieuChi2.lstdm_KhachHang = new List<ComboboxFrom>();
				if (v_dm_LoaiPhieuChi2.TYPE == 2)
				{
					apiResponse.TYPE = "divKHACHHANGAdd";
					ct_PhieuChi2.lstdm_KhachHang = Utility.GetListData<ComboboxFrom>("Customer", "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
					ct_PhieuChi2.ID_KHACHHANG = ID_KHACHAHANG;
					foreach (ComboboxFrom item2 in ct_PhieuChi2.lstdm_KhachHang.Where((ComboboxFrom s) => s.ISDEFAULT))
					{
						item2.ISDEFAULT = false;
					}
					ComboboxFrom comboboxFrom2 = ct_PhieuChi2.lstdm_KhachHang.Where((ComboboxFrom s) => s.ID == ct_PhieuChi2.ID_KHACHHANG).FirstOrDefault();
					if (comboboxFrom2 != null)
					{
						comboboxFrom2.ISDEFAULT = true;
					}
				}
				ct_PhieuChi2.lstdm_NhanVien = new List<ComboboxFrom>();
				if (v_dm_LoaiPhieuChi2.TYPE == 3)
				{
					apiResponse.TYPE = "divNHANVIENAdd";
					ct_PhieuChi2.lstdm_NhanVien = Utility.GetListData<ComboboxFrom>("Employee", "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
					ct_PhieuChi2.ID_NHANVIEN = ID_KHACHAHANG;
					foreach (ComboboxFrom item3 in ct_PhieuChi2.lstdm_NhanVien.Where((ComboboxFrom s) => s.ISDEFAULT))
					{
						item3.ISDEFAULT = false;
					}
					ComboboxFrom comboboxFrom3 = ct_PhieuChi2.lstdm_NhanVien.Where((ComboboxFrom s) => s.ID == ct_PhieuChi2.ID_NHANVIEN).FirstOrDefault();
					if (comboboxFrom3 != null)
					{
						comboboxFrom3.ISDEFAULT = true;
					}
				}
				ct_PhieuChi2.lstdm_Xe = new List<ComboboxFrom>();
				if (v_dm_LoaiPhieuChi2.TYPE == 4)
				{
					apiResponse.TYPE = "divXEAdd";
					ct_PhieuChi2.lstdm_Xe = Utility.GetListData<ComboboxFrom>("Car", "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
					ct_PhieuChi2.ID_XE = ID_KHACHAHANG;
					foreach (ComboboxFrom item4 in ct_PhieuChi2.lstdm_Xe.Where((ComboboxFrom s) => s.ISDEFAULT))
					{
						item4.ISDEFAULT = false;
					}
					ComboboxFrom comboboxFrom4 = ct_PhieuChi2.lstdm_Xe.Where((ComboboxFrom s) => s.ID == ct_PhieuChi2.ID_XE).FirstOrDefault();
					if (comboboxFrom4 != null)
					{
						comboboxFrom4.ISDEFAULT = true;
					}
				}
				if (CHUNGTUKEMTHEO.StartsWith("PGH-"))
				{
					string nameController = "Delivery";
					ApiResponse value = GetValue<v_v_ct_PhieuGiaoHang>(apiResponse, nameController, CHUNGTUKEMTHEO);
					if (value.Detail != null)
					{
						ct_PhieuChi2.NGAYLAP = (value.Detail as v_v_ct_PhieuGiaoHang).NGAYLAP;
					}
				}
				if (CHUNGTUKEMTHEO.StartsWith("PX-"))
				{
					string nameController2 = "Output";
					ApiResponse value2 = GetValue<v_v_ct_PhieuXuat>(apiResponse, nameController2, CHUNGTUKEMTHEO);
					if (value2.Detail != null)
					{
						ct_PhieuChi2.SOTIEN = (value2.Detail as v_v_ct_PhieuXuat).TONGTIEN;
						ct_PhieuChi2.NGAYLAP = (value2.Detail as v_v_ct_PhieuXuat).NGAYLAP;
					}
				}
				ct_PhieuChi2.SOPHIEU = Utility.GetMaxID((ct_PhieuChi)ct_PhieuChi2, Utility.LOC_ID, ct_PhieuChi2.NGAYLAP.ToString("yyyy-MM-dd"));
				ct_PhieuChi2.MAPHIEU = API.GetMaPhieu("Payment", ct_PhieuChi2.NGAYLAP, ct_PhieuChi2.SOPHIEU);
				ct_PhieuChi2.lstdm_LoaiPhieuChi = new List<v_dm_LoaiPhieuChi>();
				ct_PhieuChi2.lstdm_LoaiPhieuChi = Utility.GetListData<v_dm_LoaiPhieuChi>("TypePayment", "", "", Utility.LOC_ID).Data as List<v_dm_LoaiPhieuChi>;
				ct_PhieuChi2.lstdm_TaiKhoanNganHang = new List<v_dm_TaiKhoanNganHang>();
				ct_PhieuChi2.lstdm_TaiKhoanNganHang = Utility.GetListData<v_dm_TaiKhoanNganHang>("BankAccount", "", "", Utility.LOC_ID).Data as List<v_dm_TaiKhoanNganHang>;
				List<ValueEdit> list = Utility.ConvertobjectTo(ct_PhieuChi2);
				ValueEdit valueEdit = new ValueEdit();
				valueEdit.Key = "lblName";
				valueEdit.Value = v_dm_LoaiPhieuChi2.NAME.ToUpper();
				list.Add(valueEdit);
				apiResponse.Detail = list;
				if (!string.IsNullOrEmpty(ID_KHACHAHANG) || !string.IsNullOrEmpty(CHUNGTUKEMTHEO))
				{
					apiResponse.NAME = "myModalAddPayment";
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

		[HttpPost]
		[ValidateInput(false)]
		[ValidateAntiForgeryToken]
		public ActionResult CreatePopup([Bind(Include = "LOC_ID,ID,NAME_LOAIPHIEUCHI,ID_LOAIPHIEUCHI,NGAYLAP,MAPHIEU,SOPHIEU,NAME_KHACHHANG_NCC_NHANVIEN,ID_KHACHHANG,ID_NHACUNGCAP,ID_NHANVIEN,TENNGUOINHAN,DIACHI,NGUOICHITIEN,SOTIEN,LYDO,CHUNGTUKEMTHEO,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO,ID_TAIKHOANNGANHANG")] v_v_ct_PhieuChi ct_PhieuChi)
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
				if (!Utility.KiemTraQuyen("Payment", "Create"))
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
				if (base.ModelState.IsValid)
				{
					ct_PhieuChi.NGAYLAP = ct_PhieuChi.NGAYLAP.AddHours(Utility.CurrentTime.Hour).AddMinutes(Utility.CurrentTime.Minute);
					ct_PhieuChi.LOC_ID = Utility.LOC_ID;
					ct_PhieuChi.ID_NGUOITAO = base.Session["idUser"].ToString();
					ct_PhieuChi.THOIGIANTHEM = Utility.CurrentTime;
					apiResponse = Utility.Create((v_ct_PhieuChi)ct_PhieuChi, "Payment");
					if (apiResponse.Success)
					{
						ct_PhieuChi.NGAYLAP = Utility.CurrentTime;
						ApiResponse apiResponse2 = apiResponse;
						int sOPHIEU = (ct_PhieuChi.SOPHIEU = Utility.GetMaxID((ct_PhieuChi)ct_PhieuChi, Utility.LOC_ID, ct_PhieuChi.NGAYLAP.ToString("yyyy-MM-dd")));
						apiResponse2.SOPHIEU = sOPHIEU;
						ct_PhieuChi.MAPHIEU = API.GetMaPhieu("Payment", ct_PhieuChi.NGAYLAP, ct_PhieuChi.SOPHIEU);
						apiResponse.NewID = Guid.NewGuid().ToString();
						apiResponse.MAPHIEU = ct_PhieuChi.MAPHIEU;
						if (apiResponse.Data != null)
						{
							ct_PhieuChi = JsonConvert.DeserializeObject<v_v_ct_PhieuChi>(apiResponse.Data.ToString());
						}
					}
					else
					{
						base.ModelState.AddModelError(string.Empty, apiResponse.Message);
						if (apiResponse.CheckValue)
						{
							ct_PhieuChi.NGAYLAP = Utility.CurrentTime;
							ApiResponse apiResponse3 = apiResponse;
							int sOPHIEU = (ct_PhieuChi.SOPHIEU = Utility.GetMaxID((ct_PhieuChi)ct_PhieuChi, Utility.LOC_ID, ct_PhieuChi.NGAYLAP.ToString("yyyy-MM-dd")));
							apiResponse3.SOPHIEU = sOPHIEU;
							ct_PhieuChi.MAPHIEU = API.GetMaPhieu("Payment", ct_PhieuChi.NGAYLAP, ct_PhieuChi.SOPHIEU);
							apiResponse.NewID = Guid.NewGuid().ToString();
							apiResponse.MAPHIEU = ct_PhieuChi.MAPHIEU;
						}
					}
				}
				else
				{
					apiResponse.Success = false;
					apiResponse.Data = Utility.GetModelState(base.ModelState, "Payment");
				}
				apiResponse.ID = ct_PhieuChi.ID;
				List<v_dm_LoaiPhieuChi> source = Utility.GetListData<v_dm_LoaiPhieuChi>("TypePayment", "", "", Utility.LOC_ID).Data as List<v_dm_LoaiPhieuChi>;
				v_dm_LoaiPhieuChi v_dm_LoaiPhieuChi2 = source.Where((v_dm_LoaiPhieuChi e) => e.ID == ct_PhieuChi.ID_LOAIPHIEUCHI).FirstOrDefault();
				if (v_dm_LoaiPhieuChi2 == null || string.IsNullOrEmpty(v_dm_LoaiPhieuChi2.ID))
				{
					base.TempData["TitleError"] = "Không tìm thấy loại phiếu chi";
					apiResponse.Success = false;
					apiResponse.URL = base.Url.Action("Index", "Notfound");
					return new JsonResult
					{
						Data = apiResponse,
						JsonRequestBehavior = JsonRequestBehavior.AllowGet,
						MaxJsonLength = int.MaxValue
					};
				}
				ct_PhieuChi.lstdm_NhaCungCap = new List<ComboboxFrom>();
				if (v_dm_LoaiPhieuChi2.TYPE == 1)
				{
					ct_PhieuChi.lstdm_NhaCungCap = Utility.GetListData<ComboboxFrom>("Provider", "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
					apiResponse.TYPE = "divNCCAdd";
				}
				ct_PhieuChi.lstdm_KhachHang = new List<ComboboxFrom>();
				if (v_dm_LoaiPhieuChi2.TYPE == 2)
				{
					apiResponse.TYPE = "divKHACHHANGAdd";
					ct_PhieuChi.lstdm_KhachHang = Utility.GetListData<ComboboxFrom>("Customer", "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
				}
				ct_PhieuChi.lstdm_NhanVien = new List<ComboboxFrom>();
				if (v_dm_LoaiPhieuChi2.TYPE == 3)
				{
					apiResponse.TYPE = "divNHANVIENAdd";
					ct_PhieuChi.lstdm_NhanVien = Utility.GetListData<ComboboxFrom>("Employee", "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
				}
				ct_PhieuChi.lstdm_Xe = new List<ComboboxFrom>();
				if (v_dm_LoaiPhieuChi2.TYPE == 4)
				{
					apiResponse.TYPE = "divXeAdd";
					ct_PhieuChi.lstdm_Xe = Utility.GetListData<ComboboxFrom>("Car", "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
				}
				ct_PhieuChi.lstdm_LoaiPhieuChi = new List<v_dm_LoaiPhieuChi>();
				ct_PhieuChi.lstdm_TaiKhoanNganHang = new List<v_dm_TaiKhoanNganHang>();
				ct_PhieuChi.lstdm_TaiKhoanNganHang = Utility.GetListData<v_dm_TaiKhoanNganHang>("BankAccount", "", "", Utility.LOC_ID).Data as List<v_dm_TaiKhoanNganHang>;
				List<ValueEdit> detail = Utility.ConvertobjectToView(ct_PhieuChi);
				apiResponse.Detail = detail;
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
				if (!Utility.KiemTraQuyen("Payment", "Edit"))
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
				v_v_ct_PhieuChi ct_PhieuChi2 = new v_v_ct_PhieuChi();
				if (!string.IsNullOrEmpty(id))
				{
					apiResponse = Utility.GetDetail<v_v_ct_PhieuChi>(Utility.LOC_ID + "/" + id, "Payment");
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
						ct_PhieuChi2 = apiResponse.Data as v_v_ct_PhieuChi;
					}
				}
				ct_PhieuChi2.lstdm_NhaCungCap = new List<ComboboxFrom>();
				apiResponse.Success = true;
				List<v_dm_LoaiPhieuChi> list = Utility.GetListData<v_dm_LoaiPhieuChi>("TypePayment", "", "", Utility.LOC_ID).Data as List<v_dm_LoaiPhieuChi>;
				v_dm_LoaiPhieuChi v_dm_LoaiPhieuChi2 = list.Where((v_dm_LoaiPhieuChi e) => e.ID == ct_PhieuChi2.ID_LOAIPHIEUCHI).FirstOrDefault();
				if (v_dm_LoaiPhieuChi2 == null || string.IsNullOrEmpty(v_dm_LoaiPhieuChi2.ID))
				{
					base.TempData["TitleError"] = "Không tìm thấy loại phiếu chi";
					apiResponse.Success = false;
					apiResponse.URL = base.Url.Action("Index", "Notfound");
					return new JsonResult
					{
						Data = apiResponse,
						JsonRequestBehavior = JsonRequestBehavior.AllowGet,
						MaxJsonLength = int.MaxValue
					};
				}
				if (v_dm_LoaiPhieuChi2.TYPE == 1)
				{
					ct_PhieuChi2.lstdm_NhaCungCap = Utility.GetListData<ComboboxFrom>("Provider", "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
					apiResponse.TYPE = "divNCCEdit";
				}
				ct_PhieuChi2.lstdm_KhachHang = new List<ComboboxFrom>();
				if (v_dm_LoaiPhieuChi2.TYPE == 2)
				{
					apiResponse.TYPE = "divKHACHHANGEdit";
					ct_PhieuChi2.lstdm_KhachHang = Utility.GetListData<ComboboxFrom>("Customer", "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
				}
				ct_PhieuChi2.lstdm_NhanVien = new List<ComboboxFrom>();
				if (v_dm_LoaiPhieuChi2.TYPE == 3)
				{
					apiResponse.TYPE = "divNHANVIENEdit";
					ct_PhieuChi2.lstdm_NhanVien = Utility.GetListData<ComboboxFrom>("Employee", "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
				}
				ct_PhieuChi2.lstdm_Xe = new List<ComboboxFrom>();
				if (v_dm_LoaiPhieuChi2.TYPE == 4)
				{
					apiResponse.TYPE = "divXeEdit";
					ct_PhieuChi2.lstdm_Xe = Utility.GetListData<ComboboxFrom>("Car", "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
				}
				ct_PhieuChi2.lstdm_LoaiPhieuChi = new List<v_dm_LoaiPhieuChi>();
				ct_PhieuChi2.lstdm_LoaiPhieuChi = list;
				ct_PhieuChi2.lstdm_TaiKhoanNganHang = new List<v_dm_TaiKhoanNganHang>();
				ct_PhieuChi2.lstdm_TaiKhoanNganHang = Utility.GetListData<v_dm_TaiKhoanNganHang>("BankAccount", "", "", Utility.LOC_ID).Data as List<v_dm_TaiKhoanNganHang>;
				List<ValueEdit> detail = Utility.ConvertobjectTo(ct_PhieuChi2);
				apiResponse.Detail = detail;
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
		public ActionResult EditPopup([Bind(Include = "LOC_ID,ID,NAME_LOAIPHIEUCHI,ID_LOAIPHIEUCHI,NGAYLAP,MAPHIEU,SOPHIEU,NAME_KHACHHANG_NCC_NHANVIEN,ID_KHACHHANG,ID_NHACUNGCAP,ID_NHANVIEN,TENNGUOINHAN,DIACHI,NGUOICHITIEN,SOTIEN,LYDO,CHUNGTUKEMTHEO,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO,ID_TAIKHOANNGANHANG")] v_v_ct_PhieuChi ct_PhieuChi)
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
				if (!Utility.KiemTraQuyen("Payment", "Edit"))
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
				if (base.ModelState.IsValid)
				{
					ct_PhieuChi.LOC_ID = Utility.LOC_ID;
					ct_PhieuChi.ID_NGUOISUA = base.Session["idUser"].ToString();
					ct_PhieuChi.THOIGIANSUA = Utility.CurrentTime;
					apiResponse = Utility.Edit(Utility.LOC_ID + "/" + ct_PhieuChi.ID, (v_ct_PhieuChi)ct_PhieuChi, "Payment");
					if (apiResponse.Success)
					{
						apiResponse.ID = ct_PhieuChi.ID;
						if (apiResponse.Data != null)
						{
							ct_PhieuChi = JsonConvert.DeserializeObject<v_v_ct_PhieuChi>(apiResponse.Data.ToString());
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
					apiResponse.Data = Utility.GetModelState(base.ModelState, "Payment");
				}
				ct_PhieuChi.lstdm_NhaCungCap = new List<ComboboxFrom>();
				List<v_dm_LoaiPhieuChi> list = Utility.GetListData<v_dm_LoaiPhieuChi>("TypePayment", "", "", Utility.LOC_ID).Data as List<v_dm_LoaiPhieuChi>;
				v_dm_LoaiPhieuChi v_dm_LoaiPhieuChi2 = list.Where((v_dm_LoaiPhieuChi e) => e.ID == ct_PhieuChi.ID_LOAIPHIEUCHI).FirstOrDefault();
				if (v_dm_LoaiPhieuChi2 == null || string.IsNullOrEmpty(v_dm_LoaiPhieuChi2.ID))
				{
					base.TempData["TitleError"] = "Không tìm thấy loại phiếu chi";
					apiResponse.Success = false;
					apiResponse.URL = base.Url.Action("Index", "Notfound");
					return new JsonResult
					{
						Data = apiResponse,
						JsonRequestBehavior = JsonRequestBehavior.AllowGet,
						MaxJsonLength = int.MaxValue
					};
				}
				if (v_dm_LoaiPhieuChi2.TYPE == 1)
				{
					ct_PhieuChi.lstdm_NhaCungCap = Utility.GetListData<ComboboxFrom>("Provider", "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
					apiResponse.TYPE = "divNCCEdit";
				}
				ct_PhieuChi.lstdm_KhachHang = new List<ComboboxFrom>();
				if (v_dm_LoaiPhieuChi2.TYPE == 2)
				{
					apiResponse.TYPE = "divKHACHHANGEdit";
					ct_PhieuChi.lstdm_KhachHang = Utility.GetListData<ComboboxFrom>("Customer", "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
				}
				ct_PhieuChi.lstdm_NhanVien = new List<ComboboxFrom>();
				if (v_dm_LoaiPhieuChi2.TYPE == 3)
				{
					apiResponse.TYPE = "divNHANVIENEdit";
					ct_PhieuChi.lstdm_NhanVien = Utility.GetListData<ComboboxFrom>("Employee", "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
				}
				ct_PhieuChi.lstdm_Xe = new List<ComboboxFrom>();
				if (v_dm_LoaiPhieuChi2.TYPE == 4)
				{
					apiResponse.TYPE = "divXeEdit";
					ct_PhieuChi.lstdm_Xe = Utility.GetListData<ComboboxFrom>("Car", "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
				}
				ct_PhieuChi.lstdm_LoaiPhieuChi = new List<v_dm_LoaiPhieuChi>();
				ct_PhieuChi.lstdm_LoaiPhieuChi = list;
				ct_PhieuChi.lstdm_TaiKhoanNganHang = new List<v_dm_TaiKhoanNganHang>();
				ct_PhieuChi.lstdm_TaiKhoanNganHang = Utility.GetListData<v_dm_TaiKhoanNganHang>("BankAccount", "", "", Utility.LOC_ID).Data as List<v_dm_TaiKhoanNganHang>;
				apiResponse.Detail = Utility.ConvertobjectToView(ct_PhieuChi);
				List<ValueEdit> detail = Utility.ConvertobjectToView(ct_PhieuChi);
				apiResponse.Detail = detail;
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
				if (!Utility.KiemTraQuyen("Payment", "Delete"))
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
				apiResponse = Utility.Delete<v_ct_PhieuChi>(Utility.LOC_ID + "/" + id, "Payment");
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
				v_ct_PhieuChi v_ct_PhieuChi2 = new v_ct_PhieuChi();
				SP_Parameter sP_Parameter = new SP_Parameter();
				sP_Parameter.ID_PHIEUCHI = ID;
				apiResponse = Utility.ExecuteStoredProc<v_ct_PhieuChi>(sP_Parameter, "Sp_Get_DanhSachPhieuChi");
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
					v_ct_PhieuChi2 = (apiResponse.Data as List<v_ct_PhieuChi>).FirstOrDefault();
				}
				ReportClass reportClass = new ReportClass();
				reportClass.FileName = base.Server.MapPath("~/Report/rptPhieuChi.rpt");
				SP_Parameter_Report sP_Parameter_Report = new SP_Parameter_Report();
				sP_Parameter_Report.LOC_ID = Utility.LOC_ID;
				sP_Parameter_Report.ID_PHIEUCHI = ID;
				apiResponse = Utility.ExecuteStoredProc<DataTable>(sP_Parameter_Report, "Sp_Get_DanhSachPhieuChi");
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
				reportClass = Utility.GetFormulaFields(reportClass, v_ct_PhieuChi2);
				reportClass.SetDataSource(dataTable);
				base.Response.Buffer = false;
				base.Response.ClearContent();
				base.Response.ClearHeaders();
				Stream stream = reportClass.ExportToStream(ExportFormatType.PortableDocFormat);
				Utility.Report = reportClass;
				apiResponse = new ApiResponse();
				apiResponse.Success = true;
				apiResponse.NAME = Utility.GetTitleFrom("Payment") + " - " + v_ct_PhieuChi2.MAPHIEU;
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
