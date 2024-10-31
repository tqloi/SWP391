using System.Diagnostics.Contracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow.CopyAnalysis;
using Microsoft.EntityFrameworkCore;
using OnlineLearning.Models;
using OnlineLearning.Models.ViewModel;
using OnlineLearning.Services;
using OnlineLearningApp.Respositories;

namespace OnlineLearning.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VideoCallController : Controller
    {
        private readonly StringeeService _stringeeService;
        private readonly DataContext _datacontext;
        private readonly UserManager<AppUserModel> _userManager;
        public VideoCallController(StringeeService stringeeService, DataContext dataContext, UserManager<AppUserModel> userManager)
        {
            _stringeeService = stringeeService;
            _datacontext = dataContext;
            _userManager = userManager;
        }

        [HttpGet("{callerId}/{receiverId}")]
        public  IActionResult StartCall(string callerId, string receiverId)
        {
            if (string.IsNullOrEmpty(callerId) || string.IsNullOrEmpty(receiverId))
            {
                return BadRequest("Caller ID and Receiver ID are required.");
            }
            var videocallinfo = new VideoCallModel
            {
                SendID = callerId,
                ReceiveID = receiverId
            };
            
            _datacontext.VideoCallInfo.Add(videocallinfo);
             _datacontext.SaveChanges();
            var token = _stringeeService.GenerateAccessToken(callerId);
            return RedirectToAction("VideoCallView", new { token, callerId, receiverId });
        }

       
       
        public async Task<IActionResult> VideoCallView(string token, string callerId, string receiverId)
        {
            // Kiểm tra token và ID
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(callerId) || string.IsNullOrEmpty(receiverId))
            {
                return BadRequest("Token, Caller ID, and Receiver ID are required.");
            }
            var senduser = await _userManager.FindByIdAsync(callerId);
            var receiveuser = await _userManager.FindByIdAsync(receiverId);
            if (senduser == null || receiveuser == null)
            {
                return NotFound("Caller or receiver not found.");
            }

            var model = new VideoCallViewModel
                {
                    Token = token,
                    SendId = callerId,
                    ReceiveId = receiverId,
                    FullNameSend = senduser.FirstName + " " + senduser.LastName,
                    FullNameReceive = receiveuser.FirstName + receiveuser.LastName
                };
            
           

            return View(model);
        }
    }
}