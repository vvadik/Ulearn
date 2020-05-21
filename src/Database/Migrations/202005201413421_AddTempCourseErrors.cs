namespace Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTempCourseErrors : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TempCourseErrors",
                c => new
                    {
                        CourseId = c.String(nullable: false, maxLength: 64),
                        Error = c.String(),
                    })
                .PrimaryKey(t => t.CourseId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.TempCourseErrors");
        }
    }
}
