using System.Security.Claims;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using OnlineLearning.Email;
using OnlineLearning.Models;
using OnlineLearning.Models.ViewModel;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace OnlineLearning.Controllers
{
    public class AccountController : Controller
    {
        private UserManager<AppUserModel> _userManager;
        private SignInManager<AppUserModel> _signInManager;
        private IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;
       
        public AccountController(SignInManager<AppUserModel> signInManager, UserManager<AppUserModel> userManager, IWebHostEnvironment webHostEnvironment, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _webHostEnvironment = webHostEnvironment;
            _configuration = configuration;
           
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginVM)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(loginVM.Username);
                bool checkVerify = await _userManager.IsEmailConfirmedAsync(user);
                if (checkVerify)
                {
                    var result = await _signInManager.PasswordSignInAsync(loginVM.Username, loginVM.Password, loginVM.RememberMe, false);
                    if (result.Succeeded)
                    {
                        TempData["success"] = "Login successful!";
                         return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        TempData["error"] = "Login failed!";
                        ModelState.AddModelError("", "Username or Password are incorrect!");
                        return View(loginVM);
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Your need verify your email before login!");
                    return RedirectToAction("EnterOTP", "Account");
                }
                
            }
            return View(loginVM);

        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var emailexist = await _userManager.FindByEmailAsync(model.Email);
                if (emailexist == null)
                {


                    var user = new AppUserModel
                    {
                        UserName = model.Username,
                        Email = model.Email,
                        PhoneNumber = model.PhoneNumber,
                        ProfileImagePath = "piccl.jpg"
                    };

                    var result = await _userManager.CreateAsync(user, model.Password);



                    if (result.Succeeded)
                    {
                        //// Tạo token xác nhận email
                        //var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                        //// Tạo đường dẫn xác nhận email
                        //var confirmationLink = Url.Action("ConfirmEmail", "Account",
                        //    new { userId = user.Id, token = token }, Request.Scheme);

                        //var message = $"Please confirm your account by <a href='{confirmationLink}'>clicking here</a>.";

                        //// Gửi email xác nhận
                        //var emailSender = new EmailSender(_configuration);
                        //await emailSender.SendEmailAsync(user.Email, "Confirm your email", message);

                        //TempData["success"] = "Registration successful! Please check your email to confirm your account.";
                        //return RedirectToAction("Login", "Account");
                        Random random = new Random();
                        int otp = random.Next(100000, 999999);

                        var emailSender = new EmailSender(_configuration);
                        await emailSender.SendEmailAsync(user.Email, "Your OTP Code", $"Your OTP code is {otp}.");

                        HttpContext.Session.SetInt32("otp", otp);
                        HttpContext.Session.SetString("userId", user.Id);

                        return RedirectToAction("EnterOtp");
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
			TempData["error"] = "Failed!";
			return View(model);
        }
        [HttpGet]
        public IActionResult EnterOTP()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> EnterOTP(OTPViewModel model)
        {
            var storedOtp = HttpContext.Session.GetInt32("otp");
            var userId = HttpContext.Session.GetString("userId");
            var usernamess = HttpContext.Session.GetString("username");
            if (storedOtp != null)
            {
                if (usernamess != null)
                {
                    return RedirectToAction("ChangePassword", new { username = usernamess });
                }
                var user = await _userManager.FindByIdAsync(userId);
                if (model.Otp == storedOtp)
                {
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var result = await _userManager.ConfirmEmailAsync(user, token);
                    if (result.Succeeded)
                    {
                        HttpContext.Session.Remove("userId");
                        HttpContext.Session.Remove("otp");
                        TempData["success"] = "Sign in successful!";

                        return RedirectToAction("Login", "Account");
                    }

                }

                else
                {
                    TempData["error"] = "Sign in failed!";
                    ModelState.AddModelError("", "Invalid OTP.");
                    return View();
                }
            }
            ModelState.AddModelError("", "Invalid OTP.");
            return View("EnterOtp");
        }
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            TempData["success"] = "Logout successful!";
            return RedirectToAction("Login", "Account");
        }
        public IActionResult VerifyEmail()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> VerifyEmail(VerifyEmailViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    ModelState.AddModelError("", "Something is wrong!");
                    return View(model);
                }
                else
                {
                    Random random = new Random();
                    int otp = random.Next(100000, 999999);

                    var emailSender = new EmailSender(_configuration);
                    await emailSender.SendEmailAsync(user.Email, "Your OTP Code", $"Your OTP code is {otp}.");

                    HttpContext.Session.SetInt32("otp", otp);
                    HttpContext.Session.SetString("username", user.UserName);

                    return RedirectToAction("EnterOtp");
                   // return RedirectToAction("ChangePassword", new { username = user.UserName });
                }

            }

            return View(model);
        }
        public IActionResult ChangePassword(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("VerifyEmail", "Account");
            }
            
            return View(new ResetPasswordViewModel { Username = username });
        }
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ResetPasswordViewModel model)
        {
            //if (ModelState.IsValid)
            //{
                var user = await _userManager.FindByNameAsync(model.Username);
                HttpContext.Session.Remove("username");
                if (user != null)
                {
                    var result = await _userManager.RemovePasswordAsync(user);
                    if (result.Succeeded)
                    {
                        result = await _userManager.AddPasswordAsync(user, model.NewPassword);
                        return RedirectToAction("Login", "Account");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                        return View(model);
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Email is not valid!");
                    return View(model);
                }
            }
        //    else
        //    {
        //        ModelState.AddModelError("", "Something is wrong!");
        //        return View(model);
        //    }

        //}
       

    }
}
