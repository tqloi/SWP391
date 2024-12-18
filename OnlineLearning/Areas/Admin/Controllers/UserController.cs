
using System.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineLearning.Areas.Admin.Models.ViewModel;
using OnlineLearning.Models;
using OnlineLearning.Models.ViewModel;
using OnlineLearningApp.Respositories;

namespace OnlineLearning.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]

    [Route("Admin/[controller]/[action]")]
    public class UserController : Controller
    {
        private UserManager<AppUserModel> _userManager;
        private SignInManager<AppUserModel> _signInManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly DataContext _dataContext;
        public UserController(DataContext dataContext, UserManager<AppUserModel> userManager, SignInManager<AppUserModel> signInManager, IWebHostEnvironment webHostEnvironment)
        {
            _dataContext = dataContext;
            _userManager = userManager;
            _signInManager = signInManager;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> UserList()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var users = await _dataContext.Users.Where(u => !u.Id.Equals(userId)).ToListAsync();
            var list = new List<UserRolesViewModel>();
            foreach (var user in users)
            {
                var userrole = await _userManager.GetRolesAsync(user);
                list.Add(new UserRolesViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    ProfileImagePath = user.ProfileImagePath,
                    Email = user.Email,
                    Address = user.Address,
                    Roles = string.Join("", userrole)
                });
            }

            return View(list);
        }
        public async Task<IActionResult> InstructorList()
        {
            var users = await _dataContext.Users.ToListAsync();
            var list = new List<UserRolesViewModel>();
            foreach (var user in users)
            {
                var userrole = await _userManager.GetRolesAsync(user);
                if (userrole.Contains("Instructor"))
                {
                    list.Add(new UserRolesViewModel
                    {
                        UserId = user.Id,
                        UserName = user.UserName,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        ProfileImagePath = user.ProfileImagePath,
                        Email = user.Email,
                        Address = user.Address,
                        Roles = string.Join("", userrole)
                    });
                }
            }

            return View(list);
        }
        public async Task<IActionResult> StudentList()
        {
            var users = await _dataContext.Users.ToListAsync();
            var list = new List<UserRolesViewModel>();
            foreach (var user in users)
            {
                var userrole = await _userManager.GetRolesAsync(user);
                if (userrole.Contains("Student"))
                {
                    list.Add(new UserRolesViewModel
                    {
                        UserId = user.Id,
                        UserName = user.UserName,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        ProfileImagePath = user.ProfileImagePath,
                        Email = user.Email,
                        Address = user.Address,
                        Roles = string.Join("", userrole)
                    });
                }
            }

            return View(list);
        }
        public async Task<IActionResult> UserProfile(string Id)
        {
            var user = await _dataContext.Users.FirstOrDefaultAsync(u => u.Id.Equals(Id));
            if (user == null)
            {
                return NotFound();
            }
            AdminProfileViewModel model = new AdminProfileViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Dob = user.Dob,
                Address = user.Address,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Gender = user.Gender,
                ExistingProfileImagePath = user.ProfileImagePath
            };
            return View(model);
        }
        public async Task<IActionResult> InstructorConfirm()
        {
            var listInstructors = await _dataContext.InstructorConfirmation.Include(i => i.user).ToListAsync();
            return View(listInstructors);
        }
        public ActionResult ViewPdf(int Id)
        {
             var confirmation = _dataContext.InstructorConfirmation.Include(c => c.user).FirstOrDefault(c => c.ConfirmationID == Id);

            return View(confirmation);
        }
        [HttpPost]
        public async Task<IActionResult> ChangeRoleToInstructor(string Id)
        {
            var user = await _userManager.FindByIdAsync(Id);
            if (user == null)
            {
                TempData["error"] = "Action failed!";
                return NotFound();
            }
            if (await _userManager.IsInRoleAsync(user, "Student"))
            {
                await _userManager.RemoveFromRoleAsync(user, "Student");

                await _userManager.AddToRoleAsync(user, "Instructor");
                var instructor = new InstructorModel
                {
                    InstructorID = user.Id,
                    Description = "Toi la mot giang vien"
                };
                _dataContext.Add(instructor);
                await _dataContext.SaveChangesAsync();
                var confirmation = _dataContext.InstructorConfirmation.Include(c => c.user).FirstOrDefault(c => c.UserID == user.Id);
                if (confirmation != null)
                {
                    TempData["success"] = "Upgrade successful";
                    _dataContext.InstructorConfirmation.Remove(confirmation);
                    await _dataContext.SaveChangesAsync(); 
                }
            }

            return RedirectToAction("InstructorConfirm"); 
        }
        public async Task<IActionResult> Search(string search)
        {
            var list = new ListSearchViewModel();

            list.Courses = await _dataContext.Courses.Where(u => u.Title.Contains(search)).ToListAsync();
            list.Users = await _dataContext.Users.Where(i => i.FirstName.Contains(search) || i.LastName.Contains(search) || search.Equals(i.FirstName + " " + i.LastName)).ToListAsync();
            return View(list);
        }
        public async Task<IActionResult> ReportList()
        {
           var report = await _dataContext.Report.Where(r => r.Subject.StartsWith("LECTURE") || r.Subject.StartsWith("Course")).Include(r => r.User).OrderByDescending(r => r.FeedbackDate).ToListAsync();
           return View(report);
        }

        public  async Task<IActionResult> Top5Student()
        {
            List<Top5UserViewModel> top5 = new List<Top5UserViewModel>();
            var liststudent = _dataContext.StudentCourses.ToList();
            int size = liststudent.Count;
            if (size > 0)
            {
                for (int i = 0; i < size; i++)
                {
                    Top5UserViewModel user1 = new Top5UserViewModel();
                    int count = 1;
                    for (int j = i + 1; j < size; j++)
                    {
                        if (liststudent[i].StudentID.Equals(liststudent[j].StudentID))
                        {
                            count++;
                        }

                    }
                    user1.Count = count;
                    user1.StudentID = liststudent[i].StudentID;
                    
                    AppUserModel user = await _userManager.FindByIdAsync(user1.StudentID);
                    user1.FirstName = user.FirstName;
                    user1.LastName = user.LastName;
                    user1.ImagesPath = user.ProfileImagePath;
                    user1.Email = user.Email;
                    user1.Address = user.Address;
                    top5.Add(user1);
                    user1 = null;
                    count = 1;

                }
                top5 = top5.GroupBy(u => u.StudentID).Select(g => g.First()).OrderByDescending(x => x.Count).ToList();
            }
            
            return View(top5);
        }

    }
}
