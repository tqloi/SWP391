using System.Diagnostics;
using System.Security.Claims;
using System.Web.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineLearning.Areas.Admin.Models.ViewModel;
using OnlineLearning.Email;
using OnlineLearning.Models;
using OnlineLearning.Models.ViewModel;
using OnlineLearning.Services;
using OnlineLearningApp.Respositories;
using AuthorizeAttribute = Microsoft.AspNetCore.Authorization.AuthorizeAttribute;
using Controller = Microsoft.AspNetCore.Mvc.Controller;
using HttpGetAttribute = Microsoft.AspNetCore.Mvc.HttpGetAttribute;
using HttpPostAttribute = Microsoft.AspNetCore.Mvc.HttpPostAttribute;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;
using ValidateAntiForgeryTokenAttribute = Microsoft.AspNetCore.Mvc.ValidateAntiForgeryTokenAttribute;

namespace OnlineLearning.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]

    [Route("/[controller]/[action]")]
    public class AdminController : Controller
    {
        private UserManager<AppUserModel> _userManager;
        private SignInManager<AppUserModel> _signInManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly DataContext _dataContext;
        private IConfiguration _configuration;
        private readonly FileService _fileService;
        public AdminController(DataContext dataContext, UserManager<AppUserModel> userManager, SignInManager<AppUserModel> signInManager, IWebHostEnvironment webHostEnvironment, IConfiguration configuration, FileService fileService)
        {
            _dataContext = dataContext;
            _userManager = userManager;
            _signInManager = signInManager;
            _webHostEnvironment = webHostEnvironment;
            _configuration = configuration;
            _fileService = fileService;
        }
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var users = await _dataContext.Users.ToListAsync();
            var payments = await _dataContext.Payment.OrderByDescending(p => p.PaymentDate).ToListAsync();
            var listindex = new ListIndexViewModel
            {
                ListUser = users,
                ListPayment = payments,
            };
            return View(listindex);
        }

        public async Task<IActionResult> AdminProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }
            AdminProfileViewModel model = new AdminProfileViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Dob = user.Dob,
                Address = user.Address,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                ExistingProfileImagePath = user.ProfileImagePath

            };
            return View(model);
        }
        
        public async Task<IActionResult> Changepass()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }
            return View(new ChangePasswordViewModel {Username = user.UserName});

        }
        [HttpPost]
        public async Task<IActionResult> Changepass(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid) {
                var user = await _userManager.FindByNameAsync(model.Username);
                
                if(!await _userManager.CheckPasswordAsync(user, model.OldPassword))
                {
                    TempData["error"] = "Invalid Old Password!";
                    return View(model);
                }
                var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                if (result.Succeeded)
                {
                 TempData["success"] = "Password changed successfully!";
                return RedirectToAction("AdminProfile");
                  }


            }
            TempData["error"] = "Something is wrong!";
                return View(model);

        }

            

        

        [HttpGet]
        public async Task<IActionResult> AdminEdit()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            var model = new EditUserViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                ExistingProfileImagePath = user.ProfileImagePath,
                Address = user.Address,
                Dob = user.Dob,
                gender = user.Gender,
            };


            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdminEdit(EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = await _userManager.FindByIdAsync(userId);

                if (user == null)
                {
                    return NotFound();
                }
                if (!model.Email.Equals(user.Email))
                {
                    var existemail = await _userManager.FindByEmailAsync(model.Email);
                    if (existemail != null)
                    {
                        TempData["error"] = "Email is existed!";
                        return View(model);
                    }
                    Random random = new Random();
                    int otp = random.Next(100000, 999999);

                    var emailSender = new EmailSender(_configuration);
                    await emailSender.SendEmailAsync(model.Email, "Your OTP Code", $"Your OTP code is {otp}.");

                    HttpContext.Session.SetInt32("otp", otp);
                    HttpContext.Session.SetString("changeemail", model.Email);
                    
                    return RedirectToAction("OTP_ChangeEmail", "Account", new { Areas = "" });
                }
                var message = HttpContext.Session.GetString("message");
                string New_email = HttpContext.Session.GetString("changeemail");
                if(message != null)
                {
                    if (message.Equals("emailsuccess"))
                    {
                        user.Email = New_email;
                    }
                    
                }
                else
                {
                    user.Email = model.Email;
                }

                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.UserName = model.UserName;
                user.PhoneNumber = model.PhoneNumber;
                user.Address = model.Address;
                user.Dob = model.Dob;
                user.Gender = model.gender;
                HttpContext.Session.Remove("otp");
                HttpContext.Session.Remove("changeemail");
                HttpContext.Session.Remove("message");
                if (model.ProfileImage != null)
                {
                 try
                 {
                        string downloadUrl = await _fileService.UploadImage(model.ProfileImage);
                      user.ProfileImagePath = downloadUrl;
                      
                 }
                 catch (Exception ex)
                 {
                        ModelState.AddModelError("", "Error uploading file: " + ex.Message);
                        TempData["error"] = "Edit failed due to file upload error!";
                       return View(model);
                  }
                 }
                else
                {
                    user.ProfileImagePath = user.ProfileImagePath;
                }

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {

                    TempData["success"] = "Edit successful!";

                    return RedirectToAction("AdminProfile", "Admin");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            TempData["erorr"] = "Edit failed!";
            return View(model);
        }


        public async Task<IActionResult> ViewNotification(int? page)
        {
            int pageSize = 10; // Số lượng thông báo trên mỗi trang
            int pageNumber = (page ?? 1); // Trang hiện tại, mặc định là trang 1

            var notifications = await _dataContext.Notification
                .OrderByDescending(n => n.CreatedAt)
                .Include(n => n.User)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            int totalRecords = await _dataContext.Notification.CountAsync();
            int totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

            ViewBag.CurrentPage = pageNumber;
            ViewBag.TotalPages = totalPages;

            return View(notifications);
        }




    }
}


