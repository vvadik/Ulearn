using Microsoft.EntityFrameworkCore.Migrations;

namespace AntiPlagiarism.Web.Migrations
{
    public partial class AddAddingTimeAndSnippetSubmissionIndexes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SnippetsOccurences_SnippetId",
                schema: "antiplagiarism",
                table: "SnippetsOccurences");

            migrationBuilder.CreateIndex(
                name: "IX_Submissions_ClientId_TaskId_AddingTime_AuthorId",
                schema: "antiplagiarism",
                table: "Submissions",
                columns: new[] { "ClientId", "TaskId", "AddingTime", "AuthorId" });

            migrationBuilder.CreateIndex(
                name: "IX_Submissions_ClientId_TaskId_AddingTime_Language_AuthorId",
                schema: "antiplagiarism",
                table: "Submissions",
                columns: new[] { "ClientId", "TaskId", "AddingTime", "Language", "AuthorId" });

            migrationBuilder.CreateIndex(
                name: "IX_SnippetsOccurences_SnippetId_SubmissionId",
                schema: "antiplagiarism",
                table: "SnippetsOccurences",
                columns: new[] { "SnippetId", "SubmissionId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Submissions_ClientId_TaskId_AddingTime_AuthorId",
                schema: "antiplagiarism",
                table: "Submissions");

            migrationBuilder.DropIndex(
                name: "IX_Submissions_ClientId_TaskId_AddingTime_Language_AuthorId",
                schema: "antiplagiarism",
                table: "Submissions");

            migrationBuilder.DropIndex(
                name: "IX_SnippetsOccurences_SnippetId_SubmissionId",
                schema: "antiplagiarism",
                table: "SnippetsOccurences");

            migrationBuilder.CreateIndex(
                name: "IX_SnippetsOccurences_SnippetId",
                schema: "antiplagiarism",
                table: "SnippetsOccurences",
                column: "SnippetId");
        }
    }
}
