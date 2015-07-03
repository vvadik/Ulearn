namespace uLearn.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddConsumers : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.LtiRequestModels",
                c => new
                    {
                        RequestId = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 64),
                        Request = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.RequestId);
            
            CreateTable(
                "dbo.Scores",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        Value = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Scores");
            DropTable("dbo.LtiRequestModels");
        }
    }
}
