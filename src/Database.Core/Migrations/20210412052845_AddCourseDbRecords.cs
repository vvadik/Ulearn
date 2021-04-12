using Microsoft.EntityFrameworkCore.Migrations;

namespace Database.Migrations
{
    public partial class AddCourseDbRecords : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CourseDbRecords",
                columns: table => new
                {
                    CourseId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                        .Annotation("Npgsql:DefaultColumnCollation", "case_insensitive"),
                    Name = table.Column<string>(type: "text", nullable: false)
                        .Annotation("Npgsql:DefaultColumnCollation", "case_insensitive"),
                    IsArchived = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseDbRecords", x => x.CourseId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CourseDbRecords");
        }
    }
}
