using System;
using System.Reflection;
using System.Web.Mvc;
using DatabaseTHP.Class;
using MVC_QuanLyTHP.Class;

namespace MVC_QuanLyTHP.Controllers
{

	public class TestController : Controller
	{
		public ActionResult Index()
		{
			try
			{
				double lat = 10.7045182;
				double lon = 106.7130296;
				double lat2 = 10.8314986;
				double lon2 = 106.6111826;
				double num = API.CalculateDistance(lat, lon, lat2, lon2);
				return View();
			}
			catch (Exception ex)
			{
				Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
				base.TempData["TitleError"] = "Đã có lỗi phát sinh trong phiên làm việc!";
				base.TempData["DetailError"] = ex.Message;
				return RedirectToAction("Index", "Notfound");
			}
		}
	}
}
