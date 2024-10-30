using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NuGet.Packaging.Signing;
using OnlineLearning.Models;
using OnlineLearningApp.Respositories;
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
            Console.WriteLine($"Content: {model.Content}");
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
            return RedirectToAction("LectureDetail", "Participation", new {LectureID = model.LectureID });
        }
    }
}
