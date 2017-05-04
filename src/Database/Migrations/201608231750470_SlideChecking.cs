using System.Data.Entity.Migrations;

namespace Database.Migrations
{
	public partial class SlideChecking : DbMigration
	{
		public override void Up()
		{
			RenameTable(name: "dbo.ManualQuizCheckQueueItems", newName: "ManualQuizCheckings");
			RenameIndex(table: "dbo.ManualQuizCheckings", name: "IDX_ManualQuizCheck_ManualQuizCheckBySlide", newName: "IDX_AbstractSlideChecking_AbstractSlideCheckingBySlide");
			RenameIndex(table: "dbo.ManualQuizCheckings", name: "IDX_ManualQuizCheck_ManualQuizCheckBySlideAndTime", newName: "IDX_AbstractSlideChecking_AbstractSlideCheckingBySlideAndTime");
			RenameIndex(table: "dbo.ManualQuizCheckings", name: "IDX_ManualQuizCheck_ManualQuizCheckBySlideAndUser", newName: "IDX_AbstractSlideChecking_AbstractSlideCheckingBySlideAndUser");
			CreateTable(
				"dbo.AutomaticQuizCheckings",
				c => new
				{
					Id = c.Int(nullable: false, identity: true),
					CourseId = c.String(nullable: false, maxLength: 64),
					SlideId = c.Guid(nullable: false),
					Timestamp = c.DateTime(nullable: false),
					UserId = c.String(nullable: false, maxLength: 128),
					Score = c.Int(nullable: false),
				})
				.PrimaryKey(t => t.Id)
				.ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
				.Index(t => t.SlideId, name: "IDX_AbstractSlideChecking_AbstractSlideCheckingBySlide")
				.Index(t => new { t.SlideId, t.Timestamp }, name: "IDX_AbstractSlideChecking_AbstractSlideCheckingBySlideAndTime")
				.Index(t => new { t.SlideId, t.UserId }, name: "IDX_AbstractSlideChecking_AbstractSlideCheckingBySlideAndUser");
		}

		public override void Down()
		{
			DropForeignKey("dbo.AutomaticQuizCheckings", "UserId", "dbo.AspNetUsers");
			DropIndex("dbo.AutomaticQuizCheckings", "IDX_AbstractSlideChecking_AbstractSlideCheckingBySlideAndUser");
			DropIndex("dbo.AutomaticQuizCheckings", "IDX_AbstractSlideChecking_AbstractSlideCheckingBySlideAndTime");
			DropIndex("dbo.AutomaticQuizCheckings", "IDX_AbstractSlideChecking_AbstractSlideCheckingBySlide");
			DropTable("dbo.AutomaticQuizCheckings");
			RenameIndex(table: "dbo.ManualQuizCheckings", name: "IDX_AbstractSlideChecking_AbstractSlideCheckingBySlideAndUser", newName: "IDX_ManualQuizCheck_ManualQuizCheckBySlideAndUser");
			RenameIndex(table: "dbo.ManualQuizCheckings", name: "IDX_AbstractSlideChecking_AbstractSlideCheckingBySlideAndTime", newName: "IDX_ManualQuizCheck_ManualQuizCheckBySlideAndTime");
			RenameIndex(table: "dbo.ManualQuizCheckings", name: "IDX_AbstractSlideChecking_AbstractSlideCheckingBySlide", newName: "IDX_ManualQuizCheck_ManualQuizCheckBySlide");
			RenameTable(name: "dbo.ManualQuizCheckings", newName: "ManualQuizCheckQueueItems");
		}
	}
}