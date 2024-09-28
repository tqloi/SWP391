using Microsoft.AspNetCore.Mvc;
using OnlineLearningApp.Respositories;

namespace OnlineLearning.Controllers
{
    public class CourseController : Controller
    {
        private readonly DataContext _datacontext;
        public CourseController(DataContext datacontext)
        {
            _datacontext = datacontext;
        }

        public IActionResult Index()
        {
            var course = _datacontext.Courses.ToList();
            return View(course);
        }

        public IActionResult CourseDeatail()
        {
            return View();
        }
    }
}
