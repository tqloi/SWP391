using System.Security.Claims;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
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
        private RoleManager<IdentityRole> _roleManager;
        private IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AccountController(SignInManager<AppUserModel> signInManager, UserManager<AppUserModel> userManager, RoleManager<IdentityRole> roleManager, IWebHostEnvironment webHostEnvironment, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _webHostEnvironment = webHostEnvironment;
            _configuration = configuration;

        }
        [HttpGet]
        [Route("Account/Login")]
        public IActionResult Login()
        {
            return View();
        }
        //this method will be called first, redirect user to the google login page 
        public async Task LoginGoogle()
        {
            await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme, new AuthenticationProperties
            {
                RedirectUri = Url.Action("GoogleResponse")
            });

        }
        //selection and proper authentication from google and token exchange is return
        //all claim value and print on the page
        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (result.Principal != null)
            {
                var claims = result.Principal.Identities.FirstOrDefault()?.Claims;
                if (claims != null)
                {
                    var claimValues = claims.Select(claim => new
                    {
                        claim.Issuer,
                        claim.OriginalIssuer,
                        claim.Type,
                        claim.Value
                    });
                    //return Json(claimValues);
                    return RedirectToAction("Index", "Home", new { area = "" });
                }
            }
            // Handle the case where Principal or Claims are null
            return RedirectToAction("Index", "Home", new { area = "" });
        }


    
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            TempData["success"] = "Logout Success!";
            return RedirectToAction("Login", "Account");
        }
        public IActionResult VerifyEmail()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginVM)
        {
            if (!ModelState.IsValid)
            {
                TempData["error"] = "Login failed!";
                return View(loginVM);
            }
            var user = await _userManager.FindByNameAsync(loginVM.Username);
            if (user == null)
            {
                TempData["error"] = "Login failed!";
                return View(loginVM);
            }
            var checkVerify = await _userManager.IsEmailConfirmedAsync(user);
            if (!checkVerify)
            {
                TempData["error"] = "Your need verify your email before login!!";
                ModelState.AddModelError("", "Your need verify your email before login!");
                return RedirectToAction("EnterOTP", "Account");
            }
            var result = await _signInManager.PasswordSignInAsync(loginVM.Username, loginVM.Password, loginVM.RememberMe, false);
            if (!result.Succeeded)
            {
                TempData["error"] = "Username or Password is wrong!";
                ModelState.AddModelError("", "Username or Password are incorrect!");
                return View(loginVM);

            }
            HttpContext.Session.Remove("Otp");
            HttpContext.Session.Remove("Username");
            var roles = await _userManager.GetRolesAsync(user);

            // Redirect based on role
            if (roles.Contains("Admin"))
            {
                return RedirectToAction("Index", "Admin", new { area = "Admin" });
            }
            TempData["success"] = "Login successful!";
            return RedirectToAction("Index", "Home");
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
                if (emailexist != null)
                {
                    TempData["error"] = "Emaiil existed!";
                    return View(model);
                }
                var user = new AppUserModel

                 {
                        UserName = model.Username,
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Email = model.Email,
                        PhoneNumber = "123456789",
                        ProfileImagePath = "default.jpg",
                        Address = "",
                        Dob = DateOnly.FromDateTime(DateTime.Now),
                        Gender = true
                    };


                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    //set role
                    var roleResult = await _userManager.AddToRoleAsync(user, "Student");
                    if (!roleResult.Succeeded)
                    {
                        foreach (var error in roleResult.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                        return View(model); // Trả về form với các lỗi
                    }

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
            TempData["error"] = "Failed!";
            return View(model);
        }
        [HttpGet]
        public IActionResult OTP_ChangeEmail()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> OTP_ChangeEmail(OTPViewModel model)
        {
            var storedOtp = HttpContext.Session.GetInt32("otp");
            var changeemail = HttpContext.Session.GetString("changeemail");
             if (model.Otp == storedOtp && changeemail != null)
                {
                    HttpContext.Session.SetString("message", "emailsuccess");
                    return RedirectToAction("AdminEdit", "Admin", new { Areas = "Admin" });
                }
                
            ModelState.AddModelError("", "Invalid OTP.");
            return View("OTP_ChangeEmail");

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
            var changeemail = HttpContext.Session.GetString("changeemail");
            if (storedOtp != null)
            {
                if (usernamess != null && model.Otp == storedOtp)
                {
                    return RedirectToAction("ChangePassword", new { username = usernamess, otp = model.Otp });
                }
                else if (model.Otp == storedOtp && changeemail != null)
                {
                    HttpContext.Session.SetString("message", "emailsuccess");
                    return RedirectToAction("AdminEdit", "Admin", new { Areas = "Admin" });
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
                    await emailSender.SendEmailAsync(user.Email, "Your OTP Code", $"Your OTP code to verify your email  is {otp}.");

                    HttpContext.Session.SetInt32("otp", otp);
                    HttpContext.Session.SetString("username", user.UserName);

                    return RedirectToAction("EnterOtp");
                    // return RedirectToAction("ChangePassword", new { username = user.UserName });
                }

            }

            return View(model);
        }
        public IActionResult ChangePassword(string username, int otp)
        {
            HttpContext.Session.Remove("otp");
            if (string.IsNullOrEmpty(username) )
            {
                return RedirectToAction("VerifyEmail", "Account");
            }

            return View(new ResetPasswordViewModel { Username = username });
        }
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(model.Username);
                HttpContext.Session.Remove("username");
                HttpContext.Session.Remove("otp");
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
            else
            {
                ModelState.AddModelError("", "Something is wrong!");
                return View(model);
            }

        }


    }
}
