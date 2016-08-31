namespace uLearn.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MultipleManualExerciseCheckings : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.UserExerciseSubmissions", "ManualChecking_Id", "dbo.ManualExerciseCheckings");
            DropIndex("dbo.UserExerciseSubmissions", new[] { "ManualChecking_Id" });
            AddColumn("dbo.ManualExerciseCheckings", "SubmissionId", c => c.Int(nullable: false));
            CreateIndex("dbo.ManualExerciseCheckings", "SubmissionId");
            AddForeignKey("dbo.ManualExerciseCheckings", "SubmissionId", "dbo.UserExerciseSubmissions", "Id");
            DropColumn("dbo.UserExerciseSubmissions", "ManualChecking_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.UserExerciseSubmissions", "ManualChecking_Id", c => c.Int());
            DropForeignKey("dbo.ManualExerciseCheckings", "SubmissionId", "dbo.UserExerciseSubmissions");
            DropIndex("dbo.ManualExerciseCheckings", new[] { "SubmissionId" });
            DropColumn("dbo.ManualExerciseCheckings", "SubmissionId");
            CreateIndex("dbo.UserExerciseSubmissions", "ManualChecking_Id");
            AddForeignKey("dbo.UserExerciseSubmissions", "ManualChecking_Id", "dbo.ManualExerciseCheckings", "Id");
        }
    }
}
