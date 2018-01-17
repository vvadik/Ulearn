using System.Data.Entity.Migrations;

namespace Database.Migrations
{
	public partial class ChangeVisitsIndex : DbMigration
	{
		public override void Up()
		{
			DropIndex("dbo.Visiters", "SlideAndUser");
			CreateIndex("dbo.Visiters", new[] { "UserId", "SlideId" }, name: "IDX_Visits_UserAndSlide");
			CreateIndex("dbo.Visiters", new[] { "SlideId", "Timestamp" }, name: "IDX_Visits_SlideAndTime");
		}

		public override void Down()
		{
			DropIndex("dbo.Visiters", "IDX_Visits_SlideAndTime");
			DropIndex("dbo.Visiters", "IDX_Visits_UserAndSlide");
			CreateIndex("dbo.Visiters", new[] { "SlideId", "UserId" }, name: "SlideAndUser");
		}
	}
}