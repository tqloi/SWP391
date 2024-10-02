using Microsoft.AspNetCore.Mvc;

namespace OnlineLearning.Areas.Instructor.Controllers
{
    [Area("Instructor")]
    public class InstructorController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
