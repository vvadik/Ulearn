using System.Data.Entity.Migrations;

namespace Database.Migrations
{
	public partial class UserSolution_TimestampIndex : DbMigration
	{
		public override void Up()
		{
			CreateIndex("dbo.UserSolutions", "Timestamp", name: "IDX_UserSolution_Timestamp");
		}

		public override void Down()
		{
			DropIndex("dbo.UserSolutions", "IDX_UserSolution_Timestamp");
		}
	}
}