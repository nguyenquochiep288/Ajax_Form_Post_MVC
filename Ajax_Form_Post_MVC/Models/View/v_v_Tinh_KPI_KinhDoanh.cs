using DatabaseTHP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_QuanLyTHP.Models
{
    public class v_v_Tinh_KPI_KinhDoanh : v_Tinh_KPI_KinhDoanh
    {
        public PagedList.IPagedList<v_Tinh_KPI_KinhDoanh> IPagedList;
        public List<v_web_NhomQuyen> lstweb_NhomQuyen { get; set; }
        public List<v_dm_NhanVien> lstdm_NhanVien { get; set; }
        public string ID_NHOMQUYEN { get; set; }

        public DateTime TUNGAY { get; set; }

        public DateTime DENNGAY { get; set; }

    }
}