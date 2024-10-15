using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace OnlineLearning.Models
{
    public class MessageModel
    {
        [Key]
        public int MessageID { get; set; }

        [Required]
        public int ChatBoxID { get; set; }

        [Required]
        public string SenderID { get; set; }

        [Required]
        public string Content { get; set; }

        public bool IsRead { get; set; } = false;

        [Required]
        public DateTime Timestamp { get; set; } = DateTime.Now;

        [ForeignKey("ChatBoxID")]
        public ChatBoxModel ChatBox { get; set; }

        [ForeignKey("SenderID")]
        public AppUserModel Sender { get; set; }
    }
}
