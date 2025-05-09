using DatabaseTHP;
using MVC_QuanLyTHP.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_QuanLyTHP.Models
{
    public class v_v_ct_PhieuNhap : v_ct_PhieuNhap
    {
        public PagedList.IPagedList<v_ct_PhieuNhap> IPagedList;
        public List<ComboboxFrom> lstdm_NhaCungCap { get; set; }
        public List<ComboboxFrom> lstdm_KhachHang { get; set; }
        public List<v_dm_Kho> lstdm_Kho { get; set; }
        public List<v_dm_LoaiPhieuNhap> lstdm_LoaiPhieuNhap { get; set; }
        public List<ComboboxFrom> lstdm_NhanVien { get; set; }

    }
}