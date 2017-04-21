namespace uLearn.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddNotifications : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.NotificationDeliveries",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        NotificationId = c.Int(nullable: false),
                        NotificationTransportId = c.Int(nullable: false),
                        Status = c.Byte(nullable: false),
                        CreateTime = c.DateTime(nullable: false),
                        SendTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Notifications", t => t.NotificationId)
                .ForeignKey("dbo.NotificationTransports", t => t.NotificationTransportId, cascadeDelete: true)
                .Index(t => new { t.NotificationId, t.NotificationTransportId }, unique: true, name: "IDX_NotificationDelivery_ByNotificationAndTransport")
                .Index(t => t.SendTime, name: "IDX_NotificationDelivery_BySendTime");
            
            CreateTable(
                "dbo.Notifications",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CourseId = c.String(maxLength: 100),
                        InitiatedById = c.String(maxLength: 128),
                        CreateTime = c.DateTime(nullable: false),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.InitiatedById)
                .Index(t => t.InitiatedById);
            
            CreateTable(
                "dbo.NotificationTransports",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(maxLength: 128),
                        IsEnabled = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        Email = c.String(maxLength: 200),
                        ChatId = c.String(maxLength: 200),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId, name: "IDX_NotificationTransportSettings_ByUser");
            
            CreateTable(
                "dbo.NotificationTransportSettings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        NotificationTransportId = c.Int(nullable: false),
                        CourseId = c.String(maxLength: 100),
                        NotificationType = c.Short(nullable: false),
                        Frequency = c.Byte(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.NotificationTransports", t => t.NotificationTransportId, cascadeDelete: true)
                .Index(t => t.NotificationTransportId, name: "IDX_NotificationTransportSettings_ByNotificationTransport")
                .Index(t => t.CourseId, name: "IDX_NotificationTransportSettings_ByCourse")
                .Index(t => new { t.CourseId, t.NotificationType }, name: "IDX_NotificationTransportSettings_ByCourseAndNofiticationType")
                .Index(t => t.NotificationType, name: "IDX_NotificationTransportSettings_ByNotificationType");
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.NotificationTransportSettings", "NotificationTransportId", "dbo.NotificationTransports");
            DropForeignKey("dbo.NotificationDeliveries", "NotificationTransportId", "dbo.NotificationTransports");
            DropForeignKey("dbo.NotificationTransports", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.NotificationDeliveries", "NotificationId", "dbo.Notifications");
            DropForeignKey("dbo.Notifications", "InitiatedById", "dbo.AspNetUsers");
            DropIndex("dbo.NotificationTransportSettings", "IDX_NotificationTransportSettings_ByNotificationType");
            DropIndex("dbo.NotificationTransportSettings", "IDX_NotificationTransportSettings_ByCourseAndNofiticationType");
            DropIndex("dbo.NotificationTransportSettings", "IDX_NotificationTransportSettings_ByCourse");
            DropIndex("dbo.NotificationTransportSettings", "IDX_NotificationTransportSettings_ByNotificationTransport");
            DropIndex("dbo.NotificationTransports", "IDX_NotificationTransportSettings_ByUser");
            DropIndex("dbo.Notifications", new[] { "InitiatedById" });
            DropIndex("dbo.NotificationDeliveries", "IDX_NotificationDelivery_BySendTime");
            DropIndex("dbo.NotificationDeliveries", "IDX_NotificationDelivery_ByNotificationAndTransport");
            DropTable("dbo.NotificationTransportSettings");
            DropTable("dbo.NotificationTransports");
            DropTable("dbo.Notifications");
            DropTable("dbo.NotificationDeliveries");
        }
    }
}
