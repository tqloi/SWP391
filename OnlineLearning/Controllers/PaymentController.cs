using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineLearning.Models;
using OnlineLearning.Models.ViewModel;
using OnlineLearningApp.Respositories;
using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Security.Claims;

namespace OnlineLearning.Controllers
{

    [Authorize(Roles = "Student")]
    public class PaymentController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DataContext datacontext;
        private UserManager<AppUserModel> _userManager;
        private SignInManager<AppUserModel> _signInManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public PaymentController(ILogger<HomeController> logger, DataContext context, SignInManager<AppUserModel> signInManager, UserManager<AppUserModel> userManager, IWebHostEnvironment webHostEnvironment)
        {
            datacontext = context;
            _logger = logger;
            _signInManager = signInManager;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> PaymentConfirmation(int id)
        {
            var course = await datacontext.Courses
                .FirstOrDefaultAsync(c => c.CourseID == id);

            if (course == null)
            {
                return NotFound();
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);

            var payment = new PaymentViewModel
            {
                UserID = userId,
                UserFullName = $"{user.FirstName} {user.LastName}", 
                CourseID = course.CourseID,
                CourseName = course.Title, 
                Price = course.Price, 
                PaymentDate = DateTime.Now 
            };

            return View(payment);
        }

        [HttpPost]
        public async Task<IActionResult> Pay(PaymentViewModel model)
        {
            var payment = new PaymentModel
            {
                CourseID = model.CourseID,
                StudentID = model.UserID,
                Amount = model.Price,
                PaymentDate = DateTime.Now,
                Status = "Completed"
            };

            try
            {
                datacontext.Payment.Add(payment);
                await datacontext.SaveChangesAsync();

                var studentCourse = new StudentCourseModel
                {
                    CourseID = model.CourseID,
                    StudentID = model.UserID,
                    EnrollmentDate = DateTime.Now,
                    Progress = 0,
                    CertificateStatus = "In Progress"
                };

                datacontext.StudentCourses.Add(studentCourse);
                var course = await datacontext.Courses.FirstOrDefaultAsync(c => c.CourseID == model.CourseID);
                course.NumberOfStudents += 1;
                await datacontext.SaveChangesAsync();

                TempData["success"] = "Payment completed successfully!";
                return RedirectToAction("CourseDetail", "Course", new { id = model.CourseID });
            }
            catch (Exception ex)
            {
                TempData["error"] = "An error occurred while processing the payment. Please try again.";
                return RedirectToAction("CourseDetail", "Course", new { id = model.CourseID });
            }
        }
    }
}
