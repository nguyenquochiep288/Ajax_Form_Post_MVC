using System.ComponentModel.DataAnnotations;

namespace MVC_QuanLyTHP.Models
{
    public class objTruyCap
    {
        public int idNhanVien { get; set; }

        [StringLength(50)]
        public string idMay { get; set; }
    }
}