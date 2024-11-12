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

namespace OnlineLearning.Areas.Student.Controllers
{
    /// <summary>
    /// Web
    /// https://docs.api.video/reference/api/Live-Streams
    /// Github
    /// https://github.com/apivideo/api.video-csharp-client
    /// </summary>
    [ApiController]
    [Area("Student")]
    [Route("Student/live-streams/[controller]/[action]")]
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
            //_client = new ApiVideoClient(apiVideoSettings.Value.ApiKey, ApiVideoClient.Environment.SANDBOX);
            _datacontext = datacontext;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
            _fileService = fileService;
        }
        [HttpGet("{CourseID}")]
        [Authorize(Roles = "Student")]
        public IActionResult SeeAllLive(int CourseID)
        {
            var course = _datacontext.Courses.FirstOrDefault(c => c.CourseID == CourseID);
            var liveList = new List<LiveStream>();

            try
            {
                var records = _datacontext.LivestreamRecord
                    .ToList()
                    .Where(r => r.CourseID == CourseID);
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
                        else
                        {
                            if (live._public == true)
                            {
                                liveList.Add(live);
                            }
                        }
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
            // Sort active streams first, followed by inactive streams
            var sortedlist = liveList.OrderByDescending(stream => stream.broadcasting).ToList();
            return View(sortedlist);
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
