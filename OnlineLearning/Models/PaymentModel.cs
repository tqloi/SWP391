using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineLearning.Models
{
    public class PaymentModel
    {
        [Key]
        public int PaymentID { get; set; } 

        [ForeignKey("Course")]
        public int CourseID { get; set; } 

        [ForeignKey("Student")]
        public string StudentID { get; set; } 

        [Column(TypeName = "decimal(10, 2)")]
        public decimal Amount { get; set; }

        public DateTime PaymentDate { get; set; } = DateTime.Now;

        [Required]
        [StringLength(30)]
        [Column("Status")]
        public string Status { get; set; } 

        public virtual CourseModel Course { get; set; }

        public virtual AppUserModel Student { get; set; }
    }
}
