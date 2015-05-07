namespace uLearn.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveTextBlobTextRequired : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.TextBlobs", "Text", c => c.String(maxLength: 4000));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.TextBlobs", "Text", c => c.String(nullable: false, maxLength: 4000));
        }
    }
}
