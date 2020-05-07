using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Snippy.Data.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Owners",
                columns: table => new
                {
                    UserName = table.Column<string>(maxLength: 100, nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 100, nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<string>(maxLength: 100, nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    FullName = table.Column<string>(maxLength: 200, nullable: true),
                    Email = table.Column<string>(maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Owners", x => x.UserName);
                });

            migrationBuilder.CreateTable(
                name: "URLs",
                columns: table => new
                {
                    Key = table.Column<string>(maxLength: 60, nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 100, nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<string>(maxLength: 100, nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    Url = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_URLs", x => x.Key);
                });

            migrationBuilder.CreateTable(
                name: "Clicks",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 100, nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<string>(maxLength: 100, nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    SourceIp = table.Column<string>(maxLength: 20, nullable: true),
                    IdentId = table.Column<string>(maxLength: 100, nullable: true),
                    ShortUrlKey = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clicks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Clicks_URLs_ShortUrlKey",
                        column: x => x.ShortUrlKey,
                        principalTable: "URLs",
                        principalColumn: "Key",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OwnerUrls",
                columns: table => new
                {
                    OwnerUserName = table.Column<string>(nullable: false),
                    ShortUrlKey = table.Column<string>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 100, nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<string>(maxLength: 100, nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OwnerUrls", x => new { x.OwnerUserName, x.ShortUrlKey });
                    table.ForeignKey(
                        name: "FK_OwnerUrls_Owners_OwnerUserName",
                        column: x => x.OwnerUserName,
                        principalTable: "Owners",
                        principalColumn: "UserName",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OwnerUrls_URLs_ShortUrlKey",
                        column: x => x.ShortUrlKey,
                        principalTable: "URLs",
                        principalColumn: "Key",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Clicks_ShortUrlKey",
                table: "Clicks",
                column: "ShortUrlKey");

            migrationBuilder.CreateIndex(
                name: "IX_OwnerUrls_ShortUrlKey",
                table: "OwnerUrls",
                column: "ShortUrlKey");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Clicks");

            migrationBuilder.DropTable(
                name: "OwnerUrls");

            migrationBuilder.DropTable(
                name: "Owners");

            migrationBuilder.DropTable(
                name: "URLs");
        }
    }
}
