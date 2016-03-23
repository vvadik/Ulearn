using ApprovalUtilities.Utilities;

namespace uLearn.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SlideIdIsGuid : DbMigration
    {
	    private void InsertHyphensIntoGuids(string tableName)
	    {
			Sql("UPDATE {0} SET SlideId = SUBSTRING(SlideId, 1, 8) + '-' + SUBSTRING(SlideId, 9, 4) + '-' + SUBSTRING(SlideId, 13, 4) + '-' + SUBSTRING(SlideId, 17, 4) + '-' + SUBSTRING(SlideId, 21, 12) WHERE LEN(SlideId) = 32".FormatWith(tableName));
		}

        public override void Up()
        {
            DropIndex("dbo.Comments", "IDX_Comment_CommentBySlide");
            DropIndex("dbo.UserSolutions", "AcceptedList");
            DropIndex("dbo.SlideHints", "FullIndex");
            DropIndex("dbo.LtiSlideRequests", "IDX_LtiSlideRequest_SlideAndUser");
            DropIndex("dbo.SlideRates", "SlideAndUser");
            DropIndex("dbo.UserQuizs", "FullIndex");
            DropIndex("dbo.UserQuizs", "StatisticsIndex");
            DropIndex("dbo.Visits", "IDX_Visits_UserAndSlide");
            DropIndex("dbo.Visits", "IDX_Visits_SlideAndTime");
			InsertHyphensIntoGuids("dbo.Comments");
			AlterColumn("dbo.Comments", "SlideId", c => c.Guid(nullable: false));
			InsertHyphensIntoGuids("dbo.UserQuestions");
			AlterColumn("dbo.UserQuestions", "SlideId", c => c.Guid(nullable: false));
			Sql("UPDATE dbo.UserSolutions SET SlideId = '00000000-0000-0000-0000-000000000000' WHERE SlideId = 'runner'");
			InsertHyphensIntoGuids("dbo.UserSolutions");
			AlterColumn("dbo.UserSolutions", "SlideId", c => c.Guid(nullable: false));
			InsertHyphensIntoGuids("dbo.SlideHints");
			AlterColumn("dbo.SlideHints", "SlideId", c => c.Guid(nullable: false));
			InsertHyphensIntoGuids("dbo.LtiSlideRequests");
			AlterColumn("dbo.LtiSlideRequests", "SlideId", c => c.Guid(nullable: false));
			InsertHyphensIntoGuids("dbo.SlideRates");
			AlterColumn("dbo.SlideRates", "SlideId", c => c.Guid(nullable: false));
			InsertHyphensIntoGuids("dbo.UserQuizs");
			AlterColumn("dbo.UserQuizs", "SlideId", c => c.Guid(nullable: false));
			InsertHyphensIntoGuids("dbo.Visits");
			AlterColumn("dbo.Visits", "SlideId", c => c.Guid(nullable: false));
            CreateIndex("dbo.Comments", "SlideId", name: "IDX_Comment_CommentBySlide");
            CreateIndex("dbo.UserSolutions", new[] { "SlideId", "IsRightAnswer", "CodeHash", "Timestamp" }, name: "AcceptedList");
            CreateIndex("dbo.SlideHints", new[] { "SlideId", "UserId", "HintId", "IsHintHelped" }, name: "FullIndex");
            CreateIndex("dbo.LtiSlideRequests", new[] { "SlideId", "UserId" }, name: "IDX_LtiSlideRequest_SlideAndUser");
            CreateIndex("dbo.SlideRates", new[] { "SlideId", "UserId" }, name: "SlideAndUser");
            CreateIndex("dbo.UserQuizs", new[] { "UserId", "SlideId", "isDropped", "QuizId", "ItemId" }, name: "FullIndex");
            CreateIndex("dbo.UserQuizs", new[] { "SlideId", "Timestamp" }, name: "StatisticsIndex");
            CreateIndex("dbo.Visits", new[] { "UserId", "SlideId" }, name: "IDX_Visits_UserAndSlide");
            CreateIndex("dbo.Visits", new[] { "SlideId", "Timestamp" }, name: "IDX_Visits_SlideAndTime");
        }

		/*
		For correct migration's down:
			Replace all slides' guids (in courses files) with version with hyphens (see InsertHyphensIntoGuids())
		*/
		public override void Down()
        {
            DropIndex("dbo.Visits", "IDX_Visits_SlideAndTime");
            DropIndex("dbo.Visits", "IDX_Visits_UserAndSlide");
            DropIndex("dbo.UserQuizs", "StatisticsIndex");
            DropIndex("dbo.UserQuizs", "FullIndex");
            DropIndex("dbo.SlideRates", "SlideAndUser");
            DropIndex("dbo.LtiSlideRequests", "IDX_LtiSlideRequest_SlideAndUser");
            DropIndex("dbo.SlideHints", "FullIndex");
            DropIndex("dbo.UserSolutions", "AcceptedList");
            DropIndex("dbo.Comments", "IDX_Comment_CommentBySlide");
            AlterColumn("dbo.Visits", "SlideId", c => c.String(nullable: false, maxLength: 64));
			AlterColumn("dbo.UserQuizs", "SlideId", c => c.String(nullable: false, maxLength: 64));
			AlterColumn("dbo.SlideRates", "SlideId", c => c.String(nullable: false, maxLength: 64));
            AlterColumn("dbo.LtiSlideRequests", "SlideId", c => c.String(nullable: false, maxLength: 64));
            AlterColumn("dbo.SlideHints", "SlideId", c => c.String(nullable: false, maxLength: 64));
            AlterColumn("dbo.UserSolutions", "SlideId", c => c.String(nullable: false, maxLength: 64));
			Sql("UPDATE dbo.UserSolutions SET SlideId = 'runner' WHERE SlideId = '00000000-0000-0000-0000-000000000000' AND CourseId = 'web'");
			AlterColumn("dbo.UserQuestions", "SlideId", c => c.String(maxLength: 64));
            AlterColumn("dbo.Comments", "SlideId", c => c.String(nullable: false, maxLength: 64));
            CreateIndex("dbo.Visits", new[] { "SlideId", "Timestamp" }, name: "IDX_Visits_SlideAndTime");
            CreateIndex("dbo.Visits", new[] { "UserId", "SlideId" }, name: "IDX_Visits_UserAndSlide");
            CreateIndex("dbo.UserQuizs", new[] { "SlideId", "Timestamp" }, name: "StatisticsIndex");
            CreateIndex("dbo.UserQuizs", new[] { "UserId", "SlideId", "isDropped", "QuizId", "ItemId" }, name: "FullIndex");
            CreateIndex("dbo.SlideRates", new[] { "SlideId", "UserId" }, name: "SlideAndUser");
            CreateIndex("dbo.LtiSlideRequests", new[] { "SlideId", "UserId" }, name: "IDX_LtiSlideRequest_SlideAndUser");
            CreateIndex("dbo.SlideHints", new[] { "SlideId", "UserId", "HintId", "IsHintHelped" }, name: "FullIndex");
            CreateIndex("dbo.UserSolutions", new[] { "SlideId", "IsRightAnswer", "CodeHash", "Timestamp" }, name: "AcceptedList");
            CreateIndex("dbo.Comments", "SlideId", name: "IDX_Comment_CommentBySlide");
        }
    }
}
