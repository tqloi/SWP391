namespace OnlineLearning.Models.ViewModel
{
    public class CourseListViewModel
    {
        public List<CourseModel> CourseList { get; set; }
        public int TotalPage { get; set; }
        public int CurrentPage { get; set; }
    }
}
