// MVC_QuanLyTHP, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVC_QuanLyTHP.Models.v_Sp_Get_DanhSachPhieuXuat_ChiTiet_Result
using System;
using System.Collections.Generic;
using DatabaseTHP;
using DatabaseTHP.StoredProcedure;
namespace MVC_QuanLyTHP.Models
{
    public class v_Sp_Get_DanhSachPhieuXuat_ChiTiet_Result : Sp_Get_DanhSachPhieuXuat_ChiTiet_Result
    {
        public List<Sp_Get_DanhSachPhieuXuat_ChiTiet_Result> IPagedList { get; set; }

        public List<v_dm_KhachHang> lstdm_KhachHang { get; set; }

        public List<v_dm_KhuVuc> lstdm_KhuVuc { get; set; }

        public List<v_dm_NhomKhachHang> lstdm_NhomKhachHang { get; set; }

        public bool ISTHEOTHOIGIAN { get; set; }

        public DateTime TUNGAY { get; set; }

        public DateTime DENNGAY { get; set; }

        public string ID_KHACHHANG { get; set; }

        public string ID_NHOMKHACHHANG { get; set; }

        public string ID_KHUVUC { get; set; }

        public string KEY { get; set; }
    }
}
