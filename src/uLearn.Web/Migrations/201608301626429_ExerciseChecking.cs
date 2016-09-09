namespace uLearn.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ExerciseChecking : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Likes", "UserSolutionId", "dbo.UserSolutions");
            DropForeignKey("dbo.UserSolutions", "SolutionCodeHash", "dbo.TextBlobs");
            DropForeignKey("dbo.UserSolutions", "UserId", "dbo.AspNetUsers");
			RenameTable(name: "dbo.UserSolutions", newName: "AutomaticExerciseCheckings");
			DropIndex("dbo.AutomaticExerciseCheckings", "AcceptedList");

			/* Remove nulled userids before index destruction */
			Sql("DELETE FROM dbo.AutomaticExerciseCheckings WHERE UserId IS NULL");

			DropIndex("dbo.AutomaticExerciseCheckings", new[] { "UserId" });
            DropIndex("dbo.AutomaticExerciseCheckings", new[] { "SolutionCodeHash" });
            DropIndex("dbo.AutomaticExerciseCheckings", "IDX_UserSolution_Timestamp");
            DropIndex("dbo.Likes", "UserAndSolution");
            CreateTable(
                "dbo.UserExerciseSubmissions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        CourseId = c.String(nullable: false, maxLength: 40),
                        SlideId = c.Guid(nullable: false),
                        Timestamp = c.DateTime(nullable: false),
                        SolutionCodeHash = c.String(nullable: false, maxLength: 40),
                        CodeHash = c.Int(nullable: false),
                        AutomaticChecking_Id = c.Int(nullable: false),
                        ManualChecking_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AutomaticExerciseCheckings", t => t.AutomaticChecking_Id)
                .ForeignKey("dbo.ManualExerciseCheckings", t => t.ManualChecking_Id)
                .ForeignKey("dbo.TextBlobs", t => t.SolutionCodeHash, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.SolutionCodeHash)
                .Index(t => t.AutomaticChecking_Id)
                .Index(t => t.ManualChecking_Id);
            
            CreateTable(
                "dbo.ManualExerciseCheckings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LockedUntil = c.DateTime(),
                        LockedById = c.String(maxLength: 128),
                        IsChecked = c.Boolean(nullable: false),
                        CourseId = c.String(nullable: false, maxLength: 64),
                        SlideId = c.Guid(nullable: false),
                        Timestamp = c.DateTime(nullable: false),
                        UserId = c.String(nullable: false, maxLength: 128),
                        Score = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.LockedById)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.LockedById)
                .Index(t => t.SlideId, name: "IDX_AbstractSlideChecking_AbstractSlideCheckingBySlide")
                .Index(t => new { t.SlideId, t.Timestamp }, name: "IDX_AbstractSlideChecking_AbstractSlideCheckingBySlideAndTime")
                .Index(t => new { t.SlideId, t.UserId }, name: "IDX_AbstractSlideChecking_AbstractSlideCheckingBySlideAndUser");
            
            CreateTable(
                "dbo.ExerciseCodeReviews",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ExerciseCheckingId = c.Int(nullable: false),
                        StartPosition = c.Int(nullable: false),
                        FinishPosition = c.Int(nullable: false),
                        Comment = c.String(nullable: false),
                        AuthorId = c.String(nullable: false, maxLength: 128),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.AuthorId, cascadeDelete: true)
                .ForeignKey("dbo.ManualExerciseCheckings", t => t.ExerciseCheckingId)
                .Index(t => t.ExerciseCheckingId, name: "IDX_ExerciseCodeReview_ByManualExerciseChecking")
                .Index(t => t.AuthorId);
            
            AddColumn("dbo.AutomaticExerciseCheckings", "Score", c => c.Int(nullable: false));
            AddColumn("dbo.Likes", "SubmissionId", c => c.Int(nullable: false));

            AlterColumn("dbo.AutomaticExerciseCheckings", "UserId", c => c.String(nullable: false, maxLength: 128));
            CreateIndex("dbo.AutomaticExerciseCheckings", "SlideId", name: "IDX_AbstractSlideChecking_AbstractSlideCheckingBySlide");
            CreateIndex("dbo.AutomaticExerciseCheckings", new[] { "SlideId", "Timestamp" }, name: "IDX_AbstractSlideChecking_AbstractSlideCheckingBySlideAndTime");
            CreateIndex("dbo.AutomaticExerciseCheckings", new[] { "SlideId", "UserId" }, name: "IDX_AbstractSlideChecking_AbstractSlideCheckingBySlideAndUser");

			Sql("INSERT INTO dbo.UserExerciseSubmissions" +
				"      ([SlideId], [CourseId], [UserId], [Timestamp], [SolutionCodeHash], [CodeHash], [AutomaticChecking_Id]) " +
				"SELECT [SlideId], [CourseId], [UserId], [Timestamp], [SolutionCodeHash], [CodeHash], [Id] FROM dbo.AutomaticExerciseCheckings");
	        Sql("UPDATE dbo.AutomaticExerciseCheckings SET [Score] = 5 WHERE [IsRightAnswer] = 'true'");
			Sql("UPDATE dbo.Likes SET Likes.SubmissionId = UserExerciseSubmissions.Id FROM dbo.Likes JOIN dbo.UserExerciseSubmissions ON Likes.UserSolutionId = UserExerciseSubmissions.AutomaticChecking_Id");
	        Sql("DELETE FROM dbo.Likes WHERE SubmissionId = 0");

			CreateIndex("dbo.Likes", new[] { "UserId", "SubmissionId" }, name: "IDX_Like_ByUserAndSubmission");
            AddForeignKey("dbo.Likes", "SubmissionId", "dbo.UserExerciseSubmissions", "Id");
            AddForeignKey("dbo.AutomaticExerciseCheckings", "UserId", "dbo.AspNetUsers", "Id", cascadeDelete: true);
            DropColumn("dbo.AutomaticExerciseCheckings", "SolutionCodeHash");
            DropColumn("dbo.AutomaticExerciseCheckings", "CodeHash");
            DropColumn("dbo.Likes", "UserSolutionId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Likes", "UserSolutionId", c => c.Int(nullable: false));
            AddColumn("dbo.AutomaticExerciseCheckings", "CodeHash", c => c.Int(nullable: false));
            AddColumn("dbo.AutomaticExerciseCheckings", "SolutionCodeHash", c => c.String(nullable: false, maxLength: 40));

	        Sql("MERGE dbo.AutomaticExerciseCheckings AS TARGET " +
				"USING dbo.UserExerciseSubmissions AS SOURCE " +
				"ON(TARGET.Id = SOURCE.AutomaticChecking_Id) " +
				"WHEN MATCHED THEN " +
				"UPDATE SET TARGET.[CodeHash] = SOURCE.[CodeHash]," +
				"TARGET.[SolutionCodeHash] = SOURCE.[SolutionCodeHash]");

			DropForeignKey("dbo.AutomaticExerciseCheckings", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.UserExerciseSubmissions", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.UserExerciseSubmissions", "SolutionCodeHash", "dbo.TextBlobs");
            DropForeignKey("dbo.UserExerciseSubmissions", "ManualChecking_Id", "dbo.ManualExerciseCheckings");
            DropForeignKey("dbo.ManualExerciseCheckings", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.ExerciseCodeReviews", "ExerciseCheckingId", "dbo.ManualExerciseCheckings");
            DropForeignKey("dbo.ExerciseCodeReviews", "AuthorId", "dbo.AspNetUsers");
            DropForeignKey("dbo.ManualExerciseCheckings", "LockedById", "dbo.AspNetUsers");
            DropForeignKey("dbo.Likes", "SubmissionId", "dbo.UserExerciseSubmissions");
            DropForeignKey("dbo.UserExerciseSubmissions", "AutomaticChecking_Id", "dbo.AutomaticExerciseCheckings");
            DropIndex("dbo.ExerciseCodeReviews", new[] { "AuthorId" });
            DropIndex("dbo.ExerciseCodeReviews", "IDX_ExerciseCodeReview_ByManualExerciseChecking");
            DropIndex("dbo.ManualExerciseCheckings", "IDX_AbstractSlideChecking_AbstractSlideCheckingBySlideAndUser");
            DropIndex("dbo.ManualExerciseCheckings", "IDX_AbstractSlideChecking_AbstractSlideCheckingBySlideAndTime");
            DropIndex("dbo.ManualExerciseCheckings", "IDX_AbstractSlideChecking_AbstractSlideCheckingBySlide");
            DropIndex("dbo.ManualExerciseCheckings", new[] { "LockedById" });
            DropIndex("dbo.Likes", "IDX_Like_ByUserAndSubmission");
            DropIndex("dbo.UserExerciseSubmissions", new[] { "ManualChecking_Id" });
            DropIndex("dbo.UserExerciseSubmissions", new[] { "AutomaticChecking_Id" });
            DropIndex("dbo.UserExerciseSubmissions", new[] { "SolutionCodeHash" });
            DropIndex("dbo.UserExerciseSubmissions", new[] { "UserId" });
            DropIndex("dbo.AutomaticExerciseCheckings", "IDX_AbstractSlideChecking_AbstractSlideCheckingBySlideAndUser");
            DropIndex("dbo.AutomaticExerciseCheckings", "IDX_AbstractSlideChecking_AbstractSlideCheckingBySlideAndTime");
            DropIndex("dbo.AutomaticExerciseCheckings", "IDX_AbstractSlideChecking_AbstractSlideCheckingBySlide");
            AlterColumn("dbo.AutomaticExerciseCheckings", "UserId", c => c.String(maxLength: 128));
            DropColumn("dbo.Likes", "SubmissionId");
            DropColumn("dbo.AutomaticExerciseCheckings", "Score");
            DropTable("dbo.ExerciseCodeReviews");
            DropTable("dbo.ManualExerciseCheckings");
            DropTable("dbo.UserExerciseSubmissions");
            CreateIndex("dbo.Likes", new[] { "UserId", "UserSolutionId" }, name: "UserAndSolution");
            CreateIndex("dbo.AutomaticExerciseCheckings", "Timestamp", name: "IDX_UserSolution_Timestamp");
            CreateIndex("dbo.AutomaticExerciseCheckings", "SolutionCodeHash");
            CreateIndex("dbo.AutomaticExerciseCheckings", "UserId");
            CreateIndex("dbo.AutomaticExerciseCheckings", new[] { "SlideId", "IsRightAnswer", "CodeHash", "Timestamp" }, name: "AcceptedList");
			RenameTable(name: "dbo.AutomaticExerciseCheckings", newName: "UserSolutions");
			AddForeignKey("dbo.UserSolutions", "UserId", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.UserSolutions", "SolutionCodeHash", "dbo.TextBlobs", "Hash", cascadeDelete: true);
            AddForeignKey("dbo.Likes", "UserSolutionId", "dbo.UserSolutions", "Id", cascadeDelete: true);
        }
    }
}
