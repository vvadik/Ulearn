namespace Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveUniqueFromNotificationDelivery : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.NotificationDeliveries", "IDX_NotificationDelivery_ByNotificationAndTransport");
            CreateIndex("dbo.NotificationDeliveries", new[] { "NotificationId", "NotificationTransportId" }, name: "IDX_NotificationDelivery_ByNotificationAndTransport");
        }
        
        public override void Down()
        {
            DropIndex("dbo.NotificationDeliveries", "IDX_NotificationDelivery_ByNotificationAndTransport");
            CreateIndex("dbo.NotificationDeliveries", new[] { "NotificationId", "NotificationTransportId" }, unique: true, name: "IDX_NotificationDelivery_ByNotificationAndTransport");
        }
    }
}
