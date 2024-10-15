using System.ComponentModel.DataAnnotations;

namespace OnlineLearning.Models.ViewModel
{
    public class InstructorProfileViewModel
    {
        [Required]
        public string UserId { get; set; }

        [Required]
            [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Username cannot contain spaces or special characters")]
            public string UserName { get; set; }
            [Required]
            public string FirstName { get; set; }
            [Required]
            public string LastName { get; set; }
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [Phone]
            public string PhoneNumber { get; set; }

            [Display(Name = "Profile Image")]
            public IFormFile? ProfileImage { get; set; }

            public string? ExistingProfileImagePath { get; set; }

            public string Address { get; set; }
            public DateOnly Dob { get; set; }
            public bool gender { get; set; }
         

        
    }
}
