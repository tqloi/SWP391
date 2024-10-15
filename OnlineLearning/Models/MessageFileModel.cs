using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace OnlineLearning.Models
{
    public class MessageFileModel
    {
        [Key]
        public int FileID { get; set; }

        [Required]
        public int MessageID { get; set; }

        [Required]
        [MaxLength(255)]
        public string FileName { get; set; }

        [Required]
        [MaxLength(500)]
        public string FilePath { get; set; }

        [Required]
        public DateTime UploadDate { get; set; } = DateTime.Now;

        [ForeignKey("MessageID")]
        public MessageModel Message { get; set; }
    }
}
