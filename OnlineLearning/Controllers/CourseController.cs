using Microsoft.AspNetCore.Mvc;
using OnlineLearningApp.Respositories;
﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineLearning.Models;
using OnlineLearning.Models.ViewModel;

using System.Security.Claims;
using System.Drawing.Printing;
using Firebase.Auth;
using Microsoft.CodeAnalysis;
using YourNamespace.Models;

namespace OnlineLearning.Controllers
{
    [ServiceFilter(typeof(AdminRedirectFilter))]
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
        public async Task<IActionResult> CourseList(string keyword = null, int? category = null, string level = null, int page = 1)
        {
            var coursesQuery = datacontext.Courses
                .Include(course => course.Instructor)
                .ThenInclude(instructor => instructor.AppUser)
                .AsQueryable();

            // Tìm kiếm theo keyword
            if (!string.IsNullOrEmpty(keyword))
            {
                coursesQuery = coursesQuery.Where(course =>
                    course.Title.Contains(keyword) ||
                    course.Instructor.AppUser.FirstName.Contains(keyword) ||
                    course.Instructor.AppUser.LastName.Contains(keyword));
            }

            // Lọc theo category nếu có
            if (category.HasValue)
            {
                coursesQuery = coursesQuery.Where(course => course.CategoryID == category.Value);
            }

            // Lọc theo level nếu có
            if (!string.IsNullOrEmpty(level))
            {
                coursesQuery = coursesQuery.Where(course => course.Level == level);
            }

            var totalCourses = await coursesQuery.CountAsync();
            var pageSize = 5;
            var courses = await coursesQuery
                .OrderByDescending(course => course.Rating)
                .ThenByDescending(course => course.NumberOfRate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var model = new CourseListViewModel
            {
                CourseList = courses,
                TotalPage = (int)Math.Ceiling(totalCourses / (double)pageSize),
                CurrentPage = page,
                Keyword = keyword,
                Category = category,
                Level = level
            };
            ViewBag.Keyword = keyword;
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "Instructor, Student")]
        public IActionResult UserCourse()
        {
            if (User.IsInRole("Student"))
            {
                return RedirectToAction("MyCourse", "Course", new { area = "Student"});
            }
            if (User.IsInRole("Instructor"))
            {
                return RedirectToAction("MyCourse", "Course", new { area = "Instructor"});
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet]
        public async Task<IActionResult> CourseDetail(int CourseID, int page = 1)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int pageSize = 5;

            //Tìm course
                var course = await datacontext.Courses
                         .Include(course => course.Instructor)       
                         .ThenInclude(instructor => instructor.AppUser)
                         .FirstOrDefaultAsync(c => c.CourseID == CourseID);
            if (course == null)
                {
                    return NotFound();
                }
                var isEnrolled = await datacontext.StudentCourses
                    .AnyAsync(sc => sc.CourseID == CourseID && sc.StudentID == userId);
                ViewBag.IsEnrolled = isEnrolled;
                ViewBag.UserID = userId;


            //Tìm Review của người xem
            var yourReview = await datacontext.Review
                         .Include(r => r.User)
                         .Include(r => r.Course)
                         .Where(c => c.CourseID == CourseID && c.UserID == userId)
                         .FirstOrDefaultAsync();  
            
            //Hiện List Review
            var review = await datacontext.Review
                     .Include(r => r.User)
                     .Include(r => r.Course)
                     .Where(c => c.CourseID == CourseID && (userId == null || c.UserID != userId))
                     .OrderByDescending(r => r.ReviewDate)
                     .ToListAsync();
            var pagedReviews = review.Skip((page - 1) * pageSize)
                              .Take(pageSize)
                              .ToList();
            var totalReview = review.Count();
            var totalPages = (int)Math.Ceiling(totalReview / (double)pageSize);

            var model = new CourseDetailViewModel
            {
                Course = course,
                Reviews = pagedReviews,
                TotalPage = totalPages,
                CurrentPage = page,
                YourReview = yourReview,
                //Tìm tiến độ hoàn thành của người dùng nếu có
                StudentCourses = isEnrolled
                                        ? await datacontext.StudentCourses.FirstOrDefaultAsync(sc => sc.CourseID == CourseID && sc.StudentID == userId)
                                        : null 
            };

            //bookmark
            var bookmark = datacontext.BookMark
                            .Where(bm => bm.CourseID == CourseID && bm.StudentID == userId)
                            .FirstOrDefault();
            if (bookmark == null)
            {
                model.IsMark = false;
            }
            else 
            { 
                model.IsMark = true; 
            }
            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> BookMark(int CourseID, string returnUrl = null)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var bookmark = datacontext.BookMark
                                .Where(bm => bm.CourseID == CourseID && bm.StudentID == userId)
                                .FirstOrDefault();
            if(bookmark != null)
            {
                datacontext.BookMark.Remove(bookmark);
                await datacontext.SaveChangesAsync();
                TempData["info"] = "Undo saved!";
            }
            else
            {
                bookmark = new BookMarkModel
                {
                    StudentID = userId,
                    CourseID = CourseID,
                };
                datacontext.BookMark.Add(bookmark);
                await datacontext.SaveChangesAsync();
                TempData["info"] = "Saved!";
            }
            return Redirect(returnUrl);
        }

        public IActionResult Create()
        {
            return View();
        }

    }
}
