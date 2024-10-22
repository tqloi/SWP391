using Firebase.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineLearning.Models;
using OnlineLearning.Models.ViewModel;

using OnlineLearning.Services;
using OnlineLearningApp.Respositories;
using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Security.Claims;
using System.Web.Helpers;

namespace OnlineLearning.Controllers
{

    [Authorize(Roles = "Student, Instructor" )]
    public class PaymentController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DataContext datacontext;
        private UserManager<AppUserModel> _userManager;
        private SignInManager<AppUserModel> _signInManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _configuration;
        private readonly IVnPayService _vnPayservice;

        public PaymentController(IConfiguration configuration, ILogger<HomeController> logger, DataContext context, SignInManager<AppUserModel> signInManager, UserManager<AppUserModel> userManager, IWebHostEnvironment webHostEnvironment, IVnPayService vnPayservice)
        {
            datacontext = context;
            _logger = logger;
            _signInManager = signInManager;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
            _configuration = configuration;
            _vnPayservice = vnPayservice;
        }
        [HttpGet]
        public async Task<IActionResult> PaymentConfirmation(int CourseID)
        {
            var course = await datacontext.Courses
                .FirstOrDefaultAsync(c => c.CourseID == CourseID);

            if (course == null)
            {
                return NotFound();
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);


            var payment = new PaymentViewModel
            {
                UserID = userId,
                UserFullName = $"{user.FirstName} {user.LastName}",
                CourseID = course.CourseID,
                CourseName = course.Title,
                Price = course.Price,
                PaymentDate = DateTime.Now
            };

            return View(payment);
        }

        [HttpPost]
            public async Task<IActionResult> PaymentConfirmation(PaymentViewModel model)
            {
            var user = await _userManager.FindByIdAsync(model.UserID);
            
            if (user == null) 
            {
                return NotFound();
            }
            if (user.WalletUser >= (double)model.Price)
            {
                // them hoc sinh vao khoa hoc neu thanh toan thanh cong
                var studentCourse = new StudentCourseModel
                {
                    CourseID = model.CourseID,
                    StudentID = user.Id,
                    EnrollmentDate = DateTime.Now,
                    Progress = 0,
                    CertificateStatus = "In Progress"
                };
            
                datacontext.StudentCourses.Add(studentCourse);
                var course = await datacontext.Courses.FirstOrDefaultAsync(c => c.CourseID == model.CourseID);
                course.NumberOfStudents += 1;
                await datacontext.SaveChangesAsync();
                // them vao lich su thanh toan
                var payment = new PaymentModel
                {
                    CourseID = model.CourseID,
                    StudentID = model.UserID,
                    Amount = model.Price,
                    PaymentDate = DateTime.Now,
                    Status = "Completed"
                };
                datacontext.Payment.Add(payment);
                await datacontext.SaveChangesAsync();
                // cap nhat vi cua nguoi dung
                user.WalletUser -= (double)model.Price;
                // them vao vi cua giang vien
                var instructor = await datacontext.Instructors.FirstOrDefaultAsync(i => i.InstructorID == course.InstructorID);
                var giangvien = await _userManager.FindByIdAsync(instructor.InstructorID);
                giangvien.WalletUser += (double)model.Price;
                var result = await _userManager.UpdateAsync(user);
                var result1 = await _userManager.UpdateAsync(giangvien);
                if (result.Succeeded && result1.Succeeded)
                {
                    TempData["success"] = "Payment completed successfully!";
                    return RedirectToAction("CourseDetail", "Course", new { CourseID = model.CourseID });
                }

            }
            else
            {
                var vnPayModel = new VnPaymentRequestModel
                {
                    Amount = (double)model.Price - (user.WalletUser ?? 0.0),
                    CreatedDate = DateTime.Now,
                    Description = "thanh toan cho khoa hoc",
                    FullName = model.UserFullName,
                    OrderID = new Random().Next(1000, 100000)
                };
                HttpContext.Session.SetString("UserWallet", user.WalletUser.ToString());
                HttpContext.Session.SetInt32("CourseId", model.CourseID);
               // HttpContext.Session.SetString("UserId", model.UserID);
                HttpContext.Session.SetString("Price", model.Price.ToString());
                return Redirect(_vnPayservice.CreatePaymentUrl(HttpContext, vnPayModel));
            }
            return View(model);
            }

