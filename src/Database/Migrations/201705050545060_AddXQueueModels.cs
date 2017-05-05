namespace Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddXQueueModels : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.XQueueExerciseSubmissions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SubmissionId = c.Int(nullable: false),
                        WatcherId = c.Int(nullable: false),
                        XQueueHeader = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.UserExerciseSubmissions", t => t.SubmissionId, cascadeDelete: true)
                .ForeignKey("dbo.XQueueWatchers", t => t.WatcherId, cascadeDelete: true)
                .Index(t => t.SubmissionId)
                .Index(t => t.WatcherId);
            
            CreateTable(
                "dbo.XQueueWatchers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        BaseUrl = c.String(nullable: false),
                        QueueName = c.String(nullable: false),
                        UserName = c.String(nullable: false),
                        Password = c.String(nullable: false),
                        IsEnabled = c.Boolean(nullable: false),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.XQueueExerciseSubmissions", "WatcherId", "dbo.XQueueWatchers");
            DropForeignKey("dbo.XQueueWatchers", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.XQueueExerciseSubmissions", "SubmissionId", "dbo.UserExerciseSubmissions");
            DropIndex("dbo.XQueueWatchers", new[] { "UserId" });
            DropIndex("dbo.XQueueExerciseSubmissions", new[] { "WatcherId" });
            DropIndex("dbo.XQueueExerciseSubmissions", new[] { "SubmissionId" });
            DropTable("dbo.XQueueWatchers");
            DropTable("dbo.XQueueExerciseSubmissions");
        }
    }
}
