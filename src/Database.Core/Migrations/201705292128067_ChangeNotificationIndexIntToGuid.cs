namespace Database.Migrations
{
	using System;
	using System.Data.Entity.Migrations;

	public partial class ChangeNotificationIndexIntToGuid : DbMigration
	{
		public override void Up()
		{
			DropIndex("dbo.Notifications", new[] { "CourseVersion_Id" });
			DropIndex("dbo.Notifications", new[] { "CourseVersion_Id1" });
			DropColumn("dbo.Notifications", "CourseVersionId");
			DropColumn("dbo.Notifications", "CourseVersionId1");
			RenameColumn(table: "dbo.Notifications", name: "CourseVersion_Id", newName: "CourseVersionId");
			RenameColumn(table: "dbo.Notifications", name: "CourseVersion_Id1", newName: "CourseVersionId1");
			AlterColumn("dbo.Notifications", "CourseVersionId", c => c.Guid());
			AlterColumn("dbo.Notifications", "CourseVersionId1", c => c.Guid());
			CreateIndex("dbo.Notifications", "CourseVersionId");
			CreateIndex("dbo.Notifications", "CourseVersionId1");
		}

		public override void Down()
		{
			DropIndex("dbo.Notifications", new[] { "CourseVersionId1" });
			DropIndex("dbo.Notifications", new[] { "CourseVersionId" });
			AlterColumn("dbo.Notifications", "CourseVersionId1", c => c.Int());
			AlterColumn("dbo.Notifications", "CourseVersionId", c => c.Int());
			RenameColumn(table: "dbo.Notifications", name: "CourseVersionId1", newName: "CourseVersion_Id1");
			RenameColumn(table: "dbo.Notifications", name: "CourseVersionId", newName: "CourseVersion_Id");
			AddColumn("dbo.Notifications", "CourseVersionId1", c => c.Int());
			AddColumn("dbo.Notifications", "CourseVersionId", c => c.Int());
			CreateIndex("dbo.Notifications", "CourseVersion_Id1");
			CreateIndex("dbo.Notifications", "CourseVersion_Id");
		}
	}
}