using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineLearning.Areas.Instructor.Models.ViewModel;
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
        public async Task<IActionResult> Dashboard(int CourseID)
        {
            var course =  _dataContext.Courses.Find(CourseID);
            ViewBag.Course = course;
            var startdate = DateTime.Now.AddDays(-7);
            var now  = DateTime.Now;
            var earning = _dataContext.Payment.Where(c => c.CourseID == CourseID).Sum(c => c.Amount);
            var earningweek = _dataContext.Payment.Where(c => c.CourseID == CourseID && c.PaymentDate >= startdate && c.PaymentDate <= now).Sum(c => c.Amount);
            var numstd = _dataContext.StudentCourses.Where(c => c.CourseID == CourseID).Count();
            var liststd = await _dataContext.StudentCourses.Where(c => c.CourseID == CourseID).Include(c => c.AppUser).ToListAsync();
            var dashboard = new DashBoardViewModel
            {
                EarningMonth = (double)earning,
                EarningDay = (double)earningweek,
                NumStudent = numstd,
                Rating = course.Rating,
                ListStudent = liststd
            };
            return View(dashboard);
        }
    }
}
