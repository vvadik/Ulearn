using System.Data.Entity.Migrations;

namespace Database.Migrations
{
	public partial class addindex : DbMigration
	{
		public override void Up()
		{
			DropIndex("dbo.Likes", new[] { "UserId" });
			DropIndex("dbo.Likes", new[] { "UserSolutionId" });
			DropIndex("dbo.SlideHints", new[] { "UserId" });
			DropIndex("dbo.SlideRates", new[] { "UserId" });
			DropIndex("dbo.UserQuizs", new[] { "UserId" });
			DropIndex("dbo.Visiters", new[] { "UserId" });
			CreateIndex("dbo.SlideHints", new[] { "SlideId", "UserId", "HintId", "IsHintHelped" }, name: "FullIndex");
			CreateIndex("dbo.UserSolutions", "IsRightAnswer");
			CreateIndex("dbo.Likes", new[] { "UserId", "UserSolutionId" }, name: "UserAndSolution");
			CreateIndex("dbo.SlideRates", new[] { "SlideId", "UserId" }, name: "SlideAndUser");
			CreateIndex("dbo.UnitAppearances", new[] { "CourseId", "PublishTime" }, name: "CourseAndTime");
			CreateIndex("dbo.UserQuizs", new[] { "UserId", "SlideId", "isDropped", "QuizId", "ItemId" }, name: "FullIndex");
			CreateIndex("dbo.Visiters", new[] { "SlideId", "UserId" }, name: "SlideAndUser");
		}

		public override void Down()
		{
			DropIndex("dbo.Visiters", "SlideAndUser");
			DropIndex("dbo.UserQuizs", "FullIndex");
			DropIndex("dbo.UnitAppearances", "CourseAndTime");
			DropIndex("dbo.SlideRates", "SlideAndUser");
			DropIndex("dbo.Likes", "UserAndSolution");
			DropIndex("dbo.UserSolutions", new[] { "IsRightAnswer" });
			DropIndex("dbo.SlideHints", "FullIndex");
			CreateIndex("dbo.Visiters", "UserId");
			CreateIndex("dbo.UserQuizs", "UserId");
			CreateIndex("dbo.SlideRates", "UserId");
			CreateIndex("dbo.SlideHints", "UserId");
			CreateIndex("dbo.Likes", "UserSolutionId");
			CreateIndex("dbo.Likes", "UserId");
		}
	}
}