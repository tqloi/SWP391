using Microsoft.AspNetCore.Identity;

namespace OnlineLearning.Models
{
    public class AppUserModel: IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ProfileImagePath { get; set; }
        public string Address { get; set; }
        public DateOnly Dob { get; set; }
        public bool Gender { get; set; }
        public double? WalletUser { get; set; } = 0.0;
    }
}
 