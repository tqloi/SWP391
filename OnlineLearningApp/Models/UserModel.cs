using System.ComponentModel.DataAnnotations;

namespace OnlineLearningApp.Models
{
	public class UserModel
	{
		[Key]
        public int UserId { get; set; }
		[Required(ErrorMessage ="Please enter username")]
		public string Username { get; set; }
		[Required(ErrorMessage = "Please enter password")]
		public string Password { get; set; }
		[Required(ErrorMessage = "Please enter email")]
		public string Email { get; set; }
		public string Phone { get; set; }
		public string ProfileImages { get; set; }
		public string Status { get; set; }
		[Required(ErrorMessage = "Please enter usertype")]
		public string UserType { get; set; }
        public DateTime Lastlogin { get; set; }

    }
}
