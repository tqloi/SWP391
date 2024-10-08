using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineLearning.Models
{
    public class SubmissionModel
    {
        [Key]
        public int SubmissionID { get; set; }
        public int AssignmentID { get; set; }
        public string StudentID { get; set; }
        public string SubmissionLink { get; set; }
        public DateTime SubmissionDate { get; set; }
        [ForeignKey("AssignmentID")]
        public AssignmentModel Assignment { get; set; }
        [ForeignKey("StudentID")]
        public AppUserModel User { get; set; }
    }
}
