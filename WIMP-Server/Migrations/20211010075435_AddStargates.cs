using Microsoft.EntityFrameworkCore.Migrations;

namespace WIMP_Server.Migrations
{
    public partial class AddStargates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StarGateId",
                table: "Intel",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Stargate",
                columns: table => new
                {
                    StarGateId = table.Column<int>(type: "INTEGER", nullable: false),
                    SrcStarSystemId = table.Column<int>(type: "INTEGER", nullable: false),
                    DstStarSystemId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stargate", x => x.StarGateId);
                    table.ForeignKey(
                        name: "FK_Stargate_StarSystems_DstStarSystemId",
                        column: x => x.DstStarSystemId,
                        principalTable: "StarSystems",
                        principalColumn: "StarSystemId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Stargate_StarSystems_SrcStarSystemId",
                        column: x => x.SrcStarSystemId,
                        principalTable: "StarSystems",
                        principalColumn: "StarSystemId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Intel_StarGateId",
                table: "Intel",
                column: "StarGateId");

            migrationBuilder.CreateIndex(
                name: "IX_Stargate_DstStarSystemId",
                table: "Stargate",
                column: "DstStarSystemId");

            migrationBuilder.CreateIndex(
                name: "IX_Stargate_SrcStarSystemId",
                table: "Stargate",
                column: "SrcStarSystemId");

            migrationBuilder.AddForeignKey(
                name: "FK_Intel_Stargate_StarGateId",
                table: "Intel",
                column: "StarGateId",
                principalTable: "Stargate",
                principalColumn: "StarGateId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Intel_Stargate_StarGateId",
                table: "Intel");

            migrationBuilder.DropTable(
                name: "Stargate");

            migrationBuilder.DropIndex(
                name: "IX_Intel_StarGateId",
                table: "Intel");

            migrationBuilder.DropColumn(
                name: "StarGateId",
                table: "Intel");
        }
    }
}
