using Microsoft.AspNetCore.Mvc;

namespace OnlineLearning.Areas.Student.Controllers
{
    public class StudentController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
