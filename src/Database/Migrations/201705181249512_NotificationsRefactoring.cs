namespace Database.Migrations
{
	using System.Data.Entity.Migrations;

	public partial class NotificationsRefactoring : DbMigration
	{
		public override void Up()
		{
			DropIndex("dbo.NotificationDeliveries", "IDX_NotificationDelivery_BySendTime");
			DropIndex("dbo.NotificationTransports", "IDX_TelegramNotificationTransport_ByChatId");
			AddColumn("dbo.AspNetUsers", "TelegramChatId", c => c.Long());
			AddColumn("dbo.AspNetUsers", "TelegramChatTitle", c => c.String(maxLength: 200));
			AddColumn("dbo.NotificationDeliveries", "NextTryTime", c => c.DateTime());
			AddColumn("dbo.NotificationDeliveries", "FailsCount", c => c.Int(nullable: false));
			AddColumn("dbo.Notifications", "AreDeliveriesCreated", c => c.Boolean(nullable: false));
			AddColumn("dbo.NotificationTransportSettings", "IsEnabled", c => c.Boolean(nullable: false));
			CreateIndex("dbo.AspNetUsers", "TelegramChatId", name: "IDX_ApplicationUser_ByTelegramChatId");
			CreateIndex("dbo.NotificationDeliveries", "NextTryTime", name: "IDX_NotificationDelivery_ByNextTryTime");
			DropColumn("dbo.NotificationDeliveries", "SendTime");
			DropColumn("dbo.NotificationTransports", "Email");
			DropColumn("dbo.NotificationTransports", "ChatId");
			DropColumn("dbo.NotificationTransports", "ChatTitle");
			DropColumn("dbo.NotificationTransportSettings", "Frequency");
		}

		public override void Down()
		{
			AddColumn("dbo.NotificationTransportSettings", "Frequency", c => c.Byte(nullable: false));
			AddColumn("dbo.NotificationTransports", "ChatTitle", c => c.String(maxLength: 200));
			AddColumn("dbo.NotificationTransports", "ChatId", c => c.Long());
			AddColumn("dbo.NotificationTransports", "Email", c => c.String(maxLength: 200));
			AddColumn("dbo.NotificationDeliveries", "SendTime", c => c.DateTime(nullable: false));
			DropIndex("dbo.NotificationDeliveries", "IDX_NotificationDelivery_ByNextTryTime");
			DropIndex("dbo.AspNetUsers", "IDX_ApplicationUser_ByTelegramChatId");
			DropColumn("dbo.NotificationTransportSettings", "IsEnabled");
			DropColumn("dbo.Notifications", "AreDeliveriesCreated");
			DropColumn("dbo.NotificationDeliveries", "FailsCount");
			DropColumn("dbo.NotificationDeliveries", "NextTryTime");
			DropColumn("dbo.AspNetUsers", "TelegramChatTitle");
			DropColumn("dbo.AspNetUsers", "TelegramChatId");
			CreateIndex("dbo.NotificationTransports", "ChatId", name: "IDX_TelegramNotificationTransport_ByChatId");
			CreateIndex("dbo.NotificationDeliveries", "SendTime", name: "IDX_NotificationDelivery_BySendTime");
		}
	}
}