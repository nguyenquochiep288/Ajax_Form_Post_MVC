using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Web.Mvc;

namespace MVC_QuanLyTHP.Controllers
{

	public class WifiController : Controller
	{
		public ActionResult Index()
		{
			return View();
		}

		[HttpGet]
		public JsonResult GetCurrentSSID()
		{
			string text = "";
			string text2 = ((base.Request != null) ? base.Request.UserHostAddress : "");
			string text3 = ((!string.IsNullOrEmpty(text2)) ? Dns.GetHostEntry(text2).HostName : "");
			text = text + "<br>ipAddress:" + text2;
			text = text + "<br>hostname:" + text3;
			return Json(new
			{
				ssid = text
			}, JsonRequestBehavior.AllowGet);
		}

		private string GetSSID()
		{
			string result = string.Empty;
			ProcessStartInfo startInfo = new ProcessStartInfo("netsh", "wlan show interfaces")
			{
				RedirectStandardOutput = true,
				UseShellExecute = false,
				CreateNoWindow = true
			};
			using (Process process = Process.Start(startInfo))
			{
				 StreamReader streamReader = process.StandardOutput;
				string text = streamReader.ReadToEnd();
				int num = text.IndexOf("SSID", StringComparison.OrdinalIgnoreCase);
				if (num >= 0)
				{
					int num2 = text.IndexOf(':', num) + 1;
					int num3 = text.IndexOf('\n', num2);
					result = text.Substring(num2, num3 - num2).Trim();
				}
			}
			return result;
		}
	}
}
