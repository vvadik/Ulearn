namespace uLearn.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeLikesStorage : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Likes",
                c => new
                    {
                        SolutionId = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false),
                        Timestamp = c.DateTime(nullable: false),
                        UserSolution_Id = c.Int(),
                    })
                .PrimaryKey(t => t.SolutionId)
                .ForeignKey("dbo.UserSolutions", t => t.UserSolution_Id)
                .Index(t => t.UserSolution_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Likes", "UserSolution_Id", "dbo.UserSolutions");
            DropIndex("dbo.Likes", new[] { "UserSolution_Id" });
            DropTable("dbo.Likes");
        }
    }
}
