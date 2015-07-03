namespace uLearn.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddLti : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Consumers",
                c => new
                    {
                        ConsumerId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Key = c.String(nullable: false),
                        Secret = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.ConsumerId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Consumers");
        }
    }
}
