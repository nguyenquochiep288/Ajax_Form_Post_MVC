using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DatabaseTHP;
namespace MVC_QuanLyTHP.Models.Order
{
    public class OrderProduct : INV_DEPOSIT_DTL_TEMP
    {
        /// <summary>
        /// id Nhóm hàng
        /// </summary>
        //public string idItemGroup { get; set; }

        /// <summary>
        /// id Hàng hóa
        /// </summary>
        //public string idProduct { get; set; }

        /// <summary>
        /// Tên hàng hóa
        /// </summary>

        public string NameProduct { get; set; }

        /// <summary>
        /// Đơn vị tính
        /// </summary>
        public string UnitProduct { get; set; }

        /// <summary>
        /// Đơn giá
        /// </summary>

        //public Decimal PriceProduct { get; set; }

        /// <summary>
        /// Số lượng
        /// </summary>

        //public Decimal QTY { get; set; }

        /// <summary>
        /// Thanh tiền
        /// </summary>

        private Decimal _ThanhTien;
        public Decimal ThanhTien {
            set
            {
                _ThanhTien = Math.Round(Convert.ToDecimal(PRICE * QTY));
            }

            get { return Math.Round(Convert.ToDecimal(PRICE * QTY)); }
        }

        /// <summary>
        /// % Giảm giá
        /// </summary>
        //public double DISCOUNT { get; set; }

        /// <summary>
        /// Số tiền giảm giá
        /// </summary>
        //public double dis_values { get; set; }


        /// <summary>
        /// Tổng cộng
        /// </summary>
        private Decimal _TongCong;
        public Decimal TongCong { 
            set 
            {
                _TongCong = Math.Round(ThanhTien - Convert.ToDecimal(dis_values));
            }

            get { return Math.Round(ThanhTien - Convert.ToDecimal(dis_values)); }
        }

        /// <summary>
        /// id Hàng Hóa Combo
        /// </summary>
        // string promat_id { get; set; }

        /// <summary>
        /// Số lượng thực sản phẩm con combo
        /// </summary>
        public Decimal QtyProductCombo { get; set; }

        /// <summary>
        /// Là combo
        /// </summary>
        public bool isCombo { get; set; }

        /// <summary>
        /// Là khuyến mãi
        /// </summary>
        public bool isKhuyenMai { get; set; }

        /// <summary>
        /// id Hàng hóa khuyến mãi
        /// </summary>
        //public string idProductKhuyenMai { get; set; }

        /// <summary>
        /// Số lượng khuyến mãi
        /// </summary>
        //public Decimal QtyProductKhuyenMai { get; set; }


        /// <summary>
        /// Số lượng quy đổi sản phẩm
        /// </summary>
        public double Qty_QD { get; set; }

        /// <summary>
        /// Mặt hàng là Combo thì giá trị 1(Tbl_Promat), mặt hàng là nhóm hàng thì giá trị 2(Tbl_item_group). Mặt hàng bình thường là giá trị 0.  
        /// </summary>
        //public string STATUS { get; set; }

        /// <summary>
        /// Đơn giá để tính chiết khấu trên mặt hàng khuyến mãi
        /// </summary>
        public double PriceHangKhuyenMai { get; set; }

        /// <summary>
        /// Ghi chú
        /// </summary>
        //public string Diengiai { get; set; }

        // <summary>
        // Số lượng yêu cầu khuyến mãi
        // </summary>
        public double SoLuongLayKhuyenMaiLonNhat { get; set; }
        
    }
}