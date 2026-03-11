using System.ComponentModel.DataAnnotations;

namespace MVC_QuanLyTHP.Controllers
{

	public class ChangePassword
	{
		[Required(ErrorMessage = "Vui lòng nhập mật khẩu cũ.")]
		public string Password { get; set; }

		[Required(ErrorMessage = "Vui lòng nhập mật khẩu mới.")]
		[RegularExpression("^(?=.*[!@#$%^&*(),.?\":{}|<>])(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)[a-zA-Z\\d!@#$%^&*(),.?\":{}|<>]{10,}$", ErrorMessage = "Mật khẩu phải có ít nhất 10 ký tự, trong đó có ít nhất 1 chữ hoa, ít nhất 1 chữ thường, ít nhất một số và một ký tự đặc biệt.")]
		public string NewPassword { get; set; }

		[Required(ErrorMessage = "Vui lòng nhập mật khẩu xác nhận.")]
		public string ConfirmPassword { get; set; }
	}
}
