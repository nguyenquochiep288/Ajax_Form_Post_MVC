using System.Web.Mvc;
using System.Web.Routing;

namespace MVC_QuanLyTHP
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute(
                name: "KhuyenMai",
                url: "KhuyenMai",
                defaults: new { controller = "Promotion", action = "ForCustomers" }
            );
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Admin", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
