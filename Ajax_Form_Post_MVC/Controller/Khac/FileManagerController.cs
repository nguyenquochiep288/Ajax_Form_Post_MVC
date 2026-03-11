using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Web.Mvc;
using DatabaseTHP;
using DatabaseTHP.Class;
using MVC_QuanLyTHP.Class;

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
				if (base.ModelState.IsValid)
				{
					dm_HangHoa.LOC_ID = Utility.LOC_ID;
					dm_HangHoa.ID = Guid.NewGuid().ToString();
					dm_HangHoa.ID_NGUOITAO = base.Session["idUser"].ToString();
					dm_HangHoa.THOIGIANTHEM = Utility.CurrentTime;
					if (base.Request.Files["MaHinh"] != null)
					{
						string fileName = base.Request.Files["MaHinh"].FileName;
						if (fileName != "")
						{
							string text = dm_HangHoa.ID.ToString() + "." + fileName.Split('.')[1];
							string text2 = "/Images_Upload/Delivery/" + dm_HangHoa.ID_NGUOITAO + "/" + dm_HangHoa.ID_PHIEUXUAT + "/";
							string text3 = Path.Combine(base.Server.MapPath("~" + text2), text);
							if (!Directory.Exists(base.Server.MapPath("~" + text2)))
							{
								Directory.CreateDirectory(base.Server.MapPath("~" + text2));
							}
							base.Request.Files["MaHinh"].SaveAs(text3);
							dm_HangHoa.URL_IMAGE = text2 + text;
							byte[] inArray = System.IO.File.ReadAllBytes(text3);
							string text4 = Convert.ToBase64String(inArray);
						}
					}
					apiResponse = Utility.Create(dm_HangHoa, "Delivery_Image");
					apiResponse = Utility.GetDetail<List<v_ct_PhieuGiaoHang_HinhAnh>>(Utility.LOC_ID + "/" + dm_HangHoa.ID_PHIEUXUAT, "Delivery_Image");
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
					List<v_ct_PhieuGiaoHang_HinhAnh> list = new List<v_ct_PhieuGiaoHang_HinhAnh>();
					if (apiResponse.Data != null)
					{
						list = apiResponse.Data as List<v_ct_PhieuGiaoHang_HinhAnh>;
					}
					foreach (v_ct_PhieuGiaoHang_HinhAnh item in list)
					{
						string[] array = item.URL_IMAGE.Split('/');
						string authority = base.Request.Url.Authority;
						string text5 = "";
						text5 = ((!base.Request.Url.AbsoluteUri.StartsWith("https")) ? ("http://" + authority + item.URL_IMAGE) : ("https://" + authority + item.URL_IMAGE));
						apiResponse.CONTENT += "<div class='col-xs-6 col-sm-4 col-md-3 image'>";
						apiResponse.CONTENT += "<div class='thmb'>";
						apiResponse.CONTENT += "<div class='ckbox ckbox-default'>";
						apiResponse.CONTENT += "</div>";
						apiResponse.CONTENT += "<div class='btn-group fm-group'>";
						apiResponse.CONTENT += "</div><!-- btn-group -->";
						apiResponse.CONTENT += "<div class='thmb-prev'>";
						ApiResponse apiResponse2 = apiResponse;
						apiResponse2.CONTENT = apiResponse2.CONTENT + "<a href='" + text5 + "' data-rel='prettyPhoto'>";
						ApiResponse apiResponse3 = apiResponse;
						apiResponse3.CONTENT = apiResponse3.CONTENT + "<img src='" + text5 + "' class='img-responsive' alt='' />";
						apiResponse.CONTENT += "</a>";
						apiResponse.CONTENT += "</div>";
						ApiResponse apiResponse4 = apiResponse;
						apiResponse4.CONTENT = apiResponse4.CONTENT + "<h5 class='fm-title'><a href='#'>" + item.NAME_NGUOITAO + "</a></h5> ";
						ApiResponse apiResponse5 = apiResponse;
						apiResponse5.CONTENT = apiResponse5.CONTENT + "<small class='text-muted'>" + item.NGAYTAO.ToString("dd/MM/yyyy HH:mm") + "</small>";
						apiResponse.CONTENT += "</div><!-- thmb -->";
						apiResponse.CONTENT += "</div><!-- col-xs-6 -->";
					}
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
	}
}
