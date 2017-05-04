using System.Data.Entity.Migrations;

namespace Database.Migrations
{
	public partial class AddNotificationsFields : DbMigration
	{
		public override void Up()
		{
			DropIndex("dbo.Notifications", new[] { "InitiatedById" });
			AddColumn("dbo.Notifications", "CommentId", c => c.Int());
			AddColumn("dbo.Notifications", "LikedUserId", c => c.String(maxLength: 128));
			AddColumn("dbo.Notifications", "AddedUserId", c => c.String(maxLength: 128));
			AddColumn("dbo.Notifications", "GroupId", c => c.Int());
			AddColumn("dbo.Notifications", "Text", c => c.String());
			AddColumn("dbo.Notifications", "GroupId1", c => c.Int());
			AddColumn("dbo.Notifications", "JoinedUserId", c => c.String(maxLength: 128));
			AddColumn("dbo.Notifications", "ErrorId", c => c.String(maxLength: 100));
			AddColumn("dbo.Notifications", "ErrorMessage", c => c.String());
			AddColumn("dbo.Notifications", "CheckingId", c => c.Int());
			AddColumn("dbo.Notifications", "CheckingId1", c => c.Int());
			AddColumn("dbo.Notifications", "CourseVersionId", c => c.Int());
			AddColumn("dbo.Notifications", "ScoreId", c => c.Int());
			AddColumn("dbo.Notifications", "CertificateId", c => c.Guid());
			AddColumn("dbo.Notifications", "Text1", c => c.String());
			AddColumn("dbo.Notifications", "CourseVersionId1", c => c.Int());
			AddColumn("dbo.Notifications", "CourseVersion_Id", c => c.Guid());
			AddColumn("dbo.Notifications", "CourseVersion_Id1", c => c.Guid());
			AddColumn("dbo.NotificationTransports", "ChatTitle", c => c.String(maxLength: 200));
			AlterColumn("dbo.Notifications", "CourseId", c => c.String(nullable: false, maxLength: 100));
			AlterColumn("dbo.Notifications", "InitiatedById", c => c.String(nullable: false, maxLength: 128));
			CreateIndex("dbo.Notifications", "CourseId", name: "IDX_Notification_ByCourse");
			CreateIndex("dbo.Notifications", "InitiatedById");
			CreateIndex("dbo.Notifications", "CreateTime", name: "IDX_Notification_ByCreateTime");
			CreateIndex("dbo.Notifications", "CommentId");
			CreateIndex("dbo.Notifications", "LikedUserId");
			CreateIndex("dbo.Notifications", "AddedUserId");
			CreateIndex("dbo.Notifications", "GroupId");
			CreateIndex("dbo.Notifications", "GroupId1");
			CreateIndex("dbo.Notifications", "JoinedUserId");
			CreateIndex("dbo.Notifications", "CheckingId");
			CreateIndex("dbo.Notifications", "CheckingId1");
			CreateIndex("dbo.Notifications", "ScoreId");
			CreateIndex("dbo.Notifications", "CertificateId");
			CreateIndex("dbo.Notifications", "CourseVersion_Id");
			CreateIndex("dbo.Notifications", "CourseVersion_Id1");
			AddForeignKey("dbo.Notifications", "CommentId", "dbo.Comments", "Id", cascadeDelete: true);
			AddForeignKey("dbo.Notifications", "LikedUserId", "dbo.AspNetUsers", "Id");
			AddForeignKey("dbo.Notifications", "AddedUserId", "dbo.AspNetUsers", "Id");
			AddForeignKey("dbo.Notifications", "GroupId", "dbo.Groups", "Id");
			AddForeignKey("dbo.Notifications", "GroupId1", "dbo.Groups", "Id");
			AddForeignKey("dbo.Notifications", "JoinedUserId", "dbo.AspNetUsers", "Id");
			AddForeignKey("dbo.Notifications", "CheckingId", "dbo.ManualExerciseCheckings", "Id");
			AddForeignKey("dbo.Notifications", "CheckingId1", "dbo.ManualQuizCheckings", "Id");
			AddForeignKey("dbo.Notifications", "CourseVersion_Id", "dbo.CourseVersions", "Id");
			AddForeignKey("dbo.Notifications", "ScoreId", "dbo.AdditionalScores", "Id", cascadeDelete: true);
			AddForeignKey("dbo.Notifications", "CertificateId", "dbo.Certificates", "Id", cascadeDelete: true);
			AddForeignKey("dbo.Notifications", "CourseVersion_Id1", "dbo.CourseVersions", "Id");
		}

