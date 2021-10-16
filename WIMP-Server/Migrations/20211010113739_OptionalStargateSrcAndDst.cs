using Microsoft.EntityFrameworkCore.Migrations;

namespace WIMP_Server.Migrations
{
    public partial class OptionalStargateSrcAndDst : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Stargates_StarSystems_DstStarSystemId",
                table: "Stargates");

            migrationBuilder.DropForeignKey(
                name: "FK_Stargates_StarSystems_SrcStarSystemId",
                table: "Stargates");

            migrationBuilder.AlterColumn<int>(
                name: "SrcStarSystemId",
                table: "Stargates",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<int>(
                name: "DstStarSystemId",
                table: "Stargates",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddForeignKey(
                name: "FK_Stargates_StarSystems_DstStarSystemId",
                table: "Stargates",
                column: "DstStarSystemId",
                principalTable: "StarSystems",
                principalColumn: "StarSystemId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Stargates_StarSystems_SrcStarSystemId",
                table: "Stargates",
                column: "SrcStarSystemId",
                principalTable: "StarSystems",
                principalColumn: "StarSystemId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Stargates_StarSystems_DstStarSystemId",
                table: "Stargates");

            migrationBuilder.DropForeignKey(
                name: "FK_Stargates_StarSystems_SrcStarSystemId",
                table: "Stargates");

            migrationBuilder.AlterColumn<int>(
                name: "SrcStarSystemId",
                table: "Stargates",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DstStarSystemId",
                table: "Stargates",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Stargates_StarSystems_DstStarSystemId",
                table: "Stargates",
                column: "DstStarSystemId",
                principalTable: "StarSystems",
                principalColumn: "StarSystemId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Stargates_StarSystems_SrcStarSystemId",
                table: "Stargates",
                column: "SrcStarSystemId",
                principalTable: "StarSystems",
                principalColumn: "StarSystemId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
