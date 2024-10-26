using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using OnlineLearning.Models;
using OnlineLearningApp.Respositories;
using System.Threading.Tasks;

namespace OnlineLearning.Filter
{
    public class AssignmentAccessFilter : IAsyncActionFilter
    {
        private readonly DataContext _context;
        private readonly UserManager<AppUserModel> _userManager;

        public AssignmentAccessFilter(DataContext context, UserManager<AppUserModel> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // Lấy thông tin người dùng hiện tại
            var user = await _userManager.GetUserAsync(context.HttpContext.User);
            if (user == null)
            {
                context.Result = new RedirectToActionResult("Login", "Account", null);
                return;
            }

            if (context.ActionDescriptor.RouteValues["action"] == "SubmitAssignment")
            {
                var assignmentId = (int)context.ActionArguments["id"];
                var assignment = await _context.Assignment.FindAsync(assignmentId);
                var course = await _context.Courses.FindAsync(assignment.CourseID);

                if (course.InstructorID == user.Id)
                {
                    context.Result = new RedirectToActionResult("AssignmentList", "Participation", new { CourseID = course.CourseID });
                    return;
                }
                if (await _context.StudentCourses.AnyAsync(cs => cs.CourseID == course.CourseID && cs.StudentID == user.Id))
                {
                    await next();
                    return;
                }
                else
                {
                    context.Result = new ForbidResult();
                    return;
                }
            }
            else
            {
                await next();
            }
        }
    }
}
