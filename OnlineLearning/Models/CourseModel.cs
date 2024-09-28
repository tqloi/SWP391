using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;

namespace OnlineLearning.Models
{
    public class CourseModel
    {
        [Key]
        public int CourseId { get; set; }
        public string Title { get; set; }
        public string description { get; set; }
        public string coverImagePath { get; set; }
        [Display(Name ="Display Images cover Course")]
        [NotMapped]
        public IFormFile? CourseImagePath { get; set; }
        public decimal Price { get; set; }
        public bool Status { get; set; }
        public DateTime CreateDate { get; set; }
        public int NumberOfRate { get; set; }
        public string InstructorId { get; set; }
        public InstructorModel Instructor { get; set; }
        public int CategoryId { get; set; }
        public CategoryModel Category { get; set; }
        public DateTime LastUpdate { get; set; }


    }
}
