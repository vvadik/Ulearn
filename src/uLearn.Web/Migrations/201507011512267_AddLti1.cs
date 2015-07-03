namespace uLearn.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddLti1 : DbMigration
    {
        public override void Up()
        {
            DropTable("dbo.Scores");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Scores",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        Value = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.UserId);
            
        }
    }
}
