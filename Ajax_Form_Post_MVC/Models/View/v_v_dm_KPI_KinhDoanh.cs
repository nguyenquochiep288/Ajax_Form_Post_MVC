using DatabaseTHP;
using DatabaseTHP.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_QuanLyTHP.Models
{
    public class v_v_dm_KPI_KinhDoanh : v_view_dm_KPI_KinhDoanh
    {
        public PagedList.IPagedList<v_dm_KPI_KinhDoanh> IPagedList;
        public List<v_dm_DonViTinh> lstdm_DonViTinh { get; set; }
      
    }
}