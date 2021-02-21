using Microsoft.EntityFrameworkCore.Migrations;

namespace AntiPlagiarism.Web.Migrations
{
    public partial class LanguageInAntiplagiarism : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_TasksStatisticsParameters",
                schema: "antiplagiarism",
                table: "TasksStatisticsParameters");

            migrationBuilder.DropIndex(
                name: "IX_Submissions_ClientId_TaskId",
                schema: "antiplagiarism",
                table: "Submissions");

            migrationBuilder.DropIndex(
                name: "IX_Submissions_ClientId_TaskId_AuthorId",
                schema: "antiplagiarism",
                table: "Submissions");

            migrationBuilder.DropIndex(
                name: "IX_Submissions_ClientId_TaskId_AddingTime_AuthorId",
                schema: "antiplagiarism",
                table: "Submissions");

            migrationBuilder.DropIndex(
                name: "IX_SnippetsStatistics_SnippetId_TaskId_ClientId",
                schema: "antiplagiarism",
                table: "SnippetsStatistics");

            migrationBuilder.AddColumn<short>(
                name: "Language",
                schema: "antiplagiarism",
                table: "TasksStatisticsParameters",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<short>(
                name: "Language",
                schema: "antiplagiarism",
                table: "SnippetsStatistics",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_TasksStatisticsParameters",
                schema: "antiplagiarism",
                table: "TasksStatisticsParameters",
                columns: new[] { "TaskId", "Language" });

            migrationBuilder.CreateIndex(
                name: "IX_Submissions_ClientId_TaskId_Language",
                schema: "antiplagiarism",
                table: "Submissions",
                columns: new[] { "ClientId", "TaskId", "Language" });

            migrationBuilder.CreateIndex(
                name: "IX_SnippetsStatistics_SnippetId_TaskId_Language_ClientId",
                schema: "antiplagiarism",
                table: "SnippetsStatistics",
                columns: new[] { "SnippetId", "TaskId", "Language", "ClientId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_TasksStatisticsParameters",
                schema: "antiplagiarism",
                table: "TasksStatisticsParameters");

            migrationBuilder.DropIndex(
                name: "IX_Submissions_ClientId_TaskId_Language",
                schema: "antiplagiarism",
                table: "Submissions");

            migrationBuilder.DropIndex(
                name: "IX_SnippetsStatistics_SnippetId_TaskId_Language_ClientId",
                schema: "antiplagiarism",
                table: "SnippetsStatistics");

            migrationBuilder.DropColumn(
                name: "Language",
                schema: "antiplagiarism",
                table: "TasksStatisticsParameters");

            migrationBuilder.DropColumn(
                name: "Language",
                schema: "antiplagiarism",
                table: "SnippetsStatistics");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TasksStatisticsParameters",
                schema: "antiplagiarism",
                table: "TasksStatisticsParameters",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_Submissions_ClientId_TaskId",
                schema: "antiplagiarism",
                table: "Submissions",
                columns: new[] { "ClientId", "TaskId" });

            migrationBuilder.CreateIndex(
                name: "IX_Submissions_ClientId_TaskId_AuthorId",
                schema: "antiplagiarism",
                table: "Submissions",
                columns: new[] { "ClientId", "TaskId", "AuthorId" });

            migrationBuilder.CreateIndex(
                name: "IX_Submissions_ClientId_TaskId_AddingTime_AuthorId",
                schema: "antiplagiarism",
                table: "Submissions",
                columns: new[] { "ClientId", "TaskId", "AddingTime", "AuthorId" });

            migrationBuilder.CreateIndex(
                name: "IX_SnippetsStatistics_SnippetId_TaskId_ClientId",
                schema: "antiplagiarism",
                table: "SnippetsStatistics",
                columns: new[] { "SnippetId", "TaskId", "ClientId" },
                unique: true);
        }
    }
}
