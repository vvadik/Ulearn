namespace Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSandboxToUserExerciseSubmission : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserExerciseSubmissions", "Sandbox", c => c.String(maxLength: 40));
            CreateIndex("dbo.UserExerciseSubmissions", "Sandbox", name: "IDX_UserExerciseSubmissions_BySandbox");
        }
        
        public override void Down()
        {
            DropIndex("dbo.UserExerciseSubmissions", "IDX_UserExerciseSubmissions_BySandbox");
            DropColumn("dbo.UserExerciseSubmissions", "Sandbox");
        }
    }
}
