namespace Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddQuizIgnoreInAttemptsCount : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AutomaticQuizCheckings", "IgnoreInAttemptsCount", c => c.Boolean(nullable: false));
            AddColumn("dbo.ManualQuizCheckings", "IgnoreInAttemptsCount", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ManualQuizCheckings", "IgnoreInAttemptsCount");
            DropColumn("dbo.AutomaticQuizCheckings", "IgnoreInAttemptsCount");
        }
    }
}
