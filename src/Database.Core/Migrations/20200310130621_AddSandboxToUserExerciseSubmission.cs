using Microsoft.EntityFrameworkCore.Migrations;

namespace Database.Migrations
{
    public partial class AddSandboxToUserExerciseSubmission : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Sandbox",
                table: "UserExerciseSubmissions",
                maxLength: 40,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserExerciseSubmissions_Sandbox",
                table: "UserExerciseSubmissions",
                column: "Sandbox");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserExerciseSubmissions_Sandbox",
                table: "UserExerciseSubmissions");

            migrationBuilder.DropColumn(
                name: "Sandbox",
                table: "UserExerciseSubmissions");
        }
    }
}
