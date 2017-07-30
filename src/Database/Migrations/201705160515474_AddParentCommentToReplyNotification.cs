namespace Database.Migrations
{
	using System;
	using System.Data.Entity.Migrations;

	public partial class AddParentCommentToReplyNotification : DbMigration
	{
		public override void Up()
		{
			AddColumn("dbo.Notifications", "ParentCommentId", c => c.Int());
			CreateIndex("dbo.Notifications", "ParentCommentId");
			AddForeignKey("dbo.Notifications", "ParentCommentId", "dbo.Comments", "Id");
		}

		public override void Down()
		{
			DropForeignKey("dbo.Notifications", "ParentCommentId", "dbo.Comments");
			DropIndex("dbo.Notifications", new[] { "ParentCommentId" });
			DropColumn("dbo.Notifications", "ParentCommentId");
		}
	}
}