using DatabaseTHP;
using MVC_QuanLyTHP.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static DatabaseTHP.Class.Uniben.Uniben;

namespace MVC_QuanLyTHP.Models
{
    public class v_v_UnibenOrderListResponse : UnibenOrderListResponse
    {
        public List<ComboboxFrom> lstdm_KhachHang { get; set; }
        public List<ComboboxFrom> lstdm_HangHoa { get; set; }

        public List<ComboboxFrom> lstAspNetUsers { get; set; }
    }
}