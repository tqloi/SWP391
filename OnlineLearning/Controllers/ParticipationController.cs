using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineLearning.Models;
using OnlineLearning.Models.ViewModel;
using OnlineLearningApp.Respositories;

using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

using YourNamespace.Models;
using System.Security.Claims;


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
        [ServiceFilter(typeof(CourseAccessFilter))]
        public async Task<IActionResult> TestList(int CourseID)
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
        public async Task<IActionResult> LectureDetail(int LectureID)
        {
            var lecture = await datacontext.Lecture.FindAsync(LectureID);
            var course = await datacontext.Courses.FindAsync(lecture.CourseID);

            ViewBag.Course = course;
            return View(lecture); 
        }
    }
}
