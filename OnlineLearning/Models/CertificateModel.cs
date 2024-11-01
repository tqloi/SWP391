using OnlineLearning.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YourNamespace.Models
{
    public class CertificateModel
    {
        [Key]
        public int CertificateID { get; set; }

        [Required]
        [StringLength(450)]
        public string StudentID { get; set; }

        [Required]
        public int CourseID { get; set; }

        [Required]
        public DateTime CompletionDate { get; set; }

        [Column(TypeName = "nvarchar(MAX)")]
        public string CertificateLink { get; set; }

        // Navigation properties
        [ForeignKey("StudentID")]
        public AppUserModel Student { get; set; }

        [ForeignKey("CourseID")]
        public CourseModel Course { get; set; }
    }
}
