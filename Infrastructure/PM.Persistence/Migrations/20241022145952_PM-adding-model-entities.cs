using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PM.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class PMaddingmodelentities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Plan",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PlanName = table.Column<string>(name: "Plan Name", type: "nvarchar(max)", nullable: false),
                    CreateAt = table.Column<DateTime>(name: "Create At", type: "datetime2", nullable: false),
                    StartAt = table.Column<DateTime>(name: "Start At", type: "datetime2", nullable: false),
                    EndAt = table.Column<DateTime>(name: "End At", type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Plan", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Project",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProjectName = table.Column<string>(name: "Project Name", type: "nvarchar(max)", nullable: false),
                    ProjectDescription = table.Column<string>(name: "Project Description", type: "nvarchar(max)", nullable: false),
                    ProjectVersion = table.Column<string>(name: "Project Version", type: "nvarchar(max)", nullable: false),
                    ProjectStatus = table.Column<string>(name: "Project Status", type: "nvarchar(max)", nullable: false),
                    CreateAt = table.Column<string>(name: "Create At", type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(name: "Is Deleted", type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Project", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Role In Project",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleName = table.Column<string>(name: "Role Name", type: "nvarchar(max)", nullable: false),
                    RoleDescription = table.Column<string>(name: "Role Description", type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role In Project", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Task",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TaskName = table.Column<string>(name: "Task Name", type: "nvarchar(max)", nullable: false),
                    TaskDescription = table.Column<string>(name: "Task Description", type: "nvarchar(max)", nullable: false),
                    TaskStatus = table.Column<string>(name: "Task Status", type: "nvarchar(max)", nullable: false),
                    CreateAt = table.Column<DateTime>(name: "Create At", type: "datetime2", nullable: false),
                    StartAt = table.Column<DateTime>(name: "Start At", type: "datetime2", nullable: false),
                    EndAt = table.Column<DateTime>(name: "End At", type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Task", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FirstName = table.Column<string>(name: "First Name", type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(name: "Last Name", type: "nvarchar(max)", nullable: false),
                    FullName = table.Column<string>(name: "Full Name", type: "nvarchar(max)", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PathImage = table.Column<string>(name: "Path Image", type: "nvarchar(max)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Plan In Project",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProjectId = table.Column<string>(name: "Project Id", type: "nvarchar(450)", nullable: false),
                    PlanId = table.Column<string>(name: "Plan Id", type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Plan In Project", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Plan In Project_Plan_Plan Id",
                        column: x => x.PlanId,
                        principalTable: "Plan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Plan In Project_Project_Project Id",
                        column: x => x.ProjectId,
                        principalTable: "Project",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Position In Project",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PositionName = table.Column<string>(name: "Position Name", type: "nvarchar(max)", nullable: false),
                    PositionDescription = table.Column<string>(name: "Position Description", type: "nvarchar(max)", nullable: false),
                    ProjectId = table.Column<string>(name: "Project Id", type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Position In Project", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Position In Project_Project_Project Id",
                        column: x => x.ProjectId,
                        principalTable: "Project",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Role Application User In Project",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProjectId = table.Column<string>(name: "Project Id", type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(name: "Role Id", type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role Application User In Project", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Role Application User In Project_Project_Project Id",
                        column: x => x.ProjectId,
                        principalTable: "Project",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Role Application User In Project_Role In Project_Role Id",
                        column: x => x.RoleId,
                        principalTable: "Role In Project",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoleClaims_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Task In Plan",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PlanId = table.Column<string>(name: "Plan Id", type: "nvarchar(450)", nullable: false),
                    TaskId = table.Column<string>(name: "Task Id", type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Task In Plan", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Task In Plan_Plan_Plan Id",
                        column: x => x.PlanId,
                        principalTable: "Plan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Task In Plan_Task_Task Id",
                        column: x => x.TaskId,
                        principalTable: "Task",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserClaims_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_UserLogins_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_UserTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Position Work Of Member",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PositionId = table.Column<string>(name: "Position Id", type: "nvarchar(max)", nullable: false),
                    PostitionInProjectId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    UserId = table.Column<string>(name: "User Id", type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Position Work Of Member", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Position Work Of Member_Position In Project_PostitionInProjectId",
                        column: x => x.PostitionInProjectId,
                        principalTable: "Position In Project",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Position Work Of Member_Users_User Id",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Member In Task",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MemberId = table.Column<string>(name: "Member Id", type: "nvarchar(450)", nullable: false),
                    TaskId = table.Column<string>(name: "Task Id", type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Member In Task", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Member In Task_Position Work Of Member_Member Id",
                        column: x => x.MemberId,
                        principalTable: "Position Work Of Member",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Member In Task_Task_Task Id",
                        column: x => x.TaskId,
                        principalTable: "Task",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Plan",
                columns: new[] { "Id", "Create At", "End At", "Plan Name", "Start At" },
                values: new object[,]
                {
                    { "1", new DateTime(2024, 10, 22, 21, 59, 51, 879, DateTimeKind.Local).AddTicks(3589), new DateTime(2024, 11, 22, 21, 59, 51, 879, DateTimeKind.Local).AddTicks(3592), "Plan A", new DateTime(2024, 10, 23, 21, 59, 51, 879, DateTimeKind.Local).AddTicks(3590) },
                    { "2", new DateTime(2024, 10, 22, 21, 59, 51, 879, DateTimeKind.Local).AddTicks(3597), new DateTime(2024, 12, 22, 21, 59, 51, 879, DateTimeKind.Local).AddTicks(3598), "Plan B", new DateTime(2024, 10, 24, 21, 59, 51, 879, DateTimeKind.Local).AddTicks(3598) },
                    { "3", new DateTime(2024, 10, 22, 21, 59, 51, 879, DateTimeKind.Local).AddTicks(3600), new DateTime(2025, 1, 22, 21, 59, 51, 879, DateTimeKind.Local).AddTicks(3601), "Plan C", new DateTime(2024, 10, 25, 21, 59, 51, 879, DateTimeKind.Local).AddTicks(3600) },
                    { "4", new DateTime(2024, 10, 22, 21, 59, 51, 879, DateTimeKind.Local).AddTicks(3602), new DateTime(2025, 2, 22, 21, 59, 51, 879, DateTimeKind.Local).AddTicks(3603), "Plan D", new DateTime(2024, 10, 26, 21, 59, 51, 879, DateTimeKind.Local).AddTicks(3603) },
                    { "5", new DateTime(2024, 10, 22, 21, 59, 51, 879, DateTimeKind.Local).AddTicks(3605), new DateTime(2025, 3, 22, 21, 59, 51, 879, DateTimeKind.Local).AddTicks(3607), "Plan E", new DateTime(2024, 10, 27, 21, 59, 51, 879, DateTimeKind.Local).AddTicks(3605) }
                });

            migrationBuilder.InsertData(
                table: "Project",
                columns: new[] { "Id", "Create At", "Is Deleted", "Project Description", "Project Name", "Project Version", "Project Status" },
                values: new object[,]
                {
                    { "1", "10/22/2024 9:59:51 PM", false, "First project description", "Project Alpha", "1.0", "In Progress" },
                    { "2", "10/22/2024 9:59:51 PM", false, "Second project description", "Project Beta", "1.1", "Completed" },
                    { "3", "10/22/2024 9:59:51 PM", false, "Third project description", "Project Gamma", "1.2", "On Hold" },
                    { "4", "10/22/2024 9:59:51 PM", false, "Fourth project description", "Project Delta", "1.3", "Cancelled" },
                    { "5", "10/22/2024 9:59:51 PM", false, "Fifth project description", "Project Epsilon", "2.0", "In Progress" }
                });

            migrationBuilder.InsertData(
                table: "Role In Project",
                columns: new[] { "Id", "Role Description", "Role Name" },
                values: new object[,]
                {
                    { "1", "Administrator Role", "Admin" },
                    { "2", "Contributor Role", "Contributor" },
                    { "3", "Viewer Role", "Viewer" },
                    { "4", "Editor Role", "Editor" },
                    { "5", "Owner Role", "Owner" }
                });

            migrationBuilder.InsertData(
                table: "Task",
                columns: new[] { "Id", "Create At", "End At", "Start At", "Task Description", "Task Name", "Task Status" },
                values: new object[,]
                {
                    { "1", new DateTime(2024, 10, 22, 21, 59, 51, 879, DateTimeKind.Local).AddTicks(3542), new DateTime(2024, 10, 27, 21, 59, 51, 879, DateTimeKind.Local).AddTicks(3550), new DateTime(2024, 10, 23, 21, 59, 51, 879, DateTimeKind.Local).AddTicks(3543), "Task description for project 1", "Task 1", "Pending" },
                    { "2", new DateTime(2024, 10, 22, 21, 59, 51, 879, DateTimeKind.Local).AddTicks(3552), new DateTime(2024, 10, 28, 21, 59, 51, 879, DateTimeKind.Local).AddTicks(3553), new DateTime(2024, 10, 24, 21, 59, 51, 879, DateTimeKind.Local).AddTicks(3552), "Task description for project 2", "Task 2", "Completed" },
                    { "3", new DateTime(2024, 10, 22, 21, 59, 51, 879, DateTimeKind.Local).AddTicks(3554), new DateTime(2024, 10, 29, 21, 59, 51, 879, DateTimeKind.Local).AddTicks(3555), new DateTime(2024, 10, 25, 21, 59, 51, 879, DateTimeKind.Local).AddTicks(3555), "Task description for project 3", "Task 3", "In Progress" },
                    { "4", new DateTime(2024, 10, 22, 21, 59, 51, 879, DateTimeKind.Local).AddTicks(3557), new DateTime(2024, 10, 30, 21, 59, 51, 879, DateTimeKind.Local).AddTicks(3558), new DateTime(2024, 10, 26, 21, 59, 51, 879, DateTimeKind.Local).AddTicks(3557), "Task description for project 4", "Task 4", "On Hold" },
                    { "5", new DateTime(2024, 10, 22, 21, 59, 51, 879, DateTimeKind.Local).AddTicks(3559), new DateTime(2024, 10, 31, 21, 59, 51, 879, DateTimeKind.Local).AddTicks(3560), new DateTime(2024, 10, 27, 21, 59, 51, 879, DateTimeKind.Local).AddTicks(3559), "Task description for project 5", "Task 5", "Pending" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "First Name", "Full Name", "Last Name", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "Path Image", "Phone", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { "1", 0, "03389193-950d-4480-b099-908ac04732bf", "john.doe@example.com", false, "John", "John Doe", "Doe", false, null, null, null, null, "john.jpg", "123456789", null, false, null, false, "john_doe" },
                    { "2", 0, "f150db86-ef12-406e-bee9-5c0eecad483f", "jane.smith@example.com", false, "Jane", "Jane Smith", "Smith", false, null, null, null, null, "jane.jpg", "987654321", null, false, null, false, "jane_smith" },
                    { "3", 0, "5a28b5d6-1fba-44b9-8691-122be765c467", "michael.j@example.com", false, "Michael", "Michael Johnson", "Johnson", false, null, null, null, null, "michael.jpg", "234567890", null, false, null, false, "michael_j" },
                    { "4", 0, "b6721080-2237-48b9-b75b-abc8b52300a7", "emily.d@example.com", false, "Emily", "Emily Davis", "Davis", false, null, null, null, null, "emily.jpg", "345678901", null, false, null, false, "emily_d" },
                    { "5", 0, "d65bfde0-6143-4ee2-a28a-6384238fc7b1", "chris.b@example.com", false, "Chris", "Chris Brown", "Brown", false, null, null, null, null, "chris.jpg", "456789012", null, false, null, false, "chris_b" }
                });

            migrationBuilder.InsertData(
                table: "Plan In Project",
                columns: new[] { "Id", "Plan Id", "Project Id" },
                values: new object[,]
                {
                    { "1", "1", "1" },
                    { "2", "2", "2" },
                    { "3", "3", "3" },
                    { "4", "4", "4" },
                    { "5", "5", "5" }
                });

            migrationBuilder.InsertData(
                table: "Position In Project",
                columns: new[] { "Id", "Position Description", "Position Name", "Project Id" },
                values: new object[,]
                {
                    { "1", "Project Manager", "Manager", "1" },
                    { "2", "Software Developer", "Developer", "2" },
                    { "3", "Software Tester", "Tester", "3" },
                    { "4", "UI/UX Designer", "Designer", "4" },
                    { "5", "DevOps Engineer", "DevOps", "5" }
                });

            migrationBuilder.InsertData(
                table: "Position Work Of Member",
                columns: new[] { "Id", "Position Id", "PostitionInProjectId", "User Id" },
                values: new object[,]
                {
                    { "1", "1", null, "1" },
                    { "2", "2", null, "2" },
                    { "3", "3", null, "3" },
                    { "4", "4", null, "4" },
                    { "5", "5", null, "5" }
                });

            migrationBuilder.InsertData(
                table: "Role Application User In Project",
                columns: new[] { "Id", "Project Id", "Role Id" },
                values: new object[,]
                {
                    { "1", "1", "1" },
                    { "2", "2", "2" },
                    { "3", "3", "3" },
                    { "4", "4", "4" },
                    { "5", "5", "5" }
                });

            migrationBuilder.InsertData(
                table: "Task In Plan",
                columns: new[] { "Id", "Plan Id", "Task Id" },
                values: new object[,]
                {
                    { "1", "1", "1" },
                    { "2", "2", "2" },
                    { "3", "3", "3" },
                    { "4", "4", "4" },
                    { "5", "5", "5" }
                });

            migrationBuilder.InsertData(
                table: "Member In Task",
                columns: new[] { "Id", "Member Id", "Task Id" },
                values: new object[,]
                {
                    { "1", "1", "1" },
                    { "2", "2", "2" },
                    { "3", "3", "3" },
                    { "4", "4", "4" },
                    { "5", "5", "5" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Member In Task_Member Id",
                table: "Member In Task",
                column: "Member Id");

            migrationBuilder.CreateIndex(
                name: "IX_Member In Task_Task Id",
                table: "Member In Task",
                column: "Task Id");

            migrationBuilder.CreateIndex(
                name: "IX_Plan In Project_Plan Id",
                table: "Plan In Project",
                column: "Plan Id");

            migrationBuilder.CreateIndex(
                name: "IX_Plan In Project_Project Id",
                table: "Plan In Project",
                column: "Project Id");

            migrationBuilder.CreateIndex(
                name: "IX_Position In Project_Project Id",
                table: "Position In Project",
                column: "Project Id");

            migrationBuilder.CreateIndex(
                name: "IX_Position Work Of Member_PostitionInProjectId",
                table: "Position Work Of Member",
                column: "PostitionInProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Position Work Of Member_User Id",
                table: "Position Work Of Member",
                column: "User Id");

            migrationBuilder.CreateIndex(
                name: "IX_Role Application User In Project_Project Id",
                table: "Role Application User In Project",
                column: "Project Id");

            migrationBuilder.CreateIndex(
                name: "IX_Role Application User In Project_Role Id",
                table: "Role Application User In Project",
                column: "Role Id");

            migrationBuilder.CreateIndex(
                name: "IX_RoleClaims_RoleId",
                table: "RoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "Roles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Task In Plan_Plan Id",
                table: "Task In Plan",
                column: "Plan Id");

            migrationBuilder.CreateIndex(
                name: "IX_Task In Plan_Task Id",
                table: "Task In Plan",
                column: "Task Id");

            migrationBuilder.CreateIndex(
                name: "IX_UserClaims_UserId",
                table: "UserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserLogins_UserId",
                table: "UserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId",
                table: "UserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "Users",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "Users",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Member In Task");

            migrationBuilder.DropTable(
                name: "Plan In Project");

            migrationBuilder.DropTable(
                name: "Role Application User In Project");

            migrationBuilder.DropTable(
                name: "RoleClaims");

            migrationBuilder.DropTable(
                name: "Task In Plan");

            migrationBuilder.DropTable(
                name: "UserClaims");

            migrationBuilder.DropTable(
                name: "UserLogins");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "UserTokens");

            migrationBuilder.DropTable(
                name: "Position Work Of Member");

            migrationBuilder.DropTable(
                name: "Role In Project");

            migrationBuilder.DropTable(
                name: "Plan");

            migrationBuilder.DropTable(
                name: "Task");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Position In Project");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Project");
        }
    }
}
