using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_QuanLyTHP.Class
{
    public class Search
    {
        public string MyModal { get; set; }
        public string TitleSearch { get; set; }
        public string ShowSearchValue { get; set; }

        public string SearchString { get; set; }

        public string ClassName { get; set; }

        public string ValueField { get; set; }

        public string TextField { get; set; }

        public string TrField { get; set; }

        public string BodyField { get; set; }

        public List<Tuple<string, string, bool, int>> listSearch { get; set; }
        public int HinhThucTimKiem { get; set; }
        public string ValueSelected { get; set; }
        public string ID_KHO { get; set; }
        public string ID_KHUVUC { get; set; }
    }
}