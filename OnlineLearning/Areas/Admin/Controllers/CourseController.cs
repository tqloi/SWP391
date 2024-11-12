using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineLearning.Areas.Admin.Models.ViewModel;
using OnlineLearning.Models;
using OnlineLearningApp.Respositories;

namespace OnlineLearning.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]

    [Route("Admin/[controller]/[action]")]
    public class CourseController : Controller
    {
        private UserManager<AppUserModel> _userManager;
        private SignInManager<AppUserModel> _signInManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly DataContext _dataContext;
        public CourseController(DataContext dataContext, UserManager<AppUserModel> userManager, SignInManager<AppUserModel> signInManager, IWebHostEnvironment webHostEnvironment)
        {
            _dataContext = dataContext;
            _userManager = userManager;
            _signInManager = signInManager;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<IActionResult> Index()
        {
            var courses = await _dataContext.Courses.Include(c => c.Instructor).ThenInclude(i => i.AppUser).ToListAsync();
            return View(courses);
        }
        public async Task<IActionResult> GetCourse(string Comment)
        {
            var match = Regex.Match(Comment, @"\[(.*?)\]");
            var coursename = "";
            if (match.Success)
            {
                var result = match.Groups[1].Value;
                coursename = result.ToString();
            }
            var course = await _dataContext.Courses.FirstOrDefaultAsync(c => c.Title.Equals(coursename));
            return View(course);
        }
        public async Task<IActionResult> SetStatusCourse(int Id)
        {
            var course = await _dataContext.Courses.FindAsync(Id);
            if (course == null)
            {
                TempData["error"] = "Course Not Found";
                return RedirectToAction("Index", "Course");
            }
            if (course.Status == true)
            {
                course.Status = false;
                course.IsBaned = true;
                await _dataContext.SaveChangesAsync();
            }
            else if (course.Status == false)
            {
                course.Status = true;
                course.IsBaned = false;
                await _dataContext.SaveChangesAsync();
            }
                TempData["success"] = "Set Status Successful!";
                return RedirectToAction("Index", "Course");
            }
        [HttpGet]
        public async Task<IActionResult> AddCategory()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddCategory(CategoryViewModel model) 
        {
            if(model == null)
            {
                return View(model);
            }
            if (!ModelState.IsValid)
            {
                TempData["error"] = "Something wrong!";
            }
            else
            {
                var category = new CategoryModel
                {
                    FullName = model.FullName,
                    Description = model.Description
                };
                _dataContext.Category.Add(category);
                await _dataContext.SaveChangesAsync();
                TempData["success"] = "Successfully!";
                return RedirectToAction("ViewCategory", "Course", new { area = "Admin" });
            }
            return View(model);
        }
        public IActionResult ViewCategory()
        {
            var categories = _dataContext.Category.ToList();
            return View(categories);
        }
        [HttpGet]
        public async Task<IActionResult> EditCategory(int id)
        {
            var category = await _dataContext.Category.FindAsync(id);
            if (category == null)
            {
                TempData["error"] = "Something wrong";
                return RedirectToAction("ViewCategory", "Course", new { area = "Admin" });
            }
            var model = new CategoryViewModel
            {
                FullName = category.FullName,
                Description = category.Description
            };
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> EditCategory(CategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var category = new CategoryModel
                {
                    FullName = model.FullName,
                    Description = model.Description
                };
                _dataContext.Category.Update(category);
                await _dataContext.SaveChangesAsync();
                TempData["success"] = "Edit successfully!";
                return RedirectToAction("ViewCategory", "Course", new { area = "Admin" });
            }
            return View(model);
        }
        public async Task<IActionResult> RemoveCate(int id)
        {
            var category = await _dataContext.Category.FindAsync(id);
            if (category == null)
            {
                TempData["error"] = "Something wrong";
                return RedirectToAction("ViewCategory", "Course", new { area = "Admin" });
            }
            _dataContext.Category.Remove(category);
            await _dataContext.SaveChangesAsync();
            TempData["success"] = "Remove successfully!";
            return RedirectToAction("ViewCategory", "Course", new { area = "Admin" });
        }

        public async Task<IActionResult> Top5Course()
        {
            List<Top5CourseViewModel> top5 = new List<Top5CourseViewModel>();
            var listcourse = _dataContext.StudentCourses.ToList();
            int size = listcourse.Count;
            if (size > 0)
            {
                for (int i = 0; i < size; i++)
                {
                    Top5CourseViewModel user1 = new Top5CourseViewModel();
                    int count = 1;
                    for (int j = i + 1; j < size; j++)
                    {
                        if (listcourse[i].CourseID == listcourse[j].CourseID)
                        {
                            count++;
                        }

                    }
                    user1.Count = count;
                    var course = await _dataContext.Courses.FirstOrDefaultAsync(c => c.CourseID == listcourse[i].CourseID);
                    var user = await _userManager.FindByIdAsync(course.InstructorID);
                    user1.NameInstructor = user.FirstName + user.LastName;
                    user1.Images = course.CoverImagePath;
                    user1.Createdate = course.CreateDate;
                    user1.Price = course.Price;
                    user1.Title = course.Title;
                    
                    top5.Add(user1);
                    user1 = null;
                    count = 1;

                }
                top5 = top5.GroupBy(u => u.CourseID).Select(g => g.First()).OrderByDescending(x => x.Count).ToList();
            }

            return View(top5);
        }
    }
}

