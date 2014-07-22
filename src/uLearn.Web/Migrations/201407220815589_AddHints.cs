namespace uLearn.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddHints : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Hints",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false),
                        HintId = c.Int(nullable: false),
                        AnalyticsTable_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AnalyticsTables", t => t.AnalyticsTable_Id)
                .Index(t => t.AnalyticsTable_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Hints", "AnalyticsTable_Id", "dbo.AnalyticsTables");
            DropIndex("dbo.Hints", new[] { "AnalyticsTable_Id" });
            DropTable("dbo.Hints");
        }
    }
}
