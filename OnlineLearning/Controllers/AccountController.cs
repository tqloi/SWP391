using System.Security.Claims;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineLearning.Models;
using OnlineLearning.Models.ViewModel;

namespace OnlineLearning.Controllers
{
    public class AccountController : Controller
    {
        private UserManager<AppUserModel> _userManager;
        private SignInManager<AppUserModel> _signInManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public AccountController(SignInManager<AppUserModel> signInManager, UserManager<AppUserModel> userManager, IWebHostEnvironment webHostEnvironment)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _webHostEnvironment = webHostEnvironment;
        }
        [HttpGet]
		public IActionResult Login(string returnurl)
		{
			return View(new LoginViewModel { ReturnUrl = returnurl});
		}
		[HttpPost]
		public async Task<IActionResult> Login(LoginViewModel loginVM)
		{
			Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(loginVM.Username, loginVM.Password, false, false);
				if (result.Succeeded)
				{
					return Redirect(loginVM.ReturnUrl ?? "/");
				}
				ModelState.AddModelError("", "Invalid username or password");
			
            TempData["erorr"] = "Failed!";
			return View(loginVM);
		}
		[HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(UserModel model)
        {
            if (ModelState.IsValid)
            {
                // Copy data from RegisterViewModel to IdentityUser
                var user = new AppUserModel
                {
                    UserName = model.Username,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber
                };
                if (model.ProfileImage != null)
                {
                    string uploadimg = Path.Combine(_webHostEnvironment.WebRootPath, "Images");
                    string imageName = Guid.NewGuid().ToString() + "_" + model.ProfileImage.FileName;
                    string filePath = Path.Combine(uploadimg, imageName);

                    using (var fs = new FileStream(filePath, FileMode.Create))
                    {
                        await model.ProfileImage.CopyToAsync(fs);
                    }

                    user.ProfileImagePath = imageName;
                }

                // Store user data in AspNetUsers database table
                var result = await _userManager.CreateAsync(user, model.Password);

                // If user is successfully created, sign-in the user using
                // SignInManager and redirect to index action of HomeController
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    TempData["success"] = "Create success";
                    return RedirectToAction("Login", "Account");
                }

                // If there are any errors, add them to the ModelState object
                // which will be displayed by the validation summary tag helper
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }

    }
}
        
 