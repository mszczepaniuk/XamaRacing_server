using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class UserOnDeleteBehavior : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RaceMaps_AspNetUsers_UserId",
                table: "RaceMaps");

            migrationBuilder.DropForeignKey(
                name: "FK_RaceResults_AspNetUsers_UserId",
                table: "RaceResults");

            migrationBuilder.AddForeignKey(
                name: "FK_RaceMaps_AspNetUsers_UserId",
                table: "RaceMaps",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_RaceResults_AspNetUsers_UserId",
                table: "RaceResults",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RaceMaps_AspNetUsers_UserId",
                table: "RaceMaps");

            migrationBuilder.DropForeignKey(
                name: "FK_RaceResults_AspNetUsers_UserId",
                table: "RaceResults");

            migrationBuilder.AddForeignKey(
                name: "FK_RaceMaps_AspNetUsers_UserId",
                table: "RaceMaps",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RaceResults_AspNetUsers_UserId",
                table: "RaceResults",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
