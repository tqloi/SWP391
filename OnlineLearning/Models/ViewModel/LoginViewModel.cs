using System.ComponentModel.DataAnnotations;

namespace OnlineLearning.Models.ViewModel
{
    public class LoginViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Please enter Username")]
        public string Username { get; set; }
        

        [DataType(DataType.Password), Required(ErrorMessage = "Please enter password")]
        public string Password { get; set; }
        [Display(Name = "Remember Me?")]
        public bool RememberMe { get; set; }
        
    }
}
