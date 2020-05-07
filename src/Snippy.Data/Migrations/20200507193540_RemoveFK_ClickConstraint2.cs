using Microsoft.EntityFrameworkCore.Migrations;

namespace Snippy.Data.Migrations
{
    public partial class RemoveFK_ClickConstraint2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clicks_URLs_ShortUrlKey",
                table: "Clicks");

            migrationBuilder.DropIndex(
                name: "IX_Clicks_ShortUrlKey",
                table: "Clicks");

            migrationBuilder.AlterColumn<string>(
                name: "ShortUrlKey",
                table: "Clicks",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(60)",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ShortUrlKey",
                table: "Clicks",
                type: "nvarchar(60)",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Clicks_ShortUrlKey",
                table: "Clicks",
                column: "ShortUrlKey");

            migrationBuilder.AddForeignKey(
                name: "FK_Clicks_URLs_ShortUrlKey",
                table: "Clicks",
                column: "ShortUrlKey",
                principalTable: "URLs",
                principalColumn: "Key",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
