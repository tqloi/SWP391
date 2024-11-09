using ApiVideo.Client;
using ApiVideo.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using OnlineLearning.Areas.Instructor.Models;
using OnlineLearning.Models;
using OnlineLearningApp.Respositories;
using System.Diagnostics;

namespace OnlineLearning.Areas.Instructor.Controllers
{
    /// <summary>
    /// Web 
    /// https://docs.api.video/reference/api/Videos
    /// Github
    /// https://github.com/apivideo/api.video-csharp-client
    /// </summary>
    [ApiController]
    [Area("Instructor")]
    [Route("Instructor/Lived-videos/[controller]/[action]")]
    public class LiveStreamVideoController : Controller
    {
        private ApiVideoClient _client;
        private readonly DataContext _datacontext;
        private UserManager<AppUserModel> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public LiveStreamVideoController(DataContext datacontext, UserManager<AppUserModel> userManager, IOptions<ApiVideoSettings> apiVideoSettings, IWebHostEnvironment webHostEnvironment)
        {
            _client = new ApiVideoClient(apiVideoSettings.Value.ApiKey);
            // if you rather like to use the sandbox environment:
            //_client = new ApiVideoClient(apiVideoSettings.Value.ApiKey, ApiVideoClient.Environment.SANDBOX);
            _datacontext = datacontext;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }
        [HttpGet("{CourseID}")]
        [Authorize(Roles = "Instructor")]
        public IActionResult SeeAllLivedVideo(int CourseID)
        {
            var Course = _datacontext.Courses.FirstOrDefault(c => c.CourseID == CourseID);
            if (Course == null)
            {
                return NotFound();
            }
            var recordList = _datacontext.LivestreamRecord
                .ToList()
                .Where(r => r.CourseID == r.CourseID);

            // Create a dictionary to store videos by Title
            Dictionary<string, List<Video>> videosByLivestream = new Dictionary<string, List<Video>>();
            try
            {
                foreach (var record in recordList)
                {
                    var videoList = RetrieveFromLive(record.LivestreamId).data;
                    // Add the retrieved videos to the dictionary
                    if (!videosByLivestream.ContainsKey(record.Title))
                    {
                        videosByLivestream[record.Title] = videoList;
                    }
                    else
                    {
                        videosByLivestream.Add(record.Title, videoList);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return View(videosByLivestream);
        }
        /// <summary>
        /// https://github.com/apivideo/api.video-csharp-client/blob/main/docs/VideosApi.md#list
        /// </summary>
        /// <param name="livestreamId"></param>
        /// <returns></returns>
        private VideosListResponse? RetrieveFromLive(string livestreamId)
        {
            var apiVideosInstance = _client.Videos();
            try
            {
                //list all video for a live with livestreamid
                var result = apiVideosInstance.list(liveStreamId: livestreamId);
                return result;
            }
            catch (ApiException e)
            {
                Debug.Print("Exception when calling VideosApi.get: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
                return null;
            }
        }

        [HttpPost]
        [Authorize(Roles = "Instructor")]
        public IActionResult DeleteVideoObject(string videoId, int CourseID)
        {
            var apiVideosInstance = _client.Videos();
            try
            {
                //delete video
                apiVideosInstance.delete(videoId);
                TempData["success"] = "Delete Video Successfully";
                TempData.Keep();
                return RedirectToAction("SeeAllLivedVideo", new { CourseID = CourseID });
            }
            catch (ApiException e)
            {
                Debug.Print("Exception when calling VideosApi.delete: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
                TempData["error"] = "Delete Video Failed";
                TempData.Keep();
                return RedirectToAction("SeeAllLivedVideo", new { CourseID = CourseID });

            }
        }
        private Video? RetreiveVideoObject(string videoId)
        {
            var apiVideosInstance = _client.Videos();
            try
            {
                // Show a video
                Video result = apiVideosInstance.get(videoId);
                return result;
            }
            catch (ApiException e)
            {
                return null;
            }
        }
    }
}
