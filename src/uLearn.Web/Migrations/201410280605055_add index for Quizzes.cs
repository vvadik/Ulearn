namespace uLearn.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addindexforQuizzes : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.UserQuizs", "SlideId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.UserQuizs", new[] { "SlideId" });
        }
    }
}
