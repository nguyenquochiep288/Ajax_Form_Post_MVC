using DatabaseTHP;
using DatabaseTHP.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static DatabaseTHP.Class.API;

namespace MVC_QuanLyTHP.Models
{
    public class v_v_dm_NhanVien : v_dm_NhanVien
    {
	    public PagedList.IPagedList<v_dm_NhanVien> IPagedList;
        public List<v_dm_ChucVu> lstdm_ChucVu { get; set; }
        public List<v_dm_PhongBan> lstdm_PhongBan { get; set; }

        public List<v_AspNetUsers> lstAspNetUsers { get; set; }

        public List<LoaiHangHoa> lstGioiTinh
        {
            get { return API.lstGioiTinh(); }
        }
    }
}