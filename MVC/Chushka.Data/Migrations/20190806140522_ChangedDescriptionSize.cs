using Microsoft.EntityFrameworkCore.Migrations;

namespace Chushka.Data.Migrations
{
    public partial class ChangedDescriptionSize : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Products",
                maxLength: 2560,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 256,
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Products",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 2560,
                oldNullable: true);
        }
    }
}
