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

            return View("CourseList", courses); 
        }

        [HttpGet]
        [Authorize(Roles = "Instructor, Student")]
        public async Task<IActionResult> MyCourse()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var model = new ListViewModel();

            if (User.IsInRole("Admin"))
            {
                return Forbid(); 
            }
            if (User.IsInRole("Instructor"))
            {
                model.Courses = await datacontext.Courses
                    .Where(course => course.InstructorID == userId)
                    .Include(course => course.Instructor)   
                    .ThenInclude(instructor => instructor.AppUser)
                    .OrderByDescending(sc => sc.CourseID)
                    .ToListAsync();
            }
            if (User.IsInRole("Student"))
            {
                model.StudentCourses = await datacontext.StudentCourses
                    .Where(sc => sc.StudentID == userId)
                    .Include(sc => sc.Course)
                    .ToListAsync();
                model.Courses = await datacontext.StudentCourses
                    .Where(sc => sc.StudentID == userId)
                    .Include(sc => sc.Course)
                    .ThenInclude(course => course.Instructor)      
                    .ThenInclude(instructor => instructor.AppUser)
                    .Select(sc => sc.Course)
                    .OrderByDescending(sc => sc.CourseID)
                    .ToListAsync();
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> CourseDetail(int id)
        {
            var course = await datacontext.Courses
                     .Include(course => course.Instructor)       
                     .ThenInclude(instructor => instructor.AppUser)
                     .FirstOrDefaultAsync(c => c.CourseID == id);

            if (course == null)
            {
                return NotFound();
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var isEnrolled = await datacontext.StudentCourses
                .AnyAsync(sc => sc.CourseID == id && sc.StudentID == userId);

            ViewBag.IsEnrolled = isEnrolled;

            return View(course);
        }
    }
}
