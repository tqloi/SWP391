using OnlineLearning.Models;

namespace OnlineLearning.Areas.Student.Models.ViewModel
{
    public class GradeListViewModel
    {
        public List<ScoreModel> scoretests { get; set; }
        public List<ScoreAssignmentModel> scoreAssignments { get; set; }
        public List<SubmissionModel> submissions { get; set; }
    }
}
