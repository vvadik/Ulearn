namespace Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addLastUpdateTime : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TempCourses", "LastUpdateTime", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TempCourses", "LastUpdateTime");
        }
    }
}
