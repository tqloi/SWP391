using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineLearning.Models;
using OnlineLearningApp.Respositories;
using YourNamespace.Models;

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
        public async Task<IActionResult> LectureList(int id)
        {
            var course = await datacontext.Courses.FindAsync(id);

            if (course == null)
            {
                return NotFound(); 
            }
            //------- code ----
            ViewBag.CourseId = course.CourseID;
            return View(course); 
        }
        public async Task<IActionResult> AssignmentList(int id)
        {
            var course = await datacontext.Courses.FindAsync(id);

            if (course == null)
            {
                return NotFound();
            }
            //------- code ----
            ViewBag.CourseId = course.CourseID;
            return View(course);
        }
        public async Task<IActionResult> TestList(int id)
        {
            var course = await datacontext.Courses.FindAsync(id);

            if (course == null)
            {
                return NotFound();
            }
            // ------ codde ----------
            ViewBag.CourseId = course.CourseID;
            return View(course);
        }
        public async Task<IActionResult> LectureDetail(int id)
        {
            var lectureFile = await datacontext.LectureFiles
                .Include(l => l.Lecture) 
                .FirstOrDefaultAsync(l => l.LectureID == id);

            var lecture = lectureFile.Lecture;

            ViewBag.CourseId = lecture.CourseID; 
            return View(lecture); 
        }

    }
}
