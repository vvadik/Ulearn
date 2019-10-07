namespace Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddLastVisits : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.LastVisits",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        CourseId = c.String(nullable: false, maxLength: 64),
                        SlideId = c.Guid(nullable: false),
                        Timestamp = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => new { t.CourseId, t.UserId }, name: "IDX_LastVisits_ByCourseAndUser");
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.LastVisits", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.LastVisits", "IDX_LastVisits_ByCourseAndUser");
            DropTable("dbo.LastVisits");
        }
    }
}
