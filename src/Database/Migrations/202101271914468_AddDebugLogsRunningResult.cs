namespace Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDebugLogsRunningResult : DbMigration
    {
		public override void Up()
		{
			AddColumn("dbo.AutomaticExerciseCheckings", "DebugLogsHash", c => c.String(maxLength: 40));
			AddForeignKey("dbo.AutomaticExerciseCheckings", "DebugLogsHash", "dbo.TextBlobs", "Hash");
		}

		public override void Down()
		{
			DropForeignKey("dbo.AutomaticExerciseCheckings", "DebugLogsHash", "dbo.TextBlobs");
			DropColumn("dbo.AutomaticExerciseCheckings", "DebugLogsHash");
		}
	}
}
