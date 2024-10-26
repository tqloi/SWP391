namespace OnlineLearning.Models.ViewModel
{
    public class AssignmentListViewModel
    {
        public List<AssignmentModel> Assignments { get; set; }
        public List<SubmissionModel> Submissions { get; set; }
        public List<ScoreAssignmentModel> ScoreAssignments { get; set; }
        public int TotalPage { get; set; }
        public int CurrentPage { get; set; }
    }
}
