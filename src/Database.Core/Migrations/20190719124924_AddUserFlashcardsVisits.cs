using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Database.Migrations
{
    public partial class AddUserFlashcardsVisits : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserFlashcardsVisits",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<string>(nullable: false),
                    CourseId = table.Column<string>(maxLength: 64, nullable: false),
                    UnitId = table.Column<Guid>(nullable: false),
                    FlashcardId = table.Column<string>(maxLength: 64, nullable: false),
                    Rate = table.Column<int>(nullable: false),
                    Timestamp = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFlashcardsVisits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserFlashcardsVisits_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserFlashcardsVisits_UserId_CourseId_UnitId_FlashcardId",
                table: "UserFlashcardsVisits",
                columns: new[] { "UserId", "CourseId", "UnitId", "FlashcardId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserFlashcardsVisits");
        }
    }
}
