using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineLearning.Controllers;
using OnlineLearning.Models;
using OnlineLearningApp.Respositories;
using Org.BouncyCastle.Asn1;
using System.Diagnostics;

namespace OnlineLearning.Areas.Instructor.Controllers
{
    [Area("Instructor")]

    [Route("[controller]/[action]")]

    public class InstructorController : Controller
    {

        private readonly ILogger<InstructorController> _logger;
        public readonly DataContext _context;
        private UserManager<AppUserModel> _userManager;
        private SignInManager<AppUserModel> _signInManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public InstructorController(DataContext context, ILogger<InstructorController> logger, UserManager<AppUserModel> userManager, SignInManager<AppUserModel> signInManager, IWebHostEnvironment webHostEnvironment)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        public IActionResult Index(int CourseID)
        {
            //Debug.WriteLine("Course from My course: " + course);
            //var course = _context.Courses.FirstOrDefault(c => c.CourseID == CourseID);
            ViewBag.CourseID =CourseID;
            
            return View();
        }

        [Route("/CreateTest")]
        [HttpPost]
        public async Task<IActionResult> CreateTest(TestModel model)
        {
            if (ModelState.IsValid)
            {
                var course = _context.Courses.FirstOrDefault(c => c.CourseID == model.CourseID);
                if (course == null)
                {
                    TempData["error"] = "Course not found!";
                    return RedirectToAction("Index");
                }
                model.Course = course;

                Debug.WriteLine("ID retrieved valid");
                var newTest = new TestModel
                {
                    AlowRedo = model.AlowRedo,
                    Title = model.Title,
                    Course = course,
                    Description = model.Description,
                    StartTime = model.StartTime,
                    EndTime = model.EndTime,
                    Status = model.Status,
                    CourseID = model.CourseID,
                    NumberOfQuestion = 0
                };
                // Add the test to the context and save changes
                _context.Test.Add(newTest);
                await _context.SaveChangesAsync();
                Debug.WriteLine("Test saved to database");

                TempData["success"] = "Test created successfully!";
                return RedirectToAction("Index");
            }
            Debug.WriteLine("ModelState is invalid");
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                Debug.WriteLine($"Error: {error.ErrorMessage}");
            }
            TempData["error"] = "Test creation failed!";
            return RedirectToAction("Index");
        }

    }
}
