using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AS.Data.Migrations
{
    public partial class EventUsers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateTime",
                table: "ASEvents",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "ASEvents",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ASEvents_UserId",
                table: "ASEvents",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ASEvents_AspNetUsers_UserId",
                table: "ASEvents",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ASEvents_AspNetUsers_UserId",
                table: "ASEvents");

            migrationBuilder.DropIndex(
                name: "IX_ASEvents_UserId",
                table: "ASEvents");

            migrationBuilder.DropColumn(
                name: "DateTime",
                table: "ASEvents");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "ASEvents");
        }
    }
}
