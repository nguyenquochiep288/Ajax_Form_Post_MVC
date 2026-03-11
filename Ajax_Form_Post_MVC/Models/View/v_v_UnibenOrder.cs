using DatabaseTHP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static DatabaseTHP.Class.Uniben.Uniben;

namespace MVC_QuanLyTHP.Models
{
    public class v_v_UnibenOrderData : UnibenOrderData
    {
	    public PagedList.IPagedList<UnibenOrderData> IPagedList;
       
    }
}