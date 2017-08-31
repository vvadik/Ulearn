namespace Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddStepikAccessTokens : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.StepikAccessTokens",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        AccessToken = c.String(nullable: false, maxLength: 100),
                        AddedTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.AddedTime);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.StepikAccessTokens", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.StepikAccessTokens", new[] { "AddedTime" });
            DropIndex("dbo.StepikAccessTokens", new[] { "UserId" });
            DropTable("dbo.StepikAccessTokens");
        }
    }
}
