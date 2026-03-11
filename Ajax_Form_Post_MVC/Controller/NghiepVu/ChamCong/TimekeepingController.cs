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
using PagedList;

namespace MVC_QuanLyTHP.Controllers
{

	public class TimekeepingController : Controller
	{
		public ActionResult Index()
		{
			try
			{
				if (Utility.KiemTra())
				{
					return RedirectToAction("Index", "Admin");
				}
				if (!Utility.KiemTraQuyen("Timekeeping", "View"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				ApiResponse apiResponse = new ApiResponse();
				IPagedList<v_nv_ChamCong> iPagedList = new List<v_nv_ChamCong>().ToList().ToPagedList(1, Utility.GetPageSize());
				v_v_nv_ChamCong v_v_nv_ChamCong2 = new v_v_nv_ChamCong();
				v_v_nv_ChamCong2.IPagedList = iPagedList;
				v_v_nv_ChamCong2.lstdm_NhanVien = new List<ComboboxFrom>();
				v_v_nv_ChamCong2.lstdm_NhanVien = Utility.GetListData<ComboboxFrom>("User").Data as List<ComboboxFrom>;
				v_v_nv_ChamCong2.lstdm_PhongBan = new List<dm_PhongBan>();
				v_v_nv_ChamCong2.lstdm_PhongBan = Utility.GetListData<dm_PhongBan>("Department", "", "", Utility.LOC_ID).Data as List<dm_PhongBan>;
				v_v_nv_ChamCong2.TUNGAY = DateTime.Now;
				v_v_nv_ChamCong2.DENNGAY = DateTime.Now;
				base.ViewBag.PermissionEdit = Utility.KiemTraQuyen("Timekeeping", "Edit");
				base.ViewBag.PermissionDelete = Utility.KiemTraQuyen("Timekeeping", "Delete");
				base.ViewBag.PermissionCreate = Utility.KiemTraQuyen("Timekeeping", "Create");
				if ((!base.ViewBag.PermissionCreate))
				{
					v_v_nv_ChamCong2.lstdm_NhanVien = v_v_nv_ChamCong2.lstdm_NhanVien.Where((ComboboxFrom s) => s.ID == base.Session["idUser"].ToString()).ToList();
				}
				return View(v_v_nv_ChamCong2);
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
		public ActionResult Index(SP_Parameter objParameter)
		{
			try
			{
				if (Utility.KiemTra())
				{
					return RedirectToAction("Index", "Admin");
				}
				if (!Utility.KiemTraQuyen("Timekeeping", "View"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				ApiResponse apiResponse = new ApiResponse();
				IPagedList<v_nv_ChamCong> pagedList = new List<v_nv_ChamCong>().ToList().ToPagedList(1, Utility.GetPageSize());
				base.ViewBag.PermissionCreate = Utility.KiemTraQuyen("Timekeeping", "Create");
				if ((!base.ViewBag.PermissionCreate))
				{
					objParameter.ID_NHANVIEN = base.Session["idUser"].ToString();
				}
				else
				{
					objParameter.ID_NHANVIEN = objParameter.ID_NHANVIEN;
				}
				apiResponse = Utility.Get_DanhSachChamCong<v_nv_ChamCong>(objParameter.TUNGAY, objParameter.DENNGAY, null, objParameter.KEY, objParameter.ID_NHANVIEN);
				if (!apiResponse.Success)
				{
					base.TempData["TitleError"] = apiResponse.Message;
					return RedirectToAction("Index", "Notfound");
				}
				List<v_nv_ChamCong> list = apiResponse.Data as List<v_nv_ChamCong>;
				pagedList = list.ToPagedList(1, (list.Count() > 0) ? list.Count() : 50);
				v_v_nv_ChamCong v_v_nv_ChamCong2 = new v_v_nv_ChamCong();
				v_v_nv_ChamCong2.IPagedList = pagedList;
				v_v_nv_ChamCong2.lstdm_NhanVien = new List<ComboboxFrom>();
				v_v_nv_ChamCong2.lstdm_NhanVien = Utility.GetListData<ComboboxFrom>("User").Data as List<ComboboxFrom>;
				v_v_nv_ChamCong2.lstdm_PhongBan = new List<dm_PhongBan>();
				v_v_nv_ChamCong2.lstdm_PhongBan = Utility.GetListData<dm_PhongBan>("Department", "", "", Utility.LOC_ID).Data as List<dm_PhongBan>;
				v_v_nv_ChamCong2.TUNGAY = (objParameter.TUNGAY.HasValue ? objParameter.TUNGAY.Value : Utility.CurrentTime);
				v_v_nv_ChamCong2.DENNGAY = (objParameter.DENNGAY.HasValue ? objParameter.DENNGAY.Value : Utility.CurrentTime);
				v_v_nv_ChamCong2.ID_NHANVIEN = objParameter.ID_NHANVIEN;
				v_v_nv_ChamCong2.ID_PHONGBAN = objParameter.ID_PHONGBAN;
				base.ViewBag.PermissionEdit = Utility.KiemTraQuyen("Timekeeping", "Edit");
				base.ViewBag.PermissionDelete = Utility.KiemTraQuyen("Timekeeping", "Delete");
				if ((!base.ViewBag.PermissionCreate))
				{
					v_v_nv_ChamCong2.lstdm_NhanVien = v_v_nv_ChamCong2.lstdm_NhanVien.Where((ComboboxFrom s) => s.ID == base.Session["idUser"].ToString()).ToList();
				}
				return View(v_v_nv_ChamCong2);
			}
			catch (Exception ex)
			{
				Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
				base.TempData["TitleError"] = "Đã có lỗi phát sinh trong phiên làm việc!";
				base.TempData["DetailError"] = ex.Message;
				return RedirectToAction("Index", "Notfound");
			}
		}

		public ActionResult TableTimekeeping()
		{
			try
			{
				if (Utility.KiemTra())
				{
					return RedirectToAction("Index", "Admin");
				}
				if (!Utility.KiemTraQuyen("Timekeeping", "View"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				ApiResponse apiResponse = new ApiResponse();
				List<v_nv_ChamCong> lstnv_ChamCong_Table = new List<v_nv_ChamCong>().ToList();
				v_v_nv_ChamCong v_v_nv_ChamCong2 = new v_v_nv_ChamCong();
				v_v_nv_ChamCong2.lstnv_ChamCong_Table = lstnv_ChamCong_Table;
				v_v_nv_ChamCong2.lstdm_NhanVien = new List<ComboboxFrom>();
				v_v_nv_ChamCong2.lstdm_NhanVien = Utility.GetListData<ComboboxFrom>("User").Data as List<ComboboxFrom>;
				v_v_nv_ChamCong2.lstdm_PhongBan = new List<dm_PhongBan>();
				v_v_nv_ChamCong2.lstdm_PhongBan = Utility.GetListData<dm_PhongBan>("Department", "", "", Utility.LOC_ID).Data as List<dm_PhongBan>;
				v_v_nv_ChamCong2.TUNGAY = DateTime.Now;
				v_v_nv_ChamCong2.DENNGAY = DateTime.Now;
				base.ViewBag.PermissionEdit = Utility.KiemTraQuyen("Timekeeping", "Edit");
				base.ViewBag.PermissionDelete = Utility.KiemTraQuyen("Timekeeping", "Delete");
				base.ViewBag.PermissionCreate = Utility.KiemTraQuyen("Timekeeping", "Create");
				base.ViewBag.IsLoad = false;
				return View(v_v_nv_ChamCong2);
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
		public ActionResult TableTimekeeping(SP_Parameter objParameter)
		{
			try
			{
				if (Utility.KiemTra())
				{
					return RedirectToAction("Index", "Admin");
				}
				if (!Utility.KiemTraQuyen("Timekeeping", "View"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				ApiResponse apiResponse = new ApiResponse();
				List<v_nv_ChamCong> list = new List<v_nv_ChamCong>().ToList();
				apiResponse = Utility.Get_DanhSachChamCong<v_nv_ChamCong>(objParameter.TUNGAY, objParameter.DENNGAY, null, objParameter.KEY, objParameter.ID_NHANVIEN);
				if (!apiResponse.Success)
				{
					base.TempData["TitleError"] = apiResponse.Message;
					return RedirectToAction("Index", "Notfound");
				}
				list = apiResponse.Data as List<v_nv_ChamCong>;
				List<v_dm_NhanVien> list2 = Utility.GetListData<v_dm_NhanVien>("Employee", "", "", Utility.LOC_ID).Data as List<v_dm_NhanVien>;
				if (!string.IsNullOrEmpty(objParameter.ID_NHANVIEN))
				{
					list2 = list2.Where((v_dm_NhanVien s) => s.ID_TAIKHOAN == objParameter.ID_NHANVIEN).ToList();
				}
				if (!string.IsNullOrEmpty(objParameter.ID_PHONGBAN))
				{
					list2 = list2.Where((v_dm_NhanVien s) => s.ID_PHONGBAN == objParameter.ID_PHONGBAN).ToList();
				}
				apiResponse = Utility.Get_DanhSachNghiPhep<v_nv_NghiPhep>(objParameter.TUNGAY, objParameter.DENNGAY, null, objParameter.KEY, objParameter.ID_NHANVIEN);
				if (!apiResponse.Success)
				{
					base.TempData["TitleError"] = apiResponse.Message;
					return RedirectToAction("Index", "Notfound");
				}
				List<v_nv_NghiPhep> lstnv_NghiPhep_Table = apiResponse.Data as List<v_nv_NghiPhep>;
				v_v_nv_ChamCong v_v_nv_ChamCong2 = new v_v_nv_ChamCong();
				v_v_nv_ChamCong2.lstnv_ChamCong_Table = list;
				v_v_nv_ChamCong2.lstdm_NhanVien = new List<ComboboxFrom>();
				v_v_nv_ChamCong2.lstdm_NhanVien = Utility.GetListData<ComboboxFrom>("User").Data as List<ComboboxFrom>;
				v_v_nv_ChamCong2.lstdm_PhongBan = new List<dm_PhongBan>();
				v_v_nv_ChamCong2.lstdm_PhongBan = Utility.GetListData<dm_PhongBan>("Department", "", "", Utility.LOC_ID).Data as List<dm_PhongBan>;
				v_v_nv_ChamCong2.TUNGAY = (objParameter.TUNGAY.HasValue ? objParameter.TUNGAY.Value : Utility.CurrentTime);
				v_v_nv_ChamCong2.DENNGAY = (objParameter.DENNGAY.HasValue ? objParameter.DENNGAY.Value : Utility.CurrentTime);
				v_v_nv_ChamCong2.ID_NHANVIEN = objParameter.ID_NHANVIEN;
				v_v_nv_ChamCong2.ID_PHONGBAN = objParameter.ID_PHONGBAN;
				v_v_nv_ChamCong2.lstdm_NhanVien_Table = new List<v_dm_NhanVien>();
				v_v_nv_ChamCong2.lstdm_NhanVien_Table = list2;
				v_v_nv_ChamCong2.lstdm_ThangLuong_Table = new List<dm_ThangLuong>();
				v_v_nv_ChamCong2.lstdm_ThangLuong_Table = Utility.GetListData<dm_ThangLuong>("MonthlySalary", "", "", Utility.LOC_ID).Data as List<dm_ThangLuong>;
				v_v_nv_ChamCong2.lstnv_NghiPhep_Table = new List<v_nv_NghiPhep>();
				v_v_nv_ChamCong2.lstnv_NghiPhep_Table = lstnv_NghiPhep_Table;
				base.ViewBag.PermissionEdit = Utility.KiemTraQuyen("Timekeeping", "Edit");
				base.ViewBag.PermissionDelete = Utility.KiemTraQuyen("Timekeeping", "Delete");
				base.ViewBag.PermissionCreate = Utility.KiemTraQuyen("Timekeeping", "Create");
				base.ViewBag.IsLoad = true;
				return View(v_v_nv_ChamCong2);
			}
			catch (Exception ex)
			{
				Utility.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex);
				base.TempData["TitleError"] = "Đã có lỗi phát sinh trong phiên làm việc!";
				base.TempData["DetailError"] = ex.Message;
				return RedirectToAction("Index", "Notfound");
			}
		}

		public ActionResult Timekeeping()
		{
			try
			{
				if (Utility.KiemTra())
				{
					return RedirectToAction("Index", "Admin");
				}
				if (!Utility.KiemTraQuyen("Timekeeping", "View"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				Login_Model login_Model = (Login_Model)base.Session["Login_Model"];
				ApiResponse apiResponse = new ApiResponse();
				apiResponse = Utility.Get_DanhSachChamCong<v_nv_ChamCong>(null, null, Utility.CurrentTime.Date, null, (login_Model != null) ? login_Model.iduser : "");
				if (!apiResponse.Success)
				{
					base.TempData["TitleError"] = apiResponse.Message;
					return RedirectToAction("Index", "Notfound");
				}
				List<v_nv_ChamCong> list = apiResponse.Data as List<v_nv_ChamCong>;
				v_nv_ChamCong v_nv_ChamCong2 = new v_nv_ChamCong();
				if (list != null && list.Count > 0)
				{
					v_nv_ChamCong2 = list.FirstOrDefault();
				}
				else
				{
					v_nv_ChamCong2.ID_NHANVIEN = ((login_Model != null) ? login_Model.iduser : "");
					v_nv_ChamCong2.NGAYCONG = Utility.CurrentTime.Date;
					v_nv_ChamCong2.LOC_ID = Utility.LOC_ID;
					v_nv_ChamCong2.ID = Guid.NewGuid().ToString();
				}
				base.ViewBag.NAME_NHANVIEN = ((login_Model != null) ? login_Model.fullname : "");
				base.ViewBag.AVATAR = ((login_Model != null && login_Model.fullname != null && login_Model.fullname.Length > 0) ? login_Model.fullname.Substring(0, 1) : "");
				base.ViewBag.TIMER = Utility.CurrentTime;
				base.ViewBag.TYPE = (v_nv_ChamCong2.THOIGIANVAO.HasValue ? "đăng xuất" : "đăng nhập");
				base.ViewBag.TYPEFORM = (v_nv_ChamCong2.THOIGIANVAO.HasValue ? ("CheckOut('Timekeeping','" + Utility.CurrentTime.ToString("yyyy-MM-ddT00:00:00.000Z") + "','" + v_nv_ChamCong2.ID + "');") : ("CheckIn('Timekeeping','" + Utility.CurrentTime.ToString("yyyy-MM-ddT00:00:00.000Z") + "');"));
				base.ViewBag.LOGO = (v_nv_ChamCong2.THOIGIANVAO.HasValue ? "logout.png" : "login.png");
				string text = (v_nv_ChamCong2.THOIGIANVAO.HasValue ? ("<label name=\"TXTTHOIGIANVAO\" id=\"TXTTHOIGIANVAO\">Thời gian vào: " + v_nv_ChamCong2.THOIGIANVAO.Value.ToString("H:mm:ss") + "</label>") : "<label name=\"TXTTHOIGIANVAO\" id=\"TXTTHOIGIANVAO\"></label>");
				text += (v_nv_ChamCong2.THOIGIANRA.HasValue ? ((string.IsNullOrEmpty(text) ? "" : "<br>") + "<label name=\"TXTTHOIGIANRA\" id=\"TXTTHOIGIANRA\">Thời gian ra: " + v_nv_ChamCong2.THOIGIANRA.Value.ToString("H:mm:ss") + "</label>") : "<label name=\"TXTTHOIGIANRA\" id=\"TXTTHOIGIANRA\"></label>");
				base.ViewBag.TIMERTEXT = text;
				return View(v_nv_ChamCong2);
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
		public ActionResult CheckIn(string NGAYCONG, string LATITUDELONGITUDE, string MYPUBLICIPV4)
		{
			try
			{
				ApiResponse apiResponse = new ApiResponse();
				if (Utility.KiemTra())
				{
					return RedirectToAction("Index", "Admin");
				}
				if (!Utility.KiemTraQuyen("Timekeeping", "Create"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				double num = 0.0;
				double num2 = 0.0;
				string text = "";
				if (!string.IsNullOrEmpty(LATITUDELONGITUDE))
				{
					num = Convert.ToDouble(LATITUDELONGITUDE.Split('-')[0].Replace(".", ","));
					num2 = Convert.ToDouble(LATITUDELONGITUDE.Split('-')[1].Replace(".", ","));
					apiResponse = Utility.GetListData<v_dm_DiaDiemChamCong>("Location", "", "", Utility.LOC_ID);
					if (!apiResponse.Success)
					{
						base.TempData["TitleError"] = apiResponse.Message;
						return RedirectToAction("Index", "Notfound");
					}
					List<v_dm_DiaDiemChamCong> list = apiResponse.Data as List<v_dm_DiaDiemChamCong>;
					bool flag = false;
					foreach (v_dm_DiaDiemChamCong item in list)
					{
						if (item.ISACTIVE)
						{
							double num3 = API.CalculateDistance(Convert.ToDouble(item.LATITUDE.Replace(".", ",")), Convert.ToDouble(item.LONGITUDE.Replace(".", ",")), num, num2);
							if (num3 <= item.KHOANGCACH)
							{
								flag = true;
								text = text + item.NAME + ": " + num3.ToString("N0") + " m; ";
							}
							else
							{
								text = text + item.NAME + ": " + num3.ToString("N0") + " m; ";
							}
						}
					}
					if (!flag)
					{
						base.ModelState.AddModelError(string.Empty, "Khảng cách xa với điểm được chỉ định chấm công!");
						apiResponse.Message = "Khảng cách xa với điểm được chỉ định chấm công!" + text;
					}
				}
				else
				{
					base.ModelState.AddModelError(string.Empty, "Không lấy được địa điểm chấm công!");
					apiResponse.Message = "Không lấy được địa điểm chấm công!";
				}
				v_nv_ChamCong v_nv_ChamCong2 = new v_nv_ChamCong();
				v_nv_ChamCong2.NGAYCONG = Convert.ToDateTime(NGAYCONG);
				if (v_nv_ChamCong2.NGAYCONG.Date != Utility.CurrentTime.Date)
				{
					base.ModelState.AddModelError(string.Empty, "Ngày chấm công khác với ngày hiện tại!");
					apiResponse.Message = "Ngày chấm công khác với ngày hiện tại!";
				}
				v_nv_ChamCong2.LOC_ID = Utility.LOC_ID;
				v_nv_ChamCong2.ID_NHANVIEN = base.Session["idUser"].ToString();
				v_nv_ChamCong2.ID = Guid.NewGuid().ToString();
				if (base.ModelState.IsValid)
				{
					v_nv_ChamCong2.ID_NGUOITAO = base.Session["idUser"].ToString();
					v_nv_ChamCong2.THOIGIANTHEM = Utility.CurrentTime;
					v_nv_ChamCong2.THOIGIANVAO = Utility.CurrentTime;
					v_nv_ChamCong2.NGAYCONG = Utility.CurrentTime.Date;
					v_nv_ChamCong2.IP_CHAMCONGVAO = MYPUBLICIPV4;
					v_nv_ChamCong2.GHICHU = text;
					apiResponse = Utility.Create((nv_ChamCong)v_nv_ChamCong2, "Timekeeping/PostCheckIn");
					if (apiResponse.Success)
					{
						apiResponse.Message = "Chấm công vào thành công! " + v_nv_ChamCong2.THOIGIANVAO.Value.ToString("dd/MM HH:mm:ss");
					}
					else
					{
						base.ModelState.AddModelError(string.Empty, apiResponse.Message);
						apiResponse.Message = apiResponse.Message;
					}
					return new JsonResult
					{
						Data = apiResponse,
						JsonRequestBehavior = JsonRequestBehavior.AllowGet,
						MaxJsonLength = int.MaxValue
					};
				}
				apiResponse.Success = false;
				apiResponse.Data = Utility.GetModelState(base.ModelState, "Timekeeping");
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
				return RedirectToAction("Index", "Notfound");
			}
		}

		[HttpPost]
		public ActionResult CheckOut(string NGAYCONG, string ID, string LATITUDELONGITUDE, string MYPUBLICIPV4)
		{
			try
			{
				ApiResponse apiResponse = new ApiResponse();
				if (Utility.KiemTra())
				{
					return RedirectToAction("Index", "Admin");
				}
				if (!Utility.KiemTraQuyen("Timekeeping", "Create"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				double num = 0.0;
				double num2 = 0.0;
				string text = "";
				if (!string.IsNullOrEmpty(LATITUDELONGITUDE))
				{
					num = Convert.ToDouble(LATITUDELONGITUDE.Split('-')[0].Replace(".", ","));
					num2 = Convert.ToDouble(LATITUDELONGITUDE.Split('-')[1].Replace(".", ","));
					apiResponse = Utility.GetListData<v_dm_DiaDiemChamCong>("Location", "", "", Utility.LOC_ID);
					if (!apiResponse.Success)
					{
						base.TempData["TitleError"] = apiResponse.Message;
						return RedirectToAction("Index", "Notfound");
					}
					List<v_dm_DiaDiemChamCong> list = apiResponse.Data as List<v_dm_DiaDiemChamCong>;
					bool flag = false;
					foreach (v_dm_DiaDiemChamCong item in list)
					{
						if (item.ISACTIVE)
						{
							double num3 = API.CalculateDistance(Convert.ToDouble(item.LATITUDE.Replace(".", ",")), Convert.ToDouble(item.LONGITUDE.Replace(".", ",")), num, num2);
							if (num3 <= item.KHOANGCACH)
							{
								flag = true;
								text = text + item.NAME + ": " + num3.ToString("N0") + " m; ";
							}
							else
							{
								text = text + item.NAME + ": " + num3.ToString("N0") + " m; ";
							}
						}
					}
					if (!flag)
					{
						base.ModelState.AddModelError(string.Empty, "Khảng cách xa với điểm được chỉ định chấm công!");
						apiResponse.Message = "Khảng cách xa với điểm được chỉ định chấm công!" + text;
					}
				}
				else
				{
					base.ModelState.AddModelError(string.Empty, "Không lấy được địa điểm chấm công!");
					apiResponse.Message = "Không lấy được địa điểm chấm công!";
				}
				v_nv_ChamCong v_nv_ChamCong2 = new v_nv_ChamCong();
				v_nv_ChamCong2.ID = ID;
				v_nv_ChamCong2.NGAYCONG = Convert.ToDateTime(NGAYCONG);
				if (v_nv_ChamCong2.NGAYCONG.Date != Utility.CurrentTime.Date)
				{
					base.ModelState.AddModelError(string.Empty, "Ngày chấm công khác với ngày hiện tại!");
					apiResponse.Message = "Ngày chấm công khác với ngày hiện tại!";
				}
				if (base.ModelState.IsValid)
				{
					v_nv_ChamCong2.LOC_ID = Utility.LOC_ID;
					v_nv_ChamCong2.ID_NHANVIEN = base.Session["idUser"].ToString();
					v_nv_ChamCong2.ID_NGUOISUA = base.Session["idUser"].ToString();
					v_nv_ChamCong2.THOIGIANSUA = Utility.CurrentTime;
					v_nv_ChamCong2.THOIGIANRA = Utility.CurrentTime;
					v_nv_ChamCong2.IP_CHAMCONGRA = MYPUBLICIPV4;
					v_nv_ChamCong2.GHICHU = text;
					apiResponse = Utility.Create((nv_ChamCong)v_nv_ChamCong2, "Timekeeping/PostCheckOut");
					if (apiResponse.Success)
					{
						apiResponse.Message = "Chấm công ra thành công! " + v_nv_ChamCong2.THOIGIANRA.Value.ToString("dd/MM HH:mm:ss");
					}
					else
					{
						base.ModelState.AddModelError(string.Empty, apiResponse.Message);
						apiResponse.Message = apiResponse.Message;
					}
					return new JsonResult
					{
						Data = apiResponse,
						JsonRequestBehavior = JsonRequestBehavior.AllowGet,
						MaxJsonLength = int.MaxValue
					};
				}
				apiResponse.Success = false;
				apiResponse.Data = Utility.GetModelState(base.ModelState, "Timekeeping");
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
				if (!Utility.KiemTraQuyen("Timekeeping", "Create"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				v_v_nv_ChamCong v_v_nv_ChamCong2 = new v_v_nv_ChamCong();
				v_v_nv_ChamCong2.LOC_ID = Utility.LOC_ID;
				v_v_nv_ChamCong2.ID_NGUOITAO = base.Session["idUser"].ToString();
				v_v_nv_ChamCong2.THOIGIANTHEM = Utility.CurrentTime;
				v_v_nv_ChamCong2.ID = Guid.NewGuid().ToString();
				v_v_nv_ChamCong2.lstdm_NhanVien = new List<ComboboxFrom>();
				v_v_nv_ChamCong2.lstdm_NhanVien = Utility.GetListData<ComboboxFrom>("User").Data as List<ComboboxFrom>;
				return View(v_v_nv_ChamCong2);
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
		public ActionResult Create([Bind(Include = "LOC_ID,ID,ID_NHANVIEN,ID_THANGLUONG,NGAYCONG,THOIGIANVAO,THOIGIANRA,SOTIENGLAMVIEC,GHICHU,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO,ISNGHIPHEP")] v_nv_ChamCong nv_ChamCong)
		{
			try
			{
				if (Utility.KiemTra())
				{
					return RedirectToAction("Index", "Admin");
				}
				if (!Utility.KiemTraQuyen("Timekeeping", "Create"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				if (base.ModelState.IsValid)
				{
					nv_ChamCong.LOC_ID = Utility.LOC_ID;
					nv_ChamCong.ID_NGUOITAO = base.Session["idUser"].ToString();
					nv_ChamCong.THOIGIANTHEM = Utility.CurrentTime;
					ApiResponse apiResponse = Utility.Create((nv_ChamCong)nv_ChamCong, "Timekeeping");
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
				return View(nv_ChamCong);
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
				if (!Utility.KiemTraQuyen("Timekeeping", "Edit"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				v_v_nv_ChamCong v_v_nv_ChamCong2 = new v_v_nv_ChamCong();
				if (!string.IsNullOrEmpty(id))
				{
					apiResponse = Utility.GetDetail<v_nv_ChamCong>(Utility.LOC_ID + "/" + id, "Timekeeping");
					if (!apiResponse.Success)
					{
						base.TempData["TitleError"] = apiResponse.Message;
						return RedirectToAction("Index", "Notfound");
					}
					if (apiResponse.Data != null)
					{
						v_v_nv_ChamCong2 = apiResponse.Data as v_v_nv_ChamCong;
					}
				}
				v_v_nv_ChamCong2.lstdm_NhanVien = new List<ComboboxFrom>();
				v_v_nv_ChamCong2.lstdm_NhanVien = Utility.GetListData<ComboboxFrom>("User").Data as List<ComboboxFrom>;
				return View(v_v_nv_ChamCong2);
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
		public ActionResult Edit([Bind(Include = "LOC_ID,ID,ID_NHANVIEN,ID_THANGLUONG,NGAYCONG,THOIGIANVAO,THOIGIANRA,SOTIENGLAMVIEC,GHICHU,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO,ISNGHIPHEP")] v_nv_ChamCong nv_ChamCong)
		{
			try
			{
				if (Utility.KiemTra())
				{
					return RedirectToAction("Index", "Admin");
				}
				if (!Utility.KiemTraQuyen("Timekeeping", "Edit"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				if (base.ModelState.IsValid)
				{
					nv_ChamCong.LOC_ID = Utility.LOC_ID;
					nv_ChamCong.ID_NGUOISUA = base.Session["idUser"].ToString();
					nv_ChamCong.THOIGIANSUA = Utility.CurrentTime;
					ApiResponse apiResponse = Utility.Edit(Utility.LOC_ID + "/" + nv_ChamCong.ID, nv_ChamCong, "Timekeeping");
					if (apiResponse.Success)
					{
						return RedirectToAction("Index");
					}
					base.ModelState.AddModelError(string.Empty, apiResponse.Message);
				}
				return View(nv_ChamCong);
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
				if (!Utility.KiemTraQuyen("Timekeeping", "Delete"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				ApiResponse apiResponse = Utility.Delete<v_nv_ChamCong>(Utility.LOC_ID + "/" + id, "Timekeeping");
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
					apiResponse.URL = base.Url.Action("Index", "Admin");
					return new JsonResult
					{
						Data = apiResponse,
						JsonRequestBehavior = JsonRequestBehavior.AllowGet,
						MaxJsonLength = int.MaxValue
					};
				}
				if (!Utility.KiemTraQuyen("Timekeeping", "Create"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					apiResponse.URL = base.Url.Action("Index", "Notfound");
					return new JsonResult
					{
						Data = apiResponse,
						JsonRequestBehavior = JsonRequestBehavior.AllowGet,
						MaxJsonLength = int.MaxValue
					};
				}
				v_v_nv_ChamCong v_v_nv_ChamCong2 = new v_v_nv_ChamCong();
				apiResponse.Success = true;
				v_v_nv_ChamCong2.LOC_ID = Utility.LOC_ID;
				v_v_nv_ChamCong2.ID = Guid.NewGuid().ToString();
				v_v_nv_ChamCong2.NGAYCONG = Utility.CurrentTime;
				v_v_nv_ChamCong2.lstdm_NhanVien = new List<ComboboxFrom>();
				List<ComboboxFrom> list = Utility.GetListData<ComboboxFrom>("User").Data as List<ComboboxFrom>;
				foreach (ComboboxFrom item in list)
				{
					item.ISACTIVE = true;
				}
				v_v_nv_ChamCong2.lstdm_NhanVien = list;
				apiResponse.Detail = Utility.ConvertobjectTo(v_v_nv_ChamCong2);
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
		public ActionResult CreatePopupDate(string Date, string ID_TAIKHOAN)
		{
			ApiResponse apiResponse = new ApiResponse();
			try
			{
				if (Utility.KiemTra())
				{
					apiResponse.URL = base.Url.Action("Index", "Admin");
					return new JsonResult
					{
						Data = apiResponse,
						JsonRequestBehavior = JsonRequestBehavior.AllowGet,
						MaxJsonLength = int.MaxValue
					};
				}
				if (!Utility.KiemTraQuyen("Timekeeping", "Create"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					apiResponse.URL = base.Url.Action("Index", "Notfound");
					return new JsonResult
					{
						Data = apiResponse,
						JsonRequestBehavior = JsonRequestBehavior.AllowGet,
						MaxJsonLength = int.MaxValue
					};
				}
				v_v_nv_ChamCong v_v_nv_ChamCong2 = new v_v_nv_ChamCong();
				apiResponse.Success = true;
				v_v_nv_ChamCong2.LOC_ID = Utility.LOC_ID;
				v_v_nv_ChamCong2.ID = Guid.NewGuid().ToString();
				DateTime dateTime = (v_v_nv_ChamCong2.NGAYCONG = Convert.ToDateTime(Date));
				v_v_nv_ChamCong2.THOIGIANVAO = dateTime.AddHours(8.0);
				v_v_nv_ChamCong2.THOIGIANRA = dateTime.AddHours(17.0);
				v_v_nv_ChamCong2.ID_NHANVIEN = ID_TAIKHOAN;
				v_v_nv_ChamCong2.lstdm_NhanVien = new List<ComboboxFrom>();
				List<ComboboxFrom> list = Utility.GetListData<ComboboxFrom>("User").Data as List<ComboboxFrom>;
				foreach (ComboboxFrom item in list)
				{
					item.ISACTIVE = true;
					if (item.ID == ID_TAIKHOAN)
					{
						item.ISDEFAULT = true;
					}
				}
				v_v_nv_ChamCong2.lstdm_NhanVien = list;
				apiResponse.Detail = Utility.ConvertobjectTo(v_v_nv_ChamCong2);
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
		public ActionResult CreatePopup([Bind(Include = "BUTTONTYPE,LOC_ID,ID,ID_NHANVIEN,ID_THANGLUONG,NGAYCONG,THOIGIANVAO,THOIGIANRA,SOTIENGLAMVIEC,GHICHU,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO,ISNGHIPHEP")] v_nv_ChamCong nv_ChamCong)
		{
			ApiResponse apiResponse = new ApiResponse();
			try
			{
				if (Utility.KiemTra())
				{
					apiResponse.URL = base.Url.Action("Index", "Admin");
					return new JsonResult
					{
						Data = apiResponse,
						JsonRequestBehavior = JsonRequestBehavior.AllowGet,
						MaxJsonLength = int.MaxValue
					};
				}
				if (!Utility.KiemTraQuyen("Timekeeping", "Create"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
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
					nv_ChamCong.LOC_ID = Utility.LOC_ID;
					nv_ChamCong.ID_NGUOITAO = base.Session["idUser"].ToString();
					nv_ChamCong.THOIGIANTHEM = Utility.CurrentTime;
					apiResponse = Utility.Create((nv_ChamCong)nv_ChamCong, "Timekeeping");
					if (apiResponse.Success)
					{
						apiResponse.NewID = Guid.NewGuid().ToString();
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
					apiResponse.Data = Utility.GetModelState(base.ModelState, "Timekeeping");
				}
				apiResponse.ID = nv_ChamCong.ID;
				List<ValueEdit> list = Utility.ConvertobjectTo((nv_ChamCong)nv_ChamCong, "yyyy-MM-dd HH:mm:ss");
				if (nv_ChamCong.BUTTONTYPE == "TABLETIME")
				{
					list.Clear();
					List<dm_ThangLuong> list2 = Utility.GetListData<dm_ThangLuong>("MonthlySalary", "", "", Utility.LOC_ID).Data as List<dm_ThangLuong>;
					string text = "#FFFF00";
					string text2 = "#0033FF;color: white;";
					string text3 = "";
					if (list2 != null)
					{
						dm_ThangLuong dm_ThangLuong2 = list2.Where((dm_ThangLuong s) => s.THANG == (double)nv_ChamCong.NGAYCONG.Month && s.NAM == (double)nv_ChamCong.NGAYCONG.Year && s.NGAYBATDAU <= nv_ChamCong.NGAYCONG && s.NGAYKETTHUC >= nv_ChamCong.NGAYCONG && s.ISACTIVE).FirstOrDefault();
						if (dm_ThangLuong2 != null)
						{
							if (dm_ThangLuong2 != null)
							{
								if (nv_ChamCong.THOIGIANVAO.Value.TimeOfDay > dm_ThangLuong2.GIOBATDAU)
								{
									text3 = text;
								}
								if (nv_ChamCong.THOIGIANRA.Value.TimeOfDay < dm_ThangLuong2.GIOKETTHUC)
								{
									text3 = text;
								}
								if (string.IsNullOrEmpty(text3))
								{
									text3 = text2;
								}
							}
							list.Add(new ValueEdit
							{
								Key = nv_ChamCong.ID_NHANVIEN + "-" + nv_ChamCong.NGAYCONG.ToString("dd/MM/yyyy"),
								Value = "<button style=\"width:70px;height:50px;background-color:" + text3 + ";\" id=\"" + nv_ChamCong.ID_NHANVIEN + "-" + nv_ChamCong.NGAYCONG.ToString("dd/MM/yyy") + "\" ondblclick=\"myFunctionEdit('Timekeeping','" + nv_ChamCong.ID + "')\">" + nv_ChamCong.THOIGIANVAO.Value.ToString("HH:mm") + "<br>" + nv_ChamCong.THOIGIANRA.Value.ToString("HH:mm") + "</button>"
							});
						}
					}
					apiResponse.ID = nv_ChamCong.ID_NHANVIEN;
					apiResponse.MAPHIEU = nv_ChamCong.ID;
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
					apiResponse.URL = base.Url.Action("Index", "Admin");
					return new JsonResult
					{
						Data = apiResponse,
						JsonRequestBehavior = JsonRequestBehavior.AllowGet,
						MaxJsonLength = int.MaxValue
					};
				}
				if (!Utility.KiemTraQuyen("Timekeeping", "Edit"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					apiResponse.URL = base.Url.Action("Index", "Notfound");
					return new JsonResult
					{
						Data = apiResponse,
						JsonRequestBehavior = JsonRequestBehavior.AllowGet,
						MaxJsonLength = int.MaxValue
					};
				}
				v_v_nv_ChamCong v_v_nv_ChamCong2 = new v_v_nv_ChamCong();
				if (!string.IsNullOrEmpty(id))
				{
					apiResponse = Utility.GetDetail<v_v_nv_ChamCong>(Utility.LOC_ID + "/" + id, "Timekeeping");
					if (!apiResponse.Success)
					{
						base.TempData["TitleError"] = apiResponse.Message;
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
						v_v_nv_ChamCong2 = apiResponse.Data as v_v_nv_ChamCong;
					}
				}
				apiResponse.Success = true;
				v_v_nv_ChamCong2.lstdm_NhanVien = new List<ComboboxFrom>();
				List<ComboboxFrom> list = Utility.GetListData<ComboboxFrom>("User").Data as List<ComboboxFrom>;
				foreach (ComboboxFrom item in list)
				{
					item.ISACTIVE = true;
				}
				v_v_nv_ChamCong2.lstdm_NhanVien = list;
				apiResponse.Detail = Utility.ConvertobjectTo(v_v_nv_ChamCong2);
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
		public ActionResult EditPopup([Bind(Include = "BUTTONTYPE,LOC_ID,ID,ID_NHANVIEN,ID_THANGLUONG,NGAYCONG,THOIGIANVAO,THOIGIANRA,SOTIENGLAMVIEC,GHICHU,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO,ISNGHIPHEP")] v_nv_ChamCong nv_ChamCong)
		{
			ApiResponse apiResponse = new ApiResponse();
			try
			{
				if (Utility.KiemTra())
				{
					apiResponse.URL = base.Url.Action("Index", "Admin");
					return new JsonResult
					{
						Data = apiResponse,
						JsonRequestBehavior = JsonRequestBehavior.AllowGet,
						MaxJsonLength = int.MaxValue
					};
				}
				if (!Utility.KiemTraQuyen("Timekeeping", "Edit"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
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
					nv_ChamCong.LOC_ID = Utility.LOC_ID;
					nv_ChamCong.ID_NGUOISUA = base.Session["idUser"].ToString();
					nv_ChamCong.THOIGIANSUA = Utility.CurrentTime;
					apiResponse = Utility.Edit(Utility.LOC_ID + "/" + nv_ChamCong.ID, nv_ChamCong, "Timekeeping");
					if (apiResponse.Success)
					{
						apiResponse.ID = nv_ChamCong.ID;
					}
					else
					{
						base.ModelState.AddModelError(string.Empty, apiResponse.Message);
					}
				}
				else
				{
					apiResponse.Success = false;
					apiResponse.Data = Utility.GetModelState(base.ModelState, "Timekeeping");
				}
				List<ValueEdit> list = Utility.ConvertobjectTo((nv_ChamCong)nv_ChamCong, "yyyy-MM-dd HH:mm:ss");
				if (nv_ChamCong.BUTTONTYPE == "TABLETIME")
				{
					list.Clear();
					List<dm_ThangLuong> list2 = Utility.GetListData<dm_ThangLuong>("MonthlySalary", "", "", Utility.LOC_ID).Data as List<dm_ThangLuong>;
					string text = "#FFFF00";
					string text2 = "#0033FF;color: white;";
					string text3 = "";
					if (list2 != null)
					{
						dm_ThangLuong dm_ThangLuong2 = list2.Where((dm_ThangLuong s) => s.THANG == (double)nv_ChamCong.NGAYCONG.Month && s.NAM == (double)nv_ChamCong.NGAYCONG.Year && s.NGAYBATDAU <= nv_ChamCong.NGAYCONG && s.NGAYKETTHUC >= nv_ChamCong.NGAYCONG && s.ISACTIVE).FirstOrDefault();
						if (dm_ThangLuong2 != null)
						{
							if (dm_ThangLuong2 != null)
							{
								if (nv_ChamCong.THOIGIANVAO.Value.TimeOfDay > dm_ThangLuong2.GIOBATDAU)
								{
									text3 = text;
								}
								if (nv_ChamCong.THOIGIANRA.Value.TimeOfDay < dm_ThangLuong2.GIOKETTHUC)
								{
									text3 = text;
								}
								if (string.IsNullOrEmpty(text3))
								{
									text3 = text2;
								}
							}
							list.Add(new ValueEdit
							{
								Key = nv_ChamCong.ID_NHANVIEN + "-" + nv_ChamCong.NGAYCONG.ToString("dd/MM/yyyy"),
								Value = "<button style=\"width:70px;height:50px;background-color:" + text3 + ";\" id=\"" + nv_ChamCong.ID_NHANVIEN + "-" + nv_ChamCong.NGAYCONG.ToString("dd/MM/yyy") + "\" ondblclick=\"myFunctionEdit('Timekeeping','" + nv_ChamCong.ID + "')\">" + nv_ChamCong.THOIGIANVAO.Value.ToString("HH:mm") + "<br>" + nv_ChamCong.THOIGIANRA.Value.ToString("HH:mm") + "</button>"
							});
						}
					}
					apiResponse.ID = nv_ChamCong.ID_NHANVIEN;
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
					apiResponse.URL = base.Url.Action("Index", "Admin");
					return new JsonResult
					{
						Data = apiResponse,
						JsonRequestBehavior = JsonRequestBehavior.AllowGet,
						MaxJsonLength = int.MaxValue
					};
				}
				if (!Utility.KiemTraQuyen("Timekeeping", "Delete"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					apiResponse.URL = base.Url.Action("Index", "Notfound");
					return new JsonResult
					{
						Data = apiResponse,
						JsonRequestBehavior = JsonRequestBehavior.AllowGet,
						MaxJsonLength = int.MaxValue
					};
				}
				apiResponse = Utility.Delete<v_nv_ChamCong>(Utility.LOC_ID + "/" + id, "Timekeeping");
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
