namespace uLearn.Web.Migrations
{
	using System;
	using System.Data.Entity.Migrations;
	
	public partial class EnablingManualCheckingForGroups : DbMigration
	{
		public override void Up()
		{
			AddColumn("dbo.Groups", "IsManualCheckingEnabled", c => c.Boolean(nullable: false));
			Sql("UPDATE dbo.Groups SET IsManualCheckingEnabled = 1");

			AddColumn("dbo.Visits", "HasManualChecking", c => c.Boolean(nullable: false));
			Sql("UPDATE dbo.Visits SET HasManualChecking = 1 " +
				"FROM dbo.Visits JOIN dbo.ManualExerciseCheckings ON Visits.SlideId = ManualExerciseCheckings.SlideId AND " +
				"Visits.CourseId = ManualExerciseCheckings.CourseId AND " +
				"Visits.UserId = ManualExerciseCheckings.UserId");
			Sql("UPDATE dbo.Visits SET HasManualChecking = 1 " +
				"FROM dbo.Visits JOIN dbo.ManualQuizCheckings ON Visits.SlideId = ManualQuizCheckings.SlideId AND " +
				"Visits.CourseId = ManualQuizCheckings.CourseId AND " +
				"Visits.UserId = ManualQuizCheckings.UserId");
		}
		
		public override void Down()
		{
			DropColumn("dbo.Visits", "HasManualChecking");
			DropColumn("dbo.Groups", "IsManualCheckingEnabled");
		}
	}
}
