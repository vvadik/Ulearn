namespace Database.Migrations
{
	using System;
	using System.Data.Entity.Migrations;

	public partial class AddFeedNotificationTransport : DbMigration
	{
		public override void Up()
		{
			CreateTable(
					"dbo.FeedViewTimestamps",
					c => new
					{
						UserId = c.String(nullable: false, maxLength: 64),
						Timestamp = c.DateTime(nullable: false),
					})
				.PrimaryKey(t => t.UserId)
				.Index(t => t.UserId, name: "IDX_FeedUpdateTimestamp_ByUser");

			CreateIndex("dbo.NotificationDeliveries", "CreateTime", name: "IDX_NotificatoinDelivery_ByCreateTime");

			Sql("INSERT INTO dbo.NotificationTransports (UserId, IsEnabled, IsDeleted, Discriminator) VALUES (NULL, 1, 0, 'FeedNotificationTransport')");
		}

		public override void Down()
		{
			Sql("DELETE FROM dbo.NotificationTransports WHERE Discriminator = 'FeedNotificationTransport' AND UserId IS NULL");
			DropIndex("dbo.NotificationDeliveries", "IDX_NotificatoinDelivery_ByCreateTime");
			DropIndex("dbo.FeedViewTimestamps", "IDX_FeedUpdateTimestamp_ByUser");
			DropTable("dbo.FeedViewTimestamps");
		}
	}
}