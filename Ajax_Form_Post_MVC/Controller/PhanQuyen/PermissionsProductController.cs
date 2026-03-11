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

	public class PermissionsProductController : Controller
	{
		public ActionResult Index(string idNhomQuyen = "")
		{
			try
			{
				if (Utility.KiemTra())
				{
					return RedirectToAction("Index", "Admin");
				}
				if (!Utility.KiemTraQuyen("PermissionsProduct", "View"))
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
				ApiResponse listData2 = Utility.GetListData<web_PhanQuyenNhomSanPham>("PermissionsGroupProduct", clsMaHoa.Encrypt("ID_NHOMQUYEN", "tmt6364"), idNhomQuyen, Utility.LOC_ID, "equal");
				ApiResponse listData3 = Utility.GetListData<web_PhanQuyenSanPham>("PermissionsProduct", clsMaHoa.Encrypt("ID_NHOMQUYEN", "tmt6364"), idNhomQuyen, Utility.LOC_ID, "equal");
				ApiResponse listData4 = Utility.GetListData<v_dm_HangHoa>("Product", "", "", Utility.LOC_ID);
				ApiResponse listData5 = Utility.GetListData<v_dm_NhomHangHoa>("GroupProduct", "", "", Utility.LOC_ID);
				foreach (v_dm_NhomHangHoa item in (listData5.Data as List<v_dm_NhomHangHoa>).OrderBy((v_dm_NhomHangHoa s) => s.NAME))
				{
					text += TreeView(item, listData4.Data as List<v_dm_HangHoa>, listData2.Data as List<web_PhanQuyenNhomSanPham>, listData3.Data as List<web_PhanQuyenSanPham>);
				}
				base.ViewBag.Treeview = text;
				base.ViewBag.PermissionEdit = Utility.KiemTraQuyen("PermissionsProduct", "Edit");
				base.ViewBag.PermissionDelete = Utility.KiemTraQuyen("PermissionsProduct", "Delete");
				base.ViewBag.PermissionCreate = Utility.KiemTraQuyen("PermissionsProduct", "Create");
				base.ViewBag.showsearchValue = idNhomQuyen;
				return View(listData.Data as List<v_v_web_NhomQuyen>);
			}
			catch (Exception ex)
			{
				Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
				base.TempData["TitleError"] = "Đã có lỗi phát sinh trong phiên làm việc!";
				base.TempData["DetailError"] = ex.Message;
				return RedirectToAction("Index", "Notfound");
			}
		}

		private string TreeView(v_dm_NhomHangHoa tbl_dept, List<v_dm_HangHoa> lstSanPham, List<web_PhanQuyenNhomSanPham> lstPhanQuyenNhomSanPham, List<web_PhanQuyenSanPham> lstPhanQuyenSanPham)
		{
			string text = "";
			IEnumerable<v_dm_HangHoa> enumerable = lstSanPham.Where((v_dm_HangHoa s) => s.ID_NHOMHANGHOA == tbl_dept.ID);
			if (enumerable != null && enumerable.Count() > 0)
			{
				web_PhanQuyenNhomSanPham web_PhanQuyenNhomSanPham2 = lstPhanQuyenNhomSanPham.Where((web_PhanQuyenNhomSanPham s) => s.ID_NHOMSANPHAM == tbl_dept.ID).FirstOrDefault();
				text = text + "<li class=\"TBL_DEPT-" + tbl_dept.ID + "\"><span class=\"caret1\"></span><input type=\"checkbox\" name=\"TBL_DEPT\" id=\"" + tbl_dept.ID + "\"   " + ((web_PhanQuyenNhomSanPham2 != null && web_PhanQuyenNhomSanPham2.ISACTIVE) ? "checked" : "") + " data-id=\"" + tbl_dept.ID + "\">" + ((web_PhanQuyenNhomSanPham2 != null && web_PhanQuyenNhomSanPham2.ISACTIVE) ? ("<label for=\"tall\" class=\"custom-checked\" style=\"color: #428BCA\">" + tbl_dept.MA + " - " + tbl_dept.NAME + " (" + enumerable.Count().ToString("N0") + ")</label>") : ("<label for=\"tall\" class=\"custom-unchecked\"  style=\"color: #428BCA\">" + tbl_dept.MA + " - " + tbl_dept.NAME + " (" + enumerable.Count().ToString("N0") + ")</label>")) + "<label>- Sản phẩm:</label><input type=\"checkbox\" class=\"cbx\" onchange=\"checkboxChanged()\" name=\"TBL_DEPTALL\" id=\"" + tbl_dept.ID + "\" " + ((web_PhanQuyenNhomSanPham2 != null && web_PhanQuyenNhomSanPham2.ISPHANQUYENSANPHAM) ? "checked" : "") + " > <ul class=\"nested\">";
				foreach (v_dm_HangHoa itm in enumerable)
				{
					web_PhanQuyenSanPham web_PhanQuyenSanPham2 = lstPhanQuyenSanPham.Where((web_PhanQuyenSanPham s) => s.ID_SANPHAM == itm.ID).FirstOrDefault();
					text = text + "<li class=\"licbx\"><input type=\"checkbox\" name=\"TBL_ITEM-" + tbl_dept.ID + "\" id=\"" + itm.ID + "\" onchange=\"checkboxChanged()\" class=\"cbx\" " + ((web_PhanQuyenSanPham2 != null && web_PhanQuyenSanPham2.ISACTIVE) ? "checked" : "") + " data-id=\"" + tbl_dept.ID + "\">" + ((web_PhanQuyenSanPham2 != null && web_PhanQuyenSanPham2.ISACTIVE) ? ("<label for=\"tall-1\" class=\"custom-checked\">" + itm.MA + " - " + itm.NAME + "</label>") : ("<label for=\"tall-1\" class=\"custom-unchecked\">" + itm.MA + " - " + itm.NAME + "</label>")) + "</li>";
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
			if (!Utility.KiemTraQuyen("PermissionsProduct", "Edit"))
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
				ApiResponse apiResponse = Utility.Create(list, "PermissionsProduct");
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
