using System.Diagnostics;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using OnlineLearning.Models;
using OnlineLearning.Models.ViewModel;
using OnlineLearningApp.Respositories;

namespace OnlineLearning.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
        private readonly DataContext datacontext;
        private UserManager<AppUserModel> _userManager;
        private SignInManager<AppUserModel> _signInManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        
        public HomeController(ILogger<HomeController> logger, DataContext context, SignInManager<AppUserModel> signInManager, UserManager<AppUserModel> userManager, IWebHostEnvironment webHostEnvironment)
        {
            datacontext = context;
            _logger = logger;
            _signInManager = signInManager;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
		{
            var model = new ListViewModel();
            model.Courses = await datacontext.Courses.Include(c => c.Category)
                .OrderByDescending(sc => sc.CourseID).ToListAsync();
            model.Categories = await datacontext.Category.ToListAsync();
           
			return View(model);
		}

        public IActionResult Contact()
        {
            if (User.IsInRole("Admin"))
            {
                return Forbid();
            }
            return View();
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
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.UserName = model.UserName;
                user.Email = model.Email;
                user.PhoneNumber = model.PhoneNumber;
                user.Address    = model.Address;
                user.Dob = model.Dob;
               user.Gender = model.gender;
                
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

                    return RedirectToAction("UserProfile");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            TempData["erorr"] = "Edit failed!";
            return View(model);
        }
        


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
