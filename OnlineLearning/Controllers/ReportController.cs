using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineLearning.Models;
using OnlineLearningApp.Respositories;
using System.Security.Claims;

namespace OnlineLearning.Controllers
{
    [Authorize(Roles = "Student, Instructor")]
    public class ReportController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DataContext _dataContext;
        private UserManager<AppUserModel> _userManager;
        private SignInManager<AppUserModel> _signInManager;

        public ReportController(ILogger<HomeController> logger, DataContext context, SignInManager<AppUserModel> signInManager, UserManager<AppUserModel> userManager)
        {
            _dataContext = context;
            _logger = logger;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> ReportCourse(int CourseID, string Reason)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var course = await _dataContext.Courses.FindAsync(CourseID);

            try
            {
                var feedback = new FeedbackModel
                {
                    UserID = userId,
                    Subject = "COURSE REPORT",
                    Comment = $"Course ({course.Title}): {Reason}",
                    FeedbackDate = DateTime.Now,
                };
                _dataContext.Feedback.Add(feedback);
                await _dataContext.SaveChangesAsync();

                TempData["success"] = "Feedback has been submitted successfully!";
            }
            catch (Exception ex)
            {
                TempData["error"] = "Failed to submit feedback. Please try again later.";
                return RedirectToAction("MyCourse", "Course", new { area = "Student" });
            }

            return RedirectToAction("MyCourse", "Course", new { area = "Student" });
        }
    }
}
