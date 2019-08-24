namespace Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateUserRoles : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserRoles", "GrantedById", c => c.String());
            AddColumn("dbo.UserRoles", "GrantTime", c => c.DateTime());
            AddColumn("dbo.UserRoles", "IsEnabled", c => c.Boolean());
            AddColumn("dbo.UserRoles", "Comment", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserRoles", "Comment");
            DropColumn("dbo.UserRoles", "IsEnabled");
            DropColumn("dbo.UserRoles", "GrantTime");
            DropColumn("dbo.UserRoles", "GrantedById");
        }
    }
}
