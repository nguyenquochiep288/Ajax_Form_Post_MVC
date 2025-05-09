using DatabaseTHP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_QuanLyTHP.Models
{
    public class v_v_web_NhomQuyen : v_web_NhomQuyen
    {
        public PagedList.IPagedList<web_NhomQuyen> IPagedList;
    }
}