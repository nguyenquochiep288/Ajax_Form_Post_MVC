using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using DatabaseTHP;
using DatabaseTHP.Class;
using DatabaseTHP.StoredProcedure.Parameter;
using MVC_QuanLyTHP.Class;
using MVC_QuanLyTHP.Models;
using Newtonsoft.Json;
using PagedList;
using QRCoder;

namespace MVC_QuanLyTHP.Controllers
{

	public class DeliveryController : Controller
	{
		public ActionResult Index(int Page = 1, DateTime? FromDate = null, DateTime? ToDate = null, string SearchString = "", string MAPHIEU = "", string IDCODE = "")
		{
			try
			{
				if (Utility.KiemTra())
				{
					return RedirectToAction("Index", "Admin");
				}
				if (!Utility.KiemTraQuyen("Delivery", "View"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				string text = "";
				ApiResponse apiResponse = new ApiResponse();
				IPagedList<v_ct_PhieuGiaoHang> iPagedList = new List<v_ct_PhieuGiaoHang>().ToList().ToPagedList(Page, Utility.GetPageSize());
				if (FromDate.HasValue || !string.IsNullOrEmpty(IDCODE) || !string.IsNullOrEmpty(MAPHIEU))
				{
					if (!string.IsNullOrEmpty(IDCODE) || !string.IsNullOrEmpty(MAPHIEU))
					{
						apiResponse = Utility.Get_DanhSachPhieuGiaoHang<v_ct_PhieuGiaoHang>("", null, null, MAPHIEU, IDCODE);
					}
					if (FromDate.HasValue)
					{
						apiResponse = Utility.Get_DanhSachPhieuGiaoHang<v_ct_PhieuGiaoHang>("", FromDate, ToDate, SearchString);
					}
					if (!apiResponse.Success)
					{
						apiResponse.Data = new List<v_dm_HangHoa>();
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
					if (!apiResponse.Success)
					{
						base.TempData["TitleError"] = apiResponse.Message;
						return RedirectToAction("Index", "Notfound");
					}
					text = (apiResponse.Data as List<v_ct_PhieuGiaoHang>).Sum((v_ct_PhieuGiaoHang s) => s.SOTIENGIAOHANG).ToString("N0");
					iPagedList = (apiResponse.Data as List<v_ct_PhieuGiaoHang>).ToPagedList(Page, Utility.GetPageSize());
				}
				v_v_ct_PhieuGiaoHang v_v_ct_PhieuGiaoHang2 = new v_v_ct_PhieuGiaoHang();
				v_v_ct_PhieuGiaoHang2.IPagedList = iPagedList;
				v_v_ct_PhieuGiaoHang2.lstdm_Xe = new List<v_dm_Xe>();
				v_v_ct_PhieuGiaoHang2.lstdm_KhuVuc = new List<v_dm_KhuVuc>();
				v_v_ct_PhieuGiaoHang2.lstdm_KhuVuc = Utility.GetListData<v_dm_KhuVuc>("Area", "", "", Utility.LOC_ID).Data as List<v_dm_KhuVuc>;
				base.ViewBag.searchValue = SearchString;
				base.ViewBag.TotalSum = text;
				base.ViewBag.PermissionEdit = Utility.KiemTraQuyen("Delivery", "Edit");
				base.ViewBag.PermissionDelete = Utility.KiemTraQuyen("Delivery", "Delete");
				base.ViewBag.PermissionCreate = Utility.KiemTraQuyen("Delivery", "Create");
				base.ViewBag.PermissionDelivery = Utility.KiemTraQuyen("Delivery", "Delivery");
				base.ViewBag.fromdate = (FromDate.HasValue ? FromDate.Value.ToString("yyyy-MM-dd") : Utility.CurrentTime.AddMonths(-1).ToString("yyyy-MM-dd"));
				base.ViewBag.todate = (ToDate.HasValue ? ToDate.Value.ToString("yyyy-MM-dd") : Utility.CurrentTime.ToString("yyyy-MM-dd"));
				return View(v_v_ct_PhieuGiaoHang2);
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
				if (!Utility.KiemTraQuyen("Delivery", "Create"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				v_v_ct_PhieuGiaoHang v_v_ct_PhieuGiaoHang2 = new v_v_ct_PhieuGiaoHang();
				v_v_ct_PhieuGiaoHang2.LOC_ID = Utility.LOC_ID;
				v_v_ct_PhieuGiaoHang2.ID_NGUOITAO = base.Session["idUser"].ToString();
				v_v_ct_PhieuGiaoHang2.THOIGIANTHEM = Utility.CurrentTime;
				v_v_ct_PhieuGiaoHang2.NGAYLAP = Utility.CurrentTime;
				v_v_ct_PhieuGiaoHang2.SOPHIEU = Utility.GetMaxID((ct_PhieuGiaoHang)v_v_ct_PhieuGiaoHang2, Utility.LOC_ID, v_v_ct_PhieuGiaoHang2.NGAYLAP.ToString("yyyy-MM-dd"));
				v_v_ct_PhieuGiaoHang2.MAPHIEU = API.GetMaPhieu("Delivery", v_v_ct_PhieuGiaoHang2.NGAYLAP, v_v_ct_PhieuGiaoHang2.SOPHIEU);
				v_v_ct_PhieuGiaoHang2.lstdm_Xe = new List<v_dm_Xe>();
				v_v_ct_PhieuGiaoHang2.ID = Guid.NewGuid().ToString();
				return View(v_v_ct_PhieuGiaoHang2);
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
		public ActionResult Create([Bind(Include = "LOC_ID,ID,ID_XEGIAOHANG,MAPHIEU,SOPHIEU,NGAYLAP,GHICHU,ISHOANTAT,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO")] v_ct_PhieuGiaoHang ct_PhieuGiaoHang)
		{
			try
			{
				if (Utility.KiemTra())
				{
					return RedirectToAction("Index", "Admin");
				}
				if (!Utility.KiemTraQuyen("Delivery", "Create"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				if (base.ModelState.IsValid)
				{
					ct_PhieuGiaoHang.LOC_ID = Utility.LOC_ID;
					ct_PhieuGiaoHang.ID_NGUOITAO = base.Session["idUser"].ToString();
					ct_PhieuGiaoHang.THOIGIANTHEM = Utility.CurrentTime;
					ApiResponse apiResponse = Utility.Create((ct_PhieuGiaoHang)ct_PhieuGiaoHang, "Delivery");
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
				return View(ct_PhieuGiaoHang);
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
				if (!Utility.KiemTraQuyen("Delivery", "Edit"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				v_v_ct_PhieuGiaoHang v_v_ct_PhieuGiaoHang2 = new v_v_ct_PhieuGiaoHang();
				if (!string.IsNullOrEmpty(id))
				{
					apiResponse = Utility.GetDetail<v_ct_PhieuGiaoHang>(Utility.LOC_ID + "/" + id, "Delivery");
					if (!apiResponse.Success)
					{
						base.TempData["TitleError"] = apiResponse.Message;
						return RedirectToAction("Index", "Notfound");
					}
					if (apiResponse.Data != null)
					{
						v_v_ct_PhieuGiaoHang2 = apiResponse.Data as v_v_ct_PhieuGiaoHang;
					}
				}
				v_v_ct_PhieuGiaoHang2.lstdm_Xe = new List<v_dm_Xe>();
				return View(v_v_ct_PhieuGiaoHang2);
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
		public ActionResult Edit([Bind(Include = "LOC_ID,ID,ID_XEGIAOHANG,MAPHIEU,SOPHIEU,NGAYLAP,GHICHU,ISHOANTAT,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO")] v_ct_PhieuGiaoHang ct_PhieuGiaoHang)
		{
			try
			{
				if (Utility.KiemTra())
				{
					return RedirectToAction("Index", "Admin");
				}
				if (!Utility.KiemTraQuyen("Delivery", "Edit"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				if (base.ModelState.IsValid)
				{
					ct_PhieuGiaoHang.LOC_ID = Utility.LOC_ID;
					ct_PhieuGiaoHang.ID_NGUOISUA = base.Session["idUser"].ToString();
					ct_PhieuGiaoHang.THOIGIANSUA = Utility.CurrentTime;
					ApiResponse apiResponse = Utility.Edit(Utility.LOC_ID + "/" + ct_PhieuGiaoHang.ID, ct_PhieuGiaoHang, "Delivery");
					if (apiResponse.Success)
					{
						return RedirectToAction("Index");
					}
					base.ModelState.AddModelError(string.Empty, apiResponse.Message);
				}
				return View(ct_PhieuGiaoHang);
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
				if (!Utility.KiemTraQuyen("Delivery", "Delete"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				ApiResponse apiResponse = Utility.Delete<v_ct_PhieuGiaoHang>(Utility.LOC_ID + "/" + id, "Delivery");
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

		public ActionResult CreatePopup(int HINHTHUC = 0)
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
				if (!Utility.KiemTraQuyen("Delivery", "Create"))
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
				v_v_ct_PhieuGiaoHang v_v_ct_PhieuGiaoHang2 = new v_v_ct_PhieuGiaoHang();
				apiResponse.Success = true;
				v_v_ct_PhieuGiaoHang2.LOC_ID = Utility.LOC_ID;
				v_v_ct_PhieuGiaoHang2.ID = Guid.NewGuid().ToString();
				v_v_ct_PhieuGiaoHang2.NGAYLAP = Utility.CurrentTime.AddDays(HINHTHUC);
				v_v_ct_PhieuGiaoHang2.SOPHIEU = Utility.GetMaxID((ct_PhieuGiaoHang)v_v_ct_PhieuGiaoHang2, Utility.LOC_ID, v_v_ct_PhieuGiaoHang2.NGAYLAP.ToString("yyyy-MM-dd"));
				v_v_ct_PhieuGiaoHang2.MAPHIEU = API.GetMaPhieu("Delivery", v_v_ct_PhieuGiaoHang2.NGAYLAP, v_v_ct_PhieuGiaoHang2.SOPHIEU);
				v_v_ct_PhieuGiaoHang2.lstdm_Xe = new List<v_dm_Xe>();
				v_v_ct_PhieuGiaoHang2.lstdm_Xe = Utility.GetListData<v_dm_Xe>("Car", "", "", Utility.LOC_ID).Data as List<v_dm_Xe>;
				base.Session["lstDelivery_Detail"] = new List<v_ct_PhieuGiaoHang_ChiTiet>();
				base.Session["lstDelivery_Shipper"] = new List<v_ct_PhieuGiaoHang_NhanVienGiao>();
				v_v_ct_PhieuGiaoHang2.lstct_PhieuGiaoHang_ChiTiet = new List<v_ct_PhieuGiaoHang_ChiTiet>();
				v_v_ct_PhieuGiaoHang2.lstct_PhieuGiaoHang_NhanVienGiao = new List<v_ct_PhieuGiaoHang_NhanVienGiao>();
				List<ValueEdit> list = Utility.ConvertobjectTo(v_v_ct_PhieuGiaoHang2);
				apiResponse.ProductCombo = Utility.GetDelivery_Detail(new List<v_ct_PhieuGiaoHang_ChiTiet>());
				list.Add(new ValueEdit
				{
					Key = "lstct_PhieuGiaoHang_ChiTiet",
					Value = apiResponse.ProductCombo
				});
				apiResponse.ProductCombo = Utility.GetDelivery_Shipper(new List<v_ct_PhieuGiaoHang_NhanVienGiao>());
				list.Add(new ValueEdit
				{
					Key = "lstct_PhieuGiaoHang_NhanVienGiao",
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
		public ActionResult CreatePopup([Bind(Include = "LOC_ID,ID,ID_XEGIAOHANG,MAPHIEU,SOPHIEU,NGAYLAP,GHICHU,ISHOANTAT,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO")] v_v_ct_PhieuGiaoHang ct_PhieuGiaoHang)
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
				if (!Utility.KiemTraQuyen("Delivery", "Create"))
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
				IEnumerable<string> enumerable = base.Request.Form.AllKeys.Where((string e) => e.StartsWith("txtDetail"));
				if (enumerable == null || enumerable.Count() == 0)
				{
					base.ModelState.AddModelError("lstct_PhieuGiaoHang_ChiTiet", "Thêm danh sách phiếu xuất.");
				}
				IEnumerable<string> enumerable2 = base.Request.Form.AllKeys.Where((string e) => e.StartsWith("txtShipper"));
				if (enumerable == null || enumerable.Count() == 0)
				{
					base.ModelState.AddModelError("lstct_PhieuGiaoHang_NhanVienGiao", "Thêm nhân viên giao.");
				}
				if (base.ModelState.IsValid)
				{
					ct_PhieuGiaoHang.NGAYLAP = ct_PhieuGiaoHang.NGAYLAP.AddHours(Utility.CurrentTime.Hour).AddMinutes(Utility.CurrentTime.Minute);
					ct_PhieuGiaoHang.LOC_ID = Utility.LOC_ID;
					ct_PhieuGiaoHang.ID_NGUOITAO = base.Session["idUser"].ToString();
					ct_PhieuGiaoHang.THOIGIANTHEM = Utility.CurrentTime;
					ct_PhieuGiaoHang.lstct_PhieuGiaoHang_ChiTiet = new List<v_ct_PhieuGiaoHang_ChiTiet>();
					ct_PhieuGiaoHang.lstct_PhieuGiaoHang_NhanVienGiao = new List<v_ct_PhieuGiaoHang_NhanVienGiao>();
					v_ct_PhieuGiaoHang_ChiTiet v_ct_PhieuGiaoHang_ChiTiet2 = new v_ct_PhieuGiaoHang_ChiTiet();
					foreach (string item in enumerable)
					{
						string[] array = item.ToString().Split('|');
						string[] values = base.HttpContext.Request.Params.GetValues(item.ToString());
						string value = clsMaHoa.Decrypt(array[1].ToString(), "tmt6364");
						v_ct_PhieuGiaoHang_ChiTiet v_ct_PhieuGiaoHang_ChiTiet3 = JsonConvert.DeserializeObject<v_ct_PhieuGiaoHang_ChiTiet>(value);
						if (v_ct_PhieuGiaoHang_ChiTiet2.ID != v_ct_PhieuGiaoHang_ChiTiet3.ID)
						{
							v_ct_PhieuGiaoHang_ChiTiet2 = new v_ct_PhieuGiaoHang_ChiTiet();
							v_ct_PhieuGiaoHang_ChiTiet2 = JsonConvert.DeserializeObject<v_ct_PhieuGiaoHang_ChiTiet>(value);
							v_ct_PhieuGiaoHang_ChiTiet2.LOC_ID = ct_PhieuGiaoHang.LOC_ID;
							ct_PhieuGiaoHang.lstct_PhieuGiaoHang_ChiTiet.Add(v_ct_PhieuGiaoHang_ChiTiet2);
						}
						Utility.EditObject(v_ct_PhieuGiaoHang_ChiTiet2, array[0].ToString().Substring(3, array[0].ToString().Length - 3), values[0]);
					}
					v_ct_PhieuGiaoHang_NhanVienGiao v_ct_PhieuGiaoHang_NhanVienGiao2 = new v_ct_PhieuGiaoHang_NhanVienGiao();
					foreach (string item2 in enumerable2)
					{
						string[] array2 = item2.ToString().Split('|');
						string[] values2 = base.HttpContext.Request.Params.GetValues(item2.ToString());
						string value2 = clsMaHoa.Decrypt(array2[1].ToString(), "tmt6364");
						v_ct_PhieuGiaoHang_NhanVienGiao v_ct_PhieuGiaoHang_NhanVienGiao3 = JsonConvert.DeserializeObject<v_ct_PhieuGiaoHang_NhanVienGiao>(value2);
						if (v_ct_PhieuGiaoHang_NhanVienGiao2.ID != v_ct_PhieuGiaoHang_NhanVienGiao3.ID)
						{
							v_ct_PhieuGiaoHang_NhanVienGiao2 = new v_ct_PhieuGiaoHang_NhanVienGiao();
							v_ct_PhieuGiaoHang_NhanVienGiao2 = JsonConvert.DeserializeObject<v_ct_PhieuGiaoHang_NhanVienGiao>(value2);
							v_ct_PhieuGiaoHang_NhanVienGiao2.LOC_ID = ct_PhieuGiaoHang.LOC_ID;
							ct_PhieuGiaoHang.lstct_PhieuGiaoHang_NhanVienGiao.Add(v_ct_PhieuGiaoHang_NhanVienGiao2);
						}
						Utility.EditObject(v_ct_PhieuGiaoHang_NhanVienGiao2, array2[0].ToString().Substring(3, array2[0].ToString().Length - 3), values2[0]);
					}
					apiResponse = Utility.Create((v_ct_PhieuGiaoHang)ct_PhieuGiaoHang, "Delivery");
					if (apiResponse.Success)
					{
						ct_PhieuGiaoHang.NGAYLAP = Utility.CurrentTime;
						ApiResponse apiResponse2 = apiResponse;
						int sOPHIEU = (ct_PhieuGiaoHang.SOPHIEU = Utility.GetMaxID((ct_PhieuGiaoHang)ct_PhieuGiaoHang, Utility.LOC_ID, ct_PhieuGiaoHang.NGAYLAP.ToString("yyyy-MM-dd")));
						apiResponse2.SOPHIEU = sOPHIEU;
						ct_PhieuGiaoHang.MAPHIEU = API.GetMaPhieu("Delivery", ct_PhieuGiaoHang.NGAYLAP, ct_PhieuGiaoHang.SOPHIEU);
						apiResponse.NewID = Guid.NewGuid().ToString();
						apiResponse.MAPHIEU = ct_PhieuGiaoHang.MAPHIEU;
						if (apiResponse.Data != null)
						{
							ct_PhieuGiaoHang = JsonConvert.DeserializeObject<v_v_ct_PhieuGiaoHang>(apiResponse.Data.ToString());
						}
						ct_PhieuGiaoHang.lstct_PhieuGiaoHang_ChiTiet = new List<v_ct_PhieuGiaoHang_ChiTiet>();
						ct_PhieuGiaoHang.lstct_PhieuGiaoHang_NhanVienGiao = new List<v_ct_PhieuGiaoHang_NhanVienGiao>();
					}
					else
					{
						base.ModelState.AddModelError(string.Empty, apiResponse.Message);
						if (apiResponse.CheckValue)
						{
							ct_PhieuGiaoHang.NGAYLAP = Utility.CurrentTime;
							ApiResponse apiResponse3 = apiResponse;
							int sOPHIEU = (ct_PhieuGiaoHang.SOPHIEU = Utility.GetMaxID((ct_PhieuGiaoHang)ct_PhieuGiaoHang, Utility.LOC_ID, ct_PhieuGiaoHang.NGAYLAP.ToString("yyyy-MM-dd")));
							apiResponse3.SOPHIEU = sOPHIEU;
							ct_PhieuGiaoHang.MAPHIEU = API.GetMaPhieu("Delivery", ct_PhieuGiaoHang.NGAYLAP, ct_PhieuGiaoHang.SOPHIEU);
							apiResponse.NewID = Guid.NewGuid().ToString();
							apiResponse.MAPHIEU = ct_PhieuGiaoHang.MAPHIEU;
						}
					}
				}
				else
				{
					apiResponse.Success = false;
					apiResponse.Data = Utility.GetModelState(base.ModelState, "Delivery");
				}
				apiResponse.ID = ct_PhieuGiaoHang.ID;
				ct_PhieuGiaoHang.lstdm_Xe = new List<v_dm_Xe>();
				ct_PhieuGiaoHang.lstdm_Xe = Utility.GetListData<v_dm_Xe>("Car", "", "", Utility.LOC_ID).Data as List<v_dm_Xe>;
				List<ValueEdit> list = Utility.ConvertobjectToView(ct_PhieuGiaoHang);
				apiResponse.ProductCombo = Utility.GetDelivery_Detail(ct_PhieuGiaoHang.lstct_PhieuGiaoHang_ChiTiet);
				list.Add(new ValueEdit
				{
					Key = "lstct_PhieuGiaoHang_ChiTiet",
					Value = apiResponse.ProductCombo
				});
				apiResponse.ProductCombo = Utility.GetDelivery_Shipper(ct_PhieuGiaoHang.lstct_PhieuGiaoHang_NhanVienGiao);
				list.Add(new ValueEdit
				{
					Key = "lstct_PhieuGiaoHang_NhanVienGiao",
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
				if (!Utility.KiemTraQuyen("Delivery", "Edit"))
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
				v_v_ct_PhieuGiaoHang v_v_ct_PhieuGiaoHang2 = new v_v_ct_PhieuGiaoHang();
				if (!string.IsNullOrEmpty(id))
				{
					apiResponse = Utility.GetDetail<v_v_ct_PhieuGiaoHang>(Utility.LOC_ID + "/" + id, "Delivery");
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
						v_v_ct_PhieuGiaoHang2 = apiResponse.Data as v_v_ct_PhieuGiaoHang;
					}
				}
				v_v_ct_PhieuGiaoHang2.lstdm_Xe = new List<v_dm_Xe>();
				v_v_ct_PhieuGiaoHang2.lstdm_Xe = Utility.GetListData<v_dm_Xe>("Car", "", "", Utility.LOC_ID).Data as List<v_dm_Xe>;
				List<v_ct_PhieuGiaoHang_ChiTiet> list = new List<v_ct_PhieuGiaoHang_ChiTiet>();
				foreach (v_ct_PhieuGiaoHang_ChiTiet item in v_v_ct_PhieuGiaoHang2.lstct_PhieuGiaoHang_ChiTiet)
				{
					list.Add(item);
				}
				List<v_ct_PhieuGiaoHang_NhanVienGiao> list2 = new List<v_ct_PhieuGiaoHang_NhanVienGiao>();
				foreach (v_ct_PhieuGiaoHang_NhanVienGiao item2 in v_v_ct_PhieuGiaoHang2.lstct_PhieuGiaoHang_NhanVienGiao)
				{
					list2.Add(item2);
				}
				base.Session["lstDelivery_Detail"] = list;
				base.Session["lstDelivery_Shipper"] = list2;
				List<ValueEdit> list3 = Utility.ConvertobjectTo(v_v_ct_PhieuGiaoHang2);
				apiResponse.ProductCombo = Utility.GetDelivery_Detail(list);
				list3.Add(new ValueEdit
				{
					Key = "lstct_PhieuGiaoHang_ChiTietEdit",
					Value = apiResponse.ProductCombo
				});
				apiResponse.ProductCombo = Utility.GetDelivery_Shipper(list2);
				list3.Add(new ValueEdit
				{
					Key = "lstct_PhieuGiaoHang_NhanVienGiaoEdit",
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
		public ActionResult EditPopup([Bind(Include = "LOC_ID,ID,ID_XEGIAOHANG,MAPHIEU,SOPHIEU,NGAYLAP,GHICHU,ISHOANTAT,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO")] v_v_ct_PhieuGiaoHang ct_PhieuGiaoHang)
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
				if (!Utility.KiemTraQuyen("Delivery", "Edit"))
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
				IEnumerable<string> enumerable = base.Request.Form.AllKeys.Where((string e) => e.StartsWith("txtDetail"));
				if (enumerable == null || enumerable.Count() == 0)
				{
					base.ModelState.AddModelError("lstct_PhieuGiaoHang_ChiTiet", "Thêm danh sách phiếu xuất.");
				}
				IEnumerable<string> enumerable2 = base.Request.Form.AllKeys.Where((string e) => e.StartsWith("txtShipper"));
				if (enumerable == null || enumerable.Count() == 0)
				{
					base.ModelState.AddModelError("lstct_PhieuGiaoHang_NhanVienGiao", "Thêm nhân viên giao.");
				}
				if (base.ModelState.IsValid)
				{
					ct_PhieuGiaoHang.LOC_ID = Utility.LOC_ID;
					ct_PhieuGiaoHang.ID_NGUOISUA = base.Session["idUser"].ToString();
					ct_PhieuGiaoHang.THOIGIANSUA = Utility.CurrentTime;
					ct_PhieuGiaoHang.lstct_PhieuGiaoHang_ChiTiet = new List<v_ct_PhieuGiaoHang_ChiTiet>();
					ct_PhieuGiaoHang.lstct_PhieuGiaoHang_NhanVienGiao = new List<v_ct_PhieuGiaoHang_NhanVienGiao>();
					v_ct_PhieuGiaoHang_ChiTiet v_ct_PhieuGiaoHang_ChiTiet2 = new v_ct_PhieuGiaoHang_ChiTiet();
					foreach (string item in enumerable)
					{
						string[] array = item.ToString().Split('|');
						string[] values = base.HttpContext.Request.Params.GetValues(item.ToString());
						string value = clsMaHoa.Decrypt(array[1].ToString(), "tmt6364");
						v_ct_PhieuGiaoHang_ChiTiet v_ct_PhieuGiaoHang_ChiTiet3 = JsonConvert.DeserializeObject<v_ct_PhieuGiaoHang_ChiTiet>(value);
						if (v_ct_PhieuGiaoHang_ChiTiet2.ID != v_ct_PhieuGiaoHang_ChiTiet3.ID)
						{
							v_ct_PhieuGiaoHang_ChiTiet2 = new v_ct_PhieuGiaoHang_ChiTiet();
							v_ct_PhieuGiaoHang_ChiTiet2 = JsonConvert.DeserializeObject<v_ct_PhieuGiaoHang_ChiTiet>(value);
							v_ct_PhieuGiaoHang_ChiTiet2.LOC_ID = ct_PhieuGiaoHang.LOC_ID;
							ct_PhieuGiaoHang.lstct_PhieuGiaoHang_ChiTiet.Add(v_ct_PhieuGiaoHang_ChiTiet2);
						}
						Utility.EditObject(v_ct_PhieuGiaoHang_ChiTiet2, array[0].ToString().Substring(3, array[0].ToString().Length - 3), values[0]);
					}
					v_ct_PhieuGiaoHang_NhanVienGiao v_ct_PhieuGiaoHang_NhanVienGiao2 = new v_ct_PhieuGiaoHang_NhanVienGiao();
					foreach (string item2 in enumerable2)
					{
						string[] array2 = item2.ToString().Split('|');
						string[] values2 = base.HttpContext.Request.Params.GetValues(item2.ToString());
						string value2 = clsMaHoa.Decrypt(array2[1].ToString(), "tmt6364");
						v_ct_PhieuGiaoHang_NhanVienGiao v_ct_PhieuGiaoHang_NhanVienGiao3 = JsonConvert.DeserializeObject<v_ct_PhieuGiaoHang_NhanVienGiao>(value2);
						if (v_ct_PhieuGiaoHang_NhanVienGiao2.ID != v_ct_PhieuGiaoHang_NhanVienGiao3.ID)
						{
							v_ct_PhieuGiaoHang_NhanVienGiao2 = new v_ct_PhieuGiaoHang_NhanVienGiao();
							v_ct_PhieuGiaoHang_NhanVienGiao2 = JsonConvert.DeserializeObject<v_ct_PhieuGiaoHang_NhanVienGiao>(value2);
							v_ct_PhieuGiaoHang_NhanVienGiao2.LOC_ID = ct_PhieuGiaoHang.LOC_ID;
							ct_PhieuGiaoHang.lstct_PhieuGiaoHang_NhanVienGiao.Add(v_ct_PhieuGiaoHang_NhanVienGiao2);
						}
						Utility.EditObject(v_ct_PhieuGiaoHang_NhanVienGiao2, array2[0].ToString().Substring(3, array2[0].ToString().Length - 3), values2[0]);
					}
					apiResponse = Utility.Edit(Utility.LOC_ID + "/" + ct_PhieuGiaoHang.ID, (v_ct_PhieuGiaoHang)ct_PhieuGiaoHang, "Delivery");
					if (apiResponse.Success)
					{
						apiResponse.ID = ct_PhieuGiaoHang.ID;
						if (apiResponse.Data != null)
						{
							ct_PhieuGiaoHang = JsonConvert.DeserializeObject<v_v_ct_PhieuGiaoHang>(apiResponse.Data.ToString());
						}
					}
					else
					{
						base.ModelState.AddModelError(string.Empty, apiResponse.Message);
					}
				}
				else
				{
					apiResponse.Success = false;
					apiResponse.Data = Utility.GetModelState(base.ModelState, "Delivery");
				}
				ct_PhieuGiaoHang.lstdm_Xe = new List<v_dm_Xe>();
				ct_PhieuGiaoHang.lstdm_Xe = Utility.GetListData<v_dm_Xe>("Car", "", "", Utility.LOC_ID).Data as List<v_dm_Xe>;
				apiResponse.Detail = Utility.ConvertobjectToView(ct_PhieuGiaoHang);
				List<ValueEdit> list = Utility.ConvertobjectToView(ct_PhieuGiaoHang);
				apiResponse.ProductCombo = Utility.GetDelivery_Detail(ct_PhieuGiaoHang.lstct_PhieuGiaoHang_ChiTiet);
				list.Add(new ValueEdit
				{
					Key = "lstct_PhieuGiaoHang_ChiTietEdit",
					Value = apiResponse.ProductCombo
				});
				apiResponse.ProductCombo = Utility.GetDelivery_Shipper(ct_PhieuGiaoHang.lstct_PhieuGiaoHang_NhanVienGiao);
				list.Add(new ValueEdit
				{
					Key = "lstct_PhieuGiaoHang_NhanVienGiaoEdit",
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
				if (!Utility.KiemTraQuyen("Delivery", "Delete"))
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
				apiResponse = Utility.Delete<v_ct_PhieuGiaoHang>(Utility.LOC_ID + "/" + id, "Delivery");
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

		[HttpGet]
		public ActionResult AddDeliveryDetail(string cartOrder)
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
				v_v_ct_PhieuXuat v_v_ct_PhieuXuat2 = new v_v_ct_PhieuXuat();
				List<v_ct_PhieuGiaoHang_ChiTiet> lstPhieuGiaoHang_ChiTiet = Utility.LstPhieuGiaoHang_ChiTiet;
				Return obj = new Return();
				List<Deposit> list = new JavaScriptSerializer().Deserialize<List<Deposit>>(cartOrder);
				foreach (Deposit Deposit in list)
				{
					if (lstPhieuGiaoHang_ChiTiet.Where((v_ct_PhieuGiaoHang_ChiTiet e) => e.ID_PHIEUXUAT == Deposit.ID).Count() > 0)
					{
						apiResponse.Success = true;
						continue;
					}
					apiResponse = Utility.GetDetail<v_v_ct_PhieuXuat>(Utility.LOC_ID + "/" + Deposit.ID, "Output");
					if (!apiResponse.Success)
					{
						base.TempData["TitleError"] = apiResponse.Message;
						return RedirectToAction("Index", "Notfound");
					}
					if (apiResponse.Data != null)
					{
						v_v_ct_PhieuXuat2 = apiResponse.Data as v_v_ct_PhieuXuat;
					}
					v_ct_PhieuGiaoHang_ChiTiet v_ct_PhieuGiaoHang_ChiTiet2 = new v_ct_PhieuGiaoHang_ChiTiet();
					v_ct_PhieuGiaoHang_ChiTiet2.ID = Guid.NewGuid().ToString();
					v_ct_PhieuGiaoHang_ChiTiet2.ID_PHIEUXUAT = v_v_ct_PhieuXuat2.ID;
					v_ct_PhieuGiaoHang_ChiTiet2.MAPHIEUXUAT = v_v_ct_PhieuXuat2.MAPHIEU;
					v_ct_PhieuGiaoHang_ChiTiet2.NGAYLAP = v_v_ct_PhieuXuat2.NGAYLAP;
					v_ct_PhieuGiaoHang_ChiTiet2.ID_KHACHHANG_NCC = v_v_ct_PhieuXuat2.ID_KHACHHANG;
					v_ct_PhieuGiaoHang_ChiTiet2.NAME_KHACHHANG_NCC = v_v_ct_PhieuXuat2.NAME_KHACHHANG_NCC;
					v_ct_PhieuGiaoHang_ChiTiet2.SOTIENGIAOHANG = v_v_ct_PhieuXuat2.TONGTIEN;
					v_ct_PhieuGiaoHang_ChiTiet2.TONGSOLUONG = v_v_ct_PhieuXuat2.lstct_PhieuXuat_ChiTiet.Sum((v_ct_PhieuXuat_ChiTiet e) => e.SOLUONG);
					v_ct_PhieuGiaoHang_ChiTiet2.TONGKHOILUONG = v_v_ct_PhieuXuat2.lstct_PhieuXuat_ChiTiet.Sum((v_ct_PhieuXuat_ChiTiet e) => e.TONGSOLUONG * e.TRONGLUONG);
					lstPhieuGiaoHang_ChiTiet.Add(v_ct_PhieuGiaoHang_ChiTiet2);
				}
				base.Session["lstDelivery_Detail"] = lstPhieuGiaoHang_ChiTiet;
				List<ValueEdit> list2 = new List<ValueEdit>();
				apiResponse.ProductCombo = Utility.GetDelivery_Detail(lstPhieuGiaoHang_ChiTiet);
				list2.Add(new ValueEdit
				{
					Key = "lstct_PhieuGiaoHang_ChiTietEdit",
					Value = apiResponse.ProductCombo
				});
				list2.Add(new ValueEdit
				{
					Key = "lstct_PhieuGiaoHang_ChiTiet",
					Value = apiResponse.ProductCombo
				});
				apiResponse.Detail = list2;
				apiResponse.Success = true;
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

		[HttpGet]
		public ActionResult AddDeliveryShipper(string ID)
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
				v_dm_NhanVien v_dm_NhanVien2 = new v_dm_NhanVien();
				List<v_ct_PhieuGiaoHang_NhanVienGiao> lstPhieuGiaoHang_NhanVienGiao = Utility.LstPhieuGiaoHang_NhanVienGiao;
				if (lstPhieuGiaoHang_NhanVienGiao.Where((v_ct_PhieuGiaoHang_NhanVienGiao e) => e.ID_NHANVIENGIAO == ID).Count() <= 0)
				{
					apiResponse = Utility.GetDetail<v_dm_NhanVien>(Utility.LOC_ID + "/" + ID, "Employee");
					if (!apiResponse.Success)
					{
						base.TempData["TitleError"] = apiResponse.Message;
						return RedirectToAction("Index", "Notfound");
					}
					if (apiResponse.Data != null)
					{
						v_dm_NhanVien2 = apiResponse.Data as v_dm_NhanVien;
					}
					v_ct_PhieuGiaoHang_NhanVienGiao v_ct_PhieuGiaoHang_NhanVienGiao2 = new v_ct_PhieuGiaoHang_NhanVienGiao();
					v_ct_PhieuGiaoHang_NhanVienGiao2.ID = Guid.NewGuid().ToString();
					v_ct_PhieuGiaoHang_NhanVienGiao2.ID_NHANVIENGIAO = v_dm_NhanVien2.ID;
					v_ct_PhieuGiaoHang_NhanVienGiao2.MA_NHANVIEN = v_dm_NhanVien2.MA;
					v_ct_PhieuGiaoHang_NhanVienGiao2.NAME_NHANVIEN = v_dm_NhanVien2.NAME;
					lstPhieuGiaoHang_NhanVienGiao.Add(v_ct_PhieuGiaoHang_NhanVienGiao2);
				}
				List<ValueEdit> list = new List<ValueEdit>();
				base.Session["lstDelivery_Shipper"] = lstPhieuGiaoHang_NhanVienGiao;
				apiResponse.ProductCombo = Utility.GetDelivery_Shipper(lstPhieuGiaoHang_NhanVienGiao);
				list.Add(new ValueEdit
				{
					Key = "lstct_PhieuGiaoHang_NhanVienGiaoEdit",
					Value = apiResponse.ProductCombo
				});
				list.Add(new ValueEdit
				{
					Key = "lstct_PhieuGiaoHang_NhanVienGiao",
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

		[HttpGet]
		public ActionResult DeleteDeliveryDetail(string ID)
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
				List<v_ct_PhieuGiaoHang_ChiTiet> lstPhieuGiaoHang_ChiTiet = Utility.LstPhieuGiaoHang_ChiTiet;
				v_ct_PhieuGiaoHang_ChiTiet v_ct_PhieuGiaoHang_ChiTiet2 = lstPhieuGiaoHang_ChiTiet.FirstOrDefault((v_ct_PhieuGiaoHang_ChiTiet e) => e.ID_PHIEUXUAT == ID);
				if (v_ct_PhieuGiaoHang_ChiTiet2 != null)
				{
					lstPhieuGiaoHang_ChiTiet.Remove(v_ct_PhieuGiaoHang_ChiTiet2);
				}
				base.Session["lstDelivery_Detail"] = lstPhieuGiaoHang_ChiTiet;
				List<ValueEdit> list = new List<ValueEdit>();
				apiResponse.ProductCombo = Utility.GetDelivery_Detail(lstPhieuGiaoHang_ChiTiet);
				list.Add(new ValueEdit
				{
					Key = "lstct_PhieuGiaoHang_ChiTietEdit",
					Value = apiResponse.ProductCombo
				});
				list.Add(new ValueEdit
				{
					Key = "lstct_PhieuGiaoHang_ChiTiet",
					Value = apiResponse.ProductCombo
				});
				apiResponse.Success = true;
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

		[HttpGet]
		public ActionResult DeleteDeliveryShipper(string ID)
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
				List<v_ct_PhieuGiaoHang_NhanVienGiao> lstPhieuGiaoHang_NhanVienGiao = Utility.LstPhieuGiaoHang_NhanVienGiao;
				v_ct_PhieuGiaoHang_NhanVienGiao v_ct_PhieuGiaoHang_NhanVienGiao2 = lstPhieuGiaoHang_NhanVienGiao.FirstOrDefault((v_ct_PhieuGiaoHang_NhanVienGiao e) => e.ID_NHANVIENGIAO == ID);
				if (v_ct_PhieuGiaoHang_NhanVienGiao2 != null)
				{
					lstPhieuGiaoHang_NhanVienGiao.Remove(v_ct_PhieuGiaoHang_NhanVienGiao2);
				}
				base.Session["lstDelivery_Shipper"] = lstPhieuGiaoHang_NhanVienGiao;
				List<ValueEdit> list = new List<ValueEdit>();
				apiResponse.ProductCombo = Utility.GetDelivery_Shipper(lstPhieuGiaoHang_NhanVienGiao);
				list.Add(new ValueEdit
				{
					Key = "lstct_PhieuGiaoHang_NhanVienGiaoEdit",
					Value = apiResponse.ProductCombo
				});
				list.Add(new ValueEdit
				{
					Key = "lstct_PhieuGiaoHang_NhanVienGiao",
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

		public ActionResult Search(DateTime? FromDate = null, DateTime? ToDate = null, string SearchString = "", string ID_KHUVUC = "")
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
				if (!Utility.KiemTraQuyen("Output", "View"))
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
				List<v_ct_PhieuXuat> list = new List<v_ct_PhieuXuat>();
				string text = "";
				string text2 = "";
				if (FromDate.HasValue)
				{
					apiResponse = Utility.Get_DanhSachPhieuXuat_TimKiem<v_ct_PhieuXuat>("", FromDate, ToDate, SearchString, "", ID_KHUVUC);
					if (!apiResponse.Success)
					{
						base.TempData["TitleError"] = apiResponse.Message;
						return RedirectToAction("Index", "Notfound");
					}
					if (apiResponse.Data != null)
					{
						Login_Model login_Model = (Login_Model)base.Session["Login_Model"];
						list = (from s in apiResponse.Data as List<v_ct_PhieuXuat>
								where !s.ISHOANTAT && !string.IsNullOrEmpty(s.ID_KHACHHANG)
								orderby s.NGAYLAP descending
								select s).ToList();
						IEnumerable<PropertyInfo> runtimeProperties = typeof(v_ct_PhieuXuat).GetRuntimeProperties();
						List<view_web_NoteClass> list2 = Utility.GetNoteClass();
						if (list2 != null)
						{
							list2 = list2.Where((view_web_NoteClass s) => !string.IsNullOrEmpty(s.NAMECLASS) && s.NAMECLASS.ToLower() == typeof(v_ct_PhieuXuat).Name.Replace("v_", "").ToLower() && s.ISSEARCH).ToList();
						}
						if (list2 != null && list2.Count > 0)
						{
							text += "<th style=\"font-weight: bold; text-align:center; white-space: nowrap;\">";
							text += "<input type=\"checkbox\" onchange=\"OnchangeCheckbox(event, 'tbodySearchDelivery')\" />";
							text += "</th>";
							foreach (view_web_NoteClass item in list2.OrderBy((view_web_NoteClass s) => s.STT))
							{
								text = text + "<th style=\"font-weight: bold; text-align:center; white-space: nowrap;\"> " + item.DISPLAYNAME + "</th>";
							}
							foreach (v_ct_PhieuXuat item2 in list)
							{
								text2 = text2 + "<tr id=\"" + item2.ID + "\">";
								text2 = text2 + "<td style=\"white-space: nowrap; \" id=\"" + item2.ID + "\"><input type=\"checkbox\" id=\"" + item2.ID + "\" name=\"TBL_ITEM\" onchange=\"checkboxChanged()\" class=\"cbx\"></td>";
								foreach (view_web_NoteClass itmSearch in list2.OrderBy((view_web_NoteClass s) => s.STT))
								{
									PropertyInfo propertyInfo = runtimeProperties.Where((PropertyInfo e) => e.Name.ToUpper() == (string.IsNullOrEmpty(itmSearch.REPLACESEARCH) ? itmSearch.NAMECOLUMN : itmSearch.REPLACESEARCH).ToUpper()).FirstOrDefault();
									if (propertyInfo != null)
									{
										object value = propertyInfo.GetValue(item2);
										if (value != null && value.GetType().ToString().Contains("Date"))
										{
											text2 = text2 + "<td style=\"white-space: nowrap; \" id=\"" + propertyInfo.Name + "\">" + ((DateTime)value).ToString("dd/MM/yyyy") + "</td></a>";
										}
										else if (value != null && value.GetType().ToString().Contains("Bool"))
										{
											text2 = text2 + "<td style=\"white-space: nowrap; \" id=\"" + propertyInfo.Name + "\"><input " + (((bool)value) ? "checked=\"checked\"" : "") + " class=\"check-box\" disabled=\"disabled\" type=\"checkbox\"></td>";
										}
										else if (value != null && Utility.IsNumericType(value.GetType()))
										{
											decimal num = Convert.ToDecimal(value);
											text2 = text2 + "<td style=\"white-space: nowrap; \" id=\"" + propertyInfo.Name + "\">" + num.ToString("N0") + "</td>";
										}
										else
										{
											text2 = text2 + "<td style=\"white-space: nowrap; \" id=\"" + propertyInfo.Name + "\">" + value?.ToString() + "</td>";
										}
									}
									else
									{
										text2 += "<td></td>";
									}
								}
								text2 += "</tr>";
							}
						}
					}
				}
				List<ValueEdit> list3 = new List<ValueEdit>();
				list3.Add(new ValueEdit
				{
					Key = "tbodySearchDelivery",
					Value = text2
				});
				list3.Add(new ValueEdit
				{
					Key = "trSearchDelivery",
					Value = text
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

		public ActionResult CheckData(string ID = "")
		{
			try
			{
				if (Utility.KiemTra())
				{
					return RedirectToAction("Index", "Admin");
				}
				if (!Utility.KiemTraQuyen("Delivery", "View"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				ApiResponse apiResponse = new ApiResponse();
				v_ct_PhieuGiaoHang v_ct_PhieuGiaoHang2 = new v_ct_PhieuGiaoHang();
				if (!string.IsNullOrEmpty(ID))
				{
					apiResponse = Utility.GetDetail<v_v_ct_PhieuGiaoHang>(Utility.LOC_ID + "/" + ID, "Delivery");
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
						v_ct_PhieuGiaoHang2 = apiResponse.Data as v_ct_PhieuGiaoHang;
					}
				}
				base.ViewBag.PermissionEdit = Utility.KiemTraQuyen("Delivery", "Edit");
				base.ViewBag.PermissionDelete = Utility.KiemTraQuyen("Delivery", "Delete");
				base.ViewBag.PermissionCreate = Utility.KiemTraQuyen("Payment", "Create");
				base.ViewBag.PermissionDelivery_CreateReceipt = Utility.KiemTraQuyen("Delivery", "Delivery_CreateReceipt");
				base.ViewBag.PermissionDelivery_CreateReturn = Utility.KiemTraQuyen("Delivery", "Delivery_CreateReturn");
				List<v_dm_LoaiPhieuThu> list = Utility.GetListData<v_dm_LoaiPhieuThu>("TypeReceipt", "", "", Utility.LOC_ID).Data as List<v_dm_LoaiPhieuThu>;
				v_dm_LoaiPhieuThu v_dm_LoaiPhieuThu2 = list.FirstOrDefault((v_dm_LoaiPhieuThu e) => e.MA == API.PTKH);
				List<v_dm_LoaiPhieuNhap> source = Utility.GetListData<v_dm_LoaiPhieuNhap>("TypeInput", "", "", Utility.LOC_ID).Data as List<v_dm_LoaiPhieuNhap>;
				v_dm_LoaiPhieuNhap v_dm_LoaiPhieuNhap2 = source.FirstOrDefault((v_dm_LoaiPhieuNhap e) => e.MA == API.NTHKH);
				v_ct_PhieuGiaoHang2.lstdm_LoaiPhieuChi = new List<v_dm_LoaiPhieuChi>();
				if (Utility.GetListData<v_dm_LoaiPhieuChi>("TypePayment", "", "", Utility.LOC_ID).Data is List<v_dm_LoaiPhieuChi> source2)
				{
					v_ct_PhieuGiaoHang2.lstdm_LoaiPhieuChi = (from e in source2
															  where e.ISACTIVE && (e.TYPE == 3 || e.TYPE == 4)
															  orderby e.TYPE
															  select e).ToList();
				}
				else
				{
					v_ct_PhieuGiaoHang2.lstdm_LoaiPhieuChi = new List<v_dm_LoaiPhieuChi>();
				}
				v_dm_LoaiPhieuChi v_dm_LoaiPhieuChi2 = v_ct_PhieuGiaoHang2.lstdm_LoaiPhieuChi.FirstOrDefault((v_dm_LoaiPhieuChi e) => e.MA == API.PCGCNKHCNV);
				v_ct_PhieuGiaoHang2.lstdm_LoaiPhieuThu = new List<v_dm_LoaiPhieuThu>();
				if (list != null)
				{
					v_ct_PhieuGiaoHang2.lstdm_LoaiPhieuThu = (from e in list
															  where e.ISACTIVE && (e.TYPE == 2 || e.TYPE == 3)
															  orderby e.TYPE
															  select e).ToList();
				}
				else
				{
					v_ct_PhieuGiaoHang2.lstdm_LoaiPhieuThu = new List<v_dm_LoaiPhieuThu>();
				}
				base.ViewBag.ID_LOAIPHIEUTHU = ((v_dm_LoaiPhieuThu2 != null) ? v_dm_LoaiPhieuThu2.ID : "");
				base.ViewBag.ID_LOAIPHIEUNHAP = ((v_dm_LoaiPhieuNhap2 != null) ? v_dm_LoaiPhieuNhap2.ID : "");
				base.ViewBag.ID_LOAIPHIEUCHI = ((v_dm_LoaiPhieuChi2 != null) ? v_dm_LoaiPhieuChi2.ID : "");
				v_ct_PhieuGiaoHang2.SOLAN = v_ct_PhieuGiaoHang2.lstct_PhieuGiaoHang_ChiTiet.Max((v_ct_PhieuGiaoHang_ChiTiet s) => s.SOLAN);
				return View(v_ct_PhieuGiaoHang2);
			}
			catch (Exception ex)
			{
				Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
				base.TempData["TitleError"] = "Đã có lỗi phát sinh trong phiên làm việc!";
				base.TempData["DetailError"] = ex.Message;
				return RedirectToAction("Index", "Notfound");
			}
		}

		public ActionResult Completed_Detail(string ID = "", string TRANGTHAI = "")
		{
			ApiResponse apiResponse = new ApiResponse();
			try
			{
				if (Utility.KiemTra())
				{
					return RedirectToAction("Index", "Admin");
				}
				if (!Utility.KiemTraQuyen("Delivery", "View"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				v_ct_PhieuGiaoHang v_ct_PhieuGiaoHang2 = new v_ct_PhieuGiaoHang();
				v_ct_PhieuGiaoHang_ChiTiet v_ct_PhieuGiaoHang_ChiTiet2 = new v_ct_PhieuGiaoHang_ChiTiet();
				if (!string.IsNullOrEmpty(ID))
				{
					apiResponse = Utility.GetDetail<v_ct_PhieuGiaoHang_ChiTiet>(Utility.LOC_ID + "/" + ID, "Delivery_Detail");
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
						v_ct_PhieuGiaoHang_ChiTiet2 = apiResponse.Data as v_ct_PhieuGiaoHang_ChiTiet;
					}
					if (TRANGTHAI != null && TRANGTHAI.Contains("1"))
					{
						v_ct_PhieuGiaoHang_ChiTiet2.ISDAGIAOHANG = true;
					}
					else
					{
						v_ct_PhieuGiaoHang_ChiTiet2.ISDAGIAOHANG = false;
					}
					apiResponse = Utility.Edit(Utility.LOC_ID + "/" + ID, v_ct_PhieuGiaoHang_ChiTiet2, "Delivery_Detail");
				}
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

		public ActionResult Completed(string ID = "", string TRANGTHAI = "")
		{
			ApiResponse apiResponse = new ApiResponse();
			try
			{
				if (Utility.KiemTra())
				{
					return RedirectToAction("Index", "Admin");
				}
				if (!Utility.KiemTraQuyen("Delivery", "View"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				v_ct_PhieuGiaoHang v_ct_PhieuGiaoHang2 = new v_ct_PhieuGiaoHang();
				if (!string.IsNullOrEmpty(ID))
				{
					apiResponse = Utility.GetDetail<v_ct_PhieuGiaoHang>(Utility.LOC_ID + "/" + ID, "Delivery");
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
						v_ct_PhieuGiaoHang2 = apiResponse.Data as v_ct_PhieuGiaoHang;
					}
					if (TRANGTHAI != null && TRANGTHAI.Contains("1"))
					{
						foreach (v_ct_PhieuGiaoHang_ChiTiet item in v_ct_PhieuGiaoHang2.lstct_PhieuGiaoHang_ChiTiet)
						{
							if (!item.ISDAGIAOHANG)
							{
								apiResponse.Success = false;
								apiResponse.Message = "Chưa giao hàng phiếu xuất " + item.MAPHIEUXUAT + "!";
							}
						}
						v_ct_PhieuGiaoHang2.ISHOANTAT = true;
					}
					else
					{
						v_ct_PhieuGiaoHang2.ISHOANTAT = false;
					}
					apiResponse = Utility.Edit(Utility.LOC_ID + "/" + ID, v_ct_PhieuGiaoHang2, "Delivery");
				}
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

		public ActionResult ViewReport(string ID)
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
				v_ct_PhieuGiaoHang v_ct_PhieuGiaoHang2 = new v_ct_PhieuGiaoHang();
				SP_Parameter sP_Parameter = new SP_Parameter();
				sP_Parameter.LOC_ID = Utility.LOC_ID;
				sP_Parameter.ID_PHIEUGIAOHANG = ID;
				apiResponse = Utility.ExecuteStoredProc<v_ct_PhieuGiaoHang>(sP_Parameter, "Sp_Get_DanhSachPhieuGiaoHang");
				if (!apiResponse.Success)
				{
					apiResponse.Success = false;
					apiResponse.Message = apiResponse.Message;
					return new JsonResult
					{
						Data = apiResponse,
						JsonRequestBehavior = JsonRequestBehavior.AllowGet,
						MaxJsonLength = int.MaxValue
					};
				}
				if (apiResponse.Data != null)
				{
					v_ct_PhieuGiaoHang2 = (apiResponse.Data as List<v_ct_PhieuGiaoHang>).FirstOrDefault();
				}
				SP_Parameter_Report sP_Parameter_Report = new SP_Parameter_Report();
				sP_Parameter_Report.NAME_SP = "Sp_Get_DanhSachPhieuGiaoHang_PhieuXuat";
				sP_Parameter_Report.LOC_ID = Utility.LOC_ID;
				sP_Parameter_Report.ID_PHIEUGIAOHANG = ID;
				ReportClass reportClass = new ReportClass();
				apiResponse = Utility.ExecuteStoredProc<DataTable>(sP_Parameter_Report, "SP_GetReport");
				if (!apiResponse.Success)
				{
					apiResponse.Success = false;
					apiResponse.Message = apiResponse.Message;
					return new JsonResult
					{
						Data = apiResponse,
						JsonRequestBehavior = JsonRequestBehavior.AllowGet,
						MaxJsonLength = int.MaxValue
					};
				}
				string filename = Path.Combine(base.Server.MapPath("~/Images_Upload/Logo/"), "logoTrangHiepPhat.jpg");
				string plainText = Utility.UrlWebsite + "/Delivery/CheckData?ID=" + ((v_ct_PhieuGiaoHang2 != null) ? v_ct_PhieuGiaoHang2.ID : "");
				QRCodeGenerator qRCodeGenerator = new QRCodeGenerator();
				Bitmap icon = new Bitmap(filename);
				QRCodeData data = qRCodeGenerator.CreateQrCode(plainText, QRCodeGenerator.ECCLevel.Q);
				QRCode qRCode = new QRCode(data);
				Bitmap graphic = qRCode.GetGraphic(9, Color.Black, Color.White, icon);
				string text = Path.Combine(base.Server.MapPath("~/Images_Upload/Product/"), "MyBinaryQR.png");
				graphic.Save(text, System.Drawing.Imaging.ImageFormat.Png);
				DisplayQRCodeImage(text);
				DataTable dataTable = apiResponse.Data as DataTable;
				if (apiResponse.CheckValue)
				{
					dataTable.Rows.Clear();
				}
				if (dataTable.Columns.Contains("QR_CODE"))
				{
					foreach (DataRow row in dataTable.Rows)
					{
						row["QR_CODE"] = Utility.UrlWebsite + "/Output/Edit?ID=" + row["ID"];
					}
				}
				if (v_ct_PhieuGiaoHang2 != null)
				{
					reportClass = Utility.GetFormulaFields(reportClass, v_ct_PhieuGiaoHang2);
				}
				reportClass.DataDefinition.FormulaFields["QRCode"].Text = "'" + text + "'";
				reportClass.SetDataSource(dataTable);
				base.Response.Buffer = false;
				base.Response.ClearContent();
				base.Response.ClearHeaders();
				Stream stream = reportClass.ExportToStream(ExportFormatType.PortableDocFormat);
				Utility.Report = reportClass;
				apiResponse = new ApiResponse();
				apiResponse.Success = true;
				apiResponse.NAME = Utility.GetTitleFrom("Delivery") + " - " + ((v_ct_PhieuGiaoHang2 != null) ? v_ct_PhieuGiaoHang2.MAPHIEU : "");
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

		private static void DisplayQRCodeImage(string imagePath)
		{
			try
			{
				if (!System.IO.File.Exists(imagePath))
				{
					Console.WriteLine("QR code image not found.");
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error: " + ex.Message);
			}
		}

		public ActionResult ViewReportType(string ID, string LOAIPHIEUIN, int SOLAN = -1)
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
				v_ct_PhieuGiaoHang v_ct_PhieuGiaoHang2 = new v_ct_PhieuGiaoHang();
				SP_Parameter sP_Parameter = new SP_Parameter();
				sP_Parameter.LOC_ID = Utility.LOC_ID;
				sP_Parameter.ID_PHIEUGIAOHANG = ID;
				apiResponse = Utility.ExecuteStoredProc<v_ct_PhieuGiaoHang>(sP_Parameter, "Sp_Get_DanhSachPhieuGiaoHang");
				if (!apiResponse.Success)
				{
					apiResponse.Success = false;
					apiResponse.Message = apiResponse.Message;
					return new JsonResult
					{
						Data = apiResponse,
						JsonRequestBehavior = JsonRequestBehavior.AllowGet,
						MaxJsonLength = int.MaxValue
					};
				}
				if (apiResponse.Data != null)
				{
					v_ct_PhieuGiaoHang2 = (apiResponse.Data as List<v_ct_PhieuGiaoHang>).FirstOrDefault();
				}
				SP_Parameter sP_Parameter2 = new SP_Parameter();
				sP_Parameter2.LOC_ID = Utility.LOC_ID;
				sP_Parameter2.ID_PHIEUGIAOHANG = ID;
				sP_Parameter2.SOLAN = SOLAN;
				ReportClass report = new ReportClass();
				apiResponse = Utility.ExecuteStoredProc<Sp_Get_DanhSachPhieuGiaoHang_In>(sP_Parameter2, (LOAIPHIEUIN == "3") ? "Sp_Get_DanhSachPhieuGiaoHang_InPhieuGiao" : "Sp_Get_DanhSachPhieuGiaoHang_In");
				if (!apiResponse.Success)
				{
					apiResponse.Success = false;
					apiResponse.Message = apiResponse.Message;
					return new JsonResult
					{
						Data = apiResponse,
						JsonRequestBehavior = JsonRequestBehavior.AllowGet,
						MaxJsonLength = int.MaxValue
					};
				}
				string mapPath = "~/Report/rptDanhSachPhieuGiaoHang_InGroupType.rpt";
				List<Sp_Get_DanhSachPhieuGiaoHang_In> lstSp_Get_DanhSachPhieuGiaoHang_In = apiResponse.Data as List<Sp_Get_DanhSachPhieuGiaoHang_In>;
				List<v_PhieuGioaHang_InTheoGroup> list = new List<v_PhieuGioaHang_InTheoGroup>();
				int num = 0;
				string text = "";
				string text2 = "";
				if (LOAIPHIEUIN == "1")
				{
					v_v_ct_PhieuGiaoHang v_v_ct_PhieuGiaoHang2 = new v_v_ct_PhieuGiaoHang();
					if (!string.IsNullOrEmpty(ID))
					{
						apiResponse = Utility.GetDetail<v_v_ct_PhieuGiaoHang>(Utility.LOC_ID + "/" + ID, "Delivery");
						if (!apiResponse.Success)
						{
							base.TempData["TitleError"] = apiResponse.Message;
							return RedirectToAction("Index", "Notfound");
						}
						if (apiResponse.Data != null)
						{
							v_v_ct_PhieuGiaoHang2 = apiResponse.Data as v_v_ct_PhieuGiaoHang;
						}
					}
					mapPath = "~/Report/rptBaoCaoPhieuDatHang.rpt";
					SP_Parameter_Report sP_Parameter_Report = new SP_Parameter_Report();
					sP_Parameter_Report.ID_PHIEUGIAOHANG = ID;
					sP_Parameter_Report.SOLAN = SOLAN;
					apiResponse = Utility.ExecuteStoredProcT<v_ct_PhieuDatHang_ChiTiet_BaoCao>(sP_Parameter_Report, "Sp_Get_DanhSachPhieuGiaoHang_ChiTiet_BaoCao");
					List<v_ct_PhieuDatHang_ChiTiet_BaoCao> list2 = apiResponse.Data as List<v_ct_PhieuDatHang_ChiTiet_BaoCao>;
					if (!apiResponse.Success)
					{
						apiResponse.Success = false;
						apiResponse.Message = apiResponse.Message;
						return new JsonResult
						{
							Data = apiResponse,
							JsonRequestBehavior = JsonRequestBehavior.AllowGet,
							MaxJsonLength = int.MaxValue
						};
					}
					if (list2 != null)
					{
						text = v_v_ct_PhieuGiaoHang2.MAPHIEU;
						num = (from s in list2
							   group s by new { s.MAPHIEU }).Count();
						text2 = string.Join(";", from s in list2
												 group s by new { s.NAME_KHUVUC } into s
												 select s.Key.NAME_KHUVUC);
						list = (from s in list2
								group s by new { s.TYLE_QD, s.TRONGLUONG, s.NAME, s.MA, s.NAME_DVT, s.NAME_DVT_QD, s.NAME_NHOMHANGHOA } into s
								select new v_PhieuGioaHang_InTheoGroup
								{
									MAPHIEUXUAT = s.Key.NAME_NHOMHANGHOA,
									MA_HANGHOA = s.Key.MA,
									NAME_HANGHOA = s.Key.NAME,
									NAME_DVT = s.Key.NAME_DVT,
									CHIETKHAU = s.Sum((v_ct_PhieuDatHang_ChiTiet_BaoCao x) => Math.Round(x.CHIETKHAU, 0)),
									TONGTIENGIAMGIA = s.Sum((v_ct_PhieuDatHang_ChiTiet_BaoCao x) => Math.Round(x.TONGTIENGIAMGIA, 0)),
									THANHTIEN = s.Sum((v_ct_PhieuDatHang_ChiTiet_BaoCao x) => Math.Round(x.THANHTIEN, 0)),
									THUESUAT = s.Sum((v_ct_PhieuDatHang_ChiTiet_BaoCao x) => Math.Round(x.THUESUAT, 0)),
									TONGTIENVAT = s.Sum((v_ct_PhieuDatHang_ChiTiet_BaoCao x) => Math.Round(x.TONGTIENVAT, 0)),
									TONGCONG = s.Sum((v_ct_PhieuDatHang_ChiTiet_BaoCao x) => Math.Round(x.TONGCONG, 0)),
									TONGSOLUONG = Convert.ToDecimal(s.Sum((v_ct_PhieuDatHang_ChiTiet_BaoCao x) => Math.Round(x.TONGSOLUONG, 0))),
									TONGTRONGLUONG = Convert.ToDecimal(s.Key.TRONGLUONG * s.Sum((v_ct_PhieuDatHang_ChiTiet_BaoCao x) => Math.Round(x.TONGSOLUONG, 0))),
									NAME_DVT_QD = s.Key.NAME_DVT_QD,
									TYLE_QD = s.Key.TYLE_QD
								}).ToList();
					}
				}
				if (LOAIPHIEUIN == "4")
				{
					v_v_ct_PhieuGiaoHang v_v_ct_PhieuGiaoHang3 = new v_v_ct_PhieuGiaoHang();
					if (!string.IsNullOrEmpty(ID))
					{
						apiResponse = Utility.GetDetail<v_v_ct_PhieuGiaoHang>(Utility.LOC_ID + "/" + ID, "Delivery");
						if (!apiResponse.Success)
						{
							base.TempData["TitleError"] = apiResponse.Message;
							return RedirectToAction("Index", "Notfound");
						}
						if (apiResponse.Data != null)
						{
							v_v_ct_PhieuGiaoHang3 = apiResponse.Data as v_v_ct_PhieuGiaoHang;
						}
					}
					mapPath = "~/Report/rptBaoCaoPhieuDatHang.rpt";
					SP_Parameter_Report sP_Parameter_Report2 = new SP_Parameter_Report();
					sP_Parameter_Report2.ID_PHIEUGIAOHANG = ID;
					sP_Parameter_Report2.SOLAN = SOLAN;
					apiResponse = Utility.ExecuteStoredProcT<v_ct_PhieuDatHang_ChiTiet_BaoCao>(sP_Parameter_Report2, "Sp_Get_DanhSachPhieuGiaoHang_ChiTiet_BaoCao");
					List<v_ct_PhieuDatHang_ChiTiet_BaoCao> list3 = apiResponse.Data as List<v_ct_PhieuDatHang_ChiTiet_BaoCao>;
					if (!apiResponse.Success)
					{
						apiResponse.Success = false;
						apiResponse.Message = apiResponse.Message;
						return new JsonResult
						{
							Data = apiResponse,
							JsonRequestBehavior = JsonRequestBehavior.AllowGet,
							MaxJsonLength = int.MaxValue
						};
					}
					if (list3 != null)
					{
						text = v_v_ct_PhieuGiaoHang3.MAPHIEU;
						num = (from s in list3
							   group s by new { s.MAPHIEU }).Count();
						list = (from s in list3
								group s by new { s.NAME_KHUVUC, s.TYLE_QD, s.TRONGLUONG, s.NAME, s.MA, s.NAME_DVT, s.NAME_DVT_QD, s.NAME_NHOMHANGHOA } into s
								select new v_PhieuGioaHang_InTheoGroup
								{
									ID_KHACHHANG = "",
									NAME_GROUP = s.Key.NAME_KHUVUC,
									MAPHIEUXUAT = s.Key.NAME_NHOMHANGHOA,
									MA_HANGHOA = s.Key.MA,
									NAME_HANGHOA = s.Key.NAME,
									NAME_DVT = s.Key.NAME_DVT,
									CHIETKHAU = s.Sum((v_ct_PhieuDatHang_ChiTiet_BaoCao x) => Math.Round(x.CHIETKHAU, 0)),
									TONGTIENGIAMGIA = s.Sum((v_ct_PhieuDatHang_ChiTiet_BaoCao x) => Math.Round(x.TONGTIENGIAMGIA, 0)),
									THANHTIEN = s.Sum((v_ct_PhieuDatHang_ChiTiet_BaoCao x) => Math.Round(x.THANHTIEN, 0)),
									THUESUAT = s.Sum((v_ct_PhieuDatHang_ChiTiet_BaoCao x) => Math.Round(x.THUESUAT, 0)),
									TONGTIENVAT = s.Sum((v_ct_PhieuDatHang_ChiTiet_BaoCao x) => Math.Round(x.TONGTIENVAT, 0)),
									TONGCONG = s.Sum((v_ct_PhieuDatHang_ChiTiet_BaoCao x) => Math.Round(x.TONGCONG, 0)),
									TONGSOLUONG = Convert.ToDecimal(s.Sum((v_ct_PhieuDatHang_ChiTiet_BaoCao x) => Math.Round(x.TONGSOLUONG, 0))),
									TONGTRONGLUONG = Convert.ToDecimal(s.Key.TRONGLUONG * s.Sum((v_ct_PhieuDatHang_ChiTiet_BaoCao x) => Math.Round(x.TONGSOLUONG, 0))),
									NAME_DVT_QD = s.Key.NAME_DVT_QD,
									TYLE_QD = s.Key.TYLE_QD
								}).ToList();
					}
				}
				if (LOAIPHIEUIN == "2")
				{
					mapPath = "~/Report/rptDanhSachPhieuGiaoHang_InGroupType.rpt";
					list = (from s in lstSp_Get_DanhSachPhieuGiaoHang_In
							group s by new { s.NAME_NCC, s.MA, s.NAME, s.NAME_DVT, s.NAME_DVT_QD, s.TYLE_QD_HH, s.ISKHUYENMAI } into s
							select new v_PhieuGioaHang_InTheoGroup
							{
								ID_KHACHHANG = "",
								NAME_GROUP = s.Key.NAME_NCC,
								MA_HANGHOA = s.Key.MA,
								NAME_HANGHOA = (s.Key.ISKHUYENMAI ? "(KM)" : "") + s.Key.NAME,
								NAME_DVT = s.Key.NAME_DVT,
								NAME_DVT_QD = s.Key.NAME_DVT_QD,
								TYLE_QD = s.Key.TYLE_QD_HH,
								TONGSOLUONG = Convert.ToDecimal(s.Sum((Sp_Get_DanhSachPhieuGiaoHang_In x) => Math.Round(x.TONGSOLUONG, 0)))
							}).ToList();
				}
				if (LOAIPHIEUIN == "3")
				{
					mapPath = "~/Report/rptDanhSachPhieuGiaoHang_InGroupBy.rpt";
					list = lstSp_Get_DanhSachPhieuGiaoHang_In.Select((Sp_Get_DanhSachPhieuGiaoHang_In s) => new v_PhieuGioaHang_InTheoGroup
					{
						ID_KHACHHANG = s.ID_KHACHHANG_NCC,
						NAME_GROUP = "Họ tên khách hàng: " + s.NAME_KHACHHANG_NCC + (string.IsNullOrEmpty(s.TEL_KHACHHANG_NCC) ? "" : (Environment.NewLine + "Điện thoại: ")) + s.TEL_KHACHHANG_NCC + (string.IsNullOrEmpty(s.DIACHI_KHACHHANG_NCC) ? "" : (Environment.NewLine + "Địa chỉ: ")) + s.DIACHI_KHACHHANG_NCC,
						MAPHIEU_GROUP = string.Join(",", (from x in lstSp_Get_DanhSachPhieuGiaoHang_In
														  where x.ID_KHACHHANG_NCC == s.ID_KHACHHANG_NCC
														  group x by new { x.MAPHIEU } into x
														  select x.Key.MAPHIEU).ToList()),
						MAPHIEUXUAT = (((from x in lstSp_Get_DanhSachPhieuGiaoHang_In
										 where x.ID_KHACHHANG_NCC == s.ID_KHACHHANG_NCC
										 group x by new { x.MAPHIEU }).Count() > 1) ? s.MAPHIEU : ""),
						NAME_HANGHOA = (s.ISKHUYENMAI ? "(KM)" : "") + s.NAME,
						NAME_DVT = s.NAME_DVT,
						SOLUONG = s.SOLUONG,
						DONGIA = s.DONGIA,
						CHIETKHAU = s.CHIETKHAU,
						TONGTIENGIAMGIA = ((s.TONGTIENGIAMGIA > 0.0) ? s.TONGTIENGIAMGIA : 0.0),
						THANHTIEN = s.THANHTIEN,
						THUESUAT = s.THUESUAT,
						TONGTIENVAT = ((s.TONGTIENGIAMGIA < 0.0) ? (s.TONGTIENVAT - s.TONGTIENGIAMGIA) : s.TONGTIENVAT),
						TONGCONG = s.TONGCONG,
						TONGSOLUONG = 0m,
						TYLE_QD = 1.0
					}).ToList();
					var list4 = (from s in list
								 group s by new { s.ID_KHACHHANG } into s
								 select s.Key).ToList();
					foreach (var item in list4)
					{
						SP_Parameter sp_Parameter = new SP_Parameter();
						sp_Parameter.LOC_ID = Utility.LOC_ID;
						sp_Parameter.ID_KHACHHANG = item.ID_KHACHHANG.ToString();
						sp_Parameter.ISTHEOTHOIGIAN = false;
						sp_Parameter.ISPHATSINHCONGNO = false;
						sp_Parameter.ISPHATSINHCONGNOTRONGKY = false;
						sp_Parameter.ISCONCONGNO = false;
						apiResponse = Utility.Get_ThongKeCongNoKhachHang<v_ThongKeCongNoKhachHang>(sp_Parameter);
						if (!apiResponse.Success)
						{
							base.TempData["TitleError"] = apiResponse.Message;
							return RedirectToAction("Index", "Notfound");
						}
						if (apiResponse.Data == null)
						{
							continue;
						}
						double num2 = list.Where((v_PhieuGioaHang_InTheoGroup e) => e.ID_KHACHHANG == sp_Parameter.ID_KHACHHANG).Sum((v_PhieuGioaHang_InTheoGroup e) => e.TONGCONG);
						v_ThongKeCongNoKhachHang v_ThongKeCongNoKhachHang2 = (apiResponse.Data as List<v_ThongKeCongNoKhachHang>).FirstOrDefault();
						if (v_ThongKeCongNoKhachHang2 == null || !(v_ThongKeCongNoKhachHang2.TONGTIENCONGNOCUOIKY - num2 > 0.0))
						{
							continue;
						}
						foreach (v_PhieuGioaHang_InTheoGroup item2 in list.Where((v_PhieuGioaHang_InTheoGroup e) => e.ID_KHACHHANG == sp_Parameter.ID_KHACHHANG))
						{
							item2.NAME_DVT_QD = "Nợ cũ: " + (v_ThongKeCongNoKhachHang2.TONGTIENCONGNOCUOIKY - num2).ToString("N0");
							item2.MA_HANGHOA = "Tổng tiền: " + v_ThongKeCongNoKhachHang2.TONGTIENCONGNOCUOIKY.ToString("N0");
						}
					}
				}
				DataTable dataSource = Utility.ToDataTable(list);
				report = Utility.GetFormulaFields(report, v_ct_PhieuGiaoHang2, mapPath);
				if (LOAIPHIEUIN == "1")
				{
					v_v_ct_PhieuGiaoHang v_v_ct_PhieuGiaoHang4 = new v_v_ct_PhieuGiaoHang();
					if (!string.IsNullOrEmpty(ID))
					{
						apiResponse = Utility.GetDetail<v_v_ct_PhieuGiaoHang>(Utility.LOC_ID + "/" + ID, "Delivery");
						if (!apiResponse.Success)
						{
							base.TempData["TitleError"] = apiResponse.Message;
							return RedirectToAction("Index", "Notfound");
						}
						if (apiResponse.Data != null)
						{
							v_v_ct_PhieuGiaoHang4 = apiResponse.Data as v_v_ct_PhieuGiaoHang;
						}
					}
					report.DataDefinition.FormulaFields["TIEUDE"].Text = "'BÁO CÁO GIAO HÀNG THEO NHÓM HÀNG'";
					report.DataDefinition.FormulaFields["TONGCONG"].Text = "'" + list.Sum((v_PhieuGioaHang_InTheoGroup s) => s.TONGCONG).ToString("N0") + "'";
					report.DataDefinition.FormulaFields["TONGTRONGLUONG"].Text = "'" + list.Sum((v_PhieuGioaHang_InTheoGroup s) => s.TONGTRONGLUONG / 1000m).ToString("N0") + "'";
					report.DataDefinition.FormulaFields["TONGSODONHANG"].Text = "'" + num.ToString("N0") + "'";
					report.DataDefinition.FormulaFields["MAPHIEU"].Text = "'" + text + "'";
					report.DataDefinition.FormulaFields["KHUVUC"].Text = "'Khu vực: " + text2 + "'";
				}
				if (LOAIPHIEUIN == "3")
				{
					v_v_ct_PhieuGiaoHang v_v_ct_PhieuGiaoHang5 = new v_v_ct_PhieuGiaoHang();
					if (!string.IsNullOrEmpty(ID))
					{
						apiResponse = Utility.GetDetail<v_v_ct_PhieuGiaoHang>(Utility.LOC_ID + "/" + ID, "Delivery");
						if (!apiResponse.Success)
						{
							base.TempData["TitleError"] = apiResponse.Message;
							return RedirectToAction("Index", "Notfound");
						}
						if (apiResponse.Data != null)
						{
							v_v_ct_PhieuGiaoHang5 = apiResponse.Data as v_v_ct_PhieuGiaoHang;
						}
					}
					string text3 = "Nhân viên giao hàng: ";
					foreach (v_ct_PhieuGiaoHang_NhanVienGiao item3 in v_v_ct_PhieuGiaoHang5.lstct_PhieuGiaoHang_NhanVienGiao)
					{
						text3 = text3 + item3.NAME_NHANVIEN + "; ";
					}
					report.DataDefinition.FormulaFields["THONGTINTHEM"].Text = "'" + text3 + "'";
					string text4 = Path.Combine(base.Server.MapPath("~/Images_Upload/Logo/"), "040937143939.png");
					report.DataDefinition.FormulaFields["QRCode1"].Text = "'" + text4 + "'";
					text4 = Path.Combine(base.Server.MapPath("~/Images_Upload/Logo/"), "117000052509.png");
					report.DataDefinition.FormulaFields["QRCode2"].Text = "'" + text4 + "'";
				}
				report.SetDataSource(dataSource);
				base.Response.Buffer = false;
				base.Response.ClearContent();
				base.Response.ClearHeaders();
				Stream stream = report.ExportToStream(ExportFormatType.PortableDocFormat);
				Utility.Report = report;
				apiResponse = new ApiResponse();
				apiResponse.Success = true;
				apiResponse.NAME = Utility.GetTitleFrom("Delivery") + " - " + v_ct_PhieuGiaoHang2.MAPHIEU;
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

		public ActionResult GetImageDelivery(string ID = "", string ID_PHIEUXUAT = "")
		{
			ApiResponse apiResponse = new ApiResponse();
			try
			{
				if (Utility.KiemTra())
				{
					return RedirectToAction("Index", "Admin");
				}
				List<v_ct_PhieuGiaoHang_HinhAnh> list = new List<v_ct_PhieuGiaoHang_HinhAnh>();
				if (!string.IsNullOrEmpty(ID) || !string.IsNullOrEmpty(ID_PHIEUXUAT))
				{
					apiResponse = Utility.GetDetail<List<v_ct_PhieuGiaoHang_HinhAnh>>(Utility.LOC_ID + "/" + ((!string.IsNullOrEmpty(ID_PHIEUXUAT)) ? ID_PHIEUXUAT : ID), "Delivery_Image");
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
						list = apiResponse.Data as List<v_ct_PhieuGiaoHang_HinhAnh>;
					}
					foreach (v_ct_PhieuGiaoHang_HinhAnh item in list)
					{
						string[] array = item.URL_IMAGE.Split('/');
						string authority = base.Request.Url.Authority;
						string text = "";
						text = ((!base.Request.Url.AbsoluteUri.StartsWith("https")) ? ("http://" + authority + item.URL_IMAGE) : ("https://" + authority + item.URL_IMAGE));
						ApiResponse apiResponse2 = apiResponse;
						apiResponse2.CONTENT = apiResponse2.CONTENT + "<div class='col-xs-6 col-sm-4 col-md-3 image' id='" + item.ID + "'>";
						apiResponse.CONTENT += "<div class='thmb'>";
						apiResponse.CONTENT += "<div class='ckbox ckbox-default'>";
						apiResponse.CONTENT += "</div>";
						apiResponse.CONTENT += "<div class='btn-group fm-group'>";
						apiResponse.CONTENT += "</div><!-- btn-group -->";
						apiResponse.CONTENT += "<div class='thmb-prev'>";
						ApiResponse apiResponse3 = apiResponse;
						apiResponse3.CONTENT = apiResponse3.CONTENT + "<a href='" + text + "' data-rel='prettyPhoto'>";
						ApiResponse apiResponse4 = apiResponse;
						apiResponse4.CONTENT = apiResponse4.CONTENT + "<img src='" + text + "' class='img-responsive' alt='' />";
						apiResponse.CONTENT += "</a>";
						apiResponse.CONTENT += "</div>";
						ApiResponse apiResponse5 = apiResponse;
						apiResponse5.CONTENT = apiResponse5.CONTENT + "<h5 class='fm-title'><a href='#'>" + item.NAME_NGUOITAO + "</a></h5> ";
						ApiResponse apiResponse6 = apiResponse;
						apiResponse6.CONTENT = apiResponse6.CONTENT + "<small class=\"text-muted\"><a href=\"#\" style=\"color:red\" onclick=\"myFunctionPopupImage('Delivery_Image','" + item.ID + "')\"><i class=\"glyphicon glyphicon-trash\" style=\"margin-right:5px\" ></i>" + Utility.Xoa + "\r\n</a></small>";
						ApiResponse apiResponse7 = apiResponse;
						apiResponse7.CONTENT = apiResponse7.CONTENT + "<small class='text-muted'>" + item.NGAYTAO.ToString("dd/MM/yyyy HH:mm") + "</small>";
						apiResponse.CONTENT += "</div><!-- thmb -->";
						apiResponse.CONTENT += "</div><!-- col-xs-6 -->";
					}
					apiResponse.ID = ID;
					apiResponse.ID_PHIEUXUAT = ID_PHIEUXUAT;
				}
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
	}
}
