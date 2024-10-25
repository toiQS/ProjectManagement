using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PM.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class PMUpgradedata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Application User Id",
                table: "Role Application User In Project",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Plan",
                keyColumn: "Id",
                keyValue: "PL1",
                columns: new[] { "Create At", "End At", "Start At" },
                values: new object[] { new DateTime(2024, 10, 25, 15, 43, 49, 607, DateTimeKind.Local).AddTicks(323), new DateTime(2024, 11, 25, 15, 43, 49, 607, DateTimeKind.Local).AddTicks(326), new DateTime(2024, 10, 26, 15, 43, 49, 607, DateTimeKind.Local).AddTicks(324) });

            migrationBuilder.UpdateData(
                table: "Plan",
                keyColumn: "Id",
                keyValue: "PL2",
                columns: new[] { "Create At", "End At", "Start At" },
                values: new object[] { new DateTime(2024, 10, 25, 15, 43, 49, 607, DateTimeKind.Local).AddTicks(334), new DateTime(2024, 12, 25, 15, 43, 49, 607, DateTimeKind.Local).AddTicks(335), new DateTime(2024, 10, 27, 15, 43, 49, 607, DateTimeKind.Local).AddTicks(334) });

            migrationBuilder.UpdateData(
                table: "Plan",
                keyColumn: "Id",
                keyValue: "PL3",
                columns: new[] { "Create At", "End At", "Start At" },
                values: new object[] { new DateTime(2024, 10, 25, 15, 43, 49, 607, DateTimeKind.Local).AddTicks(336), new DateTime(2025, 1, 25, 15, 43, 49, 607, DateTimeKind.Local).AddTicks(337), new DateTime(2024, 10, 28, 15, 43, 49, 607, DateTimeKind.Local).AddTicks(337) });

            migrationBuilder.UpdateData(
                table: "Plan",
                keyColumn: "Id",
                keyValue: "PL4",
                columns: new[] { "Create At", "End At", "Start At" },
                values: new object[] { new DateTime(2024, 10, 25, 15, 43, 49, 607, DateTimeKind.Local).AddTicks(339), new DateTime(2025, 2, 25, 15, 43, 49, 607, DateTimeKind.Local).AddTicks(340), new DateTime(2024, 10, 29, 15, 43, 49, 607, DateTimeKind.Local).AddTicks(339) });

            migrationBuilder.UpdateData(
                table: "Plan",
                keyColumn: "Id",
                keyValue: "PL5",
                columns: new[] { "Create At", "End At", "Start At" },
                values: new object[] { new DateTime(2024, 10, 25, 15, 43, 49, 607, DateTimeKind.Local).AddTicks(341), new DateTime(2025, 3, 25, 15, 43, 49, 607, DateTimeKind.Local).AddTicks(342), new DateTime(2024, 10, 30, 15, 43, 49, 607, DateTimeKind.Local).AddTicks(342) });

            migrationBuilder.UpdateData(
                table: "Project",
                keyColumn: "Id",
                keyValue: "PR1",
                column: "Create At",
                value: "10/25/2024 3:43:49 PM");

            migrationBuilder.UpdateData(
                table: "Project",
                keyColumn: "Id",
                keyValue: "PR2",
                column: "Create At",
                value: "10/25/2024 3:43:49 PM");

            migrationBuilder.UpdateData(
                table: "Project",
                keyColumn: "Id",
                keyValue: "PR3",
                column: "Create At",
                value: "10/25/2024 3:43:49 PM");

            migrationBuilder.UpdateData(
                table: "Project",
                keyColumn: "Id",
                keyValue: "Pr4",
                column: "Create At",
                value: "10/25/2024 3:43:49 PM");

            migrationBuilder.UpdateData(
                table: "Project",
                keyColumn: "Id",
                keyValue: "Pr5",
                column: "Create At",
                value: "10/25/2024 3:43:49 PM");

            migrationBuilder.UpdateData(
                table: "Role Application User In Project",
                keyColumn: "Id",
                keyValue: "RAUIP1",
                column: "Application User Id",
                value: "US1");

            migrationBuilder.UpdateData(
                table: "Role Application User In Project",
                keyColumn: "Id",
                keyValue: "RAUIP2",
                column: "Application User Id",
                value: "US2");

            migrationBuilder.UpdateData(
                table: "Role Application User In Project",
                keyColumn: "Id",
                keyValue: "RAUIP3",
                column: "Application User Id",
                value: "US3");

            migrationBuilder.UpdateData(
                table: "Role Application User In Project",
                keyColumn: "Id",
                keyValue: "RAUIP4",
                column: "Application User Id",
                value: "US4");

            migrationBuilder.UpdateData(
                table: "Role Application User In Project",
                keyColumn: "Id",
                keyValue: "RAUIP5",
                column: "Application User Id",
                value: "US5");

            migrationBuilder.UpdateData(
                table: "Task",
                keyColumn: "Id",
                keyValue: "TA1",
                columns: new[] { "Create At", "End At", "Start At" },
                values: new object[] { new DateTime(2024, 10, 25, 15, 43, 49, 607, DateTimeKind.Local).AddTicks(275), new DateTime(2024, 10, 30, 15, 43, 49, 607, DateTimeKind.Local).AddTicks(282), new DateTime(2024, 10, 26, 15, 43, 49, 607, DateTimeKind.Local).AddTicks(275) });

            migrationBuilder.UpdateData(
                table: "Task",
                keyColumn: "Id",
                keyValue: "TA2",
                columns: new[] { "Create At", "End At", "Start At" },
                values: new object[] { new DateTime(2024, 10, 25, 15, 43, 49, 607, DateTimeKind.Local).AddTicks(284), new DateTime(2024, 10, 31, 15, 43, 49, 607, DateTimeKind.Local).AddTicks(285), new DateTime(2024, 10, 27, 15, 43, 49, 607, DateTimeKind.Local).AddTicks(285) });

            migrationBuilder.UpdateData(
                table: "Task",
                keyColumn: "Id",
                keyValue: "TA3",
                columns: new[] { "Create At", "End At", "Start At" },
                values: new object[] { new DateTime(2024, 10, 25, 15, 43, 49, 607, DateTimeKind.Local).AddTicks(287), new DateTime(2024, 11, 1, 15, 43, 49, 607, DateTimeKind.Local).AddTicks(288), new DateTime(2024, 10, 28, 15, 43, 49, 607, DateTimeKind.Local).AddTicks(287) });

            migrationBuilder.UpdateData(
                table: "Task",
                keyColumn: "Id",
                keyValue: "TA4",
                columns: new[] { "Create At", "End At", "Start At" },
                values: new object[] { new DateTime(2024, 10, 25, 15, 43, 49, 607, DateTimeKind.Local).AddTicks(289), new DateTime(2024, 11, 2, 15, 43, 49, 607, DateTimeKind.Local).AddTicks(290), new DateTime(2024, 10, 29, 15, 43, 49, 607, DateTimeKind.Local).AddTicks(289) });

            migrationBuilder.UpdateData(
                table: "Task",
                keyColumn: "Id",
                keyValue: "TA5",
                columns: new[] { "Create At", "End At", "Start At" },
                values: new object[] { new DateTime(2024, 10, 25, 15, 43, 49, 607, DateTimeKind.Local).AddTicks(291), new DateTime(2024, 11, 3, 15, 43, 49, 607, DateTimeKind.Local).AddTicks(294), new DateTime(2024, 10, 30, 15, 43, 49, 607, DateTimeKind.Local).AddTicks(294) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "US1",
                column: "ConcurrencyStamp",
                value: "e698111c-9876-4bbd-a4bc-6403a642d8f2");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "US2",
                column: "ConcurrencyStamp",
                value: "4980d039-330e-428b-a658-645e3eb0d310");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "US3",
                column: "ConcurrencyStamp",
                value: "b97dab2d-df94-47be-b6f2-fd0d64b0eac5");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "US4",
                column: "ConcurrencyStamp",
                value: "9571a0a2-4f84-4e0e-9321-1a8008f25a32");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "US5",
                column: "ConcurrencyStamp",
                value: "692e00e6-d9f8-4212-aecd-26bbc1dc697f");

            migrationBuilder.CreateIndex(
                name: "IX_Role Application User In Project_Application User Id",
                table: "Role Application User In Project",
                column: "Application User Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Role Application User In Project_Users_Application User Id",
                table: "Role Application User In Project",
                column: "Application User Id",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Role Application User In Project_Users_Application User Id",
                table: "Role Application User In Project");

            migrationBuilder.DropIndex(
                name: "IX_Role Application User In Project_Application User Id",
                table: "Role Application User In Project");

            migrationBuilder.DropColumn(
                name: "Application User Id",
                table: "Role Application User In Project");

            migrationBuilder.UpdateData(
                table: "Plan",
                keyColumn: "Id",
                keyValue: "PL1",
                columns: new[] { "Create At", "End At", "Start At" },
                values: new object[] { new DateTime(2024, 10, 25, 10, 48, 7, 733, DateTimeKind.Local).AddTicks(2312), new DateTime(2024, 11, 25, 10, 48, 7, 733, DateTimeKind.Local).AddTicks(2314), new DateTime(2024, 10, 26, 10, 48, 7, 733, DateTimeKind.Local).AddTicks(2313) });

            migrationBuilder.UpdateData(
                table: "Plan",
                keyColumn: "Id",
                keyValue: "PL2",
                columns: new[] { "Create At", "End At", "Start At" },
                values: new object[] { new DateTime(2024, 10, 25, 10, 48, 7, 733, DateTimeKind.Local).AddTicks(2320), new DateTime(2024, 12, 25, 10, 48, 7, 733, DateTimeKind.Local).AddTicks(2321), new DateTime(2024, 10, 27, 10, 48, 7, 733, DateTimeKind.Local).AddTicks(2320) });

            migrationBuilder.UpdateData(
                table: "Plan",
                keyColumn: "Id",
                keyValue: "PL3",
                columns: new[] { "Create At", "End At", "Start At" },
                values: new object[] { new DateTime(2024, 10, 25, 10, 48, 7, 733, DateTimeKind.Local).AddTicks(2322), new DateTime(2025, 1, 25, 10, 48, 7, 733, DateTimeKind.Local).AddTicks(2323), new DateTime(2024, 10, 28, 10, 48, 7, 733, DateTimeKind.Local).AddTicks(2323) });

            migrationBuilder.UpdateData(
                table: "Plan",
                keyColumn: "Id",
                keyValue: "PL4",
                columns: new[] { "Create At", "End At", "Start At" },
                values: new object[] { new DateTime(2024, 10, 25, 10, 48, 7, 733, DateTimeKind.Local).AddTicks(2328), new DateTime(2025, 2, 25, 10, 48, 7, 733, DateTimeKind.Local).AddTicks(2329), new DateTime(2024, 10, 29, 10, 48, 7, 733, DateTimeKind.Local).AddTicks(2329) });

            migrationBuilder.UpdateData(
                table: "Plan",
                keyColumn: "Id",
                keyValue: "PL5",
                columns: new[] { "Create At", "End At", "Start At" },
                values: new object[] { new DateTime(2024, 10, 25, 10, 48, 7, 733, DateTimeKind.Local).AddTicks(2331), new DateTime(2025, 3, 25, 10, 48, 7, 733, DateTimeKind.Local).AddTicks(2332), new DateTime(2024, 10, 30, 10, 48, 7, 733, DateTimeKind.Local).AddTicks(2331) });

            migrationBuilder.UpdateData(
                table: "Project",
                keyColumn: "Id",
                keyValue: "PR1",
                column: "Create At",
                value: "10/25/2024 10:48:07 AM");

            migrationBuilder.UpdateData(
                table: "Project",
                keyColumn: "Id",
                keyValue: "PR2",
                column: "Create At",
                value: "10/25/2024 10:48:07 AM");

            migrationBuilder.UpdateData(
                table: "Project",
                keyColumn: "Id",
                keyValue: "PR3",
                column: "Create At",
                value: "10/25/2024 10:48:07 AM");

            migrationBuilder.UpdateData(
                table: "Project",
                keyColumn: "Id",
                keyValue: "Pr4",
                column: "Create At",
                value: "10/25/2024 10:48:07 AM");

            migrationBuilder.UpdateData(
                table: "Project",
                keyColumn: "Id",
                keyValue: "Pr5",
                column: "Create At",
                value: "10/25/2024 10:48:07 AM");

            migrationBuilder.UpdateData(
                table: "Task",
                keyColumn: "Id",
                keyValue: "TA1",
                columns: new[] { "Create At", "End At", "Start At" },
                values: new object[] { new DateTime(2024, 10, 25, 10, 48, 7, 733, DateTimeKind.Local).AddTicks(2271), new DateTime(2024, 10, 30, 10, 48, 7, 733, DateTimeKind.Local).AddTicks(2279), new DateTime(2024, 10, 26, 10, 48, 7, 733, DateTimeKind.Local).AddTicks(2273) });

            migrationBuilder.UpdateData(
                table: "Task",
                keyColumn: "Id",
                keyValue: "TA2",
                columns: new[] { "Create At", "End At", "Start At" },
                values: new object[] { new DateTime(2024, 10, 25, 10, 48, 7, 733, DateTimeKind.Local).AddTicks(2280), new DateTime(2024, 10, 31, 10, 48, 7, 733, DateTimeKind.Local).AddTicks(2282), new DateTime(2024, 10, 27, 10, 48, 7, 733, DateTimeKind.Local).AddTicks(2281) });

            migrationBuilder.UpdateData(
                table: "Task",
                keyColumn: "Id",
                keyValue: "TA3",
                columns: new[] { "Create At", "End At", "Start At" },
                values: new object[] { new DateTime(2024, 10, 25, 10, 48, 7, 733, DateTimeKind.Local).AddTicks(2283), new DateTime(2024, 11, 1, 10, 48, 7, 733, DateTimeKind.Local).AddTicks(2284), new DateTime(2024, 10, 28, 10, 48, 7, 733, DateTimeKind.Local).AddTicks(2283) });

            migrationBuilder.UpdateData(
                table: "Task",
                keyColumn: "Id",
                keyValue: "TA4",
                columns: new[] { "Create At", "End At", "Start At" },
                values: new object[] { new DateTime(2024, 10, 25, 10, 48, 7, 733, DateTimeKind.Local).AddTicks(2285), new DateTime(2024, 11, 2, 10, 48, 7, 733, DateTimeKind.Local).AddTicks(2286), new DateTime(2024, 10, 29, 10, 48, 7, 733, DateTimeKind.Local).AddTicks(2285) });

            migrationBuilder.UpdateData(
                table: "Task",
                keyColumn: "Id",
                keyValue: "TA5",
                columns: new[] { "Create At", "End At", "Start At" },
                values: new object[] { new DateTime(2024, 10, 25, 10, 48, 7, 733, DateTimeKind.Local).AddTicks(2287), new DateTime(2024, 11, 3, 10, 48, 7, 733, DateTimeKind.Local).AddTicks(2288), new DateTime(2024, 10, 30, 10, 48, 7, 733, DateTimeKind.Local).AddTicks(2287) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "US1",
                column: "ConcurrencyStamp",
                value: "abd13cf0-2f98-4690-8e66-eeb4da5e9c00");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "US2",
                column: "ConcurrencyStamp",
                value: "469ab14c-564c-4461-8fa3-a87f17d26d6f");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "US3",
                column: "ConcurrencyStamp",
                value: "1693a449-764f-46aa-ae7e-93733901e86b");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "US4",
                column: "ConcurrencyStamp",
                value: "7a34f458-52a7-4338-860f-3a84fd733419");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "US5",
                column: "ConcurrencyStamp",
                value: "89cbf8b9-d7d2-4d49-9be2-cd041c145fc5");
        }
    }
}
