using System.Security.Claims;
using System.Web.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineLearning.Areas.Admin.Models.ViewModel;
using OnlineLearning.Models;
using OnlineLearningApp.Respositories;
using AuthorizeAttribute = Microsoft.AspNetCore.Authorization.AuthorizeAttribute;
using Controller = Microsoft.AspNetCore.Mvc.Controller;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace OnlineLearning.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles ="Admin")]
    
    [Route("/[controller]/[action]")]
    public class AdminController : Controller
    {
        private UserManager<AppUserModel> _userManager;
        private SignInManager<AppUserModel> _signInManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly DataContext _dataContext;
        public AdminController(DataContext dataContext, UserManager<AppUserModel> userManager, SignInManager<AppUserModel> signInManager) {
             _dataContext = dataContext;
            _userManager = userManager;
            _signInManager = signInManager;
        }
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var users = await _dataContext.Users.ToListAsync();
           
            return View(users);
        }
        public  async Task<IActionResult> UserList()
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
        public async Task<IActionResult> UserProfile(string Id)
        {
           
            var user = await _userManager.FindByIdAsync(Id);
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
                Gender = user.Gender,
                ExistingProfileImagePath= user.ProfileImagePath
            };
            return View(model);
        }
        //public async Task<IActionResult> RemoveUser(string Id)
        //{

        //    var user = await _userManager.FindByIdAsync(Id);
        //    if (!string.Equals(user.ProfileImagePath, "default.jpg"))
        //    {
        //        string uploadimg = Path.Combine(_webHostEnvironment.WebRootPath, "Images");
        //        string oldpathImages = Path.Combine(uploadimg, user.ProfileImagePath);
        //        if (System.IO.File.Exists(oldpathImages))
        //        {
        //            System.IO.File.Delete(oldpathImages);
        //        }
        //    }
        //    await _userManager.DeleteAsync(user);
           
        //    TempData["success"] = "User deleted!";
        //    return RedirectToAction("UserList");
        //}
    }
}
