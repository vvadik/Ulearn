namespace uLearn.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddLike : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserSolutions", "CodeHash", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserSolutions", "CodeHash");
        }
    }
}
