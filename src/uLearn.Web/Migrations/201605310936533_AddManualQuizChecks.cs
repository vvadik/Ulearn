namespace uLearn.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddManualQuizChecks : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ManualQuizCheckQueueItems",
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
                .Index(t => t.SlideId, name: "IDX_ManualQuizCheck_ManualQuizCheckBySlide")
                .Index(t => new { t.SlideId, t.Timestamp }, name: "IDX_ManualQuizCheck_ManualQuizCheckBySlideAndTime")
                .Index(t => new { t.SlideId, t.UserId }, name: "IDX_ManualQuizCheck_ManualQuizCheckBySlideAndUser")
                .Index(t => t.LockedById);
            
            AddColumn("dbo.UserQuizs", "QuizBlockScore", c => c.Int(nullable: false));
            AddColumn("dbo.UserQuizs", "QuizBlockMaxScore", c => c.Int(nullable: false, defaultValue: 1));
			Sql("UPDATE dbo.UserQuizs SET QuizBlockScore = CONVERT(INT, IsRightQuizBlock)");
            DropColumn("dbo.UserQuizs", "IsRightQuizBlock");
        }
        
        public override void Down()
        {
            AddColumn("dbo.UserQuizs", "IsRightQuizBlock", c => c.Boolean(nullable: false));
			Sql("UPDATE dbo.UserQuizs SET IsRightQuizBlock = CONVERT(BIT, IsRightQuizBlock)");
			DropForeignKey("dbo.ManualQuizCheckQueueItems", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.ManualQuizCheckQueueItems", "LockedById", "dbo.AspNetUsers");
            DropIndex("dbo.ManualQuizCheckQueueItems", new[] { "LockedById" });
            DropIndex("dbo.ManualQuizCheckQueueItems", "IDX_ManualQuizCheck_ManualQuizCheckBySlideAndUser");
            DropIndex("dbo.ManualQuizCheckQueueItems", "IDX_ManualQuizCheck_ManualQuizCheckBySlideAndTime");
            DropIndex("dbo.ManualQuizCheckQueueItems", "IDX_ManualQuizCheck_ManualQuizCheckBySlide");
            DropColumn("dbo.UserQuizs", "QuizBlockMaxScore");
            DropColumn("dbo.UserQuizs", "QuizBlockScore");
            DropTable("dbo.ManualQuizCheckQueueItems");
        }
    }
}
