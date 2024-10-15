
﻿using System.Diagnostics;
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
        public AdminController(DataContext dataContext, UserManager<AppUserModel> userManager, SignInManager<AppUserModel> signInManager, IWebHostEnvironment webHostEnvironment, IConfiguration configuration)
        {
            _dataContext = dataContext;
            _userManager = userManager;
            _signInManager = signInManager;
            _webHostEnvironment = webHostEnvironment;
            _configuration = configuration;
        }
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var users = await _dataContext.Users.ToListAsync();

            return View(users);
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
            return View(new ChangePasswordViewModel { Username = user.UserName});

        }
        [HttpPost]
        public async Task<IActionResult> Changepass(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(model.Username);
                
                if(!await _userManager.CheckPasswordAsync(user, model.OldPassword))
                {
                    TempData["error"] = "Invalid Old Password!";
                    return View(model);
                }
                if(await _userManager.CheckPasswordAsync(user, model.NewPassword))
                {
                    var result = await _userManager.RemovePasswordAsync(user);
                    if (result.Succeeded)
                    {
                        await _userManager.AddPasswordAsync(user, model.NewPassword);
                        TempData["success"] = "Change Password Successful!";
                        return RedirectToAction("AdminProfile");
                    }
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

                    string uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "Images");
                    if (!string.IsNullOrEmpty(user.ProfileImagePath) && !user.ProfileImagePath.Equals("default.jpg"))
                    {
                        string oldImagePath = Path.Combine(uploadPath, user.ProfileImagePath);
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    string imageName = Guid.NewGuid() + "_" + model.ProfileImage.FileName;
                    string filePath = Path.Combine(uploadPath, imageName);

                    using (var fs = new FileStream(filePath, FileMode.Create))
                    {
                        await model.ProfileImage.CopyToAsync(fs);
                    }


                    user.ProfileImagePath = imageName;
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


        public async Task<IActionResult> ViewNotification() 
        {
            var notifications = await _dataContext.Notification.Include(n => n.User).ToListAsync();
            return View(notifications);
        }



    }
}


