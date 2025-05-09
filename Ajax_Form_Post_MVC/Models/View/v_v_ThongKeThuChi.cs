using DatabaseTHP;
using DatabaseTHP.StoredProcedure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_QuanLyTHP.Models
{
    public class v_v_ThongKeThuChi : Sp_Get_ThongKeThuChi_Result
    {
        public List<Sp_Get_ThongKeThuChi_Result> IPagedList { get; set; }


    }
}