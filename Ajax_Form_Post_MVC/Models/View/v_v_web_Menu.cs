using DatabaseTHP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_QuanLyTHP.Models
{
    public class v_v_web_Menu : v_web_Menu
    {
		public PagedList.IPagedList<v_web_Menu> IPagedList;
        public List<v_web_Menu> lstweb_Menu { get; set; }
    }
}