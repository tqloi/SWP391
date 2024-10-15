using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineLearning.Controllers;
using OnlineLearning.Models;
using OnlineLearning.Models.ViewModel;
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

        public CourseController(DataContext context, UserManager<AppUserModel> userManager, IWebHostEnvironment webHostEnvironment)
        {
            datacontext = context;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
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
                //Define the upload path (wwwroot/Images)
                string uploadpath = Path.Combine(_webHostEnvironment.WebRootPath, "Images");

                //Generate a unique image file name, this shit(Guid.NewGuid())
                string imagename = Guid.NewGuid() + "_" + model.CoverImage.FileName;

                //combine em
                string filepath = Path.Combine(uploadpath, imagename);

                //idk why?
                using (var fs = new FileStream(filepath, FileMode.Create))
                {
                    await model.CoverImage.CopyToAsync(fs);
                }
                course.CoverImagePath = imagename;
            }
            else { course.CoverImagePath = ""; }

            datacontext.Courses.Add(course);
            await datacontext.SaveChangesAsync();

            int newCourseId = course.CourseID;

            if (model.CourseMaterials != null && model.CourseMaterials.Count > 0)
            {
                string uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "CourseMaterials");
                foreach (var file in model.CourseMaterials)
                {
                    string fileName = Guid.NewGuid() + "_" + file.FileName;
                    string filePath = Path.Combine(uploadPath, fileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }
                    var material = new CourseMaterialModel
                    {
                        CourseID = newCourseId,
                        MaterialsLink = fileName
                    };
                    datacontext.CourseMaterials.Add(material);
                    await datacontext.SaveChangesAsync();
                }
            }

            TempData["success"] = "Course created successfully!";
            //return RedirectToAction("Index", "Instructor", new { area = "Instructor" });
            return RedirectToAction("MyCourse", "Course");
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
                course.CategoryID = model.CategoryID;

            if (model.Level != "none")
                course.Level = model.Level;

            if (model.EndDate != null)
            {
                course.EndDate = model.EndDate;
            }
            course.Level = model.Level;
            course.LastUpdate = DateTime.Now;

            if (model.CoverImage != null)
            {
                string uploadpath = Path.Combine(_webHostEnvironment.WebRootPath, "Images");
                string imagename = Guid.NewGuid() + "_" + model.CoverImage.FileName;
                string filepath = Path.Combine(uploadpath, imagename);

                using (var fs = new FileStream(filepath, FileMode.Create))
                {
                    await model.CoverImage.CopyToAsync(fs);
                }
                course.CoverImagePath = imagename;
            }

            await datacontext.SaveChangesAsync();

            TempData["success"] = "Update created successfully!";
            //return RedirectToAction("Index", "Instructor", new { area = "Instructor" });
            return RedirectToAction("MyCourse", "Course");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int CourseId)
        {
            var course = await datacontext.Courses.FindAsync(CourseId);
            if (course == null)
            {
                TempData["error"] = "Course not found!";
                return RedirectToAction("MyCourse", "Course");
            }

            datacontext.Courses.Remove(course);
            await datacontext.SaveChangesAsync();

            TempData["success"] = "Course deleted successfully!";
            return RedirectToAction("MyCourse", "Course");
        }

        [HttpPost]
        public async Task<IActionResult> SetSate(int CourseId)
        {
            var course = await datacontext.Courses.FindAsync(CourseId);
            if (course == null)
            {
                TempData["error"] = "Course not found!";
                return RedirectToAction("MyCourse", "Course");
            }

            course.Status = !course.Status;
            course.LastUpdate = DateTime.Now;
            await datacontext.SaveChangesAsync();

            TempData["success"] = course.Status ? "Course enabled successfully!" : "Course disable successfully!";
            return RedirectToAction("MyCourse", "Course");
        }
    }
}
