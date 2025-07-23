using MVC_QuanLyTHP.Class;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MVC_QuanLyTHP.Models
{
    public class TimKiem
    {
        private DateTime _NgayBatDau;
        [Required(ErrorMessage = "Vui lòng nhập ngày bắt đầu(Ngày/Tháng/Năm)!")]
        [Display(Name = "Ngày bắt đầu")]
        public DateTime NgayBatDau
        {
            get
            {
                if (_NgayBatDau.Year == 1)
                    _NgayBatDau = Utility.CurrentTime;
                return _NgayBatDau;
            }
            set
            {
                _NgayBatDau = value;
            }
        }

        private DateTime _NgayKetThuc;
        [Required(ErrorMessage = "Vui lòng nhập ngày kết thúc(Ngày/Tháng/Năm)!")]
        [Display(Name = "Ngày kết thúc")]
        public DateTime NgayKetThuc
        {
            get
            {
                if (_NgayKetThuc.Year == 1)
                    _NgayKetThuc = Utility.CurrentTime;
                return _NgayKetThuc;
            }
            set
            {
                _NgayKetThuc = value;
            }
        }

        public string idNhomKhachHang { get; set; }
        public List<Combobox> listNhomKhachHang { get; set; }

        public string idKhuVuc { get; set; }
        public List<Combobox> listKhuVuc { get; set; }

        private List<int> _SoLuongShow;
        public List<int> SoLuongShow
        {
            get
            {
                _SoLuongShow = new List<int>();
                for (int i = 1000; i <= 5000; i += 1000)
                    _SoLuongShow.Add(i);
                return _SoLuongShow;
            }
            set
            {
                _SoLuongShow = value;
            }
        }

        private int _show;
        public int show
        {
            get
            {
                if (_show == 0)
                    _show = 2500;
                return _show;
            }
            set
            {
                _show = value;
            }
        }

        private int _page;
        public int page
        {
            get
            {
                if (_page == 0)
                    _page = 1;
                return _page;
            }
            set
            {
                _page = value;
            }
        }


        private bool _TheoThoiGian;
        public bool TheoThoiGian
        {
            get
            {
                return _TheoThoiGian;
            }
            set
            {
                _TheoThoiGian = value;
            }
        }

        public string key { get; set; }


        private bool _CoPhatSinh;
        public bool CoPhatSinh
        {
            get
            {
                return _CoPhatSinh;
            }
            set
            {
                _CoPhatSinh = value;
            }
        }

        private bool _ConCongNo;
        public bool ConCongNo
        {
            get
            {
                return _ConCongNo;
            }
            set
            {
                _ConCongNo = value;
            }
        }

        private bool _CoPhatSinhTrongKy;
        public bool CoPhatSinhTrongKy
        {
            get
            {
                return _CoPhatSinhTrongKy;
            }
            set
            {
                _CoPhatSinhTrongKy = value;
            }
        }

        public string GroupID { get; set; }

        public string keySearch { get; set; }

        public string idKhachHang { get; set; }

        #region Khách hàng
        public string GROUP_NHOM { get; set; }
        public string CUM_ID { get; set; }
        #endregion
        public string idNhomQuyen { get; set; }

        public string ID_KHO { get; set; }

        public Boolean BOLTONKHO { get; set; }
    }
}
