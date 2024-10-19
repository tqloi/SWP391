using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineLearning.Models;
using OnlineLearning.Models.ViewModel;
using OnlineLearning.Services;
using OnlineLearningApp.Respositories;

namespace OnlineLearning.Areas.Student.Controllers
{
    [Area("Student")]
    [Authorize(Roles ="Student")]
    [Route("Student/[controller]/[action]")]
    public class AssignmentController : Controller
    {
        private UserManager<AppUserModel> _userManager;
        private SignInManager<AppUserModel> _signInManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly DataContext _dataContext;
        private IConfiguration _configuration;
        private readonly FileService _fileService;

        public AssignmentController(DataContext dataContext, UserManager<AppUserModel> userManager, SignInManager<AppUserModel> signInManager, IWebHostEnvironment webHostEnvironment, IConfiguration configuration, FileService file)
        {
            _dataContext = dataContext;
            _userManager = userManager;
            _signInManager = signInManager;
            _webHostEnvironment = webHostEnvironment;
            _configuration = configuration;
            _fileService = file;
        }
        public IActionResult Index()
        {
            return View();
        }
        
        [HttpGet]
        public async Task<IActionResult> SubmitAssignment(int id)
        {
            var assignment = await _dataContext.Assignment.FindAsync(id);
            if (assignment == null)
            {
                return RedirectToAction("Index", "Home", new {Areas=""});
            }
            var model = new SubmissionViewModel();
            model.AssignmentID = id;
           var course = _dataContext.Courses.FirstOrDefault(c => c.CourseID == assignment.CourseID);
            ViewBag.Course= course;
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> SubmitAssignment(SubmissionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["error"] = "Error";
                return RedirectToAction("MyCourse", "Course", new { Areas = "" });
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }
            var assignment = await _dataContext.Assignment.FirstOrDefaultAsync(a => a.AssignmentID == model.AssignmentID);
            if (assignment == null) 
            { 
                return NotFound();
            }
            if (DateTime.Now > assignment.DueDate)
            {
                TempData["error"] = "Qua han nop bai";
                return RedirectToAction("AssignmentList", "Participation", new { Areas = "", CourseID = assignment.CourseID });
            }
            else
            {

                string filesubmit = "";
                if (model.SubmissionLink != null)
                {
                    string filename = model.SubmissionLink.FileName;
                    var submit = new SubmissionModel();
                    try
                    {
                        string downloadUrl = await _fileService.UploadAssignment(model.SubmissionLink);
                        submit.AssignmentID = model.AssignmentID;
                        submit.SubmissionLink = downloadUrl;
                        submit.StudentID = user.Id;
                        submit.SubmissionDate = DateTime.Now;
                        submit.FileName = filename;

                        TempData["success"] = "Submit successful!";
                        _dataContext.Submission.Add(submit);
                        await _dataContext.SaveChangesAsync();

                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", "Error uploading file: " + ex.Message);
                        TempData["error"] = "Edit failed due to file upload error!";
                        return View(model);
                    }
                    
                }
                else
                {
                    TempData["error"] = "Student did not submit file pdf";
                    return RedirectToAction("AssignmentList", "Participation", new { Areas = "", CourseID = assignment.CourseID });
                }
            }
            
            return RedirectToAction("AssignmentList", "Participation", new { Areas = "", CourseID = assignment.CourseID });
        }
    }
}
