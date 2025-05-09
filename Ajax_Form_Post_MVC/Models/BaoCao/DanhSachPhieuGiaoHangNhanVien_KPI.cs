using DatabaseTHP;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVC_QuanLyTHP.Models
{
    public class DanhSachPhieuGiaoHangNhanVien_KPI
    {
        public string NAME_GROUP { get; set; }
        public string NAME_NHANVIEN { get; set; }
        public string MA_NHANVIEN { get; set; }
        public decimal SOLUONG_TRAHANG { get; set; }
        public decimal SOLUONG_GIAOHANG { get; set; }
        public decimal SOLUONG_DONHANG { get; set; }
        public decimal TONGTIEN { get; set; }
    }
}