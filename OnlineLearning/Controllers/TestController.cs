using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using Newtonsoft.Json;
using OnlineLearning.Models;
using OnlineLearning.Models.ViewModel;
using OnlineLearningApp.Respositories;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Security.Claims;

namespace OnlineLearning.Controllers
{
    [Authorize]
    public class TestController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DataContext datacontext;
        private UserManager<AppUserModel> _userManager;
        private SignInManager<AppUserModel> _signInManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public TestController(ILogger<HomeController> logger, DataContext context, SignInManager<AppUserModel> signInManager, UserManager<AppUserModel> userManager, IWebHostEnvironment webHostEnvironment)
        {
            datacontext = context;
            _logger = logger;
            _signInManager = signInManager;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
        }

        //currently not in use yet
        public IActionResult Index(int CourseID)
        {
            ViewBag.CourseID = CourseID;
            var Course = datacontext.Courses.Find(CourseID);
            //absolutely useless foking line but i add it just for fun
            ViewBag.Course = Course;
            return View();
        }

        public IActionResult CreateTestRedirector(int TestID)
        {
            var Test = datacontext.Test.FirstOrDefault(t => t.TestID == TestID);
            ViewBag.CourseID = Test.CourseID;
            if (Test == null)
            {
                return NotFound();
            }
            return View("EditTest", Test);
        }
        public IActionResult EditTestRedirector(int CourseID)
        {

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateTest(TestModel model)
        {
            var Course = datacontext.Courses.Find(model.CourseID);
            if (Course == null)
            {
                return NotFound();
            }
            //fucku viewbag shit
            if (model.Course == null)
            {
                model.Course = Course;
            }
            // if (ModelState.IsValid) invalid as always
            try
            {
                Debug.WriteLine("ID retrieved valid");
                var newTest = new TestModel
                {
                    Title = model.Title,
                    Course = model.Course,
                    Description = model.Description,
                    StartTime = model.StartTime,
                    EndTime = model.EndTime,
                    Status = model.Status,
                    CourseID = model.CourseID,
                    NumberOfQuestion = 0
                };
                // Add the test to the context and save changes
                datacontext.Test.Add(newTest);
                await datacontext.SaveChangesAsync();
                Debug.WriteLine("Test saved to database");

                TempData["success"] = "Test created successfully!";
                return RedirectToAction("CreateTestRedirector", new { courseID = model.CourseID });
            }

            catch (Exception)
            {
                TempData["error"] = "Test creation failed!";
                return View();
            }
        }

        [HttpGet]
        public IActionResult DoTest(int TestID)
        {
            var Test = datacontext.Test.FirstOrDefault(t => t.TestID == TestID);
            if (Test.Course == null)
            {
                try
                {
                    var Course = datacontext.Courses.FirstOrDefault(c => c.CourseID == Test.CourseID);
                    Test.Course = Course;
                }
                catch
                {
                    TempData["Error"] = "Course Null";
                    return View("~/Views/Participation/TestList.cshtml");
                }
            }
            ViewBag.Course = Test.Course;
            ViewBag.TestID = TestID;
            ViewBag.CourseID = Test.CourseID;

            var list = datacontext.Question.ToList()
                .Where(q => q.TestID == TestID);
            return View(list);
        }

        //https://stackoverflow.com/questions/13621934/validateantiforgerytoken-purpose-explanation-and-example
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DoTest(Dictionary<int, string> answers, CourseModel course, int courseID, int testID)
        {
            // Retrieve the User from the claims
            var studentID = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(studentID);

            course = datacontext.Courses.FirstOrDefault(c => c.CourseID == courseID);

            var test = datacontext.Test.FirstOrDefault(t => t.TestID == testID);
            Dictionary<int, string> correctAnswers = new Dictionary<int, string>();

            int totalQuestions = datacontext.Question.ToList()
                .Where(q => q.TestID == testID).Count();
            int correctAnswersCount = 0;
            // Iterate through the answers
            foreach (var answer in answers)
            {
                int questionId = answer.Key; // The Question ID
                string selectedAnswer = answer.Value; // The selected answer

                var question = datacontext.Question.FirstOrDefault(c => c.QuestionID == questionId);

                if (question != null)
                {
                    string correctAnswer = question.CorrectAnswer;
                    correctAnswers.Add(questionId, correctAnswer);
                    // Check if the selected answer is correct, ignorecase
                    if (selectedAnswer.Equals(correctAnswer, StringComparison.OrdinalIgnoreCase))
                    {
                        correctAnswersCount++; // Increment count for correct answers
                    }
                }
            }

            // Calculate the score as a float out of 10.0
            double score = (float)correctAnswersCount / totalQuestions * 10.0f;
            //var previousResult = datacontext.Score.FromSqlRaw("SELECT * FROM Score WHERE TestID = {0} AND StudentID = {1}", testID, userID).FirstOrDefault();
            var previousResult = datacontext.Score.FirstOrDefault(s => s.TestID == testID && s.StudentID == studentID);
            if (previousResult != null)
            {
                previousResult.Student = user;
                previousResult.Score = score;
                previousResult.TestID = testID;
                previousResult.StudentID = studentID;
                previousResult.Test = test;

                datacontext.SaveChanges();
                //TempData["correctAnswers"] = correctAnswers;
                //TempData["answers"] = answers;
                //serialize the dictionary into a JSON string before storing it in TempData
                TempData["correctAnswers"] = JsonConvert.SerializeObject(correctAnswers);
                TempData["answers"] = JsonConvert.SerializeObject(answers);
                return RedirectToAction("TestResult", new { ScoreID = previousResult.ScoreID, studentID = studentID });
            }
            ScoreModel result = new ScoreModel
            {
                Student = user,
                Score = score,
                TestID = testID,
                StudentID = studentID,
                Test = test
            };

            datacontext.Score.Add(result);
            datacontext.SaveChanges();
            //TempData["correctAnswers"] = correctAnswers;
            //TempData["answers"] = answers;
            //serialize the dictionary into a JSON string before storing it in TempData
            TempData["correctAnswers"] = JsonConvert.SerializeObject(correctAnswers);
            TempData["answers"] = JsonConvert.SerializeObject(answers);

            return RedirectToAction("TestResult", new { ScoreID = result.ScoreID, studentID = studentID });
        }
        [HttpGet]
        public async Task<IActionResult> TestResult(int ScoreID, string studentID)
        {
            var user = await _userManager.FindByIdAsync(studentID);
            var result = datacontext.Score.FirstOrDefault(s => s.ScoreID == ScoreID);
            var test = datacontext.Test.FirstOrDefault(t => t.TestID == result.TestID);
            var course = datacontext.Courses.Find(test.CourseID);

            ViewBag.Course = course;

            //var correctAnswers = TempData["correctAnswers"] as Dictionary<int, string>;
            //var answers = TempData["answers"] as Dictionary<int, string>; 
            //Deserialize when retrieving from TempData
            var correctAnswers = JsonConvert.DeserializeObject<Dictionary<int, string>>(TempData["correctAnswers"] as string);
            var answers = JsonConvert.DeserializeObject<Dictionary<int, string>>(TempData["answers"] as string);

            TestResultViewModel model = new TestResultViewModel
            {
                Answers = answers,
                CorrectAnswers = correctAnswers,
                CourseName = course.Title,
                Score = result.Score,
                TestID = result.TestID,
                TotalQuestions = result.Test.NumberOfQuestion
            };

            return View(model);
        }

    }
}
