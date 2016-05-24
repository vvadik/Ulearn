namespace uLearn.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddManualChecks : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ManualChecks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CourseId = c.String(nullable: false, maxLength: 64),
                        SlideId = c.Guid(nullable: false),
                        Timestamp = c.DateTime(nullable: false),
                        UserId = c.String(nullable: false, maxLength: 128),
                        LockedUntil = c.DateTime(),
                        LockedById = c.String(maxLength: 128),
                        IsChecked = c.Boolean(nullable: false),
                        Score = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.LockedById)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.SlideId, name: "IDX_ManualCheck_ManualCheckBySlide")
                .Index(t => new { t.SlideId, t.Timestamp }, name: "IDX_ManualCheck_ManualCheckBySlideAndTime")
                .Index(t => new { t.SlideId, t.UserId }, name: "IDX_ManualCheck_ManualCheckBySlideAndUser")
                .Index(t => t.LockedById);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ManualChecks", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.ManualChecks", "LockedById", "dbo.AspNetUsers");
            DropIndex("dbo.ManualChecks", new[] { "LockedById" });
            DropIndex("dbo.ManualChecks", "IDX_ManualCheck_ManualCheckBySlideAndUser");
            DropIndex("dbo.ManualChecks", "IDX_ManualCheck_ManualCheckBySlideAndTime");
            DropIndex("dbo.ManualChecks", "IDX_ManualCheck_ManualCheckBySlide");
            DropTable("dbo.ManualChecks");
        }
    }
}
