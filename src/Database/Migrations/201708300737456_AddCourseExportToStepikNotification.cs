namespace Database.Migrations
{
	using System.Data.Entity.Migrations;
    
    public partial class AddCourseExportToStepikNotification : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.StepikExportProcesses", "OwnerId", "dbo.AspNetUsers");
            AddColumn("dbo.Notifications", "ProcessId", c => c.Int());
            AddColumn("dbo.StepikExportProcesses", "StepikCourseTitle", c => c.String(nullable: false, maxLength: 100));
            AddColumn("dbo.StepikExportProcesses", "IsInitialExport", c => c.Boolean(nullable: false));
            AddColumn("dbo.StepikExportProcesses", "IsSuccess", c => c.Boolean(nullable: false));
            AddColumn("dbo.StepikExportProcesses", "Log", c => c.String(nullable: false));
            AddColumn("dbo.StepikExportProcesses", "FinishTime", c => c.DateTime(nullable: false));
            CreateIndex("dbo.Notifications", "ProcessId");
            AddForeignKey("dbo.Notifications", "ProcessId", "dbo.StepikExportProcesses", "Id");
            AddForeignKey("dbo.StepikExportProcesses", "OwnerId", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.StepikExportProcesses", "OwnerId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Notifications", "ProcessId", "dbo.StepikExportProcesses");
            DropIndex("dbo.Notifications", new[] { "ProcessId" });
            DropColumn("dbo.StepikExportProcesses", "FinishTime");
            DropColumn("dbo.StepikExportProcesses", "Log");
            DropColumn("dbo.StepikExportProcesses", "IsSuccess");
            DropColumn("dbo.StepikExportProcesses", "IsInitialExport");
            DropColumn("dbo.StepikExportProcesses", "StepikCourseTitle");
            DropColumn("dbo.Notifications", "ProcessId");
            AddForeignKey("dbo.StepikExportProcesses", "OwnerId", "dbo.AspNetUsers", "Id", cascadeDelete: true);
        }
    }
}
