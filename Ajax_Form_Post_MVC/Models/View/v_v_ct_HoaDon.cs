using System.Collections.Generic;
using DatabaseTHP;
using MVC_QuanLyTHP.Class;
using PagedList;

namespace MVC_QuanLyTHP.Models
{

	public class v_v_ct_HoaDon : v_ct_HoaDon
	{
		public IPagedList<v_ct_HoaDon> IPagedList;

		public List<ComboboxFrom> lstdm_KhachHang { get; set; }

		public List<v_dm_LoaiHoaDon> lstdm_LoaiHoaDon { get; set; }

		public List<v_dm_ThueSuat> lstdm_ThueSuat { get; set; }

		public List<ComboboxFrom> lstdm_HTTT { get; set; }

		public List<v_dm_TienTe> lstdm_TienTe { get; set; }

		public string myModalAdd { get; set; }
	}
}
