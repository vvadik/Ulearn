using Microsoft.EntityFrameworkCore.Migrations;

namespace Database.Migrations
{
    public partial class AutomaticExerciseCheckingPoints : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "Points",
                table: "AutomaticExerciseCheckings",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Points",
                table: "AutomaticExerciseCheckings");
        }
    }
}
