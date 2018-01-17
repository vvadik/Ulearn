using System.Data.Entity.Migrations;

namespace Database.Migrations
{
	public partial class AddUserAvatars : DbMigration
	{
		public override void Up()
		{
			AddColumn("dbo.AspNetUsers", "AvatarUrl", c => c.String());
		}

		public override void Down()
		{
			DropColumn("dbo.AspNetUsers", "AvatarUrl");
		}
	}
}