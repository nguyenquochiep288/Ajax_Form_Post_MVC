using DatabaseTHP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_QuanLyTHP.Models
{
    public class v_v_dm_LoaiLuong : v_dm_LoaiLuong
    {
	public PagedList.IPagedList<v_dm_LoaiLuong> IPagedList;
    }
}