using Microsoft.Build.Framework;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RequiredAttribute = System.ComponentModel.DataAnnotations.RequiredAttribute;

namespace OnlineLearning.Models.ViewModel
{
    public class CourseViewModel
    {
        [Required]
        public int CourseID { get; set; }
        [Required]
        public int CategoryID { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string CourseCode { get; set; }
        public IFormFile? CoverImage { get; set; }
        [Required]
        public int CategoryId { get; set; }
        [Required]
        public string Level { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Please enter a valid price")]
        public decimal Price { get; set; }
        [Required]
        public IFormFileCollection? CourseMaterials { get; set; } 
        public string InstructorId { get; set; }
        public bool State { get; set; }
    }
}
