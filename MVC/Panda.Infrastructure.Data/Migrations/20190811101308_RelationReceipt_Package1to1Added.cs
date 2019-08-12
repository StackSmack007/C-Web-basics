using Microsoft.EntityFrameworkCore.Migrations;

namespace Panda.Infrastructure.Data.Migrations
{
    public partial class RelationReceipt_Package1to1Added : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Packages_Receipts_ReceiptId",
                table: "Packages");

            migrationBuilder.DropForeignKey(
                name: "FK_Receipts_Packages_PackageId",
                table: "Receipts");

            migrationBuilder.DropIndex(
                name: "IX_Receipts_PackageId",
                table: "Receipts");

            migrationBuilder.DropIndex(
                name: "IX_Packages_ReceiptId",
                table: "Packages");

            migrationBuilder.CreateIndex(
                name: "IX_Receipts_PackageId",
                table: "Receipts",
                column: "PackageId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Receipts_Packages_PackageId",
                table: "Receipts",
                column: "PackageId",
                principalTable: "Packages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Receipts_Packages_PackageId",
                table: "Receipts");

            migrationBuilder.DropIndex(
                name: "IX_Receipts_PackageId",
                table: "Receipts");

            migrationBuilder.CreateIndex(
                name: "IX_Receipts_PackageId",
                table: "Receipts",
                column: "PackageId");

            migrationBuilder.CreateIndex(
                name: "IX_Packages_ReceiptId",
                table: "Packages",
                column: "ReceiptId");

            migrationBuilder.AddForeignKey(
                name: "FK_Packages_Receipts_ReceiptId",
                table: "Packages",
                column: "ReceiptId",
                principalTable: "Receipts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Receipts_Packages_PackageId",
                table: "Receipts",
                column: "PackageId",
                principalTable: "Packages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
