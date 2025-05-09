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
    public class PermissionsCustomerController : Controller
    {
        // GET: PermissionsCustomer
        public ActionResult Index(string idNhomQuyen = "", string idLichLamViec = "")
        {
            try
            {
                if (Utility.KiemTra())
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (!Utility.KiemTraQuyen(API.web_PhanQuyenKhachHang, API.Xem))
                {
                    TempData["TitleError"] = API.TitlePermission;
                    return RedirectToAction("Index", "Notfound");
                }
                string Treeview = "";
                var lstNhomQuyen = Utility.GetListData<v_v_web_NhomQuyen>(API.web_NhomQuyen, "", "", Utility.LOC_ID);
                if (string.IsNullOrEmpty(idNhomQuyen))
                    idNhomQuyen = (lstNhomQuyen.Data as List<v_v_web_NhomQuyen>).Select(s => s.ID).FirstOrDefault();

                var lstLichLamViec = Utility.GetListData<v_v_dm_LichLamViec>(API.dm_LichLamViec, "", "", Utility.LOC_ID);
                if (string.IsNullOrEmpty(idLichLamViec))
                    idLichLamViec = (lstLichLamViec.Data as List<v_v_dm_LichLamViec>).Select(s => s.ID).FirstOrDefault();
                if(!string.IsNullOrEmpty(idNhomQuyen) && !string.IsNullOrEmpty(idLichLamViec))
                {
                    var ApiResponsePhanQuyenNhomKhachHang = Utility.GetListData<web_PhanQuyenKhuVuc>(API.web_PhanQuyenKhuVuc, clsMaHoa.Encrypt("ID_NHOMQUYEN", clsMaHoa.PassMaHoa), idNhomQuyen, Utility.LOC_ID, "equal");
                    var lstPhanQuyenNhomKhachHang = (ApiResponsePhanQuyenNhomKhachHang.Data as List<web_PhanQuyenKhuVuc>).Where(s => s.ID_LICHLAMVIEC == idLichLamViec).ToList();
                    var ApiResponsePhanQuyenKhachHang = Utility.GetListData<web_PhanQuyenKhachHang>(API.web_PhanQuyenKhachHang, clsMaHoa.Encrypt("ID_NHOMQUYEN", clsMaHoa.PassMaHoa), idNhomQuyen, Utility.LOC_ID, "equal");
                    var lstPhanQuyenKhachHang = (ApiResponsePhanQuyenKhachHang.Data as List<web_PhanQuyenKhachHang>).Where(s => s.ID_LICHLAMVIEC == idLichLamViec).ToList();
                    var lstSanPham = Utility.GetListData<v_dm_KhachHang>(API.dm_KhachHang, "", "", Utility.LOC_ID);
                    var lstTBL_GROUP_CUS_VENDOR = Utility.GetListData<v_dm_KhuVuc>(API.dm_KhuVuc, "", "", Utility.LOC_ID);

                    foreach (v_dm_KhuVuc itm in (lstTBL_GROUP_CUS_VENDOR.Data as List<v_dm_KhuVuc>))
                    {
                        Treeview += TreeView(itm, lstSanPham.Data as List<v_dm_KhachHang>, lstPhanQuyenNhomKhachHang, lstPhanQuyenKhachHang);
                    }
                }
                ViewBag.Treeview = Treeview;

                ViewBag.PermissionEdit = Utility.KiemTraQuyen(API.web_PhanQuyenKhachHang, API.Edit);
                ViewBag.PermissionDelete = Utility.KiemTraQuyen(API.web_PhanQuyenKhachHang, API.Delete);
                ViewBag.PermissionCreate = Utility.KiemTraQuyen(API.web_PhanQuyenKhachHang, API.Create);
                ViewBag.showsearchValue = idNhomQuyen;
                ViewBag.idLichLamViec = idLichLamViec;
                PhanQuyenKhachHang PhanQuyenKhachHang = new PhanQuyenKhachHang();
                PhanQuyenKhachHang.lstLichLamViec = (lstLichLamViec.Data as List<v_v_dm_LichLamViec>).Where(e => e.ISACTIVE).ToList();
                PhanQuyenKhachHang.lstNhomQuyen = (lstNhomQuyen.Data as List<v_v_web_NhomQuyen>);
                return View(PhanQuyenKhachHang);
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
                TempData["TitleError"] = API.TitleTryCatch;
                TempData["DetailError"] = ex.Message;
                return RedirectToAction("Index", "Notfound");
            }
        }

        private string TreeView(v_dm_KhuVuc tbl_dept, List<v_dm_KhachHang> lstSanPham, List<web_PhanQuyenKhuVuc> lstPhanQuyenNhomSanPham, List<web_PhanQuyenKhachHang> lstPhanQuyenSanPham)
        {
            string Treeview = "";
            var lstSP = lstSanPham.Where(s => s.ID_KHUVUC == tbl_dept.ID).OrderBy(s => s.NAME);
            if (lstSP != null && lstSP.Count() > 0)
            {
                var web_PhanQuyenNhomSanPham = lstPhanQuyenNhomSanPham.Where(s => s.ID_KHUVUC == tbl_dept.ID).FirstOrDefault();
                Treeview += "<li class=\"TBL_DEPT-" + tbl_dept.ID + "\">" +
                            "<span class=\"caret1\"></span>" +
                            "<input type=\"checkbox\" name=\"TBL_DEPT\" id=\"" + tbl_dept.ID + "\" " + (web_PhanQuyenNhomSanPham != null && web_PhanQuyenNhomSanPham.ISACTIVE ? "checked" : "") + " data-id=\"" + tbl_dept.ID + "\">" +
                            (web_PhanQuyenNhomSanPham != null && web_PhanQuyenNhomSanPham.ISACTIVE ? "<label for=\"tall\" class=\"custom-checked\" style=\"color: #428BCA\">" + (tbl_dept.MA) + " - "  + (tbl_dept.NAME) + " ("+ lstSP.Count().ToString("N0") + ")</label>" : "<label for=\"tall\" class=\"custom-unchecked\" style=\"color: #428BCA\">" + (tbl_dept.MA) + " - " + (tbl_dept.NAME) + " (" + lstSP.Count().ToString("N0") + ")</label>" ) +
                             "<label>- Khách hàng:</label><input type=\"checkbox\" name=\"TBL_DEPTALL\" onchange=\"checkboxChanged()\" class=\"cbx\" id=\"" + tbl_dept.ID + "\" " + (web_PhanQuyenNhomSanPham != null && web_PhanQuyenNhomSanPham.ISPHANQUYENKHUVUC ? "checked" : "") + " > " +
                            "<ul class=\"nested\">";
                foreach (v_dm_KhachHang itm in lstSP)
                {
                    var web_PhanQuyenSanPham = lstPhanQuyenSanPham.Where(s => s.ID_KHACHHANG == itm.ID).FirstOrDefault();
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
            if (!Utility.KiemTraQuyen(API.web_PhanQuyenKhachHang, API.Edit))
            {
                TempData["TitleError"] = API.TitlePermission;
                return RedirectToAction("Index", "Notfound");
            }
            if (ModelState.IsValid)
            {
                var lstcartOrder = new JavaScriptSerializer().Deserialize<List<Treeview>>(cartOrder);
                foreach (Treeview itm in lstcartOrder)
                    itm.LOC_ID = Utility.LOC_ID;
                var apiResponse = Utility.Create<List<Treeview>>(lstcartOrder, API.web_PhanQuyenKhachHang);
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