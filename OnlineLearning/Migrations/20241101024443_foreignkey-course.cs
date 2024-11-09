using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineLearning.Migrations
{
    /// <inheritdoc />
    public partial class foreignkeycourse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CourseID",
                table: "LivestreamRecord",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1",
                column: "ConcurrencyStamp",
                value: "adb48b3f-4ca6-4a41-a931-e73550cc4ade");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2",
                column: "ConcurrencyStamp",
                value: "966ee4bc-b650-433d-a2dd-0211246ec24c");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3",
                column: "ConcurrencyStamp",
                value: "981364b2-b73c-44ee-bdeb-f09794986cb9");

            migrationBuilder.CreateIndex(
                name: "IX_LivestreamRecord_CourseID",
                table: "LivestreamRecord",
                column: "CourseID");

            migrationBuilder.AddForeignKey(
                name: "FK_LivestreamRecord_Courses_CourseID",
                table: "LivestreamRecord",
                column: "CourseID",
                principalTable: "Courses",
                principalColumn: "CourseID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LivestreamRecord_Courses_CourseID",
                table: "LivestreamRecord");

            migrationBuilder.DropIndex(
                name: "IX_LivestreamRecord_CourseID",
                table: "LivestreamRecord");

            migrationBuilder.DropColumn(
                name: "CourseID",
                table: "LivestreamRecord");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1",
                column: "ConcurrencyStamp",
                value: "efaf9e63-1421-4c25-bc05-b0d596746181");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2",
                column: "ConcurrencyStamp",
                value: "b4b7a275-e697-4cc2-a978-3a7a10ed9311");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3",
                column: "ConcurrencyStamp",
                value: "db10adec-1693-4025-952b-e3efa647a88f");
        }
    }
}
