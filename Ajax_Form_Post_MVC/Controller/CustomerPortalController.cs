using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using MVC_QuanLyTHP.Class;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using QRCoder;

namespace MVC_QuanLyTHP.Controllers
{
    public class CustomerPortalController : Controller
    {
        private static readonly HttpClient Client = new HttpClient { Timeout = TimeSpan.FromSeconds(150) };

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            Response.Cache.SetCacheability(System.Web.HttpCacheability.NoCache);
            Response.Cache.SetNoStore();
            Response.Headers["Referrer-Policy"] = "no-referrer";
            Response.Headers["X-Robots-Tag"] = "noindex, nofollow, noarchive";
            Response.Headers["X-Content-Type-Options"] = "nosniff";
            base.OnActionExecuting(filterContext);
        }

        [HttpGet, AllowAnonymous]
        public ActionResult Index(string token)
        {
            if (!ValidToken(token)) return new HttpStatusCodeResult(404, "Link khong hop le");
            Response.Headers["Content-Security-Policy"] = "default-src 'self'; script-src 'self'; style-src 'self'; img-src 'self' data:; connect-src 'self'; frame-ancestors 'none'; base-uri 'none'; object-src 'none'; form-action 'self'";
            ViewBag.PortalToken = token;
            return View();
        }

        private static bool ValidToken(string token)
        {
            if (token == null || token.Length != 43) return false;
            foreach (char c in token)
                if (!((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || (c >= '0' && c <= '9') || c == '-' || c == '_')) return false;
            return true;
        }

        [HttpPost, AllowAnonymous]
        public Task<ActionResult> Read(string token, string section = "overview", string id = null, string from = null, string to = null, int page = 1, string search = null, decimal? amount = null)
        {
            if (!ValidToken(token)) return Task.FromResult<ActionResult>(new HttpStatusCodeResult(404));
            // This request never uses Utility.Token or a staff session, even if the browser is logged in.
            return Proxy(HttpMethod.Post, "read", new { Section = section, Id = id, From = from, To = to, Page = page, Search = search, Amount = amount }, token, false);
        }

        [HttpPost, AllowAnonymous]
        public async Task<ActionResult> ExportExcel(string token, string from, string to)
        {
            if (!ValidToken(token)) return new HttpStatusCodeResult(404);
            var result = await Proxy(HttpMethod.Post, "read", new { Section = "debt-export", From = from, To = to }, token, false);
            var content = result as ContentResult;
            if (Response.StatusCode >= 400 || content == null) return result;
            try
            {
                var data = JObject.Parse(content.Content);
                var bytes = PortalStatementExcel.Create(data);
                return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "doi-chieu-cong-no-" + data["from"].Value<DateTime>().ToString("yyyyMMdd") + "-" + data["to"].Value<DateTime>().ToString("yyyyMMdd") + ".xlsx");
            }
            catch { Response.StatusCode = 503; return Json(new { message = "Chưa thể xuất bảng đối chiếu. Vui lòng thử lại sau." }); }
        }

        private bool CanManage()
        {
            return !Utility.KiemTra() && Utility.KiemTraQuyen("Customer", "Edit")
                && Utility.KiemTraQuyen("Deposit", "View") && Utility.KiemTraQuyen("Output", "View")
                && Utility.KiemTraQuyen("Invoiced", "View") && Utility.KiemTraQuyen("DebtCustomer", "View");
        }

        [HttpGet]
        public ActionResult Manage(string id)
        {
            if (Utility.KiemTra()) return RedirectToAction("Index", "Admin");
            if (!CanManage()) return new HttpStatusCodeResult(403);
            if (string.IsNullOrWhiteSpace(id) || id.Length > 50) return new HttpStatusCodeResult(400);
            ViewBag.CustomerId = id;
            return View();
        }

