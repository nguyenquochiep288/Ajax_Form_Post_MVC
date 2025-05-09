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
using static System.Data.Entity.Infrastructure.Design.Executor;
using DatabaseTHP.StoredProcedure.Parameter;
using System.Data.SqlClient;

namespace MVC_QuanLyTHP.Controllers
{
    public class InventoryController : Controller
    {

        // GET: Inventory
        public ActionResult Index()
        {
            try
            {
                if (Utility.KiemTra())
                {
                    return RedirectToAction("Index", "Admin");
                }
                //if (!Utility.KiemTraQuyen(API.ThongKeTonKhoHangHoa, API.Xem))
                //{
                //    TempData["TitleError"] = API.TitlePermission;
                //    return RedirectToAction("Index", "Notfound");
                //}
                ApiResponse apiResponse = new ApiResponse();
                List<v_ThongKeTonKhoHangHoa> lstpage = (new List<v_ThongKeTonKhoHangHoa>()).OrderByDescending(s => s.NAME).ToList();               
                v_v_ThongKeTonKhoHangHoa ThongKeTonKhoHangHoa = new v_v_ThongKeTonKhoHangHoa();
                ThongKeTonKhoHangHoa.IPagedList = lstpage;
               
                ThongKeTonKhoHangHoa.lstdm_NhomHangHoa = new List<v_dm_NhomHangHoa>();
                ThongKeTonKhoHangHoa.lstdm_NhomHangHoa = Utility.GetListData<v_dm_NhomHangHoa>(API.dm_NhomHangHoa, "", "", Utility.LOC_ID).Data as List<v_dm_NhomHangHoa>;
                ThongKeTonKhoHangHoa.lstdm_Kho = new List<v_dm_Kho>();
                ThongKeTonKhoHangHoa.lstdm_Kho = Utility.GetListData<v_dm_Kho>(API.dm_Kho, "", "", Utility.LOC_ID).Data as List<v_dm_Kho>;
                ThongKeTonKhoHangHoa.lstdm_HangHoa = new List<v_dm_HangHoa>();
                ThongKeTonKhoHangHoa.lstdm_HangHoa = Utility.GetListData<v_dm_HangHoa>(API.dm_HangHoa, "", "", Utility.LOC_ID).Data as List<v_dm_HangHoa>;
                ThongKeTonKhoHangHoa.ISTHEOTHOIGIAN = true;
                ThongKeTonKhoHangHoa.ISPHATSINHCONGNO = true;
                ThongKeTonKhoHangHoa.ISPHATSINHCONGNOTRONGKY = false;
                ThongKeTonKhoHangHoa.ISCONCONGNO = false;
                ThongKeTonKhoHangHoa.TUNGAY = DateTime.Now;
                ThongKeTonKhoHangHoa.DENNGAY = DateTime.Now;
                return View(ThongKeTonKhoHangHoa);
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
                TempData["TitleError"] = API.TitleTryCatch;
                TempData["DetailError"] = ex.Message;
                return RedirectToAction("Index", "Notfound");
            }
        }


