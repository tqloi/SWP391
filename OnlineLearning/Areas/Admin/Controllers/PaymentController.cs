using System.Security.Policy;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineLearning.Models;
using OnlineLearningApp.Respositories;

namespace OnlineLearning.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]

    [Route("Admin/[controller]/[action]")]
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
        
        public async Task<IActionResult> PaymentRequestList()
        {
            var listrequest = await _dataContext.RequestTranfer.Include(i => i.User).OrderByDescending(r => r.CreateAt).ToListAsync();
            return View(listrequest);
        }
        public async Task<IActionResult> PaymentConfirm(int id)
        {
            var paymentUser = await _dataContext.RequestTranfer.FirstOrDefaultAsync(p => p.TranferID == id);
            var UserRequest = await _userManager.FindByIdAsync(paymentUser.UserID);
            if (UserRequest == null)
            {
                TempData["error"] = "User not exist!";
                return RedirectToAction("PaymentRequestList");
            }
            UserRequest.WalletUser -= paymentUser.MoneyNumber;
             var result = await _userManager.UpdateAsync(UserRequest);
            if (result.Succeeded)
            {
                paymentUser.Status = "Success";
                _dataContext.RequestTranfer.Update(paymentUser);
                await _dataContext.SaveChangesAsync();
                TempData["success"] = "Successful!";
                return RedirectToAction("PaymentRequestList");
            }
            TempData["error"] = "Failed!";
            return RedirectToAction("PaymentRequestList");
        }


    }
}
