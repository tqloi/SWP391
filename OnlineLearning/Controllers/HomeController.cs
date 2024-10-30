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
using YourNamespace.Models;

namespace OnlineLearning.Controllers
{
    [ServiceFilter(typeof(AdminRedirectFilter))]
    public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
        private readonly DataContext _dataContext;
        private UserManager<AppUserModel> _userManager;
        private SignInManager<AppUserModel> _signInManager;
        
        public HomeController(ILogger<HomeController> logger, DataContext context, SignInManager<AppUserModel> signInManager, UserManager<AppUserModel> userManager)
        {
            _dataContext = context;
            _logger = logger;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var model = new ListViewModel();
            model.Courses = await _dataContext.Courses.Include(c => c.Category)
                .OrderByDescending(sc => sc.CourseID).ToListAsync();
            model.Categories = await _dataContext.Category.ToListAsync();
           
			return View(model);
		}

        [HttpGet]
        [Authorize]
        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult Error404()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Contact(ReportModel model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            try
            {
                var feedback = new ReportModel
                {
                    UserID = userId,
                    Subject = model.Subject,
                    Comment = model.Comment,
                    FeedbackDate = DateTime.Now,
                };
                _dataContext.Report.Add(feedback);
                await _dataContext.SaveChangesAsync();

                TempData["success"] = "Feedback has been submitted successfully!";
            }
            catch (Exception ex)
            {
                TempData["error"] = "Failed to submit feedback. Please try again later.";
                return View(model);
            }

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
