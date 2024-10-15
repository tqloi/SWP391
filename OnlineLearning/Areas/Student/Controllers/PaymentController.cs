using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineLearning.Models;
using OnlineLearning.Models.ViewModel;
using OnlineLearningApp.Respositories;

namespace OnlineLearning.Areas.Student.Controllers
{
    [Area("Student")]
    [Authorize(Roles = "Student")]
    [Route("Student/[controller]/[action]")]
    public class PaymentController : Controller
    {
        private UserManager<AppUserModel> _userManager;
        private SignInManager<AppUserModel> _signInManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly DataContext _dataContext;
        private IConfiguration _configuration;
        public PaymentController(DataContext dataContext, UserManager<AppUserModel> userManager, SignInManager<AppUserModel> signInManager, IWebHostEnvironment webHostEnvironment, IConfiguration configuration)
        {
            _dataContext = dataContext;
            _userManager = userManager;
            _signInManager = signInManager;
            _webHostEnvironment = webHostEnvironment;
            _configuration = configuration;
        }
        [HttpGet]
        public async Task<IActionResult> TransferStudent()
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
        public async Task<IActionResult> TransferStudent(RequestTransferViewModel model)
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
                _dataContext.RequestTranfer.Add(request);
                await _dataContext.SaveChangesAsync();
                TempData["success"] = "Request successfully!";
                return RedirectToAction("Index", "Home", new { Areas = "" });
            }
          


        }
    }
}
