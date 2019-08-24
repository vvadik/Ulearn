using Microsoft.EntityFrameworkCore.Migrations;

namespace Database.Migrations
{
    public partial class UpdateCourseAccess : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Comment",
                table: "CourseAccesses",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Comment",
                table: "CourseAccesses");
        }
    }
}
