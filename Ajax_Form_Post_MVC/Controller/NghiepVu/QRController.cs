#define DEBUG
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Aspose.BarCode.BarCodeRecognition;
using MVC_QuanLyTHP.Class;

namespace MVC_QuanLyTHP.Controllers
{

	public class QRController : Controller
	{
		public ActionResult Index()
		{
			try
			{
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

		public JsonResult Scan(HttpPostedFileBase file)
		{
			string data = "";
			try
			{
				string filename = "";
				if (file.ContentLength > 0)
				{
					string fileName = Path.GetFileName(file.FileName);
					filename = Path.Combine(base.Server.MapPath("~/App_Data"), fileName);
					file.SaveAs(filename);
				}
				Image image = Image.FromFile(filename);
				Debug.WriteLine("Width:" + image.Width + " - Height:" + image.Height);
				try
				{
					BarCodeReader barCodeReader = new BarCodeReader(filename, DecodeType.AllSupportedTypes);
					BarCodeResult[] array = barCodeReader.ReadBarCodes();
					foreach (BarCodeResult barCodeResult in array)
					{
						data = barCodeResult.CodeText;
					}
				}
				catch (Exception ex)
				{
					Console.Write(ex.Message);
				}
			}
			catch (Exception ex2)
			{
				base.ViewBag.Title = ex2.Message;
			}
			return Json(data);
		}
	}
}
