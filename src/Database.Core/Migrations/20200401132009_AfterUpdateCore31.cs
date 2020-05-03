using Microsoft.EntityFrameworkCore.Migrations;

namespace Database.Migrations
{
    public partial class AfterUpdateCore31 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Comments_CommentId11",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_CommentId11",
                table: "Notifications");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Notifications_CommentId11",
                table: "Notifications",
                column: "CommentId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Comments_CommentId11",
                table: "Notifications",
                column: "CommentId1",
                principalTable: "Comments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
