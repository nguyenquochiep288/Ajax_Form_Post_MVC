using DatabaseTHP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_QuanLyTHP.Models
{
    public class v_v_ThongKeCongNoNhanVien : v_ThongKeCongNoNhanVien
    {
        public List<v_ThongKeCongNoNhanVien> IPagedList { get; set; }
        public List<v_dm_NhanVien> lstdm_NhanVien { get; set; }
        public List<v_dm_PhongBan> lstdm_PhongBan { get; set; }
    }
}