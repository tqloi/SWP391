using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace OnlineLearning.Models
{
    public class CommentFileModel
    {
        [Key]
        public int FileID { get; set; }

        [ForeignKey("Comment")]
        public int CommentID { get; set; }

        [StringLength(255)]
        public string FileName { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string FilePath { get; set; }
        public DateTime UploadDate { get; set; } = DateTime.Now;
        public CommentModel Comment { get; set; }
    }
}
