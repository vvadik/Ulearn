namespace uLearn.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TelegramNotificationsAndIndexes : DbMigration
    {
        public override void Up()
        {
            RenameIndex(table: "dbo.NotificationTransports", name: "IDX_NotificationTransportSettings_ByUser", newName: "IDX_NotificationTransport_ByUser");
            AddColumn("dbo.NotificationTransports", "ConfirmationCode", c => c.Guid(nullable: false));
            AddColumn("dbo.NotificationTransports", "IsConfirmed", c => c.Boolean(nullable: false));
            AlterColumn("dbo.NotificationTransports", "ChatId", c => c.Long());
            CreateIndex("dbo.NotificationTransports", "ConfirmationCode", name: "IDX_NotificationTransport_ByConfirmationCode");
            CreateIndex("dbo.NotificationTransports", "ChatId", name: "IDX_TelegramNotificationTransport_ByChatId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.NotificationTransports", "IDX_TelegramNotificationTransport_ByChatId");
            DropIndex("dbo.NotificationTransports", "IDX_NotificationTransport_ByConfirmationCode");
            AlterColumn("dbo.NotificationTransports", "ChatId", c => c.String(maxLength: 200));
            DropColumn("dbo.NotificationTransports", "IsConfirmed");
            DropColumn("dbo.NotificationTransports", "ConfirmationCode");
            RenameIndex(table: "dbo.NotificationTransports", name: "IDX_NotificationTransport_ByUser", newName: "IDX_NotificationTransportSettings_ByUser");
        }
    }
}
