using System.Data.Entity.Migrations;

namespace Database.Migrations
{
	public partial class Sandbox : DbMigration
	{
		public override void Up()
		{
			AddColumn("dbo.UserSolutions", "Elapsed", c => c.Time(precision: 7));
		}

		public override void Down()
		{
			DropColumn("dbo.UserSolutions", "Elapsed");
		}
	}
}