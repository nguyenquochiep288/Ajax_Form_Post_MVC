using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DatabaseTHP;
using DatabaseTHP.Class;
using MVC_QuanLyTHP.Class;
using MVC_QuanLyTHP.Models;

namespace MVC_QuanLyTHP.Controllers
{

	public class Product_ComboController : Controller
	{
		[HttpGet]
		public ActionResult LoadProduct(string ID)
		{
			ApiResponse apiResponse = new ApiResponse();
			v_v_dm_HangHoa v_v_dm_HangHoa2 = new v_v_dm_HangHoa();
			apiResponse = Utility.GetDetail<v_v_dm_HangHoa>(Utility.LOC_ID + "/" + ID, "Product");
			if (!apiResponse.Success)
			{
				base.TempData["TitleError"] = apiResponse.Message;
				apiResponse.Success = false;
				apiResponse.URL = base.Url.Action("Index", "Notfound");
				return new JsonResult
				{
					Data = apiResponse,
					JsonRequestBehavior = JsonRequestBehavior.AllowGet,
					MaxJsonLength = int.MaxValue
				};
			}
			if (apiResponse.Data != null)
			{
				v_v_dm_HangHoa2 = apiResponse.Data as v_v_dm_HangHoa;
			}
			v_v_dm_HangHoa2.GIA = v_v_dm_HangHoa2.GIA01;
			v_v_dm_HangHoa2.GIA_QD = v_v_dm_HangHoa2.GIA01_QD;
			apiResponse.Detail = Utility.ConvertobjectTo((v_dm_HangHoa)v_v_dm_HangHoa2, "yyyy-MM-dd HH:mm:ss");
			return new JsonResult
			{
				Data = apiResponse,
				JsonRequestBehavior = JsonRequestBehavior.AllowGet,
				MaxJsonLength = int.MaxValue
			};
		}

		[HttpPost]
		[ValidateInput(false)]
		public ActionResult AddProductCombo([Bind(Include = "ID_HANGHOA,QTY,ID_DVT")] v_v_dm_HangHoa_Combo dm_HangHoa_Combo)
		{
			ApiResponse apiResponse = new ApiResponse();
			v_v_dm_HangHoa v_v_dm_HangHoa2 = new v_v_dm_HangHoa();
			apiResponse = Utility.GetDetail<v_v_dm_HangHoa>(Utility.LOC_ID + "/" + dm_HangHoa_Combo.ID_HANGHOA, "Product");
			if (!apiResponse.Success)
			{
				base.TempData["TitleError"] = apiResponse.Message;
				apiResponse.Success = false;
				apiResponse.URL = base.Url.Action("Index", "Notfound");
				return new JsonResult
				{
					Data = apiResponse,
					JsonRequestBehavior = JsonRequestBehavior.AllowGet,
					MaxJsonLength = int.MaxValue
				};
			}
			if (apiResponse.Data != null)
			{
				v_v_dm_HangHoa2 = apiResponse.Data as v_v_dm_HangHoa;
			}
			if (v_v_dm_HangHoa2 != null)
			{
				dm_HangHoa_Combo.NAME = v_v_dm_HangHoa2.NAME;
				dm_HangHoa_Combo.MA = v_v_dm_HangHoa2.MA;
				if (v_v_dm_HangHoa2.ID_DVT == dm_HangHoa_Combo.ID_DVT)
				{
					dm_HangHoa_Combo.NAME_DVT = v_v_dm_HangHoa2.NAME_DVT;
					if (!string.IsNullOrEmpty(v_v_dm_HangHoa2.ID_DVT_QD))
					{
						dm_HangHoa_Combo.TYLE_QD = v_v_dm_HangHoa2.TYLE_QD;
					}
					else if (v_v_dm_HangHoa2.LOAIHANGHOA == 2.ToString())
					{
						dm_HangHoa_Combo.TYLE_QD = 0.0;
					}
					else
					{
						dm_HangHoa_Combo.TYLE_QD = 1.0;
					}
				}
				else if (v_v_dm_HangHoa2.ID_DVT_QD == dm_HangHoa_Combo.ID_DVT && !string.IsNullOrEmpty(v_v_dm_HangHoa2.ID_DVT_QD))
				{
					dm_HangHoa_Combo.NAME_DVT = v_v_dm_HangHoa2.NAME_DVT_QD;
					dm_HangHoa_Combo.TYLE_QD = 1.0;
				}
				v_dm_HangHoa_Combo v_dm_HangHoa_Combo2 = Utility.LstProductCombo.Where((v_dm_HangHoa_Combo e) => e.ID_HANGHOA == dm_HangHoa_Combo.ID_HANGHOA && e.ID_DVT == dm_HangHoa_Combo.ID_DVT).FirstOrDefault();
				if (v_dm_HangHoa_Combo2 == null)
				{
					List<v_dm_HangHoa_Combo> lstProductCombo = Utility.LstProductCombo;
					lstProductCombo.Add(dm_HangHoa_Combo);
					base.Session["lstProductCombo"] = lstProductCombo;
				}
				else
				{
					v_dm_HangHoa_Combo2.QTY = dm_HangHoa_Combo.QTY;
				}
			}
			apiResponse.ProductCombo = Utility.GetProductCombo();
			return new JsonResult
			{
				Data = apiResponse,
				JsonRequestBehavior = JsonRequestBehavior.AllowGet,
				MaxJsonLength = int.MaxValue
			};
		}

		[HttpPost]
		public ActionResult DeleteProductCombo(string ID_HANGHOA, string ID_DVT)
		{
			ApiResponse apiResponse = new ApiResponse();
			v_v_dm_HangHoa v_v_dm_HangHoa2 = new v_v_dm_HangHoa();
			List<v_dm_HangHoa_Combo> lstProductCombo = Utility.LstProductCombo;
			v_dm_HangHoa_Combo v_dm_HangHoa_Combo2 = Utility.LstProductCombo.Where((v_dm_HangHoa_Combo e) => e.ID_HANGHOA == ID_HANGHOA && e.ID_DVT == ID_DVT).FirstOrDefault();
			if (v_dm_HangHoa_Combo2 != null)
			{
				lstProductCombo.Remove(v_dm_HangHoa_Combo2);
			}
			base.Session["lstProductCombo"] = lstProductCombo;
			apiResponse.ProductCombo = Utility.GetProductCombo();
			apiResponse.Success = true;
			return new JsonResult
			{
				Data = apiResponse,
				JsonRequestBehavior = JsonRequestBehavior.AllowGet,
				MaxJsonLength = int.MaxValue
			};
		}
	}
}
