using DatabaseTHP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_QuanLyTHP.Models
{
    public class v_v_view_dm_ChuongTrinhKhuyenMai : v_view_dm_ChuongTrinhKhuyenMai
    {
        public PagedList.IPagedList<view_dm_ChuongTrinhKhuyenMai> IPagedList;

        public List<v_dm_DonViTinh> lstdm_DonViTinh { get; set; }
    }
}