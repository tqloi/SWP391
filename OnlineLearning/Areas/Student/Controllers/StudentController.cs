using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using OnlineLearning.Models.ViewModel;
using OnlineLearning.Models;
using OnlineLearningApp.Respositories;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using OnlineLearning.Controllers;
using Microsoft.AspNetCore.Authorization;
using OnlineLearning.Services;
using Microsoft.EntityFrameworkCore;

namespace OnlineLearning.Areas.Student.Controllers
{
    [Area("Student")]
    [Authorize(Roles = "Student")]
    [Route("Student/[controller]/[action]")]
    public class StudentController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DataContext datacontext;
        private UserManager<AppUserModel> _userManager;
        private SignInManager<AppUserModel> _signInManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly FileService _fileService;
        public StudentController(ILogger<HomeController> logger, DataContext context, SignInManager<AppUserModel> signInManager, UserManager<AppUserModel> userManager, IWebHostEnvironment webHostEnvironment, FileService fileService)
        {
            datacontext = context;
            _logger = logger;
            _signInManager = signInManager;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
            _fileService = fileService;
        }
        public IActionResult InstructorRegistration()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> InstructorRegistration(UploadFileViewModel model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (model.Filepdf != null)
            {
                var instructorConfirmation = new InstructorConfirmationModel();
                string fileName = model.Filepdf.FileName;
                try
                {
                    string downloadUrl = await _fileService.UploadInstructorCV(model.Filepdf);
                    instructorConfirmation.Certificatelink = downloadUrl;
                    instructorConfirmation.FileName = fileName;
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error uploading file: " + ex.Message);
                    TempData["error"] = "Edit failed due to file upload error!";
                    return View(model);
                }
                instructorConfirmation.UserID = userId;
                instructorConfirmation.SendDate = DateTime.Now;
                instructorConfirmation.Description = model.Description;

                datacontext.InstructorConfirmation.Add(instructorConfirmation);
                await datacontext.SaveChangesAsync();

                TempData["success"] = "Request sended successfully!";
                return View();
            }
            TempData["error"] = "An error occurred while processing the payment. Please try again.";
            return View(model);
        }

        [ServiceFilter(typeof(CourseAccessFilter))]
        public async Task<IActionResult> GradeList(int CourseID)
        {
            var course = await datacontext.Courses.FindAsync(CourseID);
            ViewBag.Course = course;
            return View();
        }
    }
}
