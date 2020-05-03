using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Database.Migrations
{
    public partial class AddTempCourse : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TempCourses",
                columns: table => new
                {
                    CourseId = table.Column<string>(maxLength: 64, nullable: false),
                    LoadingTime = table.Column<DateTime>(nullable: false),
                    AuthorId = table.Column<string>(maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TempCourses", x => x.CourseId);
                    table.ForeignKey(
                        name: "FK_TempCourses_AspNetUsers_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TempCourses_AuthorId",
                table: "TempCourses",
                column: "AuthorId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TempCourses");
        }
    }
}
