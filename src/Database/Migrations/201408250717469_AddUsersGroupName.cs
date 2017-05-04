using System.Data.Entity.Migrations;

namespace Database.Migrations
{
	public partial class AddUsersGroupName : DbMigration
	{
		public override void Up()
		{
			AddColumn("dbo.AspNetUsers", "GroupName", c => c.String());
		}

		public override void Down()
		{
			DropColumn("dbo.AspNetUsers", "GroupName");
		}
	}
}