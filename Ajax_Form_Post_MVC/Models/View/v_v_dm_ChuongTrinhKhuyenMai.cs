using DatabaseTHP;
using DatabaseTHP.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static DatabaseTHP.Class.API;

namespace MVC_QuanLyTHP.Models
{
    public class v_v_dm_ChuongTrinhKhuyenMai : v_view_dm_ChuongTrinhKhuyenMai
    {
        public PagedList.IPagedList<v_dm_ChuongTrinhKhuyenMai> IPagedList;

        public List<v_dm_DonViTinh> lstdm_DonViTinh { get; set; }

        public List<LoaiHangHoa> lstHINHTHUC_TINHKPI
        {
            get { return API.lstHinhThucTinhKPI(); }
        }
    }
}