using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineLearning.Models;
using OnlineLearningApp.Respositories;

namespace OnlineLearning.Controllers
{
    public class CategoryController : Controller
    {
        private readonly DataContext _dataContext;
        public CategoryController(DataContext context)
        {
            _dataContext = context;
        }
        public async Task<IActionResult> Index(string CategoryId = "")
        {
            CategoryModel category = _dataContext.Category.Where(c => c.CategoryID.Equals(CategoryId)).FirstOrDefault();
            if (category == null)
            {
                return RedirectToAction("Index");
            }
            var course = await _dataContext.Courses.Where(p => p.CategoryID == category.CategoryID).ToListAsync();
            return View(course);
        }
    }
}
