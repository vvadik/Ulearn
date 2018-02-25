namespace Database.Migrations
{
	using System.Data.Entity.Migrations;
    
    public partial class AddKonturLoginToApplicationUser : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "KonturLogin", c => c.String(maxLength: 200));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "KonturLogin");
        }
    }
}
