using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineLearning.Migrations
{
    /// <inheritdoc />
    public partial class Valoi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RequestTransfer_AspNetUsers_UserID",
                table: "RequestTransfer");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RequestTransfer",
                table: "RequestTransfer");

            migrationBuilder.RenameTable(
                name: "RequestTransfer",
                newName: "RequestTranfer");

            migrationBuilder.RenameIndex(
                name: "IX_RequestTransfer_UserID",
                table: "RequestTranfer",
                newName: "IX_RequestTranfer_UserID");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateAt",
                table: "RequestTranfer",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddPrimaryKey(
                name: "PK_RequestTranfer",
                table: "RequestTranfer",
                column: "TranferID");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1",
                column: "ConcurrencyStamp",
                value: "c29ef634-856f-4572-886a-fb742c32a18e");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2",
                column: "ConcurrencyStamp",
                value: "7b0d32b7-1d6d-435d-bd61-2298f203d4bb");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3",
                column: "ConcurrencyStamp",
                value: "7bbed806-ede1-4cec-8bb6-33f4d8e91bc6");

            migrationBuilder.AddForeignKey(
                name: "FK_RequestTranfer_AspNetUsers_UserID",
                table: "RequestTranfer",
                column: "UserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RequestTranfer_AspNetUsers_UserID",
                table: "RequestTranfer");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RequestTranfer",
                table: "RequestTranfer");

            migrationBuilder.DropColumn(
                name: "CreateAt",
                table: "RequestTranfer");

            migrationBuilder.RenameTable(
                name: "RequestTranfer",
                newName: "RequestTransfer");

            migrationBuilder.RenameIndex(
                name: "IX_RequestTranfer_UserID",
                table: "RequestTransfer",
                newName: "IX_RequestTransfer_UserID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RequestTransfer",
                table: "RequestTransfer",
                column: "TranferID");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1",
                column: "ConcurrencyStamp",
                value: "57aabc6b-fb2e-4c5d-8a2f-bea248aca7ef");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2",
                column: "ConcurrencyStamp",
                value: "09b72def-289e-4643-b777-0e1806f2e7de");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3",
                column: "ConcurrencyStamp",
                value: "a935c40b-9309-45a0-8570-a89a0822c21c");

            migrationBuilder.AddForeignKey(
                name: "FK_RequestTransfer_AspNetUsers_UserID",
                table: "RequestTransfer",
                column: "UserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
