using DatabaseTHP;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVC_QuanLyTHP.Models
{
    public class v_v_AspNetUsers : v_AspNetUsers
    {
		public PagedList.IPagedList<v_AspNetUsers> IPagedList;
        public List<web_NhomQuyen> lstweb_NhomQuyen { get; set; }
    }
}