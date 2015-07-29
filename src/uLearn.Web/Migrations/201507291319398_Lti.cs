namespace uLearn.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Lti : DbMigration
    {
        public override void Up()
        {
			Sql("ALTER TABLE dbo.UserSolutions DROP CONSTRAINT [FK_dbo.UserSolutions_dbo.AspNetUsers_UserId]");
			Sql("ALTER TABLE dbo.UserSolutions ADD CONSTRAINT [FK_dbo.UserSolutions_dbo.AspNetUsers_UserId] FOREIGN KEY (UserId) REFERENCES dbo.[AspNetUsers](Id) ON UPDATE NO ACTION ON DELETE SET NULL");
			
			CreateTable(
                "dbo.LtiConsumers",
                c => new
                    {
                        ConsumerId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 64),
                        Key = c.String(nullable: false, maxLength: 64),
                        Secret = c.String(nullable: false, maxLength: 64),
                    })
                .PrimaryKey(t => t.ConsumerId)
                .Index(t => t.Key, name: "IDX_LtiConsumer_Key");
            
            CreateTable(
                "dbo.LtiSlideRequests",
                c => new
                    {
                        RequestId = c.Int(nullable: false, identity: true),
                        SlideId = c.String(nullable: false, maxLength: 64),
                        UserId = c.String(nullable: false, maxLength: 64),
                        Request = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.RequestId)
                .Index(t => new { t.SlideId, t.UserId }, name: "IDX_LtiSlideRequest_SlideAndUser");
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.LtiSlideRequests", "IDX_LtiSlideRequest_SlideAndUser");
            DropIndex("dbo.LtiConsumers", "IDX_LtiConsumer_Key");
            DropTable("dbo.LtiSlideRequests");
            DropTable("dbo.LtiConsumers");
        }
    }
}
