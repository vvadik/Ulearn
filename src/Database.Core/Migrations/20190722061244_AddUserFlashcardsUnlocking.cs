using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Database.Migrations
{
    public partial class AddUserFlashcardsUnlocking : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserFlashcardsUnlocking",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<string>(nullable: false),
                    CourseId = table.Column<string>(maxLength: 64, nullable: false),
                    UnitId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFlashcardsUnlocking", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserFlashcardsUnlocking_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserFlashcardsUnlocking_UserId_CourseId_UnitId",
                table: "UserFlashcardsUnlocking",
                columns: new[] { "UserId", "CourseId", "UnitId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserFlashcardsUnlocking");
        }
    }
}
