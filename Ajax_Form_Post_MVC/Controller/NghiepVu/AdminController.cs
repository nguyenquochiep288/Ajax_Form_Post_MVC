using System;
using System.Reflection;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DatabaseTHP;
using MVC_QuanLyTHP.Class;
using MVC_QuanLyTHP.Models;

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

        [HttpGet]
        public ActionResult My_profile()
        {
            try
            {
                if (Utility.KiemTra())
                {
                    return RedirectToAction("Index", "Admin");
                }

                Login_Model login = base.Session["Login_Model"] as Login_Model;
                string userId = Convert.ToString(base.Session["idUser"] ?? (object)login?.iduser);
                v_v_AspNetUsers profile = new v_v_AspNetUsers
                {
                    ID = userId,
                    FullName = login?.fullname ?? Convert.ToString(base.Session["User"]),
                    NAME = login?.fullname ?? Convert.ToString(base.Session["User"]),
                    UserName = login?.user ?? Convert.ToString(base.Session["User"]),
                    URL_IMAGE = string.IsNullOrWhiteSpace(login?.url_image) ? "~/Images_sp/hinh.jpg" : login.url_image
                };

                if (string.IsNullOrWhiteSpace(userId))
                {
                    ModelState.AddModelError(string.Empty, "Phiên đăng nhập chưa có mã người dùng. Vui lòng đăng xuất và đăng nhập lại.");
                    return View(profile);
                }

                ApiResponse response = Utility.GetDetail<v_v_AspNetUsers>(userId, "User");
                v_v_AspNetUsers apiProfile = response.Data as v_v_AspNetUsers;
                if (!response.Success || apiProfile == null)
                {
                    ModelState.AddModelError(string.Empty, response.Message ?? "Không tải được dữ liệu từ máy chủ. Thông tin phiên đăng nhập đang được hiển thị.");
                    return View(profile);
                }

                return View(apiProfile);
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
                base.TempData["TitleError"] = "Không tải được thông tin tài khoản.";
                base.TempData["DetailError"] = ex.Message;
                return RedirectToAction("Index", "Notfound");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult My_profile(string fullName, string userName, string email, string phoneNumber, HttpPostedFileBase MaHinh)
        {
            v_v_AspNetUsers profile = null;
            try
            {
                if (Utility.KiemTra())
                {
                    return RedirectToAction("Index", "Admin");
                }

                string userId = base.Session["idUser"] as string;
                if (string.IsNullOrWhiteSpace(userId))
                {
                    return RedirectToAction("Logout", "Admin");
                }

                ApiResponse detail = Utility.GetDetail<v_v_AspNetUsers>(userId, "User");
                profile = detail.Data as v_v_AspNetUsers;
                if (!detail.Success || profile == null)
                {
                    ModelState.AddModelError(string.Empty, detail.Message ?? "Không tải được thông tin tài khoản.");
                    return View(new v_v_AspNetUsers { URL_IMAGE = "~/Images_sp/hinh.jpg" });
                }

                fullName = (fullName ?? string.Empty).Trim();
                userName = (userName ?? string.Empty).Trim();
                email = (email ?? string.Empty).Trim();
                phoneNumber = (phoneNumber ?? string.Empty).Trim();

                if (string.IsNullOrWhiteSpace(fullName) || fullName.Length > 256)
                    ModelState.AddModelError("fullName", "Vui lòng nhập họ tên (tối đa 256 ký tự).");
                if (string.IsNullOrWhiteSpace(userName) || userName.Length > 256)
                    ModelState.AddModelError("userName", "Vui lòng nhập tên đăng nhập (tối đa 256 ký tự).");
                if (email.Length > 256 || (!string.IsNullOrWhiteSpace(email) && !new System.ComponentModel.DataAnnotations.EmailAddressAttribute().IsValid(email)))
                    ModelState.AddModelError("Email", "Email không hợp lệ.");
                if (phoneNumber.Length > 50)
                    ModelState.AddModelError("PhoneNumber", "Số điện thoại không được vượt quá 50 ký tự.");

                if (MaHinh != null && MaHinh.ContentLength > 0)
                {
                    string extension = Path.GetExtension(MaHinh.FileName).ToLowerInvariant();
                    string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                    if (!allowedExtensions.Contains(extension) || MaHinh.ContentLength > 5 * 1024 * 1024)
                        ModelState.AddModelError("MaHinh", "Ảnh phải có định dạng JPG, PNG, GIF hoặc WEBP và dung lượng không quá 5 MB.");
                }

                if (!ModelState.IsValid)
                {
                    profile.NAME = fullName;
                    profile.UserName = userName;
                    profile.Email = email;
                    profile.PhoneNumber = phoneNumber;
                    return View(profile);
                }

                string imageUrl = profile.URL_IMAGE;
                if (MaHinh != null && MaHinh.ContentLength > 0)
                {
                    string extension = Path.GetExtension(MaHinh.FileName).ToLowerInvariant();
                    string folder = base.Server.MapPath("~/Images_Upload/User/");
                    Directory.CreateDirectory(folder);
                    string fileName = userId.Trim() + extension;
                    MaHinh.SaveAs(Path.Combine(folder, fileName));
                    imageUrl = "/Images_Upload/User/" + fileName;
                }

                // Keep identity/security fields from the API record; only apply fields editable on this page.
                profile.FullName = fullName;
                profile.UserName = userName;
                profile.Email = email;
                profile.PhoneNumber = phoneNumber;
                profile.URL_IMAGE = imageUrl;
                ApiResponse update = Utility.Edit(userId, (v_AspNetUsers)profile, "Accounts/ChangeUser");
                if (!update.Success)
                {
                    ModelState.AddModelError(string.Empty, update.Message ?? "Cập nhật thông tin thất bại.");
                    profile.NAME = fullName;
                    return View(profile);
                }

                Login_Model login = base.Session["Login_Model"] as Login_Model;
                if (login != null)
                {
                    login.fullname = fullName;
                    login.user = userName;
                    login.url_image = imageUrl;
                    base.Session["Login_Model"] = login;
                }
                base.Session["User"] = userName;

                base.TempData["ProfileSuccess"] = "Thông tin tài khoản đã được cập nhật.";
                return RedirectToAction("My_profile");
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
                base.ModelState.AddModelError(string.Empty, "Có lỗi khi cập nhật tài khoản: " + ex.Message);
                return View(profile ?? new v_v_AspNetUsers { URL_IMAGE = "~/Images_sp/hinh.jpg" });
            }
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
            MVC_QuanLyTHP.MvcApplication.RemoveActiveSession(base.Session.SessionID);
            Utility.RevokeRefreshToken(base.Session["RefreshToken"] as string);
            base.Session.Clear();
            Utility.ExpirePersistedTokenCookie();
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
