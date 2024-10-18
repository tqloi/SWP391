using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineLearning.Models;
using OnlineLearningApp.Respositories;
using System.Security.Claims;
using YourNamespace.Models;

namespace OnlineLearning.Areas.Student.Controllers
{
    [Area("Student")]
    [Authorize(Roles = "Student")]
    [Route("Student/[controller]/[action]")]
    public class LectureController : Controller
    {
        private UserManager<AppUserModel> _userManager;
        private SignInManager<AppUserModel> _signInManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly DataContext _dataContext;
        private IConfiguration _configuration;
        public LectureController(DataContext dataContext, UserManager<AppUserModel> userManager, SignInManager<AppUserModel> signInManager, IWebHostEnvironment webHostEnvironment, IConfiguration configuration)
        {
            _dataContext = dataContext;
            _userManager = userManager;
            _signInManager = signInManager;
            _webHostEnvironment = webHostEnvironment;
            _configuration = configuration;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [ServiceFilter(typeof(CourseAccessFilter))]
        public async Task<IActionResult> LectureDetail(int LectureID)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var lecture = await _dataContext.Lecture.FindAsync(LectureID);
            var course = await _dataContext.Courses.FindAsync(lecture.CourseID);
            var lectureFiles = await _dataContext.LectureFiles.Where(lf => lf.LectureID == LectureID).ToListAsync();

            var completion = await _dataContext.LectureCompletion
                                .Include(lc => lc.Lecture)
                                .FirstOrDefaultAsync(lc => lc.LectureID == LectureID && lc.UserID == userId);

            ViewBag.Course = course;
            ViewBag.Lecture = lecture;
            ViewBag.Completion = completion;

            return View(lectureFiles);
        }

        [HttpPost]
        public async Task<IActionResult> Complete(int LectureID) 
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var lecture = await _dataContext.Lecture.FindAsync(LectureID);
            var completion = new LectureCompletionModel
            {
                UserID = userId,
                LectureID = LectureID,
                CompletionDate = DateTime.Now,
            };
            _dataContext.LectureCompletion.Add(completion);
            var studentCourse = _dataContext.StudentCourses.
                Where(sc => sc.CourseID == lecture.CourseID && sc.StudentID == userId).FirstOrDefault();

            if (studentCourse == null)
            {
                TempData["erroe"] = "Lecture Completed";
            }

                if (studentCourse != null)
            {
                var completedLecturesCount = await _dataContext.LectureCompletion
                    .CountAsync(c => c.UserID == userId &&
                                     c.LectureID == lecture.LectureID);

                var totalLecturesCount = await _dataContext.Lecture
                    .CountAsync(l => l.CourseID == lecture.CourseID);

                if (totalLecturesCount > 0)
                {
                    studentCourse.Progress = (decimal) completedLecturesCount / totalLecturesCount; 
                }
                _dataContext.StudentCourses.Update(studentCourse);
                Console.WriteLine($"User ID: {userId}");
                Console.WriteLine($"Total Lectures Completed: {completedLecturesCount}");
                Console.WriteLine($"Total Lectures in Course: {totalLecturesCount}");
                Console.WriteLine($"Progress: {studentCourse.Progress}%");
            }

            await _dataContext.SaveChangesAsync();

            TempData["success"] = "Lecture Completed";
            return RedirectToAction("LectureDetail", "Lecture", new { area = "Student", LectureID = LectureID});
        }

        public async Task<IActionResult> GoNext(int lectureID)
        {
            var currentLecture = await _dataContext.Lecture.FindAsync(lectureID);
            if (currentLecture == null)
            {
                return NotFound();
            }

            var nextLecture = await _dataContext.Lecture
                .Where(l => l.CourseID == currentLecture.CourseID && l.LectureID > currentLecture.LectureID)
                .OrderBy(l => l.LectureID)
                .FirstOrDefaultAsync();

            if (nextLecture != null)
            {
                return RedirectToAction("LectureDetail", new { area = "Student", LectureID = nextLecture.LectureID });
            }
            else
            {
				TempData["error"] = "This is the last lecture in the course.";
                return RedirectToAction("LectureDetail", new { area = "Student", LectureID = lectureID });
            }
        }


        [HttpGet]
        public async Task<IActionResult> GoPrevious(int lectureID)
        {
            var currentLecture = await _dataContext.Lecture.FindAsync(lectureID);
            if (currentLecture == null)
            {
                return NotFound();
            }

            var previousLecture = await _dataContext.Lecture
                .Where(l => l.CourseID == currentLecture.CourseID && l.LectureID < currentLecture.LectureID)
                .OrderByDescending(l => l.LectureID)
                .FirstOrDefaultAsync();

            if (previousLecture != null)
            {
                return RedirectToAction("LectureDetail", new { area = "Student", LectureID = previousLecture.LectureID });
            }
            else
            {
                TempData["erroe"] = "This is the first lecture in the course.";
                return RedirectToAction("LectureDetail", new { area = "Student", LectureID = lectureID });
            }
        }
    }
}
