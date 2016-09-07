namespace uLearn.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeCascadeDeletions : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.UserExerciseSubmissions", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.UserExerciseSubmissions", "AutomaticChecking_Id", "dbo.AutomaticExerciseCheckings");
            DropForeignKey("dbo.ManualExerciseCheckings", "SubmissionId", "dbo.UserExerciseSubmissions");
            DropForeignKey("dbo.ExerciseCodeReviews", "ExerciseCheckingId", "dbo.ManualExerciseCheckings");
            DropForeignKey("dbo.ManualExerciseCheckings", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.ExerciseCodeReviews", "AuthorId", "dbo.AspNetUsers");
            AddColumn("dbo.UserExerciseSubmissions", "ApplicationUser_Id", c => c.String(maxLength: 128));
            CreateIndex("dbo.UserExerciseSubmissions", "ApplicationUser_Id");
            AddForeignKey("dbo.UserExerciseSubmissions", "ApplicationUser_Id", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.UserExerciseSubmissions", "AutomaticChecking_Id", "dbo.AutomaticExerciseCheckings", "Id", cascadeDelete: true);
            AddForeignKey("dbo.ManualExerciseCheckings", "SubmissionId", "dbo.UserExerciseSubmissions", "Id", cascadeDelete: true);
            AddForeignKey("dbo.UserExerciseSubmissions", "UserId", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.ExerciseCodeReviews", "ExerciseCheckingId", "dbo.ManualExerciseCheckings", "Id", cascadeDelete: true);
            AddForeignKey("dbo.ManualExerciseCheckings", "UserId", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.ExerciseCodeReviews", "AuthorId", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ExerciseCodeReviews", "AuthorId", "dbo.AspNetUsers");
            DropForeignKey("dbo.ManualExerciseCheckings", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.ExerciseCodeReviews", "ExerciseCheckingId", "dbo.ManualExerciseCheckings");
            DropForeignKey("dbo.UserExerciseSubmissions", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.ManualExerciseCheckings", "SubmissionId", "dbo.UserExerciseSubmissions");
            DropForeignKey("dbo.UserExerciseSubmissions", "AutomaticChecking_Id", "dbo.AutomaticExerciseCheckings");
            DropForeignKey("dbo.UserExerciseSubmissions", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropIndex("dbo.UserExerciseSubmissions", new[] { "ApplicationUser_Id" });
            DropColumn("dbo.UserExerciseSubmissions", "ApplicationUser_Id");
            AddForeignKey("dbo.ExerciseCodeReviews", "AuthorId", "dbo.AspNetUsers", "Id", cascadeDelete: true);
            AddForeignKey("dbo.ManualExerciseCheckings", "UserId", "dbo.AspNetUsers", "Id", cascadeDelete: true);
            AddForeignKey("dbo.ExerciseCodeReviews", "ExerciseCheckingId", "dbo.ManualExerciseCheckings", "Id");
            AddForeignKey("dbo.ManualExerciseCheckings", "SubmissionId", "dbo.UserExerciseSubmissions", "Id");
            AddForeignKey("dbo.UserExerciseSubmissions", "AutomaticChecking_Id", "dbo.AutomaticExerciseCheckings", "Id");
            AddForeignKey("dbo.UserExerciseSubmissions", "UserId", "dbo.AspNetUsers", "Id", cascadeDelete: true);
        }
    }
}
