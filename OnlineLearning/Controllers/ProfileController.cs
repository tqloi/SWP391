using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineLearning.Models.ViewModel;
using OnlineLearning.Models;
using Microsoft.AspNetCore.Identity;
using OnlineLearningApp.Respositories;
using System.Security.Claims;

namespace OnlineLearning.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DataContext datacontext;
        private UserManager<AppUserModel> _userManager;
        private SignInManager<AppUserModel> _signInManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProfileController(ILogger<HomeController> logger, DataContext context, SignInManager<AppUserModel> signInManager, UserManager<AppUserModel> userManager, IWebHostEnvironment webHostEnvironment)
        {
            datacontext = context;
            _logger = logger;
            _signInManager = signInManager;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
        }
        public IActionResult InstructorRegistration()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> InstructorRegistration(UploadFileViewModel model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (model.Filepdf != null)
            {
                string uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "ConfirmInstructor");
                string fileName = Guid.NewGuid() + "_" + model.Filepdf.FileName;
                string filePath = Path.Combine(uploadPath, fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.Filepdf.CopyToAsync(fileStream);
                }

                var instructorConfirmation = new InstructorConfirmationModel
                {
                    UserID = userId,
                    Certificatelink = fileName,
                    SendDate = DateTime.Now,
                    Description = model.Description
                };
                datacontext.InstructorConfirmation.Add(instructorConfirmation);
                await datacontext.SaveChangesAsync();

                TempData["success"] = "Request sended successfully!";
                return View();
            }
            TempData["error"] = "An error occurred while processing the payment. Please try again.";
            return View(model);
        }
    }
}
