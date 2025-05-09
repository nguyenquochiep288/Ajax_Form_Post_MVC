using System.ComponentModel.DataAnnotations;

namespace MVC_QuanLyTHP.Controllers
{
    public class Login_Model
    {
        [Required]
        public string user { get; set; }


        [Required]
        public string pass { get; set; }

        public bool check { get; set; }

        public string fullname { get; set; }

        public string iduser { get; set; }

        public string url_image { get; set; }
    }
}
