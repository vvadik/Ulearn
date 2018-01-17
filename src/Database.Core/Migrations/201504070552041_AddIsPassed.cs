using System.Data.Entity.Migrations;

namespace Database.Migrations
{
	public partial class AddIsPassed : DbMigration
	{
		public override void Up()
		{
			AddColumn("dbo.Visiters", "IsPassed", c => c.Boolean(nullable: false));

			Sql(@"update Visiters
				set IsPassed = 1
				from (
					select distinct UserId, SlideId
						from UserQuizs 
						where isDropped = 0
				) as t
				where Visiters.UserId = t.UserId and Visiters.SlideId = t.SlideId");

			Sql(@"update Visiters
					set IsPassed = 1
					from (
						select distinct UserId, SlideId 
							from UserSolutions 
							where IsRightAnswer = 1
					) as t
					where Visiters.UserId = t.UserId and Visiters.SlideId = t.SlideId");
		}

		public override void Down()
		{
			DropColumn("dbo.Visiters", "IsPassed");
		}
	}
}