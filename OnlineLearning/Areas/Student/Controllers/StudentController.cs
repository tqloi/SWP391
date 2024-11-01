using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using OnlineLearning.Models.ViewModel;
using OnlineLearning.Models;
using OnlineLearningApp.Respositories;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using OnlineLearning.Controllers;
using Microsoft.AspNetCore.Authorization;
using OnlineLearning.Services;
using Microsoft.EntityFrameworkCore;
using OnlineLearning.Areas.Student.Models.ViewModel;
using SelectPdf;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Firebase.Auth;
using YourNamespace.Models;


namespace OnlineLearning.Areas.Student.Controllers
{
    [Area("Student")]
    [Authorize(Roles = "Student")]
    [Route("Student/[controller]/[action]")]
    public class StudentController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DataContext datacontext;
        private UserManager<AppUserModel> _userManager;
        private SignInManager<AppUserModel> _signInManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly FileService _fileService;
        public StudentController(ILogger<HomeController> logger, DataContext context, SignInManager<AppUserModel> signInManager, UserManager<AppUserModel> userManager, IWebHostEnvironment webHostEnvironment, FileService fileService)
        {
            datacontext = context;
            _logger = logger;
            _signInManager = signInManager;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
            _fileService = fileService;
        }
        public IActionResult InstructorRegistration()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> InstructorRegistration(UploadFileViewModel model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (model.Filepdf != null)
            {
                var instructorConfirmation = new InstructorConfirmationModel();
                string fileName = model.Filepdf.FileName;
                try
                {
                    string downloadUrl = await _fileService.UploadInstructorCV(model.Filepdf);
                    instructorConfirmation.Certificatelink = downloadUrl;
                    instructorConfirmation.FileName = fileName;
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error uploading file: " + ex.Message);
                    TempData["error"] = "Edit failed due to file upload error!";
                    return View(model);
                }
                instructorConfirmation.UserID = userId;
                instructorConfirmation.SendDate = DateTime.Now;
                instructorConfirmation.Description = model.Description;

                datacontext.InstructorConfirmation.Add(instructorConfirmation);
                await datacontext.SaveChangesAsync();

                TempData["success"] = "Request sended successfully!";
                return View();
            }
            TempData["error"] = "An error occurred while processing the payment. Please try again.";
            return View(model);
        }

        [ServiceFilter(typeof(CourseAccessFilter))]
        public async Task<IActionResult> GradeList(int CourseID)
        {
            var course = await datacontext.Courses.FindAsync(CourseID);
            ViewBag.Course = course;
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }
            var testScore = await datacontext.Score.Where(t => t.StudentID.Equals(user.Id)).Include(t => t.Test).ToListAsync();
            var assignmentScore = await datacontext.ScoreAssignment.Where(a => a.StudentID.Equals(user.Id)).Include(a => a.Assignment).ToListAsync();
            var model = new GradeListViewModel
            {
                scoretests = testScore,
                scoreAssignments = assignmentScore
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> GenerateCertificate(string StudentID, int CourseID, string InstructorID)
        {
            var student = await _userManager.FindByIdAsync(StudentID);
            var instructor = await _userManager.FindByIdAsync(InstructorID);
            var course = await datacontext.Courses.FindAsync(CourseID);
            var studentName = $"{student.FirstName} {student.LastName}";
            var studentCourse = await datacontext.StudentCourses
                               .Where(x => x.CourseID == CourseID && x.StudentID == StudentID)
                               .FirstOrDefaultAsync();

            var certificate = new CertificateModel
            {
                StudentID = StudentID,
                CourseID = CourseID,
                CompletionDate = DateTime.Now,
                CertificateLink = ""
            };

            string htmlString = $@"
    <!DOCTYPE html>
    <html lang='en'>
    <head>
        <meta charset='UTF-8'>
        <title>Certificate</title>
        <style>
            body {{ font-family: Arial, sans-serif; text-align: center; padding: 50px; }}
            .certificate {{ border: 5px solid #4CAF50; padding: 20px; border-radius: 10px; box-shadow: 2px 2px 12px #aaaaaa; }}
            .certificate h1 {{ font-size: 36px; color: #4CAF50; }}
            .certificate p {{ font-size: 24px; }}
            .signature {{ margin-top: 50px; font-size: 20px; }}
        </style>
    </head>
    <body>
        <div class='certificate'>
            <h1>Certificate of Completion</h1>
            <p>This is to certify that</p>
            <h2>{studentName}</h2>
            <p>has successfully completed the course</p>
            <h3>{course.Title}</h3>
            <p>on {DateTime.Now:dd/MM/yyyy}</p>
            <div class='signature'>
                <p>__________________</p>
                <p>{instructor.LastName}</p>
                <p>{instructor.FirstName} {instructor.LastName}</p>
            </div>
        </div>
    </body>
    </html>";

            HtmlToPdf converter = new HtmlToPdf();
            PdfDocument doc = converter.ConvertHtmlString(htmlString, "https://www.example.com");

            byte[] pdfBytes;
            using (var ms = new MemoryStream())
            {
                doc.Save(ms);
                pdfBytes = ms.ToArray();
            }
            doc.Close();

            using (var stream = new MemoryStream(pdfBytes))
            {
                IFormFile formFile = new FormFile(stream, 0, stream.Length, "file", $"{studentName}_{course.Title}.pdf")
                {
                    Headers = new HeaderDictionary(),
                    ContentType = "application/pdf"
                };

                try
                {
                    string downloadUrl = await _fileService.UploadCertificate(formFile);
                    certificate.CertificateLink = downloadUrl;
                    studentCourse.CertificateStatus = "Completed";
                    studentCourse.CompletionDate = DateTime.Now;
                    datacontext.Certificate.Add(certificate);
                    await datacontext.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error uploading file: " + ex.Message);
                    return RedirectToAction("CourseInfo", "Participation");
                }
            }

            return File(pdfBytes, "application/pdf", $"{studentName}_{course.Title}.pdf");
        }
    }
}
