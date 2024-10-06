using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineLearning.Models
{
    public class InstructorConfirmationModel
    {
        [Key]
        public int ConfirmationID { get; set; }
        public string UserID { get; set; }
        public string Certificatelink { get; set; }
        [ForeignKey("UserID")]
        public AppUserModel user { get; set; }
        public DateTime SendDate { get; set; }
    }
}
