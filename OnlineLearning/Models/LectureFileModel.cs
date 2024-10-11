using OnlineLearning.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YourNamespace.Models
{
    public class LectureFileModlel
    {
        [Key]
        public int FileID { get; set; }

        [ForeignKey("Lecture")]
        public int LectureID { get; set; }

        [Required]
        [StringLength(255)]
        public string FileName { get; set; }

        [Required]
        [StringLength(500)]
        public string FilePath { get; set; }

        [DataType(DataType.Date)]
        public DateTime UploadDate { get; set; } = DateTime.Now;
        public LectureModel Lecture { get; set; }
    }
}
