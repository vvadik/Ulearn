namespace uLearn.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddHiddenFromTopComments : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ExerciseCodeReviews", "HiddenFromTopComments", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ExerciseCodeReviews", "HiddenFromTopComments");
        }
    }
}
