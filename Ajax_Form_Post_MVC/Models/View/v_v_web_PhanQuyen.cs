using DatabaseTHP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_QuanLyTHP.Models
{
    public class v_v_web_PhanQuyen : v_web_PhanQuyen
    {
        public PagedList.IPagedList<web_PhanQuyen> IPagedList;
        public List<web_NhomQuyen> lstweb_NhomQuyen = new List<web_NhomQuyen>();
        public List<web_PhanQuyen> lstweb_PhanQuyen = new List<web_PhanQuyen>();

    }
}