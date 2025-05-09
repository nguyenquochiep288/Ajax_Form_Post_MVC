using DatabaseTHP;
using DatabaseTHP.StoredProcedure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_QuanLyTHP.Models
{
    public class v_v_ThongKeTheoNhanVien: Sp_Get_BaoCaoTheoNhanVien_Result
    {
        public List<Sp_Get_BaoCaoTheoNhanVien_Result> IPagedList { get; set; }
        public List<v_AspNetUsers> lstdm_NhanVien { get; set; }

       
    }
}