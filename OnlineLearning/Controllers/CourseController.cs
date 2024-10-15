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


        [HttpPost]
        [Authorize]
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
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                //notification for admin
                var notification = new NotificationModel
                {
                    UserId = userId,
                    Description = $"{user.UserName} has just created a courses",
                    CreatedAt = DateTime.Now
                };

                datacontext.Notification.Add(notification);
                await datacontext.SaveChangesAsync();
            }
            TempData["success"] = "Course created successfully!";
            //return RedirectToAction("Index", "Instructor", new { area = "Instructor" });
            return RedirectToAction("MyCourse", "Course");
        }

        public IActionResult Create()
        {
            return View();
        }

    }
}
