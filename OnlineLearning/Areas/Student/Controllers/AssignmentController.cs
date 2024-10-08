using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineLearning.Models;
using OnlineLearning.Models.ViewModel;
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
        public AssignmentController(DataContext dataContext, UserManager<AppUserModel> userManager, SignInManager<AppUserModel> signInManager, IWebHostEnvironment webHostEnvironment, IConfiguration configuration)
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
        public async Task<IActionResult> SubmitAssignment(int id)
        {
            var assignment = await _dataContext.Assignment.FindAsync(id);
            if (assignment == null)
            {
                return RedirectToAction("Index", "Home", new {Areas=""});
            }
            var model = new SubmissionViewModel();
            model.AssignmentID = id;
           
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
                return RedirectToAction("AssignmentList", "Participation", new { Areas = "", id = assignment.CourseID });
            }
            else
            {
                string filesubmit = "";
                if (model.SubmissionLink != null)
                {
                    string originalFileName = Path.GetFileName(model.SubmissionLink.FileName);
                    string uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "Assignment");
                    string fileName = Guid.NewGuid() + "_" + originalFileName;
                    string filePath = Path.Combine(uploadPath, fileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.SubmissionLink.CopyToAsync(fileStream);
                    }
                    filesubmit = fileName;
                    var submit = new SubmissionModel
                    {
                        AssignmentID = assignment.AssignmentID,
                        StudentID = user.Id,
                        SubmissionDate = DateTime.Now,
                        SubmissionLink = filesubmit

                    };
                    TempData["success"] = "Nop bai thanh cong";
                    _dataContext.Submission.Add(submit);
                    await _dataContext.SaveChangesAsync();
                }
                else
                {
                    TempData["error"] = "Student did not submit file pdf";
                    return RedirectToAction("AssignmentList", "Participation", new { Areas = "", id = assignment.CourseID });
                }
            }
            
            return RedirectToAction("AssignmentList", "Participation", new { Areas = "", id = assignment.CourseID });
        }
    }
}
