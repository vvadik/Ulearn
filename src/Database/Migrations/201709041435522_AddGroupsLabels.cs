namespace Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddGroupsLabels : DbMigration
    {
        public override void Up()
        {
            RenameIndex(table: "dbo.GroupMembers", name: "IDX_GroupUser_UserByGroup", newName: "IDX_GroupMember_MembersByGroup");
            RenameIndex(table: "dbo.GroupMembers", name: "IDX_GroupUser_GroupByUser", newName: "IDX_GroupMember_GroupByMember");
            CreateTable(
                "dbo.GroupLabels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OwnerId = c.String(nullable: false, maxLength: 128),
                        Name = c.String(maxLength: 100),
                        ColorHex = c.String(maxLength: 6),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.OwnerId)
                .Index(t => t.OwnerId, name: "IDX_GroupLabel_ByOwner")
                .Index(t => new { t.OwnerId, t.IsDeleted }, name: "IDX_GroupLabel_ByOwnerAndIsDeleted");
            
            CreateTable(
                "dbo.LabelOnGroups",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        GroupId = c.Int(nullable: false),
                        LabelId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Groups", t => t.GroupId)
                .ForeignKey("dbo.GroupLabels", t => t.LabelId)
                .Index(t => t.GroupId, name: "IDX_LabelOnGroup_ByGroup")
                .Index(t => new { t.GroupId, t.LabelId }, unique: true, name: "IDX_LabelOnGroup_ByGroupAndLabel")
                .Index(t => t.LabelId, name: "IDX_LabelOnGroup_ByLabel");
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.LabelOnGroups", "LabelId", "dbo.GroupLabels");
            DropForeignKey("dbo.LabelOnGroups", "GroupId", "dbo.Groups");
            DropForeignKey("dbo.GroupLabels", "OwnerId", "dbo.AspNetUsers");
            DropIndex("dbo.LabelOnGroups", "IDX_LabelOnGroup_ByLabel");
            DropIndex("dbo.LabelOnGroups", "IDX_LabelOnGroup_ByGroupAndLabel");
            DropIndex("dbo.LabelOnGroups", "IDX_LabelOnGroup_ByGroup");
            DropIndex("dbo.GroupLabels", "IDX_GroupLabel_ByOwnerAndIsDeleted");
            DropIndex("dbo.GroupLabels", "IDX_GroupLabel_ByOwner");
            DropTable("dbo.LabelOnGroups");
            DropTable("dbo.GroupLabels");
            RenameIndex(table: "dbo.GroupMembers", name: "IDX_GroupMember_GroupByMember", newName: "IDX_GroupUser_GroupByUser");
            RenameIndex(table: "dbo.GroupMembers", name: "IDX_GroupMember_MembersByGroup", newName: "IDX_GroupUser_UserByGroup");
        }
    }
}
