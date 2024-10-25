using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using OnlineLearning.Models.ViewModel;
using OnlineLearning.Models;
using OnlineLearningApp.Respositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using OnlineLearning.Services;

namespace OnlineLearning.Areas.Instructor.Controllers
{
    [Area("Instructor")]
    [Authorize(Roles = "Instructor")]

    [Route("/Instructor/[controller]/[action]")]

    public class AssignmentController : Controller
    {
        private UserManager<AppUserModel> _userManager;
        private SignInManager<AppUserModel> _signInManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly DataContext _dataContext;
        private IConfiguration _configuration;
        private readonly FileService _fileService;
        public AssignmentController(DataContext dataContext, UserManager<AppUserModel> userManager, SignInManager<AppUserModel> signInManager, IWebHostEnvironment webHostEnvironment, IConfiguration configuration, FileService fileService)
        {
            _dataContext = dataContext;
            _userManager = userManager;
            _signInManager = signInManager;
            _webHostEnvironment = webHostEnvironment;
            _configuration = configuration;
            _fileService = fileService;
        }
        [HttpPost]
        public async Task<IActionResult> CreateAssignment(AssignmentViewModel model)
        {
            var courseid = HttpContext.Session.GetInt32("courseid");
            if (ModelState.IsValid)
            {
                var assignment = new AssignmentModel();
                assignment.CourseID = (int)courseid;
                assignment.Title = model.Title;

                if (model.DueDate < DateTime.Now || model.DueDate < model.StartDate)
                {
                    TempData["error"] = "Invalid date!";
                    return RedirectToAction("AssignmentList", "Participation", new { Areas = "", CourseID = assignment.CourseID });
                }
                assignment.StartDate = model.StartDate;
                assignment.DueDate = model.DueDate;

                try
                {
                    string downloadUrl = await _fileService.UploadLectureDocument(model.AssignmentLink);
                    assignment.AssignmentLink = downloadUrl;
                    model.ExistedAssignmentLink = downloadUrl;
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error uploading file: " + ex.Message);
                    TempData["error"] = "Edit failed due to file upload error!";
                    return RedirectToAction("AssignmentList", "Participation", new { Areas = "", CourseID = assignment.CourseID });
                }

                HttpContext.Session.Remove("courseid");
                _dataContext.Assignment.Add(assignment);
                await _dataContext.SaveChangesAsync();
                TempData["success"] = "Create Assignment successfully!";
                return RedirectToAction("AssignmentList", "Participation", new { Areas = "", CourseID = assignment.CourseID });

            }
            return RedirectToAction("CourseList", "Course", new {Areas =""});
        }
        [HttpGet]
        public async Task<IActionResult> EditAssignment(int id)
        {
            var assignment = await _dataContext.Assignment.FindAsync(id);
            var course = await _dataContext.Courses.FindAsync(assignment.CourseID);
            ViewBag.Course = course;
            if (assignment == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var model = new AssignmentViewModel();
            model.AssignmentID = id;
            model.CourseID = assignment.CourseID;
            model.Title = assignment.Title;
            model.StartDate = assignment.StartDate;
            model.DueDate = assignment.DueDate;
            model.ExistedAssignmentLink = assignment.AssignmentLink;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditAssignment(AssignmentViewModel model)
        {
            var assignment = await _dataContext.Assignment.FirstOrDefaultAsync(a => a.AssignmentID == model.AssignmentID);

            if (assignment == null)
            {
                return RedirectToAction("UserProfile", "Home");
            }
            assignment.Title = model.Title;
            if (model.DueDate < DateTime.Now || model.DueDate < model.StartDate)
            {
                TempData["error"] = "Invalid date!";
                return RedirectToAction("AssignmentList", "Participation", new { Areas = "", CourseID = assignment.CourseID });
            }
            assignment.StartDate = model.StartDate;
            assignment.DueDate = model.DueDate;
            assignment.CourseID = model.CourseID;
            if (model.AssignmentLink != null)
            {
                try
                {
                    string downloadUrl = await _fileService.UploadLectureDocument(model.AssignmentLink);
                    assignment.AssignmentLink = downloadUrl;
                    model.ExistedAssignmentLink = downloadUrl;
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error uploading file: " + ex.Message);
                    TempData["error"] = "Edit failed due to file upload error!";
                    return RedirectToAction("AssignmentList", "Participation", new { Areas = "", CourseID = assignment.CourseID });
                }
            }

            _dataContext.Update(assignment);
            await _dataContext.SaveChangesAsync();
            TempData["success"] = "Edit successful!";
            return RedirectToAction("AssignmentList", "Participation", new {Areas = "", CourseID = assignment.CourseID });
        }
        
        public async Task<IActionResult> DeleteAssignmentConfirmed(int id)
        {
            var assignment = await _dataContext.Assignment.FindAsync(id);
            if (assignment == null)
            {
                TempData["error"] = "Assignment not found!";
                return RedirectToAction("CourseList", "Course");
            }

            
            _dataContext.Assignment.Remove(assignment);
            await _dataContext.SaveChangesAsync();
            TempData["success"] = "Remove Assignment successful!";
            return RedirectToAction("AssignmentList", "Participation", new { Areas = "", CourseID = assignment.CourseID });
        }
        public async Task<IActionResult> ListAssignment(int id)
        {
            
            var submissions = await _dataContext.Submission.Where(s => s.AssignmentID == id).Include(s => s.User).ToListAsync();
            var assignment = await _dataContext.Assignment.FindAsync(id);
			var course = await _dataContext.Courses.FirstOrDefaultAsync(c => c.CourseID == assignment.CourseID);
			ViewBag.Course = course;
			if (submissions.Count == 0)
            {
                TempData["error"] = "No students have submitted their assignments yet.";
                return RedirectToAction("AssignmentList", "Participation", new { Areas = "", CourseID = assignment.CourseID });
            }
            return View(submissions);
        }
		public async Task<IActionResult> ListScore(int id)
		{
			var listScore = await _dataContext.ScoreAssignment.Include(i => i.Student).Where(s => s.AssignmentID == id).ToListAsync();
            var assignment = await _dataContext.Assignment.FindAsync(id);
            var course = await _dataContext.Courses.FirstOrDefaultAsync(c => c.CourseID == assignment.CourseID);
            ViewBag.Course = course;
            if (listScore.Count == 0)
			{
                TempData["error"] = "No students have been graded yet.";
                return RedirectToAction("AssignmentList", "Participation", new { Areas = "", CourseID = assignment.CourseID });
            }
			return View(listScore);
		}

        [HttpPost]
        public async Task<IActionResult> Score(ScoreAssignmentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index", "Home", new {Areas = ""});
            }
            var assignment = await _dataContext.Assignment.FirstOrDefaultAsync(a => a.AssignmentID == model.AssignmentID);
            var student = await _userManager.FindByIdAsync(model.StudentID);
            
			var existScore = await _dataContext.ScoreAssignment
		.FirstOrDefaultAsync(s => s.AssignmentID == model.AssignmentID && s.StudentID == model.StudentID);

			if (existScore != null)
			{
				existScore.Score = model.Score;
				_dataContext.ScoreAssignment.Update(existScore);
				await _dataContext.SaveChangesAsync();
				TempData["success"] = "Score updated successfully!";
			}
			else
            {
				var score = new ScoreAssignmentModel
				{
					AssignmentID = model.AssignmentID,
					StudentID = model.StudentID,
					Score = model.Score

				};
                 _dataContext.ScoreAssignment.Add(score);
            await _dataContext.SaveChangesAsync();
            TempData["success"] = "Successfully!";

			}
			
            
            return RedirectToAction("ListAssignment", new {id = assignment.AssignmentID});
        }
    }
}
