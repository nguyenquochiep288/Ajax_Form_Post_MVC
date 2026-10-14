using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;
using DatabaseTHP;
using DatabaseTHP.Class;
using MVC_QuanLyTHP.Class;
using MVC_QuanLyTHP.Models;
using Newtonsoft.Json;
using PagedList;

namespace MVC_QuanLyTHP.Controllers
{

	public class PromotionController : Controller
	{
		[AllowAnonymous]
		public async Task<ActionResult> ForCustomers()
		{
			PromotionCustomerPageModel model = new PromotionCustomerPageModel();
			DateTime today = DateTime.Today;
			ViewBag.MonthTitle = today.ToString("'Tháng' MM 'năm' yyyy", System.Globalization.CultureInfo.GetCultureInfo("vi-VN"));

			try
			{
				string apiBaseUrl = Utility.URL.TrimEnd('/');
				using (HttpClient client = new HttpClient())
				{
					client.Timeout = TimeSpan.FromSeconds(15);
					HttpResponseMessage response = await client.GetAsync(apiBaseUrl + "/Promotion/Public/CurrentMonth");
					if (!response.IsSuccessStatusCode)
					{
						model.LoadError = "Hiện chưa thể tải chương trình khuyến mãi. Vui lòng thử lại sau.";
					}
					else
					{
						string json = await response.Content.ReadAsStringAsync();
						PromotionCustomerApiResponse apiResponse = JsonConvert.DeserializeObject<PromotionCustomerApiResponse>(json);
						if (apiResponse != null && apiResponse.Success && apiResponse.Data != null)
						{
							model.Promotions = apiResponse.Data;
						}
						else
						{
							model.LoadError = "Hiện chưa thể tải chương trình khuyến mãi. Vui lòng thử lại sau.";
						}
					}
				}
			}
			catch (Exception ex)
			{
				Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
				model.LoadError = "Hiện chưa thể tải chương trình khuyến mãi. Vui lòng thử lại sau.";
			}

			return View("ForCustomers", model);
		}

