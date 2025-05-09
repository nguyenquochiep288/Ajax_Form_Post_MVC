using DatabaseTHP;
using DatabaseTHP.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_QuanLyTHP.Models
{
    public class v_v_web_Parameter : v_web_Parameter
    {
        public PagedList.IPagedList<v_web_Parameter> IPagedList;

        public List<ListValue> lstValue {  get; set; }
    }
}