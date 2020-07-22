using Microsoft.EntityFrameworkCore.Migrations;

namespace Database.Migrations
{
    public partial class AddQuizIgnoreInAttemptsCount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IgnoreInAttemptsCount",
                table: "ManualQuizCheckings",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IgnoreInAttemptsCount",
                table: "AutomaticQuizCheckings",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IgnoreInAttemptsCount",
                table: "ManualQuizCheckings");

            migrationBuilder.DropColumn(
                name: "IgnoreInAttemptsCount",
                table: "AutomaticQuizCheckings");
        }
    }
}
