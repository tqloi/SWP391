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
        public async Task<IActionResult> ReportCourse(int CourseID, string Reason, string OtherReason)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var course = await _dataContext.Courses.FindAsync(CourseID);
            if (OtherReason != null) { Reason = OtherReason; }

            try
            {
                var feedback = new ReportModel
                {
                    UserID = userId,
                    Subject = "COURSE REPORT",
                    Comment = $"Course [{course.Title}] : {Reason}",
                    FeedbackDate = DateTime.Now,
                };
                _dataContext.Report.Add(feedback);
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

        [HttpPost]
        public async Task<IActionResult> ReportLecture(int LectureID, string Reason, string OtherReason)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var lecture = await _dataContext.Lecture.FindAsync(LectureID);
            var course = await _dataContext.Courses.FindAsync(lecture.CourseID);
            if (OtherReason != null) { Reason = OtherReason; }

            try
            {
                var feedback = new ReportModel
                {
                    UserID = userId,
                    Subject = "LECTURE REPORT",
                    Comment = $"Course [{course.Title}] : Lecture [{lecture.Title}] : {Reason}",
                    FeedbackDate = DateTime.Now,
                };
                _dataContext.Report.Add(feedback);
                await _dataContext.SaveChangesAsync();

                TempData["success"] = "Feedback has been submitted successfully!";
            }
            catch (Exception ex)
            {
                TempData["error"] = "Failed to submit feedback. Please try again later.";
                return RedirectToAction("LectureDetail", "Lecture", new { area = "Student", LectureID = LectureID });
            }

            return RedirectToAction("LectureDetail", "Lecture", new { area = "Student", LectureID = LectureID });
        }
    }
}
