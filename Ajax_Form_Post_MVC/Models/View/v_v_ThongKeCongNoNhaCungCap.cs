using DatabaseTHP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_QuanLyTHP.Models
{
    public class v_v_ThongKeCongNoNhaCungCap : v_ThongKeCongNoNhaCungCap
    {
        public List<v_ThongKeCongNoNhaCungCap> IPagedList { get; set; }
        public List<v_dm_NhaCungCap> lstdm_NhaCungCap { get; set; }
        public List<v_dm_NhomNhaCungCap> lstdm_NhomNhaCungCap { get; set; }
    }
}