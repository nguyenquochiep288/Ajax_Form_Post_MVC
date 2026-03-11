using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using DatabaseTHP;
using DatabaseTHP.Class;
using MVC_QuanLyTHP.Class;
using MVC_QuanLyTHP.Models;
using Newtonsoft.Json;
using PagedList;

namespace MVC_QuanLyTHP.Controllers
{

	public class KPI_SaleController : Controller
	{
		public ActionResult Index(int Page = 1, string SearchString = "", string ShowSearchValue = "")
		{
			try
			{
				if (Utility.KiemTra())
				{
					return RedirectToAction("Index", "Admin");
				}
				if (!Utility.KiemTraQuyen("KPI_Sale", "View"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				ShowSearchValue = Utility.GetShowSearchValue<dm_KPI_KinhDoanh>(ShowSearchValue);
				string cipherText = ShowSearchValue;
				string s = SearchString;
				if (clsMaHoa.Decrypt(ShowSearchValue, "tmt6364") == "TUNGAY" || clsMaHoa.Decrypt(ShowSearchValue, "tmt6364") == "DENNGAY")
				{
					ShowSearchValue = "";
					SearchString = "";
				}
				ApiResponse listData = Utility.GetListData<v_dm_KPI_KinhDoanh>("KPI_Sale", ShowSearchValue, SearchString, Utility.LOC_ID);
				if (!listData.Success)
				{
					base.TempData["TitleError"] = listData.Message;
					return RedirectToAction("Index", "Notfound");
				}
				List<v_dm_KPI_KinhDoanh> superset = new List<v_dm_KPI_KinhDoanh>();
				if (clsMaHoa.Decrypt(cipherText, "tmt6364") == "TUNGAY" || clsMaHoa.Decrypt(cipherText, "tmt6364") == "DENNGAY")
				{
					if (clsMaHoa.Decrypt(cipherText, "tmt6364") == "TUNGAY")
					{
						DateTime myDate = DateTime.ParseExact(s, "dd/MM/yyyy", CultureInfo.InvariantCulture);
						superset = (from v_dm_KPI_KinhDoanh2 in listData.Data as List<v_dm_KPI_KinhDoanh>
									where v_dm_KPI_KinhDoanh2.TUNGAY == myDate
									orderby v_dm_KPI_KinhDoanh2.DENNGAY descending
									select v_dm_KPI_KinhDoanh2).ToList();
					}
					if (clsMaHoa.Decrypt(cipherText, "tmt6364") == "DENNGAY")
					{
						DateTime myDate2 = DateTime.ParseExact(s, "dd/MM/yyyy", CultureInfo.InvariantCulture);
						superset = (from v_dm_KPI_KinhDoanh2 in listData.Data as List<v_dm_KPI_KinhDoanh>
									where v_dm_KPI_KinhDoanh2.DENNGAY >= myDate2
									orderby v_dm_KPI_KinhDoanh2.DENNGAY descending
									select v_dm_KPI_KinhDoanh2).ToList();
					}
				}
				else
				{
					superset = (listData.Data as List<v_dm_KPI_KinhDoanh>).OrderByDescending((v_dm_KPI_KinhDoanh v_dm_KPI_KinhDoanh2) => v_dm_KPI_KinhDoanh2.DENNGAY).ToList();
				}
				IPagedList<v_dm_KPI_KinhDoanh> iPagedList = superset.ToPagedList(Page, Utility.GetPageSize());
				v_v_dm_KPI_KinhDoanh v_v_dm_KPI_KinhDoanh2 = new v_v_dm_KPI_KinhDoanh();
				v_v_dm_KPI_KinhDoanh2.IPagedList = iPagedList;
				v_v_dm_KPI_KinhDoanh2.lstdm_DonViTinh = new List<v_dm_DonViTinh>();
				v_v_dm_KPI_KinhDoanh2.lstdm_DonViTinh = Utility.GetListData<v_dm_DonViTinh>("Unit", "", "", Utility.LOC_ID).Data as List<v_dm_DonViTinh>;
				base.ViewBag.searchValue = SearchString;
				base.ViewBag.showsearchValue = ShowSearchValue;
				base.ViewBag.PermissionEdit = Utility.KiemTraQuyen("KPI_Sale", "Edit");
				base.ViewBag.PermissionDelete = Utility.KiemTraQuyen("KPI_Sale", "Delete");
				base.ViewBag.PermissionCreate = Utility.KiemTraQuyen("KPI_Sale", "Create");
				return View(v_v_dm_KPI_KinhDoanh2);
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
				if (!Utility.KiemTraQuyen("KPI_Sale", "Create"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				v_v_dm_KPI_KinhDoanh v_v_dm_KPI_KinhDoanh2 = new v_v_dm_KPI_KinhDoanh();
				v_v_dm_KPI_KinhDoanh2.LOC_ID = Utility.LOC_ID;
				v_v_dm_KPI_KinhDoanh2.ID_NGUOITAO = base.Session["idUser"].ToString();
				v_v_dm_KPI_KinhDoanh2.THOIGIANTHEM = Utility.CurrentTime;
				v_v_dm_KPI_KinhDoanh2.ID = Guid.NewGuid().ToString();
				v_v_dm_KPI_KinhDoanh2.lstdm_DonViTinh = new List<v_dm_DonViTinh>();
				v_v_dm_KPI_KinhDoanh2.lstdm_DonViTinh = Utility.GetListData<v_dm_DonViTinh>("Unit", "", "", Utility.LOC_ID).Data as List<v_dm_DonViTinh>;
				return View(v_v_dm_KPI_KinhDoanh2);
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
		public ActionResult Create([Bind(Include = "LOC_ID,ID,MA,NAME,TUNGAY,DENNGAY,CHIETKHAU,TIENGIAM,IS_YEUCAUCHITIET,TONGTIEN_DATKM,SOLUONG_DATKM,ID_DVT_DATKM,ISACTIVE,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO,ISTINHLUYTUYEN,ISTONGHOADON,CAPDO")] v_dm_KPI_KinhDoanh dm_KPI_KinhDoanh)
		{
			try
			{
				if (Utility.KiemTra())
				{
					return RedirectToAction("Index", "Admin");
				}
				if (!Utility.KiemTraQuyen("KPI_Sale", "Create"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				if (base.ModelState.IsValid)
				{
					dm_KPI_KinhDoanh.LOC_ID = Utility.LOC_ID;
					dm_KPI_KinhDoanh.ID_NGUOITAO = base.Session["idUser"].ToString();
					dm_KPI_KinhDoanh.THOIGIANTHEM = Utility.CurrentTime;
					ApiResponse apiResponse = Utility.Create((dm_KPI_KinhDoanh)dm_KPI_KinhDoanh, "KPI_Sale");
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
				return View(dm_KPI_KinhDoanh);
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
				if (!Utility.KiemTraQuyen("KPI_Sale", "Edit"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				v_v_dm_KPI_KinhDoanh v_v_dm_KPI_KinhDoanh2 = new v_v_dm_KPI_KinhDoanh();
				if (!string.IsNullOrEmpty(id))
				{
					apiResponse = Utility.GetDetail<v_dm_KPI_KinhDoanh>(Utility.LOC_ID + "/" + id, "KPI_Sale");
					if (!apiResponse.Success)
					{
						base.TempData["TitleError"] = apiResponse.Message;
						return RedirectToAction("Index", "Notfound");
					}
					if (apiResponse.Data != null)
					{
						v_v_dm_KPI_KinhDoanh2 = apiResponse.Data as v_v_dm_KPI_KinhDoanh;
					}
				}
				v_v_dm_KPI_KinhDoanh2.lstdm_DonViTinh = new List<v_dm_DonViTinh>();
				v_v_dm_KPI_KinhDoanh2.lstdm_DonViTinh = Utility.GetListData<v_dm_DonViTinh>("Unit", "", "", Utility.LOC_ID).Data as List<v_dm_DonViTinh>;
				return View(v_v_dm_KPI_KinhDoanh2);
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
		public ActionResult Edit([Bind(Include = "LOC_ID,ID,MA,NAME,TUNGAY,DENNGAY,CHIETKHAU,TIENGIAM,IS_YEUCAUCHITIET,TONGTIEN_DATKM,SOLUONG_DATKM,ID_DVT_DATKM,ISACTIVE,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO,ISTINHLUYTUYEN,ISTONGHOADON,CAPDO")] v_dm_KPI_KinhDoanh dm_KPI_KinhDoanh)
		{
			try
			{
				if (Utility.KiemTra())
				{
					return RedirectToAction("Index", "Admin");
				}
				if (!Utility.KiemTraQuyen("KPI_Sale", "Edit"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				if (base.ModelState.IsValid)
				{
					dm_KPI_KinhDoanh.LOC_ID = Utility.LOC_ID;
					dm_KPI_KinhDoanh.ID_NGUOISUA = base.Session["idUser"].ToString();
					dm_KPI_KinhDoanh.THOIGIANSUA = Utility.CurrentTime;
					ApiResponse apiResponse = Utility.Edit(Utility.LOC_ID + "/" + dm_KPI_KinhDoanh.MA, dm_KPI_KinhDoanh, "KPI_Sale");
					if (apiResponse.Success)
					{
						return RedirectToAction("Index");
					}
					base.ModelState.AddModelError(string.Empty, apiResponse.Message);
				}
				return View(dm_KPI_KinhDoanh);
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
				if (!Utility.KiemTraQuyen("KPI_Sale", "Delete"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				ApiResponse apiResponse = Utility.Delete<v_dm_KPI_KinhDoanh>(Utility.LOC_ID + "/" + id, "KPI_Sale");
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
				if (!Utility.KiemTraQuyen("KPI_Sale", "Create"))
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
				v_v_dm_KPI_KinhDoanh v_v_dm_KPI_KinhDoanh2 = new v_v_dm_KPI_KinhDoanh();
				apiResponse.Success = true;
				v_v_dm_KPI_KinhDoanh2.LOC_ID = Utility.LOC_ID;
				v_v_dm_KPI_KinhDoanh2.TUNGAY = Utility.CurrentTime;
				v_v_dm_KPI_KinhDoanh2.DENNGAY = Utility.CurrentTime.AddMonths(1);
				v_v_dm_KPI_KinhDoanh2.ID = Guid.NewGuid().ToString();
				v_v_dm_KPI_KinhDoanh2.lstdm_DonViTinh = new List<v_dm_DonViTinh>();
				v_v_dm_KPI_KinhDoanh2.lstdm_DonViTinh = Utility.GetListData<v_dm_DonViTinh>("Unit", "", "", Utility.LOC_ID).Data as List<v_dm_DonViTinh>;
				List<v_dm_KPI_KinhDoanh_YeuCau> value = new List<v_dm_KPI_KinhDoanh_YeuCau>();
				List<v_dm_KPI_KinhDoanh_NhanVien> value2 = new List<v_dm_KPI_KinhDoanh_NhanVien>();
				base.Session["lstKPISale_YeuCau"] = value;
				base.Session["lstKPISale_NhanVien"] = value2;
				List<ValueEdit> list = Utility.ConvertobjectTo(v_v_dm_KPI_KinhDoanh2);
				apiResponse.ProductCombo = Utility.GetKPISale_YeuCau();
				list.Add(new ValueEdit
				{
					Key = "tbodyTempItemdivPromotion_YC",
					Value = apiResponse.ProductCombo
				});
				apiResponse.ProductCombo = Utility.GetKPISale_NhanVien();
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
		public ActionResult CreatePopup([Bind(Include = "LOC_ID,ID,MA,NAME,TUNGAY,DENNGAY,CHIETKHAU,TIENGIAM,IS_YEUCAUCHITIET,TONGTIEN_DATKM,SOLUONG_DATKM,ID_DVT_DATKM,ISACTIVE,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO,ISTINHLUYTUYEN,ISTONGHOADON,CAPDO")] v_dm_KPI_KinhDoanh dm_KPI_KinhDoanh)
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
				if (!Utility.KiemTraQuyen("KPI_Sale", "Create"))
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
					dm_KPI_KinhDoanh.LOC_ID = Utility.LOC_ID;
					dm_KPI_KinhDoanh.ID_NGUOITAO = base.Session["idUser"].ToString();
					dm_KPI_KinhDoanh.THOIGIANTHEM = Utility.CurrentTime;
					dm_KPI_KinhDoanh.lstdm_KPI_KinhDoanh_YeuCau = new List<v_dm_KPI_KinhDoanh_YeuCau>();
					dm_KPI_KinhDoanh.lstdm_KPI_KinhDoanh_NhanVien = new List<v_dm_KPI_KinhDoanh_NhanVien>();
					IEnumerable<string> source = base.Request.Form.AllKeys.Where((string e) => e.StartsWith("HINHTHUC_TINHKPI|"));
					IEnumerable<string> enumerable = base.Request.Form.AllKeys.Where((string e) => e.StartsWith("txtMoney_YC|"));
					IEnumerable<string> enumerable2 = base.Request.Form.AllKeys.Where((string e) => e.StartsWith("txtQuantity_YC|"));
					IEnumerable<string> enumerable3 = base.Request.Form.AllKeys.Where((string e) => e.StartsWith("txtCHIETKHAU_YC|"));
					IEnumerable<string> enumerable4 = base.Request.Form.AllKeys.Where((string e) => e.StartsWith("txtTIENGIAM_YC|"));
					if (enumerable == null || enumerable.Count() == 0 || enumerable2 == null || enumerable2.Count() == 0 || enumerable3 == null || enumerable3.Count() == 0 || enumerable4 == null || enumerable4.Count() == 0)
					{
						base.ModelState.AddModelError("lstdm_KPI_KinhDoanh_YeuCau", "Thêm sản phẩm.");
					}
					else
					{
						int num = 0;
						foreach (string item in enumerable)
						{
							string[] array = item.ToString().Split('|');
							string[] values = base.HttpContext.Request.Params.GetValues(item.ToString());
							string[] values2 = base.HttpContext.Request.Params.GetValues(enumerable2.ToList()[num].ToString());
							string[] values3 = base.HttpContext.Request.Params.GetValues(enumerable3.ToList()[num].ToString());
							string[] values4 = base.HttpContext.Request.Params.GetValues(enumerable4.ToList()[num].ToString());
							string[] values5 = base.HttpContext.Request.Params.GetValues(source.ToList()[num].ToString());
							string value = clsMaHoa.Decrypt(array[1].ToString(), "tmt6364");
							v_dm_KPI_KinhDoanh_YeuCau v_dm_KPI_KinhDoanh_YeuCau2 = JsonConvert.DeserializeObject<v_dm_KPI_KinhDoanh_YeuCau>(value);
							if (array != null)
							{
								if (string.IsNullOrEmpty(v_dm_KPI_KinhDoanh_YeuCau2.ID))
								{
									v_dm_KPI_KinhDoanh_YeuCau2.ID = Guid.NewGuid().ToString();
								}
								v_dm_KPI_KinhDoanh_YeuCau2.LOC_ID = Utility.LOC_ID;
								v_dm_KPI_KinhDoanh_YeuCau2.ID_KPI_KINHDOANH = dm_KPI_KinhDoanh.ID;
								v_dm_KPI_KinhDoanh_YeuCau2.SOTIEN = Utility.ConvertStringToDouble(values[0]);
								v_dm_KPI_KinhDoanh_YeuCau2.SOLUONG = Utility.ConvertStringToDouble(values2[0]);
								v_dm_KPI_KinhDoanh_YeuCau2.CHIETKHAU = Utility.ConvertStringToDouble(values3[0]);
								v_dm_KPI_KinhDoanh_YeuCau2.TIENGIAM = Utility.ConvertStringToDouble(values4[0]);
								v_dm_KPI_KinhDoanh_YeuCau2.HINHTHUC_TINHKPI = Convert.ToInt32(Utility.ConvertStringToDouble(values5[0]));
								dm_KPI_KinhDoanh.lstdm_KPI_KinhDoanh_YeuCau.Add(v_dm_KPI_KinhDoanh_YeuCau2);
							}
							num++;
						}
					}
					IEnumerable<string> enumerable5 = base.Request.Form.AllKeys.Where((string e) => e.StartsWith("txtISACTIVE|"));
					if (enumerable5 == null || enumerable5.Count() == 0)
					{
						base.ModelState.AddModelError("lstdm_KPI_KinhDoanh_NhanVien", "Thêm nhân viên.");
					}
					else
					{
						int num2 = 0;
						foreach (string item2 in enumerable5)
						{
							string[] array2 = item2.ToString().Split('|');
							string[] values6 = base.HttpContext.Request.Params.GetValues(item2.ToString());
							string value2 = clsMaHoa.Decrypt(array2[1].ToString(), "tmt6364");
							v_dm_KPI_KinhDoanh_NhanVien v_dm_KPI_KinhDoanh_NhanVien2 = JsonConvert.DeserializeObject<v_dm_KPI_KinhDoanh_NhanVien>(value2);
							if (array2 != null)
							{
								if (string.IsNullOrEmpty(v_dm_KPI_KinhDoanh_NhanVien2.ID))
								{
									v_dm_KPI_KinhDoanh_NhanVien2.ID = Guid.NewGuid().ToString();
								}
								v_dm_KPI_KinhDoanh_NhanVien2.LOC_ID = Utility.LOC_ID;
								v_dm_KPI_KinhDoanh_NhanVien2.ID_KPI_KINHDOANH = dm_KPI_KinhDoanh.ID;
								dm_KPI_KinhDoanh.lstdm_KPI_KinhDoanh_NhanVien.Add(v_dm_KPI_KinhDoanh_NhanVien2);
							}
							num2++;
						}
					}
					apiResponse = Utility.Create(dm_KPI_KinhDoanh, "KPI_Sale");
					if (apiResponse.Success)
					{
						if (apiResponse.Data != null)
						{
							dm_KPI_KinhDoanh = JsonConvert.DeserializeObject<v_dm_KPI_KinhDoanh>(apiResponse.Data.ToString());
						}
						apiResponse.NewID = Guid.NewGuid().ToString();
						List<v_dm_KPI_KinhDoanh_YeuCau> value3 = new List<v_dm_KPI_KinhDoanh_YeuCau>();
						List<v_dm_KPI_KinhDoanh_NhanVien> value4 = new List<v_dm_KPI_KinhDoanh_NhanVien>();
						base.Session["lstKPISale_YeuCau"] = value3;
						base.Session["lstKPISale_NhanVien"] = value4;
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
					apiResponse.Data = Utility.GetModelState(base.ModelState, "KPI_Sale");
				}
				apiResponse.ID = dm_KPI_KinhDoanh.ID;
				List<ValueEdit> list = Utility.ConvertobjectTo(dm_KPI_KinhDoanh);
				if (flag)
				{
					apiResponse.ProductCombo = Utility.GetKPISale_YeuCau();
					list.Add(new ValueEdit
					{
						Key = "tbodyTempItemdivPromotion_YC",
						Value = apiResponse.ProductCombo
					});
					apiResponse.ProductCombo = Utility.GetKPISale_NhanVien();
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
				if (!Utility.KiemTraQuyen("KPI_Sale", "Edit"))
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
				v_v_dm_KPI_KinhDoanh v_v_dm_KPI_KinhDoanh2 = new v_v_dm_KPI_KinhDoanh();
				if (!string.IsNullOrEmpty(id))
				{
					apiResponse = Utility.GetDetail<v_v_dm_KPI_KinhDoanh>(Utility.LOC_ID + "/" + id, "KPI_Sale");
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
						v_v_dm_KPI_KinhDoanh2 = apiResponse.Data as v_v_dm_KPI_KinhDoanh;
					}
				}
				apiResponse.Success = true;
				v_v_dm_KPI_KinhDoanh2.lstdm_DonViTinh = new List<v_dm_DonViTinh>();
				v_v_dm_KPI_KinhDoanh2.lstdm_DonViTinh = Utility.GetListData<v_dm_DonViTinh>("Unit", "", "", Utility.LOC_ID).Data as List<v_dm_DonViTinh>;
				List<v_dm_KPI_KinhDoanh_YeuCau> list = new List<v_dm_KPI_KinhDoanh_YeuCau>();
				List<v_dm_KPI_KinhDoanh_NhanVien> list2 = new List<v_dm_KPI_KinhDoanh_NhanVien>();
				foreach (v_dm_KPI_KinhDoanh_YeuCau item in v_v_dm_KPI_KinhDoanh2.lstdm_KPI_KinhDoanh_YeuCau)
				{
					list.Add(item);
				}
				foreach (v_dm_KPI_KinhDoanh_NhanVien item2 in v_v_dm_KPI_KinhDoanh2.lstdm_KPI_KinhDoanh_NhanVien)
				{
					list2.Add(item2);
				}
				base.Session["lstKPISale_YeuCau"] = list;
				base.Session["lstKPISale_NhanVien"] = list2;
				List<ValueEdit> list3 = Utility.ConvertobjectTo(v_v_dm_KPI_KinhDoanh2);
				apiResponse.ProductCombo = Utility.GetKPISale_YeuCau();
				list3.Add(new ValueEdit
				{
					Key = "tbodyTempItemdivPromotion_YCEdit",
					Value = apiResponse.ProductCombo
				});
				apiResponse.ProductCombo = Utility.GetKPISale_NhanVien();
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
		public ActionResult EditPopup([Bind(Include = "LOC_ID,ID,MA,NAME,TUNGAY,DENNGAY,CHIETKHAU,TIENGIAM,IS_YEUCAUCHITIET,TONGTIEN_DATKM,SOLUONG_DATKM,ID_DVT_DATKM,ISACTIVE,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO,ISTINHLUYTUYEN,ISTONGHOADON,CAPDO")] v_dm_KPI_KinhDoanh dm_KPI_KinhDoanh)
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
				if (!Utility.KiemTraQuyen("KPI_Sale", "Edit"))
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
					dm_KPI_KinhDoanh.LOC_ID = Utility.LOC_ID;
					dm_KPI_KinhDoanh.ID_NGUOISUA = base.Session["idUser"].ToString();
					dm_KPI_KinhDoanh.THOIGIANSUA = Utility.CurrentTime;
					dm_KPI_KinhDoanh.lstdm_KPI_KinhDoanh_YeuCau = new List<v_dm_KPI_KinhDoanh_YeuCau>();
					dm_KPI_KinhDoanh.lstdm_KPI_KinhDoanh_NhanVien = new List<v_dm_KPI_KinhDoanh_NhanVien>();
					IEnumerable<string> source = base.Request.Form.AllKeys.Where((string e) => e.StartsWith("HINHTHUC_TINHKPI|"));
					IEnumerable<string> enumerable = base.Request.Form.AllKeys.Where((string e) => e.StartsWith("txtMoney_YC|"));
					IEnumerable<string> enumerable2 = base.Request.Form.AllKeys.Where((string e) => e.StartsWith("txtQuantity_YC|"));
					IEnumerable<string> enumerable3 = base.Request.Form.AllKeys.Where((string e) => e.StartsWith("txtCHIETKHAU_YC|"));
					IEnumerable<string> enumerable4 = base.Request.Form.AllKeys.Where((string e) => e.StartsWith("txtTIENGIAM_YC|"));
					if (enumerable == null || enumerable.Count() == 0 || enumerable2 == null || enumerable2.Count() == 0 || enumerable3 == null || enumerable3.Count() == 0 || enumerable4 == null || enumerable4.Count() == 0)
					{
						base.ModelState.AddModelError("lstdm_KPI_KinhDoanh_YeuCau", "Thêm sản phẩm trong yêu cầu.");
					}
					else
					{
						int num = 0;
						foreach (string item in enumerable)
						{
							string[] array = item.ToString().Split('|');
							string[] values = base.HttpContext.Request.Params.GetValues(item.ToString());
							string value = clsMaHoa.Decrypt(array[1].ToString(), "tmt6364");
							string[] values2 = base.HttpContext.Request.Params.GetValues(enumerable2.ToList()[num].ToString());
							string[] values3 = base.HttpContext.Request.Params.GetValues(enumerable3.ToList()[num].ToString());
							string[] values4 = base.HttpContext.Request.Params.GetValues(enumerable4.ToList()[num].ToString());
							string[] values5 = base.HttpContext.Request.Params.GetValues(source.ToList()[num].ToString());
							v_dm_KPI_KinhDoanh_YeuCau v_dm_KPI_KinhDoanh_YeuCau2 = JsonConvert.DeserializeObject<v_dm_KPI_KinhDoanh_YeuCau>(value);
							if (array != null)
							{
								if (string.IsNullOrEmpty(v_dm_KPI_KinhDoanh_YeuCau2.ID))
								{
									v_dm_KPI_KinhDoanh_YeuCau2.ID = Guid.NewGuid().ToString();
								}
								v_dm_KPI_KinhDoanh_YeuCau2.LOC_ID = Utility.LOC_ID;
								v_dm_KPI_KinhDoanh_YeuCau2.ID_KPI_KINHDOANH = dm_KPI_KinhDoanh.ID;
								v_dm_KPI_KinhDoanh_YeuCau2.SOLUONG = Utility.ConvertStringToDouble(values2[0]);
								v_dm_KPI_KinhDoanh_YeuCau2.SOTIEN = Utility.ConvertStringToDouble(values[0]);
								v_dm_KPI_KinhDoanh_YeuCau2.CHIETKHAU = Utility.ConvertStringToDouble(values3[0]);
								v_dm_KPI_KinhDoanh_YeuCau2.TIENGIAM = Utility.ConvertStringToDouble(values4[0]);
								v_dm_KPI_KinhDoanh_YeuCau2.HINHTHUC_TINHKPI = Convert.ToInt32(Utility.ConvertStringToDouble(values5[0]));
								dm_KPI_KinhDoanh.lstdm_KPI_KinhDoanh_YeuCau.Add(v_dm_KPI_KinhDoanh_YeuCau2);
							}
							num++;
						}
					}
					IEnumerable<string> enumerable5 = base.Request.Form.AllKeys.Where((string e) => e.StartsWith("txtISACTIVE|"));
					if (enumerable5 == null || enumerable5.Count() == 0)
					{
						base.ModelState.AddModelError("lstdm_KPI_KinhDoanh_NhanVien", "Thêm nhân viên .");
					}
					else
					{
						int num2 = 0;
						foreach (string item2 in enumerable5)
						{
							string[] array2 = item2.ToString().Split('|');
							string[] values6 = base.HttpContext.Request.Params.GetValues(item2.ToString());
							string value2 = clsMaHoa.Decrypt(array2[1].ToString(), "tmt6364");
							v_dm_KPI_KinhDoanh_NhanVien v_dm_KPI_KinhDoanh_NhanVien2 = JsonConvert.DeserializeObject<v_dm_KPI_KinhDoanh_NhanVien>(value2);
							if (array2 != null)
							{
								if (string.IsNullOrEmpty(v_dm_KPI_KinhDoanh_NhanVien2.ID))
								{
									v_dm_KPI_KinhDoanh_NhanVien2.ID = Guid.NewGuid().ToString();
								}
								v_dm_KPI_KinhDoanh_NhanVien2.LOC_ID = Utility.LOC_ID;
								v_dm_KPI_KinhDoanh_NhanVien2.ID_KPI_KINHDOANH = dm_KPI_KinhDoanh.ID;
								dm_KPI_KinhDoanh.lstdm_KPI_KinhDoanh_NhanVien.Add(v_dm_KPI_KinhDoanh_NhanVien2);
							}
							num2++;
						}
					}
					apiResponse = Utility.Edit(Utility.LOC_ID + "/" + dm_KPI_KinhDoanh.MA, dm_KPI_KinhDoanh, "KPI_Sale");
					if (apiResponse.Success)
					{
						if (apiResponse.Data != null)
						{
							dm_KPI_KinhDoanh = JsonConvert.DeserializeObject<v_dm_KPI_KinhDoanh>(apiResponse.Data.ToString());
						}
						apiResponse.ID = dm_KPI_KinhDoanh.ID;
					}
					else
					{
						base.ModelState.AddModelError(string.Empty, apiResponse.Message);
					}
				}
				else
				{
					apiResponse.Success = false;
					apiResponse.Data = Utility.GetModelState(base.ModelState, "KPI_Sale");
				}
				apiResponse.Detail = Utility.ConvertobjectToView((dm_KPI_KinhDoanh)dm_KPI_KinhDoanh, "dd/MM/yyyy");
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
				if (!Utility.KiemTraQuyen("KPI_Sale", "Delete"))
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
				apiResponse = Utility.Delete<v_dm_KPI_KinhDoanh>(Utility.LOC_ID + "/" + id, "KPI_Sale");
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
		public ActionResult AddProductPromotion_YC([Bind(Include = "ID_HANGHOA,SOLUONG,ID_DVT")] v_v_dm_KPI_KinhDoanh_YeuCau dm_HangHoa_Combo)
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
				v_dm_KPI_KinhDoanh_YeuCau v_dm_KPI_KinhDoanh_YeuCau2 = Utility.LstKPISale_YeuCau.Where((v_dm_KPI_KinhDoanh_YeuCau e) => e.ID_HANGHOA == dm_HangHoa_Combo.ID_HANGHOA && e.ID_DVT == dm_HangHoa_Combo.ID_DVT).FirstOrDefault();
				if (v_dm_KPI_KinhDoanh_YeuCau2 == null)
				{
					List<v_dm_KPI_KinhDoanh_YeuCau> lstKPISale_YeuCau = Utility.LstKPISale_YeuCau;
					lstKPISale_YeuCau.Add(dm_HangHoa_Combo);
					base.Session["lstKPISale_YeuCau"] = lstKPISale_YeuCau;
				}
				else
				{
					v_dm_KPI_KinhDoanh_YeuCau2.SOLUONG = dm_HangHoa_Combo.SOLUONG;
					v_dm_KPI_KinhDoanh_YeuCau2.SOTIEN = dm_HangHoa_Combo.SOTIEN;
				}
			}
			apiResponse.ProductCombo = Utility.GetKPISale_YeuCau();
			return new JsonResult
			{
				Data = apiResponse,
				JsonRequestBehavior = JsonRequestBehavior.AllowGet,
				MaxJsonLength = int.MaxValue
			};
		}

		[HttpPost]
		[ValidateInput(false)]
		public ActionResult AddProductPromotionNHH_YC([Bind(Include = "ID_HANGHOA,SOLUONG,ID_DVT")] v_v_dm_KPI_KinhDoanh_YeuCau dm_CTKM_YC)
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
				dm_CTKM_YC.HINHTHUC = 1;
				dm_CTKM_YC.ID_HANGHOA = v_v_dm_NhomHangHoa2.ID;
				dm_CTKM_YC.NAME = v_v_dm_NhomHangHoa2.NAME;
				dm_CTKM_YC.MA = v_v_dm_NhomHangHoa2.MA;
				dm_CTKM_YC.NAME_DVT = v_v_dm_DonViTinh2.NAME;
				v_dm_KPI_KinhDoanh_YeuCau v_dm_KPI_KinhDoanh_YeuCau2 = Utility.LstKPISale_YeuCau.Where((v_dm_KPI_KinhDoanh_YeuCau e) => e.ID_HANGHOA == dm_CTKM_YC.ID_HANGHOA && e.ID_DVT == dm_CTKM_YC.ID_DVT).FirstOrDefault();
				if (v_dm_KPI_KinhDoanh_YeuCau2 == null)
				{
					List<v_dm_KPI_KinhDoanh_YeuCau> lstKPISale_YeuCau = Utility.LstKPISale_YeuCau;
					lstKPISale_YeuCau.Add(dm_CTKM_YC);
					base.Session["lstKPISale_YeuCau"] = lstKPISale_YeuCau;
				}
				else
				{
					v_dm_KPI_KinhDoanh_YeuCau2.SOLUONG = dm_CTKM_YC.SOLUONG;
					v_dm_KPI_KinhDoanh_YeuCau2.SOTIEN = dm_CTKM_YC.SOTIEN;
				}
			}
			apiResponse.ProductCombo = Utility.GetKPISale_YeuCau();
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
			List<v_dm_KPI_KinhDoanh_YeuCau> lstKPISale_YeuCau = Utility.LstKPISale_YeuCau;
			v_dm_KPI_KinhDoanh_YeuCau v_dm_KPI_KinhDoanh_YeuCau2 = Utility.LstKPISale_YeuCau.Where((v_dm_KPI_KinhDoanh_YeuCau e) => e.ID_HANGHOA == ID_HANGHOA && e.ID_DVT == ID_DVT).FirstOrDefault();
			if (v_dm_KPI_KinhDoanh_YeuCau2 != null)
			{
				lstKPISale_YeuCau.Remove(v_dm_KPI_KinhDoanh_YeuCau2);
			}
			base.Session["lstKPISale_YeuCau"] = lstKPISale_YeuCau;
			apiResponse.ProductCombo = Utility.GetKPISale_YeuCau();
			apiResponse.Success = true;
			return new JsonResult
			{
				Data = apiResponse,
				JsonRequestBehavior = JsonRequestBehavior.AllowGet,
				MaxJsonLength = int.MaxValue
			};
		}

		[HttpGet]
		public ActionResult AddProductPromotion_NQ(string ID)
		{
			ApiResponse apiResponse = new ApiResponse();
			v_v_web_NhomQuyen v_v_web_NhomQuyen2 = new v_v_web_NhomQuyen();
			apiResponse = Utility.GetDetail<v_v_web_NhomQuyen>(Utility.LOC_ID + "/" + ID, "GroupPermissions");
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
				v_v_web_NhomQuyen2 = apiResponse.Data as v_v_web_NhomQuyen;
			}
			if (v_v_web_NhomQuyen2 != null)
			{
				v_dm_KPI_KinhDoanh_NhanVien dm_CTKM_YC = new v_dm_KPI_KinhDoanh_NhanVien();
				dm_CTKM_YC.HINHTHUC = 1;
				dm_CTKM_YC.ID_NHANVIEN = ID;
				dm_CTKM_YC.NAME = v_v_web_NhomQuyen2.NAME;
				dm_CTKM_YC.MA = v_v_web_NhomQuyen2.MA;
				v_dm_KPI_KinhDoanh_NhanVien v_dm_KPI_KinhDoanh_NhanVien2 = Utility.LstKPISale_NhanVien.Where((v_dm_KPI_KinhDoanh_NhanVien e) => e.ID_NHANVIEN == dm_CTKM_YC.ID_NHANVIEN && e.HINHTHUC == 1).FirstOrDefault();
				if (v_dm_KPI_KinhDoanh_NhanVien2 == null)
				{
					List<v_dm_KPI_KinhDoanh_NhanVien> lstKPISale_NhanVien = Utility.LstKPISale_NhanVien;
					lstKPISale_NhanVien.Add(dm_CTKM_YC);
					base.Session["lstKPISale_NhanVien"] = lstKPISale_NhanVien;
				}
			}
			apiResponse.ProductCombo = Utility.GetKPISale_NhanVien();
			List<ValueEdit> list = new List<ValueEdit>();
			apiResponse.ProductCombo = Utility.GetKPISale_NhanVien();
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

		[HttpGet]
		public ActionResult AddProductPromotion_NV(string ID)
		{
			ApiResponse apiResponse = new ApiResponse();
			v_v_dm_NhanVien v_v_dm_NhanVien2 = new v_v_dm_NhanVien();
			apiResponse = Utility.GetDetail<v_v_dm_NhanVien>(Utility.LOC_ID + "/" + ID, "Employee");
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
				v_v_dm_NhanVien2 = apiResponse.Data as v_v_dm_NhanVien;
			}
			if (v_v_dm_NhanVien2 != null)
			{
				v_dm_KPI_KinhDoanh_NhanVien dm_CTKM_YC = new v_dm_KPI_KinhDoanh_NhanVien();
				dm_CTKM_YC.HINHTHUC = 0;
				dm_CTKM_YC.ID_NHANVIEN = ID;
				dm_CTKM_YC.NAME = v_v_dm_NhanVien2.NAME;
				dm_CTKM_YC.MA = v_v_dm_NhanVien2.MA;
				v_dm_KPI_KinhDoanh_NhanVien v_dm_KPI_KinhDoanh_NhanVien2 = Utility.LstKPISale_NhanVien.Where((v_dm_KPI_KinhDoanh_NhanVien e) => e.ID_NHANVIEN == dm_CTKM_YC.ID_NHANVIEN && e.HINHTHUC == dm_CTKM_YC.HINHTHUC).FirstOrDefault();
				if (v_dm_KPI_KinhDoanh_NhanVien2 == null)
				{
					List<v_dm_KPI_KinhDoanh_NhanVien> lstKPISale_NhanVien = Utility.LstKPISale_NhanVien;
					lstKPISale_NhanVien.Add(dm_CTKM_YC);
					base.Session["lstKPISale_NhanVien"] = lstKPISale_NhanVien;
				}
			}
			apiResponse.ProductCombo = Utility.GetKPISale_NhanVien();
			List<ValueEdit> list = new List<ValueEdit>();
			apiResponse.ProductCombo = Utility.GetKPISale_NhanVien();
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

		[HttpPost]
		public ActionResult DeleteProductPromotion_Tang(string ID_HANGHOA, string ID_DVT)
		{
			ApiResponse apiResponse = new ApiResponse();
			v_v_dm_HangHoa v_v_dm_HangHoa2 = new v_v_dm_HangHoa();
			List<v_dm_KPI_KinhDoanh_NhanVien> lstKPISale_NhanVien = Utility.LstKPISale_NhanVien;
			v_dm_KPI_KinhDoanh_NhanVien v_dm_KPI_KinhDoanh_NhanVien2 = Utility.LstKPISale_NhanVien.Where((v_dm_KPI_KinhDoanh_NhanVien e) => e.ID_NHANVIEN == ID_HANGHOA && e.HINHTHUC.ToString() == ID_DVT).FirstOrDefault();
			if (v_dm_KPI_KinhDoanh_NhanVien2 != null)
			{
				lstKPISale_NhanVien.Remove(v_dm_KPI_KinhDoanh_NhanVien2);
			}
			base.Session["lstKPISale_NhanVien"] = lstKPISale_NhanVien;
			List<ValueEdit> list = new List<ValueEdit>();
			apiResponse.ProductCombo = Utility.GetKPISale_NhanVien();
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

		public ActionResult OnSubmitKPI_Sale(string cartOrder)
		{
			ApiResponse apiResponse = new ApiResponse();
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
			if (base.ModelState.IsValid)
			{
				Return obj = new Return();
				List<Deposit> list = new JavaScriptSerializer().Deserialize<List<Deposit>>(cartOrder);
				foreach (Deposit item in list)
				{
					item.ID_NGUOITAO = base.Session["idUser"].ToString();
					item.LOC_ID = Utility.LOC_ID;
					item.NGAYLAP = Utility.CurrentTime;
				}
				apiResponse = Utility.Create(list, "KPI_Sale/PostCreateKPI_Sale");
				if (apiResponse.Success)
				{
					obj.Message = "Tạo chương trình thành công!";
				}
				else
				{
					base.ModelState.AddModelError(string.Empty, apiResponse.Message);
					obj.Message = apiResponse.Message;
				}
				return Json(obj, JsonRequestBehavior.AllowGet);
			}
			Return obj2 = new Return();
			obj2.DATA = "";
			return Json(obj2, JsonRequestBehavior.AllowGet);
		}
	}
}
