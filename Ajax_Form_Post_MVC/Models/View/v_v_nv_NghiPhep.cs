using DatabaseTHP;
using DatabaseTHP.Class;
using MVC_QuanLyTHP.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static DatabaseTHP.Class.API;

namespace MVC_QuanLyTHP.Models
{
    public class v_v_nv_NghiPhep : v_nv_NghiPhep
    {
        public PagedList.IPagedList<v_nv_NghiPhep> IPagedList;
        public List<ComboboxFrom> lstdm_NhanVien { get; set; }

        public List<ComboboxFrom> lstnv_PhepNam { get; set; }
        public DateTime TUNGAY { get; set; }
        public DateTime DENNGAY { get; set; }

        public List<LoaiHangHoa> lstTYPENghiPhep
        {
            get { return API.lstHinhThucNghiPhep(); }
        }
    }
}