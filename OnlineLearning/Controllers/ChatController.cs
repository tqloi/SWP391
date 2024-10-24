using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using OnlineLearning.Models;
using OnlineLearning.Models.ViewModel;
using OnlineLearning.Services;
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
        private readonly StringeeService _stringeeService;
        public ChatController(
            DataContext context,
            UserManager<AppUserModel> userManager,
            IHubContext<ChatHub> basicChatHub, StringeeService stringeeService)
        {
            _db = context;
            _userManager = userManager;
            _basicChatHub = basicChatHub;
            _stringeeService = stringeeService;
        }
        
        [Authorize]
        public async Task<IActionResult> Index(string id)
        {
           
            var model = new RoleViewModel();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            var list = await _db.Users.Where(u => !u.Id.Equals(user.Id)).ToListAsync();
            var receiver = await _userManager.FindByIdAsync(id);
            var messages = await _db.Message
        .Where(m => (m.SenderId == user.Id && m.ReceiverId == id) ||
                     (m.SenderId == id && m.ReceiverId == user.Id))
        .OrderBy(m => m.Timestamp)
        .ToListAsync();
            var isreadmess = await _db.Message.Where(m => (m.ReceiverId.Equals(userId) && m.SenderId.Equals(id)) || (m.SenderId.Equals(userId) && m.ReceiverId.Equals(id))).OrderByDescending(t => t.Timestamp).Select(m => new {m.Content, m.SenderId, m.ReceiverId}).FirstOrDefaultAsync();
            if(isreadmess == null)
            {
                model.IsReadmess = null;
            }
            else
            {
                model.receimess = $"{isreadmess.ReceiverId}";
                model.IsReadmess = $"{isreadmess.SenderId}: {isreadmess.Content}";
            }

            var token = _stringeeService.GenerateAccessToken(userId);
            if (user != null)
            {
                var roles = await _userManager.GetRolesAsync(user);
                model.UserRoles = roles;
                model.ListUser = list;
                model.ReceiverId = id;
                model.SendName = user.FirstName + " " + user.LastName;
                model.Messages = messages;
                model.SendId = user.Id;
                model.sendimg = user.ProfileImagePath;  
                model.Token = token;
               if (receiver != null)
                {
                    model.receiveimg = receiver.ProfileImagePath;
                    model.ReceiveName = receiver.FirstName + " " + receiver.LastName;
                }
                else
                {
                    model.ReceiveName = "Please choose someone to get message";
                }
            }

            return View(model);

        }
        
        [HttpGet("SendMessageToReceiver")]
        [Authorize]
        public async Task<IActionResult> SendMessageToReceiver(string sender, string receiver, string message)
        {
            var userId = _db.Users.FirstOrDefault(u => u.Id.Equals(receiver))?.Id;
             
            if (!string.IsNullOrEmpty(userId))
            {
                var messageModel = new MessageModel
                {
                    SenderId = sender,
                    ReceiverId = receiver,
                    Content = message,
                    Timestamp = DateTime.Now,
                };

                _db.Message.Add(messageModel);
                await _db.SaveChangesAsync();
                await _basicChatHub.Clients.User(userId).SendAsync("MessageReceived", sender, message);
            }
            return Ok();
        }
        //[HttpGet("SendMessageToGroup")]
        //[Authorize]
        //public async Task SendMessageToGroup(string message)
        //{
        //    var user = GetUserId();
        //    var role = (await GetUserRoles(user)).FirstOrDefault();
        //    var username = _db.Users.FirstOrDefault(u => u.Id == user)?.Email ?? "";
        //    if (!string.IsNullOrEmpty(role))
        //    {
        //        await _basicChatHub.Clients.Group(role).SendAsync("MessageReceived", username, message);
        //    }
        //}
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
