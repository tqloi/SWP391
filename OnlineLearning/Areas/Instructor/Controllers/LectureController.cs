using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineLearning.Models;
using OnlineLearning.Models.ViewModel;
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

        public LectureController(DataContext context, UserManager<AppUserModel> userManager, IWebHostEnvironment webHostEnvironment)
        {
            datacontext = context;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
        }
        [HttpPost]
        public async Task<IActionResult> Create(LectureViewModel model)
        {
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
                    string uploadpath = Path.Combine(_webHostEnvironment.WebRootPath, "CourseVideo");
                    string fileName = Guid.NewGuid() + "_" + model.VideoFile.FileName;
                    string filepath = Path.Combine(uploadpath, fileName);

                    using (var fs = new FileStream(filepath, FileMode.Create))
                    {
                        await model.VideoFile.CopyToAsync(fs);
                    }
                    var lectueFile = new LectureFileModel
                    {
                        LectureID = lecture.LectureID,
                        FilePath = fileName,
                    };
                    datacontext.LectureFiles.Add(lectueFile);
                    await datacontext.SaveChangesAsync();
                }

                if (model.LectureFile != null && model.LectureFile.Count > 0)
                {
                    string uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "CourseMaterials");
                    foreach (var file in model.LectureFile)
                    {
                        string fileName = Guid.NewGuid() + "_" + file.FileName;
                        string filePath = Path.Combine(uploadPath, fileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(fileStream);
                        }
                        var lectueFile = new LectureFileModel
                        {
                            LectureID = lecture.LectureID,
                            FilePath = fileName,
                        };
                        datacontext.LectureFiles.Add(lectueFile);
                        await datacontext.SaveChangesAsync();
                    }
                }

                TempData["success"] = "Lecture Addded successfully!";
                //return RedirectToAction("Index", "Instructor", new { area = "Instructor" });
                return RedirectToAction("LectureList", "Participation", new { id = lecture.CourseID });
            }
            catch
            {
                TempData["success"] = "Added Failed!";
                //return RedirectToAction("Index", "Instructor", new { area = "Instructor" });
                return RedirectToAction("LectureList", "Participation", new { id = model.CourseID });
            }
        }
    }
}
