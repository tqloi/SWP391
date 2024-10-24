using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineLearning.Models
{
	public class ScoreAssignmentModel
	{
        [Key]
        public int ScoreAssignmentID { get; set; }
        public string StudentID { get; set; }
        public int AssignmentID { get; set; }
        public double Score { get; set; }
        [ForeignKey("StudentID")]
        public AppUserModel Student { get; set; }
        [ForeignKey("AssignmentID")]
        public AssignmentModel Assignment { get; set; }
    }
}
