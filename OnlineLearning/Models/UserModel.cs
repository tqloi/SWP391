using System.ComponentModel.DataAnnotations;

namespace OnlineLearning.Models
{
    public class UserModel
    {
        [Key]
        public int UserId { get; set; }
        [Required(ErrorMessage = "Please enter username")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Please enter password")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Please enter email")]
        public string Email { get; set; }
        

    }
}
