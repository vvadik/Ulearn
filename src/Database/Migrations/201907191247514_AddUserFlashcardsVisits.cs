namespace Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUserFlashcardsVisits : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserFlashcardsVisits",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        CourseId = c.String(nullable: false, maxLength: 64),
                        UnitId = c.Guid(nullable: false),
                        FlashcardId = c.String(nullable: false, maxLength: 64),
                        Rate = c.Int(nullable: false),
                        Timestamp = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => new { t.UserId, t.CourseId, t.UnitId, t.FlashcardId }, name: "IDX_UserFlashcardsVisits_ByUserIdAndCourseIdAndUnitIdAndFlashcardId");
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserFlashcardsVisits", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.UserFlashcardsVisits", "IDX_UserFlashcardsVisits_ByUserIdAndCourseIdAndUnitIdAndFlashcardId");
            DropTable("dbo.UserFlashcardsVisits");
        }
    }
}
