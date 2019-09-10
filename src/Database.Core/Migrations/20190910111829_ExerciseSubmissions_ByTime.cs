using Microsoft.EntityFrameworkCore.Migrations;

namespace Database.Migrations
{
    public partial class ExerciseSubmissions_ByTime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_UserExerciseSubmissions_Timestamp",
                table: "UserExerciseSubmissions",
                column: "Timestamp");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserExerciseSubmissions_Timestamp",
                table: "UserExerciseSubmissions");
        }
    }
}
