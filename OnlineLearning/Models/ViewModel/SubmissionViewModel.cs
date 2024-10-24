using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineLearning.Models.ViewModel
{
    public class SubmissionViewModel
    {
        public int AssignmentID { get; set; }
        
        [Required(ErrorMessage ="Request File PDF")]
        public IFormFile SubmissionLink { get; set; }
        public DateTime SubmissionDate { get; set; }
        
    }
}
