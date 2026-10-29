using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using DatabaseTHP;
using MVC_QuanLyTHP.Class;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MVC_QuanLyTHP.Controllers
{
    public class InventoryInsightsController : Controller
    {
        public ActionResult Index(string mode = "slow")
        {
            if (Utility.KiemTra()) return RedirectToAction("Index", "Admin");
            if (!Utility.KiemTraQuyen("Inventory", "View")) return RedirectToAction("Index", "Notfound");
            Response.ContentEncoding = Encoding.UTF8;
            Response.Charset = "utf-8";
            ViewBag.Mode = mode == "reorder" || mode == "sellers" ? mode : "slow";
            var groups = Utility.GetListData<dm_NhomHangHoa>("GroupProduct", "", "", Utility.LOC_ID);
            var depots = Utility.GetListData<dm_Kho>("Depot", "", "", Utility.LOC_ID);
            ViewBag.Groups = groups.Data as List<dm_NhomHangHoa> ?? new List<dm_NhomHangHoa>();
            ViewBag.Depots = depots.Data as List<dm_Kho> ?? new List<dm_Kho>();
            ViewBag.LoadError = !groups.Success || !depots.Success ? "Không tải được danh mục kho/nhóm. Vui lòng tải lại trang." : null;
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Data(string payload)
        {
            if (Utility.KiemTra() || !Utility.KiemTraQuyen("Inventory", "View"))
                return Failure("Phiên đăng nhập hoặc quyền xem tồn kho không hợp lệ.");
            try
            {
                var request = JObject.Parse(payload ?? "{}");
                request["LOC_ID"] = Utility.LOC_ID;
                using (var client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(120);
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + Utility.Token);
                    var response = await client.PostAsync(Utility.URL.TrimEnd('/') + "/InventoryInsights/Report",
                        new StringContent(request.ToString(Formatting.None), Encoding.UTF8, "application/json"), Response.ClientDisconnectedToken);
                    if (!response.IsSuccessStatusCode) return Failure("Không tải được báo cáo (HTTP " + (int)response.StatusCode + ").");
                    return Content(await response.Content.ReadAsStringAsync(), "application/json", Encoding.UTF8);
                }
            }
            catch { return Failure("Không nhận được báo cáo. Vui lòng thử lại hoặc thu hẹp khoảng ngày/nhóm hàng."); }
        }

        private ActionResult Failure(string message)
        {
            return Content(JsonConvert.SerializeObject(new { Success = false, Message = message }), "application/json", Encoding.UTF8);
        }
    }
}
