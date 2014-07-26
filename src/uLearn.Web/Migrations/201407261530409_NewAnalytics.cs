namespace uLearn.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NewAnalytics : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SlideHints",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false),
                        HintId = c.Int(nullable: false),
                        CourseId = c.String(nullable: false, maxLength: 64),
                        SlideId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SlideRates",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Rate = c.Int(nullable: false),
                        UserId = c.String(nullable: false),
                        CourseId = c.String(nullable: false, maxLength: 64),
                        SlideId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Visiters",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false),
                        CourseId = c.String(nullable: false, maxLength: 64),
                        SlideId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Visiters");
            DropTable("dbo.SlideRates");
            DropTable("dbo.SlideHints");
        }
    }
}
