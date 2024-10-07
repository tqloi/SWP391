using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace OnlineLearning.Models
{
    public class Lecture
    {
        [Key]
        public int LectureID { get; set; }

        [ForeignKey("Course")]
        public int CourseID { get; set; }  // Foreign key to Courses table

        [Required]
        [StringLength(255)]
        public required string Title { get; set; }

        [StringLength(255)]
        public required string Description { get; set; }

        [Required]
        public DateTime UpLoadDate { get; set; }

        // Navigation property to represent the relationship with the Course
        public required CourseModel Course { get; set; }
    }
}
