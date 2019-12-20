using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class RaceToRaceMap : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RaceCheckpoints_Races_RaceId",
                table: "RaceCheckpoints");

            migrationBuilder.DropForeignKey(
                name: "FK_RaceResults_Races_RaceId",
                table: "RaceResults");

            migrationBuilder.DropTable(
                name: "Races");

            migrationBuilder.CreateTable(
                name: "RaceMaps",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    CreatorId = table.Column<int>(nullable: false),
                    CreationDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RaceMaps", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_RaceCheckpoints_RaceMaps_RaceId",
                table: "RaceCheckpoints",
                column: "RaceId",
                principalTable: "RaceMaps",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RaceResults_RaceMaps_RaceId",
                table: "RaceResults",
                column: "RaceId",
                principalTable: "RaceMaps",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RaceCheckpoints_RaceMaps_RaceId",
                table: "RaceCheckpoints");

            migrationBuilder.DropForeignKey(
                name: "FK_RaceResults_RaceMaps_RaceId",
                table: "RaceResults");

            migrationBuilder.DropTable(
                name: "RaceMaps");

            migrationBuilder.CreateTable(
                name: "Races",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Races", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_RaceCheckpoints_Races_RaceId",
                table: "RaceCheckpoints",
                column: "RaceId",
                principalTable: "Races",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RaceResults_Races_RaceId",
                table: "RaceResults",
                column: "RaceId",
                principalTable: "Races",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
