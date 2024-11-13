namespace OnlineLearning.Areas.Admin.Models.ViewModel
{
    public class Top5CourseViewModel
    {
        public int CourseID { get; set; }
        public string Title { get; set; }
        public string  NameInstructor { get; set; }
        public string  Images { get; set; }
        public int Count { get; set; }
        public decimal Price { get; set; }
        public DateTime Createdate { get; set; }
    }
}
