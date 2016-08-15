namespace uLearn.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveOldUserGroups : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.AspNetUsers", "GroupName");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUsers", "GroupName", c => c.String());
        }
    }
}
