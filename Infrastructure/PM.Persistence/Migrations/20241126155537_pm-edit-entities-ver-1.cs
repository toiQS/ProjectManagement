using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PM.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class pmeditentitiesver1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Position Work Of Member_Position In Project_Position In Project Id",
                table: "Position Work Of Member");

            migrationBuilder.DropIndex(
                name: "IX_Position Work Of Member_Position In Project Id",
                table: "Position Work Of Member");

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "101111/23/2024 10:09:57 PM10");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Create At",
                table: "Project",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Position In Project Id",
                table: "Position Work Of Member",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "PositionInProjectId",
                table: "Position Work Of Member",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "First Name", "Full Name", "Last Name", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "Path Image", "Phone", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "101111/26/2024 10:55:36 PM10", 0, "68ea4340-d963-4f4f-92f3-e99dd12fc35c", "nguyena@gmail.com", false, "nguyen", "nguyen a", "a", false, null, null, null, null, "", "0123456789", null, false, "9e3191c7-60f1-4ede-b6bc-2d751590c0f4", false, null });

            migrationBuilder.CreateIndex(
                name: "IX_Position Work Of Member_PositionInProjectId",
                table: "Position Work Of Member",
                column: "PositionInProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_Position Work Of Member_Position In Project_PositionInProjectId",
                table: "Position Work Of Member",
                column: "PositionInProjectId",
                principalTable: "Position In Project",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Position Work Of Member_Position In Project_PositionInProjectId",
                table: "Position Work Of Member");

            migrationBuilder.DropIndex(
                name: "IX_Position Work Of Member_PositionInProjectId",
                table: "Position Work Of Member");

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "101111/26/2024 10:55:36 PM10");

            migrationBuilder.DropColumn(
                name: "PositionInProjectId",
                table: "Position Work Of Member");

            migrationBuilder.AlterColumn<string>(
                name: "Create At",
                table: "Project",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "Position In Project Id",
                table: "Position Work Of Member",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "First Name", "Full Name", "Last Name", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "Path Image", "Phone", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "101111/23/2024 10:09:57 PM10", 0, "68e6ee22-779d-43ed-9143-3d2740eab8e1", "nguyena@gmail.com", false, "nguyen", "nguyen a", "a", false, null, null, null, null, "", "0123456789", null, false, "89a1abf1-c7b6-4268-bf8e-92f25f7f9d53", false, null });

            migrationBuilder.CreateIndex(
                name: "IX_Position Work Of Member_Position In Project Id",
                table: "Position Work Of Member",
                column: "Position In Project Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Position Work Of Member_Position In Project_Position In Project Id",
                table: "Position Work Of Member",
                column: "Position In Project Id",
                principalTable: "Position In Project",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
