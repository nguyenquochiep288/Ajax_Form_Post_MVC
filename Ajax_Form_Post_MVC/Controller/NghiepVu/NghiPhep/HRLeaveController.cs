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

	public class HRLeaveController : Controller
	{
		public ActionResult Index()
		{
			try
			{
				if (Utility.KiemTra())
				{
					return RedirectToAction("Index", "Admin");
				}
				if (!Utility.KiemTraQuyen("HRLeave", "View"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				ApiResponse apiResponse = new ApiResponse();
				IPagedList<v_nv_NghiPhep> iPagedList = new List<v_nv_NghiPhep>().ToList().ToPagedList(1, Utility.GetPageSize());
				v_v_nv_NghiPhep v_v_nv_NghiPhep2 = new v_v_nv_NghiPhep();
				v_v_nv_NghiPhep2.IPagedList = iPagedList;
				v_v_nv_NghiPhep2.lstdm_NhanVien = new List<ComboboxFrom>();
				v_v_nv_NghiPhep2.lstdm_NhanVien = Utility.GetListData<ComboboxFrom>("User").Data as List<ComboboxFrom>;
				v_v_nv_NghiPhep2.lstnv_PhepNam = new List<ComboboxFrom>();
				v_v_nv_NghiPhep2.TUNGAY = DateTime.Now;
				v_v_nv_NghiPhep2.DENNGAY = DateTime.Now;
				base.ViewBag.PermissionEdit = Utility.KiemTraQuyen("HRLeave", "Edit");
				base.ViewBag.PermissionDelete = Utility.KiemTraQuyen("HRLeave", "Delete");
				base.ViewBag.PermissionCreate = Utility.KiemTraQuyen("HRLeave", "Create");
				base.ViewBag.PermissionCreateUser = Utility.KiemTraQuyen("HRLeave", "CreateUser");
				base.ViewBag.PermissionApproveLeave = Utility.KiemTraQuyen("HRLeave", "ApproveLeave");
				if ((!base.ViewBag.PermissionCreateUser))
				{
					v_v_nv_NghiPhep2.lstdm_NhanVien = v_v_nv_NghiPhep2.lstdm_NhanVien.Where((ComboboxFrom s) => s.ID == base.Session["idUser"].ToString()).ToList();
				}
				return View(v_v_nv_NghiPhep2);
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
				if (!Utility.KiemTraQuyen("HRLeave", "View"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				ApiResponse apiResponse = new ApiResponse();
				IPagedList<v_nv_NghiPhep> iPagedList = new List<v_nv_NghiPhep>().ToList().ToPagedList(1, Utility.GetPageSize());
				apiResponse = Utility.Get_DanhSachNghiPhep<v_nv_NghiPhep>(objParameter.TUNGAY, objParameter.DENNGAY, null, objParameter.KEY, objParameter.ID_NHANVIEN);
				if (!apiResponse.Success)
				{
					base.TempData["TitleError"] = apiResponse.Message;
					return RedirectToAction("Index", "Notfound");
				}
				List<v_nv_NghiPhep> source = apiResponse.Data as List<v_nv_NghiPhep>;
				if (Utility.KiemTraQuyen("HRLeave", "AllData") && source.Count() > 0)
				{
					iPagedList = source.OrderByDescending((v_nv_NghiPhep s) => s.THOIGIANVAO).ToList().ToPagedList(1, source.Count());
				}
				else
				{
					Login_Model Login_Model = (Login_Model)base.Session["Login_Model"];
					if (Utility.KiemTraQuyen("HRLeave", "UserData") && source.Count() > 0)
					{
						iPagedList = (from s in source
									  where s.ID_NHANVIEN == Login_Model.iduser
									  orderby s.THOIGIANVAO descending
									  select s).ToList().ToPagedList(1, source.Count());
					}
				}
				base.ViewBag.PermissionCreateUser = Utility.KiemTraQuyen("HRLeave", "CreateUser");
				base.ViewBag.PermissionApproveLeave = Utility.KiemTraQuyen("HRLeave", "ApproveLeave");
				v_v_nv_NghiPhep v_v_nv_NghiPhep2 = new v_v_nv_NghiPhep();
				v_v_nv_NghiPhep2.IPagedList = iPagedList;
				v_v_nv_NghiPhep2.lstdm_NhanVien = new List<ComboboxFrom>();
				v_v_nv_NghiPhep2.lstdm_NhanVien = Utility.GetListData<ComboboxFrom>("User").Data as List<ComboboxFrom>;
				v_v_nv_NghiPhep2.lstnv_PhepNam = new List<ComboboxFrom>();
				v_v_nv_NghiPhep2.TUNGAY = (objParameter.TUNGAY.HasValue ? objParameter.TUNGAY.Value : Utility.CurrentTime);
				v_v_nv_NghiPhep2.DENNGAY = (objParameter.DENNGAY.HasValue ? objParameter.DENNGAY.Value : Utility.CurrentTime);
				if ((!base.ViewBag.PermissionCreateUser))
				{
					v_v_nv_NghiPhep2.ID_NHANVIEN = base.Session["idUser"].ToString();
				}
				else
				{
					v_v_nv_NghiPhep2.ID_NHANVIEN = objParameter.ID_NHANVIEN;
				}
				base.ViewBag.PermissionEdit = Utility.KiemTraQuyen("HRLeave", "Edit");
				base.ViewBag.PermissionDelete = Utility.KiemTraQuyen("HRLeave", "Delete");
				base.ViewBag.PermissionCreate = Utility.KiemTraQuyen("HRLeave", "Create");
				if ((!base.ViewBag.PermissionCreateUser))
				{
					v_v_nv_NghiPhep2.lstdm_NhanVien = v_v_nv_NghiPhep2.lstdm_NhanVien.Where((ComboboxFrom s) => s.ID == base.Session["idUser"].ToString()).ToList();
				}
				return View(v_v_nv_NghiPhep2);
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
				if (!Utility.KiemTraQuyen("HRLeave", "Create"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				base.ViewBag.PermissionCreateUser = Utility.KiemTraQuyen("HRLeave", "CreateUser");
				v_v_nv_NghiPhep v_v_nv_NghiPhep2 = new v_v_nv_NghiPhep();
				v_v_nv_NghiPhep2.LOC_ID = Utility.LOC_ID;
				v_v_nv_NghiPhep2.ID_NGUOITAO = base.Session["idUser"].ToString();
				v_v_nv_NghiPhep2.THOIGIANTHEM = Utility.CurrentTime;
				v_v_nv_NghiPhep2.ID_NHANVIEN = base.Session["idUser"].ToString();
				v_v_nv_NghiPhep2.ID = Guid.NewGuid().ToString();
				v_v_nv_NghiPhep2.lstdm_NhanVien = new List<ComboboxFrom>();
				v_v_nv_NghiPhep2.lstnv_PhepNam = new List<ComboboxFrom>();
				base.ViewBag.PermissionCreateUser = Utility.KiemTraQuyen("HRLeave", "CreateUser");
				return View(v_v_nv_NghiPhep2);
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
		public ActionResult Create([Bind(Include = "LOC_ID,ID,ID_NHANVIEN,ID_THANGLUONG,THOIGIANVAO,THOIGIANRA,GHICHU,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO,ISNGHIPHEP,SOLUONG,ISDUYETPHEP,THOIGIANDUYETPHEP,ID_NGUOIDUYETPHEP,HINHTHUCNGHIPHEP,ID_PHEPNAM")] v_nv_NghiPhep nv_NghiPhep)
		{
			try
			{
				if (Utility.KiemTra())
				{
					return RedirectToAction("Index", "Admin");
				}
				if (!Utility.KiemTraQuyen("HRLeave", "Create"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				if (base.ModelState.IsValid)
				{
					nv_NghiPhep.LOC_ID = Utility.LOC_ID;
					nv_NghiPhep.ID_NGUOITAO = base.Session["idUser"].ToString();
					nv_NghiPhep.THOIGIANTHEM = Utility.CurrentTime;
					ApiResponse apiResponse = Utility.Create((nv_NghiPhep)nv_NghiPhep, "HRLeave");
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
				return View(nv_NghiPhep);
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
				if (!Utility.KiemTraQuyen("HRLeave", "Edit"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				v_v_nv_NghiPhep nv_NghiPhep2 = new v_v_nv_NghiPhep();
				if (!string.IsNullOrEmpty(id))
				{
					apiResponse = Utility.GetDetail<v_nv_NghiPhep>(Utility.LOC_ID + "/" + id, "HRLeave");
					if (!apiResponse.Success)
					{
						base.TempData["TitleError"] = apiResponse.Message;
						return RedirectToAction("Index", "Notfound");
					}
					if (apiResponse.Data != null)
					{
						nv_NghiPhep2 = apiResponse.Data as v_v_nv_NghiPhep;
					}
				}
				nv_NghiPhep2.lstdm_NhanVien = new List<ComboboxFrom>();
				nv_NghiPhep2.lstdm_NhanVien = Utility.GetListData<ComboboxFrom>("User").Data as List<ComboboxFrom>;
				nv_NghiPhep2.lstdm_NhanVien = nv_NghiPhep2.lstdm_NhanVien.Where((ComboboxFrom s) => s.ID == nv_NghiPhep2.ID_NHANVIEN).ToList();
				return View(nv_NghiPhep2);
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
		public ActionResult Edit([Bind(Include = "LOC_ID,ID,ID_NHANVIEN,ID_THANGLUONG,THOIGIANVAO,THOIGIANRA,GHICHU,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO,ISNGHIPHEP,SOLUONG,ISDUYETPHEP,THOIGIANDUYETPHEP,ID_NGUOIDUYETPHEP,HINHTHUCNGHIPHEP,ID_PHEPNAM")] v_nv_NghiPhep nv_NghiPhep)
		{
			try
			{
				if (Utility.KiemTra())
				{
					return RedirectToAction("Index", "Admin");
				}
				if (!Utility.KiemTraQuyen("HRLeave", "Edit"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				if (base.ModelState.IsValid)
				{
					nv_NghiPhep.LOC_ID = Utility.LOC_ID;
					nv_NghiPhep.ID_NGUOISUA = base.Session["idUser"].ToString();
					nv_NghiPhep.THOIGIANSUA = Utility.CurrentTime;
					ApiResponse apiResponse = Utility.Edit(Utility.LOC_ID + "/" + nv_NghiPhep.ID, nv_NghiPhep, "HRLeave");
					if (apiResponse.Success)
					{
						return RedirectToAction("Index");
					}
					base.ModelState.AddModelError(string.Empty, apiResponse.Message);
				}
				return View(nv_NghiPhep);
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
				if (!Utility.KiemTraQuyen("HRLeave", "Delete"))
				{
					base.TempData["TitleError"] = "Bạn không có quyền truy cập chức năng!";
					return RedirectToAction("Index", "Notfound");
				}
				ApiResponse apiResponse = Utility.Delete<v_nv_NghiPhep>(Utility.LOC_ID + "/" + id, "HRLeave");
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
				if (!Utility.KiemTraQuyen("HRLeave", "Create"))
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
				base.ViewBag.PermissionCreateUser = Utility.KiemTraQuyen("HRLeave", "CreateUser");
				v_v_nv_NghiPhep v_v_nv_NghiPhep2 = new v_v_nv_NghiPhep();
				apiResponse.Success = true;
				v_v_nv_NghiPhep2.LOC_ID = Utility.LOC_ID;
				v_v_nv_NghiPhep2.ID = Guid.NewGuid().ToString();
				v_v_nv_NghiPhep2.HINHTHUCNGHIPHEP = 0;
				v_v_nv_NghiPhep2.THOIGIANVAO = Utility.CurrentTime;
				v_v_nv_NghiPhep2.THOIGIANRA = Utility.CurrentTime;
				v_v_nv_NghiPhep2.lstdm_NhanVien = new List<ComboboxFrom>();
				List<ComboboxFrom> list = Utility.GetListData<ComboboxFrom>("User").Data as List<ComboboxFrom>;
				foreach (ComboboxFrom item in list)
				{
					item.ISACTIVE = true;
					if (item.ID == base.Session["idUser"].ToString())
					{
						item.ISDEFAULT = true;
					}
				}
				if ((!base.ViewBag.PermissionCreateUser))
				{
					List<v_nv_PhepNam> source = ((!(Utility.GetListData<v_nv_PhepNam>("AnnualLeave").Data is List<v_nv_PhepNam> source2)) ? new List<v_nv_PhepNam>() : source2.Where((v_nv_PhepNam s) => s.ID_NHANVIEN == base.Session["idUser"].ToString()).ToList());
					bool flag = false;
					List<ComboboxFrom> list2 = new List<ComboboxFrom>();
					foreach (v_nv_PhepNam item2 in source.OrderBy((v_nv_PhepNam s) => s.NAM))
					{
						if ((item2.SONGAYPHEP - item2.SONGAYPHEPDADUNG > 0.0 || item2.NAM == (double)Utility.CurrentTime.Year) && item2.NGAYBATDAU <= Utility.CurrentTime && item2.NGAYKETTHUC >= Utility.CurrentTime)
						{
							ComboboxFrom comboboxFrom = new ComboboxFrom();
							comboboxFrom.ID = item2.ID;
							comboboxFrom.NAME = item2.NAM + "(" + (item2.SONGAYPHEP - item2.SONGAYPHEPDADUNG) + " ngày)";
							comboboxFrom.ISACTIVE = true;
							if (flag)
							{
								bool flag2 = (comboboxFrom.ISDEFAULT = true);
								flag = flag2;
							}
							list2.Add(comboboxFrom);
						}
					}
					v_v_nv_NghiPhep2.lstnv_PhepNam = list2;
					v_v_nv_NghiPhep2.lstdm_NhanVien = list.Where((ComboboxFrom s) => s.ID == base.Session["idUser"].ToString()).ToList();
				}
				else
				{
					v_v_nv_NghiPhep2.lstdm_NhanVien = list;
				}
				apiResponse.Detail = Utility.ConvertobjectTo(v_v_nv_NghiPhep2);
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
		[ValidateAntiForgeryToken]
		public ActionResult CreatePopup([Bind(Include = "LOC_ID,ID,ID_NHANVIEN,ID_THANGLUONG,THOIGIANVAO,THOIGIANRA,GHICHU,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO,ISNGHIPHEP,SOLUONG,ISDUYETPHEP,THOIGIANDUYETPHEP,ID_NGUOIDUYETPHEP,HINHTHUCNGHIPHEP,ID_PHEPNAM")] v_nv_NghiPhep nv_NghiPhep)
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
				if (!Utility.KiemTraQuyen("HRLeave", "Create"))
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
				if (nv_NghiPhep.HINHTHUCNGHIPHEP == 0 && nv_NghiPhep.THOIGIANVAO > nv_NghiPhep.THOIGIANRA)
				{
					base.ModelState.AddModelError("THOIGIANRA", "Sai thời gian ra");
				}
				if (base.ModelState.IsValid)
				{
					TimeSpan timeSpan = nv_NghiPhep.THOIGIANRA - nv_NghiPhep.THOIGIANVAO;
					if (nv_NghiPhep.HINHTHUCNGHIPHEP == 0)
					{
						nv_NghiPhep.SOLUONG = timeSpan.Days + 1;
					}
					else
					{
						nv_NghiPhep.THOIGIANRA = nv_NghiPhep.THOIGIANVAO;
						nv_NghiPhep.SOLUONG = 0.5;
					}
					nv_NghiPhep.LOC_ID = Utility.LOC_ID;
					nv_NghiPhep.ID_NGUOITAO = base.Session["idUser"].ToString();
					nv_NghiPhep.THOIGIANTHEM = Utility.CurrentTime;
					apiResponse = Utility.Create((nv_NghiPhep)nv_NghiPhep, "HRLeave");
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
					apiResponse.Data = Utility.GetModelState(base.ModelState, "HRLeave");
				}
				apiResponse.ID = nv_NghiPhep.ID;
				apiResponse.Detail = Utility.ConvertobjectTo(nv_NghiPhep);
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
				if (!Utility.KiemTraQuyen("HRLeave", "Edit"))
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
				v_v_nv_NghiPhep nv_NghiPhep2 = new v_v_nv_NghiPhep();
				if (!string.IsNullOrEmpty(id))
				{
					apiResponse = Utility.GetDetail<v_v_nv_NghiPhep>(Utility.LOC_ID + "/" + id, "HRLeave");
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
						nv_NghiPhep2 = apiResponse.Data as v_v_nv_NghiPhep;
					}
				}
				apiResponse.Success = true;
				nv_NghiPhep2.lstdm_NhanVien = new List<ComboboxFrom>();
				nv_NghiPhep2.lstdm_NhanVien = Utility.GetListData<ComboboxFrom>("User").Data as List<ComboboxFrom>;
				nv_NghiPhep2.lstdm_NhanVien = nv_NghiPhep2.lstdm_NhanVien.Where((ComboboxFrom s) => s.ID == nv_NghiPhep2.ID_NHANVIEN).ToList();
				List<ComboboxFrom> list = new List<ComboboxFrom>();
				List<v_nv_PhepNam> source = ((!(Utility.GetListData<v_nv_PhepNam>("AnnualLeave", "", "", Utility.LOC_ID).Data is List<v_nv_PhepNam> source2)) ? new List<v_nv_PhepNam>() : source2.Where((v_nv_PhepNam s) => s.ID_NHANVIEN == nv_NghiPhep2.ID_NHANVIEN).ToList());
				foreach (v_nv_PhepNam item in source.OrderBy((v_nv_PhepNam s) => s.NAM))
				{
					ComboboxFrom comboboxFrom = new ComboboxFrom();
					comboboxFrom.ID = item.ID;
					comboboxFrom.NAME = item.NAM + "(" + (item.SONGAYPHEP - item.SONGAYPHEPDADUNG) + " ngày)";
					comboboxFrom.ISACTIVE = true;
					list.Add(comboboxFrom);
				}
				nv_NghiPhep2.lstnv_PhepNam = list;
				apiResponse.Detail = Utility.ConvertobjectTo((nv_NghiPhep)nv_NghiPhep2, "yyyy-MM-dd HH:mm:ss");
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
		[ValidateAntiForgeryToken]
		public ActionResult EditPopup([Bind(Include = "LOC_ID,ID,ID_NHANVIEN,ID_THANGLUONG,THOIGIANVAO,THOIGIANRA,GHICHU,THOIGIANSUA,ID_NGUOISUA,THOIGIANTHEM,ID_NGUOITAO,ISNGHIPHEP,SOLUONG,ISDUYETPHEP,THOIGIANDUYETPHEP,ID_NGUOIDUYETPHEP,HINHTHUCNGHIPHEP,ID_PHEPNAM")] v_nv_NghiPhep nv_NghiPhep)
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
				if (!Utility.KiemTraQuyen("HRLeave", "Edit"))
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
				if (nv_NghiPhep.HINHTHUCNGHIPHEP == 0 && nv_NghiPhep.THOIGIANVAO > nv_NghiPhep.THOIGIANRA)
				{
					base.ModelState.AddModelError("THOIGIANRA", "Sai thời gian ra");
				}
				if (base.ModelState.IsValid)
				{
					TimeSpan timeSpan = nv_NghiPhep.THOIGIANRA - nv_NghiPhep.THOIGIANVAO;
					if (nv_NghiPhep.HINHTHUCNGHIPHEP == 0)
					{
						nv_NghiPhep.SOLUONG = timeSpan.Days + 1;
					}
					else
					{
						nv_NghiPhep.THOIGIANRA = nv_NghiPhep.THOIGIANVAO;
						nv_NghiPhep.SOLUONG = 0.5;
					}
					nv_NghiPhep.LOC_ID = Utility.LOC_ID;
					nv_NghiPhep.ID_NGUOISUA = base.Session["idUser"].ToString();
					nv_NghiPhep.THOIGIANSUA = Utility.CurrentTime;
					if (nv_NghiPhep.ISDUYETPHEP)
					{
						nv_NghiPhep.ID_NGUOIDUYETPHEP = base.Session["idUser"].ToString();
					}
					apiResponse = Utility.Edit(Utility.LOC_ID + "/" + nv_NghiPhep.ID, nv_NghiPhep, "HRLeave");
					if (apiResponse.Success)
					{
						apiResponse.ID = nv_NghiPhep.ID;
					}
					else
					{
						base.ModelState.AddModelError(string.Empty, apiResponse.Message);
					}
				}
				else
				{
					apiResponse.Success = false;
					apiResponse.Data = Utility.GetModelState(base.ModelState, "HRLeave");
				}
				apiResponse.Detail = Utility.ConvertobjectTo(nv_NghiPhep, "dd/MM/yyyy");
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
				if (!Utility.KiemTraQuyen("HRLeave", "Delete"))
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
				apiResponse = Utility.Delete<v_nv_NghiPhep>(Utility.LOC_ID + "/" + id, "HRLeave");
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

		[HttpPost]
		public ActionResult CallChangeEmployee(string id)
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
				List<v_nv_PhepNam> source = ((!(Utility.GetListData<v_nv_PhepNam>("AnnualLeave", "", "", Utility.LOC_ID).Data is List<v_nv_PhepNam> source2)) ? new List<v_nv_PhepNam>() : source2.Where((v_nv_PhepNam s) => s.ID_NHANVIEN == id).ToList());
				List<ComboboxFrom> list = new List<ComboboxFrom>();
				foreach (v_nv_PhepNam item in source.OrderBy((v_nv_PhepNam s) => s.NAM))
				{
					if ((item.SONGAYPHEP - item.SONGAYPHEPDADUNG > 0.0 || item.NAM == (double)Utility.CurrentTime.Year) && item.NGAYBATDAU <= Utility.CurrentTime && item.NGAYKETTHUC >= Utility.CurrentTime)
					{
						ComboboxFrom comboboxFrom = new ComboboxFrom();
						comboboxFrom.ID = item.ID;
						comboboxFrom.NAME = item.NAM + "(" + (item.SONGAYPHEP - item.SONGAYPHEPDADUNG) + " ngày)";
						comboboxFrom.ISACTIVE = true;
						list.Add(comboboxFrom);
					}
				}
				List<ValueEdit> list2 = new List<ValueEdit>();
				ValueEdit valueEdit = new ValueEdit();
				valueEdit.Key = "lstnv_PhepNam";
				valueEdit.Value = list;
				list2.Add(valueEdit);
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
