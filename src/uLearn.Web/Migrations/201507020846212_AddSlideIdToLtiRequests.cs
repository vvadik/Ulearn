namespace uLearn.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSlideIdToLtiRequests : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.LtiRequestModels", "SlideId", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.LtiRequestModels", "SlideId");
        }
    }
}
