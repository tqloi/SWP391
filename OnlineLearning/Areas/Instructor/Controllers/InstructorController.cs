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
        public readonly DataContext _dataContext;
        private UserManager<AppUserModel> _userManager;
        private SignInManager<AppUserModel> _signInManager;
        public InstructorController(DataContext context, ILogger<InstructorController> logger, UserManager<AppUserModel> userManager, SignInManager<AppUserModel> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _dataContext = context;
        }

        [HttpGet]
        [ServiceFilter(typeof(CourseAccessFilter))]
        public IActionResult Dashboard(int CourseID)
        {
            var course =  _dataContext.Courses.Find(CourseID);
            ViewBag.Course = course;

            return View();
        }
    }
}
