using Microsoft.EntityFrameworkCore.Migrations;

namespace Database.Migrations
{
    public partial class AddUsersFlashcardsVisits : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Score",
                table: "UserFlashcardsVisits");

            migrationBuilder.AddColumn<int>(
                name: "Rate",
                table: "UserFlashcardsVisits",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Rate",
                table: "UserFlashcardsVisits");

            migrationBuilder.AddColumn<int>(
                name: "Score",
                table: "UserFlashcardsVisits",
                nullable: false,
                defaultValue: 0);
        }
    }
}
