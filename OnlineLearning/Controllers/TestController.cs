using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineLearning.Models;
using OnlineLearningApp.Respositories;
using System.Diagnostics;

namespace OnlineLearning.Controllers
{
    [Authorize]
    public class TestController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DataContext datacontext;
        private UserManager<AppUserModel> _userManager;
        private SignInManager<AppUserModel> _signInManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public TestController(ILogger<HomeController> logger, DataContext context, SignInManager<AppUserModel> signInManager, UserManager<AppUserModel> userManager, IWebHostEnvironment webHostEnvironment)
        {
            datacontext = context;
            _logger = logger;
            _signInManager = signInManager;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
        }

        //currently not in use yet
        public IActionResult Index(int CourseID)
        {
            ViewBag.CourseID = CourseID;
            var Course = datacontext.Courses.Find(CourseID);
            //absolutely useless foking line but i add it just for fun
            ViewBag.Course = Course;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> CreateTest(int CourseID)
        {
            ViewBag.CourseID = CourseID;
            var course = await datacontext.Courses.FindAsync(CourseID);

            if (course == null)
            {
                return NotFound();
            }

            ViewBag.Course = course;
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> CreateTest(TestModel model)
        {
            var Course = datacontext.Courses.Find(model.CourseID);
            if (Course == null)
            {
                return NotFound();
            }
            //fucku viewbag shit
            if (model.Course == null)
            {
                model.Course = Course;
            }
            // if (ModelState.IsValid) invalid as always
            try
            {
                Debug.WriteLine("ID retrieved valid");
                var newTest = new TestModel
                {
                    Title = model.Title,
                    Course = model.Course,
                    Description = model.Description,
                    StartTime = model.StartTime,
                    EndTime = model.EndTime,
                    Status = model.Status,
                    CourseID = model.CourseID,
                    NumberOfQuestion = 0
                };
                // Add the test to the context and save changes
                datacontext.Test.Add(newTest);
                await datacontext.SaveChangesAsync();
                Debug.WriteLine("Test saved to database");

                TempData["success"] = "Test created successfully!";
                return RedirectToAction("TestList", new { courseID = model.CourseID });
            }

            catch (Exception)
            {
                TempData["error"] = "Test creation failed!";
                return View();
            }
        }
    }
}
