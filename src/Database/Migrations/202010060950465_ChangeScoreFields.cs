namespace Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeScoreFields : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ManualExerciseCheckings", "Percent", c => c.Int());
            AlterColumn("dbo.AutomaticExerciseCheckings", "Score", c => c.Int());
            AlterColumn("dbo.ManualExerciseCheckings", "Score", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ManualExerciseCheckings", "Score", c => c.Int(nullable: false));
            AlterColumn("dbo.AutomaticExerciseCheckings", "Score", c => c.Int(nullable: false));
            DropColumn("dbo.ManualExerciseCheckings", "Percent");
        }
    }
}
