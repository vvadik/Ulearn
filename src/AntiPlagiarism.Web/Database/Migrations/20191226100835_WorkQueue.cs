using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AntiPlagiarism.Web.Migrations
{
    public partial class WorkQueue : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WorkQueueItems",
                schema: "antiplagiarism",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    QueueId = table.Column<int>(nullable: false),
                    ItemId = table.Column<string>(nullable: false),
                    TakeAfterTime = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkQueueItems", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WorkQueueItems_QueueId_TakeAfterTime",
                schema: "antiplagiarism",
                table: "WorkQueueItems",
                columns: new[] { "QueueId", "TakeAfterTime" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WorkQueueItems",
                schema: "antiplagiarism");
        }
    }
}
