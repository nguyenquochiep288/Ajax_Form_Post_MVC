using DatabaseTHP;
using DatabaseTHP.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_QuanLyTHP.Models
{
    public class v_v_dm_BangLuong : v_dm_BangLuong
    {
        public PagedList.IPagedList<v_dm_BangLuong> IPagedList;

        public List<v_dm_PhongBan> lstdm_PhongBan { get; set; }
    }
}