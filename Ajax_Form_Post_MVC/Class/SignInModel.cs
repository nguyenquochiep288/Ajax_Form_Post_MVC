using System.ComponentModel.DataAnnotations;

namespace MVC_QuanLyTHP.Class
{

	public class SignInModel
	{
		[Required]
		public string UserName { get; set; }

		[Required]
		public string Password { get; set; }
	}
}
