using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using OnlineLearning.Models;
using OnlineLearningApp.Respositories;

namespace OnlineLearning.Filter
{
    public class LectureAccessFilter : IAsyncActionFilter
    {
        private readonly DataContext _context;
        private readonly UserManager<AppUserModel> _userManager;

        public LectureAccessFilter(DataContext context, UserManager<AppUserModel> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {

            var user = await _userManager.GetUserAsync(context.HttpContext.User);
            if (user == null)
            {
                context.Result = new RedirectToActionResult("Login", "Account", null);
                return;
            }

            var course = new CourseModel();

            if (context.ActionDescriptor.RouteValues["action"] == "LectureDetail")
            {
                var lectureId = (int)context.ActionArguments["lectureId"];
                var lecture = await _context.Lecture.FindAsync(lectureId);

                if (lecture == null)
                {
                    context.Result = new NotFoundResult();
                    return;
                }

                course = await _context.Courses.FindAsync(lecture.CourseID);

                if (course.InstructorID == user.Id)
                {
                    context.Result = new RedirectToActionResult("LectureDetail", "Lecture", new { area = "Instructor", LectureID = lecture.LectureID });
                    return;
                }
                else 
                {
                    await next();
                    return;
                }
            }
        }
    }
}
