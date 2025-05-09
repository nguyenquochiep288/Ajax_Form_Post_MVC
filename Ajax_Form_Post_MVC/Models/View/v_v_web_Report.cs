using DatabaseTHP;
using DatabaseTHP.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_QuanLyTHP.Models
{
    public class v_v_web_Report : v_web_Report
    {
        public PagedList.IPagedList<v_web_Report> IPagedList;

        public List<v_web_Menu> lstweb_Menu { get; set; }

        public List<ListValue> lstValue { get; set; }

        public string ListText { get; set; }

        public string ListValue { get; set; }
    }
}