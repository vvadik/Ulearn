using System.Data.Entity.Migrations;

namespace Database.Migrations
{
	public partial class addindexforQuizzes : DbMigration
	{
		public override void Up()
		{
			CreateIndex("dbo.UserQuizs", "SlideId");
		}

		public override void Down()
		{
			DropIndex("dbo.UserQuizs", new[] { "SlideId" });
		}
	}
}