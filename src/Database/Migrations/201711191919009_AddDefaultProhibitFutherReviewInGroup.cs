namespace Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDefaultProhibitFutherReviewInGroup : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Groups", "DefaultProhibitFutherReview", c => c.Boolean(nullable: false, defaultValue: true));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Groups", "DefaultProhibitFutherReview");
        }
    }
}
