namespace uLearn.Web.Migrations
{
	using System;
	using System.Data.Entity.Migrations;

	public partial class VisitResults : DbMigration
	{
		public override void Up()
		{
			AddColumn("dbo.Visiters", "Score", c => c.Int(nullable: false));
			AddColumn("dbo.Visiters", "AttemptsCount", c => c.Int(nullable: false));
			AddColumn("dbo.Visiters", "IsSkipped", c => c.Boolean(nullable: false));

			Sql(@"update Visiters
					set AttemptsCount = t.cnt
					from (select UserId, SlideId, count(distinct Timestamp) as cnt from UserQuizs group by UserId, SlideId) as t
					where Visiters.UserId = t.UserId and Visiters.SlideId = t.SlideId");

			Sql(@"update Visiters
				set Score = t.score
				from (
					select UserId, SlideId, count(distinct QuizId) as score
						from UserQuizs 
						where isDropped = 0 and IsRightQuizBlock = 1
						group by UserId, SlideId
				) as t
				where Visiters.UserId = t.UserId and Visiters.SlideId = t.SlideId");

			Sql(@"update Visiters
					set AttemptsCount = t.cnt, Score = 5 * t.IsRightAnswer
					from (select UserId, SlideId, count(1) as cnt, max(cast(IsRightAnswer as int)) as IsRightAnswer from UserSolutions group by UserId, SlideId) as t
					where Visiters.UserId = t.UserId and Visiters.SlideId = t.SlideId");
		}

		public override void Down()
		{
			DropColumn("dbo.Visiters", "IsSkipped");
			DropColumn("dbo.Visiters", "AttemptsCount");
			DropColumn("dbo.Visiters", "Score");
		}
	}
}