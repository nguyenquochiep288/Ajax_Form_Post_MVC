using System.Collections.Generic;
using System.Web.Mvc;
using DatabaseTHP;
using DatabaseTHP.Class;
using MVC_QuanLyTHP.Class;
using MVC_QuanLyTHP.Models;

namespace MVC_QuanLyTHP.Controllers
{

	public class Partial_adminController : Controller
	{
		private Login_Model GetLoginModel()
		{
			Login_Model loginModel = base.Session["Login_Model"] as Login_Model;
			if (loginModel == null)
			{
				loginModel = new Login_Model
				{
					user = base.Session["User"] as string ?? string.Empty,
					fullname = base.Session["User"] as string ?? string.Empty,
					iduser = base.Session["idUser"] as string ?? string.Empty
				};
				base.Session["Login_Model"] = loginModel;
			}
			return loginModel;
		}

		[ChildActionOnly]
		public ActionResult Index()
		{
			{
				Login_Model login_Model = GetLoginModel();
				v_v_AspNetUsers v_v_AspNetUsers2 = new v_v_AspNetUsers();
				ApiResponse apiResponse = new ApiResponse();
				if (login_Model != null && string.IsNullOrEmpty(login_Model.url_image))
				{
					apiResponse = Utility.GetDetail<List<v_v_AspNetUsers>>(login_Model.iduser, "User");
					if (apiResponse.Success)
					{
						v_v_AspNetUsers2 = apiResponse.Data as v_v_AspNetUsers;
					}
					if (v_v_AspNetUsers2 != null && !string.IsNullOrEmpty(v_v_AspNetUsers2.URL_IMAGE))
					{
						login_Model.url_image = v_v_AspNetUsers2.URL_IMAGE;
					}
					else
					{
						login_Model.url_image = "~/Images_sp/hinh.jpg";
					}
				}
				base.Session["Login_Model"] = login_Model;
				string fullname = login_Model.fullname;
				base.ViewBag.Images = string.IsNullOrWhiteSpace(login_Model.url_image) ? "~/Images_sp/hinh.jpg" : login_Model.url_image;
				base.ViewBag.Fullname = login_Model.fullname ?? string.Empty;
				base.ViewBag.Username = login_Model.user ?? string.Empty;
			}
			return PartialView();
		}

		public ActionResult Home()
		{
			{
				Login_Model login_Model = GetLoginModel();
				v_AspNetUsers v_AspNetUsers2 = new v_AspNetUsers();
				ApiResponse apiResponse = new ApiResponse();
				if (login_Model != null && string.IsNullOrEmpty(login_Model.url_image))
				{
					apiResponse = Utility.GetDetail<v_AspNetUsers>(login_Model.iduser, "User");
					if (apiResponse.Success)
					{
						v_AspNetUsers2 = apiResponse.Data as v_AspNetUsers;
					}
					if (v_AspNetUsers2 != null && !string.IsNullOrEmpty(v_AspNetUsers2.URL_IMAGE))
					{
						login_Model.url_image = v_AspNetUsers2.URL_IMAGE;
					}
					else
					{
						login_Model.url_image = "~/Images_sp/hinh.jpg";
					}
				}
				base.Session["Login_Model"] = login_Model;
				string fullname = login_Model.fullname;
				base.ViewBag.Images = string.IsNullOrWhiteSpace(login_Model.url_image) ? "~/Images_sp/hinh.jpg" : login_Model.url_image;
				base.ViewBag.Fullname = login_Model.fullname ?? string.Empty;
				base.ViewBag.Username = login_Model.user ?? string.Empty;
			}
			return PartialView();
		}
	}
}
