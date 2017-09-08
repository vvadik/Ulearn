namespace Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddGroupLabelsAccessAndTimes : DbMigration
    {
        public override void Up()
        {
            RenameIndex(table: "dbo.GroupMembers", name: "IDX_GroupUser_UserByGroup", newName: "IDX_GroupMember_MembersByGroup");
            RenameIndex(table: "dbo.GroupMembers", name: "IDX_GroupUser_GroupByUser", newName: "IDX_GroupMember_GroupByMember");
            CreateTable(
                "dbo.GroupAccesses",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        GroupId = c.Int(nullable: false),
                        UserId = c.String(maxLength: 128),
                        GrantedById = c.String(maxLength: 128),
                        AccessType = c.Short(nullable: false),
                        GrantTime = c.DateTime(nullable: false),
                        IsEnabled = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.GrantedById)
                .ForeignKey("dbo.Groups", t => t.GroupId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.GroupId, name: "IDX_GroupAccess_ByGroup")
                .Index(t => new { t.GroupId, t.IsEnabled }, name: "IDX_GroupAccess_ByGroupAndIsEnabled")
                .Index(t => new { t.GroupId, t.UserId, t.IsEnabled }, name: "IDX_GroupAccess_ByGroupUserAndIsEnabled")
                .Index(t => t.UserId, name: "IDX_GroupAccess_ByUser")
                .Index(t => t.GrantedById)
                .Index(t => t.GrantTime, name: "IDX_GroupAccess_ByGrantTime");
            
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
            
            AddColumn("dbo.Groups", "CreateTime", c => c.DateTime());
            AddColumn("dbo.GroupMembers", "AddingTime", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.LabelOnGroups", "LabelId", "dbo.GroupLabels");
            DropForeignKey("dbo.LabelOnGroups", "GroupId", "dbo.Groups");
            DropForeignKey("dbo.GroupLabels", "OwnerId", "dbo.AspNetUsers");
            DropForeignKey("dbo.GroupAccesses", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.GroupAccesses", "GroupId", "dbo.Groups");
            DropForeignKey("dbo.GroupAccesses", "GrantedById", "dbo.AspNetUsers");
            DropIndex("dbo.LabelOnGroups", "IDX_LabelOnGroup_ByLabel");
            DropIndex("dbo.LabelOnGroups", "IDX_LabelOnGroup_ByGroupAndLabel");
            DropIndex("dbo.LabelOnGroups", "IDX_LabelOnGroup_ByGroup");
            DropIndex("dbo.GroupLabels", "IDX_GroupLabel_ByOwnerAndIsDeleted");
            DropIndex("dbo.GroupLabels", "IDX_GroupLabel_ByOwner");
            DropIndex("dbo.GroupAccesses", "IDX_GroupAccess_ByGrantTime");
            DropIndex("dbo.GroupAccesses", new[] { "GrantedById" });
            DropIndex("dbo.GroupAccesses", "IDX_GroupAccess_ByUser");
            DropIndex("dbo.GroupAccesses", "IDX_GroupAccess_ByGroupUserAndIsEnabled");
            DropIndex("dbo.GroupAccesses", "IDX_GroupAccess_ByGroupAndIsEnabled");
            DropIndex("dbo.GroupAccesses", "IDX_GroupAccess_ByGroup");
            DropColumn("dbo.GroupMembers", "AddingTime");
            DropColumn("dbo.Groups", "CreateTime");
            DropTable("dbo.LabelOnGroups");
            DropTable("dbo.GroupLabels");
            DropTable("dbo.GroupAccesses");
            RenameIndex(table: "dbo.GroupMembers", name: "IDX_GroupMember_GroupByMember", newName: "IDX_GroupUser_GroupByUser");
            RenameIndex(table: "dbo.GroupMembers", name: "IDX_GroupMember_MembersByGroup", newName: "IDX_GroupUser_UserByGroup");
        }
    }
}
