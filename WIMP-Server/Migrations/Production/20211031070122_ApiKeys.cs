using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WIMP_Server.Migrations.Production
{
    public partial class ApiKeys : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApiKeys",
                columns: table => new
                {
                    Key = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Owner = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table => table.PrimaryKey("PK_ApiKeys", x => x.Key));

            migrationBuilder.CreateTable(
                name: "ApiKeyRole",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApiKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiKeyRole", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApiKeyRole_ApiKeys_ApiKey",
                        column: x => x.ApiKey,
                        principalTable: "ApiKeys",
                        principalColumn: "Key",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApiKeyRole_ApiKey",
                table: "ApiKeyRole",
                column: "ApiKey");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApiKeyRole");

            migrationBuilder.DropTable(
                name: "ApiKeys");
        }
    }
}
