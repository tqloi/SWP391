using Microsoft.AspNetCore.Mvc;
using OnlineLearning.Models.ViewModel;
using OnlineLearning.Services;

namespace OnlineLearning.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VideoCallController : Controller
    {
        private readonly StringeeService _stringeeService;

        public VideoCallController(StringeeService stringeeService)
        {
            _stringeeService = stringeeService;
        }

        [HttpGet("{callerId}/{receiverId}")]
        public IActionResult StartCall(string callerId, string receiverId)
        {
            if (string.IsNullOrEmpty(callerId) || string.IsNullOrEmpty(receiverId))
            {
                return BadRequest("Caller ID and Receiver ID are required.");
            }
            
            var token = _stringeeService.GenerateAccessToken(callerId);
            return RedirectToAction("VideoCallView", new { token, callerId, receiverId });
        }

        public IActionResult VideoCallView(string token, string callerId, string receiverId)
        {
            // Kiểm tra token và ID
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(callerId) || string.IsNullOrEmpty(receiverId))
            {
                return BadRequest("Token, Caller ID, and Receiver ID are required.");
            }
            HttpContext.Session.SetString("idsend", callerId);
            var model = new VideoCallViewModel
            {
                Token = token,
                SendId = callerId,
                ReceiveId = receiverId
            };

            return View(model);
        }
    }
}