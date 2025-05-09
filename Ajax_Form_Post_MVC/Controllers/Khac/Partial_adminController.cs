using System.Linq;
using System.Web.Mvc;
using System.Configuration;
using DatabaseTHP;
using DatabaseTHP.Class;
using MVC_QuanLyTHP.Class;
using MVC_QuanLyTHP.Models;
using System.Collections.Generic;

namespace MVC_QuanLyTHP.Controllers
{
    public class Partial_adminController : Controller
    {
        //
        // GET: /Partial_admin/
        [ChildActionOnly]
        public ActionResult Index()
        {
            if (Session[Sessions.Login_Model] != null)
            {
                var Login_Model = (Login_Model)Session[Sessions.Login_Model];

                v_v_AspNetUsers Login = new v_v_AspNetUsers();
                ApiResponse apiResponse = new ApiResponse();
                if(Login_Model != null && string.IsNullOrEmpty(Login_Model.url_image))
                {
                    apiResponse = Utility.GetDetail<List<v_v_AspNetUsers>>(Login_Model.iduser, API.AspNetUser);
                    if (apiResponse.Success)
                        Login = apiResponse.Data as v_v_AspNetUsers;
                    if(Login != null && !string.IsNullOrEmpty(Login.URL_IMAGE))
                        Login_Model.url_image = Login.URL_IMAGE;
                    else
                        Login_Model.url_image = "~/Images_sp/hinh.jpg";
                }
                Session[Sessions.Login_Model] = Login_Model;
                string full = Login_Model.fullname;
                ViewBag.Images = Login_Model.url_image;
                ViewBag.Fullname = Login_Model.fullname;
                ViewBag.Username = Login_Model.user;
            }
            return PartialView();
        }
        public ActionResult Home()
        {
            if (Session[Sessions.Login_Model] != null)
            {
                var Login_Model = (Login_Model)Session[Sessions.Login_Model];
                v_AspNetUsers Login = new v_AspNetUsers();
                ApiResponse apiResponse = new ApiResponse();
                if (Login_Model != null && string.IsNullOrEmpty(Login_Model.url_image))
                {
                    apiResponse = Utility.GetDetail<v_AspNetUsers>(Login_Model.iduser, API.AspNetUser);
                    if (apiResponse.Success)
                        Login = apiResponse.Data as v_AspNetUsers;
                    if (Login != null && !string.IsNullOrEmpty(Login.URL_IMAGE))
                        Login_Model.url_image = Login.URL_IMAGE;
                    else
                        Login_Model.url_image = "~/Images_sp/hinh.jpg";
                }
                Session[Sessions.Login_Model] = Login_Model;
                string full = Login_Model.fullname;
                ViewBag.Images = Login_Model.url_image;
                ViewBag.Fullname = Login_Model.fullname;
                ViewBag.Username = Login_Model.user;
            }
            return PartialView();
        }
    }
}
