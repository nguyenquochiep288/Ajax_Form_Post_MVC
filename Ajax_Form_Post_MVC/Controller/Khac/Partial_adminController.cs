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
		[ChildActionOnly]
		public ActionResult Index()
		{
			if (base.Session["Login_Model"] != null)
			{
				Login_Model login_Model = (Login_Model)base.Session["Login_Model"];
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
				base.ViewBag.Images = login_Model.url_image;
				base.ViewBag.Fullname = login_Model.fullname;
				base.ViewBag.Username = login_Model.user;
			}
			return PartialView();
		}

		public ActionResult Home()
		{
			if (base.Session["Login_Model"] != null)
			{
				Login_Model login_Model = (Login_Model)base.Session["Login_Model"];
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
				base.ViewBag.Images = login_Model.url_image;
				base.ViewBag.Fullname = login_Model.fullname;
				base.ViewBag.Username = login_Model.user;
			}
			return PartialView();
		}
	}
}
