using Microsoft.AspNetCore.Identity;

namespace OnlineLearning.Models
{
    public class AppUserModel: IdentityUser
    {
        public string ProfileImagePath { get; set; }
        public string Address { get; set; }
        public DateOnly Dob { get; set; }
        public bool Gender { get; set; }

    }
}
