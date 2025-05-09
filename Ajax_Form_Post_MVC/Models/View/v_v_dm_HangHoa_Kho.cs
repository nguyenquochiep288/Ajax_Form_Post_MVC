using DatabaseTHP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_QuanLyTHP.Models
{
    public class v_v_dm_HangHoa_Kho : v_dm_HangHoa_Kho
    {
	public PagedList.IPagedList<v_dm_HangHoa_Kho> IPagedList;
    }
}