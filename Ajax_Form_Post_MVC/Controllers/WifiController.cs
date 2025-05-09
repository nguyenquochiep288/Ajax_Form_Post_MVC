using DatabaseTHP;
using MVC_QuanLyTHP.Models;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Configuration;
using MVC_QuanLyTHP.Class;
using System.Web.UI;
using System.Collections.Generic;
using System;
using System.Web.DynamicData;
using PagedList;
using Syncfusion.EJ2.Linq;
using System.Reflection;
using System.Web.Routing;
using DatabaseTHP.Class;
using Newtonsoft.Json;
using SixLabors.ImageSharp.PixelFormats;
using System.Diagnostics;
using System.Web;

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
            //var ssid = GetSSID();
            string ssid = "";
            
            string ipAddress = Request != null ? Request.UserHostAddress : ""; // Lấy địa chỉ IP
            string hostname = !string.IsNullOrEmpty(ipAddress) ? Dns.GetHostEntry(ipAddress).HostName : ""; // Lấy hostname từ IP
            ssid += "<br>ipAddress:" + ipAddress;
            ssid += "<br>hostname:" + hostname;
            return Json(new { ssid = ssid }, JsonRequestBehavior.AllowGet);
        }

        private string GetSSID()
        {
            string ssid = string.Empty;
            ProcessStartInfo psi = new ProcessStartInfo("netsh", "wlan show interfaces")
            {
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            using (Process process = Process.Start(psi))
            {
                using (System.IO.StreamReader reader = process.StandardOutput)
                {
                    string output = reader.ReadToEnd();
                    int ssidIndex = output.IndexOf("SSID", StringComparison.OrdinalIgnoreCase);
                    if (ssidIndex >= 0)
                    {
                        int ssidStart = output.IndexOf(':', ssidIndex) + 1;
                        int ssidEnd = output.IndexOf('\n', ssidStart);
                        ssid = output.Substring(ssidStart, ssidEnd - ssidStart).Trim();
                    }
                }
            }
            return ssid;
        }
    }
}