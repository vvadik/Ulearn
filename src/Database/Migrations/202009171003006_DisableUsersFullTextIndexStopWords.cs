namespace Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DisableUsersFullTextIndexStopWords : DbMigration
    {
        public override void Up()
        {
            Sql("ALTER FULLTEXT INDEX ON dbo.AspNetUsers SET STOPLIST = OFF", true);
        }
        
        public override void Down()
        {
            Sql("ALTER FULLTEXT INDEX ON dbo.AspNetUsers SET STOPLIST = SYSTEM", true);
        }
    }
}
