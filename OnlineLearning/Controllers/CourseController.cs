using Microsoft.AspNetCore.Mvc;
using OnlineLearningApp.Respositories;
﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineLearning.Models;
using OnlineLearning.Models.ViewModel;

using System.Security.Claims;

namespace OnlineLearning.Controllers
{
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
        public async Task<IActionResult> CourseList()
        {
            var courses = await datacontext.Courses
                     .Include(course => course.Instructor)
                     .ThenInclude(instructor => instructor.AppUser)
                     .ToListAsync();

            return View(courses);

        }

        [HttpGet]
        public async Task<IActionResult> Search(string keyword)
        {
            var courses = await datacontext.Courses
                .Include(course => course.Instructor) 
                .ThenInclude(instructor => instructor.AppUser) 
                .Where(course => course.Title.Contains(keyword) ||
                                 course.Instructor.AppUser.FirstName.Contains(keyword) ||
                                 course.Instructor.AppUser.LastName.Contains(keyword))
                .ToListAsync();
            ViewBag.Keyword = keyword;
            return View("CourseList", courses); 
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
        public async Task<IActionResult> CourseDetail(int CourseID)
        {
            var course = await datacontext.Courses
                     .Include(course => course.Instructor)       
                     .ThenInclude(instructor => instructor.AppUser)
                     .FirstOrDefaultAsync(c => c.CourseID == CourseID);

            if (course == null)
            {
                return NotFound();
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var isEnrolled = await datacontext.StudentCourses
                .AnyAsync(sc => sc.CourseID == CourseID && sc.StudentID == userId);

            ViewBag.IsEnrolled = isEnrolled;

            return View(course);
        }


        public IActionResult Create()
        {
            return View();
        }

    }
}
