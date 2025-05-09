using DatabaseTHP;
using MVC_QuanLyTHP.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_QuanLyTHP.Models
{
    public class v_v_ct_PhieuChi : v_ct_PhieuChi
    {
        public PagedList.IPagedList<v_ct_PhieuChi> IPagedList;
        public List<ComboboxFrom> lstdm_KhachHang { get; set; }
        public List<v_dm_LoaiPhieuChi> lstdm_LoaiPhieuChi { get; set; }
        public List<ComboboxFrom> lstdm_NhaCungCap { get; set; }
        public List<ComboboxFrom> lstdm_NhanVien { get; set; }
        public List<ComboboxFrom> lstdm_Xe { get; set; }
        public List<v_dm_TaiKhoanNganHang> lstdm_TaiKhoanNganHang { get; set; }
    }
}