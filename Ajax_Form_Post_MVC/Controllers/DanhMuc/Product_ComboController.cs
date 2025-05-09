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
using System.IO;
using Newtonsoft.Json;
using System.Security.Cryptography;

namespace MVC_QuanLyTHP.Controllers
{
    public class Product_ComboController : Controller
    {
        [HttpGet]
        public ActionResult LoadProduct(string ID)
        {
            ApiResponse apiResponse = new ApiResponse();
            v_v_dm_HangHoa dm_HangHoa = new v_v_dm_HangHoa();
            apiResponse = Utility.GetDetail<v_v_dm_HangHoa>(Utility.LOC_ID + "/" + ID, API.dm_HangHoa);
            if (!apiResponse.Success)
            {

                TempData["TitleError"] = apiResponse.Message;
                apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
            }
            if (apiResponse.Data != null)
                dm_HangHoa = apiResponse.Data as v_v_dm_HangHoa;

            dm_HangHoa.GIA = dm_HangHoa.GIA01;
            dm_HangHoa.GIA_QD = dm_HangHoa.GIA01_QD;
            
            apiResponse.Detail = Utility.ConvertobjectTo<v_dm_HangHoa>(dm_HangHoa);
            return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult AddProductCombo([Bind(Include = "ID_HANGHOA,QTY,ID_DVT")] v_v_dm_HangHoa_Combo dm_HangHoa_Combo)
        {
            ApiResponse apiResponse = new ApiResponse();
            v_v_dm_HangHoa dm_HangHoa = new v_v_dm_HangHoa();
            apiResponse = Utility.GetDetail<v_v_dm_HangHoa>(Utility.LOC_ID + "/" + dm_HangHoa_Combo.ID_HANGHOA, API.dm_HangHoa);
            if (!apiResponse.Success)
            {
                TempData["TitleError"] = apiResponse.Message;
                apiResponse.Success = false; apiResponse.URL = Url.Action("Index", "Notfound");
                return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
            }
            if (apiResponse.Data != null)
                dm_HangHoa = apiResponse.Data as v_v_dm_HangHoa;

            if (dm_HangHoa != null)
            {
                dm_HangHoa_Combo.NAME = dm_HangHoa.NAME;
                dm_HangHoa_Combo.MA = dm_HangHoa.MA;
                if (dm_HangHoa.ID_DVT == dm_HangHoa_Combo.ID_DVT)
                {
                    dm_HangHoa_Combo.NAME_DVT = dm_HangHoa.NAME_DVT;
                    if (!string.IsNullOrEmpty(dm_HangHoa.ID_DVT_QD))
                    {
                        dm_HangHoa_Combo.TYLE_QD = dm_HangHoa.TYLE_QD;
                    }
                    else
                    {
                        if (dm_HangHoa.LOAIHANGHOA == ((int)API.LoaiSanPham.KhongQuanLyTonKho).ToString())
                            dm_HangHoa_Combo.TYLE_QD = 0;
                        else
                            dm_HangHoa_Combo.TYLE_QD = 1;

                    }
                }
                else if (dm_HangHoa.ID_DVT_QD == dm_HangHoa_Combo.ID_DVT)
                {
                    if (!string.IsNullOrEmpty(dm_HangHoa.ID_DVT_QD))
                    {
                        dm_HangHoa_Combo.NAME_DVT = dm_HangHoa.NAME_DVT_QD;
                        dm_HangHoa_Combo.TYLE_QD = 1;
                    }
                }

                var check = Utility.LstProductCombo.Where(e => e.ID_HANGHOA == dm_HangHoa_Combo.ID_HANGHOA && e.ID_DVT == dm_HangHoa_Combo.ID_DVT).FirstOrDefault();
                if (check == null)
                {
                    var LstProductComboAdd =  Utility.LstProductCombo;
                    LstProductComboAdd.Add(dm_HangHoa_Combo);
                    Session[Sessions.lstProductCombo] = LstProductComboAdd;
                }
                else
                {
                    check.QTY = dm_HangHoa_Combo.QTY;
                }    
            }
            apiResponse.ProductCombo = Utility.GetProductCombo();
            return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
        }

        [HttpPost]
        public ActionResult DeleteProductCombo(string ID_HANGHOA, string ID_DVT)
        {
            ApiResponse apiResponse = new ApiResponse();
            v_v_dm_HangHoa dm_HangHoa = new v_v_dm_HangHoa();
            var LstProductComboAdd = Utility.LstProductCombo;
            var check = Utility.LstProductCombo.Where(e => e.ID_HANGHOA == ID_HANGHOA && e.ID_DVT == ID_DVT).FirstOrDefault();
            if (check != null)
                LstProductComboAdd.Remove(check);

            Session[Sessions.lstProductCombo] = LstProductComboAdd;
            apiResponse.ProductCombo = Utility.GetProductCombo();
            apiResponse.Success = true;
            return new JsonResult() { Data = apiResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
        }
    }
}