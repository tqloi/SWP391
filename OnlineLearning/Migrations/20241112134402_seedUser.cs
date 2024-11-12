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
                value: "639e74e0-1827-4a20-bba2-c68701135f98");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2",
                column: "ConcurrencyStamp",
                value: "dc90d5ad-2c68-41b4-a654-3708af39be4e");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3",
                column: "ConcurrencyStamp",
                value: "dfd6bb09-f448-448b-a7bd-621b13a80261");

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "Address", "ConcurrencyStamp", "Dob", "Email", "EmailConfirmed", "FirstName", "Gender", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "ProfileImagePath", "SecurityStamp", "TwoFactorEnabled", "UserName", "WalletUser" },
                values: new object[,]
                {
                    { "admin-user-id", 0, "123 Admin Street", "1937a323-51d3-49ad-bed0-9e3645f88fb2", new DateOnly(2000, 1, 1), "admin@example.com", true, "Admin", true, "User", false, null, "ADMIN@EXAMPLE.COM", "ADMIN@EXAMPLE.COM", "AQAAAAIAAYagAAAAEB7gTmQQ72geaR5cPp1Njk2KfZuitkHy2Cfl8jwYXR04YeRT1zz5wuFIcn2FmulRFA==", "1234567890", true, "/images/default.jpg", "380e61f7-8df8-43a5-a931-c5c6345df93d", false, "admin", 200000.0 },
                    { "instructor-user-id", 0, "789 Instructor Road", "d1f2819d-8655-4680-91b4-bf23e0f5b934", new DateOnly(2000, 1, 1), "instructor@example.com", true, "Instructor", true, "User", false, null, "INSTRUCTOR@EXAMPLE.COM", "INSTRUCTOR@EXAMPLE.COM", "AQAAAAIAAYagAAAAEKXNLE7xJ89cv7JDPq26SHE9d7u2ZvwjTXlICVEq2K7DA4XwPEIE1Wp1sJzCDNckoA==", "5551234567", true, "/images/default.jpg", "e854987b-f173-4231-af56-5c478b0e1c2f", false, "instructor", 200000.0 },
                    { "student-user-id", 0, "456 Student Avenue", "242914a4-b0fb-48c6-a549-2e0d471197ee", new DateOnly(2000, 1, 1), "student", true, "Student", true, "User", false, null, "STUDENT@EXAMPLE.COM", "STUDENT@EXAMPLE.COM", "AQAAAAIAAYagAAAAEAQVZwv6Fde8DJw9beWmYj2ANgOAGVBfFzrtR+R4k5s7PeA68BRdA4SBp/qIZNfpyw==", "9876543210", true, "/images/default.jpg", "eb9fa16b-27a5-408d-ab9f-3ed6547d5126", false, "student@example.com", 200000.0 }
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
                value: "d3344138-1193-4eeb-8550-e3ccf499a97d");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2",
                column: "ConcurrencyStamp",
                value: "034d8d50-5884-441f-9970-71e126ad930a");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3",
                column: "ConcurrencyStamp",
                value: "95b3a7d1-c13c-4536-bcd2-b7a8a594f1b4");
        }
    }
}
