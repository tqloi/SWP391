using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

public class AdminRedirectFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {

        var user = context.HttpContext.User;
        if (user.Identity.IsAuthenticated && user.IsInRole("Admin"))
        {
            // Chuyển hướng đến trang Admin
            context.Result = new RedirectToActionResult("Index", "Admin", new { area = "Admin" });
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        // Không cần xử lý gì sau khi hành động được thực thi
    }
}
