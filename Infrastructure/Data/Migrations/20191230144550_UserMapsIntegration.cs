using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class UserMapsIntegration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RaceMaps_AspNetUsers_ApplicationUserId",
                table: "RaceMaps");

            migrationBuilder.DropForeignKey(
                name: "FK_RaceResults_AspNetUsers_ApplicationUserId",
                table: "RaceResults");

            migrationBuilder.DropIndex(
                name: "IX_RaceResults_ApplicationUserId",
                table: "RaceResults");

            migrationBuilder.DropIndex(
                name: "IX_RaceMaps_ApplicationUserId",
                table: "RaceMaps");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "RaceResults");

            migrationBuilder.DropColumn(
                name: "Nickname",
                table: "RaceResults");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "RaceMaps");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "RaceResults",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatorId",
                table: "RaceMaps",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "RaceMaps",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_RaceResults_UserId",
                table: "RaceResults",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RaceMaps_UserId",
                table: "RaceMaps",
                column: "UserId");

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RaceMaps_AspNetUsers_UserId",
                table: "RaceMaps");

            migrationBuilder.DropForeignKey(
                name: "FK_RaceResults_AspNetUsers_UserId",
                table: "RaceResults");

            migrationBuilder.DropIndex(
                name: "IX_RaceResults_UserId",
                table: "RaceResults");

            migrationBuilder.DropIndex(
                name: "IX_RaceMaps_UserId",
                table: "RaceMaps");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "RaceResults");

            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "RaceMaps");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "RaceMaps");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "RaceResults",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Nickname",
                table: "RaceResults",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "RaceMaps",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RaceResults_ApplicationUserId",
                table: "RaceResults",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_RaceMaps_ApplicationUserId",
                table: "RaceMaps",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_RaceMaps_AspNetUsers_ApplicationUserId",
                table: "RaceMaps",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RaceResults_AspNetUsers_ApplicationUserId",
                table: "RaceResults",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
