namespace uLearn.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MoreSpace_For_UserSolution_Fields : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.UserSolutions", "Code", c => c.String(nullable: false));
            AlterColumn("dbo.UserSolutions", "CompilationError", c => c.String());
            AlterColumn("dbo.UserSolutions", "Output", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.UserSolutions", "Output", c => c.String(maxLength: 1024));
            AlterColumn("dbo.UserSolutions", "CompilationError", c => c.String(maxLength: 1024));
            AlterColumn("dbo.UserSolutions", "Code", c => c.String(nullable: false, maxLength: 1024));
        }
    }
}