		public override void Down()
		{
			DropForeignKey("dbo.Notifications", "CourseVersion_Id1", "dbo.CourseVersions");
			DropForeignKey("dbo.Notifications", "CertificateId", "dbo.Certificates");
			DropForeignKey("dbo.Notifications", "ScoreId", "dbo.AdditionalScores");
			DropForeignKey("dbo.Notifications", "CourseVersion_Id", "dbo.CourseVersions");
			DropForeignKey("dbo.Notifications", "CheckingId1", "dbo.ManualQuizCheckings");
			DropForeignKey("dbo.Notifications", "CheckingId", "dbo.ManualExerciseCheckings");
			DropForeignKey("dbo.Notifications", "JoinedUserId", "dbo.AspNetUsers");
			DropForeignKey("dbo.Notifications", "GroupId1", "dbo.Groups");
			DropForeignKey("dbo.Notifications", "GroupId", "dbo.Groups");
			DropForeignKey("dbo.Notifications", "AddedUserId", "dbo.AspNetUsers");
			DropForeignKey("dbo.Notifications", "LikedUserId", "dbo.AspNetUsers");
			DropForeignKey("dbo.Notifications", "CommentId", "dbo.Comments");
			DropIndex("dbo.Notifications", new[] { "CourseVersion_Id1" });
			DropIndex("dbo.Notifications", new[] { "CourseVersion_Id" });
			DropIndex("dbo.Notifications", new[] { "CertificateId" });
			DropIndex("dbo.Notifications", new[] { "ScoreId" });
			DropIndex("dbo.Notifications", new[] { "CheckingId1" });
			DropIndex("dbo.Notifications", new[] { "CheckingId" });
			DropIndex("dbo.Notifications", new[] { "JoinedUserId" });
			DropIndex("dbo.Notifications", new[] { "GroupId1" });
			DropIndex("dbo.Notifications", new[] { "GroupId" });
			DropIndex("dbo.Notifications", new[] { "AddedUserId" });
			DropIndex("dbo.Notifications", new[] { "LikedUserId" });
			DropIndex("dbo.Notifications", new[] { "CommentId" });
			DropIndex("dbo.Notifications", "IDX_Notification_ByCreateTime");
			DropIndex("dbo.Notifications", new[] { "InitiatedById" });
			DropIndex("dbo.Notifications", "IDX_Notification_ByCourse");
			AlterColumn("dbo.Notifications", "InitiatedById", c => c.String(maxLength: 128));
			AlterColumn("dbo.Notifications", "CourseId", c => c.String(maxLength: 100));
			DropColumn("dbo.NotificationTransports", "ChatTitle");
			DropColumn("dbo.Notifications", "CourseVersion_Id1");
			DropColumn("dbo.Notifications", "CourseVersion_Id");
			DropColumn("dbo.Notifications", "CourseVersionId1");
			DropColumn("dbo.Notifications", "Text1");
			DropColumn("dbo.Notifications", "CertificateId");
			DropColumn("dbo.Notifications", "ScoreId");
			DropColumn("dbo.Notifications", "CourseVersionId");
			DropColumn("dbo.Notifications", "CheckingId1");
			DropColumn("dbo.Notifications", "CheckingId");
			DropColumn("dbo.Notifications", "ErrorMessage");
			DropColumn("dbo.Notifications", "ErrorId");
			DropColumn("dbo.Notifications", "JoinedUserId");
			DropColumn("dbo.Notifications", "GroupId1");
			DropColumn("dbo.Notifications", "Text");
			DropColumn("dbo.Notifications", "GroupId");
			DropColumn("dbo.Notifications", "AddedUserId");
			DropColumn("dbo.Notifications", "LikedUserId");
			DropColumn("dbo.Notifications", "CommentId");
			CreateIndex("dbo.Notifications", "InitiatedById");
		}
	}
}