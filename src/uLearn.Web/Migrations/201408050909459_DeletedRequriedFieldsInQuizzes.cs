namespace uLearn.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DeletedRequriedFieldsInQuizzes : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.UserQuizs", "ItemId", c => c.String(maxLength: 64));
            AlterColumn("dbo.UserQuizs", "Text", c => c.String(maxLength: 1024));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.UserQuizs", "Text", c => c.String(nullable: false, maxLength: 1024));
            AlterColumn("dbo.UserQuizs", "ItemId", c => c.String(nullable: false, maxLength: 64));
        }
    }
}
