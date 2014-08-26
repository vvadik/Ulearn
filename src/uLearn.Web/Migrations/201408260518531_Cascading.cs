namespace uLearn.Web.Migrations
{
	using System;
	using System.Data.Entity.Migrations;

	public partial class Cascading : DbMigration
	{
		public override void Up()
		{
			DropForeignKey("dbo.UserQuizs", "UserId", "dbo.AspNetUsers");
			DropForeignKey("dbo.UserQuestions", "ApplicationUser_Id", "dbo.AspNetUsers");
			DropForeignKey("dbo.Likes", "UserSolution_Id", "dbo.UserSolutions");
			DropIndex("dbo.UserQuizs", new[] { "UserId" });
			DropIndex("dbo.UserQuestions", new[] { "ApplicationUser_Id" });
			DropIndex("dbo.Likes", new[] { "UserSolution_Id" });
			AddColumn("dbo.UserQuestions", "UserName", c => c.String(nullable: false, maxLength: 64));
			AddColumn("dbo.Likes", "UserSolutionId", c => c.Int(nullable: false));
			Sql("UPDATE dbo.Likes SET UserSolutionId=SolutionId");
			Sql("DELETE FROM dbo.UserQuestions");
			AlterColumn("dbo.SlideHints", "UserId", c => c.String(nullable: false, maxLength: 128));
			AlterColumn("dbo.SlideRates", "UserId", c => c.String(nullable: false, maxLength: 128));
			AlterColumn("dbo.UnitAppearances", "UserName", c => c.String(nullable: false));
			AlterColumn("dbo.UserQuestions", "UserId", c => c.String(nullable: false, maxLength: 128));
			AlterColumn("dbo.UserQuizs", "UserId", c => c.String(nullable: false, maxLength: 128));
			AlterColumn("dbo.Likes", "Id", c => c.Int(nullable: false, identity: true));
			AlterColumn("dbo.Likes", "UserId", c => c.String(nullable: false, maxLength: 128));
			AlterColumn("dbo.Visiters", "UserId", c => c.String(nullable: false, maxLength: 128));
			DropPrimaryKey("dbo.Likes");
			AddPrimaryKey("dbo.Likes", "Id");
			CreateIndex("dbo.SlideHints", "UserId");
			CreateIndex("dbo.SlideRates", "UserId");
			CreateIndex("dbo.Likes", "UserId");
			CreateIndex("dbo.Visiters", "UserId");
			CreateIndex("dbo.UserQuizs", "UserId");
			CreateIndex("dbo.UserQuestions", "UserId");
			CreateIndex("dbo.Likes", "UserSolutionId");
			AddForeignKey("dbo.SlideHints", "UserId", "dbo.AspNetUsers", "Id", cascadeDelete: true);
			AddForeignKey("dbo.SlideRates", "UserId", "dbo.AspNetUsers", "Id", cascadeDelete: true);
			AddForeignKey("dbo.Likes", "UserId", "dbo.AspNetUsers", "Id", cascadeDelete: true);
			AddForeignKey("dbo.Visiters", "UserId", "dbo.AspNetUsers", "Id", cascadeDelete: true);
			AddForeignKey("dbo.UserQuizs", "UserId", "dbo.AspNetUsers", "Id", cascadeDelete: true);
			AddForeignKey("dbo.UserQuestions", "UserId", "dbo.AspNetUsers", "Id", cascadeDelete: true);
			AddForeignKey("dbo.Likes", "UserSolutionId", "dbo.UserSolutions", "Id", cascadeDelete: true);
			DropColumn("dbo.UnitAppearances", "UserId");
			DropColumn("dbo.UserQuestions", "ApplicationUser_Id");
			DropColumn("dbo.Likes", "SolutionId");
			DropColumn("dbo.Likes", "UserSolution_Id");
		}

		public override void Down()
		{
			AddColumn("dbo.Likes", "UserSolution_Id", c => c.Int());
			AddColumn("dbo.Likes", "SolutionId", c => c.Int(nullable: false));
			AddColumn("dbo.UserQuestions", "ApplicationUser_Id", c => c.String(maxLength: 128));
			AddColumn("dbo.UnitAppearances", "UserId", c => c.String(nullable: false));
			Sql("UPDATE dbo.Likes SET SolutionId=UserSolutionId");
			DropForeignKey("dbo.Likes", "UserSolutionId", "dbo.UserSolutions");
			DropForeignKey("dbo.UserQuestions", "UserId", "dbo.AspNetUsers");
			DropForeignKey("dbo.UserQuizs", "UserId", "dbo.AspNetUsers");
			DropForeignKey("dbo.Visiters", "UserId", "dbo.AspNetUsers");
			DropForeignKey("dbo.Likes", "UserId", "dbo.AspNetUsers");
			DropForeignKey("dbo.SlideRates", "UserId", "dbo.AspNetUsers");
			DropForeignKey("dbo.SlideHints", "UserId", "dbo.AspNetUsers");
			DropIndex("dbo.Likes", new[] { "UserSolutionId" });
			DropIndex("dbo.UserQuestions", new[] { "UserId" });
			DropIndex("dbo.UserQuizs", new[] { "UserId" });
			DropIndex("dbo.Visiters", new[] { "UserId" });
			DropIndex("dbo.Likes", new[] { "UserId" });
			DropIndex("dbo.SlideRates", new[] { "UserId" });
			DropIndex("dbo.SlideHints", new[] { "UserId" });
			DropPrimaryKey("dbo.Likes");
			AddPrimaryKey("dbo.Likes", "ID");
			AlterColumn("dbo.Visiters", "UserId", c => c.String(nullable: false));
			AlterColumn("dbo.Likes", "UserId", c => c.String(nullable: false));
			AlterColumn("dbo.Likes", "Id", c => c.Int(nullable: false, identity: true));
			AlterColumn("dbo.UserQuizs", "UserId", c => c.String(maxLength: 128));
			AlterColumn("dbo.UserQuestions", "UserId", c => c.String(nullable: false));
			AlterColumn("dbo.UnitAppearances", "UserName", c => c.String());
			AlterColumn("dbo.SlideRates", "UserId", c => c.String(nullable: false));
			AlterColumn("dbo.SlideHints", "UserId", c => c.String(nullable: false));
			DropColumn("dbo.Likes", "UserSolutionId");
			DropColumn("dbo.UserQuestions", "UserName");
			CreateIndex("dbo.Likes", "UserSolution_Id");
			CreateIndex("dbo.UserQuestions", "ApplicationUser_Id");
			CreateIndex("dbo.UserQuizs", "UserId");
			AddForeignKey("dbo.Likes", "UserSolution_Id", "dbo.UserSolutions", "Id");
			AddForeignKey("dbo.UserQuestions", "ApplicationUser_Id", "dbo.AspNetUsers", "Id");
			AddForeignKey("dbo.UserQuizs", "UserId", "dbo.AspNetUsers", "Id");
		}
	}
}
