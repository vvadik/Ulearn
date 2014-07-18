namespace uLearn.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SolutionLikes : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Likes",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        SolutionId = c.Int(nullable: false),
                        UserId = c.String(nullable: false),
                        Timestamp = c.DateTime(nullable: false),
                        UserSolution_Id = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.UserSolutions", t => t.UserSolution_Id)
                .Index(t => t.UserSolution_Id);
            
            AddColumn("dbo.UserSolutions", "CodeHash", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Likes", "UserSolution_Id", "dbo.UserSolutions");
            DropIndex("dbo.Likes", new[] { "UserSolution_Id" });
            DropColumn("dbo.UserSolutions", "CodeHash");
            DropTable("dbo.Likes");
        }
    }
}
