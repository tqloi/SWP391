using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineLearning.Migrations
{
    /// <inheritdoc />
    public partial class name : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Courses_CategoryModel_CategoryID",
                table: "Courses");

            migrationBuilder.DropForeignKey(
                name: "FK_Courses_instructors_InstructorID",
                table: "Courses");

            migrationBuilder.DropForeignKey(
                name: "FK_instructors_AspNetUsers_InstructorID",
                table: "instructors");

            migrationBuilder.DropPrimaryKey(
                name: "PK_instructors",
                table: "instructors");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CategoryModel",
                table: "CategoryModel");

            migrationBuilder.RenameTable(
                name: "instructors",
                newName: "Instructors");

            migrationBuilder.RenameTable(
                name: "CategoryModel",
                newName: "Category");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Instructors",
                table: "Instructors",
                column: "InstructorID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Category",
                table: "Category",
                column: "CategoryID");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1",
                column: "ConcurrencyStamp",
                value: "1fb039f0-acd4-431a-9013-c622a15b8fa2");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2",
                column: "ConcurrencyStamp",
                value: "70c0bed7-d725-4c05-8362-4d29212b5283");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3",
                column: "ConcurrencyStamp",
                value: "fe42b7d6-a40f-4253-b732-002925bad73a");

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_Category_CategoryID",
                table: "Courses",
                column: "CategoryID",
                principalTable: "Category",
                principalColumn: "CategoryID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_Instructors_InstructorID",
                table: "Courses",
                column: "InstructorID",
                principalTable: "Instructors",
                principalColumn: "InstructorID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Instructors_AspNetUsers_InstructorID",
                table: "Instructors",
                column: "InstructorID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Courses_Category_CategoryID",
                table: "Courses");

            migrationBuilder.DropForeignKey(
                name: "FK_Courses_Instructors_InstructorID",
                table: "Courses");

            migrationBuilder.DropForeignKey(
                name: "FK_Instructors_AspNetUsers_InstructorID",
                table: "Instructors");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Instructors",
                table: "Instructors");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Category",
                table: "Category");

            migrationBuilder.RenameTable(
                name: "Instructors",
                newName: "instructors");

            migrationBuilder.RenameTable(
                name: "Category",
                newName: "CategoryModel");

            migrationBuilder.AddPrimaryKey(
                name: "PK_instructors",
                table: "instructors",
                column: "InstructorID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CategoryModel",
                table: "CategoryModel",
                column: "CategoryID");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1",
                column: "ConcurrencyStamp",
                value: "34408e49-7dec-4695-ac1e-6e489478ff6d");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2",
                column: "ConcurrencyStamp",
                value: "f78dc664-cbf6-406f-8853-acc33fda111e");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3",
                column: "ConcurrencyStamp",
                value: "9d03236d-3289-4d12-826b-2205ba394cfb");

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_CategoryModel_CategoryID",
                table: "Courses",
                column: "CategoryID",
                principalTable: "CategoryModel",
                principalColumn: "CategoryID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_instructors_InstructorID",
                table: "Courses",
                column: "InstructorID",
                principalTable: "instructors",
                principalColumn: "InstructorID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_instructors_AspNetUsers_InstructorID",
                table: "instructors",
                column: "InstructorID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
