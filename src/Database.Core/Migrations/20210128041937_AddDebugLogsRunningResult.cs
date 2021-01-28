using Microsoft.EntityFrameworkCore.Migrations;

namespace Database.Migrations
{
    public partial class AddDebugLogsRunningResult : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DebugLogsHash",
                table: "AutomaticExerciseCheckings",
                maxLength: 40,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AutomaticExerciseCheckings_DebugLogsHash",
                table: "AutomaticExerciseCheckings",
                column: "DebugLogsHash");

            migrationBuilder.AddForeignKey(
                name: "FK_AutomaticExerciseCheckings_TextBlobs_DebugLogsHash",
                table: "AutomaticExerciseCheckings",
                column: "DebugLogsHash",
                principalTable: "TextBlobs",
                principalColumn: "Hash",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AutomaticExerciseCheckings_TextBlobs_DebugLogsHash",
                table: "AutomaticExerciseCheckings");

            migrationBuilder.DropIndex(
                name: "IX_AutomaticExerciseCheckings_DebugLogsHash",
                table: "AutomaticExerciseCheckings");

            migrationBuilder.DropColumn(
                name: "DebugLogsHash",
                table: "AutomaticExerciseCheckings");
        }
    }
}
