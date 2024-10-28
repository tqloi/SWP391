namespace OnlineLearning.Models.ViewModel
{
    public class DoTestViewModel
    {
        public required IEnumerable<QuestionModel> Questions { get; set; }
        public double TimeLeft { get; set; }
    }
}
