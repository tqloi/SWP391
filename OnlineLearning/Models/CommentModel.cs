using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace OnlineLearning.Models
{
    public class CommentModel
    {
        [Key]
        public int CommentID { get; set; }

        [ForeignKey("Lecture")]
        public int LectureID { get; set; } 

        [ForeignKey("User")]
        [StringLength(450)]
        public string UserID { get; set; }  // Foreign key to Users table

        [Column(TypeName = "text")]
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }

        [ForeignKey("ParentComment")]
        public int? ParentCmtId { get; set; }  // Nullable for root comments
        public LectureModel Lecture { get; set; }
        public AppUserModel User { get; set; }
        public CommentModel ParentComment { get; set; }

    }
}
