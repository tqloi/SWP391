using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineLearning.Models.ViewModel;
using OnlineLearning.Models;
using OnlineLearningApp.Respositories;

namespace OnlineLearning.Areas.Instructor.Controllers
{
    [Area("Instructor")]
    [Authorize(Roles = "Instructor")]
    [Route("Instructor/[controller]/[action]")]
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
        //[HttpGet]
        //public async Task<IActionResult> TransferStudent()
        //{

        //    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //    var user = await _userManager.FindByIdAsync(userId);
        //    var request = new RequestTransferViewModel
        //    {
        //        UserID = user.Id,

        //    };
        //    return View(request);
        //}
        //[HttpPost]
        //public async Task<IActionResult> TransferStudent(RequestTransferViewModel model)
        //{

        //    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //    var user = await _userManager.FindByIdAsync(userId);
        //    var request = new RequestTranferModel();

        //    request.UserID = userId;
        //    request.FullName = model.FullName;
        //    request.BankName = model.BankName;

        //    request.AccountNumber = model.AccountNumber;
        //    request.Status = "In processing";

        //    if (model.MoneyNumber > user.WalletUser)
        //    {
        //        TempData["error"] = "Your wallet don't have enough money";
        //        return View(model);
        //    }
        //    request.MoneyNumber = model.MoneyNumber;
        //    await _dataContext.RequestTransfers.AddAsync(request);
        //    await _dataContext.SaveChangesAsync();
        //    TempData["success"] = "Request successfully!";
        //    return RedirectToAction("Index", "Home", new { Areas = "" });
        //}
    }
}
