using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WIMP_Server.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Characters",
                columns: table => new
                {
                    CharacterId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Characters", x => x.CharacterId);
                });

            migrationBuilder.CreateTable(
                name: "Ships",
                columns: table => new
                {
                    ShipId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ships", x => x.ShipId);
                });

            migrationBuilder.CreateTable(
                name: "StarSystems",
                columns: table => new
                {
                    StarSystemId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConstellationId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StarSystems", x => x.StarSystemId);
                });

            migrationBuilder.CreateTable(
                name: "Stargates",
                columns: table => new
                {
                    StargateId = table.Column<int>(type: "int", nullable: false),
                    SrcStarSystemId = table.Column<int>(type: "int", nullable: true),
                    DstStarSystemId = table.Column<int>(type: "int", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stargates", x => x.StargateId);
                    table.ForeignKey(
                        name: "FK_Stargates_StarSystems_DstStarSystemId",
                        column: x => x.DstStarSystemId,
                        principalTable: "StarSystems",
                        principalColumn: "StarSystemId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Stargates_StarSystems_SrcStarSystemId",
                        column: x => x.SrcStarSystemId,
                        principalTable: "StarSystems",
                        principalColumn: "StarSystemId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Intel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StarSystemId = table.Column<int>(type: "int", nullable: false),
                    CharacterId = table.Column<int>(type: "int", nullable: true),
                    ShipId = table.Column<int>(type: "int", nullable: true),
                    ReportedById = table.Column<int>(type: "int", nullable: true),
                    IsSpike = table.Column<bool>(type: "bit", nullable: false),
                    IsClear = table.Column<bool>(type: "bit", nullable: false),
                    StargateId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Intel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Intel_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "CharacterId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Intel_Characters_ReportedById",
                        column: x => x.ReportedById,
                        principalTable: "Characters",
                        principalColumn: "CharacterId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Intel_Ships_ShipId",
                        column: x => x.ShipId,
                        principalTable: "Ships",
                        principalColumn: "ShipId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Intel_Stargates_StargateId",
                        column: x => x.StargateId,
                        principalTable: "Stargates",
                        principalColumn: "StargateId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Intel_StarSystems_StarSystemId",
                        column: x => x.StarSystemId,
                        principalTable: "StarSystems",
                        principalColumn: "StarSystemId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Intel_CharacterId",
                table: "Intel",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_Intel_ReportedById",
                table: "Intel",
                column: "ReportedById");

            migrationBuilder.CreateIndex(
                name: "IX_Intel_ShipId",
                table: "Intel",
                column: "ShipId");

            migrationBuilder.CreateIndex(
                name: "IX_Intel_StargateId",
                table: "Intel",
                column: "StargateId");

            migrationBuilder.CreateIndex(
                name: "IX_Intel_StarSystemId",
                table: "Intel",
                column: "StarSystemId");

            migrationBuilder.CreateIndex(
                name: "IX_Stargates_DstStarSystemId",
                table: "Stargates",
                column: "DstStarSystemId");

            migrationBuilder.CreateIndex(
                name: "IX_Stargates_SrcStarSystemId",
                table: "Stargates",
                column: "SrcStarSystemId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Intel");

            migrationBuilder.DropTable(
                name: "Characters");

            migrationBuilder.DropTable(
                name: "Ships");

            migrationBuilder.DropTable(
                name: "Stargates");

            migrationBuilder.DropTable(
                name: "StarSystems");
        }
    }
}
