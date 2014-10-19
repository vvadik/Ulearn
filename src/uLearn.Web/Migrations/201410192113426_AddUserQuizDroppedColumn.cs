namespace uLearn.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUserQuizDroppedColumn : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserQuizs", "isDropped", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserQuizs", "isDropped");
        }
    }
}
