namespace uLearn.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddStatus : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserSolutions", "Status", c => c.Int(nullable: false));
            AddColumn("dbo.UserSolutions", "DisplayName", c => c.String());
            AlterColumn("dbo.AspNetUsers", "UserName", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.AspNetUsers", "UserName", c => c.String());
            DropColumn("dbo.UserSolutions", "DisplayName");
            DropColumn("dbo.UserSolutions", "Status");
        }
    }
}
