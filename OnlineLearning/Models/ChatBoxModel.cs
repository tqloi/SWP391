using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineLearning.Models
{
    public class ChatBoxModel
    {
        [Key]
        public int ChatBoxID { get; set; }
        [Required]
        public string SenderID { get; set; }
        [Required]
        public string ReceiverID { get; set; }
        [Required]
        public string Title { get; set; }
        [ForeignKey("SenderID")]
        public AppUserModel UserSend { get; set; }
        [ForeignKey("ReceiverID")]
        public AppUserModel UserReceive { get; set; }
    }
}
