namespace Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateCourseAccess : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CourseAccesses", "Comment", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.CourseAccesses", "Comment");
        }
    }
}
