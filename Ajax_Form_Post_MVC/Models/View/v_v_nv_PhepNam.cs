using DatabaseTHP;
using DatabaseTHP.Class;
using MVC_QuanLyTHP.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static DatabaseTHP.Class.API;

namespace MVC_QuanLyTHP.Models
{
    public class v_v_nv_PhepNam : v_nv_PhepNam
    {
        public PagedList.IPagedList<v_nv_PhepNam> IPagedList;
        public List<ComboboxFrom> lstdm_NhanVien { get; set; }

        
    }
}