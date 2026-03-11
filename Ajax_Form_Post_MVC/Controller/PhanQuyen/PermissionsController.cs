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

	public class PermissionsController : Controller
	{
		public ActionResult Index(string idNhomQuyen = "")
		{
			try
			{
				if (Utility.KiemTra())
				{
					return RedirectToAction("Index", "Admin");
				}
				if (!Utility.KiemTraQuyen("Permissions", "View"))
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
				ApiResponse listData2 = Utility.GetListData<web_Menu>("Menu");
				ApiResponse listData3 = Utility.GetListData<web_Quyen>("Quyen", "", "", Utility.LOC_ID);
				ApiResponse listData4 = Utility.GetListData<web_PhanQuyen>("Permissions", clsMaHoa.Encrypt("ID_NHOMQUYEN", "tmt6364"), idNhomQuyen, Utility.LOC_ID, "equal");
				IOrderedEnumerable<web_Menu> orderedEnumerable = from s in listData2.Data as List<web_Menu>
																 where string.IsNullOrEmpty(s.ID_QUYENCHA)
																 orderby s.STT
																 select s;
				foreach (web_Menu itmCha in orderedEnumerable)
				{
					IOrderedEnumerable<web_Menu> orderedEnumerable2 = from s in listData2.Data as List<web_Menu>
																	  where s.ID_QUYENCHA == itmCha.ID
																	  orderby s.STT
																	  select s;
					if (orderedEnumerable2 != null && orderedEnumerable2.Count() > 0)
					{
						text = text + " <ul class=\"treeview\" id=\"treeview\"> <label class=\"control-label\">" + itmCha.NAME + ":</label>";
					}
					foreach (web_Menu itm in orderedEnumerable2)
					{
						if (!string.IsNullOrEmpty(itm.CONTROLLERNAME) && !string.IsNullOrEmpty(itm.ACTIONNAME) && itm.CONTROLLERNAME != "ViewReport")
						{
							text += TreeView(itm, listData3.Data as List<web_Quyen>, listData4.Data as List<web_PhanQuyen>);
							continue;
						}
						IOrderedEnumerable<web_Menu> orderedEnumerable3 = from s in listData2.Data as List<web_Menu>
																		  where s.ID_QUYENCHA == itm.ID
																		  orderby s.STT
																		  select s;
						foreach (web_Menu item in orderedEnumerable3)
						{
							text += TreeView(item, listData3.Data as List<web_Quyen>, listData4.Data as List<web_PhanQuyen>);
						}
					}
					if (orderedEnumerable2 != null && orderedEnumerable2.Count() > 0)
					{
						text += "</ul>";
					}
				}
				base.ViewBag.Treeview = text;
				base.ViewBag.PermissionEdit = Utility.KiemTraQuyen("Permissions", "Edit");
				base.ViewBag.PermissionDelete = Utility.KiemTraQuyen("Permissions", "Delete");
				base.ViewBag.PermissionCreate = Utility.KiemTraQuyen("Permissions", "Create");
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

		private string TreeView(web_Menu tbl_dept, List<web_Quyen> lstQuyen, List<web_PhanQuyen> lstweb_PhanQuyen)
		{
			string text = "";
			IOrderedEnumerable<web_Quyen> orderedEnumerable = from s in lstQuyen
															  where s.ID_MENU == tbl_dept.ID
															  orderby s.MAQUYEN
															  select s;
			if (orderedEnumerable != null && orderedEnumerable.Count() > 0)
			{
				IEnumerable<string> web_PhanQuyenNhomSanPham2 = from s in lstQuyen
																where s.ID_MENU == tbl_dept.ID
																select s.ID;
				int num = 0;
				if (web_PhanQuyenNhomSanPham2 != null)
				{
					num = lstweb_PhanQuyen.Where((web_PhanQuyen s) => web_PhanQuyenNhomSanPham2.Contains(s.ID_QUYEN) && s.ISACTIVE).Count();
				}
				text = text + "<li class=\"TBL_DEPT-" + tbl_dept.ID + "\"><span class=\"caret1\"></span><input type=\"checkbox\" name=\"TBL_DEPT\" onchange=\"checkboxChanged()\" class=\"cbx\" id=\"" + tbl_dept.ID + "\" " + ((num > 0) ? "checked" : "") + " data-id=\"" + tbl_dept.ID + "\">" + ((num > 0) ? ("<label for=\"tall\" class=\"custom-checked\" style=\"color: #428BCA\">" + tbl_dept.CONTROLLERNAME + " - " + tbl_dept.NAME + "</label>") : ("<label for=\"tall\" class=\"custom-unchecked\" style=\"color: #428BCA\">" + tbl_dept.CONTROLLERNAME + " - " + tbl_dept.NAME + "</label>")) + "<ul class=\"nested\">";
				foreach (web_Quyen itm in orderedEnumerable)
				{
					web_PhanQuyen web_PhanQuyen2 = lstweb_PhanQuyen.Where((web_PhanQuyen s) => s.ID_QUYEN == itm.ID).FirstOrDefault();
					text = text + "<li class=\"licbx\"><input type=\"checkbox\" name=\"TBL_ITEM-" + tbl_dept.ID + "\" id=\"" + itm.ID + "\" onchange=\"checkboxChanged()\" class=\"cbx\" " + ((web_PhanQuyen2 != null && web_PhanQuyen2.ISACTIVE) ? "checked" : "") + " data-id=\"" + tbl_dept.ID + "\">" + ((web_PhanQuyen2 != null && web_PhanQuyen2.ISACTIVE) ? ("<label for=\"tall-1\" class=\"custom-checked\">" + itm.MAQUYEN + " - " + itm.TENQUYEN + "</label>") : ("<label for=\"tall-1\" class=\"custom-unchecked\">" + itm.MAQUYEN + " - " + itm.TENQUYEN + "</label>")) + "</li>";
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
			if (!Utility.KiemTraQuyen("Permissions", "Edit"))
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
				ApiResponse apiResponse = Utility.Create(list, "Permissions");
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
