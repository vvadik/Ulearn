using System.Data.Entity.Migrations;

namespace Database.Migrations
{
	public partial class AddUserRegistrationTimestamp : DbMigration
	{
		public override void Up()
		{
			AddColumn("dbo.AspNetUsers", "Registered", c => c.DateTime(nullable: false));
		}

		public override void Down()
		{
			DropColumn("dbo.AspNetUsers", "Registered");
		}
	}
}