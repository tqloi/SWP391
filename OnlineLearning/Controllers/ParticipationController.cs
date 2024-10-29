using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineLearning.Models;
using OnlineLearning.Models.ViewModel;
using OnlineLearningApp.Respositories;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;
using System.Diagnostics;

using YourNamespace.Models;
using System.Security.Claims;
using Google.Api;


namespace OnlineLearning.Controllers
{
    [Authorize]
    public class ParticipationController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DataContext datacontext;
        private UserManager<AppUserModel> _userManager;
        private SignInManager<AppUserModel> _signInManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ParticipationController(ILogger<HomeController> logger, DataContext context, SignInManager<AppUserModel> signInManager, UserManager<AppUserModel> userManager, IWebHostEnvironment webHostEnvironment)
        {
            datacontext = context;
            _logger = logger;
            _signInManager = signInManager;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
        }

        [HttpGet]
        [ServiceFilter(typeof(CourseAccessFilter))]
        public async Task<IActionResult> CourseInfo(int CourseID)
        {
            var course = await datacontext.Courses.FindAsync(CourseID);

            if (course == null)
            {
                return NotFound();
            }

            ViewBag.Course = course;
            return View(course);
        }


        [HttpGet]
        [ServiceFilter(typeof(CourseAccessFilter))]
        public async Task<IActionResult> AssignmentList(int CourseID)
        {
            var course = await datacontext.Courses.FindAsync(CourseID);
            var assignments = await datacontext.Assignment.Where(a => a.CourseID == CourseID).ToListAsync();

            if (assignments == null)
            {
                return NotFound();
            }
            HttpContext.Session.SetInt32("courseid", CourseID);
            ViewBag.Course = course;
            return View(assignments);
        }

        public ActionResult ViewAssignmentPdf(int Id)
        {
            var assignmentlink = datacontext.Assignment.FirstOrDefault(c => c.AssignmentID == Id);

            return View(assignmentlink);
        }

        [HttpGet]
        public async Task<IActionResult> TestList(int CourseID)
        {
            ViewBag.CourseId = CourseID;
            var course = await datacontext.Courses.FindAsync(CourseID);
            ViewBag.Course = course;

            var TestList = datacontext.Test
                                      .Where(test => test.CourseID == CourseID)
                                      .ToList();
            var ScoreList = new List<ScoreModel>();
            //find Score in each test
            foreach (var test in TestList)
            {
                var score = datacontext.Score
                     .Where(s => s.TestID == test.TestID)
                     .FirstOrDefault();
                if (score != null)
                {
                    ScoreList.Add(score);
                }
            }
            var model = new TestListViewModel
            {
                Tests = TestList,
                Scores = ScoreList
            };
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> LectureDetail(int LectureID)
        {
            var lectue = await datacontext.Lecture.FindAsync(LectureID);
            var course = await datacontext.Courses.FindAsync(lectue.CourseID);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var isEnrolled = await datacontext.StudentCourses
                                      .FirstOrDefaultAsync(sc => sc.StudentID == userId && sc.CourseID == course.CourseID);

            var isInstrucotr = await datacontext.Courses
                                        .FirstOrDefaultAsync(c => c.InstructorID == userId && c.CourseID == course.CourseID);

            if (isEnrolled != null)
            {
                return RedirectToAction("LectureDetail", "Lecture", new { area = "Student", LectureID = LectureID });
            }
            if (isInstrucotr != null)
            {
                return RedirectToAction("LectureDetail", "Lecture", new { area = "Instructor", LectureID = LectureID });
            }
            else
            {
                return NotFound();
            }
        }
            [HttpGet]
            public async Task<IActionResult> MaterialList(int CourseID)
            {
               
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var isEnrolled = await datacontext.StudentCourses
                                          .FirstOrDefaultAsync(sc => sc.StudentID == userId && sc.CourseID == CourseID);

                var isInstrucotr = await datacontext.Courses
                                            .FirstOrDefaultAsync(c => c.InstructorID == userId && c.CourseID == CourseID);

            if (User.IsInRole("Student"))
            {
                return RedirectToAction("MaterialList", "Material", new { area = "Student", CourseID = CourseID });
            }
            if (User.IsInRole("Instructor"))
            {
                return RedirectToAction("MaterialList", "Material", new { area = "Instructor", CourseID = CourseID });
            }
            else
            {
                return NotFound();
            }
        }
    }
}
