using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineLearning.Models
{
    public class AssignmentModel
    {
        [Key] 
        public int AssignmentID { get; set; }
        public int CourseID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        [ForeignKey("CourseID")]
        public CourseModel Course { get; set; }
        public string AssignmentLink { get; set; }
    }
}
