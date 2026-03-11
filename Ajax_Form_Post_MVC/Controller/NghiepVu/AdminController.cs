using System;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using DatabaseTHP;
using DatabaseTHP.Class;
using MVC_QuanLyTHP.Class;

namespace MVC_QuanLyTHP.Controllers
{

    public class AdminController : Controller
    {
        public ActionResult Index()
        {
            try
            {
                if (!Utility.KiemTra())
                {
                    return RedirectToAction("Home", "Admin");
                }
                Login_Model login_Model = new Login_Model();
                HttpCookie httpCookie = base.Request.Cookies["THP"];
                if (httpCookie != null && httpCookie.Value != "")
                {
                    string text = clsMaHoa.Decrypt(httpCookie.Values["Us"].ToString(), "tmt6364");
                    string[] array = text.Split(new string[1] { Environment.NewLine }, StringSplitOptions.None);
                    if (array != null && array.Count() == 2)
                    {
                        login_Model.user = array[0];
                        login_Model.pass = array[1];
                        login_Model.check = true;
                    }
                }
                return View(login_Model);
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
                base.TempData["TitleError"] = "Đã có lỗi phát sinh trong phiên làm việc!";
                base.TempData["DetailError"] = ex.Message;
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

        [HttpPost]
        public ActionResult Index(Login_Model model)
        {
            try
            {
                if (base.ModelState.IsValid)
                {
                    ApiResponse apiResponse = Utility.Login(model.user, model.pass);
                    if (apiResponse.Success)
                    {
                        Utility.SetSession(apiResponse, model, null);
                        if (model.check)
                        {
                            HttpCookie httpCookie = new HttpCookie("THP");
                            string value = clsMaHoa.Encrypt(model.user + Environment.NewLine + model.pass, "tmt6364");
                            httpCookie.Values["Us"] = value;
                            httpCookie.Expires = Utility.CurrentTime.AddDays(90.0);
                            base.ControllerContext.HttpContext.Response.Cookies.Add(httpCookie);
                        }
                        return RedirectToAction("Home", "Admin");
                    }
                    base.Session.Clear();
                    base.ModelState.AddModelError(string.Empty, apiResponse.Message);
                }
                return View(model);
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
                base.TempData["TitleError"] = "Đã có lỗi phát sinh trong phiên làm việc!";
                base.TempData["DetailError"] = ex.Message;
                return RedirectToAction("Index", "Notfound");
            }
        }

        [HttpPost]
        public ActionResult show(int Show, string Controller)
        {
            base.Session["PageSize"] = Show;
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult GetWidth(int Width)
        {
            if (Width < 650)
            {
                base.Session["StypeWidth_Level1"] = "style='width: 92%; margin-left: 4%;'";
                base.Session["StypeWidth_Level2"] = "style='width: 84%; margin-left: 8%;'";
                base.Session["StypeWidth_Level3"] = "style='width: 78%; margin-left: 11%;'";
            }
            else
            {
                base.Session["StypeWidth_Level1"] = "style='width: 70%; margin-left: 15%;'";
                base.Session["StypeWidth_Level2"] = "style='width: 60%; margin-left: 20%;'";
                base.Session["StypeWidth_Level3"] = "style='width: 50%; margin-left: 25%;'";
            }
            base.Session["IntWidth"] = 1;
            return RedirectToAction("Index");
        }

        public ActionResult Logout()
        {
            base.Session.Clear();
            HttpCookie httpCookie = new HttpCookie("THP");
            if (httpCookie != null)
            {
                httpCookie.Values["Us"] = string.Empty;
                httpCookie.Expires = DateTime.Now.AddDays(-1.0);
                base.ControllerContext.HttpContext.Response.Cookies.Add(httpCookie);
            }
            Utility.Reset();
            return RedirectToAction("Index", "Admin");
        }

        public ActionResult Cache()
        {
            Utility.KiemTra(bolCach: true);
            return RedirectToAction("Index", "Admin");
        }
    }
}
