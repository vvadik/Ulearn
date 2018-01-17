using System.Data.Entity.Migrations;

namespace Database.Migrations
{
	public partial class AddIsManualCheckingEnabledForOldSolutions : DbMigration
	{
		public override void Up()
		{
			AddColumn("dbo.Groups", "IsManualCheckingEnabledForOldSolutions", c => c.Boolean(nullable: false));
		}

		public override void Down()
		{
			DropColumn("dbo.Groups", "IsManualCheckingEnabledForOldSolutions");
		}
	}
}