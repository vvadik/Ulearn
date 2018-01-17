using System.Data.Entity.Migrations;

namespace Database.Migrations
{
	public partial class AddQuizVersions : DbMigration
	{
		public override void Up()
		{
			CreateTable(
					"dbo.QuizVersions",
					c => new
					{
						Id = c.Int(nullable: false, identity: true),
						CourseId = c.String(nullable: false, maxLength: 64),
						SlideId = c.Guid(nullable: false),
						NormalizedXml = c.String(nullable: false),
						LoadingTime = c.DateTime(nullable: false),
					})
				.PrimaryKey(t => t.Id)
				.Index(t => t.SlideId, name: "IDX_QuizVersion_QuizVersionBySlide")
				.Index(t => new { t.SlideId, t.LoadingTime }, name: "IDX_QuizVersion_QuizVersionBySlideAndTime");

			AddColumn("dbo.UserQuizs", "QuizVersionId", c => c.Int());
			CreateIndex("dbo.UserQuizs", "QuizVersionId");
			AddForeignKey("dbo.UserQuizs", "QuizVersionId", "dbo.QuizVersions", "Id");
		}

		public override void Down()
		{
			DropForeignKey("dbo.UserQuizs", "QuizVersionId", "dbo.QuizVersions");
			DropIndex("dbo.UserQuizs", new[] { "QuizVersionId" });
			DropIndex("dbo.QuizVersions", "IDX_QuizVersion_QuizVersionBySlideAndTime");
			DropIndex("dbo.QuizVersions", "IDX_QuizVersion_QuizVersionBySlide");
			DropColumn("dbo.UserQuizs", "QuizVersionId");
			DropTable("dbo.QuizVersions");
		}
	}
}