using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;
using OnlineLearning.Models;

public class ChatHub : Hub
{
    private readonly UserManager<AppUserModel> _userManager;
    public ChatHub(UserManager<AppUserModel> userManager)
    {
        _userManager = userManager;
    }
    public override async Task OnConnectedAsync()
    {
        var httpContext = Context.GetHttpContext();
        var userId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var roles = await GetUserRoles(userId);
        // Now you can use the userId as needed
        await base.OnConnectedAsync();
    }

    public string GetUserId()
    {
        var httpContext = Context.GetHttpContext();
        var userId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return userId;
    }

    public async Task<IList<string>> GetUserRoles(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        var roles = await _userManager.GetRolesAsync(user);
        return roles;
    }
    public async Task SendMessage(string user, string message)
    {
        await Clients.All.SendAsync("ReceiveMessage", user, message);
    }
}
