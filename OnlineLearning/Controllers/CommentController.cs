using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging.Signing;
using OnlineLearning.Models;
using OnlineLearningApp.Respositories;
using System.ComponentModel.Design;
using System.Security.Claims;

namespace OnlineLearning.Controllers
{
    public class CommentController : Controller
    {
        private UserManager<AppUserModel> _userManager;
        private SignInManager<AppUserModel> _signInManager;
        private readonly DataContext _dataContext;
        public CommentController(SignInManager<AppUserModel> signInManager, UserManager<AppUserModel> userManager, DataContext dataContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _dataContext = dataContext;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CommentModel model)
        {
            var userID = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var comment = new CommentModel
            {
                UserID = userID,
                LectureID = model.LectureID,
                Content = model.Content,
                Timestamp = DateTime.Now,
                ParentCmtId = model.ParentCmtId
            };
            _dataContext.Comment.Add(comment);
            await _dataContext.SaveChangesAsync();
            return RedirectToAction("LectureDetail", "Participation", new { LectureID = model.LectureID });
        }

        [HttpPost]
        public async Task<IActionResult> Update(CommentModel model)
        {
            var userID = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var comment = await _dataContext.Comment.FindAsync(model.CommentID);

            try
            {
                comment.Content = model.Content;

                _dataContext.Comment.Update(comment);
                await _dataContext.SaveChangesAsync();
            }
            catch
            {
                TempData["warning"] = "Something wnent wrong";
                return RedirectToAction("LectureDetail", "Participation", new { LectureID = comment.LectureID });
            }
            TempData["success"] = "Comment Updated";
            return RedirectToAction("LectureDetail", "Participation", new { LectureID = comment.LectureID });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int CommentID)
        {
            var userID = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var comment = await _dataContext.Comment.FindAsync(CommentID);

            await DeleteCommentWithChildren(comment);
            await _dataContext.SaveChangesAsync();

            TempData["success"] = "Comment Deleted";
            return RedirectToAction("LectureDetail", "Participation", new { LectureID = comment.LectureID });
        }

        private async Task DeleteCommentWithChildren(CommentModel comment)
        {
            var childComments = await _dataContext.Comment
                .Where(c => c.ParentCmtId == comment.CommentID).ToListAsync();

            foreach (var childComment in childComments)
            {
                await DeleteCommentWithChildren(childComment);
            }

            _dataContext.Comment.Remove(comment);
        }
    }
}
