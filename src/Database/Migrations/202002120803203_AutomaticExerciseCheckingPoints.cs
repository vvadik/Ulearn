namespace Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AutomaticExerciseCheckingPoints : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AutomaticExerciseCheckings", "Points", c => c.Single());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AutomaticExerciseCheckings", "Points");
        }
    }
}
