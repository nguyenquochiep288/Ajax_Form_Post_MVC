using System.Collections.Generic;
using DatabaseTHP;
using MVC_QuanLyTHP.Class;
using PagedList;

namespace MVC_QuanLyTHP.Models
{

	public class v_v_ct_PhieuXuat : v_ct_PhieuXuat
	{
		public IPagedList<v_ct_PhieuXuat> IPagedList;

		public List<v_dm_LoaiHoaDon> lstdm_LoaiHoaDon { get; set; }

		public List<ComboboxFrom> lstdm_KhachHang { get; set; }

		public List<ComboboxFrom> lstdm_NhaCungCap { get; set; }

		public List<v_dm_Kho> lstdm_Kho { get; set; }

		public List<v_dm_LoaiPhieuXuat> lstdm_LoaiPhieuXuat { get; set; }

		public List<ComboboxFrom> lstdm_NhanVien { get; set; }

		public List<v_dm_ThueSuat> lstdm_ThueSuat { get; set; }

		public string BUTTONTYPE { get; set; }
	}
}