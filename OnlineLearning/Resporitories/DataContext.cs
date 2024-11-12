using System.Collections.Generic;
using System.Reflection.Emit;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OnlineLearning.Models;
using YourNamespace.Models;


namespace OnlineLearningApp.Respositories
{
    public class DataContext : IdentityDbContext<AppUserModel>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }
        public DbSet<LivestreamRecordModel> LivestreamRecord { get; set; }
        public DbSet<ScoreModel> Score { get; set; }
        public DbSet<QuestionModel> Question { get; set; }
        public DbSet<TestModel> Test { get; set; }
        public DbSet<AppUserModel> Users { get; set; }
        public DbSet<InstructorModel> Instructors { get; set; }
        public DbSet<CourseModel> Courses { get; set; }
        public DbSet<CategoryModel> Category { get; set; }
        public DbSet<ScoreAssignmentModel> ScoreAssignment { get; set; }
        public DbSet<NotificationModel> Notification { get; set; }
        public DbSet<InstructorConfirmationModel> InstructorConfirmation { get; set; }
        public DbSet<SubmissionModel> Submission { get; set; }
        public DbSet<AssignmentModel> Assignment { get; set; }
        public DbSet<StudentCourseModel> StudentCourses { get; set; }
        public DbSet<ReviewModel> Review { get; set; }
        public DbSet<CourseMaterialModel> CourseMaterials { get; set; }
        public DbSet<PaymentModel> Payment { get; set; }
        public DbSet<LectureModel> Lecture { get; set; }
        public DbSet<RequestTranferModel> RequestTranfer { get; set; }
        public DbSet<MessageModel> Message { get; set; }
        public DbSet<MessageFileModel> MessageFile { get; set; }
        public DbSet<LectureFileModel> LectureFiles { get; set; }
        public DbSet<LectureCompletionModel> LectureCompletion { get; set; }
        public DbSet<ReportModel> Report { get; set; }
        public DbSet<BookMarkModel> BookMark { get; set; }
        public DbSet<VideoCallModel> VideoCallInfo { get; set; }
        public DbSet<CommentModel> Comment { get; set; }
        public DbSet<CommentFileModel> CommentFile { get; set; }
        public DbSet<CertificateModel> Certificate { get; set; }
        protected override void OnModelCreating(ModelBuilder builder) 
        {
            base.OnModelCreating(builder);
            SeedRoles(builder);
            SeedCategories(builder);

            // Thiết lập DeleteBehavior.Restrict cho các khóa ngoại trong bảng 
            builder.Entity<MessageModel>()
                .HasOne(m => m.Sender)
                .WithMany()
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<MessageModel>()
                .HasOne(m => m.Receiver)
                .WithMany()
                .HasForeignKey(m => m.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<VideoCallModel>()
                .HasOne(v => v.Send)
                .WithMany()
                .HasForeignKey(v => v.SendID)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<VideoCallModel>()
                .HasOne(v => v.Receive)
                .WithMany()
                .HasForeignKey(v => v.ReceiveID)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<BookMarkModel>()
                .HasOne(b => b.Student)
                .WithMany()
                .HasForeignKey(b => b.StudentID)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<CertificateModel>()
                .HasOne(c => c.Student)
                .WithMany()
                .HasForeignKey(c => c.StudentID)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<CourseModel>()
                .HasOne(c => c.Instructor)
                .WithMany()
                .HasForeignKey(c => c.InstructorID)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<LectureCompletionModel>()
                .HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserID)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<LivestreamRecordModel>()
                .HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserID)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<PaymentModel>()
                .HasOne(c => c.Student)
                .WithMany()
                .HasForeignKey(c => c.StudentID)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ReviewModel>()
                .HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserID)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<StudentCourseModel>()
                .HasOne(c => c.AppUser)
                .WithMany()
                .HasForeignKey(c => c.StudentID)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ScoreModel>()
               .HasOne(c => c.Student)
               .WithMany()
               .HasForeignKey(c => c.StudentID)
               .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ScoreAssignmentModel>()
                .HasOne(c => c.Student)
                .WithMany()
                .HasForeignKey(c => c.StudentID)
                .OnDelete(DeleteBehavior.Restrict);

            SeedUser(builder);
        }
        private void SeedRoles(ModelBuilder builder)
        {
            builder.Entity<IdentityRole>().HasData
            (
                new IdentityRole
                {
                    Id = "1",  // Role ID cho Admin
                    Name = "Admin",
                    ConcurrencyStamp = Guid.NewGuid().ToString(),
                    NormalizedName = "ADMIN"
                },
                new IdentityRole
                {
                    Id = "2",  // Role ID cho Student
                    Name = "Student",
                    ConcurrencyStamp = Guid.NewGuid().ToString(),
                    NormalizedName = "STUDENT"
                },
                new IdentityRole
                {
                    Id = "3",  // Role ID cho Instructor
                    Name = "Instructor",
                    ConcurrencyStamp = Guid.NewGuid().ToString(),
                    NormalizedName = "INSTRUCTOR"
                }
            );
        }
        private void SeedCategories(ModelBuilder builder)
        {
            builder.Entity<CategoryModel>().HasData
            (
                new CategoryModel
                {
                    CategoryID = 1,
                    FullName = "Programming",
                    Description = "Courses related to programming and software development."
                },
                new CategoryModel
                {
                    CategoryID = 2,
                    FullName = "Data Science",
                    Description = "Courses focused on data analysis and machine learning."
                },
                new CategoryModel
                {
                    CategoryID = 3,
                    FullName = "Web Development",
                    Description = "Courses for building websites and web applications."
                },
                new CategoryModel
                {
                    CategoryID = 4,
                    FullName = "Design",
                    Description = "Courses for graphic design and multimedia."
                }
            );
        }
        private void SeedUser(ModelBuilder builder)
        {
            var passwordHasher = new PasswordHasher<AppUserModel>();

            var admin = new AppUserModel
            {
                Id = "admin-user-id",
                UserName = "admin",
                NormalizedUserName = "ADMIN@EXAMPLE.COM",
                Email = "admin@example.com",
                NormalizedEmail = "ADMIN@EXAMPLE.COM",
                FirstName = "Admin",
                LastName = "User",
                EmailConfirmed = true,
                PasswordHash = passwordHasher.HashPassword(null, "Abc123"),
                PhoneNumber = "1234567890",
                PhoneNumberConfirmed = true,
                Address = "123 Admin Street",
                Dob = new DateOnly(2000, 1, 1),
                Gender = true,
                WalletUser = 200000.000,
                ProfileImagePath = "/images/default.jpg"
            };

            var student = new AppUserModel
            {
                Id = "student-user-id",
                UserName = "student",
                NormalizedUserName = "STUDENT@EXAMPLE.COM",
                Email = "student",
                NormalizedEmail = "STUDENT@EXAMPLE.COM",
                FirstName = "student@example.com",
                LastName = "User",
                EmailConfirmed = true,
                PasswordHash = passwordHasher.HashPassword(null, "Abc123!"),
                PhoneNumber = "9876543210",
                PhoneNumberConfirmed = true,
                Address = "456 Student Avenue",
                Dob = new DateOnly(2000, 1, 1),
                Gender = true,
                WalletUser = 200000.000,
                ProfileImagePath = "/images/default.jpg"
            };

            var instructor = new AppUserModel
            {
                Id = "instructor-user-id",
                UserName = "instructor",
                NormalizedUserName = "INSTRUCTOR@EXAMPLE.COM",
                Email = "instructor@example.com",
                NormalizedEmail = "INSTRUCTOR@EXAMPLE.COM",
                FirstName = "Instructor",
                LastName = "User",
                EmailConfirmed = true,
                PasswordHash = passwordHasher.HashPassword(null, "Abc123!"),
                PhoneNumber = "5551234567",
                PhoneNumberConfirmed = true,
                Address = "789 Instructor Road",
                Dob = new DateOnly(2000, 1, 1),
                Gender = true,
                WalletUser = 200000.000,
                ProfileImagePath = "/images/default.jpg"
            };

            builder.Entity<AppUserModel>().HasData(admin, student, instructor);

            builder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string> { UserId = "admin-user-id", RoleId = "1" }, // Admin
                new IdentityUserRole<string> { UserId = "student-user-id", RoleId = "2" }, // Student
                new IdentityUserRole<string> { UserId = "instructor-user-id", RoleId = "3" } // Instructor
            );
        }
    }
}
