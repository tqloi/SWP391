using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineLearning.Models
{
    public class RequestTranferModel
    {
        [Key] 
        public int TranferID { get; set; }
        public string UserID { get; set; }
        [Required(ErrorMessage = "Please input Name Bank")]
        public string BankName { get; set; }
        [Required(ErrorMessage = "Please input Account Number")]
        public string AccountNumber { get; set; }
        [Required(ErrorMessage = "Please input Full Name")]
        public string FullName { get; set; }
        [Required]
        public double MoneyNumber { get; set; }
        public string Status { get; set; }
        
        public AppUserModel User { get; set; }
        public DateTime CreateAt { get; set; }
    }
}
