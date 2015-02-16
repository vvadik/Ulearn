namespace uLearn.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AcceptedSolutionsIndex : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.UserSolutions", new[] { "IsRightAnswer" });
            CreateIndex("dbo.UserSolutions", new[] { "SlideId", "IsRightAnswer", "CodeHash", "Timestamp" }, name: "AcceptedList");
        }
        
        public override void Down()
        {
            DropIndex("dbo.UserSolutions", "AcceptedList");
            CreateIndex("dbo.UserSolutions", "IsRightAnswer");
        }
    }
}
