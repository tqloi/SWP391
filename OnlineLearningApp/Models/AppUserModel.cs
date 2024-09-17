using Microsoft.AspNetCore.Identity;

namespace OnlineLearningApp.Models
{
	public class AppUserModel : IdentityUser
	{
		public string Occupation { get; set; }
	}
}
