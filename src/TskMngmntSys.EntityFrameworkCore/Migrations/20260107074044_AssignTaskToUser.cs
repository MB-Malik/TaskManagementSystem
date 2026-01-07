using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TskMngmntSys.Migrations
{
    /// <inheritdoc />
    public partial class AssignTaskToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "AssignedUserId",
                table: "Tasks",
                type: "bigint",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssignedUserId",
                table: "Tasks");
        }
    }
}
