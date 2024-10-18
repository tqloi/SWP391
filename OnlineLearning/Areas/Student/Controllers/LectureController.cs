using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineLearning.Models;
using OnlineLearningApp.Respositories;
using System.Security.Claims;

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
    }
}
