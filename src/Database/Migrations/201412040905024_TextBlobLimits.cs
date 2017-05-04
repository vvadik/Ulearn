using System.Data.Entity.Migrations;

namespace Database.Migrations
{
	public partial class TextBlobLimits : DbMigration
	{
		public override void Up()
		{
			Sql("update dbo.TextBlobs set Text = Left(Text, 4000) where len(Text) > 4000");
			AlterColumn("dbo.TextBlobs", "Text", c => c.String(nullable: false, maxLength: 4000));
		}

		public override void Down()
		{
			AlterColumn("dbo.TextBlobs", "Text", c => c.String(nullable: false));
		}
	}
}