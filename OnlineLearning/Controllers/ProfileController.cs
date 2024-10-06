using Microsoft.AspNetCore.Mvc;

namespace OnlineLearning.Controllers
{
    public class ProfileController : Controller
    {
        public IActionResult InstructorRegistration()
        {
            return View();
        }
    }
}
