using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using Newtonsoft.Json;
using OnlineLearning.Models;
using OnlineLearning.Models.ViewModel;
using OnlineLearningApp.Respositories;
using QRCoder;
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

        [Authorize(Roles = "Instructor")]
        public async Task<IActionResult> CreateTest(int CourseID)
        {
            ViewBag.CourseID = CourseID;
            var course = await datacontext.Courses.FindAsync(CourseID);

            if (course == null)
            {
                return NotFound();
            }

            ViewBag.Course = course;
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "Instructor")]
        public IActionResult EditTest(int TestID)
        {
            var Test = datacontext.Test.FirstOrDefault(t => t.TestID == TestID);
            var Course = datacontext.Courses.FirstOrDefault(c => c.CourseID == Test.CourseID);

            ViewBag.CourseID = Test.CourseID;
            ViewBag.Course = Course;
            if (Test == null)
            {
                return NotFound();
            }
            return View("EditTest", Test);
        }

        //[HttpGet]
        //public async Task<IActionResult> CreateTest(int CourseID)
        //{
        //    ViewBag.CourseID = CourseID;
        //    var course = await datacontext.Courses.FindAsync(CourseID);

        //    if (course == null)
        //    {
        //        return NotFound();
        //    }

        //    ViewBag.Course = course;
        //    return View();
        //}

        [HttpPost]
        [Authorize(Roles = "Instructor")]
        public IActionResult EditTest(TestModel model)
        {
            if (model == null)
            {
                return NotFound();
            }
            var submissions = datacontext.Score
                .Where(s => s.TestID == model.TestID)
                .ToList();
            foreach (var sub in submissions)
            {
                if (sub.NumberOfAttempt > model.NumberOfMaxAttempt)
                {
                    TempData["error"] = $"Cannot change Number Of Attempt Lower since " +
                        $"there's a student with {sub.NumberOfAttempt} submission";
                    TempData.Keep();
                    return RedirectToAction("TestList", "Participation", new { id = model.CourseID });
                }
            }

            ViewBag.CourseID = model.CourseID;
            ViewBag.Course = model.Course;
            datacontext.Update(model);
            datacontext.SaveChanges();
            TempData["success"] = "Edit Successfully";
            TempData.Keep();
            return RedirectToAction("TestList", "Participation", new { id = model.CourseID });
        }

        [HttpPost]
        [Authorize(Roles = "Instructor")]
        public async Task<IActionResult> CreateTest(TestModel model)
        {
            var Course = datacontext.Courses.Find(model.CourseID);
            ViewBag.Course = Course;

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
                //handle null from view
                if (model.PassingScore == null)
                {
                    model.PassingScore = 5.0;
                }
                if (model.AlowRedo == null)
                {
                    model.AlowRedo = "Yes";
                }
                if (model.NumberOfMaxAttempt == null)
                {
                    model.NumberOfMaxAttempt = 1;
                }

                Debug.WriteLine("ID retrieved valid");
                var newTest = new TestModel
                {
                    AlowRedo = model.AlowRedo,
                    NumberOfMaxAttempt = model.NumberOfMaxAttempt,
                    Title = model.Title,
                    Course = model.Course,
                    Description = model.Description,
                    StartTime = model.StartTime,
                    EndTime = model.EndTime,
                    Status = model.Status,
                    CourseID = model.CourseID,
                    NumberOfQuestion = 0,
                    PassingScore = model.PassingScore
                };
                // Add the test to the context and save changes
                datacontext.Test.Add(newTest);
                await datacontext.SaveChangesAsync();
                Debug.WriteLine("Test saved to database");
                ViewBag.CourseID = Course.CourseID;
                TempData["success"] = "Test created successfully!";
                //keep it alive for 2 request 
                TempData.Keep();
                return RedirectToAction("TestList", "Participation", new { CourseID = model.CourseID });
                //=======
                //                if (model.StartTime < model.EndTime)
                //                {
                //                    Debug.WriteLine("ID retrieved valid");
                //                    var newTest = new TestModel
                //                    {
                //                        Title = model.Title,
                //                        Course = model.Course,
                //                        Description = model.Description,
                //                        StartTime = model.StartTime,
                //                        EndTime = model.EndTime,
                //                        Status = model.Status,
                //                        CourseID = model.CourseID,
                //                        NumberOfQuestion = 0
                //                    };

                //                    datacontext.Test.Add(newTest);
                //                    await datacontext.SaveChangesAsync();
                //                    Debug.WriteLine("Test saved to database");


                //                    TempData["success"] = "Test created successfully!";
                //                    return RedirectToAction("TestList", "Participation", new { CourseID = model.CourseID });
                //                }
                //                else
                //                {
                //                    TempData["error"] = "End Time must after start time";
                //                    return View(model);
                //                }
                //>>>>>>> loii
            }

            catch (Exception)
            {
                TempData["error"] = "Test creation failed!";
                TempData.Keep();
                return RedirectToAction("CreateTestRedirector", new { courseID = model.CourseID });
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
                    TempData["error"] = "Course Null";
                    TempData.Keep();
                    return RedirectToAction("TestList", "Participation", new { id = Test.CourseID });
                }
            }
            ViewBag.Course = Test.Course;
            ViewBag.TestID = TestID;
            ViewBag.CourseID = Test.CourseID;

            var list = datacontext.Question
                .ToList()
                .OrderBy(q => Guid.NewGuid())
                .Where(q => q.TestID == TestID);
            return View(list);
        }

        //https://stackoverflow.com/questions/13621934/validateantiforgerytoken-purpose-explanation-and-example
        [HttpPost]
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
                return RedirectToAction("TestList", "Participation", new { id = test.CourseID });
            }

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
                previousResult.NumberOfAttempt++;

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
                Test = test,
                NumberOfAttempt = 1
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
                TotalQuestions = result.Test.NumberOfQuestion,
                CourseID = course.CourseID,
                NumberOfAttemptLeft = test.NumberOfMaxAttempt - result.NumberOfAttempt
            };
            TempData["success"] = "Test Completed";
            return View(model);
        }
        [HttpPost]
        [Authorize(Roles = "Instructor")]
        public async Task<IActionResult> DeleteTest(int TestID)
        {
            var test = await datacontext.Test.FirstOrDefaultAsync(t => t.TestID == TestID);
            if (test == null)
            {
                return NotFound();
            }

            var questions = datacontext.Question
                .Where(q => q.TestID == TestID)
                .ToList();

            if (questions.Any())
            {
                var scores = datacontext.Score
                    .Where(s => s.TestID == TestID)
                    .ToList();
                if (scores.Any())
                {
                    //currently not allow delete test if there's a submission from a student
                    TempData["error"] = "Test already been done by students";
                    TempData.Keep();
                    return RedirectToAction("TestList", "Participation", new { id = test.CourseID });
                }
                //delete image first
                foreach (var question in questions)
                {
                    if (!string.IsNullOrEmpty(question.ImagePath))
                    {
                        string ImageFullPath = Path.Combine(_webHostEnvironment.WebRootPath, "Images", "QuestionImages", question.ImagePath);
                        if (System.IO.File.Exists(ImageFullPath))
                        {
                            System.IO.File.Delete(ImageFullPath);
                        }
                    }
                }
                // Remove associated questions first
                datacontext.Question.RemoveRange(questions);
            }

            // Delete the test after removing associated questions
            datacontext.Test.Remove(test);
            datacontext.SaveChanges();

            // Redirect to the test list or any other appropriate action
            TempData["success"] = "Test deletion successfully";
            TempData.Keep();
            return RedirectToAction("TestList", "Participation", new { id = test.CourseID });
        }

        [HttpPost]
        [Authorize(Roles = "Instructor")]
        public IActionResult ClearAllQuestions(int TestID)
        {
            var test = datacontext.Test.FirstOrDefault(t => t.TestID == TestID);
            var questions = datacontext.Question
                .Where(q => q.TestID == TestID)
                .ToList();
            if (test == null)
            {
                return NotFound();
            }
            foreach (var question in questions)
            {
                if (!question.ImagePath.IsNullOrEmpty())
                {
                    string ImageFullPath = Path.Combine(_webHostEnvironment.WebRootPath, "Images", "QuestionImages", question.ImagePath);
                    if (System.IO.File.Exists(ImageFullPath))
                    {
                        System.IO.File.Delete(ImageFullPath);
                    }
                }
            }
            datacontext.Question.RemoveRange(questions);
            datacontext.SaveChanges();
            TempData["success"] = "Clear all questions successfully";
            TempData.Keep();
            return RedirectToAction("TestList", "Participation", new { id = test.TestID });
        }
        [HttpPost]
        [Authorize(Roles = "Instructor")]
        public IActionResult ClearAllSubmission(int TestID)
        {
            var test = datacontext.Test.FirstOrDefault(t => t.TestID == TestID);
            if (test == null)
            {
                return NotFound();
            }
            var submissions = datacontext.Score
                .Where(s => s.TestID == TestID)
                .ToList();
            datacontext.Score.RemoveRange(submissions);
            datacontext.SaveChanges();
            TempData["success"] = "Clear all submission successfully";
            TempData.Keep();
            return RedirectToAction("TestList", "Participation", new { id = TestID });
        }
    }
}
