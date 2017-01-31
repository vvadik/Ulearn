namespace uLearn.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAdditionalScores : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AdditionalScores",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CourseId = c.String(nullable: false, maxLength: 64),
                        UnitId = c.Guid(nullable: false),
                        ScoringGroupId = c.String(),
                        UserId = c.String(nullable: false, maxLength: 128),
                        Score = c.Int(nullable: false),
                        InstructorId = c.String(nullable: false, maxLength: 128),
                        Timestamp = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.InstructorId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => new { t.CourseId, t.UserId }, name: "IDX_AdditionalScore_ByCourseAndUser")
                .Index(t => new { t.CourseId, t.UnitId, t.UserId }, unique: true, name: "IDX_AdditionalScore_ByCourseUnitAndUser")
                .Index(t => t.UnitId, name: "IDX_AdditionalScore_ByUnit")
                .Index(t => new { t.UnitId, t.UserId }, name: "IDX_AdditionalScore_ByUnitAndUser")
                .Index(t => t.InstructorId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AdditionalScores", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AdditionalScores", "InstructorId", "dbo.AspNetUsers");
            DropIndex("dbo.AdditionalScores", new[] { "InstructorId" });
            DropIndex("dbo.AdditionalScores", "IDX_AdditionalScore_ByUnitAndUser");
            DropIndex("dbo.AdditionalScores", "IDX_AdditionalScore_ByUnit");
            DropIndex("dbo.AdditionalScores", "IDX_AdditionalScore_ByCourseUnitAndUser");
            DropIndex("dbo.AdditionalScores", "IDX_AdditionalScore_ByCourseAndUser");
            DropTable("dbo.AdditionalScores");
        }
    }
}
