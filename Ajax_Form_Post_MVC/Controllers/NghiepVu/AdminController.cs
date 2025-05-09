using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using MVC_QuanLyTHP.Filters;
using MVC_QuanLyTHP.Models;
using System.IO;
using System.ComponentModel.DataAnnotations;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.Entity.Core.Objects;
using DatabaseTHP;
using System.Configuration;
using MVC_QuanLyTHP.Class;
using PagedList;
using System.EnterpriseServices.CompensatingResourceManager;
using DatabaseTHP.StoredProcedure.Parameter;
using System.Reflection;
using DatabaseTHP.StoredProcedure;
using DatabaseTHP.Class;
using Newtonsoft.Json;
using MVC_QuanLyTHP.Models.Order;
using System.Web.DynamicData;

namespace MVC_QuanLyTHP.Controllers
{
    public class AdminController : Controller
    {
        //
        // GET: /Admin/
        public ActionResult Index()
        {
            try
            {
                if (!Utility.KiemTra())
                {
                    return RedirectToAction("Home", "Admin");
                }
                Login_Model Login_Model = new Login_Model();
                HttpCookie cookie = Request.Cookies[Cookies.Name];
                if ((cookie != null) && (cookie.Value != ""))
                {
                    string MaHoa = clsMaHoa.Decrypt(cookie.Values[Cookies.User].ToString(), clsMaHoa.PassMaHoa);
                    var lst = MaHoa.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                    if (lst != null && lst.Count() == 2)
                    {
                        Login_Model.user = lst[0];
                        Login_Model.pass = lst[1];
                        Login_Model.check = true;
                    }
                }
                return View(Login_Model);
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
                TempData["TitleError"] = API.TitleTryCatch;
                TempData["DetailError"] = ex.Message;
                return RedirectToAction("Index", "Notfound");
            }
        }
        public ActionResult Home()
        {
            if (Utility.KiemTra())
            {
                return RedirectToAction("Index", "Admin");
            }
            return View();
        }
      
        #region[login]
        #region Login_admin
        [HttpPost]
        public ActionResult Index(Login_Model model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ApiResponse apiResponse = Utility.Login(model.user, model.pass);
                    if (apiResponse.Success)
                    {
                        Utility.SetSession(apiResponse, model, null);
                        if (model.check)
                        {
                            HttpCookie cookie = new HttpCookie(Cookies.Name);
                            string MaHoa = clsMaHoa.Encrypt(model.user + Environment.NewLine + model.pass, clsMaHoa.PassMaHoa);
                            cookie.Values[Cookies.User] = MaHoa;
                            cookie.Expires = Utility.CurrentTime.AddDays(90);
                            this.ControllerContext.HttpContext.Response.Cookies.Add(cookie);
                        }
                        return RedirectToAction("Home", "Admin");
                    }
                    else
                    {
                        Session.Clear();
                        ModelState.AddModelError(string.Empty, apiResponse.Message);
                    }
                }
                return View(model);
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
                TempData["TitleError"] = API.TitleTryCatch;
                TempData["DetailError"] = ex.Message;
                return RedirectToAction("Index", "Notfound");
            }
        }
        #endregion
        #endregion

        #region Show
        [HttpPost]
        public ActionResult show(int Show, string Controller)
        {
            Session[Sessions.PageSize] = Show;
            return RedirectToAction("Index");
        }
        #endregion

        #region 
        [HttpPost]
        public ActionResult GetWidth(int Width)
        {
            if (Width < 650) 
            {
                Session[Sessions.StypeWidth_Level1] = "style='width: 92%; margin-left: 4%;'";
                Session[Sessions.StypeWidth_Level2] = "style='width: 84%; margin-left: 8%;'";
                Session[Sessions.StypeWidth_Level3] = "style='width: 78%; margin-left: 11%;'";
            }
            else
            {
                Session[Sessions.StypeWidth_Level1] = "style='width: 70%; margin-left: 15%;'";
                Session[Sessions.StypeWidth_Level2] = "style='width: 60%; margin-left: 20%;'";
                Session[Sessions.StypeWidth_Level3] = "style='width: 50%; margin-left: 25%;'";

            }    
               

            //if (Width < 650)
            //    Session[Sessions.IntWidth] = 2;
            //else
            Session[Sessions.IntWidth] = 1;
            return RedirectToAction("Index");
        }
        #endregion

        #region Logout
        public ActionResult Logout()
        {
            Session.Clear();
            HttpCookie cookie = new HttpCookie(Cookies.Name);
            if(cookie != null)
            {
                cookie.Values[Cookies.User] = string.Empty;
                cookie.Expires = DateTime.Now.AddDays(-1);
                this.ControllerContext.HttpContext.Response.Cookies.Add(cookie);
            }
            Utility.Reset();
            return RedirectToAction("Index", "Admin");
        }

        public ActionResult Cache()
        {
            Utility.KiemTra(true);
            return RedirectToAction("Index", "Admin");
        }
        #endregion
    }
}
