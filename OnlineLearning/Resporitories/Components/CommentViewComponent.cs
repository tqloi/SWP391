using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineLearning.Models;
using OnlineLearning.Models.ViewModel;
using OnlineLearningApp.Respositories;

namespace OnlineLearning.Resporitories.Components
{
    public class CommentViewComponent : ViewComponent
    {
        private readonly DataContext _dataContext;
        public CommentViewComponent(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        public async Task<IViewComponentResult> InvokeAsync(int LectureID, int? ParentCommentID = null)
        {
            IQueryable<CommentModel> commentsQuery = _dataContext.Comment
                .Include(c => c.User).Where(c => c.LectureID == LectureID);

            if (ParentCommentID != null)
            {
                commentsQuery = commentsQuery.Where(c => c.ParentCmtId == ParentCommentID);
            }
            else
            {
                commentsQuery = commentsQuery.Where(c => c.ParentCmtId == null);
            }

            var comments = await commentsQuery.OrderByDescending(c => c.Timestamp).ToListAsync();
            var commentFiles = await _dataContext.CommentFile.ToListAsync();

            var model = new ListCommentViewModel 
            {
                Comments = comments,
                CommentFiles = commentFiles,
                LectureID = LectureID
            };
            return View(model);
        }
    }
}
