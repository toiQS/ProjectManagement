using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PM.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class PMEditidseedingdata : Migration
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
                    IsDeleted = table.Column<bool>(name: "Is Deleted", type: "bit", nullable: false),
                    IsModified = table.Column<bool>(name: "Is Modified", type: "bit", nullable: false)
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
                    PositionId = table.Column<string>(name: "Position Id", type: "nvarchar(450)", nullable: false),
                    UserId = table.Column<string>(name: "User Id", type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Position Work Of Member", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Position Work Of Member_Position In Project_Position Id",
                        column: x => x.PositionId,
                        principalTable: "Position In Project",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                    { "PL1", new DateTime(2024, 10, 25, 10, 48, 7, 733, DateTimeKind.Local).AddTicks(2312), new DateTime(2024, 11, 25, 10, 48, 7, 733, DateTimeKind.Local).AddTicks(2314), "Plan A", new DateTime(2024, 10, 26, 10, 48, 7, 733, DateTimeKind.Local).AddTicks(2313) },
                    { "PL2", new DateTime(2024, 10, 25, 10, 48, 7, 733, DateTimeKind.Local).AddTicks(2320), new DateTime(2024, 12, 25, 10, 48, 7, 733, DateTimeKind.Local).AddTicks(2321), "Plan B", new DateTime(2024, 10, 27, 10, 48, 7, 733, DateTimeKind.Local).AddTicks(2320) },
                    { "PL3", new DateTime(2024, 10, 25, 10, 48, 7, 733, DateTimeKind.Local).AddTicks(2322), new DateTime(2025, 1, 25, 10, 48, 7, 733, DateTimeKind.Local).AddTicks(2323), "Plan C", new DateTime(2024, 10, 28, 10, 48, 7, 733, DateTimeKind.Local).AddTicks(2323) },
                    { "PL4", new DateTime(2024, 10, 25, 10, 48, 7, 733, DateTimeKind.Local).AddTicks(2328), new DateTime(2025, 2, 25, 10, 48, 7, 733, DateTimeKind.Local).AddTicks(2329), "Plan D", new DateTime(2024, 10, 29, 10, 48, 7, 733, DateTimeKind.Local).AddTicks(2329) },
                    { "PL5", new DateTime(2024, 10, 25, 10, 48, 7, 733, DateTimeKind.Local).AddTicks(2331), new DateTime(2025, 3, 25, 10, 48, 7, 733, DateTimeKind.Local).AddTicks(2332), "Plan E", new DateTime(2024, 10, 30, 10, 48, 7, 733, DateTimeKind.Local).AddTicks(2331) }
                });

            migrationBuilder.InsertData(
                table: "Project",
                columns: new[] { "Id", "Create At", "Is Deleted", "Is Modified", "Project Description", "Project Name", "Project Version", "Project Status" },
                values: new object[,]
                {
                    { "PR1", "10/25/2024 10:48:07 AM", false, true, "First project description", "Project Alpha", "1.0", "In Progress" },
                    { "PR2", "10/25/2024 10:48:07 AM", false, true, "Second project description", "Project Beta", "1.1", "Completed" },
                    { "PR3", "10/25/2024 10:48:07 AM", false, false, "Third project description", "Project Gamma", "1.2", "On Hold" },
                    { "Pr4", "10/25/2024 10:48:07 AM", false, false, "Fourth project description", "Project Delta", "1.3", "Cancelled" },
                    { "Pr5", "10/25/2024 10:48:07 AM", false, false, "Fifth project description", "Project Epsilon", "2.0", "In Progress" }
                });

            migrationBuilder.InsertData(
                table: "Role In Project",
                columns: new[] { "Id", "Role Description", "Role Name" },
                values: new object[,]
                {
                    { "RIP1", "Administrator Role", "Admin" },
                    { "RIP2", "Contributor Role", "Contributor" },
                    { "RIP3", "Viewer Role", "Viewer" },
                    { "RIP4", "Editor Role", "Editor" },
                    { "RIP5", "Owner Role", "Owner" }
                });

            migrationBuilder.InsertData(
                table: "Task",
                columns: new[] { "Id", "Create At", "End At", "Start At", "Task Description", "Task Name", "Task Status" },
                values: new object[,]
                {
                    { "TA1", new DateTime(2024, 10, 25, 10, 48, 7, 733, DateTimeKind.Local).AddTicks(2271), new DateTime(2024, 10, 30, 10, 48, 7, 733, DateTimeKind.Local).AddTicks(2279), new DateTime(2024, 10, 26, 10, 48, 7, 733, DateTimeKind.Local).AddTicks(2273), "Task description for project 1", "Task 1", "Pending" },
                    { "TA2", new DateTime(2024, 10, 25, 10, 48, 7, 733, DateTimeKind.Local).AddTicks(2280), new DateTime(2024, 10, 31, 10, 48, 7, 733, DateTimeKind.Local).AddTicks(2282), new DateTime(2024, 10, 27, 10, 48, 7, 733, DateTimeKind.Local).AddTicks(2281), "Task description for project 2", "Task 2", "Completed" },
                    { "TA3", new DateTime(2024, 10, 25, 10, 48, 7, 733, DateTimeKind.Local).AddTicks(2283), new DateTime(2024, 11, 1, 10, 48, 7, 733, DateTimeKind.Local).AddTicks(2284), new DateTime(2024, 10, 28, 10, 48, 7, 733, DateTimeKind.Local).AddTicks(2283), "Task description for project 3", "Task 3", "In Progress" },
                    { "TA4", new DateTime(2024, 10, 25, 10, 48, 7, 733, DateTimeKind.Local).AddTicks(2285), new DateTime(2024, 11, 2, 10, 48, 7, 733, DateTimeKind.Local).AddTicks(2286), new DateTime(2024, 10, 29, 10, 48, 7, 733, DateTimeKind.Local).AddTicks(2285), "Task description for project 4", "Task 4", "On Hold" },
                    { "TA5", new DateTime(2024, 10, 25, 10, 48, 7, 733, DateTimeKind.Local).AddTicks(2287), new DateTime(2024, 11, 3, 10, 48, 7, 733, DateTimeKind.Local).AddTicks(2288), new DateTime(2024, 10, 30, 10, 48, 7, 733, DateTimeKind.Local).AddTicks(2287), "Task description for project 5", "Task 5", "Pending" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "First Name", "Full Name", "Last Name", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "Path Image", "Phone", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { "US1", 0, "abd13cf0-2f98-4690-8e66-eeb4da5e9c00", "john.doe@example.com", false, "John", "John Doe", "Doe", false, null, null, null, null, "john.jpg", "123456789", null, false, null, false, "john_doe" },
                    { "US2", 0, "469ab14c-564c-4461-8fa3-a87f17d26d6f", "jane.smith@example.com", false, "Jane", "Jane Smith", "Smith", false, null, null, null, null, "jane.jpg", "987654321", null, false, null, false, "jane_smith" },
                    { "US3", 0, "1693a449-764f-46aa-ae7e-93733901e86b", "michael.j@example.com", false, "Michael", "Michael Johnson", "Johnson", false, null, null, null, null, "michael.jpg", "234567890", null, false, null, false, "michael_j" },
                    { "US4", 0, "7a34f458-52a7-4338-860f-3a84fd733419", "emily.d@example.com", false, "Emily", "Emily Davis", "Davis", false, null, null, null, null, "emily.jpg", "345678901", null, false, null, false, "emily_d" },
                    { "US5", 0, "89cbf8b9-d7d2-4d49-9be2-cd041c145fc5", "chris.b@example.com", false, "Chris", "Chris Brown", "Brown", false, null, null, null, null, "chris.jpg", "456789012", null, false, null, false, "chris_b" }
                });

            migrationBuilder.InsertData(
                table: "Plan In Project",
                columns: new[] { "Id", "Plan Id", "Project Id" },
                values: new object[,]
                {
                    { "PLIP1", "PL1", "PR1" },
                    { "PLIP2", "PL2", "PR2" },
                    { "PLIP3", "PL3", "PR3" },
                    { "PLIP4", "PL4", "PR4" },
                    { "PLIP5", "PL5", "PR5" }
                });

            migrationBuilder.InsertData(
                table: "Position In Project",
                columns: new[] { "Id", "Position Description", "Position Name", "Project Id" },
                values: new object[,]
                {
                    { "POIP1", "Project Manager", "Manager", "PR1" },
                    { "POIP2", "Software Developer", "Developer", "PR2" },
                    { "POIP3", "Software Tester", "Tester", "PR3" },
                    { "POIP4", "UI/UX Designer", "Designer", "PR4" },
                    { "POIP5", "DevOps Engineer", "DevOps", "PR5" }
                });

            migrationBuilder.InsertData(
                table: "Role Application User In Project",
                columns: new[] { "Id", "Project Id", "Role Id" },
                values: new object[,]
                {
                    { "RAUIP1", "PR1", "RIP1" },
                    { "RAUIP2", "PR2", "RIP2" },
                    { "RAUIP3", "PR3", "RIP3" },
                    { "RAUIP4", "PR4", "RIP4" },
                    { "RAUIP5", "PR5", "RIP5" }
                });

            migrationBuilder.InsertData(
                table: "Task In Plan",
                columns: new[] { "Id", "Plan Id", "Task Id" },
                values: new object[,]
                {
                    { "TIP1", "PL1", "TA1" },
                    { "TIP2", "PL2", "TA2" },
                    { "TIP3", "PL3", "TA3" },
                    { "TIP4", "PL4", "TA4" },
                    { "TIP5", "PL5", "TA5" }
                });

            migrationBuilder.InsertData(
                table: "Position Work Of Member",
                columns: new[] { "Id", "Position Id", "User Id" },
                values: new object[,]
                {
                    { "PWOM1", "POIP1", "US1" },
                    { "PWOM2", "POIP2", "US2" },
                    { "PWOM3", "POIP3", "US3" },
                    { "PWOM4", "POIP4", "US4" },
                    { "PWOM5", "POIP5", "US5" }
                });

            migrationBuilder.InsertData(
                table: "Member In Task",
                columns: new[] { "Id", "Member Id", "Task Id" },
                values: new object[,]
                {
                    { "MIT1", "PWOM1", "TA1" },
                    { "MIT2", "PWOM2", "TA2" },
                    { "MIT3", "PWOM3", "TA3" },
                    { "MIT4", "PWOM4", "TA4" },
                    { "MIT5", "PWOM5", "TA5" }
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
                name: "IX_Position Work Of Member_Position Id",
                table: "Position Work Of Member",
                column: "Position Id");

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
