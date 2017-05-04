using System.Data.Entity.Migrations;

namespace Database.Migrations
{
	public partial class VisitTimestamp : DbMigration
	{
		public override void Up()
		{
			AddColumn("dbo.Visiters", "Timestamp", c => c.DateTime(nullable: false));
			Sql("UPDATE dbo.Visiters SET Timestamp='2014-09-02'");
		}

		public override void Down()
		{
			DropColumn("dbo.Visiters", "Timestamp");
		}
	}
}