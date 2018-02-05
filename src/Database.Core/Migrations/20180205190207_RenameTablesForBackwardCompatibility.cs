using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Database.Migrations
{
    public partial class RenameTablesForBackwardCompatibility : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AutomaticExerciseCheckings_Texts_CompilationErrorHash",
                table: "AutomaticExerciseCheckings");

            migrationBuilder.DropForeignKey(
                name: "FK_AutomaticExerciseCheckings_Texts_OutputHash",
                table: "AutomaticExerciseCheckings");

            migrationBuilder.DropForeignKey(
                name: "FK_Hints_AspNetUsers_UserId",
                table: "Hints");

            migrationBuilder.DropForeignKey(
                name: "FK_LabelsOnGroups_Groups_GroupId",
                table: "LabelsOnGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_LabelsOnGroups_GroupLabels_LabelId",
                table: "LabelsOnGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_SolutionLikes_UserExerciseSubmissions_SubmissionId",
                table: "SolutionLikes");

            migrationBuilder.DropForeignKey(
                name: "FK_SolutionLikes_AspNetUsers_UserId",
                table: "SolutionLikes");

            migrationBuilder.DropForeignKey(
                name: "FK_UserExerciseSubmissions_Texts_SolutionCodeHash",
                table: "UserExerciseSubmissions");

            migrationBuilder.DropForeignKey(
                name: "FK_UserQuizzes_QuizVersions_QuizVersionId",
                table: "UserQuizzes");

            migrationBuilder.DropForeignKey(
                name: "FK_UserQuizzes_AspNetUsers_UserId",
                table: "UserQuizzes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserQuizzes",
                table: "UserQuizzes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Texts",
                table: "Texts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SolutionLikes",
                table: "SolutionLikes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LtiRequests",
                table: "LtiRequests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LabelsOnGroups",
                table: "LabelsOnGroups");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Hints",
                table: "Hints");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Consumers",
                table: "Consumers");

            migrationBuilder.RenameTable(
                name: "UserQuizzes",
                newName: "UserQuizs");

            migrationBuilder.RenameTable(
                name: "Texts",
                newName: "TextBlobs");

            migrationBuilder.RenameTable(
                name: "SolutionLikes",
                newName: "Likes");

            migrationBuilder.RenameTable(
                name: "LtiRequests",
                newName: "LtiSlideRequests");

            migrationBuilder.RenameTable(
                name: "LabelsOnGroups",
                newName: "LabelOnGroups");

            migrationBuilder.RenameTable(
                name: "Hints",
                newName: "SlideHints");

            migrationBuilder.RenameTable(
                name: "Consumers",
                newName: "LtiConsumers");

            migrationBuilder.RenameIndex(
                name: "IX_UserQuizzes_UserId_SlideId_isDropped_QuizId_ItemId",
                table: "UserQuizs",
                newName: "IX_UserQuizs_UserId_SlideId_isDropped_QuizId_ItemId");

            migrationBuilder.RenameIndex(
                name: "IX_UserQuizzes_SlideId_Timestamp",
                table: "UserQuizs",
                newName: "IX_UserQuizs_SlideId_Timestamp");

            migrationBuilder.RenameIndex(
                name: "IX_UserQuizzes_QuizVersionId",
                table: "UserQuizs",
                newName: "IX_UserQuizs_QuizVersionId");

            migrationBuilder.RenameIndex(
                name: "IX_SolutionLikes_UserId_SubmissionId",
                table: "Likes",
                newName: "IX_Likes_UserId_SubmissionId");

            migrationBuilder.RenameIndex(
                name: "IX_SolutionLikes_SubmissionId",
                table: "Likes",
                newName: "IX_Likes_SubmissionId");

            migrationBuilder.RenameIndex(
                name: "IX_LtiRequests_SlideId_UserId",
                table: "LtiSlideRequests",
                newName: "IX_LtiSlideRequests_SlideId_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_LabelsOnGroups_GroupId_LabelId",
                table: "LabelOnGroups",
                newName: "IX_LabelOnGroups_GroupId_LabelId");

            migrationBuilder.RenameIndex(
                name: "IX_LabelsOnGroups_LabelId",
                table: "LabelOnGroups",
                newName: "IX_LabelOnGroups_LabelId");

            migrationBuilder.RenameIndex(
                name: "IX_LabelsOnGroups_GroupId",
                table: "LabelOnGroups",
                newName: "IX_LabelOnGroups_GroupId");

            migrationBuilder.RenameIndex(
                name: "IX_Hints_SlideId_HintId_UserId_IsHintHelped",
                table: "SlideHints",
                newName: "IX_SlideHints_SlideId_HintId_UserId_IsHintHelped");

            migrationBuilder.RenameIndex(
                name: "IX_Hints_UserId",
                table: "SlideHints",
                newName: "IX_SlideHints_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Consumers_Key",
                table: "LtiConsumers",
                newName: "IX_LtiConsumers_Key");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserQuizs",
                table: "UserQuizs",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TextBlobs",
                table: "TextBlobs",
                column: "Hash");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Likes",
                table: "Likes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LtiSlideRequests",
                table: "LtiSlideRequests",
                column: "RequestId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LabelOnGroups",
                table: "LabelOnGroups",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SlideHints",
                table: "SlideHints",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LtiConsumers",
                table: "LtiConsumers",
                column: "ConsumerId");

            migrationBuilder.AddForeignKey(
                name: "FK_AutomaticExerciseCheckings_TextBlobs_CompilationErrorHash",
                table: "AutomaticExerciseCheckings",
                column: "CompilationErrorHash",
                principalTable: "TextBlobs",
                principalColumn: "Hash",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AutomaticExerciseCheckings_TextBlobs_OutputHash",
                table: "AutomaticExerciseCheckings",
                column: "OutputHash",
                principalTable: "TextBlobs",
                principalColumn: "Hash",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LabelOnGroups_Groups_GroupId",
                table: "LabelOnGroups",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LabelOnGroups_GroupLabels_LabelId",
                table: "LabelOnGroups",
                column: "LabelId",
                principalTable: "GroupLabels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Likes_UserExerciseSubmissions_SubmissionId",
                table: "Likes",
                column: "SubmissionId",
                principalTable: "UserExerciseSubmissions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Likes_AspNetUsers_UserId",
                table: "Likes",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SlideHints_AspNetUsers_UserId",
                table: "SlideHints",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserExerciseSubmissions_TextBlobs_SolutionCodeHash",
                table: "UserExerciseSubmissions",
                column: "SolutionCodeHash",
                principalTable: "TextBlobs",
                principalColumn: "Hash",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserQuizs_QuizVersions_QuizVersionId",
                table: "UserQuizs",
                column: "QuizVersionId",
                principalTable: "QuizVersions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserQuizs_AspNetUsers_UserId",
                table: "UserQuizs",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AutomaticExerciseCheckings_TextBlobs_CompilationErrorHash",
                table: "AutomaticExerciseCheckings");

            migrationBuilder.DropForeignKey(
                name: "FK_AutomaticExerciseCheckings_TextBlobs_OutputHash",
                table: "AutomaticExerciseCheckings");

            migrationBuilder.DropForeignKey(
                name: "FK_LabelOnGroups_Groups_GroupId",
                table: "LabelOnGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_LabelOnGroups_GroupLabels_LabelId",
                table: "LabelOnGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_Likes_UserExerciseSubmissions_SubmissionId",
                table: "Likes");

            migrationBuilder.DropForeignKey(
                name: "FK_Likes_AspNetUsers_UserId",
                table: "Likes");

            migrationBuilder.DropForeignKey(
                name: "FK_SlideHints_AspNetUsers_UserId",
                table: "SlideHints");

            migrationBuilder.DropForeignKey(
                name: "FK_UserExerciseSubmissions_TextBlobs_SolutionCodeHash",
                table: "UserExerciseSubmissions");

            migrationBuilder.DropForeignKey(
                name: "FK_UserQuizs_QuizVersions_QuizVersionId",
                table: "UserQuizs");

            migrationBuilder.DropForeignKey(
                name: "FK_UserQuizs_AspNetUsers_UserId",
                table: "UserQuizs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserQuizs",
                table: "UserQuizs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TextBlobs",
                table: "TextBlobs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SlideHints",
                table: "SlideHints");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LtiSlideRequests",
                table: "LtiSlideRequests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LtiConsumers",
                table: "LtiConsumers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Likes",
                table: "Likes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LabelOnGroups",
                table: "LabelOnGroups");

            migrationBuilder.RenameTable(
                name: "UserQuizs",
                newName: "UserQuizzes");

            migrationBuilder.RenameTable(
                name: "TextBlobs",
                newName: "Texts");

            migrationBuilder.RenameTable(
                name: "SlideHints",
                newName: "Hints");

            migrationBuilder.RenameTable(
                name: "LtiSlideRequests",
                newName: "LtiRequests");

            migrationBuilder.RenameTable(
                name: "LtiConsumers",
                newName: "Consumers");

            migrationBuilder.RenameTable(
                name: "Likes",
                newName: "SolutionLikes");

            migrationBuilder.RenameTable(
                name: "LabelOnGroups",
                newName: "LabelsOnGroups");

            migrationBuilder.RenameIndex(
                name: "IX_UserQuizs_UserId_SlideId_isDropped_QuizId_ItemId",
                table: "UserQuizzes",
                newName: "IX_UserQuizzes_UserId_SlideId_isDropped_QuizId_ItemId");

            migrationBuilder.RenameIndex(
                name: "IX_UserQuizs_SlideId_Timestamp",
                table: "UserQuizzes",
                newName: "IX_UserQuizzes_SlideId_Timestamp");

            migrationBuilder.RenameIndex(
                name: "IX_UserQuizs_QuizVersionId",
                table: "UserQuizzes",
                newName: "IX_UserQuizzes_QuizVersionId");

            migrationBuilder.RenameIndex(
                name: "IX_SlideHints_SlideId_HintId_UserId_IsHintHelped",
                table: "Hints",
                newName: "IX_Hints_SlideId_HintId_UserId_IsHintHelped");

            migrationBuilder.RenameIndex(
                name: "IX_SlideHints_UserId",
                table: "Hints",
                newName: "IX_Hints_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_LtiSlideRequests_SlideId_UserId",
                table: "LtiRequests",
                newName: "IX_LtiRequests_SlideId_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_LtiConsumers_Key",
                table: "Consumers",
                newName: "IX_Consumers_Key");

            migrationBuilder.RenameIndex(
                name: "IX_Likes_UserId_SubmissionId",
                table: "SolutionLikes",
                newName: "IX_SolutionLikes_UserId_SubmissionId");

            migrationBuilder.RenameIndex(
                name: "IX_Likes_SubmissionId",
                table: "SolutionLikes",
                newName: "IX_SolutionLikes_SubmissionId");

            migrationBuilder.RenameIndex(
                name: "IX_LabelOnGroups_GroupId_LabelId",
                table: "LabelsOnGroups",
                newName: "IX_LabelsOnGroups_GroupId_LabelId");

            migrationBuilder.RenameIndex(
                name: "IX_LabelOnGroups_LabelId",
                table: "LabelsOnGroups",
                newName: "IX_LabelsOnGroups_LabelId");

            migrationBuilder.RenameIndex(
                name: "IX_LabelOnGroups_GroupId",
                table: "LabelsOnGroups",
                newName: "IX_LabelsOnGroups_GroupId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserQuizzes",
                table: "UserQuizzes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Texts",
                table: "Texts",
                column: "Hash");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Hints",
                table: "Hints",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LtiRequests",
                table: "LtiRequests",
                column: "RequestId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Consumers",
                table: "Consumers",
                column: "ConsumerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SolutionLikes",
                table: "SolutionLikes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LabelsOnGroups",
                table: "LabelsOnGroups",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AutomaticExerciseCheckings_Texts_CompilationErrorHash",
                table: "AutomaticExerciseCheckings",
                column: "CompilationErrorHash",
                principalTable: "Texts",
                principalColumn: "Hash",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AutomaticExerciseCheckings_Texts_OutputHash",
                table: "AutomaticExerciseCheckings",
                column: "OutputHash",
                principalTable: "Texts",
                principalColumn: "Hash",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Hints_AspNetUsers_UserId",
                table: "Hints",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LabelsOnGroups_Groups_GroupId",
                table: "LabelsOnGroups",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LabelsOnGroups_GroupLabels_LabelId",
                table: "LabelsOnGroups",
                column: "LabelId",
                principalTable: "GroupLabels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SolutionLikes_UserExerciseSubmissions_SubmissionId",
                table: "SolutionLikes",
                column: "SubmissionId",
                principalTable: "UserExerciseSubmissions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SolutionLikes_AspNetUsers_UserId",
                table: "SolutionLikes",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserExerciseSubmissions_Texts_SolutionCodeHash",
                table: "UserExerciseSubmissions",
                column: "SolutionCodeHash",
                principalTable: "Texts",
                principalColumn: "Hash",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserQuizzes_QuizVersions_QuizVersionId",
                table: "UserQuizzes",
                column: "QuizVersionId",
                principalTable: "QuizVersions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserQuizzes_AspNetUsers_UserId",
                table: "UserQuizzes",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
