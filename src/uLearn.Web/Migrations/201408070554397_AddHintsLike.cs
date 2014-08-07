namespace uLearn.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddHintsLike : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SlideHints", "IsHintHelped", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SlideHints", "IsHintHelped");
        }
    }
}
