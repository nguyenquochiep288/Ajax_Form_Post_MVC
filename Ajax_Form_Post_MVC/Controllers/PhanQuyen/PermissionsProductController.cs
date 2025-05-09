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
    public class PermissionsProductController : Controller
    {
        // GET: PermissionsProduct
        public ActionResult Index(string idNhomQuyen = "")
        {
            try
            {
                if (Utility.KiemTra())
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (!Utility.KiemTraQuyen(API.web_PhanQuyenSanPham, API.Xem))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                string Treeview = "";
                var lstNhomQuyen = Utility.GetListData<v_v_web_NhomQuyen>(API.web_NhomQuyen, "", "", Utility.LOC_ID);
                if (string.IsNullOrEmpty(idNhomQuyen))
                    idNhomQuyen = (lstNhomQuyen.Data as List<v_v_web_NhomQuyen>).Select(s => s.ID).FirstOrDefault();
                var lstPhanQuyenNhomSanPham = Utility.GetListData<web_PhanQuyenNhomSanPham>(API.web_PhanQuyenNhomSanPham, clsMaHoa.Encrypt("ID_NHOMQUYEN", clsMaHoa.PassMaHoa), idNhomQuyen, Utility.LOC_ID, "equal");
                var lstPhanQuyenSanPham = Utility.GetListData<web_PhanQuyenSanPham>(API.web_PhanQuyenSanPham, clsMaHoa.Encrypt("ID_NHOMQUYEN", clsMaHoa.PassMaHoa), idNhomQuyen, Utility.LOC_ID, "equal");
                var lstSanPham = Utility.GetListData<v_dm_HangHoa>(API.dm_HangHoa, "", "", Utility.LOC_ID);
                var lstNhomSanPham = Utility.GetListData<v_dm_NhomHangHoa>(API.dm_NhomHangHoa, "", "", Utility.LOC_ID);

                foreach (v_dm_NhomHangHoa itm in (lstNhomSanPham.Data as List<v_dm_NhomHangHoa>).OrderBy(s => s.NAME))
                {
                    Treeview += TreeView(itm, lstSanPham.Data as List<v_dm_HangHoa>, lstPhanQuyenNhomSanPham.Data as List<web_PhanQuyenNhomSanPham>, lstPhanQuyenSanPham.Data as List<web_PhanQuyenSanPham>);
                }    
                 
                ViewBag.Treeview = Treeview;

                ViewBag.PermissionEdit = Utility.KiemTraQuyen(API.web_PhanQuyenSanPham, API.Edit);
                ViewBag.PermissionDelete = Utility.KiemTraQuyen(API.web_PhanQuyenSanPham, API.Delete);
                ViewBag.PermissionCreate = Utility.KiemTraQuyen(API.web_PhanQuyenSanPham, API.Create);
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

        private string TreeView(v_dm_NhomHangHoa tbl_dept, List<v_dm_HangHoa> lstSanPham, List<web_PhanQuyenNhomSanPham> lstPhanQuyenNhomSanPham, List<web_PhanQuyenSanPham> lstPhanQuyenSanPham)
        {
            string Treeview = "";
            var lstSP = lstSanPham.Where(s => s.ID_NHOMHANGHOA == tbl_dept.ID);
            if (lstSP != null && lstSP.Count() > 0)
            {
                var web_PhanQuyenNhomSanPham = lstPhanQuyenNhomSanPham.Where(s => s.ID_NHOMSANPHAM == tbl_dept.ID).FirstOrDefault();
                Treeview += "<li class=\"TBL_DEPT-" + tbl_dept.ID + "\">" +
                            "<span class=\"caret1\"></span>" +
                            "<input type=\"checkbox\" name=\"TBL_DEPT\" id=\"" + tbl_dept.ID + "\"   " + (web_PhanQuyenNhomSanPham != null && web_PhanQuyenNhomSanPham.ISACTIVE ? "checked" : "") + " data-id=\"" + tbl_dept.ID + "\">" +
                            (web_PhanQuyenNhomSanPham != null && web_PhanQuyenNhomSanPham.ISACTIVE ? "<label for=\"tall\" class=\"custom-checked\" style=\"color: #428BCA\">" + (tbl_dept.MA) + " - " + (tbl_dept.NAME) + " (" + lstSP.Count().ToString("N0") + ")</label>" : "<label for=\"tall\" class=\"custom-unchecked\"  style=\"color: #428BCA\">" + (tbl_dept.MA) + " - " + (tbl_dept.NAME) + " (" + lstSP.Count().ToString("N0") + ")</label>" ) +
                             "<label>- Sản phẩm:</label><input type=\"checkbox\" class=\"cbx\" onchange=\"checkboxChanged()\" name=\"TBL_DEPTALL\" id=\"" + tbl_dept.ID + "\" " + (web_PhanQuyenNhomSanPham != null && web_PhanQuyenNhomSanPham.ISPHANQUYENSANPHAM ? "checked" : "") + " > " +
                            "<ul class=\"nested\">";
                foreach (v_dm_HangHoa itm in lstSP)
                {
                    var web_PhanQuyenSanPham = lstPhanQuyenSanPham.Where(s => s.ID_SANPHAM == itm.ID).FirstOrDefault();
                    Treeview += "<li class=\"licbx\">" +
                                "<input type=\"checkbox\" name=\"TBL_ITEM-" + tbl_dept.ID + "\" id=\"" + itm.ID + "\" onchange=\"checkboxChanged()\" class=\"cbx\" " + (web_PhanQuyenSanPham != null && web_PhanQuyenSanPham.ISACTIVE ? "checked" : "") + " data-id=\"" + tbl_dept.ID + "\">" +
                                 (web_PhanQuyenSanPham != null && web_PhanQuyenSanPham.ISACTIVE ? "<label for=\"tall-1\" class=\"custom-checked\">" + (itm.MA) + " - " + (itm.NAME) + "</label>" : "<label for=\"tall-1\" class=\"custom-unchecked\">" + (itm.MA) + " - " + (itm.NAME) + "</label>") +
                                "</li>";
                }
                Treeview += "</li>"+
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
            if (!Utility.KiemTraQuyen(API.web_PhanQuyenSanPham, API.Edit))
            {
                TempData["TitleError"] = API.TitlePermission;
                return RedirectToAction("Index", "Notfound");
            }
            if (ModelState.IsValid)
            {
                var lstcartOrder = new JavaScriptSerializer().Deserialize<List<Treeview>>(cartOrder);
                foreach (Treeview itm in lstcartOrder)
                    itm.LOC_ID = Utility.LOC_ID;
                var apiResponse = Utility.Create<List<Treeview>>(lstcartOrder, API.web_PhanQuyenSanPham);
                if(apiResponse.Success)
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