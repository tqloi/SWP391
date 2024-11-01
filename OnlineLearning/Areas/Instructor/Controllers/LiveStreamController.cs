using ApiVideo.Api;
using ApiVideo.Client;
using ApiVideo.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using MimeKit.Cryptography;
using OnlineLearning.Areas.Instructor.Models;
using OnlineLearning.Models;
using OnlineLearning.Services;
using OnlineLearningApp.Respositories;
using System.Collections.Generic;
using System.Security.Claims;

namespace OnlineLearning.Areas.Instructor.Controllers
{
    /// <summary>
    /// Web
    /// https://docs.api.video/reference/api/Live-Streams
    /// Github
    /// https://github.com/apivideo/api.video-csharp-client
    /// </summary>
    [ApiController]
    [Area("Instructor")]
    [Route("Instructor/live-streams/[controller]/[action]")]
    public class LiveStreamController : Controller
    {
        private ApiVideoClient _client;
        private readonly DataContext _datacontext;
        private UserManager<AppUserModel> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly FileService _fileService;
        public LiveStreamController(DataContext datacontext, UserManager<AppUserModel> userManager, IOptions<ApiVideoSettings> apiVideoSettings, IWebHostEnvironment webHostEnvironment, FileService fileService)
        {
            _client = new ApiVideoClient(apiVideoSettings.Value.ApiKey);
            // if you rather like to use the sandbox environment:
            //_client = new ApiVideoClient("EugYjajLHbC5jEkLAng2tywbO1nP2vY3NmMX4nNtyyR", ApiVideoClient.Environment.SANDBOX);
            _datacontext = datacontext;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
            _fileService = fileService;
        }
        [HttpGet]
        public IActionResult SeeAllLive(int CourseID)
        {
            var course = _datacontext.Courses.FirstOrDefault(c => c.CourseID == CourseID);
            var liveList = new List<LiveStream>();
            try
            {
                var user = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var records = _datacontext.LivestreamRecord
                    .ToList()
                    .Where(r => r.UserID.Equals(user));
                if (records != null)
                {
                    foreach (var record in records)
                    {
                        var live = RetrieveLiveStream(record.LivestreamId);
                        if (live == null)
                        {
                            TempData["error"] = "Could not found live in the server";
                            return RedirectToAction("Livestream", "Participation", new { CourseID = CourseID });
                        }
                        liveList.Add(live);
                    }
                }
                else
                {
                    TempData["error"] = "No livestream found";
                    TempData.Keep();
                    return RedirectToAction("Livestream", "Participation", new { CourseID = CourseID });
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            ViewBag.Course = course;
            ViewBag.CourseID = CourseID;
            return View(liveList);
        }
        [HttpGet("{liveStreamId}")]
        public IActionResult Details(string liveStreamId)
        {
            var live = _client.LiveStreams().get(liveStreamId);
            var record = _datacontext.LivestreamRecord.FirstOrDefault(r => r.LivestreamId.Equals(liveStreamId));
            var course = _datacontext.Courses.FirstOrDefault(c => c.CourseID == record.CourseID);
            ViewBag.Course = course;
            return View(live);
        }
        /// <summary>
        /// Should never be use, if so use by admin at least.
        /// </summary>
        /// <param name="liveStreamId"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        //[Route("/live-streams")]
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
        [Authorize(Roles = "Instructor")]
        //field now fix temporary
        public IActionResult CreateLiveStream([FromForm] string Title, [FromForm] int CourseID)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var course = _datacontext.Courses.FirstOrDefault(c => c.CourseID == CourseID);
            if (course == null)
            {
                return NotFound();
            }

            var liveStreamCreationPayload = new LiveStreamCreationPayload
            {
                name = Title,
                _public = true,
                //playerid = "_playerId"

                //currently no restream
            };

            try
            {
                var liveStream = _client.LiveStreams().create(liveStreamCreationPayload);
                var record = new LivestreamRecordModel
                {
                    CreateDate = liveStream.createdat ?? DateTime.Now,
                    UpdateDate = liveStream.updatedat ?? DateTime.Now,
                    UserID = userId,
                    LivestreamId = liveStream.livestreamid,
                    Title = Title,
                    CourseID = CourseID,

                };
                _datacontext.LivestreamRecord.Add(record);
                _datacontext.SaveChanges();
                ViewBag.CourseID = CourseID;
                TempData["success"] = "Create Success";
                return RedirectToAction("SeeAllLive", new { CourseID = CourseID });
            }
            catch (ApiException e)
            {
                // Handle the error (e.g., log it and return an error response)
                return BadRequest($"Error creating live stream: {e.Message}");
            }
        }
        private LiveStream? RetrieveLiveStream(string liveStreamId)
        {
            try
            {
                // Access the live streams API instance
                LiveStreamsApi apiInstance = _client.LiveStreams();

                // Retrieve the live stream with the specified ID
                var liveStream = apiInstance.get(liveStreamId);

                return liveStream; // Return live stream details
            }
            catch (ApiException e)
            {
                // Log the error details and return an error response
                Console.WriteLine("Exception when calling LiveStreamsApi#get");
                Console.WriteLine("Status code: " + e.ErrorCode);
                Console.WriteLine("Reason: " + e.Message);
                //Console.WriteLine("Response headers: " + e.getResponseHeaders());
            }
            return null;
        }

        [HttpPost]
        [Authorize(Roles = "Instructor")]
        public IActionResult DeleteLiveStream([FromQuery] string liveStreamId)
        {
            try
            {
                var record = _datacontext.LivestreamRecord.FirstOrDefault(r => r.LivestreamId.Equals(liveStreamId));

                if (record == null)
                {
                    // If no record is found, return a NotFound response or an appropriate message
                    TempData["error"] = "Live stream not found.";
                    return RedirectToAction("SeeAllLive", new { CourseID = record.CourseID });
                }

                // Attempt to delete the live stream with the specified ID
                _client.LiveStreams().delete(liveStreamId);
                _datacontext.LivestreamRecord.Remove(record);
                _datacontext.SaveChanges();

                TempData["success"] = "Live stream deleted successfully";
                return RedirectToAction("SeeAllLive", new { CourseID = record.CourseID });
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

        [HttpPost]
        [Authorize(Roles = "Instructor")]
        public IActionResult UpdateLiveStream([FromQuery] string liveStreamId, [FromForm] string _public, [FromForm] string name)
        {
            try
            {
                bool isPublic;  // Declare the variable
                bool.TryParse(_public, out isPublic);
                LiveStreamUpdatePayload liveStreamUpdatePayload = new LiveStreamUpdatePayload
                {
                    name = name,
                    _public = isPublic,
                };
                var liveStream = _client.LiveStreams().update(liveStreamId, liveStreamUpdatePayload);
                var record = _datacontext.LivestreamRecord.FirstOrDefault(r => r.LivestreamId == liveStreamId);
                record.Title = name;
                record.UpdateDate = liveStream.updatedat ?? DateTime.Now;
                _datacontext.LivestreamRecord.Update(record);
                _datacontext.SaveChanges();
                TempData["success"] = "Update Livestream successfully";
                return RedirectToAction("SeeAllLive", new { CourseID = record.CourseID });
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

        //  [HttpPost("{liveStreamId}/thumbnail")]
        [HttpPost]
        [Authorize(Roles = "Instructor")]
        public IActionResult UploadThumbnail([FromQuery] string liveStreamId, [FromForm] IFormFile thumbnailFile)
        {
            if (liveStreamId == null)
            {
                return BadRequest("livestreamId not found.");
            }
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
                    // Reset the stream position
                    stream.Position = 0;

                    // Upload the thumbnail
                    var liveStream = _client.LiveStreams().uploadThumbnail(liveStreamId, stream);
                    var record = _datacontext.LivestreamRecord.FirstOrDefault(s => s.LivestreamId == liveStreamId);
                    return RedirectToAction("SeeAllLive", new { CourseID = record.CourseID });
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
        // [HttpDelete("{liveStreamId}/thumbnail")]
        [HttpPost]
        [Authorize(Roles = "Instructor")]
        public IActionResult DeleteThumbnail(string liveStreamId)
        {
            try
            {
                var record = _datacontext.LivestreamRecord.FirstOrDefault(r => r.LivestreamId.Equals(liveStreamId));
                // Access the live streams API instance
                var liveStream = _client.LiveStreams().deleteThumbnail(liveStreamId);
                TempData["success"] = "Thumbnail delete successfully";
                TempData.Keep();
                return RedirectToAction("SeeAllLive", new { CourseID = record.CourseID });
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

        [HttpPost]
        [Authorize(Roles = "Instructor")]
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
        //Not sure if needed?
        private LiveStreamAssets ParseLiveStreamAssets(string assetsString)
        {
            var liveStreamAssets = new LiveStreamAssets();

            // Remove the outer braces and split the string into lines
            var lines = assetsString.Trim(new char[] { '{', '}', ' ' }).Split(new[] { System.Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in lines)
            {
                // Split by the first colon to separate the key and value
                var parts = line.Split(new[] { ':' }, 2);
                if (parts.Length == 2)
                {
                    var key = parts[0].Trim();
                    var value = parts[1].Trim().Trim(new char[] { ' ', '"' });

                    switch (key)
                    {
                        case "Hls":
                            liveStreamAssets.hls = value;
                            break;
                        case "Iframe":
                            liveStreamAssets.iframe = value;
                            break;
                        case "Player":
                            liveStreamAssets.player = value;
                            break;
                        case "Thumbnail":
                            liveStreamAssets.thumbnail = value;
                            break;
                    }
                }
            }

            return liveStreamAssets;
        }
    }
}
