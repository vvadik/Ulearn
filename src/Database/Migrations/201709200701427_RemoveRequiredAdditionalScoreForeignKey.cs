namespace Database.Migrations
{
	using System.Data.Entity.Migrations;
    
    public partial class RemoveRequiredAdditionalScoreForeignKey : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Notifications", "ScoreId", "dbo.AdditionalScores");
			/* Manually set ON DELETE SET NULL because EF can'not do it itself */
	        Sql("ALTER TABLE dbo.Notifications ADD CONSTRAINT [FK_dbo.Notifications_dbo.AdditionalScores_ScoreId] FOREIGN KEY (ScoreId) REFERENCES dbo.AdditionalScores (Id) ON UPDATE NO ACTION ON DELETE SET NULL");
		}
        
        public override void Down()
        {
            DropForeignKey("dbo.Notifications", "ScoreId", "dbo.AdditionalScores");
            AddForeignKey("dbo.Notifications", "ScoreId", "dbo.AdditionalScores", "Id", cascadeDelete: true);
        }
    }
}
