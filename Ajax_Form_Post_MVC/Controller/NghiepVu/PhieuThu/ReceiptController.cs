using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
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

	public class ReceiptController : Controller
	{
		public ActionResult Index(int Page = 1, DateTime? FromDate = null, DateTime? ToDate = null, string SearchString = "", string MAPHIEU = "", string IDCODE = "")
		{
			try
			{
				if (Utility.KiemTra())
				{
					return RedirectToAction("Index", "Admin");
				}
				if (!Utility.KiemTraQuyen("Receipt", "View"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				string text = "";
				ApiResponse apiResponse = new ApiResponse();
				IPagedList<v_ct_PhieuThu> iPagedList = new List<v_ct_PhieuThu>().ToList().ToPagedList(Page, Utility.GetPageSize());
				if (FromDate.HasValue || !string.IsNullOrEmpty(IDCODE) || !string.IsNullOrEmpty(MAPHIEU))
				{
					if (!string.IsNullOrEmpty(IDCODE) || !string.IsNullOrEmpty(MAPHIEU))
					{
						apiResponse = Utility.Get_DanhSachPhieuThu<v_ct_PhieuThu>("", null, null, MAPHIEU, IDCODE);
					}
					if (FromDate.HasValue)
					{
						apiResponse = Utility.Get_DanhSachPhieuThu<v_ct_PhieuThu>("", FromDate, ToDate, SearchString);
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
					text = (apiResponse.Data as List<v_ct_PhieuThu>).Sum((v_ct_PhieuThu s) => s.SOTIEN).ToString("N0");
					iPagedList = (apiResponse.Data as List<v_ct_PhieuThu>).ToPagedList(Page, Utility.GetPageSize());
				}
				v_v_ct_PhieuThu v_v_ct_PhieuThu2 = new v_v_ct_PhieuThu();
				v_v_ct_PhieuThu2.IPagedList = iPagedList;
				v_v_ct_PhieuThu2.lstdm_NhaCungCap = new List<ComboboxFrom>();
				v_v_ct_PhieuThu2.lstdm_KhachHang = new List<ComboboxFrom>();
				v_v_ct_PhieuThu2.lstdm_LoaiPhieuThu = new List<v_dm_LoaiPhieuThu>();
				if (Utility.GetListData<v_dm_LoaiPhieuThu>("TypeReceipt", "", "", Utility.LOC_ID).Data is List<v_dm_LoaiPhieuThu> source)
				{
					v_v_ct_PhieuThu2.lstdm_LoaiPhieuThu = (from e in source
														   where e.ISACTIVE
														   orderby e.TYPE
														   select e).ToList();
				}
				else
				{
					v_v_ct_PhieuThu2.lstdm_LoaiPhieuThu = new List<v_dm_LoaiPhieuThu>();
				}
				v_v_ct_PhieuThu2.lstdm_NhanVien = new List<ComboboxFrom>();
				v_v_ct_PhieuThu2.lstdm_TaiKhoanNganHang = new List<v_dm_TaiKhoanNganHang>();
				base.ViewBag.TotalSum = text;
				base.ViewBag.searchValue = SearchString;
				base.ViewBag.PermissionEdit = Utility.KiemTraQuyen("Receipt", "Edit");
				base.ViewBag.PermissionDelete = Utility.KiemTraQuyen("Receipt", "Delete");
				base.ViewBag.PermissionCreate = Utility.KiemTraQuyen("Receipt", "Create");
				base.ViewBag.fromdate = (FromDate.HasValue ? FromDate.Value.ToString("yyyy-MM-dd") : Utility.CurrentTime.AddMonths(-1).ToString("yyyy-MM-dd"));
				base.ViewBag.todate = (ToDate.HasValue ? ToDate.Value.ToString("yyyy-MM-dd") : Utility.CurrentTime.ToString("yyyy-MM-dd"));
				return View(v_v_ct_PhieuThu2);
			}
			catch (Exception ex)
			{
				Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
				base.TempData["TitleError"] = "Đã có lỗi phát sinh trong phiên làm việc!";
				base.TempData["DetailError"] = ex.Message;
				return RedirectToAction("Index", "Notfound");
			}
		}

		public ActionResult Create(int type = 2, string myModalAdd = "myModalAdd", string hienthichuyencongno = "0")
		{
			try
			{
				base.Session["IntWidth"] = type;
				if (Utility.KiemTra())
				{
					return RedirectToAction("Index", "Admin");
				}
				if (!Utility.KiemTraQuyen("Delivery", "Delivery_CreateReceipt") && !Utility.KiemTraQuyen("Receipt", "Create"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				v_v_ct_PhieuThu v_v_ct_PhieuThu2 = new v_v_ct_PhieuThu();
				v_v_ct_PhieuThu2.LOC_ID = Utility.LOC_ID;
				v_v_ct_PhieuThu2.ID_NGUOITAO = base.Session["idUser"].ToString();
				v_v_ct_PhieuThu2.THOIGIANTHEM = Utility.CurrentTime;
				v_v_ct_PhieuThu2.NGAYLAP = Utility.CurrentTime;
				v_v_ct_PhieuThu2.SOPHIEU = Utility.GetMaxID((ct_PhieuThu)v_v_ct_PhieuThu2, Utility.LOC_ID, v_v_ct_PhieuThu2.NGAYLAP.ToString("yyyy-MM-dd"));
				v_v_ct_PhieuThu2.MAPHIEU = API.GetMaPhieu("Receipt", v_v_ct_PhieuThu2.NGAYLAP, v_v_ct_PhieuThu2.SOPHIEU);
				v_v_ct_PhieuThu2.lstdm_KhachHang = new List<ComboboxFrom>();
				v_v_ct_PhieuThu2.ID = Guid.NewGuid().ToString();
				v_v_ct_PhieuThu2.lstdm_NhaCungCap = new List<ComboboxFrom>();
				v_v_ct_PhieuThu2.lstdm_KhachHang = new List<ComboboxFrom>();
				v_v_ct_PhieuThu2.lstdm_LoaiPhieuThu = new List<v_dm_LoaiPhieuThu>();
				v_v_ct_PhieuThu2.lstdm_NhanVien = new List<ComboboxFrom>();
				v_v_ct_PhieuThu2.lstdm_TaiKhoanNganHang = new List<v_dm_TaiKhoanNganHang>();
				base.ViewBag.myModalAdd = myModalAdd;
				v_v_ct_PhieuThu2.myModalAdd = myModalAdd;
				base.ViewBag.HienThiChuyenCongNo = hienthichuyencongno == "1";
				return View(v_v_ct_PhieuThu2);
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
		public ActionResult Create([Bind(Include = "ISCHUYENCONGNOCHONHANVIEN,LOC_ID,ID,ID_LOAIPHIEUTHU,NAME_LOAIPHIEUTHU,NGAYLAP,MAPHIEU,SOPHIEU,NAME_KHACHHANG_NCC_NHANVIEN,ID_KHACHHANG,ID_NHACUNGCAP,ID_NHANVIEN,NGUOINHANTIEN,TENNGUOINOPTIEN,DIACHI,SOTIEN,LYDO,CHUNGTUKEMTHEO,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO,ID_TAIKHOANNGANHANG")] v_ct_PhieuThu ct_PhieuThu)
		{
			try
			{
				if (Utility.KiemTra())
				{
					return RedirectToAction("Index", "Admin");
				}
				if (!Utility.KiemTraQuyen("Receipt", "Create"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				if (base.ModelState.IsValid)
				{
					ct_PhieuThu.LOC_ID = Utility.LOC_ID;
					ct_PhieuThu.ID_NGUOITAO = base.Session["idUser"].ToString();
					ct_PhieuThu.THOIGIANTHEM = Utility.CurrentTime;
					ApiResponse apiResponse = Utility.Create((ct_PhieuThu)ct_PhieuThu, "Receipt");
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
				return View(ct_PhieuThu);
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
				if (!Utility.KiemTraQuyen("Receipt", "Edit"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				v_v_ct_PhieuThu v_v_ct_PhieuThu2 = new v_v_ct_PhieuThu();
				if (!string.IsNullOrEmpty(id))
				{
					apiResponse = Utility.GetDetail<v_ct_PhieuThu>(Utility.LOC_ID + "/" + id, "Receipt");
					if (!apiResponse.Success)
					{
						base.TempData["TitleError"] = apiResponse.Message;
						return RedirectToAction("Index", "Notfound");
					}
					if (apiResponse.Data != null)
					{
						v_v_ct_PhieuThu2 = apiResponse.Data as v_v_ct_PhieuThu;
					}
				}
				v_v_ct_PhieuThu2.lstdm_KhachHang = new List<ComboboxFrom>();
				v_v_ct_PhieuThu2.lstdm_NhaCungCap = new List<ComboboxFrom>();
				v_v_ct_PhieuThu2.lstdm_LoaiPhieuThu = new List<v_dm_LoaiPhieuThu>();
				v_v_ct_PhieuThu2.lstdm_NhanVien = new List<ComboboxFrom>();
				v_v_ct_PhieuThu2.lstdm_TaiKhoanNganHang = new List<v_dm_TaiKhoanNganHang>();
				return View(v_v_ct_PhieuThu2);
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
		public ActionResult Edit([Bind(Include = "ISCHUYENCONGNOCHONHANVIEN,LOC_ID,ID,ID_LOAIPHIEUTHU,NAME_LOAIPHIEUTHU,NGAYLAP,MAPHIEU,SOPHIEU,NAME_KHACHHANG_NCC_NHANVIEN,ID_KHACHHANG,ID_NHACUNGCAP,ID_NHANVIEN,NGUOINHANTIEN,TENNGUOINOPTIEN,DIACHI,SOTIEN,LYDO,CHUNGTUKEMTHEO,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO,ID_TAIKHOANNGANHANG")] v_ct_PhieuThu ct_PhieuThu)
		{
			try
			{
				if (Utility.KiemTra())
				{
					return RedirectToAction("Index", "Admin");
				}
				if (!Utility.KiemTraQuyen("Receipt", "Edit"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				if (base.ModelState.IsValid)
				{
					ct_PhieuThu.LOC_ID = Utility.LOC_ID;
					ct_PhieuThu.ID_NGUOISUA = base.Session["idUser"].ToString();
					ct_PhieuThu.THOIGIANSUA = Utility.CurrentTime;
					ApiResponse apiResponse = Utility.Edit(Utility.LOC_ID + "/" + ct_PhieuThu.ID, ct_PhieuThu, "Receipt");
					if (apiResponse.Success)
					{
						return RedirectToAction("Index");
					}
					base.ModelState.AddModelError(string.Empty, apiResponse.Message);
				}
				return View(ct_PhieuThu);
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
				if (!Utility.KiemTraQuyen("Receipt", "Delete"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				ApiResponse apiResponse = Utility.Delete<v_ct_PhieuThu>(Utility.LOC_ID + "/" + id, "Receipt");
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
				if (!Utility.KiemTraQuyen("Delivery", "Delivery_CreateReceipt") && !Utility.KiemTraQuyen("Receipt", "Create"))
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
				List<v_dm_LoaiPhieuThu> source = Utility.GetListData<v_dm_LoaiPhieuThu>("TypeReceipt", "", "", Utility.LOC_ID).Data as List<v_dm_LoaiPhieuThu>;
				v_dm_LoaiPhieuThu v_dm_LoaiPhieuThu2 = source.Where((v_dm_LoaiPhieuThu e) => e.ID == ID_LOAIPHIEU).FirstOrDefault();
				if (v_dm_LoaiPhieuThu2 == null || string.IsNullOrEmpty(v_dm_LoaiPhieuThu2.ID))
				{
					base.TempData["TitleError"] = "Không tìm thấy loại phiếu thu";
					apiResponse.Success = false;
					apiResponse.URL = base.Url.Action("Index", "Notfound");
					return new JsonResult
					{
						Data = apiResponse,
						JsonRequestBehavior = JsonRequestBehavior.AllowGet,
						MaxJsonLength = int.MaxValue
					};
				}
				v_v_ct_PhieuThu ct_PhieuThu2 = new v_v_ct_PhieuThu();
				apiResponse.Success = true;
				ct_PhieuThu2.ID_LOAIPHIEUTHU = ID_LOAIPHIEU;
				ct_PhieuThu2.LOC_ID = Utility.LOC_ID;
				ct_PhieuThu2.ID = Guid.NewGuid().ToString();
				ct_PhieuThu2.NGAYLAP = Utility.CurrentTime;
				if (CHUNGTUKEMTHEO.StartsWith("PX-"))
				{
					string nameController = "Output";
					ApiResponse value = GetValue<v_v_ct_PhieuXuat>(apiResponse, nameController, CHUNGTUKEMTHEO);
					if (value.Detail != null)
					{
						ct_PhieuThu2.SOTIEN = (value.Detail as v_v_ct_PhieuXuat).TONGTIEN;
						ct_PhieuThu2.NGAYLAP = (value.Detail as v_v_ct_PhieuXuat).NGAYLAP;
					}
				}
				if (CHUNGTUKEMTHEO.StartsWith("PGH-"))
				{
					string nameController2 = "Delivery";
					ApiResponse value2 = GetValue<v_v_ct_PhieuGiaoHang>(apiResponse, nameController2, CHUNGTUKEMTHEO);
					if (value2.Detail != null)
					{
						ct_PhieuThu2.NGAYLAP = (value2.Detail as v_v_ct_PhieuGiaoHang).NGAYLAP;
					}
				}
				ct_PhieuThu2.SOPHIEU = Utility.GetMaxID((ct_PhieuThu)ct_PhieuThu2, Utility.LOC_ID, ct_PhieuThu2.NGAYLAP.ToString("yyyy-MM-dd"));
				ct_PhieuThu2.MAPHIEU = API.GetMaPhieu("Receipt", ct_PhieuThu2.NGAYLAP, ct_PhieuThu2.SOPHIEU);
				ct_PhieuThu2.CHUNGTUKEMTHEO = CHUNGTUKEMTHEO;
				ct_PhieuThu2.lstdm_NhaCungCap = new List<ComboboxFrom>();
				if (v_dm_LoaiPhieuThu2.TYPE == 1)
				{
					ct_PhieuThu2.lstdm_NhaCungCap = Utility.GetListData<ComboboxFrom>("Provider", "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
					ct_PhieuThu2.ID_NHACUNGCAP = ID_KHACHAHANG;
					apiResponse.TYPE = "divNCCAdd";
					foreach (ComboboxFrom item in ct_PhieuThu2.lstdm_NhaCungCap.Where((ComboboxFrom s) => s.ISDEFAULT))
					{
						item.ISDEFAULT = false;
					}
					ComboboxFrom comboboxFrom = ct_PhieuThu2.lstdm_NhaCungCap.Where((ComboboxFrom s) => s.ID == ct_PhieuThu2.ID_NHANVIEN).FirstOrDefault();
					if (comboboxFrom != null)
					{
						comboboxFrom.ISDEFAULT = true;
					}
				}
				ct_PhieuThu2.lstdm_KhachHang = new List<ComboboxFrom>();
				if (v_dm_LoaiPhieuThu2.TYPE == 2)
				{
					apiResponse.TYPE = "divKHACHHANGAdd";
					ct_PhieuThu2.lstdm_KhachHang = Utility.GetListData<ComboboxFrom>("Customer", "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
					ct_PhieuThu2.ID_KHACHHANG = ID_KHACHAHANG;
					foreach (ComboboxFrom item2 in ct_PhieuThu2.lstdm_KhachHang.Where((ComboboxFrom s) => s.ISDEFAULT))
					{
						item2.ISDEFAULT = false;
					}
					ComboboxFrom comboboxFrom2 = ct_PhieuThu2.lstdm_KhachHang.Where((ComboboxFrom s) => s.ID == ct_PhieuThu2.ID_KHACHHANG).FirstOrDefault();
					if (comboboxFrom2 != null)
					{
						comboboxFrom2.ISDEFAULT = true;
					}
				}
				ct_PhieuThu2.lstdm_NhanVien = new List<ComboboxFrom>();
				if (v_dm_LoaiPhieuThu2.TYPE == 3)
				{
					apiResponse.TYPE = "divNHANVIENAdd";
					ct_PhieuThu2.lstdm_NhanVien = Utility.GetListData<ComboboxFrom>("Employee", "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
					ct_PhieuThu2.ID_NHANVIEN = ID_KHACHAHANG;
					foreach (ComboboxFrom item3 in ct_PhieuThu2.lstdm_NhanVien.Where((ComboboxFrom s) => s.ISDEFAULT))
					{
						item3.ISDEFAULT = false;
					}
					ComboboxFrom comboboxFrom3 = ct_PhieuThu2.lstdm_NhanVien.Where((ComboboxFrom s) => s.ID == ct_PhieuThu2.ID_NHANVIEN).FirstOrDefault();
					if (comboboxFrom3 != null)
					{
						comboboxFrom3.ISDEFAULT = true;
					}
				}
				ct_PhieuThu2.lstdm_LoaiPhieuThu = new List<v_dm_LoaiPhieuThu>();
				ct_PhieuThu2.lstdm_LoaiPhieuThu = Utility.GetListData<v_dm_LoaiPhieuThu>("TypeReceipt", "", "", Utility.LOC_ID).Data as List<v_dm_LoaiPhieuThu>;
				ct_PhieuThu2.lstdm_TaiKhoanNganHang = new List<v_dm_TaiKhoanNganHang>();
				ct_PhieuThu2.lstdm_TaiKhoanNganHang = Utility.GetListData<v_dm_TaiKhoanNganHang>("BankAccount", "", "", Utility.LOC_ID).Data as List<v_dm_TaiKhoanNganHang>;
				List<ValueEdit> list = Utility.ConvertobjectTo(ct_PhieuThu2);
				ValueEdit valueEdit = new ValueEdit();
				valueEdit.Key = "lblName";
				valueEdit.Value = v_dm_LoaiPhieuThu2.NAME.ToUpper();
				list.Add(valueEdit);
				apiResponse.Detail = list;
				if (!string.IsNullOrEmpty(ID_KHACHAHANG) || !string.IsNullOrEmpty(CHUNGTUKEMTHEO))
				{
					apiResponse.NAME = "myModalAddReceipt";
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
		public ActionResult CreatePopup([Bind(Include = "ISCHUYENCONGNOCHONHANVIEN,LOC_ID,ID,ID_LOAIPHIEUTHU,NAME_LOAIPHIEUTHU,NGAYLAP,MAPHIEU,SOPHIEU,NAME_KHACHHANG_NCC_NHANVIEN,ID_KHACHHANG,ID_NHACUNGCAP,ID_NHANVIEN,NGUOINHANTIEN,TENNGUOINOPTIEN,DIACHI,SOTIEN,LYDO,CHUNGTUKEMTHEO,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO,ID_TAIKHOANNGANHANG,myModalAdd")] v_v_ct_PhieuThu ct_PhieuThu)
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
				if (!Utility.KiemTraQuyen("Delivery", "Delivery_CreateReceipt") && !Utility.KiemTraQuyen("Receipt", "Create"))
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
					ct_PhieuThu.NGAYLAP = ct_PhieuThu.NGAYLAP.AddHours(Utility.CurrentTime.Hour).AddMinutes(Utility.CurrentTime.Minute);
					ct_PhieuThu.LOC_ID = Utility.LOC_ID;
					ct_PhieuThu.ID_NGUOITAO = base.Session["idUser"].ToString();
					ct_PhieuThu.THOIGIANTHEM = Utility.CurrentTime;
					apiResponse = Utility.Create((v_ct_PhieuThu)ct_PhieuThu, "Receipt");
					if (apiResponse.Success)
					{
						ct_PhieuThu.NGAYLAP = Utility.CurrentTime;
						ApiResponse apiResponse2 = apiResponse;
						int sOPHIEU = (ct_PhieuThu.SOPHIEU = Utility.GetMaxID((ct_PhieuThu)ct_PhieuThu, Utility.LOC_ID, ct_PhieuThu.NGAYLAP.ToString("yyyy-MM-dd")));
						apiResponse2.SOPHIEU = sOPHIEU;
						ct_PhieuThu.MAPHIEU = API.GetMaPhieu("Receipt", ct_PhieuThu.NGAYLAP, ct_PhieuThu.SOPHIEU);
						apiResponse.NewID = Guid.NewGuid().ToString();
						apiResponse.MAPHIEU = ct_PhieuThu.MAPHIEU;
						if (apiResponse.Data != null)
						{
							ct_PhieuThu = JsonConvert.DeserializeObject<v_v_ct_PhieuThu>(apiResponse.Data.ToString());
						}
					}
					else
					{
						base.ModelState.AddModelError(string.Empty, apiResponse.Message);
						if (apiResponse.CheckValue)
						{
							ct_PhieuThu.NGAYLAP = Utility.CurrentTime;
							ApiResponse apiResponse3 = apiResponse;
							int sOPHIEU = (ct_PhieuThu.SOPHIEU = Utility.GetMaxID((ct_PhieuThu)ct_PhieuThu, Utility.LOC_ID, ct_PhieuThu.NGAYLAP.ToString("yyyy-MM-dd")));
							apiResponse3.SOPHIEU = sOPHIEU;
							ct_PhieuThu.MAPHIEU = API.GetMaPhieu("Receipt", ct_PhieuThu.NGAYLAP, ct_PhieuThu.SOPHIEU);
							apiResponse.NewID = Guid.NewGuid().ToString();
							apiResponse.MAPHIEU = ct_PhieuThu.MAPHIEU;
						}
					}
				}
				else
				{
					apiResponse.Success = false;
					apiResponse.Data = Utility.GetModelState(base.ModelState, "Receipt");
				}
				apiResponse.ID = ct_PhieuThu.ID;
				List<v_dm_LoaiPhieuThu> source = Utility.GetListData<v_dm_LoaiPhieuThu>("TypeReceipt", "", "", Utility.LOC_ID).Data as List<v_dm_LoaiPhieuThu>;
				v_dm_LoaiPhieuThu v_dm_LoaiPhieuThu2 = source.Where((v_dm_LoaiPhieuThu e) => e.ID == ct_PhieuThu.ID_LOAIPHIEUTHU).FirstOrDefault();
				if (v_dm_LoaiPhieuThu2 == null || string.IsNullOrEmpty(v_dm_LoaiPhieuThu2.ID))
				{
					base.TempData["TitleError"] = "Không tìm thấy loại phiếu thu";
					apiResponse.Success = false;
					apiResponse.URL = base.Url.Action("Index", "Notfound");
					return new JsonResult
					{
						Data = apiResponse,
						JsonRequestBehavior = JsonRequestBehavior.AllowGet,
						MaxJsonLength = int.MaxValue
					};
				}
				ct_PhieuThu.lstdm_NhaCungCap = new List<ComboboxFrom>();
				if (v_dm_LoaiPhieuThu2.TYPE == 1)
				{
					ct_PhieuThu.lstdm_NhaCungCap = Utility.GetListData<ComboboxFrom>("Provider", "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
					apiResponse.TYPE = "divNCCAdd";
				}
				ct_PhieuThu.lstdm_KhachHang = new List<ComboboxFrom>();
				if (v_dm_LoaiPhieuThu2.TYPE == 2)
				{
					apiResponse.TYPE = "divKHACHHANGAdd";
					ct_PhieuThu.lstdm_KhachHang = Utility.GetListData<ComboboxFrom>("Customer", "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
				}
				ct_PhieuThu.lstdm_NhanVien = new List<ComboboxFrom>();
				if (v_dm_LoaiPhieuThu2.TYPE == 3)
				{
					apiResponse.TYPE = "divNHANVIENAdd";
					ct_PhieuThu.lstdm_NhanVien = Utility.GetListData<ComboboxFrom>("Employee", "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
				}
				ct_PhieuThu.lstdm_LoaiPhieuThu = new List<v_dm_LoaiPhieuThu>();
				ct_PhieuThu.lstdm_TaiKhoanNganHang = new List<v_dm_TaiKhoanNganHang>();
				ct_PhieuThu.lstdm_TaiKhoanNganHang = Utility.GetListData<v_dm_TaiKhoanNganHang>("BankAccount", "", "", Utility.LOC_ID).Data as List<v_dm_TaiKhoanNganHang>;
				List<ValueEdit> detail = Utility.ConvertobjectToView(ct_PhieuThu);
				apiResponse.Detail = detail;
				apiResponse.NAME = ct_PhieuThu.myModalAdd;
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
				if (!Utility.KiemTraQuyen("Receipt", "Edit"))
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
				v_v_ct_PhieuThu ct_PhieuThu2 = new v_v_ct_PhieuThu();
				if (!string.IsNullOrEmpty(id))
				{
					apiResponse = Utility.GetDetail<v_v_ct_PhieuThu>(Utility.LOC_ID + "/" + id, "Receipt");
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
						ct_PhieuThu2 = apiResponse.Data as v_v_ct_PhieuThu;
					}
				}
				ct_PhieuThu2.lstdm_NhaCungCap = new List<ComboboxFrom>();
				apiResponse.Success = true;
				List<v_dm_LoaiPhieuThu> list = Utility.GetListData<v_dm_LoaiPhieuThu>("TypeReceipt", "", "", Utility.LOC_ID).Data as List<v_dm_LoaiPhieuThu>;
				v_dm_LoaiPhieuThu v_dm_LoaiPhieuThu2 = list.Where((v_dm_LoaiPhieuThu e) => e.ID == ct_PhieuThu2.ID_LOAIPHIEUTHU).FirstOrDefault();
				if (v_dm_LoaiPhieuThu2 == null || string.IsNullOrEmpty(v_dm_LoaiPhieuThu2.ID))
				{
					base.TempData["TitleError"] = "Không tìm thấy loại phiếu thu";
					apiResponse.Success = false;
					apiResponse.URL = base.Url.Action("Index", "Notfound");
					return new JsonResult
					{
						Data = apiResponse,
						JsonRequestBehavior = JsonRequestBehavior.AllowGet,
						MaxJsonLength = int.MaxValue
					};
				}
				if (v_dm_LoaiPhieuThu2.TYPE == 1)
				{
					ct_PhieuThu2.lstdm_NhaCungCap = Utility.GetListData<ComboboxFrom>("Provider", "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
					apiResponse.TYPE = "divNCCEdit";
				}
				ct_PhieuThu2.lstdm_KhachHang = new List<ComboboxFrom>();
				if (v_dm_LoaiPhieuThu2.TYPE == 2)
				{
					apiResponse.TYPE = "divKHACHHANGEdit";
					ct_PhieuThu2.lstdm_KhachHang = Utility.GetListData<ComboboxFrom>("Customer", "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
				}
				ct_PhieuThu2.lstdm_NhanVien = new List<ComboboxFrom>();
				if (v_dm_LoaiPhieuThu2.TYPE == 3)
				{
					apiResponse.TYPE = "divNHANVIENEdit";
					ct_PhieuThu2.lstdm_NhanVien = Utility.GetListData<ComboboxFrom>("Employee", "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
				}
				ct_PhieuThu2.lstdm_LoaiPhieuThu = new List<v_dm_LoaiPhieuThu>();
				ct_PhieuThu2.lstdm_LoaiPhieuThu = list;
				ct_PhieuThu2.lstdm_TaiKhoanNganHang = new List<v_dm_TaiKhoanNganHang>();
				ct_PhieuThu2.lstdm_TaiKhoanNganHang = Utility.GetListData<v_dm_TaiKhoanNganHang>("BankAccount", "", "", Utility.LOC_ID).Data as List<v_dm_TaiKhoanNganHang>;
				List<ValueEdit> detail = Utility.ConvertobjectTo(ct_PhieuThu2);
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
		public ActionResult EditPopup([Bind(Include = "ISCHUYENCONGNOCHONHANVIEN,LOC_ID,ID,ID_LOAIPHIEUTHU,NAME_LOAIPHIEUTHU,NGAYLAP,MAPHIEU,SOPHIEU,NAME_KHACHHANG_NCC_NHANVIEN,ID_KHACHHANG,ID_NHACUNGCAP,ID_NHANVIEN,NGUOINHANTIEN,TENNGUOINOPTIEN,DIACHI,SOTIEN,LYDO,CHUNGTUKEMTHEO,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO,ID_TAIKHOANNGANHANG")] v_v_ct_PhieuThu ct_PhieuThu)
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
				if (!Utility.KiemTraQuyen("Receipt", "Edit"))
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
					ct_PhieuThu.LOC_ID = Utility.LOC_ID;
					ct_PhieuThu.ID_NGUOISUA = base.Session["idUser"].ToString();
					ct_PhieuThu.THOIGIANSUA = Utility.CurrentTime;
					apiResponse = Utility.Edit(Utility.LOC_ID + "/" + ct_PhieuThu.ID, (v_ct_PhieuThu)ct_PhieuThu, "Receipt");
					if (apiResponse.Success)
					{
						apiResponse.ID = ct_PhieuThu.ID;
						if (apiResponse.Data != null)
						{
							ct_PhieuThu = JsonConvert.DeserializeObject<v_v_ct_PhieuThu>(apiResponse.Data.ToString());
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
					apiResponse.Data = Utility.GetModelState(base.ModelState, "Receipt");
				}
				ct_PhieuThu.lstdm_NhaCungCap = new List<ComboboxFrom>();
				List<v_dm_LoaiPhieuThu> list = Utility.GetListData<v_dm_LoaiPhieuThu>("TypeReceipt", "", "", Utility.LOC_ID).Data as List<v_dm_LoaiPhieuThu>;
				v_dm_LoaiPhieuThu v_dm_LoaiPhieuThu2 = list.Where((v_dm_LoaiPhieuThu e) => e.ID == ct_PhieuThu.ID_LOAIPHIEUTHU).FirstOrDefault();
				if (v_dm_LoaiPhieuThu2 == null || string.IsNullOrEmpty(v_dm_LoaiPhieuThu2.ID))
				{
					base.TempData["TitleError"] = "Không tìm thấy loại phiếu thu";
					apiResponse.Success = false;
					apiResponse.URL = base.Url.Action("Index", "Notfound");
					return new JsonResult
					{
						Data = apiResponse,
						JsonRequestBehavior = JsonRequestBehavior.AllowGet,
						MaxJsonLength = int.MaxValue
					};
				}
				if (v_dm_LoaiPhieuThu2.TYPE == 1)
				{
					ct_PhieuThu.lstdm_NhaCungCap = Utility.GetListData<ComboboxFrom>("Provider", "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
					apiResponse.TYPE = "divNCCEdit";
				}
				ct_PhieuThu.lstdm_KhachHang = new List<ComboboxFrom>();
				if (v_dm_LoaiPhieuThu2.TYPE == 2)
				{
					apiResponse.TYPE = "divKHACHHANGEdit";
					ct_PhieuThu.lstdm_KhachHang = Utility.GetListData<ComboboxFrom>("Customer", "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
				}
				ct_PhieuThu.lstdm_NhanVien = new List<ComboboxFrom>();
				if (v_dm_LoaiPhieuThu2.TYPE == 3)
				{
					apiResponse.TYPE = "divNHANVIENEdit";
					ct_PhieuThu.lstdm_NhanVien = Utility.GetListData<ComboboxFrom>("Employee", "", "", Utility.LOC_ID).Data as List<ComboboxFrom>;
				}
				ct_PhieuThu.lstdm_LoaiPhieuThu = new List<v_dm_LoaiPhieuThu>();
				ct_PhieuThu.lstdm_LoaiPhieuThu = list;
				ct_PhieuThu.lstdm_TaiKhoanNganHang = new List<v_dm_TaiKhoanNganHang>();
				ct_PhieuThu.lstdm_TaiKhoanNganHang = Utility.GetListData<v_dm_TaiKhoanNganHang>("BankAccount", "", "", Utility.LOC_ID).Data as List<v_dm_TaiKhoanNganHang>;
				apiResponse.Detail = Utility.ConvertobjectToView(ct_PhieuThu);
				List<ValueEdit> detail = Utility.ConvertobjectToView(ct_PhieuThu);
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
				if (!Utility.KiemTraQuyen("Receipt", "Delete"))
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
				apiResponse = Utility.Delete<v_ct_PhieuThu>(Utility.LOC_ID + "/" + id, "Receipt");
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
				byte[] bytes = Encoding.UTF8.GetBytes("https://ironsoftware.com/csharp/barcode/");
				string text = Path.Combine(base.Server.MapPath("~/Images_Upload/Product/"), "MyBinaryQR.png");
				string text2 = Path.Combine(base.Server.MapPath("~/Images_Upload/Logo/"), "logoTrangHiepPhat.jpg");
				v_ct_PhieuThu v_ct_PhieuThu2 = new v_ct_PhieuThu();
				SP_Parameter sP_Parameter = new SP_Parameter();
				sP_Parameter.ID_PHIEUTHU = ID;
				apiResponse = Utility.ExecuteStoredProc<v_ct_PhieuThu>(sP_Parameter, "Sp_Get_DanhSachPhieuThu");
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
					v_ct_PhieuThu2 = (apiResponse.Data as List<v_ct_PhieuThu>).FirstOrDefault();
				}
				ReportClass report = new ReportClass();
				SP_Parameter_Report sP_Parameter_Report = new SP_Parameter_Report();
				sP_Parameter_Report.LOC_ID = Utility.LOC_ID;
				sP_Parameter_Report.ID_PHIEUTHU = ID;
				apiResponse = Utility.ExecuteStoredProc<DataTable>(sP_Parameter_Report, "Sp_Get_DanhSachPhieuThu");
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
				report = Utility.GetFormulaFields(report, v_ct_PhieuThu2);
				report.SetDataSource(dataTable);
				base.Response.Buffer = false;
				base.Response.ClearContent();
				base.Response.ClearHeaders();
				Stream stream = report.ExportToStream(ExportFormatType.PortableDocFormat);
				Utility.Report = report;
				apiResponse = new ApiResponse();
				apiResponse.Success = true;
				apiResponse.NAME = Utility.GetTitleFrom("Receipt") + " - " + v_ct_PhieuThu2.MAPHIEU;
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
