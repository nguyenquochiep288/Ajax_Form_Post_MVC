using DatabaseTHP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_QuanLyTHP.Models
{
    public class v_v_Search
    {
        public List<v_dm_ThueSuat> lstdm_ThueSuat { get; set; }

        public List<v_dm_DonViTinh> lstdm_DonViTinh { get; set; }

        public List<v_dm_KhuVuc> lstKhuVuc { get; set; }
    }
}