using System.Data.Entity.Migrations;

namespace Database.Migrations
{
	public partial class ExecutionServiceName : DbMigration
	{
		public override void Up()
		{
			AddColumn("dbo.UserSolutions", "ExecutionServiceName", c => c.String(maxLength: 40));
		}

		public override void Down()
		{
			DropColumn("dbo.UserSolutions", "ExecutionServiceName");
		}
	}
}