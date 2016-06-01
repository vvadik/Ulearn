namespace uLearn.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCourseVersions : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CourseVersions",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        CourseId = c.String(nullable: false, maxLength: 64),
                        LoadingTime = c.DateTime(nullable: false),
                        PublishTime = c.DateTime(),
                        AuthorId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.AuthorId, cascadeDelete: true)
                .Index(t => new { t.CourseId, t.LoadingTime }, name: "IDX_CourseVersion_ByCourseAndLoadingTime")
                .Index(t => new { t.CourseId, t.PublishTime }, name: "IDX_CourseVersion_ByCourseAndPublishTime")
                .Index(t => t.AuthorId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CourseVersions", "AuthorId", "dbo.AspNetUsers");
            DropIndex("dbo.CourseVersions", new[] { "AuthorId" });
            DropIndex("dbo.CourseVersions", "IDX_CourseVersion_ByCourseAndPublishTime");
            DropIndex("dbo.CourseVersions", "IDX_CourseVersion_ByCourseAndLoadingTime");
            DropTable("dbo.CourseVersions");
        }
    }
}