        [HttpPost, ValidateInput(false)]
        public ActionResult Index(SP_Parameter sp_Parameter)
        {
            try
            {
                if (Utility.KiemTra())
                {
                    return RedirectToAction("Index", "Admin");
                }
                //if (!Utility.KiemTraQuyen(API.dm_KhachHang, API.Create))
                //{
                //    TempData["TitleError"] = API.TitlePermission;
                //    return RedirectToAction("Index", "Notfound");
                //}
                List<v_ThongKeTonKhoHangHoa> lstpage = (new List<v_ThongKeTonKhoHangHoa>()).OrderByDescending(s => s.NAME).ToList();
                if (ModelState.IsValid)
                {
                    ApiResponse apiResponse = new ApiResponse();
                    sp_Parameter.LOC_ID = Utility.LOC_ID;
                    sp_Parameter.ID_KHO = sp_Parameter.ID_KHO ?? "";
                    sp_Parameter.ID_NHOMHANGHOA = sp_Parameter.ID_NHOMHANGHOA ?? "";
                    sp_Parameter.ID_HANGHOA = sp_Parameter.ID_HANGHOA ?? "";
                    sp_Parameter.ISTHEOTHOIGIAN = sp_Parameter.ISTHEOTHOIGIAN ?? false;
                    sp_Parameter.ISPHATSINHCONGNO = sp_Parameter.ISPHATSINHCONGNO ?? false;
                    sp_Parameter.ISPHATSINHCONGNOTRONGKY = sp_Parameter.ISPHATSINHCONGNOTRONGKY ?? false;
                    sp_Parameter.ISCONCONGNO = sp_Parameter.ISCONCONGNO ?? false;

                    lstpage = (new List<v_ThongKeTonKhoHangHoa>()).OrderByDescending(s => s.NAME).ToList();
                    apiResponse = Utility.Get_ThongKeTonKhoHangHoa<v_ThongKeTonKhoHangHoa>(sp_Parameter);
                    if (!apiResponse.Success)
                    {
                        TempData["TitleError"] = apiResponse.Message;
                        return RedirectToAction("Index", "Notfound");
                    }
                    if (apiResponse.Data != null)
                    {
                        lstpage = (apiResponse.Data as List<v_ThongKeTonKhoHangHoa>).OrderByDescending(s => s.NAME).ToList();
                    }
                }
                v_v_ThongKeTonKhoHangHoa ThongKeTonKhoHangHoa = new v_v_ThongKeTonKhoHangHoa();
                ThongKeTonKhoHangHoa.IPagedList = lstpage;
                ThongKeTonKhoHangHoa.lstdm_NhomHangHoa = new List<v_dm_NhomHangHoa>();
                ThongKeTonKhoHangHoa.lstdm_NhomHangHoa = Utility.GetListData<v_dm_NhomHangHoa>(API.dm_NhomHangHoa, "", "", Utility.LOC_ID).Data as List<v_dm_NhomHangHoa>;
                ThongKeTonKhoHangHoa.lstdm_Kho = new List<v_dm_Kho>();
                ThongKeTonKhoHangHoa.lstdm_Kho = Utility.GetListData<v_dm_Kho>(API.dm_Kho, "", "", Utility.LOC_ID).Data as List<v_dm_Kho>;
                ThongKeTonKhoHangHoa.lstdm_HangHoa = new List<v_dm_HangHoa>();
                ThongKeTonKhoHangHoa.lstdm_HangHoa = Utility.GetListData<v_dm_HangHoa>(API.dm_HangHoa, "", "", Utility.LOC_ID).Data as List<v_dm_HangHoa>;
                ThongKeTonKhoHangHoa.ISTHEOTHOIGIAN = sp_Parameter.ISTHEOTHOIGIAN ?? false;
                ThongKeTonKhoHangHoa.ID_HANGHOA = sp_Parameter.ID_HANGHOA;
                ThongKeTonKhoHangHoa.ID_NHOMHANGHOA = sp_Parameter.ID_NHOMHANGHOA;
                ThongKeTonKhoHangHoa.ID_KHO = sp_Parameter.ID_KHO;
                ThongKeTonKhoHangHoa.ISPHATSINHCONGNO = sp_Parameter.ISPHATSINHCONGNO ?? false;
                ThongKeTonKhoHangHoa.ISPHATSINHCONGNOTRONGKY = sp_Parameter.ISPHATSINHCONGNOTRONGKY ?? false;
                ThongKeTonKhoHangHoa.ISCONCONGNO = sp_Parameter.ISCONCONGNO ?? false;
                ThongKeTonKhoHangHoa.TUNGAY = sp_Parameter.TUNGAY ?? DateTime.Now;
                ThongKeTonKhoHangHoa.DENNGAY = sp_Parameter.DENNGAY ?? DateTime.Now;
                return View(ThongKeTonKhoHangHoa);
            }
            catch (Exception ex)
            {
                Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
                TempData["TitleError"] = API.TitleTryCatch;
                TempData["DetailError"] = ex.Message;
                return RedirectToAction("Index", "Notfound");
            }
        }
    }
}