using System.Data.Entity.Migrations;

namespace Database.Migrations
{
	public partial class AddPreviewedCertificates : DbMigration
	{
		public override void Up()
		{
			AddColumn("dbo.Certificates", "IsPreview", c => c.Boolean(nullable: false));
		}

		public override void Down()
		{
			DropColumn("dbo.Certificates", "IsPreview");
		}
	}
}