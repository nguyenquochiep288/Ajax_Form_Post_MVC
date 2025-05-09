using DatabaseTHP;
using MVC_QuanLyTHP.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_QuanLyTHP.Models
{
    public class v_v_ct_PhieuThu : v_ct_PhieuThu
    {
        public PagedList.IPagedList<v_ct_PhieuThu> IPagedList;
        public List<ComboboxFrom> lstdm_KhachHang { get; set; }
        public List<v_dm_LoaiPhieuThu> lstdm_LoaiPhieuThu { get; set; }
        public List<ComboboxFrom> lstdm_NhaCungCap { get; set; }
        public List<ComboboxFrom> lstdm_NhanVien { get; set; }

        public List<v_dm_TaiKhoanNganHang> lstdm_TaiKhoanNganHang { get; set; }
    }
}