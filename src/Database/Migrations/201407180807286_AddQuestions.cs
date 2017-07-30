using System.Data.Entity.Migrations;

namespace Database.Migrations
{
	public partial class AddQuestions : DbMigration
	{
		public override void Up()
		{
			CreateTable(
					"dbo.UserQuestions",
					c => new
					{
						QuestionId = c.Int(nullable: false, identity: true),
						SlideTitle = c.String(nullable: false),
						UserId = c.String(nullable: false),
						Question = c.String(nullable: false),
						UnitName = c.String(nullable: false),
						Time = c.DateTime(nullable: false),
						ApplicationUser_Id = c.String(maxLength: 128),
					})
				.PrimaryKey(t => t.QuestionId)
				.ForeignKey("dbo.AspNetUsers", t => t.ApplicationUser_Id)
				.Index(t => t.ApplicationUser_Id);
		}

		public override void Down()
		{
			DropForeignKey("dbo.UserQuestions", "ApplicationUser_Id", "dbo.AspNetUsers");
			DropIndex("dbo.UserQuestions", new[] { "ApplicationUser_Id" });
			DropTable("dbo.UserQuestions");
		}
	}
}