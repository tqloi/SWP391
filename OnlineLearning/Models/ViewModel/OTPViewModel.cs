using System.ComponentModel.DataAnnotations;

namespace OnlineLearning.Models.ViewModel
{
    public class OTPViewModel
    {
        [Required(ErrorMessage ="Please input OTP in your email to login")]

        public int Otp { get; set; }
    }
}
