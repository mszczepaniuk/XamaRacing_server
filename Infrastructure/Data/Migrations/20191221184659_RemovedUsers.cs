using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class RemovedUsers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "RaceResults");

            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "RaceMaps");

            migrationBuilder.AddColumn<string>(
                name: "Nickname",
                table: "RaceResults",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "RaceMaps",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Score",
                table: "RaceMaps",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Nickname",
                table: "RaceResults");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "RaceMaps");

            migrationBuilder.DropColumn(
                name: "Score",
                table: "RaceMaps");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "RaceResults",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CreatorId",
                table: "RaceMaps",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
