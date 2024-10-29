using System.ComponentModel.DataAnnotations;

namespace OnlineLearning.Models.ViewModel
{
    public class AssignmentViewModel
    {
        public int AssignmentID { get; set; }
        public int CourseID { get; set; }
        public string Title { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        public DateTime DueDate { get; set; }
        
        public IFormFile? AssignmentLink { get; set; }
        public string? ExistedAssignmentLink { get; set; }
    }
}
