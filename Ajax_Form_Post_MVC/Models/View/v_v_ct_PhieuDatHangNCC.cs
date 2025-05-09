using DatabaseTHP;
using MVC_QuanLyTHP.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_QuanLyTHP.Models
{
    public class v_v_ct_PhieuDatHangNCC : v_ct_PhieuDatHangNCC
    {
        public PagedList.IPagedList<v_ct_PhieuDatHangNCC> IPagedList;
        public List<ComboboxFrom> lstdm_NhaCungCap { get; set; }
        public List<v_dm_Kho> lstdm_Kho { get; set; }
        public List<v_dm_LoaiPhieuNhap> lstdm_LoaiPhieuNhap { get; set; }
        public List<ComboboxFrom> lstdm_NhanVien { get; set; }

    }
}