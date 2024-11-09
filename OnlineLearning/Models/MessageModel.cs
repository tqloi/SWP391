using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace OnlineLearning.Models
{
    public class MessageModel
    {
        [Key]
        public int Id { get; set; }
        public string SenderId { get; set; }
        public string ReceiverId { get; set; }
        [Required]
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }
        [ForeignKey("SenderId")]
        public AppUserModel Sender { get; set; }
        [ForeignKey("ReceiverId")]
        public AppUserModel Receiver { get; set; }
    }
}
