using DatabaseTHP;
using DatabaseTHP.Class;
using DatabaseTHP.StoredProcedure.Parameter;
using DatabaseTHP.StoredProcedure;
using MVC_QuanLyTHP.Class;
using MVC_QuanLyTHP.Models.Order;
using MVC_QuanLyTHP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using DatabaseTHP.Treeview;

namespace MVC_QuanLyTHP.Controllers
{
    public class PermissionsController : Controller
    {
        // GET: PermissionsCustomer
        public ActionResult Index(string idNhomQuyen = "")
        {
            try
            {
                if (Utility.KiemTra())
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (!Utility.KiemTraQuyen(API.web_PhanQuyen, API.Xem))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                string Treeview = "";
                var lstNhomQuyen = Utility.GetListData<v_v_web_NhomQuyen>(API.web_NhomQuyen, "", "", Utility.LOC_ID);
                if (string.IsNullOrEmpty(idNhomQuyen))
                    idNhomQuyen = (lstNhomQuyen.Data as List<v_v_web_NhomQuyen>).Select(s => s.ID).FirstOrDefault();
                var lstweb_Menu = Utility.GetListData<web_Menu>(API.web_Menu);
                var lstweb_Quyen = Utility.GetListData<web_Quyen>(API.web_Quyen, "", "", Utility.LOC_ID);
                var lstweb_PhanQuyen = Utility.GetListData<web_PhanQuyen>(API.web_PhanQuyen, clsMaHoa.Encrypt("ID_NHOMQUYEN", clsMaHoa.PassMaHoa), idNhomQuyen, Utility.LOC_ID, "equal");

                var lstMenuCha = (lstweb_Menu.Data as List<web_Menu>).Where(s => string.IsNullOrEmpty(s.ID_QUYENCHA)).OrderBy(s => s.STT);

                foreach (web_Menu itmCha in lstMenuCha)
                {

                    var lstMenu = (lstweb_Menu.Data as List<web_Menu>).Where(s => s.ID_QUYENCHA == itmCha.ID).OrderBy(s => s.STT);
                    if (lstMenu != null && lstMenu.Count() > 0)
                    {
                        Treeview += " <ul class=\"treeview\" id=\"treeview\"> <label class=\"control-label\">" + itmCha.NAME + ":</label>";
                    }
                    foreach (web_Menu itm in lstMenu)
                    {
                        if (!string.IsNullOrEmpty(itm.CONTROLLERNAME) && (!string.IsNullOrEmpty(itm.ACTIONNAME)) && itm.CONTROLLERNAME != API.BaoCao)
                        {
                            Treeview += TreeView(itm, lstweb_Quyen.Data as List<web_Quyen>, lstweb_PhanQuyen.Data as List<web_PhanQuyen>);
                        }
                        else
                        {
                            var lstMenuCha1 = (lstweb_Menu.Data as List<web_Menu>).Where(s => s.ID_QUYENCHA == itm.ID).OrderBy(s => s.STT);
                            foreach (web_Menu itmCha1 in lstMenuCha1)
                            {
                                Treeview += TreeView(itmCha1, lstweb_Quyen.Data as List<web_Quyen>, lstweb_PhanQuyen.Data as List<web_PhanQuyen>);
                            }
                        }
                    }
                    if (lstMenu != null && lstMenu.Count() > 0)
                    {
                        Treeview += "</ul>";
                    }
                }

                ViewBag.Treeview = Treeview;

                ViewBag.PermissionEdit = Utility.KiemTraQuyen(API.web_PhanQuyen, API.Edit);
                ViewBag.PermissionDelete = Utility.KiemTraQuyen(API.web_PhanQuyen, API.Delete);
                ViewBag.PermissionCreate = Utility.KiemTraQuyen(API.web_PhanQuyen, API.Create);
                ViewBag.showsearchValue = idNhomQuyen;
                return View(lstNhomQuyen.Data as List<v_v_web_NhomQuyen>);
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
                TempData["TitleError"] = API.TitleTryCatch;
                TempData["DetailError"] = ex.Message;
                return RedirectToAction("Index", "Notfound");
            }
        }

