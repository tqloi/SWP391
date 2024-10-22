using Firebase.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineLearning.Controllers;
using OnlineLearning.Models;
using OnlineLearning.Models.ViewModel;
using OnlineLearning.Services;
using OnlineLearningApp.Respositories;
using System.Security.Claims;

namespace OnlineLearning.Areas.Instructor.Controllers
{
    [Area("Instructor")]
    [Authorize]
    [Route("/[controller]/[action]")]
    public class CourseController : Controller
    {
        private readonly DataContext datacontext;
        private UserManager<AppUserModel> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly FileService _fileService;

        public CourseController(DataContext context, UserManager<AppUserModel> userManager, IWebHostEnvironment webHostEnvironment, FileService fileService)
        {
            datacontext = context;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
            _fileService = fileService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CourseViewModel model)
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var course = new CourseModel
            {
                Title = model.Title,
                Description = model.Description,
                CourseCode = model.CourseCode,
                CategoryID = model.CategoryId,
                Level = model.Level,
                EndDate = model.EndDate,
                Price = model.Price,
                CreateDate = DateTime.Now,
                LastUpdate = DateTime.Now,
                NumberOfStudents = 0,
                NumberOfRate = 0,
                InstructorID = userId,
                //Status = true
            };
            //Check if a cover image is provided
            if (model.CoverImage != null)
            {
                try
                {
                    string downloadUrl = await _fileService.UploadImage(model.CoverImage);
                    course.CoverImagePath = downloadUrl;
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error uploading file: " + ex.Message);
                    TempData["error"] = "Edit failed due to file upload error!";
                    return View(model);
                }
            }
            else { course.CoverImagePath = "/Images/faq_graphic.jpg"; }

            datacontext.Courses.Add(course);
            await datacontext.SaveChangesAsync();

            int newCourseId = course.CourseID;

            if (model.CourseMaterials != null && model.CourseMaterials.Count > 0)
            {
                foreach (var file in model.CourseMaterials)
                {
                    var material = new CourseMaterialModel();
                    string fileName = file.FileName;
                    try
                    {
                        string downloadUrl = await _fileService.UploadCourseDocument(file);
                        material.MaterialsLink = downloadUrl;
                        material.CourseID = newCourseId;
                        material.FIleName = fileName;

                        datacontext.CourseMaterials.Add(material);
                        await datacontext.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", "Error uploading file: " + ex.Message);
                        TempData["error"] = "Edit failed due to file upload error!";
                        return View(model);
                    }
                }
            }

            TempData["success"] = "Course created successfully!";
            //return RedirectToAction("Index", "Instructor", new { area = "Instructor" });
            return RedirectToAction("CourseInfo", "Participation", new { CourseID = newCourseId });
        }

        [HttpPost]
        public async Task<IActionResult> Update(CourseViewModel model)
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var course = await datacontext.Courses.FindAsync(model.CourseID);

            if (course == null)
            {
                return NotFound();
            }
            course.Title = model.Title;
            course.Description = model.Description;
            course.CourseCode = model.CourseCode;
            course.Price = model.Price;

            if (model.CategoryID != 0)
            {
                course.CategoryID = model.CategoryID;
            }
            if (model.Level != "none")
            {
                course.Level = model.Level;
            }
            if (model.EndDate != DateTime.MinValue)
            {
                course.EndDate = model.EndDate;
            }
            course.LastUpdate = DateTime.Now;

            if (model.CoverImage != null)
            {
                try
                {
                    //await _fileService.DeleteFile(course.CoverImagePath);
                    string downloadUrl = await _fileService.UploadImage(model.CoverImage);
                    course.CoverImagePath = downloadUrl;
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error uploading file: " + ex.Message);
                    TempData["error"] = "Edit failed due to file upload error!";
                    return RedirectToAction("MyCourse", "Course", new { area = "" });
                }
            }

            await datacontext.SaveChangesAsync();

            TempData["success"] = "Update created successfully!";
            //return RedirectToAction("Index", "Instructor", new { area = "Instructor" });
            return RedirectToAction("InstructorCourse", "Course",  new { area = "Instructor" });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int CourseId)
        {
            var course = await datacontext.Courses.FindAsync(CourseId);
            if (course == null)
            {
                TempData["error"] = "Course not found!";
                return RedirectToAction("InstructorCourse", "Course", new { area = "Instructor" });
            }

            datacontext.Courses.Remove(course);
            await datacontext.SaveChangesAsync();

            TempData["success"] = "Course deleted successfully!";
            return RedirectToAction("InstructorCourse", "Course", new { area = "Instructor" });
        }

        [HttpPost]
        public async Task<IActionResult> SetSate(int CourseId)
        {
            var course = await datacontext.Courses.FindAsync(CourseId);
            if (course == null)
            {
                TempData["error"] = "Course not found!";
                return RedirectToAction("InstructorCourse", "Course", new { area = "Instructor" });
            }

            course.Status = !course.Status;
            course.LastUpdate = DateTime.Now;
            await datacontext.SaveChangesAsync();

            TempData["success"] = course.Status ? "Course enabled successfully!" : "Course disable successfully!";
            return RedirectToAction("InstructorCourse", "Course", new { area = "Instructor" });
        }

        [HttpGet]
        public async Task<IActionResult> InstructorCourse()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var model = new ListViewModel();

                model.Courses = await datacontext.Courses
                    .Where(course => course.InstructorID == userId)
                    .Include(course => course.Instructor)
                    .ThenInclude(instructor => instructor.AppUser)
                    .OrderByDescending(sc => sc.CourseID)
                    .ToListAsync();

            return View(model);
        }
    }
}
