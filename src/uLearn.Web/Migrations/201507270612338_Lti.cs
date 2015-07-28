namespace uLearn.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Lti : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Consumers",
                c => new
                    {
                        ConsumerId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 64),
                        Key = c.String(nullable: false, maxLength: 64),
                        Secret = c.String(nullable: false, maxLength: 64),
                    })
                .PrimaryKey(t => t.ConsumerId)
                .Index(t => new { t.Name, t.Key, t.Secret }, name: "FullIndex");
            
            CreateTable(
                "dbo.LtiRequestModels",
                c => new
                    {
                        RequestId = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 64),
                        SlideId = c.String(nullable: false, maxLength: 64),
                        Request = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.RequestId)
                .Index(t => new { t.SlideId, t.UserId }, name: "SlideAndUser");
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.LtiRequestModels", "SlideAndUser");
            DropIndex("dbo.Consumers", "FullIndex");
            DropTable("dbo.LtiRequestModels");
            DropTable("dbo.Consumers");
        }
    }
}
