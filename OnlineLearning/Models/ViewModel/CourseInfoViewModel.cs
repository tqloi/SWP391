using YourNamespace.Models;

namespace OnlineLearning.Models.ViewModel
{
    public class CourseInfoViewModel
    {
        public List<LectureModel> Lectures { get; set; }
        public List<LectureCompletionModel> Completion { get; set; }
        public StudentCourseModel StudentCourse { get; set; }
        public int TotalCourse { get; set; }
        public int TotalStudent { get; set; }
        public bool IsPass { get; set; }
    }
}
