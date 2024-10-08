using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineLearningApp.Respositories;

namespace OnlineLearning.Resporitories.Components
{
    public class LectureViewComponent : ViewComponent
    {
        private readonly DataContext _dataContext;
        public LectureViewComponent(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        public async Task<IViewComponentResult> InvokeAsync(int courseId)
        {
            var lectures = await _dataContext.Lecture
                .Where(l => l.CourseID == courseId) 
                .ToListAsync();

            return View(lectures);
        }
    }
}
