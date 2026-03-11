using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using DatabaseTHP;
using DatabaseTHP.Class;
using DatabaseTHP.StoredProcedure;
using DatabaseTHP.StoredProcedure.Parameter;
using MVC_QuanLyTHP.Class;
using MVC_QuanLyTHP.Models;

namespace MVC_QuanLyTHP.Controllers
{

	public class OutputDetailController : Controller
	{
		public ActionResult Index()
		{
			try
			{
				if (Utility.KiemTra())
				{
					return RedirectToAction("Index", "Admin");
				}
				ApiResponse apiResponse = new ApiResponse();
				List<Sp_Get_DanhSachPhieuXuat_ChiTiet_Result> iPagedList = new List<Sp_Get_DanhSachPhieuXuat_ChiTiet_Result>().OrderByDescending((Sp_Get_DanhSachPhieuXuat_ChiTiet_Result s) => s.NAME).ToList();
				v_Sp_Get_DanhSachPhieuXuat_ChiTiet_Result v_Sp_Get_DanhSachPhieuXuat_ChiTiet_Result2 = new v_Sp_Get_DanhSachPhieuXuat_ChiTiet_Result();
				v_Sp_Get_DanhSachPhieuXuat_ChiTiet_Result2.IPagedList = iPagedList;
				v_Sp_Get_DanhSachPhieuXuat_ChiTiet_Result2.lstdm_KhuVuc = new List<v_dm_KhuVuc>();
				v_Sp_Get_DanhSachPhieuXuat_ChiTiet_Result2.lstdm_KhuVuc = Utility.GetListData<v_dm_KhuVuc>("Area", "", "", Utility.LOC_ID).Data as List<v_dm_KhuVuc>;
				v_Sp_Get_DanhSachPhieuXuat_ChiTiet_Result2.lstdm_NhomKhachHang = new List<v_dm_NhomKhachHang>();
				v_Sp_Get_DanhSachPhieuXuat_ChiTiet_Result2.lstdm_NhomKhachHang = Utility.GetListData<v_dm_NhomKhachHang>("GroupCustomer", "", "", Utility.LOC_ID).Data as List<v_dm_NhomKhachHang>;
				v_Sp_Get_DanhSachPhieuXuat_ChiTiet_Result2.lstdm_KhachHang = new List<v_dm_KhachHang>();
				v_Sp_Get_DanhSachPhieuXuat_ChiTiet_Result2.lstdm_KhachHang = Utility.GetListData<v_dm_KhachHang>("Customer", "", "", Utility.LOC_ID).Data as List<v_dm_KhachHang>;
				v_Sp_Get_DanhSachPhieuXuat_ChiTiet_Result2.ISTHEOTHOIGIAN = true;
				v_Sp_Get_DanhSachPhieuXuat_ChiTiet_Result2.TUNGAY = DateTime.Now;
				v_Sp_Get_DanhSachPhieuXuat_ChiTiet_Result2.DENNGAY = DateTime.Now;
				return View(v_Sp_Get_DanhSachPhieuXuat_ChiTiet_Result2);
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
		public ActionResult Index(SP_Parameter sp_Parameter)
		{
			try
			{
				if (Utility.KiemTra())
				{
					return RedirectToAction("Index", "Admin");
				}
				List<Sp_Get_DanhSachPhieuXuat_ChiTiet_Result> iPagedList = new List<Sp_Get_DanhSachPhieuXuat_ChiTiet_Result>().OrderByDescending((Sp_Get_DanhSachPhieuXuat_ChiTiet_Result s) => s.NAME).ToList();
				if (base.ModelState.IsValid)
				{
					ApiResponse apiResponse = new ApiResponse();
					sp_Parameter.LOC_ID = Utility.LOC_ID;
					sp_Parameter.ID_KHACHHANG = sp_Parameter.ID_KHACHHANG ?? "";
					sp_Parameter.ID_NHOMKHACHHANG = sp_Parameter.ID_NHOMKHACHHANG ?? "";
					sp_Parameter.ID_KHUVUC = sp_Parameter.ID_KHUVUC ?? "";
					sp_Parameter.ISTHEOTHOIGIAN = sp_Parameter.ISTHEOTHOIGIAN == true;
					iPagedList = new List<Sp_Get_DanhSachPhieuXuat_ChiTiet_Result>().OrderByDescending((Sp_Get_DanhSachPhieuXuat_ChiTiet_Result s) => s.NAME).ToList();
					apiResponse = Utility.Get_DanhSachPhieuXuat_ChiTiet<Sp_Get_DanhSachPhieuXuat_ChiTiet_Result>(sp_Parameter);
					if (!apiResponse.Success)
					{
						base.TempData["TitleError"] = apiResponse.Message;
						return RedirectToAction("Index", "Notfound");
					}
					if (apiResponse.Data != null)
					{
						iPagedList = (string.IsNullOrEmpty(sp_Parameter.KEY) ? (apiResponse.Data as List<Sp_Get_DanhSachPhieuXuat_ChiTiet_Result>).ToList() : (apiResponse.Data as List<Sp_Get_DanhSachPhieuXuat_ChiTiet_Result>).Where((Sp_Get_DanhSachPhieuXuat_ChiTiet_Result s) => s.TINHTRANGHOADON.ToUpper().Contains(sp_Parameter.KEY.ToUpper())).ToList());
					}
				}
				v_Sp_Get_DanhSachPhieuXuat_ChiTiet_Result v_Sp_Get_DanhSachPhieuXuat_ChiTiet_Result2 = new v_Sp_Get_DanhSachPhieuXuat_ChiTiet_Result();
				v_Sp_Get_DanhSachPhieuXuat_ChiTiet_Result2.IPagedList = iPagedList;
				v_Sp_Get_DanhSachPhieuXuat_ChiTiet_Result2.lstdm_KhuVuc = new List<v_dm_KhuVuc>();
				v_Sp_Get_DanhSachPhieuXuat_ChiTiet_Result2.lstdm_KhuVuc = Utility.GetListData<v_dm_KhuVuc>("Area", "", "", Utility.LOC_ID).Data as List<v_dm_KhuVuc>;
				v_Sp_Get_DanhSachPhieuXuat_ChiTiet_Result2.lstdm_NhomKhachHang = new List<v_dm_NhomKhachHang>();
				v_Sp_Get_DanhSachPhieuXuat_ChiTiet_Result2.lstdm_NhomKhachHang = Utility.GetListData<v_dm_NhomKhachHang>("GroupCustomer", "", "", Utility.LOC_ID).Data as List<v_dm_NhomKhachHang>;
				v_Sp_Get_DanhSachPhieuXuat_ChiTiet_Result2.lstdm_KhachHang = new List<v_dm_KhachHang>();
				v_Sp_Get_DanhSachPhieuXuat_ChiTiet_Result2.lstdm_KhachHang = Utility.GetListData<v_dm_KhachHang>("Customer", "", "", Utility.LOC_ID).Data as List<v_dm_KhachHang>;
				v_Sp_Get_DanhSachPhieuXuat_ChiTiet_Result2.ISTHEOTHOIGIAN = sp_Parameter.ISTHEOTHOIGIAN == true;
				v_Sp_Get_DanhSachPhieuXuat_ChiTiet_Result2.ID_KHACHHANG = sp_Parameter.ID_KHACHHANG;
				v_Sp_Get_DanhSachPhieuXuat_ChiTiet_Result2.ID_NHOMKHACHHANG = sp_Parameter.ID_NHOMKHACHHANG;
				v_Sp_Get_DanhSachPhieuXuat_ChiTiet_Result2.ID_KHUVUC = sp_Parameter.ID_KHUVUC;
				v_Sp_Get_DanhSachPhieuXuat_ChiTiet_Result2.KEY = sp_Parameter.KEY;
				v_Sp_Get_DanhSachPhieuXuat_ChiTiet_Result2.TUNGAY = sp_Parameter.TUNGAY ?? DateTime.Now;
				v_Sp_Get_DanhSachPhieuXuat_ChiTiet_Result2.DENNGAY = sp_Parameter.DENNGAY ?? DateTime.Now;
				return View(v_Sp_Get_DanhSachPhieuXuat_ChiTiet_Result2);
			}
			catch (Exception ex)
			{
				Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
				base.TempData["TitleError"] = "Đã có lỗi phát sinh trong phiên làm việc!";
				base.TempData["DetailError"] = ex.Message;
				return RedirectToAction("Index", "Notfound");
			}
		}
	}
}
