using System.Collections.Generic;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OnlineLearning.Models;


namespace OnlineLearningApp.Respositories
{
    public class DataContext : IdentityDbContext<AppUserModel>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }
		public DbSet<AppUserModel> Users { get; set; }
        public DbSet<InstructorModel> Instructors { get; set; }
        public DbSet<CourseModel> Courses { get; set; }
        public DbSet<CategoryModel> Category{ get; set; }

    }
}
