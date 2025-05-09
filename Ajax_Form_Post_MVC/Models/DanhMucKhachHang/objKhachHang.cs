using System.ComponentModel.DataAnnotations;

namespace MVC_QuanLyTHP.Models
{
    public class objKhachHang
    {
        [StringLength(2)]
        public string LOC_ID { get; set; }

        [StringLength(100)]
        public string ID { get; set; }

        [StringLength(1000, ErrorMessage = "Tên KH tối đa 1000 ký tự.", MinimumLength = 0)]
        public string NAME { get; set; }

        [StringLength(1000, ErrorMessage = "Địa chỉ tối đa 1000 ký tự.", MinimumLength = 0)]
        public string ADDRESS { get; set; }

        [StringLength(20, ErrorMessage = "Số điện thoại tối đa 20 ký tự.", MinimumLength = 0)]
        public string TEL { get; set; }

        [StringLength(20, ErrorMessage = "Fax tối đa 20 ký tự.", MinimumLength = 0)]
        public string FAX { get; set; }

        [StringLength(20, ErrorMessage = "Email tối đa 20 ký tự.", MinimumLength = 0)]
        public string EMAIL { get; set; }

        [StringLength(20, ErrorMessage = "Ngày sinh tối đa 20 ký tự.", MinimumLength = 0)]
        public string NGAYSINH { get; set; }

        [StringLength(50, ErrorMessage = "Nghề nghiệp tối đa 50 ký tự.", MinimumLength = 0)]
        public string NGHENGHIEP { get; set; }

        public double? DIS { get; set; }

        public double? RATE { get; set; }

        [StringLength(50)]
        public string GROUP_NHOM { get; set; }

        public double? MAX_CONGNO { get; set; }

        public int? SONGAY { get; set; }

        [StringLength(100)]
        public string MAHANG_KH_LK { get; set; }

        [StringLength(100)]
        public string CUM_ID { get; set; }

        public double? CONGNODAUKY { get; set; }

        public string TENNHOM { get; set; }

        public string TENKHUVUC { get; set; }
    }
}