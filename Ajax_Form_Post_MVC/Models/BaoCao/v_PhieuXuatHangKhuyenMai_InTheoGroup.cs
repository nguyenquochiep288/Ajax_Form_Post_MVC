using DatabaseTHP;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVC_QuanLyTHP.Models
{
    public class v_PhieuXuatHangKhuyenMai_InTheoGroup
    {
        public string NAME_GROUP { get; set; }
        public string MA_HANGHOA { get; set; }
        public string NAME_HANGHOA { get; set; }
        public string NAME_DVT { get; set; }
        public string NAME_DVT_QD { get; set; }
        public double TYLE_QD { get; set; }
        public decimal TONGSOLUONG { get; set; }
        public decimal TONGTIENGIAMGIA { get; set; }
        public decimal TONGSOLUONG_TRAHANG { get; set; }
        public decimal TONGTIENGIAMGIA_TRAHANG { get; set; }
        public string NAME_SOLUONG
        {
            get
            {
                int PhanNguyen = 0;
                if (TYLE_QD == 0)
                    TYLE_QD = 1;
                PhanNguyen = Convert.ToInt32(TONGSOLUONG) / Convert.ToInt32(TYLE_QD);
                return (PhanNguyen > 0 ? PhanNguyen.ToString("N0") + " " + NAME_DVT : "") + ((TONGSOLUONG - Convert.ToDecimal(PhanNguyen * TYLE_QD)) > 0 ? (" " + (TONGSOLUONG - Convert.ToDecimal(PhanNguyen * TYLE_QD)).ToString("N0") + " " + NAME_DVT_QD) : "");
            }
        }

        public string NAME_SOLUONG_TRAHANG
        {
            get
            {
                int PhanNguyen = 0;
                if (TYLE_QD == 0)
                    TYLE_QD = 1;
                PhanNguyen = Convert.ToInt32(TONGSOLUONG_TRAHANG) / Convert.ToInt32(TYLE_QD);
                return (PhanNguyen > 0 ? PhanNguyen.ToString("N0") + " " + NAME_DVT : "") + ((TONGSOLUONG_TRAHANG - Convert.ToDecimal(PhanNguyen * TYLE_QD)) > 0 ? (" " + (TONGSOLUONG_TRAHANG - Convert.ToDecimal(PhanNguyen * TYLE_QD)).ToString("N0") + " " + NAME_DVT_QD) : "");
            }
        }
        public decimal TONGSOLUONG_CONLAI
        {
            get
            {
                return (TONGSOLUONG - TONGSOLUONG_TRAHANG);
            }
        }
        public decimal TONGTIENGIAMGIA_CONLAI
        {
            get
            {
                return (TONGTIENGIAMGIA - TONGTIENGIAMGIA_TRAHANG);
            }
        }
        public string NAME_SOLUONG_CONLAI
        {
            get
            {
                int PhanNguyen = 0;
                if (TYLE_QD == 0)
                    TYLE_QD = 1;
                PhanNguyen = Convert.ToInt32(TONGSOLUONG_CONLAI) / Convert.ToInt32(TYLE_QD);
                return (PhanNguyen > 0 ? PhanNguyen.ToString("N0") + " " + NAME_DVT : "") + ((TONGSOLUONG_CONLAI - Convert.ToDecimal(PhanNguyen * TYLE_QD)) > 0 ? (" " + (TONGSOLUONG_CONLAI - Convert.ToDecimal(PhanNguyen * TYLE_QD)).ToString("N0") + " " + NAME_DVT_QD) : "");
            }
        }
    }
}