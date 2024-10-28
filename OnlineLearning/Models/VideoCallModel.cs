using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineLearning.Models
{
    public class VideoCallModel
    {
        [Key]
        public int VideoCallId { get; set; }
        public string SendID { get; set; }
        public string ReceiveID { get; set; }
        [ForeignKey("SendID")]
        public AppUserModel Send { get; set; }
        [ForeignKey("ReceiveID")]
        public AppUserModel Receive { get; set; }
    }
}
