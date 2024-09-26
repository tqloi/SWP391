using System.ComponentModel.DataAnnotations;

namespace OnlineLearning.Models
{
    public class CourseModel
    {
        [Key]
        public int CourseId { get; set; }
        public string Title { get; set; }
        public string CourseDescription { get; set; }
        [Display(Name ="Display Images cover Course")]
        public IFormFile? CourseImagePath { get; set; }
        public decimal PriceCourse { get; set; }
        public bool Status { get; set; }
        public DateTime CreateCourse { get; set; }
        public double NumberOfRate { get; set; }
        public string InstructorId { get; set; }
        public InstructorModel Instructor { get; set; }
        public int CategoryId { get; set; }
        public CategoryModel Category { get; set; }
        public DateTime LastUpdate { get; set; }


    }
}
