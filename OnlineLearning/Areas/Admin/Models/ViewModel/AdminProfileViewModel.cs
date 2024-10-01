using System.ComponentModel.DataAnnotations;

namespace OnlineLearning.Areas.Admin.Models.ViewModel
{
    public class AdminProfileViewModel
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateOnly Dob { get; set; }
        public string Address { get; set; }
        [Display(Name = "Profile Image")]
        public IFormFile? ProfileImage { get; set; }
        public string? ExistingProfileImagePath { get; set; }
        public bool Gender { get; set; }
    }
}
