using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineLearning.Controllers;
using OnlineLearning.Models;
using OnlineLearning.Models.ViewModel;
using OnlineLearningApp.Respositories;
using System.Security.Claims;

namespace OnlineLearning.Areas.Student.Controllers
{
    [Area("Student")]
    [Authorize(Roles = "Student")]
    [Route("Student/[controller]/[action]")]
    public class CourseController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DataContext datacontext;
        private UserManager<AppUserModel> _userManager;
        private SignInManager<AppUserModel> _signInManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CourseController(ILogger<HomeController> logger, DataContext context, SignInManager<AppUserModel> signInManager, UserManager<AppUserModel> userManager, IWebHostEnvironment webHostEnvironment)
        {
            datacontext = context;
            _logger = logger;
            _signInManager = signInManager;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> MyCourse()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var model = new ListViewModel();

            if (User.IsInRole("Student"))
            {
                model.StudentCourses = await datacontext.StudentCourses
                    .Where(sc => sc.StudentID == userId)
                    .Include(sc => sc.Course)
                    .ToListAsync();
                model.Courses = await datacontext.StudentCourses
                    .Where(sc => sc.StudentID == userId)
                    .Include(sc => sc.Course)
                    .ThenInclude(course => course.Instructor)
                    .ThenInclude(instructor => instructor.AppUser)
                    .Select(sc => sc.Course)
                    .OrderByDescending(sc => sc.CourseID)
                    .ToListAsync();
            }
            return View(model);
        }
    }
}
