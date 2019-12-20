using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class RaceResultTimeChange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TimeInSeconds",
                table: "RaceResults");

            migrationBuilder.AddColumn<int>(
                name: "TimeInMilliseconds",
                table: "RaceResults",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TimeInMilliseconds",
                table: "RaceResults");

            migrationBuilder.AddColumn<double>(
                name: "TimeInSeconds",
                table: "RaceResults",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
