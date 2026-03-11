using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using DatabaseTHP;
using DatabaseTHP.Class;
using MVC_QuanLyTHP.Class;
using MVC_QuanLyTHP.Models;

namespace MVC_QuanLyTHP.Controllers
{

	public class MapController : Controller
	{
		public ActionResult Index(string ID, string LATITUDE, string LONGITUDE)
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

		[HttpPost]
		public ActionResult GetListMap(string ID, string LATITUDE, string LONGITUDE)
		{
			List<Coordinates> list = new List<Coordinates>();
			if (Utility.KiemTra())
			{
				return RedirectToAction("Index", "Admin");
			}
			double num = 0.0;
			double num2 = 0.0;
			if (!string.IsNullOrEmpty(ID))
			{
				v_dm_KhachHang v_dm_KhachHang2 = new v_dm_KhachHang();
				ApiResponse detail = Utility.GetDetail<v_v_dm_KhachHang>(Utility.LOC_ID + "/" + ID, "Customer");
				if (!detail.Success)
				{
					base.TempData["TitleError"] = detail.Message;
					return RedirectToAction("Index", "Notfound");
				}
				if (detail.Data != null)
				{
					v_dm_KhachHang2 = detail.Data as v_v_dm_KhachHang;
				}
				if (v_dm_KhachHang2 != null && v_dm_KhachHang2.LATITUDE.HasValue && v_dm_KhachHang2.LATITUDE != 0.0)
				{
					Coordinates coordinates = new Coordinates();
					coordinates.Text = "<b>" + v_dm_KhachHang2.NAME + "</b><br><b>Vĩ độ:</b> " + v_dm_KhachHang2.LATITUDE + "<br><b>Kinh độ:</b> " + v_dm_KhachHang2.LONGITUDE;
					coordinates.Latitude = v_dm_KhachHang2.LATITUDE;
					coordinates.Longitude = v_dm_KhachHang2.LONGITUDE;
					list.Add(coordinates);
				}
			}
			if (!string.IsNullOrEmpty(LATITUDE) && !string.IsNullOrEmpty(LONGITUDE))
			{
				try
				{
					num = Convert.ToDouble(LATITUDE.Replace(".", ","));
					num2 = Convert.ToDouble(LONGITUDE.Replace(".", ","));
				}
				catch
				{
				}
				Coordinates coordinates2 = new Coordinates();
				if ((list.Count() == 0 && num != 0.0 && num2 != 0.0) || (list.Count() > 0 && list[0].Latitude != num && list[0].Longitude != num2))
				{
					coordinates2.Text = "Vị trí hiện tại";
					coordinates2.Latitude = num;
					coordinates2.Longitude = num2;
					list.Add(coordinates2);
				}
			}
			return Json(list, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		public ActionResult SetMap(double LATITUDE, double LONGITUDE)
		{
			Utility.Latitude = LATITUDE;
			Utility.Longitude = LONGITUDE;
			return Json("", JsonRequestBehavior.AllowGet);
		}
	}
}