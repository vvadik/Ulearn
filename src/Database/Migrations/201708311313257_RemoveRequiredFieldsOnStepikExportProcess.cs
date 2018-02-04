namespace Database.Migrations
{
	using System.Data.Entity.Migrations;
    
    public partial class RemoveRequiredFieldsOnStepikExportProcess : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.StepikExportProcesses", "StepikCourseTitle", c => c.String(maxLength: 100));
            AlterColumn("dbo.StepikExportProcesses", "Log", c => c.String());
            AlterColumn("dbo.StepikExportProcesses", "FinishTime", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.StepikExportProcesses", "FinishTime", c => c.DateTime(nullable: false));
            AlterColumn("dbo.StepikExportProcesses", "Log", c => c.String(nullable: false));
            AlterColumn("dbo.StepikExportProcesses", "StepikCourseTitle", c => c.String(nullable: false, maxLength: 100));
        }
    }
}
