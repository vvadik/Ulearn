namespace Database.Migrations
{
	using System.Data.Entity.Migrations;
    
    public partial class AddIsResultSentForXQueueExerciseSubmission : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.XQueueExerciseSubmissions", "IsResultSent", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.XQueueExerciseSubmissions", "IsResultSent");
        }
    }
}
