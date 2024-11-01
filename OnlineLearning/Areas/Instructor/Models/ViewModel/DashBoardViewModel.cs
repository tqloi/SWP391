using OnlineLearning.Models;

namespace OnlineLearning.Areas.Instructor.Models.ViewModel
{
    public class DashBoardViewModel
    {
        public double EarningMonth { get; set; }
        public double EarningDay { get; set; }
        public int NumStudent { get; set; }
        public double Rating { get; set; }
        public List<StudentCourseModel> ListStudent { get; set; }
    }
}
