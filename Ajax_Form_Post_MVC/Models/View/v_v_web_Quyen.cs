using DatabaseTHP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_QuanLyTHP.Models
{
    public class v_v_web_Quyen : v_web_Quyen
    {
		public PagedList.IPagedList<v_web_Quyen> IPagedList;
		public List<v_web_Menu> lstweb_Menu { get; set; }
    }
}