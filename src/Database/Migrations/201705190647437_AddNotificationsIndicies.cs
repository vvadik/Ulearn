namespace Database.Migrations
{
	using System;
	using System.Data.Entity.Migrations;

	public partial class AddNotificationsIndicies : DbMigration
	{
		public override void Up()
		{
			CreateIndex("dbo.NotificationTransports", new[] { "UserId", "IsDeleted" }, name: "IDX_NotificationTransport_ByUserAndDeleted");
		}

		public override void Down()
		{
			DropIndex("dbo.NotificationTransports", "IDX_NotificationTransport_ByUserAndDeleted");
		}
	}
}