namespace Database.Migrations
{
	using System.Data.Entity.Migrations;
    
    public partial class RemoveGroupIsPublic : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Groups", "IsPublic");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Groups", "IsPublic", c => c.Boolean(nullable: false));
        }
    }
}
