namespace Database.Migrations
{
	using System.Data.Entity.Migrations;

	public partial class RemoveNotificationTransportsConfirmation : DbMigration
	{
		public override void Up()
		{
			DropIndex("dbo.NotificationTransports", "IDX_NotificationTransport_ByConfirmationCode");
			CreateIndex("dbo.Notifications", "AreDeliveriesCreated", name: "IDX_Notification_ByAreDeliveriesCreated");
			DropColumn("dbo.NotificationTransports", "ConfirmationCode");
			DropColumn("dbo.NotificationTransports", "IsConfirmed");
		}

		public override void Down()
		{
			AddColumn("dbo.NotificationTransports", "IsConfirmed", c => c.Boolean(nullable: false));
			AddColumn("dbo.NotificationTransports", "ConfirmationCode", c => c.Guid(nullable: false));
			DropIndex("dbo.Notifications", "IDX_Notification_ByAreDeliveriesCreated");
			CreateIndex("dbo.NotificationTransports", "ConfirmationCode", name: "IDX_NotificationTransport_ByConfirmationCode");
		}
	}
}