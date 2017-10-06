namespace Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCourseAccesses : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CourseAccesses",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CourseId = c.String(nullable: false, maxLength: 64),
                        UserId = c.String(maxLength: 128),
                        GrantedById = c.String(maxLength: 128),
                        AccessType = c.Short(nullable: false),
                        GrantTime = c.DateTime(nullable: false),
                        IsEnabled = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.GrantedById)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.CourseId, name: "IDX_CourseAccess_ByCourse")
                .Index(t => new { t.CourseId, t.IsEnabled }, name: "IDX_CourseAccess_ByCourseAndIsEnabled")
                .Index(t => new { t.CourseId, t.UserId, t.IsEnabled }, name: "IDX_CourseAccess_ByCourseUserAndIsEnabled")
                .Index(t => t.UserId, name: "IDX_CourseAccess_ByUser")
                .Index(t => t.GrantedById)
                .Index(t => t.GrantTime, name: "IDX_CourseAccess_ByGrantTime");
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CourseAccesses", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.CourseAccesses", "GrantedById", "dbo.AspNetUsers");
            DropIndex("dbo.CourseAccesses", "IDX_CourseAccess_ByGrantTime");
            DropIndex("dbo.CourseAccesses", new[] { "GrantedById" });
            DropIndex("dbo.CourseAccesses", "IDX_CourseAccess_ByUser");
            DropIndex("dbo.CourseAccesses", "IDX_CourseAccess_ByCourseUserAndIsEnabled");
            DropIndex("dbo.CourseAccesses", "IDX_CourseAccess_ByCourseAndIsEnabled");
            DropIndex("dbo.CourseAccesses", "IDX_CourseAccess_ByCourse");
            DropTable("dbo.CourseAccesses");
        }
    }
}
