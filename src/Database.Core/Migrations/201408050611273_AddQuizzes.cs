using System.Data.Entity.Migrations;

namespace Database.Migrations
{
	public partial class AddQuizzes : DbMigration
	{
		public override void Up()
		{
			CreateTable(
					"dbo.UserQuizs",
					c => new
					{
						Id = c.Int(nullable: false, identity: true),
						CourseId = c.String(nullable: false, maxLength: 64),
						SlideId = c.String(nullable: false, maxLength: 64),
						UserId = c.String(maxLength: 128),
						QuizId = c.String(maxLength: 64),
						ItemId = c.String(nullable: false, maxLength: 64),
						Text = c.String(nullable: false, maxLength: 1024),
						Timestamp = c.DateTime(nullable: false),
						IsRightAnswer = c.Boolean(nullable: false),
					})
				.PrimaryKey(t => t.Id)
				.ForeignKey("dbo.AspNetUsers", t => t.UserId)
				.Index(t => t.UserId);
		}

		public override void Down()
		{
			DropForeignKey("dbo.UserQuizs", "UserId", "dbo.AspNetUsers");
			DropIndex("dbo.UserQuizs", new[] { "UserId" });
			DropTable("dbo.UserQuizs");
		}
	}
}