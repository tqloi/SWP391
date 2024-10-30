using ApiVideo.Api;
using ApiVideo.Client;
using ApiVideo.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using MimeKit.Cryptography;
using OnlineLearning.Models;
using OnlineLearning.Services;
using OnlineLearningApp.Respositories;
using System.Collections.Generic;
using System.Security.Claims;

namespace OnlineLearning.Areas.Instructor.Controllers
{
    [ApiController]
    [Area("Instructor")]
    [Authorize(Roles = "Instructor")]
    [Route("Instructor/[controller]/[action]")]
    public class LiveStreamController : Controller
    {
        private readonly ApiVideoClient _client;
        private readonly string _streamKey;
        // private readonly string _playerId;
        private readonly string _rtmpServer;

        private readonly DataContext _datacontext;
        private UserManager<AppUserModel> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly FileService _fileService;
        public LiveStreamController(DataContext datacontext, UserManager<AppUserModel> userManager, IWebHostEnvironment webHostEnvironment, FileService fileService)
        {
            _client = new ApiVideoClient("EugYjajLHbC5jEkLAng2tywbO1nP2vY3NmMX4nNtyyR");
            // if you rather like to use the sandbox environment:
            //_client = new ApiVideoClient("EugYjajLHbC5jEkLAng2tywbO1nP2vY3NmMX4nNtyyR", ApiVideoClient.Environment.SANDBOX);
            _streamKey = "a3b0b9ad-7ef8-44bc-94e0-9809182f73cc";
            //_playerId = "pt6xbd2agEoDgHIQhCNuNsFI";
            _rtmpServer = "rtmp://broadcast.api.video/s";
            _datacontext = datacontext;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
            _fileService = fileService;
        }

        [HttpGet]
        public IActionResult ListAllLiveStream(string liveStreamId)
        {
            try
            {
                // Retrieve the live stream with the specified ID
                var liveStream = _client.LiveStreams().get(liveStreamId);

                return Ok(liveStream); // Return live stream details as JSON
            }
            catch (ApiException e)
            {
                // Log the error details and return an error response
                Console.WriteLine("Exception when calling LiveStreamsApi.get");
                Console.WriteLine("Status code: " + e.ErrorCode);
                Console.WriteLine("Reason: " + e.Message);

                return BadRequest(new
                {
                    error = "Error retrieving live stream",
                    message = e.Message,
                    statusCode = e.ErrorCode
                });
            }
        }

        [HttpPost]
        [Authorize("instructor")]
        //field now fix temporary
        public IActionResult CreateLiveStream(string name, int CourseId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var course = _datacontext.Courses.FirstOrDefault(c => c.CourseID  == CourseId);
            if (course == null)
            {
                return NotFound();
            }
            
            var liveStreamCreationPayload = new LiveStreamCreationPayload
            {
                name = name,
                _public = true,
                //playerid = "_playerId"

                //currently no restream
            };

            try
            {
                var liveStream = _client.LiveStreams().create(liveStreamCreationPayload);
                var record = new LivestreamRecordModel
                {
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                    UserID = userId,
                    StreamKey = _streamKey,
                };
                _datacontext.LivestreamRecord.Add(record);
                _datacontext.SaveChanges();
                TempData["success"] = "Create Success";
                return Ok(liveStream);
            }
            catch (ApiException e)
            {
                // Handle the error (e.g., log it and return an error response)
                return BadRequest($"Error creating live stream: {e.Message}");
            }
        }
        [HttpGet("{liveStreamId}")]
        public IActionResult RetrieveLiveStream(string liveStreamId)
        {
            try
            {
                // Access the live streams API instance
                LiveStreamsApi apiInstance = _client.LiveStreams();

                // Retrieve the live stream with the specified ID
                var liveStream = apiInstance.get(liveStreamId);

                return Ok(liveStream); // Return live stream details as JSON
            }
            catch (ApiException e)
            {
                // Log the error details and return an error response
                Console.WriteLine("Exception when calling LiveStreamsApi#get");
                Console.WriteLine("Status code: " + e.ErrorCode);
                Console.WriteLine("Reason: " + e.Message);
                //Console.WriteLine("Response headers: " + e.getResponseHeaders());

                return BadRequest(new
                {
                    error = "Error retrieving live stream",
                    message = e.Message,
                    statusCode = e.ErrorCode
                });
            }
        }

