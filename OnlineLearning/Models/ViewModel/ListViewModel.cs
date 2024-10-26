using System.ComponentModel.DataAnnotations;

namespace OnlineLearning.Models.ViewModel
{
    public class ListViewModel
    {
        public List<CourseModel>? Courses { get; set; } 
        public List<StudentCourseModel>? StudentCourses { get; set; }
        public List<CategoryModel> Categories { get; set; }
        public List<BookmarkModel>? Bookmarks { get; set; }
        public List<ReviewModel>? Reviews { get; set; }
        public int TotalPage { get; set; }
        public int CurrentPage { get; set; }
        public int? Category { get; set; }
        public string Level { get; set; }
        public string Status { get; set; }
    }
}