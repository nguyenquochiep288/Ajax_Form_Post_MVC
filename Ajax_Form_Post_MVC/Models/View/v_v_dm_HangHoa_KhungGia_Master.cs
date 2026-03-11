using System.Collections.Generic;
using DatabaseTHP;
using PagedList;

namespace MVC_QuanLyTHP.Models
{

	public class v_v_dm_HangHoa_KhungGia_Master : v_dm_HangHoa_KhungGia_Master
	{
		public IPagedList<v_dm_HangHoa_KhungGia_Master> IPagedList;

		public List<v_dm_HangHoa> lstdm_HangHoa { get; set; }

		public string ID_HANGHOA { get; set; }
	}
}