        private string TreeView(web_Menu tbl_dept, List<web_Quyen> lstQuyen, List<web_PhanQuyen> lstweb_PhanQuyen)
        {
            string Treeview = "";
            var lstSP = lstQuyen.Where(s => s.ID_MENU == tbl_dept.ID).OrderBy(s => s.MAQUYEN);
            if (lstSP != null && lstSP.Count() > 0)
            {
                var web_PhanQuyenNhomSanPham = lstQuyen.Where(s => s.ID_MENU == tbl_dept.ID).Select(s => s.ID);
                int intCout = 0;
                if(web_PhanQuyenNhomSanPham != null)
                    intCout = lstweb_PhanQuyen.Where(s => web_PhanQuyenNhomSanPham.Contains(s.ID_QUYEN) && s.ISACTIVE).Count();
                Treeview += "<li class=\"TBL_DEPT-" + tbl_dept.ID + "\">" +
                            "<span class=\"caret1\"></span>" +
                            "<input type=\"checkbox\" name=\"TBL_DEPT\" onchange=\"checkboxChanged()\" class=\"cbx\" id=\"" + tbl_dept.ID + "\" " + (intCout > 0 ? "checked" : "") + " data-id=\"" + tbl_dept.ID + "\">" +
                            (intCout > 0  ? "<label for=\"tall\" class=\"custom-checked\" style=\"color: #428BCA\">" + tbl_dept.CONTROLLERNAME + " - " + tbl_dept.NAME + "</label>" : "<label for=\"tall\" class=\"custom-unchecked\" style=\"color: #428BCA\">" + (tbl_dept.CONTROLLERNAME) + " - " + (tbl_dept.NAME) + "</label>") +
                            "<ul class=\"nested\">";
                foreach (web_Quyen itm in lstSP)
                {
                    var web_PhanQuyenSanPham = lstweb_PhanQuyen.Where(s => s.ID_QUYEN == itm.ID).FirstOrDefault();
                    Treeview += "<li class=\"licbx\">" +
                                "<input type=\"checkbox\" name=\"TBL_ITEM-" + tbl_dept.ID + "\" id=\"" + itm.ID + "\" onchange=\"checkboxChanged()\" class=\"cbx\" " + (web_PhanQuyenSanPham != null && web_PhanQuyenSanPham.ISACTIVE ? "checked" : "") + " data-id=\"" + tbl_dept.ID + "\">" +
                                 (web_PhanQuyenSanPham != null && web_PhanQuyenSanPham.ISACTIVE ? "<label for=\"tall-1\" class=\"custom-checked\">" + itm.MAQUYEN + " - " + itm.TENQUYEN + "</label>" : "<label for=\"tall-1\" class=\"custom-unchecked\">" + (itm.MAQUYEN) + " - " + (itm.TENQUYEN) + "</label>") +
                                "</li>";
                }
                Treeview += "</li>" +
                            "</ul>";
            }
            return Treeview;
        }

        [HttpPost]
        public ActionResult Update(String cartOrder)
        {
            if (Utility.KiemTra())
            {
                return RedirectToAction("Index", "Admin");
            }
            if (!Utility.KiemTraQuyen(API.web_PhanQuyen, API.Edit))
            {
                TempData["TitleError"] = API.TitlePermission;
                return RedirectToAction("Index", "Notfound");
            }
            if (ModelState.IsValid)
            {
                var lstcartOrder = new JavaScriptSerializer().Deserialize<List<Treeview>>(cartOrder);
                foreach (Treeview itm in lstcartOrder)
                    itm.LOC_ID = Utility.LOC_ID;
                var apiResponse = Utility.Create<List<Treeview>>(lstcartOrder, API.web_PhanQuyen);
                if (apiResponse.Success)
                    return Json("Lưu thành công!", JsonRequestBehavior.AllowGet);
                else
                    return Json("Lỗi phát sinh!" + apiResponse.Message, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("Lỗi phát sinh", JsonRequestBehavior.AllowGet);
            }
        }
    }
}