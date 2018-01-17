using System.Data.Entity.Migrations;

namespace Database.Migrations
{
	public partial class RemoveTextBlobLimits : DbMigration
	{
		public override void Up()
		{
			AlterColumn("dbo.TextBlobs", "Text", c => c.String());
		}

		public override void Down()
		{
			AlterColumn("dbo.TextBlobs", "Text", c => c.String(maxLength: 4000));
		}
	}
}