namespace uLearn.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddIndexes : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.UserExerciseSubmissions", new[] { "UserId" });
            RenameIndex(table: "dbo.Visits", name: "IDX_Visits_UserAndSlide", newName: "IDX_Visits_BySlideAndUser");
            RenameIndex(table: "dbo.Visits", name: "IDX_Visits_SlideAndTime", newName: "IDX_Visits_BySlideAndTime");
            CreateIndex("dbo.AutomaticExerciseCheckings", "IsRightAnswer", name: "IDX_AutomaticExerciseChecking_ByIsRightanswer");
            CreateIndex("dbo.AutomaticExerciseCheckings", new[] { "CourseId", "UserId" }, name: "IDX_AbstractSlideChecking_AbstractSlideCheckingByCourseAndUser");
            CreateIndex("dbo.UserExerciseSubmissions", new[] { "CourseId", "SlideId", "UserId" }, name: "IDX_UserExerciseSubmissions_BySlideAndUser");
            CreateIndex("dbo.Likes", "SubmissionId", name: "IDX_Like_BySubmission");
            CreateIndex("dbo.ManualExerciseCheckings", new[] { "CourseId", "UserId" }, name: "IDX_AbstractSlideChecking_AbstractSlideCheckingByCourseAndUser");
            CreateIndex("dbo.AutomaticQuizCheckings", new[] { "CourseId", "UserId" }, name: "IDX_AbstractSlideChecking_AbstractSlideCheckingByCourseAndUser");
            CreateIndex("dbo.ManualQuizCheckings", new[] { "CourseId", "UserId" }, name: "IDX_AbstractSlideChecking_AbstractSlideCheckingByCourseAndUser");
            CreateIndex("dbo.Visits", new[] { "CourseId", "SlideId", "UserId" }, name: "IDX_Visits_ByCourseSlideAndUser");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Visits", "IDX_Visits_ByCourseSlideAndUser");
            DropIndex("dbo.ManualQuizCheckings", "IDX_AbstractSlideChecking_AbstractSlideCheckingByCourseAndUser");
            DropIndex("dbo.AutomaticQuizCheckings", "IDX_AbstractSlideChecking_AbstractSlideCheckingByCourseAndUser");
            DropIndex("dbo.ManualExerciseCheckings", "IDX_AbstractSlideChecking_AbstractSlideCheckingByCourseAndUser");
            DropIndex("dbo.Likes", "IDX_Like_BySubmission");
            DropIndex("dbo.UserExerciseSubmissions", "IDX_UserExerciseSubmissions_BySlideAndUser");
            DropIndex("dbo.AutomaticExerciseCheckings", "IDX_AbstractSlideChecking_AbstractSlideCheckingByCourseAndUser");
            DropIndex("dbo.AutomaticExerciseCheckings", "IDX_AutomaticExerciseChecking_ByIsRightanswer");
            RenameIndex(table: "dbo.Visits", name: "IDX_Visits_BySlideAndTime", newName: "IDX_Visits_SlideAndTime");
            RenameIndex(table: "dbo.Visits", name: "IDX_Visits_BySlideAndUser", newName: "IDX_Visits_UserAndSlide");
            CreateIndex("dbo.UserExerciseSubmissions", "UserId");
        }
    }
}
