namespace OnlineLearning.Models.ViewModel
{
    public class TestResultViewModel
    {
        public string CourseName { get; set; }
        public int CourseID { get; set; }
        public int TestID { get; set; }
        public double Score { get; set; }
        public DateTime DoneAt { get; set; }
        public int TotalQuestions { get; set; }
        public int? NumberOfAttemptLeft { get; set; }
        public Dictionary<int, string> Answers { get; set; }
        public Dictionary<int, string> CorrectAnswers { get; set; }
    }
}
