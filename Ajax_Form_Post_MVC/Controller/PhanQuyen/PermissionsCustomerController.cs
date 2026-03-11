using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using DatabaseTHP;
using DatabaseTHP.Class;
using DatabaseTHP.Treeview;
using MVC_QuanLyTHP.Class;
using MVC_QuanLyTHP.Models;

namespace MVC_QuanLyTHP.Controllers
{

	public class PermissionsCustomerController : Controller
	{
		public ActionResult Index(string idNhomQuyen = "", string idLichLamViec = "")
		{
			try
			{
				if (Utility.KiemTra())
				{
					return RedirectToAction("Index", "Admin");
				}
				if (!Utility.KiemTraQuyen("PermissionsCustomer", "View"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				string text = "";
				ApiResponse listData = Utility.GetListData<v_v_web_NhomQuyen>("GroupPermissions", "", "", Utility.LOC_ID);
				if (string.IsNullOrEmpty(idNhomQuyen))
				{
					idNhomQuyen = (listData.Data as List<v_v_web_NhomQuyen>).Select((v_v_web_NhomQuyen s) => s.ID).FirstOrDefault();
				}
				ApiResponse listData2 = Utility.GetListData<v_v_dm_LichLamViec>("Calendar", "", "", Utility.LOC_ID);
				if (string.IsNullOrEmpty(idLichLamViec))
				{
					idLichLamViec = (listData2.Data as List<v_v_dm_LichLamViec>).Select((v_v_dm_LichLamViec s) => s.ID).FirstOrDefault();
				}
				if (!string.IsNullOrEmpty(idNhomQuyen) && !string.IsNullOrEmpty(idLichLamViec))
				{
					ApiResponse listData3 = Utility.GetListData<web_PhanQuyenKhuVuc>("PermissionsArea", clsMaHoa.Encrypt("ID_NHOMQUYEN", "tmt6364"), idNhomQuyen, Utility.LOC_ID, "equal");
					List<web_PhanQuyenKhuVuc> lstPhanQuyenNhomSanPham = (listData3.Data as List<web_PhanQuyenKhuVuc>).Where((web_PhanQuyenKhuVuc s) => s.ID_LICHLAMVIEC == idLichLamViec).ToList();
					ApiResponse listData4 = Utility.GetListData<web_PhanQuyenKhachHang>("PermissionsCustomer", clsMaHoa.Encrypt("ID_NHOMQUYEN", "tmt6364"), idNhomQuyen, Utility.LOC_ID, "equal");
					List<web_PhanQuyenKhachHang> lstPhanQuyenSanPham = (listData4.Data as List<web_PhanQuyenKhachHang>).Where((web_PhanQuyenKhachHang s) => s.ID_LICHLAMVIEC == idLichLamViec).ToList();
					ApiResponse listData5 = Utility.GetListData<v_dm_KhachHang>("Customer", "", "", Utility.LOC_ID);
					ApiResponse listData6 = Utility.GetListData<v_dm_KhuVuc>("Area", "", "", Utility.LOC_ID);
					foreach (v_dm_KhuVuc item in listData6.Data as List<v_dm_KhuVuc>)
					{
						text += TreeView(item, listData5.Data as List<v_dm_KhachHang>, lstPhanQuyenNhomSanPham, lstPhanQuyenSanPham);
					}
				}
				base.ViewBag.Treeview = text;
				base.ViewBag.PermissionEdit = Utility.KiemTraQuyen("PermissionsCustomer", "Edit");
				base.ViewBag.PermissionDelete = Utility.KiemTraQuyen("PermissionsCustomer", "Delete");
				base.ViewBag.PermissionCreate = Utility.KiemTraQuyen("PermissionsCustomer", "Create");
				base.ViewBag.showsearchValue = idNhomQuyen;
				base.ViewBag.idLichLamViec = idLichLamViec;
				PhanQuyenKhachHang phanQuyenKhachHang = new PhanQuyenKhachHang();
				phanQuyenKhachHang.lstLichLamViec = (listData2.Data as List<v_v_dm_LichLamViec>).Where((v_v_dm_LichLamViec e) => e.ISACTIVE).ToList();
				phanQuyenKhachHang.lstNhomQuyen = listData.Data as List<v_v_web_NhomQuyen>;
				return View(phanQuyenKhachHang);
			}
			catch (Exception ex)
			{
				Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
				base.TempData["TitleError"] = "Đã có lỗi phát sinh trong phiên làm việc!";
				base.TempData["DetailError"] = ex.Message;
				return RedirectToAction("Index", "Notfound");
			}
		}

		private string TreeView(v_dm_KhuVuc tbl_dept, List<v_dm_KhachHang> lstSanPham, List<web_PhanQuyenKhuVuc> lstPhanQuyenNhomSanPham, List<web_PhanQuyenKhachHang> lstPhanQuyenSanPham)
		{
			string text = "";
			IOrderedEnumerable<v_dm_KhachHang> orderedEnumerable = from s in lstSanPham
																   where s.ID_KHUVUC == tbl_dept.ID
																   orderby s.NAME
																   select s;
			if (orderedEnumerable != null && orderedEnumerable.Count() > 0)
			{
				web_PhanQuyenKhuVuc web_PhanQuyenKhuVuc2 = lstPhanQuyenNhomSanPham.Where((web_PhanQuyenKhuVuc s) => s.ID_KHUVUC == tbl_dept.ID).FirstOrDefault();
				text = text + "<li class=\"TBL_DEPT-" + tbl_dept.ID + "\"><span class=\"caret1\"></span><input type=\"checkbox\" name=\"TBL_DEPT\" id=\"" + tbl_dept.ID + "\" " + ((web_PhanQuyenKhuVuc2 != null && web_PhanQuyenKhuVuc2.ISACTIVE) ? "checked" : "") + " data-id=\"" + tbl_dept.ID + "\">" + ((web_PhanQuyenKhuVuc2 != null && web_PhanQuyenKhuVuc2.ISACTIVE) ? ("<label for=\"tall\" class=\"custom-checked\" style=\"color: #428BCA\">" + tbl_dept.MA + " - " + tbl_dept.NAME + " (" + orderedEnumerable.Count().ToString("N0") + ")</label>") : ("<label for=\"tall\" class=\"custom-unchecked\" style=\"color: #428BCA\">" + tbl_dept.MA + " - " + tbl_dept.NAME + " (" + orderedEnumerable.Count().ToString("N0") + ")</label>")) + "<label>- Khách hàng:</label><input type=\"checkbox\" name=\"TBL_DEPTALL\" onchange=\"checkboxChanged()\" class=\"cbx\" id=\"" + tbl_dept.ID + "\" " + ((web_PhanQuyenKhuVuc2 != null && web_PhanQuyenKhuVuc2.ISPHANQUYENKHUVUC) ? "checked" : "") + " > <ul class=\"nested\">";
				foreach (v_dm_KhachHang itm in orderedEnumerable)
				{
					web_PhanQuyenKhachHang web_PhanQuyenKhachHang2 = lstPhanQuyenSanPham.Where((web_PhanQuyenKhachHang s) => s.ID_KHACHHANG == itm.ID).FirstOrDefault();
					text = text + "<li class=\"licbx\"><input type=\"checkbox\" name=\"TBL_ITEM-" + tbl_dept.ID + "\" id=\"" + itm.ID + "\" onchange=\"checkboxChanged()\" class=\"cbx\" " + ((web_PhanQuyenKhachHang2 != null && web_PhanQuyenKhachHang2.ISACTIVE) ? "checked" : "") + " data-id=\"" + tbl_dept.ID + "\">" + ((web_PhanQuyenKhachHang2 != null && web_PhanQuyenKhachHang2.ISACTIVE) ? ("<label for=\"tall-1\" class=\"custom-checked\">" + itm.MA + " - " + itm.NAME + "</label>") : ("<label for=\"tall-1\" class=\"custom-unchecked\">" + itm.MA + " - " + itm.NAME + "</label>")) + "</li>";
				}
				text += "</li></ul>";
			}
			return text;
		}

		[HttpPost]
		public ActionResult Update(string cartOrder)
		{
			if (Utility.KiemTra())
			{
				return RedirectToAction("Index", "Admin");
			}
			if (!Utility.KiemTraQuyen("PermissionsCustomer", "Edit"))
			{
				base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
				return RedirectToAction("Index", "Notfound");
			}
			if (base.ModelState.IsValid)
			{
				List<Treeview> list = new JavaScriptSerializer().Deserialize<List<Treeview>>(cartOrder);
				foreach (Treeview item in list)
				{
					item.LOC_ID = Utility.LOC_ID;
				}
				ApiResponse apiResponse = Utility.Create(list, "PermissionsCustomer");
				if (apiResponse.Success)
				{
					return Json("Lưu thành công!", JsonRequestBehavior.AllowGet);
				}
				return Json("Lỗi phát sinh!" + apiResponse.Message, JsonRequestBehavior.AllowGet);
			}
			return Json("Lỗi phát sinh", JsonRequestBehavior.AllowGet);
		}
	}
}
