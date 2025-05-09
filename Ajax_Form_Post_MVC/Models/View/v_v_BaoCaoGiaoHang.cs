using DatabaseTHP;
using DatabaseTHP.StoredProcedure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_QuanLyTHP.Models
{
    public class v_v_BaoCaoGiaoHang
    {
        public List<Sp_Get_BaoCaoGiaoHang_Result> IPagedList { get; set; }
        public List<v_dm_Xe> lstdm_Xe { get; set; }
        public DateTime TUNGAY { get; set; }
        public DateTime DENNGAY { get; set; }
        public string ID_XE { get; set; }
    }
}