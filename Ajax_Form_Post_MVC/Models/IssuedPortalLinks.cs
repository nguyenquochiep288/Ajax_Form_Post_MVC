using System;
using System.Collections.Generic;

namespace MVC_QuanLyTHP.Models
{
    public class IssuedPortalLinks
    {
        public List<IssuedPortalLink> Rows { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int Total { get; set; }
    }

    public class IssuedPortalLink
    {
        public string CustomerId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public DateTime CreatedUtc { get; set; }
        public DateTime? ExpiresUtc { get; set; }
        public DateTime? LastAccessUtc { get; set; }
    }
}
