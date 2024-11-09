using System.ComponentModel.DataAnnotations;

namespace OnlineLearning.Models.ViewModel
{
    public class RequestTransferViewModel
    {
        public string UserID { get; set; }
        [Required(ErrorMessage = "Bank Name is required.")]
        public string BankName { get; set; }

        [Required(ErrorMessage = "Account Number is required.")]
        public string AccountNumber { get; set; }

        [Required(ErrorMessage = "Full Name is required.")]
        public string FullName { get; set; }

        [Range(1, double.MaxValue, ErrorMessage = "Withdrawal amount must be greater than zero.")]
        public double MoneyNumber { get; set; }
        public double CurrentMoney { get; set; }
        public string Status { get; set; }
        public DateTime CreateAtTime { get; set; }
    }
}
