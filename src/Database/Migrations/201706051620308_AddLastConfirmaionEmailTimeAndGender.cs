namespace Database.Migrations
{
	using System.Data.Entity.Migrations;

	public partial class AddLastConfirmaionEmailTimeAndGender : DbMigration
	{
		public override void Up()
		{
			AddColumn("dbo.AspNetUsers", "LastConfirmationEmailTime", c => c.DateTime());
			AddColumn("dbo.AspNetUsers", "Gender", c => c.Short());
		}

		public override void Down()
		{
			DropColumn("dbo.AspNetUsers", "Gender");
			DropColumn("dbo.AspNetUsers", "LastConfirmationEmailTime");
		}
	}
}