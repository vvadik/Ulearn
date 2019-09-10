namespace Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserExerciseSubmissions_ByTime : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.UserExerciseSubmissions", "Timestamp", name: "IDX_UserExerciseSubmissions_ByTime");
        }
        
        public override void Down()
        {
            DropIndex("dbo.UserExerciseSubmissions", "IDX_UserExerciseSubmissions_ByTime");
        }
    }
}
