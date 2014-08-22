namespace uLearn.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUnitAppearance : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UnitAppearances",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CourseId = c.String(nullable: false, maxLength: 64),
                        UnitName = c.String(nullable: false),
                        UserId = c.String(nullable: false),
                        UserName = c.String(),
                        PublishTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.UnitAppearances");
        }
    }
}
