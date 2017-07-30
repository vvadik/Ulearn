using System.Data.Entity.Migrations;

namespace Database.Migrations
{
	public partial class RestorePassword : DbMigration
	{
		public override void Up()
		{
			CreateTable(
					"dbo.RestoreRequests",
					c => new
					{
						Id = c.String(nullable: false, maxLength: 128),
						UserId = c.String(nullable: false, maxLength: 128),
						Timestamp = c.DateTime(nullable: false),
					})
				.PrimaryKey(t => t.Id)
				.ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
				.Index(t => t.UserId);
		}

		public override void Down()
		{
			DropForeignKey("dbo.RestoreRequests", "UserId", "dbo.AspNetUsers");
			DropIndex("dbo.RestoreRequests", new[] { "UserId" });
			DropTable("dbo.RestoreRequests");
		}
	}
}