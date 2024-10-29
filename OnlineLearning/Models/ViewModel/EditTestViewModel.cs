using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace OnlineLearning.Models.ViewModel
{
    public class EditTestViewModel
    {
        [MaxLength(255)]
        public required string Title { get; set; } // Title of the test

        [MaxLength(255)]
        public required string Description { get; set; }

        [ForeignKey("Course")]
        public required int CourseID { get; set; } // Foreign key to Courses
        public DateTime StartTime { get; set; } // Start time of the test
        public DateTime EndTime { get; set; } // End time of the test
        public int NumberOfQuestion { get; set; } // Number of questions in the test
        public required string Status { get; set; } // Status of the test (e.g., active, inactive)
        public double? PassingScore { get; set; }
        public required string AlowRedo { get; set; }
        public int? NumberOfMaxAttempt { get; set; }
        public int TestHours { get; set; }
        public int TestMinutes { get; set; }
        public int TestID { get; set; }
        public CourseModel? Course { get; set; }
    }
}
