using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Threading.Tasks;
using OnlineLearning.Models;
using Microsoft.AspNetCore.Identity;
using OnlineLearningApp.Respositories;
using Microsoft.EntityFrameworkCore;

public class CourseAccessFilter : IAsyncActionFilter
{
    private readonly DataContext _context;
    private readonly UserManager<AppUserModel> _userManager;

    public CourseAccessFilter(DataContext context, UserManager<AppUserModel> userManager)
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

        // new action la lectureDdetail
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
        }
        else //con lai..
        {
            var courseId = (int)context.ActionArguments["CourseID"];
            course = await _context.Courses.FindAsync(courseId);
        }

        if (course == null)
        {
            context.Result = new NotFoundResult();
            return;
        }

        if (course.InstructorID == user.Id)
        {
            await next();
            return;
        }

        var studentCourse = await _context.StudentCourses.FirstOrDefaultAsync(sc => sc.StudentID == user.Id && sc.CourseID == course.CourseID);

        if (studentCourse != null)
        {
            await next();
            return;
        }

        context.Result = new RedirectToActionResult("CourseDetail", "Course", new { CourseID = course.CourseID });
    }

}
