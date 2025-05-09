using DatabaseTHP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_QuanLyTHP.Models
{
    public class v_v_ThongKeTonKhoHangHoa : v_ThongKeTonKhoHangHoa
    {
        public List<v_ThongKeTonKhoHangHoa> IPagedList { get; set; }
        public List<v_dm_HangHoa> lstdm_HangHoa { get; set; }
        public List<v_dm_Kho> lstdm_Kho { get; set; }
        public List<v_dm_NhomHangHoa> lstdm_NhomHangHoa { get; set; }
    }
}