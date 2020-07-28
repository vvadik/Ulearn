namespace Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTempCourse : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TempCourses",
                c => new
                    {
                        CourseId = c.String(nullable: false, maxLength: 64),
                        LoadingTime = c.DateTime(nullable: false),
                        AuthorId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.CourseId)
                .ForeignKey("dbo.AspNetUsers", t => t.AuthorId, cascadeDelete: true)
                .Index(t => t.AuthorId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TempCourses", "AuthorId", "dbo.AspNetUsers");
            DropIndex("dbo.TempCourses", new[] { "AuthorId" });
            DropTable("dbo.TempCourses");
        }
    }
}
