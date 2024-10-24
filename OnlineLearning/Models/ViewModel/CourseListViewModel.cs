namespace OnlineLearning.Models.ViewModel
{
    public class CourseListViewModel
    {
        public List<CourseModel> CourseList { get; set; }
        public int TotalPage { get; set; }
        public int CurrentPage { get; set; }
        public string Keyword { get; set; }
        public int? Category { get; set; }
        public string Level { get; set; }
    }
}
