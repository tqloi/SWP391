using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using OnlineLearning.Models;
using OnlineLearningApp.Respositories;

namespace OnlineLearning.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
        private readonly DataContext datacontext;
        public HomeController(ILogger<HomeController> logger, DataContext context)
        {
            datacontext = context;
            _logger = logger;
        }

        public IActionResult Index()
		{
			return View();
		}

		public IActionResult UserProfile()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
