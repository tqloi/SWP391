namespace OnlineLearning.Models.ViewModel
{
    public class ListCommentViewModel
    {
        public List<CommentModel> Comments { get; set; }
        public List<CommentFileModel> CommentFiles { get; set; }
        public int LectureID { get; set; }
    }
}
