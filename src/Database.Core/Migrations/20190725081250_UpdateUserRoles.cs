using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Database.Migrations
{
    public partial class UpdateUserRoles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Comment",
                table: "UserRoles",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "GrantTime",
                table: "UserRoles",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GrantedById",
                table: "UserRoles",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsEnabled",
                table: "UserRoles",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Comment",
                table: "UserRoles");

            migrationBuilder.DropColumn(
                name: "GrantTime",
                table: "UserRoles");

            migrationBuilder.DropColumn(
                name: "GrantedById",
                table: "UserRoles");

            migrationBuilder.DropColumn(
                name: "IsEnabled",
                table: "UserRoles");
        }
    }
}
