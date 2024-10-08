using Microsoft.AspNetCore.Mvc;

namespace OnlineLearning.Areas.Instructor.Controllers
{
    [Area("Instructor")]
    [Route("[controller]/[action]")]
    public class InstructorController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
