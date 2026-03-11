using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using DatabaseTHP;
using DatabaseTHP.Class;
using DatabaseTHP.StoredProcedure.Parameter;
using MVC_QuanLyTHP.Class;
using MVC_QuanLyTHP.Models;

namespace MVC_QuanLyTHP.Controllers
{

	public class InventoryController : Controller
	{
		public ActionResult Index()
		{
			try
			{
				if (Utility.KiemTra())
				{
					return RedirectToAction("Index", "Admin");
				}
				ApiResponse apiResponse = new ApiResponse();
				List<v_ThongKeTonKhoHangHoa> iPagedList = new List<v_ThongKeTonKhoHangHoa>().OrderByDescending((v_ThongKeTonKhoHangHoa s) => s.NAME).ToList();
				v_v_ThongKeTonKhoHangHoa v_v_ThongKeTonKhoHangHoa2 = new v_v_ThongKeTonKhoHangHoa();
				v_v_ThongKeTonKhoHangHoa2.IPagedList = iPagedList;
				v_v_ThongKeTonKhoHangHoa2.lstdm_NhomHangHoa = new List<v_dm_NhomHangHoa>();
				v_v_ThongKeTonKhoHangHoa2.lstdm_NhomHangHoa = Utility.GetListData<v_dm_NhomHangHoa>("GroupProduct", "", "", Utility.LOC_ID).Data as List<v_dm_NhomHangHoa>;
				v_v_ThongKeTonKhoHangHoa2.lstdm_Kho = new List<v_dm_Kho>();
				v_v_ThongKeTonKhoHangHoa2.lstdm_Kho = Utility.GetListData<v_dm_Kho>("Depot", "", "", Utility.LOC_ID).Data as List<v_dm_Kho>;
				v_v_ThongKeTonKhoHangHoa2.lstdm_HangHoa = new List<v_dm_HangHoa>();
				v_v_ThongKeTonKhoHangHoa2.lstdm_HangHoa = Utility.GetListData<v_dm_HangHoa>("Product", "", "", Utility.LOC_ID).Data as List<v_dm_HangHoa>;
				v_v_ThongKeTonKhoHangHoa2.ISTHEOTHOIGIAN = true;
				v_v_ThongKeTonKhoHangHoa2.ISPHATSINHCONGNO = true;
				v_v_ThongKeTonKhoHangHoa2.ISPHATSINHCONGNOTRONGKY = false;
				v_v_ThongKeTonKhoHangHoa2.ISCONCONGNO = false;
				v_v_ThongKeTonKhoHangHoa2.ISCHITIET = false;
				v_v_ThongKeTonKhoHangHoa2.TUNGAY = DateTime.Now;
				v_v_ThongKeTonKhoHangHoa2.DENNGAY = DateTime.Now;
				return View(v_v_ThongKeTonKhoHangHoa2);
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
		[ValidateInput(false)]
		public ActionResult Index(SP_Parameter sp_Parameter)
		{
			try
			{
				if (Utility.KiemTra())
				{
					return RedirectToAction("Index", "Admin");
				}
				List<v_ThongKeTonKhoHangHoa> iPagedList = new List<v_ThongKeTonKhoHangHoa>().OrderByDescending((v_ThongKeTonKhoHangHoa s) => s.NAME).ToList();
				if (base.ModelState.IsValid)
				{
					ApiResponse apiResponse = new ApiResponse();
					sp_Parameter.LOC_ID = Utility.LOC_ID;
					sp_Parameter.ID_KHO = sp_Parameter.ID_KHO ?? "";
					sp_Parameter.ID_NHOMHANGHOA = sp_Parameter.ID_NHOMHANGHOA ?? "";
					sp_Parameter.ID_HANGHOA = sp_Parameter.ID_HANGHOA ?? "";
					sp_Parameter.ISTHEOTHOIGIAN = sp_Parameter.ISTHEOTHOIGIAN == true;
					sp_Parameter.ISPHATSINHCONGNO = sp_Parameter.ISPHATSINHCONGNO == true;
					sp_Parameter.ISPHATSINHCONGNOTRONGKY = sp_Parameter.ISPHATSINHCONGNOTRONGKY == true;
					sp_Parameter.ISCONCONGNO = sp_Parameter.ISCONCONGNO == true;
					sp_Parameter.ISCHITIET = sp_Parameter.ISCHITIET == true;
					iPagedList = new List<v_ThongKeTonKhoHangHoa>().OrderByDescending((v_ThongKeTonKhoHangHoa s) => s.NAME).ToList();
					apiResponse = Utility.Get_ThongKeTonKhoHangHoa<v_ThongKeTonKhoHangHoa>(sp_Parameter);
					if (!apiResponse.Success)
					{
						base.TempData["TitleError"] = apiResponse.Message;
						return RedirectToAction("Index", "Notfound");
					}
					if (apiResponse.Data != null)
					{
						iPagedList = (apiResponse.Data as List<v_ThongKeTonKhoHangHoa>).OrderByDescending((v_ThongKeTonKhoHangHoa s) => s.NAME).ToList();
					}
				}
				v_v_ThongKeTonKhoHangHoa v_v_ThongKeTonKhoHangHoa2 = new v_v_ThongKeTonKhoHangHoa();
				v_v_ThongKeTonKhoHangHoa2.IPagedList = iPagedList;
				v_v_ThongKeTonKhoHangHoa2.lstdm_NhomHangHoa = new List<v_dm_NhomHangHoa>();
				v_v_ThongKeTonKhoHangHoa2.lstdm_NhomHangHoa = Utility.GetListData<v_dm_NhomHangHoa>("GroupProduct", "", "", Utility.LOC_ID).Data as List<v_dm_NhomHangHoa>;
				v_v_ThongKeTonKhoHangHoa2.lstdm_Kho = new List<v_dm_Kho>();
				v_v_ThongKeTonKhoHangHoa2.lstdm_Kho = Utility.GetListData<v_dm_Kho>("Depot", "", "", Utility.LOC_ID).Data as List<v_dm_Kho>;
				v_v_ThongKeTonKhoHangHoa2.lstdm_HangHoa = new List<v_dm_HangHoa>();
				v_v_ThongKeTonKhoHangHoa2.lstdm_HangHoa = Utility.GetListData<v_dm_HangHoa>("Product", "", "", Utility.LOC_ID).Data as List<v_dm_HangHoa>;
				v_v_ThongKeTonKhoHangHoa2.ISTHEOTHOIGIAN = sp_Parameter.ISTHEOTHOIGIAN == true;
				v_v_ThongKeTonKhoHangHoa2.ID_HANGHOA = sp_Parameter.ID_HANGHOA;
				v_v_ThongKeTonKhoHangHoa2.ID_NHOMHANGHOA = sp_Parameter.ID_NHOMHANGHOA;
				v_v_ThongKeTonKhoHangHoa2.ID_KHO = sp_Parameter.ID_KHO;
				v_v_ThongKeTonKhoHangHoa2.ISPHATSINHCONGNO = sp_Parameter.ISPHATSINHCONGNO == true;
				v_v_ThongKeTonKhoHangHoa2.ISPHATSINHCONGNOTRONGKY = sp_Parameter.ISPHATSINHCONGNOTRONGKY == true;
				v_v_ThongKeTonKhoHangHoa2.ISCONCONGNO = sp_Parameter.ISCONCONGNO == true;
				v_v_ThongKeTonKhoHangHoa2.ISCHITIET = sp_Parameter.ISCHITIET == true;
				v_v_ThongKeTonKhoHangHoa2.TUNGAY = sp_Parameter.TUNGAY ?? DateTime.Now;
				v_v_ThongKeTonKhoHangHoa2.DENNGAY = sp_Parameter.DENNGAY ?? DateTime.Now;
				return View(v_v_ThongKeTonKhoHangHoa2);
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
