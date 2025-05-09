using DatabaseTHP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_QuanLyTHP.Models
{
    public class v_v_ct_PhieuGiaoHang_HinhAnh : v_ct_PhieuGiaoHang_HinhAnh
    {
        public PagedList.IPagedList<ct_PhieuGiaoHang_HinhAnh> IPagedList;
    }
}