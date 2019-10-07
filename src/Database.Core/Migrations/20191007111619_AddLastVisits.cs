using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Database.Migrations
{
    public partial class AddLastVisits : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LastVisits",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<string>(maxLength: 128, nullable: false),
                    CourseId = table.Column<string>(maxLength: 64, nullable: false),
                    SlideId = table.Column<Guid>(nullable: false),
                    Timestamp = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LastVisits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LastVisits_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LastVisits_UserId",
                table: "LastVisits",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_LastVisits_CourseId_UserId",
                table: "LastVisits",
                columns: new[] { "CourseId", "UserId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LastVisits");
        }
    }
}
