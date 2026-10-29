using System;
using System.Web.Mvc;
using MVC_QuanLyTHP.Class;
using THP.Catalogs;

namespace MVC_QuanLyTHP.Controllers
{
    // Resolve feature availability in the compiled MVC layer. Shared Razor views
    // remain compilable even when deployed before the matching MVC assembly.
    public class CatalogFlagPageFilter : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            if (!(filterContext.Result is ViewResultBase)) return;
            var route = filterContext.RouteData.Values;
            var catalog = Convert.ToString(route["controller"]);
            var action = Convert.ToString(route["action"]);
            filterContext.Controller.ViewData["CatalogFlagsEnabled"] =
                string.Equals(action, "Index", StringComparison.OrdinalIgnoreCase)
                && CatalogFlagDefinitions.Find(catalog) != null;
        }
    }

    public class CatalogFlagsController : Controller
    {
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Update(string catalog, string id, string field, bool? enabled)
        {
            if (Utility.KiemTra()) return new HttpStatusCodeResult(401);
            if (!CatalogFlagDefinitions.Allows(CatalogFlagDefinitions.Find(catalog), field)
                || !enabled.HasValue || string.IsNullOrWhiteSpace(id) || id.Length > 50)
                return new HttpStatusCodeResult(400);
            if (!Utility.KiemTraQuyen(catalog, "Edit")) return new HttpStatusCodeResult(403);
            var result = Utility.Edit(Uri.EscapeDataString(catalog) + "/"
                + Uri.EscapeDataString(Utility.LOC_ID) + "/" + Uri.EscapeDataString(id),
                new { FIELD = field, ENABLED = enabled.Value, USER_ID = Convert.ToString(Session["idUser"]) }, "CatalogFlags");
            return Json(new { Success = result.Success,
                Message = result.Success ? "Đã lưu thay đổi." : "Không lưu được thay đổi. Vui lòng tải lại danh sách và thử lại." });
        }
    }
}
