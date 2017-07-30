namespace Database.Migrations
{
	using System;
	using System.Data.Entity.Migrations;

	public partial class RemoveOccuredErrorNotification : DbMigration
	{
		public override void Up()
		{
			DropColumn("dbo.Notifications", "ErrorId");
			DropColumn("dbo.Notifications", "ErrorMessage");
		}

		public override void Down()
		{
			AddColumn("dbo.Notifications", "ErrorMessage", c => c.String());
			AddColumn("dbo.Notifications", "ErrorId", c => c.String(maxLength: 100));
		}
	}
}