        [HttpDelete("{liveStreamId}")]
        public IActionResult DeleteLiveStream(string liveStreamId)
        {
            try
            {
                // Attempt to delete the live stream with the specified ID
                _client.LiveStreams().delete(liveStreamId);
                return Ok(new { message = "Live stream deleted successfully." });
            }
            catch (ApiException e)
            {
                // Log and handle deletion errors
                Console.WriteLine("Exception when calling LiveStreamsApi#delete");
                Console.WriteLine("Status code: " + e.ErrorCode);
                Console.WriteLine("Reason: " + e.Message);

                return BadRequest(new
                {
                    error = "Error deleting live stream",
                    message = e.Message,
                    statusCode = e.ErrorCode
                });
            }
        }
        [HttpPatch("{liveStreamId}")]
        public IActionResult UpdateLiveStream(string liveStreamId, [FromBody] LiveStreamUpdatePayload liveStreamUpdatePayload)
        {
            try
            {
                var liveStream = _client.LiveStreams().update(liveStreamId, liveStreamUpdatePayload);
                return Ok(liveStream);
            }
            catch (ApiException e)
            {
                return BadRequest(new
                {
                    error = "Error updating live stream",
                    message = e.Message,
                    statusCode = e.ErrorCode
                });
            }
        }

        [HttpPost("{liveStreamId}/thumbnail")]
        public IActionResult UploadThumbnail(string liveStreamId, IFormFile thumbnailFile)
        {
            if (thumbnailFile == null || thumbnailFile.Length == 0)
            {
                return BadRequest("Thumbnail file is required.");
            }

            try
            {
                // Convert the uploaded file to a stream
                using (var stream = new MemoryStream())
                {
                    thumbnailFile.CopyTo(stream);
                    stream.Position = 0; // Reset the stream position

                    // Upload the thumbnail
                    var liveStream = _client.LiveStreams().uploadThumbnail(liveStreamId, stream);
                    return Ok(liveStream);
                }
            }
            catch (ApiException e)
            {
                return BadRequest(new
                {
                    error = "Error uploading thumbnail",
                    message = e.Message,
                    statusCode = e.ErrorCode
                });
            }
        }
        [HttpDelete("{liveStreamId}/thumbnail")]
        public IActionResult DeleteThumbnail(string liveStreamId)
        {
            try
            {
                // Access the live streams API instance
                var liveStream = _client.LiveStreams().deleteThumbnail(liveStreamId);

                return Ok(new { message = "Thumbnail deleted successfully", liveStream });
            }
            catch (ApiException e)
            {
                // Log the error details and return an error response
                Console.WriteLine("Exception when calling LiveStreamsApi.deleteThumbnail");
                Console.WriteLine("Status code: " + e.ErrorCode);
                Console.WriteLine("Reason: " + e.Message);

                return BadRequest(new
                {
                    error = "Error deleting thumbnail",
                    message = e.Message,
                    statusCode = e.ErrorCode
                });
            }
        }

        [HttpPut("{liveStreamId}/complete")]
        public IActionResult CompleteLiveStream(string liveStreamId)
        {
            try
            {
                // Complete the live stream with the specified ID
                _client.LiveStreams().complete(liveStreamId);

                return Ok(new { message = "Live stream completed successfully." });
            }
            catch (ApiException e)
            {
                // Log the error details and return an error response
                Console.WriteLine("Exception when calling LiveStreamsApi.complete");
                Console.WriteLine("Status code: " + e.ErrorCode);
                Console.WriteLine("Reason: " + e.Message);

                return BadRequest(new
                {
                    error = "Error completing live stream",
                    message = e.Message,
                    statusCode = e.ErrorCode
                });
            }
        }
    }
}
