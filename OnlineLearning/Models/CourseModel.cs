using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.VisualStudio.TestPlatform.ObjectModel;


namespace OnlineLearning.Models
{
    public class CourseModel
    {
        [Key]
        public int CourseID { get; set; }
        [MaxLength(255)]
        public string Title { get; set; }
        [MaxLength(20)]
        public string CourseCode { get; set; }
        [MaxLength(255)]
        public string Description { get; set; }
        public string CoverImagePath { get; set; }
        public string InstructorID { get; set; }
        public int NumberOfStudents { get; set; } = 0;
        [Column(TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }
        public int CategoryID { get; set; }
        [MaxLength(50)]
        public string Level { get; set; }
        public bool Status { get; set; } = true;
        public bool IsBaned { get; set; } = false;

        public DateTime CreateDate { get; set; }
        public DateTime LastUpdate { get; set; }
        public DateTime? EndDate { get; set; }
        public double Rating { get; set; }
        public int NumberOfRate { get; set; } = 0;
        [ForeignKey("CategoryID")]
        public CategoryModel Category { get; set; }
        [ForeignKey("InstructorID")]
        public InstructorModel Instructor { get; set; }

    }
}
