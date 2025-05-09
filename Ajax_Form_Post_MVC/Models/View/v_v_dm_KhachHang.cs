using DatabaseTHP;
using DatabaseTHP.Class;
using MVC_QuanLyTHP.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_QuanLyTHP.Models
{
    public class v_v_dm_KhachHang : v_dm_KhachHang
    {
	    public PagedList.IPagedList<v_dm_KhachHang> IPagedList;
        public List<v_dm_KhuVuc> lstdm_KhuVuc { get; set; }
        public List<v_dm_NhomKhachHang> lstdm_NhomKhachHang { get; set; }

        public Double KHOANGCACH
        {
            get
            {
                if (Utility.Latitude != 0 && Utility.Longitude != 0 && (LATITUDE ?? 0) != 0 && (LONGITUDE ?? 0) != 0)
                {

                    //string origin = (API.LATITUDE.ToString().Replace(",",".") + "," + API.LONGITUDE.ToString().Replace(",", "."));
                    //string destination = ((LATITUDE ?? 0).ToString().Replace(",", ".") + "," + (LONGITUDE ?? 0).ToString().Replace(",", "."));
                    //string e = API.TinhKhoangCachBangGoogleAPI(origin, destination).GetAwaiter().GetResult();
                    //return Convert.ToDouble(e);
                    //if (_KHOANGCACH == null)
                    //{
                    //}
                    //string origin = (API.LATITUDE.ToString() + "," + API.LONGITUDE.ToString());
                    //string destination = ((LATITUDE ?? 0).ToString() + "," + (LONGITUDE ?? 0).ToString());
                    //string t1111 = await API.TinhKhoangCachBangGoogleAPI(origin, destination);
                    var khoangcach = API.CalculateDistance(Utility.Latitude, Utility.Longitude, LATITUDE ?? 0, LONGITUDE ?? 0);
                    return khoangcach == 0 ? 1 : khoangcach;
                }
                return 0;
            }
            set { }
        }
    }
}