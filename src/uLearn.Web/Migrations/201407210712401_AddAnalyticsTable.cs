namespace uLearn.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAnalyticsTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AnalyticsTables",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SlideMarks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Mark = c.Int(nullable: false),
                        UserId = c.String(nullable: false),
                        AnalyticsTable_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AnalyticsTables", t => t.AnalyticsTable_Id)
                .Index(t => t.AnalyticsTable_Id);
            
            CreateTable(
                "dbo.Solvers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false),
                        AnalyticsTable_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AnalyticsTables", t => t.AnalyticsTable_Id)
                .Index(t => t.AnalyticsTable_Id);
            
            CreateTable(
                "dbo.Visiters",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false),
                        AnalyticsTable_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AnalyticsTables", t => t.AnalyticsTable_Id)
                .Index(t => t.AnalyticsTable_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Visiters", "AnalyticsTable_Id", "dbo.AnalyticsTables");
            DropForeignKey("dbo.Solvers", "AnalyticsTable_Id", "dbo.AnalyticsTables");
            DropForeignKey("dbo.SlideMarks", "AnalyticsTable_Id", "dbo.AnalyticsTables");
            DropIndex("dbo.Visiters", new[] { "AnalyticsTable_Id" });
            DropIndex("dbo.Solvers", new[] { "AnalyticsTable_Id" });
            DropIndex("dbo.SlideMarks", new[] { "AnalyticsTable_Id" });
            DropTable("dbo.Visiters");
            DropTable("dbo.Solvers");
            DropTable("dbo.SlideMarks");
            DropTable("dbo.AnalyticsTables");
        }
    }
}
