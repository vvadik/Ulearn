namespace uLearn.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeExerciseCodeReview : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ExerciseCodeReviews", "StartLine", c => c.Int(nullable: false));
            AddColumn("dbo.ExerciseCodeReviews", "FinishLine", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ExerciseCodeReviews", "FinishLine");
            DropColumn("dbo.ExerciseCodeReviews", "StartLine");
        }
    }
}
