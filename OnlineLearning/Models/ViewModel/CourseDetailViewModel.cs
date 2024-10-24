namespace OnlineLearning.Models.ViewModel
{
    public class CourseDetailViewModel
    {
        public CourseModel Course { get; set; }
        public List<ReviewModel>? Reviews { get; set; }
        public ReviewModel? YourReview { get; set; }
        public int TotalPage { get; set; }
        public int CurrentPage { get; set; }
        public bool IsMark { get; set; }
        public StudentCourseModel? StudentCourses { get; set; }
    }
}
