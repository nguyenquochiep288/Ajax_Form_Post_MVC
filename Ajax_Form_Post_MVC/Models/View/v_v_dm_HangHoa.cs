using DatabaseTHP;
using DatabaseTHP.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static DatabaseTHP.Class.API;

namespace MVC_QuanLyTHP.Models
{
    public class v_v_dm_HangHoa : v_dm_HangHoa
    {
        public PagedList.IPagedList<v_dm_HangHoa> IPagedList;
        public List<v_dm_DonViTinh> lstdm_DonViTinh { get; set; }
        public List<v_dm_DonViTinh> lstdm_DonViTinh_QD { get; set; }
        public List<v_dm_NhaCungCap> lstdm_NhaCungCap { get; set; }
        public List<v_dm_NhomHangHoa> lstdm_NhomHangHoa { get; set; }
        public List<v_dm_ThueSuat> lstdm_ThueSuat { get; set; }

        public List<LoaiHangHoa> lstLoaiHangHoa
        {
            get { return API.lstLoaiHangHoa(); }
        }
    }
}