        [HttpGet]
        public Task<ActionResult> Status(string id)
        {
            if (!CanManage()) return Task.FromResult<ActionResult>(new HttpStatusCodeResult(403));
            return Proxy(HttpMethod.Get, LinkPath(id), null, null, true);
        }

        [HttpGet]
        public Task<ActionResult> History(string id, int page = 1)
        {
            if (!CanManage()) return Task.FromResult<ActionResult>(new HttpStatusCodeResult(403));
            if (page < 1 || page > 100000) return Task.FromResult<ActionResult>(new HttpStatusCodeResult(400));
            return Proxy(HttpMethod.Get, LinkPath(id) + "/history?page=" + page, null, null, true);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public Task<ActionResult> Issue(string id, int? validDays)
        {
            if (!CanManage()) return Task.FromResult<ActionResult>(new HttpStatusCodeResult(403));
            return Proxy(HttpMethod.Post, LinkPath(id), new { ValidDays = validDays }, null, true);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public Task<ActionResult> Revoke(string id)
        {
            if (!CanManage()) return Task.FromResult<ActionResult>(new HttpStatusCodeResult(403));
            return Proxy(HttpMethod.Delete, LinkPath(id), null, null, true);
        }

        private static string LinkPath(string id)
        {
            return "links/" + Uri.EscapeDataString(Utility.LOC_ID) + "/" + Uri.EscapeDataString(id ?? "");
        }

        private async Task<ActionResult> Proxy(HttpMethod method, string path, object body, string token, bool staff)
        {
            try
            {
                using (var message = new HttpRequestMessage(method, Utility.URL.TrimEnd('/') + "/CustomerPortal/" + path))
                {
                    if (staff) message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Utility.Token);
                    if (token != null) message.Headers.Add("X-Customer-Portal-Token", token);
                    if (body != null) message.Content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
                    using (var response = await Client.SendAsync(message))
                    {
                        Response.StatusCode = (int)response.StatusCode;
                        Response.TrySkipIisCustomErrors = true;
                        if (!response.IsSuccessStatusCode)
                        {
                            string error = response.StatusCode == HttpStatusCode.NotFound ? "Link hoặc chứng từ không còn hiệu lực."
                                : response.StatusCode == HttpStatusCode.Unauthorized ? "Phiên xác thực API không còn hiệu lực. Vui lòng đăng xuất và đăng nhập lại."
                                : response.StatusCode == HttpStatusCode.Forbidden ? "Bạn không có quyền quản lý link khách hàng này."
                                : response.StatusCode == HttpStatusCode.BadRequest ? "Thông tin không hợp lệ. Kiểm tra thời gian (tối đa 3 năm), số tiền hoặc chọn kỳ xuất ngắn hơn (tối đa 50.000 phát sinh)."
                                : response.StatusCode == HttpStatusCode.Conflict ? "Link vừa được thay đổi. Vui lòng tải lại trang."
                                : "Chưa thể tải dữ liệu. Vui lòng thử lại sau.";
                            return Json(new { message = error }, JsonRequestBehavior.AllowGet);
                        }
                        string json = await response.Content.ReadAsStringAsync();
                        // Only API-produced JSON is relayed; never expose upstream error pages or stack traces.
                        var data = JObject.Parse(json);
                        string payload = (string)data["payload"];
                        if (!staff && path == "read" && !string.IsNullOrEmpty(payload))
                        {
                            using (var generator = new QRCodeGenerator())
                            using (var qrData = generator.CreateQrCode(payload, QRCodeGenerator.ECCLevel.M))
                            using (var png = new PngByteQRCode(qrData))
                                data["qrDataUrl"] = "data:image/png;base64," + Convert.ToBase64String(png.GetGraphic(8));
                        }
                        return Content(data.ToString(Formatting.None), "application/json");
                    }
                }
            }
            catch
            {
                Response.StatusCode = 503;
                Response.TrySkipIisCustomErrors = true;
                return Json(new { message = "Chưa thể kết nối dữ liệu. Vui lòng thử lại sau." }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
