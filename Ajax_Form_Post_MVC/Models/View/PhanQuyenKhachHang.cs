using DatabaseTHP;
using MVC_QuanLyTHP.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_QuanLyTHP.Models
{
    public class PhanQuyenKhachHang
    {
        public List<v_v_web_NhomQuyen> lstNhomQuyen { get; set; }
        public List<v_v_dm_LichLamViec> lstLichLamViec { get; set; }
    }
}