namespace uLearn.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddComputedColumnsForUser : DbMigration
    {
        public override void Up()
        {
			Sql("ALTER TABLE dbo.AspNetUsers ADD FirstAndLastName AS FirstName + ' ' + LastName");
			Sql("ALTER TABLE dbo.AspNetUsers ADD LastAndFirstName AS LastName + ' ' + FirstName");
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "LastAndFirstName");
            DropColumn("dbo.AspNetUsers", "FirstAndLastName");
        }
    }
}
