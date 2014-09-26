namespace uLearn.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddQuestionSlideId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserQuestions", "SlideId", c => c.String(maxLength: 64));
            AddColumn("dbo.UserQuestions", "CourseId", c => c.String(maxLength: 64));
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserQuestions", "CourseId");
            DropColumn("dbo.UserQuestions", "SlideId");
        }
    }
}
