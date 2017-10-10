namespace Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddIndeciesToFeedViewTimestamp : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.FeedViewTimestamps", "IDX_FeedUpdateTimestamp_ByUser");
            DropIndex("dbo.FeedViewTimestamps", new[] { "TransportId" });
            DropPrimaryKey("dbo.FeedViewTimestamps");
            AddColumn("dbo.FeedViewTimestamps", "Id", c => c.Int(nullable: false, identity: true));
            AlterColumn("dbo.FeedViewTimestamps", "UserId", c => c.String(maxLength: 64));
            AddPrimaryKey("dbo.FeedViewTimestamps", "Id");
            CreateIndex("dbo.FeedViewTimestamps", "UserId", name: "IDX_FeedUpdateTimestamp_ByUser");
            CreateIndex("dbo.FeedViewTimestamps", new[] { "UserId", "TransportId" }, name: "IDX_FeedUpdateTimestamp_ByUserAndTransport");
            CreateIndex("dbo.FeedViewTimestamps", "Timestamp", name: "IDX_FeedUpdateTimestamp_ByTimestamp");
        }
        
        public override void Down()
        {
            DropIndex("dbo.FeedViewTimestamps", "IDX_FeedUpdateTimestamp_ByTimestamp");
            DropIndex("dbo.FeedViewTimestamps", "IDX_FeedUpdateTimestamp_ByUserAndTransport");
            DropIndex("dbo.FeedViewTimestamps", "IDX_FeedUpdateTimestamp_ByUser");
            DropPrimaryKey("dbo.FeedViewTimestamps");
            AlterColumn("dbo.FeedViewTimestamps", "UserId", c => c.String(nullable: false, maxLength: 64));
            DropColumn("dbo.FeedViewTimestamps", "Id");
            AddPrimaryKey("dbo.FeedViewTimestamps", "UserId");
            CreateIndex("dbo.FeedViewTimestamps", "TransportId");
            CreateIndex("dbo.FeedViewTimestamps", "UserId", name: "IDX_FeedUpdateTimestamp_ByUser");
        }
    }
}
