using System.ComponentModel.DataAnnotations;

namespace MVC_QuanLyTHP.Models
{
    public class PersonModel
    {
        /// <summary>
        /// Gets or sets PersonId.
        /// </summary>
        /// 
        [Required(ErrorMessage = "Vui lòng nhập số điện thoại!!!")]
        [RegularExpression(@"^\(?([0-9]{4})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{3})$",
                   ErrorMessage = "Định dạng điện thoại đã nhập không hợp lệ! VD: 0907.565.434 - 0907565434")]
        public string PersonId { get; set; }

        /// <summary>
        /// Gets or sets Name.
        /// </summary>
        /// 
        [Required(ErrorMessage = "Vui lòng nhập họ và tên!!!")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets Message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets intCapNhat.
        /// </summary>
        public int intCapNhat { get; set; }

        /// <summary>
        /// Gets or sets City.
        /// </summary>
        /// 
        [Required(ErrorMessage = "Vui lòng nhập số lượng người tham gia!!!")]
        public int City { get; set; }
    }
}