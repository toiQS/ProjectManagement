using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PM.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class PMeditentities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "101111/19/2024 12:12:57 AM10");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "First Name", "Full Name", "Last Name", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "Path Image", "Phone", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "101111/23/2024 10:09:57 PM10", 0, "68e6ee22-779d-43ed-9143-3d2740eab8e1", "nguyena@gmail.com", false, "nguyen", "nguyen a", "a", false, null, null, null, null, "", "0123456789", null, false, "89a1abf1-c7b6-4268-bf8e-92f25f7f9d53", false, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "101111/23/2024 10:09:57 PM10");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "First Name", "Full Name", "Last Name", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "Path Image", "Phone", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "101111/19/2024 12:12:57 AM10", 0, "71b22880-ce60-43bd-83d5-d62416632535", "nguyena@gmail.com", false, "nguyen", "nguyen a", "a", false, null, null, null, null, "", "0123456789", null, false, null, false, null });
        }
    }
}
