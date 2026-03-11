using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using DatabaseTHP;
using DatabaseTHP.Class;
using MVC_QuanLyTHP.Class;
using MVC_QuanLyTHP.Models;
using Newtonsoft.Json;
using PagedList;

namespace MVC_QuanLyTHP.Controllers
{

	public class ProductPriceRangeController : Controller
	{
		public ActionResult Index(int Page = 1, string SearchString = "", string ShowSearchValue = "")
		{
			try
			{
				if (Utility.KiemTra())
				{
					return RedirectToAction("Index", "Admin");
				}
				if (!Utility.KiemTraQuyen("ProductPriceRange", "View"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				ShowSearchValue = "";
				string text = ShowSearchValue;
				ApiResponse listData = Utility.GetListData<v_dm_HangHoa_KhungGia_Master>("ProductPriceRange", "ALL", SearchString, Utility.LOC_ID);
				if (!listData.Success)
				{
					base.TempData["TitleError"] = listData.Message;
					return RedirectToAction("Index", "Notfound");
				}
				List<v_dm_HangHoa_KhungGia_Master> list = new List<v_dm_HangHoa_KhungGia_Master>();
				list = (listData.Data as List<v_dm_HangHoa_KhungGia_Master>).ToList();
				IPagedList<v_dm_HangHoa_KhungGia_Master> iPagedList = list.ToPagedList(Page, Utility.GetPageSize());
				v_v_dm_HangHoa_KhungGia_Master v_v_dm_HangHoa_KhungGia_Master2 = new v_v_dm_HangHoa_KhungGia_Master();
				v_v_dm_HangHoa_KhungGia_Master2.IPagedList = iPagedList;
				v_v_dm_HangHoa_KhungGia_Master2.lstdm_HangHoa = new List<v_dm_HangHoa>();
				v_v_dm_HangHoa_KhungGia_Master2.lstdm_HangHoa = Utility.GetListData<v_dm_HangHoa>("Product", "", "", Utility.LOC_ID).Data as List<v_dm_HangHoa>;
				base.ViewBag.searchValue = SearchString;
				base.ViewBag.showsearchValue = ShowSearchValue;
				base.ViewBag.PermissionEdit = Utility.KiemTraQuyen("ProductPriceRange", "Edit");
				base.ViewBag.PermissionDelete = Utility.KiemTraQuyen("ProductPriceRange", "Delete");
				base.ViewBag.PermissionCreate = Utility.KiemTraQuyen("ProductPriceRange", "Create");
				return View(v_v_dm_HangHoa_KhungGia_Master2);
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
				if (!Utility.KiemTraQuyen("ProductPriceRange", "Create"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				v_v_dm_HangHoa_KhungGia_Master v_v_dm_HangHoa_KhungGia_Master2 = new v_v_dm_HangHoa_KhungGia_Master();
				v_v_dm_HangHoa_KhungGia_Master2.LOC_ID = Utility.LOC_ID;
				v_v_dm_HangHoa_KhungGia_Master2.ID = Guid.NewGuid().ToString();
				v_v_dm_HangHoa_KhungGia_Master2.lstdm_HangHoa = new List<v_dm_HangHoa>();
				v_v_dm_HangHoa_KhungGia_Master2.lstdm_HangHoa = Utility.GetListData<v_dm_HangHoa>("Product", "", "", Utility.LOC_ID).Data as List<v_dm_HangHoa>;
				return View(v_v_dm_HangHoa_KhungGia_Master2);
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
		public ActionResult Create([Bind(Include = "LOC_ID,ID,MA,NAME,ISACTIVE")] v_dm_HangHoa_KhungGia_Master dm_HangHoa_KhungGia)
		{
			try
			{
				if (Utility.KiemTra())
				{
					return RedirectToAction("Index", "Admin");
				}
				if (!Utility.KiemTraQuyen("ProductPriceRange", "Create"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				if (base.ModelState.IsValid)
				{
					dm_HangHoa_KhungGia.LOC_ID = Utility.LOC_ID;
					ApiResponse apiResponse = Utility.Create(dm_HangHoa_KhungGia, "ProductPriceRange");
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
				return View(dm_HangHoa_KhungGia);
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
				if (!Utility.KiemTraQuyen("ProductPriceRange", "Edit"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				v_v_dm_HangHoa_KhungGia_Master v_v_dm_HangHoa_KhungGia_Master2 = new v_v_dm_HangHoa_KhungGia_Master();
				if (!string.IsNullOrEmpty(id))
				{
					apiResponse = Utility.GetDetail<v_v_dm_HangHoa_KhungGia_Master>(Utility.LOC_ID + "/" + id, "ProductPriceRange");
					if (!apiResponse.Success)
					{
						base.TempData["TitleError"] = apiResponse.Message;
						return RedirectToAction("Index", "Notfound");
					}
					if (apiResponse.Data != null)
					{
						v_v_dm_HangHoa_KhungGia_Master2 = apiResponse.Data as v_v_dm_HangHoa_KhungGia_Master;
					}
				}
				v_v_dm_HangHoa_KhungGia_Master2.lstdm_HangHoa = new List<v_dm_HangHoa>();
				v_v_dm_HangHoa_KhungGia_Master2.lstdm_HangHoa = Utility.GetListData<v_dm_HangHoa>("Product", "", "", Utility.LOC_ID).Data as List<v_dm_HangHoa>;
				return View(v_v_dm_HangHoa_KhungGia_Master2);
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
		public ActionResult Edit([Bind(Include = "LOC_ID,ID,MA,NAME,ISACTIVE")] v_v_dm_HangHoa_KhungGia_Master dm_HangHoa_KhungGia)
		{
			try
			{
				if (Utility.KiemTra())
				{
					return RedirectToAction("Index", "Admin");
				}
				if (!Utility.KiemTraQuyen("ProductPriceRange", "Edit"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				if (base.ModelState.IsValid)
				{
					dm_HangHoa_KhungGia.LOC_ID = Utility.LOC_ID;
					ApiResponse apiResponse = Utility.Edit(Utility.LOC_ID + "/" + dm_HangHoa_KhungGia.ID, dm_HangHoa_KhungGia, "ProductPriceRange");
					if (apiResponse.Success)
					{
						return RedirectToAction("Index");
					}
					base.ModelState.AddModelError(string.Empty, apiResponse.Message);
				}
				return View(dm_HangHoa_KhungGia);
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
				if (!Utility.KiemTraQuyen("ProductPriceRange", "Delete"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				ApiResponse apiResponse = Utility.Delete<v_v_dm_HangHoa_KhungGia_Master>(Utility.LOC_ID + "/" + id, "ProductPriceRange");
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
				if (!Utility.KiemTraQuyen("ProductPriceRange", "Create"))
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
				v_v_dm_HangHoa_KhungGia_Master v_v_dm_HangHoa_KhungGia_Master2 = new v_v_dm_HangHoa_KhungGia_Master();
				apiResponse.Success = true;
				v_v_dm_HangHoa_KhungGia_Master2.LOC_ID = Utility.LOC_ID;
				v_v_dm_HangHoa_KhungGia_Master2.ID = Guid.NewGuid().ToString();
				v_v_dm_HangHoa_KhungGia_Master2.ISACTIVE = true;
				v_v_dm_HangHoa_KhungGia_Master2.lstdm_HangHoa = new List<v_dm_HangHoa>();
				v_v_dm_HangHoa_KhungGia_Master2.lstdm_HangHoa = Utility.GetListData<v_dm_HangHoa>("Product", "", "", Utility.LOC_ID).Data as List<v_dm_HangHoa>;
				List<v_dm_HangHoa_KhungGia> value = new List<v_dm_HangHoa_KhungGia>();
				List<v_dm_HangHoa_KhungGia_HangHoa> list = new List<v_dm_HangHoa_KhungGia_HangHoa>();
				base.Session["lstProductPriceRange"] = value;
				base.Session["lstProductPriceRangeHangHoa"] = value;
				List<ValueEdit> list2 = Utility.ConvertobjectTo(v_v_dm_HangHoa_KhungGia_Master2);
				apiResponse.ProductCombo = Utility.GetProductPriceRange(new List<v_dm_DonViTinh>());
				list2.Add(new ValueEdit
				{
					Key = "tbodyTempItemdivPromotion_YC",
					Value = apiResponse.ProductCombo
				});
				apiResponse.ProductCombo = Utility.GetProductPriceRange_HangHoa();
				list2.Add(new ValueEdit
				{
					Key = "tbodyTempItemdivPromotion_Tang",
					Value = apiResponse.ProductCombo
				});
				apiResponse.Detail = list2;
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
		public ActionResult CreatePopup([Bind(Include = "LOC_ID,ID,MA,NAME,ISACTIVE")] v_v_dm_HangHoa_KhungGia_Master dm_HangHoa_KhungGia)
		{
			ApiResponse apiResponse = new ApiResponse();
			try
			{
				bool flag = false;
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
				if (!Utility.KiemTraQuyen("ProductPriceRange", "Create"))
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
					dm_HangHoa_KhungGia.LOC_ID = Utility.LOC_ID;
					dm_HangHoa_KhungGia.lstdm_HangHoa_KhungGia = new List<dm_HangHoa_KhungGia>();
					dm_HangHoa_KhungGia.lstdm_HangHoa_KhungGia_HangHoa = new List<v_dm_HangHoa_KhungGia_HangHoa>();
					IEnumerable<string> source = base.Request.Form.AllKeys.Where((string e) => e.StartsWith("HINHTHUC_TINHKPI|"));
					IEnumerable<string> enumerable = base.Request.Form.AllKeys.Where((string e) => e.StartsWith("txtTU|"));
					IEnumerable<string> enumerable2 = base.Request.Form.AllKeys.Where((string e) => e.StartsWith("txtDEN|"));
					IEnumerable<string> source2 = base.Request.Form.AllKeys.Where((string e) => e.StartsWith("txtDONGIA|"));
					IEnumerable<string> enumerable3 = base.Request.Form.AllKeys.Where((string e) => e.StartsWith("txtTIEN_KPI|"));
					IEnumerable<string> enumerable4 = base.Request.Form.AllKeys.Where((string e) => e.StartsWith("txtCK_KPI|"));
					IEnumerable<string> source3 = base.Request.Form.AllKeys.Where((string e) => e.StartsWith("ID_DVT|"));
					if (enumerable == null || enumerable.Count() == 0 || enumerable2 == null || enumerable2.Count() == 0 || enumerable3 == null || enumerable3.Count() == 0 || enumerable4 == null || enumerable4.Count() == 0)
					{
						base.ModelState.AddModelError("lstdm_HangHoa_KhungGia", "Thêm khung giá.");
					}
					else
					{
						int num = 0;
						foreach (string item in enumerable)
						{
							string[] array = item.ToString().Split('|');
							string[] values = base.HttpContext.Request.Params.GetValues(item.ToString());
							string[] values2 = base.HttpContext.Request.Params.GetValues(enumerable2.ToList()[num].ToString());
							string[] values3 = base.HttpContext.Request.Params.GetValues(source2.ToList()[num].ToString());
							string[] values4 = base.HttpContext.Request.Params.GetValues(enumerable3.ToList()[num].ToString());
							string[] values5 = base.HttpContext.Request.Params.GetValues(enumerable4.ToList()[num].ToString());
							string[] values6 = base.HttpContext.Request.Params.GetValues(source.ToList()[num].ToString());
							string[] values7 = base.HttpContext.Request.Params.GetValues(source3.ToList()[num].ToString());
							string value = clsMaHoa.Decrypt(array[1].ToString(), "tmt6364");
							dm_HangHoa_KhungGia dm_HangHoa_KhungGia2 = JsonConvert.DeserializeObject<dm_HangHoa_KhungGia>(value);
							if (array != null)
							{
								if (string.IsNullOrEmpty(dm_HangHoa_KhungGia2.ID))
								{
									dm_HangHoa_KhungGia2.ID = Guid.NewGuid().ToString();
								}
								dm_HangHoa_KhungGia2.ISACTIVE = true;
								dm_HangHoa_KhungGia2.LOC_ID = Utility.LOC_ID;
								dm_HangHoa_KhungGia2.TU = Utility.ConvertStringToDouble(values[0]);
								dm_HangHoa_KhungGia2.DEN = Utility.ConvertStringToDouble(values2[0]);
								dm_HangHoa_KhungGia2.DONGIA = Utility.ConvertStringToDouble(values3[0]);
								dm_HangHoa_KhungGia2.TIEN_KPI = Utility.ConvertStringToDouble(values4[0]);
								dm_HangHoa_KhungGia2.CK_KPI = Utility.ConvertStringToDouble(values5[0]);
								dm_HangHoa_KhungGia2.HINHTHUC_TINHKPI = Convert.ToInt32(Utility.ConvertStringToDouble(values6[0]));
								dm_HangHoa_KhungGia2.ID_DVT = values7[0].ToString();
								dm_HangHoa_KhungGia2.ISACTIVE = dm_HangHoa_KhungGia.ISACTIVE;
								dm_HangHoa_KhungGia2.ID_HANGHOA_KHUNGGIA_MASTER = dm_HangHoa_KhungGia.ID;
								dm_HangHoa_KhungGia.lstdm_HangHoa_KhungGia.Add(dm_HangHoa_KhungGia2);
							}
							num++;
						}
					}
					IEnumerable<string> enumerable5 = base.Request.Form.AllKeys.Where((string e) => e.StartsWith("txtISACTIVE|"));
					if (enumerable5 == null || enumerable5.Count() == 0)
					{
						base.ModelState.AddModelError("lstdm_HangHoa_KhungGia_HangHoa", "Thêm sản phẩm.");
					}
					else
					{
						int num2 = 0;
						foreach (string item2 in enumerable5)
						{
							string[] array2 = item2.ToString().Split('|');
							string[] values8 = base.HttpContext.Request.Params.GetValues(item2.ToString());
							string value2 = clsMaHoa.Decrypt(array2[1].ToString(), "tmt6364");
							v_dm_HangHoa_KhungGia_HangHoa v_dm_HangHoa_KhungGia_HangHoa2 = JsonConvert.DeserializeObject<v_dm_HangHoa_KhungGia_HangHoa>(value2);
							if (array2 != null)
							{
								if (string.IsNullOrEmpty(v_dm_HangHoa_KhungGia_HangHoa2.ID))
								{
									v_dm_HangHoa_KhungGia_HangHoa2.ID = Guid.NewGuid().ToString();
								}
								v_dm_HangHoa_KhungGia_HangHoa2.LOC_ID = Utility.LOC_ID;
								v_dm_HangHoa_KhungGia_HangHoa2.ID_HANGHOA_KHUNGGIA_MASTER = dm_HangHoa_KhungGia.ID;
								dm_HangHoa_KhungGia.lstdm_HangHoa_KhungGia_HangHoa.Add(v_dm_HangHoa_KhungGia_HangHoa2);
							}
							num2++;
						}
					}
					apiResponse = Utility.Create(dm_HangHoa_KhungGia, "ProductPriceRange");
					if (apiResponse.Success)
					{
						if (apiResponse.Data != null)
						{
							dm_HangHoa_KhungGia = JsonConvert.DeserializeObject<v_v_dm_HangHoa_KhungGia_Master>(apiResponse.Data.ToString());
						}
						apiResponse.NewID = Guid.NewGuid().ToString();
						List<v_dm_HangHoa_KhungGia> value3 = new List<v_dm_HangHoa_KhungGia>();
						List<v_dm_HangHoa_KhungGia_HangHoa> value4 = new List<v_dm_HangHoa_KhungGia_HangHoa>();
						base.Session["lstProductPriceRange"] = value3;
						base.Session["lstProductPriceRangeHangHoa"] = value4;
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
					apiResponse.Data = Utility.GetModelState(base.ModelState, "ProductPriceRange");
				}
				apiResponse.ID = dm_HangHoa_KhungGia.ID;
				List<ValueEdit> list = Utility.ConvertobjectTo(dm_HangHoa_KhungGia);
				if (flag)
				{
					List<v_dm_DonViTinh> list2 = new List<v_dm_DonViTinh>();
					list2 = Utility.GetListData<v_dm_DonViTinh>("Unit", "", "", Utility.LOC_ID).Data as List<v_dm_DonViTinh>;
					apiResponse.ProductCombo = Utility.GetProductPriceRange(list2);
					list.Add(new ValueEdit
					{
						Key = "tbodyTempItemdivPromotion_YC",
						Value = apiResponse.ProductCombo
					});
					apiResponse.ProductCombo = Utility.GetProductPriceRange_HangHoa();
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
				if (!Utility.KiemTraQuyen("ProductPriceRange", "Edit"))
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
				v_v_dm_HangHoa_KhungGia_Master v_v_dm_HangHoa_KhungGia_Master2 = new v_v_dm_HangHoa_KhungGia_Master();
				if (!string.IsNullOrEmpty(id))
				{
					apiResponse = Utility.GetDetail<v_v_dm_HangHoa_KhungGia_Master>(Utility.LOC_ID + "/" + id, "ProductPriceRange");
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
						v_v_dm_HangHoa_KhungGia_Master2 = apiResponse.Data as v_v_dm_HangHoa_KhungGia_Master;
					}
				}
				apiResponse.Success = true;
				v_v_dm_HangHoa_KhungGia_Master2.lstdm_HangHoa = new List<v_dm_HangHoa>();
				v_v_dm_HangHoa_KhungGia_Master2.lstdm_HangHoa = Utility.GetListData<v_dm_HangHoa>("Product", "", "", Utility.LOC_ID).Data as List<v_dm_HangHoa>;
				List<dm_HangHoa_KhungGia> list = new List<dm_HangHoa_KhungGia>();
				foreach (dm_HangHoa_KhungGia item in v_v_dm_HangHoa_KhungGia_Master2.lstdm_HangHoa_KhungGia.OrderBy((dm_HangHoa_KhungGia s) => s.TU))
				{
					list.Add(item);
				}
				base.Session["lstProductPriceRange"] = list;
				List<v_dm_HangHoa_KhungGia_HangHoa> list2 = new List<v_dm_HangHoa_KhungGia_HangHoa>();
				foreach (v_dm_HangHoa_KhungGia_HangHoa item2 in v_v_dm_HangHoa_KhungGia_Master2.lstdm_HangHoa_KhungGia_HangHoa)
				{
					list2.Add(item2);
				}
				base.Session["lstProductPriceRangeHangHoa"] = list2;
				List<ValueEdit> list3 = Utility.ConvertobjectTo(v_v_dm_HangHoa_KhungGia_Master2);
				List<v_dm_DonViTinh> list4 = new List<v_dm_DonViTinh>();
				list4 = Utility.GetListData<v_dm_DonViTinh>("Unit", "", "", Utility.LOC_ID).Data as List<v_dm_DonViTinh>;
				apiResponse.ProductCombo = Utility.GetProductPriceRange(list4);
				list3.Add(new ValueEdit
				{
					Key = "tbodyTempItemdivPromotion_YCEdit",
					Value = apiResponse.ProductCombo
				});
				apiResponse.ProductCombo = Utility.GetProductPriceRange_HangHoa();
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
		public ActionResult EditPopup([Bind(Include = "LOC_ID,ID,MA,NAME,ISACTIVE")] v_v_dm_HangHoa_KhungGia_Master dm_HangHoa_KhungGia)
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
				if (!Utility.KiemTraQuyen("ProductPriceRange", "Edit"))
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
					dm_HangHoa_KhungGia.LOC_ID = Utility.LOC_ID;
					dm_HangHoa_KhungGia.lstdm_HangHoa_KhungGia = new List<dm_HangHoa_KhungGia>();
					dm_HangHoa_KhungGia.lstdm_HangHoa_KhungGia_HangHoa = new List<v_dm_HangHoa_KhungGia_HangHoa>();
					IEnumerable<string> source = base.Request.Form.AllKeys.Where((string e) => e.StartsWith("HINHTHUC_TINHKPI|"));
					IEnumerable<string> enumerable = base.Request.Form.AllKeys.Where((string e) => e.StartsWith("txtTU|"));
					IEnumerable<string> enumerable2 = base.Request.Form.AllKeys.Where((string e) => e.StartsWith("txtDEN|"));
					IEnumerable<string> source2 = base.Request.Form.AllKeys.Where((string e) => e.StartsWith("txtDONGIA|"));
					IEnumerable<string> enumerable3 = base.Request.Form.AllKeys.Where((string e) => e.StartsWith("txtTIEN_KPI|"));
					IEnumerable<string> enumerable4 = base.Request.Form.AllKeys.Where((string e) => e.StartsWith("txtCK_KPI|"));
					IEnumerable<string> source3 = base.Request.Form.AllKeys.Where((string e) => e.StartsWith("ID_DVT|"));
					if (enumerable == null || enumerable.Count() == 0 || enumerable2 == null || enumerable2.Count() == 0 || enumerable3 == null || enumerable3.Count() == 0 || enumerable4 == null || enumerable4.Count() == 0)
					{
						base.ModelState.AddModelError("lstdm_HangHoa_KhungGia", "Thêm khung giá.");
					}
					else
					{
						int num = 0;
						foreach (string item in enumerable)
						{
							string[] array = item.ToString().Split('|');
							string[] values = base.HttpContext.Request.Params.GetValues(item.ToString());
							string[] values2 = base.HttpContext.Request.Params.GetValues(enumerable2.ToList()[num].ToString());
							string[] values3 = base.HttpContext.Request.Params.GetValues(source2.ToList()[num].ToString());
							string[] values4 = base.HttpContext.Request.Params.GetValues(enumerable3.ToList()[num].ToString());
							string[] values5 = base.HttpContext.Request.Params.GetValues(enumerable4.ToList()[num].ToString());
							string[] values6 = base.HttpContext.Request.Params.GetValues(source.ToList()[num].ToString());
							string[] values7 = base.HttpContext.Request.Params.GetValues(source3.ToList()[num].ToString());
							string value = clsMaHoa.Decrypt(array[1].ToString(), "tmt6364");
							dm_HangHoa_KhungGia dm_HangHoa_KhungGia2 = new dm_HangHoa_KhungGia();
							dm_HangHoa_KhungGia2 = JsonConvert.DeserializeObject<dm_HangHoa_KhungGia>(value);
							if (array != null)
							{
								if (string.IsNullOrEmpty(dm_HangHoa_KhungGia2.ID))
								{
									dm_HangHoa_KhungGia2.ID = Guid.NewGuid().ToString();
								}
								dm_HangHoa_KhungGia2.ISACTIVE = true;
								dm_HangHoa_KhungGia2.LOC_ID = Utility.LOC_ID;
								dm_HangHoa_KhungGia2.TU = Utility.ConvertStringToDouble(values[0]);
								dm_HangHoa_KhungGia2.DEN = Utility.ConvertStringToDouble(values2[0]);
								dm_HangHoa_KhungGia2.DONGIA = Utility.ConvertStringToDouble(values3[0]);
								dm_HangHoa_KhungGia2.TIEN_KPI = Utility.ConvertStringToDouble(values4[0]);
								dm_HangHoa_KhungGia2.CK_KPI = Utility.ConvertStringToDouble(values5[0]);
								dm_HangHoa_KhungGia2.HINHTHUC_TINHKPI = Convert.ToInt32(Utility.ConvertStringToDouble(values6[0]));
								dm_HangHoa_KhungGia2.ID_DVT = values7[0].ToString();
								dm_HangHoa_KhungGia2.ISACTIVE = dm_HangHoa_KhungGia.ISACTIVE;
								dm_HangHoa_KhungGia2.ID_HANGHOA_KHUNGGIA_MASTER = dm_HangHoa_KhungGia.ID;
								dm_HangHoa_KhungGia.lstdm_HangHoa_KhungGia.Add(dm_HangHoa_KhungGia2);
							}
							num++;
						}
					}
					base.Session["lstProductPriceRange"] = dm_HangHoa_KhungGia.lstdm_HangHoa_KhungGia;
					IEnumerable<string> enumerable5 = base.Request.Form.AllKeys.Where((string e) => e.StartsWith("txtISACTIVE|"));
					if (enumerable5 == null || enumerable5.Count() == 0)
					{
						base.ModelState.AddModelError("lstdm_HangHoa_KhungGia_HangHoa", "Thêm sản phẩm.");
					}
					else
					{
						int num2 = 0;
						foreach (string item2 in enumerable5)
						{
							string[] array2 = item2.ToString().Split('|');
							string[] values8 = base.HttpContext.Request.Params.GetValues(item2.ToString());
							string value2 = clsMaHoa.Decrypt(array2[1].ToString(), "tmt6364");
							v_dm_HangHoa_KhungGia_HangHoa v_dm_HangHoa_KhungGia_HangHoa2 = JsonConvert.DeserializeObject<v_dm_HangHoa_KhungGia_HangHoa>(value2);
							if (array2 != null)
							{
								if (string.IsNullOrEmpty(v_dm_HangHoa_KhungGia_HangHoa2.ID))
								{
									v_dm_HangHoa_KhungGia_HangHoa2.ID = Guid.NewGuid().ToString();
								}
								v_dm_HangHoa_KhungGia_HangHoa2.LOC_ID = Utility.LOC_ID;
								v_dm_HangHoa_KhungGia_HangHoa2.ID_HANGHOA_KHUNGGIA_MASTER = dm_HangHoa_KhungGia.ID;
								dm_HangHoa_KhungGia.lstdm_HangHoa_KhungGia_HangHoa.Add(v_dm_HangHoa_KhungGia_HangHoa2);
							}
							num2++;
						}
					}
					base.Session["lstProductPriceRangeHangHoa"] = dm_HangHoa_KhungGia.lstdm_HangHoa_KhungGia_HangHoa;
					apiResponse = Utility.Edit(Utility.LOC_ID + "/" + dm_HangHoa_KhungGia.ID, dm_HangHoa_KhungGia, "ProductPriceRange");
					if (apiResponse.Success)
					{
						if (apiResponse.Data != null)
						{
							dm_HangHoa_KhungGia = JsonConvert.DeserializeObject<v_v_dm_HangHoa_KhungGia_Master>(apiResponse.Data.ToString());
						}
						apiResponse.ID = dm_HangHoa_KhungGia.ID;
					}
					else
					{
						base.ModelState.AddModelError(string.Empty, apiResponse.Message);
					}
				}
				else
				{
					apiResponse.Success = false;
					apiResponse.Data = Utility.GetModelState(base.ModelState, "ProductPriceRange");
				}
				List<ValueEdit> list = Utility.ConvertobjectToView(dm_HangHoa_KhungGia);
				List<v_dm_DonViTinh> list2 = new List<v_dm_DonViTinh>();
				list2 = Utility.GetListData<v_dm_DonViTinh>("Unit", "", "", Utility.LOC_ID).Data as List<v_dm_DonViTinh>;
				apiResponse.ProductCombo = Utility.GetProductPriceRange(list2);
				list.Add(new ValueEdit
				{
					Key = "tbodyTempItemdivPromotion_YCEdit",
					Value = apiResponse.ProductCombo
				});
				apiResponse.ProductCombo = Utility.GetProductPriceRange_HangHoa();
				list.Add(new ValueEdit
				{
					Key = "tbodyTempItemdivPromotion_TangEdit",
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
				if (!Utility.KiemTraQuyen("ProductPriceRange", "Delete"))
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
				apiResponse = Utility.Delete<v_dm_HangHoa_KhungGia_Master>(Utility.LOC_ID + "/" + id, "ProductPriceRange");
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
		public ActionResult DeleteProductPromotion_YC(string ID_HANGHOA, string ID_DVT)
		{
			ApiResponse apiResponse = new ApiResponse();
			v_dm_HangHoa v_dm_HangHoa2 = new v_dm_HangHoa();
			List<dm_HangHoa_KhungGia> lstProductPriceRange = Utility.LstProductPriceRange;
			dm_HangHoa_KhungGia dm_HangHoa_KhungGia2 = Utility.LstProductPriceRange.Where((dm_HangHoa_KhungGia e) => e.ID == ID_DVT).FirstOrDefault();
			if (dm_HangHoa_KhungGia2 != null)
			{
				lstProductPriceRange.Remove(dm_HangHoa_KhungGia2);
			}
			base.Session["lstProductPriceRange"] = lstProductPriceRange;
			List<v_dm_DonViTinh> list = new List<v_dm_DonViTinh>();
			list = Utility.GetListData<v_dm_DonViTinh>("Unit", "", "", Utility.LOC_ID).Data as List<v_dm_DonViTinh>;
			apiResponse.ProductCombo = Utility.GetProductPriceRange(list);
			apiResponse.Success = true;
			return new JsonResult
			{
				Data = apiResponse,
				JsonRequestBehavior = JsonRequestBehavior.AllowGet,
				MaxJsonLength = int.MaxValue
			};
		}

		[HttpPost]
		public ActionResult AddProductPriceRange(string ID_HANGHOA)
		{
			ApiResponse apiResponse = new ApiResponse();
			if (string.IsNullOrEmpty(ID_HANGHOA))
			{
				v_dm_HangHoa dm_HangHoa2 = new v_dm_HangHoa();
				List<v_dm_HangHoa_KhungGia_HangHoa> lstProductPriceRangeHangHoa = Utility.LstProductPriceRangeHangHoa;
				ID_HANGHOA = ((lstProductPriceRangeHangHoa != null && lstProductPriceRangeHangHoa.Count > 0) ? lstProductPriceRangeHangHoa.FirstOrDefault().ID_HANGHOA : "");
				dm_HangHoa2 = Utility.GetDetail<v_dm_HangHoa>(Utility.LOC_ID + "/" + ID_HANGHOA, "Product").Data as v_dm_HangHoa;
				List<dm_HangHoa_KhungGia> lstProductPriceRange = Utility.LstProductPriceRange;
				v_dm_HangHoa_KhungGia v_dm_HangHoa_KhungGia2 = new v_dm_HangHoa_KhungGia();
				List<v_dm_DonViTinh> list = new List<v_dm_DonViTinh>();
				v_dm_HangHoa_KhungGia2.ID = Guid.NewGuid().ToString();
				v_dm_HangHoa_KhungGia2.ID_DVT = dm_HangHoa2?.ID_DVT;
				v_dm_HangHoa_KhungGia2.DONGIA = ((dm_HangHoa2 != null) ? dm_HangHoa2.GIA01 : 0.0);
				lstProductPriceRange.Add(v_dm_HangHoa_KhungGia2);
				base.Session["lstProductPriceRange"] = lstProductPriceRange;
				list = Utility.GetListData<v_dm_DonViTinh>("Unit", "", "", Utility.LOC_ID).Data as List<v_dm_DonViTinh>;
				if (dm_HangHoa2 != null)
				{
					list = list.Where((v_dm_DonViTinh s) => s.ID == dm_HangHoa2.ID_DVT || s.ID == dm_HangHoa2.ID_DVT_QD).ToList();
				}
				apiResponse.ProductCombo = Utility.GetProductPriceRange(list);
				apiResponse.Success = true;
			}
			else
			{
				v_dm_HangHoa v_dm_HangHoa2 = new v_dm_HangHoa();
				List<v_dm_HangHoa_KhungGia_HangHoa> lstProductPriceRangeHangHoa2 = Utility.LstProductPriceRangeHangHoa;
				v_dm_HangHoa_KhungGia_HangHoa v_dm_HangHoa_KhungGia_HangHoa2 = Utility.LstProductPriceRangeHangHoa.Where((v_dm_HangHoa_KhungGia_HangHoa e) => e.ID_HANGHOA == ID_HANGHOA).FirstOrDefault();
				if (v_dm_HangHoa_KhungGia_HangHoa2 == null)
				{
					v_dm_HangHoa_KhungGia_HangHoa v_dm_HangHoa_KhungGia_HangHoa3 = new v_dm_HangHoa_KhungGia_HangHoa();
					apiResponse = Utility.GetDetail<v_dm_HangHoa>(Utility.LOC_ID + "/" + ID_HANGHOA, "Product");
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
						v_dm_HangHoa2 = apiResponse.Data as v_dm_HangHoa;
						v_dm_HangHoa_KhungGia_HangHoa3.ID = Guid.NewGuid().ToString();
						v_dm_HangHoa_KhungGia_HangHoa3.NAME = v_dm_HangHoa2.NAME;
						v_dm_HangHoa_KhungGia_HangHoa3.MA = v_dm_HangHoa2.MA;
						v_dm_HangHoa_KhungGia_HangHoa3.ID_HANGHOA = v_dm_HangHoa2.ID;
						lstProductPriceRangeHangHoa2.Add(v_dm_HangHoa_KhungGia_HangHoa3);
					}
				}
				base.Session["lstProductPriceRangeHangHoa"] = lstProductPriceRangeHangHoa2;
				apiResponse.ProductCombo = Utility.GetProductPriceRange_HangHoa();
				apiResponse.Success = true;
			}
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
			List<v_dm_HangHoa_KhungGia_HangHoa> lstProductPriceRangeHangHoa = Utility.LstProductPriceRangeHangHoa;
			v_dm_HangHoa_KhungGia_HangHoa v_dm_HangHoa_KhungGia_HangHoa2 = Utility.LstProductPriceRangeHangHoa.Where((v_dm_HangHoa_KhungGia_HangHoa e) => e.ID_HANGHOA == ID_HANGHOA).FirstOrDefault();
			if (v_dm_HangHoa_KhungGia_HangHoa2 != null)
			{
				lstProductPriceRangeHangHoa.Remove(v_dm_HangHoa_KhungGia_HangHoa2);
			}
			base.Session["lstProductPriceRangeHangHoa"] = lstProductPriceRangeHangHoa;
			List<ValueEdit> list = new List<ValueEdit>();
			apiResponse.ProductCombo = Utility.GetProductPriceRange_HangHoa();
			list.Add(new ValueEdit
			{
				Key = "tbodyTempItemdivPromotion_TangEdit",
				Value = apiResponse.ProductCombo
			});
			list.Add(new ValueEdit
			{
				Key = "tbodyTempItemdivPromotion_Tang",
				Value = apiResponse.ProductCombo
			});
			apiResponse.Detail = list;
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
