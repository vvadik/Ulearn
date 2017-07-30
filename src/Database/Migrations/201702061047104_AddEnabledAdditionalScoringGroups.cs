using System.Data.Entity.Migrations;

namespace Database.Migrations
{
	public partial class AddEnabledAdditionalScoringGroups : DbMigration
	{
		public override void Up()
		{
			DropIndex("dbo.AdditionalScores", "IDX_AdditionalScore_ByCourseUnitAndUser");
			CreateTable(
					"dbo.EnabledAdditionalScoringGroups",
					c => new
					{
						Id = c.Int(nullable: false, identity: true),
						GroupId = c.Int(nullable: false),
						ScoringGroupId = c.String(nullable: false),
					})
				.PrimaryKey(t => t.Id)
				.ForeignKey("dbo.Groups", t => t.GroupId, cascadeDelete: true)
				.Index(t => t.GroupId, name: "IDX_EnabledAdditionalScoringGroup_ByGroup");

			AddColumn("dbo.Groups", "CanUsersSeeGroupProgress", c => c.Boolean(nullable: false));
			AddColumn("dbo.Groups", "IsArchived", c => c.Boolean(nullable: false));
			AlterColumn("dbo.AdditionalScores", "ScoringGroupId", c => c.String(nullable: false, maxLength: 64));
			CreateIndex("dbo.AdditionalScores", new[] { "CourseId", "UnitId", "ScoringGroupId", "UserId" }, unique: true, name: "IDX_AdditionalScore_ByCourseUnitScoringGroupAndUser");
		}

		public override void Down()
		{
			DropForeignKey("dbo.EnabledAdditionalScoringGroups", "GroupId", "dbo.Groups");
			DropIndex("dbo.EnabledAdditionalScoringGroups", "IDX_EnabledAdditionalScoringGroup_ByGroup");
			DropIndex("dbo.AdditionalScores", "IDX_AdditionalScore_ByCourseUnitScoringGroupAndUser");
			AlterColumn("dbo.AdditionalScores", "ScoringGroupId", c => c.String());
			DropColumn("dbo.Groups", "IsArchived");
			DropColumn("dbo.Groups", "CanUsersSeeGroupProgress");
			DropTable("dbo.EnabledAdditionalScoringGroups");
			CreateIndex("dbo.AdditionalScores", new[] { "CourseId", "UnitId", "UserId" }, unique: true, name: "IDX_AdditionalScore_ByCourseUnitAndUser");
		}
	}
}