using System.Web.Mvc;
using Microsoft.AspNetCore.Mvc;
using Controller = Microsoft.AspNetCore.Mvc.Controller;

namespace OnlineLearning.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles ="Admin")]
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
