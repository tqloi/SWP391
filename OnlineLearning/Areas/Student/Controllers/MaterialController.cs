using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineLearning.Filter;
using OnlineLearning.Models;
using OnlineLearningApp.Respositories;

namespace OnlineLearning.Areas.Student.Controllers
{
    [Area("Student")]
    [Authorize]
    [Route("Student/[controller]/[action]")]
    public class MaterialController : Controller
    {
        private UserManager<AppUserModel> _userManager;
        private SignInManager<AppUserModel> _signInManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly DataContext _dataContext;
        private IConfiguration _configuration;
        public MaterialController(DataContext dataContext, UserManager<AppUserModel> userManager, SignInManager<AppUserModel> signInManager, IWebHostEnvironment webHostEnvironment, IConfiguration configuration)
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
        public async Task<IActionResult> MaterialList(int courseID)
        {

            var materialList = await _dataContext.CourseMaterials
                                     .Where(m => m.CourseID == courseID)
                                     .ToListAsync();
            var course = await _dataContext.Courses.FindAsync(courseID);


            ViewBag.Course = course;

            return View(materialList);
        }
    }
}
