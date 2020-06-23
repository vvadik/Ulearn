using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AntiPlagiarism.Web.Migrations
{
    public partial class AddMostSimilarSubmissionsAndTaskStatisticsSourceData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Timestamp",
                schema: "antiplagiarism",
                table: "TasksStatisticsParameters",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MostSimilarSubmissions",
                schema: "antiplagiarism",
                columns: table => new
                {
                    SubmissionId = table.Column<int>(nullable: false),
                    SimilarSubmissionId = table.Column<int>(nullable: false),
                    Weight = table.Column<double>(nullable: false),
                    Timestamp = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MostSimilarSubmissions", x => x.SubmissionId);
                    table.ForeignKey(
                        name: "FK_MostSimilarSubmissions_Submissions_SimilarSubmissionId",
                        column: x => x.SimilarSubmissionId,
                        principalSchema: "antiplagiarism",
                        principalTable: "Submissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MostSimilarSubmissions_Submissions_SubmissionId",
                        column: x => x.SubmissionId,
                        principalSchema: "antiplagiarism",
                        principalTable: "Submissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TaskStatisticsSourceData",
                schema: "antiplagiarism",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Submission1Id = table.Column<int>(nullable: false),
                    Submission2Id = table.Column<int>(nullable: false),
                    Weight = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskStatisticsSourceData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskStatisticsSourceData_Submissions_Submission1Id",
                        column: x => x.Submission1Id,
                        principalSchema: "antiplagiarism",
                        principalTable: "Submissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TaskStatisticsSourceData_Submissions_Submission2Id",
                        column: x => x.Submission2Id,
                        principalSchema: "antiplagiarism",
                        principalTable: "Submissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Submissions_AddingTime",
                schema: "antiplagiarism",
                table: "Submissions",
                column: "AddingTime");

            migrationBuilder.CreateIndex(
                name: "IX_MostSimilarSubmissions_SimilarSubmissionId",
                schema: "antiplagiarism",
                table: "MostSimilarSubmissions",
                column: "SimilarSubmissionId");

            migrationBuilder.CreateIndex(
                name: "IX_MostSimilarSubmissions_Timestamp",
                schema: "antiplagiarism",
                table: "MostSimilarSubmissions",
                column: "Timestamp");

            migrationBuilder.CreateIndex(
                name: "IX_TaskStatisticsSourceData_Submission1Id",
                schema: "antiplagiarism",
                table: "TaskStatisticsSourceData",
                column: "Submission1Id");

            migrationBuilder.CreateIndex(
                name: "IX_TaskStatisticsSourceData_Submission2Id",
                schema: "antiplagiarism",
                table: "TaskStatisticsSourceData",
                column: "Submission2Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MostSimilarSubmissions",
                schema: "antiplagiarism");

            migrationBuilder.DropTable(
                name: "TaskStatisticsSourceData",
                schema: "antiplagiarism");

            migrationBuilder.DropIndex(
                name: "IX_Submissions_AddingTime",
                schema: "antiplagiarism",
                table: "Submissions");

            migrationBuilder.DropColumn(
                name: "Timestamp",
                schema: "antiplagiarism",
                table: "TasksStatisticsParameters");
        }
    }
}
