using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AntiPlagiarism.Web.Migrations
{
    public partial class AddManualSuspicionLevels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ManualSuspicionLevels",
                schema: "antiplagiarism",
                columns: table => new
                {
                    TaskId = table.Column<Guid>(nullable: false),
                    FaintSuspicion = table.Column<double>(nullable: true),
                    StrongSuspicion = table.Column<double>(nullable: true),
                    Timestamp = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ManualSuspicionLevels", x => x.TaskId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ManualSuspicionLevels",
                schema: "antiplagiarism");
        }
    }
}
