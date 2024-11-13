using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace OnlineLearning.Migrations
{
    /// <inheritdoc />
    public partial class seedUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1",
                column: "ConcurrencyStamp",
                value: "a7edc889-b0fa-49d4-8165-a906a086e64c");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2",
                column: "ConcurrencyStamp",
                value: "5ec7cf85-021d-4d07-9089-3a9eaa64b391");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3",
                column: "ConcurrencyStamp",
                value: "682378f6-bbe7-4693-a83a-88245adfcd2b");

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "Address", "ConcurrencyStamp", "Dob", "Email", "EmailConfirmed", "FirstName", "Gender", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "ProfileImagePath", "SecurityStamp", "TwoFactorEnabled", "UserName", "WalletUser" },
                values: new object[,]
                {
                    { "admin-user-id", 0, "123 Admin Street", "78bf85cb-1e54-4c06-aca4-97f22a01d820", new DateOnly(2000, 1, 1), "admin@example.com", true, "Admin", true, "User", false, null, "ADMIN@EXAMPLE.COM", "ADMIN", "AQAAAAIAAYagAAAAECFf0PJ50+dDvFbPdt69bcDhosEHF5yM+RBhr7X0fFnbd0P24MRNhEOdmfmjRW3ngw==", "1234567890", true, "/images/default.jpg", "082bbe4c-4934-4918-99a5-03a7b00b17d7", false, "admin", 200000.0 },
                    { "instructor-user-id", 0, "789 Instructor Road", "36514a8f-7911-4f8e-a8d3-0c9fc24b8b11", new DateOnly(2000, 1, 1), "instructor@example.com", true, "Instructor", true, "User", false, null, "INSTRUCTOR@EXAMPLE.COM", "INSTRUCTOR", "AQAAAAIAAYagAAAAEPOOQ7SSIjs2W3Nu9rQgZfQ3jANMJopZ7caD2Q60ARWlQFdHYCBJuxr4aR2NKpCXpA==", "5551234567", true, "/images/default.jpg", "d9d9b958-9079-4345-b9e9-176e817fb629", false, "instructor", 200000.0 },
                    { "student-user-id", 0, "456 Student Avenue", "056cf342-8202-46ac-b11a-0104b93780b8", new DateOnly(2000, 1, 1), "student@example.com", true, "Student", true, "User", false, null, "STUDENT@EXAMPLE.COM", "STUDENT", "AQAAAAIAAYagAAAAEOjeM+b0k9TiswHhamkhgD56JzdkHwFeWN0S+xcaabc0y+rriCE6+0i11Z0VHv++Rg==", "9876543210", true, "/images/default.jpg", "28f66637-ec77-46ab-8fa8-c95b45dca6e6", false, "student", 200000.0 }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { "1", "admin-user-id" },
                    { "3", "instructor-user-id" },
                    { "2", "student-user-id" }
                });

            migrationBuilder.InsertData(
                table: "Instructors",
                columns: new[] { "InstructorID", "Description" },
                values: new object[] { "instructor-user-id", "Experienced Java and C# instructor specializing in advanced programming techniques." });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "1", "admin-user-id" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "3", "instructor-user-id" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "2", "student-user-id" });

            migrationBuilder.DeleteData(
                table: "Instructors",
                keyColumn: "InstructorID",
                keyValue: "instructor-user-id");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin-user-id");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "instructor-user-id");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "student-user-id");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1",
                column: "ConcurrencyStamp",
                value: "e38ff06e-f779-47ee-b510-c26c5c541b5e");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2",
                column: "ConcurrencyStamp",
                value: "cac4ef3a-4aee-4a79-8c4b-98586ce31efa");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3",
                column: "ConcurrencyStamp",
                value: "32e85dc5-dc1d-4ed0-8be8-811e22c7b6c2");
        }
    }
}
