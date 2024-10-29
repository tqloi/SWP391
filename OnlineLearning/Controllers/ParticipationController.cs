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
using Firebase.Auth;


namespace OnlineLearning.Controllers
{
    [Authorize(Roles = "Instructor, Student")]
    public class ParticipationController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DataContext datacontext;
        private UserManager<AppUserModel> _userManager;
        private SignInManager<AppUserModel> _signInManager;

        public ParticipationController(ILogger<HomeController> logger, DataContext context, SignInManager<AppUserModel> signInManager, UserManager<AppUserModel> userManager)
        {
            datacontext = context;
            _logger = logger;
            _signInManager = signInManager;
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
        public async Task<IActionResult> AssignmentList(int CourseID, int page = 1)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var course = await datacontext.Courses.FindAsync(CourseID);
            var assignments = await datacontext.Assignment.Where(a => a.CourseID == CourseID).ToListAsync();
            var submissions = await datacontext.Submission.ToListAsync();
            var scores = await datacontext.ScoreAssignment.ToListAsync();

            if (assignments == null)
            {
                return NotFound();
            }

            //page
            int pageSize = 5;
            var totalAssignments = assignments.Count();
            assignments = assignments.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            var model = new AssignmentListViewModel
            {
                Assignments = assignments,
                Submissions = submissions,
                ScoreAssignments = scores,
                CurrentPage = page,
                TotalPage = (int)Math.Ceiling(totalAssignments / (double)pageSize)
            };

            HttpContext.Session.SetInt32("courseid", CourseID);
            ViewBag.Course = course;
            return View(model);
        }


        [HttpGet]
        [ServiceFilter(typeof(CourseAccessFilter))]
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

            if (course.InstructorID == userId)
            {
                return RedirectToAction("LectureDetail", "Lecture", new { area = "Instructor", LectureID = LectureID });
            }
            else 
            {
                return RedirectToAction("LectureDetail", "Lecture", new { area = "Student", LectureID = LectureID });
            }

        }
    }
}
