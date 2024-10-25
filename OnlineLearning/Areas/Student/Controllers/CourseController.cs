using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineLearning.Controllers;
using OnlineLearning.Models;
using OnlineLearning.Models.ViewModel;
using OnlineLearningApp.Respositories;
using System.Security.Claims;

namespace OnlineLearning.Areas.Student.Controllers
{
    [Area("Student")]
    [Authorize(Roles = "Student")]
    [Route("Student/[controller]/[action]")]
    public class CourseController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DataContext datacontext;
        private UserManager<AppUserModel> _userManager;
        private SignInManager<AppUserModel> _signInManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CourseController(ILogger<HomeController> logger, DataContext context, SignInManager<AppUserModel> signInManager, UserManager<AppUserModel> userManager, IWebHostEnvironment webHostEnvironment)
        {
            datacontext = context;
            _logger = logger;
            _signInManager = signInManager;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> MyCourse(int? category = null, string level = null, string status = null, int page = 1)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var model = new ListViewModel();
            if (User.IsInRole("Student"))
            {
                var studentCourseQuery =datacontext.StudentCourses
                                .Where(sc => sc.StudentID == userId)
                                .Include(sc => sc.Course)
                                .AsQueryable();
                var courseQuery = (status == "Saved")
                                        ? datacontext.Bookmark
                                            .Where(sc => sc.StudentID == userId)
                                            .Include(sc => sc.Course)
                                            .ThenInclude(course => course.Instructor)
                                            .ThenInclude(instructor => instructor.AppUser)
                                            .Select(sc => sc.Course)
                                            .OrderByDescending(sc => sc.CourseID)
                                            .AsQueryable()
                                        : datacontext.StudentCourses
                                            .Where(sc => sc.StudentID == userId)
                                            .Include(sc => sc.Course)
                                            .ThenInclude(course => course.Instructor)
                                            .ThenInclude(instructor => instructor.AppUser)
                                            .Select(sc => sc.Course)
                                            .OrderByDescending(sc => sc.CourseID)
                                            .AsQueryable();
                var saveCourseQuery = datacontext.Bookmark
                                        .Where(sc => sc.StudentID == userId)
                                        .Include(sc => sc.Course)
                                        .OrderByDescending(sc => sc.CourseID)
                                        .AsQueryable();


                // Lọc theo category nếu có
                if (category.HasValue)
                {
                    studentCourseQuery = studentCourseQuery.Where(sc => sc.Course.CategoryID == category.Value);
                    courseQuery = courseQuery.Where(c => c.CategoryID == category.Value);
                    saveCourseQuery = saveCourseQuery.Where(sc => sc.Course.CategoryID == category.Value);
                }

                // Lọc theo level nếu có
                if (!string.IsNullOrEmpty(level))
                {
                    studentCourseQuery = studentCourseQuery.Where(sc => sc.Course.Level == level);
                    courseQuery = courseQuery.Where(c => c.Level == level);
                    saveCourseQuery = saveCourseQuery.Where(sc => sc.Course.Level == level);
                }

                if(!string.IsNullOrEmpty(status) && status != "Saved")
                {
                    studentCourseQuery = studentCourseQuery.Where(sc => sc.CertificateStatus == status);
                }

                var pageSize = 2;

                //Tìm mây bookmark nếu như bấm Saved
                if (status == "Saved")
                {
                    var totalBookmark = await saveCourseQuery.CountAsync();
                    var bookmark = await saveCourseQuery
                                            .Skip((page - 1) * pageSize)
                                            .Take(pageSize)
                                            .ToListAsync();

                    model.Bookmarks = bookmark;
                    model.TotalPage = (int)Math.Ceiling(totalBookmark / (double)pageSize);
                }
                    
                else
                {

                    var totalCourses = await studentCourseQuery.CountAsync();

                    var StudentCourses = await studentCourseQuery
                        .OrderBy(course => course.EnrollmentDate)
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToListAsync();


                    model.StudentCourses = StudentCourses;
                    model.TotalPage = (int)Math.Ceiling(totalCourses / (double)pageSize);
                }
                model.Courses = await courseQuery.ToListAsync();
                model.CurrentPage = page;
                model.Level = level;
                model.Category = category;
                model.Status = status;
                
            }
            return View(model);
        }
    }
}
