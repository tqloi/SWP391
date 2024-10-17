using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using OnlineLearning.Models;
using OnlineLearning.Models.ViewModel;
using OnlineLearningApp.Respositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OnlineLearning.Controllers
{
    public class ChatController : Controller
    {
        private readonly DataContext _db;
        private readonly IHubContext<ChatHub> _basicChatHub;
        private readonly UserManager<AppUserModel> _userManager;
        public ChatController(
            DataContext context,
            UserManager<AppUserModel> userManager,
            IHubContext<ChatHub> basicChatHub)
        {
            _db = context;
            _userManager = userManager;
            _basicChatHub = basicChatHub;
        }
        [Authorize]
        public async Task<IActionResult> Index(string id)
        {
           
            var model = new RoleViewModel();
            var user = await _userManager.GetUserAsync(User);
            var list = await _db.Users.Where(u => !u.Id.Equals(user.Id)).ToListAsync();
            var receiver = await _db.Users.FirstOrDefaultAsync(i => i.Id.Equals(id));
            if (user != null)
            {
                var roles = await _userManager.GetRolesAsync(user);
                model.UserRoles = roles;
                model.ListUser = list;
                model.ReceiverId = id;
                model.SendName = user.FirstName + " " + user.LastName;
                if(receiver != null)
                {
                    model.ReceiveName = receiver.FirstName + " " + receiver.LastName;
                }
                else
                {
                    model.ReceiveName = "Please choose someone to get message";
                }
            }

            return View(model);

        }
        [HttpGet("SendMessageToAll")]
        [Authorize]
        public async Task<IActionResult> SendMessageToAll(string user, string message)
        {
            await _basicChatHub.Clients.All.SendAsync("MessageReceived", user, message);
            return Ok();
        }
        [HttpGet("SendMessageToReceiver")]
        [Authorize]
        public async Task<IActionResult> SendMessageToReceiver(string sender, string receiver, string message)
        {
            var userId = _db.Users.FirstOrDefault(u => u.Id.Equals(receiver))?.Id;

            if (!string.IsNullOrEmpty(userId))
            {
                await _basicChatHub.Clients.User(userId).SendAsync("MessageReceived", sender, message);
            }
            return Ok();
        }
        [HttpGet("SendMessageToGroup")]
        [Authorize]
        public async Task SendMessageToGroup(string message)
        {
            var user = GetUserId();
            var role = (await GetUserRoles(user)).FirstOrDefault();
            var username = _db.Users.FirstOrDefault(u => u.Id == user)?.Email ?? "";
            if (!string.IsNullOrEmpty(role))
            {
                await _basicChatHub.Clients.Group(role).SendAsync("MessageReceived", username, message);
            }
        }
        private string GetUserId()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return userId;
        }
        private async Task<IList<string>> GetUserRoles(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var roles = await _userManager.GetRolesAsync(user);
            return roles;
        }


    }
}
