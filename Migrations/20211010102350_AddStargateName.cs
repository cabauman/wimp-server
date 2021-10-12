using Microsoft.EntityFrameworkCore.Migrations;

namespace WIMP_Server.Migrations
{
    public partial class AddStargateName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Intel_Stargate_StarGateId",
                table: "Intel");

            migrationBuilder.RenameColumn(
                name: "StarGateId",
                table: "Stargate",
                newName: "StargateId");

            migrationBuilder.RenameColumn(
                name: "StarGateId",
                table: "Intel",
                newName: "StargateId");

            migrationBuilder.RenameIndex(
                name: "IX_Intel_StarGateId",
                table: "Intel",
                newName: "IX_Intel_StargateId");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Stargate",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_Intel_Stargate_StargateId",
                table: "Intel",
                column: "StargateId",
                principalTable: "Stargate",
                principalColumn: "StargateId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Intel_Stargate_StargateId",
                table: "Intel");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Stargate");

            migrationBuilder.RenameColumn(
                name: "StargateId",
                table: "Stargate",
                newName: "StarGateId");

            migrationBuilder.RenameColumn(
                name: "StargateId",
                table: "Intel",
                newName: "StarGateId");

            migrationBuilder.RenameIndex(
                name: "IX_Intel_StargateId",
                table: "Intel",
                newName: "IX_Intel_StarGateId");

            migrationBuilder.AddForeignKey(
                name: "FK_Intel_Stargate_StarGateId",
                table: "Intel",
                column: "StarGateId",
                principalTable: "Stargate",
                principalColumn: "StarGateId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
