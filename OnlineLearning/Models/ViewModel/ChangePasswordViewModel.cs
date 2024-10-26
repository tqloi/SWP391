using System.ComponentModel.DataAnnotations;

namespace OnlineLearning.Models.ViewModel
{
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; }
       
        [Required(ErrorMessage = "Password is required")]
        [StringLength(40, MinimumLength = 4)]
        [DataType(DataType.Password)]
        
        public string  OldPassword { get; set; }
        [Required(ErrorMessage = "Password is required")]
        [StringLength(40, MinimumLength = 4)]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        
        public string NewPassword { get; set; }
        [Required(ErrorMessage = "Confirm Password is required")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm New Password")]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
        public string ConfirmNewPassword { get; set; }
    }
}
