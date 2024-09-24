using Microsoft.AspNetCore.Mvc;

namespace OnlineLearning.Controllers
{
    public class CourseController : Controller
    {
        public IActionResult CourseList()
        {
            return View();
        }

        public IActionResult CourseDeatail()
        {
            return View();
        }
    }
}
