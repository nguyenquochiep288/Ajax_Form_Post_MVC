using DatabaseTHP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_QuanLyTHP.Models
{
    public class v_v_ThongKeCongNoKhachHang : v_ThongKeCongNoKhachHang
    {
        public List<v_ThongKeCongNoKhachHang> IPagedList { get; set; }
        public List<v_dm_KhachHang> lstdm_KhachHang { get; set; }
        public List<v_dm_KhuVuc> lstdm_KhuVuc { get; set; }
        public List<v_dm_NhomKhachHang> lstdm_NhomKhachHang { get; set; }
    }
}