		public ActionResult PreviewCustomer(string id)
		{
			try
			{
				if (Utility.KiemTra())
				{
					return RedirectToAction("Index", "Admin");
				}
				if (!Utility.KiemTraQuyen("Promotion", "View"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				if (string.IsNullOrWhiteSpace(id))
				{
					return RedirectToAction("Index");
				}

				ApiResponse detail = Utility.GetDetail<v_v_dm_ChuongTrinhKhuyenMai>(Utility.LOC_ID + "/" + id, "Promotion");
				v_v_dm_ChuongTrinhKhuyenMai promotion = detail != null && detail.Success
					? detail.Data as v_v_dm_ChuongTrinhKhuyenMai
					: null;
				if (promotion == null)
				{
					base.TempData["TitleError"] = detail == null ? "Không thể tải chương trình khuyến mãi." : detail.Message;
					return RedirectToAction("Index", "Notfound");
				}

				List<v_dm_HangHoa> products = Utility.GetListData<v_dm_HangHoa>("Product", "", "", Utility.LOC_ID).Data as List<v_dm_HangHoa>;
				Dictionary<string, string> pictures = products == null
					? new Dictionary<string, string>()
					: products.Where(p => !string.IsNullOrWhiteSpace(p.ID)).GroupBy(p => p.ID).ToDictionary(g => g.Key, g => g.First().PICTURE);
				Func<string, string> getPicture = productId =>
				{
					string picture;
					return !string.IsNullOrWhiteSpace(productId) && pictures.TryGetValue(productId, out picture) ? picture : null;
				};

				PromotionCustomerItem item = new PromotionCustomerItem
				{
					MA = promotion.MA,
					NAME = promotion.NAME,
					TUNGAY = promotion.TUNGAY,
					DENNGAY = promotion.DENNGAY,
					CHIETKHAU = promotion.CHIETKHAU,
					TIENGIAM = promotion.TIENGIAM,
					IS_YEUCAUCHITIET = promotion.IS_YEUCAUCHITIET,
					TONGTIEN_DATKM = promotion.TONGTIEN_DATKM,
					SOLUONG_DATKM = promotion.SOLUONG_DATKM,
					NAME_DVT = promotion.NAME_DVT,
					ISTONGHOADON = promotion.ISTONGHOADON,
					ISTINHLUYTUYEN = promotion.ISTINHLUYTUYEN,
					Requirements = (promotion.lstdm_ChuongTrinhKhuyenMai_YeuCau ?? new List<v_dm_ChuongTrinhKhuyenMai_YeuCau>()).Select(r => new PromotionCustomerRequirement
					{
						NAME = r.NAME,
						SOLUONG = r.SOLUONG,
						NAME_DVT = r.NAME_DVT,
						SOTIEN = r.SOTIEN,
						CHIETKHAU = r.CHIETKHAU,
						TIENGIAM = r.TIENGIAM,
						NAME_HINHTHUC = r.NAME_HINHTHUC,
						PICTURE = getPicture(r.ID_HANGHOA)
					}).ToList(),
					Gifts = (promotion.lstdm_ChuongTrinhKhuyenMai_Tang ?? new List<v_dm_ChuongTrinhKhuyenMai_Tang>()).Select(g => new PromotionCustomerGift
					{
						NAME = g.NAME,
						SOLUONG = g.SOLUONG,
						NAME_DVT = g.NAME_DVT,
						PICTURE = getPicture(g.ID_HANGHOA)
					}).ToList()
				};

				PromotionCustomerPageModel model = new PromotionCustomerPageModel
				{
					Promotions = new List<PromotionCustomerItem> { item },
					PreviewMode = true
				};
				ViewBag.MonthTitle = DateTime.Today.ToString("'Tháng' MM 'năm' yyyy", System.Globalization.CultureInfo.GetCultureInfo("vi-VN"));
				return View("ForCustomers", model);
			}
			catch (Exception ex)
			{
				Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
				base.TempData["TitleError"] = "Không thể tải bản xem trước chương trình khuyến mãi.";
				return RedirectToAction("Index", "Notfound");
			}
		}

		public ActionResult Index(int Page = 1, string SearchString = "", string ShowSearchValue = "")
		{
			try
			{
				if (Utility.KiemTra())
				{
					return RedirectToAction("Index", "Admin");
				}
				if (!Utility.KiemTraQuyen("Promotion", "View"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				ShowSearchValue = Utility.GetShowSearchValue<dm_ChuongTrinhKhuyenMai>(ShowSearchValue);
				ApiResponse listData = Utility.GetListData<v_dm_ChuongTrinhKhuyenMai>("Promotion", ShowSearchValue, SearchString, Utility.LOC_ID);
				if (!listData.Success)
				{
					base.TempData["TitleError"] = listData.Message;
					return RedirectToAction("Index", "Notfound");
				}
				IPagedList<v_dm_ChuongTrinhKhuyenMai> iPagedList = (listData.Data as List<v_dm_ChuongTrinhKhuyenMai>).ToPagedList(Page, Utility.GetPageSize());
				v_v_dm_ChuongTrinhKhuyenMai v_v_dm_ChuongTrinhKhuyenMai2 = new v_v_dm_ChuongTrinhKhuyenMai();
				v_v_dm_ChuongTrinhKhuyenMai2.IPagedList = iPagedList;
				v_v_dm_ChuongTrinhKhuyenMai2.lstdm_DonViTinh = new List<v_dm_DonViTinh>();
				v_v_dm_ChuongTrinhKhuyenMai2.lstdm_DonViTinh = Utility.GetListData<v_dm_DonViTinh>("Unit", "", "", Utility.LOC_ID).Data as List<v_dm_DonViTinh>;
				base.ViewBag.searchValue = SearchString;
				base.ViewBag.showsearchValue = ShowSearchValue;
				base.ViewBag.PermissionEdit = Utility.KiemTraQuyen("Promotion", "Edit");
				base.ViewBag.PermissionDelete = Utility.KiemTraQuyen("Promotion", "Delete");
				base.ViewBag.PermissionCreate = Utility.KiemTraQuyen("Promotion", "Create");
				return View(v_v_dm_ChuongTrinhKhuyenMai2);
			}
			catch (Exception ex)
			{
				Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
				base.TempData["TitleError"] = "Đã có lỗi phát sinh trong phiên làm việc!";
				base.TempData["DetailError"] = ex.Message;
				return RedirectToAction("Index", "Notfound");
			}
		}

		public ActionResult Create(int type = 2)
		{
			try
			{
				base.Session["IntWidth"] = type;
				if (Utility.KiemTra())
				{
					return RedirectToAction("Index", "Admin");
				}
				if (!Utility.KiemTraQuyen("Promotion", "Create"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				v_v_dm_ChuongTrinhKhuyenMai v_v_dm_ChuongTrinhKhuyenMai2 = new v_v_dm_ChuongTrinhKhuyenMai();
				v_v_dm_ChuongTrinhKhuyenMai2.LOC_ID = Utility.LOC_ID;
				v_v_dm_ChuongTrinhKhuyenMai2.ID_NGUOITAO = base.Session["idUser"].ToString();
				v_v_dm_ChuongTrinhKhuyenMai2.THOIGIANTHEM = Utility.CurrentTime;
				v_v_dm_ChuongTrinhKhuyenMai2.ID = Guid.NewGuid().ToString();
				v_v_dm_ChuongTrinhKhuyenMai2.lstdm_DonViTinh = new List<v_dm_DonViTinh>();
				v_v_dm_ChuongTrinhKhuyenMai2.lstdm_DonViTinh = Utility.GetListData<v_dm_DonViTinh>("Unit", "", "", Utility.LOC_ID).Data as List<v_dm_DonViTinh>;
				return View(v_v_dm_ChuongTrinhKhuyenMai2);
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
		[ValidateAntiForgeryToken]
		public ActionResult Create([Bind(Include = "LOC_ID,ID,MA,NAME,TUNGAY,DENNGAY,CHIETKHAU,TIENGIAM,IS_YEUCAUCHITIET,TONGTIEN_DATKM,SOLUONG_DATKM,ID_DVT_DATKM,ISACTIVE,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO,ISTINHLUYTUYEN,ISTONGHOADON,SOLUONG_DATKM_DEN,TONGTIEN_DATKM_DEN,HINHTHUC_TINHKPI")] v_dm_ChuongTrinhKhuyenMai dm_ChuongTrinhKhuyenMai)
		{
			try
			{
				if (Utility.KiemTra())
				{
					return RedirectToAction("Index", "Admin");
				}
				if (!Utility.KiemTraQuyen("Promotion", "Create"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				if (base.ModelState.IsValid)
				{
					dm_ChuongTrinhKhuyenMai.LOC_ID = Utility.LOC_ID;
					dm_ChuongTrinhKhuyenMai.ID_NGUOITAO = base.Session["idUser"].ToString();
					dm_ChuongTrinhKhuyenMai.THOIGIANTHEM = Utility.CurrentTime;
					ApiResponse apiResponse = Utility.Create((dm_ChuongTrinhKhuyenMai)dm_ChuongTrinhKhuyenMai, "Promotion");
					if (apiResponse.Success)
					{
						return RedirectToAction("Index");
					}
					base.ModelState.AddModelError(string.Empty, apiResponse.Message);
					if (apiResponse.CheckValue)
					{
						base.ViewBag.ID = Guid.NewGuid().ToString();
					}
				}
				return View(dm_ChuongTrinhKhuyenMai);
			}
			catch (Exception ex)
			{
				Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
				base.TempData["TitleError"] = "Đã có lỗi phát sinh trong phiên làm việc!";
				base.TempData["DetailError"] = ex.Message;
				return RedirectToAction("Index", "Notfound");
			}
		}

		public ActionResult Edit(string id, int type = 2)
		{
			ApiResponse apiResponse = new ApiResponse();
			try
			{
				base.Session["IntWidth"] = type;
				if (Utility.KiemTra())
				{
					return RedirectToAction("Index", "Admin");
				}
				if (!Utility.KiemTraQuyen("Promotion", "Edit"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				v_v_dm_ChuongTrinhKhuyenMai v_v_dm_ChuongTrinhKhuyenMai2 = new v_v_dm_ChuongTrinhKhuyenMai();
				if (!string.IsNullOrEmpty(id))
				{
					apiResponse = Utility.GetDetail<v_dm_ChuongTrinhKhuyenMai>(Utility.LOC_ID + "/" + id, "Promotion");
					if (!apiResponse.Success)
					{
						base.TempData["TitleError"] = apiResponse.Message;
						return RedirectToAction("Index", "Notfound");
					}
					if (apiResponse.Data != null)
					{
						v_v_dm_ChuongTrinhKhuyenMai2 = apiResponse.Data as v_v_dm_ChuongTrinhKhuyenMai;
					}
				}
				v_v_dm_ChuongTrinhKhuyenMai2.lstdm_DonViTinh = new List<v_dm_DonViTinh>();
				v_v_dm_ChuongTrinhKhuyenMai2.lstdm_DonViTinh = Utility.GetListData<v_dm_DonViTinh>("Unit", "", "", Utility.LOC_ID).Data as List<v_dm_DonViTinh>;
				return View(v_v_dm_ChuongTrinhKhuyenMai2);
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
		[ValidateAntiForgeryToken]
		public ActionResult Edit([Bind(Include = "LOC_ID,ID,MA,NAME,TUNGAY,DENNGAY,CHIETKHAU,TIENGIAM,IS_YEUCAUCHITIET,TONGTIEN_DATKM,SOLUONG_DATKM,ID_DVT_DATKM,ISACTIVE,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO,ISTINHLUYTUYEN,ISTONGHOADON,SOLUONG_DATKM_DEN,TONGTIEN_DATKM_DEN,HINHTHUC_TINHKPI")] v_dm_ChuongTrinhKhuyenMai dm_ChuongTrinhKhuyenMai)
		{
			try
			{
				if (Utility.KiemTra())
				{
					return RedirectToAction("Index", "Admin");
				}
				if (!Utility.KiemTraQuyen("Promotion", "Edit"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				if (base.ModelState.IsValid)
				{
					dm_ChuongTrinhKhuyenMai.LOC_ID = Utility.LOC_ID;
					dm_ChuongTrinhKhuyenMai.ID_NGUOISUA = base.Session["idUser"].ToString();
					dm_ChuongTrinhKhuyenMai.THOIGIANSUA = Utility.CurrentTime;
					ApiResponse apiResponse = Utility.Edit(Utility.LOC_ID + "/" + dm_ChuongTrinhKhuyenMai.MA, dm_ChuongTrinhKhuyenMai, "Promotion");
					if (apiResponse.Success)
					{
						return RedirectToAction("Index");
					}
					base.ModelState.AddModelError(string.Empty, apiResponse.Message);
				}
				return View(dm_ChuongTrinhKhuyenMai);
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
		public ActionResult Delete(string id)
		{
			try
			{
				if (Utility.KiemTra())
				{
					return RedirectToAction("Index", "Admin");
				}
				if (!Utility.KiemTraQuyen("Promotion", "Delete"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				ApiResponse apiResponse = Utility.Delete<v_dm_ChuongTrinhKhuyenMai>(Utility.LOC_ID + "/" + id, "Promotion");
				if (apiResponse.Success)
				{
					return RedirectToAction("Index");
				}
				base.ModelState.AddModelError(string.Empty, apiResponse.Message);
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

		public ActionResult CreatePopup()
		{
			ApiResponse apiResponse = new ApiResponse();
			try
			{
				if (Utility.KiemTra())
				{
					apiResponse.Success = false;
					apiResponse.URL = base.Url.Action("Index", "Admin");
					return new JsonResult
					{
						Data = apiResponse,
						JsonRequestBehavior = JsonRequestBehavior.AllowGet,
						MaxJsonLength = int.MaxValue
					};
				}
				if (!Utility.KiemTraQuyen("Promotion", "Create"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					apiResponse.Success = false;
					apiResponse.URL = base.Url.Action("Index", "Notfound");
					return new JsonResult
					{
						Data = apiResponse,
						JsonRequestBehavior = JsonRequestBehavior.AllowGet,
						MaxJsonLength = int.MaxValue
					};
				}
				List<v_dm_ChuongTrinhKhuyenMai_YeuCau> value = new List<v_dm_ChuongTrinhKhuyenMai_YeuCau>();
				base.Session["lstCTKM_YeuCau"] = value;
				List<v_dm_ChuongTrinhKhuyenMai_Tang> value2 = new List<v_dm_ChuongTrinhKhuyenMai_Tang>();
				base.Session["lstCTKM_Tang"] = value2;
				v_v_dm_ChuongTrinhKhuyenMai v_v_dm_ChuongTrinhKhuyenMai2 = new v_v_dm_ChuongTrinhKhuyenMai();
				apiResponse.Success = true;
				v_v_dm_ChuongTrinhKhuyenMai2.LOC_ID = Utility.LOC_ID;
				v_v_dm_ChuongTrinhKhuyenMai2.TUNGAY = Utility.CurrentTime;
				v_v_dm_ChuongTrinhKhuyenMai2.DENNGAY = Utility.CurrentTime.AddMonths(1);
				v_v_dm_ChuongTrinhKhuyenMai2.ID = Guid.NewGuid().ToString();
				v_v_dm_ChuongTrinhKhuyenMai2.lstdm_DonViTinh = new List<v_dm_DonViTinh>();
				v_v_dm_ChuongTrinhKhuyenMai2.lstdm_DonViTinh = Utility.GetListData<v_dm_DonViTinh>("Unit", "", "", Utility.LOC_ID).Data as List<v_dm_DonViTinh>;
				List<ValueEdit> list = Utility.ConvertobjectToView(v_v_dm_ChuongTrinhKhuyenMai2);
				apiResponse.ProductCombo = Utility.GetCTKM_YeuCau();
				list.Add(new ValueEdit
				{
					Key = "tbodyTempItemdivPromotion_YC",
					Value = apiResponse.ProductCombo
				});
				apiResponse.ProductCombo = Utility.GetCTKM_Tang();
				list.Add(new ValueEdit
				{
					Key = "tbodyTempItemdivPromotion_Tang",
					Value = apiResponse.ProductCombo
				});
				apiResponse.Detail = list;
				return new JsonResult
				{
					Data = apiResponse,
					JsonRequestBehavior = JsonRequestBehavior.AllowGet,
					MaxJsonLength = int.MaxValue
				};
			}
			catch (Exception ex)
			{
				Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
				base.TempData["TitleError"] = "Đã có lỗi phát sinh trong phiên làm việc!";
				base.TempData["DetailError"] = ex.Message;
				apiResponse.Success = false;
				apiResponse.URL = base.Url.Action("Index", "Notfound");
				return new JsonResult
				{
					Data = apiResponse,
					JsonRequestBehavior = JsonRequestBehavior.AllowGet,
					MaxJsonLength = int.MaxValue
				};
			}
		}

		[HttpPost]
		[ValidateInput(false)]
		[ValidateAntiForgeryToken]
		public ActionResult CreatePopup([Bind(Include = "LOC_ID,ID,MA,NAME,TUNGAY,DENNGAY,CHIETKHAU,TIENGIAM,IS_YEUCAUCHITIET,TONGTIEN_DATKM,SOLUONG_DATKM,ID_DVT_DATKM,ISACTIVE,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO,ISTINHLUYTUYEN,ISTONGHOADON,SOLUONG_DATKM_DEN,TONGTIEN_DATKM_DEN,HINHTHUC_TINHKPI")] v_dm_ChuongTrinhKhuyenMai dm_ChuongTrinhKhuyenMai)
		{
			ApiResponse apiResponse = new ApiResponse();
			try
			{
				if (Utility.KiemTra())
				{
					apiResponse.Success = false;
					apiResponse.URL = base.Url.Action("Index", "Admin");
					return new JsonResult
					{
						Data = apiResponse,
						JsonRequestBehavior = JsonRequestBehavior.AllowGet,
						MaxJsonLength = int.MaxValue
					};
				}
				if (!Utility.KiemTraQuyen("Promotion", "Create"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					apiResponse.Success = false;
					apiResponse.URL = base.Url.Action("Index", "Notfound");
					return new JsonResult
					{
						Data = apiResponse,
						JsonRequestBehavior = JsonRequestBehavior.AllowGet,
						MaxJsonLength = int.MaxValue
					};
				}
				bool flag = false;
				if (base.ModelState.IsValid)
				{
					dm_ChuongTrinhKhuyenMai.LOC_ID = Utility.LOC_ID;
					dm_ChuongTrinhKhuyenMai.ID_NGUOITAO = base.Session["idUser"].ToString();
					dm_ChuongTrinhKhuyenMai.THOIGIANTHEM = Utility.CurrentTime;
					dm_ChuongTrinhKhuyenMai.lstdm_ChuongTrinhKhuyenMai_YeuCau = new List<v_dm_ChuongTrinhKhuyenMai_YeuCau>();
					dm_ChuongTrinhKhuyenMai.lstdm_ChuongTrinhKhuyenMai_Tang = new List<v_dm_ChuongTrinhKhuyenMai_Tang>();
					IEnumerable<string> enumerable = base.Request.Form.AllKeys.Where((string e) => e.StartsWith("txt") && !e.StartsWith("txtQuantity_Tang") && !e.StartsWith("txtMoney_Tang"));
					if (enumerable == null)
					{
						base.ModelState.AddModelError("lstdm_ChuongTrinhKhuyenMai_YeuCau", "Thêm sản phẩm trong yêu cầu.");
					}
					else
					{
						dm_ChuongTrinhKhuyenMai.lstdm_ChuongTrinhKhuyenMai_YeuCau = new List<v_dm_ChuongTrinhKhuyenMai_YeuCau>();
						v_dm_ChuongTrinhKhuyenMai_YeuCau v_dm_ChuongTrinhKhuyenMai_YeuCau2 = new v_dm_ChuongTrinhKhuyenMai_YeuCau();
						foreach (string item in enumerable)
						{
							string[] array = item.ToString().Split('|');
							string[] values = base.HttpContext.Request.Params.GetValues(item.ToString());
							string value = clsMaHoa.Decrypt(array[1].ToString(), "tmt6364");
							v_dm_ChuongTrinhKhuyenMai_YeuCau v_dm_ChuongTrinhKhuyenMai_YeuCau3 = JsonConvert.DeserializeObject<v_dm_ChuongTrinhKhuyenMai_YeuCau>(value);
							if (v_dm_ChuongTrinhKhuyenMai_YeuCau2.ID != v_dm_ChuongTrinhKhuyenMai_YeuCau3.ID)
							{
								v_dm_ChuongTrinhKhuyenMai_YeuCau2 = new v_dm_ChuongTrinhKhuyenMai_YeuCau();
								v_dm_ChuongTrinhKhuyenMai_YeuCau2 = JsonConvert.DeserializeObject<v_dm_ChuongTrinhKhuyenMai_YeuCau>(value);
								v_dm_ChuongTrinhKhuyenMai_YeuCau2.ISBATBUOC = false;
								v_dm_ChuongTrinhKhuyenMai_YeuCau2.ID_CHUONGTRINHKHUYENMAI = dm_ChuongTrinhKhuyenMai.ID;
								v_dm_ChuongTrinhKhuyenMai_YeuCau2.LOC_ID = dm_ChuongTrinhKhuyenMai.LOC_ID;
								dm_ChuongTrinhKhuyenMai.lstdm_ChuongTrinhKhuyenMai_YeuCau.Add(v_dm_ChuongTrinhKhuyenMai_YeuCau2);
							}
							string text = array[0].ToString().Substring(3, array[0].ToString().Length - 3);
							if (text == "Money_YC")
							{
								text = "SOTIEN";
							}
							if (text == "Quantity_YC")
							{
								text = "SOLUONG";
							}
							if (text == "CHIETKHAU_YC")
							{
								text = "CHIETKHAU";
							}
							if (text == "TIENGIAM_YC")
							{
								text = "TIENGIAM";
							}
							Utility.EditObject(v_dm_ChuongTrinhKhuyenMai_YeuCau2, text, values[0]);
						}
					}
					IEnumerable<string> enumerable2 = base.Request.Form.AllKeys.Where((string e) => e.StartsWith("txtQuantity_Tang|"));
					IEnumerable<string> enumerable3 = base.Request.Form.AllKeys.Where((string e) => e.StartsWith("txtMoney_Tang|"));
					if (enumerable2 != null && enumerable2.Count() != 0 && enumerable3 != null && enumerable3.Count() != 0)
					{
						int num = 0;
						foreach (string item2 in enumerable2)
						{
							string[] array2 = item2.ToString().Split('|');
							string[] values2 = base.HttpContext.Request.Params.GetValues(item2.ToString());
							string[] values3 = base.HttpContext.Request.Params.GetValues(enumerable3.ToList()[num].ToString());
							string value2 = clsMaHoa.Decrypt(array2[1].ToString(), "tmt6364");
							v_dm_ChuongTrinhKhuyenMai_Tang v_dm_ChuongTrinhKhuyenMai_Tang2 = JsonConvert.DeserializeObject<v_dm_ChuongTrinhKhuyenMai_Tang>(value2);
							if (array2 != null)
							{
								if (string.IsNullOrEmpty(v_dm_ChuongTrinhKhuyenMai_Tang2.ID))
								{
									v_dm_ChuongTrinhKhuyenMai_Tang2.ID = Guid.NewGuid().ToString();
								}
								v_dm_ChuongTrinhKhuyenMai_Tang2.LOC_ID = Utility.LOC_ID;
								v_dm_ChuongTrinhKhuyenMai_Tang2.ID_CHUONGTRINHKHUYENMAI = dm_ChuongTrinhKhuyenMai.ID;
								v_dm_ChuongTrinhKhuyenMai_Tang2.SOLUONG = Utility.ConvertStringToDouble(values2[0]);
								v_dm_ChuongTrinhKhuyenMai_Tang2.SOTIEN = Utility.ConvertStringToDouble(values3[0]);
								dm_ChuongTrinhKhuyenMai.lstdm_ChuongTrinhKhuyenMai_Tang.Add(v_dm_ChuongTrinhKhuyenMai_Tang2);
							}
							num++;
						}
					}
					apiResponse = Utility.Create(dm_ChuongTrinhKhuyenMai, "Promotion");
					if (apiResponse.Success)
					{
						if (apiResponse.Data != null)
						{
							dm_ChuongTrinhKhuyenMai = JsonConvert.DeserializeObject<v_dm_ChuongTrinhKhuyenMai>(apiResponse.Data.ToString());
						}
						apiResponse.NewID = Guid.NewGuid().ToString();
						List<v_dm_ChuongTrinhKhuyenMai_YeuCau> value3 = new List<v_dm_ChuongTrinhKhuyenMai_YeuCau>();
						base.Session["lstCTKM_YeuCau"] = value3;
						List<v_dm_ChuongTrinhKhuyenMai_Tang> value4 = new List<v_dm_ChuongTrinhKhuyenMai_Tang>();
						base.Session["lstCTKM_Tang"] = value4;
						flag = true;
					}
					else
					{
						base.ModelState.AddModelError(string.Empty, apiResponse.Message);
						if (apiResponse.CheckValue)
						{
							apiResponse.NewID = Guid.NewGuid().ToString();
						}
					}
				}
				else
				{
					apiResponse.Success = false;
					apiResponse.Data = Utility.GetModelState(base.ModelState, "Promotion");
				}
				apiResponse.ID = dm_ChuongTrinhKhuyenMai.ID;
				List<ValueEdit> list = Utility.ConvertobjectToView((dm_ChuongTrinhKhuyenMai)dm_ChuongTrinhKhuyenMai, "dd/MM/yyyy");
				if (flag)
				{
					apiResponse.ProductCombo = Utility.GetCTKM_YeuCau();
					list.Add(new ValueEdit
					{
						Key = "tbodyTempItemdivPromotion_YC",
						Value = apiResponse.ProductCombo
					});
					apiResponse.ProductCombo = Utility.GetCTKM_Tang();
					list.Add(new ValueEdit
					{
						Key = "tbodyTempItemdivPromotion_Tang",
						Value = apiResponse.ProductCombo
					});
				}
				apiResponse.Detail = list;
				return new JsonResult
				{
					Data = apiResponse,
					JsonRequestBehavior = JsonRequestBehavior.AllowGet,
					MaxJsonLength = int.MaxValue
				};
			}
			catch (Exception ex)
			{
				Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
				base.TempData["TitleError"] = "Đã có lỗi phát sinh trong phiên làm việc!";
				base.TempData["DetailError"] = ex.Message;
				apiResponse.Success = false;
				apiResponse.URL = base.Url.Action("Index", "Notfound");
				return new JsonResult
				{
					Data = apiResponse,
					JsonRequestBehavior = JsonRequestBehavior.AllowGet,
					MaxJsonLength = int.MaxValue
				};
			}
		}

		public ActionResult EditPopup(string id)
		{
			ApiResponse apiResponse = new ApiResponse();
			try
			{
				if (Utility.KiemTra())
				{
					apiResponse.Success = false;
					apiResponse.URL = base.Url.Action("Index", "Admin");
					return new JsonResult
					{
						Data = apiResponse,
						JsonRequestBehavior = JsonRequestBehavior.AllowGet,
						MaxJsonLength = int.MaxValue
					};
				}
				if (!Utility.KiemTraQuyen("Promotion", "Edit"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					apiResponse.Success = false;
					apiResponse.URL = base.Url.Action("Index", "Notfound");
					return new JsonResult
					{
						Data = apiResponse,
						JsonRequestBehavior = JsonRequestBehavior.AllowGet,
						MaxJsonLength = int.MaxValue
					};
				}
				v_v_dm_ChuongTrinhKhuyenMai v_v_dm_ChuongTrinhKhuyenMai2 = new v_v_dm_ChuongTrinhKhuyenMai();
				if (!string.IsNullOrEmpty(id))
				{
					apiResponse = Utility.GetDetail<v_v_dm_ChuongTrinhKhuyenMai>(Utility.LOC_ID + "/" + id, "Promotion");
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
						v_v_dm_ChuongTrinhKhuyenMai2 = apiResponse.Data as v_v_dm_ChuongTrinhKhuyenMai;
					}
				}
				apiResponse.Success = true;
				apiResponse.Success = true;
				v_v_dm_ChuongTrinhKhuyenMai2.lstdm_DonViTinh = new List<v_dm_DonViTinh>();
				v_v_dm_ChuongTrinhKhuyenMai2.lstdm_DonViTinh = Utility.GetListData<v_dm_DonViTinh>("Unit", "", "", Utility.LOC_ID).Data as List<v_dm_DonViTinh>;
				List<v_dm_ChuongTrinhKhuyenMai_YeuCau> list = new List<v_dm_ChuongTrinhKhuyenMai_YeuCau>();
				foreach (v_dm_ChuongTrinhKhuyenMai_YeuCau item in v_v_dm_ChuongTrinhKhuyenMai2.lstdm_ChuongTrinhKhuyenMai_YeuCau)
				{
					list.Add(item);
				}
				base.Session["lstCTKM_YeuCau"] = list;
				List<v_dm_ChuongTrinhKhuyenMai_Tang> list2 = new List<v_dm_ChuongTrinhKhuyenMai_Tang>();
				foreach (v_dm_ChuongTrinhKhuyenMai_Tang item2 in v_v_dm_ChuongTrinhKhuyenMai2.lstdm_ChuongTrinhKhuyenMai_Tang)
				{
					list2.Add(item2);
				}
				base.Session["lstCTKM_Tang"] = list2;
				List<ValueEdit> list3 = Utility.ConvertobjectTo(v_v_dm_ChuongTrinhKhuyenMai2);
				apiResponse.ProductCombo = Utility.GetCTKM_YeuCau();
				list3.Add(new ValueEdit
				{
					Key = "tbodyTempItemdivPromotion_YCEdit",
					Value = apiResponse.ProductCombo
				});
				apiResponse.ProductCombo = Utility.GetCTKM_Tang();
				list3.Add(new ValueEdit
				{
					Key = "tbodyTempItemdivPromotion_TangEdit",
					Value = apiResponse.ProductCombo
				});
				apiResponse.Detail = list3;
				return new JsonResult
				{
					Data = apiResponse,
					JsonRequestBehavior = JsonRequestBehavior.AllowGet,
					MaxJsonLength = int.MaxValue
				};
			}
			catch (Exception ex)
			{
				Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
				base.TempData["TitleError"] = "Đã có lỗi phát sinh trong phiên làm việc!";
				base.TempData["DetailError"] = ex.Message;
				apiResponse.Success = false;
				apiResponse.URL = base.Url.Action("Index", "Notfound");
				return new JsonResult
				{
					Data = apiResponse,
					JsonRequestBehavior = JsonRequestBehavior.AllowGet,
					MaxJsonLength = int.MaxValue
				};
			}
		}

		[HttpPost]
		[ValidateInput(false)]
		[ValidateAntiForgeryToken]
		public ActionResult EditPopup([Bind(Include = "LOC_ID,ID,MA,NAME,TUNGAY,DENNGAY,CHIETKHAU,TIENGIAM,IS_YEUCAUCHITIET,TONGTIEN_DATKM,SOLUONG_DATKM,ID_DVT_DATKM,ISACTIVE,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO,ISTINHLUYTUYEN,ISTONGHOADON,SOLUONG_DATKM_DEN,TONGTIEN_DATKM_DEN,HINHTHUC_TINHKPI")] v_dm_ChuongTrinhKhuyenMai dm_ChuongTrinhKhuyenMai)
		{
			ApiResponse apiResponse = new ApiResponse();
			try
			{
				if (Utility.KiemTra())
				{
					apiResponse.Success = false;
					apiResponse.URL = base.Url.Action("Index", "Admin");
					return new JsonResult
					{
						Data = apiResponse,
						JsonRequestBehavior = JsonRequestBehavior.AllowGet,
						MaxJsonLength = int.MaxValue
					};
				}
				if (!Utility.KiemTraQuyen("Promotion", "Edit"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					apiResponse.Success = false;
					apiResponse.URL = base.Url.Action("Index", "Notfound");
					return new JsonResult
					{
						Data = apiResponse,
						JsonRequestBehavior = JsonRequestBehavior.AllowGet,
						MaxJsonLength = int.MaxValue
					};
				}
				if (base.ModelState.IsValid)
				{
					dm_ChuongTrinhKhuyenMai.LOC_ID = Utility.LOC_ID;
					dm_ChuongTrinhKhuyenMai.ID_NGUOISUA = base.Session["idUser"].ToString();
					dm_ChuongTrinhKhuyenMai.THOIGIANSUA = Utility.CurrentTime;
					dm_ChuongTrinhKhuyenMai.lstdm_ChuongTrinhKhuyenMai_YeuCau = new List<v_dm_ChuongTrinhKhuyenMai_YeuCau>();
					dm_ChuongTrinhKhuyenMai.lstdm_ChuongTrinhKhuyenMai_Tang = new List<v_dm_ChuongTrinhKhuyenMai_Tang>();
					IEnumerable<string> enumerable = base.Request.Form.AllKeys.Where((string e) => e.StartsWith("txt") && !e.StartsWith("txtQuantity_Tang") && !e.StartsWith("txtMoney_Tang"));
					if (enumerable == null)
					{
						base.ModelState.AddModelError("lstdm_ChuongTrinhKhuyenMai_YeuCau", "Thêm sản phẩm trong yêu cầu.");
					}
					else
					{
						dm_ChuongTrinhKhuyenMai.lstdm_ChuongTrinhKhuyenMai_YeuCau = new List<v_dm_ChuongTrinhKhuyenMai_YeuCau>();
						v_dm_ChuongTrinhKhuyenMai_YeuCau v_dm_ChuongTrinhKhuyenMai_YeuCau2 = new v_dm_ChuongTrinhKhuyenMai_YeuCau();
						foreach (string item in enumerable)
						{
							string[] array = item.ToString().Split('|');
							string[] values = base.HttpContext.Request.Params.GetValues(item.ToString());
							string value = clsMaHoa.Decrypt(array[1].ToString(), "tmt6364");
							v_dm_ChuongTrinhKhuyenMai_YeuCau v_dm_ChuongTrinhKhuyenMai_YeuCau3 = JsonConvert.DeserializeObject<v_dm_ChuongTrinhKhuyenMai_YeuCau>(value);
							if (v_dm_ChuongTrinhKhuyenMai_YeuCau2.ID != v_dm_ChuongTrinhKhuyenMai_YeuCau3.ID)
							{
								v_dm_ChuongTrinhKhuyenMai_YeuCau2 = new v_dm_ChuongTrinhKhuyenMai_YeuCau();
								v_dm_ChuongTrinhKhuyenMai_YeuCau2 = JsonConvert.DeserializeObject<v_dm_ChuongTrinhKhuyenMai_YeuCau>(value);
								v_dm_ChuongTrinhKhuyenMai_YeuCau2.ISBATBUOC = false;
								v_dm_ChuongTrinhKhuyenMai_YeuCau2.ID_CHUONGTRINHKHUYENMAI = dm_ChuongTrinhKhuyenMai.ID;
								v_dm_ChuongTrinhKhuyenMai_YeuCau2.LOC_ID = dm_ChuongTrinhKhuyenMai.LOC_ID;
								dm_ChuongTrinhKhuyenMai.lstdm_ChuongTrinhKhuyenMai_YeuCau.Add(v_dm_ChuongTrinhKhuyenMai_YeuCau2);
							}
							string text = array[0].ToString().Substring(3, array[0].ToString().Length - 3);
							if (text == "Money_YC")
							{
								text = "SOTIEN";
							}
							if (text == "Quantity_YC")
							{
								text = "SOLUONG";
							}
							if (text == "CHIETKHAU_YC")
							{
								text = "CHIETKHAU";
							}
							if (text == "TIENGIAM_YC")
							{
								text = "TIENGIAM";
							}
							Utility.EditObject(v_dm_ChuongTrinhKhuyenMai_YeuCau2, text, values[0]);
						}
					}
					IEnumerable<string> enumerable2 = base.Request.Form.AllKeys.Where((string e) => e.StartsWith("txtQuantity_Tang|"));
					IEnumerable<string> source = base.Request.Form.AllKeys.Where((string e) => e.StartsWith("txtMoney_Tang|"));
					if (enumerable2 != null && enumerable2.Count() != 0)
					{
						int num = 0;
						foreach (string item2 in enumerable2)
						{
							string[] array2 = item2.ToString().Split('|');
							string[] values2 = base.HttpContext.Request.Params.GetValues(item2.ToString());
							string[] values3 = base.HttpContext.Request.Params.GetValues(source.ToList()[num].ToString());
							string value2 = clsMaHoa.Decrypt(array2[1].ToString(), "tmt6364");
							v_dm_ChuongTrinhKhuyenMai_Tang v_dm_ChuongTrinhKhuyenMai_Tang2 = JsonConvert.DeserializeObject<v_dm_ChuongTrinhKhuyenMai_Tang>(value2);
							if (array2 != null)
							{
								if (string.IsNullOrEmpty(v_dm_ChuongTrinhKhuyenMai_Tang2.ID))
								{
									v_dm_ChuongTrinhKhuyenMai_Tang2.ID = Guid.NewGuid().ToString();
								}
								v_dm_ChuongTrinhKhuyenMai_Tang2.LOC_ID = Utility.LOC_ID;
								v_dm_ChuongTrinhKhuyenMai_Tang2.ID_CHUONGTRINHKHUYENMAI = dm_ChuongTrinhKhuyenMai.ID;
								v_dm_ChuongTrinhKhuyenMai_Tang2.SOLUONG = Utility.ConvertStringToDouble(values2[0]);
								v_dm_ChuongTrinhKhuyenMai_Tang2.SOTIEN = Utility.ConvertStringToDouble(values3[0]);
								dm_ChuongTrinhKhuyenMai.lstdm_ChuongTrinhKhuyenMai_Tang.Add(v_dm_ChuongTrinhKhuyenMai_Tang2);
							}
							num++;
						}
					}
					apiResponse = Utility.Edit(Utility.LOC_ID + "/" + dm_ChuongTrinhKhuyenMai.MA, dm_ChuongTrinhKhuyenMai, "Promotion");
					if (apiResponse.Success)
					{
						if (apiResponse.Data != null)
						{
							dm_ChuongTrinhKhuyenMai = JsonConvert.DeserializeObject<v_dm_ChuongTrinhKhuyenMai>(apiResponse.Data.ToString());
						}
						apiResponse.ID = dm_ChuongTrinhKhuyenMai.ID;
					}
					else
					{
						base.ModelState.AddModelError(string.Empty, apiResponse.Message);
					}
				}
				else
				{
					apiResponse.Success = false;
					apiResponse.Data = Utility.GetModelState(base.ModelState, "Promotion");
				}
				apiResponse.Detail = Utility.ConvertobjectToView((dm_ChuongTrinhKhuyenMai)dm_ChuongTrinhKhuyenMai, "dd/MM/yyyy");
				return new JsonResult
				{
					Data = apiResponse,
					JsonRequestBehavior = JsonRequestBehavior.AllowGet,
					MaxJsonLength = int.MaxValue
				};
			}
			catch (Exception ex)
			{
				Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
				base.TempData["TitleError"] = "Đã có lỗi phát sinh trong phiên làm việc!";
				base.TempData["DetailError"] = ex.Message;
				apiResponse.Success = false;
				apiResponse.URL = base.Url.Action("Index", "Notfound");
				return new JsonResult
				{
					Data = apiResponse,
					JsonRequestBehavior = JsonRequestBehavior.AllowGet,
					MaxJsonLength = int.MaxValue
				};
			}
		}

		[HttpPost]
		public ActionResult DeletePopup(string id)
		{
			ApiResponse apiResponse = new ApiResponse();
			try
			{
				if (Utility.KiemTra())
				{
					apiResponse.Success = false;
					apiResponse.URL = base.Url.Action("Index", "Admin");
					return new JsonResult
					{
						Data = apiResponse,
						JsonRequestBehavior = JsonRequestBehavior.AllowGet,
						MaxJsonLength = int.MaxValue
					};
				}
				if (!Utility.KiemTraQuyen("Promotion", "Delete"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					apiResponse.Success = false;
					apiResponse.URL = base.Url.Action("Index", "Notfound");
					return new JsonResult
					{
						Data = apiResponse,
						JsonRequestBehavior = JsonRequestBehavior.AllowGet,
						MaxJsonLength = int.MaxValue
					};
				}
				apiResponse = Utility.Delete<v_dm_ChuongTrinhKhuyenMai>(Utility.LOC_ID + "/" + id, "Promotion");
				apiResponse.ID = id;
				return new JsonResult
				{
					Data = apiResponse,
					JsonRequestBehavior = JsonRequestBehavior.AllowGet,
					MaxJsonLength = int.MaxValue
				};
			}
			catch (Exception ex)
			{
				Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
				base.TempData["TitleError"] = "Đã có lỗi phát sinh trong phiên làm việc!";
				base.TempData["DetailError"] = ex.Message;
				apiResponse.Success = false;
				apiResponse.URL = base.Url.Action("Index", "Notfound");
				return new JsonResult
				{
					Data = apiResponse,
					JsonRequestBehavior = JsonRequestBehavior.AllowGet,
					MaxJsonLength = int.MaxValue
				};
			}
		}

		[HttpPost]
		[ValidateInput(false)]
		public ActionResult AddProductPromotion_YC([Bind(Include = "ID_HANGHOA,SOLUONG,ID_DVT")] v_v_dm_ChuongTrinhKhuyenMai_YeuCau dm_HangHoa_Combo)
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
				dm_HangHoa_Combo.ID = Guid.NewGuid().ToString();
				dm_HangHoa_Combo.HINHTHUC = 0;
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
				v_dm_ChuongTrinhKhuyenMai_YeuCau v_dm_ChuongTrinhKhuyenMai_YeuCau2 = Utility.LstCTKM_YeuCau.Where((v_dm_ChuongTrinhKhuyenMai_YeuCau e) => e.ID_HANGHOA == dm_HangHoa_Combo.ID_HANGHOA && e.ID_DVT == dm_HangHoa_Combo.ID_DVT).FirstOrDefault();
				if (v_dm_ChuongTrinhKhuyenMai_YeuCau2 == null)
				{
					List<v_dm_ChuongTrinhKhuyenMai_YeuCau> lstCTKM_YeuCau = Utility.LstCTKM_YeuCau;
					lstCTKM_YeuCau.Add(dm_HangHoa_Combo);
					base.Session["lstCTKM_YeuCau"] = lstCTKM_YeuCau;
				}
				else
				{
					v_dm_ChuongTrinhKhuyenMai_YeuCau2.SOLUONG = dm_HangHoa_Combo.SOLUONG;
					v_dm_ChuongTrinhKhuyenMai_YeuCau2.SOTIEN = dm_HangHoa_Combo.SOTIEN;
				}
			}
			apiResponse.ProductCombo = Utility.GetCTKM_YeuCau();
			return new JsonResult
			{
				Data = apiResponse,
				JsonRequestBehavior = JsonRequestBehavior.AllowGet,
				MaxJsonLength = int.MaxValue
			};
		}

		[HttpPost]
		[ValidateInput(false)]
		public ActionResult AddProductPromotionNHH_YC([Bind(Include = "ID_HANGHOA,SOLUONG,ID_DVT")] v_v_dm_ChuongTrinhKhuyenMai_YeuCau dm_CTKM_YC)
		{
			ApiResponse apiResponse = new ApiResponse();
			v_v_dm_NhomHangHoa v_v_dm_NhomHangHoa2 = new v_v_dm_NhomHangHoa();
			v_v_dm_DonViTinh v_v_dm_DonViTinh2 = new v_v_dm_DonViTinh();
			apiResponse = Utility.GetDetail<v_v_dm_NhomHangHoa>(Utility.LOC_ID + "/" + dm_CTKM_YC.ID_HANGHOA, "GroupProduct");
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
				v_v_dm_NhomHangHoa2 = apiResponse.Data as v_v_dm_NhomHangHoa;
			}
			apiResponse = Utility.GetDetail<v_v_dm_DonViTinh>(Utility.LOC_ID + "/" + dm_CTKM_YC.ID_DVT, "Unit");
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
				v_v_dm_DonViTinh2 = apiResponse.Data as v_v_dm_DonViTinh;
			}
			if (v_v_dm_NhomHangHoa2 != null && v_v_dm_DonViTinh2 != null)
			{
				dm_CTKM_YC.ID = Guid.NewGuid().ToString();
				dm_CTKM_YC.HINHTHUC = 1;
				dm_CTKM_YC.ID_HANGHOA = v_v_dm_NhomHangHoa2.ID;
				dm_CTKM_YC.NAME = v_v_dm_NhomHangHoa2.NAME;
				dm_CTKM_YC.MA = v_v_dm_NhomHangHoa2.MA;
				dm_CTKM_YC.ID_DVT = dm_CTKM_YC.ID_DVT;
				dm_CTKM_YC.NAME_DVT = v_v_dm_DonViTinh2.NAME;
				v_dm_ChuongTrinhKhuyenMai_YeuCau v_dm_ChuongTrinhKhuyenMai_YeuCau2 = Utility.LstCTKM_YeuCau.Where((v_dm_ChuongTrinhKhuyenMai_YeuCau e) => e.ID_HANGHOA == dm_CTKM_YC.ID_HANGHOA && e.ID_DVT == dm_CTKM_YC.ID_DVT).FirstOrDefault();
				if (v_dm_ChuongTrinhKhuyenMai_YeuCau2 == null)
				{
					List<v_dm_ChuongTrinhKhuyenMai_YeuCau> lstCTKM_YeuCau = Utility.LstCTKM_YeuCau;
					lstCTKM_YeuCau.Add(dm_CTKM_YC);
					base.Session["lstCTKM_YeuCau"] = lstCTKM_YeuCau;
				}
				else
				{
					v_dm_ChuongTrinhKhuyenMai_YeuCau2.SOLUONG = dm_CTKM_YC.SOLUONG;
					v_dm_ChuongTrinhKhuyenMai_YeuCau2.SOTIEN = dm_CTKM_YC.SOTIEN;
				}
			}
			apiResponse.ProductCombo = Utility.GetCTKM_YeuCau();
			return new JsonResult
			{
				Data = apiResponse,
				JsonRequestBehavior = JsonRequestBehavior.AllowGet,
				MaxJsonLength = int.MaxValue
			};
		}

		[HttpPost]
		public ActionResult DeleteProductPromotion_YC(string ID_HANGHOA, string ID_DVT)
		{
			ApiResponse apiResponse = new ApiResponse();
			v_v_dm_HangHoa v_v_dm_HangHoa2 = new v_v_dm_HangHoa();
			List<v_dm_ChuongTrinhKhuyenMai_YeuCau> lstCTKM_YeuCau = Utility.LstCTKM_YeuCau;
			v_dm_ChuongTrinhKhuyenMai_YeuCau v_dm_ChuongTrinhKhuyenMai_YeuCau2 = Utility.LstCTKM_YeuCau.Where((v_dm_ChuongTrinhKhuyenMai_YeuCau e) => e.ID_HANGHOA == ID_HANGHOA && e.ID_DVT == ID_DVT).FirstOrDefault();
			if (v_dm_ChuongTrinhKhuyenMai_YeuCau2 != null)
			{
				lstCTKM_YeuCau.Remove(v_dm_ChuongTrinhKhuyenMai_YeuCau2);
			}
			base.Session["lstCTKM_YeuCau"] = lstCTKM_YeuCau;
			apiResponse.ProductCombo = Utility.GetCTKM_YeuCau();
			apiResponse.Success = true;
			return new JsonResult
			{
				Data = apiResponse,
				JsonRequestBehavior = JsonRequestBehavior.AllowGet,
				MaxJsonLength = int.MaxValue
			};
		}

		[HttpPost]
		[ValidateInput(false)]
		public ActionResult AddProductPromotion_Tang([Bind(Include = "ID_HANGHOA,SOLUONG,ID_DVT")] v_v_dm_ChuongTrinhKhuyenMai_Tang dm_HangHoa_Combo)
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
				}
				else if (v_v_dm_HangHoa2.ID_DVT_QD == dm_HangHoa_Combo.ID_DVT && !string.IsNullOrEmpty(v_v_dm_HangHoa2.ID_DVT_QD))
				{
					dm_HangHoa_Combo.NAME_DVT = v_v_dm_HangHoa2.NAME_DVT_QD;
					dm_HangHoa_Combo.TYLE_QD = 1.0;
				}
				v_dm_ChuongTrinhKhuyenMai_Tang v_dm_ChuongTrinhKhuyenMai_Tang2 = Utility.LstCTKM_Tang.Where((v_dm_ChuongTrinhKhuyenMai_Tang e) => e.ID_HANGHOA == dm_HangHoa_Combo.ID_HANGHOA && e.ID_DVT == dm_HangHoa_Combo.ID_DVT).FirstOrDefault();
				if (v_dm_ChuongTrinhKhuyenMai_Tang2 == null)
				{
					List<v_dm_ChuongTrinhKhuyenMai_Tang> lstCTKM_Tang = Utility.LstCTKM_Tang;
					lstCTKM_Tang.Add(dm_HangHoa_Combo);
					base.Session["lstCTKM_Tang"] = lstCTKM_Tang;
				}
				else
				{
					v_dm_ChuongTrinhKhuyenMai_Tang2.SOLUONG = dm_HangHoa_Combo.SOLUONG;
				}
			}
			apiResponse.ProductCombo = Utility.GetCTKM_Tang();
			return new JsonResult
			{
				Data = apiResponse,
				JsonRequestBehavior = JsonRequestBehavior.AllowGet,
				MaxJsonLength = int.MaxValue
			};
		}

		[HttpPost]
		public ActionResult DeleteProductPromotion_Tang(string ID_HANGHOA, string ID_DVT)
		{
			ApiResponse apiResponse = new ApiResponse();
			v_v_dm_HangHoa v_v_dm_HangHoa2 = new v_v_dm_HangHoa();
			List<v_dm_ChuongTrinhKhuyenMai_Tang> lstCTKM_Tang = Utility.LstCTKM_Tang;
			v_dm_ChuongTrinhKhuyenMai_Tang v_dm_ChuongTrinhKhuyenMai_Tang2 = Utility.LstCTKM_Tang.Where((v_dm_ChuongTrinhKhuyenMai_Tang e) => e.ID_HANGHOA == ID_HANGHOA && e.ID_DVT == ID_DVT).FirstOrDefault();
			if (v_dm_ChuongTrinhKhuyenMai_Tang2 != null)
			{
				lstCTKM_Tang.Remove(v_dm_ChuongTrinhKhuyenMai_Tang2);
			}
			base.Session["lstCTKM_Tang"] = lstCTKM_Tang;
			apiResponse.ProductCombo = Utility.GetCTKM_Tang();
			apiResponse.Success = true;
			return new JsonResult
			{
				Data = apiResponse,
				JsonRequestBehavior = JsonRequestBehavior.AllowGet,
				MaxJsonLength = int.MaxValue
			};
		}
	}

	public class PromotionCustomerPageModel
	{
		public List<PromotionCustomerItem> Promotions { get; set; } = new List<PromotionCustomerItem>();
		public string LoadError { get; set; }
		public bool PreviewMode { get; set; }
	}

	public class PromotionCustomerApiResponse
	{
		public bool Success { get; set; }
		public string Message { get; set; }
		public List<PromotionCustomerItem> Data { get; set; }
	}

	public class PromotionCustomerItem
	{
		public string MA { get; set; }
		public string NAME { get; set; }
		public DateTime TUNGAY { get; set; }
		public DateTime DENNGAY { get; set; }
		public double CHIETKHAU { get; set; }
		public double TIENGIAM { get; set; }
		public bool IS_YEUCAUCHITIET { get; set; }
		public double TONGTIEN_DATKM { get; set; }
		public double SOLUONG_DATKM { get; set; }
		public string NAME_DVT { get; set; }
		public bool ISTONGHOADON { get; set; }
		public bool ISTINHLUYTUYEN { get; set; }
		public List<PromotionCustomerGift> Gifts { get; set; } = new List<PromotionCustomerGift>();
		public List<PromotionCustomerRequirement> Requirements { get; set; } = new List<PromotionCustomerRequirement>();
	}

	public class PromotionCustomerGift
	{
		public string NAME { get; set; }
		public double SOLUONG { get; set; }
		public string NAME_DVT { get; set; }
		public string PICTURE { get; set; }
	}

	public class PromotionCustomerRequirement
	{
		public string NAME { get; set; }
		public double SOLUONG { get; set; }
		public string NAME_DVT { get; set; }
		public double SOTIEN { get; set; }
		public double CHIETKHAU { get; set; }
		public double TIENGIAM { get; set; }
		public string NAME_HINHTHUC { get; set; }
		public string PICTURE { get; set; }
	}
}
