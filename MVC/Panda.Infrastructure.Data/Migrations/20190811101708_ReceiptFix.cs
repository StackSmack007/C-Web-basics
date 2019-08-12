using Microsoft.EntityFrameworkCore.Migrations;

namespace Panda.Infrastructure.Data.Migrations
{
    public partial class ReceiptFix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShippingAddress",
                table: "Receipts");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Receipts");

            migrationBuilder.AlterColumn<string>(
                name: "ShippingAddress",
                table: "Packages",
                maxLength: 512,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 512,
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ShippingAddress",
                table: "Receipts",
                maxLength: 512,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Receipts",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "ShippingAddress",
                table: "Packages",
                maxLength: 512,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 512);
        }
    }
}
