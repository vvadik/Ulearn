using Microsoft.EntityFrameworkCore.Migrations;

namespace Database.Migrations
{
    public partial class ChangeScoreFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_WorkQueue",
                table: "WorkQueue");

            migrationBuilder.RenameTable(
                name: "WorkQueue",
                newName: "WorkQueueItems");

            migrationBuilder.AlterColumn<int>(
                name: "Score",
                table: "ManualExerciseCheckings",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "Percent",
                table: "ManualExerciseCheckings",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Score",
                table: "AutomaticExerciseCheckings",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WorkQueueItems",
                table: "WorkQueueItems",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_WorkQueueItems",
                table: "WorkQueueItems");

            migrationBuilder.DropColumn(
                name: "Percent",
                table: "ManualExerciseCheckings");

            migrationBuilder.RenameTable(
                name: "WorkQueueItems",
                newName: "WorkQueue");

            migrationBuilder.AlterColumn<int>(
                name: "Score",
                table: "ManualExerciseCheckings",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Score",
                table: "AutomaticExerciseCheckings",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_WorkQueue",
                table: "WorkQueue",
                column: "Id");
        }
    }
}
