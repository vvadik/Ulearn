namespace uLearn.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddGroups : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.GroupMembers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        GroupId = c.Int(nullable: false),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Groups", t => t.GroupId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.GroupId, name: "IDX_GroupUser_UserByGroup")
                .Index(t => t.UserId, name: "IDX_GroupUser_GroupByUser");
            
            CreateTable(
                "dbo.Groups",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CourseId = c.String(nullable: false, maxLength: 64),
                        Name = c.String(nullable: false, maxLength: 300),
                        OwnerId = c.String(nullable: false, maxLength: 128),
                        IsPublic = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        InviteHash = c.Guid(nullable: false, identity: true),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.OwnerId, cascadeDelete: true)
                .Index(t => t.CourseId, name: "IDX_Group_GroupByCourse")
                .Index(t => t.OwnerId, name: "IDX_Group_GroupByOwner")
                .Index(t => t.InviteHash, name: "IDX_Group_GroupByInviteHash");
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.GroupMembers", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.GroupMembers", "GroupId", "dbo.Groups");
            DropForeignKey("dbo.Groups", "OwnerId", "dbo.AspNetUsers");
            DropIndex("dbo.Groups", "IDX_Group_GroupByInviteHash");
            DropIndex("dbo.Groups", "IDX_Group_GroupByOwner");
            DropIndex("dbo.Groups", "IDX_Group_GroupByCourse");
            DropIndex("dbo.GroupMembers", "IDX_GroupUser_GroupByUser");
            DropIndex("dbo.GroupMembers", "IDX_GroupUser_UserByGroup");
            DropTable("dbo.Groups");
            DropTable("dbo.GroupMembers");
        }
    }
}
