using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineLearningApp.Respositories;

namespace OnlineLearning.Resporitories.Components
{
    public class CategoryViewComponent:ViewComponent
    {
        private readonly DataContext _dataContext;
        public CategoryViewComponent(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        public async Task<IViewComponentResult> InvokeAsync() => View(await _dataContext.Category.ToListAsync()); 
    }
}
