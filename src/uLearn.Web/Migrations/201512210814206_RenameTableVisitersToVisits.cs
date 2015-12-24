namespace uLearn.Web.Migrations
{
	using System.Data.Entity.Migrations;
    
    public partial class RenameTableVisitersToVisits : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.Visiters", newName: "Visits");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.Visits", newName: "Visiters");
        }
    }
}
