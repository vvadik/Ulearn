namespace Database.Migrations
{
	using System.Data.Entity.Migrations;
    
    public partial class AddTransportToFeedViewTimestamp : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FeedViewTimestamps", "TransportId", c => c.Int());
            CreateIndex("dbo.FeedViewTimestamps", "TransportId");
            AddForeignKey("dbo.FeedViewTimestamps", "TransportId", "dbo.NotificationTransports", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.FeedViewTimestamps", "TransportId", "dbo.NotificationTransports");
            DropIndex("dbo.FeedViewTimestamps", new[] { "TransportId" });
            DropColumn("dbo.FeedViewTimestamps", "TransportId");
        }
    }
}
