using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineLearning.Models.ViewModel
{
    public class SubmissionViewModel
    {
        public int AssignmentID { get; set; }
        
        [Required(ErrorMessage ="Request File PDF")]
        public string SubmissionLink { get; set; }
        public IFormFile SubmissionFile { get; set; }
        public DateTime SubmissionDate { get; set; }
        
    }
}
