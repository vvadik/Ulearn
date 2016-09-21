namespace uLearn.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUserExerciseSolutionIndexes : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.UserExerciseSubmissions", new[] { "CourseId", "SlideId" }, name: "IDX_UserExerciseSubmissions_ByCourseAndSlide");
            CreateIndex("dbo.UserExerciseSubmissions", new[] { "CourseId", "SlideId", "Timestamp" }, name: "IDX_UserExerciseSubmissions_BySlideAndTime");
        }
        
        public override void Down()
        {
            DropIndex("dbo.UserExerciseSubmissions", "IDX_UserExerciseSubmissions_BySlideAndTime");
            DropIndex("dbo.UserExerciseSubmissions", "IDX_UserExerciseSubmissions_ByCourseAndSlide");
        }
    }
}
