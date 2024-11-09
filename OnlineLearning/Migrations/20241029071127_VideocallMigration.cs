using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineLearning.Migrations
{
    /// <inheritdoc />
    public partial class VideocallMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Score_AspNetUsers_UserId",
                table: "Score");

            migrationBuilder.DropTable(
                name: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_Score_UserId",
                table: "Score");

            migrationBuilder.DropColumn(
                name: "ScoreValue",
                table: "Score");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Score");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Assignment");

            migrationBuilder.AddColumn<string>(
                name: "AlowRedo",
                table: "Test",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "NumberOfMaxAttempt",
                table: "Test",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "PassingScore",
                table: "Test",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "Submission",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "StudentID",
                table: "Score",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "NumberOfAttempt",
                table: "Score",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "Score",
                table: "Score",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "fileExtension",
                table: "LectureFiles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "InstructorConfirmation",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FIleName",
                table: "CourseMaterials",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "fileExtension",
                table: "CourseMaterials",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "Assignment",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "BookMark",
                columns: table => new
                {
                    BookmarkID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentID = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    CourseID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookMark", x => x.BookmarkID);
                    table.ForeignKey(
                        name: "FK_BookMark_AspNetUsers_StudentID",
                        column: x => x.StudentID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookMark_Courses_CourseID",
                        column: x => x.CourseID,
                        principalTable: "Courses",
                        principalColumn: "CourseID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LectureCompletion",
                columns: table => new
                {
                    CompletionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    LectureID = table.Column<int>(type: "int", nullable: false),
                    CompletionDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LectureCompletion", x => x.CompletionID);
                    table.ForeignKey(
                        name: "FK_LectureCompletion_AspNetUsers_UserID",
                        column: x => x.UserID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LectureCompletion_Lecture_LectureID",
                        column: x => x.LectureID,
                        principalTable: "Lecture",
                        principalColumn: "LectureID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Report",
                columns: table => new
                {
                    ReportID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FeedbackDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Report", x => x.ReportID);
                    table.ForeignKey(
                        name: "FK_Report_AspNetUsers_UserID",
                        column: x => x.UserID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Review",
                columns: table => new
                {
                    ReviewID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CourseID = table.Column<int>(type: "int", nullable: false),
                    UserID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Rating = table.Column<double>(type: "float", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ReviewDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Review", x => x.ReviewID);
                    table.ForeignKey(
                        name: "FK_Review_AspNetUsers_UserID",
                        column: x => x.UserID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Review_Courses_CourseID",
                        column: x => x.CourseID,
                        principalTable: "Courses",
                        principalColumn: "CourseID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VideoCallInfo",
                columns: table => new
                {
                    VideoCallId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SendID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ReceiveID = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VideoCallInfo", x => x.VideoCallId);
                    table.ForeignKey(
                        name: "FK_VideoCallInfo_AspNetUsers_ReceiveID",
                        column: x => x.ReceiveID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VideoCallInfo_AspNetUsers_SendID",
                        column: x => x.SendID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1",
                column: "ConcurrencyStamp",
                value: "188923da-843b-4aa6-ad64-57708f4269f3");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2",
                column: "ConcurrencyStamp",
                value: "1418a316-6bdd-4075-9c94-65941ec50f28");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3",
                column: "ConcurrencyStamp",
                value: "ea5a1705-ade7-4d79-84d0-aeb7bb14471d");

            migrationBuilder.CreateIndex(
                name: "IX_Score_StudentID",
                table: "Score",
                column: "StudentID");

            migrationBuilder.CreateIndex(
                name: "IX_BookMark_CourseID",
                table: "BookMark",
                column: "CourseID");

            migrationBuilder.CreateIndex(
                name: "IX_BookMark_StudentID",
                table: "BookMark",
                column: "StudentID");

            migrationBuilder.CreateIndex(
                name: "IX_LectureCompletion_LectureID",
                table: "LectureCompletion",
                column: "LectureID");

            migrationBuilder.CreateIndex(
                name: "IX_LectureCompletion_UserID",
                table: "LectureCompletion",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_Report_UserID",
                table: "Report",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_Review_CourseID",
                table: "Review",
                column: "CourseID");

            migrationBuilder.CreateIndex(
                name: "IX_Review_UserID",
                table: "Review",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_VideoCallInfo_ReceiveID",
                table: "VideoCallInfo",
                column: "ReceiveID");

            migrationBuilder.CreateIndex(
                name: "IX_VideoCallInfo_SendID",
                table: "VideoCallInfo",
                column: "SendID");

            migrationBuilder.AddForeignKey(
                name: "FK_Score_AspNetUsers_StudentID",
                table: "Score",
                column: "StudentID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Score_AspNetUsers_StudentID",
                table: "Score");

            migrationBuilder.DropTable(
                name: "BookMark");

            migrationBuilder.DropTable(
                name: "LectureCompletion");

            migrationBuilder.DropTable(
                name: "Report");

            migrationBuilder.DropTable(
                name: "Review");

            migrationBuilder.DropTable(
                name: "VideoCallInfo");

            migrationBuilder.DropIndex(
                name: "IX_Score_StudentID",
                table: "Score");

            migrationBuilder.DropColumn(
                name: "AlowRedo",
                table: "Test");

            migrationBuilder.DropColumn(
                name: "NumberOfMaxAttempt",
                table: "Test");

            migrationBuilder.DropColumn(
                name: "PassingScore",
                table: "Test");

            migrationBuilder.DropColumn(
                name: "FileName",
                table: "Submission");

            migrationBuilder.DropColumn(
                name: "NumberOfAttempt",
                table: "Score");

            migrationBuilder.DropColumn(
                name: "Score",
                table: "Score");

            migrationBuilder.DropColumn(
                name: "fileExtension",
                table: "LectureFiles");

            migrationBuilder.DropColumn(
                name: "FileName",
                table: "InstructorConfirmation");

            migrationBuilder.DropColumn(
                name: "FIleName",
                table: "CourseMaterials");

            migrationBuilder.DropColumn(
                name: "fileExtension",
                table: "CourseMaterials");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "Assignment");

            migrationBuilder.AlterColumn<string>(
                name: "StudentID",
                table: "Score",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<float>(
                name: "ScoreValue",
                table: "Score",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Score",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Assignment",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    ReviewID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CourseID = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Comment = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Rating = table.Column<float>(type: "real", nullable: false),
                    ReviewDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StudentID = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.ReviewID);
                    table.ForeignKey(
                        name: "FK_Reviews_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Reviews_Courses_CourseID",
                        column: x => x.CourseID,
                        principalTable: "Courses",
                        principalColumn: "CourseID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1",
                column: "ConcurrencyStamp",
                value: "d83a197e-e819-4f67-88cb-674564598162");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2",
                column: "ConcurrencyStamp",
                value: "6f71d88e-0e1a-46d9-bfd9-1f090e37fbe7");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3",
                column: "ConcurrencyStamp",
                value: "4e283010-d20c-446f-9a17-de4eb6aa2b9c");

            migrationBuilder.CreateIndex(
                name: "IX_Score_UserId",
                table: "Score",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_CourseID",
                table: "Reviews",
                column: "CourseID");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_UserId",
                table: "Reviews",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Score_AspNetUsers_UserId",
                table: "Score",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
