using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineLearning.Models
{
    public class NotificationModel
    {
        [Key]
        public int NotificationID { get; set; }
        public string UserId { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        [ForeignKey("UserId")]
        public AppUserModel User { get; set; }
    }
}
