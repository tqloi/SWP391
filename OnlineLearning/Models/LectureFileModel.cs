using OnlineLearning.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YourNamespace.Models
{
    public class LectureFileModel
    {
        [Key]
        public int FileID { get; set; }

        [ForeignKey("Lecture")]
        public int LectureID { get; set; }

        [Required]
        [StringLength(255)]
        public string FileName { get; set; }

        [Required]
        [StringLength(255)]
        [RegularExpression(@"Document|Video", ErrorMessage = "FileType must be either 'Document' or 'Video'")]
        public string FileType { get; set; }

        [Required]
        [StringLength(500)]
        public string FilePath { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime UploadDate { get; set; } = DateTime.Now;

        // Navigation property
        public LectureModel Lecture { get; set; }
    }
}
