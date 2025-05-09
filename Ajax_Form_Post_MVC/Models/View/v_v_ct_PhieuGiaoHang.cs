using DatabaseTHP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_QuanLyTHP.Models
{
    public class v_v_ct_PhieuGiaoHang : v_ct_PhieuGiaoHang
    {
        public PagedList.IPagedList<v_ct_PhieuGiaoHang> IPagedList;
        public List<v_dm_Xe> lstdm_Xe { get; set; }

        public List<v_dm_KhuVuc> lstdm_KhuVuc { get; set; }

        public string ID_KHUVUC { get; set; }
    }
}