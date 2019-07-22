namespace Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUserFlashcardsUnlocking : DbMigration
    {
        public override void Up()
        {
			CreateTable(
					"dbo.UserFlashcardsUnlocking",
					c => new
					{
						Id = c.Int(nullable: false, identity: true),
						UserId = c.String(nullable: false, maxLength: 128),
						CourseId = c.String(nullable: false, maxLength: 64),
						UnitId = c.Guid(nullable: false),
					})
				.PrimaryKey(t => t.Id)
				.ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
				.Index(t => new { t.UserId, t.CourseId, t.UnitId }, name: "IDX_UserFlashcardsUnlocking_ByUserIdAndCourseIdAndUnitId");
        }
        
        public override void Down()
        {
			DropForeignKey("dbo.UserFlashcardsUnlocking", "UserId", "dbo.AspNetUsers");
			DropIndex("dbo.UserFlashcardsUnlocking", "IDX_UserFlashcardsUnlocking_ByUserIdAndCourseIdAndUnitId");
			DropTable("dbo.UserFlashcardsUnlocking");
        }
    }
}
