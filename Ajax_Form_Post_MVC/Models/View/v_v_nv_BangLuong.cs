using DatabaseTHP;
using DatabaseTHP.StoredProcedure;
using MVC_QuanLyTHP.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_QuanLyTHP.Models
{
    public class v_v_nv_BangLuong : v_nv_BangLuong
    {
        public PagedList.IPagedList<v_nv_BangLuong> IPagedList;
        public List<ComboboxFrom> lstdm_NhanVien { get; set; }
        public List<ComboboxFrom> lstdm_ThangLuong { get; set; }

        public string BUTTONTYPE { get; set; }
    }
}