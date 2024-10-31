using OnlineLearning.Models;

namespace OnlineLearning.Areas.Instructor.Models.ViewModel
{
    public class ScoreListViewModel
    {
        public List<ScoreModel> ListScore { get; set; }
        public int TotalPage { get; set; }
        public int CurrentPage { get; set; }
        public DateTime DoneAt { get; set; }
        public int TestID { get; set; }
    }
}
