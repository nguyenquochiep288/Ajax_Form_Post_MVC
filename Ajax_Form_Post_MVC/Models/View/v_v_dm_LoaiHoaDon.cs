using System.Collections.Generic;
using DatabaseTHP;
using MVC_QuanLyTHP.Class;
using PagedList;

namespace MVC_QuanLyTHP.Models
{

	public class v_v_dm_LoaiHoaDon : v_dm_LoaiHoaDon
	{
		public IPagedList<v_dm_LoaiHoaDon> IPagedList;

		public List<ComboboxFrom> lstDanhSachMau { get; set; }
	}
}
