using Firebase.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic.FileIO;
using OnlineLearning.Models;
using OnlineLearning.Models.ViewModel;
using OnlineLearning.Services;
using OnlineLearningApp.Respositories;
using System.Security.Claims;
using YourNamespace.Models;

namespace OnlineLearning.Areas.Instructor.Controllers
{
    [Area("Instructor")]
    [Authorize]

    [Route("/[controller]/[action]")]
    public class LectureController : Controller
    {
        private readonly DataContext datacontext;
        private UserManager<AppUserModel> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly FileService _fileService;

        public LectureController(DataContext context, UserManager<AppUserModel> userManager, IWebHostEnvironment webHostEnvironment, FileService fileService)
        {
            datacontext = context;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
            _fileService = fileService;
        }
        [HttpPost]
        public async Task<IActionResult> Create(LectureViewModel model)
        {
            var course = await datacontext.Courses.FindAsync(model.CourseID);
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

                datacontext.Lecture.Add(lecture);
                await datacontext.SaveChangesAsync();

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
                        datacontext.LectureFiles.Add(lectueFile);
                        await datacontext.SaveChangesAsync();
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
                            datacontext.LectureFiles.Add(lectueFile);
                            await datacontext.SaveChangesAsync();
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
                return RedirectToAction("LectureDetail", "Participation", new { LectureID = lecture.LectureID });
            }
            catch
            {
                TempData["success"] = "Added Failed!";
                //return RedirectToAction("Index", "Instructor", new { area = "Instructor" });
                return RedirectToAction("CourseInfo", "Participation", new { CourseID = model.CourseID });
            }
        }
    }
}
