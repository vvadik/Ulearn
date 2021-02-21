using Microsoft.EntityFrameworkCore.Migrations;

namespace AntiPlagiarism.Web.Migrations
{
    public partial class LanguageInAntiplagiarism2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ManualSuspicionLevels",
                schema: "antiplagiarism",
                table: "ManualSuspicionLevels");

            migrationBuilder.AddColumn<short>(
                name: "Language",
                schema: "antiplagiarism",
                table: "ManualSuspicionLevels",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ManualSuspicionLevels",
                schema: "antiplagiarism",
                table: "ManualSuspicionLevels",
                columns: new[] { "TaskId", "Language" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ManualSuspicionLevels",
                schema: "antiplagiarism",
                table: "ManualSuspicionLevels");

            migrationBuilder.DropColumn(
                name: "Language",
                schema: "antiplagiarism",
                table: "ManualSuspicionLevels");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ManualSuspicionLevels",
                schema: "antiplagiarism",
                table: "ManualSuspicionLevels",
                column: "TaskId");
        }
    }
}
