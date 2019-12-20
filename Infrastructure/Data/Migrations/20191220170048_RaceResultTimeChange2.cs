using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class RaceResultTimeChange2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TimeInMilliseconds",
                table: "RaceResults");

            migrationBuilder.AddColumn<DateTime>(
                name: "Time",
                table: "RaceResults",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Time",
                table: "RaceResults");

            migrationBuilder.AddColumn<int>(
                name: "TimeInMilliseconds",
                table: "RaceResults",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