        [Authorize]
        public async Task<IActionResult> PaymentRollBack()
        {
            var response = _vnPayservice.PaymentExecute(Request.Query);

            if (response == null || response.VnPayResponseCode != "00")
            {
                TempData["error"] = $"Lỗi thanh toán VN Pay: {response.VnPayResponseCode}";
                return RedirectToAction("ErrorPayment");
            }
            var userid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userid);
            // thanh toan thanh cong
            int courseid = (int)HttpContext.Session.GetInt32("CourseId");
            //var userid = HttpContext.Session.GetString("UserId");
            double price = -1;
            string pricevalue = HttpContext.Session.GetString("Price");
            if (double.TryParse(pricevalue, out price))
            {
                var payment = new PaymentModel
                {
                    CourseID = courseid,
                    StudentID = userid,
                    Amount = (decimal)price,
                    PaymentDate = DateTime.Now,
                    Status = "Completed"
                };
                datacontext.Payment.Add(payment);
                await datacontext.SaveChangesAsync();
            }
            var course = await datacontext.Courses.FindAsync(courseid);
            var notify = new NotificationModel
            {
                UserId = userid,
                Description = $"{user.UserName} just paid for the Course: {course.Title}",
                CreatedAt = DateTime.Now
            };
            datacontext.Notification.Add(notify);
                var studentCourse = new StudentCourseModel
                {
                    CourseID = courseid,
                    StudentID = userid,
                    EnrollmentDate = DateTime.Now,
                    Progress = 0,
                    CertificateStatus = "In Progress"
                };

                datacontext.StudentCourses.Add(studentCourse);
                
                course.NumberOfStudents += 1;
                await datacontext.SaveChangesAsync();
            string getwallet = HttpContext.Session.GetString("UserWallet");
            if(getwallet != null)
            {
                double userwallet = double.Parse(getwallet);
                user.WalletUser -= userwallet;
                datacontext.Users.Update(user);
                await datacontext.SaveChangesAsync();
            }


            HttpContext.Session.Remove("UserWallet");
                 HttpContext.Session.Remove("CourseId");
                 HttpContext.Session.Remove("Price");
                TempData["success"] = "Payment completed successfully!";
            // them vao vi cua giang vien
            var instructor = await datacontext.Instructors.FirstOrDefaultAsync(i => i.InstructorID == course.InstructorID);
            var giangvien = await _userManager.FindByIdAsync(instructor.InstructorID);
            giangvien.WalletUser += (double)price;
            var result = await _userManager.UpdateAsync(giangvien);
            if (result.Succeeded)
            {
                TempData["success"] = "successfully!";
                return RedirectToAction("CourseDetail", "Course", new { CourseID = studentCourse.CourseID });
            }
            return RedirectToAction("CourseDetail", "Course", new { CourseID = studentCourse.CourseID });



            
            }
             public IActionResult ErrorPayment()
            {
                return View();
             }
        [HttpGet]
        [Authorize]
        
        public async Task<IActionResult> RequestWithdraw()
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            var request = new RequestTransferViewModel
            {
                UserID = user.Id,
                Status = "In processing",
                CreateAtTime = DateTime.Now
            };
            return View(request);
        }
        [HttpPost]
        public async Task<IActionResult> RequestWithdraw(RequestTransferViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserID);
            if (user == null)
            {
                TempData["error"] = "User not exist!";
                return RedirectToAction("UserProfile", "Home", new { Areas = "" });
            }
            if (model.MoneyNumber > user.WalletUser)
            {
                TempData["error"] = "Your wallet don't have enough money";
                return View(model);
            }
            else
            {
                var notify = new NotificationModel
                {
                    UserId = user.Id,
                    Description = $"{user.UserName} has just requested to withdraw {model.MoneyNumber} VND",
                    CreatedAt = DateTime.Now
                };
                datacontext.Notification.Add(notify);
                var request = new RequestTranferModel
                {
                    UserID = model.UserID,
                    Status = "In processing",
                    FullName = model.FullName,
                    BankName = model.BankName,
                    AccountNumber = model.AccountNumber,
                    MoneyNumber = model.MoneyNumber,
                    CreateAt = DateTime.Now
                };
                datacontext.RequestTranfer.Add(request);
                await datacontext.SaveChangesAsync();
                TempData["success"] = "Request successfully!";
                return RedirectToAction("Index", "Home", new { Areas = "" });
            }



        }
        public IActionResult ListRequest()
        {
            return View();
        }

    }

    }

