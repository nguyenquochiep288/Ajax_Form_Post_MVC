using DatabaseTHP;
using MVC_QuanLyTHP.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_QuanLyTHP.Models
{
    public class v_v_nv_ChamCong : v_nv_ChamCong
    {
        public PagedList.IPagedList<v_nv_ChamCong> IPagedList;
        public List<ComboboxFrom> lstdm_NhanVien { get; set; }
        public List<dm_PhongBan> lstdm_PhongBan { get; set; }
        public DateTime TUNGAY { get; set; }
        public DateTime DENNGAY { get; set; }
        public string ID_PHONGBAN { get; set; }

        public List<v_nv_ChamCong> lstnv_ChamCong_Table { get; set; }
        public List<v_dm_NhanVien> lstdm_NhanVien_Table { get; set; }
        public List<dm_ThangLuong> lstdm_ThangLuong_Table { get; set; }
        public List<v_nv_NghiPhep> lstnv_NghiPhep_Table { get; set; }
    }
}