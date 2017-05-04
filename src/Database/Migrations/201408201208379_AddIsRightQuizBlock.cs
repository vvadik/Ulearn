using System.Data.Entity.Migrations;

namespace Database.Migrations
{
	public partial class AddIsRightQuizBlock : DbMigration
	{
		public override void Up()
		{
			AddColumn("dbo.UserQuizs", "IsRightQuizBlock", c => c.Boolean(nullable: false));
		}

		public override void Down()
		{
			DropColumn("dbo.UserQuizs", "IsRightQuizBlock");
		}
	}
}