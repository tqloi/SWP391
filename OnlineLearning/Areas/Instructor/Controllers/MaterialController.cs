using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineLearning.Filter;
using OnlineLearning.Models;
using OnlineLearning.Models.ViewModel;
using OnlineLearning.Services;
using OnlineLearningApp.Respositories;

namespace OnlineLearning.Areas.Instructor.Controllers
{
    [Area("Instructor")]
    [Authorize(Roles = "Instructor")]
    [Route("Instructor/[controller]/[action]")]
    public class MaterialController : Controller
    {
        private UserManager<AppUserModel> _userManager;
        private SignInManager<AppUserModel> _signInManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly DataContext _dataContext;
        private IConfiguration _configuration;
        private readonly FileService _fileService;
        public MaterialController(DataContext dataContext, UserManager<AppUserModel> userManager, SignInManager<AppUserModel> signInManager, IWebHostEnvironment webHostEnvironment, IConfiguration configuration, FileService fileService)
        {
            _dataContext = dataContext;
            _userManager = userManager;
            _signInManager = signInManager;
            _webHostEnvironment = webHostEnvironment;
            _configuration = configuration;
            _fileService = fileService;
        }

        public IActionResult Index()
        {
            return View();
        }


        [HttpGet]
        [ServiceFilter(typeof(CourseAccessFilter))]
        public async Task<IActionResult> MaterialList(int courseID)
        {

            var materialList = await _dataContext.CourseMaterials
                                     .Where(m => m.CourseID == courseID)
                                     .ToListAsync();
            var course = await _dataContext.Courses.FindAsync(courseID);


            ViewBag.Course = course;

            return View(materialList);
        }
        [HttpPost]
        public async Task<IActionResult> AddMaterial(CourseMaterialViewModel model, int courseID)
        {
            var course = await _dataContext.Courses.FindAsync(courseID);
            ViewBag.Course = course;
            try
            {  
                if (model.CourseMaterial != null && model.CourseMaterial.Count > 0)
                {
                    foreach (var material in model.CourseMaterial)
                    {
                        var Material = new CourseMaterialModel();
                        string fileName = material.FileName;
                        try
                        {
                            string downloadUrl = await _fileService.UploadCourseDocument(material);
                            Material.CourseID = courseID;
                            Material.MaterialsLink = downloadUrl;
                            Material.FIleName = fileName;
                            Material.fileExtension = Path.GetExtension(fileName);
                            _dataContext.CourseMaterials.Add(Material);
                            await _dataContext.SaveChangesAsync();
                        }
                        catch (Exception ex)
                        {
                            ModelState.AddModelError("", "Error uploading file: " + ex.Message);
                            TempData["error"] = "Edit failed due to file upload error!";
                            return RedirectToAction("MaterialList", "Material", new { area = "Instructor", CourseID = courseID });

                        }
                    }

                }

                TempData["success"] = "Material Addded successfully!";
                //return RedirectToAction("Index", "Instructor", new { area = "Instructor" });
                  return RedirectToAction("MaterialList", "Material", new { area = "Instructor", CourseID = courseID });
            }
            catch
            {
                TempData["error"] = "Added Failed!";
                //return RedirectToAction("Index", "Instructor", new { area = "Instructor" });
                return RedirectToAction("MaterialList", "Material", new { area = "Instructor", CourseID = courseID });
            }
        }
        [HttpPost]
        public async Task<IActionResult>DeleteMaterial(int materialID,int courseID)
        {
            var course = await _dataContext.Courses.FindAsync(courseID);
            ViewBag.Course = course;
            var  material = await _dataContext.CourseMaterials.FindAsync(materialID);
            if (material != null)
            {
            _dataContext.CourseMaterials.Remove(material);
            await _dataContext.SaveChangesAsync();               
            }
            return RedirectToAction("MaterialList", "Material", new { area = "Instructor", CourseID = courseID });
        }
    }
}
