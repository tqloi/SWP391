using System.ComponentModel.DataAnnotations;

namespace OnlineLearningApp.Models.ViewModel
{
	public class LoginViewModel
	{
		public int Id { get; set; }
		[Required(ErrorMessage = "Please enter username")]
		public string Username { get; set; }
		[DataType(DataType.Password), Required(ErrorMessage = "Please enter password")]
		public string Password { get; set; }
	}
}
