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
using System.IO;
using System.Web;
using Aspose.BarCode.BarCodeRecognition;

namespace MVC_QuanLyTHP.Controllers
{
    public class QRController : Controller
    {

        // GET: Payment
        public ActionResult Index()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
                TempData["TitleError"] = API.TitleTryCatch;
                TempData["DetailError"] = ex.Message;
                return RedirectToAction("Index", "Notfound");
            }
        }

        public JsonResult Scan(HttpPostedFileBase file)
        {
            string readedBarcode = "";
            try
            {
                string path = "";
                if (file.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    path = Path.Combine(Server.MapPath("~/App_Data"), fileName);
                    file.SaveAs(path);
                }


                System.Drawing.Image img = System.Drawing.Image.FromFile(path);
                System.Diagnostics.Debug.WriteLine("Width:" + img.Width + " - Height:" + img.Height);

                try
                {
                    // Initialize barcode reader
                    using (BarCodeReader reader = new BarCodeReader(path, DecodeType.AllSupportedTypes))
                    {
                        // Recognize barcodes on the image
                        foreach (var barcode in reader.ReadBarCodes())
                        {

                            readedBarcode = barcode.CodeText;
                        }
                    }

                }

                catch (Exception exp)
                {

                    System.Console.Write(exp.Message);
                }


            }
            catch (Exception ex)
            {
                ViewBag.Title = ex.Message;
            }
            return Json(readedBarcode);


        }
    }
}