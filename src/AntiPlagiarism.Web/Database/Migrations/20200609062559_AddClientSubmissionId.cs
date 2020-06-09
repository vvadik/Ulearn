using Microsoft.EntityFrameworkCore.Migrations;

namespace AntiPlagiarism.Web.Migrations
{
    public partial class AddClientSubmissionId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ClientSubmissionId",
                schema: "antiplagiarism",
                table: "Submissions",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Submissions_ClientId_ClientSubmissionId",
                schema: "antiplagiarism",
                table: "Submissions",
                columns: new[] { "ClientId", "ClientSubmissionId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Submissions_ClientId_ClientSubmissionId",
                schema: "antiplagiarism",
                table: "Submissions");

            migrationBuilder.DropColumn(
                name: "ClientSubmissionId",
                schema: "antiplagiarism",
                table: "Submissions");
        }
    }
}
