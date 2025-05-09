using DatabaseTHP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_QuanLyTHP.Models
{
    public class v_v_dm_PhongBan : v_dm_PhongBan
    {
	    public PagedList.IPagedList<v_dm_PhongBan> IPagedList;
        public List<v_dm_PhongBan> lstdm_PhongBan { get; set; }
    }
}