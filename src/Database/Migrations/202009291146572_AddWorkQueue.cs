namespace Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddWorkQueue : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.WorkQueueItems",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        QueueId = c.Int(nullable: false),
                        ItemId = c.String(nullable: false),
                        Priority = c.Int(nullable: false),
                        Type = c.String(),
                        TakeAfterTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.WorkQueueItems");
        }
    }
}
