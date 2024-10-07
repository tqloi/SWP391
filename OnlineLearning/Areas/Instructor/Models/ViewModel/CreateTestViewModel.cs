using OnlineLearning.Models;
using OnlineLearning.Models.OnlineLearning.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace OnlineLearning.Areas.Instructor.Models.ViewModel
{
    public class CreateTestViewModel
    {
        public CourseModel? Course { get; set; }
        public List<TestModel>? Tests { get; set; }

        //input for the new test
        public string Title { get; set; } 
        public string Description { get; set; }
        public int CourseID { get; set; }
        public DateTime StartTime { get; set; } // Start time of the test
        public DateTime EndTime { get; set; } // End time of the test
        public string Status { get; set; } // Status of the test (e.g., active, inactive)
    }
}
