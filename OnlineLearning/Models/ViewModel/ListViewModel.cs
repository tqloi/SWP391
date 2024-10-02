using System.ComponentModel.DataAnnotations;

namespace OnlineLearning.Models.ViewModel
{
    public class ListViewModel
    {
        public List<CourseModel> Courses { get; set; } 
        public List<StudentCourseModel> StudentCourses { get; set; }

    }
}