using Microsoft.EntityFrameworkCore.Migrations;

namespace TorshiaApp.Migrations
{
    public partial class AutoGenOfIsReported : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsReported",
                table: "Tasks");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsReported",
                table: "Tasks",
                nullable: false,
                defaultValue: false);
        }
    }
}
