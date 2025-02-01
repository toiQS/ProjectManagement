using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PM.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class edittabledata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Member In Task_Position Work Of Member_Member Id",
                table: "Member In Task");

            migrationBuilder.DropTable(
                name: "Position Work Of Member");

            migrationBuilder.DropTable(
                name: "Role Application User In Project");

            migrationBuilder.CreateTable(
                name: "Member In Project",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleInProjectId = table.Column<string>(name: "Role In Project Id", type: "nvarchar(450)", nullable: false),
                    ApplicationUserId = table.Column<string>(name: "Application User Id", type: "nvarchar(450)", nullable: false),
                    PositionInProjectId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Member In Project", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Member In Project_Position In Project_PositionInProjectId",
                        column: x => x.PositionInProjectId,
                        principalTable: "Position In Project",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Member In Project_Role In Project_Role In Project Id",
                        column: x => x.RoleInProjectId,
                        principalTable: "Role In Project",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Member In Project_Users_Application User Id",
                        column: x => x.ApplicationUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Member In Project_Application User Id",
                table: "Member In Project",
                column: "Application User Id");

            migrationBuilder.CreateIndex(
                name: "IX_Member In Project_PositionInProjectId",
                table: "Member In Project",
                column: "PositionInProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Member In Project_Role In Project Id",
                table: "Member In Project",
                column: "Role In Project Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Member In Task_Member In Project_Member Id",
                table: "Member In Task",
                column: "Member Id",
                principalTable: "Member In Project",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Member In Task_Member In Project_Member Id",
                table: "Member In Task");

            migrationBuilder.DropTable(
                name: "Member In Project");

            migrationBuilder.CreateTable(
                name: "Role Application User In Project",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ApplicationUserId = table.Column<string>(name: "Application User Id", type: "nvarchar(450)", nullable: false),
                    ProjectId = table.Column<string>(name: "Project Id", type: "nvarchar(450)", nullable: false),
                    RoleInProjectId = table.Column<string>(name: "Role In Project Id", type: "nvarchar(450)", nullable: false)
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
                        name: "FK_Role Application User In Project_Role In Project_Role In Project Id",
                        column: x => x.RoleInProjectId,
                        principalTable: "Role In Project",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Role Application User In Project_Users_Application User Id",
                        column: x => x.ApplicationUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Position Work Of Member",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PositionInProjectId = table.Column<string>(name: "Position In Project Id", type: "nvarchar(450)", nullable: false),
                    RoleApplicationUserInProjectId = table.Column<string>(name: "Role Application User In Project Id", type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Position Work Of Member", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Position Work Of Member_Position In Project_Position In Project Id",
                        column: x => x.PositionInProjectId,
                        principalTable: "Position In Project",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Position Work Of Member_Role Application User In Project_Role Application User In Project Id",
                        column: x => x.RoleApplicationUserInProjectId,
                        principalTable: "Role Application User In Project",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Position Work Of Member_Position In Project Id",
                table: "Position Work Of Member",
                column: "Position In Project Id");

            migrationBuilder.CreateIndex(
                name: "IX_Position Work Of Member_Role Application User In Project Id",
                table: "Position Work Of Member",
                column: "Role Application User In Project Id");

            migrationBuilder.CreateIndex(
                name: "IX_Role Application User In Project_Application User Id",
                table: "Role Application User In Project",
                column: "Application User Id");

            migrationBuilder.CreateIndex(
                name: "IX_Role Application User In Project_Project Id",
                table: "Role Application User In Project",
                column: "Project Id");

            migrationBuilder.CreateIndex(
                name: "IX_Role Application User In Project_Role In Project Id",
                table: "Role Application User In Project",
                column: "Role In Project Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Member In Task_Position Work Of Member_Member Id",
                table: "Member In Task",
                column: "Member Id",
                principalTable: "Position Work Of Member",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
