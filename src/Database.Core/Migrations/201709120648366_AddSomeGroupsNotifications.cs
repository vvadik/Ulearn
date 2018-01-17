namespace Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSomeGroupsNotifications : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Notifications", name: "GroupId1", newName: "GroupId2");
            RenameIndex(table: "dbo.Notifications", name: "IX_GroupId1", newName: "IX_GroupId2");
            AddColumn("dbo.Notifications", "AccessId", c => c.Int());
            AddColumn("dbo.Notifications", "UserId", c => c.String(maxLength: 128));
            AddColumn("dbo.Notifications", "AccessId1", c => c.Int());
	        AddColumn("dbo.Notifications", "GroupId1", c => c.Int());
			CreateIndex("dbo.Notifications", "AccessId");
            CreateIndex("dbo.Notifications", "UserId");
            CreateIndex("dbo.Notifications", "GroupId1");
            CreateIndex("dbo.Notifications", "AccessId1");
            AddForeignKey("dbo.Notifications", "AccessId", "dbo.GroupAccesses", "Id");
            AddForeignKey("dbo.Notifications", "GroupId2", "dbo.Groups", "Id");
            AddForeignKey("dbo.Notifications", "UserId", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.Notifications", "AccessId1", "dbo.GroupAccesses", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Notifications", "AccessId1", "dbo.GroupAccesses");
            DropForeignKey("dbo.Notifications", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Notifications", "GroupId2", "dbo.Groups");
            DropForeignKey("dbo.Notifications", "AccessId", "dbo.GroupAccesses");
            DropIndex("dbo.Notifications", new[] { "AccessId1" });
            DropIndex("dbo.Notifications", new[] { "GroupId1" });
            DropIndex("dbo.Notifications", new[] { "UserId" });
            DropIndex("dbo.Notifications", new[] { "AccessId" });
	        DropColumn("dbo.Notifications", "GroupId1");
			DropColumn("dbo.Notifications", "AccessId1");
            DropColumn("dbo.Notifications", "UserId");
            DropColumn("dbo.Notifications", "AccessId");
            RenameIndex(table: "dbo.Notifications", name: "IX_GroupId2", newName: "IX_GroupId1");
            RenameColumn(table: "dbo.Notifications", name: "GroupId2", newName: "GroupId1");
        }
    }
}
