using DatabaseTHP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_QuanLyTHP.Models
{
    public class v_v_ThongKeQuyTien : v_ThongKeQuyTien
    {
        public List<v_ThongKeQuyTien> IPagedList { get; set; }
        public List<v_dm_TaiKhoanNganHang> lstdm_TaiKhoanNganHang { get; set; }
    }
}