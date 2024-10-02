using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineLearning.Models
{
    public class StudentCourseModel
    {
        [Key]
        public int StudentCourseID { get; set; }
        public string StudentID { get; set; }
        public int CourseID { get; set; }
        [Column(TypeName = "decimal(5,2)")]
        [Range(0, 100, ErrorMessage = "Progress must be between 0 and 100.")]
        public decimal Progress { get; set; }
        [MaxLength(50)]
        public string CertificateStatus { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public DateTime? CompletionDate { get; set; }
        [ForeignKey("StudentID")]
        public AppUserModel AppUser { get; set; }

        [ForeignKey("CourseID")]
        public CourseModel Course { get; set; }
    }
}
