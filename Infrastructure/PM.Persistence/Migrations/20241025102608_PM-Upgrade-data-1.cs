using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PM.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class PMUpgradedata1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Is Modified",
                table: "Project",
                newName: "Is Accessed");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Is Accessed",
                table: "Project",
                newName: "Is Modified");
        }
    }
}
