using System.Data.Entity.Migrations;

namespace Database.Migrations
{
	public partial class ExtractAutomaticCheckingIsRightAnswer : DbMigration
	{
		public override void Up()
		{
			RenameColumn(table: "dbo.UserExerciseSubmissions", name: "AutomaticChecking_Id", newName: "AutomaticCheckingId");
			RenameIndex(table: "dbo.UserExerciseSubmissions", name: "IX_AutomaticChecking_Id", newName: "IX_AutomaticCheckingId");
			AddColumn("dbo.UserExerciseSubmissions", "AutomaticCheckingIsRightAnswer", c => c.Boolean(nullable: false));
			CreateIndex("dbo.UserExerciseSubmissions", new[] { "CourseId", "AutomaticCheckingIsRightAnswer" }, name: "IDX_UserExerciseSubmissions_ByCourseAndIsRightAnswer");
			CreateIndex("dbo.UserExerciseSubmissions", new[] { "CourseId", "SlideId", "AutomaticCheckingIsRightAnswer" }, name: "IDX_UserExerciseSubmissions_BySlideAndIsRightAnswer");
			CreateIndex("dbo.UserExerciseSubmissions", "AutomaticCheckingIsRightAnswer", name: "IDX_UserExerciseSubmissions_ByIsRightAnswer");

			/* Duplicate IsRightAnswer: copy it from AutomaticExerciseCheckings into UserExerciseSubmissions */
			Sql("UPDATE dbo.UserExerciseSubmissions " +
				"SET UserExerciseSubmissions.AutomaticCheckingIsRightAnswer = AutomaticExerciseCheckings.IsRightAnswer " +
				"FROM dbo.UserExerciseSubmissions " +
				"LEFT JOIN dbo.AutomaticExerciseCheckings ON UserExerciseSubmissions.AutomaticCheckingId = AutomaticExerciseCheckings.Id");
		}

		public override void Down()
		{
			DropIndex("dbo.UserExerciseSubmissions", "IDX_UserExerciseSubmissions_ByIsRightAnswer");
			DropIndex("dbo.UserExerciseSubmissions", "IDX_UserExerciseSubmissions_BySlideAndIsRightAnswer");
			DropIndex("dbo.UserExerciseSubmissions", "IDX_UserExerciseSubmissions_ByCourseAndIsRightAnswer");
			DropColumn("dbo.UserExerciseSubmissions", "AutomaticCheckingIsRightAnswer");
			RenameIndex(table: "dbo.UserExerciseSubmissions", name: "IX_AutomaticCheckingId", newName: "IX_AutomaticChecking_Id");
			RenameColumn(table: "dbo.UserExerciseSubmissions", name: "AutomaticCheckingId", newName: "AutomaticChecking_Id");
		}
	}
}