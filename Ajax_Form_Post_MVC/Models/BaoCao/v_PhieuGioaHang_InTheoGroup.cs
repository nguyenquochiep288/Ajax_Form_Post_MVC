using DatabaseTHP;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVC_QuanLyTHP.Models
{
    public class v_PhieuGioaHang_InTheoGroup 
    {
        public string ID_KHACHHANG { get; set; }
        public string MAPHIEU_GROUP { get; set; }
        public string NAME_GROUP { get; set; }
        public string MA_HANGHOA { get; set; }
        public string NAME_HANGHOA { get; set; }
        public string NAME_DVT { get; set; }
        public string NAME_DVT_QD { get; set; }
        public string MAPHIEUXUAT { get; set; }

        public decimal TONGTRONGLUONG { get; set; }
        public double TYLE_QD { get; set; }
        public decimal TONGSOLUONG { get; set; }

        public double SOLUONG { get; set; }
        public double DONGIA { get; set; }
        public double CHIETKHAU { get; set; }
        public double TONGTIENGIAMGIA { get; set; }
        public double THANHTIEN { get; set; }
        public double THUESUAT { get; set; }
        public double TONGTIENVAT { get; set; }
        public double TONGCONG { get; set; }
        public double TONGTIENTINHTHUE { get; set; }
        public string NAME_SOLUONG
        {
            get
            {
                int PhanNguyen = 0;
                if (TYLE_QD == 0)
                    TYLE_QD = 1;
                PhanNguyen = Convert.ToInt32(TONGSOLUONG) / Convert.ToInt32(TYLE_QD);
                //(TYLE_QD > 1 ? "("+ TONGSOLUONG + " " + NAME_DVT_QD  + ") " : "") +
                return (PhanNguyen > 0 ? PhanNguyen.ToString("N0") + " " + NAME_DVT : "") + ((TONGSOLUONG - Convert.ToDecimal(PhanNguyen * TYLE_QD)) > 0 ? (" " + (TONGSOLUONG - Convert.ToDecimal(PhanNguyen * TYLE_QD)).ToString("N0") + " " + NAME_DVT_QD) : "");
            }
        }

        public string KHUVUC { get; set; }
        public string THONGTINTHEM { get; set; }
    }
}