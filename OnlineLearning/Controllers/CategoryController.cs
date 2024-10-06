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
        public async Task<IActionResult> Index(int CategoryId)
        {
            CategoryModel category = await _dataContext.Category.FirstOrDefaultAsync(c => c.CategoryID == CategoryId);

            if (category == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var courses = await _dataContext.Courses
                .Where(p => p.CategoryID == category.CategoryID)
                .ToListAsync();

            return View(courses);
        } 
    }
}
