using DatabaseTHP;
using MVC_QuanLyTHP.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_QuanLyTHP.Models
{
    public class v_v_dm_ThangLuong : v_dm_ThangLuong
    {
        public PagedList.IPagedList<v_dm_ThangLuong> IPagedList;
    }
}