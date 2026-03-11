using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using DatabaseTHP;
using DatabaseTHP.Class;
using DatabaseTHP.StoredProcedure.Parameter;
using MVC_QuanLyTHP.Class;
using MVC_QuanLyTHP.Models;
using Newtonsoft.Json;
using PagedList;

namespace MVC_QuanLyTHP.Controllers
{

	public class ProductController : Controller
	{
		public ActionResult Index(int Page = 1, string SearchString = "", string ShowSearchValue = "")
		{
			try
			{
				if (Utility.KiemTra())
				{
					return RedirectToAction("Index", "Admin");
				}
				if (!Utility.KiemTraQuyen("Product", "View"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				ShowSearchValue = Utility.GetShowSearchValue<dm_HangHoa>(ShowSearchValue);
				ApiResponse listData = Utility.GetListData<v_dm_HangHoa>("Product", ShowSearchValue, SearchString, Utility.LOC_ID);
				if (!listData.Success)
				{
					base.TempData["TitleError"] = listData.Message;
					return RedirectToAction("Index", "Notfound");
				}
				IPagedList<v_dm_HangHoa> iPagedList = (listData.Data as List<v_dm_HangHoa>).ToPagedList(Page, Utility.GetPageSize());
				v_v_dm_HangHoa v_v_dm_HangHoa2 = new v_v_dm_HangHoa();
				v_v_dm_HangHoa2.IPagedList = iPagedList;
				v_v_dm_HangHoa2.lstdm_DonViTinh = new List<v_dm_DonViTinh>();
				v_v_dm_HangHoa2.lstdm_DonViTinh = Utility.GetListData<v_dm_DonViTinh>("Unit", "", "", Utility.LOC_ID).Data as List<v_dm_DonViTinh>;
				v_v_dm_HangHoa2.lstdm_DonViTinh_QD = new List<v_dm_DonViTinh>();
				v_v_dm_HangHoa2.lstdm_DonViTinh_QD = Utility.GetListData<v_dm_DonViTinh>("Unit", "", "", Utility.LOC_ID).Data as List<v_dm_DonViTinh>;
				v_v_dm_HangHoa2.lstdm_NhaCungCap = new List<v_dm_NhaCungCap>();
				v_v_dm_HangHoa2.lstdm_NhaCungCap = Utility.GetListData<v_dm_NhaCungCap>("Provider", "", "", Utility.LOC_ID).Data as List<v_dm_NhaCungCap>;
				v_v_dm_HangHoa2.lstdm_NhomHangHoa = new List<v_dm_NhomHangHoa>();
				v_v_dm_HangHoa2.lstdm_NhomHangHoa = Utility.GetListData<v_dm_NhomHangHoa>("GroupProduct", "", "", Utility.LOC_ID).Data as List<v_dm_NhomHangHoa>;
				v_v_dm_HangHoa2.lstdm_ThueSuat = new List<v_dm_ThueSuat>();
				v_v_dm_HangHoa2.lstdm_ThueSuat = Utility.GetListData<v_dm_ThueSuat>("Tax", "", "", Utility.LOC_ID).Data as List<v_dm_ThueSuat>;
				base.ViewBag.searchValue = SearchString;
				base.ViewBag.showsearchValue = ShowSearchValue;
				base.ViewBag.PermissionEdit = Utility.KiemTraQuyen("Product", "Edit");
				base.ViewBag.PermissionDelete = Utility.KiemTraQuyen("Product", "Delete");
				base.ViewBag.PermissionCreate = Utility.KiemTraQuyen("Product", "Create");
				return View(v_v_dm_HangHoa2);
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
				if (!Utility.KiemTraQuyen("Product", "Create"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				v_v_dm_HangHoa v_v_dm_HangHoa2 = new v_v_dm_HangHoa();
				v_v_dm_HangHoa2.LOC_ID = Utility.LOC_ID;
				v_v_dm_HangHoa2.ID_NGUOITAO = base.Session["idUser"].ToString();
				v_v_dm_HangHoa2.THOIGIANTHEM = Utility.CurrentTime;
				v_v_dm_HangHoa2.ID = Guid.NewGuid().ToString();
				v_v_dm_HangHoa2.lstdm_DonViTinh = new List<v_dm_DonViTinh>();
				v_v_dm_HangHoa2.lstdm_DonViTinh = Utility.GetListData<v_dm_DonViTinh>("Unit", "", "", Utility.LOC_ID).Data as List<v_dm_DonViTinh>;
				v_v_dm_HangHoa2.lstdm_DonViTinh_QD = new List<v_dm_DonViTinh>();
				v_v_dm_HangHoa2.lstdm_DonViTinh_QD = Utility.GetListData<v_dm_DonViTinh>("Unit", "", "", Utility.LOC_ID).Data as List<v_dm_DonViTinh>;
				v_v_dm_HangHoa2.lstdm_NhaCungCap = new List<v_dm_NhaCungCap>();
				v_v_dm_HangHoa2.lstdm_NhaCungCap = Utility.GetListData<v_dm_NhaCungCap>("Provider", "", "", Utility.LOC_ID).Data as List<v_dm_NhaCungCap>;
				v_v_dm_HangHoa2.lstdm_NhomHangHoa = new List<v_dm_NhomHangHoa>();
				v_v_dm_HangHoa2.lstdm_NhomHangHoa = Utility.GetListData<v_dm_NhomHangHoa>("GroupProduct", "", "", Utility.LOC_ID).Data as List<v_dm_NhomHangHoa>;
				v_v_dm_HangHoa2.lstdm_ThueSuat = new List<v_dm_ThueSuat>();
				v_v_dm_HangHoa2.lstdm_ThueSuat = Utility.GetListData<v_dm_ThueSuat>("Tax", "", "", Utility.LOC_ID).Data as List<v_dm_ThueSuat>;
				return View(v_v_dm_HangHoa2);
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
		public ActionResult Create([Bind(Include = "ISKHONGHIENTHITONKHO,LOC_ID,ID,BARCODE,MA,NAME,PICTURE,GIA01,GIA02,GIA03,GIA01_QD,GIA02_QD,ID_NHOMHANGHOA,ISACTIVE,LOAIHANGHOA,ISCOMBO,ID_DVT,STATUS_QD,ID_DVT_QD,TYLE_QD,TRONGLUONG,STATUS_HIENTHI,ID_NCC,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO,BAOGOMTHUESUAT,ID_THUESUAT,GIA03_QD,ISKHUYENMAI,ISXUATHOADON,VAT")] v_v_dm_HangHoa dm_HangHoa)
		{
			try
			{
				if (Utility.KiemTra())
				{
					return RedirectToAction("Index", "Admin");
				}
				if (!Utility.KiemTraQuyen("Product", "Create"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				if (base.ModelState.IsValid)
				{
					dm_HangHoa.LOC_ID = Utility.LOC_ID;
					dm_HangHoa.ID_NGUOITAO = base.Session["idUser"].ToString();
					dm_HangHoa.THOIGIANTHEM = Utility.CurrentTime;
					if (base.Request.Files["MaHinh"] != null)
					{
						string fileName = base.Request.Files["MaHinh"].FileName;
						if (fileName != "")
						{
							string text = Guid.NewGuid().ToString() + fileName.Split('.')[1];
							string text2 = Path.Combine(base.Server.MapPath("~/Images_Upload/Product/"), text);
							base.Request.Files["MaHinh"].SaveAs(text2);
							dm_HangHoa.PICTURE = text;
							byte[] inArray = System.IO.File.ReadAllBytes(text2);
							string fILEBASE = Convert.ToBase64String(inArray);
							dm_HangHoa.FILEBASE64 = fILEBASE;
						}
					}
					ApiResponse apiResponse = Utility.Create((dm_HangHoa)dm_HangHoa, "Product");
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
				return View(dm_HangHoa);
			}
			catch (Exception ex)
			{
				Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
				base.TempData["TitleError"] = "Đã có lỗi phát sinh trong phiên làm việc!";
				base.TempData["DetailError"] = ex.Message;
				return RedirectToAction("Index", "Notfound");
			}
		}

		public ActionResult Edit(string id = "", int type = 2)
		{
			ApiResponse apiResponse = new ApiResponse();
			try
			{
				base.Session["IntWidth"] = type;
				if (Utility.KiemTra())
				{
					return RedirectToAction("Index", "Admin");
				}
				if (!Utility.KiemTraQuyen("Product", "Edit"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				v_v_dm_HangHoa v_v_dm_HangHoa2 = new v_v_dm_HangHoa();
				if (!string.IsNullOrEmpty(id))
				{
					apiResponse = Utility.GetDetail<v_v_dm_HangHoa>(Utility.LOC_ID + "/" + id, "Product");
					if (!apiResponse.Success)
					{
						base.TempData["TitleError"] = apiResponse.Message;
						return RedirectToAction("Index", "Notfound");
					}
					if (apiResponse.Data != null)
					{
						v_v_dm_HangHoa2 = apiResponse.Data as v_v_dm_HangHoa;
					}
				}
				v_v_dm_HangHoa2.lstdm_DonViTinh = new List<v_dm_DonViTinh>();
				v_v_dm_HangHoa2.lstdm_DonViTinh = Utility.GetListData<v_dm_DonViTinh>("Unit", "", "", Utility.LOC_ID).Data as List<v_dm_DonViTinh>;
				v_v_dm_HangHoa2.lstdm_DonViTinh_QD = new List<v_dm_DonViTinh>();
				v_v_dm_HangHoa2.lstdm_DonViTinh_QD = Utility.GetListData<v_dm_DonViTinh>("Unit", "", "", Utility.LOC_ID).Data as List<v_dm_DonViTinh>;
				v_v_dm_HangHoa2.lstdm_NhaCungCap = new List<v_dm_NhaCungCap>();
				v_v_dm_HangHoa2.lstdm_NhaCungCap = Utility.GetListData<v_dm_NhaCungCap>("Provider", "", "", Utility.LOC_ID).Data as List<v_dm_NhaCungCap>;
				v_v_dm_HangHoa2.lstdm_NhomHangHoa = new List<v_dm_NhomHangHoa>();
				v_v_dm_HangHoa2.lstdm_NhomHangHoa = Utility.GetListData<v_dm_NhomHangHoa>("GroupProduct", "", "", Utility.LOC_ID).Data as List<v_dm_NhomHangHoa>;
				v_v_dm_HangHoa2.lstdm_ThueSuat = new List<v_dm_ThueSuat>();
				v_v_dm_HangHoa2.lstdm_ThueSuat = Utility.GetListData<v_dm_ThueSuat>("Tax", "", "", Utility.LOC_ID).Data as List<v_dm_ThueSuat>;
				return View(v_v_dm_HangHoa2);
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
		public ActionResult Edit([Bind(Include = "ISKHONGHIENTHITONKHO,LOC_ID,ID,BARCODE,MA,NAME,PICTURE,GIA01,GIA02,GIA03,GIA01_QD,GIA02_QD,ID_NHOMHANGHOA,ISACTIVE,LOAIHANGHOA,ISCOMBO,ID_DVT,STATUS_QD,ID_DVT_QD,TYLE_QD,TRONGLUONG,STATUS_HIENTHI,ID_NCC,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO,BAOGOMTHUESUAT,ID_THUESUAT,GIA03_QD,ISKHUYENMAI,ISXUATHOADON,VAT")] v_v_dm_HangHoa dm_HangHoa)
		{
			try
			{
				if (Utility.KiemTra())
				{
					return RedirectToAction("Index", "Admin");
				}
				if (!Utility.KiemTraQuyen("Product", "Edit"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				if (base.ModelState.IsValid)
				{
					dm_HangHoa.LOC_ID = Utility.LOC_ID;
					dm_HangHoa.ID_NGUOISUA = base.Session["idUser"].ToString();
					dm_HangHoa.THOIGIANSUA = Utility.CurrentTime;
					ApiResponse apiResponse = Utility.Edit(Utility.LOC_ID + "/" + dm_HangHoa.MA, (v_dm_HangHoa)dm_HangHoa, "Product");
					if (apiResponse.Success)
					{
						return RedirectToAction("Index");
					}
					base.ModelState.AddModelError(string.Empty, apiResponse.Message);
				}
				return View(dm_HangHoa);
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
				if (!Utility.KiemTraQuyen("Product", "Delete"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				ApiResponse apiResponse = Utility.Delete<v_dm_HangHoa>(Utility.LOC_ID + "/" + id, "Product");
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
				if (!Utility.KiemTraQuyen("Product", "Create"))
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
				v_v_dm_HangHoa v_v_dm_HangHoa2 = new v_v_dm_HangHoa();
				apiResponse.Success = true;
				v_v_dm_HangHoa2.LOC_ID = Utility.LOC_ID;
				v_v_dm_HangHoa2.ID = Guid.NewGuid().ToString();
				v_v_dm_HangHoa2.lstdm_DonViTinh = new List<v_dm_DonViTinh>();
				v_v_dm_HangHoa2.lstdm_DonViTinh = Utility.GetListData<v_dm_DonViTinh>("Unit", "", "", Utility.LOC_ID).Data as List<v_dm_DonViTinh>;
				v_v_dm_HangHoa2.lstdm_DonViTinh_QD = new List<v_dm_DonViTinh>();
				v_v_dm_HangHoa2.lstdm_DonViTinh_QD = Utility.GetListData<v_dm_DonViTinh>("Unit", "", "", Utility.LOC_ID).Data as List<v_dm_DonViTinh>;
				v_v_dm_HangHoa2.lstdm_NhaCungCap = new List<v_dm_NhaCungCap>();
				v_v_dm_HangHoa2.lstdm_NhaCungCap = Utility.GetListData<v_dm_NhaCungCap>("Provider", "", "", Utility.LOC_ID).Data as List<v_dm_NhaCungCap>;
				v_v_dm_HangHoa2.lstdm_NhomHangHoa = new List<v_dm_NhomHangHoa>();
				v_v_dm_HangHoa2.lstdm_NhomHangHoa = Utility.GetListData<v_dm_NhomHangHoa>("GroupProduct", "", "", Utility.LOC_ID).Data as List<v_dm_NhomHangHoa>;
				v_v_dm_HangHoa2.lstdm_ThueSuat = new List<v_dm_ThueSuat>();
				v_v_dm_HangHoa2.lstdm_ThueSuat = Utility.GetListData<v_dm_ThueSuat>("Tax", "", "", Utility.LOC_ID).Data as List<v_dm_ThueSuat>;
				v_v_dm_HangHoa2.BAOGOMTHUESUAT = true;
				apiResponse.Detail = Utility.ConvertobjectTo(v_v_dm_HangHoa2);
				apiResponse.PathProduct = "/Images_Upload/Product/";
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
		public ActionResult CreatePopup([Bind(Include = "ISKHONGHIENTHITONKHO,LOC_ID,ID,BARCODE,MA,NAME,PICTURE,GIA01,GIA02,GIA03,GIA01_QD,GIA02_QD,ID_NHOMHANGHOA,ISACTIVE,LOAIHANGHOA,ISCOMBO,ID_DVT,STATUS_QD,ID_DVT_QD,TYLE_QD,TRONGLUONG,STATUS_HIENTHI,ID_NCC,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO,BAOGOMTHUESUAT,ID_THUESUAT,GIA03_QD,GIAMUA,GIAMUA_QD,ISKHUYENMAI,ISXUATHOADON,VAT")] v_dm_HangHoa dm_HangHoa)
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
				if (!Utility.KiemTraQuyen("Product", "Create"))
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
				IEnumerable<string> enumerable = base.Request.Form.AllKeys.Where((string e) => e.StartsWith("txtQuantity|"));
				if (dm_HangHoa.LOAIHANGHOA == 1.ToString())
				{
					if (enumerable == null || enumerable.Count() == 0)
					{
						base.ModelState.AddModelError("lstdm_HangHoa_Combo", "Thêm sản phẩm trong combo.");
					}
				}
				else if (dm_HangHoa.STATUS_QD)
				{
					if (dm_HangHoa.TYLE_QD <= 0.0)
					{
						base.ModelState.AddModelError("TYLE_QD", "The TYLE_QD field is required.");
					}
					if (string.IsNullOrEmpty(dm_HangHoa.ID_DVT_QD))
					{
						base.ModelState.AddModelError("ID_DVT_QD", "The ID_DVT_QD field is required.");
					}
				}
				if (base.ModelState.IsValid)
				{
					if (dm_HangHoa.LOAIHANGHOA == 1.ToString())
					{
						dm_HangHoa.lstdm_HangHoa_Combo = new List<v_dm_HangHoa_Combo>();
						if (enumerable != null)
						{
							foreach (string item in enumerable)
							{
								string[] array = item.ToString().Split('|');
								string[] values = base.HttpContext.Request.Params.GetValues(item.ToString());
								if (array != null && array.Length > 3)
								{
									v_dm_HangHoa_Combo v_dm_HangHoa_Combo2 = new v_dm_HangHoa_Combo();
									v_dm_HangHoa_Combo2.ID_HANGHOA = array[1];
									v_dm_HangHoa_Combo2.ID_DVT = array[2];
									v_dm_HangHoa_Combo2.TYLE_QD = Utility.ConvertStringToDouble(array[3]);
									v_dm_HangHoa_Combo2.QTY = Utility.ConvertStringToDouble(values[0]);
									dm_HangHoa.lstdm_HangHoa_Combo.Add(v_dm_HangHoa_Combo2);
								}
							}
						}
					}
					dm_HangHoa.LOC_ID = Utility.LOC_ID;
					dm_HangHoa.ID_NGUOITAO = base.Session["idUser"].ToString();
					dm_HangHoa.THOIGIANTHEM = Utility.CurrentTime;
					if (base.Request.Files["MaHinh"] != null)
					{
						string fileName = base.Request.Files["MaHinh"].FileName;
						if (fileName != "")
						{
							string text = Guid.NewGuid().ToString() + "." + fileName.Split('.')[1];
							string text2 = Path.Combine(base.Server.MapPath("~/Images_Upload/Product/"), text);
							if (!Directory.Exists(base.Server.MapPath("~/Images_Upload/Product/")))
							{
								Directory.CreateDirectory(base.Server.MapPath("~/Images_Upload/Product/"));
							}
							base.Request.Files["MaHinh"].SaveAs(text2);
							dm_HangHoa.PICTURE = text;
							byte[] inArray = System.IO.File.ReadAllBytes(text2);
							string fILEBASE = Convert.ToBase64String(inArray);
							dm_HangHoa.FILEBASE64 = fILEBASE;
						}
					}
					apiResponse = Utility.Create(dm_HangHoa, "Product");
					if (apiResponse.Success)
					{
						apiResponse.NewID = Guid.NewGuid().ToString();
						if (apiResponse.Data != null)
						{
							dm_HangHoa = JsonConvert.DeserializeObject<v_v_dm_HangHoa>(apiResponse.Data.ToString());
						}
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
					apiResponse.Data = Utility.GetModelState(base.ModelState, "Product");
				}
				apiResponse.ID = dm_HangHoa.ID;
				apiResponse.Detail = Utility.ConvertobjectToView(dm_HangHoa);
				apiResponse.PathProduct = "/Images_Upload/Product/";
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
				if (!Utility.KiemTraQuyen("Product", "Edit"))
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
				v_v_dm_HangHoa v_v_dm_HangHoa2 = new v_v_dm_HangHoa();
				if (!string.IsNullOrEmpty(id))
				{
					apiResponse = Utility.GetDetail<v_v_dm_HangHoa>(Utility.LOC_ID + "/" + id, "Product");
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
				}
				v_v_dm_HangHoa2.lstdm_DonViTinh = new List<v_dm_DonViTinh>();
				v_v_dm_HangHoa2.lstdm_DonViTinh = Utility.GetListData<v_dm_DonViTinh>("Unit", "", "", Utility.LOC_ID).Data as List<v_dm_DonViTinh>;
				v_v_dm_HangHoa2.lstdm_DonViTinh_QD = new List<v_dm_DonViTinh>();
				v_v_dm_HangHoa2.lstdm_DonViTinh_QD = Utility.GetListData<v_dm_DonViTinh>("Unit", "", "", Utility.LOC_ID).Data as List<v_dm_DonViTinh>;
				v_v_dm_HangHoa2.lstdm_NhaCungCap = new List<v_dm_NhaCungCap>();
				v_v_dm_HangHoa2.lstdm_NhaCungCap = Utility.GetListData<v_dm_NhaCungCap>("Provider", "", "", Utility.LOC_ID).Data as List<v_dm_NhaCungCap>;
				v_v_dm_HangHoa2.lstdm_NhomHangHoa = new List<v_dm_NhomHangHoa>();
				v_v_dm_HangHoa2.lstdm_NhomHangHoa = Utility.GetListData<v_dm_NhomHangHoa>("GroupProduct", "", "", Utility.LOC_ID).Data as List<v_dm_NhomHangHoa>;
				v_v_dm_HangHoa2.lstdm_ThueSuat = new List<v_dm_ThueSuat>();
				v_v_dm_HangHoa2.lstdm_ThueSuat = Utility.GetListData<v_dm_ThueSuat>("Tax", "", "", Utility.LOC_ID).Data as List<v_dm_ThueSuat>;
				v_v_dm_HangHoa2.lstdm_HangHoa_Combo = new List<v_dm_HangHoa_Combo>();
				v_v_dm_HangHoa2.lstdm_HangHoa_Combo = Utility.GetListData<v_dm_HangHoa_Combo>("Product_Combo", "", "", Utility.LOC_ID + "/" + v_v_dm_HangHoa2.ID).Data as List<v_dm_HangHoa_Combo>;
				base.Session["lstProductCombo"] = v_v_dm_HangHoa2.lstdm_HangHoa_Combo;
				apiResponse.Success = true;
				apiResponse.ProductCombo = Utility.GetProductCombo();
				List<ValueEdit> list = Utility.ConvertobjectTo((v_dm_HangHoa)v_v_dm_HangHoa2, "yyyy-MM-dd HH:mm:ss");
				list.Add(new ValueEdit
				{
					Key = "tbodyTempItemComboEdit",
					Value = apiResponse.ProductCombo
				});
				apiResponse.Detail = list;
				apiResponse.PathProduct = "/Images_Upload/Product/";
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
		public ActionResult EditPopup([Bind(Include = "ISKHONGHIENTHITONKHO,LOC_ID,ID,BARCODE,MA,NAME,PICTURE,GIA01,GIA02,GIA03,GIA01_QD,GIA02_QD,GIA03_QD,ID_NHOMHANGHOA,ISACTIVE,LOAIHANGHOA,ISCOMBO,ID_DVT,STATUS_QD,ID_DVT_QD,TYLE_QD,TRONGLUONG,STATUS_HIENTHI,ID_NCC,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO,BAOGOMTHUESUAT,ID_THUESUAT,GIAMUA,GIAMUA_QD,ISKHUYENMAI,ISXUATHOADON,VAT")] v_v_dm_HangHoa dm_HangHoa)
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
				if (!Utility.KiemTraQuyen("Product", "Edit"))
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
				IEnumerable<string> enumerable = base.Request.Form.AllKeys.Where((string e) => e.StartsWith("txtQuantity|"));
				if (dm_HangHoa.LOAIHANGHOA == 1.ToString())
				{
					if (enumerable == null || enumerable.Count() == 0)
					{
						base.ModelState.AddModelError("lstdm_HangHoa_Combo", "Thêm hàng hóa trong combo.");
					}
				}
				else if (dm_HangHoa.STATUS_QD)
				{
					if (dm_HangHoa.TYLE_QD <= 0.0)
					{
						base.ModelState.AddModelError("TYLE_QD", "The TYLE_QD field is required.");
					}
					if (string.IsNullOrEmpty(dm_HangHoa.ID_DVT_QD))
					{
						base.ModelState.AddModelError("ID_DVT_QD", "The ID_DVT_QD field is required.");
					}
				}
				if (base.ModelState.IsValid)
				{
					if (dm_HangHoa.LOAIHANGHOA == 1.ToString())
					{
						dm_HangHoa.lstdm_HangHoa_Combo = new List<v_dm_HangHoa_Combo>();
						if (enumerable != null)
						{
							foreach (string item in enumerable)
							{
								string[] array = item.ToString().Split('|');
								string[] values = base.HttpContext.Request.Params.GetValues(item.ToString());
								if (array != null && array.Length > 3)
								{
									v_dm_HangHoa_Combo v_dm_HangHoa_Combo2 = new v_dm_HangHoa_Combo();
									v_dm_HangHoa_Combo2.ID_HANGHOA = array[1];
									v_dm_HangHoa_Combo2.ID_DVT = array[2];
									v_dm_HangHoa_Combo2.TYLE_QD = Utility.ConvertStringToDouble(array[3]);
									v_dm_HangHoa_Combo2.QTY = Utility.ConvertStringToDouble(values[0]);
									v_dm_HangHoa_Combo2.THOIGIANTHEM = Utility.CurrentTime;
									v_dm_HangHoa_Combo2.ID_NGUOITAO = base.Session["idUser"].ToString();
									dm_HangHoa.lstdm_HangHoa_Combo.Add(v_dm_HangHoa_Combo2);
								}
							}
						}
					}
					dm_HangHoa.LOC_ID = Utility.LOC_ID;
					dm_HangHoa.ID_NGUOISUA = base.Session["idUser"].ToString();
					dm_HangHoa.THOIGIANSUA = Utility.CurrentTime;
					if (base.Request.Files["MaHinh"] != null)
					{
						string fileName = base.Request.Files["MaHinh"].FileName;
						if (fileName != "")
						{
							string text = dm_HangHoa.ID.Trim() + "." + fileName.Split('.')[1];
							string text2 = Path.Combine(base.Server.MapPath("~/Images_Upload/Product/"), text);
							if (!Directory.Exists(base.Server.MapPath("~/Images_Upload/Product/")))
							{
								Directory.CreateDirectory(base.Server.MapPath("~/Images_Upload/Product/"));
							}
							if (System.IO.File.Exists(text2))
							{
								System.IO.File.Delete(text2);
							}
							base.Request.Files["MaHinh"].SaveAs(text2);
							dm_HangHoa.PICTURE = text;
							byte[] inArray = System.IO.File.ReadAllBytes(text2);
							string fILEBASE = Convert.ToBase64String(inArray);
							dm_HangHoa.FILEBASE64 = fILEBASE;
							dm_HangHoa.FILENEW = true;
						}
					}
					apiResponse = Utility.Edit(Utility.LOC_ID + "/" + dm_HangHoa.MA, (v_dm_HangHoa)dm_HangHoa, "Product");
					if (apiResponse.Success)
					{
						apiResponse.ID = dm_HangHoa.ID;
						if (apiResponse.Data != null)
						{
							dm_HangHoa = JsonConvert.DeserializeObject<v_v_dm_HangHoa>(apiResponse.Data.ToString());
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
					apiResponse.Data = Utility.GetModelState(base.ModelState, "Product");
				}
				apiResponse.Detail = Utility.ConvertobjectToView((v_dm_HangHoa)dm_HangHoa, "dd/MM/yyyy");
				apiResponse.PathProduct = "/Images_Upload/Product/";
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
				if (!Utility.KiemTraQuyen("Product", "Delete"))
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
				apiResponse = Utility.Delete<v_dm_HangHoa>(Utility.LOC_ID + "/" + id, "Product");
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
		public ActionResult LoadProduct(string ID, string Type)
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
				v_v_dm_HangHoa v_v_dm_HangHoa2 = new v_v_dm_HangHoa();
				apiResponse = Utility.GetDetail<v_v_dm_HangHoa>(Utility.LOC_ID + "/" + ID, "Product");
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
				if (apiResponse.Data != null)
				{
					v_v_dm_HangHoa2 = apiResponse.Data as v_v_dm_HangHoa;
				}
				if (v_v_dm_HangHoa2 != null)
				{
					switch (Type)
					{
						case "Input":
							v_v_dm_HangHoa2.GIA = v_v_dm_HangHoa2.GIAMUA;
							v_v_dm_HangHoa2.GIA_QD = v_v_dm_HangHoa2.GIAMUA_QD;
							break;
						case "InputOther":
							v_v_dm_HangHoa2.GIA = v_v_dm_HangHoa2.GIAMUA;
							v_v_dm_HangHoa2.GIA_QD = v_v_dm_HangHoa2.GIAMUA_QD;
							break;
						case "Output":
							v_v_dm_HangHoa2.GIA = v_v_dm_HangHoa2.GIA01;
							v_v_dm_HangHoa2.GIA_QD = v_v_dm_HangHoa2.GIA01_QD;
							break;
						case "OutputOther":
							v_v_dm_HangHoa2.GIA = v_v_dm_HangHoa2.GIA01;
							v_v_dm_HangHoa2.GIA_QD = v_v_dm_HangHoa2.GIA01_QD;
							break;
						case "WarehouseTransfer":
							v_v_dm_HangHoa2.GIA = 0.0;
							v_v_dm_HangHoa2.GIA_QD = 0.0;
							break;
						case "Product":
							v_v_dm_HangHoa2.GIA = v_v_dm_HangHoa2.GIA01;
							v_v_dm_HangHoa2.GIA_QD = v_v_dm_HangHoa2.GIA01_QD;
							break;
					}
					if (!string.IsNullOrEmpty(v_v_dm_HangHoa2.ID_THUESUAT))
					{
						ApiResponse detail = Utility.GetDetail<v_v_dm_ThueSuat>(Utility.LOC_ID + "/" + v_v_dm_HangHoa2.ID_THUESUAT, "Tax");
						if (detail.Data != null && detail.Data is v_v_dm_ThueSuat v_v_dm_ThueSuat2)
						{
							v_v_dm_HangHoa2.THANHTIEN = v_v_dm_HangHoa2.GIA * 1.0;
							v_v_dm_HangHoa2.THUESUAT = v_v_dm_ThueSuat2.THUESUAT;
							v_v_dm_HangHoa2.TONGTIENVAT = v_v_dm_HangHoa2.THANHTIEN * v_v_dm_HangHoa2.THUESUAT / 100.0;
							v_v_dm_HangHoa2.TONGCONG = v_v_dm_HangHoa2.THANHTIEN + v_v_dm_HangHoa2.TONGTIENVAT;
						}
					}
				}
				apiResponse.Detail = Utility.ConvertobjectTo((v_dm_HangHoa)v_v_dm_HangHoa2, "yyyy-MM-dd HH:mm:ss");
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
		public ActionResult LoadProductKho(string ID, string Type, string ID_KHO)
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
				v_dm_HangHoa v_dm_HangHoa2 = new v_dm_HangHoa();
				apiResponse = Utility.Get_DanhSachSanPhamKho<v_dm_HangHoa>(ID_KHO, bolTonKho: false, ID);
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
				if (apiResponse.Data != null)
				{
					v_dm_HangHoa2 = (apiResponse.Data as List<v_dm_HangHoa>).FirstOrDefault();
				}
				if (v_dm_HangHoa2 != null)
				{
					switch (Type)
					{
						case "Input":
							v_dm_HangHoa2.GIA = v_dm_HangHoa2.GIAMUA;
							v_dm_HangHoa2.GIA_QD = v_dm_HangHoa2.GIAMUA_QD;
							break;
						case "InputOther":
							v_dm_HangHoa2.GIA = v_dm_HangHoa2.GIAMUA;
							v_dm_HangHoa2.GIA_QD = v_dm_HangHoa2.GIAMUA_QD;
							break;
						case "Output":
							v_dm_HangHoa2.GIA = v_dm_HangHoa2.GIA01;
							v_dm_HangHoa2.GIA_QD = v_dm_HangHoa2.GIA01_QD;
							break;
						case "OutputOther":
							v_dm_HangHoa2.GIA = v_dm_HangHoa2.GIA01;
							v_dm_HangHoa2.GIA_QD = v_dm_HangHoa2.GIA01_QD;
							break;
						case "WarehouseTransfer":
							v_dm_HangHoa2.GIA = 0.0;
							v_dm_HangHoa2.GIA_QD = 0.0;
							break;
					}
					if (!string.IsNullOrEmpty(v_dm_HangHoa2.ID_THUESUAT))
					{
						ApiResponse detail = Utility.GetDetail<v_v_dm_ThueSuat>(Utility.LOC_ID + "/" + v_dm_HangHoa2.ID_THUESUAT, "Tax");
						if (detail.Data != null)
						{
							if (detail.Data is v_v_dm_ThueSuat v_v_dm_ThueSuat2)
							{
								v_dm_HangHoa2.THANHTIEN = v_dm_HangHoa2.GIA * 1.0;
								v_dm_HangHoa2.THUESUAT = v_v_dm_ThueSuat2.THUESUAT;
								v_dm_HangHoa2.TONGTIENVAT = v_dm_HangHoa2.THANHTIEN * v_dm_HangHoa2.THUESUAT / 100.0;
								v_dm_HangHoa2.TONGCONG = v_dm_HangHoa2.THANHTIEN + v_dm_HangHoa2.TONGTIENVAT;
							}
						}
						else
						{
							v_dm_HangHoa2.THANHTIEN = v_dm_HangHoa2.GIA * 1.0;
							v_dm_HangHoa2.THUESUAT = 0.0;
							v_dm_HangHoa2.TONGTIENVAT = v_dm_HangHoa2.THANHTIEN * v_dm_HangHoa2.THUESUAT / 100.0;
							v_dm_HangHoa2.TONGCONG = v_dm_HangHoa2.THANHTIEN + v_dm_HangHoa2.TONGTIENVAT;
						}
					}
				}
				apiResponse.Detail = Utility.ConvertobjectTo(v_dm_HangHoa2);
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
		public ActionResult AddProductInputOutput([Bind(Include = "ID_HANGHOA,ID_HANGHOAKHO,DONGIA,ID_DVT,SOLUONG,CHIETKHAU,TONGTIENGIAMGIA,THANHTIEN,THUESUAT,ID_THUESUAT,TONGTIENVAT,TONGCONG,ID_KHO")] Product_Detail Product_Detail)
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
				if (base.ModelState.IsValid)
				{
					v_dm_HangHoa v_dm_HangHoa2 = new v_dm_HangHoa();
					apiResponse = Utility.Get_DanhSachSanPhamKho<v_dm_HangHoa>(Product_Detail.ID_KHO, bolTonKho: false, Product_Detail.ID_HANGHOAKHO);
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
					if (apiResponse.Data != null)
					{
						v_dm_HangHoa2 = (apiResponse.Data as List<v_dm_HangHoa>).FirstOrDefault();
					}
					if (v_dm_HangHoa2 != null)
					{
						Product_Detail.ID = Guid.NewGuid().ToString();
						Product_Detail.NAME = v_dm_HangHoa2.NAME;
						Product_Detail.MA = v_dm_HangHoa2.MA;
						if (v_dm_HangHoa2.ID_DVT == Product_Detail.ID_DVT)
						{
							Product_Detail.NAME_DVT = v_dm_HangHoa2.NAME_DVT;
							if (!string.IsNullOrEmpty(v_dm_HangHoa2.ID_DVT_QD))
							{
								Product_Detail.TYLE_QD = v_dm_HangHoa2.TYLE_QD;
							}
							else if (v_dm_HangHoa2.LOAIHANGHOA == 2.ToString())
							{
								Product_Detail.TYLE_QD = 0.0;
							}
							else
							{
								Product_Detail.TYLE_QD = 1.0;
							}
						}
						else if (v_dm_HangHoa2.ID_DVT_QD == Product_Detail.ID_DVT && !string.IsNullOrEmpty(v_dm_HangHoa2.ID_DVT_QD))
						{
							Product_Detail.NAME_DVT = v_dm_HangHoa2.NAME_DVT_QD;
							Product_Detail.TYLE_QD = 1.0;
						}
						Product_Detail product_Detail = Utility.LstProductInput.Where((Product_Detail e) => e.ID_HANGHOAKHO == Product_Detail.ID_HANGHOAKHO && e.ID_DVT == Product_Detail.ID_DVT && e.DONGIA == Product_Detail.DONGIA).FirstOrDefault();
						if (product_Detail == null)
						{
							List<Product_Detail> lstProductInput = Utility.LstProductInput;
							lstProductInput.Add(Product_Detail);
							if (v_dm_HangHoa2.LOAIHANGHOA == 1.ToString())
							{
								SP_Parameter sP_Parameter = new SP_Parameter();
								sP_Parameter.LOC_ID = Utility.LOC_ID;
								sP_Parameter.ID_KHO = Product_Detail.ID_KHO;
								sP_Parameter.ID_COMBO = Product_Detail.ID_HANGHOA;
								ApiResponse apiResponse2 = Utility.ExecuteStoredProc<Product_Detail>(sP_Parameter, "Sp_Get_DanhSachSanPhamKho_Combo");
								if (!apiResponse2.Success)
								{
									apiResponse.Data = new List<Product_Detail>();
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
								if (apiResponse2.Data != null)
								{
									List<Product_Detail> list = apiResponse2.Data as List<Product_Detail>;
									foreach (Product_Detail item in list)
									{
										item.ID = Guid.NewGuid().ToString();
										item.ID_DVT = item.ID_DVT_COMBO;
										item.SOLUONG = Product_Detail.SOLUONG * item.QTY_COMBO;
										item.TYLE_QD = item.TYLE_QD_COMBO;
										item.TONGSOLUONG = Product_Detail.SOLUONG * item.QTY_TOTAL_COMBO;
										item.DONGIA = 0.0;
										item.ISCOMBO = true;
										item.ID_COMBO = Product_Detail.ID_HANGHOA;
										Product_Detail.ID_COMBO = Product_Detail.ID_HANGHOA;
										lstProductInput.Add(item);
									}
								}
							}
							base.Session["lstProductInput"] = lstProductInput;
						}
					}
					List<Product_Detail> lstProductInput2 = Utility.LstProductInput;
					string absolutePath = base.Request.Url.AbsolutePath;
					apiResponse.ProductCombo = Utility.GetProductInputOutput(lstProductInput2, "InputOutput", bolTinhLai: true, 0.0, 0.0, 0.0, 0.0, bolSuaSoLuong: false, absolutePath.Contains("Input"));
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
				Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex, JsonConvert.SerializeObject(Product_Detail));
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
		public ActionResult UpdateAddProduct(Product_Detail Product_Detail)
		{
			ApiResponse apiResponse = new ApiResponse();
			try
			{
				List<Product_Detail> lstProductInput = Utility.LstProductInput;
				Utility.TinhTong(Product_Detail, null, lstProductInput);
				apiResponse.Success = true;
				apiResponse.Detail = Product_Detail;
				return new JsonResult
				{
					Data = apiResponse,
					JsonRequestBehavior = JsonRequestBehavior.AllowGet,
					MaxJsonLength = int.MaxValue
				};
			}
			catch (Exception ex)
			{
				Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex, JsonConvert.SerializeObject(Product_Detail));
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
		public ActionResult DeleteProductInputOutput(string ID)
		{
			ApiResponse apiResponse = new ApiResponse();
			try
			{
				v_v_dm_HangHoa v_v_dm_HangHoa2 = new v_v_dm_HangHoa();
				List<Product_Detail> lstProductInput = Utility.LstProductInput;
				Product_Detail check = Utility.LstProductInput.Where((Product_Detail e) => e.ID == ID).FirstOrDefault();
				if (check != null && lstProductInput != null)
				{
					if (!string.IsNullOrEmpty(check.ID_COMBO))
					{
						foreach (Product_Detail item in lstProductInput.Where((Product_Detail e) => e.ID_COMBO == check.ID_COMBO).ToList())
						{
							lstProductInput.Remove(item);
						}
					}
					else
					{
						lstProductInput.Remove(check);
					}
				}
				base.Session["lstProductInput"] = lstProductInput;
				string absolutePath = base.Request.Url.AbsolutePath;
				apiResponse.ProductCombo = Utility.GetProductInputOutput(lstProductInput, "InputOutput", bolTinhLai: true, 0.0, 0.0, 0.0, 0.0, bolSuaSoLuong: false, absolutePath.Contains("Input"));
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

		[HttpPost]
		public ActionResult UpdateProductInputOutput(string ID, string TYPE, string VALUE)
		{
			ApiResponse apiResponse = new ApiResponse();
			try
			{
				v_v_dm_HangHoa v_v_dm_HangHoa2 = new v_v_dm_HangHoa();
				List<Product_Detail> lstProductInput = Utility.LstProductInput;
				Product_Detail product_Detail = Utility.LstProductInput.Where((Product_Detail e) => e.ID == ID).FirstOrDefault();
				if (product_Detail != null)
				{
					product_Detail.TYPE = TYPE;
					Utility.TinhTong(product_Detail, VALUE, lstProductInput);
				}
				string absolutePath = base.Request.Url.AbsolutePath;
				base.Session["lstProductInput"] = lstProductInput;
				string text = base.Request.UrlReferrer?.ToString();
				apiResponse.ProductCombo = Utility.GetProductInputOutput(lstProductInput, "InputOutput", bolTinhLai: true, 0.0, 0.0, 0.0, 0.0, absolutePath.Contains("Input"), absolutePath.Contains("Input"), (!string.IsNullOrEmpty(text) && text.Contains("CheckData")) ? true : false);
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
	}
}
