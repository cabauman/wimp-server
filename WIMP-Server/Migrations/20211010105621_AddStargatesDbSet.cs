using Microsoft.EntityFrameworkCore.Migrations;

namespace WIMP_Server.Migrations
{
    public partial class AddStargatesDbSet : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Intel_Stargate_StargateId",
                table: "Intel");

            migrationBuilder.DropForeignKey(
                name: "FK_Stargate_StarSystems_DstStarSystemId",
                table: "Stargate");

            migrationBuilder.DropForeignKey(
                name: "FK_Stargate_StarSystems_SrcStarSystemId",
                table: "Stargate");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Stargate",
                table: "Stargate");

            migrationBuilder.RenameTable(
                name: "Stargate",
                newName: "Stargates");

            migrationBuilder.RenameIndex(
                name: "IX_Stargate_SrcStarSystemId",
                table: "Stargates",
                newName: "IX_Stargates_SrcStarSystemId");

            migrationBuilder.RenameIndex(
                name: "IX_Stargate_DstStarSystemId",
                table: "Stargates",
                newName: "IX_Stargates_DstStarSystemId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Stargates",
                table: "Stargates",
                column: "StargateId");

            migrationBuilder.AddForeignKey(
                name: "FK_Intel_Stargates_StargateId",
                table: "Intel",
                column: "StargateId",
                principalTable: "Stargates",
                principalColumn: "StargateId",
                onDelete: ReferentialAction.Restrict);

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Intel_Stargates_StargateId",
                table: "Intel");

            migrationBuilder.DropForeignKey(
                name: "FK_Stargates_StarSystems_DstStarSystemId",
                table: "Stargates");

            migrationBuilder.DropForeignKey(
                name: "FK_Stargates_StarSystems_SrcStarSystemId",
                table: "Stargates");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Stargates",
                table: "Stargates");

            migrationBuilder.RenameTable(
                name: "Stargates",
                newName: "Stargate");

            migrationBuilder.RenameIndex(
                name: "IX_Stargates_SrcStarSystemId",
                table: "Stargate",
                newName: "IX_Stargate_SrcStarSystemId");

            migrationBuilder.RenameIndex(
                name: "IX_Stargates_DstStarSystemId",
                table: "Stargate",
                newName: "IX_Stargate_DstStarSystemId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Stargate",
                table: "Stargate",
                column: "StargateId");

            migrationBuilder.AddForeignKey(
                name: "FK_Intel_Stargate_StargateId",
                table: "Intel",
                column: "StargateId",
                principalTable: "Stargate",
                principalColumn: "StargateId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Stargate_StarSystems_DstStarSystemId",
                table: "Stargate",
                column: "DstStarSystemId",
                principalTable: "StarSystems",
                principalColumn: "StarSystemId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Stargate_StarSystems_SrcStarSystemId",
                table: "Stargate",
                column: "SrcStarSystemId",
                principalTable: "StarSystems",
                principalColumn: "StarSystemId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
