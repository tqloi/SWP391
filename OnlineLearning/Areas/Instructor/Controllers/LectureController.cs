using Firebase.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic.FileIO;
using OnlineLearning.Filter;
using OnlineLearning.Models;
using OnlineLearning.Models.ViewModel;
using OnlineLearning.Services;
using OnlineLearningApp.Respositories;
using System.Security.Claims;
using YourNamespace.Models;

namespace OnlineLearning.Areas.Instructor.Controllers
{
    [Area("Instructor")]
    [Authorize(Roles = "Instructor")]
    [Route("Instructor/[controller]/[action]")]
    public class LectureController : Controller
    {
        private readonly DataContext _dataContext;
        private UserManager<AppUserModel> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly FileService _fileService;

        public LectureController(DataContext context, UserManager<AppUserModel> userManager, IWebHostEnvironment webHostEnvironment, FileService fileService)
        {
            _dataContext = context;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
            _fileService = fileService;
        }
        [HttpPost]
        public async Task<IActionResult> Create(LectureViewModel model)
        {
            var course = await _dataContext.Courses.FindAsync(model.CourseID);
            ViewBag.Course = course;

            try
            {
                var lecture = new LectureModel
                {
                    CourseID = model.CourseID,
                    Title = model.Title,
                    Description = model.Description,
                    UpLoadDate = DateTime.Now,
                };

                _dataContext.Lecture.Add(lecture);
                await _dataContext.SaveChangesAsync();

                if (model.VideoFile != null)
                {
                    var lectueFile = new LectureFileModel();
                    string fileName = model.VideoFile.FileName;
                    try
                    {
                        string downloadUrl = await _fileService.UploadVideo(model.VideoFile);
                        lectueFile.LectureID = lecture.LectureID;
                        lectueFile.FilePath = downloadUrl;
                        lectueFile.FileName = fileName;
                        lectueFile.FileType = "Video";
                        _dataContext.LectureFiles.Add(lectueFile);
                        await _dataContext.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", "Error uploading file: " + ex.Message);
                        TempData["error"] = "Edit failed due to file upload error!";
                        return View(model);
                    }
                }

                if (model.LectureFile != null && model.LectureFile.Count > 0)
                {
                    foreach (var file in model.LectureFile)
                    {
                        var lectueFile = new LectureFileModel();
                        string fileName = file.FileName;
                        try
                        {
                            string downloadUrl = await _fileService.UploadLectureDocument(file);
                            lectueFile.LectureID = lecture.LectureID;
                            lectueFile.FilePath = downloadUrl;
                            lectueFile.FileName = fileName;
                            lectueFile.FileType = "Document";
                            _dataContext.LectureFiles.Add(lectueFile);
                            await _dataContext.SaveChangesAsync();
                        }
                        catch (Exception ex)
                        {
                            ModelState.AddModelError("", "Error uploading file: " + ex.Message);
                            TempData["error"] = "Edit failed due to file upload error!";
                            return View(model);
                        }
                    }

                }

                TempData["success"] = "Lecture Addded successfully!";
                //return RedirectToAction("Index", "Instructor", new { area = "Instructor" });
                return RedirectToAction("LectureDetail", "Lecture", new { area = "Instructor", LectureID = lecture.LectureID });
            }
            catch
            {
                TempData["success"] = "Added Failed!";
                //return RedirectToAction("Index", "Instructor", new { area = "Instructor" });
                return RedirectToAction("CourseInfo", "Participation", new { CourseID = model.CourseID });
            }
        }
        [HttpGet]
        [ServiceFilter(typeof(CourseAccessFilter))]
        public async Task<IActionResult> LectureDetail(int LectureID)
        {
            var lecture = await _dataContext.Lecture.FindAsync(LectureID);
            var course = await _dataContext.Courses.FindAsync(lecture.CourseID);
            var lectureFiles = await _dataContext.LectureFiles.Where(lf => lf.LectureID == LectureID).ToListAsync();

            ViewBag.Course = course;
            ViewBag.Lecture = lecture;

            return View(lectureFiles);
        }
        public async Task<IActionResult> GoNext(int lectureID)
        {
            var currentLecture = await _dataContext.Lecture.FindAsync(lectureID);
            if (currentLecture == null)
            {
                return NotFound();
            }

            var nextLecture = await _dataContext.Lecture
                .Where(l => l.CourseID == currentLecture.CourseID && l.LectureID > currentLecture.LectureID)
                .OrderBy(l => l.LectureID)
                .FirstOrDefaultAsync();

            if (nextLecture != null)
            {
                return RedirectToAction("LectureDetail", "Lecture", new { area = "Instructor", LectureID = nextLecture.LectureID });
            }
            else
            {
                TempData["error"] = "This is the last lecture in the course.";
                return RedirectToAction("LectureDetail", "Lecture", new { area = "Instructor", LectureID = lectureID });
            }
        }


        [HttpGet]
        public async Task<IActionResult> GoPrevious(int lectureID)
        {
            var currentLecture = await _dataContext.Lecture.FindAsync(lectureID);
            if (currentLecture == null)
            {
                return NotFound();
            }

            var previousLecture = await _dataContext.Lecture
                .Where(l => l.CourseID == currentLecture.CourseID && l.LectureID < currentLecture.LectureID)
                .OrderByDescending(l => l.LectureID)
                .FirstOrDefaultAsync();

            if (previousLecture != null)
            {
                return RedirectToAction("LectureDetail", "Lecture", new { area = "Instructor", LectureID = previousLecture.LectureID });
            }
            else
            {
                TempData["error"] = "This is the first lecture in the course.";
                return RedirectToAction("LectureDetail", "Lecture", new { area = "Instructor", LectureID = lectureID });
            }
        }
    }
}
