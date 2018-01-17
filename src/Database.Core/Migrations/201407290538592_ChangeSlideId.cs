using System.Data.Entity.Migrations;

namespace Database.Migrations
{
	public partial class ChangeSlideId : DbMigration
	{
		public override void Up()
		{
			AlterColumn("dbo.SlideHints", "SlideId", c => c.String(nullable: false, maxLength: 64));
			AlterColumn("dbo.SlideRates", "SlideId", c => c.String(nullable: false, maxLength: 64));
			AlterColumn("dbo.Visiters", "SlideId", c => c.String(nullable: false, maxLength: 64));
		}

		public override void Down()
		{
			AlterColumn("dbo.Visiters", "SlideId", c => c.Int(nullable: false));
			AlterColumn("dbo.SlideRates", "SlideId", c => c.Int(nullable: false));
			AlterColumn("dbo.SlideHints", "SlideId", c => c.Int(nullable: false));
		}
	}
}