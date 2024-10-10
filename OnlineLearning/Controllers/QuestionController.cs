using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineLearning.Models;
using OnlineLearningApp.Respositories;

namespace OnlineLearning.Controllers
{
    [Authorize]
    public class QuestionController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DataContext datacontext;
        private UserManager<AppUserModel> _userManager;
        private SignInManager<AppUserModel> _signInManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public QuestionController(ILogger<HomeController> logger, DataContext context, SignInManager<AppUserModel> signInManager, UserManager<AppUserModel> userManager, IWebHostEnvironment webHostEnvironment)
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
        public IActionResult CreateQuestionRedirector(int TestID)
        {
            ViewBag.TestID = TestID;
            TestModel test = datacontext.Test.Find(TestID);
            ViewBag.CourseID = test.CourseID; 
            ViewBag.Test = test;
            return View("CreateQuestion");
        }
    }
}
