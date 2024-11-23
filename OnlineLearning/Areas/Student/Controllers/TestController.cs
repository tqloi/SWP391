using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using Newtonsoft.Json;
using OnlineLearning.Controllers;
using OnlineLearning.Hubs;
using OnlineLearning.Models;
using OnlineLearning.Models.ViewModel;
using OnlineLearningApp.Respositories;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Security.Claims;

namespace OnlineLearning.Areas.Student.Controllers
{
    [Authorize]
    [Route("Student/[controller]/[action]")]
    public class TestController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DataContext datacontext;
        private UserManager<AppUserModel> _userManager;
        private SignInManager<AppUserModel> _signInManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IHubContext<TestHub> _hubContext;
        public TestController(ILogger<HomeController> logger, DataContext context, SignInManager<AppUserModel> signInManager, UserManager<AppUserModel> userManager, IWebHostEnvironment webHostEnvironment, IHubContext<TestHub> hubContext)
        {
            _hubContext = hubContext;
            datacontext = context;
            _logger = logger;
            _signInManager = signInManager;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
        }
        [HttpGet]
        [Area("Student")]
        [Authorize(Roles = "Student")]
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
                    TempData["error"] = "Course Null";
                    TempData.Keep();
                    return RedirectToAction("TestList", "Participation", new { Test.CourseID });
                }
            }
            ViewBag.Course = Test.Course;
            ViewBag.TestID = TestID;
            ViewBag.CourseID = Test.CourseID;

            var list = datacontext.Question
                .ToList()
                .OrderBy(q => Guid.NewGuid())
                .Where(q => q.TestID == TestID);

            var timeLeft = Test.TestTime.GetValueOrDefault(TimeSpan.Zero);
            // var timeLeftString = timeLeft.GetValueOrDefault(TimeSpan.Zero).TotalHours.ToString();
            var formatedTime = $"{(int)timeLeft.TotalHours:D2}:{timeLeft.Minutes:D2}:{timeLeft.Seconds:D2}";
            var model = new DoTestViewModel
            {
                Questions = list,
                TimeLeft = formatedTime,
            };
            return View(model);
        }

        //https://stackoverflow.com/questions/13621934/validateantiforgerytoken-purpose-explanation-and-example
        [HttpPost]
        [Area("Student")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> DoTest(Dictionary<int, string> answers, CourseModel course, int courseID, int testID)
        {
            // Retrieve the User from the claims
            var studentID = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(studentID);
            course = datacontext.Courses.FirstOrDefault(c => c.CourseID == courseID);
            var test = datacontext.Test.FirstOrDefault(t => t.TestID == testID);

            var attempts = datacontext.Score.FirstOrDefault(s => s.TestID == testID
            && s.StudentID == studentID);
            if (attempts != null && test.NumberOfMaxAttempt == attempts.NumberOfAttempt)
            {
                TempData["error"] = "You've reach your maximum attempt";
                TempData.Keep();
                return RedirectToAction("TestList", "Participation", new { test.CourseID });
            }

            Dictionary<int, string[]> correctAnswers = new Dictionary<int, string[]>();

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
                    string theQuestion = question.Question;
                    string correctAnswerString = ConvertCorrectAnswer(question.CorrectAnswer, question);
                    string choiceString = ConvertCorrectAnswer(selectedAnswer, question);

                    //store both correct answer and question to this
                    string[] arr = new string[] { theQuestion, correctAnswer , correctAnswerString, choiceString};

                    correctAnswers.Add(questionId, arr);
                    // Check if the selected answer is correct, ignorecase
                    if (selectedAnswer.Equals(correctAnswer, StringComparison.OrdinalIgnoreCase))
                    {
                        correctAnswersCount++; // Increment count for correct answers
                    }
                }
            }
            // Calculate the score as a float out of 10.0 and round to 2 decimal places
            double score = Math.Round((double)correctAnswersCount / totalQuestions * 10.0, 2);
            //var previousResult = datacontext.Score.FromSqlRaw("SELECT * FROM Score WHERE TestID = {0} AND StudentID = {1}", testID, userID).FirstOrDefault();
            var previousResult = datacontext.Score.FirstOrDefault(s => s.TestID == testID && s.StudentID == studentID);
            if (previousResult != null)
            {
                previousResult.Student = user;
                previousResult.Score = score;
                previousResult.TestID = testID;
                previousResult.StudentID = studentID;
                previousResult.Test = test;
                previousResult.NumberOfAttempt++;

                datacontext.SaveChanges();
                //TempData["correctAnswers"] = correctAnswers;
                //TempData["answers"] = answers;

                //serialize the dictionary into a JSON string before storing it in TempData
                TempData["correctAnswers"] = JsonConvert.SerializeObject(correctAnswers);
                TempData["answers"] = JsonConvert.SerializeObject(answers);
                return RedirectToAction("TestResult", new { previousResult.ScoreID, studentID });
            }
            ScoreModel result = new ScoreModel
            {
                Student = user,
                Score = score,
                TestID = testID,
                StudentID = studentID,
                Test = test,
                NumberOfAttempt = 1,
                DoTestAt = DateTime.Now
            };

            datacontext.Score.Add(result);
            datacontext.SaveChanges();
            //TempData["correctAnswers"] = correctAnswers;
            //TempData["answers"] = answers;
            //serialize the dictionary into a JSON string before storing it in TempData
            TempData["correctAnswers"] = JsonConvert.SerializeObject(correctAnswers);
            TempData["answers"] = JsonConvert.SerializeObject(answers);
            return RedirectToAction("TestResult", new { result.ScoreID, studentID });
        }
        [HttpGet]
        [Area("Student")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> TestResult(int ScoreID, string studentID)
        {
            var user = await _userManager.FindByIdAsync(studentID);
            var result = datacontext.Score.FirstOrDefault(s => s.ScoreID == ScoreID);
            var test = datacontext.Test.FirstOrDefault(t => t.TestID == result.TestID);
            var course = datacontext.Courses.Find(test.CourseID);

            var correctAnswers = TempData["correctAnswers"] != null
                ? JsonConvert.DeserializeObject<Dictionary<int, string[]>>(TempData["correctAnswers"] as string)
                : new Dictionary<int, string[]>();

            var answers = TempData["answers"] != null
                ? JsonConvert.DeserializeObject<Dictionary<int, string>>(TempData["answers"] as string)
                : new Dictionary<int, string>();

            var model = new TestResultViewModel
            {
                CourseName = course.Title,
                CourseID = course.CourseID,
                TestID = result.TestID,
                Score = result.Score,
                TotalQuestions = result.Test.NumberOfQuestion,
                NumberOfAttemptLeft = test.NumberOfMaxAttempt - result.NumberOfAttempt,
                DoneAt = result.DoTestAt,
                Answers = answers,
                CorrectAnswers = correctAnswers
            };

            TempData.Keep(); // Keep TempData for further usage
            return View(model);
        }

        public async Task<IActionResult> StartTest(int testId)
        {
            var test = datacontext.Test.FirstOrDefault(t => t.TestID == testId);
            _logger.LogInformation("StartTest method called for testId: {testId}", testId);

            try
            {
                await _hubContext.Clients.All.SendAsync("TestStarted", testId);
                _logger.LogInformation("TestStarted event successfully sent for testId: {testId}", testId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send TestStarted event for testId: {testId}", testId);
            }

            return RedirectToAction("TestList", "Participation", new { CourseID = test.CourseID });
        }
        private String ConvertCorrectAnswer(String answer, QuestionModel question)
        {
            string result;
            if (answer.Equals("A"))
            {
                result = question.AnswerA;
            }
            else if (answer.Equals("B"))
            {
                result = question.AnswerB;
            }
            else if (answer.Equals("C"))
            {
                result = question.AnswerC;
            }
            else
            {
                result = question.AnswerD;
            }
            return result;
        }
    }
}