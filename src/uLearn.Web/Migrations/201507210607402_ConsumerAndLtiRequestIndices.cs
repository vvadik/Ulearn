namespace uLearn.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ConsumerAndLtiRequestIndices : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.Consumers", new[] { "Name", "Key", "Secret" }, name: "FullIndex");
            CreateIndex("dbo.LtiRequestModels", new[] { "SlideId", "UserId" }, name: "SlideAndUser");
        }
        
        public override void Down()
        {
            DropIndex("dbo.LtiRequestModels", "SlideAndUser");
            DropIndex("dbo.Consumers", "FullIndex");
        }
    }
}
