namespace Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSystemAccesses : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SystemAccesses",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        GrantedById = c.String(nullable: false, maxLength: 128),
                        AccessType = c.Short(nullable: false),
                        GrantTime = c.DateTime(nullable: false),
                        IsEnabled = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.GrantedById)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId, name: "IDX_SystemAccess_ByUser")
                .Index(t => new { t.UserId, t.IsEnabled }, name: "IDX_SystemAccess_ByUserAndIsEnabled")
                .Index(t => t.GrantedById)
                .Index(t => t.GrantTime, name: "IDX_SystemAccess_ByGrantTime")
                .Index(t => t.IsEnabled, name: "IDX_SystemAccess_ByIsEnabled");
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SystemAccesses", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.SystemAccesses", "GrantedById", "dbo.AspNetUsers");
            DropIndex("dbo.SystemAccesses", "IDX_SystemAccess_ByIsEnabled");
            DropIndex("dbo.SystemAccesses", "IDX_SystemAccess_ByGrantTime");
            DropIndex("dbo.SystemAccesses", new[] { "GrantedById" });
            DropIndex("dbo.SystemAccesses", "IDX_SystemAccess_ByUserAndIsEnabled");
            DropIndex("dbo.SystemAccesses", "IDX_SystemAccess_ByUser");
            DropTable("dbo.SystemAccesses");
        }
    }
}
