using DatabaseTHP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_QuanLyTHP.Models
{
    public class v_v_dm_NhaCungCap : v_dm_NhaCungCap
    {
	    public PagedList.IPagedList<v_dm_NhaCungCap> IPagedList;
        public List<v_dm_NhomNhaCungCap> lstdm_NhomNhaCungCap { get; set; }
    }
}