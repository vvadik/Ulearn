namespace uLearn.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeVisitsIndex : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Visiters", "SlideAndUser");
            CreateIndex("dbo.Visiters", new[] { "UserId", "SlideId" }, name: "SlideAndUser");
            CreateIndex("dbo.Visiters", new[] { "SlideId", "Timestamp" }, name: "SlideAndTime");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Visiters", "SlideAndTime");
            DropIndex("dbo.Visiters", "SlideAndUser");
            CreateIndex("dbo.Visiters", new[] { "SlideId", "UserId" }, name: "SlideAndUser");
        }
    }
}
