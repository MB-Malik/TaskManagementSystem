using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TskMngmntSys.Migrations
{
    /// <inheritdoc />
    public partial class AddingTenantId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "Tasks",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Tasks");
        }
    }
}
