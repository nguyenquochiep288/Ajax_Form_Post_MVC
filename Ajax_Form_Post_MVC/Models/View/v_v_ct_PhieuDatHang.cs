using DatabaseTHP;
using MVC_QuanLyTHP.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_QuanLyTHP.Models
{
    public class v_v_ct_PhieuDatHang : v_ct_PhieuDatHang
    {
        public PagedList.IPagedList<v_ct_PhieuDatHang> IPagedList;
        public List<ComboboxFrom> lstdm_KhachHang { get; set; }
        public List<v_dm_NhaCungCap> lstdm_NhaCungCap { get; set; }
        public List<v_dm_Kho> lstdm_Kho { get; set; }
        public List<v_AspNetUsers> lstAspNetUsers { get; set; }
        public List<v_dm_ThueSuat> lstdm_ThueSuat { get; set; }

        public List<v_dm_KhuVuc> lstdm_KhuVuc { get; set; }
        public string BUTTONTYPE { get; set; }
        public string ID_KHUVUC { get; set; }

    }
}