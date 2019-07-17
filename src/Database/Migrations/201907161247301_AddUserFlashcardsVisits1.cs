namespace Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUserFlashcardsVisits1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserFlashcardsVisits", "Rate", c => c.Int(nullable: false));
            DropColumn("dbo.UserFlashcardsVisits", "Score");
        }
        
        public override void Down()
        {
            AddColumn("dbo.UserFlashcardsVisits", "Score", c => c.Int(nullable: false));
            DropColumn("dbo.UserFlashcardsVisits", "Rate");
        }
    }
}
