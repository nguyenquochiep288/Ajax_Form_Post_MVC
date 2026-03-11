using DatabaseTHP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_QuanLyTHP.Models
{
    public class v_v_view_dm_HangHoa_KhungGia : v_view_dm_HangHoa_KhungGia
    {
        public PagedList.IPagedList<v_view_dm_HangHoa_KhungGia> IPagedList;
        public List<v_dm_HangHoa> lstdm_HangHoa { get; set; }
        public List<v_view_dm_HangHoa_KhungGia> lstdm_HangHoa_KhungGia { get; set; }
    }
}