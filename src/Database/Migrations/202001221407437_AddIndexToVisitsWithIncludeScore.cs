namespace Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddIndexToVisitsWithIncludeScore : DbMigration
    {
		private string indexName = "IDX_Visits_ByCourseSlideAndUser_IncludeScoreAndTimestamp";
		
        public override void Up()
        {
			Sql($"CREATE NONCLUSTERED INDEX [{indexName}] ON [dbo].[Visits] ([CourseId], [SlideId], [UserId]) INCLUDE ([Score], [Timestamp])");
        }
        
        public override void Down()
        {
			Sql($"DROP INDEX [{indexName}]");
        }
    }
}
