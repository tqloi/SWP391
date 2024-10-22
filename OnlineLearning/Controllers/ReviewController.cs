using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineLearning.Models;
using OnlineLearning.Models.ViewModel;
using OnlineLearningApp.Respositories;
using System.Security.Claims;

namespace OnlineLearning.Controllers
{
    public class ReviewController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DataContext datacontext;
        private UserManager<AppUserModel> _userManager;
        private SignInManager<AppUserModel> _signInManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ReviewController(ILogger<HomeController> logger, DataContext context, SignInManager<AppUserModel> signInManager, UserManager<AppUserModel> userManager, IWebHostEnvironment webHostEnvironment)
        {
            datacontext = context;
            _logger = logger;
            _signInManager = signInManager;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;

        }
        
        //public async Task<IActionResult> LoadReview(int CourseID, int page = 1)
        //{
        //    int pageSize = 5;

        //    var review = await datacontext.Review
        //             .Include(r => r.User)
        //             .Include(r => r.Course)
        //             .Where(c => c.CourseID == CourseID)
        //             .OrderByDescending(r => r.ReviewDate)
        //             .ToListAsync();

        //    var pagedReviews = review.Skip((page - 1) * pageSize)
        //                      .Take(pageSize)
        //                      .ToList();
        //    var totalReview = review.Count();
        //    var totalPages = (int)Math.Ceiling(totalReview / (double)pageSize);
        //    var model = new CourseDetailViewModel
        //    {

        //        Reviews = pagedReviews,
        //        TotalPage = totalPages,
        //        CurrentPage = page
        //    };
        //    return PartialView("_ReviewPartial", model);
        //}

        [HttpPost]
        public async Task<IActionResult> SummitReview(ReviewModel model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var existReview = datacontext.Review
                                 .Where(r => r.CourseID == model.CourseID && r.UserID == userId)
                                 .FirstOrDefault(); 
            if (existReview != null)            
            {
                var review = new ReviewModel
                {
                    CourseID = model.CourseID,
                    UserID = userId,
                    Rating = model.Rating,
                    Comment = model.Comment,
                    ReviewDate = DateTime.Now,
                };
                datacontext.Review.Add(review);
                datacontext.SaveChanges();

            }
            var FCourse = await datacontext.Courses
                            .FirstOrDefaultAsync(c => c.CourseID == model.CourseID);
            if (FCourse != null)
            {
                var reviews = await datacontext.Review
                                     .Where(r => r.CourseID == model.CourseID)
                                     .ToListAsync();

                var Nor = reviews.Count;
                var Sum = reviews.Sum(r => r.Rating);

                if (Nor > 0)
                {
                    FCourse.Rating = (double)Sum / Nor; // Chia cho số lượng review
                    FCourse.NumberOfRate = Nor;
                }

                await datacontext.SaveChangesAsync();
            }
            return RedirectToAction("CourseDetail", "Course", new {CourseID = model.CourseID});
        }
        
        [HttpPost]
        public async Task<IActionResult> UpdateReview(ReviewModel model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var existReview = datacontext.Review
                                 .Where(r => r.CourseID == model.CourseID && r.UserID == userId)
                                 .FirstOrDefault();
            if (existReview == null)
            {
                return NotFound("Review not found.");
            }
            existReview.Comment = model.Comment;
            existReview.Rating = model.Rating;
            await datacontext.SaveChangesAsync();
            // Tính tống rating mới cho Course
            var FCourse = await datacontext.Courses
                            .FirstOrDefaultAsync(c => c.CourseID == model.CourseID);
            if (FCourse != null)
            {
                var reviews = await datacontext.Review
                                     .Where(r => r.CourseID == model.CourseID)
                                     .ToListAsync();

                var Nor = reviews.Count;
                var Sum = reviews.Sum(r => r.Rating);

                if (Nor > 0)
                {
                    FCourse.Rating = (double)Sum / Nor; // Chia cho số lượng review
                    FCourse.NumberOfRate = Nor;
                }

                await datacontext.SaveChangesAsync();
            }
            return RedirectToAction("CourseDetail", "Course", new {CourseID = model.CourseID});
        }

        [HttpPost]
        public async Task<IActionResult> DeleteReview(ReviewModel model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var existReview = datacontext.Review
                                 .Where(r => r.CourseID == model.CourseID && r.UserID == userId)
                                 .FirstOrDefault();
            if (existReview != null) {
                datacontext.Review.Remove(existReview); 
                await datacontext.SaveChangesAsync();
            }
            var FCourse = await datacontext.Courses
                            .FirstOrDefaultAsync(c => c.CourseID == model.CourseID);
            if (FCourse != null)
            {
                var reviews = await datacontext.Review
                                     .Where(r => r.CourseID == model.CourseID)
                                     .ToListAsync();

                var Nor = reviews.Count;
                var Sum = reviews.Sum(r => r.Rating);

                if (Nor > 0)
                {
                    FCourse.Rating = (double)Sum / Nor; // Chia cho số lượng review
                    FCourse.NumberOfRate = Nor;
                }

                await datacontext.SaveChangesAsync();
            }
            return RedirectToAction("CourseDetail", "Course", new { CourseID = model.CourseID });
        }
        public IActionResult Index()
        {
            return View();
        }

    }
}
