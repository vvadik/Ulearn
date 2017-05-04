using System.Data.Entity.Migrations;

namespace Database.Migrations
{
	public partial class RemoveOldUserGroups : DbMigration
	{
		public override void Up()
		{
			DropColumn("dbo.AspNetUsers", "GroupName");
		}

		public override void Down()
		{
			AddColumn("dbo.AspNetUsers", "GroupName", c => c.String());
		}
	}
}