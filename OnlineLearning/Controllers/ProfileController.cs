using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineLearning.Models.ViewModel;
using OnlineLearning.Models;
using Microsoft.AspNetCore.Identity;
using OnlineLearningApp.Respositories;
using System.Security.Claims;
using OnlineLearning.Services;
using OnlineLearning.Email;

namespace OnlineLearning.Controllers
{
    [Authorize]
    [ServiceFilter(typeof(AdminRedirectFilter))]
    public class ProfileController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DataContext datacontext;
        private UserManager<AppUserModel> _userManager;
        private SignInManager<AppUserModel> _signInManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly FileService _fileService;
        private IConfiguration _configuration;
        public ProfileController(ILogger<HomeController> logger, DataContext context, SignInManager<AppUserModel> signInManager, UserManager<AppUserModel> userManager, IWebHostEnvironment webHostEnvironment, FileService fileService, IConfiguration configuration)
        {
            datacontext = context;
            _logger = logger;
            _signInManager = signInManager;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
            _fileService = fileService;
            _configuration = configuration;
        }
   
        [HttpGet]
        public async Task<IActionResult> UserProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            var model = new EditUserViewModel
            {
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                ExistingProfileImagePath = user.ProfileImagePath,
                Address = user.Address,
                Dob = user.Dob,
                gender = user.Gender,
                WalletUser = (double)user.WalletUser
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit()
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
        public async Task<IActionResult> Edit(EditUserViewModel model)
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
                if (message != null)
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
                    return RedirectToAction("UserProfile", "Profile");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            TempData["error"] = "Edit failed! Something wrong";
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ViewUserProfile(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            var model = new InstructorProfileViewModel
            {
                UserId = user.Id,
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                ExistingProfileImagePath = user.ProfileImagePath,
                Address = user.Address,
                Dob = user.Dob,
                gender = user.Gender,

            };

            return View(model);
        }
    }
}
