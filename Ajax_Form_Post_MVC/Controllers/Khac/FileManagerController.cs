using System.Linq;
using System.Web.Mvc;
using System.Configuration;
using DatabaseTHP;
using DatabaseTHP.Class;
using MVC_QuanLyTHP.Class;
using MVC_QuanLyTHP.Models;
using System.Collections.Generic;
using System.Reflection;
using System;
using System.IO;

namespace MVC_QuanLyTHP.Controllers
{
    public class FileManagerController : Controller
    {
        public ActionResult Image_Input()
        {
            return PartialView();
        }

        [HttpPost]
        public ActionResult AddFileManager_Image([Bind(Include = "URL_IMAGE,NGAYTAO,ID_PHIEUGIAOHANG,ID_PHIEUXUAT")] v_ct_PhieuGiaoHang_HinhAnh dm_HangHoa)
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                if (Utility.KiemTra())
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (ModelState.IsValid)
                {
                    dm_HangHoa.LOC_ID = Utility.LOC_ID;
                    dm_HangHoa.ID = Guid.NewGuid().ToString();
                    dm_HangHoa.ID_NGUOITAO = Session[Sessions.idUser].ToString();
                    dm_HangHoa.THOIGIANTHEM = Utility.CurrentTime;
                    //@ConvertObjectUnicodeToTCVN3
                    if (Request.Files["MaHinh"] != null)//Nếu có uploads
                    {
                        String fulName = Request.Files["MaHinh"].FileName;
                        if (fulName != "")
                        {
                            String Name = dm_HangHoa.ID.ToString() + "." + fulName.Split('.')[1];
                            String path = API.PathDelivery_Image + dm_HangHoa.ID_NGUOITAO + "/" + dm_HangHoa.ID_PHIEUXUAT + "/";
                            String fullpath = Path.Combine(Server.MapPath("~" + path), Name);
                            if (!System.IO.Directory.Exists(Server.MapPath("~" + path)))
                            {
                                System.IO.Directory.CreateDirectory(Server.MapPath("~" + path));
                            }
                            Request.Files["MaHinh"].SaveAs(fullpath);
                            dm_HangHoa.URL_IMAGE = path + Name;//cập nhật tên file ảnh
                            Byte[] AsBytes = System.IO.File.ReadAllBytes(fullpath);
                            String AsBase64String = Convert.ToBase64String(AsBytes);
                            //dm_HangHoa.FILEBASE64 = AsBase64String;
                        }
                    }
                    apiResponse = Utility.Create<v_ct_PhieuGiaoHang_HinhAnh>(dm_HangHoa, API.ct_PhieuGiaoHang_HinhAnh);


                    apiResponse = Utility.GetDetail<List<v_ct_PhieuGiaoHang_HinhAnh>>(Utility.LOC_ID + "/" + dm_HangHoa.ID_PHIEUXUAT, API.ct_PhieuGiaoHang_HinhAnh);
                    if (!apiResponse.Success)
                    {
                        TempData["TitleError"] = apiResponse.Message;
                        apiResponse.Success = false;
                        apiResponse.URL = Url.Action("Index", "Notfound");
                        return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                    }

                    List<v_ct_PhieuGiaoHang_HinhAnh> lstct_PhieuGiaoHang = new List<v_ct_PhieuGiaoHang_HinhAnh>();
                    if (apiResponse.Data != null)
                        lstct_PhieuGiaoHang = apiResponse.Data as List<v_ct_PhieuGiaoHang_HinhAnh>;

                    foreach (v_ct_PhieuGiaoHang_HinhAnh itm in lstct_PhieuGiaoHang)
                    {
                        var lst = itm.URL_IMAGE.Split('/');
                        string url = Request.Url.Authority;
                        string URL_IMAGE = "";
                        if (Request.Url.AbsoluteUri.StartsWith("https"))
                            URL_IMAGE = "https://" + url + itm.URL_IMAGE;
                        else
                            URL_IMAGE = "http://" + url + itm.URL_IMAGE;
                        apiResponse.CONTENT += "<div class='col-xs-6 col-sm-4 col-md-3 image'>";
                        apiResponse.CONTENT += "<div class='thmb'>";
                        apiResponse.CONTENT += "<div class='ckbox ckbox-default'>";
                        apiResponse.CONTENT += "</div>";
                        apiResponse.CONTENT += "<div class='btn-group fm-group'>";
                        apiResponse.CONTENT += "</div><!-- btn-group -->";
                        apiResponse.CONTENT += "<div class='thmb-prev'>";
                        apiResponse.CONTENT += "<a href='" + URL_IMAGE + "' data-rel='prettyPhoto'>";
                        apiResponse.CONTENT += "<img src='" + URL_IMAGE + "' class='img-responsive' alt='' />";
                        apiResponse.CONTENT += "</a>";
                        apiResponse.CONTENT += "</div>";
                        apiResponse.CONTENT += "<h5 class='fm-title'><a href='#'>" + itm.NAME_NGUOITAO + "</a></h5> ";
                        apiResponse.CONTENT += "<small class='text-muted'>" + itm.NGAYTAO.ToString("dd/MM/yyyy HH:mm") + "</small>";
                        apiResponse.CONTENT += "</div><!-- thmb -->";
                        apiResponse.CONTENT += "</div><!-- col-xs-6 -->";
                    }
                }
                return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
                TempData["TitleError"] = API.TitleTryCatch;
                TempData["DetailError"] = ex.Message;
                apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
            }
        }
    }
}
