using Microsoft.EntityFrameworkCore.Migrations;

namespace WIMP_Server.Migrations
{
    public partial class NullableReportedById : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Intel_Characters_ReportedById",
                table: "Intel");

            migrationBuilder.DropForeignKey(
                name: "FK_Intel_Ships_ShipId",
                table: "Intel");

            migrationBuilder.AlterColumn<int>(
                name: "ShipId",
                table: "Intel",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<int>(
                name: "ReportedById",
                table: "Intel",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddForeignKey(
                name: "FK_Intel_Characters_ReportedById",
                table: "Intel",
                column: "ReportedById",
                principalTable: "Characters",
                principalColumn: "CharacterId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Intel_Ships_ShipId",
                table: "Intel",
                column: "ShipId",
                principalTable: "Ships",
                principalColumn: "ShipId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Intel_Characters_ReportedById",
                table: "Intel");

            migrationBuilder.DropForeignKey(
                name: "FK_Intel_Ships_ShipId",
                table: "Intel");

            migrationBuilder.AlterColumn<int>(
                name: "ShipId",
                table: "Intel",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ReportedById",
                table: "Intel",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Intel_Characters_ReportedById",
                table: "Intel",
                column: "ReportedById",
                principalTable: "Characters",
                principalColumn: "CharacterId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Intel_Ships_ShipId",
                table: "Intel",
                column: "ShipId",
                principalTable: "Ships",
                principalColumn: "ShipId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
